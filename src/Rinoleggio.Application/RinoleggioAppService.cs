using Rinoleggio.Localization;
using Volo.Abp.Application.Services;

namespace Rinoleggio;

/* Inherit your application services from this class.
 */
public abstract class RinoleggioAppService : ApplicationService
{
    protected RinoleggioAppService()
    {
        LocalizationResource = typeof(RinoleggioResource);
    }
}
