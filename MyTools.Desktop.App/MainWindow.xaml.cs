using MyTools.Desktop.App.Helpers;
using MyTools.Desktop.App.Managers;
using MyTools.Desktop.App.Models;
using MyTools.Desktop.App.Services;
using MyTools.Desktop.App.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        readonly DispatcherTimer _topMostResolveTimer;

        readonly DispatcherTimer _reminderTimer;

        readonly DispatcherTimer _updatesCheckTimer;

        readonly DispatcherTimer _focusTimer;

        readonly ICollection<IReminder> _showedReminders;

        readonly WorkAreaManager _workAreaManager;

        readonly ClipboardsManager _clipboardsManager;

        readonly int _defaultFocusTimeInMinutes;

        Settings _settings;

        DateTime _nextCheckUpdatesAt;

        DateTime _lastFocusAt;

        Border _focusElement;

        readonly IStackService _stackService;

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

            this._focusTimer = new DispatcherTimer();
            this._focusTimer.Tick += FocusEventProcessor;
            this._focusTimer.Interval = new TimeSpan(0, 0, 1);

            var copyConfig = new StackConfig
            {
                ForegroundColor = Brushes.Gray,
                BackgroundColor = Brushes.Black
            };

            var focusConfig = new StackConfig
            {
                ForegroundColor = Brushes.White,
                BackgroundColor = Brushes.Red
            };

            var reminderConfig = new StackConfig
            {
                ForegroundColor = Brushes.White,
                BackgroundColor = Brushes.Red
            };

            this._stackService = new StackService(copyConfig, focusConfig, reminderConfig);
            
            this._defaultFocusTimeInMinutes = 25;
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

        private void FocusEventProcessor(object sender, EventArgs e)
        {
            var stackElements = this._stackService.GetFocus();
            foreach (var stack in stackElements)
            {
                if(stack.LastFocusAt < DateTime.UtcNow)
                {
                    this._focusTimer.Stop();

                    this.WorkArea.Children.Add(stack.UIElement);
                    this.ModifyElementThread<Border>(stack.UIElement, 60000, (a) => this.WorkArea.Children.Remove(a)).Start();
                    return;
                }

                //var focusTimeSpan = stack.LastFocusAt.Subtract(DateTime.UtcNow);

                //bool isOpen = true;
                //bool isOpenIn10Seconds = this._defaultFocusTimeInMinutes * 60 - focusTimeSpan.TotalSeconds <= 10;
                //if (!isOpenIn10Seconds)
                //{
                //    isOpen = false;
                //}

                //string timelapsOutput = focusTimeSpan.Minutes < 1 ? focusTimeSpan.Seconds.ToString() : $"{focusTimeSpan.Minutes}:{focusTimeSpan.Seconds}";

                //this._focusElement = this._workAreaManager.BuildClipboardElement(timelapsOutput, this.ActionStartFocusTimer, Brushes.White, Brushes.Red, isOpen: isOpen);
                //this.WorkArea.Children.Add(this._focusElement);
            }

            this.WorkArea.Children.Remove(this._focusElement);

            if (this._lastFocusAt < DateTime.UtcNow)
            {
                this._focusTimer.Stop();

                var element = this._workAreaManager.BuildClipboardElement("Focus time is over!", this.ActionStartFocusTimer, Brushes.White, Brushes.Red, isOpen: true);
                this.WorkArea.Children.Add(element);
                this.ModifyElementThread<Border>(element, 60000, (a) => this.WorkArea.Children.Remove(a)).Start();
                return;
            }

            var focusTimeSpan = this._lastFocusAt.Subtract(DateTime.UtcNow);

            bool isOpen = true;
            bool isOpenIn10Seconds = this._defaultFocusTimeInMinutes * 60 - focusTimeSpan.TotalSeconds <= 10;
            if (!isOpenIn10Seconds)
            {
                isOpen = false;
            }

            string timelapsOutput = focusTimeSpan.Minutes < 1 ? focusTimeSpan.Seconds.ToString() : $"{focusTimeSpan.Minutes}:{focusTimeSpan.Seconds}";

            this._focusElement = this._workAreaManager.BuildClipboardElement(timelapsOutput, this.ActionStartFocusTimer, Brushes.White, Brushes.Red, isOpen: isOpen);
            this.WorkArea.Children.Add(this._focusElement);
        }

        public void OnLoadClipboards()
        {
            this.WorkArea.Children.Clear();
            this._showedReminders.Clear();

            //var popupElement = this._workAreaManager.BuildClipboardElement("Resp", this.OpenResposiveTable, Brushes.OrangeRed);
            //this.WorkArea.Children.Add(popupElement);

            var stackElements = this._stackService.GetCopies();
            foreach (var stack in stackElements)
            {
                this.WorkArea.Children.Add(stack.UIElement);
            }


            //var clipboards = this._clipboardsManager.GetClipboards();
            //foreach (var clipboard in clipboards)
            //{
            //    var element = this._workAreaManager.BuildClipboardElement(clipboard, this.CopyClick, Brushes.Gray, Brushes.Black);
            //    this.WorkArea.Children.Add(element);
            //}
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
            var reminders = this._stackService.GetReminders();
            foreach (var reminder in reminders)
            {
                //var timeSpanNow = DateTime.UtcNow.TimeOfDay;

                //if (reminder.TimeSpan.Hours == timeSpanNow.Hours && reminder.TimeSpan.Minutes == timeSpanNow.Minutes)
                //{
                //    var element = this._workAreaManager.BuildClipboardElement(reminder, this.CopyClick, Brushes.White, Brushes.Red);

                //    this.WorkArea.Children.Add(element);

                //    reminder.CreatedAt = DateTime.UtcNow;

                //    this._showedReminders.Add(reminder);

                //    this.ModifyElementThread<Border>(element, 60000, (a) => this.WorkArea.Children.Remove(a)).Start();
                //}
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

        private void OpenResposiveTable(object sender, RoutedEventArgs e)
        {
        }

        private void ActionStartFocusTimer(object sender, RoutedEventArgs e)
        {
            if(this._focusTimer.IsEnabled)
            {
                this._focusTimer.Stop();
                return;
            }

            this._lastFocusAt = DateTime.UtcNow.AddMinutes(this._defaultFocusTimeInMinutes);
            this._focusTimer.Start();
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
            this.Height = SystemParameters.PrimaryScreenHeight;

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

            this._focusElement = this._workAreaManager.BuildClipboardElement("Start focus!", this.ActionStartFocusTimer, Brushes.White, Brushes.Red, isOpen: true);
            this.WorkArea.Children.Add(this._focusElement);
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