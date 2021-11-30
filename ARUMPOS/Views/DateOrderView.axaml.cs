using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ARUMPOS.Views
{
    public partial class DateOrderView : UserControl
    {
        public DateOrderView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
