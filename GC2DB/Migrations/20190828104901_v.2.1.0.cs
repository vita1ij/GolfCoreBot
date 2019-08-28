using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GC2DB.Migrations
{
    public partial class v210 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChatId",
                table: "Locations");

            migrationBuilder.AddColumn<bool>(
                name: "isActive",
                table: "Players",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<long>(
                name: "Radius",
                table: "Games",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AddColumn<string>(
                name: "Guid",
                table: "Games",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "isActive",
                table: "Games",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "PlayersLocation",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ChatId = table.Column<long>(nullable: false),
                    LocationId = table.Column<int>(nullable: true),
                    Title = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayersLocation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayersLocation_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlayersLocation_LocationId",
                table: "PlayersLocation",
                column: "LocationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayersLocation");

            migrationBuilder.DropColumn(
                name: "isActive",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "Guid",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "isActive",
                table: "Games");

            migrationBuilder.AddColumn<long>(
                name: "ChatId",
                table: "Locations",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AlterColumn<long>(
                name: "Radius",
                table: "Games",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);
        }
    }
}
