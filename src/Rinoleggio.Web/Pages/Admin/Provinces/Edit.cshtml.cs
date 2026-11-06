using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rinoleggio.Lookups;
using Rinoleggio.Permissions;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Rinoleggio.Web.Pages.Admin.Provinces;

[Authorize(RinoleggioPermissions.Lookups.Edit)]
public class EditModel : AbpPageModel
{
    private readonly IProvinceAppService _service;

    public EditModel(IProvinceAppService service) => _service = service;

    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    [BindProperty]
    public CreateUpdateProvinceDto Province { get; set; } = new();

    public async Task OnGetAsync()
    {
        var dto = await _service.GetAsync(Id);
        Province = new CreateUpdateProvinceDto
        {
            Name = dto.Name,
            Code = dto.Code,
            SortOrder = dto.SortOrder,
            IsActive = dto.IsActive
        };
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();
        await _service.UpdateAsync(Id, Province);
        return RedirectToPage("./Index");
    }
}
