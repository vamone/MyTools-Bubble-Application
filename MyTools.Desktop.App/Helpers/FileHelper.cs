using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MyTools.Desktop.App.Helpers
{
    public static class FileHelper
    {
        public static ICollection<string> GetLines()
        {
            var lines = File.ReadLines("data.txt");

            var fiteredLines = lines.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()).ToList();

            return fiteredLines;
        }
    }
}