using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SuopCommerce.Migrations
{
    /// <inheritdoc />
    public partial class renameImageIdToUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Images",
                newName: "Url");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Url",
                table: "Images",
                newName: "Id");
        }
    }
}
