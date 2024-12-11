using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DuckI.Migrations
{
    /// <inheritdoc />
    public partial class AddingStringFieldToUserRolesStatuses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "UserRoleStatuses",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "UserRoleStatuses");
        }
    }
}
