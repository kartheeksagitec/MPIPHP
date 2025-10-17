using System;
using System.Windows.Forms;
using System.ServiceProcess;

namespace MPIPHP.BusinessTier
{
	static class EntryPoint
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
        static void Main(string[] args)
		{
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //F/W upgrade - security related changes.
            Sagitec.Common.utlConstants.ilstReadonlyMethodNames.Add("btnValidateExecuteBusinessMethod_Click");
            Sagitec.Common.utlConstants.ilstReadonlyMethodNames.Add("btnExecuteBusinessMethodSelectRows_Click");
            Sagitec.Common.utlConstants.ilstReadonlyMethodNames.Add("btnExecuteBusinessMethod_Click");
            Sagitec.Common.utlConstants.ilstReadonlyMethodNames.Add("btnDownloadFile_Click");
            Sagitec.Common.utlConstants.ilstReadonlyMethodNames.Add("btnDownload_Click");
            Sagitec.Common.utlConstants.ilstReadonlyMethodNames.Add("btnOpenDoc_Click");
            Sagitec.Common.utlConstants.ilstReadonlyMethodNames.Add("btnNewPopupDialog_Click");
            Sagitec.Common.utlConstants.ilstReadonlyMethodNames.Add("btnOpenPopupDialog_Click");
            Sagitec.Common.utlConstants.ilstReadonlyMethodNames.Add("btnFinishPopupDialog_Click");
            Sagitec.Common.utlConstants.ilstReadonlyMethodNames.Add("btnShowURLCLick");
            Sagitec.Common.utlConstants.ilstReadonlyMethodNames.Add("btn_OpenPDF");
			Sagitec.Common.utlConstants.ilstReadonlyMethodNames.Add("btnGridViewSelect_Click");
            Sagitec.Common.utlConstants.ilstReadonlyMethodNames.Add("btnGridViewDelete_Click");

            //FM upgrade: 6.0.7.0 changes - Code will ensure framework will create an object of Solution side class while its loading SystemSettings.
            MPIPHP.Common.ApplicationSettings.MapSettingsObject();
            Application.Run(new frmMPIPHPBusinessTier());

            //bool blnConsoleMode = false;
            //if (args.Length > 0)
            //{
            //    string strParam = args[0].Trim();
            //    blnConsoleMode = strParam == "/c" || strParam == "/console" ||
            //                     strParam == "-c" || strParam == "-console";
            //}

            //if (blnConsoleMode)  // Run as Windows application
            //{
            //    Application.EnableVisualStyles();
            //    Application.SetCompatibleTextRenderingDefault(false);
            //    Application.Run(new frmMPIPHPBusinessTier());
            //}
            //else  // Run as Windows service
            //{
            //    // More than one user Service may run within the same process. To add
            //    // another service to this process, change the following line to
            //    // create a second service object. For example,

            //    // ServicesToRun = New System.ServiceProcess.ServiceBase[]
            //    // {new MyMainServiceClass(), new MySecondUserService()};

            //    ServiceBase[] ServicesToRun;
            //    ServicesToRun = new ServiceBase[] 
            //    { 
            //        new MPIPHPService() 
            //    };
            //    ServiceBase.Run(ServicesToRun);
            //}
		}
	}
}
