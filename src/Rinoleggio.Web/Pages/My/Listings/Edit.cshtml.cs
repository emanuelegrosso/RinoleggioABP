using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rinoleggio.Listings;
using Rinoleggio.Permissions;
using Rinoleggio.Web.Images;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace Rinoleggio.Web.Pages.My.Listings;

[Authorize]
public class EditModel : RinoleggioPageModel
{
    private readonly IListingAppService _service;
    private readonly Volo.Abp.Authorization.Permissions.IPermissionChecker _permissionChecker;
    private readonly ListingImageStorage _imageStorage;
    private IReadOnlyList<string> CurrentImageUrls { get; set; } = Array.Empty<string>();

    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    [BindProperty]
    public CreateUpdateListingDto Input { get; set; } = new();
    public IReadOnlyList<string> Provinces { get; private set; } = Array.Empty<string>();
    public IReadOnlyList<string> Brands { get; private set; } = Array.Empty<string>();
    public IReadOnlyList<string> RentalCompanies { get; private set; } = Array.Empty<string>();
    public IReadOnlyList<string> ExistingImages => CurrentImageUrls;
    [BindProperty] public List<IFormFile> UploadPhotos { get; set; } = new();
    [BindProperty] public List<string> RemoveImageUrls { get; set; } = new();

    public EditModel(IListingAppService service, Volo.Abp.Authorization.Permissions.IPermissionChecker permissionChecker, ListingImageStorage imageStorage)
    {
        _service = service;
        _permissionChecker = permissionChecker;
        _imageStorage = imageStorage;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        if (!await _permissionChecker.IsGrantedAsync(RinoleggioPermissions.Dealer.ManageOwnListings) &&
            !await _permissionChecker.IsGrantedAsync(RinoleggioPermissions.BackOffice.ManageListings)) return Forbid();
        var listing = await _service.GetAsync(Id);
        Input = new CreateUpdateListingDto
        {
            Title = listing.Title,
            Description = listing.Description,
            ImageUrls = listing.ImageUrls,
            Province = listing.Province,
            RentalCompany = listing.RentalCompany,
            Brand = listing.Brand,
            Model = listing.Model,
            Version = listing.Version,
            FuelType = listing.FuelType,
            Transmission = listing.Transmission,
            RegistrationYear = listing.RegistrationYear,
            StartDate = listing.StartDate,
            EndDate = listing.EndDate,
            MonthlyFee = listing.MonthlyFee,
            VatIncluded = listing.VatIncluded,
            InitialPayment = listing.InitialPayment,
            TotalKm = listing.TotalKm,
            CurrentKm = listing.CurrentKm,
            ExcessKmRate = listing.ExcessKmRate,
            UnderKmRate = listing.UnderKmRate,
            ContributionType = listing.ContributionType,
            ContributionAmount = listing.ContributionAmount,
            MaintenanceIncluded = listing.MaintenanceIncluded,
            InsuranceIncluded = listing.InsuranceIncluded,
            InsuranceKinds = listing.InsuranceKinds,
            RoadsideAssistanceIncluded = listing.RoadsideAssistanceIncluded,
            ReplacementCarIncluded = listing.ReplacementCarIncluded,
            TiresIncluded = listing.TiresIncluded,
            RoadTaxIncluded = listing.RoadTaxIncluded,
            Publish = listing.IsPublished
        };
        CurrentImageUrls = (listing.ImageUrls ?? string.Empty).Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        await LoadLookupsAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!await _permissionChecker.IsGrantedAsync(RinoleggioPermissions.Dealer.ManageOwnListings) &&
            !await _permissionChecker.IsGrantedAsync(RinoleggioPermissions.BackOffice.ManageListings)) return Forbid();
        var listing = await _service.GetAsync(Id);
        CurrentImageUrls = (listing.ImageUrls ?? string.Empty).Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        List<PreparedListingImage> photos;
        try { photos = await _imageStorage.PrepareAsync(UploadPhotos); }
        catch (InvalidOperationException ex) { ModelState.AddModelError(nameof(UploadPhotos), ex.Message); await LoadLookupsAsync(); return Page(); }
        var keptImages = CurrentImageUrls.Except(RemoveImageUrls, StringComparer.Ordinal).ToList();
        if (keptImages.Count + photos.Count > ListingImageStorage.MaxImagesPerListing)
            ModelState.AddModelError(nameof(UploadPhotos), $"L'annuncio può avere al massimo {ListingImageStorage.MaxImagesPerListing} foto.");
        if (!ModelState.IsValid) { await LoadLookupsAsync(); return Page(); }
        var newUrls = photos.Select(x => x.Url(Id)).ToList();
        var removedUrls = CurrentImageUrls.Except(keptImages, StringComparer.Ordinal).ToList();
        try
        {
            await _imageStorage.StoreAsync(Id, photos);
            Input.ImageUrls = string.Join(';', keptImages.Concat(newUrls));
            await _service.UpdateAsync(Id, Input);
            await _imageStorage.DeleteAsync(Id, removedUrls);
        }
        catch
        {
            await _imageStorage.DeleteAsync(Id, newUrls);
            throw;
        }
        return await _permissionChecker.IsGrantedAsync(RinoleggioPermissions.BackOffice.ManageListings)
            ? RedirectToPage("/BackOffice/Listings/Details", new { id = Id })
            : RedirectToPage("/My/Listings/Index");
    }

    private async Task LoadLookupsAsync()
    {
        Provinces = await _service.GetLookupValuesAsync("province");
        Brands = await _service.GetLookupValuesAsync("brand");
        RentalCompanies = await _service.GetLookupValuesAsync("rentalcompany");
    }
}
