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
        //Define your own permissions here. Example:
        //myGroup.AddPermission(RinoleggioPermissions.MyPermission1, L("Permission:MyPermission1"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<RinoleggioResource>(name);
    }
}
