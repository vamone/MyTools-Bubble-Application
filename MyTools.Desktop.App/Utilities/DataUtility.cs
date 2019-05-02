using MyTools.Desktop.App.Helpers;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Controls;

namespace MyTools.Desktop.App.Utilities
{
    public static class DataUtility
    {
        private const string _dataFileName = "data.json";

        private const string _defaultStartClipboard = "My first clipboard.";

        static DataUtility()
        {
            FileHelper.CreateIfNotExists(_dataFileName);
        }

        public static string GetLines()
        {
            var lines = Get();

            return string.Join("\r\n", lines);
        }

        public static ICollection<string> Get()
        {
            string json = File.ReadAllText(_dataFileName);
            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<string> { _defaultStartClipboard };
            }

            var data = JsonConvert.DeserializeObject<List<string>>(json);
            if (data == null)
            {
                return new List<string> { _defaultStartClipboard };
            }

            return data;
        }

        public static void Set(TextBox textBox)
        {
            var lines = textBox.AsLines();
            if (!lines.Any())
            {
                return;
            }

            string json = JsonConvert.SerializeObject(lines);
            if (string.IsNullOrWhiteSpace(json))
            {
                return;
            }

            File.WriteAllText(_dataFileName, json);
        }
    }
}
