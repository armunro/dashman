using System;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
using DashMan.Configuration.Model;
using DashMan.Presentation.CEFSharp;

namespace DashMan.Presentation
{
 

    public class DashmanBrowser : Control
    {
        private ChromiumWebBrowser _chromiumBrowser;
        private readonly BrowserConfiguration _configuration;

        public DashmanBrowser(BrowserConfiguration configuration)
        {
            _configuration = configuration;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            //control
            Padding = Padding.Empty;
            Margin = Padding.Empty;
            AutoSize = false;
            Width = _configuration.Width;
            Height = _configuration.Height;
            Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            //browser
            _chromiumBrowser = new ChromiumWebBrowser(_configuration.Url);
            _chromiumBrowser.Dock = DockStyle.Fill;
          
            _chromiumBrowser.JsDialogHandler = new SuppressAlertsJsDialogHandler();
            _chromiumBrowser.IsBrowserInitializedChanged += OnChromiumBrowserOnIsBrowserInitializedChanged;
            Controls.Add(_chromiumBrowser);
        }

        private  void OnChromiumBrowserOnIsBrowserInitializedChanged(object sender,
            EventArgs args)
        {
            var chromiumBrowser = (ChromiumWebBrowser) sender;
            
            if (_configuration.ShowDevTools)
                chromiumBrowser.ShowDevTools();
            _configuration.CustomCss?.ForEach(x => RegisterCustomCss(chromiumBrowser, x));
            _configuration.CustomJs?.ForEach(x => RegisterCustomJs(chromiumBrowser, x));
        }


        private static void RegisterCustomCss(ChromiumWebBrowser browser, string customCssPath)
        {
            browser.ExecuteScriptAsyncWhenPageLoaded(@"
                var link = document.createElement(""link"");
                link.type = ""text/css"";
                link.rel = ""stylesheet"";
                link.href = ""localfolder://cefsharp/" + customCssPath + @""";
                document.head.appendChild(link);", false);
        }

        private static void RegisterCustomJs(ChromiumWebBrowser browser, string customJsPath)
        {
            browser.ExecuteScriptAsyncWhenPageLoaded(@"
                var script = document.createElement(""script"");
                script.type = ""application/javascript"";
                script.src = ""localfolder://cefsharp/" + customJsPath + @""";
                document.head.appendChild(script);", false);
        }
    }
}