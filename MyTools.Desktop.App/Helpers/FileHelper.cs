using System;
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
            CreateIfNotExists();
            PopulateWithFirstData();

            var lines = File.ReadLines(_storageFileName);

            var fiteredLines = lines.Where(x => !string.IsNullOrWhiteSpace(x) && !x.StartsWith("#")).Select(x => x.Trim()).ToList();

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

        private static void CreateIfNotExists()
        {
            string binPath = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = Path.Combine(binPath, _storageFileName);

            if (!File.Exists(filePath))
            {
                File.Create(filePath);
            }
        }

        private static void PopulateWithFirstData()
        {
            var lines = File.ReadLines(_storageFileName);
            if (!lines.Any())
            {
                File.WriteAllLines(_storageFileName, new List<string> { "My first clipboard." });
            }
        }
    }
}