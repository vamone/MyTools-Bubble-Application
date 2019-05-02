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

        public static Border Build(string text, double backgroundOpacity, Action<object, RoutedEventArgs> copyClick, bool isReminder = false, double clipboardLeftMargin = 0)
        {
            var grid = new Grid
            {
                Cursor = Cursors.Hand,
                Width = _defaultFactoryWidth,
                Margin = new Thickness(0)
            };

            //var stackPanelHorisontal = new StackPanel
            //{
            //    Orientation = Orientation.Horizontal,
            //    Width = 190
            //};

            var textBlockMessage = new TextBlock
            {
                Text = text,
                Name = $"textInput",
                FontStyle = FontStyles.Normal,
                Margin = new Thickness { Left = 5, Top = 0, Right = 0, Bottom = 0 },
                Foreground = new SolidColorBrush(Colors.Gray),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                TextWrapping = TextWrapping.NoWrap,
                FontSize = 15,
            };

            var button = new Button();//ElementHelper.ButtonWithIcon(ImagesContants.CopyIcon);
            button.Background = new SolidColorBrush(Colors.Black);
            button.BorderThickness = new Thickness(0);
            button.Name = "Copy";
            button.Content = textBlockMessage;
            button.Click += new RoutedEventHandler(copyClick);
            button.Margin = new Thickness { Right = _defaultFactoryWidth - clipboardLeftMargin };
            button.Opacity = backgroundOpacity;
          
            //stackPanelHorisontal.Children.Add(button);
            //stackPanelHorisontal.Children.Add(textBlockMessage);

            grid.Children.Add(button);

            var border = new Border
            {
                Background = new SolidColorBrush(Colors.Black),
                Opacity = backgroundOpacity,
                //CornerRadius = new CornerRadius(5),
                Margin = new Thickness { Left = 0, Top = 5, Right = 0, Bottom = 5 },
                Uid = "",
                Child = grid,
            };

            if (clipboardLeftMargin > 0)
            {
                border.Margin = new Thickness(clipboardLeftMargin, border.Margin.Top, border.Margin.Right, border.Margin.Bottom);

                border.MouseEnter += (sender, eventArgs) =>
                {
                    border.Margin = new Thickness(0, border.Margin.Top, border.Margin.Right, border.Margin.Bottom);
                };

                border.MouseLeave += (sender, eventArgs) =>
                {
                    border.Margin = new Thickness(clipboardLeftMargin, border.Margin.Top, border.Margin.Right, border.Margin.Bottom);
                };

                //border.CornerRadius = new CornerRadius { TopLeft = 5, BottomLeft = 5 };
            }

            return border;
        }
    }
}