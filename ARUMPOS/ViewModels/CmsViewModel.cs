using ArumModels.Models;
using ArumModels.Models;
using ARUMPOS.Views;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Material.Dialog;
using Material.Dialog.Enums;
using Material.Dialog.Icons;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using Menu = ArumModels.Models.Menu;

namespace ARUMPOS.ViewModels
{
    public class CmsViewModel : ViewModelBase
    {
        public CmsViewModel()
        {
            BuildPage();

            PickItem = ReactiveCommand.CreateFromTask(async () =>
            {
                if (App.Current.ApplicationLifetime is not ClassicDesktopStyleApplicationLifetime cda)
                {
                    return;
                }
                if (SelectedItem is null)
                {
                    await DialogHelper.CreateAlertDialog(new AlertDialogBuilderParams() {
                        Borderless = false, ContentHeader = "오류", SupportingText = "항목을 먼저 선택하세요.",
                        StartupLocation = WindowStartupLocation.CenterScreen
                    }).ShowDialog(cda.MainWindow);
                    return;
                }

                switch (this.Status)
                {
                    case >= 4:
                        this.Status = 4;
                        return;
                    case 3:
                        return;
                    case 1:
                        this.Status = 2;
                        break;
                    case 2:
                        var result = await DialogHelper.CreateAlertDialog(new AlertDialogBuilderParams() {
                            DialogButtons = new [] {
                                new DialogResultButton() {Content = "옵션", Result = "Option"},
                                new DialogResultButton() {Content = "재료", Result = "Ingredient"},
                            },
                            StartupLocation = WindowStartupLocation.CenterScreen
                        }).ShowDialog(cda.MainWindow);
                        switch (result.GetResult)
                        {
                            case "Option":
                                var optionList = ((Menu)SelectedItem.Model).OptionList;
                                if (optionList is not null && optionList.Count > 0)
                                {
                                    Status = 3;
                                    break;
                                }
                                await DialogHelper.CreateAlertDialog(new AlertDialogBuilderParams()
                                {
                                    Borderless = false,
                                    ContentHeader = "오류",
                                    SupportingText = "옵션이 없는 메뉴입니다.",
                                    StartupLocation = WindowStartupLocation.CenterScreen
                                }).ShowDialog(cda.MainWindow);
                                return;
                            case "Ingredient":
                                var ingredientList = ((Menu)SelectedItem.Model).IngredientList;
                                if (ingredientList is not null && ingredientList.Count > 0)
                                {
                                    Status = 4;
                                    break;
                                }
                                await DialogHelper.CreateAlertDialog(new AlertDialogBuilderParams()
                                {
                                    Borderless = false,
                                    ContentHeader = "오류",
                                    SupportingText = "재료가 없는 메뉴입니다.",
                                    StartupLocation = WindowStartupLocation.CenterScreen
                                }).ShowDialog(cda.MainWindow);
                                return;
                            default:
                                return;
                        }
                        break;
                }
                
                BuildPage();
                SelectedItem = null;
            });

            BackButtonCommand = ReactiveCommand.Create(() =>
            {
                Status = 1;
                BuildPage();
                SelectedItem = null;
            });

            ModifyButtonCommand = ReactiveCommand.Create(async () =>
            {
                if (Application.Current.ApplicationLifetime is not ClassicDesktopStyleApplicationLifetime desktop)
                {
                    return;
                }

                if (SelectedItem == null || Status != 2)
                {
                    ShowMessageBox(desktop, "메뉴를 먼저 선택하세요");
                    return;
                }

                var window = new MenuEditFormWindow {
                    DataContext = new MenuEditFormWindowViewModel((Menu)SelectedItem.Model)
                };
                await window.ShowDialog(desktop.MainWindow);
            });

            SoldOut = ReactiveCommand.Create(() =>
            {
                if (App.Current.ApplicationLifetime is not ClassicDesktopStyleApplicationLifetime cda)
                {
                    return;
                }

                using var db = new ArumDbContext();
                if (SelectedItem is null)
                {
                    return;
                }

                if (Status != 2)
                {
                    ShowMessageBox(cda, "메뉴만 품절처리 가능합니다");
                    return;
                }

                var menu = db.Menu.Single(e => e.Id == SelectedItem.Model.Id);
                menu.IsSoldOut = !menu.IsSoldOut;
                db.SaveChanges();

                ShowMessageBox(cda, menu.IsSoldOut ? "품절처리 하였습니다" : "품절처리 취소하였습니다");
            });

            DeleteItem = ReactiveCommand.Create(() =>
            {
                using var db = new ArumDbContext();
                if (SelectedItem is null)
                {
                    return;
                }

                switch (Status)
                {
                    case 1:
                        db.Category.Remove(((Category)SelectedItem.Model));
                        db.SaveChanges();
                        break;
                    case 2:
                        db.Menu.Remove(((Menu)SelectedItem.Model));
                        db.SaveChanges();
                        break;
                    case 3:
                        db.Option.Remove(((Option)SelectedItem.Model));
                        db.SaveChanges();
                        break;
                    case 4:
                        db.Ingredient.Remove(((Ingredient)SelectedItem.Model));
                        db.SaveChanges();
                        break;
                }
                ItemList.Remove(SelectedItem);
            });
        }

        private void BuildPage()
        {
            var newList = new AvaloniaList<CmsListBoxCardViewModel>();
            switch (Status)
            {
                case 1:
                    foreach (var category in Category.GetCategories(Program.ShopId))
                    {
                        newList.Add(new CmsListBoxCardViewModel(category, Status));
                    }
                    break;
                case 2:
                    var menus = Menu.GetMenusWithOptionsAndIngredients(((Category)SelectedItem.Model).Id).ToList();
                    menus.ForEach(e => newList.Add(new CmsListBoxCardViewModel(e, Status)));
                    break;
                case 3:
                    var options = ((Menu)SelectedItem.Model).OptionList.ToList();
                    options.ForEach(e => newList.Add(new CmsListBoxCardViewModel(e, Status)));
                    break;
                case 4:
                    var ingredients = ((Menu)SelectedItem.Model).IngredientList.ToList();
                    ingredients.ForEach(e => newList.Add(new CmsListBoxCardViewModel(e, Status)));
                    break;
            }
            if (newList.Count == 0)
            {
                Status -= 1;
                if (App.Current.ApplicationLifetime is ClassicDesktopStyleApplicationLifetime
                    cda)
                {
                    ShowMessageBox(cda, "NO ITEMS");
                }
                return;
            }
            ItemList = newList;
        }

        async public void AddButtonCommand()
        {
            if (App.Current.ApplicationLifetime is ClassicDesktopStyleApplicationLifetime
                desktopStyleApplicationLifetime)
            {
                var result = await DialogHelper.CreateAlertDialog(new AlertDialogBuilderParams() {
                    ContentHeader = "추가",
                    SupportingText = "추가할 영역을 선택하세요",
                    StartupLocation = WindowStartupLocation.CenterOwner,
                    NegativeResult = new DialogResult("cancel"),
                    Borderless = true,
                    DialogButtons = new DialogResultButton[] {
                        new DialogResultButton {Content = "카테고리", Result = "category"},
                        new DialogResultButton {Content = "메뉴", Result = "menu"},
                        new DialogResultButton {Content = "옵션", Result = "option"},
                        new DialogResultButton {Content = "재료", Result = "ingredient"},
                    },
                }).ShowDialog(desktopStyleApplicationLifetime.MainWindow);
                switch (result.GetResult)
                {
                    case "category":
                        await OpenCategoryEditDialog();
                        break;
                    case "menu":
                        await OpenMenuDialog();
                        break;
                    case "option":
                        await OpenOptionEditDialog();
                        break;
                    case "ingredient":
                        await OpenIngredientEditDialog();
                        break;
                }
            }
        }

        async private Task OpenMenuDialog()
        {
            var window = new MenuEditFormWindow();
            if (App.Current.ApplicationLifetime is ClassicDesktopStyleApplicationLifetime
                desktopStyleApplicationLifetime)
            {
                await window.ShowDialog(desktopStyleApplicationLifetime.MainWindow);
                BuildPage();
            }
        }

        async private Task OpenIngredientEditDialog()
        {
            var window = new IngredientEditFormWindow();
            if (App.Current.ApplicationLifetime is ClassicDesktopStyleApplicationLifetime
                desktopStyleApplicationLifetime)
            {
                await window.ShowDialog(desktopStyleApplicationLifetime.MainWindow);
                BuildPage();
            }
        }

        async private Task OpenOptionEditDialog()
        {
            if (App.Current.ApplicationLifetime is ClassicDesktopStyleApplicationLifetime
                cda)
            {
                var result = await DialogHelper.CreateTextFieldDialog(new TextFieldDialogBuilderParams()
                {
                    ContentHeader = "옵션 추가",
                    SupportingText = "옵션 정보를 입력하세요",
                    StartupLocation = WindowStartupLocation.CenterOwner,
                    DialogHeaderIcon = DialogIconKind.Info,
                    Borderless = true,
                    Width = 400,
                    TextFields = new TextFieldBuilderParams[] {
                            new TextFieldBuilderParams() {
                                HelperText = "* 필수",
                                Classes = "Outline",
                                Label = "옵션 이름",
                                MaxCountChars = 24
                            },
                            new TextFieldBuilderParams() {
                                HelperText = "* 필수",
                                Classes = "Outline",
                                Label = "가격",
                                MaxCountChars = 24
                            },
                            new TextFieldBuilderParams() {
                                HelperText = "* 필수",
                                Classes = "Outline",
                                Label = "우선순위",
                                MaxCountChars = 24
                            },
                        },
                    PositiveButton = new DialogResultButton()
                    {
                        Content = "추가",
                        Result = "add"
                    }
                })
                    .ShowDialog(cda.MainWindow);
                if (result.GetResult == "add")
                {
                    if (result.GetFieldsResult().Any(e => e.Text.Length < 0))
                    {
                        ShowMessageBox(cda, "THERE ARE SOME NULL VALUES");
                    }
                    else
                    {
                        var name = result.GetFieldsResult()[0].Text;
                        int price;
                        if (!int.TryParse(result.GetFieldsResult()[1].Text, out price))
                        {
                            ShowMessageBox(cda, "PRICE VALUE IS NOT INTEGER");
                            return;
                        }
                        int priority;
                        if (!int.TryParse(result.GetFieldsResult()[2].Text, out priority))
                        {
                            ShowMessageBox(cda, "PRIORITY VALUE IS NOT INTEGER");
                            return;
                        }

                        Option.Add(name, price, priority);
                        ShowMessageBox(cda, "SUCCESS");
                    }
                }
                BuildPage();
            }
        }

        async private Task OpenCategoryEditDialog()
        {
            if (App.Current.ApplicationLifetime is ClassicDesktopStyleApplicationLifetime
                cda)
            {
                var result = await DialogHelper.CreateTextFieldDialog(new TextFieldDialogBuilderParams() {
                        ContentHeader = "카테고리 추가",
                        SupportingText = "카테고리 이름을 입력하세요",
                        StartupLocation = WindowStartupLocation.CenterOwner,
                        DialogHeaderIcon = DialogIconKind.Info,
                        Borderless = true,
                        Width = 400,
                        TextFields = new TextFieldBuilderParams[] {
                            new TextFieldBuilderParams() {
                                HelperText = "* 필수",
                                Classes = "Outline",
                                Label = "카테고리 이름",
                                MaxCountChars = 24
                            }
                        },
                        PositiveButton = new DialogResultButton() {
                            Content = "추가",
                            Result = "add"
                        }
                    })
                    .ShowDialog(cda.MainWindow);
                if (result.GetResult == "add")
                {
                    if (result.GetFieldsResult()[0].Text.Length > 0)
                    {
                        Category.Add(result.GetFieldsResult()[0].Text, Program.ShopId);
                        await DialogHelper.CreateAlertDialog(new AlertDialogBuilderParams()
                        {
                            Borderless = false,
                            ContentHeader = "알림",
                            StartupLocation = WindowStartupLocation.CenterOwner,
                            SupportingText = "카테고리 추가 성공"
                        }).ShowDialog(cda.MainWindow);
                    }
                    else
                    {
                        await DialogHelper.CreateAlertDialog(new AlertDialogBuilderParams()
                        {
                            Borderless = false,
                            ContentHeader = "알림",
                            StartupLocation = WindowStartupLocation.CenterOwner,
                            SupportingText = "카테고리 이름을 입력하세요"
                        }).ShowDialog(cda.MainWindow);
                    }
                }
                BuildPage();
            }
        }

        void ShowMessageBox(ClassicDesktopStyleApplicationLifetime cda, string msg)
        {
            DialogHelper.CreateAlertDialog(new AlertDialogBuilderParams()
            {
                Borderless = false,
                ContentHeader = "오류",
                SupportingText = msg
            }).ShowDialog(cda.MainWindow);
        }


        public ReactiveCommand<Unit, Unit> PickItem { get; set; }
        public ReactiveCommand<Unit, Unit> BackButtonCommand { get; set; }
        public ReactiveCommand<Unit, Task> ModifyButtonCommand { get; set; }
        public ReactiveCommand<Unit, Unit> DeleteItem { get; set; }
        public ReactiveCommand<Unit, Unit> SoldOut { get; set; }

        public int Status { get; set; } = 1;
        private AvaloniaList<CmsListBoxCardViewModel> _itemList = new();
        public AvaloniaList<CmsListBoxCardViewModel> ItemList { get => _itemList; set => this.RaiseAndSetIfChanged(ref _itemList, value); }
        public CmsListBoxCardViewModel? SelectedItem { get; set; }
    }
}
