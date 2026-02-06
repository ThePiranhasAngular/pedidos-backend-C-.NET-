using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrdersBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderItemProductRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_ProductId",
                schema: "ThePirahns",
                table: "OrderItems",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Products_ProductId",
                schema: "ThePirahns",
                table: "OrderItems",
                column: "ProductId",
                principalSchema: "ThePirahns",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Products_ProductId",
                schema: "ThePirahns",
                table: "OrderItems");

            migrationBuilder.DropIndex(
                name: "IX_OrderItems_ProductId",
                schema: "ThePirahns",
                table: "OrderItems");
        }
    }
}
