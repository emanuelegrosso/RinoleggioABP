using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Rinoleggio.Permissions;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.Identity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.Uow;

namespace Rinoleggio.Identity;

public class RinoleggioIdentityDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    public static readonly Guid DemoDealerUserId = Guid.Parse("11111111-1111-1111-1111-111111111101");
    public static readonly Guid DemoCustomerUserId = Guid.Parse("11111111-1111-1111-1111-111111111102");
    public static readonly Guid DemoBackOfficeUserId = Guid.Parse("11111111-1111-1111-1111-111111111103");

    private readonly IdentityRoleManager _roleManager;
    private readonly IdentityUserManager _userManager;
    private readonly IPermissionDataSeeder _permissionDataSeeder;
    private readonly IGuidGenerator _guidGenerator;
    private readonly IHostEnvironment _hostEnvironment;

    public RinoleggioIdentityDataSeedContributor(
        IdentityRoleManager roleManager,
        IdentityUserManager userManager,
        IPermissionDataSeeder permissionDataSeeder,
        IGuidGenerator guidGenerator,
        IHostEnvironment hostEnvironment)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _permissionDataSeeder = permissionDataSeeder;
        _guidGenerator = guidGenerator;
        _hostEnvironment = hostEnvironment;
    }

    [UnitOfWork]
    public async Task SeedAsync(DataSeedContext context)
    {
        await CreateRoleIfNotExistsAsync(RinoleggioRoles.Dealer);
        await CreateRoleIfNotExistsAsync(RinoleggioRoles.Customer);
        await CreateRoleIfNotExistsAsync(RinoleggioRoles.BackOffice);

        await SeedRolePermissionsAsync(RinoleggioRoles.Dealer, new[]
        {
            RinoleggioPermissions.Contracts.Create,
            RinoleggioPermissions.Contracts.Edit,
            RinoleggioPermissions.Contracts.ViewOwn,
            RinoleggioPermissions.Contracts.Close,
            RinoleggioPermissions.Contracts.Delete,
            RinoleggioPermissions.Leads.Create,
            RinoleggioPermissions.Leads.ViewOwn,
            RinoleggioPermissions.Profile.Default
        });

        await SeedRolePermissionsAsync(RinoleggioRoles.Customer, new[]
        {
            RinoleggioPermissions.Contracts.Create,
            RinoleggioPermissions.Contracts.Edit,
            RinoleggioPermissions.Contracts.ViewOwn,
            RinoleggioPermissions.Contracts.Close,
            RinoleggioPermissions.Contracts.Delete,
            RinoleggioPermissions.Leads.Create,
            RinoleggioPermissions.Leads.ViewOwn,
            RinoleggioPermissions.Profile.Default
        });

        await SeedRolePermissionsAsync(RinoleggioRoles.BackOffice, new[]
        {
            RinoleggioPermissions.Lookups.Default,
            RinoleggioPermissions.Lookups.Create,
            RinoleggioPermissions.Lookups.Edit,
            RinoleggioPermissions.Lookups.Delete,
            RinoleggioPermissions.Contracts.Create,
            RinoleggioPermissions.Contracts.Edit,
            RinoleggioPermissions.Contracts.ManageAll,
            RinoleggioPermissions.Contracts.Suspend,
            RinoleggioPermissions.Contracts.Close,
            RinoleggioPermissions.Contracts.Delete,
            RinoleggioPermissions.Leads.Create,
            RinoleggioPermissions.Leads.ViewAll,
            RinoleggioPermissions.Leads.Manage,
            RinoleggioPermissions.Profile.Default
        });

        await SeedRolePermissionsAsync("admin", new[]
        {
            RinoleggioPermissions.Contracts.Restore
        });

        if (!_hostEnvironment.IsDevelopment())
        {
            return;
        }

        await CreateDemoUserAsync(DemoDealerUserId, "demo.dealer", "dealer@demo.local", RinoleggioRoles.Dealer);
        await CreateDemoUserAsync(DemoCustomerUserId, "demo.customer", "customer@demo.local", RinoleggioRoles.Customer);
        await CreateDemoUserAsync(DemoBackOfficeUserId, "demo.backoffice", "backoffice@demo.local", RinoleggioRoles.BackOffice);
    }

    private async Task CreateRoleIfNotExistsAsync(string roleName)
    {
        if (await _roleManager.FindByNameAsync(roleName) != null)
        {
            return;
        }

        await _roleManager.CreateAsync(new IdentityRole(_guidGenerator.Create(), roleName));
    }

    private async Task SeedRolePermissionsAsync(string roleName, IEnumerable<string> permissions)
    {
        await _permissionDataSeeder.SeedAsync(
            RolePermissionValueProvider.ProviderName,
            roleName,
            permissions,
            null);
    }

    private async Task CreateDemoUserAsync(Guid id, string userName, string email, string roleName)
    {
        var existing = await _userManager.FindByIdAsync(id.ToString());
        if (existing != null)
        {
            return;
        }

        var user = new IdentityUser(id, userName, email);
        var result = await _userManager.CreateAsync(user, RinoleggioConsts.AdminPasswordDefaultValue);
        if (!result.Succeeded)
        {
            return;
        }

        await _userManager.AddToRoleAsync(user, roleName);
    }
}
