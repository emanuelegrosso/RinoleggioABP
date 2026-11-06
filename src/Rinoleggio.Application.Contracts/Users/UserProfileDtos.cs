using System.ComponentModel.DataAnnotations;

namespace Rinoleggio.Users;

public class UserProfileDto
{
    public string FullName { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public AccountType AccountType { get; set; }
    public bool PrivacyConsent { get; set; }
}

public class UpdateUserProfileDto
{
    [Required]
    [StringLength(UserProfileConsts.MaxFullNameLength)]
    public string FullName { get; set; } = null!;

    [StringLength(UserProfileConsts.MaxPhoneNumberLength)]
    public string? PhoneNumber { get; set; }

    public AccountType AccountType { get; set; }

    public bool PrivacyConsent { get; set; }
}
