using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Rinoleggio.Leads;

public interface ILeadAppService : IApplicationService
{
    Task<LeadDto> CreateAsync(CreateLeadDto input);
    Task<PagedResultDto<LeadDto>> GetSentAsync(GetLeadListInput input);
    Task<PagedResultDto<LeadDto>> GetReceivedAsync(GetLeadListInput input);
    Task<PagedResultDto<LeadDto>> GetBackOfficeListAsync(GetLeadListInput input);
    Task<LeadDto> UpdateStatusAsync(Guid id, UpdateLeadStatusDto input);
}
