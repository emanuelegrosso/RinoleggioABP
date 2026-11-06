using System;
using System.Collections.Generic;

namespace Rinoleggio.Contracts;

public class ContractLookupsDto
{
    public List<LookupItemDto> RentalCompanies { get; set; } = [];
    public List<LookupItemDto> Brands { get; set; } = [];
    public List<LookupItemDto> Provinces { get; set; } = [];
}

public class LookupItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Code { get; set; }
}
