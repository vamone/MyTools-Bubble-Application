using System.Windows;

namespace MyTools.Desktop.App
{
    public partial class ReminderWindow : Window
    {
        public ReminderWindow()
        {
            InitializeComponent();
        }

        public void SetReminderText(string text)
        {
            this.ReminderTextBlock.Text = text;
        }
    }
}
