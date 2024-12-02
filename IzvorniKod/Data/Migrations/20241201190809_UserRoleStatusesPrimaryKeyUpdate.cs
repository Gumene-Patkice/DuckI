using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DuckI.Migrations
{
    /// <inheritdoc />
    public partial class UserRoleStatusesPrimaryKeyUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserRoleStatuses",
                table: "UserRoleStatuses");

            migrationBuilder.DropIndex(
                name: "IX_UserRoleStatuses_UserId",
                table: "UserRoleStatuses");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserRoleStatuses",
                table: "UserRoleStatuses",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserRoleStatuses",
                table: "UserRoleStatuses");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserRoleStatuses",
                table: "UserRoleStatuses",
                columns: new[] { "UserId", "RoleId" });

            migrationBuilder.CreateIndex(
                name: "IX_UserRoleStatuses_UserId",
                table: "UserRoleStatuses",
                column: "UserId",
                unique: true);
        }
    }
}
