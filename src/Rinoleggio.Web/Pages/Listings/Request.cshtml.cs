using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rinoleggio.Listings;
using Rinoleggio.Permissions;

namespace Rinoleggio.Web.Pages.Listings;

[Authorize(RinoleggioPermissions.Customer.CreateLead)]
public class RequestModel : RinoleggioPageModel
{
    private readonly IListingAppService _service;
    public ListingDto Listing { get; private set; } = new();
    [BindProperty] public CreateLeadDto Input { get; set; } = new();
    public RequestModel(IListingAppService service) => _service = service;
    public async Task<IActionResult> OnGetAsync(Guid listingId)
    {
        try { Listing = await _service.GetPublicAsync(listingId); return Page(); }
        catch (Volo.Abp.BusinessException) { return NotFound(); }
    }
    public async Task<IActionResult> OnPostAsync(Guid listingId)
    {
        if (!ModelState.IsValid) return await ReturnToFormAsync(listingId);
        try
        {
            await _service.CreateLeadAsync(listingId, Input);
        }
        catch (Volo.Abp.BusinessException exception)
        {
            var message = exception.Code switch
            {
                "Rinoleggio:DuplicateOpenLead" => "Hai già una richiesta aperta per questo contratto. Puoi verificarne lo stato nelle tue richieste.",
                "Rinoleggio:CannotRequestOwnListing" => "Non puoi inviare una richiesta per una tua offerta.",
                "Rinoleggio:ListingNotAvailable" => "Questa offerta non è più disponibile.",
                "Rinoleggio:PrivacyConsentRequired" => "Per inviare la richiesta devi accettare il trattamento dei dati.",
                _ => "Non è stato possibile inviare la richiesta. Controlla i dati e riprova."
            };
            ModelState.AddModelError(string.Empty, message);
            return await ReturnToFormAsync(listingId);
        }

        TempData["RequestSubmitted"] = true;
        return RedirectToPage("/My/Requests/Index");
    }

    private async Task<IActionResult> ReturnToFormAsync(Guid listingId)
    {
        try
        {
            Listing = await _service.GetPublicAsync(listingId);
            return Page();
        }
        catch (Volo.Abp.BusinessException)
        {
            return NotFound();
        }
    }
}
