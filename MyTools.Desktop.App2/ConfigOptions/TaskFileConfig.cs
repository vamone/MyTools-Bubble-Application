using MyTools.Desktop.App2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTools.Desktop.App2.ConfigOptions
{
    public record TaskFileConfig : IDataFileConfig
    {
        public const string ConfigBinding = "TaskFileConfig";

        public TaskFileConfig(string name)
        {
            this.Name = name;
        }

        public string Name { get; init; }

        public string DefaultValue { get; init; }
    }
}
