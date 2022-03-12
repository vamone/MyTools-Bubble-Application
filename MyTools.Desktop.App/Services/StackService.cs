using MyTools.Desktop.App.Helpers;
using MyTools.Desktop.App.Models;
using MyTools.Desktop.App.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyTools.Desktop.App.Services
{
    public class StackService : IStackService
    {
        const string _reminderRegexPattern = "^!([0-9]{1,2}:[0-9]{2});(.*)$";

        readonly ICollection<IReminder> _showedReminders;

        readonly IStackConfig _copyConfig;

        readonly IStackConfig _focusCopyConfig;

        readonly IStackConfig _reminderConfig;

        public StackService(IStackConfig copyConfig, IStackConfig focusCopies, IStackConfig reminderConfig)
        {
            _copyConfig = copyConfig;
            _focusCopyConfig = focusCopies;
            _reminderConfig = reminderConfig;
        }

        public IEnumerable<IStackElement> GetCopies()
        {
            return DataUtility.Get()
                .Where(x => !x.StartsWith("!") 
                && !x.StartsWith("#") 
                && !string.IsNullOrWhiteSpace(x)).Select(x => new StackElement(x, this._copyConfig)).ToList();
        }

        public IEnumerable<IStackElement> GetFocus()
        {
            yield return new StackElement("Focus time!", this._focusCopyConfig);
        }

        public IEnumerable<IStackElement> GetReminders()
        {
            return DataUtility.Get()
                .Where(x => x.StartsWith("!"))
                .Select(x => FormatReminder(x))
                .Where(x => x != null)
                .Where(x => FilterByShowedReminders(x)).Select(x => new StackElement(x.Text, _reminderConfig)).ToList();
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
            //Not sure if that works??

            return false;

            bool isAllreadyShowed = _showedReminders.Where(x => x.CreatedAt < DateTime.UtcNow).Any(x => x.TimeSpan.Equals(reminder.TimeSpan));
            return !isAllreadyShowed;
        }
    }
}
