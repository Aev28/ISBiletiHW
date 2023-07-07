using Microsoft.EntityFrameworkCore.Migrations;

namespace Bileti.Repository.Migrations
{
    public partial class Genres : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "selectedGenre",
                table: "Ticket",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "selectedGenre",
                table: "Ticket");
        }
    }
}
