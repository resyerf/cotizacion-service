using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cotizacion.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "cotiz");

            migrationBuilder.CreateTable(
                name: "actividades",
                schema: "cotiz",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Codigo = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Nombre = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Orden = table.Column<int>(type: "integer", nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_actividades", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "clientes",
                schema: "cotiz",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nombre = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Ruc = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Telefono = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Direccion = table.Column<string>(type: "text", nullable: true),
                    Activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_clientes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "items_catalogo",
                schema: "cotiz",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ActividadId = table.Column<Guid>(type: "uuid", nullable: false),
                    Codigo = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: false),
                    Unidad = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    PrecioBase = table.Column<decimal>(type: "numeric(14,2)", nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_items_catalogo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_items_catalogo_actividades_ActividadId",
                        column: x => x.ActividadId,
                        principalSchema: "cotiz",
                        principalTable: "actividades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "cotizaciones",
                schema: "cotiz",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Numero = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    ClienteId = table.Column<Guid>(type: "uuid", nullable: false),
                    Proyecto = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Ubicacion = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Fecha = table.Column<DateOnly>(type: "date", nullable: false),
                    FechaValidez = table.Column<DateOnly>(type: "date", nullable: true),
                    Estado = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Moneda = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    Notas = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cotizaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_cotizaciones_clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalSchema: "cotiz",
                        principalTable: "clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "cotizacion_partidas",
                schema: "cotiz",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CotizacionId = table.Column<Guid>(type: "uuid", nullable: false),
                    ItemCatalogoId = table.Column<Guid>(type: "uuid", nullable: false),
                    PrecioUnitario = table.Column<decimal>(type: "numeric(14,2)", nullable: false),
                    Cantidad = table.Column<decimal>(type: "numeric(10,3)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cotizacion_partidas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_cotizacion_partidas_cotizaciones_CotizacionId",
                        column: x => x.CotizacionId,
                        principalSchema: "cotiz",
                        principalTable: "cotizaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_cotizacion_partidas_items_catalogo_ItemCatalogoId",
                        column: x => x.ItemCatalogoId,
                        principalSchema: "cotiz",
                        principalTable: "items_catalogo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_actividades_Codigo",
                schema: "cotiz",
                table: "actividades",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_clientes_Ruc",
                schema: "cotiz",
                table: "clientes",
                column: "Ruc",
                unique: true,
                filter: "\"Ruc\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_cotizacion_partidas_CotizacionId_ItemCatalogoId",
                schema: "cotiz",
                table: "cotizacion_partidas",
                columns: new[] { "CotizacionId", "ItemCatalogoId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_cotizacion_partidas_ItemCatalogoId",
                schema: "cotiz",
                table: "cotizacion_partidas",
                column: "ItemCatalogoId");

            migrationBuilder.CreateIndex(
                name: "IX_cotizaciones_ClienteId",
                schema: "cotiz",
                table: "cotizaciones",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_cotizaciones_Estado",
                schema: "cotiz",
                table: "cotizaciones",
                column: "Estado");

            migrationBuilder.CreateIndex(
                name: "IX_cotizaciones_Numero",
                schema: "cotiz",
                table: "cotizaciones",
                column: "Numero",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_items_catalogo_ActividadId",
                schema: "cotiz",
                table: "items_catalogo",
                column: "ActividadId");

            migrationBuilder.CreateIndex(
                name: "IX_items_catalogo_Codigo",
                schema: "cotiz",
                table: "items_catalogo",
                column: "Codigo",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cotizacion_partidas",
                schema: "cotiz");

            migrationBuilder.DropTable(
                name: "cotizaciones",
                schema: "cotiz");

            migrationBuilder.DropTable(
                name: "items_catalogo",
                schema: "cotiz");

            migrationBuilder.DropTable(
                name: "clientes",
                schema: "cotiz");

            migrationBuilder.DropTable(
                name: "actividades",
                schema: "cotiz");
        }
    }
}
