using System.Threading.Tasks;
using Rinoleggio.Localization;
using Rinoleggio.Permissions;
using Rinoleggio.MultiTenancy;
using Volo.Abp.SettingManagement.Web.Navigation;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Identity.Web.Navigation;
using Volo.Abp.UI.Navigation;
using Volo.Abp.TenantManagement.Web.Navigation;
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
        var l = context.GetLocalizer<RinoleggioResource>();
        //Home
        context.Menu.AddItem(
            new ApplicationMenuItem(
                RinoleggioMenus.Home,
                l["Menu:Home"],
                "~/",
                icon: "fa fa-home",
                order: 1
            )
        );

        context.Menu.AddItem(new ApplicationMenuItem(
            RinoleggioMenus.MyProfile,
            "Il mio profilo",
            url: "/My/Profile",
            icon: "fa fa-user",
            order: 2).RequireAuthenticated());

        context.Menu.AddItem(new ApplicationMenuItem(
            RinoleggioMenus.Listings,
            "Cerca contratti",
            url: "/Listings",
            icon: "fa fa-search",
            order: 3));

        context.Menu.AddItem(new ApplicationMenuItem(
            RinoleggioMenus.HowItWorks,
            "Come funziona",
            url: "/ComeFunziona",
            icon: "fa fa-circle-info",
            order: 4));

        context.Menu.AddItem(new ApplicationMenuItem(
            RinoleggioMenus.Privacy,
            "Privacy",
            url: "/Privacy",
            icon: "fa fa-shield-halved",
            order: 5));

        context.Menu.AddItem(new ApplicationMenuItem(
            RinoleggioMenus.MyFavorites,
            "I miei preferiti",
            url: "/My/Favorites",
            icon: "fa fa-heart",
            order: 6).RequirePermissions(RinoleggioPermissions.Customer.ManageFavorites));

        context.Menu.AddItem(new ApplicationMenuItem(
            RinoleggioMenus.MyRequests,
            "Le mie richieste",
            url: "/My/Requests",
            icon: "fa fa-inbox",
            order: 7).RequirePermissions(RinoleggioPermissions.Customer.ViewOwnLeads));

        context.Menu.AddItem(new ApplicationMenuItem(
            RinoleggioMenus.MyListings,
            "I miei annunci",
            url: "/My/Listings",
            icon: "fa fa-car",
            order: 8).RequirePermissions(RinoleggioPermissions.Dealer.ViewOwnListings));

        context.Menu.AddItem(new ApplicationMenuItem(
            RinoleggioMenus.CreateListing,
            "Pubblica un’offerta",
            url: "/My/Listings/Create",
            icon: "fa fa-plus",
            order: 9).RequirePermissions(RinoleggioPermissions.Dealer.CreateListing));

        context.Menu.AddItem(new ApplicationMenuItem(
            RinoleggioMenus.ReceivedLeads,
            "Richieste ricevute",
            url: "/Dealer/Leads",
            icon: "fa fa-comments",
            order: 10).RequirePermissions(RinoleggioPermissions.Dealer.ViewReceivedLeads));

        context.Menu.AddItem(new ApplicationMenuItem(
            RinoleggioMenus.BackOfficeListings,
            "Gestione offerte",
            url: "/BackOffice/Listings",
            icon: "fa fa-list-check",
            order: 11).RequirePermissions(RinoleggioPermissions.BackOffice.Listings));

        context.Menu.AddItem(new ApplicationMenuItem(
            RinoleggioMenus.BackOfficeLeads,
            "Gestione richieste",
            url: "/BackOffice/Leads",
            icon: "fa fa-headset",
            order: 12).RequirePermissions(RinoleggioPermissions.BackOffice.Leads));

        context.Menu.AddItem(new ApplicationMenuItem(
            RinoleggioMenus.AdminCatalogs,
            "Anagrafiche",
            url: "/Admin/Catalogs",
            icon: "fa fa-list",
            order: 13).RequirePermissions(RinoleggioPermissions.Administrator.ManageCatalogs));

        context.Menu.AddItem(new ApplicationMenuItem(
            RinoleggioMenus.AdminDeletedListings,
            "Annunci eliminati",
            url: "/Admin/DeletedListings",
            icon: "fa fa-trash-restore",
            order: 14).RequirePermissions(RinoleggioPermissions.Administrator.RestoreListings));

        //Administration
        var administration = context.Menu.GetAdministration();
        administration.Order = 15;
        //Administration->Identity
        administration.SetSubItemOrder(IdentityMenuNames.GroupName, 1);

        if (MultiTenancyConsts.IsEnabled)
        {
            administration.SetSubItemOrder(TenantManagementMenuNames.GroupName, 1);
        }
        else
        {
            administration.TryRemoveMenuItem(TenantManagementMenuNames.GroupName);
        }

        administration.SetSubItemOrder(SettingManagementMenuNames.GroupName, 3);
        //Administration->Settings
        administration.SetSubItemOrder(SettingManagementMenuNames.GroupName, 8);

        return Task.CompletedTask;
    }
}
