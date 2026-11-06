using System;
using System.Linq;
using System.Threading.Tasks;
using Rinoleggio.Identity;
using Rinoleggio.Listings;
using Rinoleggio.Security;
using Shouldly;
using Volo.Abp.Modularity;
using Xunit;

namespace Rinoleggio.Contracts;

public abstract class ContractAppService_Manage_Tests<TStartupModule> : RinoleggioApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    private readonly IContractAppService _contractAppService;

    protected ContractAppService_Manage_Tests()
    {
        _contractAppService = GetRequiredService<IContractAppService>();
    }

    [Fact]
    public async Task GetManageList_Should_Hide_Deleted_For_BackOffice()
    {
        var listingId = Guid.Parse("a0000000-0000-0000-0000-000000000002");

        FakeCurrentPrincipalAccessor.OverrideUserId = RinoleggioIdentityDataSeedContributor.DemoDealerUserId;
        await _contractAppService.DeleteAsync(listingId, new DeleteContractDto { Reason = "Test delete" });

        FakeCurrentPrincipalAccessor.OverrideUserId = RinoleggioIdentityDataSeedContributor.DemoBackOfficeUserId;
        var manage = await _contractAppService.GetManageListAsync(new ContractSearchInput { MaxResultCount = 100 });

        manage.Items.ShouldNotContain(x => x.Id == listingId);
    }
}
