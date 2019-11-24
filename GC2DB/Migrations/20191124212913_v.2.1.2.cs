using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GC2DB.Migrations
{
    public partial class v212 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
                name: "LatestTaskTime",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "PreviousTaskTime",
                table: "Games");
        }
    }
}
