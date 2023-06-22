using MyTools.Desktop.App.Services;
using System;
using System.Windows;

namespace MyTools.Desktop.App.Models
{
    public class ReminderElement : IReminderElement
    {
        readonly IStackConfig<IReminderElement> _config;

        readonly string _textValue;

        public ReminderElement(string textValue, TimeSpan timeSpan, IStackConfig<IReminderElement> config)
        {
            this._textValue = textValue;
            this.TimeSpan = timeSpan;
            this._config = config;
        }

        public TimeSpan TimeSpan { get; private set; }

        public UIElement BuildUIElement()
        {
            return StackElement.BuildUIElement(this._textValue, this._config);
        }

        public UIElement BuildUIElement(string textValue)
        {
            return StackElement.BuildUIElement(textValue, this._config);
        }

        public IStackElement KeepItOpen()
        {
            this._config.IsStackOpen = true;
            return this;
        }
    }
}
