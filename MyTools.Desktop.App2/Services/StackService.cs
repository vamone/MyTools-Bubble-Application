using MyTools.Desktop.App.Helpers;
using MyTools.Desktop.App.Models;
using MyTools.Desktop.App.Utilities;
using MyTools.Desktop.App2.ConfigOptions;
using MyTools.Desktop.App2.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyTools.Desktop.App.Services;

public class StackService : IStackService
{
    readonly IStackConfig<ICopyElement> _copyConfig;

    readonly IStackConfig<IFocusElement> _focusConfig;

    readonly IStackConfig<IReminderElement> _reminderConfig;

    readonly IFileReaderService<ICollection<string>> _dataService;

    readonly IFileReaderService<Settings> _settingsService;

    public StackService(
        IStackConfig<ICopyElement> copyConfig, 
        IStackConfig<IFocusElement> focusConfig, 
        IStackConfig<IReminderElement> reminderConfig,
        IFileReaderService<ICollection<string>> dataService)
    {
        this._copyConfig = copyConfig;
        this._focusConfig = focusConfig;
        this._reminderConfig = reminderConfig;

        this._dataService = dataService;
        //this._settingsService = settingsService;
    }

    public IEnumerable<IStackElement> GetCopies()
    {
        return this._dataService.Get()
            .Where(x => !x.StartsWith("!")
            && !x.StartsWith("#")
            && !x.StartsWith("?")
            && !string.IsNullOrWhiteSpace(x)).Select(x => new CopyElement(x, this._copyConfig)).ToList();
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

    public IEnumerable<IReminderElement> GetReminders()
    {
        return this._dataService.Get()
            .Where(x => x.StartsWith("!"))
            .Select(x =>
            {
                var reminderData = x.Split('|');

                string textData = reminderData.FirstOrDefault();
                string regexData = reminderData.LastOrDefault();

                if (string.IsNullOrWhiteSpace(textData) || string.IsNullOrWhiteSpace(regexData))
                {
                    return null;
                }

                string reminderTimeSpan = RegexHelper.GetGroupValue(textData, regexData, 1);
                string reminderText = RegexHelper.GetGroupValue(textData, regexData, 2);

                if (string.IsNullOrWhiteSpace(reminderTimeSpan))
                {
                    return null;
                }

                if (string.IsNullOrWhiteSpace(reminderText))
                {
                    return null;
                }

                TimeSpan.TryParse(reminderTimeSpan, out TimeSpan reminderTime);

                return new ReminderElement($"{reminderTimeSpan} - {reminderText}", reminderTime, this._reminderConfig);
            })
            .Where(x => x != null)
            .ToList();
    }
}