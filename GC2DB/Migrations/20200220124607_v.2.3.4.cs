using Microsoft.EntityFrameworkCore.Migrations;

namespace GC2DB.Migrations
{
    public partial class v234 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FullHtmlMain",
                table: "Tasks",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "HtmlAdditional",
                table: "Tasks",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "HtmlLeft",
                table: "Tasks",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FullHtmlMain",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "HtmlAdditional",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "HtmlLeft",
                table: "Tasks");
        }
    }
}
