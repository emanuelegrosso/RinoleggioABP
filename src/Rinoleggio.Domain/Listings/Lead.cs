using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace Rinoleggio.Listings;

public class Lead : AuditedAggregateRoot<Guid>
{
    public Guid ListingId { get; set; }
    public Guid BuyerId { get; set; }
    public Guid SellerId { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? BuyerPhone { get; set; }
    public bool PrivacyConsent { get; set; }
    public LeadStatus Status { get; set; } = LeadStatus.New;
    public LeadOutcome Outcome { get; set; } = LeadOutcome.None;
    public string? OutcomeNotes { get; set; }
    public string? InternalNotes { get; set; }

    protected Lead() { }
    public Lead(Guid id, Guid listingId, Guid buyerId, Guid sellerId, string message) : base(id)
    {
        ListingId = listingId;
        BuyerId = buyerId;
        SellerId = sellerId;
        Message = message;
    }
}
