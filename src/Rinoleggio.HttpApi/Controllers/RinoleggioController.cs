using Rinoleggio.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace Rinoleggio.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class RinoleggioController : AbpControllerBase
{
    protected RinoleggioController()
    {
        LocalizationResource = typeof(RinoleggioResource);
    }
}
