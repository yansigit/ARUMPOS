using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;

namespace ArumModels.Models
{
    public class Category : BaseModel
    {
        [Column(TypeName = "varchar(100)")]
        public string Name { get; set; }
        public ICollection<Menu> MenuList { get; set; }
        public int ShopId { get; set; }

        public static IEnumerable<Category> GetCategories(int shopId)
        {
            using var db = new ArumDbContext();
            var shop = db.Shop.Include(s => s.Categories).
                ThenInclude(e => e.MenuList).Single(s => s.Id == shopId);
            return shop.Categories.ToList();
        }

        public static Category GetCategory(int categoryId, bool withMenus = false)
        {
            using var db = new ArumDbContext();
            return !withMenus ? db.Category.Single(c => c.Id == categoryId) : db.Category.Include(e => e.MenuList).Single(c => c.Id == categoryId);
        }

        public static bool Add(string name, int shopId)
        {
            using var db = new ArumDbContext();
            db.Add(new Category()
            {
                Name = name,
                ShopId = shopId
            });
            return db.SaveChanges() > 0;
        }
    }

    public class Menu : BaseModel
    {
        [Column(TypeName = "varchar(100)")]
        public string Name { get; set; }
        [Column(TypeName = "varchar(500)")]
        public string ImagePath { get; set; }
        public int HotPrice { get; set; }
        public int ColdPrice { get; set; }
        public ICollection<Option> OptionList { get; set; }
        public ICollection<Ingredient> IngredientList { get; set; }
        public int Priority { get; set; }
        public bool IsSoldOut { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string BackgroundColor { get; set; } = "ffffffffff";

        public int CategoryId { get; set; }

        public static Menu GetMenu(int menuId)
        {
            using var db = new ArumDbContext();
            return db.Menu.Single(m => m.Id == menuId);
        }

        public static IEnumerable<Menu> GetMenus(int categoryId)
        {
            using var db = new ArumDbContext();
            var category = db.Category.Include(c => c.MenuList).Single(c => c.Id == categoryId);
            return category.MenuList.ToList();
        }

        public static IEnumerable<Menu> GetMenusWithOptionsAndIngredients(int categoryId)
        {
            using var db = new ArumDbContext();
            var category = db.Category
                .Include(c => c.MenuList)
                .ThenInclude(e => e.IngredientList)
                .Include(e => e.MenuList)
                .ThenInclude(e => e.OptionList).Single(c => c.Id == categoryId);
            return category.MenuList.ToList();
        }

        public static bool Add(int categoryId, string name, string imagePath, List<int> ingredientsId, List<int> optionsId, int hotPrice = 0, int coldPrice = 0, string backgroundColor = "ffffffffff")
        {
            using var db = new ArumDbContext();

            var ingredientList = db.Ingredient.Where(e => ingredientsId.Contains(e.Id)).ToList();
            var optionList = db.Option.Where(e => optionsId.Contains(e.Id)).ToList();

            var newMenu = new Menu
            {
                Name = name,
                HotPrice = hotPrice,
                ColdPrice = coldPrice,
                BackgroundColor = backgroundColor,
                ImagePath = imagePath,
                IngredientList = ingredientList,
                OptionList = optionList,
                CategoryId = categoryId
            };

            db.Menu.Add(newMenu);
            return db.SaveChanges() > 0;
        }

        public static bool Modify(int menuId, int categoryId, string name, string imagePath,
            List<int> ingredientIdList, List<int> optionIdList, int hotPrice = 0, int coldPrice = 0)
        {
            using var db = new ArumDbContext();
            var menu = db.Menu
                .Include(e => e.IngredientList)
                .Include(e => e.OptionList)
                .Single(e => e.Id == menuId);
            var ingredientList = db.Ingredient
                .Where(e => ingredientIdList.Contains(e.Id)).ToList();
            var optionList = db.Option.Where(e => optionIdList.Contains(e.Id)).ToList();

            menu.CategoryId = categoryId;
            menu.Name = name;
            menu.ImagePath = imagePath;
            menu.IngredientList = ingredientList;
            menu.OptionList = optionList;
            menu.HotPrice = hotPrice;
            menu.ColdPrice = coldPrice;

            return db.SaveChanges() > 0;
        }
    }

    public class Ingredient : BaseModel
    {
        [Column(TypeName = "varchar(100)")]
        public string Name { get; set; }
        [Column(TypeName = "varchar(250)")]
        public string ImagePath { get; set; }
        [JsonIgnore]
        public ICollection<Menu> MenuList { get; set; }
        public int Priority { get; set; }

        public static IEnumerable<Ingredient> GetIngredients(int menuId)
        {
            using var db = new ArumDbContext();
            var menu = db.Menu.Include(m => m.IngredientList).Single(m => m.Id == menuId);
            return menu.IngredientList.ToList();
        }

        public static Ingredient GetIngredient(int ingredientId)
        {
            using var db = new ArumDbContext();
            return db.Ingredient.Single(i => i.Id == ingredientId);
        }

        public static bool Add(string imagePath, string name, int priority)
        {
            using var db = new ArumDbContext();
            db.Add(new Ingredient()
            {
                ImagePath = imagePath,
                Name = name,
                Priority = priority
            });
            return db.SaveChanges() > 0;
        }
    }

    public class Option : BaseModel
    {
        [Column(TypeName = "varchar(100)")]
        public string Name { get; set; }
        public int Price { get; set; }
        public int Priority { get; set; }
        [JsonIgnore]
        public ICollection<Menu> MenuList { get; set; }

        public static Option GetOption(int optionId)
        {
            using var db = new ArumDbContext();
            return db.Option.Single(o => o.Id == optionId);
        }

        public static IEnumerable<Option> GetOptions(int menuId)
        {
            using var db = new ArumDbContext();
            var menu = db.Menu.Include(m => m.OptionList).Single(m => m.Id == menuId);
            return menu.OptionList;
        }

        public static bool Add(string name, int price, int priority)
        {
            using var db = new ArumDbContext();
            db.Add(new Option()
            {
                Name = name,
                Price = price,
                Priority = priority,
            });
            return db.SaveChanges() > 0;
        }
    }
}
