using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmacyWebAPI.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class EditImagesURL : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImgeURL",
                table: "Drugs",
                newName: "ImageURL");

            migrationBuilder.RenameColumn(
                name: "ImgeURL",
                table: "Categories",
                newName: "ImageURL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageURL",
                table: "Drugs",
                newName: "ImgeURL");

            migrationBuilder.RenameColumn(
                name: "ImageURL",
                table: "Categories",
                newName: "ImgeURL");
        }
    }
}
