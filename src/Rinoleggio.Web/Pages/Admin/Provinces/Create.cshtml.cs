using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rinoleggio.Lookups;
using Rinoleggio.Permissions;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Rinoleggio.Web.Pages.Admin.Provinces;

[Authorize(RinoleggioPermissions.Lookups.Create)]
public class CreateModel : AbpPageModel
{
    private readonly IProvinceAppService _service;

    public CreateModel(IProvinceAppService service) => _service = service;

    [BindProperty]
    public CreateUpdateProvinceDto Province { get; set; } = new() { IsActive = true };

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();
        await _service.CreateAsync(Province);
        return RedirectToPage("./Index");
    }
}
