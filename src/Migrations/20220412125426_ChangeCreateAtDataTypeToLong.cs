using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace if3250_2022_01_buletin_backend.src.Migrations
{
    public partial class ChangeCreateAtDataTypeToLong : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "CreateAt",
                table: "Videos",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CreateAt",
                table: "Videos",
                type: "text",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");
        }
    }
}
