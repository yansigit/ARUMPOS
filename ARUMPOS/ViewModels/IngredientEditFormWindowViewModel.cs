using Amazon;
using ARUMPOS.Lib;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Material.Dialog;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Transfer;
using ArumModels.Models;

namespace ARUMPOS.ViewModels
{
    public class IngredientEditFormWindowViewModel : ViewModelBase
    {
        private IImage _imageSource = new UriBitmap(new Uri("avares://ARUMPOS/Assets/coffee.png"));
        public IImage ImageSource
        {
            get => _imageSource;
            set => this.RaiseAndSetIfChanged(ref _imageSource, value);
        }
        public ReactiveCommand<Unit, Task> OpenImagePicker { get; set; }
        public ReactiveCommand<Unit, Task> AddIngredient { get; set; }
        public string FilePath { get; set; }
        public string IngredientName { get; set; } = "";
        public string IngredientPriority { get; set; } = "";

        public IngredientEditFormWindowViewModel()
        {
            OpenImagePicker = ReactiveCommand.Create<Task>(async () =>
            {
                if (App.Current.ApplicationLifetime is ClassicDesktopStyleApplicationLifetime desktopStyleApplication)
                {
                    var dialog = new OpenFileDialog() {
                        Title = "재료 이미지 선택",
                        Filters =
                            new List<FileDialogFilter>()
                            {
                                new()
                                {
                                    Extensions = new List<string>()
                                    {
                                        "jpg", "png", "gif"
                                    },
                                    Name = "Pictures"
                                }
                            }
                    };
                    var result = await dialog.ShowAsync(desktopStyleApplication.Windows[1]);
                    FilePath = result[0];
                    ImageSource = new Bitmap(FilePath);
                    Debug.WriteLine(result[0]);
                }
            });

            AddIngredient = ReactiveCommand.Create<Task>(async () =>
            {
                if (App.Current.ApplicationLifetime is ClassicDesktopStyleApplicationLifetime desktopStyleApplication)
                {
                    if (!int.TryParse(IngredientPriority, out var ingredietPriority) || FilePath is null || IngredientName.Length < 1)
                    {
                        Debug.WriteLine("FilePath is Null");
                        return;
                    }

                    var fileKey = await Lib.Common.UploadFileAsync(FilePath, IngredientName, variant:"ingredients");
                    fileKey = "https://arumdream.s3.ap-northeast-2.amazonaws.com/" + fileKey;
                    Ingredient.Add(fileKey, IngredientName, ingredietPriority);

                    var dialog = DialogHelper.CreateAlertDialog(new AlertDialogBuilderParams() {
                        Borderless = true, ContentHeader = "성공",
                        SupportingText = "재료를 저장했습니다", StartupLocation = WindowStartupLocation.CenterOwner,
                    });

                    await dialog.ShowDialog(desktopStyleApplication.Windows[1]);
                    desktopStyleApplication.Windows[1].Close();
                }
            });
        }
    }
}
