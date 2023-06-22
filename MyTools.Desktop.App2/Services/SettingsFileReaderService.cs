using Microsoft.Extensions.Options;
using MyTools.Desktop.App.Models;
using MyTools.Desktop.App2.ConfigOptions;

namespace MyTools.Desktop.App2.Services;

public class SettingsFileReaderService : FileReaderService<Settings>
{
    public SettingsFileReaderService(IOptions<SettingsFileConfig> fileConfig) : base(fileConfig.Value)
    {
    }
}