using System.Collections.Generic;
using DashMan.Configuration.Model;

// ReSharper disable RedundantEmptyObjectOrCollectionInitializer

namespace DashMan.Configuration.Defaults
{
    public static class ConfigDefaults
    {
        public static readonly DashConfiguration Basic = new DashConfiguration
        {
            App = new AppConfiguration
            {
                //defaults in AppConfiguration.cs
            },
            Windows = new List<WindowConfiguration>()
            {
                new WindowConfiguration()
                {
                    Browsers = new List<BrowserConfiguration>()
                    {
                        new BrowserConfiguration
                        {
                            Width = 300,
                            Height = 300,
                            Top = 1,
                            Left = 1,
                            Url = "https://google.ca",
                            CustomCss = new List<string> {"google.css"},
                            ShowDevTools = false
                        }
                    }
                },
                new WindowConfiguration()
                {
                    Browsers = new List<BrowserConfiguration>()
                    {
                        new BrowserConfiguration
                        {
                            Width = 300,
                            Height = 300,
                            Top = 1,
                            Left = 300,
                            Url = "https://google.ca",
                            CustomCss = new List<string> {"custom.css"},
                            CustomJs = new List<string> {"custom.js"},
                            ShowDevTools = false
                        }
                    }
                }
            }
        };
    }
}