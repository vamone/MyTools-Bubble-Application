using MyTools.Desktop.App.Services;
using System;
using System.Windows;

namespace MyTools.Desktop.App.Models
{
    public class FocusElement : IFocusElement
    {
        readonly IStackConfig<IFocusElement> _config;

        readonly string _startText;

        readonly string _endText;

        readonly TimeSpan _timeSpan;

        public FocusElement(string startText, string endText, TimeSpan timeSpan, IStackConfig<IFocusElement> config)
        {
            this._startText = startText;
            this._endText = endText;
            this._timeSpan = timeSpan;
            this._config = config;
        }

        public DateTime LastFocusAt { get; private set; }

        public void SetLastFocusAt()
        {
            this.LastFocusAt = DateTime.UtcNow.Add(_timeSpan);
        }

        public IStackElement KeepItOpen()
        {
            this._config.IsStackOpen = true;
            return this;
        }

        public UIElement BuildUIElement()
        {
            return StackElement.BuildUIElement(this._startText, this._config);
        }

        public UIElement BuildUIElementEnd()
        {
            return StackElement.BuildUIElement(this._endText, this._config);
        }

        public UIElement BuildUIElement(string textValue)
        {
            return StackElement.BuildUIElement(textValue, this._config);
        }
    }
}
