using System;
using System.Linq;
using System.Windows.Forms;

namespace DashMan
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            DashmanApp app = new DashmanApp(args.First());
            app.ConstructFromConfiguration();
            Application.Run(app);
        }
    }
}