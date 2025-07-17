using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NHT_Marine_BE.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddInventoryDistributedStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDistributed",
                table: "ProductImports",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDistributed",
                table: "ProductImports");
        }
    }
}
