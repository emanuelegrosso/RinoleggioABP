using Volo.Abp.Modularity;

namespace Rinoleggio;

/* Inherit from this class for your domain layer tests. */
public abstract class RinoleggioDomainTestBase<TStartupModule> : RinoleggioTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
