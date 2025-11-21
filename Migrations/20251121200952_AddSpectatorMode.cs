using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PokerPlanning.Migrations
{
    /// <inheritdoc />
    public partial class AddSpectatorMode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSpectator",
                table: "Participants",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSpectator",
                table: "Participants");
        }
    }
}
