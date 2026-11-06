using System;
using System.Linq;
using Rinoleggio.Listings;
using Volo.Abp.Guids;

namespace Rinoleggio.Contracts;

internal static class ListingFactory
{
    public static Listing Create(Guid ownerId, CreateUpdateContractDto input, IGuidGenerator guidGenerator, DateTime now, DateOnly today, bool publishNow)
    {
        var id = guidGenerator.Create();
        var listing = new Listing(id)
        {
            OwnerId = ownerId,
            Title = input.Title,
            DescriptionHtml = input.DescriptionHtml,
            SlugPath = ContractSlugGenerator.Generate(input.Title, id),
            ProvinceId = input.ProvinceId,
            TransferContributionType = input.TransferContributionType,
            TransferContributionAmount = input.TransferContributionAmount,
            Contract = new ListingContract
            {
                ListingId = id,
                RentalCompanyId = input.RentalCompanyId,
                StartDate = input.StartDate,
                EndDate = input.EndDate,
                MonthlyFee = input.MonthlyFee,
                VatIncluded = input.VatIncluded,
                InitialPayment = input.InitialPayment,
                TotalKm = input.TotalKm,
                CurrentKm = input.CurrentKm,
                RemainingKm = ListingKmCalculator.CalculateRemainingKm(input.TotalKm, input.CurrentKm),
                ExcessKmRate = input.ExcessKmRate,
                UnderKmRate = input.UnderKmRate
            },
            Vehicle = new ListingVehicle
            {
                ListingId = id,
                BrandId = input.BrandId,
                Model = input.Model,
                Version = input.Version,
                FuelType = input.FuelType,
                Transmission = input.Transmission,
                RegistrationYear = input.RegistrationYear
            },
            Services = new ListingServices
            {
                ListingId = id,
                MaintenanceIncluded = input.MaintenanceIncluded,
                InsuranceIncluded = input.InsuranceIncluded,
                RoadsideAssistanceIncluded = input.RoadsideAssistanceIncluded,
                ReplacementCarIncluded = input.ReplacementCarIncluded,
                TiresIncluded = input.TiresIncluded,
                RoadTaxIncluded = input.RoadTaxIncluded
            }
        };

        ApplyInsuranceTypes(listing, input);
        listing.ValidateTransferContribution();
        if (publishNow)
        {
            listing.Publish(now, today);
        }
        else
        {
            listing.SaveAsDraft();
        }

        return listing;
    }

    public static void Update(Listing listing, CreateUpdateContractDto input, DateOnly today)
    {
        if (!listing.CanBeEdited())
        {
            throw new Volo.Abp.BusinessException(RinoleggioDomainErrorCodes.ListingNotEditable);
        }

        listing.Title = input.Title;
        listing.DescriptionHtml = input.DescriptionHtml;
        listing.ProvinceId = input.ProvinceId;
        listing.TransferContributionType = input.TransferContributionType;
        listing.TransferContributionAmount = input.TransferContributionAmount;

        listing.Contract.RentalCompanyId = input.RentalCompanyId;
        listing.Contract.StartDate = input.StartDate;
        listing.Contract.EndDate = input.EndDate;
        listing.Contract.MonthlyFee = input.MonthlyFee;
        listing.Contract.VatIncluded = input.VatIncluded;
        listing.Contract.InitialPayment = input.InitialPayment;
        listing.Contract.UpdateKilometers(input.TotalKm, input.CurrentKm);
        listing.Contract.ExcessKmRate = input.ExcessKmRate;
        listing.Contract.UnderKmRate = input.UnderKmRate;

        listing.Vehicle.BrandId = input.BrandId;
        listing.Vehicle.Model = input.Model;
        listing.Vehicle.Version = input.Version;
        listing.Vehicle.FuelType = input.FuelType;
        listing.Vehicle.Transmission = input.Transmission;
        listing.Vehicle.RegistrationYear = input.RegistrationYear;

        listing.Services.MaintenanceIncluded = input.MaintenanceIncluded;
        listing.Services.InsuranceIncluded = input.InsuranceIncluded;
        listing.Services.RoadsideAssistanceIncluded = input.RoadsideAssistanceIncluded;
        listing.Services.ReplacementCarIncluded = input.ReplacementCarIncluded;
        listing.Services.TiresIncluded = input.TiresIncluded;
        listing.Services.RoadTaxIncluded = input.RoadTaxIncluded;

        listing.Services.InsuranceTypes.Clear();
        ApplyInsuranceTypes(listing, input);
        listing.ValidateTransferContribution();

        if (listing.IsPublished && listing.Contract.EndDate <= today)
        {
            throw new Volo.Abp.BusinessException(RinoleggioDomainErrorCodes.ListingEndDateMustBeFuture);
        }
    }

    private static void ApplyInsuranceTypes(Listing listing, CreateUpdateContractDto input)
    {
        if (!input.InsuranceIncluded || input.InsuranceKinds.Count == 0)
        {
            return;
        }

        foreach (var kind in input.InsuranceKinds.Distinct())
        {
            listing.Services.InsuranceTypes.Add(new ListingInsuranceType
            {
                ListingId = listing.Id,
                InsuranceKind = kind
            });
        }
    }
}
