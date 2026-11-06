using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace Rinoleggio.Pages;

[Collection(RinoleggioTestConsts.CollectionDefinitionName)]
public class Contracts_Create_Tests : RinoleggioWebTestBase
{
    [Fact]
    public async Task Contracts_Create_Page_Loads_With_Form()
    {
        var response = await GetResponseAsStringAsync("/Contracts/Create");
        response.ShouldNotBeNull();
        response.ShouldNotContain("NullReferenceException");
        response.ShouldContain("Nuovo contratto");
        response.ShouldContain("Salva bozza");
        response.ShouldContain("Pubblica");
        response.ShouldContain("text-danger");
    }
}
