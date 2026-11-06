using System.Collections.Generic;
using System.Threading.Tasks;
using Rinoleggio.Permissions;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.Identity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.Uow;

namespace Rinoleggio.Identity;

public class RinoleggioRoleDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly IdentityRoleManager _roleManager;
    private readonly IPermissionDataSeeder _permissionDataSeeder;
    private readonly IGuidGenerator _guidGenerator;

    public RinoleggioRoleDataSeedContributor(
        IdentityRoleManager roleManager,
        IPermissionDataSeeder permissionDataSeeder,
        IGuidGenerator guidGenerator)
    {
        _roleManager = roleManager;
        _permissionDataSeeder = permissionDataSeeder;
        _guidGenerator = guidGenerator;
    }

    [UnitOfWork]
    public async Task SeedAsync(DataSeedContext context)
    {
        await EnsureRoleAsync(RinoleggioRoles.Customer, isDefault: true);
        await EnsureRoleAsync(RinoleggioRoles.Dealer, isDefault: false);
        await EnsureRoleAsync(RinoleggioRoles.BackOffice, isDefault: false);

        await SeedPermissionsAsync(RinoleggioRoles.Customer, new[]
        {
            RinoleggioPermissions.Customer.ManageFavorites,
            RinoleggioPermissions.Customer.CreateLead,
            RinoleggioPermissions.Customer.ViewOwnLeads
        });

        await SeedPermissionsAsync(RinoleggioRoles.Dealer, new[]
        {
            RinoleggioPermissions.Dealer.CreateListing,
            RinoleggioPermissions.Dealer.ViewOwnListings,
            RinoleggioPermissions.Dealer.ManageOwnListings,
            RinoleggioPermissions.Dealer.ViewReceivedLeads
        });

        await SeedPermissionsAsync(RinoleggioRoles.BackOffice, new[]
        {
            RinoleggioPermissions.BackOffice.Listings,
            RinoleggioPermissions.BackOffice.ManageListings,
            RinoleggioPermissions.BackOffice.Leads,
            RinoleggioPermissions.BackOffice.ManageLeads
        });

        await SeedPermissionsAsync(RinoleggioRoles.Administrator, new[]
        {
            RinoleggioPermissions.Administrator.Default,
            RinoleggioPermissions.BackOffice.Listings,
            RinoleggioPermissions.BackOffice.ManageListings,
            RinoleggioPermissions.BackOffice.Leads,
            RinoleggioPermissions.BackOffice.ManageLeads,
            RinoleggioPermissions.Administrator.ManageCatalogs,
            RinoleggioPermissions.Administrator.RestoreListings
        });
    }

    private async Task EnsureRoleAsync(string name, bool isDefault)
    {
        var role = await _roleManager.FindByNameAsync(name);
        if (role == null)
        {
            role = new IdentityRole(_guidGenerator.Create(), name) { IsDefault = isDefault };
            var createResult = await _roleManager.CreateAsync(role);
            if (!createResult.Succeeded)
            {
                throw new Volo.Abp.AbpException($"Could not create the '{name}' role: {string.Join(", ", createResult.Errors)}");
            }

            return;
        }

        if (role.IsDefault != isDefault)
        {
            role.IsDefault = isDefault;
            var updateResult = await _roleManager.UpdateAsync(role);
            if (!updateResult.Succeeded)
            {
                throw new Volo.Abp.AbpException($"Could not update the '{name}' role: {string.Join(", ", updateResult.Errors)}");
            }
        }
    }

    private Task SeedPermissionsAsync(string roleName, IEnumerable<string> permissions)
    {
        return _permissionDataSeeder.SeedAsync(
            RolePermissionValueProvider.ProviderName,
            roleName,
            permissions,
            null);
    }
}
