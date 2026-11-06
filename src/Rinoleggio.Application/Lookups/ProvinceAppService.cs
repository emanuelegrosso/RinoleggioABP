using System;
using Microsoft.AspNetCore.Authorization;
using Rinoleggio.Permissions;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Rinoleggio.Lookups;

[Authorize(RinoleggioPermissions.Lookups.Default)]
public class ProvinceAppService :
    CrudAppService<Province, ProvinceDto, Guid, PagedAndSortedResultRequestDto, CreateUpdateProvinceDto>,
    IProvinceAppService
{
    public ProvinceAppService(IRepository<Province, Guid> repository) : base(repository)
    {
        GetPolicyName = RinoleggioPermissions.Lookups.Default;
        GetListPolicyName = RinoleggioPermissions.Lookups.Default;
        CreatePolicyName = RinoleggioPermissions.Lookups.Create;
        UpdatePolicyName = RinoleggioPermissions.Lookups.Edit;
        DeletePolicyName = RinoleggioPermissions.Lookups.Delete;
    }
}
