using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Rinoleggio.Listings;
using Rinoleggio.Permissions;
using Rinoleggio.Users;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Authorization;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Emailing;
using Volo.Abp.Identity;
using Volo.Abp.Timing;

namespace Rinoleggio.Leads;

[Authorize]
public class LeadAppService : RinoleggioAppService, ILeadAppService
{
    private readonly IRepository<Lead, Guid> _leadRepository;
    private readonly IRepository<Listing, Guid> _listingRepository;
    private readonly IRepository<UserProfile, Guid> _userProfileRepository;
    private readonly IIdentityUserRepository _userRepository;
    private readonly IdentityUserManager _userManager;
    private readonly IClock _clock;
    private readonly IEmailSender _emailSender;
    private readonly ILogger<LeadAppService> _logger;

    public LeadAppService(
        IRepository<Lead, Guid> leadRepository,
        IRepository<Listing, Guid> listingRepository,
        IRepository<UserProfile, Guid> userProfileRepository,
        IIdentityUserRepository userRepository,
        IdentityUserManager userManager,
        IClock clock,
        IEmailSender emailSender,
        ILogger<LeadAppService> logger)
    {
        _leadRepository = leadRepository;
        _listingRepository = listingRepository;
        _userProfileRepository = userProfileRepository;
        _userRepository = userRepository;
        _userManager = userManager;
        _clock = clock;
        _emailSender = emailSender;
        _logger = logger;
    }

    [Authorize(RinoleggioPermissions.Leads.Create)]
    public async Task<LeadDto> CreateAsync(CreateLeadDto input)
    {
        if (!input.PrivacyConsent)
        {
            throw new Volo.Abp.BusinessException("Rinoleggio:PrivacyConsentRequired");
        }

        var today = DateOnly.FromDateTime(_clock.Now);
        var listingQuery = await _listingRepository.WithDetailsAsync(l => l.Contract);
        var listing = await AsyncExecuter.FirstAsync(listingQuery.Where(l => l.Id == input.ListingId));

        if (!listing.IsPublished ||
            listing.BusinessStatus != ListingBusinessStatus.Active ||
            listing.Contract.EndDate <= today)
        {
            throw new Volo.Abp.BusinessException(RinoleggioDomainErrorCodes.LeadListingNotEligible);
        }

        var buyerId = CurrentUser.Id!.Value;
        if (buyerId == listing.OwnerId)
        {
            throw new Volo.Abp.BusinessException(RinoleggioDomainErrorCodes.LeadBuyerCannotBeSeller);
        }

        var openStatuses = new[] { LeadStatus.New, LeadStatus.Contacted };
        if (await _leadRepository.AnyAsync(l =>
                l.BuyerId == buyerId &&
                l.ListingId == input.ListingId &&
                openStatuses.Contains(l.Status)))
        {
            throw new Volo.Abp.BusinessException(RinoleggioDomainErrorCodes.LeadDuplicateOpen);
        }

        var lead = new Lead
        {
            ListingId = input.ListingId,
            BuyerId = buyerId,
            SellerId = listing.OwnerId,
            Title = $"Richiesta subentro - {listing.Title}",
            Message = input.Message,
            Phone = input.Phone,
            PrivacyConsent = true,
            Status = LeadStatus.New
        };

        await _leadRepository.InsertAsync(lead, autoSave: true);
        await TryNotifyNewLeadAsync(listing, input.Message);
        return await MapLeadAsync(lead, LeadViewMode.Sent);
    }

    [Authorize(RinoleggioPermissions.Leads.ViewOwn)]
    public async Task<PagedResultDto<LeadDto>> GetSentAsync(GetLeadListInput input)
    {
        var userId = CurrentUser.Id!.Value;
        var queryable = await _leadRepository.WithDetailsAsync(l => l.Listing!, l => l.Listing!.Contract);
        var query = queryable.Where(l => l.BuyerId == userId).OrderByDescending(l => l.CreationTime);
        return await ToPagedAsync(query, input, LeadViewMode.Sent);
    }

    [Authorize(RinoleggioPermissions.Leads.ViewOwn)]
    public async Task<PagedResultDto<LeadDto>> GetReceivedAsync(GetLeadListInput input)
    {
        var userId = CurrentUser.Id!.Value;
        var queryable = await _leadRepository.WithDetailsAsync(l => l.Listing!, l => l.Listing!.Contract);
        var query = queryable.Where(l => l.SellerId == userId);
        if (input.ListingId.HasValue)
        {
            query = query.Where(l => l.ListingId == input.ListingId.Value);
        }

        query = query.OrderByDescending(l => l.CreationTime);
        return await ToPagedAsync(query, input, LeadViewMode.Received);
    }

    [Authorize(RinoleggioPermissions.Leads.ViewAll)]
    public async Task<PagedResultDto<LeadDto>> GetBackOfficeListAsync(GetLeadListInput input)
    {
        var queryable = await _leadRepository.WithDetailsAsync(l => l.Listing!, l => l.Listing!.Contract);
        var query = queryable.OrderByDescending(l => l.CreationTime);
        return await ToPagedAsync(query, input, LeadViewMode.BackOffice);
    }

    [Authorize(RinoleggioPermissions.Leads.Manage)]
    public async Task<LeadDto> UpdateStatusAsync(Guid id, UpdateLeadStatusDto input)
    {
        var lead = await _leadRepository.GetAsync(id);
        var previousStatus = lead.Status;
        lead.Status = input.Status;
        lead.BackofficeNotes = input.BackofficeNotes;
        await _leadRepository.UpdateAsync(lead, autoSave: true);

        if (previousStatus != input.Status &&
            input.Status is LeadStatus.Contacted or LeadStatus.Closed)
        {
            await TryNotifyBuyerStatusChangeAsync(lead, input.Status);
        }

        return await MapLeadAsync(lead, LeadViewMode.BackOffice);
    }

    private async Task<PagedResultDto<LeadDto>> ToPagedAsync(
        IQueryable<Lead> query,
        GetLeadListInput input,
        LeadViewMode viewMode)
    {
        var totalCount = await AsyncExecuter.CountAsync(query);
        var items = await AsyncExecuter.ToListAsync(query.Skip(input.SkipCount).Take(input.MaxResultCount));
        var dtos = new List<LeadDto>();
        foreach (var lead in items)
        {
            dtos.Add(await MapLeadAsync(lead, viewMode));
        }

        return new PagedResultDto<LeadDto>(totalCount, dtos);
    }

    private enum LeadViewMode
    {
        Sent,
        Received,
        BackOffice
    }

    private async Task TryNotifyNewLeadAsync(Listing listing, string message)
    {
        try
        {
            var subject = $"Nuova richiesta subentro - {listing.Title}";
            var body =
                $"<p>Nuova richiesta su <strong>{System.Net.WebUtility.HtmlEncode(listing.Title)}</strong>.</p>" +
                $"<p><strong>Messaggio:</strong></p><p>{System.Net.WebUtility.HtmlEncode(message)}</p>" +
                "<p>Accedi al backoffice lead per gestirla.</p>";

            await QueueToBackOfficeAsync(subject, body);

            var seller = await _userRepository.GetAsync(listing.OwnerId);
            if (!string.IsNullOrWhiteSpace(seller.Email))
            {
                var sellerBody =
                    $"<p>Hai ricevuto una nuova richiesta di subentro su <strong>{System.Net.WebUtility.HtmlEncode(listing.Title)}</strong>.</p>" +
                    "<p>Il backoffice coordina il contatto. Accedi a Rinoleggio → Richieste ricevute per vedere il messaggio.</p>";
                await _emailSender.QueueAsync(seller.Email, subject, sellerBody);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not queue new lead notification emails for listing {ListingId}", listing.Id);
        }
    }

    private async Task TryNotifyBuyerStatusChangeAsync(Lead lead, LeadStatus newStatus)
    {
        try
        {
            var buyer = await _userRepository.GetAsync(lead.BuyerId);
            if (string.IsNullOrWhiteSpace(buyer.Email))
            {
                return;
            }

            var listing = await _listingRepository.FindAsync(lead.ListingId);
            var statusLabel = newStatus switch
            {
                LeadStatus.Contacted => "Presa in carico",
                LeadStatus.Closed => "Chiusa",
                _ => newStatus.ToString()
            };

            var subject = $"Aggiornamento richiesta - {listing?.Title ?? "Annuncio"}";
            var body =
                $"<p>Lo stato della tua richiesta su <strong>{System.Net.WebUtility.HtmlEncode(listing?.Title ?? "annuncio")}</strong> è ora: <strong>{statusLabel}</strong>.</p>";
            await _emailSender.QueueAsync(buyer.Email, subject, body);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not queue buyer status email for lead {LeadId}", lead.Id);
        }
    }

    private async Task QueueToBackOfficeAsync(string subject, string body)
    {
        var backOfficeUsers = await _userManager.GetUsersInRoleAsync(RinoleggioRoles.BackOffice);
        foreach (var user in backOfficeUsers.Where(u => !string.IsNullOrWhiteSpace(u.Email)))
        {
            await _emailSender.QueueAsync(user.Email!, subject, body);
        }
    }

    private async Task<LeadDto> MapLeadAsync(Lead lead, LeadViewMode viewMode)
    {
        var listing = lead.Listing ?? await _listingRepository.FindAsync(lead.ListingId);
        var buyerProfile = await _userProfileRepository.FindAsync(lead.BuyerId);
        var sellerProfile = await _userProfileRepository.FindAsync(lead.SellerId);
        IdentityUser? buyerUser = null;
        IdentityUser? sellerUser = null;

        if (viewMode == LeadViewMode.BackOffice)
        {
            buyerUser = await _userRepository.GetAsync(lead.BuyerId);
            sellerUser = await _userRepository.GetAsync(lead.SellerId);
        }
        else if (viewMode == LeadViewMode.Received)
        {
            buyerProfile ??= await _userProfileRepository.FindAsync(lead.BuyerId);
        }

        return new LeadDto
        {
            Id = lead.Id,
            ListingId = lead.ListingId,
            ListingTitle = listing?.Title,
            ListingBusinessStatus = listing?.BusinessStatus ?? ListingBusinessStatus.Active,
            BuyerId = lead.BuyerId,
            BuyerUserName = buyerUser?.UserName,
            BuyerVisibleName = buyerProfile?.FullName,
            BuyerEmail = viewMode == LeadViewMode.BackOffice ? buyerUser?.Email : null,
            SellerId = lead.SellerId,
            SellerVisibleName = sellerProfile?.FullName,
            SellerEmail = viewMode == LeadViewMode.BackOffice ? sellerUser?.Email : null,
            Title = lead.Title,
            Message = lead.Message,
            Phone = viewMode == LeadViewMode.BackOffice ? lead.Phone : null,
            Status = lead.Status,
            BackofficeNotes = viewMode == LeadViewMode.BackOffice ? lead.BackofficeNotes : null,
            CreationTime = lead.CreationTime
        };
    }
}
