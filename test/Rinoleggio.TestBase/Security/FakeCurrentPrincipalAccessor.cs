using System;
using System.Collections.Generic;
using System.Security.Claims;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Security.Claims;

namespace Rinoleggio.Security;

[Dependency(ReplaceServices = true)]
public class FakeCurrentPrincipalAccessor : ThreadCurrentPrincipalAccessor
{
    /// <summary>When set, integration tests run app services as this user id.</summary>
    public static Guid? OverrideUserId { get; set; }

    protected override ClaimsPrincipal GetClaimsPrincipal()
    {
        return GetPrincipal();
    }

    private ClaimsPrincipal GetPrincipal()
    {
        var userId = (OverrideUserId ?? Guid.Parse("2e701e62-0953-4dd3-910b-dc6cc93ccb0d")).ToString();
        return new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
        {
            new Claim(AbpClaimTypes.UserId, userId),
            new Claim(AbpClaimTypes.UserName, "test-user"),
            new Claim(AbpClaimTypes.Email, "test@abp.io")
        }, authenticationType: "Fake"));
    }
}
