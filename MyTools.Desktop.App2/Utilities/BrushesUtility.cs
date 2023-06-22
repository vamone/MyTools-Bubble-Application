using System;
using System.Linq;
using System.Windows.Media;

namespace MyTools.Desktop.App.Utilities
{
    public static class BrushesUtility
    {
        public static Brush GetRandomBrush()
        {
            string[] brushArray = typeof(Brushes).GetProperties().
                                        Select(c => c.Name).ToArray();

            var randomColorName = brushArray.OrderBy(_ => Guid.NewGuid()).FirstOrDefault();

            SolidColorBrush color = (SolidColorBrush)new BrushConverter().ConvertFromString(randomColorName);

            return color;
        }
    }
}
