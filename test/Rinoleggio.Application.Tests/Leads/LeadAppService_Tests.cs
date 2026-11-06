using System;
using System.Linq;
using System.Threading.Tasks;
using Rinoleggio.Identity;
using Rinoleggio.Listings;
using Rinoleggio.Security;
using Shouldly;
using Volo.Abp;
using Volo.Abp.Modularity;
using Xunit;

namespace Rinoleggio.Leads;

public abstract class LeadAppService_Tests<TStartupModule> : RinoleggioApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    private readonly ILeadAppService _leadAppService;

    protected LeadAppService_Tests()
    {
        _leadAppService = GetRequiredService<ILeadAppService>();
    }

    [Fact]
    public async Task Create_Should_Block_Self_Request_On_Own_Listing()
    {
        FakeCurrentPrincipalAccessor.OverrideUserId = RinoleggioIdentityDataSeedContributor.DemoDealerUserId;

        var ex = await Should.ThrowAsync<BusinessException>(async () =>
            await _leadAppService.CreateAsync(new CreateLeadDto
            {
                ListingId = ListingDemoDataSeedContributor.SeedMarkerListingId,
                Message = "Vorrei subentrare al contratto.",
                Phone = "+39 333 1234567",
                PrivacyConsent = true
            }));

        ex.Code.ShouldBe(RinoleggioDomainErrorCodes.LeadBuyerCannotBeSeller);
    }

    [Fact]
    public async Task Create_Should_Block_Duplicate_Open_Lead()
    {
        FakeCurrentPrincipalAccessor.OverrideUserId = RinoleggioIdentityDataSeedContributor.DemoCustomerUserId;

        var listingId = Guid.Parse("a0000000-0000-0000-0000-000000000003");
        var input = new CreateLeadDto
        {
            ListingId = listingId,
            Message = "Prima richiesta di subentro.",
            Phone = "+39 333 7654321",
            PrivacyConsent = true
        };

        await _leadAppService.CreateAsync(input);

        var ex = await Should.ThrowAsync<BusinessException>(async () =>
            await _leadAppService.CreateAsync(new CreateLeadDto
            {
                ListingId = input.ListingId,
                Message = "Seconda richiesta duplicata.",
                Phone = input.Phone,
                PrivacyConsent = true
            }));

        ex.Code.ShouldBe(RinoleggioDomainErrorCodes.LeadDuplicateOpen);
    }

    [Fact]
    public async Task GetReceived_Should_Not_Expose_Buyer_Phone()
    {
        FakeCurrentPrincipalAccessor.OverrideUserId = RinoleggioIdentityDataSeedContributor.DemoCustomerUserId;

        var listingId = Guid.Parse("a0000000-0000-0000-0000-000000000004");
        await _leadAppService.CreateAsync(new CreateLeadDto
        {
            ListingId = listingId,
            Message = "Richiesta con telefono nascosto al venditore.",
            Phone = "+39 333 9998877",
            PrivacyConsent = true
        });

        FakeCurrentPrincipalAccessor.OverrideUserId = RinoleggioIdentityDataSeedContributor.DemoDealerUserId;

        var received = await _leadAppService.GetReceivedAsync(new GetLeadListInput { MaxResultCount = 10 });
        received.Items.ShouldNotBeEmpty();
        received.Items.ShouldAllBe(x => string.IsNullOrEmpty(x.Phone));
        received.Items.ShouldAllBe(x => string.IsNullOrEmpty(x.BuyerEmail));
    }
}
