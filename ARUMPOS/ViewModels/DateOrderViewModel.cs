using ArumModels.Models;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform;
using POSPrinterLibrary;
using ReactiveUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reactive;
using System.Threading;
using Timer = System.Timers.Timer;

namespace ARUMPOS.ViewModels
{
    public class DateOrderViewModel : ViewModelBase
    {
        private DateTimeOffset _selectedDate;
        public DateTimeOffset SelectedDate
        {
            get => _selectedDate;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedDate, value);
                var orders = Order.GetOrdersByDate(_selectedDate.Date, Program.ShopId);
                AvaloniaList<OrderListItemViewModel> list = new();
                foreach (var order in orders)
                {
                    list.Add(new OrderListItemViewModel(order));
                }
                OrderList = list;
            }
        }

        private AvaloniaList<OrderListItemViewModel> _orderList = new();
        public AvaloniaList<OrderListItemViewModel> OrderList
        {
            get => _orderList;
            set => this.RaiseAndSetIfChanged(ref _orderList, value);
        }

        public ReactiveCommand<Unit, Unit> GoPageBefore { get; set; }
        public ReactiveCommand<Unit, Unit> GoPageAfter { get; set; }

        private int _currentPage = 1;
        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                this.RaiseAndSetIfChanged(ref _currentPage, value);
                OrderList.Clear();
                foreach (var order in Order.GetPaginatedTodayOrders(Program.ShopId, CurrentPage))
                {
                    OrderList.Add(new OrderListItemViewModel(order));
                }
            }
        }

        private const int Interval = 5000; 

        public DateOrderViewModel()
        {
            GoPageBefore = ReactiveCommand.Create(() =>
            {
                if (CurrentPage <= 1)
                {
                    return;
                }

                CurrentPage -= 1;
            });

            const int maxPage = 10;
            GoPageAfter = ReactiveCommand.Create(() =>
            {
                if(CurrentPage >= maxPage)
                {
                    return;
                }

                if (Order.GetPaginatedTodayOrders(Program.ShopId, CurrentPage + 1).Count < 1)
                {
                    return;
                }

                CurrentPage += 1;
            });
        }
    }
}