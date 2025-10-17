using System.Collections;
using System.ComponentModel;
using System.ServiceProcess;

namespace NeoBPMN.Service
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();

            ServiceProcessInstaller serviceProcessInstaller = new ServiceProcessInstaller();
            ServiceInstaller serviceInstaller = new ServiceInstaller();

            // Service Account Information
            serviceProcessInstaller.Account = ServiceAccount.LocalSystem;
            serviceProcessInstaller.Username = null;
            serviceProcessInstaller.Password = null;

            // Service Information
            serviceInstaller.ServiceName = "NeoSpin BPM Engine";
            serviceInstaller.DisplayName = "Sagitec NeoSpin BPM Engine";
            serviceInstaller.Description = "Run NeoSpin BPM Engine";

            serviceInstaller.StartType = ServiceStartMode.Automatic;

            this.Installers.Add(serviceProcessInstaller);
            this.Installers.Add(serviceInstaller);
        }

        protected override void OnBeforeInstall(IDictionary savedState)
        {
            base.OnBeforeInstall(savedState);

            // If you want to customize the name/description/accounttype etc
            // you can do it here.
        }

        /// <summary>
        /// Uninstall based on the service name
        /// </summary>
        /// <param name="savedState"></param>
        protected override void OnBeforeUninstall(IDictionary savedState)
        {
            base.OnBeforeUninstall(savedState);

            // Set the service name based on the input custom name as in
            // OnBeforeInstall
        }
    }
}