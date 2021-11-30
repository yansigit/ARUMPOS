using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArumModels.Models;
using Avalonia.Collections;
using ReactiveUI;
using System.Collections;
using System.Data;
using System.Reactive;
using POSPrinterLibrary;
using System.Diagnostics;

namespace ARUMPOS.ViewModels
{
    public class CalculateViewModel : ViewModelBase
    {
        private DateTimeOffset _selectedDate = DateTimeOffset.Now;
        public ReactiveCommand<string, Unit> PrintJungsan { get; set; }

        public DateTimeOffset SelectedDate
        {
            get => _selectedDate;
            set
            {
                CurrentDate = $"{value.Year}년 {value.Month}월 정산내역";
                this.RaiseAndSetIfChanged(ref _selectedDate, value);
                BuildPage(false);
            }
        }

        private string _currentDate = "몇월 며칠 정산내역";
        public string CurrentDate
        {
            get => _currentDate;
            set => this.RaiseAndSetIfChanged(ref _currentDate, value);
        }

        public CalculateViewModel()
        {
            BuildPage(false);
            PrintJungsan = ReactiveCommand.Create<string>((param) =>
            {
                switch (param)
                {
                    // 정산정보 새로고침
                    case "day":
                        BuildPageForDate(true);
                        break;
                    case "month":
                        BuildPage(true);
                        break;
                }
            });
        }

        private string _sales;
        public string Sales { get => _sales; set => this.RaiseAndSetIfChanged(ref _sales, value); }

        private string _discount;
        public string Discount { get => _discount; set => this.RaiseAndSetIfChanged(ref _discount, value); }

        private string _income;
        public string Income { get => _income; set => this.RaiseAndSetIfChanged(ref _income, value); }

        public void BuildPage(bool doPrint)
        {
            CurrentDate = $"{SelectedDate.Year}년 {SelectedDate.Month}월 정산내역";

            var orderList = Order.GetOrdersByMonth(SelectedDate.DateTime, Program.ShopId);
            var table = new Dictionary<int, (int total, int discount)>();

            foreach (var date in Enumerable.Range(1, DateTime.DaysInMonth(SelectedDate.Year, SelectedDate.Month)))
            {
                table[date] = (0, 0);
            }

            var sales = 0;
            var discount = 0;
            foreach (var order in orderList)
            {
                table[order.CreatedDateTime.Day] = (order.TotalPrice + table[order.CreatedDateTime.Day].total,
                    order.DiscountPrice + table[order.CreatedDateTime.Day].discount);
                sales += order.TotalPrice;
                discount += order.DiscountPrice;
            }

            var income = sales - discount;
            Sales = $"매출: \\{sales}";
            Discount = $"할인: \\{discount}";
            Income = $"순매출: \\{income}";

            var tmp = new AvaloniaList<JungsanData>();
            foreach (var date in table)
            {
                tmp.Add(new JungsanData()
                {
                    Date = new DateTime(SelectedDate.Year, SelectedDate.Month, date.Key),
                    SalesPrice = date.Value.total,
                    DiscountPrice = date.Value.discount,
                    ProfitPrice = date.Value.total - date.Value.discount
                });
            }
            OrderList = tmp;

            if (!doPrint)
            {
                return;
            }

            // 프린트
            var pSales = OrderList.Sum(d => d.SalesPrice);
            var pDiscount = OrderList.Sum(d => d.DiscountPrice);
            var pCanceled = OrderList.Sum(d => d.CanceledPrice);
            var data = new POSPrinterLibrary.JungsanData {
                Date = SelectedDate.Date,
                SalesPrice = OrderList.Sum(d => d.SalesPrice),
                DiscountPrice = OrderList.Sum(d => d.DiscountPrice),
                CanceledPrice = pCanceled,
                ProfitPrice = pSales - pDiscount - pCanceled
            };

            var shop = Shop.GetById(Program.ShopId);
            POSFunctions.StartPrintJungsan(true, data, shop.PrinterCOM, shop.BaudRate);
        }

        public void BuildPageForDate(bool doPrint)
        {
            CurrentDate = $"{SelectedDate.Year}년 {SelectedDate.Month}월 {SelectedDate.Day}일 정산내역";

            var orderList = Order.GetOrdersByDate(SelectedDate.DateTime.Date, Program.ShopId);
            var sales = orderList.Sum(order => order.TotalPrice);
            var discount = orderList.Sum(order => order.DiscountPrice);
            var cancel = orderList.Where(e => e.IsCanceled).Sum(order => order.TotalPrice);

            var income = sales - discount - cancel;
            Sales = $"매출: \\{sales - cancel}";
            Discount = $"할인: \\{discount}";
            Income = $"순매출: \\{income}";

            var tmp = new AvaloniaList<JungsanData>();
            OrderList = tmp;

            if (!doPrint)
            {
                return;
            }

            // 프린트
            var pSales = sales;
            var pDiscount = discount;
            var pCanceled = cancel;
            var data = new POSPrinterLibrary.JungsanData {
                Date = SelectedDate.Date,
                SalesPrice = pSales,
                DiscountPrice = pDiscount,
                CanceledPrice = pCanceled,
                ProfitPrice = pSales - pDiscount - pCanceled
            };

            var shop = Shop.GetById(Program.ShopId);
            POSFunctions.StartPrintJungsan(false, data, shop.PrinterCOM, shop.BaudRate);

            BuildPage(false);
        }

        private AvaloniaList<JungsanData> _orderList;
        public AvaloniaList<JungsanData> OrderList { get => _orderList; set => this.RaiseAndSetIfChanged(ref _orderList, value); }
    }
}
