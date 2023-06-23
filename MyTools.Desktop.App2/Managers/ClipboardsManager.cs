using MyTools.Desktop.App.Helpers;
using MyTools.Desktop.App.Models;
using MyTools.Desktop.App.Utilities;
using MyTools.Desktop.App2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTools.Desktop.App2.Managers;

public class ClipboardsManager
{
    private const string _reminderRegexPattern = "^!([0-9]{1,2}:[0-9]{2});(.*)$";

    private readonly ICollection<IReminder> _showedReminders;

    readonly IFileReaderService<ICollection<string>> _dataService;

    readonly IStackConfig<IFocusElement> _focusConfig;

    public ClipboardsManager(IFileReaderService<ICollection<string>> dataService, IStackConfig<IFocusElement> focusConfig)
    {
        //this._showedReminders = showedReminders ?? new List<IReminder>();
        this._dataService = dataService;
        this._focusConfig = focusConfig;
    }

    public ICollection<string> GetClipboards()
    {
        return this._dataService.Get().Where(x => !x.StartsWith("!") && !x.StartsWith("#") && !x.StartsWith("?") && !string.IsNullOrWhiteSpace(x)).ToList();
    }

    public ICollection<IReminder> GetReminders()
    {
        return this._dataService.Get()
            .Where(x => x.StartsWith("!"))
            .Select(x => this.FormatReminder(x))
            .Where(x => x != null)
            .Where(x => this.FilterByShowedReminders(x)).ToList();
    }

    public IFocusElement GetFocus()
    {
        var focus = this._dataService.Get().Where(x => x.StartsWith("?"));

        return focus!.Select(x =>
        {
            var focusData = x.Split('|');

            string textData = focusData.FirstOrDefault();
            string regexData = focusData.LastOrDefault();

            if (string.IsNullOrWhiteSpace(textData) || string.IsNullOrWhiteSpace(regexData))
            {
                return null;
            }

            string focusStartText = RegexHelper.GetGroupValue(textData, regexData, 1);
            string focusTimeSpan = RegexHelper.GetGroupValue(textData, regexData, 2);
            string focusEndText = RegexHelper.GetGroupValue(textData, regexData, 3);

            if (string.IsNullOrWhiteSpace(focusStartText))
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(focusTimeSpan))
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(focusEndText))
            {
                return null;
            }

            TimeSpan.TryParse(focusTimeSpan, out TimeSpan focusTime);

            return new FocusElement(focusStartText, focusEndText, focusTime, _focusConfig);
        }).FirstOrDefault(x => x != null);
    }

    internal IReminder FormatReminder(string clipboard)
    {
        string reminderTime = RegexHelper.GetGroupValue(clipboard, _reminderRegexPattern, 1);
        if (string.IsNullOrWhiteSpace(reminderTime))
        {
            return null;
        }

        string reminderText = RegexHelper.GetGroupValue(clipboard, _reminderRegexPattern, 2);
        if (string.IsNullOrWhiteSpace(reminderText))
        {
            return null;
        }

        return new Reminder
        {
            Text = $"{reminderTime} - {reminderText}",
            TimeSpan = TimeSpan.Parse(reminderTime)
        };
    }

    internal bool FilterByShowedReminders(IReminder reminder)
    {
        bool isAllreadyShowed = this._showedReminders?.Any(x => x.TimeSpan.Equals(reminder.TimeSpan)) ?? false;

        return !isAllreadyShowed;
    }
}
