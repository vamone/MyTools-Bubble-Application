using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MyTools.Desktop.App.Helpers
{
    public static class WorkAreaFactory
    {
        public static Border Build(string text, double backgroundOpacity, Action<object, RoutedEventArgs> copyClick, bool isReminder = false, double innerMargin = 0)
        {
            var grid = new Grid
            {
                Cursor = Cursors.Hand,
                Width = 200,
                Margin = new Thickness(0)
            };

            var stackPanelHorisontal = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Width = 190
            };

            var button = ElementHelper.ButtonWithIcon(ImagesContants.CopyIcon);
            button.BorderThickness = new Thickness(0);
            button.Name = "Copy";
            button.Click += new RoutedEventHandler(copyClick);

            var textBlockMessage = new TextBlock
            {
                Text = text,
                Name = $"textInput",
                FontStyle = FontStyles.Normal,
                Margin = new Thickness { Left = 10 },
                Foreground = new SolidColorBrush(Colors.Gray),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                TextWrapping = TextWrapping.Wrap,
                FontSize = 15,
            };

            stackPanelHorisontal.Children.Add(button);
            stackPanelHorisontal.Children.Add(textBlockMessage);

            grid.Children.Add(stackPanelHorisontal);

            var border = new Border
            {
                Background = new SolidColorBrush(Colors.Black),
                Opacity = backgroundOpacity,
                CornerRadius = new CornerRadius(5),
                Margin = new Thickness { Left = 0, Top = 5, Right = 0, Bottom = 5 },
                Uid = "",
                Child = grid,
            };

            if (innerMargin > 0)
            {
                border.Margin = new Thickness(innerMargin, border.Margin.Top, border.Margin.Right, border.Margin.Bottom);

                border.MouseEnter += (sender, eventArgs) =>
                {
                    border.Margin = new Thickness(0, border.Margin.Top, border.Margin.Right, border.Margin.Bottom);
                };

                border.MouseLeave += (sender, eventArgs) =>
                {
                    border.Margin = new Thickness(innerMargin, border.Margin.Top, border.Margin.Right, border.Margin.Bottom);
                };

                border.CornerRadius = new CornerRadius { TopLeft = 5, BottomLeft = 5 };
            }

            return border;
        }
    }
}