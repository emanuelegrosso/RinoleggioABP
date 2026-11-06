using System.Threading.Tasks;
using Rinoleggio.Listings;
using Shouldly;
using Xunit;

namespace Rinoleggio.Pages;

[Collection(RinoleggioTestConsts.CollectionDefinitionName)]
public class Contracts_Edit_Tests : RinoleggioWebTestBase
{
    [Fact]
    public async Task Contracts_Edit_Page_Shows_Image_Section()
    {
        var id = ListingDemoDataSeedContributor.SeedMarkerListingId;
        var response = await GetResponseAsStringAsync($"/Contracts/Edit/{id}");
        response.ShouldNotBeNull();
        response.ShouldNotContain("NullReferenceException");
        response.ShouldContain("Immagini annuncio");
        response.ShouldContain("Nessuna immagine caricata.");
        response.ShouldContain("Carica immagine");
        response.ShouldContain("/ 20 immagini");
        response.ShouldContain("Visibilità in ricerca");
    }
}
