using System;

namespace Rinoleggio.Listings;

public static class ListingKmCalculator
{
    public static int CalculateRemainingKm(int totalKm, int currentKm)
    {
        return Math.Max(0, totalKm - currentKm);
    }
}
