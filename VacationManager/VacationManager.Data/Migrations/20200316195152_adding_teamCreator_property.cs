using Microsoft.EntityFrameworkCore.Migrations;

namespace VacationManager.Data.Migrations
{
    public partial class adding_teamCreator_property : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                table: "Teams",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Teams");
        }
    }
}
