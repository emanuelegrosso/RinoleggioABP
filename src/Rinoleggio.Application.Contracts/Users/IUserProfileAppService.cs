using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Rinoleggio.Users;

public interface IUserProfileAppService : IApplicationService
{
    Task<UserProfileDto> GetAsync();
    Task<UserProfileDto> UpdateAsync(UpdateUserProfileDto input);
}
