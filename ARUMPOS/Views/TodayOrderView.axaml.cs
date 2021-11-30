using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ARUMPOS.Views
{
    public partial class TodayOrderView : UserControl
    {
        public TodayOrderView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
