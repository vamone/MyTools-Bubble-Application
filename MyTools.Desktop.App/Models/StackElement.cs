using MyTools.Desktop.App.Services;
using MyTools.Desktop.App.Utilities;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MyTools.Desktop.App.Models
{
    public class StackElement : IStackElement
    {
        readonly string _textValue;

        public StackElement(string textValue, IStackConfig config)
        {
            this._textValue = textValue;
            this.UIElement = this.Build(config);
        }

        public UIElement UIElement { get; }

        public DateTime LastFocusAt { get; set; }

        public int FocusTimeInMinutes { get; set; }

        UIElement Build(IStackConfig config)
        {
            var color = config.BackgroundOpacity >= 0.5 ? Brushes.Gray : Brushes.Black;

            var textBlockMessage = new TextBlock
            {
                Text = this._textValue,
                Name = $"clipboardText",
                FontStyle = FontStyles.Normal,
                Margin = new Thickness { Left = 5 },
                Foreground = config.ForegroundColor,
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
                //Style = (Style)this._funcFindResource.Invoke("defaultButtonTempalate"),
            };

            if (config.Action != null)
            {
                button.Click += new RoutedEventHandler(config.Action);
            }
            //button.MouseEnter += (s, e) => { ((e.Source as Button).Content as TextBlock).Foreground = Brushes.White; };
            //button.MouseLeave += (s, e) => { ((e.Source as Button).Content as TextBlock).Foreground = color; };

            var border = new Border
            {
                Background = config.BackgroundColor,
                Opacity = config.BackgroundOpacity,
                Margin = new Thickness { Left = 0, Top = 5, Right = 0, Bottom = 5 },
                BorderThickness = new Thickness { Left = 2 },
                BorderBrush = BrushesUtility.GetRandomBrush(),
                Uid = Guid.NewGuid().ToString(),
                Child = button,
                CornerRadius = new CornerRadius(2)
            };

            if (!config.IsStackOpen)
            {
                border.Margin = new Thickness(config.ClipboardLeftMargin, border.Margin.Top, border.Margin.Right, border.Margin.Bottom);
            }

            if (config.ClipboardLeftMargin > 0)
            {
                border.MouseEnter += (s, e) =>
                {
                    //button.Margin = new Thickness(button.Margin.Left, button.Margin.Top, button.Margin.Right - 10, button.Margin.Bottom);
                    border.Margin = new Thickness(0, border.Margin.Top, border.Margin.Right, border.Margin.Bottom);
                };

                border.MouseLeave += (s, e) =>
                {
                    //button.Margin = new Thickness(button.Margin.Left, button.Margin.Top, button.Margin.Right + 10, button.Margin.Bottom);
                    border.Margin = new Thickness(config.ClipboardLeftMargin, border.Margin.Top, border.Margin.Right, border.Margin.Bottom);
                };
            }

            return border;
        }
    }
}
