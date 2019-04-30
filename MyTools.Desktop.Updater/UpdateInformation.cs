using System.Collections.Generic;
using System.Diagnostics;

namespace MyTools.Desktop.Updater
{
    [DebuggerDisplay("Version={Version}, StartFileAfterUpdate={StartFileAfterUpdate.Name}, FilesCount={Files.Count}")]
    public class UpdateInformation
    {
        public UpdateInformation()
        {
            this.Files = this.Files ?? new List<UpdateFile>();
        }

        public string DownloadUrl { get; set; }

        public UpdateFile StartFileAfterUpdate { get; set; }

        public ICollection<UpdateFile> Files { get; set; }

        public string Version { get; set; }
    }
}
