using System.Collections.Generic;

namespace DashMan.Configuration.Model
{
    public class WindowConfiguration
    {
        public List<BrowserConfiguration> Browsers { get; set; }
        public int ScreenNumber { get; set; }
        public bool Topmost { get; set; }
        public string BackgroundColor { get; set; }
        public string BackgroundImage { get; set; }
        public bool HideCursor { get; set; }
        public string Title { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public int Top { get; set; }
        public int Left { get; set; }
    }
}