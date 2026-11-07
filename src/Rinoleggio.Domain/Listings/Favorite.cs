using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace Rinoleggio.Listings;

public class Favorite : CreationAuditedAggregateRoot<Guid>
{
    public Guid UserId { get; set; }
    public Guid ListingId { get; set; }

    protected Favorite() { }

    public Favorite(Guid id, Guid userId, Guid listingId) : base(id)
    {
        UserId = userId;
        ListingId = listingId;
    }
}
