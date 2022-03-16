using System;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MyTools.Desktop.App.Services
{
    public interface IStackElement
    {
        UIElement BuildUIElement();

        UIElement BuildUIElement(string textValue);
    }
}
