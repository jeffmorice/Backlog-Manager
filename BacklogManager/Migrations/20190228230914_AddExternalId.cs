using Microsoft.EntityFrameworkCore.Migrations;

namespace BacklogManager.Migrations
{
    public partial class AddExternalId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExternalId",
                table: "MediaObjects",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExternalId",
                table: "MediaObjects");
        }
    }
}
