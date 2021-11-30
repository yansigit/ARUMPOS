using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ArumModels.Models
{
    public class ArumDbContext : DbContext
    {
        public DbSet<Shop> Shop { get; set; }
        public DbSet<Order> Order { get; set; }
        public DbSet<OrderedMenu> OrderedMenu { get; set; }
        public DbSet<OrderedOption> OrderedOption { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<Menu> Menu { get; set; }
        public DbSet<Option> Option { get; set; }
        public DbSet<Ingredient> Ingredient { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Coupon> Coupon { get; set; }
        public DbSet<Stamp> Stamp { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            using StreamReader file = File.OpenText(@"settings.json");
            using JsonTextReader reader = new(file);
            var json = (JObject)JToken.ReadFrom(reader);
            var serverString = json["database"].ToObject<string>();

            var serverVersion = new MySqlServerVersion(new Version(8, 0, 23));
            optionsBuilder.UseMySql(serverString, serverVersion);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
            modelBuilder.Entity<User>()
                .HasIndex(u => u.PhoneNumber)
                .IsUnique();
            modelBuilder.Entity<Coupon>()
                .HasKey(c => c.CouponCode);
        }

        public override int SaveChanges()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is BaseModel && (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entityEntry in entries)
            {
                ((BaseModel)entityEntry.Entity).UpdatedDateTime = DateTime.Now;
                if (entityEntry.State == EntityState.Added)
                {
                    ((BaseModel)entityEntry.Entity).CreatedDateTime = DateTime.Now;
                }
            }

            return base.SaveChanges();
        }
    }

    public class Shop : BaseModel
    {
        [Column(TypeName = "varchar(100)")]
        public string Name { get; set; }
        [Column(TypeName = "TinyText")]
        public string Address { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        [Column(TypeName = "TinyText")]
        public string ThumbImage { get; set; }
        public ICollection<Category> Categories { get; set; }
        public ICollection<Order> Orders { get; set; }
        public ICollection<CarouselImage> CarouselImages { get; set; }
        public bool IsOpened { get; set; }
        [Column(TypeName = "varchar(100)")]
        public string LicenseNumber { get; set; }
        [Column(TypeName = "varchar(100)")]
        public string OwnerName { get; set; }
        [Column(TypeName = "varchar(100)")]
        public string PhoneNumber { get; set; }

        [Column(TypeName = "varchar(100)")]
        [JsonIgnore]
        public string ItnjSecretKey { get; set; }

        public int FranchiseCode { get; set; }

        [Column(TypeName = "varchar(100)")]
        public string PrinterCOM { get; set; }
        public int BaudRate { get; set; }

        public static bool SetIsOpened(int shopId, bool open)
        {
            using var c = new ArumDbContext();
            var shop = c.Shop.Single(s => s.Id == shopId);
            shop.IsOpened = open;
            return c.SaveChanges() > 0;
        }

        public static Shop GetById(int id)
        {
            using var c = new ArumDbContext();
            return c.Shop.Single(s => s.Id == id);
        }

        public static IEnumerable<Shop> GetShops()
        {
            using var c = new ArumDbContext();
            return c.Shop.OrderByDescending(s => s.Id).ToList();
        }

        public static Shop GetAllCategoriesAndMenus(int id)
        {
            using var db = new ArumDbContext();
            var tmp = db.Shop.IgnoreAutoIncludes()
                .Include(s => s.Categories).ThenInclude(c => c.MenuList).ThenInclude(m => m.OptionList)
                .Include(s => s.Categories).ThenInclude(c => c.MenuList).ThenInclude(m => m.IngredientList.OrderBy(i => i.Priority))
                .Single(s => s.Id == id);
            return tmp;
        }

        public static IEnumerable<Shop> GetNearbyShops(int i, double latitude, double longitude)
        {
            var queue = new List<(Shop shop, double distance)>();

            using var db = new ArumDbContext();
            var tmp = db.Shop.IgnoreAutoIncludes();
            foreach (var shop in tmp)
            {
                // 거리 구하기
                var distance = Math.Abs(shop.Latitude - latitude) * Math.Abs(shop.Longitude - longitude) / 2;
                // 초기 N개 값 리스트 설정
                if (queue.Count < i)
                {
                    queue.Add((shop, distance));
                    queue.Sort(((t, nt) => (int)((t.distance - nt.distance)*1000000)));
                } else if (queue[^1].distance > distance)
                {
                    queue.Remove(queue[^1]);
                    queue.Add((shop, distance));
                    queue.Sort(((t, nt) => (int)((t.distance - nt.distance)*1000000)));
                }
            }
            
            return queue.Select(e => e.shop).ToList();
        }
    }

    public class CarouselImage
    {
        public int Id { get; set; }
        [Column(TypeName = "TinyText")]
        public string Path { get; set; }
    }

    public abstract class BaseModel
    {
        [Key]
        public int Id { get; set; }
        public DateTime CreatedDateTime { get; set; } = DateTime.Now;
        public DateTime UpdatedDateTime { get; set; } = DateTime.Now;
    }
}