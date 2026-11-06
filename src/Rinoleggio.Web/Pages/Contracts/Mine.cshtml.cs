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

[Authorize(RinoleggioPermissions.Contracts.ViewOwn)]
public class MineModel : AbpPageModel
{
    private readonly IContractAppService _contractAppService;

    public MineModel(IContractAppService contractAppService)
    {
        _contractAppService = contractAppService;
    }

    [BindProperty(SupportsGet = true)]
    public ContractSearchInput Filters { get; set; } = new() { MaxResultCount = 20 };

    public PagedResultDto<ContractSearchResultDto> Results { get; private set; } = new();
    public bool CanCreateListing { get; private set; }
    public bool CanEditListing { get; private set; }
    public bool CanClose { get; private set; }
    public bool CanDelete { get; private set; }

    [TempData]
    public string? ActionFlash { get; set; }

    public async Task OnGetAsync()
    {
        if (Filters.MaxResultCount <= 0)
        {
            Filters.MaxResultCount = 20;
        }

        Results = await _contractAppService.GetMyListAsync(Filters);
        var permissionChecker = LazyServiceProvider.LazyGetRequiredService<IPermissionChecker>();
        CanCreateListing = await permissionChecker.IsGrantedAsync(RinoleggioPermissions.Contracts.Create);
        CanEditListing = await permissionChecker.IsGrantedAsync(RinoleggioPermissions.Contracts.Edit);
        CanClose = await permissionChecker.IsGrantedAsync(RinoleggioPermissions.Contracts.Close);
        CanDelete = await permissionChecker.IsGrantedAsync(RinoleggioPermissions.Contracts.Delete);
    }

    public async Task<IActionResult> OnPostWithdrawAsync(Guid id, string reason)
    {
        await _contractAppService.WithdrawAsync(id, new WithdrawContractDto { Reason = reason });
        ActionFlash = "withdrawn";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostRepublishAsync(Guid id)
    {
        await _contractAppService.RepublishAsync(id);
        ActionFlash = "republished";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostReserveAsync(Guid id)
    {
        await _contractAppService.ReserveAsync(id);
        ActionFlash = "reserved";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostClearReservationAsync(Guid id)
    {
        await _contractAppService.ClearListingReservationAsync(id);
        ActionFlash = "available";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostCloseAsync(Guid id, string? reason)
    {
        await _contractAppService.CloseAsync(id, new CloseContractDto { Reason = reason });
        ActionFlash = "closed";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(Guid id, string reason)
    {
        await _contractAppService.DeleteAsync(id, new DeleteContractDto { Reason = reason });
        ActionFlash = "deleted";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostPublishAsync(Guid id)
    {
        await _contractAppService.PublishListingAsync(id);
        ActionFlash = "published";
        return RedirectToPage();
    }
}
