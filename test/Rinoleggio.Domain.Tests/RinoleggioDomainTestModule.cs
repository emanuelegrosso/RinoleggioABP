using Volo.Abp.Modularity;

namespace Rinoleggio;

[DependsOn(
    typeof(RinoleggioDomainModule),
    typeof(RinoleggioTestBaseModule)
)]
public class RinoleggioDomainTestModule : AbpModule
{

}
