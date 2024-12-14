using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DuckI.Data.Migrations
{
    /// <inheritdoc />
    public partial class PdfRatingUpdateV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RaitingLogs");

            migrationBuilder.CreateTable(
                name: "RatingLogs",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    PublicPdfId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RatingLogs", x => new { x.UserId, x.PublicPdfId });
                    table.ForeignKey(
                        name: "FK_RatingLogs_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RatingLogs_PublicPdfs_PublicPdfId",
                        column: x => x.PublicPdfId,
                        principalTable: "PublicPdfs",
                        principalColumn: "PublicPdfId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RatingLogs_PublicPdfId",
                table: "RatingLogs",
                column: "PublicPdfId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RatingLogs");

            migrationBuilder.CreateTable(
                name: "RaitingLogs",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    PublicPdfId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RaitingLogs", x => new { x.UserId, x.PublicPdfId });
                    table.ForeignKey(
                        name: "FK_RaitingLogs_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RaitingLogs_PublicPdfs_PublicPdfId",
                        column: x => x.PublicPdfId,
                        principalTable: "PublicPdfs",
                        principalColumn: "PublicPdfId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RaitingLogs_PublicPdfId",
                table: "RaitingLogs",
                column: "PublicPdfId");
        }
    }
}
