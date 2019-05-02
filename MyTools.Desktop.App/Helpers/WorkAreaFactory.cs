using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MyTools.Desktop.App.Helpers
{
    public static class WorkAreaFactory
    {
        private const int _defaultFactoryWidth = 200;

        public static Border Build(string text, double backgroundOpacity, Action<object, RoutedEventArgs> copyClick, bool isReminder = false, double clipboardLeftMargin = 0, Func<object> funcFindResource = null)
        {
            var color = backgroundOpacity >= 0.5 ? Brushes.Gray : Brushes.Black;

            var textBlockMessage = new TextBlock
            {
                Text = text,
                Name = $"clipboardText",
                FontStyle = FontStyles.Normal,
                Margin = new Thickness { Left = 5 },
                Foreground = color,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                TextWrapping = TextWrapping.NoWrap,
                FontSize = 15,
            };

            var button = new Button
            {
                Background = Brushes.Transparent,
                //Opacity = backgroundOpacity,
                BorderThickness = new Thickness(0),
                Name = $"copyButton",
                Content = textBlockMessage,
                Margin = new Thickness { Left = 10, Right = 20 },
                Cursor = Cursors.Hand,
                Style = (Style)funcFindResource.Invoke()
            };

            button.Click += new RoutedEventHandler(copyClick);

            var border = new Border
            {
                Background = Brushes.Black,
                Opacity = backgroundOpacity,
                Margin = new Thickness { Left = 0, Top = 5, Right = 0, Bottom = 5 },
                BorderThickness = new Thickness(0),
                Uid = Guid.NewGuid().ToString(),
                Child = button,
                CornerRadius = new CornerRadius(2)
            };

            if (clipboardLeftMargin > 0)
            {
                border.Margin = new Thickness(clipboardLeftMargin, border.Margin.Top, border.Margin.Right, border.Margin.Bottom);

                border.MouseEnter += (s, e) =>
                {
                    button.Margin = new Thickness(button.Margin.Left, button.Margin.Top, button.Margin.Right - 10, button.Margin.Bottom);
                    border.Margin = new Thickness(0, border.Margin.Top, border.Margin.Right, border.Margin.Bottom);
                };

                border.MouseLeave += (s, e) =>
                {
                    button.Margin = new Thickness(button.Margin.Left, button.Margin.Top, button.Margin.Right + 10, button.Margin.Bottom);
                    border.Margin = new Thickness(clipboardLeftMargin, border.Margin.Top, border.Margin.Right, border.Margin.Bottom);
                };
            }

            return border;
        }
    }
}