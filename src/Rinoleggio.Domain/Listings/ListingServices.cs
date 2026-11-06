using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;

namespace Rinoleggio.Listings;

public class ListingServices : Entity
{
    public Guid ListingId { get; set; }

    public bool MaintenanceIncluded { get; set; }

    public bool InsuranceIncluded { get; set; }

    public bool RoadsideAssistanceIncluded { get; set; }

    public bool ReplacementCarIncluded { get; set; }

    public bool TiresIncluded { get; set; }

    public bool RoadTaxIncluded { get; set; }

    public ICollection<ListingInsuranceType> InsuranceTypes { get; set; } = new Collection<ListingInsuranceType>();

    public override object[] GetKeys()
    {
        return [ListingId];
    }
}
