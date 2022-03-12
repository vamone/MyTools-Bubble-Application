﻿using MyTools.Desktop.App.Models;
using MyTools.Desktop.App.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace MyTools.Desktop.App.Managers
{
    public class WorkAreaManager
    {
        readonly Func<string, object> _funcFindResource;

        readonly Settings _settings;

        public WorkAreaManager(Settings settings, Func<string, object> funcFindResource)
        {
            this._settings = settings;
            this._funcFindResource = funcFindResource;
        }

        public Border BuildClipboardElement(string text, Action<object, RoutedEventArgs> clickAction, SolidColorBrush foregroundColor, SolidColorBrush backgrounddColor, bool isOpen = false)
        {
            double backgroundOpacity = this._settings.WindowOpacity;
            double clipboardLeftMargin = this._settings.ClipboardLeftMargin;

            var color = backgroundOpacity >= 0.5 ? Brushes.Gray : Brushes.Black;

            var textBlockMessage = new TextBlock
            {
                Text = text,
                Name = $"clipboardText",
                FontStyle = FontStyles.Normal,
                Margin = new Thickness { Left = 5 },
                Foreground = foregroundColor,
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
                Style = (Style)this._funcFindResource.Invoke("defaultButtonTempalate"),
            };

            button.Click += new RoutedEventHandler(clickAction);
            //button.MouseEnter += (s, e) => { ((e.Source as Button).Content as TextBlock).Foreground = Brushes.White; };
            //button.MouseLeave += (s, e) => { ((e.Source as Button).Content as TextBlock).Foreground = color; };

            var border = new Border
            {
                Background = backgrounddColor,
                Opacity = backgroundOpacity,
                Margin = new Thickness { Left = 0, Top = 5, Right = 0, Bottom = 5 },
                BorderThickness = new Thickness { Left = 2 },
                BorderBrush = BrushesUtility.GetRandomBrush(),
                Uid = Guid.NewGuid().ToString(),
                Child = button,
                CornerRadius = new CornerRadius(2)
            };

            if(!isOpen)
            {
                border.Margin = new Thickness(clipboardLeftMargin, border.Margin.Top, border.Margin.Right, border.Margin.Bottom);
            }

            if (clipboardLeftMargin > 0)
            {
                border.MouseEnter += (s, e) =>
                {
                    //button.Margin = new Thickness(button.Margin.Left, button.Margin.Top, button.Margin.Right - 10, button.Margin.Bottom);
                    border.Margin = new Thickness(0, border.Margin.Top, border.Margin.Right, border.Margin.Bottom);
                };

                border.MouseLeave += (s, e) =>
                {
                    //button.Margin = new Thickness(button.Margin.Left, button.Margin.Top, button.Margin.Right + 10, button.Margin.Bottom);
                    border.Margin = new Thickness(clipboardLeftMargin, border.Margin.Top, border.Margin.Right, border.Margin.Bottom);
                };
            }

            return border;
        }
    }
}
