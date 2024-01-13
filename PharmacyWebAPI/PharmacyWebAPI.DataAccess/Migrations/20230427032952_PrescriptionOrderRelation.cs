using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmacyWebAPI.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class PrescriptionOrderRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrderId",
                table: "Prescription",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Prescription_OrderId",
                table: "Prescription",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Prescription_Order_OrderId",
                table: "Prescription",
                column: "OrderId",
                principalTable: "Order",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Prescription_Order_OrderId",
                table: "Prescription");

            migrationBuilder.DropIndex(
                name: "IX_Prescription_OrderId",
                table: "Prescription");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "Prescription");
        }
    }
}
