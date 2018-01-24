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
            string text = FileHelper.GetAllText();

            this.ClipBoardsEditor.Text = text;
        }

        private void SaveSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            FileHelper.WriteAllText(this.ClipBoardsEditor.Text);

            double opacity = this.OpacitySlider.Value;

            RegistryUtility.Save("OpacitySlider", opacity.ToString());

            var window = WindowHelper.GetWindowByClassName<MainWindow>();

            window.OnLoad();

            this.Close();
        }

        private void OpacitySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var slider = sender as Slider;
            double value = slider.Value / 10;

            string stringValue = value.ToString("0.0");

           // this.OpacityValueLabel.Content = stringValue;
        }
    }
}