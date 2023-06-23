using Microsoft.Extensions.Options;
using MyTools.Desktop.App.Models;
using MyTools.Desktop.App2.ConfigOptions;
using MyTools.Desktop.App2.Models;
using System;
using System.Collections.Generic;

namespace MyTools.Desktop.App2.Services;

internal class TaskLoggerService : FileReaderService<ICollection<TaskLogger>>
{
    public TaskLoggerService() : base(GetFileConfig())
    {
    }

    private static IDataFileConfig GetFileConfig()
    {
        return new TaskFileConfig($"log-{DateTime.UtcNow.Month}-{DateTime.UtcNow.Day}-{DateTime.UtcNow.Year}.json");
    }
}