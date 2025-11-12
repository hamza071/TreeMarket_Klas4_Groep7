using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TreeMarket_Klas4_Groep7.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Gebruiker",
                columns: table => new
                {
                    GebruikerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Naam = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telefoonnummer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Wachtwoord = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rol = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Discriminator = table.Column<string>(type: "nvarchar(21)", maxLength: 21, nullable: false),
                    bedrijf = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KvKNummer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IBANnummer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlanDatum = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gebruiker", x => x.GebruikerId);
                });

            migrationBuilder.CreateTable(
                name: "Product",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Foto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Artikelkenmerken = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Hoeveelheid = table.Column<int>(type: "int", nullable: false),
                    MinimumPrijs = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Dagdatum = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LeverancierID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product", x => x.ProductId);
                    table.ForeignKey(
                        name: "FK_Product_Gebruiker_LeverancierID",
                        column: x => x.LeverancierID,
                        principalTable: "Gebruiker",
                        principalColumn: "GebruikerId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Dashboard",
                columns: table => new
                {
                    DashboardID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dashboard", x => x.DashboardID);
                    table.ForeignKey(
                        name: "FK_Dashboard_Product_ProductID",
                        column: x => x.ProductID,
                        principalTable: "Product",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Veiling",
                columns: table => new
                {
                    VeilingID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<bool>(type: "bit", nullable: true),
                    StartPrijs = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EindPrijs = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    HuidigePrijs = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PrijsStap = table.Column<int>(type: "int", nullable: false),
                    PrijsStrategie = table.Column<int>(type: "int", nullable: false),
                    StartTijd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EindTijd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProductID = table.Column<int>(type: "int", nullable: false),
                    VeilingsmeesterID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Veiling", x => x.VeilingID);
                    table.ForeignKey(
                        name: "FK_Veiling_Gebruiker_VeilingsmeesterID",
                        column: x => x.VeilingsmeesterID,
                        principalTable: "Gebruiker",
                        principalColumn: "GebruikerId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Veiling_Product_ProductID",
                        column: x => x.ProductID,
                        principalTable: "Product",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Claim",
                columns: table => new
                {
                    ClaimID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Prijs = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    KlantId = table.Column<int>(type: "int", nullable: false),
                    VeilingId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Claim", x => x.ClaimID);
                    table.ForeignKey(
                        name: "FK_Claim_Gebruiker_KlantId",
                        column: x => x.KlantId,
                        principalTable: "Gebruiker",
                        principalColumn: "GebruikerId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Claim_Veiling_VeilingId",
                        column: x => x.VeilingId,
                        principalTable: "Veiling",
                        principalColumn: "VeilingID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Claim_KlantId",
                table: "Claim",
                column: "KlantId");

            migrationBuilder.CreateIndex(
                name: "IX_Claim_VeilingId",
                table: "Claim",
                column: "VeilingId");

            migrationBuilder.CreateIndex(
                name: "IX_Dashboard_ProductID",
                table: "Dashboard",
                column: "ProductID");

            migrationBuilder.CreateIndex(
                name: "IX_Product_LeverancierID",
                table: "Product",
                column: "LeverancierID");

            migrationBuilder.CreateIndex(
                name: "IX_Veiling_ProductID",
                table: "Veiling",
                column: "ProductID");

            migrationBuilder.CreateIndex(
                name: "IX_Veiling_VeilingsmeesterID",
                table: "Veiling",
                column: "VeilingsmeesterID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Claim");

            migrationBuilder.DropTable(
                name: "Dashboard");

            migrationBuilder.DropTable(
                name: "Veiling");

            migrationBuilder.DropTable(
                name: "Product");

            migrationBuilder.DropTable(
                name: "Gebruiker");
        }
    }
}
