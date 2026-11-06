using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rinoleggio.Contracts;
using Rinoleggio.Listings;

namespace Rinoleggio.Web.Pages.Contracts;

public static class ContractFormHelper
{
    public static IReadOnlyList<(FuelType Value, string Label)> FuelTypes { get; } =
    [
        (FuelType.Petrol, "Benzina"),
        (FuelType.Diesel, "Diesel"),
        (FuelType.MildHybrid, "Mild hybrid"),
        (FuelType.PlugInHybrid, "Plug-in hybrid"),
        (FuelType.Electric, "Elettrico"),
        (FuelType.LPG, "GPL"),
        (FuelType.CNG, "Metano")
    ];

    public static IReadOnlyList<(Transmission Value, string Label)> Transmissions { get; } =
    [
        (Transmission.Manual, "Manuale"),
        (Transmission.Automatic, "Automatico")
    ];

    public static IReadOnlyList<(TransferContributionType Value, string Label)> ContributionTypes { get; } =
    [
        (TransferContributionType.None, "Nessuno"),
        (TransferContributionType.Incentive, "Incentivo"),
        (TransferContributionType.Compensation, "Compensazione")
    ];

    public static IReadOnlyList<(InsuranceKind Value, string Label)> InsuranceKinds { get; } =
    [
        (InsuranceKind.RCA, "RCA"),
        (InsuranceKind.Kasko, "Kasko"),
        (InsuranceKind.FurtoIncendio, "Furto e incendio"),
        (InsuranceKind.Cristalli, "Cristalli"),
        (InsuranceKind.Infortuni, "Infortuni conducente"),
        (InsuranceKind.Altro, "Altro")
    ];

    public static string ImageUrl(string? blobPath)
    {
        return string.IsNullOrWhiteSpace(blobPath) ? string.Empty : $"/listing-images/{blobPath}";
    }

    public static string FormatMoney(decimal value) => value.ToString("N2") + " €";

    public static string GetBusinessStatusLabel(ListingBusinessStatus status) => status switch
    {
        ListingBusinessStatus.Active => "Attivo",
        ListingBusinessStatus.Reserved => "Riservato",
        ListingBusinessStatus.Closed => "Chiuso",
        ListingBusinessStatus.Suspended => "Sospeso",
        ListingBusinessStatus.Deleted => "Eliminato",
        ListingBusinessStatus.Withdrawn => "Ritirato",
        _ => status.ToString()
    };

    public static string GetListingDisplayStatus(ListingBusinessStatus status, bool isPublished)
    {
        if (status == ListingBusinessStatus.Active && !isPublished)
        {
            return "Bozza";
        }

        if (status == ListingBusinessStatus.Active && isPublished)
        {
            return "Disponibile";
        }

        return GetBusinessStatusLabel(status);
    }

    public static IEnumerable<SelectListItem> GetBusinessStatusFilterItems()
    {
        return Enum.GetValues<ListingBusinessStatus>()
            .Where(s => s != ListingBusinessStatus.Deleted)
            .Select(s => new SelectListItem(GetBusinessStatusLabel(s), ((int)s).ToString()));
    }

    public static IEnumerable<SelectListItem> GetFuelTypeItems()
    {
        return FuelTypes.Select(x => new SelectListItem(x.Label, ((int)x.Value).ToString()));
    }

    public static IEnumerable<SelectListItem> GetTransmissionItems()
    {
        return Transmissions.Select(x => new SelectListItem(x.Label, ((int)x.Value).ToString()));
    }

    public static IEnumerable<SelectListItem> GetContributionTypeItems()
    {
        return ContributionTypes.Select(x => new SelectListItem(x.Label, ((int)x.Value).ToString()));
    }

    public static string GetFuelTypeLabel(FuelType value) =>
        FuelTypes.FirstOrDefault(x => x.Value == value).Label ?? value.ToString();

    public static string GetTransmissionLabel(Transmission value) =>
        Transmissions.FirstOrDefault(x => x.Value == value).Label ?? value.ToString();

    public static string GetContributionTypeLabel(TransferContributionType value) =>
        ContributionTypes.FirstOrDefault(x => x.Value == value).Label ?? value.ToString();

    public static string GetInsuranceKindLabel(InsuranceKind value) =>
        InsuranceKinds.FirstOrDefault(x => x.Value == value).Label ?? value.ToString();

    public static string FormatInsuranceKinds(IEnumerable<InsuranceKind> kinds)
    {
        return string.Join(", ", kinds.Select(GetInsuranceKindLabel));
    }

    public static IReadOnlyList<string> GetIncludedServiceLabels(ContractSearchResultDto contract)
    {
        var labels = new List<string>();
        if (contract.MaintenanceIncluded)
        {
            labels.Add("Manutenzione");
        }

        if (contract.InsuranceIncluded)
        {
            labels.Add("Assicurazione");
        }

        if (contract.RoadsideAssistanceIncluded)
        {
            labels.Add("Assistenza stradale");
        }

        if (contract.ReplacementCarIncluded)
        {
            labels.Add("Auto sostitutiva");
        }

        if (contract.TiresIncluded)
        {
            labels.Add("Pneumatici");
        }

        if (contract.RoadTaxIncluded)
        {
            labels.Add("Bollo");
        }

        return labels;
    }

    public static bool IsEligibleForLeads(ContractSearchResultDto contract, DateOnly today)
    {
        return contract.IsPublished &&
               contract.BusinessStatus == ListingBusinessStatus.Active &&
               contract.EndDate > today;
    }
}
