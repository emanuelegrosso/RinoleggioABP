using Volo.Abp.Ui.Branding;
using Volo.Abp.DependencyInjection;
using Microsoft.Extensions.Localization;
using Rinoleggio.Localization;

namespace Rinoleggio.Web;

[Dependency(ReplaceServices = true)]
public class RinoleggioBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<RinoleggioResource> _localizer;

    public RinoleggioBrandingProvider(IStringLocalizer<RinoleggioResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];

    public override string? LogoUrl => "~/images/logo/leptonxlite/logo-light-thumbnail.png";

    public override string? LogoIconUrl => "~/images/logo/leptonxlite/logo-dark-thumbnail.png";
}
