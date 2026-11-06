using System;
using System.ComponentModel.DataAnnotations;
using Rinoleggio.Lookups;
using Volo.Abp.Application.Dtos;

namespace Rinoleggio.Lookups;

public class RentalCompanyDto : EntityDto<Guid>
{
    public string Name { get; set; } = null!;
    public string? WebsiteUrl { get; set; }
    public bool IsActive { get; set; }
}

public class CreateUpdateRentalCompanyDto
{
    [Required]
    [StringLength(LookupConsts.MaxRentalCompanyNameLength)]
    public string Name { get; set; } = null!;

    [StringLength(LookupConsts.MaxWebsiteUrlLength)]
    public string? WebsiteUrl { get; set; }

    public bool IsActive { get; set; } = true;
}

public class BrandDto : EntityDto<Guid>
{
    public string Name { get; set; } = null!;
    public int? SortOrder { get; set; }
    public bool IsActive { get; set; }
}

public class CreateUpdateBrandDto
{
    [Required]
    [StringLength(LookupConsts.MaxBrandNameLength)]
    public string Name { get; set; } = null!;

    public int? SortOrder { get; set; }
    public bool IsActive { get; set; } = true;
}

public class ProvinceDto : EntityDto<Guid>
{
    public string Name { get; set; } = null!;
    public string Code { get; set; } = null!;
    public int? SortOrder { get; set; }
    public bool IsActive { get; set; }
}

public class CreateUpdateProvinceDto
{
    [Required]
    [StringLength(LookupConsts.MaxProvinceNameLength)]
    public string Name { get; set; } = null!;

    [Required]
    [StringLength(LookupConsts.ProvinceCodeLength, MinimumLength = LookupConsts.ProvinceCodeLength)]
    public string Code { get; set; } = null!;

    public int? SortOrder { get; set; }
    public bool IsActive { get; set; } = true;
}
