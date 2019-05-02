using System.Collections.Generic;
using System.Windows.Controls;

namespace MyTools.Desktop.App
{
    public static class TextBoxExtention
    {
        public static ICollection<string> AsLines(this TextBox source)
        {
            var lines = new List<string>();

            int lineCount = source.LineCount;

            for (int line = 0; line < lineCount; line++)
            {
                string lineText = source.GetLineText(line);
                lines.Add(lineText.Trim());
            }

            return lines;
        }
    }
}
