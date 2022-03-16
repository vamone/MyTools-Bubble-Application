using System.Windows;

namespace MyTools.Desktop.App.Models
{
    public class CopyElement : ICopyElement
    {
        readonly IStackConfig<ICopyElement> _config;

        readonly string _textValue;

        public CopyElement(string textValue, IStackConfig<ICopyElement> config)
        {
            this._config = config;
            this._textValue = textValue;
        }

        public UIElement BuildUIElement()
        {
            return StackElement.BuildUIElement(this._textValue, this._config);
        }

        public UIElement BuildUIElement(string textValue)
        {
            return StackElement.BuildUIElement(textValue, this._config);
        }
    }
}
