using MyTools.Desktop.App.Helpers;
using MyTools.Desktop.App.Models;
using MyTools.Desktop.App.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
        private readonly DispatcherTimer AreaTimer;

        private readonly DispatcherTimer _reminderTimer;

        private readonly DispatcherTimer _updatesCheckTimer;

        private ICollection<string> _clipboards;

        private ICollection<string> _showedReminders;

        private DateTime NextCheckUpdatesAt;

        private Settings _settings;

        public MainWindow()
        {
            InitializeComponent();

            this._clipboards = new List<string>();
            this._showedReminders = new List<string>();

            this.GridMain.MouseDown += OnMouseDown;

            this.AreaTimer = new DispatcherTimer();
            this.AreaTimer.Tick += ClearAreaTimerEventProcessor;
            this.AreaTimer.Interval = new TimeSpan(0, 0, 1);

            this._reminderTimer = new DispatcherTimer();
            this._reminderTimer.Tick += ReminderTimerEventProcessor;
            this._reminderTimer.Interval = new TimeSpan(0, 0, 10);

            this._updatesCheckTimer = new DispatcherTimer();
            this._updatesCheckTimer.Tick += UpdatesCheckTimerEventProcessor;
            this._updatesCheckTimer.Interval = new TimeSpan(0, 0, 5);

            this.NextCheckUpdatesAt = DateTime.Now;
        }

        private void UpdatesCheckTimerEventProcessor(object sender, EventArgs e)
        {
            if (this.NextCheckUpdatesAt >= DateTime.Now)
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
                    this.NextCheckUpdatesAt = DateTime.Now.AddHours(24);
                    return;
                }

                if (updateAvailableResults == MessageBoxResult.Yes)
                {
                    Process.Start("MyTools.Desktop.Updater.exe");
                    Process.GetCurrentProcess().Kill();
                }
            }
        }

        public void OnLoad()
        {
            this.WorkArea.Children.Clear();
            this._showedReminders.Clear();

            if (!this.AreaTimer.IsEnabled)
            {
                this.AreaTimer.Start();
            }

            if (!this._reminderTimer.IsEnabled)
            {
                this._reminderTimer.Start();
            }

            this._settings = SettingsUtility.Get();
            this._clipboards = DataUtility.Get();

            int i = 0;
            foreach (var item in this._clipboards.Where(x => !x.StartsWith("!") && !x.StartsWith("#") && !string.IsNullOrWhiteSpace(x)).ToList())
            {
                var border = WorkAreaFactory.Build(item, this._settings.WindowOpacity, this.CopyClick, clipboardLeftMargin: this._settings.ClipboardLeftMargin);
                this.WorkArea.Children.Add(border);

                i++;
            }
        }

        private void ClearAreaTimerEventProcessor(object sender, EventArgs e)
        {
            this.ActionNotificationText.Content = string.Empty;

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
            string regexReminderPattern = "^!([0-9]{1,2}:[0-9]{2});(.*)$"; //TODO: BETTER REGEX

            var reminders = this._clipboards.Where(x => x.StartsWith("!")).ToList();
            foreach (var item in reminders)
            {
                string reminderTime = RegexHelper.GetGroupValue(item, regexReminderPattern, 1);

                if (this._showedReminders.Contains(reminderTime))
                {
                    continue;
                }

                string reminderText = RegexHelper.GetGroupValue(item, regexReminderPattern, 2);

                var timeSpanReminder = TimeSpan.Parse(reminderTime);
                var timeSpanNow = DateTime.Now.TimeOfDay;

                if (timeSpanReminder.Hours == timeSpanNow.Hours
                    && timeSpanReminder.Minutes == timeSpanNow.Minutes)
                {
                    bool isReminderWindowOpened = WindowHelper.IsWindowOpened<ReminderWindow>();
                    if (!isReminderWindowOpened)
                    {
                        var window = new ReminderWindow();
                        window.Show();

                        string reminderMessage = $"{reminderTime} - {reminderText}";

                        window.SetReminderText(reminderMessage);

                        this._showedReminders.Add(reminderTime);
                    }
                }
            }
        }

        private void CopyClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            //var stackPanel = button.Parent as StackPanel;

            var grid = button.Parent as Grid;
            var border = grid.Parent as Border;
            border.Background = new SolidColorBrush(Colors.Green);

            var textBlock = button.Content as TextBlock;
            Clipboard.SetText(textBlock.Text);

            var thread = new Thread(() =>
            {       
                Thread.Sleep(3000);
                this.Dispatcher.Invoke(DispatcherPriority.Normal, new Action<Border>(this.ChangeColor), border);
            });

            thread.Start();

            //var textBlock = button.Content as TextBlock;
        
            //Clipboard.SetText(textBlock.Text);

           //this.ActionNotificationText.Content = textBlock.Text;
        }

        private void ChangeColor(Border border)
        {
            border.Background = new SolidColorBrush(Colors.Black);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.OnLoad();

            this._updatesCheckTimer.Start();
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

        private void ButtonMinimizedWindow_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
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

        private void WorkArea_MouseEnter(object sender, MouseEventArgs e)
        {
        }

        private void WorkArea_MouseLeave(object sender, MouseEventArgs e)
        {
        }
    }
}