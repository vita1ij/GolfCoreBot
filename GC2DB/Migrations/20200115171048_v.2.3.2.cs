using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GC2DB.Migrations
{
    public partial class v232 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Errors",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Text = table.Column<string>(nullable: false),
                    Stack = table.Column<string>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false),
                    Chat = table.Column<long>(nullable: true),
                    Sender = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Errors", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Errors");
        }
    }
}
