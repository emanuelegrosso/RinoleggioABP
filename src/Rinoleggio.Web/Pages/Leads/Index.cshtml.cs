using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rinoleggio.Leads;
using Rinoleggio.Permissions;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Rinoleggio.Web.Pages.Leads;

[Authorize(RinoleggioPermissions.Leads.ViewAll)]
public class IndexModel : AbpPageModel
{
    private readonly ILeadAppService _leadAppService;

    public IndexModel(ILeadAppService leadAppService)
    {
        _leadAppService = leadAppService;
    }

    public PagedResultDto<LeadDto> Results { get; private set; } = new();
    public bool CanManageLeads { get; private set; }

    public async Task OnGetAsync()
    {
        CanManageLeads = await AuthorizationService.IsGrantedAsync(RinoleggioPermissions.Leads.Manage);
        Results = await _leadAppService.GetBackOfficeListAsync(new GetLeadListInput { MaxResultCount = 100 });
    }

    public async Task<IActionResult> OnPostUpdateStatusAsync(Guid leadId, LeadStatus status, string? backofficeNotes)
    {
        await AuthorizationService.CheckAsync(RinoleggioPermissions.Leads.Manage);
        await _leadAppService.UpdateStatusAsync(leadId, new UpdateLeadStatusDto
        {
            Status = status,
            BackofficeNotes = backofficeNotes
        });
        return RedirectToPage();
    }
}
