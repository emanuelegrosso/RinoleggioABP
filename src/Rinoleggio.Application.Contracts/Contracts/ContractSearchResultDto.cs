using System;
using System.Collections.Generic;
using Rinoleggio.Listings;

namespace Rinoleggio.Contracts;

public class ContractSearchResultDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string SlugPath { get; set; } = null!;
    public string BrandName { get; set; } = null!;
    public string Model { get; set; } = null!;
    public string? Version { get; set; }
    public short RegistrationYear { get; set; }
    public FuelType FuelType { get; set; }
    public Transmission Transmission { get; set; }
    public string RentalCompanyName { get; set; } = null!;
    public decimal MonthlyFee { get; set; }
    public decimal InitialPayment { get; set; }
    public bool VatIncluded { get; set; }
    public int RemainingKm { get; set; }
    public DateOnly EndDate { get; set; }
    public TransferContributionType TransferContributionType { get; set; }
    public decimal TransferContributionAmount { get; set; }
    public string ProvinceName { get; set; } = null!;
    public bool MaintenanceIncluded { get; set; }
    public bool InsuranceIncluded { get; set; }
    public bool RoadsideAssistanceIncluded { get; set; }
    public bool ReplacementCarIncluded { get; set; }
    public bool TiresIncluded { get; set; }
    public bool RoadTaxIncluded { get; set; }
    public List<InsuranceKind> InsuranceKinds { get; set; } = [];
    public string? CoverImagePath { get; set; }
    public ListingBusinessStatus BusinessStatus { get; set; }
    public bool IsPublished { get; set; }
    public string? StatusReason { get; set; }
    public StatusChangedBy? StatusChangedBy { get; set; }
    public int OpenLeadCount { get; set; }
    public int TotalLeadCount { get; set; }
    public string? SellerVisibleName { get; set; }
}
