using System;
using Microsoft.AspNetCore.Authorization;
using Rinoleggio.Permissions;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Rinoleggio.Lookups;

[Authorize(RinoleggioPermissions.Lookups.Default)]
public class RentalCompanyAppService :
    CrudAppService<RentalCompany, RentalCompanyDto, Guid, PagedAndSortedResultRequestDto, CreateUpdateRentalCompanyDto>,
    IRentalCompanyAppService
{
    public RentalCompanyAppService(IRepository<RentalCompany, Guid> repository) : base(repository)
    {
        GetPolicyName = RinoleggioPermissions.Lookups.Default;
        GetListPolicyName = RinoleggioPermissions.Lookups.Default;
        CreatePolicyName = RinoleggioPermissions.Lookups.Create;
        UpdatePolicyName = RinoleggioPermissions.Lookups.Edit;
        DeletePolicyName = RinoleggioPermissions.Lookups.Delete;
    }
}
