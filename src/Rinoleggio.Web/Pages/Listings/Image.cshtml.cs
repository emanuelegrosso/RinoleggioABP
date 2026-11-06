using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.BlobStoring;
using Rinoleggio.Listings;
using Rinoleggio.Web.Images;

namespace Rinoleggio.Web.Pages.Listings;

public class ImageModel : RinoleggioPageModel
{
    private readonly IListingAppService _listings;
    private readonly IBlobContainer<ListingImageContainer> _images;

    public ImageModel(IListingAppService listings, IBlobContainer<ListingImageContainer> images)
    {
        _listings = listings;
        _images = images;
    }

    public async Task<IActionResult> OnGetAsync(Guid listingId, string fileName)
    {
        var url = $"/Listings/Image/{listingId:D}/{fileName}";
        var blobName = ListingImageStorage.TryGetBlobName(listingId, url);
        if (blobName == null) return NotFound();
        ListingDto listing;
        try { listing = await _listings.GetPublicAsync(listingId); }
        catch (Volo.Abp.BusinessException) { return NotFound(); }
        if (!(listing.ImageUrls ?? string.Empty).Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Contains(url, StringComparer.OrdinalIgnoreCase)) return NotFound();
        var content = await _images.GetOrNullAsync(blobName);
        if (content == null) return NotFound();
        var contentType = fileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ? "image/png" :
            fileName.EndsWith(".webp", StringComparison.OrdinalIgnoreCase) ? "image/webp" : "image/jpeg";
        return File(content, contentType);
    }
}
