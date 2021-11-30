using ARUMPOS.ViewModels;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace ARUMPOS.Lib
{
    public class ServeAnimatedImage : Timer
    {
        private int _imgIdx = 0;
        public ServeAnimatedImage(ViewModelBase vm, List<string> imageSourceList)
        {
            this.Interval = 100;
            this.Elapsed += (sender, args) =>
            {
                using var sfh = File.OpenRead(imageSourceList.ElementAt(_imgIdx)).SafeFileHandle;
                vm.GetType().GetProperty("AnimationImage").SetValue(vm, new Bitmap(new FileStream(sfh, FileAccess.Read)));
                // ((MessagePopupDialogWindowViewModel)vm).AnimationImage = new Bitmap(new FileStream(sfh, FileAccess.Read));
                _imgIdx += 1;
                if (_imgIdx >= imageSourceList.Count)
                {
                    _imgIdx = 0;
                }
            };
        }
    }
}
