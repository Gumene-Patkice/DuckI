using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DuckI.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialPdfUpdateV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PrivatePdfs_PrivatePdfTags_PrivatePdfTagPrivatePdfId",
                table: "PrivatePdfs");

            migrationBuilder.DropForeignKey(
                name: "FK_PublicPdfs_PublicPdfTags_PublicPdfTagPublicPdfId",
                table: "PublicPdfs");

            migrationBuilder.DropIndex(
                name: "IX_PublicPdfs_PublicPdfTagPublicPdfId",
                table: "PublicPdfs");

            migrationBuilder.DropIndex(
                name: "IX_PrivatePdfs_PrivatePdfTagPrivatePdfId",
                table: "PrivatePdfs");

            migrationBuilder.DropColumn(
                name: "PublicPdfTagPublicPdfId",
                table: "PublicPdfs");

            migrationBuilder.DropColumn(
                name: "PrivatePdfTagPrivatePdfId",
                table: "PrivatePdfs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "PublicPdfTagPublicPdfId",
                table: "PublicPdfs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "PrivatePdfTagPrivatePdfId",
                table: "PrivatePdfs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_PublicPdfs_PublicPdfTagPublicPdfId",
                table: "PublicPdfs",
                column: "PublicPdfTagPublicPdfId");

            migrationBuilder.CreateIndex(
                name: "IX_PrivatePdfs_PrivatePdfTagPrivatePdfId",
                table: "PrivatePdfs",
                column: "PrivatePdfTagPrivatePdfId");

            migrationBuilder.AddForeignKey(
                name: "FK_PrivatePdfs_PrivatePdfTags_PrivatePdfTagPrivatePdfId",
                table: "PrivatePdfs",
                column: "PrivatePdfTagPrivatePdfId",
                principalTable: "PrivatePdfTags",
                principalColumn: "PrivatePdfId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PublicPdfs_PublicPdfTags_PublicPdfTagPublicPdfId",
                table: "PublicPdfs",
                column: "PublicPdfTagPublicPdfId",
                principalTable: "PublicPdfTags",
                principalColumn: "PublicPdfId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
