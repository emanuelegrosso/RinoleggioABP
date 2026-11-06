using System;
using Volo.Abp.Domain.Entities;

namespace Rinoleggio.Listings;

public class ListingInsuranceType : Entity
{
    public Guid ListingId { get; set; }

    public InsuranceKind InsuranceKind { get; set; }

    public override object[] GetKeys()
    {
        return [ListingId, InsuranceKind];
    }
}
