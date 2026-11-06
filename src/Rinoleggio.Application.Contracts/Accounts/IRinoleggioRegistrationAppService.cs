using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Rinoleggio.Accounts;

public interface IRinoleggioRegistrationAppService : IApplicationService
{
    Task RegisterAsync(RegisterRinoleggioUserInput input);
}
