using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace if3250_2022_01_buletin_backend.src.Migrations
{
    public partial class UpdateVideoModelAndImplementations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rating",
                table: "Videos");

            migrationBuilder.RenameColumn(
                name: "Views",
                table: "Videos",
                newName: "AuthorId");

            migrationBuilder.RenameColumn(
                name: "Release_Date",
                table: "Videos",
                newName: "CreateAt");

            migrationBuilder.RenameColumn(
                name: "Category",
                table: "Videos",
                newName: "YoutubeVideoId");

            migrationBuilder.AlterColumn<string>(
                name: "ChannelId",
                table: "Videos",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<string>(
                name: "AuthorDescription",
                table: "Videos",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AuthorName",
                table: "Videos",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AuthorTitle",
                table: "Videos",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<List<string>>(
                name: "Categories",
                table: "Videos",
                type: "text[]",
                nullable: false);

            migrationBuilder.AddColumn<string>(
                name: "ChannelName",
                table: "Videos",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ChannelThumbnailUrl",
                table: "Videos",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ThumbnailUrl",
                table: "Videos",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthorDescription",
                table: "Videos");

            migrationBuilder.DropColumn(
                name: "AuthorName",
                table: "Videos");

            migrationBuilder.DropColumn(
                name: "AuthorTitle",
                table: "Videos");

            migrationBuilder.DropColumn(
                name: "Categories",
                table: "Videos");

            migrationBuilder.DropColumn(
                name: "ChannelName",
                table: "Videos");

            migrationBuilder.DropColumn(
                name: "ChannelThumbnailUrl",
                table: "Videos");

            migrationBuilder.DropColumn(
                name: "ThumbnailUrl",
                table: "Videos");

            migrationBuilder.RenameColumn(
                name: "YoutubeVideoId",
                table: "Videos",
                newName: "Category");

            migrationBuilder.RenameColumn(
                name: "CreateAt",
                table: "Videos",
                newName: "Release_Date");

            migrationBuilder.RenameColumn(
                name: "AuthorId",
                table: "Videos",
                newName: "Views");

            migrationBuilder.AlterColumn<int>(
                name: "ChannelId",
                table: "Videos",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<float>(
                name: "Rating",
                table: "Videos",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }
    }
}
