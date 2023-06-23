using System.Linq;
using System.Windows;

namespace MyTools.Desktop.App.Helpers;

public static class WindowHelper
{
    public static bool IsWindowOpened<T>(string windowName = null) where T : Window
    {
        bool isWindowOpened = string.IsNullOrWhiteSpace(windowName) ? Application.Current.Windows.OfType<T>().Any() : Application.Current.Windows.OfType<T>().Any(w => w.Name.Equals(windowName));

        return isWindowOpened;
    }

    public static T GetWindowByClassName<T>()
    {
        var window = Application.Current.Windows.OfType<T>().SingleOrDefault();

        return window;
    }

    //public static void WindowOpen<T>() where T : Window
    //{
    //    bool isSettingsWindowOpened = IsWindowOpened<T>();
    //    if (isSettingsWindowOpened)
    //    {
    //        var window = GetWindowByClassName<T>();

    //        window.Close();

    //        return;
    //    }

    //    (new T).Show();
    //}
}