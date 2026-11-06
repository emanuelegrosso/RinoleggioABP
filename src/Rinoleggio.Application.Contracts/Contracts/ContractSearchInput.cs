using System;
using System.Collections.Generic;
using Rinoleggio.Listings;
using Volo.Abp.Application.Dtos;

namespace Rinoleggio.Contracts;

public class ContractSearchInput : PagedAndSortedResultRequestDto
{
    public string? Query { get; set; }
    public Guid? ProvinceId { get; set; }
    public ListingBusinessStatus? BusinessStatus { get; set; }
    public bool? IncludeDeleted { get; set; }
    public bool? HasLeads { get; set; }
    public bool? ContractExpired { get; set; }
    public Guid? BrandId { get; set; }
    public string? ModelVersion { get; set; }
    public Guid? RentalCompanyId { get; set; }
    public FuelType? FuelType { get; set; }
    public Transmission? Transmission { get; set; }
    public int? RegistrationYearMin { get; set; }
    public int? RegistrationYearMax { get; set; }
    public decimal? MonthlyFeeMin { get; set; }
    public decimal? MonthlyFeeMax { get; set; }
    public decimal? InitialPaymentMin { get; set; }
    public decimal? InitialPaymentMax { get; set; }
    public bool? VatIncluded { get; set; }
    public int? TotalKmMin { get; set; }
    public int? TotalKmMax { get; set; }
    public int? CurrentKmMin { get; set; }
    public int? CurrentKmMax { get; set; }
    public int? RemainingKmMin { get; set; }
    public int? RemainingKmMax { get; set; }
    public decimal? ExcessKmRateMin { get; set; }
    public decimal? ExcessKmRateMax { get; set; }
    public decimal? UnderKmRateMin { get; set; }
    public decimal? UnderKmRateMax { get; set; }
    public DateOnly? EndDateMin { get; set; }
    public DateOnly? EndDateMax { get; set; }
    public int? MinimumMonthsRemaining { get; set; }
    public TransferContributionType? ContributionType { get; set; }
    public decimal? ContributionAmountMin { get; set; }
    public decimal? ContributionAmountMax { get; set; }
    public bool? MaintenanceIncluded { get; set; }
    public bool? InsuranceIncluded { get; set; }
    public bool? RoadsideAssistanceIncluded { get; set; }
    public bool? ReplacementCarIncluded { get; set; }
    public bool? TiresIncluded { get; set; }
    public bool? RoadTaxIncluded { get; set; }
    public List<InsuranceKind> InsuranceKinds { get; set; } = [];
}
