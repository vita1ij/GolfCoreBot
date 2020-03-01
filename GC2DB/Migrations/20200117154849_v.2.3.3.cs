using Microsoft.EntityFrameworkCore.Migrations;

namespace GC2DB.Migrations
{
    public partial class v233 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Exceptions");

            migrationBuilder.AddColumn<string>(
                name: "MirrorId",
                table: "Players",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MirrorPass",
                table: "Players",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MirrorId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "MirrorPass",
                table: "Players");

            migrationBuilder.CreateTable(
                name: "Exceptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Stack = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exceptions", x => x.Id);
                });
        }
    }
}
