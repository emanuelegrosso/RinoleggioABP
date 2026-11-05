using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Rinoleggio.Data;

/* This is used if database provider does't define
 * IRinoleggioDbSchemaMigrator implementation.
 */
public class NullRinoleggioDbSchemaMigrator : IRinoleggioDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
