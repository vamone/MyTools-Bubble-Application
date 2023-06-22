using MyTools.Desktop.App2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTools.Desktop.App2.ConfigOptions
{
    public record DataFileConfig : IDataFileConfig
    {
        public const string ConfigBinding = "DataFileConfig";

        public string Name { get; init; }

        public string DefaultValue { get; init; }
    }
}
