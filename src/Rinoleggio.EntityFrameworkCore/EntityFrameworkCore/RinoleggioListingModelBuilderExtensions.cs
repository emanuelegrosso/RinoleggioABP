using Microsoft.EntityFrameworkCore;
using Rinoleggio.Leads;
using Rinoleggio.Listings;
using Rinoleggio.Lookups;
using Rinoleggio.Users;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Rinoleggio.EntityFrameworkCore;

public static class RinoleggioListingModelBuilderExtensions
{
    public static void ConfigureRinoleggioListings(this ModelBuilder builder)
    {
        builder.Entity<RentalCompany>(b =>
        {
            b.ToTable(RinoleggioConsts.DbTablePrefix + "RentalCompanies", RinoleggioConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.Name).IsRequired().HasMaxLength(LookupConsts.MaxRentalCompanyNameLength);
            b.Property(x => x.WebsiteUrl).HasMaxLength(LookupConsts.MaxWebsiteUrlLength);
            b.HasIndex(x => x.Name).IsUnique();
        });

        builder.Entity<Brand>(b =>
        {
            b.ToTable(RinoleggioConsts.DbTablePrefix + "Brands", RinoleggioConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.Name).IsRequired().HasMaxLength(LookupConsts.MaxBrandNameLength);
        });

        builder.Entity<Province>(b =>
        {
            b.ToTable(RinoleggioConsts.DbTablePrefix + "Provinces", RinoleggioConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.Name).IsRequired().HasMaxLength(LookupConsts.MaxProvinceNameLength);
            b.Property(x => x.Code).IsRequired().HasMaxLength(LookupConsts.ProvinceCodeLength);
            b.HasIndex(x => x.Code).IsUnique();
        });

        builder.Entity<Listing>(b =>
        {
            b.ToTable(RinoleggioConsts.DbTablePrefix + "Listings", RinoleggioConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.Title).IsRequired().HasMaxLength(ListingConsts.MaxTitleLength);
            b.Property(x => x.SlugPath).IsRequired().HasMaxLength(ListingConsts.MaxSlugPathLength);
            b.Property(x => x.StatusReason).HasMaxLength(ListingConsts.MaxStatusReasonLength);
            b.Property(x => x.TransferContributionAmount).HasPrecision(18, 2);
            b.HasIndex(x => x.SlugPath).IsUnique();
            b.HasIndex(x => new { x.IsPublished, x.BusinessStatus, x.ProvinceId });
            b.HasIndex(x => x.OwnerId);
            b.HasOne(x => x.Province).WithMany().HasForeignKey(x => x.ProvinceId).IsRequired();
            b.HasOne(x => x.Contract).WithOne().HasForeignKey<ListingContract>(x => x.ListingId).OnDelete(DeleteBehavior.Cascade);
            b.HasOne(x => x.Vehicle).WithOne().HasForeignKey<ListingVehicle>(x => x.ListingId).OnDelete(DeleteBehavior.Cascade);
            b.HasOne(x => x.Services).WithOne().HasForeignKey<ListingServices>(x => x.ListingId).OnDelete(DeleteBehavior.Cascade);
            b.HasMany(x => x.Images).WithOne().HasForeignKey(x => x.ListingId).OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<ListingContract>(b =>
        {
            b.ToTable(RinoleggioConsts.DbTablePrefix + "ListingContracts", RinoleggioConsts.DbSchema);
            b.HasKey(x => x.ListingId);
            b.Property(x => x.MonthlyFee).HasPrecision(18, 2);
            b.Property(x => x.InitialPayment).HasPrecision(18, 2);
            b.Property(x => x.ExcessKmRate).HasPrecision(18, 3);
            b.Property(x => x.UnderKmRate).HasPrecision(18, 3);
            b.HasIndex(x => x.RentalCompanyId);
            b.HasIndex(x => x.EndDate);
            b.HasOne(x => x.RentalCompany).WithMany().HasForeignKey(x => x.RentalCompanyId).IsRequired();
        });

        builder.Entity<ListingVehicle>(b =>
        {
            b.ToTable(RinoleggioConsts.DbTablePrefix + "ListingVehicles", RinoleggioConsts.DbSchema);
            b.HasKey(x => x.ListingId);
            b.Property(x => x.Model).IsRequired().HasMaxLength(ListingConsts.MaxModelLength);
            b.Property(x => x.Version).HasMaxLength(ListingConsts.MaxVersionLength);
            b.HasIndex(x => x.BrandId);
            b.HasOne(x => x.Brand).WithMany().HasForeignKey(x => x.BrandId).IsRequired();
        });

        builder.Entity<ListingServices>(b =>
        {
            b.ToTable(RinoleggioConsts.DbTablePrefix + "ListingServices", RinoleggioConsts.DbSchema);
            b.HasKey(x => x.ListingId);
            b.HasMany(x => x.InsuranceTypes).WithOne().HasForeignKey(x => x.ListingId).OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<ListingInsuranceType>(b =>
        {
            b.ToTable(RinoleggioConsts.DbTablePrefix + "ListingInsuranceTypes", RinoleggioConsts.DbSchema);
            b.HasKey(x => new { x.ListingId, x.InsuranceKind });
        });

        builder.Entity<ListingImage>(b =>
        {
            b.ToTable(RinoleggioConsts.DbTablePrefix + "ListingImages", RinoleggioConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.StoragePathOrBlobId).IsRequired().HasMaxLength(ListingConsts.MaxImageStoragePathLength);
            b.HasIndex(x => x.ListingId);
        });

        builder.Entity<Lead>(b =>
        {
            b.ToTable(RinoleggioConsts.DbTablePrefix + "Leads", RinoleggioConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.Title).HasMaxLength(LeadConsts.MaxTitleLength);
            b.Property(x => x.Message).IsRequired().HasMaxLength(LeadConsts.MaxMessageLength);
            b.Property(x => x.Phone).HasMaxLength(LeadConsts.MaxPhoneLength);
            b.HasIndex(x => x.ListingId);
            b.HasIndex(x => x.SellerId);
            b.HasIndex(x => new { x.BuyerId, x.ListingId });
            b.HasOne(x => x.Listing).WithMany().HasForeignKey(x => x.ListingId).IsRequired();
        });

        builder.Entity<UserProfile>(b =>
        {
            b.ToTable(RinoleggioConsts.DbTablePrefix + "UserProfiles", RinoleggioConsts.DbSchema);
            b.HasKey(x => x.Id);
            b.Property(x => x.Id).HasColumnName("UserId");
            b.Property(x => x.FullName).IsRequired().HasMaxLength(UserProfileConsts.MaxFullNameLength);
            b.Property(x => x.PhoneNumber).HasMaxLength(UserProfileConsts.MaxPhoneNumberLength);
        });
    }
}
