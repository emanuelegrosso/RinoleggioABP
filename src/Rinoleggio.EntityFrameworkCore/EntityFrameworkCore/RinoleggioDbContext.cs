using Microsoft.EntityFrameworkCore;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Rinoleggio.Authors;
using Rinoleggio.Books;
using Rinoleggio.Leads;
using Rinoleggio.Listings;
using Rinoleggio.Lookups;
using Rinoleggio.Users;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.BlobStoring.Database.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.OpenIddict.EntityFrameworkCore;

namespace Rinoleggio.EntityFrameworkCore;

[ReplaceDbContext(typeof(IIdentityDbContext))]
[ConnectionStringName("Default")]
public class RinoleggioDbContext :
    AbpDbContext<RinoleggioDbContext>,
    IIdentityDbContext
{
    /* Add DbSet properties for your Aggregate Roots / Entities here. */

    public DbSet<Author> Authors { get; set; }

    public DbSet<Book> Books { get; set; }

    public DbSet<RentalCompany> RentalCompanies { get; set; }
    public DbSet<Brand> Brands { get; set; }
    public DbSet<Province> Provinces { get; set; }
    public DbSet<Listing> Listings { get; set; }
    public DbSet<ListingContract> ListingContracts { get; set; }
    public DbSet<ListingVehicle> ListingVehicles { get; set; }
    public DbSet<ListingServices> ListingServices { get; set; }
    public DbSet<ListingInsuranceType> ListingInsuranceTypes { get; set; }
    public DbSet<ListingImage> ListingImages { get; set; }
    public DbSet<Lead> Leads { get; set; }
    public DbSet<UserProfile> UserProfiles { get; set; }

    #region Entities from the modules

    /* Notice: We only implemented IIdentityProDbContext 
     * and replaced them for this DbContext. This allows you to perform JOIN
     * queries for the entities of these modules over the repositories easily. You
     * typically don't need that for other modules. But, if you need, you can
     * implement the DbContext interface of the needed module and use ReplaceDbContext
     * attribute just like IIdentityProDbContext .
     *
     * More info: Replacing a DbContext of a module ensures that the related module
     * uses this DbContext on runtime. Otherwise, it will use its own DbContext class.
     */

    // Identity
    public DbSet<IdentityUser> Users { get; set; }
    public DbSet<IdentityRole> Roles { get; set; }
    public DbSet<IdentityClaimType> ClaimTypes { get; set; }
    public DbSet<OrganizationUnit> OrganizationUnits { get; set; }
    public DbSet<IdentitySecurityLog> SecurityLogs { get; set; }
    public DbSet<IdentityLinkUser> LinkUsers { get; set; }
    public DbSet<IdentityUserDelegation> UserDelegations { get; set; }
    public DbSet<IdentitySession> Sessions { get; set; }

    #endregion

    public RinoleggioDbContext(DbContextOptions<RinoleggioDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        /* Include modules to your migration db context */

        builder.ConfigurePermissionManagement();
        builder.ConfigureSettingManagement();
        builder.ConfigureBackgroundJobs();
        builder.ConfigureAuditLogging();
        builder.ConfigureFeatureManagement();
        builder.ConfigureIdentity();
        builder.ConfigureOpenIddict();
        builder.ConfigureBlobStoring();

        builder.Entity<Author>(b =>
        {
            b.ToTable(RinoleggioConsts.DbTablePrefix + "Authors",
                RinoleggioConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props
            b.Property(x => x.Name).IsRequired().HasMaxLength(AuthorConsts.MaxNameLength);
            b.Property(x => x.ShortBio).HasMaxLength(AuthorConsts.MaxShortBioLength);
        });

        builder.Entity<Book>(b =>
        {
            b.ToTable(RinoleggioConsts.DbTablePrefix + "Books",
                RinoleggioConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props
            b.Property(x => x.Name).IsRequired().HasMaxLength(128);
            b.HasOne<Author>().WithMany().HasForeignKey(x => x.AuthorId).IsRequired();
        });

        builder.ConfigureRinoleggioListings();

        /* Configure your own tables/entities inside here */

        //builder.Entity<YourEntity>(b =>
        //{
        //    b.ToTable(RinoleggioConsts.DbTablePrefix + "YourEntities", RinoleggioConsts.DbSchema);
        //    b.ConfigureByConvention(); //auto configure for the base class props
        //    //...
        //});
    }
}
