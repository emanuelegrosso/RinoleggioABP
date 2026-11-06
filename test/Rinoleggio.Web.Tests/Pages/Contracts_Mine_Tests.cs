using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace Rinoleggio.Pages;

[Collection(RinoleggioTestConsts.CollectionDefinitionName)]
public class Contracts_Mine_Tests : RinoleggioWebTestBase
{
    [Fact]
    public async Task Contracts_Mine_Page_Loads()
    {
        // Test host uses AlwaysAllowAuthorization; production requires login + ViewOwn.
        var response = await GetResponseAsStringAsync("/Contracts/Mine");
        response.ShouldNotBeNull();
        response.ShouldContain("I miei annunci");
    }
}
