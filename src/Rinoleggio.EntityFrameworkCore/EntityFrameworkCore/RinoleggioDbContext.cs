using Microsoft.EntityFrameworkCore;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Rinoleggio.Authors;
using Rinoleggio.Books;
using Rinoleggio.Listings;
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
using Volo.Abp.TenantManagement;
using Volo.Abp.TenantManagement.EntityFrameworkCore;

namespace Rinoleggio.EntityFrameworkCore;

[ReplaceDbContext(typeof(IIdentityDbContext))]
[ReplaceDbContext(typeof(ITenantManagementDbContext))]
[ConnectionStringName("Default")]
public class RinoleggioDbContext :
    AbpDbContext<RinoleggioDbContext>,
    ITenantManagementDbContext,
    IIdentityDbContext
{
    /* Add DbSet properties for your Aggregate Roots / Entities here. */

    public DbSet<Author> Authors { get; set; }

    public DbSet<Book> Books { get; set; }

    public DbSet<Listing> Listings { get; set; }
    public DbSet<Lead> Leads { get; set; }
    public DbSet<Favorite> Favorites { get; set; }
    public DbSet<CatalogEntry> CatalogEntries { get; set; }

    #region Entities from the modules

    /* Notice: We only implemented IIdentityProDbContext and ISaasDbContext
     * and replaced them for this DbContext. This allows you to perform JOIN
     * queries for the entities of these modules over the repositories easily. You
     * typically don't need that for other modules. But, if you need, you can
     * implement the DbContext interface of the needed module and use ReplaceDbContext
     * attribute just like IIdentityProDbContext and ISaasDbContext.
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

    // Tenant Management
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<TenantConnectionString> TenantConnectionStrings { get; set; }

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
        builder.ConfigureTenantManagement();
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

        builder.Entity<Listing>(b =>
        {
            b.ToTable(RinoleggioConsts.DbTablePrefix + "Listings", RinoleggioConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.Title).IsRequired().HasMaxLength(200);
            b.Property(x => x.Description).HasMaxLength(4000);
            b.Property(x => x.ImageUrls).HasMaxLength(2000);
            b.Property(x => x.Province).IsRequired().HasMaxLength(80);
            b.Property(x => x.RentalCompany).IsRequired().HasMaxLength(120);
            b.Property(x => x.Brand).IsRequired().HasMaxLength(80);
            b.Property(x => x.Model).IsRequired().HasMaxLength(80);
            b.Property(x => x.Version).HasMaxLength(120);
            b.Property(x => x.InsuranceKinds).HasMaxLength(300);
            b.Property(x => x.MonthlyFee).HasPrecision(18, 2);
            b.Property(x => x.InitialPayment).HasPrecision(18, 2);
            b.Property(x => x.ExcessKmRate).HasPrecision(18, 3);
            b.Property(x => x.UnderKmRate).HasPrecision(18, 3);
            b.Property(x => x.ContributionAmount).HasPrecision(18, 2);
            b.HasIndex(x => new { x.IsPublished, x.Status, x.EndDate });
            b.HasIndex(x => x.OwnerId);
        });

        builder.Entity<Lead>(b =>
        {
            b.ToTable(RinoleggioConsts.DbTablePrefix + "Leads", RinoleggioConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.Message).IsRequired().HasMaxLength(1000);
            b.Property(x => x.BuyerPhone).HasMaxLength(30);
            b.Property(x => x.InternalNotes).HasMaxLength(4000);
            b.Property(x => x.OutcomeNotes).HasMaxLength(1000);
            b.HasIndex(x => new { x.ListingId, x.BuyerId, x.Status });
            b.HasIndex(x => new { x.SellerId, x.CreationTime });
        });

        builder.Entity<Favorite>(b =>
        {
            b.ToTable(RinoleggioConsts.DbTablePrefix + "Favorites", RinoleggioConsts.DbSchema);
            b.ConfigureByConvention();
            b.HasIndex(x => new { x.UserId, x.ListingId }).IsUnique();
            b.HasIndex(x => x.UserId);
        });

        builder.Entity<CatalogEntry>(b =>
        {
            b.ToTable(RinoleggioConsts.DbTablePrefix + "CatalogEntries", RinoleggioConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.Name).IsRequired().HasMaxLength(120);
            b.Property(x => x.NormalizedName).IsRequired().HasMaxLength(120);
            b.HasIndex(x => new { x.Type, x.NormalizedName }).IsUnique();
            b.HasIndex(x => new { x.Type, x.IsActive });
        });

        /* Configure your own tables/entities inside here */

        //builder.Entity<YourEntity>(b =>
        //{
        //    b.ToTable(RinoleggioConsts.DbTablePrefix + "YourEntities", RinoleggioConsts.DbSchema);
        //    b.ConfigureByConvention(); //auto configure for the base class props
        //    //...
        //});
    }
}
