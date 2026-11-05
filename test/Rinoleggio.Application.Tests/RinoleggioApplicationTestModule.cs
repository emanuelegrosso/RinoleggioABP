using Volo.Abp.Modularity;

namespace Rinoleggio;

[DependsOn(
    typeof(RinoleggioApplicationModule),
    typeof(RinoleggioDomainTestModule)
)]
public class RinoleggioApplicationTestModule : AbpModule
{

}
