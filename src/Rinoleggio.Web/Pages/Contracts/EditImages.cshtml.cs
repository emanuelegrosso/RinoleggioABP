using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Rinoleggio.Listings;
using Rinoleggio.Permissions;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;
using Volo.Abp.Content;

namespace Rinoleggio.Web.Pages.Contracts;

[Authorize(RinoleggioPermissions.Contracts.Edit)]
public class EditImagesModel : AbpPageModel
{
    private readonly IListingImageAppService _listingImageAppService;

    public EditImagesModel(IListingImageAppService listingImageAppService)
    {
        _listingImageAppService = listingImageAppService;
    }

    [BindProperty(SupportsGet = true)]
    public Guid ListingId { get; set; }

    [BindProperty]
    public IFormFile? Upload { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        if (Upload == null || Upload.Length == 0)
        {
            return RedirectToPage("/Contracts/Edit", new { id = ListingId });
        }

        await using var stream = Upload.OpenReadStream();
        var remote = new RemoteStreamContent(stream, Upload.FileName, Upload.ContentType);
        await _listingImageAppService.UploadAsync(ListingId, remote);
        return RedirectToPage("/Contracts/Edit", new { id = ListingId });
    }
}
