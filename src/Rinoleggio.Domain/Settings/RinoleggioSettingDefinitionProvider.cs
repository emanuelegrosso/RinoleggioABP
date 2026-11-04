using Volo.Abp.Settings;

namespace Rinoleggio.Settings;

public class RinoleggioSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(RinoleggioSettings.MySetting1));
    }
}
