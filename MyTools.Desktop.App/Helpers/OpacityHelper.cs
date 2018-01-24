using MyTools.Desktop.App.Utilities;

namespace MyTools.Desktop.App.Helpers
{
    public static class OpacityHelper
    {
        private const string _registryBackgroundOpacityKey = "OpacitySlider";

        private const double _defaultBackgroundOpacityValue = 0.65;

        public static double GetBackgroundOpacity()
        {
            double opacity = 0;

            double.TryParse(RegistryUtility.Read(_registryBackgroundOpacityKey), out opacity);
            if (opacity <= 0)
            {
                opacity = _defaultBackgroundOpacityValue;
            }

            return opacity;
        }

        public static void SaveBackgroundOpacity(double opacity)
        {
            RegistryUtility.Save(_registryBackgroundOpacityKey, opacity.ToString());
        }
    }
}