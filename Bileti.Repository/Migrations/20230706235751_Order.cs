using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Bileti.Repository.Migrations
{
    public partial class Order : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "orderDate",
                table: "Order",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<double>(
                name: "total",
                table: "Order",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "orderDate",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "total",
                table: "Order");
        }
    }
}
