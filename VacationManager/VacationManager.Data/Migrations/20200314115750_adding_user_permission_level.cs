using Microsoft.EntityFrameworkCore.Migrations;

namespace VacationManager.Data.Migrations
{
    public partial class adding_user_permission_level : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PermissionLevel",
                table: "Users",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PermissionLevel",
                table: "Users");
        }
    }
}
