using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Rinoleggio.Contracts;
using Rinoleggio.Permissions;
using Volo.Abp;
using Volo.Abp.Authorization;
using Volo.Abp.BlobStoring;
using Volo.Abp.Content;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;

namespace Rinoleggio.Listings;

[Authorize]
public class ListingImageAppService : RinoleggioAppService, IListingImageAppService
{
    private const long MaxFileSizeBytes = 5 * 1024 * 1024;

    private readonly IRepository<Listing, Guid> _listingRepository;
    private readonly IRepository<ListingImage, Guid> _imageRepository;
    private readonly IBlobContainer<ListingImagesBlobContainer> _blobContainer;
    private readonly IGuidGenerator _guidGenerator;

    public ListingImageAppService(
        IRepository<Listing, Guid> listingRepository,
        IRepository<ListingImage, Guid> imageRepository,
        IBlobContainer<ListingImagesBlobContainer> blobContainer,
        IGuidGenerator guidGenerator)
    {
        _listingRepository = listingRepository;
        _imageRepository = imageRepository;
        _blobContainer = blobContainer;
        _guidGenerator = guidGenerator;
    }

    [Authorize(RinoleggioPermissions.Contracts.Edit)]
    public async Task<ListingImageDto> UploadAsync(Guid listingId, IRemoteStreamContent file)
    {
        var listing = await _listingRepository.GetAsync(listingId);
        await EnsureCanEditListingAsync(listing);

        var count = await _imageRepository.CountAsync(x => x.ListingId == listingId);
        if (count >= ListingConsts.MaxListingImages)
        {
            throw new BusinessException(RinoleggioDomainErrorCodes.ListingImageLimitExceeded);
        }

        if (file.ContentLength.HasValue && file.ContentLength.Value > MaxFileSizeBytes)
        {
            throw new BusinessException("Rinoleggio:ImageTooLarge");
        }

        var extension = Path.GetExtension(file.FileName ?? string.Empty).ToLowerInvariant();
        if (extension is not (".jpg" or ".jpeg" or ".png" or ".webp"))
        {
            throw new BusinessException("Rinoleggio:InvalidImageType");
        }

        var blobName = $"{listingId:N}/{_guidGenerator.Create():N}{extension}";
        await using (var stream = file.GetStream())
        {
            await _blobContainer.SaveAsync(blobName, stream, overrideExisting: true);
        }

        var sortOrder = (byte)Math.Min(count, 255);
        var image = new ListingImage
        {
            ListingId = listingId,
            StoragePathOrBlobId = blobName,
            SortOrder = sortOrder
        };

        await _imageRepository.InsertAsync(image, autoSave: true);

        return new ListingImageDto
        {
            Id = image.Id,
            StoragePathOrBlobId = image.StoragePathOrBlobId,
            SortOrder = image.SortOrder
        };
    }

    [Authorize(RinoleggioPermissions.Contracts.Edit)]
    public async Task DeleteAsync(Guid listingId, Guid imageId)
    {
        var listing = await _listingRepository.GetAsync(listingId);
        await EnsureCanEditListingAsync(listing);

        var image = await _imageRepository.GetAsync(imageId);
        if (image.ListingId != listingId)
        {
            throw new BusinessException("Rinoleggio:ImageNotFound");
        }

        await _blobContainer.DeleteAsync(image.StoragePathOrBlobId);
        await _imageRepository.DeleteAsync(image);
    }

    private async Task EnsureCanEditListingAsync(Listing listing)
    {
        if (listing.OwnerId == CurrentUser.Id)
        {
            return;
        }

        if (await AuthorizationService.IsGrantedAsync(RinoleggioPermissions.Contracts.ManageAll))
        {
            return;
        }

        throw new AbpAuthorizationException();
    }
}
