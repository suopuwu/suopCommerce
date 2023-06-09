using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SuopCommerce.Migrations
{
    /// <inheritdoc />
    public partial class ProductAddOns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string[]>(
                name: "PotentialAddOns",
                table: "Products",
                type: "text[]",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PotentialAddOns",
                table: "Products");
        }
    }
}
