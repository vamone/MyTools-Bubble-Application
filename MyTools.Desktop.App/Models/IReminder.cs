using System;

namespace MyTools.Desktop.App.Models
{
    public interface IReminder
    {
        string Text { get; set; }

        TimeSpan TimeSpan { get; set; }
    }
}
