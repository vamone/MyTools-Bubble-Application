using MyTools.Desktop.App.Utilities;

namespace MyTools.Desktop.App.Helpers
{
    public static class SliderHelper
    {
        private const string _registryBackgroundOpacityKey = "_OpacitySlider";

        private const string _registryInnerMarginKey = "_InnerMargin";

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
            Set(_registryBackgroundOpacityKey, opacity);
        }

        public static double GetInnerMargin()
        {
            double margin = 0;

            double.TryParse(RegistryUtility.Read(_registryInnerMarginKey), out margin);

            return margin;
        }

        public static void SaveInnerMargin(double margin)
        {
            Set(_registryInnerMarginKey, margin);
        }

        private static void Set(string key, double value)
        {
            RegistryUtility.Save(key, value.ToString());
        }

    }
}