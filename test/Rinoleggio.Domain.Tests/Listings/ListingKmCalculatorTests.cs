using Rinoleggio.Listings;
using Shouldly;
using Xunit;

namespace Rinoleggio.Listings;

public class ListingKmCalculatorTests
{
    [Theory]
    [InlineData(60000, 12000, 48000)]
    [InlineData(60000, 70000, 0)]
    public void CalculateRemainingKm_Should_Return_Expected(int total, int current, int expected)
    {
        ListingKmCalculator.CalculateRemainingKm(total, current).ShouldBe(expected);
    }
}
