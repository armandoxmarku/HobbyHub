using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HobbyHub.Migrations
{
    public partial class migrimiNjerdddxssddds : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Proficiency",
                table: "Associations",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Proficiency",
                table: "Associations");
        }
    }
}
