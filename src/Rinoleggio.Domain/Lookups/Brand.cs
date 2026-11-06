using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace Rinoleggio.Lookups;

public class Brand : CreationAuditedEntity<Guid>
{
    public Brand()
    {
    }

    public Brand(Guid id) : base(id)
    {
    }

    public string Name { get; set; } = null!;

    public int? SortOrder { get; set; }

    public bool IsActive { get; set; } = true;
}
