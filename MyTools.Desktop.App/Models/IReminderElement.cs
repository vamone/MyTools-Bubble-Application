using MyTools.Desktop.App.Services;
using System;

namespace MyTools.Desktop.App.Models
{
    public interface IReminderElement : IStackElement
    {
        TimeSpan TimeSpan { get; }
    }
}
