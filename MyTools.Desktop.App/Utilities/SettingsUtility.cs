using MyTools.Desktop.App.Helpers;
using MyTools.Desktop.App.Models;
using Newtonsoft.Json;
using System;
using System.IO;

namespace MyTools.Desktop.App.Utilities
{
    public static class SettingsUtility
    {
        private const string _settingsFileName = "settings.json";

        static SettingsUtility()
        {
            FileHelper.CreateIfNotExists(_settingsFileName);
        }

        public static Settings Get()
        {
            string json = File.ReadAllText(_settingsFileName);
            if (string.IsNullOrWhiteSpace(json))
            {
                return new Settings();
            }

            var settings = JsonConvert.DeserializeObject<Settings>(json);
            if (settings == null)
            {
                return new Settings();
            }

            return settings;
        }

        public static void Set(double windowOpacity, double clipboardLeftMargin, double positionTop, double positionLeft)
        {
            if (windowOpacity < 0 || windowOpacity > 1)
            {
                throw new ArgumentOutOfRangeException(nameof(windowOpacity));
            }

            var settings = new Settings(windowOpacity, clipboardLeftMargin)
            {
                PositionTop = positionTop,
                PositionLeft = positionLeft
            };

            Set(settings);
        }

        public static void Set(Settings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            string json = JsonConvert.SerializeObject(settings);
            if (string.IsNullOrWhiteSpace(json))
            {
                return;
            }

            File.WriteAllText(_settingsFileName, json);
        }
    }
}
