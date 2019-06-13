using Microsoft.EntityFrameworkCore.Migrations;

namespace GolfCoreDB.Migrations
{
    public partial class v091 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Settings");

            migrationBuilder.AddColumn<string>(
                name: "EnCxType",
                table: "Games",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EnCxType",
                table: "Games");

            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    Name = table.Column<string>(nullable: false),
                    ChatId = table.Column<long>(nullable: false),
                    GameId = table.Column<string>(nullable: true),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.Name);
                });
        }
    }
}
