using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MyTools.Desktop.App.Helpers
{
    public static class WorkAreaFactory
    {
        public static Border Build(string text, Action<object, RoutedEventArgs> copyClick)
        {
            var grid = new Grid
            {
                Cursor = Cursors.Hand,
                Width = 230,
                Margin = new Thickness(5)
            };

            var stackPanelHorisontal = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Width = 220
            };

            var button = new Button();
            button.Content = "Copy";
            button.Name = "Copy";
            button.Click += new RoutedEventHandler(copyClick);

            var textBlockMessage = new TextBlock
            {
                Text = text,
                Name = $"textInput",
                FontStyle = FontStyles.Normal,
                Margin = new Thickness { Left = 5, Bottom = 5 },
                Foreground = new SolidColorBrush(Colors.WhiteSmoke),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                TextWrapping = TextWrapping.Wrap,
                FontSize = 15
            };

            stackPanelHorisontal.Children.Add(button);
            stackPanelHorisontal.Children.Add(textBlockMessage);
            
            grid.Children.Add(stackPanelHorisontal);

            var border = new Border
            {
                Background = new SolidColorBrush(Colors.Black),
                Opacity = 0.70,
                CornerRadius = new CornerRadius(10, 10, 10, 10),
                Margin = new Thickness { Left = 5, Top = 5, Right = 5, Bottom = 5 },
                Uid = "",
                Child = grid
            };

            return border;
        }
    }
}