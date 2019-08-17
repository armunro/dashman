using System;
using System.IO;
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
    public class DashmanApp
    {
        private readonly string _configFilePath;

        public DashmanApp(string configFilePath)
        {
            _configFilePath = configFilePath;
        }

        public void ConstructFromConfiguration()
        {
            if (!ConfigurationProvider.DoesConfigurationExist(_configFilePath))
                ConfigurationProvider.RestoreDefaults(_configFilePath);
            var configuration = ConfigurationProvider.GetConfiguration(_configFilePath);

            InitializeCef(configuration.App);

            foreach (WindowConfiguration windowConfig in configuration.Windows)
            {
                Form window = new Form
                {
                    Text = windowConfig.Title,
                    Height = windowConfig.Height,
                    Width = windowConfig.Width,
                    FormBorderStyle = FormBorderStyle.None
                };
                foreach (BrowserConfiguration browserConfig in windowConfig.Browsers)
                {
                    var browser = new ChromiumWebBrowser(browserConfig.Url)
                    {
                        Height = browserConfig.Height,
                        Width = browserConfig.Width,
                        Top =  browserConfig.Top,
                        Left = browserConfig.Left,
                        Tag = new BrowserInfoTag(browserConfig),
                        JsDialogHandler = new SuppressAlertsJsDialogHandler()
                    };
                    browser.IsBrowserInitializedChanged += OnChromiumBrowserOnIsBrowserInitializedChanged;

                    window.Controls.Add(browser);
                }

                window.Show();
            }
            
            FileSystemWatcher assetWatcher = new FileSystemWatcher(@"..\assets");
            assetWatcher.Changed += (sender, args) => UpdateMessageBus.SendMessage();
            assetWatcher.EnableRaisingEvents = true;
        }

        private static void OnChromiumBrowserOnIsBrowserInitializedChanged(object sender,
            EventArgs args)
        {
            var chromiumBrowser = (ChromiumWebBrowser) sender;
            var infoTag = (BrowserInfoTag) chromiumBrowser.Tag;

            chromiumBrowser.Load(infoTag.Configuration.Url);
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