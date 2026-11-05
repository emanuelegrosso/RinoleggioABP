using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Rinoleggio.Data;
using Volo.Abp.DependencyInjection;

namespace Rinoleggio.EntityFrameworkCore;

public class EntityFrameworkCoreRinoleggioDbSchemaMigrator
    : IRinoleggioDbSchemaMigrator, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public EntityFrameworkCoreRinoleggioDbSchemaMigrator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolving the RinoleggioDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<RinoleggioDbContext>()
            .Database
            .MigrateAsync();
    }
}
