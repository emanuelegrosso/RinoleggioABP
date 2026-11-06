using System.ComponentModel.DataAnnotations;
using Rinoleggio.Users;

namespace Rinoleggio.Accounts;

public class RegisterRinoleggioUserInput
{
    [Required]
    [StringLength(256)]
    public string UserName { get; set; } = null!;

    [Required]
    [EmailAddress]
    [StringLength(256)]
    public string EmailAddress { get; set; } = null!;

    [Required]
    [StringLength(128)]
    public string Password { get; set; } = null!;

    [Required]
    [StringLength(UserProfileConsts.MaxFullNameLength)]
    public string FullName { get; set; } = null!;

    [Required]
    public string AccountRole { get; set; } = null!;

    public bool PrivacyConsent { get; set; }
}
