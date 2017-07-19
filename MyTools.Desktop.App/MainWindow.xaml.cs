using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MyTools.Desktop.App
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.GridMain.MouseDown += OnMouseDown;
            this.GridMain.MouseLeave += OnMouseLeave;
        }

        private void CopyClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            var stackPanel = button.Parent as StackPanel;

            var items = stackPanel.Children[1] as TextBlock;

            Clipboard.SetText(items.Text);

            this.ActionNotificationText.Content = "Copied";
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var list = FileHelper.GetLines();

            foreach (var item in list)
            {
                var border = WorkAreaFactory.Build(item, this.CopyClick);

                this.WorkArea.Children.Add(border);
            }
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }

            if (e.RightButton == MouseButtonState.Pressed)
            {

            }

            if (e.ChangedButton == MouseButton.Left)
            {
                bool isDoubleClick = e.ClickCount >= 2;
                if (isDoubleClick)
                {

                }
            }
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
        }
    }
}