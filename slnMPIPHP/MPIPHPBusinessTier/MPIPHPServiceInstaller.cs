using System;
using System.Collections;
using System.ComponentModel;
using System.ServiceProcess;
using System.Configuration.Install;

namespace MPIPHP.BusinessTier
{
    /// <summary>
    /// This is a custom project installer.
    /// Applies a name to the service, sets description to the service,
    /// sets user name and password.
    /// </summary>
    [RunInstaller(true)]
    public partial class MPIPHPServiceInstaller : Installer
    {
        /// <summary>
        /// Initializes a new instance of <see cref="MPIPHPServiceInstaller"/>.
        /// </summary>
        public MPIPHPServiceInstaller()
        {
            InitializeComponent();

            ServiceProcessInstaller serviceProcessInstaller = new ServiceProcessInstaller();
            ServiceInstaller serviceInstaller = new ServiceInstaller();

            // Service Account Information
            serviceProcessInstaller.Account = ServiceAccount.LocalSystem;
            serviceProcessInstaller.Username = null;
            serviceProcessInstaller.Password = null;

            // Service Information
            serviceInstaller.ServiceName = "MPIPHP Service";
            serviceInstaller.DisplayName = "Sagitec MPIPHP Service";
            serviceInstaller.Description = "Provides access to Sagitec 'MPIPHPMetaDataCache', 'MPIPHPDBCache' " +
                "and 'MPIPHPBusinessTier' services.";

            serviceInstaller.StartType = ServiceStartMode.Automatic;

            this.Installers.Add(serviceProcessInstaller);
            this.Installers.Add(serviceInstaller);
        }

        /// <summary>
        /// This method is run before the install process.
        /// This method cane be overriden to set the following parameters:
        /// service name ,service description, account type, user account user
        /// name,a user account password. Note that when using a user account,
        /// if the user name or password is not set,
        /// the installing user is prompted for the credentials to use.
        /// </summary>
        /// <param name="savedState"></param>
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
