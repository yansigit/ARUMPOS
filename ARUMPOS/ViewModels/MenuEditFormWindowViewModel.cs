using ArumModels.Migrations;
using ArumModels.Models;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Material.Dialog;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reactive;
using System.Threading.Tasks;
using Menu = ArumModels.Models.Menu;

namespace ARUMPOS.ViewModels
{
    public class MenuEditFormWindowViewModel : ViewModelBase
    {
        private IImage _imageSource;
        public IImage ImageSource { get => _imageSource; set => this.RaiseAndSetIfChanged(ref _imageSource, value); }

        public List<Ingredient> Ingredients { get; set; } = Shop.GetAllCategoriesAndMenus(Program.ShopId).Categories.SelectMany(e => e.MenuList).SelectMany(e => e.IngredientList).Distinct().ToList();
        public List<Option> Options { get; set; } = Shop.GetAllCategoriesAndMenus(Program.ShopId).Categories.SelectMany(e => e.MenuList).SelectMany(e => e.OptionList).Distinct().ToList();

        public List<Category> Categories { get; set; } =
            Shop.GetAllCategoriesAndMenus(Program.ShopId).Categories.ToList();

        public ReactiveCommand<Unit, Task> AddImage { get; set; }
        
        public ReactiveCommand<int, Unit> SetCategory { get; set; }
        public ReactiveCommand<int, Unit> SetOption { get; set; }
        public ReactiveCommand<int, Unit> SetIngredient { get; set; }

        public ReactiveCommand<Unit, Task> AddMenu { get; set; }

        public string Name { get; set; } = "";
        public string ColdPrice { get; set; } = "0";
        public string HotPrice { get; set; } = "0";

        private int categoryId = 0;
        private string imagePath = "https://arumdream.s3.ap-northeast-2.amazonaws.com/uploads/blank.png";
        private List<int> ingredientIdList;
        private List<int> optionIdList;
        private bool isModify = false;
        
        public Dictionary<int, bool> IngredientTable { get; set; } = new();
        public Dictionary<int, bool> OptionTable { get; set; } = new();

        public string FilePath { get; set; }
        public Menu Menu { get; set; }

        public MenuEditFormWindowViewModel()
        {
            foreach (var t in Ingredients)
            {
                Debug.WriteLine($"{t.Id}: {t.Name} false");
                IngredientTable.Add(t.Id, false);
            }

            foreach (var o in Options)
            {
                Debug.WriteLine($"{o.Id}: false");
                OptionTable.Add(o.Id, false);
            }

            SetCategory = ReactiveCommand.Create<int>(id =>
            {
                categoryId = id;
            });

            SetOption = ReactiveCommand.Create<int>(id =>
            {
                OptionTable[id] = !OptionTable[id];
                Debug.WriteLine(OptionTable[id]);
            });

            SetIngredient = ReactiveCommand.Create<int>(id =>
            {
                IngredientTable[id] = !IngredientTable[id];
                Debug.WriteLine(IngredientTable[id]);
            });

            AddImage = ReactiveCommand.Create<Task>(async () =>
            {
                var fileDialog = new OpenFileDialog { Filters = new List<FileDialogFilter>(
                    new [] {
                        new FileDialogFilter() {
                            Name = "이미지",
                            Extensions = new List<string>() {
                                "jpg", "png", "gif"
                            }
                        }
                    })
                };

                if (Application.Current.ApplicationLifetime is not ClassicDesktopStyleApplicationLifetime cda)
                {
                    return;
                }

                var result = await fileDialog.ShowAsync(cda.Windows[1]);
                FilePath = result[0];
                ImageSource = new Bitmap(FilePath);
            });

            AddMenu = ReactiveCommand.Create<Task>(async () =>
            {
                ingredientIdList = IngredientTable.Where(e => e.Value).Select(e => e.Key).ToList();
                optionIdList = OptionTable.Where(e => e.Value).Select(e => e.Key).ToList();
                
                if (Application.Current.ApplicationLifetime is not ClassicDesktopStyleApplicationLifetime cda)
                {
                    return;
                }

                if (categoryId == 0 || Name.Length <= 0)
                {
                    await DialogHelper.CreateAlertDialog(new AlertDialogBuilderParams
                    {
                        Borderless = false,
                        ContentHeader = "오류",
                        SupportingText = "카테고리, 이름을 선택했는지 확인하세요.",
                        StartupLocation = WindowStartupLocation.CenterScreen
                    }).ShowDialog(cda.MainWindow);
                    return;
                }

                // 이미지 업로드
                if (FilePath is not null)
                {
                    imagePath = await Lib.Common.UploadFileAsync(FilePath, Name, variant:"menus");
                    imagePath = "https://arumdream.s3.ap-northeast-2.amazonaws.com/" + imagePath;
                }

                if (ColdPrice == "")
                {
                    ColdPrice = "0";
                }
                if (HotPrice == "")
                {
                    HotPrice = "0";
                }

                if (int.TryParse(ColdPrice, out var coldPrice) && int.TryParse(HotPrice, out var hotPrice))
                {
                    bool result;
                    var word = "추가";
                    if (isModify)
                    {
                        word = "수정";
                        result = Menu.Modify(Menu.Id, categoryId, Name, imagePath, ingredientIdList, optionIdList,
                            hotPrice, coldPrice);
                    }
                    else
                    {
                        result = Menu.Add(categoryId, Name, imagePath, ingredientIdList, optionIdList,
                            hotPrice, coldPrice);
                    }

                    if (!result)
                    {
                        await DialogHelper.CreateAlertDialog(new AlertDialogBuilderParams
                        {
                            Borderless = false,
                            ContentHeader = "오류",
                            SupportingText = $"메뉴를 {word}할 수 없었습니다",
                            StartupLocation = WindowStartupLocation.CenterScreen
                        }).ShowDialog(cda.MainWindow);
                        return;
                    }
                    await DialogHelper.CreateAlertDialog(new AlertDialogBuilderParams
                    {
                        Borderless = false,
                        ContentHeader = "성공",
                        SupportingText = $"메뉴를 {word}했습니다",
                        WindowTitle = "알림",
                        StartupLocation = WindowStartupLocation.CenterScreen
                    }).ShowDialog(cda.MainWindow);
                    cda.Windows[1].Close();
                }
                else
                {
                    await DialogHelper.CreateAlertDialog(new AlertDialogBuilderParams {
                        Borderless = false,
                        ContentHeader = "오류",
                        SupportingText = "가격값이 잘못되었습니다",
                        StartupLocation = WindowStartupLocation.CenterScreen
                    }).ShowDialog(cda.MainWindow);
                }
            });
        }

        public MenuEditFormWindowViewModel(Menu menu): this()
        {
            Menu = menu;
            isModify = true;
            Name = menu.Name;
            ColdPrice = menu.ColdPrice.ToString();
            HotPrice = menu.HotPrice.ToString();
            imagePath = menu.ImagePath;
            ImageSource = new Bitmap(new MemoryStream(new WebClient().DownloadData(menu.ImagePath)));
        }
    }
}