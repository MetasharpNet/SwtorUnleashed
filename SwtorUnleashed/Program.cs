using System;
using System.Threading;
using System.Windows;
using SwtorUnleashed.Model;
using SwtorUnleashed.View;
using WF = System.Windows.Forms;

namespace SwtorUnleashed
{
    public class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                Setup.Load();
                // enable themes
                WF.Application.EnableVisualStyles();
                WF.Application.SetCompatibleTextRenderingDefault(false);
                // make sure only one instance of SWTOR Unleashed is running
                bool isNewMutex;
                using (new Mutex(true, "{2B051B53-9795-4a3b-A1AB-4019F3CF19F4}", out isNewMutex))
                {
                    if (isNewMutex)
                        Start(args);
                    else
                        Tools.ShowError("SWTOR Unleashed is already running.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                try { Log.Error(ex); } catch { }
            }
        }

        private static void Start(string[] args)
        {
            Log.Method();
            if (!ImDiskWrapper.IsInstalled())
            {
                Tools.ShowError("This program requires to have Imdisk installed on your machine.\n\nPlease download and install Imdisk prior using this program.", "Imdisk not installed");
                return;
            }
            if (Swtor.IsRunningGame())
            {
                Tools.ShowError("The game SWTOR is still running.\n\nPlease close the game prior using this program.", "Game is active");
                return;
            }
            if (Swtor.IsRunningLauncher())
            {
                Tools.ShowError("The launcher is already active.\n\nPlease close the launcher prior using this program.", "Launcher is active");
                return;
            }
            if (args.Length > 0)
                switch (args[0])
                {
                    case "unleashed":
                        Launcher.StartUnleashed();
                        return;
                    case "retail":
                        Launcher.StartRetail();
                        return;
                    case "setup":
                        Launcher.StartSetup();
                        return;
                    case "remove":
                        Launcher.RemoveUnleashed(true);
                        return;
                    default:
                        Tools.ShowError("Invalid given argument.\n\nValid arguments are: unleashed or retail", "Invalid argument");
                        break;
                }
            StartGui();
        }

        private static Application _app;

        public static void StartGui()
        {
            Log.Method();
            bool newApp = (_app == null);
            if (newApp)
                _app = new Application();
            var win = new LauncherWindow();
            win.Show();
            if (newApp)
                _app.Run();
        }
    }
}
