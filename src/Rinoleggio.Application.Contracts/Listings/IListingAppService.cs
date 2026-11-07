using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Rinoleggio.Listings;

public interface IListingAppService : IApplicationService
{
    Task<PagedResultDto<ListingDto>> SearchAsync(ListingSearchInput input);
    Task<ListingDto> GetPublicAsync(Guid id);
    Task<ListingDto> GetAsync(Guid id);
    Task<PagedResultDto<ListingDto>> GetMineAsync(PagedAndSortedResultRequestDto input);
    Task<ListingDto> CreateAsync(CreateUpdateListingDto input);
    Task<ListingDto> UpdateAsync(Guid id, CreateUpdateListingDto input);
    Task SetMyStatusAsync(Guid id, ListingStatusDto input);
    Task<PagedResultDto<ListingDto>> GetBackOfficeListAsync(ListingSearchInput input);
    Task SetBackOfficeStatusAsync(Guid id, ListingStatusDto input);
    Task<PagedResultDto<CustomerLeadDto>> GetMyLeadsAsync(PagedAndSortedResultRequestDto input);
    Task<CustomerLeadDto> CreateLeadAsync(Guid listingId, CreateLeadDto input);
    Task<PagedResultDto<DealerLeadDto>> GetReceivedLeadsAsync(PagedAndSortedResultRequestDto input);
    Task<PagedResultDto<LeadDto>> GetBackOfficeLeadsAsync(BackOfficeLeadSearchInput input);
    Task UpdateLeadAsync(Guid id, UpdateLeadDto input);
    Task<IReadOnlyList<string>> GetLookupValuesAsync(string kind);
    Task<PagedResultDto<FavoriteListingDto>> GetFavoritesAsync(PagedAndSortedResultRequestDto input);
    Task<FavoriteListingDto> AddFavoriteAsync(Guid listingId);
    Task RemoveFavoriteAsync(Guid listingId);
    Task<PagedResultDto<ListingDto>> GetDeletedListingsAsync(PagedAndSortedResultRequestDto input);
    Task RestoreDeletedListingAsync(Guid listingId);
}

public class FavoriteListingDto : EntityDto<Guid>
{
    public Guid ListingId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string Province { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int RegistrationYear { get; set; }
    public decimal MonthlyFee { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsAvailable { get; set; }
}

public class BackOfficeLeadSearchInput : PagedAndSortedResultRequestDto
{
    public LeadStatus? Status { get; set; }
}

public class CustomerLeadDto : EntityDto<Guid>
{
    public Guid ListingId { get; set; }
    public string ListingTitle { get; set; } = string.Empty;
    public bool ListingAvailable { get; set; }
    public string DealerDisplayName { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public LeadStatus Status { get; set; }
    public DateTime CreationTime { get; set; }
}

public class DealerLeadDto : EntityDto<Guid>
{
    public Guid ListingId { get; set; }
    public string ListingTitle { get; set; } = string.Empty;
    public ListingStatus? CurrentListingStatus { get; set; }
    public string CustomerDisplayName { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public LeadStatus Status { get; set; }
    public DateTime CreationTime { get; set; }
}
