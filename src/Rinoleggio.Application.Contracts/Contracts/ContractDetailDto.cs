using System;
using System.Collections.Generic;
using Rinoleggio.Listings;

namespace Rinoleggio.Contracts;

public class ContractDetailDto : ContractSearchResultDto
{
    public string? DescriptionHtml { get; set; }
    public Guid ProvinceId { get; set; }
    public Guid BrandId { get; set; }
    public Guid RentalCompanyId { get; set; }
    public DateOnly? StartDate { get; set; }
    public int TotalKm { get; set; }
    public int CurrentKm { get; set; }
    public decimal? ExcessKmRate { get; set; }
    public decimal? UnderKmRate { get; set; }
    public Guid OwnerId { get; set; }
    public string? SellerVisibleName { get; set; }
    public string? StatusReason { get; set; }
    public StatusChangedBy? StatusChangedBy { get; set; }
    public bool CanEdit { get; set; }
    public List<ListingImageDto> Images { get; set; } = [];
}

public class ListingImageDto
{
    public Guid Id { get; set; }
    public string StoragePathOrBlobId { get; set; } = null!;
    public byte SortOrder { get; set; }
}
