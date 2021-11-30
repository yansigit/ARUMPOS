using Microsoft.EntityFrameworkCore.Migrations;

namespace ArumModels.Migrations
{
    public partial class Rename_Order_Columns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FranchiseNumber",
                table: "Order",
                newName: "PaymentCode");

            migrationBuilder.AddColumn<string>(
                name: "Message",
                table: "Order",
                type: "varchar(100)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Message",
                table: "Order");

            migrationBuilder.RenameColumn(
                name: "PaymentCode",
                table: "Order",
                newName: "FranchiseNumber");
        }
    }
}
