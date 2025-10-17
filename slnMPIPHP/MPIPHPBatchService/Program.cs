using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace MPIPHP.MPIPHPJobService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //FM upgrade: 6.0.7.0 changes - Code will ensure framework will create an object of Solution side class while its loading SystemSettings.
            MPIPHP.Common.ApplicationSettings.MapSettingsObject();
            Application.Run(new MPIPHPJobService());
        }
    }
}
