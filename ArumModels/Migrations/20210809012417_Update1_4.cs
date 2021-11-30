using Microsoft.EntityFrameworkCore.Migrations;

namespace ArumModels.Migrations
{
    public partial class Update1_4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ingredient_Menu_MenuId",
                table: "Ingredient");

            migrationBuilder.DropForeignKey(
                name: "FK_Option_Menu_MenuId",
                table: "Option");

            migrationBuilder.DropIndex(
                name: "IX_Option_MenuId",
                table: "Option");

            migrationBuilder.DropIndex(
                name: "IX_Ingredient_MenuId",
                table: "Ingredient");

            migrationBuilder.DropColumn(
                name: "MenuId",
                table: "Option");

            migrationBuilder.DropColumn(
                name: "MenuId",
                table: "Ingredient");

            migrationBuilder.CreateTable(
                name: "IngredientMenu",
                columns: table => new
                {
                    IngredientListId = table.Column<int>(type: "int", nullable: false),
                    MenuListId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IngredientMenu", x => new { x.IngredientListId, x.MenuListId });
                    table.ForeignKey(
                        name: "FK_IngredientMenu_Ingredient_IngredientListId",
                        column: x => x.IngredientListId,
                        principalTable: "Ingredient",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IngredientMenu_Menu_MenuListId",
                        column: x => x.MenuListId,
                        principalTable: "Menu",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MenuOption",
                columns: table => new
                {
                    MenuListId = table.Column<int>(type: "int", nullable: false),
                    OptionListId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuOption", x => new { x.MenuListId, x.OptionListId });
                    table.ForeignKey(
                        name: "FK_MenuOption_Menu_MenuListId",
                        column: x => x.MenuListId,
                        principalTable: "Menu",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MenuOption_Option_OptionListId",
                        column: x => x.OptionListId,
                        principalTable: "Option",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientMenu_MenuListId",
                table: "IngredientMenu",
                column: "MenuListId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuOption_OptionListId",
                table: "MenuOption",
                column: "OptionListId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IngredientMenu");

            migrationBuilder.DropTable(
                name: "MenuOption");

            migrationBuilder.AddColumn<int>(
                name: "MenuId",
                table: "Option",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MenuId",
                table: "Ingredient",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Option_MenuId",
                table: "Option",
                column: "MenuId");

            migrationBuilder.CreateIndex(
                name: "IX_Ingredient_MenuId",
                table: "Ingredient",
                column: "MenuId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ingredient_Menu_MenuId",
                table: "Ingredient",
                column: "MenuId",
                principalTable: "Menu",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Option_Menu_MenuId",
                table: "Option",
                column: "MenuId",
                principalTable: "Menu",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
