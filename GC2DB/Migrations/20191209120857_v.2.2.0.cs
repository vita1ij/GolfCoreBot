using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GC2DB.Migrations
{
    public partial class v220 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Games_GameId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_GameId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "LastTask",
                table: "Games");

            migrationBuilder.AddColumn<string>(
                name: "EnCxId",
                table: "Tasks",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Cookies",
                table: "Games",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HadErrorsWhileLogin",
                table: "Games",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HadErrorsWhileReading",
                table: "Games",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "LastTaskId",
                table: "Games",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LatestTaskTime",
                table: "Games",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "PreviousTaskTime",
                table: "Games",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "TryToLogIn",
                table: "Games",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Exceptions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Message = table.Column<string>(nullable: true),
                    Stack = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exceptions", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Exceptions");

            migrationBuilder.DropColumn(
                name: "EnCxId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "Cookies",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "HadErrorsWhileLogin",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "HadErrorsWhileReading",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "LastTaskId",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "LatestTaskTime",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "PreviousTaskTime",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "TryToLogIn",
                table: "Games");

            migrationBuilder.AddColumn<string>(
                name: "LastTask",
                table: "Games",
                type: "nvarchar(max)",
                nullable: true);

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
