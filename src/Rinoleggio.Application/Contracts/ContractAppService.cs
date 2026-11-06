using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Rinoleggio.Leads;
using Rinoleggio.Listings;
using Rinoleggio.Lookups;
using Rinoleggio.Permissions;
using Rinoleggio.Users;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Authorization;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.Timing;

namespace Rinoleggio.Contracts;

public class ContractAppService : RinoleggioAppService, IContractAppService
{
    private readonly IRepository<Listing, Guid> _listingRepository;
    private readonly IRepository<Lead, Guid> _leadRepository;
    private readonly IRepository<UserProfile, Guid> _userProfileRepository;
    private readonly IRepository<RentalCompany, Guid> _rentalCompanyRepository;
    private readonly IRepository<Brand, Guid> _brandRepository;
    private readonly IRepository<Province, Guid> _provinceRepository;
    private readonly IGuidGenerator _guidGenerator;
    private readonly IClock _clock;

    public ContractAppService(
        IRepository<Listing, Guid> listingRepository,
        IRepository<Lead, Guid> leadRepository,
        IRepository<UserProfile, Guid> userProfileRepository,
        IRepository<RentalCompany, Guid> rentalCompanyRepository,
        IRepository<Brand, Guid> brandRepository,
        IRepository<Province, Guid> provinceRepository,
        IGuidGenerator guidGenerator,
        IClock clock)
    {
        _listingRepository = listingRepository;
        _leadRepository = leadRepository;
        _userProfileRepository = userProfileRepository;
        _rentalCompanyRepository = rentalCompanyRepository;
        _brandRepository = brandRepository;
        _provinceRepository = provinceRepository;
        _guidGenerator = guidGenerator;
        _clock = clock;
    }

    [AllowAnonymous]
    public async Task<PagedResultDto<ContractSearchResultDto>> SearchAsync(ContractSearchInput input)
    {
        var today = DateOnly.FromDateTime(_clock.Now);
        var query = await BuildListingQueryAsync();
        query = query
            .ApplyPublicEligibilityFilter(today)
            .ApplySearchFilters(input, today)
            .ApplySorting(input.Sorting);

        return await ToPagedResultAsync(query, input, includeLeadCounts: false, includeSellerName: false);
    }

    [AllowAnonymous]
    public async Task<ContractDetailDto> GetAsync(Guid id)
    {
        var listing = await GetListingWithDetailsAsync(id);
        await EnsureCanViewAsync(listing);
        return await ToDetailDtoAsync(listing);
    }

    [AllowAnonymous]
    public async Task<ContractLookupsDto> GetLookupsAsync()
    {
        var companies = await _rentalCompanyRepository.GetListAsync(x => x.IsActive);
        var brands = await _brandRepository.GetListAsync(x => x.IsActive);
        var provinces = await _provinceRepository.GetListAsync(x => x.IsActive);

        return new ContractLookupsDto
        {
            RentalCompanies = companies.OrderBy(x => x.Name).Select(x => new LookupItemDto { Id = x.Id, Name = x.Name }).ToList(),
            Brands = brands.OrderBy(x => x.SortOrder).ThenBy(x => x.Name).Select(x => new LookupItemDto { Id = x.Id, Name = x.Name }).ToList(),
            Provinces = provinces.OrderBy(x => x.SortOrder).ThenBy(x => x.Name).Select(x => new LookupItemDto { Id = x.Id, Name = x.Name, Code = x.Code }).ToList()
        };
    }

    [Authorize(RinoleggioPermissions.Contracts.Create)]
    public async Task<ContractDetailDto> CreateAsync(CreateContractDto input)
    {
        var today = DateOnly.FromDateTime(_clock.Now);
        var listing = ListingFactory.Create(
            CurrentUser.Id!.Value,
            input,
            _guidGenerator,
            _clock.Now,
            today,
            input.PublishNow);
        await _listingRepository.InsertAsync(listing, autoSave: true);
        listing = await GetListingWithDetailsAsync(listing.Id);
        return await ToDetailDtoAsync(listing);
    }

    [Authorize(RinoleggioPermissions.Contracts.Edit)]
    public async Task<ContractDetailDto> UpdateAsync(Guid id, CreateUpdateContractDto input)
    {
        var listing = await GetListingWithDetailsAsync(id);
        await EnsureCanEditAsync(listing);
        var today = DateOnly.FromDateTime(_clock.Now);
        ListingFactory.Update(listing, input, today);
        await _listingRepository.UpdateAsync(listing, autoSave: true);
        listing = await GetListingWithDetailsAsync(id);
        return await ToDetailDtoAsync(listing);
    }

    [Authorize(RinoleggioPermissions.Contracts.Edit)]
    public async Task WithdrawAsync(Guid id, WithdrawContractDto input)
    {
        var listing = await GetListingWithDetailsAsync(id);
        await EnsureCanMutateAsync(listing);
        var changedBy = await GetStatusActorAsync(listing);
        listing.Withdraw(input.Reason, changedBy);
        await _listingRepository.UpdateAsync(listing, autoSave: true);
    }

    [Authorize(RinoleggioPermissions.Contracts.Close)]
    public async Task CloseAsync(Guid id, CloseContractDto input)
    {
        var listing = await GetListingWithDetailsAsync(id);
        await EnsureCanMutateAsync(listing);
        var changedBy = await GetStatusActorAsync(listing);
        listing.Close(changedBy, input.Reason);
        await _listingRepository.UpdateAsync(listing, autoSave: true);
    }

    [Authorize(RinoleggioPermissions.Contracts.Delete)]
    public async Task DeleteAsync(Guid id, DeleteContractDto input)
    {
        var listing = await GetListingWithDetailsAsync(id);
        await EnsureCanMutateAsync(listing);
        var changedBy = await GetStatusActorAsync(listing);
        listing.MarkDeleted(input.Reason, changedBy);
        await _listingRepository.UpdateAsync(listing, autoSave: true);
    }

    [Authorize(RinoleggioPermissions.Contracts.Restore)]
    public async Task RestoreAsync(Guid id)
    {
        var listing = await GetListingWithDetailsAsync(id);
        listing.Restore();
        await _listingRepository.UpdateAsync(listing, autoSave: true);
    }

    [Authorize(RinoleggioPermissions.Contracts.Edit)]
    public async Task UnpublishAsync(Guid id)
    {
        var listing = await GetListingWithDetailsAsync(id);
        await EnsureCanEditAsync(listing);
        listing.UnpublishFromSearch();
        await _listingRepository.UpdateAsync(listing, autoSave: true);
    }

    [Authorize(RinoleggioPermissions.Contracts.Edit)]
    public async Task PublishListingAsync(Guid id)
    {
        var listing = await GetListingWithDetailsAsync(id);
        await EnsureCanEditAsync(listing);
        var today = DateOnly.FromDateTime(_clock.Now);
        if (listing.BusinessStatus == ListingBusinessStatus.Withdrawn &&
            listing.StatusChangedBy == StatusChangedBy.Seller &&
            listing.OwnerId == CurrentUser.Id)
        {
            listing.RepublishBySeller(_clock.Now, today);
        }
        else
        {
            listing.Publish(_clock.Now, today);
        }

        await _listingRepository.UpdateAsync(listing, autoSave: true);
    }

    [Authorize(RinoleggioPermissions.Contracts.Edit)]
    public async Task RepublishAsync(Guid id)
    {
        var listing = await GetListingWithDetailsAsync(id);
        await EnsureCanEditAsync(listing);
        if (listing.OwnerId != CurrentUser.Id)
        {
            throw new AbpAuthorizationException();
        }

        var today = DateOnly.FromDateTime(_clock.Now);
        listing.RepublishBySeller(_clock.Now, today);
        await _listingRepository.UpdateAsync(listing, autoSave: true);
    }

    [Authorize(RinoleggioPermissions.Contracts.Edit)]
    public async Task ReserveAsync(Guid id)
    {
        var listing = await GetListingWithDetailsAsync(id);
        await EnsureCanMutateAsync(listing);
        var changedBy = await GetStatusActorAsync(listing);
        listing.MarkReserved(changedBy);
        await _listingRepository.UpdateAsync(listing, autoSave: true);
    }

    [Authorize(RinoleggioPermissions.Contracts.Edit)]
    public async Task ClearListingReservationAsync(Guid id)
    {
        var listing = await GetListingWithDetailsAsync(id);
        await EnsureCanMutateAsync(listing);
        var changedBy = await GetStatusActorAsync(listing);
        listing.ClearReservation(changedBy);
        await _listingRepository.UpdateAsync(listing, autoSave: true);
    }

    [Authorize(RinoleggioPermissions.Contracts.Suspend)]
    public async Task SuspendAsync(Guid id, SuspendContractDto input)
    {
        var listing = await GetListingWithDetailsAsync(id);
        await AuthorizationService.CheckAsync(RinoleggioPermissions.Contracts.Suspend);
        listing.Suspend(input.Reason, StatusChangedBy.BackOffice);
        await _listingRepository.UpdateAsync(listing, autoSave: true);
    }

    [Authorize(RinoleggioPermissions.Contracts.Suspend)]
    public async Task ReactivateAsync(Guid id)
    {
        var listing = await GetListingWithDetailsAsync(id);
        listing.Reactivate();
        await _listingRepository.UpdateAsync(listing, autoSave: true);
    }

    [Authorize(RinoleggioPermissions.Contracts.ViewOwn)]
    public async Task<PagedResultDto<ContractSearchResultDto>> GetMyListAsync(ContractSearchInput input)
    {
        var today = DateOnly.FromDateTime(_clock.Now);
        var ownerId = CurrentUser.Id!.Value;
        var query = await BuildListingQueryAsync();
        query = query
            .Where(l => l.OwnerId == ownerId && l.BusinessStatus != ListingBusinessStatus.Deleted)
            .ApplySearchFilters(input, today)
            .ApplySorting(input.Sorting);

        return await ToPagedResultAsync(query, input, includeLeadCounts: true, includeSellerName: false);
    }

    [Authorize(RinoleggioPermissions.Contracts.ManageAll)]
    public async Task<PagedResultDto<ContractSearchResultDto>> GetManageListAsync(ContractSearchInput input)
    {
        var today = DateOnly.FromDateTime(_clock.Now);
        var canRestore = await AuthorizationService.IsGrantedAsync(RinoleggioPermissions.Contracts.Restore);
        var query = await BuildListingQueryAsync();

        if (input.IncludeDeleted == true && canRestore)
        {
            query = query.Where(l => l.BusinessStatus == ListingBusinessStatus.Deleted);
        }
        else
        {
            query = query.Where(l => l.BusinessStatus != ListingBusinessStatus.Deleted);
        }

        query = query.ApplySearchFilters(input, today).ApplySorting(input.Sorting);

        if (input.HasLeads == true)
        {
            var listingIdsWithLeads = await GetListingIdsWithLeadsAsync();
            query = query.Where(l => listingIdsWithLeads.Contains(l.Id));
        }
        else if (input.HasLeads == false)
        {
            var listingIdsWithLeads = await GetListingIdsWithLeadsAsync();
            query = query.Where(l => !listingIdsWithLeads.Contains(l.Id));
        }

        return await ToPagedResultAsync(query, input, includeLeadCounts: true, includeSellerName: true);
    }

    private async Task<IQueryable<Listing>> BuildListingQueryAsync()
    {
        return await _listingRepository.WithDetailsAsync(
            l => l.Contract,
            l => l.Contract.RentalCompany!,
            l => l.Vehicle,
            l => l.Vehicle.Brand!,
            l => l.Services,
            l => l.Services.InsuranceTypes,
            l => l.Province!,
            l => l.Images);
    }

    private async Task<PagedResultDto<ContractSearchResultDto>> ToPagedResultAsync(
        IQueryable<Listing> query,
        ContractSearchInput input,
        bool includeLeadCounts,
        bool includeSellerName)
    {
        var totalCount = await AsyncExecuter.CountAsync(query);
        query = query.Skip(input.SkipCount).Take(input.MaxResultCount);
        var items = await AsyncExecuter.ToListAsync(query);

        Dictionary<Guid, (int Total, int Open)>? leadCounts = null;
        if (includeLeadCounts && items.Count > 0)
        {
            leadCounts = await GetLeadCountsForListingsAsync(items.Select(i => i.Id).ToList());
        }

        Dictionary<Guid, string>? sellerNames = null;
        if (includeSellerName && items.Count > 0)
        {
            sellerNames = await GetSellerNamesAsync(items.Select(i => i.OwnerId).Distinct().ToList());
        }

        var dtos = items.Select(listing =>
        {
            var dto = ContractMapper.ToSearchResultDto(listing);
            if (leadCounts != null && leadCounts.TryGetValue(listing.Id, out var counts))
            {
                dto.TotalLeadCount = counts.Total;
                dto.OpenLeadCount = counts.Open;
            }

            if (sellerNames != null && sellerNames.TryGetValue(listing.OwnerId, out var name))
            {
                dto.SellerVisibleName = name;
            }

            return dto;
        }).ToList();

        return new PagedResultDto<ContractSearchResultDto>(totalCount, dtos);
    }

    private async Task<ContractDetailDto> ToDetailDtoAsync(Listing listing)
    {
        var dto = ContractMapper.ToDetailDto(listing);
        var profile = await _userProfileRepository.FindAsync(listing.OwnerId);
        dto.SellerVisibleName = profile?.FullName;
        dto.StatusReason = listing.StatusReason;
        dto.StatusChangedBy = listing.StatusChangedBy;
        dto.CanEdit = listing.CanBeEdited() &&
                      (listing.OwnerId == CurrentUser.Id ||
                       await AuthorizationService.IsGrantedAsync(RinoleggioPermissions.Contracts.ManageAll));
        return dto;
    }

    private async Task<Dictionary<Guid, (int Total, int Open)>> GetLeadCountsForListingsAsync(List<Guid> listingIds)
    {
        var leads = await _leadRepository.GetListAsync(l => listingIds.Contains(l.ListingId));
        return leads
            .GroupBy(l => l.ListingId)
            .ToDictionary(
                g => g.Key,
                g => (
                    Total: g.Count(),
                    Open: g.Count(l => l.Status is LeadStatus.New or LeadStatus.Contacted)));
    }

    private async Task<HashSet<Guid>> GetListingIdsWithLeadsAsync()
    {
        var leads = await _leadRepository.GetListAsync();
        return leads.Select(l => l.ListingId).ToHashSet();
    }

    private async Task<Dictionary<Guid, string>> GetSellerNamesAsync(List<Guid> ownerIds)
    {
        var profiles = await _userProfileRepository.GetListAsync(p => ownerIds.Contains(p.Id));
        return profiles.ToDictionary(p => p.Id, p => p.FullName);
    }

    private async Task<Listing> GetListingWithDetailsAsync(Guid id)
    {
        var queryable = await BuildListingQueryAsync();
        var listing = await AsyncExecuter.FirstOrDefaultAsync(queryable.Where(l => l.Id == id));
        if (listing == null)
        {
            throw new Volo.Abp.Domain.Entities.EntityNotFoundException(typeof(Listing), id);
        }

        return listing;
    }

    private async Task EnsureCanViewAsync(Listing listing)
    {
        var today = DateOnly.FromDateTime(_clock.Now);
        var isPublic = listing.IsPublished &&
                       (listing.BusinessStatus == ListingBusinessStatus.Active ||
                        listing.BusinessStatus == ListingBusinessStatus.Reserved) &&
                       listing.Contract.EndDate > today;

        if (isPublic)
        {
            return;
        }

        if (!CurrentUser.IsAuthenticated)
        {
            throw new AbpAuthorizationException();
        }

        if (listing.OwnerId == CurrentUser.Id ||
            await AuthorizationService.IsGrantedAsync(RinoleggioPermissions.Contracts.ManageAll))
        {
            return;
        }

        throw new AbpAuthorizationException();
    }

    private async Task EnsureCanEditAsync(Listing listing)
    {
        if (!listing.CanBeEdited())
        {
            throw new AbpAuthorizationException();
        }

        if (listing.OwnerId == CurrentUser.Id)
        {
            return;
        }

        if (await AuthorizationService.IsGrantedAsync(RinoleggioPermissions.Contracts.ManageAll))
        {
            return;
        }

        throw new AbpAuthorizationException();
    }

    private async Task EnsureCanMutateAsync(Listing listing)
    {
        if (listing.OwnerId == CurrentUser.Id)
        {
            return;
        }

        if (await AuthorizationService.IsGrantedAsync(RinoleggioPermissions.Contracts.ManageAll))
        {
            return;
        }

        throw new AbpAuthorizationException();
    }

    private async Task<StatusChangedBy> GetStatusActorAsync(Listing listing)
    {
        if (listing.OwnerId == CurrentUser.Id)
        {
            return StatusChangedBy.Seller;
        }

        if (await AuthorizationService.IsGrantedAsync(RinoleggioPermissions.Contracts.ManageAll))
        {
            return StatusChangedBy.BackOffice;
        }

        throw new AbpAuthorizationException();
    }
}
