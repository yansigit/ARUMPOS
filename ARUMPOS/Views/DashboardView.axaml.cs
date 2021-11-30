using ArumModels.Models;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using ScottPlot.Avalonia;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Dynamic;
using System.Linq;

namespace ARUMPOS.Views
{
    public class DashboardView : UserControl
    {
        public DashboardView()
        {
            InitializeComponent();
            InitPlots();
            Button refreshButton = this.Find<Button>("RefreshButton");
            refreshButton.Click += RefreshButtonOnClick;
        }

        private void RefreshButtonOnClick(object? sender, RoutedEventArgs e)
        {
            InitializeComponent();
            InitPlots();
            Button refreshButton = this.Find<Button>("RefreshButton");
            refreshButton.Click += RefreshButtonOnClick;
        }

        public void InitPlots()
        {
            var todayOrderedMenu = OrderedMenu.GetOrderedMenusByDate(DateTime.Today, Program.ShopId);
            if (todayOrderedMenu.Count <= 0)
            {
                return;
            }

            AvaPlot avaPlot1 = this.Find<AvaPlot>("AvaPlot1");
            Dictionary<string, double> quantityTable = new();

            foreach (var orderedMenu in todayOrderedMenu)
            {
                if (quantityTable.ContainsKey(orderedMenu.Name))
                {
                    quantityTable[orderedMenu.Name] += orderedMenu.Quantity;
                }
                else
                {
                    quantityTable[orderedMenu.Name] = orderedMenu.Quantity;
                }
            }

            var pie = avaPlot1.Plot.AddPie(quantityTable.Values.ToArray());
            pie.Explode = true;
            pie.DonutSize = .25;
            pie.SliceLabels = quantityTable.Keys.ToArray();
            pie.ShowLabels = true;
            // pie.SliceLabelColors = labelColors;
            avaPlot1.Plot.Title("");
            avaPlot1.Render();
            
            List<double> orderSales = new();
            List<string> days = new();
            for (var i = 6; i >= 0; i--)
            {
                var date = DateTime.Today - TimeSpan.FromDays(i);
                var total = Order.GetOrdersByDate(date, Program.ShopId).Sum(e => e.TotalPrice);
                orderSales.Add(total);
                switch (date.DayOfWeek)
                {
                    case DayOfWeek.Monday:
                        days.Add("월");
                        break;
                    case DayOfWeek.Tuesday:
                        days.Add("화");
                        break;
                    case DayOfWeek.Wednesday:
                        days.Add("수");
                        break;
                    case DayOfWeek.Thursday:
                        days.Add("목");
                        break;
                    case DayOfWeek.Friday:
                        days.Add("금");
                        break;
                    case DayOfWeek.Saturday:
                        days.Add("토");
                        break;
                    case DayOfWeek.Sunday:
                        days.Add("일");
                        break;
                }
            }

            AvaPlot avaPlot2 = this.Find<AvaPlot>("AvaPlot2");
            double[] dataX2 = { 1, 2, 3, 4, 5, 6, 7 };
            double[] dataY2 = orderSales.ToArray();
            avaPlot2.Plot.AddScatter(dataX2, dataY2);
            avaPlot2.Plot.Title("");
            avaPlot2.Plot.XTicks(new double[] { 1, 2, 3, 4, 5, 6, 7 }, days.ToArray());
            avaPlot2.Render();

            TextBlock weekSalesTextBlock = this.Find<TextBlock>("WeekSales");
            weekSalesTextBlock.Text = $"총 {orderSales.Sum()}원";
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}