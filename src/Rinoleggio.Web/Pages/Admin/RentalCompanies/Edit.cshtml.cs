using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rinoleggio.Lookups;
using Rinoleggio.Permissions;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Rinoleggio.Web.Pages.Admin.RentalCompanies;

[Authorize(RinoleggioPermissions.Lookups.Edit)]
public class EditModel : AbpPageModel
{
    private readonly IRentalCompanyAppService _service;

    public EditModel(IRentalCompanyAppService service) => _service = service;

    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    [BindProperty]
    public CreateUpdateRentalCompanyDto Company { get; set; } = new();

    public async Task OnGetAsync()
    {
        var dto = await _service.GetAsync(Id);
        Company = new CreateUpdateRentalCompanyDto
        {
            Name = dto.Name,
            WebsiteUrl = dto.WebsiteUrl,
            IsActive = dto.IsActive
        };
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();
        await _service.UpdateAsync(Id, Company);
        return RedirectToPage("./Index");
    }
}
