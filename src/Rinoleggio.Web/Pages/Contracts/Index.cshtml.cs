using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Rinoleggio.Contracts;
using Rinoleggio.Permissions;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;
using Volo.Abp.Authorization.Permissions;

namespace Rinoleggio.Web.Pages.Contracts;

public class IndexModel : AbpPageModel
{
    private readonly IContractAppService _contractAppService;

    public IndexModel(IContractAppService contractAppService)
    {
        _contractAppService = contractAppService;
    }

    [BindProperty(SupportsGet = true)]
    public ContractSearchInput Filters { get; set; } = new() { MaxResultCount = 12 };

    public PagedResultDto<ContractSearchResultDto> Results { get; private set; } = new();
    public ContractLookupsDto Lookups { get; private set; } = new();
    public bool CanCreateListing { get; private set; }

    public async Task OnGetAsync()
    {
        Filters.InsuranceKinds ??= [];

        if (Filters.MaxResultCount <= 0)
        {
            Filters.MaxResultCount = 12;
        }

        Lookups = await _contractAppService.GetLookupsAsync();
        Results = await _contractAppService.SearchAsync(Filters);
        var permissionChecker = LazyServiceProvider.LazyGetRequiredService<IPermissionChecker>();
        CanCreateListing = await permissionChecker.IsGrantedAsync(RinoleggioPermissions.Contracts.Create);
    }

    public IActionResult OnPostReset()
    {
        return RedirectToPage();
    }

    public string BuildPageUrl(int pageNumber)
    {
        var skip = (pageNumber - 1) * Filters.MaxResultCount;
        return ContractSearchQueryHelper.BuildPageUrl(Url.Page(null)!, Filters, skip);
    }
}
