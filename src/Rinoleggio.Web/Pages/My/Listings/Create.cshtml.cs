using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Rinoleggio.Listings;
using Rinoleggio.Permissions;
using Rinoleggio.Web.Images;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace Rinoleggio.Web.Pages.My.Listings;

[Authorize(RinoleggioPermissions.Dealer.CreateListing)]
public class CreateModel : RinoleggioPageModel
{
    private readonly IListingAppService _service;
    private readonly ListingImageStorage _imageStorage;
    public IReadOnlyList<string> Provinces { get; private set; } = Array.Empty<string>();
    public IReadOnlyList<string> Brands { get; private set; } = Array.Empty<string>();
    public IReadOnlyList<string> RentalCompanies { get; private set; } = Array.Empty<string>();
    [BindProperty] public CreateUpdateListingDto Input { get; set; } = new() { EndDate = DateTime.Today.AddMonths(12), RegistrationYear = DateTime.Today.Year, TotalKm = 100000 };
    [BindProperty] public List<IFormFile> UploadPhotos { get; set; } = new();
    public CreateModel(IListingAppService service, ListingImageStorage imageStorage) { _service = service; _imageStorage = imageStorage; }
    public async Task OnGetAsync() => await LoadLookupsAsync();
    public async Task<IActionResult> OnPostAsync()
    {
        List<PreparedListingImage> photos;
        try { photos = await _imageStorage.PrepareAsync(UploadPhotos); }
        catch (InvalidOperationException ex) { ModelState.AddModelError(nameof(UploadPhotos), ex.Message); await LoadLookupsAsync(); return Page(); }
        if (!ModelState.IsValid) { await LoadLookupsAsync(); return Page(); }
        var requestedPublish = Input.Publish;
        Input.ImageUrls = null;
        if (photos.Count > 0) Input.Publish = false;
        var listing = await _service.CreateAsync(Input);
        if (photos.Count > 0)
        {
            try
            {
                await _imageStorage.StoreAsync(listing.Id, photos);
                Input.ImageUrls = string.Join(';', photos.Select(x => x.Url(listing.Id)));
                Input.Publish = requestedPublish;
                await _service.UpdateAsync(listing.Id, Input);
            }
            catch
            {
                await _imageStorage.DeleteAsync(listing.Id, photos.Select(x => x.Url(listing.Id)));
                TempData["ListingImageError"] = "Le foto non sono state salvate. L'annuncio è stato mantenuto come bozza: puoi riprovare dalla modifica.";
                return RedirectToPage("/My/Listings/Edit", new { id = listing.Id });
            }
        }
        return RedirectToPage("/My/Listings/Index");
    }

    private async Task LoadLookupsAsync()
    {
        Provinces = await _service.GetLookupValuesAsync("province");
        Brands = await _service.GetLookupValuesAsync("brand");
        RentalCompanies = await _service.GetLookupValuesAsync("rentalcompany");
    }
}
