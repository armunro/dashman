using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using CefSharp;
using CefSharp.SchemeHandler;
using CefSharp.WinForms;
using DashMan.Configuration;
using DashMan.Configuration.Model;
using DashMan.Controls.ChromiumWebBrowser;
using DashMan.MessageBus;

namespace DashMan
{
    public class DashmanApp : ApplicationContext
    {
        private readonly string _configFilePath;

        public DashmanApp(string configFilePath)
        {
            _configFilePath = configFilePath;
            ConstructFromConfiguration();
        }

        public void ConstructFromConfiguration()
        {
            if (!ConfigurationProvider.DoesConfigurationExist(_configFilePath))
                ConfigurationProvider.RestoreDefaults(_configFilePath);
            var configuration = ConfigurationProvider.GetConfiguration(_configFilePath);

         
            Cef.EnableHighDPISupport();
            InitializeCef(configuration.App);


            foreach (WindowConfiguration windowConfig in configuration.Windows)
            {
                Form window = new Form
                {
                    Height = windowConfig.Height,
                    Width = windowConfig.Width,
                    DesktopLocation = new Point(windowConfig.Left, windowConfig.Top),
                    FormBorderStyle = FormBorderStyle.None,
                    TopMost = windowConfig.Topmost,
                    StartPosition = FormStartPosition.Manual,
                    BackColor =  ColorTranslator.FromHtml(windowConfig.BackgroundColor)
                };
               
                foreach (BrowserConfiguration browserConfig in windowConfig.Browsers)
                {
                    var browser = new ChromiumWebBrowser(browserConfig.Url)
                    {
                        AutoSize = false,
                        Size = new Size(browserConfig.Width, browserConfig.Height),
                        Location = new Point(browserConfig.Left, browserConfig.Top),
                        Tag = new BrowserInfoTag(browserConfig),
                        Dock = DockStyle.None,
                        JsDialogHandler = new SuppressAlertsJsDialogHandler(),
                        FocusHandler = null
                    };
                    browser.IsBrowserInitializedChanged += OnChromiumBrowserOnIsBrowserInitializedChanged;
                    window.Controls.Add(browser);
                }
            

                var thread = new Thread(Start);
                thread.Start(window);
            }

            FileSystemWatcher assetWatcher = new FileSystemWatcher(@"..\assets");
            assetWatcher.Changed += (sender, args) => UpdateMessageBus.SendMessage();
            assetWatcher.EnableRaisingEvents = true;
        }

        private void Start(object obj)
        {
            Form sender = (Form) obj;
            Cursor.Hide();
            sender.Closed += (o, args) => Application.Exit();

            Application.Run(sender);
        }


        private static void OnChromiumBrowserOnIsBrowserInitializedChanged(object sender,
            EventArgs args)
        {
            var chromiumBrowser = (ChromiumWebBrowser) sender;
            var infoTag = (BrowserInfoTag) chromiumBrowser.Tag;

            if (infoTag.Configuration.ShowDevTools)
                chromiumBrowser.ShowDevTools();
            RegisterCustomAssets(chromiumBrowser, infoTag);
        }

        private static void RegisterCustomAssets(ChromiumWebBrowser chromiumBrowser, BrowserInfoTag infoTag)
        {
            infoTag.Configuration.CustomCss?.ForEach(x => RegisterCustomCss(chromiumBrowser, x));
            infoTag.Configuration.CustomJs?.ForEach(x => RegisterCustomJs(chromiumBrowser, x));
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

        private static void InitializeCef(AppConfiguration appConfiguration)
        {
            if (Cef.IsInitialized) return;
            string assetsDir = @"..\assets";
            Directory.CreateDirectory(assetsDir);

            var settings = new CefSettings
            {
                PersistSessionCookies = appConfiguration.PersistSessionCookies,
                PersistUserPreferences = appConfiguration.PersistUserPreferences,
                CachePath = appConfiguration.CachePath,
                UserDataPath = appConfiguration.UserDataPath
            };


            settings.RegisterScheme(new CefCustomScheme
            {
                SchemeName = "localfolder",
                DomainName = "cefsharp",
                SchemeHandlerFactory = new FolderSchemeHandlerFactory(
                    @"..\assets",
                    hostName: "cefsharp"
                )
            });

            Cef.Initialize(settings);
        }
    }
}