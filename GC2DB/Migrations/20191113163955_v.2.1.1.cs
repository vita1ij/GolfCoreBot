using Microsoft.EntityFrameworkCore.Migrations;

namespace GC2DB.Migrations
{
    public partial class v211 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastTask",
                table: "Games");

            migrationBuilder.AddColumn<string>(
                name: "EnCxId",
                table: "Tasks",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "LastTaskId",
                table: "Games",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EnCxId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "LastTaskId",
                table: "Games");

            migrationBuilder.AddColumn<string>(
                name: "LastTask",
                table: "Games",
                nullable: true);
        }
    }
}
