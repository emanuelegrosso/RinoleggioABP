using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace Rinoleggio.Pages;

[Collection(RinoleggioTestConsts.CollectionDefinitionName)]
public class Index_Tests : RinoleggioWebTestBase
{
    [Fact]
    public async Task Welcome_Page()
    {
        var response = await GetResponseAsStringAsync("/");
        response.ShouldNotBeNull();
        response.ShouldContain("Rental contract takeovers");
        response.ShouldNotContain("Getting Started");
        response.ShouldNotContain("abp.io/docs");
    }
}
