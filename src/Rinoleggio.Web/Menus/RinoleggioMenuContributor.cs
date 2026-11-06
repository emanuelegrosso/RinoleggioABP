using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Rinoleggio.Localization;
using Rinoleggio.Permissions;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Identity.Web.Navigation;
using Volo.Abp.SettingManagement.Web.Navigation;
using Volo.Abp.UI.Navigation;
using Volo.Abp.Users;

namespace Rinoleggio.Web.Menus;

public class RinoleggioMenuContributor : IMenuContributor
{
    public async Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name == StandardMenus.Main)
        {
            await ConfigureMainMenuAsync(context);
        }
    }

    private static Task ConfigureMainMenuAsync(MenuConfigurationContext context)
    {
        if (!context.ServiceProvider.GetRequiredService<ICurrentUser>().IsAuthenticated)
        {
            // Other modules (e.g. Identity, SettingManagement) add Administration menus before this runs — clear for guests.
            context.Menu.Items.Clear();
            return Task.CompletedTask;
        }

        var l = context.GetLocalizer<RinoleggioResource>();
        context.Menu.AddItem(
            new ApplicationMenuItem(
                RinoleggioMenus.Home,
                l["Menu:Home"],
                "~/",
                icon: "fa fa-home",
                order: 1
            )
        );

        var administration = context.Menu.GetAdministration();
        administration.Order = 10;
        administration.SetSubItemOrder(IdentityMenuNames.GroupName, 1);
        administration.SetSubItemOrder(SettingManagementMenuNames.GroupName, 8);

        context.Menu.AddItem(
            new ApplicationMenuItem(
                RinoleggioMenus.Contracts,
                "Ricerca contratti",
                url: "/Contracts",
                icon: "fa fa-search",
                order: 2
            )
        );

        context.Menu.AddItem(
            new ApplicationMenuItem(
                "ComeFunziona",
                "Come funziona",
                url: "/ComeFunziona",
                icon: "fa fa-info-circle",
                order: 2
            )
        );

        context.Menu.AddItem(
            new ApplicationMenuItem(
                RinoleggioMenus.ContractsMine,
                "I miei annunci",
                url: "/Contracts/Mine",
                icon: "fa fa-list",
                order: 3
            ).RequirePermissions(RinoleggioPermissions.Contracts.ViewOwn)
        );

        context.Menu.AddItem(
            new ApplicationMenuItem(
                "LeadsSent",
                "Le mie richieste",
                url: "/Leads/Sent",
                icon: "fa fa-paper-plane",
                order: 4
            ).RequirePermissions(RinoleggioPermissions.Leads.ViewOwn)
        );

        context.Menu.AddItem(
            new ApplicationMenuItem(
                "LeadsReceived",
                "Richieste ricevute",
                url: "/Leads/Received",
                icon: "fa fa-inbox",
                order: 5
            ).RequirePermissions(RinoleggioPermissions.Leads.ViewOwn)
        );

        context.Menu.AddItem(
            new ApplicationMenuItem(
                "Profile",
                "Profilo",
                url: "/Profile",
                icon: "fa fa-user",
                order: 6
            ).RequirePermissions(RinoleggioPermissions.Profile.Default)
        );

        context.Menu.AddItem(
            new ApplicationMenuItem(
                RinoleggioMenus.AdminLookups,
                "Anagrafiche",
                icon: "fa fa-database",
                order: 7
            ).RequirePermissions(RinoleggioPermissions.Lookups.Default)
            .AddItem(new ApplicationMenuItem("Admin.RentalCompanies", "Società noleggio", url: "/Admin/RentalCompanies"))
            .AddItem(new ApplicationMenuItem("Admin.Brands", "Marche", url: "/Admin/Brands"))
            .AddItem(new ApplicationMenuItem("Admin.Provinces", "Province", url: "/Admin/Provinces"))
        );

        context.Menu.AddItem(
            new ApplicationMenuItem(
                "Admin.Contracts",
                "Gestione annunci",
                url: "/Contracts/Manage",
                icon: "fa fa-car",
                order: 8
            ).RequirePermissions(RinoleggioPermissions.Contracts.ManageAll)
        );

        context.Menu.AddItem(
            new ApplicationMenuItem(
                "Admin.Leads",
                "Lead backoffice",
                url: "/Leads",
                icon: "fa fa-inbox",
                order: 9
            ).RequirePermissions(RinoleggioPermissions.Leads.ViewAll)
        );

        return Task.CompletedTask;
    }
}
