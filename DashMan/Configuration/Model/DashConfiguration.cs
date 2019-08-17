using System.Collections.Generic;

namespace DashMan.Configuration.Model
{
    public class DashConfiguration
    {
        public AppConfiguration App { get; set; }
        public List<WindowConfiguration> Windows { get; set; }
    }
}