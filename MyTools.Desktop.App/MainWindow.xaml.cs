using MyTools.Desktop.App.Helpers;
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

        readonly IStackService _stackService;

        Settings _settings;

        DateTime _nextCheckUpdatesAt;

        IFocusElement _focusElement;

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

            this._settings = SettingsUtility.Get();

            var copyConfig = new StackConfig<ICopyElement>
            {
                ForegroundColor = Brushes.Gray,
                BackgroundColor = Brushes.Black,
                BorderBrush = () => BrushesUtility.GetRandomBrush(),
                ClickAction = (s, e) => this.CopyClick(s, e),
                FuncFindResource = (a) => FindResource(a),
                ClipboardLeftMargin = this._settings.ClipboardLeftMargin,
                BackgroundOpacity = this._settings.WindowOpacity
            };

            var focusConfig = new StackConfig<IFocusElement>
            {
                ForegroundColor = Brushes.White,
                BackgroundColor = Brushes.Red,
                BorderBrush = () => Brushes.Red,
                ClickAction = (s, e) => this.StartFocusTimer(s, e),
                FuncFindResource = (a) => FindResource(a),
                ClipboardLeftMargin = this._settings.ClipboardLeftMargin,
                BackgroundOpacity = this._settings.WindowOpacity
            };

            var reminderConfig = new StackConfig<IReminderElement>
            {
                ForegroundColor = Brushes.White,
                BackgroundColor = Brushes.Red,
                BorderBrush = () =>  Brushes.Red,
                FuncFindResource = (a) => FindResource(a),
                ClipboardLeftMargin = this._settings.ClipboardLeftMargin,
                BackgroundOpacity = this._settings.WindowOpacity
            };

            this._stackService = new StackService(copyConfig, focusConfig, reminderConfig);
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
            this.OnLoadFocus();
        }

        public void OnLoadClipboards()
        {
            this.WorkArea.Children.Clear();

            var stackElements = this._stackService.GetCopies();
            foreach (var stack in stackElements)
            {
                this.WorkArea.Children.Add(stack.BuildUIElement());
            }
        }

        public void OnLoadFocus()
        {
            this.FocusArea.Children.Clear();

            if(this._focusElement.LastFocusAt == DateTime.MinValue)
            {
                var element = this._focusElement.BuildUIElement();
                this.FocusArea.Children.Add(element);
                return;
            }
           
            if (this._focusElement.LastFocusAt != DateTime.MinValue && this._focusElement.LastFocusAt < DateTime.UtcNow)
            {
                this._focusTimer.Stop();

                var element2 = this._focusElement.BuildUIElementEnd();
                this.FocusArea.Children.Add(element2);
                //this.ModifyElementThread<Border>(element, 60000, (a) => this.WorkArea.Children.Remove(a)).Start();
                return;
            }

            var focusTimeSpan = this._focusElement.LastFocusAt.Subtract(DateTime.UtcNow);

            string timelapsOutput = focusTimeSpan.Minutes < 1 ? focusTimeSpan.Seconds.ToString("00") : $"{focusTimeSpan.Minutes:00}:{focusTimeSpan.Seconds:00}";

            var el = this._focusElement.KeepItOpen().BuildUIElement(timelapsOutput);
            this.FocusArea.Children.Add(el);    
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
            var reminderElements = this._stackService.GetReminders();
            foreach (var reminderElement in reminderElements)
            {
                var timeSpanNow = DateTime.UtcNow.TimeOfDay;

                if (reminderElement.TimeSpan.Hours == timeSpanNow.Hours && reminderElement.TimeSpan.Minutes == timeSpanNow.Minutes)
                {
                    var element = reminderElement.KeepItOpen().BuildUIElement();
                    this.WorkArea.Children.Add(element);

                    reminderElement.MarkAsShowed();

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

        private void OpenResposiveTable(object sender, RoutedEventArgs e)
        {
        }

        private void StartFocusTimer(object sender, RoutedEventArgs e)
        {
            if (this._focusTimer.IsEnabled)
            {
                this._focusTimer.Stop();
                return;
            }

            this._focusElement.SetLastFocusAt();
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

            this._focusElement = this._stackService.GetFocus().FirstOrDefault();

            this.OnLoadClipboards();
            this.OnLoadFocus();
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