using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SuopCommerce.Migrations
{
    /// <inheritdoc />
    public partial class fixTypoOne : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "desctiption",
                table: "Images",
                newName: "Description");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Images",
                newName: "desctiption");
        }
    }
}
