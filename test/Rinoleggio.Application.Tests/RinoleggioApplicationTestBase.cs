using Volo.Abp.Modularity;

namespace Rinoleggio;

public abstract class RinoleggioApplicationTestBase<TStartupModule> : RinoleggioTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
