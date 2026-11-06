using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Rinoleggio.Lookups;

public interface IRentalCompanyAppService : ICrudAppService<RentalCompanyDto, Guid, PagedAndSortedResultRequestDto, CreateUpdateRentalCompanyDto>
{
}

public interface IBrandAppService : ICrudAppService<BrandDto, Guid, PagedAndSortedResultRequestDto, CreateUpdateBrandDto>
{
}

public interface IProvinceAppService : ICrudAppService<ProvinceDto, Guid, PagedAndSortedResultRequestDto, CreateUpdateProvinceDto>
{
}
