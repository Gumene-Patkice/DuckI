using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DuckI.Data.Migrations
{
    /// <inheritdoc />
    public partial class PdfRemovedLogsUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RemovedLogs",
                columns: table => new
                {
                    RemoveLogId = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ReviewerId = table.Column<string>(type: "TEXT", nullable: false),
                    EducatorId = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    FileName = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RemovedLogs", x => x.RemoveLogId);
                    table.ForeignKey(
                        name: "FK_RemovedLogs_AspNetUsers_EducatorId",
                        column: x => x.EducatorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RemovedLogs_AspNetUsers_ReviewerId",
                        column: x => x.ReviewerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RemovedLogs_EducatorId",
                table: "RemovedLogs",
                column: "EducatorId");

            migrationBuilder.CreateIndex(
                name: "IX_RemovedLogs_ReviewerId",
                table: "RemovedLogs",
                column: "ReviewerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RemovedLogs");
        }
    }
}
