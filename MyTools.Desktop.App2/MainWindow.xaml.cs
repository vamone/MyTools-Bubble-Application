using MyTools.Desktop.App.Helpers;
using MyTools.Desktop.App.Models;
using MyTools.Desktop.App.Services;
using MyTools.Desktop.App.Utilities;
using MyTools.Desktop.App2.Managers;
using MyTools.Desktop.App2.Services;
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

namespace MyTools.Desktop.App;

public partial class MainWindow : Window
{
    readonly DispatcherTimer _topMostResolveTimer;

    readonly DispatcherTimer _reminderTimer;

    //readonly DispatcherTimer _updatesCheckTimer;

    readonly DispatcherTimer _focusTimer;

    //Settings _settings;

    DateTime _nextCheckUpdatesAt;

    IFocusElement _focusElement;

    //readonly IFileReaderService<ICollection<string>> _dataService;

    readonly IFileReaderService<Settings> _settingsService;

    readonly WorkAreaManager _workAreaManager;

    readonly ClipboardsManager _clipboardsManager;

    public MainWindow(
        IFileReaderService<Settings> settingsService, 
        WorkAreaManager workAreaManager, 
        ClipboardsManager clipboardsManager)
    {
        this._settingsService = settingsService;
        this._workAreaManager = workAreaManager;
        this._clipboardsManager = clipboardsManager;

        this.InitializeComponent();

        this.GridMain.MouseDown += OnMouseDown;

        this._topMostResolveTimer = new DispatcherTimer();
        this._topMostResolveTimer.Tick += TopMostResoveEventProcessor;
        this._topMostResolveTimer.Interval = new TimeSpan(0, 0, 1);

        this._reminderTimer = new DispatcherTimer();
        this._reminderTimer.Tick += ReminderTimerEventProcessor;
        this._reminderTimer.Interval = new TimeSpan(0, 0, 10);

        //this._updatesCheckTimer = new DispatcherTimer();
        //this._updatesCheckTimer.Tick += UpdatesCheckTimerEventProcessor;
        //this._updatesCheckTimer.Interval = new TimeSpan(0, 0, 5);
        //this._updatesCheckTimer.IsEnabled = false;

        this._focusTimer = new DispatcherTimer();
        this._focusTimer.Tick += FocusEventProcessor;
        this._focusTimer.Interval = new TimeSpan(0, 0, 1);
    }

    public void OnLoadClipboards()
    {
        this.WorkArea.Children.Clear();

        var clipboards = this._clipboardsManager.GetClipboards();
        foreach (var clipboard in clipboards)
        {
            var element = this._workAreaManager.BuildClipboardElement(clipboard, this.CopyClick);
            this.WorkArea.Children.Add(element);
        }
    }

    public void OnLoadFocus()
    {
        this.FocusArea.Children.Clear();

        if(this._focusElement == null)
        {
            return;
        }

        bool isTimerWaiting = this._focusElement.LastFocusAt == DateTime.MinValue;
        if (isTimerWaiting)
        {
            this.FocusArea.Children.Add(this._focusElement.BuildUIElement());
            return;
        }

        bool hasTimerFinished = this._focusElement.LastFocusAt != DateTime.MinValue && this._focusElement.LastFocusAt < DateTime.UtcNow;
        if (hasTimerFinished)
        {
            this._focusTimer.Stop();
            this.FocusArea.Children.Add(this._focusElement.BuildUIElementEnd());           
            return;
        }

        var focusTimeSpan = this._focusElement.LastFocusAt.Subtract(DateTime.UtcNow);

        string timelapsOutput = focusTimeSpan.Minutes < 1 ? focusTimeSpan.Seconds.ToString("00") : $"{focusTimeSpan.Minutes:00}:{focusTimeSpan.Seconds:00}";

        var element = this._focusElement.KeepItOpen().BuildUIElement(timelapsOutput);
        this.FocusArea.Children.Add(element);
    }

    public void OnLoadReminders()
    {
        this.ReminderArea.Children.Clear();

        var reminderElements = this._clipboardsManager.GetReminders();
        foreach (var reminder in reminderElements)
        {
            var timeSpanNow = DateTime.UtcNow.TimeOfDay;

            if (reminder.TimeSpan.Hours == timeSpanNow.Hours && reminder.TimeSpan.Minutes == timeSpanNow.Minutes)
            {
                var element = this._workAreaManager.BuildClipboardElement(reminder.Text, this.CopyClick, isReminder: true);

                this.WorkArea.Children.Add(element);

                //this._showedReminders.Add(reminder);

                this.ModifyElementThread<Border>(element, 60000, (a) => this.WorkArea.Children.Remove(a)).Start();
            }
        }
    }

    private void UpdatesCheckTimerEventProcessor(object sender, EventArgs e)
    {
        if (this._nextCheckUpdatesAt >= DateTime.Now)
        {
            return;
        }

        //bool hasUpdates = UpdateHelper.HasUpdates(Assembly.GetExecutingAssembly(), null);
        //if (hasUpdates)
        //{
        //    var updateAvailableResults = UpdateAppMessageBoxWrapper.Show("New version available. Download and update?", "MyTools Updates", MessageBoxButton.YesNoCancel);
        //    if (updateAvailableResults == MessageBoxResult.Cancel || updateAvailableResults == MessageBoxResult.No ||
        //          updateAvailableResults == MessageBoxResult.None)
        //    {
        //        this._nextCheckUpdatesAt = DateTime.Now.AddHours(24);
        //        return;
        //    }

        //    if (updateAvailableResults == MessageBoxResult.Yes)
        //    {
        //        Process.Start("MyTools.Desktop.Updater.exe");
        //        Process.GetCurrentProcess().Kill();
        //    }
        //}
    }

    private void FocusEventProcessor(object sender, EventArgs e)
    {
        this.OnLoadFocus();
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
        this.OnLoadReminders();
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

    public void CopyClick(object sender, RoutedEventArgs e)
    {
        var button = sender as Button;
        var border = button.Parent as Border;
        var textBlock = button.Content as TextBlock;

        border.Background = Brushes.Green;
        textBlock.Foreground = Brushes.Black;

        Clipboard.SetText(textBlock.Text);

        this.ModifyElementThread<TextBlock>(textBlock, 3000, (a) => this.ChangeClipboardTextColour(a, this._settingsService.Get().WindowOpacity)).Start();
        this.ModifyElementThread<Border>(border, 3000, (a) => this.ChangeClipboardBorderColor(a)).Start();
    }

    public void StartFocusTimer(object sender, RoutedEventArgs e)
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
        this.Top = this._settingsService.Get().PositionTop;
        this.Left = this._settingsService.Get().PositionLeft;
        this.Height = SystemParameters.PrimaryScreenHeight;

        //this.WorkArea.HorizontalAlignment = HorizontalAlignment.Left;
        //this.FocusArea.HorizontalAlignment = HorizontalAlignment.Left;

        if (!this._topMostResolveTimer.IsEnabled)
        {
            this._topMostResolveTimer.Start();
        }

        if (!this._reminderTimer.IsEnabled)
        {
            this._reminderTimer.Start();
        }

        //if (!this._updatesCheckTimer.IsEnabled)
        //{
        //    this._updatesCheckTimer.Start();
        //}

        this._focusElement = this._clipboardsManager.GetFocus();

        this.OnLoadClipboards();
        this.OnLoadFocus();
        this.OnLoadReminders();
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