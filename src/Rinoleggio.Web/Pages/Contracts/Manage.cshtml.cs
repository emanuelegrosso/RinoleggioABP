using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rinoleggio.Contracts;
using Rinoleggio.Permissions;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;
using Volo.Abp.Authorization.Permissions;

namespace Rinoleggio.Web.Pages.Contracts;

[Authorize(RinoleggioPermissions.Contracts.ManageAll)]
public class ManageModel : AbpPageModel
{
    private readonly IContractAppService _contractAppService;

    public ManageModel(IContractAppService contractAppService)
    {
        _contractAppService = contractAppService;
    }

    [BindProperty(SupportsGet = true)]
    public ContractSearchInput Filters { get; set; } = new() { MaxResultCount = 20 };

    public PagedResultDto<ContractSearchResultDto> Results { get; private set; } = new();
    public ContractLookupsDto Lookups { get; private set; } = new();
    public bool CanRestore { get; private set; }

    public async Task OnGetAsync()
    {
        Lookups = await _contractAppService.GetLookupsAsync();
        CanRestore = await AuthorizationService.IsGrantedAsync(RinoleggioPermissions.Contracts.Restore);
        Results = await _contractAppService.GetManageListAsync(Filters);
    }

    public async Task<IActionResult> OnPostSuspendAsync(Guid id, string reason)
    {
        await _contractAppService.SuspendAsync(id, new SuspendContractDto { Reason = reason });
        return RedirectToPage(new { Filters.BusinessStatus, Filters.IncludeDeleted, Filters.HasLeads });
    }

    public async Task<IActionResult> OnPostReactivateAsync(Guid id)
    {
        await _contractAppService.ReactivateAsync(id);
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostWithdrawAsync(Guid id, string reason)
    {
        await _contractAppService.WithdrawAsync(id, new WithdrawContractDto { Reason = reason });
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostReserveAsync(Guid id)
    {
        await _contractAppService.ReserveAsync(id);
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostCloseAsync(Guid id)
    {
        await _contractAppService.CloseAsync(id, new CloseContractDto());
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(Guid id, string reason)
    {
        await _contractAppService.DeleteAsync(id, new DeleteContractDto { Reason = reason });
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostRestoreAsync(Guid id)
    {
        await _contractAppService.RestoreAsync(id);
        return RedirectToPage(new { Filters = new ContractSearchInput { IncludeDeleted = true } });
    }
}
