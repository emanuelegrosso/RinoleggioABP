using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Rinoleggio.Listings;
using Rinoleggio.Permissions;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Authorization.Permissions;

namespace Rinoleggio.Web.Pages.Listings;

public class IndexModel : RinoleggioPageModel
{
    private readonly IListingAppService _service;
    private readonly IPermissionChecker _permissionChecker;
    public PagedResultDto<ListingDto> Results { get; private set; } = new();
    public bool CanCreateListing { get; private set; }
    public bool CanManageFavorites { get; private set; }
    public IReadOnlyList<string> Provinces { get; private set; } = Array.Empty<string>();
    public IReadOnlyList<string> Brands { get; private set; } = Array.Empty<string>();
    public IReadOnlyList<string> RentalCompanies { get; private set; } = Array.Empty<string>();
    [BindProperty(SupportsGet = true)] public ListingSearchInput Filter { get; set; } = new() { MaxResultCount = 12 };
    public IndexModel(IListingAppService service, IPermissionChecker permissionChecker)
    {
        _service = service;
        _permissionChecker = permissionChecker;
    }

    public async Task OnGetAsync()
    {
        Filter.SkipCount = Math.Max(0, Filter.SkipCount);
        Filter.MaxResultCount = Math.Clamp(Filter.MaxResultCount, 1, 100);
        Results = await _service.SearchAsync(Filter);
        CanCreateListing = await _permissionChecker.IsGrantedAsync(RinoleggioPermissions.Dealer.CreateListing);
        CanManageFavorites = await _permissionChecker.IsGrantedAsync(RinoleggioPermissions.Customer.ManageFavorites);
        Provinces = await _service.GetLookupValuesAsync("province");
        Brands = await _service.GetLookupValuesAsync("brand");
        RentalCompanies = await _service.GetLookupValuesAsync("rentalcompany");
    }

    public string GetPageUrl(int skipCount)
    {
        var query = new Dictionary<string, string?>
        {
            ["Filter.SkipCount"] = skipCount.ToString(CultureInfo.InvariantCulture),
            ["Filter.MaxResultCount"] = Filter.MaxResultCount.ToString(CultureInfo.InvariantCulture),
            ["Filter.Sorting"] = Filter.Sorting,
            ["Filter.Query"] = Filter.Query,
            ["Filter.Province"] = Filter.Province,
            ["Filter.RentalCompany"] = Filter.RentalCompany,
            ["Filter.Brand"] = Filter.Brand,
            ["Filter.Model"] = Filter.Model,
            ["Filter.FuelType"] = Filter.FuelType?.ToString(),
            ["Filter.Transmission"] = Filter.Transmission?.ToString(),
            ["Filter.YearFrom"] = Filter.YearFrom?.ToString(CultureInfo.InvariantCulture),
            ["Filter.YearTo"] = Filter.YearTo?.ToString(CultureInfo.InvariantCulture),
            ["Filter.MonthlyFeeFrom"] = Filter.MonthlyFeeFrom?.ToString(CultureInfo.InvariantCulture),
            ["Filter.MonthlyFeeTo"] = Filter.MonthlyFeeTo?.ToString(CultureInfo.InvariantCulture),
            ["Filter.RemainingKmFrom"] = Filter.RemainingKmFrom?.ToString(CultureInfo.InvariantCulture),
            ["Filter.RemainingKmTo"] = Filter.RemainingKmTo?.ToString(CultureInfo.InvariantCulture),
            ["Filter.EndDateFrom"] = Filter.EndDateFrom?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            ["Filter.EndDateTo"] = Filter.EndDateTo?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            ["Filter.VatIncluded"] = Filter.VatIncluded?.ToString(),
            ["Filter.ContributionType"] = Filter.ContributionType?.ToString()
        };
        return QueryHelpers.AddQueryString("/Listings", query);
    }

    public async Task<IActionResult> OnPostAddFavoriteAsync(Guid listingId)
    {
        await _service.AddFavoriteAsync(listingId);
        return RedirectToPage("/Listings/Index", new { Filter = Filter });
    }
}
