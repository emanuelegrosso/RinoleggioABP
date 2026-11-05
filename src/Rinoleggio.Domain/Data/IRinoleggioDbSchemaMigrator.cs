using System.Threading.Tasks;

namespace Rinoleggio.Data;

public interface IRinoleggioDbSchemaMigrator
{
    Task MigrateAsync();
}
