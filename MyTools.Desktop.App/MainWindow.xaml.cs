using MyTools.Desktop.App.Helpers;
using MyTools.Desktop.App.Managers;
using MyTools.Desktop.App.Models;
using MyTools.Desktop.App.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace MyTools.Desktop.App
{
    public partial class MainWindow : Window
    {
        private readonly DispatcherTimer _topMostResolveTimer;

        private readonly DispatcherTimer _reminderTimer;

        private readonly DispatcherTimer _updatesCheckTimer;

        private readonly ICollection<IReminder> _showedReminders;

        private readonly WorkAreaManager _workAreaManager;

        private readonly ClipboardsManager _clipboardsManager;

        private Settings _settings;

        private DateTime _nextCheckUpdatesAt;

        public MainWindow()
        {
            InitializeComponent();

            this.GridMain.MouseDown += OnMouseDown;

            this._topMostResolveTimer = new DispatcherTimer();
            this._topMostResolveTimer.Tick += TopMostResoveEventProcessor;
            this._topMostResolveTimer.Interval = new TimeSpan(0, 0, 1);

            this._reminderTimer = new DispatcherTimer();
            this._reminderTimer.Tick += ReminderTimerEventProcessor;
            this._reminderTimer.Interval = new TimeSpan(0, 0, 10);

            this._updatesCheckTimer = new DispatcherTimer();
            this._updatesCheckTimer.Tick += UpdatesCheckTimerEventProcessor;
            this._updatesCheckTimer.Interval = new TimeSpan(0, 0, 5);
            this._updatesCheckTimer.IsEnabled = false;

            this._nextCheckUpdatesAt = DateTime.Now;

            //NEW SOLUTION
            this._settings = SettingsUtility.Get();

            this._showedReminders = new List<IReminder>();

            this._workAreaManager = new WorkAreaManager(this._settings, (a) => FindResource(a));
            this._clipboardsManager = new ClipboardsManager(this._showedReminders);
        }

        private void UpdatesCheckTimerEventProcessor(object sender, EventArgs e)
        {
            if (this._nextCheckUpdatesAt >= DateTime.Now)
            {
                return;
            }

            bool hasUpdates = UpdateHelper.HasUpdates(Assembly.GetExecutingAssembly(), null);
            if (hasUpdates)
            {
                var updateAvailableResults = UpdateAppMessageBoxWrapper.Show("New version available. Download and update?", "MyTools Updates", MessageBoxButton.YesNoCancel);
                if (updateAvailableResults == MessageBoxResult.Cancel || updateAvailableResults == MessageBoxResult.No ||
                      updateAvailableResults == MessageBoxResult.None)
                {
                    this._nextCheckUpdatesAt = DateTime.Now.AddHours(24);
                    return;
                }

                if (updateAvailableResults == MessageBoxResult.Yes)
                {
                    Process.Start("MyTools.Desktop.Updater.exe");
                    Process.GetCurrentProcess().Kill();
                }
            }
        }

        public void OnLoadClipboards()
        {
            this.WorkArea.Children.Clear();
            this._showedReminders.Clear();

            foreach (var clipboard in this._clipboardsManager.GetClipboards())
            {
                var element = this._workAreaManager.BuildClipboardElement(clipboard, this.CopyClick);
                this.WorkArea.Children.Add(element);
            }
        }

        private void TopMostResoveEventProcessor(object sender, EventArgs e)
        {
            bool isActive = this.WindowState == WindowState.Normal;
            if (isActive)
            {
                this.Topmost = true;
            }

            bool isMinimized = this.WindowState == WindowState.Minimized;
            if (isMinimized)
            {
                this.Topmost = false;
            }
        }

        private void ReminderTimerEventProcessor(object sender, EventArgs e)
        {
            foreach (var reminder in this._clipboardsManager.GetReminders())
            {
                var timeSpanNow = DateTime.Now.TimeOfDay;

                if (reminder.TimeSpan.Hours == timeSpanNow.Hours && reminder.TimeSpan.Minutes == timeSpanNow.Minutes)
                {
                    var element = this._workAreaManager.BuildClipboardElement(reminder.Text, this.CopyClick, isReminder: true);

                    this.WorkArea.Children.Add(element);

                    this._showedReminders.Add(reminder);

                    this.ModifyElementThread<Border>(element, 60000, (a) => this.WorkArea.Children.Remove(a)).Start();
                }
            }
        }

        private Thread ModifyElementThread<E>(object element, int threadSleepInMimiseconds, Action<E> action, DispatcherPriority dispatcherPriority = DispatcherPriority.Normal)
        {
            return new Thread(() =>
            {
                if (threadSleepInMimiseconds > 0)
                {
                    Thread.Sleep(threadSleepInMimiseconds);
                }

                this.Dispatcher.Invoke(DispatcherPriority.Normal, action, element);
            });
        }

        private void CopyClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var border = button.Parent as Border;
            var textBlock = button.Content as TextBlock;

            border.Background = Brushes.Green;
            textBlock.Foreground = Brushes.Black;

            Clipboard.SetText(textBlock.Text);

            this.ModifyElementThread<TextBlock>(textBlock, 3000, (a) => this.ChangeClipboardTextColour(a, this._settings.WindowOpacity)).Start();
            this.ModifyElementThread<Border>(border, 3000, (a) => this.ChangeClipboardBorderColor(a)).Start();
        }

        private void ChangeClipboardBorderColor(Border border)
        {
            border.Background = Brushes.Black;
        }

        private void ChangeClipboardTextColour(TextBlock textBlock, double windowOpacity)
        {
            textBlock.Foreground = windowOpacity >= 0.5 ? Brushes.Gray : Brushes.Black;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this._settings = SettingsUtility.Get();

            this.Top = this._settings.PositionTop;
            this.Left = this._settings.PositionLeft;

            if (!this._topMostResolveTimer.IsEnabled)
            {
                this._topMostResolveTimer.Start();
            }

            if (!this._reminderTimer.IsEnabled)
            {
                this._reminderTimer.Start();
            }

            if (!this._updatesCheckTimer.IsEnabled)
            {
                this._updatesCheckTimer.Start();
            }

            this.OnLoadClipboards();
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }

            if (e.RightButton == MouseButtonState.Pressed)
            {
                bool isSettingsWindowOpened = WindowHelper.IsWindowOpened<SettingsWindow>();
                if (isSettingsWindowOpened)
                {
                    var window = WindowHelper.GetWindowByClassName<SettingsWindow>();
                    window.Close();
                    return;
                }

                var settingsWindow = new SettingsWindow();
                settingsWindow.Show();
            }

            if (e.ChangedButton == MouseButton.Left)
            {
                bool isDoubleClick = e.ClickCount >= 2;
                if (isDoubleClick)
                {
                    this.WindowState = WindowState.Minimized;
                }
            }
        }

        private void SettingButton_Click(object sender, RoutedEventArgs e)
        {
            bool isSettingsWindowOpened = WindowHelper.IsWindowOpened<SettingsWindow>();
            if (isSettingsWindowOpened)
            {
                var window = WindowHelper.GetWindowByClassName<SettingsWindow>();

                window.Close();

                return;
            }

            var settingsWindow = new SettingsWindow();

            settingsWindow.Show();
        }
    }
}