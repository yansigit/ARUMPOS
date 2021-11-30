using ARUMPOS.Lib;
using ArumModels.Models;
using ARUMPOS.Views;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using Material.Dialog;
using Material.Dialog.Enums;
using Material.Dialog.Icons;
using Microsoft.Win32.SafeHandles;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Timers.Timer;

namespace ARUMPOS.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public static MainWindowViewModel Instance { get; set; } = new();
        
        public ReactiveCommand<Unit, Task> OpenDialog { get; set; }

        public async void OpenMessageBox()
        {
            var dialog = DialogHelper.CreateAlertDialog(new AlertDialogBuilderParams()
            {
                ContentHeader = "Welcome to use Material.Avalonia",
                SupportingText = "Enjoy Material Design in AvaloniaUI!",
                StartupLocation = WindowStartupLocation.CenterOwner,
                DialogHeaderIcon = DialogIconKind.Info,
                Borderless = true
            });
            if (Application.Current.ApplicationLifetime is ClassicDesktopStyleApplicationLifetime desktopStyleApplication)
            {
                var result = await dialog.ShowDialog(desktopStyleApplication.MainWindow);
            }
        }

        public async void TextMessageBox()
        {
            if (App.Current.ApplicationLifetime is ClassicDesktopStyleApplicationLifetime desktopStyleApplication)
            {
                var result = await DialogHelper.CreateTextFieldDialog(new TextFieldDialogBuilderParams() {
                    ContentHeader = "옵션 추가",
                    SupportingText = "옵션 정보를 입력해주세요",
                    StartupLocation = WindowStartupLocation.CenterOwner,
                    DialogHeaderIcon = DialogIconKind.Info,
                    Borderless = true,
                    Width = 400,
                    TextFields = new TextFieldBuilderParams[] {
                        new TextFieldBuilderParams {
                            HelperText = "* Required", Classes = "Outline", Label = "이름", MaxCountChars = 24
                        },
                        new TextFieldBuilderParams {
                            HelperText = "* Required", Classes = "Outline", Label = "가격", MaxCountChars = 24
                        },
                        new TextFieldBuilderParams {
                            HelperText = "* Required",
                            Classes = "Outline",
                            Label = "옵션 표시 순서",
                            MaxCountChars = 2,
                        }
                    },
                    PositiveButton = new DialogResultButton {Content = "LOGIN", Result = "login"},
                }).ShowDialog(desktopStyleApplication.MainWindow);
            }
        }

        public MainWindowViewModel()
        {
            OpenDialog = ReactiveCommand.Create<Task>(async () =>
            {
                var window = new MessagePopupDialogWindow();
                if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
                {
                    await window.ShowDialog(desktopLifetime.MainWindow);
                }
            });

            Instance = this;
        }

        
    }
}
