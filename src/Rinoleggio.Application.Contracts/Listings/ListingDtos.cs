using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace Rinoleggio.Listings;

public class ListingSearchInput : PagedAndSortedResultRequestDto
{
    public string? Query { get; set; }
    public string? Province { get; set; }
    public string? RentalCompany { get; set; }
    public string? Brand { get; set; }
    public string? Model { get; set; }
    public FuelType? FuelType { get; set; }
    public TransmissionType? Transmission { get; set; }
    public int? YearFrom { get; set; }
    public int? YearTo { get; set; }
    public decimal? MonthlyFeeFrom { get; set; }
    public decimal? MonthlyFeeTo { get; set; }
    public int? RemainingKmFrom { get; set; }
    public int? RemainingKmTo { get; set; }
    public DateTime? EndDateFrom { get; set; }
    public DateTime? EndDateTo { get; set; }
    public bool? VatIncluded { get; set; }
    public ContributionType? ContributionType { get; set; }
    public ListingStatus? Status { get; set; }
}

public class ListingDto : AuditedEntityDto<Guid>
{
    public Guid OwnerId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrls { get; set; }
    public string Province { get; set; } = string.Empty;
    public string RentalCompany { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string? Version { get; set; }
    public FuelType FuelType { get; set; }
    public TransmissionType Transmission { get; set; }
    public int RegistrationYear { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal MonthlyFee { get; set; }
    public bool VatIncluded { get; set; }
    public decimal InitialPayment { get; set; }
    public int TotalKm { get; set; }
    public int CurrentKm { get; set; }
    public int RemainingKm { get; set; }
    public decimal? ExcessKmRate { get; set; }
    public decimal? UnderKmRate { get; set; }
    public ContributionType ContributionType { get; set; }
    public decimal ContributionAmount { get; set; }
    public bool MaintenanceIncluded { get; set; }
    public bool InsuranceIncluded { get; set; }
    public string? InsuranceKinds { get; set; }
    public bool RoadsideAssistanceIncluded { get; set; }
    public bool ReplacementCarIncluded { get; set; }
    public bool TiresIncluded { get; set; }
    public bool RoadTaxIncluded { get; set; }
    public ListingStatus Status { get; set; }
    public bool IsPublished { get; set; }
    public string? StatusReason { get; set; }
}

public class CreateUpdateListingDto
{
    [Required, StringLength(200)] public string Title { get; set; } = string.Empty;
    [StringLength(4000)] public string? Description { get; set; }
    [StringLength(2000)] public string? ImageUrls { get; set; }
    [Required, StringLength(80)] public string Province { get; set; } = string.Empty;
    [Required, StringLength(120)] public string RentalCompany { get; set; } = string.Empty;
    [Required, StringLength(80)] public string Brand { get; set; } = string.Empty;
    [Required, StringLength(80)] public string Model { get; set; } = string.Empty;
    [StringLength(120)] public string? Version { get; set; }
    public FuelType FuelType { get; set; }
    public TransmissionType Transmission { get; set; }
    [Range(2000, 2100)] public int RegistrationYear { get; set; }
    public DateTime? StartDate { get; set; }
    [Required] public DateTime EndDate { get; set; }
    [Range(typeof(decimal), "0.01", "999999")] public decimal MonthlyFee { get; set; }
    public bool VatIncluded { get; set; }
    [Range(typeof(decimal), "0", "999999")] public decimal InitialPayment { get; set; }
    [Range(1, 2000000)] public int TotalKm { get; set; }
    [Range(0, 2000000)] public int CurrentKm { get; set; }
    public decimal? ExcessKmRate { get; set; }
    public decimal? UnderKmRate { get; set; }
    public ContributionType ContributionType { get; set; }
    [Range(typeof(decimal), "0", "999999")] public decimal ContributionAmount { get; set; }
    public bool MaintenanceIncluded { get; set; }
    public bool InsuranceIncluded { get; set; }
    [StringLength(300)] public string? InsuranceKinds { get; set; }
    public bool RoadsideAssistanceIncluded { get; set; }
    public bool ReplacementCarIncluded { get; set; }
    public bool TiresIncluded { get; set; }
    public bool RoadTaxIncluded { get; set; }
    public bool Publish { get; set; }
}

public class ListingStatusDto
{
    public ListingStatus Status { get; set; }
    [StringLength(1000)] public string? Reason { get; set; }
}

public class CreateLeadDto
{
    [Required, StringLength(1000, MinimumLength = 10)] public string Message { get; set; } = string.Empty;
    [StringLength(30)] public string? BuyerPhone { get; set; }
    [Range(typeof(bool), "true", "true", ErrorMessage = "Il consenso privacy è obbligatorio.")] public bool PrivacyConsent { get; set; }
}

public class LeadDto : AuditedEntityDto<Guid>
{
    public Guid ListingId { get; set; }
    public string ListingTitle { get; set; } = string.Empty;
    public Guid BuyerId { get; set; }
    public string BuyerDisplayName { get; set; } = string.Empty;
    public string? BuyerEmail { get; set; }
    public string? BuyerAccountPhoneNumber { get; set; }
    public Guid SellerId { get; set; }
    public string SellerDisplayName { get; set; } = string.Empty;
    public string? SellerEmail { get; set; }
    public string? SellerPhoneNumber { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? BuyerPhone { get; set; }
    public LeadStatus Status { get; set; }
    public LeadOutcome Outcome { get; set; }
    public string? OutcomeNotes { get; set; }
    public ListingStatus? CurrentListingStatus { get; set; }
    public string? InternalNotes { get; set; }
}

public class UpdateLeadDto
{
    public LeadStatus Status { get; set; }
    public LeadOutcome Outcome { get; set; }
    [StringLength(1000)] public string? OutcomeNotes { get; set; }
    public ListingStatus? ListingStatus { get; set; }
    [StringLength(4000)] public string? InternalNotes { get; set; }
}
