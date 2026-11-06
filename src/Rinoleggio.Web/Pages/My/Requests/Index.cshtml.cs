using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Rinoleggio.Listings;
using Rinoleggio.Permissions;
using Volo.Abp.Application.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Rinoleggio.Web.Pages.My.Requests;

[Authorize(RinoleggioPermissions.Customer.ViewOwnLeads)]
public class IndexModel : RinoleggioPageModel
{
    private readonly IListingAppService _service;

    public PagedResultDto<CustomerLeadDto> Requests { get; private set; } = new();
    public bool RequestSubmitted { get; private set; }
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
        RequestSubmitted = TempData["RequestSubmitted"]?.ToString() == "True";
        SkipCount = Math.Max(0, SkipCount);
        Requests = await _service.GetMyLeadsAsync(new PagedAndSortedResultRequestDto { SkipCount = SkipCount, MaxResultCount = PageSize });
        if (Requests.TotalCount > 0 && SkipCount >= Requests.TotalCount)
        {
            SkipCount = (int)((Requests.TotalCount - 1) / PageSize) * PageSize;
            Requests = await _service.GetMyLeadsAsync(new PagedAndSortedResultRequestDto { SkipCount = SkipCount, MaxResultCount = PageSize });
        }
    }
}
