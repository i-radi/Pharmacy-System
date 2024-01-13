using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmacyWebAPI.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class EditImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImgURL",
                table: "Drugs",
                newName: "ImgeURL");

            migrationBuilder.RenameColumn(
                name: "ImgURL",
                table: "Categories",
                newName: "ImgeURL");

            migrationBuilder.AddColumn<string>(
                name: "ImageId",
                table: "Drugs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ImageId",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageId",
                table: "Drugs");

            migrationBuilder.DropColumn(
                name: "ImageId",
                table: "Categories");

            migrationBuilder.RenameColumn(
                name: "ImgeURL",
                table: "Drugs",
                newName: "ImgURL");

            migrationBuilder.RenameColumn(
                name: "ImgeURL",
                table: "Categories",
                newName: "ImgURL");
        }
    }
}
