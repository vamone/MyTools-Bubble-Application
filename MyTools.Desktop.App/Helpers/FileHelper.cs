using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MyTools.Desktop.App.Helpers
{
    public static class FileHelper
    {
        private const string _storageFileName = "data.txt";

        public static ICollection<string> GetLines()
        {
            var lines = File.ReadLines(_storageFileName);

            var fiteredLines = lines.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()).ToList();

            return fiteredLines;
        }

        public static string GetAllText()
        {
            var text = File.ReadAllText(_storageFileName);

            return text;
        }

        public static void WriteAllText(string text)
        {
            File.WriteAllText(_storageFileName, text);
        }
    }
}