using System;
using Rinoleggio.Listings;
using Shouldly;
using Volo.Abp;
using Xunit;

namespace Rinoleggio.Listings;

public class ListingWithdrawTests
{
    [Fact]
    public void Withdraw_Should_Set_Withdrawn_And_Unpublish()
    {
        var listing = CreatePublishedListing();
        listing.Withdraw("Non più disponibile", StatusChangedBy.Seller);
        listing.BusinessStatus.ShouldBe(ListingBusinessStatus.Withdrawn);
        listing.IsPublished.ShouldBeFalse();
        listing.StatusReason.ShouldBe("Non più disponibile");
    }

    [Fact]
    public void Close_Should_Not_Be_Withdrawn()
    {
        var listing = CreatePublishedListing();
        listing.Close(StatusChangedBy.Seller);
        listing.BusinessStatus.ShouldBe(ListingBusinessStatus.Closed);
    }

    [Fact]
    public void RepublishBySeller_Should_Work_After_Seller_Withdraw()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var listing = CreatePublishedListing();
        listing.Withdraw("Pausa", StatusChangedBy.Seller);
        listing.RepublishBySeller(DateTime.UtcNow, today);
        listing.IsPublished.ShouldBeTrue();
        listing.BusinessStatus.ShouldBe(ListingBusinessStatus.Active);
    }

    [Fact]
    public void RepublishBySeller_Should_Fail_After_BackOffice_Withdraw()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var listing = CreatePublishedListing();
        listing.Withdraw("Moderazione", StatusChangedBy.BackOffice);
        Should.Throw<BusinessException>(() => listing.RepublishBySeller(DateTime.UtcNow, today));
    }

    private static Listing CreatePublishedListing()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var listing = new Listing(Guid.NewGuid())
        {
            Contract = new ListingContract
            {
                EndDate = today.AddMonths(6)
            }
        };
        listing.Publish(DateTime.UtcNow, today);
        return listing;
    }
}
