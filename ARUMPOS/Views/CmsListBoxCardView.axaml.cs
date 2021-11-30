using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ARUMPOS.Views
{
    public partial class CmsListBoxCardView : UserControl
    {
        public CmsListBoxCardView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
