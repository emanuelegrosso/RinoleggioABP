using System.Linq;
using Rinoleggio.Listings;

namespace Rinoleggio.Contracts;

internal static class ContractMapper
{
    public static ContractSearchResultDto ToSearchResultDto(Listing listing)
    {
        var cover = listing.Images.OrderBy(i => i.SortOrder).FirstOrDefault();
        return new ContractSearchResultDto
        {
            Id = listing.Id,
            Title = listing.Title,
            SlugPath = listing.SlugPath,
            BrandName = listing.Vehicle.Brand!.Name,
            Model = listing.Vehicle.Model,
            Version = listing.Vehicle.Version,
            RegistrationYear = listing.Vehicle.RegistrationYear,
            FuelType = listing.Vehicle.FuelType,
            Transmission = listing.Vehicle.Transmission,
            RentalCompanyName = listing.Contract.RentalCompany!.Name,
            MonthlyFee = listing.Contract.MonthlyFee,
            InitialPayment = listing.Contract.InitialPayment,
            VatIncluded = listing.Contract.VatIncluded,
            RemainingKm = listing.Contract.RemainingKm,
            EndDate = listing.Contract.EndDate,
            TransferContributionType = listing.TransferContributionType,
            TransferContributionAmount = listing.TransferContributionAmount,
            ProvinceName = listing.Province!.Name,
            MaintenanceIncluded = listing.Services.MaintenanceIncluded,
            InsuranceIncluded = listing.Services.InsuranceIncluded,
            RoadsideAssistanceIncluded = listing.Services.RoadsideAssistanceIncluded,
            ReplacementCarIncluded = listing.Services.ReplacementCarIncluded,
            TiresIncluded = listing.Services.TiresIncluded,
            RoadTaxIncluded = listing.Services.RoadTaxIncluded,
            InsuranceKinds = listing.Services.InsuranceTypes.Select(t => t.InsuranceKind).ToList(),
            CoverImagePath = cover?.StoragePathOrBlobId,
            BusinessStatus = listing.BusinessStatus,
            IsPublished = listing.IsPublished,
            StatusReason = listing.StatusReason,
            StatusChangedBy = listing.StatusChangedBy
        };
    }

    public static ContractDetailDto ToDetailDto(Listing listing)
    {
        var dto = ToSearchResultDto(listing);
        return new ContractDetailDto
        {
            Id = dto.Id,
            Title = dto.Title,
            SlugPath = dto.SlugPath,
            BrandName = dto.BrandName,
            Model = dto.Model,
            Version = dto.Version,
            RegistrationYear = dto.RegistrationYear,
            FuelType = dto.FuelType,
            Transmission = dto.Transmission,
            RentalCompanyName = dto.RentalCompanyName,
            MonthlyFee = dto.MonthlyFee,
            InitialPayment = dto.InitialPayment,
            VatIncluded = dto.VatIncluded,
            RemainingKm = dto.RemainingKm,
            EndDate = dto.EndDate,
            TransferContributionType = dto.TransferContributionType,
            TransferContributionAmount = dto.TransferContributionAmount,
            ProvinceName = dto.ProvinceName,
            MaintenanceIncluded = dto.MaintenanceIncluded,
            InsuranceIncluded = dto.InsuranceIncluded,
            RoadsideAssistanceIncluded = dto.RoadsideAssistanceIncluded,
            ReplacementCarIncluded = dto.ReplacementCarIncluded,
            TiresIncluded = dto.TiresIncluded,
            RoadTaxIncluded = dto.RoadTaxIncluded,
            InsuranceKinds = dto.InsuranceKinds,
            CoverImagePath = dto.CoverImagePath,
            DescriptionHtml = listing.DescriptionHtml,
            ProvinceId = listing.ProvinceId,
            BrandId = listing.Vehicle.BrandId,
            RentalCompanyId = listing.Contract.RentalCompanyId,
            StartDate = listing.Contract.StartDate,
            TotalKm = listing.Contract.TotalKm,
            CurrentKm = listing.Contract.CurrentKm,
            ExcessKmRate = listing.Contract.ExcessKmRate,
            UnderKmRate = listing.Contract.UnderKmRate,
            BusinessStatus = listing.BusinessStatus,
            IsPublished = listing.IsPublished,
            OwnerId = listing.OwnerId,
            Images = listing.Images.OrderBy(i => i.SortOrder).Select(i => new ListingImageDto
            {
                Id = i.Id,
                StoragePathOrBlobId = i.StoragePathOrBlobId,
                SortOrder = i.SortOrder
            }).ToList()
        };
    }
}
