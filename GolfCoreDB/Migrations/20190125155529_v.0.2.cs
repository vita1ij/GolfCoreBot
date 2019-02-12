using Microsoft.EntityFrameworkCore.Migrations;

namespace GolfCoreDB.Migrations
{
    public partial class v02 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "LastStatisticsHash",
                table: "Games",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "LastStatisticsHash",
                table: "Games",
                nullable: true,
                oldClrType: typeof(int),
                oldNullable: true);
        }
    }
}
