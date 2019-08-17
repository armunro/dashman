using System.IO;
using DashMan.Configuration.Defaults;
using DashMan.Configuration.Model;
using Newtonsoft.Json;

namespace DashMan.Configuration
{
    public static class ConfigurationProvider
    {
        public static DashConfiguration GetConfiguration(string filePath)
        {
            return JsonConvert.DeserializeObject<DashConfiguration>(File.ReadAllText(filePath));
        }

        public static void SetConfiguration(string filePath, DashConfiguration config)
        {
            File.WriteAllText(filePath, JsonConvert.SerializeObject(config, Formatting.Indented));
        }

        public static void RestoreDefaults(string filePath)
        {
            File.WriteAllText(filePath, JsonConvert.SerializeObject(ConfigDefaults.Basic, Formatting.Indented));
        }

        public static bool DoesConfigurationExist(string filePath)
        {
            return File.Exists(filePath);
        }
    }
}