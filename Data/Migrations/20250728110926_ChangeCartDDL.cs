using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NHT_Marine_BE.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeCartDDL : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AddressId",
                table: "CustomerCarts",
                newName: "CartId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CartId",
                table: "CustomerCarts",
                newName: "AddressId");
        }
    }
}
