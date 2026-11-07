using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Rinoleggio.Permissions;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;

namespace Rinoleggio.Listings;

[Authorize(RinoleggioPermissions.Administrator.ManageCatalogs)]
public class CatalogAppService : ApplicationService, ICatalogAppService
{
    private readonly IRepository<CatalogEntry, Guid> _catalogEntries;
    private readonly IRepository<Listing, Guid> _listings;

    public CatalogAppService(IRepository<CatalogEntry, Guid> catalogEntries, IRepository<Listing, Guid> listings)
    {
        _catalogEntries = catalogEntries;
        _listings = listings;
    }

    public async Task<PagedResultDto<CatalogEntryDto>> GetListAsync(PagedAndSortedResultRequestDto input, CatalogEntryType? type = null)
    {
        var query = await _catalogEntries.GetQueryableAsync();
        if (type.HasValue) query = query.Where(x => x.Type == type.Value);
        var count = await AsyncExecuter.CountAsync(query);
        var entries = await AsyncExecuter.ToListAsync(query.OrderBy(x => x.Type).ThenBy(x => x.Name).ThenBy(x => x.Id)
            .Skip(Math.Max(0, input.SkipCount)).Take(Math.Clamp(input.MaxResultCount, 1, 100)));
        return new PagedResultDto<CatalogEntryDto>(count, entries.Select(ToDto).ToList());
    }

    public async Task<CatalogEntryDto> CreateAsync(CreateCatalogEntryDto input)
    {
        var name = input.Name.Trim();
        if (name.Length == 0) throw new BusinessException("Rinoleggio:CatalogNameRequired");
        var normalized = name.ToUpperInvariant();
        var query = await _catalogEntries.GetQueryableAsync();
        if (await AsyncExecuter.AnyAsync(query.Where(x => x.Type == input.Type && x.NormalizedName == normalized)))
            throw new BusinessException("Rinoleggio:CatalogEntryAlreadyExists");
        var entry = await _catalogEntries.InsertAsync(new CatalogEntry(GuidGenerator.Create(), input.Type, name), autoSave: true);
        return ToDto(entry);
    }

    public async Task UpdateAsync(Guid id, UpdateCatalogEntryDto input)
    {
        var entry = await _catalogEntries.GetAsync(id);
        var name = input.Name.Trim();
        if (name.Length == 0) throw new BusinessException("Rinoleggio:CatalogNameRequired");
        var normalized = name.ToUpperInvariant();
        var catalogQuery = await _catalogEntries.GetQueryableAsync();
        if (await AsyncExecuter.AnyAsync(catalogQuery.Where(x => x.Id != id && x.Type == entry.Type && x.NormalizedName == normalized)))
            throw new BusinessException("Rinoleggio:CatalogEntryAlreadyExists");

        List<Listing> listings;
        switch (entry.Type)
        {
            case CatalogEntryType.RentalCompany: listings = await _listings.GetListAsync(x => x.RentalCompany == entry.Name); break;
            case CatalogEntryType.Brand: listings = await _listings.GetListAsync(x => x.Brand == entry.Name); break;
            case CatalogEntryType.Province: listings = await _listings.GetListAsync(x => x.Province == entry.Name); break;
            default: listings = new List<Listing>(); break;
        }
        foreach (var listing in listings)
        {
            switch (entry.Type)
            {
                case CatalogEntryType.RentalCompany: listing.RentalCompany = name; break;
                case CatalogEntryType.Brand: listing.Brand = name; break;
                case CatalogEntryType.Province: listing.Province = name; break;
            }
            await _listings.UpdateAsync(listing, autoSave: false);
        }

        entry.Name = name;
        entry.NormalizedName = normalized;
        await _catalogEntries.UpdateAsync(entry, autoSave: true);
    }

    public async Task SetActiveAsync(Guid id, bool isActive)
    {
        var entry = await _catalogEntries.GetAsync(id);
        entry.IsActive = isActive;
        await _catalogEntries.UpdateAsync(entry, autoSave: true);
    }

    public async Task DeleteAsync(Guid id)
    {
        var entry = await _catalogEntries.GetAsync(id);
        var listings = await _listings.GetQueryableAsync();
        var inUse = entry.Type switch
        {
            CatalogEntryType.RentalCompany => await AsyncExecuter.AnyAsync(listings.Where(x => x.RentalCompany == entry.Name)),
            CatalogEntryType.Brand => await AsyncExecuter.AnyAsync(listings.Where(x => x.Brand == entry.Name)),
            CatalogEntryType.Province => await AsyncExecuter.AnyAsync(listings.Where(x => x.Province == entry.Name)),
            _ => false
        };
        if (inUse) throw new BusinessException("Rinoleggio:CatalogEntryInUse");
        await _catalogEntries.DeleteAsync(entry, autoSave: true);
    }

    private static CatalogEntryDto ToDto(CatalogEntry entry) => new()
    {
        Id = entry.Id,
        Type = entry.Type,
        Name = entry.Name,
        IsActive = entry.IsActive
    };
}
