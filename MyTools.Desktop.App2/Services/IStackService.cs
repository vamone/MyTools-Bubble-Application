using MyTools.Desktop.App.Models;
using System.Collections.Generic;

namespace MyTools.Desktop.App.Services;

public interface IStackService
{
    IEnumerable<IReminderElement> GetReminders();

    IEnumerable<IStackElement> GetCopies();

    IEnumerable<IFocusElement> GetFocus();
}