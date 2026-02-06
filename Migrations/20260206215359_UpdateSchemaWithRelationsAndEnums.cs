using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrdersBackend.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSchemaWithRelationsAndEnums : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE \"ThePirahns\".\"Users\" ALTER COLUMN \"Role\" TYPE text USING \"Role\"::text;");
            migrationBuilder.Sql("ALTER TABLE \"ThePirahns\".\"Orders\" ALTER COLUMN \"Status\" TYPE text USING \"Status\"::text;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE \"ThePirahns\".\"Users\" ALTER COLUMN \"Role\" TYPE integer USING \"Role\"::integer;");
            migrationBuilder.Sql("ALTER TABLE \"ThePirahns\".\"Orders\" ALTER COLUMN \"Status\" TYPE integer USING \"Status\"::integer;");
        }
    }
}
