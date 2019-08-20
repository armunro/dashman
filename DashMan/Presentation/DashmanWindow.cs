using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DashMan.Configuration.Model;

namespace DashMan.Presentation
{
    public class DashmanWindow : Form
    {
        private readonly WindowConfiguration _windowConfig;

        public DashmanWindow(WindowConfiguration windowConfig)
        {
            _windowConfig = windowConfig;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Point screenPoint = Screen.AllScreens[_windowConfig.ScreenNumber].WorkingArea.Location;
            StartPosition = FormStartPosition.Manual;
            Left = screenPoint.X + _windowConfig.Left;
            Top = screenPoint.Y + _windowConfig.Top;
            Width = _windowConfig.Width - 1;
            Height = _windowConfig.Height - 1;
            TopMost = _windowConfig.Topmost;
            BackColor = ColorTranslator.FromHtml(_windowConfig.BackgroundColor);
            FormBorderStyle = FormBorderStyle.None;
            Shown += (sender, args) =>
            {
                Width = _windowConfig.Width;
                Height = _windowConfig.Height;
            };
        }

        public void ConstructBrowsersFromConfiguration(List<BrowserConfiguration> browsers)
        {
            DashmanBrowser[] dashmanBrowsers = browsers.Select(browserConfig => new DashmanBrowser(browserConfig)).ToArray();
            Controls.AddRange(dashmanBrowsers);
        }
    }
}