namespace DashMan.Configuration.Model
{
    public class AppConfiguration
    {
        public string CachePath { get; set; } = "cef_cache";
        public string UserDataPath { get; set; } = "cef_userdata";
        public bool PersistUserPreferences { get; set; } = true;
        public bool PersistSessionCookies { get; set; } = true;
    }
}