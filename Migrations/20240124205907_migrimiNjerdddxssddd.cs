using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HobbyHub.Migrations
{
    public partial class migrimiNjerdddxssddd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Email",
                table: "Users",
                newName: "Username");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Username",
                table: "Users",
                newName: "Email");
        }
    }
}
