using MyTools.Desktop.App.Helpers;
using System;
using System.Diagnostics;
using System.Windows;

namespace MyTools.Desktop.Updater
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private UpdateInformation _updateInformation;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.ButtonDownloadAndUpdate.Visibility = Visibility.Visible;
            this.ButtonFinishAndReOpen.Visibility = Visibility.Hidden;

           this._updateInformation = UpdateHelper.GetUpdateInformation(null);
            if (this._updateInformation != null)
            {
                this.LabelUpdateVersion.Content = $"Version: {this._updateInformation.Version}";
            }
        }

        private void ButtonDownloadAndUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.ButtonDownloadAndUpdate.Visibility = Visibility.Hidden;

                this.LabelProgressBarTitle.Visibility = Visibility.Visible;
                this.ProressBarDownloadStatus.Visibility = Visibility.Visible;

                if(this._updateInformation == null)
                {
                    return;
                }

                int ratio = 100 / this._updateInformation.Files.Count;

                foreach (var file in this._updateInformation.Files)
                {
                    bool hasFiledDownloaded = UpdateHelper.HasDownloadedFile(this._updateInformation, file);
                    if (hasFiledDownloaded)
                    {
                        this.ProressBarDownloadStatus.Value = this.ProressBarDownloadStatus.Value + ratio;

                        this.LabelDownloadStatus.Visibility = Visibility.Visible;
                        this.LabelDownloadStatus.Content = this.ProressBarDownloadStatus.Value + ratio + "%";
                    }
                }

                double progressBarValue = this.ProressBarDownloadStatus.Value;
                if (progressBarValue != 100)
                {
                    this.LabelDownloadStatus.Visibility = Visibility.Visible;
                    this.LabelDownloadStatus.Content = "Update failed. Please try again";

                    this.ButtonDownloadAndUpdate.Visibility = Visibility.Visible;
                }
                else
                {
                    this.LabelDownloadStatus.Visibility = Visibility.Visible;
                    this.LabelDownloadStatus.Content = "Application successfully updated";

                    this.ButtonFinishAndReOpen.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                this.LabelDownloadStatus.Visibility = Visibility.Visible;
                this.LabelDownloadStatus.Content = "Update failed. Please try again";

                this.ButtonDownloadAndUpdate.Visibility = Visibility.Visible;
            }
        }

        private void ButtonFinishAndReOpen_Click(object sender, RoutedEventArgs e)
        {
            if (this._updateInformation != null)
            {
                Process.Start(this._updateInformation.StartFileAfterUpdate.Name);
                Process.GetCurrentProcess().Kill();
            }
        }
    }
}
