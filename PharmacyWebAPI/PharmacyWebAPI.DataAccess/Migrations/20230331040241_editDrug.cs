using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmacyWebAPI.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class editDrug : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CategoryName",
                table: "Drugs");

            migrationBuilder.DropColumn(
                name: "ManufacturerName",
                table: "Drugs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
    }
}
