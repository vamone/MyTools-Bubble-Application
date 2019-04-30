using System;
using System.Windows;
using System.Windows.Controls;

using MyTools.Desktop.App.Helpers;
using MyTools.Desktop.App.Utilities;

namespace MyTools.Desktop.App
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            double opacity = SliderHelper.GetBackgroundOpacity();
            double innerMargin = SliderHelper.GetInnerMargin();

            this.OpacitySlider.Value = opacity;
            this.InnerMargin.Value = innerMargin;

            string text = FileHelper.GetAllText();

            this.ClipBoardsEditor.Text = text;
        }

        private void SaveSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            FileHelper.WriteAllText(this.ClipBoardsEditor.Text);

            SliderHelper.SaveBackgroundOpacity(this.OpacitySlider.Value);
            SliderHelper.SaveInnerMargin(this.InnerMargin.Value);

            var window = WindowHelper.GetWindowByClassName<MainWindow>();
            window.OnLoad();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var window = new ReminderWindow();
            window.Show();

            string reminderMessage = "17:30 - It works in here.";

            window.SetReminderText(reminderMessage);
        }

        private void OpenAccountWindowButton_Click(object sender, RoutedEventArgs e)
        {
            bool isWindowOpened = WindowHelper.IsWindowOpened<AccountWindow>();
            if(!isWindowOpened)
            {
                var window = new AccountWindow();
                window.Show();
            }
        }

        private void SaveAndCloseSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            FileHelper.WriteAllText(this.ClipBoardsEditor.Text);

            SliderHelper.SaveBackgroundOpacity(this.OpacitySlider.Value);
            SliderHelper.SaveInnerMargin(this.InnerMargin.Value);

            var window = WindowHelper.GetWindowByClassName<MainWindow>();
            window.OnLoad();

            this.Close();
        }
    }
}