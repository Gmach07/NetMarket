using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessLogic.Data.Migrations
{
    /// <inheritdoc />
    public partial class MigracionOrdenCompra2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TipoEnvio",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DeliveryTime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Precio = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoEnvio", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrdenCompras",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompradorEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrdenCompra = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DireccionEnvio_Calle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DireccionEnvio_Ciudad = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DireccionEnvio_Departamento = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DireccionEnvio_CodigoPostal = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DireccionEnvio_Pais = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TipoEnvioId = table.Column<int>(type: "int", nullable: false),
                    Subtotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PagoIntentoId = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdenCompras", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrdenCompras_TipoEnvio_TipoEnvioId",
                        column: x => x.TipoEnvioId,
                        principalTable: "TipoEnvio",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrdenItem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemOrdenado_ProductoItemId = table.Column<int>(type: "int", nullable: false),
                    ItemOrdenado_ProductoNombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ItemOrdenado_ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Precio = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    OrdenComprasId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdenItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrdenItem_OrdenCompras_OrdenComprasId",
                        column: x => x.OrdenComprasId,
                        principalTable: "OrdenCompras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrdenCompras_TipoEnvioId",
                table: "OrdenCompras",
                column: "TipoEnvioId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenItem_OrdenComprasId",
                table: "OrdenItem",
                column: "OrdenComprasId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrdenItem");

            migrationBuilder.DropTable(
                name: "OrdenCompras");

            migrationBuilder.DropTable(
                name: "TipoEnvio");
        }
    }
}
