using MyTools.Desktop.App.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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

        public MainWindow()
        {
            InitializeComponent();

            this._clipboards = new List<string>();
            this._showedReminders = new List<string>();

            this.GridMain.MouseDown += OnMouseDown;
            this.GridMain.MouseLeave += OnMouseLeave;

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

            double opacity = SliderHelper.GetBackgroundOpacity();
            double innerMargin = SliderHelper.GetInnerMargin();

            var clipboards = FileHelper.GetLines();

            this._clipboards = clipboards;

            int i = 0;
            foreach (var item in this._clipboards.Where(x => !x.StartsWith("!")).ToList())
            {
                var border = WorkAreaFactory.Build(item, opacity, this.CopyClick, innerMargin: innerMargin);
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

            double opacity = SliderHelper.GetBackgroundOpacity();

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

            var stackPanel = button.Parent as StackPanel;

            var items = stackPanel.Children[1] as TextBlock;

            Clipboard.SetText(items.Text);

            this.ActionNotificationText.Content = "COPIED";
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

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
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