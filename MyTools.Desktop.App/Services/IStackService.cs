using System.Collections.Generic;

namespace MyTools.Desktop.App.Services
{
    public interface IStackService
    {
        IEnumerable<IStackElement> GetReminders();

        IEnumerable<IStackElement> GetCopies();

        IEnumerable<IStackElement> GetFocus();
    }
}
