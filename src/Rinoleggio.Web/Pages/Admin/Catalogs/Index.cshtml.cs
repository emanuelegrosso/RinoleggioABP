using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rinoleggio.Listings;
using Rinoleggio.Permissions;
using Volo.Abp.Application.Dtos;

namespace Rinoleggio.Web.Pages.Admin.Catalogs;

[Authorize(RinoleggioPermissions.Administrator.ManageCatalogs)]
public class IndexModel : RinoleggioPageModel
{
    private readonly ICatalogAppService _service;
    public PagedResultDto<CatalogEntryDto> Entries { get; private set; } = new();
    [BindProperty(SupportsGet = true)] public CatalogEntryType? Type { get; set; }
    [BindProperty(SupportsGet = true)] public int SkipCount { get; set; }
    public const int PageSize = 20;
    public int PageNumber => SkipCount / PageSize + 1;
    public int PageCount => Math.Max(1, (int)Math.Ceiling((double)Entries.TotalCount / PageSize));
    public int PreviousSkipCount => Math.Max(0, SkipCount - PageSize);
    public int NextSkipCount => SkipCount + PageSize;
    public bool HasNextPage => NextSkipCount < Entries.TotalCount;
    [BindProperty] public CreateCatalogEntryDto Input { get; set; } = new();

    public IndexModel(ICatalogAppService service) => _service = service;

    public async Task OnGetAsync() => await LoadEntriesAsync();

    public async Task<IActionResult> OnPostCreateAsync()
    {
        if (!ModelState.IsValid) { await LoadEntriesAsync(); return Page(); }
        try { await _service.CreateAsync(Input); }
        catch (Volo.Abp.BusinessException) { ModelState.AddModelError(nameof(Input.Name), "Esiste già una voce con questo nome per la categoria selezionata."); await LoadEntriesAsync(); return Page(); }
        return RedirectToPage(new { Type = Input.Type, SkipCount = 0 });
    }

    public async Task<IActionResult> OnPostSetActiveAsync(Guid id, bool isActive)
    {
        await _service.SetActiveAsync(id, isActive);
        return RedirectToPage(new { Type, SkipCount = Math.Max(0, SkipCount) });
    }

    public async Task<IActionResult> OnPostRenameAsync(Guid id, string name)
    {
        try { await _service.UpdateAsync(id, new UpdateCatalogEntryDto { Name = name }); }
        catch (Volo.Abp.BusinessException) { TempData["CatalogError"] = "Nome vuoto oppure già presente nella stessa categoria."; }
        return RedirectToPage(new { Type, SkipCount = Math.Max(0, SkipCount) });
    }

    public async Task<IActionResult> OnPostDeleteAsync(Guid id)
    {
        try { await _service.DeleteAsync(id); }
        catch (Volo.Abp.BusinessException) { TempData["CatalogError"] = "La voce è usata da uno o più annunci: disattivala invece di eliminarla."; }
        return RedirectToPage(new { Type, SkipCount = Math.Max(0, SkipCount) });
    }

    private async Task LoadEntriesAsync()
    {
        SkipCount = Math.Max(0, SkipCount);
        Entries = await _service.GetListAsync(new PagedAndSortedResultRequestDto { SkipCount = SkipCount, MaxResultCount = PageSize }, Type);
        if (Entries.TotalCount == 0) SkipCount = 0;
        else if (SkipCount >= Entries.TotalCount)
        {
            SkipCount = (int)((Entries.TotalCount - 1) / PageSize) * PageSize;
            Entries = await _service.GetListAsync(new PagedAndSortedResultRequestDto { SkipCount = SkipCount, MaxResultCount = PageSize }, Type);
        }
    }
}
