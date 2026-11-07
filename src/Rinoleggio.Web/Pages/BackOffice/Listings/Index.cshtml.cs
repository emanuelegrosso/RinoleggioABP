using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rinoleggio.Listings;
using Rinoleggio.Permissions;
using Volo.Abp.Application.Dtos;

namespace Rinoleggio.Web.Pages.BackOffice.Listings;

[Authorize(RinoleggioPermissions.BackOffice.Listings)]
public class IndexModel : RinoleggioPageModel
{
    private readonly IListingAppService _service;
    public PagedResultDto<ListingDto> Listings { get; private set; } = new();
    [BindProperty(SupportsGet = true)] public ListingSearchInput Filter { get; set; } = new() { MaxResultCount = 100 };

    public IndexModel(IListingAppService service) => _service = service;

    public async Task OnGetAsync()
    {
        Listings = await _service.GetBackOfficeListAsync(Filter);
    }
}
