using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Web.Services;
using System.Web.Script.Services;
using System.Configuration;
using Sagitec.Interface;
using Sagitec.DataObjects;
using Sagitec.BusinessObjects;
using MPIPHP.BusinessObjects;
using System.Reflection;
using System.Linq;
using Sagitec.Common;
using Sagitec.WebClient.WebServices;
using System.Web;
using System.ServiceModel;
using Sagitec.WebClient;
using Newtonsoft.Json;

/// <summary>
/// Summary description for SagitecWebServices
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
[System.Web.Script.Services.ScriptService]
public class SagitecWebServices : wfmWebService
{
    public SagitecWebServices()
    {
        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    public override string GetRetrievalValuesFromMethod(string astrMethodName, string astrParameters, string astrFieldNames)
    {
        string lstrResult = base.GetRetrievalValuesFromMethod(astrMethodName, astrParameters, astrFieldNames);
        //PROD PIR 135
        astrMethodName = HelperFunction.SagitecDecryptAES(astrMethodName, istrEncryptionKey, istrEncryptionIV);
        if (astrMethodName == "GetSpouseDetails")
            lstrResult = lstrResult.ToString().Replace("01/01/0001", "");
        return lstrResult;
    }
    [ScriptMethod]
    [WebMethod(EnableSession = true)]
    public string GetTaxWithHoldingScreenConfiguratorColumns(string astrTaxIdentifierVal,string astrBenType)
    {
        IBusinessTier isrvBusinessTier = null;
        IMetaDataCache isrvMetaDataCache = null;
        IDBCache isrvDBCache = null;
        try
        {
            int aintPersonID = 0;
            //Configuration MPIWebConfig = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
            //if (MPIWebConfig.AppSettings.Settings.Count > 0)
            //{
            //    KeyValueConfigurationElement WebConfigAppSetting = MPIWebConfig.AppSettings.Settings["AppSettingsLocation"];
            //    HelperFunction.istrAppSettingsLocation = WebConfigAppSetting.Value;
            //}

            //string strBusinessTierUrl = String.Format(HelperFunction.GetAppSettings("BusinessTierUrl"), "srvPayeeAccount");
            //string strMetaDataUrl = HelperFunction.GetAppSettings("MetaDataCacheUrl");
            //string strDBCacheUrl = HelperFunction.GetAppSettings("DBCacheUrl");
            string strBusinessTierUrl = String.Format(MPIPHP.Common.ApplicationSettings.Instance.BusinessTierUrl, "srvPayeeAccount");
            string strMetaDataUrl = MPIPHP.Common.ApplicationSettings.Instance.MetaDataCacheUrl;
            string strDBCacheUrl = MPIPHP.Common.ApplicationSettings.Instance.DBCacheUrl;

            //FM upgrade changes - Remoting to WCF
            //IBusinessTier isrvBusinessTier = (IBusinessTier)Activator.GetObject(typeof(IBusinessTier), strBusinessTierUrl);
            //IMetaDataCache isrvMetaDataCache = (IMetaDataCache)Activator.GetObject(typeof(IMetaDataCache), strMetaDataUrl);
            //IDBCache isrvDBCache = (IDBCache)Activator.GetObject(typeof(IDBCache), strDBCacheUrl);
            isrvBusinessTier = WCFClient<IBusinessTier>.CreateChannel(strBusinessTierUrl);
            isrvMetaDataCache = WCFClient<IMetaDataCache>.CreateChannel(strMetaDataUrl);
            isrvDBCache = WCFClient<IDBCache>.CreateChannel(strDBCacheUrl);

            Dictionary<string, object> ldictParams = new Dictionary<string, object>();

            idictParams[utlConstants.istrSessionID] = Session.SessionID;
            //idictParams[utlConstants.istrWindowName] = istrFWN;
            idictParams[utlConstants.istrWindowName] = Framework.istrWindowName;
            idictParams[utlConstants.istrParentForm] = "CenterMiddle";
            //idictParams[utlConstants.istrConstUserID] = Session[istrFWN + "UserID"];
            idictParams[utlConstants.istrConstUserID] = Session[Framework.istrWindowName + "UserID"];
            //idictParams[utlConstants.istrConstUserSerialID] = Session[istrFWN + "UserSerialID"];
            idictParams[utlConstants.istrConstUserSerialID] = Session[Framework.istrWindowName + "UserSerialID"];

            //utlPassInfo iobjPassInfo = new utlPassInfo();
            //utlPassInfo.iobjPassInfo = iobjPassInfo;
            //iobjPassInfo.isrvMetaDataCache = isrvMetaDataCache;

            Hashtable lhstParam = new Hashtable();
            lhstParam.Add("TAX_IDENTIFIER_VALUE", astrTaxIdentifierVal);
            lhstParam.Add("BENDISTYPE", astrBenType);


            // Dictionary<string, object> ldictParams = new Dictionary<string, object>();
            ldictParams[utlConstants.istrConstUserID] = "WebService";

            DataTable ldtFEDTaxCaluationFinalAmount = isrvBusinessTier.ExecuteQuery("cdoPayeeAccountTaxWithholding.GetTaxWithHoldingScreenColumns", lhstParam, ldictParams);

            string JSONString = string.Empty;
            JSONString = JsonConvert.SerializeObject(ldtFEDTaxCaluationFinalAmount);
            return JSONString;
        }
        finally
        {
            HelperFunction.CloseChannel(isrvBusinessTier);
            HelperFunction.CloseChannel(isrvMetaDataCache);
            HelperFunction.CloseChannel(isrvDBCache);
        }
    }



    [ScriptMethod]
    [WebMethod(EnableSession = true)]
    public string GetLaserFicheUrlfromDB()
    {
        IBusinessTier isrvBusinessTier = null;
        IMetaDataCache isrvMetaDataCache = null;
        IDBCache isrvDBCache = null;
        try
        {
            int aintPersonID = 0;
           
            string strBusinessTierUrl = String.Format(MPIPHP.Common.ApplicationSettings.Instance.BusinessTierUrl, "srvPerson");
            string strMetaDataUrl = MPIPHP.Common.ApplicationSettings.Instance.MetaDataCacheUrl;
            string strDBCacheUrl = MPIPHP.Common.ApplicationSettings.Instance.DBCacheUrl;

            //FM upgrade changes - Remoting to WCF
            isrvBusinessTier = WCFClient<IBusinessTier>.CreateChannel(strBusinessTierUrl);
            isrvMetaDataCache = WCFClient<IMetaDataCache>.CreateChannel(strMetaDataUrl);
            isrvDBCache = WCFClient<IDBCache>.CreateChannel(strDBCacheUrl);

            Dictionary<string, object> ldictParams = new Dictionary<string, object>();

            idictParams[utlConstants.istrSessionID] = Session.SessionID;
            idictParams[utlConstants.istrWindowName] = Framework.istrWindowName;
            idictParams[utlConstants.istrParentForm] = "CenterMiddle";
            idictParams[utlConstants.istrConstUserID] = Session[Framework.istrWindowName + "UserID"];
            idictParams[utlConstants.istrConstUserSerialID] = Session[Framework.istrWindowName + "UserSerialID"];

            Hashtable lhstParam = new Hashtable();
            //lhstParam.Add("TAX_IDENTIFIER_VALUE", astrTaxIdentifierVal);
            //lhstParam.Add("BENDISTYPE", astrBenType);


            // Dictionary<string, object> ldictParams = new Dictionary<string, object>();
            ldictParams[utlConstants.istrConstUserID] = "WebService";

            DataTable ldtLaserFiche = isrvBusinessTier.ExecuteQuery("cdoPerson.GetLaserFiche", lhstParam, ldictParams);

            string JSONString = string.Empty;
            JSONString = JsonConvert.SerializeObject(ldtLaserFiche);
            return JSONString;
        }
        finally
        {
            HelperFunction.CloseChannel(isrvBusinessTier);
            HelperFunction.CloseChannel(isrvMetaDataCache);
            HelperFunction.CloseChannel(isrvDBCache);
        }
    }

    [ScriptMethod]
    [WebMethod(EnableSession = true)]
    public string GetWebExFlagDB()
    {
        IBusinessTier isrvBusinessTier = null;
        IMetaDataCache isrvMetaDataCache = null;
        IDBCache isrvDBCache = null;
        try
        {
            int aintPersonID = 0;

            string strBusinessTierUrl = String.Format(MPIPHP.Common.ApplicationSettings.Instance.BusinessTierUrl, "srvPerson");
            string strMetaDataUrl = MPIPHP.Common.ApplicationSettings.Instance.MetaDataCacheUrl;
            string strDBCacheUrl = MPIPHP.Common.ApplicationSettings.Instance.DBCacheUrl;

            //FM upgrade changes - Remoting to WCF
            isrvBusinessTier = WCFClient<IBusinessTier>.CreateChannel(strBusinessTierUrl);
            isrvMetaDataCache = WCFClient<IMetaDataCache>.CreateChannel(strMetaDataUrl);
            isrvDBCache = WCFClient<IDBCache>.CreateChannel(strDBCacheUrl);

            Dictionary<string, object> ldictParams = new Dictionary<string, object>();

            idictParams[utlConstants.istrSessionID] = Session.SessionID;
            idictParams[utlConstants.istrWindowName] = Framework.istrWindowName;
            idictParams[utlConstants.istrParentForm] = "CenterMiddle";
            idictParams[utlConstants.istrConstUserID] = Session[Framework.istrWindowName + "UserID"];
            idictParams[utlConstants.istrConstUserSerialID] = Session[Framework.istrWindowName + "UserSerialID"];

            Hashtable lhstParam = new Hashtable();
            //lhstParam.Add("TAX_IDENTIFIER_VALUE", astrTaxIdentifierVal);
            //lhstParam.Add("BENDISTYPE", astrBenType);


            // Dictionary<string, object> ldictParams = new Dictionary<string, object>();
            ldictParams[utlConstants.istrConstUserID] = "WebService";

            DataTable ldtLaserFiche = isrvBusinessTier.ExecuteQuery("cdoPerson.GetWebExFlag", lhstParam, ldictParams);

            string JSONString = string.Empty;
            JSONString = JsonConvert.SerializeObject(ldtLaserFiche);
            return JSONString;
        }
        finally
        {
            HelperFunction.CloseChannel(isrvBusinessTier);
            HelperFunction.CloseChannel(isrvMetaDataCache);
            HelperFunction.CloseChannel(isrvDBCache);
        }
    }

    [ScriptMethod]
    [WebMethod(EnableSession = true)]
    public string GetPlanExists(string astrPlanId, string astrBenType)
    {
        try
        {
            if (astrPlanId != 9.ToString())
            {
                string lstrResult = "100";
                //FM upgrade: 6.0.0.32 build error changes - istrFWN to Framework.istrWindowName
                //object lobjMain = Session[istrFWN + "CenterMiddle"];
                object lobjMain = Session[Framework.istrWindowName + "CenterMiddle"];
                if (lobjMain is busPersonBeneficiary)
                {
                    busPersonBeneficiary lbusPersonBeneficiary = (busPersonBeneficiary)lobjMain;
                    foreach (busPersonAccountBeneficiary lbusPersonAccountBeneficiary in lbusPersonBeneficiary.iclbPersonAccountBeneficiary)
                    {
                        string lstrPlanID = lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.iaintPlan.ToString();
                        string lstrBenType = lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.beneficiary_type_value;
                        if (lstrResult != "Y")
                        {
                            if (lstrPlanID == astrPlanId && astrBenType == lstrBenType)
                            {
                                lstrResult = "Y";
                                break;
                            }
                        }
                    }
                    if (lstrResult != "Y")
                    {
                        foreach (busPersonAccountBeneficiary lbusPersonAccountBeneficiary in lbusPersonBeneficiary.iclbPersonAccountBeneficiariesAll)
                        {
                            string lstrPlanID = lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.iaintPlan.ToString();
                            string lstrBenType = lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.beneficiary_type_value;
                            if (lstrResult != "50")
                            {
                                if (lstrPlanID == astrPlanId && astrBenType == lstrBenType)
                                {
                                    lstrResult = "50";
                                    break;
                                }
                            }
                        }

                    }
                }

                return lstrResult;
            }
            else
            {
                return string.Empty;
            }
        }
        catch
        {
            return string.Empty;
        }
    }

    #region Correspondence Editor tools Changes required for https
    [ScriptMethod]
    [WebMethod(EnableSession = true)]
    public string EditCorrOnLocalTool2(object aParams)
    {
        try
        {
            Dictionary<string, object> AllParams = (Dictionary<string, object>)aParams;
            string lHttpResponseMessage = null;

            string lstrLastGeneratedCorr = "";
            //lstrLastGeneratedCorr = (string)HttpContext.Current.Session[istrFWN + "CorrFileName"];
            lstrLastGeneratedCorr = (string)HttpContext.Current.Session[Framework.istrWindowName + "CorrFileName"];

            string lstrDefaultPrinter = AllParams["DefaultPrinter"].ToString();
            //bool lblnReadOnlyMode = (string)HttpContext.Current.Session[istrFWN + "LastCorrSecurityLevel"] == "1";
            bool lblnReadOnlyMode = (string)HttpContext.Current.Session[Framework.istrWindowName + "LastCorrSecurityLevel"] == "1";
            bool lblnShowPrintDialog = AllParams["ShowPrintDialog"].ToString().ToLower() == "true";

            Dictionary<string, object> ldictParams = new Dictionary<string, object>();

            idictParams[utlConstants.istrSessionID] = Session.SessionID;
            //idictParams[utlConstants.istrWindowName] = istrFWN;
            idictParams[utlConstants.istrWindowName] = Framework.istrWindowName;
            idictParams[utlConstants.istrParentForm] = "CenterMiddle";
            //idictParams[utlConstants.istrConstUserID] = Session[istrFWN + "UserID"];
            idictParams[utlConstants.istrConstUserID] = Session[Framework.istrWindowName + "UserID"];
            //idictParams[utlConstants.istrConstUserSerialID] = Session[istrFWN + "UserSerialID"];
            idictParams[utlConstants.istrConstUserSerialID] = Session[Framework.istrWindowName + "UserSerialID"];

            ArrayList Result = isrvBusinessTier.EditCorrOnLocalTool(lstrLastGeneratedCorr, idictParams);

            if (Result.Count > 0 && Result[0] is utlError)
            {
                lHttpResponseMessage += ((utlError)Result[0]).istrErrorMessage;
                Context.Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                return lHttpResponseMessage;
            }

            string lBase64String = Result[0].ToString();

            NetTcpBinding binding = new NetTcpBinding();
            binding.MaxBufferPoolSize = 1000000000;
            binding.MaxBufferSize = 1000000000;
            binding.MaxReceivedMessageSize = 1000000000;
            binding.ReaderQuotas.MaxStringContentLength = 1000000000;
            binding.ReaderQuotas.MaxArrayLength = 1000000000;
            binding.ReaderQuotas.MaxDepth = 1000000000;
            binding.ReaderQuotas.MaxBytesPerRead = 1000000000;
            binding.Security.Mode = SecurityMode.None;

            string UserID = idictParams["UserID"].ToString();
            int UserSerialID = Convert.ToInt32(idictParams["UserSerialID"].ToString());
            string RequestIPAddress = GetIP();
            EndpointAddress address = new EndpointAddress("net.tcp://" + RequestIPAddress + ":8734/CorService");
            ChannelFactory<ICorService> serviceFactory = new ChannelFactory<ICorService>(binding, address);
            ICorService serviceChanel = serviceFactory.CreateChannel();

            int lintTrackingID = Convert.ToInt32(lstrLastGeneratedCorr.Substring(lstrLastGeneratedCorr.LastIndexOf("-") + 1).Substring(0, lstrLastGeneratedCorr.LastIndexOf(".") - lstrLastGeneratedCorr.LastIndexOf("-") - 1));

            Dictionary<string, Object> data = new Dictionary<string, Object>();
            data.Add("FilePath", lstrLastGeneratedCorr);
            data.Add("ReadOnlyMode", lblnReadOnlyMode);
            data.Add("DefaultPrinter", lstrDefaultPrinter);
            data.Add("ShowPrintDialog", lblnShowPrintDialog);
            data.Add("UserID", UserID);
            data.Add("UserSerialID", UserSerialID);
            data.Add("TrackingID", lintTrackingID);

            // adding configuration for custom buttons on corr editor tool
            //FM upgrade: 6.0.0.29 changes
            //utlCorrToolCustomButton lutlCorrToolCustomButton1 = null;
            //utlCorrToolCustomButton lutlCorrToolCustomButton2 = null;
            //ModifyCustomButtonConfigurationForCorrTool(data, out lutlCorrToolCustomButton1, out lutlCorrToolCustomButton2);
            List<utlCorrToolCustomControl> llstCorrToolCustomControls = null;
            ModifyCustomButtonConfigurationForCorrTool(data, out llstCorrToolCustomControls);

            //Dictionary<string, object> CustomButtonInfo1 = new Dictionary<string, object>();
            //Dictionary<string, object> CustomButtonInfo2 = new Dictionary<string, object>();
            Dictionary<string, object> CustomButtonInfo = null;
            Dictionary<string, object> CustomCheckboxInfo = null;
            List<Dictionary<string, object>> llstCustomControlOptions = new List<Dictionary<string, object>>();
            utlCorrToolCustomButton lCustomButton;
            utlCorrToolCustomCheckbox lCustomCheckbox;

            //if (lutlCorrToolCustomButton1 != null)
            //{
            //    CustomButtonInfo1["iblnNeedToSaveData"] = lutlCorrToolCustomButton1.iblnNeedToSaveData;
            //    CustomButtonInfo1["iblnVisible"] = lutlCorrToolCustomButton1.iblnVisible;
            //    CustomButtonInfo1["istrSuccessMessage"] = lutlCorrToolCustomButton1.istrSuccessMessage;
            //    CustomButtonInfo1["istrText"] = lutlCorrToolCustomButton1.istrText;
            //    CustomButtonInfo1["iblnCloseAfterExecution"] = lutlCorrToolCustomButton1.iblnCloseAfterExecution;
            //    data.Add("CustomButton1", CustomButtonInfo1);
            //}

            //if (lutlCorrToolCustomButton2 != null)
            //{
            //    CustomButtonInfo2["iblnNeedToSaveData"] = lutlCorrToolCustomButton2.iblnNeedToSaveData;
            //    CustomButtonInfo2["iblnVisible"] = lutlCorrToolCustomButton2.iblnVisible;
            //    CustomButtonInfo2["istrSuccessMessage"] = lutlCorrToolCustomButton2.istrSuccessMessage;
            //    CustomButtonInfo2["istrText"] = lutlCorrToolCustomButton2.istrText;
            //    CustomButtonInfo2["iblnCloseAfterExecution"] = lutlCorrToolCustomButton2.iblnCloseAfterExecution;
            //    data.Add("CustomButton2", CustomButtonInfo2);
            //}

            if (llstCorrToolCustomControls != null && llstCorrToolCustomControls.Count > 0)
            {
                foreach (utlCorrToolCustomControl ct in llstCorrToolCustomControls)
                {
                    if (ct is utlCorrToolCustomButton)
                    {
                        lCustomButton = (utlCorrToolCustomButton)ct;
                        CustomButtonInfo = new Dictionary<string, object>();
                        CustomButtonInfo["istrControlType"] = "Button";
                        CustomButtonInfo["istrControlID"] = lCustomButton.istrControlID;
                        CustomButtonInfo["iblnNeedToSaveData"] = lCustomButton.iblnNeedToSaveData;
                        CustomButtonInfo["iblnVisible"] = lCustomButton.iblnVisible;
                        CustomButtonInfo["istrSuccessMessage"] = lCustomButton.istrSuccessMessage;
                        CustomButtonInfo["istrText"] = lCustomButton.istrText;
                        CustomButtonInfo["iblnCloseAfterExecution"] = lCustomButton.iblnCloseAfterExecution;
                        CustomButtonInfo["iblnDisableAfterExecution"] = lCustomButton.iblnDisableAfterExecution;
                        llstCustomControlOptions.Add(CustomButtonInfo);
                    }
                    else if (ct is utlCorrToolCustomCheckbox)
                    {
                        lCustomCheckbox = (utlCorrToolCustomCheckbox)ct;
                        CustomCheckboxInfo = new Dictionary<string, object>();
                        CustomCheckboxInfo["istrControlType"] = "Checkbox";
                        CustomCheckboxInfo["istrControlID"] = lCustomCheckbox.istrControlID;
                        CustomCheckboxInfo["iblnDefaultValue"] = lCustomCheckbox.iblnDefaultValue;
                        CustomCheckboxInfo["iblnVisible"] = lCustomCheckbox.iblnVisible;
                        CustomCheckboxInfo["istrText"] = lCustomCheckbox.istrText;
                        llstCustomControlOptions.Add(CustomCheckboxInfo);
                    }
                }
            }
            data.Add("CustomControls", llstCustomControlOptions);

            var IPnPath = lstrLastGeneratedCorr + "~" + RequestIPAddress;

            string lstrEncryptionKey = ControlsHelper2.GetEncryptionKey(HttpContext.Current.Session.SessionID);
            string lstrEncryptionIV = ControlsHelper2.GetEncryptionIV(HttpContext.Current.Session.SessionID);

            data.Add("MainSessionID", HttpContext.Current.Session.SessionID);

            string lstrEncryptedString = HelperFunction.SagitecEncryptAES(IPnPath, lstrEncryptionKey, lstrEncryptionIV);
            data.Add("EncryptedString", lstrEncryptedString);

            string lstrWebAPIURL = HttpContext.Current.Request.Url.ToString().Replace("EditCorrOnLocalTool2", "{0}");

            if (ConfigurationManager.AppSettings["HTTPSForCorrTool"] != null && ConfigurationManager.AppSettings["HTTPSForCorrTool"].ToString().ToLower() == "true")
            {
                if (!lstrWebAPIURL.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                {
                    lstrWebAPIURL = lstrWebAPIURL.Replace("http://", "https://");
                }
            }

            data.Add("WebAPIURL", lstrWebAPIURL);

            data.Add("Base64String", lBase64String);
            Dictionary<string, string> Headers = new Dictionary<string, string>();
            data.Add("Headers", Headers);

            var lSerializeData = HelperFunction.SerializeObject(data);
            try
            {
                bool result = serviceChanel.OpenFileWithBase64String(lSerializeData);
            }
            catch (Exception e)
            {
                Context.Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                if (e.Message.Contains("No connection could be made because the target machine actively refused"))
                {
                    return "Error occured while opening correspondence file." + "Correspondence editor service is not running.";
                }
                else
                {
                    return HandleGlobalError(e);
                }
            }

            Context.Response.StatusCode = (int)System.Net.HttpStatusCode.OK;
            return "Correspondence is opened in local tool.";

        }
        catch (Exception e)
        {
            Context.Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
            return HandleGlobalError(e);
        }
    }

    [ScriptMethod]
    [WebMethod(EnableSession = true)]
    public Dictionary<string, Object> EditCorrOnLocalTool(object aParams)
    {
        Dictionary<string, Object> data = new Dictionary<string, Object>();
        try
        {
            Dictionary<string, object> AllParams = (Dictionary<string, object>)aParams;
            string lHttpResponseMessage = null;

            string lstrLastGeneratedCorr = "";
            //lstrLastGeneratedCorr = (string)HttpContext.Current.Session[istrFWN + "CorrFileName"];
            lstrLastGeneratedCorr = (string)HttpContext.Current.Session[Framework.istrWindowName + "CorrFileName"];
            //check null for file.
            if (string.IsNullOrEmpty(lstrLastGeneratedCorr))
            {

                lHttpResponseMessage = "File is not generated.";
                Context.Response.StatusCode = (int)System.Net.HttpStatusCode.OK;
                data["Message"] = lHttpResponseMessage;
                return data;
            }

            //Get Default Printer
            string lstrDefaultPrinter = AllParams["DefaultPrinter"].ToString();
            //bool lblnReadOnlyMode = (string)HttpContext.Current.Session[istrFWN + "LastCorrSecurityLevel"] == "1";
            bool lblnReadOnlyMode = (string)HttpContext.Current.Session[Framework.istrWindowName + "LastCorrSecurityLevel"] == "1";
            bool lblnShowPrintDialog = AllParams["ShowPrintDialog"].ToString().ToLower() == "true";

            idictParams[utlConstants.istrSessionID] = Session.SessionID;
            //idictParams[utlConstants.istrWindowName] = istrFWN;
            idictParams[utlConstants.istrWindowName] = Framework.istrWindowName;
            idictParams[utlConstants.istrParentForm] = "CenterMiddle";
            //idictParams[utlConstants.istrConstUserID] = Session[istrFWN + "UserID"];
            idictParams[utlConstants.istrConstUserID] = Session[Framework.istrWindowName + "UserID"];
            //idictParams[utlConstants.istrConstUserSerialID] = Session[istrFWN + "UserSerialID"];
            idictParams[utlConstants.istrConstUserSerialID] = Session[Framework.istrWindowName + "UserSerialID"];

            ArrayList Result = isrvBusinessTier.EditCorrOnLocalTool(lstrLastGeneratedCorr, idictParams);

            if (Result.Count > 0 && Result[0] is utlError)
            {
                lHttpResponseMessage += ((utlError)Result[0]).istrErrorMessage;
                Context.Response.StatusCode = (int)System.Net.HttpStatusCode.OK;
                data["Message"] = lHttpResponseMessage;
                return data;
            }

            string UserID = idictParams[utlConstants.istrConstUserID].ToString();
            int UserSerialID = Convert.ToInt32(idictParams[utlConstants.istrConstUserSerialID].ToString());
            string RequestIPAddress = GetIP();
            int lintTrackingID = 0;
            //Ticket#121821
             lintTrackingID = Convert.ToInt32(lstrLastGeneratedCorr.Substring(lstrLastGeneratedCorr.LastIndexOf("-") + 1).Substring(0, 4));

            data.Add("RequestIPAddress", RequestIPAddress);
            data.Add("FilePath", lstrLastGeneratedCorr);
            data.Add("ReadOnlyMode", lblnReadOnlyMode);
            data.Add("DefaultPrinter", lstrDefaultPrinter);
            data.Add("ShowPrintDialog", lblnShowPrintDialog);
            data.Add("UserID", UserID);
            data.Add("UserSerialID", UserSerialID);
            data.Add("TrackingID", lintTrackingID);
            //Current Form
            //data.Add("FormID", Session[istrFWN + "CurrentForm"].ToString());
            data.Add("FormID", Session[Framework.istrWindowName + "CurrentForm"].ToString());

            var IPnPath = lstrLastGeneratedCorr + "~" + RequestIPAddress;
            string lstrEncryptionKey = ControlsHelper2.GetEncryptionKey(HttpContext.Current.Session.SessionID);
            string lstrEncryptionIV = ControlsHelper2.GetEncryptionIV(HttpContext.Current.Session.SessionID);
            string lstrEncryptedString = HelperFunction.SagitecEncryptAES(IPnPath, lstrEncryptionKey, lstrEncryptionIV);
            data.Add("EncryptedString", lstrEncryptedString);

            string lstrWebAPIURL = HttpContext.Current.Request.Url.ToString().Replace("EditCorrOnLocalTool", "{0}");
            data.Add("WebAPIURL", lstrWebAPIURL);

            data.Add("MainSessionID", HttpContext.Current.Session.SessionID);

            string lBase64String = Result[0].ToString();
            data.Add("Base64String", SplitBy(lBase64String,32000));

            Dictionary<string, string> Headers = new Dictionary<string, string>();
            data.Add("Headers", Headers);

            Context.Response.StatusCode = (int)System.Net.HttpStatusCode.OK;
            data["Message"] = "Correspondence is opened in local tool.";

            //Get Send button visibilty.
            data.Add("ShowPrintButton", true);

            // adding configuration for custom buttons on corr editor tool
            ////List<utlCorrToolCustomControl> llstCorrToolCustomControls = null;
            ////ModifyCustomButtonConfigurationForCorrTool(data, out llstCorrToolCustomControls);
            //Dictionary<string, object> CustomButtonInfo = null;
            //Dictionary<string, object> CustomCheckboxInfo = null;

            //List<Dictionary<string, object>> llstCustomControlOptions = new List<Dictionary<string, object>>();
            //utlCorrToolCustomButton lCustomButton;
            //utlCorrToolCustomCheckbox lCustomCheckbox;
            //if (llstCorrToolCustomControls != null && llstCorrToolCustomControls.Count > 0)
            //{
            //    foreach (utlCorrToolCustomControl ct in llstCorrToolCustomControls)
            //    {
            //        if (ct is utlCorrToolCustomButton)
            //        {
            //            lCustomButton = (utlCorrToolCustomButton)ct;
            //            CustomButtonInfo = new Dictionary<string, object>();
            //            CustomButtonInfo["istrControlType"] = "Button";
            //            CustomButtonInfo["istrControlID"] = lCustomButton.istrControlID;
            //            CustomButtonInfo["iblnNeedToSaveData"] = lCustomButton.iblnNeedToSaveData;
            //            CustomButtonInfo["iblnVisible"] = lCustomButton.iblnVisible;
            //            CustomButtonInfo["istrSuccessMessage"] = lCustomButton.istrSuccessMessage;
            //            CustomButtonInfo["istrText"] = lCustomButton.istrText;
            //            CustomButtonInfo["iblnCloseAfterExecution"] = lCustomButton.iblnCloseAfterExecution;
            //            CustomButtonInfo["iblnDisableAfterExecution"] = lCustomButton.iblnDisableAfterExecution;
            //            llstCustomControlOptions.Add(CustomButtonInfo);
            //        }
            //        else if (ct is utlCorrToolCustomCheckbox)
            //        {
            //            lCustomCheckbox = (utlCorrToolCustomCheckbox)ct;
            //            CustomCheckboxInfo = new Dictionary<string, object>();
            //            CustomCheckboxInfo["istrControlType"] = "Checkbox";
            //            CustomCheckboxInfo["istrControlID"] = lCustomCheckbox.istrControlID;
            //            CustomCheckboxInfo["iblnDefaultValue"] = lCustomCheckbox.iblnDefaultValue;
            //            CustomCheckboxInfo["iblnVisible"] = lCustomCheckbox.iblnVisible;
            //            CustomCheckboxInfo["istrText"] = lCustomCheckbox.istrText;
            //            llstCustomControlOptions.Add(CustomCheckboxInfo);
            //        }
            //    }
            //}
            //data.Add("CustomControls", llstCustomControlOptions);
            return data;

        }
        catch (Exception e)
        {
            Context.Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
            data["Message"] = HandleGlobalError(e);
            return data;
        }
    }

    #endregion


    public static IEnumerable<string> SplitBy(string str, int chunkLength)
    {
        if (String.IsNullOrEmpty(str)) throw new ArgumentException();
        if (chunkLength < 1) throw new ArgumentException();

        for (int i = 0; i < str.Length; i += chunkLength)
        {
            if (chunkLength + i > str.Length)
                chunkLength = str.Length - i;

            yield return str.Substring(i, chunkLength);
        }
    }
}
