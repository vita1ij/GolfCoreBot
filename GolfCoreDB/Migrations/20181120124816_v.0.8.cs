using Microsoft.EntityFrameworkCore.Migrations;

namespace GolfCoreDB.Migrations
{
    public partial class v08 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EnCxId",
                table: "Games",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Href",
                table: "Games",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Games",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EnCxId",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "Href",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Games");
        }
    }
}
