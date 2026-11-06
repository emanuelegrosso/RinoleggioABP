using System;
using System.Linq;
using System.Threading.Tasks;
using Rinoleggio.Contracts;
using Rinoleggio.Listings;
using Rinoleggio.Lookups;
using Shouldly;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Xunit;

namespace Rinoleggio.Contracts;

/// <summary>
/// Integration tests for contract search. Run via a concrete EF test class when the test host seed is fixed.
/// </summary>
public abstract class ContractAppService_Tests<TStartupModule> : RinoleggioApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    private readonly IContractAppService _contractAppService;
    private readonly IRepository<Brand, Guid> _brandRepository;

    protected ContractAppService_Tests()
    {
        _contractAppService = GetRequiredService<IContractAppService>();
        _brandRepository = GetRequiredService<IRepository<Brand, Guid>>();
    }

    [Fact]
    public async Task Search_Should_Return_Seeded_Listings()
    {
        var result = await _contractAppService.SearchAsync(new ContractSearchInput
        {
            MaxResultCount = 100
        });

        result.TotalCount.ShouldBeGreaterThanOrEqualTo(20);
        result.Items.ShouldContain(x => x.Id == ListingDemoDataSeedContributor.SeedMarkerListingId);
    }

    [Fact]
    public async Task Search_By_Brand_Should_Filter_Results()
    {
        var all = await _contractAppService.SearchAsync(new ContractSearchInput { MaxResultCount = 100 });
        var sample = all.Items.First();
        var brands = await _brandRepository.GetListAsync();
        var brand = brands.First(b => b.Name == sample.BrandName);

        var filtered = await _contractAppService.SearchAsync(new ContractSearchInput
        {
            BrandId = brand.Id,
            MaxResultCount = 100
        });

        filtered.TotalCount.ShouldBeLessThanOrEqualTo(all.TotalCount);
        filtered.Items.ShouldAllBe(x => x.BrandName == brand.Name);
    }
}
