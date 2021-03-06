﻿using System;
using System.IO;

namespace MyTools.Desktop.App.Helpers
{
    public static class FileHelper
    {
        public static void CreateIfNotExists(string fileName)
        {
            string binPath = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = Path.Combine(binPath, fileName);

            if (!File.Exists(filePath))
            {
                File.Create(filePath);
            }
        }
    }
}