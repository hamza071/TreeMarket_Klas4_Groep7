using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class PendingModelChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Veiling_Gebruiker_VeilingsmeesterID",
                table: "Veiling");

            migrationBuilder.AddForeignKey(
                name: "FK_Veiling_Veilingsmeester_VeilingsmeesterID",
                table: "Veiling",
                column: "VeilingsmeesterID",
                principalTable: "Veilingsmeester",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Veiling_Veilingsmeester_VeilingsmeesterID",
                table: "Veiling");

            migrationBuilder.AddForeignKey(
                name: "FK_Veiling_Gebruiker_VeilingsmeesterID",
                table: "Veiling",
                column: "VeilingsmeesterID",
                principalTable: "Gebruiker",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
