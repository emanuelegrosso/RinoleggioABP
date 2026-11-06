using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rinoleggio.Leads;
using Rinoleggio.Permissions;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Rinoleggio.Web.Pages.Leads;

[Authorize(RinoleggioPermissions.Leads.ViewOwn)]
public class ReceivedModel : AbpPageModel
{
    private readonly ILeadAppService _leadAppService;

    public ReceivedModel(ILeadAppService leadAppService)
    {
        _leadAppService = leadAppService;
    }

    [BindProperty(SupportsGet = true)]
    public Guid? ListingId { get; set; }

    public PagedResultDto<LeadDto> Results { get; private set; } = new();

    public async Task OnGetAsync()
    {
        Results = await _leadAppService.GetReceivedAsync(new GetLeadListInput
        {
            MaxResultCount = 50,
            ListingId = ListingId
        });
    }
}
