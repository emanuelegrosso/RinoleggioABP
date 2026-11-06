using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Rinoleggio.Contracts;
using Rinoleggio.Leads;
using Rinoleggio.Localization;
using Rinoleggio.Permissions;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Timing;

namespace Rinoleggio.Web.Pages.Contracts;

public class DetailModel : AbpPageModel
{
    private readonly IContractAppService _contractAppService;
    private readonly ILeadAppService _leadAppService;
    private readonly IClock _clock;
    private readonly IStringLocalizer<RinoleggioResource> _localizer;

    public DetailModel(
        IContractAppService contractAppService,
        ILeadAppService leadAppService,
        IClock clock,
        IStringLocalizer<RinoleggioResource> localizer)
    {
        _contractAppService = contractAppService;
        _leadAppService = leadAppService;
        _clock = clock;
        _localizer = localizer;
    }

    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    public ContractDetailDto Contract { get; private set; } = null!;

    [BindProperty]
    public CreateLeadDto Lead { get; set; } = new();

    public bool IsOwner { get; private set; }
    public bool CanEdit { get; private set; }
    public bool CanViewOwnLeads { get; private set; }
    public bool IsListingEligibleForLeads { get; private set; }
    public bool CanSendLead { get; private set; }
    public bool HasLeadsCreatePermission { get; private set; }

    public string? LeadErrorMessage { get; private set; }

    [TempData]
    public bool LeadSubmitted { get; set; }

    public async Task OnGetAsync()
    {
        await LoadPageAsync();
    }

    public async Task<IActionResult> OnPostLeadAsync()
    {
        Lead.ListingId = Id;
        if (!ModelState.IsValid)
        {
            await LoadPageAsync();
            return Page();
        }

        try
        {
            await _leadAppService.CreateAsync(Lead);
            LeadSubmitted = true;
            return RedirectToPage(new { id = Id });
        }
        catch (BusinessException ex)
        {
            LeadErrorMessage = ResolveLeadErrorMessage(ex);
            await LoadPageAsync();
            return Page();
        }
    }

    private string ResolveLeadErrorMessage(BusinessException ex)
    {
        var code = ex.Code ?? string.Empty;
        var localized = _localizer[code];
        if (!localized.ResourceNotFound && !string.IsNullOrEmpty(localized.Value))
        {
            return localized.Value;
        }

        return code switch
        {
            "Rinoleggio:PrivacyConsentRequired" => "Devi acconsentire al trattamento dei dati.",
            RinoleggioDomainErrorCodes.LeadDuplicateOpen =>
                "Hai già una richiesta aperta su questo annuncio. Controlla la tua area lead.",
            RinoleggioDomainErrorCodes.LeadBuyerCannotBeSeller =>
                "Non puoi inviare una richiesta sul tuo annuncio.",
            RinoleggioDomainErrorCodes.LeadListingNotEligible =>
                "Questo annuncio non accetta richieste di subentro al momento.",
            _ => "Impossibile inviare la richiesta. Verifica i dati e riprova."
        };
    }

    private async Task LoadPageAsync()
    {
        Contract = await _contractAppService.GetAsync(Id);
        Lead.ListingId = Id;

        var permissionChecker = LazyServiceProvider.LazyGetRequiredService<IPermissionChecker>();
        var today = DateOnly.FromDateTime(_clock.Now);

        IsOwner = CurrentUser.IsAuthenticated && Contract.OwnerId == CurrentUser.Id;
        CanEdit = Contract.CanEdit;
        CanViewOwnLeads = await permissionChecker.IsGrantedAsync(RinoleggioPermissions.Leads.ViewOwn);
        HasLeadsCreatePermission = await permissionChecker.IsGrantedAsync(RinoleggioPermissions.Leads.Create);
        IsListingEligibleForLeads = ContractFormHelper.IsEligibleForLeads(Contract, today);
        CanSendLead = CurrentUser.IsAuthenticated &&
                      !IsOwner &&
                      HasLeadsCreatePermission &&
                      IsListingEligibleForLeads;
    }
}
