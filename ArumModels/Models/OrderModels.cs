using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ArumModels.Models
{
    public class Order : BaseModel
    {
        public int TotalPrice { get; set; }
        public int DiscountPrice { get; set; }
        public int Status { get; set; }
        public DateTime EstimatedTime { get; set; }
        public User User { get; set; }
        public List<OrderedMenu> MenuList { get; set; } = new();
        public int ShopId { get; set; }
        public bool IsCanceled { get; set; }

        [Column(TypeName = "varchar(100)")]
        public string CardName { get; set; }

        [Column(TypeName = "varchar(100)")]
        public string CardNumber { get; set; }
        
        [Column(TypeName = "varchar(100)")]
        public string Message { get; set; }
        
        [Column(TypeName = "varchar(100)")]
        public string Tid { get; set; }

        [Column(TypeName = "varchar(10)")]
        public string Type { get; set; }
        
        [Column(TypeName = "varchar(100)")]
        public string PaymentCode { get; set; }

        [Column(TypeName = "varchar(100)")]
        public string Moid { get; set; }

        public bool IsDeleted { get; set; }

        [JsonIgnore]
        public string MenusName
        {
            get
            {
                var nameList = "할당된 메뉴가 없는 주문입니다";
                if (MenuList.Count > 0)
                {
                    nameList = MenuList.Select(e => e.Name).Aggregate((x, y) => x + ", " + y);
                }

                return nameList;
            }
        }

        public static bool Cancel(int orderId)
        {
            var db = new ArumDbContext();
            var order = db.Order.Single(e => e.Id == orderId);
            order.IsCanceled = true;
            return db.SaveChanges() > 0;
        }

        public static bool Delete(int orderId)
        {
            var db = new ArumDbContext();
            var order = db.Order.Single(e => e.Id == orderId);
            order.IsDeleted = true;
            return db.SaveChanges() > 0;
        }

        public static bool SetEstimatedTime(int orderId, int minutes)
        {
            var db = new ArumDbContext();
            var order = db.Order.Single(e => e.Id == orderId);
            order.EstimatedTime = DateTime.Now.AddMinutes(minutes);
            return db.SaveChanges() > 0;
        }

        public static Order GetOrder(int orderId)
        {
            using var db = new ArumDbContext();
            return db.Order.Include(e => e.MenuList).ThenInclude(e => e.OptionList)
                .Include(e => e.User)
                .Single(e => e.Id == orderId);
        }

        public static bool ChangeOrderTypeByMoid(string moid, string type)
        {
            using var db = new ArumDbContext();
            var order = db.Order.Single(e => e.Moid == moid);
            order.Type = type;
            return db.SaveChanges() > 0;
        }

        // todo shopId 추가
        public static List<Order> GetPaginatedTodayOrders(int shopId, int page = 1)
        {
            page -= 1;
            const int elements = 5;
            using var db = new ArumDbContext();
            var today = DateTime.Today.Date;
            return db.Order
                .Include(o => o.MenuList)
                .ThenInclude(e => e.OptionList)
                .Where(o => o.CreatedDateTime.Date == today && o.ShopId == shopId)
                .OrderByDescending(o => o.Id)
                .Skip(page * elements)
                .Take(elements)
                .ToList();
        }

        // todo shopId 추가
        public static List<Order> GetOrdersByDate(DateTime date, int shopId)
        {
            using var db = new ArumDbContext();
            var result = db.Order.Include(e => e.MenuList)
                .Where(e =>
                e.CreatedDateTime.Year == date.Year && e.CreatedDateTime.Day == date.Day &&
                e.CreatedDateTime.Month == date.Month && e.ShopId == shopId).ToList();
            return result;
        }

        public static List<Order> GetOrdersByMonth(DateTime date, int shopId)
        {
            using var db = new ArumDbContext();
            return db.Order.Include(e => e.MenuList)
                .Where(e => e.CreatedDateTime.Year == date.Year && e.CreatedDateTime.Month == date.Month && e.ShopId == shopId)
                .ToList();
        }

        public static IEnumerable<Order> GetOrdersByUserId(int id, int page)
        {
            using var db = new ArumDbContext();
            const int cnt = 10;
            return db.Order.Where(e => e.User.Id == id).OrderByDescending(e => e.CreatedDateTime)
                .Skip(cnt * (page - 1)).Take(cnt).ToList();
        }

        public static bool SaveOrderInformation(string jsonString)
        {
            try
            {
                var order = JsonConvert.DeserializeObject<Order>(jsonString);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public static bool SaveOrderInformation(Order order)
        {
            using var db = new ArumDbContext();
            var user = db.User.Single(e => e.Id == order.User.Id);
            order.User = user;
            db.Order.Add(order);
            return db.SaveChanges() > 0;
        }

        public static ICollection<Order> GetRecentOrders(int userId, int i)
        {
            using var db = new ArumDbContext();
            return db.Order
                .Include(e => e.User)
                .Include(e => e.MenuList)
                .ThenInclude(e => e.OptionList)
                .Where(e => e.User.Id == userId)
                .OrderByDescending(e => e.CreatedDateTime)
                .Take(i).ToList();
        }

        public static Order IncreaseOrderStatus(int orderId)
        {
            var db = new ArumDbContext();
            var order = db.Order.Single(e => e.Id == orderId);
            order.Status += 1;
            db.SaveChanges();
            return order;
        }
    }

    public class OrderedMenu : BaseModel
    {
        [Column(TypeName = "varchar(100)")]
        public string Name { get; set; }
        public int Price { get; set; }
        public int Quantity { get; set; }
        [Column(TypeName = "varchar(500)")]
        public string ImagePath { get; set; }
        public List<OrderedOption> OptionList { get; set; } = new();
        [Column(TypeName = "varchar(20)")]
        public string BackgroundColor { get; set; }

        public override string ToString()
        {
            return $"{Name} {Quantity}개 {Price}원 옵션: [{OptionsToString}]";
        }

        // todo shopId 추가
        public static ICollection<OrderedMenu> GetOrderedMenusByDate(DateTime date, int shopId)
        {
            using var db = new ArumDbContext();
            var orderedMenus = Order.GetOrdersByDate(date, shopId).SelectMany(e => e.MenuList).ToList();
            return orderedMenus;
        }
        
        [JsonIgnore]
        public string OptionsToString
        {
            get
            {
                return OptionList.Aggregate("", (current, orderOption) => current + (orderOption + "\n"));
            }
        }
    }

    public class OrderedOption : BaseModel
    {
        [Column(TypeName = "varchar(100)")]
        public string Name { get; set; }
        public int Quantity { get; set; }
        [Column(TypeName = "varchar(200)")]
        public string Body { get; set; }

        public override string ToString()
        {
            return Quantity <= 0 ? $"{Name} {Body}" : $"{Name} {Quantity}개 {Body}";
        }
    }
}
