using DashMan.Configuration.Model;

namespace DashMan.Controls.ChromiumWebBrowser
{
    public class BrowserInfoTag
    {
        public BrowserConfiguration Configuration { get; }

        public BrowserInfoTag(BrowserConfiguration configuration)
        {
            Configuration = configuration;
        }
    }
}