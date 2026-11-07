using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rinoleggio.Migrations
{
    /// <inheritdoc />
    public partial class AddCatalogsAndDeletedListingRestore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StatusBeforeDeletion",
                table: "AppListings",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "StatusChangedByBackOfficeBeforeDeletion",
                table: "AppListings",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "WasPublishedBeforeDeletion",
                table: "AppListings",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AppCatalogEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    NormalizedName = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    ExtraProperties = table.Column<string>(type: "TEXT", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatorId = table.Column<Guid>(type: "TEXT", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppCatalogEntries", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppCatalogEntries_Type_IsActive",
                table: "AppCatalogEntries",
                columns: new[] { "Type", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_AppCatalogEntries_Type_NormalizedName",
                table: "AppCatalogEntries",
                columns: new[] { "Type", "NormalizedName" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppCatalogEntries");

            migrationBuilder.DropColumn(
                name: "StatusBeforeDeletion",
                table: "AppListings");

            migrationBuilder.DropColumn(
                name: "StatusChangedByBackOfficeBeforeDeletion",
                table: "AppListings");

            migrationBuilder.DropColumn(
                name: "WasPublishedBeforeDeletion",
                table: "AppListings");
        }
    }
}
