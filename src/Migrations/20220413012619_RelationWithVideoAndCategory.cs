using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace if3250_2022_01_buletin_backend.src.Migrations
{
    public partial class RelationWithVideoAndCategory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Categories",
                table: "Videos");

            migrationBuilder.CreateTable(
                name: "CategoryVideo",
                columns: table => new
                {
                    CategoriesId = table.Column<int>(type: "integer", nullable: false),
                    VideosId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryVideo", x => new { x.CategoriesId, x.VideosId });
                    table.ForeignKey(
                        name: "FK_CategoryVideo_Categories_CategoriesId",
                        column: x => x.CategoriesId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CategoryVideo_Videos_VideosId",
                        column: x => x.VideosId,
                        principalTable: "Videos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CategoryVideo_VideosId",
                table: "CategoryVideo",
                column: "VideosId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CategoryVideo");

            migrationBuilder.AddColumn<List<string>>(
                name: "Categories",
                table: "Videos",
                type: "text[]",
                nullable: false);
        }
    }
}
