using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Rinoleggio.Permissions;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Timing;

namespace Rinoleggio.Users;

[Authorize(RinoleggioPermissions.Profile.Default)]
public class UserProfileAppService : RinoleggioAppService, IUserProfileAppService
{
    private readonly IRepository<UserProfile, Guid> _userProfileRepository;
    private readonly IClock _clock;

    public UserProfileAppService(
        IRepository<UserProfile, Guid> userProfileRepository,
        IClock clock)
    {
        _userProfileRepository = userProfileRepository;
        _clock = clock;
    }

    public async Task<UserProfileDto> GetAsync()
    {
        var profile = await GetOrCreateProfileAsync();
        return Map(profile);
    }

    public async Task<UserProfileDto> UpdateAsync(UpdateUserProfileDto input)
    {
        if (!input.PrivacyConsent)
        {
            throw new BusinessException("Rinoleggio:PrivacyConsentRequired");
        }

        var profile = await GetOrCreateProfileAsync();
        profile.FullName = input.FullName;
        profile.PhoneNumber = input.PhoneNumber;
        profile.AccountType = input.AccountType;
        profile.PrivacyConsent = true;
        profile.PrivacyConsentAt = _clock.Now;
        await _userProfileRepository.UpdateAsync(profile, autoSave: true);
        return Map(profile);
    }

    private async Task<UserProfile> GetOrCreateProfileAsync()
    {
        var userId = CurrentUser.Id!.Value;
        var profile = await _userProfileRepository.FindAsync(userId);
        if (profile != null)
        {
            return profile;
        }

        profile = new UserProfile(userId)
        {
            FullName = CurrentUser.UserName ?? "Utente",
            AccountType = AccountType.Private,
            PrivacyConsent = false
        };
        await _userProfileRepository.InsertAsync(profile, autoSave: true);
        return profile;
    }

    private static UserProfileDto Map(UserProfile profile)
    {
        return new UserProfileDto
        {
            FullName = profile.FullName,
            PhoneNumber = profile.PhoneNumber,
            AccountType = profile.AccountType,
            PrivacyConsent = profile.PrivacyConsent
        };
    }
}
