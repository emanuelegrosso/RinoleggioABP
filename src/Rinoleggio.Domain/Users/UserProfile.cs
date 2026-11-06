using System;
using Volo.Abp.Domain.Entities;

namespace Rinoleggio.Users;

public class UserProfile : Entity<Guid>
{
    protected UserProfile()
    {
    }

    public UserProfile(Guid userId) : base(userId)
    {
    }

    public string FullName { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public AccountType AccountType { get; set; }

    public bool PrivacyConsent { get; set; }

    public DateTime? PrivacyConsentAt { get; set; }
}
