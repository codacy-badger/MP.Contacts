﻿using MahApps.Metro.Controls.Dialogs;
using MaterialDesignThemes.Wpf;
using MP.Contacts.Models;
using MP.Contacts.Support;
using MP.Contacts.Utils;
using MP.Contacts.Views;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Position;

namespace MP.Contacts.ViewModels
{
    public class MainViewModel : BindableBase, IDisposable
    {
        private readonly IDialogCoordinator _dlgCoord;
        private readonly Dispatcher _dispatcher;
        private readonly DialogSettings _dlgSet;

        public ICommand RefreshCmd { get; }
        public ICommand TestCmd { get; }
        public ICommand CloseCmd { get; }
        public ICommand AboutFlyoutCmd { get; }
        public ICommand SettingsFlyoutCmd { get; }

        #region Singleton

        private static MainViewModel instance = null;
        private static readonly object lockThis = new object();

        public MainViewModel()
        {
            _dlgCoord = DialogCoordinator.Instance;
            _dispatcher = Application.Current.Dispatcher;
            _dlgSet = DialogSettings.Instance;

            CloseCmd = new DelegateCommand(CloseApp);
            TestCmd = new RelayCommandAsync(TestAsync);
            AboutFlyoutCmd = new RelayCommand(ShowFlyoutAbout);
            SettingsFlyoutCmd = new RelayCommand(ShowFlyoutSettings);

            var home = new MenuItem
            {
                Name = LocalizationProvider.GetLocalizedValue<string>("Home"),
                Text = LocalizationProvider.GetLocalizedValue<string>("Home"),
                ToolTip = LocalizationProvider.GetLocalizedValue<string>("Home"),
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

            var contacts = new MenuItem
            {
                //Name = LocalizeDictionary.Instance.GetLocalizedObject("Contacts", null, LocalizeDictionary.Instance.Culture).ToString(),
                Name = LocalizationProvider.GetLocalizedValue<string>("Contacts"),
                Text = LocalizationProvider.GetLocalizedValue<string>("Contacts"),
                ToolTip = LocalizationProvider.GetLocalizedValue<string>("Contacts"),
                Icon = new PackIcon
                {
                    Kind = PackIconKind.Contacts,
                    Width = 32,
                    Height = 32,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                },
                Content = new PersonsView()
            };

            MenuItems = new[]
            {
                home,
                contacts
            };

            Notifier = new Notifier(toast =>
            {
                toast.PositionProvider = new WindowPositionProvider(
                    parentWindow: Application.Current.MainWindow,
                    corner: Corner.BottomRight,
                    offsetX: 25,
                    offsetY: 25);

                toast.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                    notificationLifetime: TimeSpan.FromSeconds(5),
                    maximumNotificationCount: MaximumNotificationCount.FromCount(3));

                toast.DisplayOptions.TopMost = false;
                toast.DisplayOptions.Width = 250;
            });

            Notifier.ClearMessages();
        }

        public static MainViewModel Instance
        {
            get
            {
                lock (lockThis)
                {
                    if (instance == null)
                    {
                        instance = new MainViewModel();
                    }
                    return instance;
                }
            }
        }

        #endregion Singleton

        #region Props

        private SnackbarMessageQueue _messageQueue = new SnackbarMessageQueue(TimeSpan.FromMilliseconds(2000));
        private bool _flyoutAbout;
        private bool _flyoutSettings;

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

        public MenuItem[] MenuItems { get; set; }

        public SnackbarMessageQueue MessageQueue
        {
            get => _messageQueue;
            set { _messageQueue = value; RaisePropertyChanged(nameof(MessageQueue)); }
        }

        public Notifier Notifier { get; }

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
                MessageDialogStyle.Affirmative, _dlgSet.DlgDefaultSets).ConfigureAwait(false);
        }

        private void CloseApp()
        {
            Environment.Exit(0);
        }

        public void Dispose()
        {
            _messageQueue.Dispose();
        }
    }
}