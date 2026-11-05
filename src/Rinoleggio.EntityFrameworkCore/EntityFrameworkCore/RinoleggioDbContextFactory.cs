using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Rinoleggio.EntityFrameworkCore;

/* This class is needed for EF Core console commands
 * (like Add-Migration and Update-Database commands) */
public class RinoleggioDbContextFactory : IDesignTimeDbContextFactory<RinoleggioDbContext>
{
    public RinoleggioDbContext CreateDbContext(string[] args)
    {
        var configuration = BuildConfiguration();
        
        RinoleggioEfCoreEntityExtensionMappings.Configure();

        var builder = new DbContextOptionsBuilder<RinoleggioDbContext>()
            .UseSqlite(configuration.GetConnectionString("Default"));
        
        return new RinoleggioDbContext(builder.Options);
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../Rinoleggio.DbMigrator/"))
            .AddJsonFile("appsettings.json", optional: false)
            .AddEnvironmentVariables();

        return builder.Build();
    }
}
