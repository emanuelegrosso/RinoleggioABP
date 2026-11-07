using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rinoleggio.Migrations
{
    /// <inheritdoc />
    public partial class AddListingLeadTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppLeads",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ListingId = table.Column<Guid>(type: "TEXT", nullable: false),
                    BuyerId = table.Column<Guid>(type: "TEXT", nullable: false),
                    SellerId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Message = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    BuyerPhone = table.Column<string>(type: "TEXT", maxLength: 30, nullable: true),
                    PrivacyConsent = table.Column<bool>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    InternalNotes = table.Column<string>(type: "TEXT", maxLength: 4000, nullable: true),
                    ExtraProperties = table.Column<string>(type: "TEXT", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatorId = table.Column<Guid>(type: "TEXT", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppLeads", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppListings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    OwnerId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 4000, nullable: true),
                    ImageUrls = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    Province = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    RentalCompany = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    Brand = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    Model = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    Version = table.Column<string>(type: "TEXT", maxLength: 120, nullable: true),
                    FuelType = table.Column<int>(type: "INTEGER", nullable: false),
                    Transmission = table.Column<int>(type: "INTEGER", nullable: false),
                    RegistrationYear = table.Column<int>(type: "INTEGER", nullable: false),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    EndDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    MonthlyFee = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    VatIncluded = table.Column<bool>(type: "INTEGER", nullable: false),
                    InitialPayment = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    TotalKm = table.Column<int>(type: "INTEGER", nullable: false),
                    CurrentKm = table.Column<int>(type: "INTEGER", nullable: false),
                    RemainingKm = table.Column<int>(type: "INTEGER", nullable: false),
                    ExcessKmRate = table.Column<decimal>(type: "TEXT", precision: 18, scale: 3, nullable: true),
                    UnderKmRate = table.Column<decimal>(type: "TEXT", precision: 18, scale: 3, nullable: true),
                    ContributionType = table.Column<int>(type: "INTEGER", nullable: false),
                    ContributionAmount = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    MaintenanceIncluded = table.Column<bool>(type: "INTEGER", nullable: false),
                    InsuranceIncluded = table.Column<bool>(type: "INTEGER", nullable: false),
                    InsuranceKinds = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true),
                    RoadsideAssistanceIncluded = table.Column<bool>(type: "INTEGER", nullable: false),
                    ReplacementCarIncluded = table.Column<bool>(type: "INTEGER", nullable: false),
                    TiresIncluded = table.Column<bool>(type: "INTEGER", nullable: false),
                    RoadTaxIncluded = table.Column<bool>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    IsPublished = table.Column<bool>(type: "INTEGER", nullable: false),
                    StatusReason = table.Column<string>(type: "TEXT", nullable: true),
                    StatusChangedByBackOffice = table.Column<bool>(type: "INTEGER", nullable: false),
                    ExtraProperties = table.Column<string>(type: "TEXT", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatorId = table.Column<Guid>(type: "TEXT", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppListings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppLeads_ListingId_BuyerId_Status",
                table: "AppLeads",
                columns: new[] { "ListingId", "BuyerId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_AppLeads_SellerId_CreationTime",
                table: "AppLeads",
                columns: new[] { "SellerId", "CreationTime" });

            migrationBuilder.CreateIndex(
                name: "IX_AppListings_IsPublished_Status_EndDate",
                table: "AppListings",
                columns: new[] { "IsPublished", "Status", "EndDate" });

            migrationBuilder.CreateIndex(
                name: "IX_AppListings_OwnerId",
                table: "AppListings",
                column: "OwnerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppLeads");

            migrationBuilder.DropTable(
                name: "AppListings");
        }
    }
}
