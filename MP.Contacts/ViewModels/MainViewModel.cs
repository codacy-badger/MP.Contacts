﻿using MahApps.Metro.Controls.Dialogs;
using MaterialDesignThemes.Wpf;
using MP.Contacts.Models;
using MP.Contacts.Support;
using MP.Contacts.Views;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace MP.Contacts.ViewModels
{
    public class MainViewModel : BindableBase
    {
        private readonly IDialogCoordinator _dlgCoord;
        private readonly Dispatcher _dispatcher;
        private readonly DialogSettings _dlgSet;
        //private readonly MsgText _msgTxt;

        public ICommand RefreshCmd { get; }
        public ICommand TestCmd { get; }
        public ICommand CloseCmd { get; }

        public ICommand AboutFlyoutCmd { get; }
        public ICommand SettingsFlyoutCmd { get; }

        public MainViewModel()
        {
            _dlgCoord = DialogCoordinator.Instance;
            _dispatcher = Application.Current.Dispatcher;
            _dlgSet = DialogSettings.Instance;
            //_msgTxt = MsgText.Instance;

            CloseCmd = new DelegateCommand(CloseApp);
            TestCmd = new RelayCommandAsync(TestAsync);
            AboutFlyoutCmd = new RelayCommand(ShowFlyoutAbout);
            SettingsFlyoutCmd = new RelayCommand(ShowFlyoutSettings);

            MenuItem home = new MenuItem()
            {
                Name = "Home",
                Text = "Home",
                ToolTip = "Home",
                Icon = new PackIcon
                {
                    Kind = PackIconKind.Home,
                    Width = 32,
                    Height = 32,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                },
                Content = new HomeView()
            };

            MenuItem contacts = new MenuItem()
            {
                Name = "Contacts",
                Text = "Contacts",
                ToolTip = "Contacts",
                Icon = new PackIcon
                {
                    Kind = PackIconKind.Contacts,
                    Width = 32,
                    Height = 32,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                },
                Content = new ContactsView()
            };

            MenuItems = new[]
            {
                home,
                contacts
            };
        }

        #region Props

        public MenuItem[] MenuItems { get; set; }

        private bool _flyoutAbout;
        private bool _flyoutSettings;
        private string _title = Settings.Default.AppName;

        public bool FlyoutAbout
        {
            get => _flyoutAbout;
            set { _flyoutAbout = value; RaisePropertyChanged(nameof(FlyoutAbout)); }
        }

        public bool FlyoutSettings
        {
            get => _flyoutSettings;
            set { _flyoutSettings = value; RaisePropertyChanged(nameof(FlyoutSettings)); }
        }

        public string Title
        {
            get => _title;
            set { _title = value; RaisePropertyChanged(nameof(Title)); }
        }

        #endregion Props

        private void ShowFlyoutAbout(object obj)
        {
            FlyoutAbout = (bool)obj;
        }

        private void ShowFlyoutSettings(object obj)
        {
            FlyoutSettings = (bool)obj;
        }

        private async Task TestAsync(object arg)
        {
            await _dlgCoord.ShowMessageAsync(this, "Info!", "Hello World!!!",
                MessageDialogStyle.Affirmative, _dlgSet.DialogDefaultSettings).ConfigureAwait(false);
        }

        private void CloseApp()
        {
            Environment.Exit(0);
        }
    }
}