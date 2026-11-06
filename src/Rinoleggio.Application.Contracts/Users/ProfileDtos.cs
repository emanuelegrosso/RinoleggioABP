using System.ComponentModel.DataAnnotations;

namespace Rinoleggio.Users;

public class UserProfileDto
{
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string AccountType { get; set; } = "Customer";
    public bool PrivacyConsent { get; set; }
}

public class UpdateUserProfileDto
{
    [Required, StringLength(64)] public string Name { get; set; } = string.Empty;
    [Required, StringLength(64)] public string Surname { get; set; } = string.Empty;
    [StringLength(30)] public string? PhoneNumber { get; set; }
    [Required, RegularExpression("^(Customer|Dealer)$")] public string AccountType { get; set; } = "Customer";
    public bool PrivacyConsent { get; set; }
}
