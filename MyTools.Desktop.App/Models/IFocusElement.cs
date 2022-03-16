using MyTools.Desktop.App.Services;
using System;
using System.Windows;

namespace MyTools.Desktop.App.Models
{
    public interface IFocusElement : IStackElement
    {
        DateTime LastFocusAt { get; }

        void SetLastFocusAt();

        IFocusElement KeepItOpen();

        UIElement BuildUIElementEnd();
    }
}