using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace Rinoleggio.Pages;

[Collection(RinoleggioTestConsts.CollectionDefinitionName)]
public class Contracts_Index_Tests : RinoleggioWebTestBase
{
    [Fact]
    public async Task Contracts_Search_Page_Loads()
    {
        var response = await GetResponseAsStringAsync("/Contracts");
        response.ShouldNotBeNull();
        response.ShouldNotContain("NullReferenceException");
        response.ShouldContain("Ricerca contratti");
    }

    [Fact]
    public async Task Contracts_Search_With_Service_Filter()
    {
        var response = await GetResponseAsStringAsync("/Contracts?Filters.MaintenanceIncluded=true");
        response.ShouldNotBeNull();
        response.ShouldContain("Filters_MaintenanceIncluded");
        response.ShouldContain("name=\"Filters.MaintenanceIncluded\" value=\"true\" checked");
    }

    [Fact]
    public async Task Contracts_Search_Pagination_Preserves_Filter()
    {
        var response = await GetResponseAsStringAsync("/Contracts?Filters.MaintenanceIncluded=true&Filters.MaxResultCount=5&Filters.SkipCount=0");
        response.ShouldNotBeNull();
        response.ShouldContain("Filters.MaintenanceIncluded=true");
    }

    [Fact]
    public async Task Contracts_Search_Returns_Seeded_Results()
    {
        var response = await GetResponseAsStringAsync("/Contracts");
        response.ShouldNotBeNull();
        // 20 published Active + 1 demo Reserved (draft/suspended/withdrawn are not public)
        response.ShouldContain("21 contratti trovati");
    }

    [Fact]
    public async Task Contracts_Search_Empty_State_When_No_Matches()
    {
        var response = await GetResponseAsStringAsync("/Contracts?Filters.Query=zzzzzz-no-match-xyz");
        response.ShouldContain("Nessun contratto corrisponde");
        response.ShouldContain("0 contratti trovati");
    }

    [Fact]
    public async Task Contracts_Search_With_Sorting_Parameter()
    {
        var response = await GetResponseAsStringAsync("/Contracts?Filters.Sorting=MonthlyFee%20desc");
        response.ShouldNotContain("NullReferenceException");
        response.ShouldContain("Ordina per");
        response.ShouldContain("MonthlyFee desc");
    }
}
