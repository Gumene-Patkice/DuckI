using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DuckI.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialPdfUpdateV4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_StudentPdfs",
                table: "StudentPdfs");

            migrationBuilder.DropIndex(
                name: "IX_StudentPdfs_PrivatePdfId",
                table: "StudentPdfs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EducatorPdfs",
                table: "EducatorPdfs");

            migrationBuilder.DropIndex(
                name: "IX_EducatorPdfs_PublicPdfId",
                table: "EducatorPdfs");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudentPdfs",
                table: "StudentPdfs",
                column: "PrivatePdfId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EducatorPdfs",
                table: "EducatorPdfs",
                column: "PublicPdfId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentPdfs_UserId",
                table: "StudentPdfs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_EducatorPdfs_UserId",
                table: "EducatorPdfs",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_StudentPdfs",
                table: "StudentPdfs");

            migrationBuilder.DropIndex(
                name: "IX_StudentPdfs_UserId",
                table: "StudentPdfs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EducatorPdfs",
                table: "EducatorPdfs");

            migrationBuilder.DropIndex(
                name: "IX_EducatorPdfs_UserId",
                table: "EducatorPdfs");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudentPdfs",
                table: "StudentPdfs",
                column: "UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EducatorPdfs",
                table: "EducatorPdfs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentPdfs_PrivatePdfId",
                table: "StudentPdfs",
                column: "PrivatePdfId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EducatorPdfs_PublicPdfId",
                table: "EducatorPdfs",
                column: "PublicPdfId",
                unique: true);
        }
    }
}
