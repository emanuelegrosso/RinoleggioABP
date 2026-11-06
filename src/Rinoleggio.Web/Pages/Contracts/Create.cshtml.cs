using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rinoleggio.Contracts;
using Rinoleggio.Permissions;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Rinoleggio.Web.Pages.Contracts;

[Authorize(RinoleggioPermissions.Contracts.Create)]
public class CreateModel : AbpPageModel
{
    private readonly IContractAppService _contractAppService;

    public CreateModel(IContractAppService contractAppService)
    {
        _contractAppService = contractAppService;
    }

    [BindProperty]
    public CreateUpdateContractDto Contract { get; set; } = new()
    {
        RegistrationYear = (short)DateTime.UtcNow.Year,
        TotalKm = 60000,
        EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(12))
    };

    public ContractLookupsDto Lookups { get; private set; } = new();

    public async Task OnGetAsync()
    {
        Lookups = await _contractAppService.GetLookupsAsync();
    }

    public Task<IActionResult> OnPostSaveDraftAsync() => CreateAsync(publishNow: false);

    public Task<IActionResult> OnPostPublishAsync() => CreateAsync(publishNow: true);

    private async Task<IActionResult> CreateAsync(bool publishNow)
    {
        Contract.InsuranceKinds ??= [];
        Lookups = await _contractAppService.GetLookupsAsync();
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var input = new CreateContractDto
        {
            PublishNow = publishNow,
            Title = Contract.Title,
            DescriptionHtml = Contract.DescriptionHtml,
            ProvinceId = Contract.ProvinceId,
            RentalCompanyId = Contract.RentalCompanyId,
            StartDate = Contract.StartDate,
            EndDate = Contract.EndDate,
            MonthlyFee = Contract.MonthlyFee,
            VatIncluded = Contract.VatIncluded,
            InitialPayment = Contract.InitialPayment,
            TotalKm = Contract.TotalKm,
            CurrentKm = Contract.CurrentKm,
            ExcessKmRate = Contract.ExcessKmRate,
            UnderKmRate = Contract.UnderKmRate,
            BrandId = Contract.BrandId,
            Model = Contract.Model,
            Version = Contract.Version,
            FuelType = Contract.FuelType,
            Transmission = Contract.Transmission,
            RegistrationYear = Contract.RegistrationYear,
            TransferContributionType = Contract.TransferContributionType,
            TransferContributionAmount = Contract.TransferContributionAmount,
            MaintenanceIncluded = Contract.MaintenanceIncluded,
            InsuranceIncluded = Contract.InsuranceIncluded,
            RoadsideAssistanceIncluded = Contract.RoadsideAssistanceIncluded,
            ReplacementCarIncluded = Contract.ReplacementCarIncluded,
            TiresIncluded = Contract.TiresIncluded,
            RoadTaxIncluded = Contract.RoadTaxIncluded,
            InsuranceKinds = Contract.InsuranceKinds
        };

        var created = await _contractAppService.CreateAsync(input);
        TempData["ContractCreated"] = true;
        return RedirectToPage("/Contracts/Edit", new { id = created.Id });
    }
}
