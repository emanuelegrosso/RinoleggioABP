using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rinoleggio.Listings;
using Rinoleggio.Permissions;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Authorization.Permissions;

namespace Rinoleggio.Web.Pages.BackOffice.Listings;

[Authorize(RinoleggioPermissions.BackOffice.Listings)]
public class DetailsModel : RinoleggioPageModel
{
    private readonly IListingAppService _service;
    private readonly IPermissionChecker _permissionChecker;
    public ListingDto Listing { get; private set; } = new();
    public bool CanManage { get; private set; }

    public DetailsModel(IListingAppService service, IPermissionChecker permissionChecker)
    {
        _service = service;
        _permissionChecker = permissionChecker;
    }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        Listing = await _service.GetAsync(id);
        CanManage = await _permissionChecker.IsGrantedAsync(RinoleggioPermissions.BackOffice.ManageListings);
        return Page();
    }

    public async Task<IActionResult> OnPostSetStatusAsync(Guid id, ListingStatus status, string? reason)
    {
        try
        {
            await _service.SetBackOfficeStatusAsync(id, new ListingStatusDto { Status = status, Reason = reason });
            return RedirectToPage(new { id });
        }
        catch (Volo.Abp.BusinessException)
        {
            ModelState.AddModelError(string.Empty, "Stato o motivo non valido. Controlla i dati e riprova.");
            Listing = await _service.GetAsync(id);
            CanManage = await _permissionChecker.IsGrantedAsync(RinoleggioPermissions.BackOffice.ManageListings);
            return Page();
        }
    }
}
