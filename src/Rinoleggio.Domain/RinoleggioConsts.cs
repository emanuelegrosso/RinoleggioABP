using Volo.Abp.Identity;

namespace Rinoleggio;

public static class RinoleggioConsts
{
    public const string DbTablePrefix = "App";
    public const string? DbSchema = null;
    public const string AdminEmailDefaultValue = IdentityDataSeedContributor.AdminEmailDefaultValue;
    public const string AdminPasswordDefaultValue = "Passw0rd!";
}
