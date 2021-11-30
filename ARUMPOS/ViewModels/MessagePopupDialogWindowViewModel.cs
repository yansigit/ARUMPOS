using ARUMPOS.Lib;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace ARUMPOS.ViewModels
{
    public class MessagePopupDialogWindowViewModel : ViewModelBase
    {
        private IImage _animatedImage;

        public IImage AnimationImage
        {
            get => _animatedImage;
            set => this.RaiseAndSetIfChanged(ref _animatedImage, value);
        }

        public MessagePopupDialogWindowViewModel()
        {
            // AnimationImage = new Bitmap(new FileStream(File.OpenRead("C:\\Users\\itnj\\Downloads\\animated-image\\1.png").SafeFileHandle, FileAccess.Read));
            List<string> imageSourceList = new();
            foreach (var _imgIdx in Enumerable.Range(1, 10))
            {
                var target = $"C:\\Users\\itnj\\Downloads\\animated-image\\{_imgIdx}.png";
                imageSourceList.Add(target);
            }
            new ServeAnimatedImage(this, imageSourceList).Start();
        }
    }
}
