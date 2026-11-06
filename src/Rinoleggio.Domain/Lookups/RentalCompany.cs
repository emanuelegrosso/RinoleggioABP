using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace Rinoleggio.Lookups;

public class RentalCompany : CreationAuditedEntity<Guid>
{
    public RentalCompany()
    {
    }

    public RentalCompany(Guid id) : base(id)
    {
    }

    public string Name { get; set; } = null!;

    public string? WebsiteUrl { get; set; }

    public bool IsActive { get; set; } = true;
}
