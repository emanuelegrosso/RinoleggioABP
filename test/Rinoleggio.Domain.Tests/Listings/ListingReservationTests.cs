using System;
using Rinoleggio.Listings;
using Shouldly;
using Volo.Abp;
using Xunit;

namespace Rinoleggio.Listings;

public class ListingReservationTests
{
    [Fact]
    public void MarkReserved_Should_Set_Status_From_Active()
    {
        var listing = CreateMinimalListing(ListingBusinessStatus.Active);
        listing.MarkReserved(StatusChangedBy.Seller);
        listing.BusinessStatus.ShouldBe(ListingBusinessStatus.Reserved);
    }

    [Fact]
    public void MarkReserved_Should_Throw_When_Not_Active()
    {
        var listing = CreateMinimalListing(ListingBusinessStatus.Reserved);
        Should.Throw<BusinessException>(() => listing.MarkReserved(StatusChangedBy.Seller));
    }

    [Fact]
    public void ClearReservation_Should_Restore_Active()
    {
        var listing = CreateMinimalListing(ListingBusinessStatus.Reserved);
        listing.ClearReservation(StatusChangedBy.Seller);
        listing.BusinessStatus.ShouldBe(ListingBusinessStatus.Active);
    }

    private static Listing CreateMinimalListing(ListingBusinessStatus status)
    {
        var listing = new Listing(Guid.NewGuid())
        {
            BusinessStatus = status,
            Contract = new ListingContract
            {
                EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(6))
            }
        };
        return listing;
    }
}
