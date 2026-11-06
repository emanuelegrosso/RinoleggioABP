using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNetCore.WebUtilities;
using Rinoleggio.Contracts;
using Rinoleggio.Listings;

namespace Rinoleggio.Web.Pages.Contracts;

public static class ContractSearchQueryHelper
{
    private const string Prefix = "Filters.";

    public static string BuildPageUrl(string pagePath, ContractSearchInput filters, int skipCount)
    {
        var pairs = ToQueryPairs(filters, skipCount);
        return QueryHelpers.AddQueryString(pagePath, pairs);
    }

    private static List<KeyValuePair<string, string?>> ToQueryPairs(ContractSearchInput filters, int skipCount)
    {
        var pairs = new List<KeyValuePair<string, string?>>();

        void Add(string suffix, string? value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                pairs.Add(new KeyValuePair<string, string?>(Prefix + suffix, value));
            }
        }

        Add(nameof(ContractSearchInput.Query), filters.Query);
        AddGuid(nameof(ContractSearchInput.BrandId), filters.BrandId);
        Add(nameof(ContractSearchInput.ModelVersion), filters.ModelVersion);
        AddGuid(nameof(ContractSearchInput.RentalCompanyId), filters.RentalCompanyId);
        AddEnum(nameof(ContractSearchInput.FuelType), filters.FuelType);
        AddEnum(nameof(ContractSearchInput.Transmission), filters.Transmission);
        AddInt(nameof(ContractSearchInput.RegistrationYearMin), filters.RegistrationYearMin);
        AddInt(nameof(ContractSearchInput.RegistrationYearMax), filters.RegistrationYearMax);
        AddDecimal(nameof(ContractSearchInput.MonthlyFeeMin), filters.MonthlyFeeMin);
        AddDecimal(nameof(ContractSearchInput.MonthlyFeeMax), filters.MonthlyFeeMax);
        AddDecimal(nameof(ContractSearchInput.InitialPaymentMin), filters.InitialPaymentMin);
        AddDecimal(nameof(ContractSearchInput.InitialPaymentMax), filters.InitialPaymentMax);
        AddBool(nameof(ContractSearchInput.VatIncluded), filters.VatIncluded);
        AddInt(nameof(ContractSearchInput.TotalKmMin), filters.TotalKmMin);
        AddInt(nameof(ContractSearchInput.TotalKmMax), filters.TotalKmMax);
        AddInt(nameof(ContractSearchInput.CurrentKmMin), filters.CurrentKmMin);
        AddInt(nameof(ContractSearchInput.CurrentKmMax), filters.CurrentKmMax);
        AddInt(nameof(ContractSearchInput.RemainingKmMin), filters.RemainingKmMin);
        AddInt(nameof(ContractSearchInput.RemainingKmMax), filters.RemainingKmMax);
        AddDecimal(nameof(ContractSearchInput.ExcessKmRateMin), filters.ExcessKmRateMin);
        AddDecimal(nameof(ContractSearchInput.ExcessKmRateMax), filters.ExcessKmRateMax);
        AddDecimal(nameof(ContractSearchInput.UnderKmRateMin), filters.UnderKmRateMin);
        AddDecimal(nameof(ContractSearchInput.UnderKmRateMax), filters.UnderKmRateMax);
        AddDate(nameof(ContractSearchInput.EndDateMin), filters.EndDateMin);
        AddDate(nameof(ContractSearchInput.EndDateMax), filters.EndDateMax);
        AddInt(nameof(ContractSearchInput.MinimumMonthsRemaining), filters.MinimumMonthsRemaining);
        AddEnum(nameof(ContractSearchInput.ContributionType), filters.ContributionType);
        AddDecimal(nameof(ContractSearchInput.ContributionAmountMin), filters.ContributionAmountMin);
        AddDecimal(nameof(ContractSearchInput.ContributionAmountMax), filters.ContributionAmountMax);
        AddBool(nameof(ContractSearchInput.MaintenanceIncluded), filters.MaintenanceIncluded);
        AddBool(nameof(ContractSearchInput.InsuranceIncluded), filters.InsuranceIncluded);
        AddBool(nameof(ContractSearchInput.RoadsideAssistanceIncluded), filters.RoadsideAssistanceIncluded);
        AddBool(nameof(ContractSearchInput.ReplacementCarIncluded), filters.ReplacementCarIncluded);
        AddBool(nameof(ContractSearchInput.TiresIncluded), filters.TiresIncluded);
        AddBool(nameof(ContractSearchInput.RoadTaxIncluded), filters.RoadTaxIncluded);

        if (filters.InsuranceKinds != null)
        {
            foreach (var kind in filters.InsuranceKinds)
            {
                pairs.Add(new KeyValuePair<string, string?>(
                    Prefix + nameof(ContractSearchInput.InsuranceKinds),
                    kind.ToString()));
            }
        }

        AddInt(nameof(ContractSearchInput.MaxResultCount), filters.MaxResultCount);
        AddInt(nameof(ContractSearchInput.SkipCount), skipCount);
        Add(nameof(ContractSearchInput.Sorting), filters.Sorting);

        return pairs;

        void AddGuid(string suffix, Guid? value)
        {
            if (value.HasValue)
            {
                Add(suffix, value.Value.ToString("D"));
            }
        }

        void AddInt(string suffix, int? value)
        {
            if (value.HasValue)
            {
                Add(suffix, value.Value.ToString(CultureInfo.InvariantCulture));
            }
        }

        void AddDecimal(string suffix, decimal? value)
        {
            if (value.HasValue)
            {
                Add(suffix, value.Value.ToString(CultureInfo.InvariantCulture));
            }
        }

        void AddBool(string suffix, bool? value)
        {
            if (value.HasValue)
            {
                Add(suffix, value.Value ? "true" : "false");
            }
        }

        void AddDate(string suffix, DateOnly? value)
        {
            if (value.HasValue)
            {
                Add(suffix, value.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
            }
        }

        void AddEnum<TEnum>(string suffix, TEnum? value) where TEnum : struct, Enum
        {
            if (value.HasValue)
            {
                Add(suffix, Convert.ToInt32(value.Value).ToString(CultureInfo.InvariantCulture));
            }
        }
    }
}
