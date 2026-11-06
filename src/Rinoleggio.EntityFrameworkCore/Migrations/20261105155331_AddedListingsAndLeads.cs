using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rinoleggio.Migrations
{
    /// <inheritdoc />
    public partial class AddedListingsAndLeads : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppBrands",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatorId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppBrands", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppProvinces",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    Code = table.Column<string>(type: "TEXT", maxLength: 2, nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatorId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppProvinces", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppRentalCompanies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    WebsiteUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatorId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppRentalCompanies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppUserProfiles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    FullName = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    PhoneNumber = table.Column<string>(type: "TEXT", maxLength: 30, nullable: true),
                    AccountType = table.Column<int>(type: "INTEGER", nullable: false),
                    PrivacyConsent = table.Column<bool>(type: "INTEGER", nullable: false),
                    PrivacyConsentAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUserProfiles", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "AppListings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    OwnerId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    DescriptionHtml = table.Column<string>(type: "TEXT", nullable: true),
                    SlugPath = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    ProvinceId = table.Column<Guid>(type: "TEXT", nullable: false),
                    TransferContributionType = table.Column<int>(type: "INTEGER", nullable: false),
                    TransferContributionAmount = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    BusinessStatus = table.Column<int>(type: "INTEGER", nullable: false),
                    StatusReason = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    StatusChangedBy = table.Column<int>(type: "INTEGER", nullable: true),
                    IsPublished = table.Column<bool>(type: "INTEGER", nullable: false),
                    PublishedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ExtraProperties = table.Column<string>(type: "TEXT", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatorId = table.Column<Guid>(type: "TEXT", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "TEXT", nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "TEXT", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppListings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppListings_AppProvinces_ProvinceId",
                        column: x => x.ProvinceId,
                        principalTable: "AppProvinces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppLeads",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ListingId = table.Column<Guid>(type: "TEXT", nullable: false),
                    BuyerId = table.Column<Guid>(type: "TEXT", nullable: false),
                    SellerId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Message = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    Phone = table.Column<string>(type: "TEXT", maxLength: 30, nullable: true),
                    PrivacyConsent = table.Column<bool>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    BackofficeNotes = table.Column<string>(type: "TEXT", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatorId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppLeads", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppLeads_AppListings_ListingId",
                        column: x => x.ListingId,
                        principalTable: "AppListings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppListingContracts",
                columns: table => new
                {
                    ListingId = table.Column<Guid>(type: "TEXT", nullable: false),
                    RentalCompanyId = table.Column<Guid>(type: "TEXT", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    EndDate = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    MonthlyFee = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    VatIncluded = table.Column<bool>(type: "INTEGER", nullable: false),
                    InitialPayment = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    TotalKm = table.Column<int>(type: "INTEGER", nullable: false),
                    CurrentKm = table.Column<int>(type: "INTEGER", nullable: false),
                    RemainingKm = table.Column<int>(type: "INTEGER", nullable: false),
                    ExcessKmRate = table.Column<decimal>(type: "TEXT", precision: 18, scale: 3, nullable: true),
                    UnderKmRate = table.Column<decimal>(type: "TEXT", precision: 18, scale: 3, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppListingContracts", x => x.ListingId);
                    table.ForeignKey(
                        name: "FK_AppListingContracts_AppListings_ListingId",
                        column: x => x.ListingId,
                        principalTable: "AppListings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppListingContracts_AppRentalCompanies_RentalCompanyId",
                        column: x => x.RentalCompanyId,
                        principalTable: "AppRentalCompanies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppListingImages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ListingId = table.Column<Guid>(type: "TEXT", nullable: false),
                    StoragePathOrBlobId = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    SortOrder = table.Column<byte>(type: "INTEGER", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatorId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppListingImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppListingImages_AppListings_ListingId",
                        column: x => x.ListingId,
                        principalTable: "AppListings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppListingServices",
                columns: table => new
                {
                    ListingId = table.Column<Guid>(type: "TEXT", nullable: false),
                    MaintenanceIncluded = table.Column<bool>(type: "INTEGER", nullable: false),
                    InsuranceIncluded = table.Column<bool>(type: "INTEGER", nullable: false),
                    RoadsideAssistanceIncluded = table.Column<bool>(type: "INTEGER", nullable: false),
                    ReplacementCarIncluded = table.Column<bool>(type: "INTEGER", nullable: false),
                    TiresIncluded = table.Column<bool>(type: "INTEGER", nullable: false),
                    RoadTaxIncluded = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppListingServices", x => x.ListingId);
                    table.ForeignKey(
                        name: "FK_AppListingServices_AppListings_ListingId",
                        column: x => x.ListingId,
                        principalTable: "AppListings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppListingVehicles",
                columns: table => new
                {
                    ListingId = table.Column<Guid>(type: "TEXT", nullable: false),
                    BrandId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Model = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    Version = table.Column<string>(type: "TEXT", maxLength: 120, nullable: true),
                    FuelType = table.Column<int>(type: "INTEGER", nullable: false),
                    Transmission = table.Column<int>(type: "INTEGER", nullable: false),
                    RegistrationYear = table.Column<short>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppListingVehicles", x => x.ListingId);
                    table.ForeignKey(
                        name: "FK_AppListingVehicles_AppBrands_BrandId",
                        column: x => x.BrandId,
                        principalTable: "AppBrands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppListingVehicles_AppListings_ListingId",
                        column: x => x.ListingId,
                        principalTable: "AppListings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppListingInsuranceTypes",
                columns: table => new
                {
                    ListingId = table.Column<Guid>(type: "TEXT", nullable: false),
                    InsuranceKind = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppListingInsuranceTypes", x => new { x.ListingId, x.InsuranceKind });
                    table.ForeignKey(
                        name: "FK_AppListingInsuranceTypes_AppListingServices_ListingId",
                        column: x => x.ListingId,
                        principalTable: "AppListingServices",
                        principalColumn: "ListingId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppLeads_BuyerId_ListingId",
                table: "AppLeads",
                columns: new[] { "BuyerId", "ListingId" });

            migrationBuilder.CreateIndex(
                name: "IX_AppLeads_ListingId",
                table: "AppLeads",
                column: "ListingId");

            migrationBuilder.CreateIndex(
                name: "IX_AppLeads_SellerId",
                table: "AppLeads",
                column: "SellerId");

            migrationBuilder.CreateIndex(
                name: "IX_AppListingContracts_EndDate",
                table: "AppListingContracts",
                column: "EndDate");

            migrationBuilder.CreateIndex(
                name: "IX_AppListingContracts_RentalCompanyId",
                table: "AppListingContracts",
                column: "RentalCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_AppListingImages_ListingId",
                table: "AppListingImages",
                column: "ListingId");

            migrationBuilder.CreateIndex(
                name: "IX_AppListings_IsPublished_BusinessStatus_ProvinceId",
                table: "AppListings",
                columns: new[] { "IsPublished", "BusinessStatus", "ProvinceId" });

            migrationBuilder.CreateIndex(
                name: "IX_AppListings_OwnerId",
                table: "AppListings",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_AppListings_ProvinceId",
                table: "AppListings",
                column: "ProvinceId");

            migrationBuilder.CreateIndex(
                name: "IX_AppListings_SlugPath",
                table: "AppListings",
                column: "SlugPath",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppListingVehicles_BrandId",
                table: "AppListingVehicles",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_AppProvinces_Code",
                table: "AppProvinces",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppRentalCompanies_Name",
                table: "AppRentalCompanies",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppLeads");

            migrationBuilder.DropTable(
                name: "AppListingContracts");

            migrationBuilder.DropTable(
                name: "AppListingImages");

            migrationBuilder.DropTable(
                name: "AppListingInsuranceTypes");

            migrationBuilder.DropTable(
                name: "AppListingVehicles");

            migrationBuilder.DropTable(
                name: "AppUserProfiles");

            migrationBuilder.DropTable(
                name: "AppRentalCompanies");

            migrationBuilder.DropTable(
                name: "AppListingServices");

            migrationBuilder.DropTable(
                name: "AppBrands");

            migrationBuilder.DropTable(
                name: "AppListings");

            migrationBuilder.DropTable(
                name: "AppProvinces");
        }
    }
}
