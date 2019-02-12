using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GolfCoreDB.Migrations
{
    public partial class v09 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "LastStatistics",
                table: "Games",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastStatisticsHash",
                table: "Games",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "GetStatistics",
                table: "GameParticipant",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "MonitorStatistics",
                table: "GameParticipant",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastStatistics",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "LastStatisticsHash",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "GetStatistics",
                table: "GameParticipant");

            migrationBuilder.DropColumn(
                name: "MonitorStatistics",
                table: "GameParticipant");
        }
    }
}
