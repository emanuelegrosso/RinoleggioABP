using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rinoleggio.Lookups;
using Rinoleggio.Permissions;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Rinoleggio.Web.Pages.Admin.Brands;

[Authorize(RinoleggioPermissions.Lookups.Edit)]
public class EditModel : AbpPageModel
{
    private readonly IBrandAppService _service;

    public EditModel(IBrandAppService service)
    {
        _service = service;
    }

    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    [BindProperty]
    public CreateUpdateBrandDto Brand { get; set; } = new();

    public async Task OnGetAsync()
    {
        var dto = await _service.GetAsync(Id);
        Brand = new CreateUpdateBrandDto
        {
            Name = dto.Name,
            SortOrder = dto.SortOrder,
            IsActive = dto.IsActive
        };
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        await _service.UpdateAsync(Id, Brand);
        return RedirectToPage("./Index");
    }
}
