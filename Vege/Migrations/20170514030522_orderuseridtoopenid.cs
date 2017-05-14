using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Vege.Migrations
{
    public partial class orderuseridtoopenid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Orders",
                newName: "OpenId");

            migrationBuilder.AddColumn<string>(
                name: "UnitName",
                table: "Products",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UnitName",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "OpenId",
                table: "Orders",
                newName: "UserId");
        }
    }
}
