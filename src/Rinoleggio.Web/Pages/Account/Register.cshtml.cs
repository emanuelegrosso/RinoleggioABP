using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rinoleggio.Accounts;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Rinoleggio.Web.Pages.Account;

[AllowAnonymous]
public class RegisterModel : AbpPageModel
{
    private readonly IRinoleggioRegistrationAppService _registrationAppService;

    public RegisterModel(IRinoleggioRegistrationAppService registrationAppService)
    {
        _registrationAppService = registrationAppService;
    }

    [BindProperty(SupportsGet = true)]
    public string? ReturnUrl { get; set; }

    [BindProperty]
    public RegisterRinoleggioUserInput Input { get; set; } = new();

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        await _registrationAppService.RegisterAsync(Input);
        return RedirectToPage("/Account/Login", new { returnUrl = ReturnUrl });
    }
}
