using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace if3250_2022_01_buletin_backend.src.Migrations
{
    public partial class DeleteChannelIdColumnInBookmarkTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChannelId",
                table: "Bookmarks");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ChannelId",
                table: "Bookmarks",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Relational:ColumnOrder", 1);
        }
    }
}
