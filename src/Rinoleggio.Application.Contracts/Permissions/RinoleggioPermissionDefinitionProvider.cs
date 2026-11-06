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

        var contracts = myGroup.AddPermission(RinoleggioPermissions.Contracts.Default, L("Permission:Contracts"));
        contracts.AddChild(RinoleggioPermissions.Contracts.Create, L("Permission:Contracts.Create"));
        contracts.AddChild(RinoleggioPermissions.Contracts.Edit, L("Permission:Contracts.Edit"));
        contracts.AddChild(RinoleggioPermissions.Contracts.ViewOwn, L("Permission:Contracts.ViewOwn"));
        contracts.AddChild(RinoleggioPermissions.Contracts.ManageAll, L("Permission:Contracts.ManageAll"));
        contracts.AddChild(RinoleggioPermissions.Contracts.Suspend, L("Permission:Contracts.Suspend"));
        contracts.AddChild(RinoleggioPermissions.Contracts.Close, L("Permission:Contracts.Close"));
        contracts.AddChild(RinoleggioPermissions.Contracts.Delete, L("Permission:Contracts.Delete"));
        contracts.AddChild(RinoleggioPermissions.Contracts.Restore, L("Permission:Contracts.Restore"));

        myGroup.AddPermission(RinoleggioPermissions.Profile.Default, L("Permission:Profile"));

        var leads = myGroup.AddPermission(RinoleggioPermissions.Leads.Default, L("Permission:Leads"));
        leads.AddChild(RinoleggioPermissions.Leads.Create, L("Permission:Leads.Create"));
        leads.AddChild(RinoleggioPermissions.Leads.ViewOwn, L("Permission:Leads.ViewOwn"));
        leads.AddChild(RinoleggioPermissions.Leads.ViewAll, L("Permission:Leads.ViewAll"));
        leads.AddChild(RinoleggioPermissions.Leads.Manage, L("Permission:Leads.Manage"));

        var lookups = myGroup.AddPermission(RinoleggioPermissions.Lookups.Default, L("Permission:Lookups"));
        lookups.AddChild(RinoleggioPermissions.Lookups.Create, L("Permission:Lookups.Create"));
        lookups.AddChild(RinoleggioPermissions.Lookups.Edit, L("Permission:Lookups.Edit"));
        lookups.AddChild(RinoleggioPermissions.Lookups.Delete, L("Permission:Lookups.Delete"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<RinoleggioResource>(name);
    }
}
