using System.Threading.Tasks;
using Rinoleggio.Listings;
using Shouldly;
using Xunit;

namespace Rinoleggio.Pages;

[Collection(RinoleggioTestConsts.CollectionDefinitionName)]
public class Contracts_Detail_Tests : RinoleggioWebTestBase
{
    [Fact]
    public async Task Contracts_Detail_Page_Loads_For_Seed_Listing()
    {
        var id = ListingDemoDataSeedContributor.SeedMarkerListingId;
        var response = await GetResponseAsStringAsync($"/Contracts/Detail/{id}");
        response.ShouldNotBeNull();
        response.ShouldNotContain("NullReferenceException");
    }

    [Fact]
    public async Task Contracts_Detail_Shows_Italian_Labels_And_Lead_Sidebar()
    {
        var id = ListingDemoDataSeedContributor.SeedMarkerListingId;
        var response = await GetResponseAsStringAsync($"/Contracts/Detail/{id}");
        response.ShouldContain("Elettrico");
        response.ShouldNotContain("TransferContributionType");
        response.ShouldNotContain(">Electric<");
        response.ShouldContain("Richiesta di subentro");
        // Test host runs as authenticated admin (not listing owner) with AlwaysAllowAuthorization.
        response.ShouldContain("Invia richiesta");
        response.ShouldContain("data-valmsg-for=\"Lead.Message\"");
    }
}
