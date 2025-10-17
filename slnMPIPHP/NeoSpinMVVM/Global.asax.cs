using Sagitec.MVVMClient;
using Sagitec.PlatformAPI;
using System;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;

namespace Neo
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode , 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : BaseGlobal
    {
        protected override void AfterApplication_Start()
        {
            base.AfterApplication_Start();
            ServiceHelperConfiguration.AddConfiguration();
            AuthConfig.RegisterAuth();
            //ServiceHelper.Initialize(utlServiceType.Remoting);
            var larrAppJsBundles = ConfigurationManager.AppSettings["JsFilesForBundle"].Split(',').Select(s => "~/Scripts/App/" + s.Trim()).ToArray();
            MVVMBundleConfig.RegisterJSBundles(larrAppJsBundles);
            var larrAppCssBundles = ConfigurationManager.AppSettings["CssFilesForBundle"].Split(',').Select(s => "~/Styles/" + s.Trim()).ToArray();
            MVVMBundleConfig.RegisterCssBundles(BundleTable.Bundles, "~/bundles/AppSideCSS", larrAppCssBundles);
            IncludeCustomWebmethodsToAPIMethodsMap();
        }

        //FMk 6.0.0.35.28?
        protected override void SetIgnoreListParamsForSecurityCheck(BaseDelegatingHandler aBaseDelegatingHandler)
        {
            base.SetIgnoreListParamsForSecurityCheck(aBaseDelegatingHandler);
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


            aBaseDelegatingHandler.AddMethodNameToSenderFormNotRequiredList("SetLogOut");                                // example for #1                  
            aBaseDelegatingHandler.AddMethodNameToSenderIDNotRequiredList("getsystemregion");                            // example for #2         
            aBaseDelegatingHandler.AddMapForWebMethodsToAPIMethods("btnCustomMethod_Click", "CustomWebApiMethod");       // example for #3            
            aBaseDelegatingHandler.AddMethodNameToSenderFormNotRequiredList("GetPlanExists");
            aBaseDelegatingHandler.AddMethodNameToSenderIDNotRequiredList("GetPlanExists");
            aBaseDelegatingHandler.AddMethodNameToSenderFormNotRequiredList("EditCorrOnLocalTool_Override");
            aBaseDelegatingHandler.AddMethodNameToSenderIDNotRequiredList("EditCorrOnLocalTool_Override");
            aBaseDelegatingHandler.AddMethodNameToSenderFormNotRequiredList("GetLaunchImageViewerUrl");
            aBaseDelegatingHandler.AddMethodNameToSenderIDNotRequiredList("GetLaunchImageViewerUrl");
            aBaseDelegatingHandler.AddMethodNameToSenderFormNotRequiredList("NavigateToMSS");
            aBaseDelegatingHandler.AddMethodNameToSenderIDNotRequiredList("NavigateToMSS");
            aBaseDelegatingHandler.AddMethodNameToSenderFormNotRequiredList("FindPersonFromSSNAndMPID");
            aBaseDelegatingHandler.AddMethodNameToSenderIDNotRequiredList("FindPersonFromSSNAndMPID");
            aBaseDelegatingHandler.AddMethodNameToSenderFormNotRequiredList("ChangeBenefitOption");
            aBaseDelegatingHandler.AddMethodNameToSenderIDNotRequiredList("ChangeBenefitOption");
            aBaseDelegatingHandler.AddMethodNameToSenderFormNotRequiredList("GetTaxWithHoldingScreenConfiguratorColumns");
            aBaseDelegatingHandler.AddMethodNameToSenderIDNotRequiredList("GetTaxWithHoldingScreenConfiguratorColumns");
            aBaseDelegatingHandler.AddMethodNameToSenderFormNotRequiredList("GetBankName");
            aBaseDelegatingHandler.AddMethodNameToSenderIDNotRequiredList("GetBankName");

            aBaseDelegatingHandler.AddMethodNameToSenderFormNotRequiredList("GetLaserFicheUrlfromDB");
            aBaseDelegatingHandler.AddMethodNameToSenderIDNotRequiredList("GetLaserFicheUrlfromDB");
            aBaseDelegatingHandler.AddMethodNameToSenderFormNotRequiredList("GetWebExFlagDB");
            aBaseDelegatingHandler.AddMethodNameToSenderIDNotRequiredList("GetWebExFlagDB");

            aBaseDelegatingHandler.AddMethodNameToSenderFormNotRequiredList("InitiateServiceRetirement");
            aBaseDelegatingHandler.AddMethodNameToSenderIDNotRequiredList("InitiateServiceRetirement");

            aBaseDelegatingHandler.AddMethodNameToSenderFormNotRequiredList("CancelServiceRetirement");
            aBaseDelegatingHandler.AddMethodNameToSenderIDNotRequiredList("CancelServiceRetirement");

            aBaseDelegatingHandler.AddMethodNameToSenderFormNotRequiredList("SetActivityInstanceRefrenceId");
            aBaseDelegatingHandler.AddMethodNameToSenderIDNotRequiredList("SetActivityInstanceRefrenceId");

        }

        /// <summary>
        /// Place holder to load custom API methods for the mapping
        /// </summary>
        private void IncludeCustomWebmethodsToAPIMethodsMap()
        {
            BaseDelegatingHandler.idictWebMethodsToAPIMethods["btnRefreshServers_Click"].Add("RefreshAllAppServers");
        }

        protected override void BeforeApplication_Start()
        {
            base.BeforeApplication_Start();
            ServiceHelperConfiguration.EnableCorsWithAttributeRoutingPlatformApi();
        }

        protected void Application_Start()
        {
            MvcHandler.DisableMvcResponseHeader = true;
        }
    }
}