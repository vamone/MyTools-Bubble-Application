using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

using MyTools.Desktop.App.Helpers;
using MyTools.Desktop.App.Utilities;

namespace MyTools.Desktop.App
{
    public partial class MainWindow : Window
    {
        private readonly DispatcherTimer AreaTimer;

        public MainWindow()
        {
            InitializeComponent();

            this.GridMain.MouseDown += OnMouseDown;
            this.GridMain.MouseLeave += OnMouseLeave;

            this.AreaTimer = new DispatcherTimer();
            this.AreaTimer.Tick += ClearAreaTimerEventProcessor;
            this.AreaTimer.Interval = new TimeSpan(0, 0, 1);
        }

        public void OnLoad()
        {
            this.WorkArea.Children.Clear();

            this.AreaTimer.Start();

            var list = FileHelper.GetLines();

            double opacity = 6.5;

            double.TryParse(RegistryUtility.Read("OpacitySlider"), out opacity);

            opacity = opacity / 10;

            foreach (var item in list)
            {
                var border = WorkAreaFactory.Build(item, opacity, this.CopyClick);

                this.WorkArea.Children.Add(border);
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
            if(isMinimized)
            {
                this.Topmost = false;
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
    }
}