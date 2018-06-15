using Microsoft.EntityFrameworkCore.Migrations;

namespace GolfCoreDB.Migrations
{
    public partial class v05 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LastTask",
                table: "Games",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Login",
                table: "Games",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Games",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastTask",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "Login",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "Password",
                table: "Games");
        }
    }
}
