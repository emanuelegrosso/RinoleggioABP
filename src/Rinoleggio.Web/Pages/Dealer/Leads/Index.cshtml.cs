using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Rinoleggio.Listings;
using Rinoleggio.Permissions;
using Volo.Abp.Application.Dtos;

namespace Rinoleggio.Web.Pages.Dealer.Leads;

[Authorize(RinoleggioPermissions.Dealer.ViewReceivedLeads)]
public class IndexModel : RinoleggioPageModel
{
    private readonly IListingAppService _service;

    public PagedResultDto<DealerLeadDto> Requests { get; private set; } = new();
    [BindProperty(SupportsGet = true)] public int SkipCount { get; set; }
    public const int PageSize = 20;
    public int PageNumber => SkipCount / PageSize + 1;
    public int PageCount => Math.Max(1, (int)Math.Ceiling((double)Requests.TotalCount / PageSize));
    public int PreviousSkipCount => Math.Max(0, SkipCount - PageSize);
    public int NextSkipCount => SkipCount + PageSize;
    public bool HasNextPage => NextSkipCount < Requests.TotalCount;

    public IndexModel(IListingAppService service) => _service = service;

    public async Task OnGetAsync()
    {
        SkipCount = Math.Max(0, SkipCount);
        Requests = await _service.GetReceivedLeadsAsync(new PagedAndSortedResultRequestDto { SkipCount = SkipCount, MaxResultCount = PageSize });
        if (Requests.TotalCount > 0 && SkipCount >= Requests.TotalCount)
        {
            SkipCount = (int)((Requests.TotalCount - 1) / PageSize) * PageSize;
            Requests = await _service.GetReceivedLeadsAsync(new PagedAndSortedResultRequestDto { SkipCount = SkipCount, MaxResultCount = PageSize });
        }
    }
}
