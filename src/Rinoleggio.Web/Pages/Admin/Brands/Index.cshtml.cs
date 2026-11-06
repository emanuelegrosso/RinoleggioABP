using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rinoleggio.Lookups;
using Rinoleggio.Permissions;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Rinoleggio.Web.Pages.Admin.Brands;

[Authorize(RinoleggioPermissions.Lookups.Default)]
public class IndexModel : AbpPageModel
{
    private readonly IBrandAppService _service;
    public IndexModel(IBrandAppService service) => _service = service;
    public PagedResultDto<BrandDto> Items { get; private set; } = new();
    public async Task OnGetAsync() => Items = await _service.GetListAsync(new PagedAndSortedResultRequestDto { MaxResultCount = 200 });

    public async Task<IActionResult> OnPostDeleteAsync(Guid id)
    {
        await AuthorizationService.CheckAsync(RinoleggioPermissions.Lookups.Delete);
        await _service.DeleteAsync(id);
        return RedirectToPage();
    }
}
