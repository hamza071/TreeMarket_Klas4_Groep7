using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TreeMarket_Klas4_Groep7.Migrations
{
    /// <inheritdoc />
    public partial class AddAuctions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Claim_Gebruiker_KlantId",
                table: "Claim");

            migrationBuilder.DropForeignKey(
                name: "FK_Product_Gebruiker_LeverancierID",
                table: "Product");

            migrationBuilder.DropForeignKey(
                name: "FK_Veiling_Gebruiker_VeilingsmeesterID",
                table: "Veiling");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Gebruiker");

            migrationBuilder.DropColumn(
                name: "IBANnummer",
                table: "Gebruiker");

            migrationBuilder.DropColumn(
                name: "KvKNummer",
                table: "Gebruiker");

            migrationBuilder.DropColumn(
                name: "PlanDatum",
                table: "Gebruiker");

            migrationBuilder.DropColumn(
                name: "bedrijf",
                table: "Gebruiker");

            migrationBuilder.AlterColumn<string>(
                name: "Naam",
                table: "Gebruiker",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "Klant",
                columns: table => new
                {
                    GebruikerId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Klant", x => x.GebruikerId);
                    table.ForeignKey(
                        name: "FK_Klant_Gebruiker_GebruikerId",
                        column: x => x.GebruikerId,
                        principalTable: "Gebruiker",
                        principalColumn: "GebruikerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Leverancier",
                columns: table => new
                {
                    GebruikerId = table.Column<int>(type: "int", nullable: false),
                    bedrijf = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KvKNummer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IBANnummer = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Leverancier", x => x.GebruikerId);
                    table.ForeignKey(
                        name: "FK_Leverancier_Gebruiker_GebruikerId",
                        column: x => x.GebruikerId,
                        principalTable: "Gebruiker",
                        principalColumn: "GebruikerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Veilingsmeester",
                columns: table => new
                {
                    GebruikerId = table.Column<int>(type: "int", nullable: false),
                    PlanDatum = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Veilingsmeester", x => x.GebruikerId);
                    table.ForeignKey(
                        name: "FK_Veilingsmeester_Gebruiker_GebruikerId",
                        column: x => x.GebruikerId,
                        principalTable: "Gebruiker",
                        principalColumn: "GebruikerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Claim_Klant_KlantId",
                table: "Claim",
                column: "KlantId",
                principalTable: "Klant",
                principalColumn: "GebruikerId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Product_Leverancier_LeverancierID",
                table: "Product",
                column: "LeverancierID",
                principalTable: "Leverancier",
                principalColumn: "GebruikerId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Veiling_Veilingsmeester_VeilingsmeesterID",
                table: "Veiling",
                column: "VeilingsmeesterID",
                principalTable: "Veilingsmeester",
                principalColumn: "GebruikerId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Claim_Klant_KlantId",
                table: "Claim");

            migrationBuilder.DropForeignKey(
                name: "FK_Product_Leverancier_LeverancierID",
                table: "Product");

            migrationBuilder.DropForeignKey(
                name: "FK_Veiling_Veilingsmeester_VeilingsmeesterID",
                table: "Veiling");

            migrationBuilder.DropTable(
                name: "Klant");

            migrationBuilder.DropTable(
                name: "Leverancier");

            migrationBuilder.DropTable(
                name: "Veilingsmeester");

            migrationBuilder.AlterColumn<string>(
                name: "Naam",
                table: "Gebruiker",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Gebruiker",
                type: "nvarchar(21)",
                maxLength: 21,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "IBANnummer",
                table: "Gebruiker",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KvKNummer",
                table: "Gebruiker",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PlanDatum",
                table: "Gebruiker",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "bedrijf",
                table: "Gebruiker",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Claim_Gebruiker_KlantId",
                table: "Claim",
                column: "KlantId",
                principalTable: "Gebruiker",
                principalColumn: "GebruikerId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Product_Gebruiker_LeverancierID",
                table: "Product",
                column: "LeverancierID",
                principalTable: "Gebruiker",
                principalColumn: "GebruikerId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Veiling_Gebruiker_VeilingsmeesterID",
                table: "Veiling",
                column: "VeilingsmeesterID",
                principalTable: "Gebruiker",
                principalColumn: "GebruikerId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
