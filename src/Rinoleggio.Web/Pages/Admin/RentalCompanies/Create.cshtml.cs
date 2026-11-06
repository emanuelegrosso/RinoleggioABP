using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rinoleggio.Lookups;
using Rinoleggio.Permissions;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Rinoleggio.Web.Pages.Admin.RentalCompanies;

[Authorize(RinoleggioPermissions.Lookups.Create)]
public class CreateModel : AbpPageModel
{
    private readonly IRentalCompanyAppService _service;

    public CreateModel(IRentalCompanyAppService service) => _service = service;

    [BindProperty]
    public CreateUpdateRentalCompanyDto Company { get; set; } = new() { IsActive = true };

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();
        await _service.CreateAsync(Company);
        return RedirectToPage("./Index");
    }
}
