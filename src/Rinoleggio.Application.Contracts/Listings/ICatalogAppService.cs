using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Application.Dtos;

namespace Rinoleggio.Listings;

public interface ICatalogAppService : IApplicationService
{
    Task<PagedResultDto<CatalogEntryDto>> GetListAsync(PagedAndSortedResultRequestDto input, CatalogEntryType? type = null);
    Task<CatalogEntryDto> CreateAsync(CreateCatalogEntryDto input);
    Task UpdateAsync(Guid id, UpdateCatalogEntryDto input);
    Task SetActiveAsync(Guid id, bool isActive);
    Task DeleteAsync(Guid id);
}

public class CatalogEntryDto
{
    public Guid Id { get; set; }
    public CatalogEntryType Type { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public class CreateCatalogEntryDto
{
    [EnumDataType(typeof(CatalogEntryType))] public CatalogEntryType Type { get; set; }
    [Required, StringLength(120)] public string Name { get; set; } = string.Empty;
}

public class UpdateCatalogEntryDto
{
    [Required, StringLength(120)] public string Name { get; set; } = string.Empty;
}
