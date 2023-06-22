using Microsoft.Extensions.Options;
using MyTools.Desktop.App2.ConfigOptions;
using System.Collections.Generic;

namespace MyTools.Desktop.App2.Services;

public class DataFileReaderService : FileReaderService<ICollection<string>>
{
    public DataFileReaderService(IOptions<DataFileConfig> fileConfig) : base(fileConfig.Value)
    {
    }
}