using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Rinoleggio.Listings;

namespace Rinoleggio.Web.Pages.Listings;

public class DetailsModel : RinoleggioPageModel
{
    private readonly IListingAppService _service;
    public ListingDto Listing { get; private set; } = new();
    public DetailsModel(IListingAppService service) => _service = service;
    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        try { Listing = await _service.GetPublicAsync(id); return Page(); }
        catch (Volo.Abp.BusinessException) { return NotFound(); }
    }
}
