using Microsoft.EntityFrameworkCore.Migrations;

namespace GC2DB.Migrations
{
    public partial class v2122 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Games_GameId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_GameId",
                table: "Tasks");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
