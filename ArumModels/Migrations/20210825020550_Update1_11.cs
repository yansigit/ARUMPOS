using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ArumModels.Migrations
{
    public partial class Update1_11 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "longitude",
                table: "Shop",
                newName: "Longitude");

            migrationBuilder.RenameColumn(
                name: "latitude",
                table: "Shop",
                newName: "Latitude");

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Shop",
                type: "TinyText",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "Text",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ThumbImage",
                table: "Shop",
                type: "TinyText",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "Menu",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "CarouselImage",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Path = table.Column<string>(type: "TinyText", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ShopId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarouselImage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarouselImage_Shop_ShopId",
                        column: x => x.ShopId,
                        principalTable: "Shop",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_CarouselImage_ShopId",
                table: "CarouselImage",
                column: "ShopId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarouselImage");

            migrationBuilder.DropColumn(
                name: "ThumbImage",
                table: "Shop");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "Menu");

            migrationBuilder.RenameColumn(
                name: "Longitude",
                table: "Shop",
                newName: "longitude");

            migrationBuilder.RenameColumn(
                name: "Latitude",
                table: "Shop",
                newName: "latitude");

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Shop",
                type: "Text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TinyText",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
