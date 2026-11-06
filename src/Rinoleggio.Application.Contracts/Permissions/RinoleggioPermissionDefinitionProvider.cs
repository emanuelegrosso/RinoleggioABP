using Rinoleggio.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace Rinoleggio.Permissions;

public class RinoleggioPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(RinoleggioPermissions.GroupName);

        var booksPermission = myGroup.AddPermission(RinoleggioPermissions.Books.Default, L("Permission:Books"));
        booksPermission.AddChild(RinoleggioPermissions.Books.Create, L("Permission:Books.Create"));
        booksPermission.AddChild(RinoleggioPermissions.Books.Edit, L("Permission:Books.Edit"));
        booksPermission.AddChild(RinoleggioPermissions.Books.Delete, L("Permission:Books.Delete"));

        var authorsPermission = myGroup.AddPermission(RinoleggioPermissions.Authors.Default, L("Permission:Authors"));
        authorsPermission.AddChild(RinoleggioPermissions.Authors.Create, L("Permission:Authors.Create"));
        authorsPermission.AddChild(RinoleggioPermissions.Authors.Edit, L("Permission:Authors.Edit"));
        authorsPermission.AddChild(RinoleggioPermissions.Authors.Delete, L("Permission:Authors.Delete"));

        var customerLeads = myGroup.AddPermission(RinoleggioPermissions.Customer.Leads, L("Permission:Customer.Leads"));
        myGroup.AddPermission(RinoleggioPermissions.Customer.Favorites, L("Permission:Customer.Favorites"))
            .AddChild(RinoleggioPermissions.Customer.ManageFavorites, L("Permission:Customer.Favorites.Manage"));
        customerLeads.AddChild(RinoleggioPermissions.Customer.CreateLead, L("Permission:Customer.Leads.Create"));
        customerLeads.AddChild(RinoleggioPermissions.Customer.ViewOwnLeads, L("Permission:Customer.Leads.ViewOwn"));

        var dealerListings = myGroup.AddPermission(RinoleggioPermissions.Dealer.Listings, L("Permission:Dealer.Listings"));
        dealerListings.AddChild(RinoleggioPermissions.Dealer.CreateListing, L("Permission:Dealer.Listings.Create"));
        dealerListings.AddChild(RinoleggioPermissions.Dealer.ViewOwnListings, L("Permission:Dealer.Listings.ViewOwn"));
        dealerListings.AddChild(RinoleggioPermissions.Dealer.ManageOwnListings, L("Permission:Dealer.Listings.ManageOwn"));
        var dealerLeads = myGroup.AddPermission(RinoleggioPermissions.Dealer.Leads, L("Permission:Dealer.Leads"));
        dealerLeads.AddChild(RinoleggioPermissions.Dealer.ViewReceivedLeads, L("Permission:Dealer.Leads.ViewReceived"));

        var backOffice = myGroup.AddPermission(RinoleggioPermissions.BackOffice.Listings, L("Permission:BackOffice.Listings"));
        backOffice.AddChild(RinoleggioPermissions.BackOffice.ManageListings, L("Permission:BackOffice.ManageListings"));
        var leads = myGroup.AddPermission(RinoleggioPermissions.BackOffice.Leads, L("Permission:BackOffice.Leads"));
        leads.AddChild(RinoleggioPermissions.BackOffice.ManageLeads, L("Permission:BackOffice.ManageLeads"));
        var administrator = myGroup.AddPermission(RinoleggioPermissions.Administrator.Default, L("Permission:Administrator"));
        administrator.AddChild(RinoleggioPermissions.Administrator.ManageCatalogs, L("Permission:Administrator.ManageCatalogs"));
        administrator.AddChild(RinoleggioPermissions.Administrator.RestoreListings, L("Permission:Administrator.RestoreListings"));
        //Define your own permissions here. Example:
        //myGroup.AddPermission(RinoleggioPermissions.MyPermission1, L("Permission:MyPermission1"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<RinoleggioResource>(name);
    }
}
