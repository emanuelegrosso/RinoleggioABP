using System.Threading.Tasks;
using Rinoleggio.Permissions;
using Volo.Abp.Authorization.Permissions;

namespace Rinoleggio.Web.Pages;

public class IndexModel : RinoleggioPageModel
{
    public bool CanViewOwnListings { get; private set; }
    public bool CanCreateListing { get; private set; }

    public async Task OnGetAsync()
    {
        var permissionChecker = LazyServiceProvider.LazyGetRequiredService<IPermissionChecker>();
        CanViewOwnListings = await permissionChecker.IsGrantedAsync(RinoleggioPermissions.Contracts.ViewOwn);
        CanCreateListing = await permissionChecker.IsGrantedAsync(RinoleggioPermissions.Contracts.Create);
    }
}
