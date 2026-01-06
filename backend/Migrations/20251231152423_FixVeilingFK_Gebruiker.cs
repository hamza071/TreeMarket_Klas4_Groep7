using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class FixVeilingFK_Gebruiker : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Veiling_Veilingsmeester_VeilingsmeesterID",
                table: "Veiling");

            migrationBuilder.RenameColumn(
                name: "Artikelkenmerken",
                table: "Product",
                newName: "Varieteit");

            migrationBuilder.AlterColumn<bool>(
                name: "Status",
                table: "Veiling",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Omschrijving",
                table: "Product",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProductNaam",
                table: "Product",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Veiling_Gebruiker_VeilingsmeesterID",
                table: "Veiling",
                column: "VeilingsmeesterID",
                principalTable: "Gebruiker",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Veiling_Gebruiker_VeilingsmeesterID",
                table: "Veiling");

            migrationBuilder.DropColumn(
                name: "Omschrijving",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "ProductNaam",
                table: "Product");

            migrationBuilder.RenameColumn(
                name: "Varieteit",
                table: "Product",
                newName: "Artikelkenmerken");

            migrationBuilder.AlterColumn<bool>(
                name: "Status",
                table: "Veiling",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AddForeignKey(
                name: "FK_Veiling_Veilingsmeester_VeilingsmeesterID",
                table: "Veiling",
                column: "VeilingsmeesterID",
                principalTable: "Veilingsmeester",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
