using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rinoleggio.Listings;
using Rinoleggio.Permissions;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Authorization.Permissions;

namespace Rinoleggio.Web.Pages.BackOffice.Leads;

[Authorize(RinoleggioPermissions.BackOffice.Leads)]
public class IndexModel : RinoleggioPageModel
{
    private readonly IListingAppService _service;
    private readonly IPermissionChecker _permissionChecker;

    public PagedResultDto<LeadDto> Requests { get; private set; } = new();
    public bool CanManage { get; private set; }
    [BindProperty(SupportsGet = true)] public int SkipCount { get; set; }
    public const int PageSize = 20;
    public int PageNumber => SkipCount / PageSize + 1;
    public int PageCount => Math.Max(1, (int)Math.Ceiling((double)Requests.TotalCount / PageSize));
    public int PreviousSkipCount => Math.Max(0, SkipCount - PageSize);
    public int NextSkipCount => SkipCount + PageSize;
    public bool HasNextPage => NextSkipCount < Requests.TotalCount;
    [BindProperty(SupportsGet = true)] public LeadStatus? Status { get; set; }

    [BindProperty]
    public UpdateLeadDto Input { get; set; } = new();

    public IndexModel(IListingAppService service, IPermissionChecker permissionChecker)
    {
        _service = service;
        _permissionChecker = permissionChecker;
    }

    public async Task OnGetAsync()
    {
        SkipCount = Math.Max(0, SkipCount);
        Requests = await _service.GetBackOfficeLeadsAsync(new BackOfficeLeadSearchInput { SkipCount = SkipCount, MaxResultCount = PageSize, Status = Status });
        if (Requests.TotalCount > 0 && SkipCount >= Requests.TotalCount)
        {
            SkipCount = (int)((Requests.TotalCount - 1) / PageSize) * PageSize;
            Requests = await _service.GetBackOfficeLeadsAsync(new BackOfficeLeadSearchInput { SkipCount = SkipCount, MaxResultCount = PageSize, Status = Status });
        }
        CanManage = await _permissionChecker.IsGrantedAsync(RinoleggioPermissions.BackOffice.ManageLeads);
    }

    public async Task<IActionResult> OnPostUpdateAsync(Guid id)
    {
        try { await _service.UpdateLeadAsync(id, Input); }
        catch (Volo.Abp.BusinessException)
        {
            TempData["UpdateError"] = "Aggiornamento non salvato. Per chiudere la richiesta seleziona un esito valido e verifica lo stato dell’annuncio.";
        }
        return RedirectToPage(new { skipCount = Math.Max(0, SkipCount), status = Status });
    }
}
