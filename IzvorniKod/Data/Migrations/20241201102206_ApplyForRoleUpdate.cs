using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DuckI.Migrations
{
    /// <inheritdoc />
    public partial class ApplyForRoleUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserRoleStatuses",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    RoleId = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoleStatuses", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRoleStatuses_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoleStatuses_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserRoleStatuses_RoleId",
                table: "UserRoleStatuses",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoleStatuses_UserId",
                table: "UserRoleStatuses",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserRoleStatuses");
        }
    }
}
