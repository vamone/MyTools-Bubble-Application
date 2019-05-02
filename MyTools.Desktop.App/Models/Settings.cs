namespace MyTools.Desktop.App.Models
{
    public class Settings
    {
        private const double _defaultWindowOpacity = 0.65f;

        private const string _defaultUpdateCheckUrl = "https://monych.se/bubbles/mytools/update/";

        public Settings(double windowOpacity = 0, double clipboardLeftMargin = 0)
        {
            this.UpdateCheckUrl = _defaultUpdateCheckUrl;
            this.WindowOpacity = windowOpacity <= 0 ? _defaultWindowOpacity : windowOpacity;
            this.ClipboardLeftMargin = clipboardLeftMargin;
        }

        public string UpdateCheckUrl { get; set; }

        public double WindowOpacity { get; set; }

        public double ClipboardLeftMargin { get; set; }

        public double PositionTop { get; set; }

        public double PositionLeft { get; set; }
    }
}
