using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rinoleggio.Permissions;
using Rinoleggio.Users;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Rinoleggio.Web.Pages.Profile;

[Authorize(RinoleggioPermissions.Profile.Default)]
public class IndexModel : AbpPageModel
{
    private readonly IUserProfileAppService _userProfileAppService;

    public IndexModel(IUserProfileAppService userProfileAppService)
    {
        _userProfileAppService = userProfileAppService;
    }

    [BindProperty]
    public UpdateUserProfileDto Input { get; set; } = new();

    public async Task OnGetAsync()
    {
        var profile = await _userProfileAppService.GetAsync();
        Input = new UpdateUserProfileDto
        {
            FullName = profile.FullName,
            PhoneNumber = profile.PhoneNumber,
            AccountType = profile.AccountType,
            PrivacyConsent = profile.PrivacyConsent
        };
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        await _userProfileAppService.UpdateAsync(Input);
        return RedirectToPage();
    }
}
