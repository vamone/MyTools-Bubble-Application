using MyTools.Desktop.App.Helpers;
using System;
using System.IO;
using System.Text.Json;

namespace MyTools.Desktop.App2.Services;

public abstract class FileReaderService<T> : IFileReaderService<T> where T : class
{
    readonly IDataFileConfig _fileConfig;

    public FileReaderService(IDataFileConfig fileConfig)
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