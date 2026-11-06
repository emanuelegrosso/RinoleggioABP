using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Rinoleggio.Permissions;
using Volo.Abp;
using Volo.Abp.Authorization;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Emailing;
using Volo.Abp.Identity;
using Volo.Abp.Users;
using Volo.Abp.Application.Dtos;

namespace Rinoleggio.Listings;

[AllowAnonymous]
public class ListingAppService : ApplicationService, IListingAppService
{
    private readonly IRepository<Listing, Guid> _listings;
    private readonly IRepository<Lead, Guid> _leads;
    private readonly IRepository<Favorite, Guid> _favorites;
    private readonly IRepository<CatalogEntry, Guid> _catalogEntries;
    private readonly IRepository<IdentityUser, Guid> _users;
    private readonly IdentityUserManager _userManager;
    private readonly IEmailSender _emailSender;
    private readonly ILogger<ListingAppService> _logger;

    public ListingAppService(IRepository<Listing, Guid> listings, IRepository<Lead, Guid> leads, IRepository<Favorite, Guid> favorites,
        IRepository<CatalogEntry, Guid> catalogEntries,
        IRepository<IdentityUser, Guid> users, IdentityUserManager userManager, IEmailSender emailSender,
        ILogger<ListingAppService> logger)
    {
        _listings = listings;
        _leads = leads;
        _favorites = favorites;
        _catalogEntries = catalogEntries;
        _users = users;
        _userManager = userManager;
        _emailSender = emailSender;
        _logger = logger;
    }

    public async Task<PagedResultDto<ListingDto>> SearchAsync(ListingSearchInput input)
    {
        var query = await _listings.GetQueryableAsync();
        query = query.Where(x => x.IsPublished && x.Status != ListingStatus.Deleted && x.Status != ListingStatus.Suspended && x.Status != ListingStatus.Closed && x.Status != ListingStatus.Withdrawn && x.EndDate > Clock.Now);
        if (!input.Query.IsNullOrWhiteSpace())
        {
            var term = input.Query.Trim();
            query = query.Where(x => x.Title.Contains(term) || x.Brand.Contains(term) || x.Model.Contains(term) || (x.Version != null && x.Version.Contains(term)));
        }
        if (!input.Province.IsNullOrWhiteSpace()) query = query.Where(x => x.Province == input.Province);
        if (!input.RentalCompany.IsNullOrWhiteSpace()) query = query.Where(x => x.RentalCompany == input.RentalCompany);
        if (!input.Brand.IsNullOrWhiteSpace()) query = query.Where(x => x.Brand == input.Brand);
        if (!input.Model.IsNullOrWhiteSpace()) query = query.Where(x => x.Model.Contains(input.Model));
        if (input.FuelType.HasValue) query = query.Where(x => x.FuelType == input.FuelType);
        if (input.Transmission.HasValue) query = query.Where(x => x.Transmission == input.Transmission);
        if (input.YearFrom.HasValue) query = query.Where(x => x.RegistrationYear >= input.YearFrom);
        if (input.YearTo.HasValue) query = query.Where(x => x.RegistrationYear <= input.YearTo);
        if (input.MonthlyFeeFrom.HasValue) query = query.Where(x => x.MonthlyFee >= input.MonthlyFeeFrom);
        if (input.MonthlyFeeTo.HasValue) query = query.Where(x => x.MonthlyFee <= input.MonthlyFeeTo);
        if (input.RemainingKmFrom.HasValue) query = query.Where(x => x.RemainingKm >= input.RemainingKmFrom);
        if (input.RemainingKmTo.HasValue) query = query.Where(x => x.RemainingKm <= input.RemainingKmTo);
        if (input.EndDateFrom.HasValue) query = query.Where(x => x.EndDate >= input.EndDateFrom);
        if (input.EndDateTo.HasValue) query = query.Where(x => x.EndDate <= input.EndDateTo);
        if (input.VatIncluded.HasValue) query = query.Where(x => x.VatIncluded == input.VatIncluded);
        if (input.ContributionType.HasValue) query = query.Where(x => x.ContributionType == input.ContributionType);

        var count = await AsyncExecuter.CountAsync(query);
        query = ApplyPublicSort(query, input.Sorting);
        var rows = await AsyncExecuter.ToListAsync(query.Skip(input.SkipCount).Take(Math.Clamp(input.MaxResultCount, 1, 100)));
        return new PagedResultDto<ListingDto>(count, rows.Select(ToDto).ToList());
    }

    public async Task<ListingDto> GetPublicAsync(Guid id)
    {
        var listing = await _listings.GetAsync(id);
        if (!listing.IsPublished || listing.Status is ListingStatus.Deleted or ListingStatus.Suspended or ListingStatus.Closed or ListingStatus.Withdrawn || listing.EndDate <= Clock.Now)
            throw new BusinessException("Rinoleggio:ListingNotAvailable");
        return ToDto(listing);
    }

    [Authorize]
    public async Task<ListingDto> GetAsync(Guid id)
    {
        var listing = await _listings.GetAsync(id);
        await CheckListingAccessAsync(listing);
        return ToDto(listing);
    }

    [Authorize(RinoleggioPermissions.Dealer.ViewOwnListings)]
    public async Task<PagedResultDto<ListingDto>> GetMineAsync(PagedAndSortedResultRequestDto input)
    {
        var query = (await _listings.GetQueryableAsync()).Where(x => x.OwnerId == CurrentUser.GetId() && x.Status != ListingStatus.Deleted);
        var count = await AsyncExecuter.CountAsync(query);
        query = ApplySort(query, input.Sorting);
        var rows = await AsyncExecuter.ToListAsync(query.Skip(input.SkipCount).Take(Math.Clamp(input.MaxResultCount, 1, 100)));
        return new PagedResultDto<ListingDto>(count, rows.Select(ToDto).ToList());
    }

    [Authorize]
    public async Task<ListingDto> CreateAsync(CreateUpdateListingDto input)
    {
        if (!await AuthorizationService.IsGrantedAsync(RinoleggioPermissions.Dealer.CreateListing) &&
            !await AuthorizationService.IsGrantedAsync(RinoleggioPermissions.BackOffice.ManageListings))
        {
            throw new AbpAuthorizationException();
        }

        ValidateListing(input);
        var listing = new Listing(GuidGenerator.Create(), CurrentUser.GetId(), input.Title);
        ApplyInput(listing, input);
        listing.Status = input.Publish ? ListingStatus.Active : ListingStatus.Draft;
        listing.IsPublished = input.Publish;
        return ToDto(await _listings.InsertAsync(listing, autoSave: true));
    }

    [Authorize]
    public async Task<ListingDto> UpdateAsync(Guid id, CreateUpdateListingDto input)
    {
        ValidateListing(input);
        var listing = await _listings.GetAsync(id);
        await CheckListingEditAccessAsync(listing);
        if (listing.Status == ListingStatus.Deleted) throw new BusinessException("Rinoleggio:ListingLocked");
        ApplyInput(listing, input);
        if (input.Publish && listing.Status == ListingStatus.Draft)
        {
            listing.Status = ListingStatus.Active;
            listing.IsPublished = true;
        }
        else if (input.Publish && listing.Status == ListingStatus.Withdrawn)
        {
            if (listing.StatusChangedByBackOffice || listing.EndDate <= Clock.Now)
            {
                throw new BusinessException("Rinoleggio:CannotRepublish");
            }

            listing.Status = ListingStatus.Active;
            listing.IsPublished = true;
            listing.StatusReason = null;
            listing.StatusChangedByBackOffice = false;
        }
        else if (input.Publish && listing.Status == ListingStatus.Active)
        {
            listing.IsPublished = true;
        }
        else if (!input.Publish && listing.Status is ListingStatus.Draft or ListingStatus.Active or ListingStatus.Withdrawn)
        {
            listing.IsPublished = false;
        }
        await _listings.UpdateAsync(listing, autoSave: true);
        return ToDto(listing);
    }

    [Authorize(RinoleggioPermissions.Dealer.ManageOwnListings)]
    public async Task SetMyStatusAsync(Guid id, ListingStatusDto input)
    {
        var listing = await _listings.GetAsync(id);
        if (listing.OwnerId != CurrentUser.GetId()) throw new AbpAuthorizationException();
        if (input.Status is not (ListingStatus.Reserved or ListingStatus.Closed or ListingStatus.Withdrawn or ListingStatus.Active or ListingStatus.Deleted))
            throw new BusinessException("Rinoleggio:InvalidListingStatus");
        if (listing.Status is ListingStatus.Suspended or ListingStatus.Deleted or ListingStatus.Closed)
            throw new BusinessException("Rinoleggio:ListingLocked");
        if (input.Status == ListingStatus.Active && listing.Status == ListingStatus.Withdrawn &&
            (listing.StatusChangedByBackOffice || listing.EndDate <= Clock.Now))
            throw new BusinessException("Rinoleggio:CannotRepublish");
        if (input.Status == ListingStatus.Active && listing.Status is not (ListingStatus.Active or ListingStatus.Reserved or ListingStatus.Withdrawn))
            throw new BusinessException("Rinoleggio:CannotRepublish");
        if (input.Status == ListingStatus.Deleted) ValidateReason(input.Reason);
        if (input.Status == ListingStatus.Deleted)
        {
            listing.StatusBeforeDeletion = listing.Status;
            listing.WasPublishedBeforeDeletion = listing.IsPublished;
            listing.StatusChangedByBackOfficeBeforeDeletion = listing.StatusChangedByBackOffice;
        }
        listing.Status = input.Status;
        listing.IsPublished = input.Status != ListingStatus.Deleted && input.Status != ListingStatus.Withdrawn;
        listing.StatusChangedByBackOffice = false;
        listing.StatusReason = input.Status == ListingStatus.Deleted ? input.Reason!.Trim() : null;
        await _listings.UpdateAsync(listing, autoSave: true);
    }

    [Authorize(RinoleggioPermissions.BackOffice.Listings)]
    public async Task<PagedResultDto<ListingDto>> GetBackOfficeListAsync(ListingSearchInput input)
    {
        var query = (await _listings.GetQueryableAsync()).Where(x => x.Status != ListingStatus.Deleted);
        if (!input.Query.IsNullOrWhiteSpace())
        {
            var term = input.Query.Trim();
            query = query.Where(x => x.Title.Contains(term) || x.Brand.Contains(term) || x.Model.Contains(term) || x.RentalCompany.Contains(term));
        }
        if (input.Status.HasValue) query = query.Where(x => x.Status == input.Status.Value);
        var count = await AsyncExecuter.CountAsync(query);
        query = ApplySort(query, input.Sorting);
        var rows = await AsyncExecuter.ToListAsync(query.Skip(input.SkipCount).Take(Math.Clamp(input.MaxResultCount, 1, 100)));
        return new PagedResultDto<ListingDto>(count, rows.Select(ToDto).ToList());
    }

    [Authorize(RinoleggioPermissions.BackOffice.ManageListings)]
    public async Task SetBackOfficeStatusAsync(Guid id, ListingStatusDto input)
    {
        var listing = await _listings.GetAsync(id);
        if (input.Status is not (ListingStatus.Active or ListingStatus.Reserved or ListingStatus.Closed or ListingStatus.Suspended or ListingStatus.Withdrawn or ListingStatus.Deleted))
            throw new BusinessException("Rinoleggio:InvalidListingStatus");
        if (listing.Status == ListingStatus.Deleted && input.Status != ListingStatus.Deleted)
            throw new BusinessException("Rinoleggio:DeletedListingRequiresAdministrator");
        if (input.Status == ListingStatus.Active && listing.EndDate <= Clock.Now)
            throw new BusinessException("Rinoleggio:CannotRepublish");
        if (input.Status is ListingStatus.Suspended or ListingStatus.Withdrawn or ListingStatus.Deleted) ValidateReason(input.Reason);
        if (input.Status == ListingStatus.Deleted && listing.Status != ListingStatus.Deleted)
        {
            listing.StatusBeforeDeletion = listing.Status;
            listing.WasPublishedBeforeDeletion = listing.IsPublished;
            listing.StatusChangedByBackOfficeBeforeDeletion = listing.StatusChangedByBackOffice;
        }
        listing.Status = input.Status;
        listing.IsPublished = input.Status is ListingStatus.Active or ListingStatus.Reserved or ListingStatus.Closed;
        listing.StatusChangedByBackOffice = input.Status is ListingStatus.Suspended or ListingStatus.Withdrawn;
        listing.StatusReason = input.Status is ListingStatus.Suspended or ListingStatus.Withdrawn or ListingStatus.Deleted ? input.Reason!.Trim() : null;
        await _listings.UpdateAsync(listing, autoSave: true);
    }

    [Authorize(RinoleggioPermissions.Administrator.RestoreListings)]
    public async Task<PagedResultDto<ListingDto>> GetDeletedListingsAsync(PagedAndSortedResultRequestDto input)
    {
        var query = (await _listings.GetQueryableAsync()).Where(x => x.Status == ListingStatus.Deleted);
        var count = await AsyncExecuter.CountAsync(query);
        var rows = await AsyncExecuter.ToListAsync(query.OrderByDescending(x => x.LastModificationTime).ThenBy(x => x.Id)
            .Skip(input.SkipCount).Take(Math.Clamp(input.MaxResultCount, 1, 100)));
        return new PagedResultDto<ListingDto>(count, rows.Select(ToDto).ToList());
    }

    [Authorize(RinoleggioPermissions.Administrator.RestoreListings)]
    public async Task RestoreDeletedListingAsync(Guid listingId)
    {
        var listing = await _listings.GetAsync(listingId);
        if (listing.Status != ListingStatus.Deleted) throw new BusinessException("Rinoleggio:ListingNotDeleted");
        var restoredStatus = listing.StatusBeforeDeletion is null or ListingStatus.Deleted
            ? ListingStatus.Draft
            : listing.StatusBeforeDeletion.Value;
        if (listing.EndDate <= Clock.Now)
        {
            listing.Status = ListingStatus.Draft;
            listing.IsPublished = false;
        }
        else
        {
            listing.Status = restoredStatus;
            listing.IsPublished = listing.WasPublishedBeforeDeletion == true;
        }
        listing.StatusReason = null;
        listing.StatusChangedByBackOffice = listing.EndDate > Clock.Now && listing.StatusChangedByBackOfficeBeforeDeletion == true;
        listing.StatusBeforeDeletion = null;
        listing.WasPublishedBeforeDeletion = null;
        listing.StatusChangedByBackOfficeBeforeDeletion = null;
        await _listings.UpdateAsync(listing, autoSave: true);
    }

    [Authorize(RinoleggioPermissions.Customer.CreateLead)]
    public async Task<CustomerLeadDto> CreateLeadAsync(Guid listingId, CreateLeadDto input)
    {
        var listing = await _listings.GetAsync(listingId);
        if (listing.OwnerId == CurrentUser.GetId()) throw new BusinessException("Rinoleggio:CannotRequestOwnListing");
        if (!listing.IsPublished || listing.Status != ListingStatus.Active || listing.EndDate <= Clock.Now)
            throw new BusinessException("Rinoleggio:ListingNotAvailable");
        if (!input.PrivacyConsent) throw new BusinessException("Rinoleggio:PrivacyConsentRequired");
        var existing = await _leads.GetQueryableAsync();
        if (await AsyncExecuter.AnyAsync(existing.Where(x => x.ListingId == listingId && x.BuyerId == CurrentUser.GetId() && (x.Status == LeadStatus.New || x.Status == LeadStatus.Contacted))))
            throw new BusinessException("Rinoleggio:DuplicateOpenLead");
        var lead = new Lead(GuidGenerator.Create(), listingId, CurrentUser.GetId(), listing.OwnerId, input.Message.Trim())
        {
            BuyerPhone = input.BuyerPhone?.Trim(),
            PrivacyConsent = true
        };
        await _leads.InsertAsync(lead, autoSave: true);
        await NotifyBackOfficeAsync(listing, lead);
        var dealer = await _users.FindAsync(listing.OwnerId);
        return new CustomerLeadDto
        {
            Id = lead.Id,
            ListingId = lead.ListingId,
            ListingTitle = listing.Title,
            DealerDisplayName = GetDisplayName(dealer),
            Message = lead.Message,
            Status = lead.Status,
            CreationTime = lead.CreationTime
        };
    }

    [Authorize(RinoleggioPermissions.Customer.ViewOwnLeads)]
    public async Task<PagedResultDto<CustomerLeadDto>> GetMyLeadsAsync(PagedAndSortedResultRequestDto input)
    {
        var query = (await _leads.GetQueryableAsync()).Where(x => x.BuyerId == CurrentUser.GetId());
        var count = await AsyncExecuter.CountAsync(query);
        var rows = await AsyncExecuter.ToListAsync(query.OrderByDescending(x => x.CreationTime).ThenBy(x => x.Id)
            .Skip(input.SkipCount).Take(Math.Clamp(input.MaxResultCount, 1, 100)));
        var listingIds = rows.Select(x => x.ListingId).Distinct().ToList();
        var dealerIds = rows.Select(x => x.SellerId).Distinct().ToList();
        var listings = await _listings.GetListAsync(x => listingIds.Contains(x.Id));
        var dealers = await _users.GetListAsync(x => dealerIds.Contains(x.Id));
        var listingsById = listings.ToDictionary(x => x.Id);
        var dealerNames = dealers.ToDictionary(x => x.Id, x => GetDisplayName(x, "Dealer"));
        var result = rows.Select(x =>
        {
            listingsById.TryGetValue(x.ListingId, out var listing);
            return new CustomerLeadDto
            {
                Id = x.Id,
                ListingId = x.ListingId,
                ListingTitle = listing?.Title ?? "Annuncio non più disponibile",
                ListingAvailable = listing != null && listing.IsPublished &&
                    listing.Status is not (ListingStatus.Deleted or ListingStatus.Suspended or ListingStatus.Closed or ListingStatus.Withdrawn) &&
                    listing.EndDate > Clock.Now,
                DealerDisplayName = dealerNames.GetValueOrDefault(x.SellerId) ?? "Dealer",
                Message = x.Message,
                Status = x.Status,
                CreationTime = x.CreationTime
            };
        }).ToList();
        return new PagedResultDto<CustomerLeadDto>(count, result);
    }

    [Authorize(RinoleggioPermissions.Dealer.ViewReceivedLeads)]
    public async Task<PagedResultDto<DealerLeadDto>> GetReceivedLeadsAsync(PagedAndSortedResultRequestDto input)
    {
        var query = (await _leads.GetQueryableAsync()).Where(x => x.SellerId == CurrentUser.GetId());
        var count = await AsyncExecuter.CountAsync(query);
        var rows = await AsyncExecuter.ToListAsync(query.OrderByDescending(x => x.CreationTime).ThenBy(x => x.Id)
            .Skip(input.SkipCount).Take(Math.Clamp(input.MaxResultCount, 1, 100)));
        var listingIds = rows.Select(x => x.ListingId).Distinct().ToList();
        var customerIds = rows.Select(x => x.BuyerId).Distinct().ToList();
        var listings = await _listings.GetListAsync(x => listingIds.Contains(x.Id));
        var customers = await _users.GetListAsync(x => customerIds.Contains(x.Id));
        var titles = listings.ToDictionary(x => x.Id, x => x.Title);
        var listingsById = listings.ToDictionary(x => x.Id);
        var customerNames = customers.ToDictionary(x => x.Id, x => GetDisplayName(x, "Cliente"));
        var result = rows.Select(x => new DealerLeadDto
        {
            Id = x.Id,
            ListingId = x.ListingId,
            ListingTitle = titles.GetValueOrDefault(x.ListingId) ?? "Annuncio non più disponibile",
            CurrentListingStatus = listingsById.TryGetValue(x.ListingId, out var listing) ? listing.Status : null,
            CustomerDisplayName = customerNames.GetValueOrDefault(x.BuyerId) ?? "Cliente",
            Message = x.Message,
            Status = x.Status,
            CreationTime = x.CreationTime
        }).ToList();
        return new PagedResultDto<DealerLeadDto>(count, result);
    }

    [Authorize(RinoleggioPermissions.BackOffice.Leads)]
    public async Task<PagedResultDto<LeadDto>> GetBackOfficeLeadsAsync(BackOfficeLeadSearchInput input)
    {
        var query = await _leads.GetQueryableAsync();
        if (input.Status.HasValue) query = query.Where(x => x.Status == input.Status.Value);
        var count = await AsyncExecuter.CountAsync(query);
        query = query.OrderByDescending(x => x.CreationTime).ThenBy(x => x.Id).Skip(input.SkipCount).Take(Math.Clamp(input.MaxResultCount, 1, 100));
        var leads = await AsyncExecuter.ToListAsync(query);
        var listingIds = leads.Select(x => x.ListingId).Distinct().ToList();
        var userIds = leads.SelectMany(x => new[] { x.BuyerId, x.SellerId }).Distinct().ToList();
        var listings = await _listings.GetListAsync(x => listingIds.Contains(x.Id));
        var users = await _users.GetListAsync(x => userIds.Contains(x.Id));
        var titles = listings.ToDictionary(x => x.Id, x => x.Title);
        var listingsById = listings.ToDictionary(x => x.Id);
        var usersById = users.ToDictionary(x => x.Id);
        var result = new List<LeadDto>();
        foreach (var lead in leads)
        {
            usersById.TryGetValue(lead.BuyerId, out var buyer);
            usersById.TryGetValue(lead.SellerId, out var seller);
            var dto = ToBackOfficeDto(lead, titles.GetValueOrDefault(lead.ListingId) ?? "Annuncio non disponibile", buyer, seller);
            if (listingsById.TryGetValue(lead.ListingId, out var listing)) dto.CurrentListingStatus = listing.Status;
            result.Add(dto);
        }
        return new PagedResultDto<LeadDto>(count, result);
    }

    [Authorize(RinoleggioPermissions.BackOffice.ManageLeads)]
    public async Task UpdateLeadAsync(Guid id, UpdateLeadDto input)
    {
        var lead = await _leads.GetAsync(id);
        if (input.Status is not (LeadStatus.Contacted or LeadStatus.Closed)) throw new BusinessException("Rinoleggio:InvalidLeadStatus");
        if (!Enum.IsDefined(typeof(LeadOutcome), input.Outcome)) throw new BusinessException("Rinoleggio:InvalidLeadOutcome");
        if (input.Status == LeadStatus.Closed && input.Outcome == LeadOutcome.None)
            throw new BusinessException("Rinoleggio:LeadOutcomeRequired");
        if (input.ListingStatus.HasValue && input.ListingStatus.Value is not (ListingStatus.Active or ListingStatus.Reserved or ListingStatus.Closed))
            throw new BusinessException("Rinoleggio:InvalidListingStatus");
        var changed = lead.Status != input.Status;
        lead.Status = input.Status;
        lead.Outcome = input.Outcome;
        lead.OutcomeNotes = input.OutcomeNotes?.Trim();
        lead.InternalNotes = input.InternalNotes?.Trim();
        await _leads.UpdateAsync(lead, autoSave: true);
        if (input.ListingStatus.HasValue)
        {
            var listing = await _listings.GetAsync(lead.ListingId);
            if (listing.Status == ListingStatus.Deleted) throw new BusinessException("Rinoleggio:DeletedListingRequiresAdministrator");
            if (input.ListingStatus == ListingStatus.Active && listing.EndDate <= Clock.Now)
                throw new BusinessException("Rinoleggio:CannotRepublish");
            listing.Status = input.ListingStatus.Value;
            listing.IsPublished = input.ListingStatus is ListingStatus.Active or ListingStatus.Reserved or ListingStatus.Closed;
            listing.StatusChangedByBackOffice = false;
            listing.StatusReason = null;
            await _listings.UpdateAsync(listing, autoSave: true);
        }
        if (changed)
        {
            var buyer = await _users.FindAsync(lead.BuyerId);
            if (buyer != null && !buyer.Email.IsNullOrWhiteSpace())
            {
                var listing = await _listings.FindAsync(lead.ListingId);
                await SendEmailSafelyAsync(buyer.Email!, "Aggiornamento richiesta Rinoleggio", $"La tua richiesta per '{listing?.Title ?? "annuncio"}' è ora: {input.Status}.");
            }
        }
    }

    public async Task<IReadOnlyList<string>> GetLookupValuesAsync(string kind)
    {
        var rows = await _listings.GetQueryableAsync();
        var entryType = kind?.ToLowerInvariant() switch
        {
            "province" => CatalogEntryType.Province,
            "brand" => CatalogEntryType.Brand,
            "rentalcompany" => CatalogEntryType.RentalCompany,
            _ => (CatalogEntryType?)null
        };
        if (!entryType.HasValue) throw new BusinessException("Rinoleggio:UnknownLookup");
        var catalogQuery = await _catalogEntries.GetQueryableAsync();
        var catalogValues = await AsyncExecuter.ToListAsync(catalogQuery.Where(x => x.Type == entryType && x.IsActive).Select(x => x.Name));
        if (await AsyncExecuter.AnyAsync(catalogQuery.Where(x => x.Type == entryType)))
            return catalogValues.OrderBy(x => x).ToList();
        var listingValues = entryType.Value switch
        {
            CatalogEntryType.Province => await AsyncExecuter.ToListAsync(rows.Where(x => x.IsPublished).Select(x => x.Province).Distinct()),
            CatalogEntryType.Brand => await AsyncExecuter.ToListAsync(rows.Where(x => x.IsPublished).Select(x => x.Brand).Distinct()),
            _ => await AsyncExecuter.ToListAsync(rows.Where(x => x.IsPublished).Select(x => x.RentalCompany).Distinct())
        };
        return catalogValues.Concat(listingValues).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(x => x).ToList();
    }

    [Authorize(RinoleggioPermissions.Customer.ManageFavorites)]
    public async Task<PagedResultDto<FavoriteListingDto>> GetFavoritesAsync(PagedAndSortedResultRequestDto input)
    {
        var userId = CurrentUser.GetId();
        var query = (await _favorites.GetQueryableAsync()).Where(x => x.UserId == userId);
        var count = await AsyncExecuter.CountAsync(query);
        var rows = await AsyncExecuter.ToListAsync(query.OrderByDescending(x => x.CreationTime).ThenBy(x => x.Id)
            .Skip(Math.Max(0, input.SkipCount)).Take(Math.Clamp(input.MaxResultCount, 1, 100)));
        var ids = rows.Select(x => x.ListingId).Distinct().ToList();
        var listings = ids.Count == 0 ? new List<Listing>() : await _listings.GetListAsync(x => ids.Contains(x.Id));
        var byId = listings.ToDictionary(x => x.Id);
        var result = rows.Select(favorite =>
        {
            byId.TryGetValue(favorite.ListingId, out var listing);
            return new FavoriteListingDto
            {
                Id = favorite.Id,
                ListingId = favorite.ListingId,
                Title = listing?.Title ?? "Annuncio non più disponibile",
                ImageUrl = listing?.ImageUrls?.Split(';', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault(),
                Province = listing?.Province ?? string.Empty,
                Brand = listing?.Brand ?? string.Empty,
                Model = listing?.Model ?? string.Empty,
                RegistrationYear = listing?.RegistrationYear ?? 0,
                MonthlyFee = listing?.MonthlyFee ?? 0,
                EndDate = listing?.EndDate ?? favorite.CreationTime,
                IsAvailable = listing != null && listing.IsPublished && listing.Status == ListingStatus.Active && listing.EndDate > Clock.Now
            };
        }).ToList();
        return new PagedResultDto<FavoriteListingDto>(count, result);
    }

    [Authorize(RinoleggioPermissions.Customer.ManageFavorites)]
    public async Task<FavoriteListingDto> AddFavoriteAsync(Guid listingId)
    {
        var listing = await _listings.GetAsync(listingId);
        if (!listing.IsPublished || listing.Status != ListingStatus.Active || listing.EndDate <= Clock.Now)
            throw new BusinessException("Rinoleggio:ListingNotAvailable");
        var userId = CurrentUser.GetId();
        var query = await _favorites.GetQueryableAsync();
        var existing = await AsyncExecuter.FirstOrDefaultAsync(query.Where(x => x.UserId == userId && x.ListingId == listingId));
        var favoriteId = existing?.Id ?? GuidGenerator.Create();
        if (existing == null)
        {
            await _favorites.InsertAsync(new Favorite(favoriteId, userId, listingId), autoSave: true);
        }
        return new FavoriteListingDto
        {
            Id = favoriteId,
            ListingId = listing.Id,
            Title = listing.Title,
            ImageUrl = listing.ImageUrls?.Split(';', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault(),
            Province = listing.Province,
            Brand = listing.Brand,
            Model = listing.Model,
            RegistrationYear = listing.RegistrationYear,
            MonthlyFee = listing.MonthlyFee,
            EndDate = listing.EndDate,
            IsAvailable = true
        };
    }

    [Authorize(RinoleggioPermissions.Customer.ManageFavorites)]
    public async Task RemoveFavoriteAsync(Guid listingId)
    {
        var query = await _favorites.GetQueryableAsync();
        var favorite = await AsyncExecuter.FirstOrDefaultAsync(query.Where(x => x.UserId == CurrentUser.GetId() && x.ListingId == listingId));
        if (favorite != null) await _favorites.DeleteAsync(favorite, autoSave: true);
    }

    private async Task CheckListingAccessAsync(Listing listing)
    {
        if (listing.OwnerId == CurrentUser.GetId() &&
            await AuthorizationService.IsGrantedAsync(RinoleggioPermissions.Dealer.ViewOwnListings))
        {
            return;
        }
        if (await AuthorizationService.IsGrantedAsync(RinoleggioPermissions.BackOffice.Listings)) return;
        throw new AbpAuthorizationException();
    }

    private async Task CheckListingEditAccessAsync(Listing listing)
    {
        if (listing.OwnerId == CurrentUser.GetId() &&
            await AuthorizationService.IsGrantedAsync(RinoleggioPermissions.Dealer.ManageOwnListings))
        {
            return;
        }

        if (await AuthorizationService.IsGrantedAsync(RinoleggioPermissions.BackOffice.ManageListings)) return;
        throw new AbpAuthorizationException();
    }

    private async Task NotifyBackOfficeAsync(Listing listing, Lead lead)
    {
        var recipients = new List<IdentityUser>();
        recipients.AddRange(await _userManager.GetUsersInRoleAsync("BackOffice"));
        recipients.AddRange(await _userManager.GetUsersInRoleAsync("Administrator"));
        var emails = recipients.Where(x => x.EmailConfirmed && !x.Email.IsNullOrWhiteSpace())
            .Select(x => x.Email!).Distinct().ToList();
        foreach (var email in emails) await SendEmailSafelyAsync(email, "Nuova richiesta Rinoleggio", $"Nuova richiesta per '{listing.Title}'.");
    }

    private async Task SendEmailSafelyAsync(string email, string subject, string body)
    {
        try
        {
            await _emailSender.SendAsync(email, subject, body, isBodyHtml: false);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Rinoleggio email notification could not be delivered.");
        }
    }

    private static string GetDisplayName(IdentityUser? user, string fallback = "Utente")
    {
        if (user == null) return fallback;
        var name = string.Join(" ", new[] { user.Name, user.Surname }.Where(x => !x.IsNullOrWhiteSpace()));
        return name.IsNullOrWhiteSpace() ? fallback : name;
    }

    private static LeadDto ToBackOfficeDto(Lead lead, string title, IdentityUser? buyer, IdentityUser? seller)
    {
        return new LeadDto
        {
            Id = lead.Id,
            ListingId = lead.ListingId,
            ListingTitle = title,
            BuyerId = lead.BuyerId,
            BuyerDisplayName = GetDisplayName(buyer),
            BuyerEmail = buyer?.Email,
            BuyerAccountPhoneNumber = buyer?.PhoneNumber,
            SellerId = lead.SellerId,
            SellerDisplayName = GetDisplayName(seller),
            SellerEmail = seller?.Email,
            SellerPhoneNumber = seller?.PhoneNumber,
            Message = lead.Message,
            BuyerPhone = lead.BuyerPhone,
            Status = lead.Status,
            Outcome = lead.Outcome,
            OutcomeNotes = lead.OutcomeNotes,
            InternalNotes = lead.InternalNotes,
            CreationTime = lead.CreationTime,
            LastModificationTime = lead.LastModificationTime
        };
    }

    private static IQueryable<Listing> ApplySort(IQueryable<Listing> query, string? sorting) => sorting?.ToLowerInvariant() switch
    {
        "monthlyfee asc" => query.OrderBy(x => x.MonthlyFee).ThenBy(x => x.Id),
        "monthlyfee desc" => query.OrderByDescending(x => x.MonthlyFee).ThenBy(x => x.Id),
        "enddate desc" => query.OrderByDescending(x => x.EndDate).ThenBy(x => x.Id),
        "creationtime desc" => query.OrderByDescending(x => x.CreationTime).ThenBy(x => x.Id),
        _ => query.OrderBy(x => x.EndDate).ThenBy(x => x.MonthlyFee).ThenBy(x => x.Id)
    };

    private static IQueryable<Listing> ApplyPublicSort(IQueryable<Listing> query, string? sorting) => sorting?.ToLowerInvariant() switch
    {
        "monthlyfee asc" => query.OrderBy(x => x.MonthlyFee).ThenBy(x => x.Id),
        "monthlyfee desc" => query.OrderByDescending(x => x.MonthlyFee).ThenBy(x => x.Id),
        "enddate desc" => query.OrderByDescending(x => x.EndDate).ThenBy(x => x.Id),
        _ => query.OrderBy(x => x.EndDate).ThenBy(x => x.MonthlyFee).ThenBy(x => x.Id)
    };

    private static void ValidateListing(CreateUpdateListingDto input)
    {
        if (input.EndDate <= DateTime.UtcNow) throw new BusinessException("Rinoleggio:EndDateMustBeFuture");
        if (input.StartDate.HasValue && input.EndDate <= input.StartDate.Value) throw new BusinessException("Rinoleggio:InvalidDateRange");
        if (input.CurrentKm > input.TotalKm) throw new BusinessException("Rinoleggio:CurrentKmExceedsTotal");
        if (input.ContributionType == ContributionType.None && input.ContributionAmount != 0) throw new BusinessException("Rinoleggio:InvalidContribution");
        if (input.ContributionType != ContributionType.None && input.ContributionAmount <= 0) throw new BusinessException("Rinoleggio:InvalidContribution");
    }

    private static void ValidateReason(string? reason)
    {
        if (reason.IsNullOrWhiteSpace()) throw new BusinessException("Rinoleggio:StatusReasonRequired");
    }

    private static void ApplyInput(Listing listing, CreateUpdateListingDto input)
    {
        listing.Title = input.Title.Trim(); listing.Description = input.Description?.Trim(); listing.ImageUrls = input.ImageUrls?.Trim();
        listing.Province = input.Province.Trim(); listing.RentalCompany = input.RentalCompany.Trim(); listing.Brand = input.Brand.Trim();
        listing.Model = input.Model.Trim(); listing.Version = input.Version?.Trim(); listing.FuelType = input.FuelType; listing.Transmission = input.Transmission;
        listing.RegistrationYear = input.RegistrationYear; listing.StartDate = input.StartDate; listing.EndDate = input.EndDate;
        listing.MonthlyFee = input.MonthlyFee; listing.VatIncluded = input.VatIncluded; listing.InitialPayment = input.InitialPayment;
        listing.TotalKm = input.TotalKm; listing.CurrentKm = input.CurrentKm; listing.RemainingKm = Math.Max(0, input.TotalKm - input.CurrentKm);
        listing.ExcessKmRate = input.ExcessKmRate; listing.UnderKmRate = input.UnderKmRate; listing.ContributionType = input.ContributionType;
        listing.ContributionAmount = input.ContributionAmount; listing.MaintenanceIncluded = input.MaintenanceIncluded; listing.InsuranceIncluded = input.InsuranceIncluded;
        listing.InsuranceKinds = input.InsuranceIncluded ? input.InsuranceKinds?.Trim() : null; listing.RoadsideAssistanceIncluded = input.RoadsideAssistanceIncluded;
        listing.ReplacementCarIncluded = input.ReplacementCarIncluded; listing.TiresIncluded = input.TiresIncluded; listing.RoadTaxIncluded = input.RoadTaxIncluded;
    }

    private static ListingDto ToDto(Listing x) => new()
    {
        Id = x.Id, OwnerId = x.OwnerId, Title = x.Title, Description = x.Description, ImageUrls = x.ImageUrls, Province = x.Province,
        RentalCompany = x.RentalCompany, Brand = x.Brand, Model = x.Model, Version = x.Version, FuelType = x.FuelType,
        Transmission = x.Transmission, RegistrationYear = x.RegistrationYear, StartDate = x.StartDate, EndDate = x.EndDate, MonthlyFee = x.MonthlyFee,
        VatIncluded = x.VatIncluded, InitialPayment = x.InitialPayment, TotalKm = x.TotalKm, CurrentKm = x.CurrentKm, RemainingKm = x.RemainingKm,
        ExcessKmRate = x.ExcessKmRate, UnderKmRate = x.UnderKmRate, ContributionType = x.ContributionType, ContributionAmount = x.ContributionAmount,
        MaintenanceIncluded = x.MaintenanceIncluded, InsuranceIncluded = x.InsuranceIncluded, InsuranceKinds = x.InsuranceKinds,
        RoadsideAssistanceIncluded = x.RoadsideAssistanceIncluded, ReplacementCarIncluded = x.ReplacementCarIncluded, TiresIncluded = x.TiresIncluded,
        RoadTaxIncluded = x.RoadTaxIncluded, Status = x.Status, IsPublished = x.IsPublished, StatusReason = x.StatusReason, CreationTime = x.CreationTime, LastModificationTime = x.LastModificationTime
    };
}
