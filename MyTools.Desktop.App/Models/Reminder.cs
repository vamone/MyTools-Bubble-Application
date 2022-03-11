using System;

namespace MyTools.Desktop.App.Models
{
    public class Reminder : IReminder
    {
        public string Text { get; set; }

        public TimeSpan TimeSpan { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
