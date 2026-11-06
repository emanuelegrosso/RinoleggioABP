using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace Rinoleggio.Listings;

public class ListingImage : CreationAuditedEntity<Guid>
{
    public Guid ListingId { get; set; }

    public string StoragePathOrBlobId { get; set; } = null!;

    public byte SortOrder { get; set; }
}
