using System;
using Rinoleggio.Lookups;
using Volo.Abp.Domain.Entities;

namespace Rinoleggio.Listings;

public class ListingVehicle : Entity
{
    public Guid ListingId { get; set; }

    public Guid BrandId { get; set; }

    public string Model { get; set; } = null!;

    public string? Version { get; set; }

    public FuelType FuelType { get; set; }

    public Transmission Transmission { get; set; }

    public short RegistrationYear { get; set; }

    public Brand? Brand { get; set; }

    public override object[] GetKeys()
    {
        return [ListingId];
    }
}
