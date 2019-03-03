using Microsoft.EntityFrameworkCore.Migrations;

namespace BacklogManager.Migrations
{
    public partial class SuggestionTracking : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SelectedCount",
                table: "MediaObjects",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SuggestedCount",
                table: "MediaObjects",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SelectedCount",
                table: "MediaObjects");

            migrationBuilder.DropColumn(
                name: "SuggestedCount",
                table: "MediaObjects");
        }
    }
}
