using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace Rinoleggio.Listings;

public class CatalogEntry : AuditedAggregateRoot<Guid>
{
    public CatalogEntryType Type { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NormalizedName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    protected CatalogEntry() { }

    public CatalogEntry(Guid id, CatalogEntryType type, string name) : base(id)
    {
        Type = type;
        Name = name;
        NormalizedName = name.Trim().ToUpperInvariant();
        IsActive = true;
    }
}
