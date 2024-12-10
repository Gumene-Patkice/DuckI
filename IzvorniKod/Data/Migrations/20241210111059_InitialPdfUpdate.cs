using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DuckI.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialPdfUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    TagId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TagName = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.TagId);
                });

            migrationBuilder.CreateTable(
                name: "EducatorPdfs",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    PublicPdfId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducatorPdfs", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_EducatorPdfs_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PrivatePdfs",
                columns: table => new
                {
                    PrivatePdfId = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PdfPath = table.Column<string>(type: "TEXT", nullable: false),
                    PrivatePdfTagPrivatePdfId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrivatePdfs", x => x.PrivatePdfId);
                });

            migrationBuilder.CreateTable(
                name: "PrivatePdfTags",
                columns: table => new
                {
                    PrivatePdfId = table.Column<long>(type: "INTEGER", nullable: false),
                    TagId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrivatePdfTags", x => x.PrivatePdfId);
                    table.ForeignKey(
                        name: "FK_PrivatePdfTags_PrivatePdfs_PrivatePdfId",
                        column: x => x.PrivatePdfId,
                        principalTable: "PrivatePdfs",
                        principalColumn: "PrivatePdfId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PrivatePdfTags_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "TagId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentPdfs",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    PrivatePdfId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentPdfs", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_StudentPdfs_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentPdfs_PrivatePdfs_PrivatePdfId",
                        column: x => x.PrivatePdfId,
                        principalTable: "PrivatePdfs",
                        principalColumn: "PrivatePdfId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PublicPdfs",
                columns: table => new
                {
                    PublicPdfId = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PdfPath = table.Column<string>(type: "TEXT", nullable: false),
                    Rating = table.Column<int>(type: "INTEGER", nullable: false),
                    PublicPdfTagPublicPdfId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PublicPdfs", x => x.PublicPdfId);
                });

            migrationBuilder.CreateTable(
                name: "PublicPdfTags",
                columns: table => new
                {
                    PublicPdfId = table.Column<long>(type: "INTEGER", nullable: false),
                    TagId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PublicPdfTags", x => x.PublicPdfId);
                    table.ForeignKey(
                        name: "FK_PublicPdfTags_PublicPdfs_PublicPdfId",
                        column: x => x.PublicPdfId,
                        principalTable: "PublicPdfs",
                        principalColumn: "PublicPdfId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PublicPdfTags_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "TagId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EducatorPdfs_PublicPdfId",
                table: "EducatorPdfs",
                column: "PublicPdfId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PrivatePdfs_PrivatePdfTagPrivatePdfId",
                table: "PrivatePdfs",
                column: "PrivatePdfTagPrivatePdfId");

            migrationBuilder.CreateIndex(
                name: "IX_PrivatePdfTags_TagId",
                table: "PrivatePdfTags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_PublicPdfs_PublicPdfTagPublicPdfId",
                table: "PublicPdfs",
                column: "PublicPdfTagPublicPdfId");

            migrationBuilder.CreateIndex(
                name: "IX_PublicPdfTags_TagId",
                table: "PublicPdfTags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentPdfs_PrivatePdfId",
                table: "StudentPdfs",
                column: "PrivatePdfId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_EducatorPdfs_PublicPdfs_PublicPdfId",
                table: "EducatorPdfs",
                column: "PublicPdfId",
                principalTable: "PublicPdfs",
                principalColumn: "PublicPdfId",
                onDelete: ReferentialAction.Cascade);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PublicPdfTags_PublicPdfs_PublicPdfId",
                table: "PublicPdfTags");

            migrationBuilder.DropForeignKey(
                name: "FK_PrivatePdfs_PrivatePdfTags_PrivatePdfTagPrivatePdfId",
                table: "PrivatePdfs");

            migrationBuilder.DropTable(
                name: "EducatorPdfs");

            migrationBuilder.DropTable(
                name: "StudentPdfs");

            migrationBuilder.DropTable(
                name: "PublicPdfs");

            migrationBuilder.DropTable(
                name: "PublicPdfTags");

            migrationBuilder.DropTable(
                name: "PrivatePdfTags");

            migrationBuilder.DropTable(
                name: "PrivatePdfs");

            migrationBuilder.DropTable(
                name: "Tags");
        }
    }
}
