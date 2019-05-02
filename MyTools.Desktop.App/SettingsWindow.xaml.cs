using MyTools.Desktop.App.Helpers;
using MyTools.Desktop.App.Utilities;
using System.Windows;

namespace MyTools.Desktop.App
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var settings = SettingsUtility.Get();

            this.OpacitySlider.Value = settings.WindowOpacity;
            this.InnerMargin.Value = settings.ClipboardLeftMargin;
            this.ClipBoardsEditor.Text = DataUtility.GetLines();
        }

        private void SaveSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            this.SaveAndReloadInternal();
        }

        private void OpenAccountWindowButton_Click(object sender, RoutedEventArgs e)
        {
            bool isWindowOpened = WindowHelper.IsWindowOpened<AccountWindow>();
            if (!isWindowOpened)
            {
                var window = new AccountWindow();
                window.Show();
            }
        }

        private void SaveAndCloseSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            this.SaveAndReloadInternal();
            this.Close();
        }

        private void SaveAndReloadInternal()
        {
            SettingsUtility.Set(this.OpacitySlider.Value, this.InnerMargin.Value);
            DataUtility.Set(this.ClipBoardsEditor);

            var window = WindowHelper.GetWindowByClassName<MainWindow>();
            window.OnLoad();
        }
    }

}