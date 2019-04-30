using System.Windows;

namespace MyTools.Desktop.App
{
    public static class UpdateAppMessageBoxWrapper
    {
        public static bool IsOpened { get; set; }

        public static MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button)
        {
            if (!IsOpened)
            {
                IsOpened = true;
                return MessageBox.Show(messageBoxText, caption, button);
            }

            return (int)MessageBoxResult.None;
        }
    }
}
