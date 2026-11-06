using System;
using Rinoleggio.Lookups;
using Volo.Abp.Domain.Entities;

namespace Rinoleggio.Listings;

public class ListingContract : Entity
{
    public Guid ListingId { get; set; }

    public Guid RentalCompanyId { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public decimal MonthlyFee { get; set; }

    public bool VatIncluded { get; set; }

    public decimal InitialPayment { get; set; }

    public int TotalKm { get; set; }

    public int CurrentKm { get; set; }

    public int RemainingKm { get; set; }

    public decimal? ExcessKmRate { get; set; }

    public decimal? UnderKmRate { get; set; }

    public RentalCompany? RentalCompany { get; set; }

    public override object[] GetKeys()
    {
        return [ListingId];
    }

    public void UpdateKilometers(int totalKm, int currentKm)
    {
        TotalKm = totalKm;
        CurrentKm = currentKm;
        RemainingKm = ListingKmCalculator.CalculateRemainingKm(totalKm, currentKm);
    }
}
