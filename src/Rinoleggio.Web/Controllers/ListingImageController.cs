using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rinoleggio.Listings;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.BlobStoring;

namespace Rinoleggio.Web.Controllers;

[Route("listing-images")]
public class ListingImageController : AbpController
{
    private readonly IBlobContainer<ListingImagesBlobContainer> _blobContainer;

    public ListingImageController(IBlobContainer<ListingImagesBlobContainer> blobContainer)
    {
        _blobContainer = blobContainer;
    }

    [AllowAnonymous]
    [HttpGet("{*blobName}")]
    public async Task<IActionResult> GetAsync(string blobName)
    {
        if (string.IsNullOrWhiteSpace(blobName) || blobName.Contains("..", StringComparison.Ordinal))
        {
            return NotFound();
        }

        if (!await _blobContainer.ExistsAsync(blobName))
        {
            return NotFound();
        }

        var stream = await _blobContainer.GetAsync(blobName);
        var contentType = Path.GetExtension(blobName).ToLowerInvariant() switch
        {
            ".png" => "image/png",
            ".webp" => "image/webp",
            ".gif" => "image/gif",
            _ => "image/jpeg"
        };

        return File(stream, contentType);
    }
}
