using System;
using System.Linq;
using System.Threading.Tasks;
using Rinoleggio.Identity;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Identity;
using Volo.Abp.Users;

namespace Rinoleggio.Users;

public class UserProfileAppService : ApplicationService, IUserProfileAppService
{
    private readonly IdentityUserManager _userManager;

    public UserProfileAppService(IdentityUserManager userManager) => _userManager = userManager;

    public async Task<UserProfileDto> GetAsync()
    {
        var user = await _userManager.GetByIdAsync(CurrentUser.GetId());
        var roles = await _userManager.GetRolesAsync(user);
        return new UserProfileDto
        {
            Name = user.Name ?? string.Empty,
            Surname = user.Surname ?? string.Empty,
            PhoneNumber = user.PhoneNumber,
            AccountType = roles.Contains(RinoleggioRoles.Dealer) ? RinoleggioRoles.Dealer : RinoleggioRoles.Customer,
            PrivacyConsent = user.ExtraProperties.TryGetValue("PrivacyConsent", out var consent) &&
                             bool.TryParse(consent?.ToString(), out var accepted) && accepted
        };
    }

    public async Task UpdateAsync(UpdateUserProfileDto input)
    {
        var user = await _userManager.GetByIdAsync(CurrentUser.GetId());
        var existingRoles = await _userManager.GetRolesAsync(user);
        if (existingRoles.Contains(RinoleggioRoles.BackOffice) || existingRoles.Contains("Administrator"))
            throw new BusinessException("Rinoleggio:AccountTypeChangeNotAllowed");
        if (!existingRoles.Contains(RinoleggioRoles.Customer) && !existingRoles.Contains(RinoleggioRoles.Dealer))
            throw new BusinessException("Rinoleggio:AccountTypeChangeNotAllowed");

        user.Name = input.Name.Trim();
        user.Surname = input.Surname.Trim();
        user.ExtraProperties["PrivacyConsent"] = input.PrivacyConsent.ToString();
        user.ExtraProperties["PrivacyConsentUpdatedAt"] = Clock.Now.ToUniversalTime().ToString("O");
        CheckIdentityResult(await _userManager.UpdateAsync(user));
        CheckIdentityResult(await _userManager.SetPhoneNumberAsync(user, input.PhoneNumber?.Trim()));

        var currentAccountRole = existingRoles.Contains(RinoleggioRoles.Dealer) ? RinoleggioRoles.Dealer : RinoleggioRoles.Customer;
        if (!string.Equals(currentAccountRole, input.AccountType, StringComparison.Ordinal))
        {
            CheckIdentityResult(await _userManager.RemoveFromRoleAsync(user, currentAccountRole));
            CheckIdentityResult(await _userManager.AddToRoleAsync(user, input.AccountType));
        }
    }

    private static void CheckIdentityResult(Microsoft.AspNetCore.Identity.IdentityResult result)
    {
        if (!result.Succeeded)
            throw new BusinessException("Rinoleggio:ProfileUpdateFailed")
                .WithData("Errors", string.Join("; ", result.Errors.Select(x => x.Description)));
    }
}
