using Microsoft.EntityFrameworkCore.Migrations;

namespace ArumModels.Migrations
{
    public partial class Add_Background_Color_To_OrderedMenu : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BackgroundColor",
                table: "OrderedMenu",
                type: "varchar(20)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BackgroundColor",
                table: "OrderedMenu");
        }
    }
}
