using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GC2DB.Migrations
{
    public partial class v231 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Players_Games_GameId",
                table: "Players");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayersLocation_Locations_LocationId",
                table: "PlayersLocation");

            migrationBuilder.DropIndex(
                name: "IX_PlayersLocation_LocationId",
                table: "PlayersLocation");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "PlayersLocation");

            migrationBuilder.RenameColumn(
                name: "isActive",
                table: "Players",
                newName: "IsActive");

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "Tasks",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LocId",
                table: "PlayersLocation",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<long>(
                name: "GameId",
                table: "Players",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "Lists",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Guid",
                table: "Games",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomEnCxDomain",
                table: "Games",
                nullable: true);

            migrationBuilder.AlterColumn<byte[]>(
                name: "Content",
                table: "DataFiles",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayersLocation_LocId",
                table: "PlayersLocation",
                column: "LocId");

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Games_GameId",
                table: "Players",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayersLocation_Locations_LocId",
                table: "PlayersLocation",
                column: "LocId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Players_Games_GameId",
                table: "Players");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayersLocation_Locations_LocId",
                table: "PlayersLocation");

            migrationBuilder.DropIndex(
                name: "IX_PlayersLocation_LocId",
                table: "PlayersLocation");

            migrationBuilder.DropColumn(
                name: "LocId",
                table: "PlayersLocation");

            migrationBuilder.DropColumn(
                name: "CustomEnCxDomain",
                table: "Games");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "Players",
                newName: "isActive");

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "Tasks",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<int>(
                name: "LocationId",
                table: "PlayersLocation",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "GameId",
                table: "Players",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "Lists",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Guid",
                table: "Games",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<byte[]>(
                name: "Content",
                table: "DataFiles",
                type: "varbinary(max)",
                nullable: true,
                oldClrType: typeof(byte[]));

            migrationBuilder.CreateIndex(
                name: "IX_PlayersLocation_LocationId",
                table: "PlayersLocation",
                column: "LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Games_GameId",
                table: "Players",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayersLocation_Locations_LocationId",
                table: "PlayersLocation",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
