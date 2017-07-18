using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MyTools.Desktop.App
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.MainIconArea.MouseDown += OnMouseDown;
            this.MainIconArea.MouseLeave += OnMouseLeave;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

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