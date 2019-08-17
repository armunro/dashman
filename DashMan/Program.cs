using System;
using System.Windows.Forms;

namespace DashMan
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            DashmanApp app = new DashmanApp("C:\\dash.config ");
            app.ConstructFromConfiguration();
            Application.Run();
        }
    }
}