using MyTools.Desktop.App.Helpers;
using MyTools.Desktop.App.Models;
using MyTools.Desktop.App.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyTools.Desktop.App.Managers
{
    public class ClipboardsManager
    {
        private const string _reminderRegexPattern = "^!([0-9]{1,2}:[0-9]{2});(.*)$";

        private readonly ICollection<IReminder> _showedReminders;

        public ClipboardsManager(ICollection<IReminder> showedReminders)
        {
            this._showedReminders = showedReminders ?? new List<IReminder>();
        }

        public ICollection<string> GetClipboards()
        {
            return DataUtility.Get().Where(x => !x.StartsWith("!") && !x.StartsWith("#") && !string.IsNullOrWhiteSpace(x)).ToList();
        }

        public ICollection<IReminder> GetReminders()
        {
            return DataUtility.Get()
                .Where(x => x.StartsWith("!"))
                .Select(x => this.FormatReminder(x))
                .Where(x => x != null)
                .Where(x => this.FilterByShowedReminders(x)).ToList();
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
            bool isAllreadyShowed = this._showedReminders.Where(x => x.CreatedAt < DateTime.UtcNow).Any(x => x.TimeSpan.Equals(reminder.TimeSpan));

            return !isAllreadyShowed;
        }
    }
}
