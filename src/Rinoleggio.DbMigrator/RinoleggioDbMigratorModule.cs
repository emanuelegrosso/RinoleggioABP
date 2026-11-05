using Rinoleggio.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace Rinoleggio.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(RinoleggioEntityFrameworkCoreModule),
    typeof(RinoleggioApplicationContractsModule)
)]
public class RinoleggioDbMigratorModule : AbpModule
{
}
