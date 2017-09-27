using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MyTools.Desktop.App.Helpers
{
    public static class ElementHelper
    {
        public static Button ButtonWithIcon(string uriString, int iconWidth = 25, int iconHeight = 25, int margin = 0)
        {
            var image = new Image()
            {
                Source = new BitmapImage(new Uri(uriString, UriKind.Relative)),
                Width = iconWidth,
                Height = iconHeight,
            };

            var stackPanel = new StackPanel()
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(margin)
            };

            stackPanel.Children.Add(image);

            var button = new Button()
            {
                Content = stackPanel,
                Background = new SolidColorBrush(Colors.Black),
            };

            return button;
        }
    }
}