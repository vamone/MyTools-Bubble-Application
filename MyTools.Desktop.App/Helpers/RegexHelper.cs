using System.Text.RegularExpressions;

namespace MyTools.Desktop.App.Helpers
{
    public static class RegexHelper
    {
        public static string GetGroupValue(string text, string regexPattern, int groupNumber)
        {
            var regex = new Regex(regexPattern);

            var match = regex.Match(text);
            if (match.Success)
            {
                return match.Groups[groupNumber].Value;
            }

            return null;
        }
    }
}