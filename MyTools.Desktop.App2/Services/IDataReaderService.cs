using MyTools.Desktop.App.Helpers;
using MyTools.Desktop.App.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MyTools.Desktop.App2.Services
{
    public interface IDataReaderService<T>
    {
        T Get();

        void Set(T obj);
    }

    public interface IDataFileConfig
    {
        string Name { get; init; }

        string DefaultValue { get; init; }
    }

    public class DataReaderService<T> : IDataReaderService<T>
    {
        readonly IDataFileConfig _fileConfig;

        public DataReaderService(IDataFileConfig fileConfig)
        {
            this._fileConfig = fileConfig;

            FileHelper.CreateIfNotExists(this._fileConfig.Name, this._fileConfig.DefaultValue);
        }

        public T Get()
        {
            string json = File.ReadAllText(this._fileConfig.Name);
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new Exception();
            }

            var data = JsonSerializer.Deserialize<T>(json);
            if (data == null)
            {
                throw new Exception();
            }

            return data;
        }

        public void Set(T obj)
        {
            string json = JsonSerializer.Serialize(obj);
            if (string.IsNullOrWhiteSpace(json))
            {
                return;
            }

            File.WriteAllText(this._fileConfig.Name, json);
        }
    }
}
