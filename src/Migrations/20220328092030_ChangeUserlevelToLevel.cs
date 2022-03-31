using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace if3250_2022_01_buletin_backend.src.Migrations
{
    public partial class ChangeUserlevelToLevel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "level",
                table: "User",
                newName: "Level");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Level",
                table: "User",
                newName: "level");
        }
    }
}
