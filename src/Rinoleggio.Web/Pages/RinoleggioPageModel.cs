using Rinoleggio.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Rinoleggio.Web.Pages;

public abstract class RinoleggioPageModel : AbpPageModel
{
    protected RinoleggioPageModel()
    {
        LocalizationResourceType = typeof(RinoleggioResource);
    }
}
