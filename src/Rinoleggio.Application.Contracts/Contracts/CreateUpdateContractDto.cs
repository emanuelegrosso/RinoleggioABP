using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Rinoleggio.Listings;

namespace Rinoleggio.Contracts;

public class CreateUpdateContractDto
{
    [Required]
    [StringLength(ListingConsts.MaxTitleLength)]
    public string Title { get; set; } = null!;

    public string? DescriptionHtml { get; set; }

    [Required]
    public Guid ProvinceId { get; set; }

    [Required]
    public Guid RentalCompanyId { get; set; }

    public DateOnly? StartDate { get; set; }

    [Required]
    public DateOnly EndDate { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    public decimal MonthlyFee { get; set; }

    public bool VatIncluded { get; set; }

    [Range(0, double.MaxValue)]
    public decimal InitialPayment { get; set; }

    [Range(1, int.MaxValue)]
    public int TotalKm { get; set; }

    [Range(0, int.MaxValue)]
    public int CurrentKm { get; set; }

    public decimal? ExcessKmRate { get; set; }
    public decimal? UnderKmRate { get; set; }

    [Required]
    public Guid BrandId { get; set; }

    [Required]
    [StringLength(ListingConsts.MaxModelLength)]
    public string Model { get; set; } = null!;

    [StringLength(ListingConsts.MaxVersionLength)]
    public string? Version { get; set; }

    public FuelType FuelType { get; set; }

    public Transmission Transmission { get; set; }

    [Required]
    [Range(ListingConsts.MinRegistrationYear, 2100)]
    public short RegistrationYear { get; set; }

    public TransferContributionType TransferContributionType { get; set; } = TransferContributionType.None;

    [Range(0, double.MaxValue)]
    public decimal TransferContributionAmount { get; set; }

    public bool MaintenanceIncluded { get; set; }
    public bool InsuranceIncluded { get; set; }
    public bool RoadsideAssistanceIncluded { get; set; }
    public bool ReplacementCarIncluded { get; set; }
    public bool TiresIncluded { get; set; }
    public bool RoadTaxIncluded { get; set; }
    public List<InsuranceKind> InsuranceKinds { get; set; } = [];
}

public class CreateContractDto : CreateUpdateContractDto
{
    public bool PublishNow { get; set; }
}

public class WithdrawContractDto
{
    [Required]
    [StringLength(ListingConsts.MaxStatusReasonLength)]
    public string Reason { get; set; } = null!;
}

public class CloseContractDto
{
    [StringLength(ListingConsts.MaxStatusReasonLength)]
    public string? Reason { get; set; }
}

public class DeleteContractDto
{
    [Required]
    [StringLength(ListingConsts.MaxStatusReasonLength)]
    public string Reason { get; set; } = null!;
}

public class SuspendContractDto
{
    [Required]
    [StringLength(ListingConsts.MaxStatusReasonLength)]
    public string Reason { get; set; } = null!;
}
