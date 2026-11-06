using System;
using System.Threading.Tasks;
using Rinoleggio.Contracts;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;

namespace Rinoleggio.Listings;

public interface IListingImageAppService : IApplicationService
{
    Task<ListingImageDto> UploadAsync(Guid listingId, IRemoteStreamContent file);
    Task DeleteAsync(Guid listingId, Guid imageId);
}
