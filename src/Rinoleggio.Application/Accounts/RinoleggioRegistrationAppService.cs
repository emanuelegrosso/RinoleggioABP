using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Rinoleggio.Users;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.Timing;

namespace Rinoleggio.Accounts;

public class RinoleggioRegistrationAppService : ApplicationService, IRinoleggioRegistrationAppService
{
    private readonly IdentityUserManager _userManager;
    private readonly IIdentityRoleRepository _roleRepository;
    private readonly IRepository<UserProfile, Guid> _userProfileRepository;
    private readonly IClock _clock;

    public RinoleggioRegistrationAppService(
        IdentityUserManager userManager,
        IIdentityRoleRepository roleRepository,
        IRepository<UserProfile, Guid> userProfileRepository,
        IClock clock)
    {
        _userManager = userManager;
        _roleRepository = roleRepository;
        _userProfileRepository = userProfileRepository;
        _clock = clock;
    }

    [AllowAnonymous]
    public async Task RegisterAsync(RegisterRinoleggioUserInput input)
    {
        if (!input.PrivacyConsent)
        {
            throw new BusinessException("Rinoleggio:PrivacyConsentRequired");
        }

        if (input.AccountRole != RinoleggioRoles.Dealer && input.AccountRole != RinoleggioRoles.Customer)
        {
            throw new BusinessException("Rinoleggio:InvalidRegistrationRole");
        }

        var user = new IdentityUser(GuidGenerator.Create(), input.UserName, input.EmailAddress);
        var result = await _userManager.CreateAsync(user, input.Password);
        if (!result.Succeeded)
        {
            throw new BusinessException("Rinoleggio:RegistrationFailed")
                .WithData("Errors", string.Join("; ", result.Errors));
        }

        var role = await _roleRepository.FindByNormalizedNameAsync(input.AccountRole.ToUpperInvariant());
        if (role != null)
        {
            await _userManager.AddToRoleAsync(user, role.Name);
        }

        var accountType = input.AccountRole == RinoleggioRoles.Dealer
            ? AccountType.Business
            : AccountType.Private;

        await _userProfileRepository.InsertAsync(new UserProfile(user.Id)
        {
            FullName = input.FullName,
            AccountType = accountType,
            PrivacyConsent = true,
            PrivacyConsentAt = _clock.Now
        }, autoSave: true);
    }
}
