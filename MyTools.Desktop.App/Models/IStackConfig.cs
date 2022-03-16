using MyTools.Desktop.App.Services;
using System;
using System.Windows;
using System.Windows.Media;

namespace MyTools.Desktop.App.Models
{
    public interface IStackConfig<T>
    {
        double ClipboardLeftMargin { get; set; }

        SolidColorBrush ForegroundColor { get; set; }

        SolidColorBrush BackgroundColor { get; set; }

        bool IsStackOpen { get; set; }

        Action<object, RoutedEventArgs> ClickAction { get; set; }

        double BackgroundOpacity { get; }

        Func<Brush> BorderBrush { get; set; }

        Func<string, object> FuncFindResource { get; set; }
    }
}
