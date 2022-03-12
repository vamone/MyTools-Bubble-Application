using System;
using System.Windows;
using System.Windows.Media;

namespace MyTools.Desktop.App.Models
{
    public interface IStackConfig
    {
        double ClipboardLeftMargin { get; set; }

        SolidColorBrush ForegroundColor { get; set; }

        SolidColorBrush BackgroundColor { get; set; }

        bool IsStackOpen { get; set; }

        Action<object, RoutedEventArgs> Action { get; set; }

        double BackgroundOpacity { get; }

        Brush BorderBrush { get; set; }
    }
}
