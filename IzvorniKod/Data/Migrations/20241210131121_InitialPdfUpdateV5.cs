using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DuckI.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialPdfUpdateV5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PdfName",
                table: "PublicPdfs",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PdfName",
                table: "PrivatePdfs",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PdfName",
                table: "PublicPdfs");

            migrationBuilder.DropColumn(
                name: "PdfName",
                table: "PrivatePdfs");
        }
    }
}
