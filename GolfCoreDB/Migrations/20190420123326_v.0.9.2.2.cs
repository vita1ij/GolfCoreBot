using Microsoft.EntityFrameworkCore.Migrations;

namespace GolfCoreDB.Migrations
{
    public partial class v0922 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Games_GameId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_GameId",
                table: "Tasks");

            migrationBuilder.AlterColumn<string>(
                name: "GameId",
                table: "Tasks",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Prefix",
                table: "Games",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Prefix",
                table: "Games");

            migrationBuilder.AlterColumn<string>(
                name: "GameId",
                table: "Tasks",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_GameId",
                table: "Tasks",
                column: "GameId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Games_GameId",
                table: "Tasks",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
