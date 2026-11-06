using System;
using System.Linq;
using Rinoleggio.Listings;

namespace Rinoleggio.Contracts;

internal static class ContractQueryExtensions
{
    private static readonly string[] AllowedSortFields =
    [
        "EndDate",
        "MonthlyFee",
        "RemainingKm",
        "RegistrationYear",
        "Title"
    ];

    public static IQueryable<Listing> ApplyPublicEligibilityFilter(this IQueryable<Listing> query, DateOnly today)
    {
        return query.Where(l =>
            l.IsPublished &&
            (l.BusinessStatus == ListingBusinessStatus.Active || l.BusinessStatus == ListingBusinessStatus.Reserved) &&
            l.Contract.EndDate > today);
    }

    public static IQueryable<Listing> ApplySearchFilters(this IQueryable<Listing> query, ContractSearchInput input, DateOnly today)
    {
        if (!string.IsNullOrWhiteSpace(input.Query))
        {
            var term = input.Query.Trim().ToLower();
            query = query.Where(l =>
                l.Title.ToLower().Contains(term) ||
                l.Vehicle.Model.ToLower().Contains(term) ||
                (l.Vehicle.Version != null && l.Vehicle.Version.ToLower().Contains(term)) ||
                l.Vehicle.Brand!.Name.ToLower().Contains(term));
        }

        if (input.ProvinceId.HasValue)
        {
            query = query.Where(l => l.ProvinceId == input.ProvinceId.Value);
        }

        if (input.BusinessStatus.HasValue)
        {
            query = query.Where(l => l.BusinessStatus == input.BusinessStatus.Value);
        }

        if (input.ContractExpired == true)
        {
            query = query.Where(l => l.Contract.EndDate <= today);
        }
        else if (input.ContractExpired == false)
        {
            query = query.Where(l => l.Contract.EndDate > today);
        }

        if (input.BrandId.HasValue)
        {
            query = query.Where(l => l.Vehicle.BrandId == input.BrandId.Value);
        }

        if (!string.IsNullOrWhiteSpace(input.ModelVersion))
        {
            var mv = input.ModelVersion.Trim().ToLower();
            query = query.Where(l =>
                l.Vehicle.Model.ToLower().Contains(mv) ||
                (l.Vehicle.Version != null && l.Vehicle.Version.ToLower().Contains(mv)));
        }

        if (input.RentalCompanyId.HasValue)
        {
            query = query.Where(l => l.Contract.RentalCompanyId == input.RentalCompanyId.Value);
        }

        if (input.FuelType.HasValue)
        {
            query = query.Where(l => l.Vehicle.FuelType == input.FuelType.Value);
        }

        if (input.Transmission.HasValue)
        {
            query = query.Where(l => l.Vehicle.Transmission == input.Transmission.Value);
        }

        if (input.RegistrationYearMin.HasValue)
        {
            query = query.Where(l => l.Vehicle.RegistrationYear >= input.RegistrationYearMin.Value);
        }

        if (input.RegistrationYearMax.HasValue)
        {
            query = query.Where(l => l.Vehicle.RegistrationYear <= input.RegistrationYearMax.Value);
        }

        if (input.MonthlyFeeMin.HasValue)
        {
            query = query.Where(l => l.Contract.MonthlyFee >= input.MonthlyFeeMin.Value);
        }

        if (input.MonthlyFeeMax.HasValue)
        {
            query = query.Where(l => l.Contract.MonthlyFee <= input.MonthlyFeeMax.Value);
        }

        if (input.InitialPaymentMin.HasValue)
        {
            query = query.Where(l => l.Contract.InitialPayment >= input.InitialPaymentMin.Value);
        }

        if (input.InitialPaymentMax.HasValue)
        {
            query = query.Where(l => l.Contract.InitialPayment <= input.InitialPaymentMax.Value);
        }

        if (input.VatIncluded.HasValue)
        {
            query = query.Where(l => l.Contract.VatIncluded == input.VatIncluded.Value);
        }

        if (input.TotalKmMin.HasValue)
        {
            query = query.Where(l => l.Contract.TotalKm >= input.TotalKmMin.Value);
        }

        if (input.TotalKmMax.HasValue)
        {
            query = query.Where(l => l.Contract.TotalKm <= input.TotalKmMax.Value);
        }

        if (input.CurrentKmMin.HasValue)
        {
            query = query.Where(l => l.Contract.CurrentKm >= input.CurrentKmMin.Value);
        }

        if (input.CurrentKmMax.HasValue)
        {
            query = query.Where(l => l.Contract.CurrentKm <= input.CurrentKmMax.Value);
        }

        if (input.RemainingKmMin.HasValue)
        {
            query = query.Where(l => l.Contract.RemainingKm >= input.RemainingKmMin.Value);
        }

        if (input.RemainingKmMax.HasValue)
        {
            query = query.Where(l => l.Contract.RemainingKm <= input.RemainingKmMax.Value);
        }

        if (input.ExcessKmRateMin.HasValue)
        {
            query = query.Where(l => l.Contract.ExcessKmRate >= input.ExcessKmRateMin.Value);
        }

        if (input.ExcessKmRateMax.HasValue)
        {
            query = query.Where(l => l.Contract.ExcessKmRate <= input.ExcessKmRateMax.Value);
        }

        if (input.UnderKmRateMin.HasValue)
        {
            query = query.Where(l => l.Contract.UnderKmRate >= input.UnderKmRateMin.Value);
        }

        if (input.UnderKmRateMax.HasValue)
        {
            query = query.Where(l => l.Contract.UnderKmRate <= input.UnderKmRateMax.Value);
        }

        if (input.EndDateMin.HasValue)
        {
            query = query.Where(l => l.Contract.EndDate >= input.EndDateMin.Value);
        }

        if (input.EndDateMax.HasValue)
        {
            query = query.Where(l => l.Contract.EndDate <= input.EndDateMax.Value);
        }

        if (input.MinimumMonthsRemaining.HasValue)
        {
            var minEnd = today.AddMonths(input.MinimumMonthsRemaining.Value);
            query = query.Where(l => l.Contract.EndDate >= minEnd);
        }

        if (input.ContributionType.HasValue)
        {
            query = query.Where(l => l.TransferContributionType == input.ContributionType.Value);
        }

        if (input.ContributionAmountMin.HasValue)
        {
            query = query.Where(l => l.TransferContributionAmount >= input.ContributionAmountMin.Value);
        }

        if (input.ContributionAmountMax.HasValue)
        {
            query = query.Where(l => l.TransferContributionAmount <= input.ContributionAmountMax.Value);
        }

        if (input.MaintenanceIncluded == true)
        {
            query = query.Where(l => l.Services.MaintenanceIncluded);
        }

        if (input.InsuranceIncluded == true)
        {
            query = query.Where(l => l.Services.InsuranceIncluded);
        }

        if (input.RoadsideAssistanceIncluded == true)
        {
            query = query.Where(l => l.Services.RoadsideAssistanceIncluded);
        }

        if (input.ReplacementCarIncluded == true)
        {
            query = query.Where(l => l.Services.ReplacementCarIncluded);
        }

        if (input.TiresIncluded == true)
        {
            query = query.Where(l => l.Services.TiresIncluded);
        }

        if (input.RoadTaxIncluded == true)
        {
            query = query.Where(l => l.Services.RoadTaxIncluded);
        }

        if (input.InsuranceKinds.Count > 0)
        {
            query = query.Where(l =>
                l.Services.InsuranceIncluded &&
                l.Services.InsuranceTypes.Any(t => input.InsuranceKinds.Contains(t.InsuranceKind)));
        }

        return query;
    }

    public static IQueryable<Listing> ApplySorting(this IQueryable<Listing> query, string? sorting)
    {
        if (string.IsNullOrWhiteSpace(sorting))
        {
            return query
                .OrderBy(l => l.Contract.EndDate)
                .ThenBy(l => l.Contract.MonthlyFee);
        }

        var parts = sorting.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var field = parts[0];
        var desc = parts.Length > 1 && parts[1].Equals("desc", StringComparison.OrdinalIgnoreCase);

        if (!AllowedSortFields.Contains(field, StringComparer.OrdinalIgnoreCase))
        {
            return query.OrderBy(l => l.Contract.EndDate).ThenBy(l => l.Contract.MonthlyFee);
        }

        return field.ToLowerInvariant() switch
        {
            "enddate" => desc ? query.OrderByDescending(l => l.Contract.EndDate) : query.OrderBy(l => l.Contract.EndDate),
            "monthlyfee" => desc ? query.OrderByDescending(l => l.Contract.MonthlyFee) : query.OrderBy(l => l.Contract.MonthlyFee),
            "remainingkm" => desc ? query.OrderByDescending(l => l.Contract.RemainingKm) : query.OrderBy(l => l.Contract.RemainingKm),
            "registrationyear" => desc ? query.OrderByDescending(l => l.Vehicle.RegistrationYear) : query.OrderBy(l => l.Vehicle.RegistrationYear),
            "title" => desc ? query.OrderByDescending(l => l.Title) : query.OrderBy(l => l.Title),
            _ => query.OrderBy(l => l.Contract.EndDate)
        };
    }
}
