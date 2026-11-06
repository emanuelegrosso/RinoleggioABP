using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace Rinoleggio.Lookups;

public class Province : CreationAuditedEntity<Guid>
{
    public Province()
    {
    }

    public Province(Guid id) : base(id)
    {
    }

    public string Name { get; set; } = null!;

    public string Code { get; set; } = null!;

    public int? SortOrder { get; set; }

    public bool IsActive { get; set; } = true;
}
