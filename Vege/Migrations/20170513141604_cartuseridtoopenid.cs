using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Vege.Migrations
{
    public partial class cartuseridtoopenid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ShoppingCarts");

            migrationBuilder.AddColumn<string>(
                name: "OpenId",
                table: "ShoppingCarts",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OpenId",
                table: "ShoppingCarts");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "ShoppingCarts",
                nullable: false,
                defaultValue: 0);
        }
    }
}
