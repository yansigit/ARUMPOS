using Microsoft.EntityFrameworkCore.Migrations;

namespace ArumModels.Migrations
{
    public partial class Update1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderMenu_Orders_OrderId",
                table: "OrderMenu");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Orders",
                table: "Orders");

            migrationBuilder.RenameTable(
                name: "Orders",
                newName: "Order");

            migrationBuilder.RenameColumn(
                name: "CreateDateTime",
                table: "OrderOption",
                newName: "CreatedDateTime");

            migrationBuilder.RenameColumn(
                name: "OrderOptionId",
                table: "OrderOption",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "CreateDateTime",
                table: "OrderMenu",
                newName: "CreatedDateTime");

            migrationBuilder.RenameColumn(
                name: "OrderMenuId",
                table: "OrderMenu",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "CreateDateTime",
                table: "Order",
                newName: "CreatedDateTime");

            migrationBuilder.RenameColumn(
                name: "OrderId",
                table: "Order",
                newName: "Id");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "OrderOption",
                type: "varchar(100)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Body",
                table: "OrderOption",
                type: "varchar(200)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "OrderMenu",
                type: "varchar(100)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Order",
                table: "Order",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderMenu_Order_OrderId",
                table: "OrderMenu",
                column: "OrderId",
                principalTable: "Order",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderMenu_Order_OrderId",
                table: "OrderMenu");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Order",
                table: "Order");

            migrationBuilder.RenameTable(
                name: "Order",
                newName: "Orders");

            migrationBuilder.RenameColumn(
                name: "CreatedDateTime",
                table: "OrderOption",
                newName: "CreateDateTime");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "OrderOption",
                newName: "OrderOptionId");

            migrationBuilder.RenameColumn(
                name: "CreatedDateTime",
                table: "OrderMenu",
                newName: "CreateDateTime");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "OrderMenu",
                newName: "OrderMenuId");

            migrationBuilder.RenameColumn(
                name: "CreatedDateTime",
                table: "Orders",
                newName: "CreateDateTime");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Orders",
                newName: "OrderId");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "OrderOption",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Body",
                table: "OrderOption",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "OrderMenu",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Orders",
                table: "Orders",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderMenu_Orders_OrderId",
                table: "OrderMenu",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "OrderId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
