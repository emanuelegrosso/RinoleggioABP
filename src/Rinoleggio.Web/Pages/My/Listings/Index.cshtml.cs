using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rinoleggio.Listings;
using Rinoleggio.Permissions;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Authorization.Permissions;
using System;

namespace Rinoleggio.Web.Pages.My.Listings;

[Authorize(RinoleggioPermissions.Dealer.ViewOwnListings)]
public class IndexModel : RinoleggioPageModel
{
    private readonly IListingAppService _service;
    private readonly IPermissionChecker _permissionChecker;
    public PagedResultDto<ListingDto> Listings { get; private set; } = new();
    public bool CanManage { get; private set; }
    public bool CanCreate { get; private set; }
    [BindProperty(SupportsGet = true)] public int SkipCount { get; set; }
    public const int PageSize = 20;
    public int PageNumber => SkipCount / PageSize + 1;
    public int PageCount => Math.Max(1, (int)Math.Ceiling((double)Listings.TotalCount / PageSize));
    public int PreviousSkipCount => Math.Max(0, SkipCount - PageSize);
    public int NextSkipCount => SkipCount + PageSize;
    public bool HasNextPage => NextSkipCount < Listings.TotalCount;

    public IndexModel(IListingAppService service, IPermissionChecker permissionChecker)
    {
        _service = service;
        _permissionChecker = permissionChecker;
    }

    public async Task OnGetAsync()
    {
        SkipCount = Math.Max(0, SkipCount);
        Listings = await _service.GetMineAsync(new PagedAndSortedResultRequestDto { SkipCount = SkipCount, MaxResultCount = PageSize });
        if (Listings.TotalCount == 0) SkipCount = 0;
        else if (SkipCount >= Listings.TotalCount)
        {
            SkipCount = (int)((Listings.TotalCount - 1) / PageSize) * PageSize;
            Listings = await _service.GetMineAsync(new PagedAndSortedResultRequestDto { SkipCount = SkipCount, MaxResultCount = PageSize });
        }
        CanManage = await _permissionChecker.IsGrantedAsync(RinoleggioPermissions.Dealer.ManageOwnListings);
        CanCreate = await _permissionChecker.IsGrantedAsync(RinoleggioPermissions.Dealer.CreateListing);
    }

    public async Task<IActionResult> OnPostSetStatusAsync(System.Guid id, ListingStatus status, string? reason)
    {
        await _service.SetMyStatusAsync(id, new ListingStatusDto { Status = status, Reason = reason });
        return RedirectToPage(new { skipCount = Math.Max(0, SkipCount) });
    }
}
