using System.Collections.Generic;

namespace DashMan.Configuration.Model
{
    public class BrowserConfiguration
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int Top { get; set; }
        public int Left { get; set; }
        public string Url { get; set; }
        public List<string> CustomCss { get; set; }
        public List<string> CustomJs { get; set; }
        public bool ShowDevTools { get; set; }
        public int RefreshInterval { get; set; }
    }
}