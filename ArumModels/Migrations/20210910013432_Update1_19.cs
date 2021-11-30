using Microsoft.EntityFrameworkCore.Migrations;

namespace ArumModels.Migrations
{
    public partial class Update1_19 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Coupon_User_UserId",
                table: "Coupon");

            migrationBuilder.DropIndex(
                name: "IX_Coupon_UserId",
                table: "Coupon");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Coupon");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Coupon",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Coupon_UserId",
                table: "Coupon",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Coupon_User_UserId",
                table: "Coupon",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
