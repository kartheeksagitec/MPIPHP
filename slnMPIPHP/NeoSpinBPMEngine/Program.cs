using NeoBase.Common;
using Sagitec.Common;
using System;
using System.Configuration;
using System.ServiceProcess;
using System.Threading;
using System.Windows.Forms;

namespace NeoBPMN.Service
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// <param name="args">"ServerName=<ServerName>" "UserId=<UserId>" "DeQueueInterval=500" "ConcurrentQueueRequests=10" "DefaultTimerInterval=300000"</param>
        [STAThread]
        static void Main(string[] args)
        {
            MPIPHP.Common.ApplicationSettings.MapSettingsObject();
            NeoBaseApplicationSettings.MapSettingsObject();
            string lstrParam = ConfigurationManager.AppSettings["IsWindowsService"].ToString();
            bool blnConsoleMode = false; // False for service, True for application
            if (lstrParam != null && lstrParam.Length > 0)
            {
                if (lstrParam == "true")
                    blnConsoleMode = false;
                else
                    blnConsoleMode = true;
            }

            if (blnConsoleMode)  // Run as Windows application
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new frmBPMNService(args));
            }
            else  // Run as Windows service
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                  {
                         new NeoSpinService(args)
                  };
                ServiceBase.Run(ServicesToRun);
            }

        }
    }
}
