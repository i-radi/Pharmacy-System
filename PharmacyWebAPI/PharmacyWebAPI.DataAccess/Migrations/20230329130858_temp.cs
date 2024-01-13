using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmacyWebAPI.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class temp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImgURL",
                table: "Manufacturers");

            migrationBuilder.AddColumn<string>(
                name: "CategoryName",
                table: "Drugs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ManufacturerName",
                table: "Drugs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CategoryName",
                table: "Drugs");

            migrationBuilder.DropColumn(
                name: "ManufacturerName",
                table: "Drugs");

            migrationBuilder.AddColumn<string>(
                name: "ImgURL",
                table: "Manufacturers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
