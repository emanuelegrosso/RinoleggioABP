using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using Volo.Abp.Application.Dtos;
using Rinoleggio.Listings;
using Rinoleggio.Permissions;

namespace Rinoleggio.Web.Pages.My.Favorites;

[Authorize(RinoleggioPermissions.Customer.ManageFavorites)]
public class IndexModel : RinoleggioPageModel
{
    private readonly IListingAppService _service;
    public PagedResultDto<FavoriteListingDto> Favorites { get; private set; } = new();
    [BindProperty(SupportsGet = true)] public int SkipCount { get; set; }
    public const int PageSize = 20;
    public int PageNumber => SkipCount / PageSize + 1;
    public int PageCount => Math.Max(1, (int)Math.Ceiling((double)Favorites.TotalCount / PageSize));
    public int PreviousSkipCount => Math.Max(0, SkipCount - PageSize);
    public int NextSkipCount => SkipCount + PageSize;
    public bool HasNextPage => NextSkipCount < Favorites.TotalCount;

    public IndexModel(IListingAppService service) => _service = service;

    public async Task OnGetAsync()
    {
        SkipCount = Math.Max(0, SkipCount);
        Favorites = await _service.GetFavoritesAsync(new PagedAndSortedResultRequestDto { SkipCount = SkipCount, MaxResultCount = PageSize });
        if (Favorites.TotalCount == 0) SkipCount = 0;
        else if (SkipCount >= Favorites.TotalCount)
        {
            SkipCount = (int)((Favorites.TotalCount - 1) / PageSize) * PageSize;
            Favorites = await _service.GetFavoritesAsync(new PagedAndSortedResultRequestDto { SkipCount = SkipCount, MaxResultCount = PageSize });
        }
    }

    public async Task<IActionResult> OnPostRemoveAsync(System.Guid listingId)
    {
        await _service.RemoveFavoriteAsync(listingId);
        return RedirectToPage(new { skipCount = Math.Max(0, SkipCount) });
    }
}
