using Microsoft.EntityFrameworkCore.Migrations;

namespace Bileti.Repository.Migrations
{
    public partial class UserRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SelectedRole",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SelectedRole",
                table: "AspNetUsers");
        }
    }
}
