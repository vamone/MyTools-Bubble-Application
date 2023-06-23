using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTools.Desktop.App2.Models
{
    public class TaskLogger
    {
        public TaskLogger()
        {
            this.CreatedAt = DateTime.UtcNow;
        }

        public string TaskId { get; set; }

        public string TaskDescription { get; set; }

        public DateTime CreatedAt { get; set; }

    }
}
