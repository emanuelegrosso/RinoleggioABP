using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rinoleggio.Migrations
{
    /// <inheritdoc />
    public partial class AddFavorites : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppFavorites",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ListingId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ExtraProperties = table.Column<string>(type: "TEXT", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatorId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppFavorites", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppFavorites_UserId",
                table: "AppFavorites",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AppFavorites_UserId_ListingId",
                table: "AppFavorites",
                columns: new[] { "UserId", "ListingId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppFavorites");
        }
    }
}
