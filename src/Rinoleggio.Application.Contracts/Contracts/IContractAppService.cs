using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Rinoleggio.Contracts;

public interface IContractAppService : IApplicationService
{
    Task<PagedResultDto<ContractSearchResultDto>> SearchAsync(ContractSearchInput input);
    Task<ContractDetailDto> GetAsync(Guid id);
    Task<ContractLookupsDto> GetLookupsAsync();
    Task<ContractDetailDto> CreateAsync(CreateContractDto input);
    Task<ContractDetailDto> UpdateAsync(Guid id, CreateUpdateContractDto input);
    Task WithdrawAsync(Guid id, WithdrawContractDto input);
    Task CloseAsync(Guid id, CloseContractDto input);
    Task DeleteAsync(Guid id, DeleteContractDto input);
    Task RestoreAsync(Guid id);
    Task UnpublishAsync(Guid id);
    Task PublishListingAsync(Guid id);
    Task RepublishAsync(Guid id);
    Task ReserveAsync(Guid id);
    Task ClearListingReservationAsync(Guid id);
    Task SuspendAsync(Guid id, SuspendContractDto input);
    Task ReactivateAsync(Guid id);
    Task<PagedResultDto<ContractSearchResultDto>> GetManageListAsync(ContractSearchInput input);
    Task<PagedResultDto<ContractSearchResultDto>> GetMyListAsync(ContractSearchInput input);
}
