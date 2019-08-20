using System;
using System.IO;
using System.Windows.Forms;
using CefSharp;
using CefSharp.SchemeHandler;
using CefSharp.WinForms;
using DashMan.Configuration;
using DashMan.Configuration.Model;
using DashMan.MessageBus;
using DashMan.Presentation;

namespace DashMan
{
    public class DashmanApp : ApplicationContext
    {
        private readonly string _configFilePath;

        public DashmanApp(string configFilePath)
        {
            _configFilePath = configFilePath;
        }

        protected override void OnMainFormClosed(object sender, EventArgs e)
        {
            Cef.Shutdown();
            base.OnMainFormClosed(sender, e);
        }

        public void ConstructFromConfiguration()
        {
            if (!ConfigurationProvider.DoesConfigurationExist(_configFilePath))
                ConfigurationProvider.RestoreDefaults(_configFilePath);
            var configuration = ConfigurationProvider.GetConfiguration(_configFilePath);


            InitializeCef(configuration.App);


            foreach (WindowConfiguration windowConfig in configuration.Windows)
            {
                DashmanWindow window = new DashmanWindow(windowConfig);
                window.FormClosed += WindowOnFormClosed;
                window.ConstructBrowsersFromConfiguration(windowConfig.Browsers);
                window.Show();
            }

            FileSystemWatcher assetWatcher = new FileSystemWatcher(@"..\assets");
            assetWatcher.Changed += (sender, args) => UpdateMessageBus.SendMessage();
            assetWatcher.EnableRaisingEvents = true;
        }

        private void WindowOnFormClosed(object sender, FormClosedEventArgs e)
        {
            Cef.Shutdown();
            Application.Exit();
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
            Cef.EnableHighDPISupport();
            Cef.Initialize(settings, performDependencyCheck: true, browserProcessHandler: null);
        }
    }
}