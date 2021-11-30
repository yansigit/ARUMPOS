using ArumModels.Models;
using Avalonia.Media;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Text.RegularExpressions;

namespace ARUMPOS.ViewModels
{
    public class DashboardViewModel : ViewModelBase
    {
        private string _storeCloseButtonText = "마감";

        private string _todayIncome;

        private string _todaySales;

        private string _todayTopProduct = "메뉴";

        public DashboardViewModel()
        {
            var shopName = Shop.GetById(Program.ShopId).Name;
            ShopName = IsStoreClosed ? shopName + "(마감됨)" : shopName;

            SwitchIsStoreClosed = ReactiveCommand.Create(() =>
            {
                IsStoreClosed = !IsStoreClosed;
                ShopNameBackground = IsStoreClosed ? Brush.Parse("#1e1e1e") : Brush.Parse("#ff5722");
                Shop.SetIsOpened(Program.ShopId, !IsStoreClosed);

                StoreCloseButtonText = IsStoreClosed ? "개시" : "마감";
                ShopName = IsStoreClosed ? shopName + "(마감됨)" : shopName;
            });

            RefreshCommand = ReactiveCommand.Create(() =>
            {
                InitDashboard();
                this.RaisePropertyChanged(nameof(TodayIncomePercentage));
                this.RaisePropertyChanged(nameof(TodaySalesPercentage));
                this.RaisePropertyChanged(nameof(TodayTopProductPercentage));
                Debug.WriteLine("RefreshCommand");
            });

            ShopName = Shop.GetById(Program.ShopId).Name;
            ShopNameBackground = IsStoreClosed ? Brush.Parse("#1e1e1e") : Brush.Parse("#ff5722");

            InitDashboard();
        }

        public bool IsStoreClosed { get; set; }
        public string TodayIncome { get => _todayIncome; set => this.RaiseAndSetIfChanged(ref _todayIncome, value); }

        private string _shopName = "점포명";
        public string ShopName
        {
            get => _shopName;
            set => this.RaiseAndSetIfChanged(ref _shopName, value);
        }

        private IBrush _shopNameBackground;
        public IBrush ShopNameBackground { get => _shopNameBackground; set => this.RaiseAndSetIfChanged(ref _shopNameBackground, value); }

        public string TodayIncomePercentage
        {
            get
            {
                double ordersYesterday =
                    Order.GetOrdersByDate(DateTime.Today - TimeSpan.FromDays(1), Program.ShopId).Sum(e => e.TotalPrice);
                var priceNumber = Regex.Match(TodayIncome, "\\d+").Value;
                var tmp = (double.Parse(priceNumber) - ordersYesterday) / ordersYesterday * 100;
                return Math.Round(tmp, 2) < 9999 ? $"{Math.Round(tmp, 2)}%" : "전일매출없음";
            }
        }

        public string TodaySales { get => _todaySales; set => this.RaiseAndSetIfChanged(ref _todaySales, value); }
        public string TodaySalesPercentage
        {
            get
            {
                double ordersYesterday = Order.GetOrdersByDate(DateTime.Today - TimeSpan.FromDays(1), Program.ShopId).Count;
                var countNumber = Regex.Match(TodaySales, "\\d+").Value;
                var tmp = (int.Parse(countNumber) - ordersYesterday) / ordersYesterday * 100;
                return Math.Round(tmp, 2) < 9999 ? $"{Math.Round(tmp, 2)}%" : "전일매출없음";
            }
        }

        public string TodayTopProduct
        {
            get => _todayTopProduct;
            set => this.RaiseAndSetIfChanged(ref _todayTopProduct, value);
        }
        public string TodayTopProductPercentage => "인기메뉴";

        public string TodayMenuCounts
        {
            get
            {
                var tmp = OrderedMenu.GetOrderedMenusByDate(DateTime.Today, Program.ShopId).Sum(e => e.Quantity);
                return $"총 {tmp}건";
            }
        }

        public ICollection<string> TodayMenuItems
        {
            get
            {
                Dictionary<string, int> table = new();
                foreach (var orderedMenu in OrderedMenu.GetOrderedMenusByDate(DateTime.Today, Program.ShopId))
                {
                    if (!table.ContainsKey(orderedMenu.Name))
                    {
                        table.Add(orderedMenu.Name, 0);
                    }

                    table[orderedMenu.Name] += orderedMenu.Quantity;
                }

                List<string> tmp = new();
                foreach (var (key, value) in table)
                {
                    tmp.Add($"● {key}: {value}건");
                }

                return tmp;
            }
        }

        public string StoreCloseButtonText
        {
            get => _storeCloseButtonText;
            set => this.RaiseAndSetIfChanged(ref _storeCloseButtonText, value);
        }

        public ReactiveCommand<Unit, Unit> SwitchIsStoreClosed { get; set; }

        public ReactiveCommand<Unit, Unit> RefreshCommand { get; set; }

        public Dictionary<string, int> Counter = new();

        private void InitDashboard()
        {
            TodayIncome = $"\\ {Order.GetOrdersByDate(DateTime.Today, Program.ShopId).Sum(e => e.TotalPrice)}";
            TodaySales = $"{Order.GetOrdersByDate(DateTime.Today, Program.ShopId).Count} 건";
            OrderedMenu.GetOrderedMenusByDate(DateTime.Today, Program.ShopId).Select(e => e.Name).ToList().ForEach(e =>
            {
                if (Counter.ContainsKey(e))
                {
                    Counter[e] += 1;
                }
                else
                {
                    Counter[e] = 1;
                }
            });
            if (Counter.Count > 0)
            {
                TodayTopProduct = Counter.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
            }
        }
    }
}