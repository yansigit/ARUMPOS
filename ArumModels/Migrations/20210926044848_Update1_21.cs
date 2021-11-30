using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ArumModels.Migrations
{
    public partial class Update1_21 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCold",
                table: "Menu");

            migrationBuilder.DropColumn(
                name: "IsHot",
                table: "Menu");

            migrationBuilder.RenameColumn(
                name: "Price",
                table: "Menu",
                newName: "HotPrice");

            migrationBuilder.AlterColumn<string>(
                name: "ImagePath",
                table: "OrderedMenu",
                type: "varchar(100)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "EstimatedTime",
                table: "Order",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "ColdPrice",
                table: "Menu",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EstimatedTime",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "ColdPrice",
                table: "Menu");

            migrationBuilder.RenameColumn(
                name: "HotPrice",
                table: "Menu",
                newName: "Price");

            migrationBuilder.AlterColumn<string>(
                name: "ImagePath",
                table: "OrderedMenu",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "IsCold",
                table: "Menu",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsHot",
                table: "Menu",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }
    }
}
