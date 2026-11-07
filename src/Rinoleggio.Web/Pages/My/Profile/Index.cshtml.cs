using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rinoleggio.Users;
using Microsoft.AspNetCore.Identity;
using Volo.Abp.Identity;
using AbpIdentityUser = Volo.Abp.Identity.IdentityUser;

namespace Rinoleggio.Web.Pages.My.Profile;

[Authorize]
public class IndexModel : RinoleggioPageModel
{
    private readonly IUserProfileAppService _service;
    private readonly SignInManager<AbpIdentityUser> _signInManager;

    [BindProperty] public UpdateUserProfileDto Input { get; set; } = new();
    public bool Saved { get; private set; }

    public IndexModel(IUserProfileAppService service, SignInManager<AbpIdentityUser> signInManager)
    {
        _service = service;
        _signInManager = signInManager;
    }

    public async Task OnGetAsync()
    {
        var profile = await _service.GetAsync();
        Input = new UpdateUserProfileDto
        {
            Name = profile.Name,
            Surname = profile.Surname,
            PhoneNumber = profile.PhoneNumber,
            AccountType = profile.AccountType,
            PrivacyConsent = profile.PrivacyConsent
        };
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();
        await _service.UpdateAsync(Input);
        var user = await _signInManager.UserManager.GetUserAsync(User);
        if (user != null) await _signInManager.RefreshSignInAsync(user);
        Saved = true;
        return Page();
    }
}
