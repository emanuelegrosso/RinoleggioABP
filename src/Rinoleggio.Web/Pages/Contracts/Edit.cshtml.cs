using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rinoleggio.Contracts;
using Rinoleggio.Listings;
using Rinoleggio.Permissions;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Rinoleggio.Web.Pages.Contracts;

[Authorize(RinoleggioPermissions.Contracts.Edit)]
public class EditModel : AbpPageModel
{
    private readonly IContractAppService _contractAppService;
    private readonly IListingImageAppService _listingImageAppService;

    public EditModel(
        IContractAppService contractAppService,
        IListingImageAppService listingImageAppService)
    {
        _contractAppService = contractAppService;
        _listingImageAppService = listingImageAppService;
    }

    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    [BindProperty]
    public CreateUpdateContractDto Contract { get; set; } = new();

    [BindProperty]
    public WithdrawContractDto Withdraw { get; set; } = new();

    public ContractLookupsDto Lookups { get; private set; } = new();

    public IReadOnlyList<ListingImageDto> Images { get; private set; } = [];

    public int MaxListingImages { get; } = ListingConsts.MaxListingImages;

    public bool CanUploadMoreImages => Images.Count < MaxListingImages;

    [TempData]
    public bool ContractCreated { get; set; }

    [TempData]
    public string? VisibilityFlash { get; set; }

    public string? VisibilityErrorMessage { get; private set; }

    public bool IsPublished { get; private set; }

    public ListingBusinessStatus BusinessStatus { get; private set; }

    public bool CanToggleSearchVisibility =>
        BusinessStatus is ListingBusinessStatus.Active or ListingBusinessStatus.Reserved;

    public async Task OnGetAsync()
    {
        await LoadPageAsync();
        if (VisibilityFlash?.StartsWith("error:") == true)
        {
            var code = VisibilityFlash["error:".Length..];
            VisibilityErrorMessage = L[code].Value;
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        Contract.InsuranceKinds ??= [];
        if (!ModelState.IsValid)
        {
            await LoadPageAsync();
            return Page();
        }

        await _contractAppService.UpdateAsync(Id, Contract);
        return RedirectToPage("/Contracts/Detail", new { id = Id });
    }

    public async Task<IActionResult> OnPostWithdrawAsync()
    {
        await _contractAppService.WithdrawAsync(Id, Withdraw);
        return RedirectToPage("/Contracts/Index");
    }

    public async Task<IActionResult> OnPostDeleteImageAsync(Guid imageId)
    {
        await _listingImageAppService.DeleteAsync(Id, imageId);
        return RedirectToPage(new { id = Id });
    }

    public async Task<IActionResult> OnPostUnpublishAsync()
    {
        try
        {
            await _contractAppService.UnpublishAsync(Id);
            VisibilityFlash = "hidden";
        }
        catch (BusinessException ex)
        {
            VisibilityFlash = "error:" + ex.Code;
        }

        return RedirectToPage(new { id = Id });
    }

    public async Task<IActionResult> OnPostPublishListingAsync()
    {
        try
        {
            await _contractAppService.PublishListingAsync(Id);
            VisibilityFlash = "visible";
        }
        catch (BusinessException ex)
        {
            VisibilityFlash = "error:" + ex.Code;
        }

        return RedirectToPage(new { id = Id });
    }

    public async Task<IActionResult> OnPostClearReservationAsync()
    {
        try
        {
            await _contractAppService.ClearListingReservationAsync(Id);
            VisibilityFlash = "available";
        }
        catch (BusinessException ex)
        {
            VisibilityFlash = "error:" + ex.Code;
        }

        return RedirectToPage(new { id = Id });
    }

    private async Task LoadPageAsync()
    {
        Lookups = await _contractAppService.GetLookupsAsync();
        var detail = await _contractAppService.GetAsync(Id);
        Contract = MapFromDetail(detail);
        Images = detail.Images;
        IsPublished = detail.IsPublished;
        BusinessStatus = detail.BusinessStatus;
    }

    private static CreateUpdateContractDto MapFromDetail(ContractDetailDto detail)
    {
        return new CreateUpdateContractDto
        {
            Title = detail.Title,
            DescriptionHtml = detail.DescriptionHtml,
            ProvinceId = detail.ProvinceId,
            RentalCompanyId = detail.RentalCompanyId,
            StartDate = detail.StartDate,
            EndDate = detail.EndDate,
            MonthlyFee = detail.MonthlyFee,
            VatIncluded = detail.VatIncluded,
            InitialPayment = detail.InitialPayment,
            TotalKm = detail.TotalKm,
            CurrentKm = detail.CurrentKm,
            ExcessKmRate = detail.ExcessKmRate,
            UnderKmRate = detail.UnderKmRate,
            BrandId = detail.BrandId,
            Model = detail.Model,
            Version = detail.Version,
            FuelType = detail.FuelType,
            Transmission = detail.Transmission,
            RegistrationYear = detail.RegistrationYear,
            TransferContributionType = detail.TransferContributionType,
            TransferContributionAmount = detail.TransferContributionAmount,
            MaintenanceIncluded = detail.MaintenanceIncluded,
            InsuranceIncluded = detail.InsuranceIncluded,
            RoadsideAssistanceIncluded = detail.RoadsideAssistanceIncluded,
            ReplacementCarIncluded = detail.ReplacementCarIncluded,
            TiresIncluded = detail.TiresIncluded,
            RoadTaxIncluded = detail.RoadTaxIncluded,
            InsuranceKinds = detail.InsuranceKinds
        };
    }
}
