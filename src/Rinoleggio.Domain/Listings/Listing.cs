using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace Rinoleggio.Listings;

public class Listing : AuditedAggregateRoot<Guid>
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
    public ListingStatus Status { get; set; } = ListingStatus.Draft;
    public bool IsPublished { get; set; }
    public string? StatusReason { get; set; }
    public bool StatusChangedByBackOffice { get; set; }
    public ListingStatus? StatusBeforeDeletion { get; set; }
    public bool? WasPublishedBeforeDeletion { get; set; }
    public bool? StatusChangedByBackOfficeBeforeDeletion { get; set; }

    protected Listing() { }
    public Listing(Guid id, Guid ownerId, string title) : base(id)
    {
        OwnerId = ownerId;
        Title = title;
    }
}
