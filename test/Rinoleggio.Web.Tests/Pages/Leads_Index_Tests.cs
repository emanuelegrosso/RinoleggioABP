using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace Rinoleggio.Pages;

[Collection(RinoleggioTestConsts.CollectionDefinitionName)]
public class Leads_Index_Tests : RinoleggioWebTestBase
{
    [Fact]
    public async Task Leads_Sent_Page_Loads()
    {
        var response = await GetResponseAsStringAsync("/Leads/Sent");
        response.ShouldNotBeNull();
        response.ShouldNotContain("NullReferenceException");
        response.ShouldContain("Le mie richieste");
    }
}
