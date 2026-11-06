using System;
using Rinoleggio.Listings;
using Volo.Abp.Domain.Entities.Auditing;

namespace Rinoleggio.Leads;

public class Lead : CreationAuditedEntity<Guid>
{
    public Guid ListingId { get; set; }

    public Guid BuyerId { get; set; }

    public Guid SellerId { get; set; }

    public string? Title { get; set; }

    public string Message { get; set; } = null!;

    public string? Phone { get; set; }

    public bool PrivacyConsent { get; set; }

    public LeadStatus Status { get; set; } = LeadStatus.New;

    public string? BackofficeNotes { get; set; }

    public Listing? Listing { get; set; }
}
