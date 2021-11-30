using ArumModels.Models;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media;
using Material.Icons;
using POSPrinterLibrary;
using ReactiveUI;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;

namespace ARUMPOS.ViewModels
{
    public class OrderListItemViewModel : ViewModelBase
    {
        public OrderListItemViewModel() : this(new Order { Id = 999 }) { }

        public OrderListItemViewModel(Order order)
        {
            Order = order;
            // HeaderText = Order.Id + " : " + MenusNameToString;
            HeaderText = $"{MenusNameToString} (주문번호: {Order.Id})";

            // 버튼 보이기 여부
            if (order.IsCanceled)
            {
                IsEstimatedTimeButtonVisible = false;
                IsProcessButtonVisible = false;
                HeaderText = $"취소된 주문입니다 (주문번호: {Order.Id})";
            } else if (order.EstimatedTime > new DateTime(2021, 1, 1))
            {
                IsEstimatedTimeButtonVisible = false;
                IsProcessButtonVisible = true;
            }

            // 버튼 Init
            switch (Order.Status)
            {
                case 0:
                    ButtonIcon = MaterialIconKind.PaperCheck;
                    ButtonString = "주문승인하기";
                    break;
                case 1:
                    ButtonIcon = MaterialIconKind.CoffeeMachine;
                    ButtonString = "제조완료하기";
                    break;
                case 2:
                    ButtonIcon = MaterialIconKind.Done;
                    ButtonString = "완료된 주문 (영수증프린트)";
                    break;
            }

            // 진행 버튼 이벤트
            ProcessCommand = ReactiveCommand.Create(() =>
            {
                switch (Order.Status)
                {
                    case 0:
                        ButtonIcon = MaterialIconKind.CoffeeMachine;
                        ButtonString = "제조완료하기";
                        Order = Order.IncreaseOrderStatus(order.Id);
                        break;
                    case 1:
                        ButtonIcon = MaterialIconKind.Done;
                        ButtonString = "완료된 주문 (영수증프린트)";
                        Order = Order.IncreaseOrderStatus(order.Id);
                        break;
                    case 2:
                        var menuList = order.MenuList.Select(orderedMenu => (orderedMenu.Name, orderedMenu.Price, orderedMenu.Quantity)).ToList();
                        var shop = Shop.GetById(order.ShopId);
                        var customerPrintData = new CustomerPrintData() {
                            Card = (cardCompany:order.CardName, cardNumber:order.CardNumber, tid:order.Moid, franchiseCode:order.PaymentCode),
                            MenuList = menuList,
                            Store = (shop.LicenseNumber, shop.OwnerName, shop.Name, shop.Address, shop.PhoneNumber),
                            Order = (order.Id, order.TotalPrice)
                        };
                        POSPrinterLibrary.POSFunctions.StartCustomerPrint(customerPrintData, shop.PrinterCOM, shop.BaudRate);
                        break;
                }
            });

            SetEstimatedTime = ReactiveCommand.Create<int>(minutes =>
            {
                Order.SetEstimatedTime(order.Id, minutes);
                IsProcessButtonVisible = true;
                IsEstimatedTimeButtonVisible = false;
            });

            CancelOrder = ReactiveCommand.Create(() =>
            {
                if (App.Current.ApplicationLifetime is not ClassicDesktopStyleApplicationLifetime cda)
                {
                    return;
                }

                if (order.Moid is null)
                {
                    var messageBoxStandardWindow = MessageBox.Avalonia.MessageBoxManager
                        .GetMessageBoxStandardWindow("ERROR", "NO MOID VALUE");
                    messageBoxStandardWindow.ShowDialog(cda.MainWindow);
                    return;
                };

                var secretKey = Shop.GetById(Program.ShopId).ItnjSecretKey;
                var httpClient = Program.SetHttpClientItnjSecretKey(secretKey);
                var result = httpClient.DeleteAsync($"http://pay.itnj.co.kr:19000/payment/v1.0/cancel/card/{order.Moid}").Result;
                
                if (!result.IsSuccessStatusCode)
                {
                    var messageBoxStandardWindow = MessageBox.Avalonia.MessageBoxManager
                        .GetMessageBoxStandardWindow("ERROR", "FAILED TO FETCH DELETE REQUEST TO PG SERVER");
                    messageBoxStandardWindow.ShowDialog(cda.MainWindow);

                    return;
                }

                Order.Cancel(order.Id);
                IsEstimatedTimeButtonVisible = false;
                HeaderText = $"취소된 주문입니다 (주문번호: {Order.Id})";
            });
        }

        public ReactiveCommand<Unit, Unit> ProcessCommand { get; set; }
        private MaterialIconKind _buttonIcon = MaterialIconKind.PaperCheck;
        public MaterialIconKind ButtonIcon { get => _buttonIcon; set => this.RaiseAndSetIfChanged(ref _buttonIcon, value); }
        private string _buttonString;
        public string ButtonString { get => _buttonString; set => this.RaiseAndSetIfChanged(ref _buttonString, value); }

        public string MenusNameToString => Order.MenusName;
        public Order Order { get; set; }

        private string _headerText;
        public string HeaderText { get => _headerText; set => this.RaiseAndSetIfChanged(ref _headerText, value); }

        private bool _isProcessButtonVisible;
        public bool IsProcessButtonVisible { get => _isProcessButtonVisible;
            set => this.RaiseAndSetIfChanged(ref _isProcessButtonVisible, value);
        }

        private bool _isEstimatedTimeButtonVisible = true;
        public bool IsEstimatedTimeButtonVisible
        {
            get => _isEstimatedTimeButtonVisible;
            set => this.RaiseAndSetIfChanged(ref _isEstimatedTimeButtonVisible, value);
        }

        public IBrush Background => (Order.Id % 2) == 0 ? new SolidColorBrush(Colors.White) : new SolidColorBrush(Colors.WhiteSmoke);

        public ReactiveCommand<int, Unit> SetEstimatedTime { get; set; }
        public ReactiveCommand<Unit, Unit> CancelOrder { get; set; }
    }
}