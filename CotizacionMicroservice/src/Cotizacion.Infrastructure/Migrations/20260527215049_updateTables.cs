using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cotizacion.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Moneda",
                schema: "cotiz",
                table: "items_catalogo",
                type: "integer",
                nullable: false,
                defaultValue: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Moneda",
                schema: "cotiz",
                table: "items_catalogo");
        }
    }
}
