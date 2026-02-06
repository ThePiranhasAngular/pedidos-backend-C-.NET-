using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrdersBackend.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProductSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Category",
                schema: "ThePirahns",
                table: "Products",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                schema: "ThePirahns",
                table: "Products",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                schema: "ThePirahns",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                schema: "ThePirahns",
                table: "Products");
        }
    }
}
