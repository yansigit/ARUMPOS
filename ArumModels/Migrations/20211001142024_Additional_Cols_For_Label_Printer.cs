using Microsoft.EntityFrameworkCore.Migrations;

namespace ArumModels.Migrations
{
    public partial class Additional_Cols_For_Label_Printer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LicenseNumber",
                table: "Shop",
                type: "varchar(100)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "OwnerName",
                table: "Shop",
                type: "varchar(100)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Shop",
                type: "varchar(100)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CardName",
                table: "Order",
                type: "varchar(100)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CardNumber",
                table: "Order",
                type: "varchar(100)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "FranchiseNumber",
                table: "Order",
                type: "varchar(100)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Tid",
                table: "Order",
                type: "varchar(100)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LicenseNumber",
                table: "Shop");

            migrationBuilder.DropColumn(
                name: "OwnerName",
                table: "Shop");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Shop");

            migrationBuilder.DropColumn(
                name: "CardName",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "CardNumber",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "FranchiseNumber",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "Tid",
                table: "Order");
        }
    }
}
