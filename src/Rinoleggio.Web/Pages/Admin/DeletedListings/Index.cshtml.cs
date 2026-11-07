using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rinoleggio.Listings;
using Rinoleggio.Permissions;
using Volo.Abp.Application.Dtos;

namespace Rinoleggio.Web.Pages.Admin.DeletedListings;

[Authorize(RinoleggioPermissions.Administrator.RestoreListings)]
public class IndexModel : RinoleggioPageModel
{
    private readonly IListingAppService _service;
    public PagedResultDto<ListingDto> Listings { get; private set; } = new();
    [BindProperty(SupportsGet = true)] public int SkipCount { get; set; }
    public const int PageSize = 20;
    public int PageNumber => SkipCount / PageSize + 1;
    public int PageCount => Math.Max(1, (int)Math.Ceiling((double)Listings.TotalCount / PageSize));
    public int PreviousSkipCount => Math.Max(0, SkipCount - PageSize);
    public int NextSkipCount => SkipCount + PageSize;
    public bool HasNextPage => NextSkipCount < Listings.TotalCount;

    public IndexModel(IListingAppService service) => _service = service;

    public async Task OnGetAsync()
    {
        SkipCount = Math.Max(0, SkipCount);
        Listings = await _service.GetDeletedListingsAsync(new PagedAndSortedResultRequestDto { SkipCount = SkipCount, MaxResultCount = PageSize });
        if (Listings.TotalCount == 0) SkipCount = 0;
        else if (SkipCount >= Listings.TotalCount)
        {
            SkipCount = (int)((Listings.TotalCount - 1) / PageSize) * PageSize;
            Listings = await _service.GetDeletedListingsAsync(new PagedAndSortedResultRequestDto { SkipCount = SkipCount, MaxResultCount = PageSize });
        }
    }

    public async Task<IActionResult> OnPostRestoreAsync(Guid id)
    {
        await _service.RestoreDeletedListingAsync(id);
        return RedirectToPage(new { skipCount = Math.Max(0, SkipCount) });
    }
}
