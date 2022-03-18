using MyTools.Desktop.App.Services;
using System;
using System.Windows;
using System.Windows.Media;

namespace MyTools.Desktop.App.Models
{
    public class StackConfig<T> : IStackConfig<T> where T : IStackElement
    {
        public StackConfig()
        {
            this.BackgroundOpacity = 1;
            this.PositionOnScreen = PositionOnScreen.Right;
        }

        public double ClipboardLeftMargin { get; set; }

        public SolidColorBrush ForegroundColor { get; set; }

        public SolidColorBrush BackgroundColor { get; set; }

        public bool IsStackOpen { get; set; }

        public Action<object, RoutedEventArgs> ClickAction { get; set; }

        public double BackgroundOpacity { get; set; }

        public Func<Brush> BorderBrush { get; set; }

        public Func<string, object> FuncFindResource { get; set; }

        public PositionOnScreen PositionOnScreen { get; set; }
    }

    public enum PositionOnScreen
    {
        Left = 1,
        Right = 2
    }
}
