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
            double opacity = OpacityHelper.GetBackgroundOpacity();

            this.OpacitySlider.Value = opacity;

            string text = FileHelper.GetAllText();

            this.ClipBoardsEditor.Text = text;
        }

        private void SaveSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            FileHelper.WriteAllText(this.ClipBoardsEditor.Text);

            double opacity = this.OpacitySlider.Value;

            OpacityHelper.SaveBackgroundOpacity(opacity);

            var window = WindowHelper.GetWindowByClassName<MainWindow>();

            window.OnLoad();

            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var window = new ReminderWindow();
            window.Show();

            string reminderMessage = "17:30 - It works in here.";

            window.SetReminderText(reminderMessage);
        }
    }
}