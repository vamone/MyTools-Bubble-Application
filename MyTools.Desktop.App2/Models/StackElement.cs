using MyTools.Desktop.App.Services;
using MyTools.Desktop.App.Utilities;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MyTools.Desktop.App.Models
{
    public static class StackElement
    {
        public static UIElement BuildUIElement<T>(string textValue, IStackConfig<T> config) where T : IStackElement
        {
            var color = config.BackgroundOpacity >= 0.5 ? Brushes.Gray : Brushes.Black;

            var textBlockMessage = new TextBlock
            {
                Text = textValue,
                Name = $"clipboardText",
                FontStyle = FontStyles.Normal,
                Margin = new Thickness { Left = 5 },
                Foreground = config.ForegroundColor,
                HorizontalAlignment = config.PositionOnScreen == PositionOnScreen.Right ? HorizontalAlignment.Left : HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center,
                TextWrapping = TextWrapping.NoWrap,
                FontSize = 15,
            };

            var button = new Button
            {
                Background = Brushes.Transparent,
                Opacity = config.BackgroundOpacity,
                BorderThickness = new Thickness(0),
                Name = $"copyButton",
                Content = textBlockMessage,
                Margin = config.PositionOnScreen == PositionOnScreen.Right ? new Thickness { Left = 10, Right = 20 } : new Thickness { Left = 20, Right = 10 },
                Cursor = Cursors.Hand,
                Style = (Style)config.FuncFindResource("defaultButtonTempalate"),
            };

            if (config.ClickAction != null)
            {
                //button.Click += (s, e) => config.ClickAction(s, e);,
                button.Click += new RoutedEventHandler(config.ClickAction);
            }

            var border = new Border
            {
                Background = config.BackgroundColor,
                Opacity = config.BackgroundOpacity,
                Margin = new Thickness { Left = 0, Top = 5, Right = 0, Bottom = 5 },
                BorderThickness = config.PositionOnScreen == PositionOnScreen.Right ? new Thickness { Left = 2 } : new Thickness { Right = 2 },
                BorderBrush = config.BorderBrush.Invoke(),
                Uid = Guid.NewGuid().ToString(),
                Child = button,
                CornerRadius = config.PositionOnScreen == PositionOnScreen.Right ? new CornerRadius(2, 0, 0, 2) : new CornerRadius(0, 2, 2, 0)
            };

            if (config.ClipboardLeftMargin > 0)
            {
                if (config.IsStackOpen)
                {
                    border.Margin = new Thickness(0, border.Margin.Top, border.Margin.Right, border.Margin.Bottom);
                }
                else
                {
                    border.Margin = new Thickness(config.ClipboardLeftMargin, border.Margin.Top, border.Margin.Right, border.Margin.Bottom);

                    border.MouseEnter += (s, e) =>
                    {
                        button.Margin = new Thickness(button.Margin.Left, button.Margin.Top, button.Margin.Right - 10, button.Margin.Bottom);
                        border.Margin = new Thickness(0, border.Margin.Top, border.Margin.Right, border.Margin.Bottom);
                    };

                    border.MouseLeave += (s, e) =>
                    {
                        button.Margin = new Thickness(button.Margin.Left, button.Margin.Top, button.Margin.Right + 10, button.Margin.Bottom);
                        border.Margin = new Thickness(config.ClipboardLeftMargin, border.Margin.Top, border.Margin.Right, border.Margin.Bottom);
                    };
                }
            }

            return border;
        }
    }

    //public class ElementButton : Button
    //{
    //    static ElementButton()
    //    {
    //        DefaultStyleKeyProperty.OverrideMetadata(typeof(ElementButton), new FrameworkPropertyMetadata(typeof(ElementButton)));

    //    }

    //    public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ElementButton));

    //    public event RoutedEventHandler Click
    //    {
    //        add { AddHandler(ClickEvent, value); }
    //        remove { RemoveHandler(ClickEvent, value); }
    //    }

    //    protected override void OnClick()
    //    {
    //        RoutedEventArgs args = new RoutedEventArgs(ClickEvent, this);
    //        RaiseEvent(args);

    //    }

    //    protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
    //    {
    //        base.OnMouseLeftButtonUp(e);

    //        OnClick();
    //    }
    //}

}