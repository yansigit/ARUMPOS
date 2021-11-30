using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ARUMPOS.Views
{
    public partial class MessagePopupDialogWindow : Window
    {
        public MessagePopupDialogWindow()
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
    }
}
