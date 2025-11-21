using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PokerPlanning.Migrations
{
    /// <inheritdoc />
    public partial class AddEstimationUnit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EstimationUnit",
                table: "Games",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EstimationUnit",
                table: "Games");
        }
    }
}
