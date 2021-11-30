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
    public class TodayOrderViewModel : ViewModelBase
    {
        private AvaloniaList<OrderListItemViewModel> _todayOrderList = new();
        public AvaloniaList<OrderListItemViewModel> TodayOrderList
        {
            get => _todayOrderList;
            set => this.RaiseAndSetIfChanged(ref _todayOrderList, value);
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
                TodayOrderList.Clear();
                foreach (var order in Order.GetPaginatedTodayOrders(Program.ShopId, CurrentPage))
                {
                    TodayOrderList.Add(new OrderListItemViewModel(order));
                }
            }
        }

        private int LatestIndex { get; set; } = -1;

        private void RefreshList(object source, System.Timers.ElapsedEventArgs e)
        {
            var orders = Order.GetPaginatedTodayOrders(Program.ShopId);
            if (LatestIndex != 0 && orders[0].Id == LatestIndex)
            {
                Debug.WriteLine($"변경사항이 없습니다 {orders[0].Id} : {LatestIndex}");
                return;
            }

            // try
            // {
            //     if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime {
            //         MainWindow: { }
            //     } desktop)
            //     {
            //         desktop.MainWindow.WindowState = WindowState.Maximized;
            //         desktop.MainWindow.Activate();
            //     }
            // }
            // catch (Exception err)
            // {
            //     Debug.WriteLine(err);
            // }

            foreach (var order in orders)
            {
                if (order.Id == LatestIndex || LatestIndex == -1)
                {
                    break;
                }
                var printData = new List<MenuPrintData>();
                order.MenuList.ForEach(menu =>
                    printData.Add(new MenuPrintData { Name = menu.Name, Quantity = menu.Quantity, OptionString = menu.OptionsToString }));

                var shop = Shop.GetById(Program.ShopId);
                POSFunctions.StartEmployeePrint(order.Id, printData, shop.PrinterCOM, shop.BaudRate);
            }

            LatestIndex = orders[0].Id;
            AvaloniaList<OrderListItemViewModel> list = new();
            foreach (var order in orders)
            {
                list.Add(new OrderListItemViewModel(order));
            }
            TodayOrderList = list;

            // 윈도우에서만 가능
            var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
            var (stream, _) = assets.OpenAndGetAssembly(new Uri("avares://ARUMPOS/Assets/order_sound.wav"));
            System.Media.SoundPlayer player = new(stream);
            player.Load();
            player.Play();

            Debug.WriteLine($"루틴 {e.SignalTime.Second}");
        }

        private const int Interval = 5000;

        public TodayOrderViewModel()
        {
            // RefreshList(null, null);
            Timer aTimer = new System.Timers.Timer();
            aTimer.Interval = Interval;
            aTimer.Elapsed += RefreshList;
            aTimer.Enabled = true;

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
                if (CurrentPage >= maxPage)
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