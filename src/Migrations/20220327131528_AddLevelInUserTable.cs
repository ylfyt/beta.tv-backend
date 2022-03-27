using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace if3250_2022_01_buletin_backend.src.Migrations
{
    public partial class AddLevelInUserTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "level",
                table: "User",
                type: "integer",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "level",
                table: "User");
        }
    }
}
