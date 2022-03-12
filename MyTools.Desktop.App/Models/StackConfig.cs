using System;
using System.Windows;
using System.Windows.Media;

namespace MyTools.Desktop.App.Models
{
    public class StackConfig : IStackConfig
    {
        public StackConfig()
        {
            this.BackgroundOpacity = 1;
        }

        public double ClipboardLeftMargin { get; set; }

        public SolidColorBrush ForegroundColor { get; set; }

        public SolidColorBrush BackgroundColor { get; set; }

        public bool IsStackOpen { get; set; }

        public Action<object, RoutedEventArgs> Action { get; set; }

        public double BackgroundOpacity { get; set; }

        public Brush BorderBrush { get; set; }

    }
}
