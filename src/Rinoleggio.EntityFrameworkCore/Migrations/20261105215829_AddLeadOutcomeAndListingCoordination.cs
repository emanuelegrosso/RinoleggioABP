using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rinoleggio.Migrations
{
    /// <inheritdoc />
    public partial class AddLeadOutcomeAndListingCoordination : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Outcome",
                table: "AppLeads",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "OutcomeNotes",
                table: "AppLeads",
                type: "TEXT",
                maxLength: 1000,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Outcome",
                table: "AppLeads");

            migrationBuilder.DropColumn(
                name: "OutcomeNotes",
                table: "AppLeads");
        }
    }
}
