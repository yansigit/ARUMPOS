using ArumModels.Models;
using ARUMPOS.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using System;
using System.Diagnostics;

namespace ARUMPOS.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        protected override void OnOpened(EventArgs e)
        {
            base.OnOpened(e);
            Shop.SetIsOpened(Program.ShopId, true);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Shop.SetIsOpened(Program.ShopId, false);
        }
    }
}
