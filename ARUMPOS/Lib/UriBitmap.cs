using Avalonia.Media.Imaging;
using Avalonia.Shared.PlatformSupport;
using System;

namespace ARUMPOS.Lib
{
    public class UriBitmap : Bitmap
    {
        public UriBitmap(Uri uri) : base(new AssetLoader().Open(uri))
        {

        }
    }
}