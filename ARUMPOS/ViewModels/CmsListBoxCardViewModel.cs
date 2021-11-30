using ArumModels.Models;
using Avalonia;
using Avalonia.Markup.Xaml.XamlIl.Runtime;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace ARUMPOS.ViewModels
{
    public class CmsListBoxCardViewModel : ViewModelBase
    {
        public CmsListBoxCardViewModel()
        {

        }

        // 1: 카테고리, 2: 메뉴, 3: 옵션, 4: 재료
        public CmsListBoxCardViewModel(BaseModel model, int status)
        {
            var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
            ImageSource = new Bitmap(assets.Open(new Uri("avares://ARUMPOS/Assets/options.png")));

            Variant = status;
            using var webclient = new WebClient();
            switch (Variant)
            {
                case 1:
                    Name = ((Category)model).Name;
                    Model = model;
                    break;
                case 2:
                    Name = ((Menu)model).Name;
                    Model = model;
                    ImageSource = new Bitmap(new MemoryStream(webclient.DownloadData(((Menu)model).ImagePath)));
                    break;
                case 3:
                    Name = ((Option)model).Name;
                    Model = model;
                    break;
                case 4:
                    Name = ((Ingredient)model).Name;
                    ImageSource = new Bitmap(new MemoryStream(webclient.DownloadData(((Ingredient)model).ImagePath)));
                    Model = model;
                    break;
            }
        }

        public BaseModel Model { get; }
        public string Name { get; } = "이름";
        public int Variant { get; }
        private IImage _imageSource;
        public IImage ImageSource { get => _imageSource; set => this.RaiseAndSetIfChanged(ref _imageSource, value); }

        public string HelperText
        {
            get
            {
                switch (Variant)
                {
                    case 1:
                        return $"메뉴 수: {((Category)Model).MenuList.Count}";
                    case 2:
                        var optionCount = ((Menu)Model).OptionList?.Count ?? 0;
                        var ingredientCount = ((Menu)Model).IngredientList?.Count ?? 0;
                        return
                            $"가격: {((Menu)Model).ColdPrice}/{((Menu)Model).HotPrice}\t옵션 수: {optionCount}\t재료 수: {ingredientCount}";
                    case 3:
                        return $"가격: {((Option)Model).Price}";
                    case 4:
                        return $"이미지 경로: {((Ingredient)Model).ImagePath}";
                    default:
                        return "도움말";
                }
            }
        }
    }
}
