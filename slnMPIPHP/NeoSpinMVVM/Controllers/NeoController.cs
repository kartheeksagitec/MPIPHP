#region [Using directives]
using NeoBase.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OnDemandAVScan;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.Interface;
using Sagitec.MVVMClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Script.Serialization;
using System.Web.Security;
using System.Xml.Linq;
#endregion [Using directives]

namespace Neo.Controllers
{
    /// <summary>
    /// Class Neo.NeoController
    /// </summary>
    public partial class NeoController : ApiControllerBase
    {
        #region[FS002 START- Administer System related Functions]

        /// <summary>
        /// Refresh All App Servers
        /// </summary>
        /// <returns>Refresh Servers</returns>
        [HttpGet]
        public HttpResponseMessage RefreshAllAppServers()
        {
            string lstrAstrRefreshAll = String.Empty;

            try
            {
                lstrAstrRefreshAll = Convert.ToString(ConfigurationManager.AppSettings["RefreshAll"]);
            }
            catch (Exception ex)
            {
                HandleGlobalError(ex);
            }
            if (!string.IsNullOrEmpty(lstrAstrRefreshAll) && lstrAstrRefreshAll == "true")
            {
                // return RefreshAllServers(); Method Missing in srvAdmin ---PIR 33649.
                return this.RefreshServers();
            }
            else
            {
                return this.RefreshServers();
            }
        }

        /// <summary>
        /// Refresh All Servers
        /// </summary>
        /// <returns>HttpResponseMessage</returns>
        private HttpResponseMessage RefreshAllServers()
        {
            HttpResponseMessage lResult;

            try
            {
                ViewModelObj lVm = new ViewModelObj();
                string lstrForm = "wfmSystemManagementMaintenance";
                isrvServers.ConnectToBT(lstrForm);
                var lIdictParams = SetParams(lstrForm);

                try
                {
                    ArrayList larrResult = new ArrayList();
                    Hashtable lhstParamValues = new Hashtable();
                    larrResult = (ArrayList)isrvServers.isrvBusinessTier.ExecuteMethod("RefreshAllServers", lhstParamValues, false, lIdictParams);

                    if (larrResult != null && larrResult.Count > 0 && larrResult[0] is utlError)
                    {
                        lVm.ValidationSummary = new ArrayList
                        {
                            (utlError)larrResult[0]
                        };
                        lVm.ResponseMessage = new utlResponseMessage
                        {
                            istrMessage = "Error while refreshing servers."
                        };
                    }
                    else
                    {
                        foreach (string lstrKey in ConfigurationManager.AppSettings.AllKeys)
                        {
                            SystemSettings.Instance.Configuration[lstrKey] = ConfigurationManager.AppSettings[lstrKey];
                        }
                        SystemSettings.Instance.SetFields();

                        lVm.ResponseMessage = new utlResponseMessage
                        {
                            istrMessage = "Servers refreshed successfully."
                        };
                    }
                }
                catch
                {
                    lVm.ValidationSummary = new ArrayList
                    {
                        "Unable to refresh servers."
                    };
                    lVm.ResponseMessage = new utlResponseMessage
                    {
                        istrMessage = "Error while refreshing servers."
                    };
                }

                lVm.ExtraInfoFields["FormId"] = "wfmSystemManagementMaintenance";
                lResult = Request.CreateResponse(HttpStatusCode.OK, lVm);
            }
            catch (Exception exception)
            {
                lResult = HandleGlobalError(exception, null, null, "RefreshAllAppServers");
            }
            return lResult;
        }

        #endregion[FS002 END- Administer System related Functions]

        #region[FS006 START- Communication related Functions]

        /// <summary>
        /// Overidden method to configure customized buttons on correspondence editor.
        /// </summary>
        /// <param name="adictCorrInfo">adictCorrInfo</param>   
        /// <param name="alstCorrToolCustomControls">alstCorrToolCustomControls</param> 
        //public override void ModifyCustomButtonConfigurationForCorrTool(Dictionary<string, object> adictCorrInfo, out List<utlCorrToolCustomControl> alstCorrToolCustomControls)
        //{
        //    alstCorrToolCustomControls = new List<utlCorrToolCustomControl>();

        //    try
        //    {
        //        Dictionary<string, object> ldictParams = SetParams("wfmLogin");
        //        isrvServers.ConnectToBT(string.Empty);
        //        int lintCorTrackingId = Convert.ToInt32(adictCorrInfo["TrackingID"]);

        //        Hashtable lhstParams = new Hashtable
        //        {
        //            { "aintCommTrackingID", lintCorTrackingId },
        //            { "adictParams", MVVMHelperFunctions.ToStringDictionary(adictCorrInfo) }
        //        };

        //        Hashtable lhstResult = (Hashtable)isrvServers.isrvBusinessTier.ExecuteMethod("GetGeneratedCommunicationInfo", lhstParams, false, ldictParams);
        //        string lstrDelivaryMethod = Convert.ToString(lhstResult["DeliveryMethod"]);
        //        string lstrEmailExists = Convert.ToString(lhstResult["EmailExists"]);

        //        // Save for later checkbox           
        //        utlCorrToolCustomCheckbox lutlCustomSaveForLater = new utlCorrToolCustomCheckbox
        //        {
        //            istrControlID = "chkSaveForLater",
        //            istrText = "Save For Later",
        //            iblnVisible = true
        //        };

        //        // Do Not Send to Communication Engine
        //        utlCorrToolCustomCheckbox lutlCustomDoNotSendToCommEngine = new utlCorrToolCustomCheckbox
        //        {
        //            istrControlID = "chkDoNotSendToCommEngine",
        //            istrText = "Do Not Send to Communication Engine",
        //            iblnVisible = true,
        //            iblnDefaultValue = false
        //        };

        //        // Close button
        //        utlCorrToolCustomButton lutlCorrCustomClose = new utlCorrToolCustomButton
        //        {
        //            istrControlID = "btnCloseCorrespondence",
        //            istrText = "Close",
        //            iblnVisible = true,
        //            iblnCloseAfterExecution = true,
        //            iblnNeedToSaveData = true,
        //            istrSuccessMessage = "Communication Closed Successfully."
        //        };

        //        // Finish button
        //        utlCorrToolCustomButton lutlCorrCustomFinishButton = new utlCorrToolCustomButton
        //        {
        //            istrControlID = "btnCompleteCorrespondence",
        //            istrText = "Finish",
        //            istrSuccessMessage = "Communication Finished Successfully.",
        //            iblnVisible = true,
        //            iblnCloseAfterExecution = true,
        //            iblnNeedToSaveData = true
        //        };

        //        // Void Button
        //        utlCorrToolCustomButton lutlVoidCommunication = new utlCorrToolCustomButton
        //        {
        //            istrControlID = "btnVoidCommunication",
        //            istrText = "Void",
        //            istrSuccessMessage = "Communication Voided Successfully.",
        //            iblnVisible = true,
        //            iblnCloseAfterExecution = true,
        //            iblnNeedToSaveData = true
        //        };

        //        // Do Not Send to Communication Engine
        //        utlCorrToolCustomCheckbox lutlUploadToEcm = new utlCorrToolCustomCheckbox
        //        {
        //            istrControlID = "chkUploadToEcm",
        //            istrText = "Image",
        //            iblnVisible = true,
        //            iblnDefaultValue = false,
        //        };

        //        // Show/Hide Print Button in Correspondence Editor Tool. [Values : true/false]
        //        // adictCorrInfo.Add("ShowPrintButton", false);

        //        // Add SaveAs Button in Correspondence Editor Tool. [Values : true/false]
        //        // adictCorrInfo.Add("AllowSaveAs", true);

        //        // Change SaveAs Button Text in Correspondence Editor Tool.
        //        // adictCorrInfo.Add("SaveAsText", "Save As NeoBase");

        //        // Close Correspondence Editor Tool after SaveAs Button Complete it's functionality. [Values : true/false]
        //        // adictCorrInfo.Add("CloseAfterSaveAs", true);

        //        // Change PrintSuccessMessage and Add Custom Message.
        //        // adictCorrInfo.Add("PrintSuccessMessage", "");

        //        // Change Default SaveSuccessMessage and Add Custom Message.
        //        // adictCorrInfo.Add("SaveSuccessMessage", "Tanaji");

        //        // Decide Sequence of Buttons.
        //        // adictCorrInfo.Add("DefaultButtonSequence", "save,print,saveas");

        //        // Change Buttons Flow Direction. [Values : LeftToRight/RightToLeft]
        //        // adictCorrInfo.Add("CustomButtonFlowDirection", "RightToLeft");

        //        string lstrTemplateType = string.Empty;
        //        if (adictCorrInfo.ContainsKey("TemplateName"))
        //        {
        //            lstrTemplateType = Convert.ToString(adictCorrInfo["TemplateName"]).Split(";")[0];
        //        }

        //        if (lstrTemplateType == "P")
        //        {
        //            alstCorrToolCustomControls.Add(lutlCustomSaveForLater);
        //            alstCorrToolCustomControls.Add(lutlCustomDoNotSendToCommEngine);

        //            lhstParams = new Hashtable
        //            {
        //                { "aintCommTrackingID", lintCorTrackingId }
        //            };

        //            bool lblnResult = (bool)isrvServers.isrvBusinessTier.ExecuteMethod("IsCorrECMEnabled", lhstParams, false, ldictParams);

        //            if (lblnResult)
        //            {
        //                alstCorrToolCustomControls.Add(lutlUploadToEcm);
        //            }

        //            alstCorrToolCustomControls.Add(lutlVoidCommunication);
        //            alstCorrToolCustomControls.Add(lutlCorrCustomFinishButton);
        //        }

        //        alstCorrToolCustomControls.Add(lutlCorrCustomClose);
        //    }
        //    catch (Exception ex)
        //    {
        //        HandleGlobalError(ex);
        //    }
        //}
        
        /// <summary>
        /// Get generated communication information
        /// </summary>
        /// <param name="aParams">aParams</param>
        /// <returns>HttpResponseMsg</returns>
        [HttpPost]
        public HttpResponseMessage GetGeneratedCommunicationInfo(Dictionary<string, string> aParams)
        {
            HttpResponseMessage lHttpResponseMsg = new HttpResponseMessage();

            if (isrvServers.IsNull())
            {
                isrvServers = new srvServers();
            }

            try
            {
                isrvServers.ConnectToBT("wfmLogin");

                int lintCorTrackingId = 0;
                if (aParams.ContainsKey("FileName"))
                {
                    lintCorTrackingId = UiHelperFunction.DecryptFilePath(Convert.ToString(aParams["FileName"]));
                }
                else
                {

                }

                Hashtable lhstParams = new Hashtable
            {
                { "aintCommTrackingID", lintCorTrackingId },
                { "adictParams", aParams }
            };
                Dictionary<string, object> ldictParams = SetParams("wfmLogin");

                Hashtable lhstResult = (Hashtable)isrvServers.isrvBusinessTier.ExecuteMethod("GetGeneratedCommunicationInfo", lhstParams, false, ldictParams);
                lHttpResponseMsg = Request.CreateResponse(HttpStatusCode.OK, lhstResult);
            }
            catch (Exception ex)
            {
                HandleGlobalError(ex);
            }

            return lHttpResponseMsg;
        }

        /// <summary>
        /// Get Communication Template Information
        /// </summary>
        /// <param name="aParams">aParams</param>
        /// <returns>HttpResponseMsg</returns>
        [HttpPost]
        public HttpResponseMessage GetCommTemplateInfo(Dictionary<string, string> aParams)
        {
            if (isrvServers.IsNotNull())
            {
                isrvServers = new srvServers();
            }
            else
            {

            }

            isrvServers.ConnectToBT();

            int lintCorTrackingId = 0;
            if (aParams.ContainsKey("FileName"))
            {
                lintCorTrackingId = UiHelperFunction.DecryptFilePath(Convert.ToString(aParams["FileName"]));
            }
            else
            {

            }

            Hashtable lhstParams = new Hashtable
            {
                { "aintCorTrackingID", lintCorTrackingId }
            };

            Dictionary<string, object> ldictParams = SetParams("wfmLogin");
            List<string> lhstResult = (List<string>)isrvServers.isrvBusinessTier.ExecuteMethod("GetCommTemplateInfo", lhstParams, false, ldictParams);
            HttpResponseMessage lHttpResponseMsg = new HttpResponseMessage();
            lHttpResponseMsg = Request.CreateResponse(HttpStatusCode.OK, lhstResult);

            return lHttpResponseMsg;
        }

        /// <summary>
        /// Get Generated Communication info
        /// </summary>
        /// <param name="adictParams"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage GetGeneratedCorrInfo(Dictionary<string, string> adictParams)
        {
            int lintCorTrackingId = 0;

            if (isrvServers == null)
                isrvServers = new srvServers();

            isrvServers.ConnectToBT();

            if (adictParams.Count > 0)
            {
                if (adictParams.ContainsKey("FileName"))
                {
                    lintCorTrackingId = UiHelperFunction.DecryptFilePath(Convert.ToString(adictParams["FileName"]));
                }
            }

            Hashtable lhstParams = new Hashtable
            {
                { "aintCorTrackingID", lintCorTrackingId }
            };

            Dictionary<string, object> ldictParams = SetParams("wfmLogin");
            List<string> lhstResult = (List<string>)isrvServers.isrvBusinessTier.ExecuteMethod("GetGeneratedCorrInfo", lhstParams, false, ldictParams);
            HttpResponseMessage lHttpResponseMsg = new HttpResponseMessage();
            lHttpResponseMsg = Request.CreateResponse(HttpStatusCode.OK, lhstResult);

            return lHttpResponseMsg;
        }

        /// <summary>
        /// Finish Non Editable Correspondence
        /// </summary>
        /// <param name="aParams">aParams</param>
        /// <returns>HttpResponseMsg</returns>
        [HttpPost]
        public HttpResponseMessage FinishNonEditableCorrespondence(Dictionary<string, string> aParams)
        {
            isrvServers = isrvServers.IsNull() ? new srvServers() : isrvServers;
            isrvServers.ConnectToBT("wfmCorrespondenceClientMVVM");

            int lintCorTrackingId = 0;
            if (aParams.ContainsKey("FileName"))
            {
                lintCorTrackingId = UiHelperFunction.DecryptFilePath(Convert.ToString(aParams["FileName"]));
            }
            else
            {
            }

            Hashtable lhstResult = new Hashtable();

            Hashtable lhstParams = new Hashtable
            {
                { "aintCommTrackingID", lintCorTrackingId }
            };

            HttpResponseMessage lHttpResponseMsg = new HttpResponseMessage();

            try
            {
                Dictionary<string, object> ldictParams = SetParams("wfmCorrespondenceClientMVVM");
                int lintMessageId = 0;

                if (aParams.ContainsKey("Action"))
                {
                    switch (Convert.ToString(aParams["Action"]))
                    {
                        case "VOID":
                            lintMessageId = (int)isrvServers.isrvBusinessTier.ExecuteMethod("VoidCommunication", lhstParams, false, ldictParams);
                            break;

                        case "FINISH":
                            if (aParams.ContainsKey("FileName"))
                            {
                                string lstrEncryptionKey = ControlsHelper2.GetEncryptionKey(HttpContext.Current.Session.SessionID);
                                string lstrEncryptionIV = ControlsHelper2.GetEncryptionIV(HttpContext.Current.Session.SessionID);
                                string lstrFilePath = HelperFunction.SagitecDecryptFIPS(Convert.ToString(aParams["FileName"]), lstrEncryptionKey, lstrEncryptionIV);
                                aParams.Add("DecryptedFilePath", lstrFilePath);
                            }
                            else
                            {
                            }

                            lhstParams.Add("adictParams", aParams);
                            lhstResult = (Hashtable)isrvServers.isrvBusinessTier.ExecuteMethod("SendCommunicationMethod", lhstParams, false, ldictParams);
                            lintMessageId = lhstResult.ContainsKey("Success") ? (int)lhstResult["Success"] : 0;
                            break;

                        case "ECM":
                            lintMessageId = (int)isrvServers.isrvBusinessTier.ExecuteMethod("UploadCorrespondenceToEcm", lhstParams, false, ldictParams);
                            break;

                        default:
                            break;
                    }
                }
                else
                {
                }

                switch (lintMessageId)
                {
                    case 0:
                        lhstResult.Add("Message", "Communication Failed to send.");
                        break;
                    case 1:
                        lhstResult.Add("Message", "Communication Finished successfully.");
                        break;
                    case 2:
                        lhstResult.Add("Message", "Communication Voided successfully.");
                        break;
                    case 3:
                        lhstResult.Add("Message", "Communication Imaged successfully.");
                        lhstResult.Add("IndexEcm", true);
                        break;
                    case 4:
                        lhstResult.Add("Message", "Failed to Image Communication.");
                        lhstResult.Add("IndexEcm", false);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                HandleGlobalError(ex);
            }

            lHttpResponseMsg = Request.CreateResponse(HttpStatusCode.OK, lhstResult);

            return lHttpResponseMsg;
        }

        /// <summary>
        /// Get Decrypted File Path
        /// </summary>
        /// <param name="aParams">aParams</param>
        /// <returns>HttpResponseMsg</returns>
        [HttpPost]
        public HttpResponseMessage GetDecryptedFilePath(Dictionary<string, string> aParams)
        {
            int lintTrackingID = 0;
            if (aParams.ContainsKey("FileName"))
            {
                lintTrackingID = UiHelperFunction.DecryptFilePath(Convert.ToString(aParams["FileName"]));
            }
            else if (aParams.ContainsKey("FileId"))
            {
                string lstrEncryptionKey = ControlsHelper2.GetEncryptionKey(HttpContext.Current.Session.SessionID);
                string lstrEncryptionIV = ControlsHelper2.GetEncryptionIV(HttpContext.Current.Session.SessionID);
                string lstrFilePath = HelperFunction.SagitecDecryptFIPS(Convert.ToString(aParams["FileId"]), lstrEncryptionKey, lstrEncryptionIV);

                if (lstrFilePath != null)
                {
                    if (lstrFilePath.Contains(";"))
                    {
                        lstrFilePath = lstrFilePath.Split(";")[1];
                    }
                    int.TryParse(lstrFilePath, out lintTrackingID);
                }
            }
            HttpResponseMessage lHttpResponseMsg = new HttpResponseMessage();
            lHttpResponseMsg = Request.CreateResponse(HttpStatusCode.OK, lintTrackingID);

            return lHttpResponseMsg;
        }

        /// <summary>
        /// Reports Related Functions
        /// Insert Report Request
        /// </summary>
        /// <param name="adictParams"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage InsertReportRequest(Dictionary<string, string> adictParams)
        {
            if (isrvServers == null)
            {
                isrvServers = new srvServers();
            }

            isrvServers.ConnectToBT("wfmReportClientMVVM");

            Hashtable lhstReportParams = new Hashtable();

            if (adictParams.TryGetValue("ReportName", out string lstrReportName))
            {
                lhstReportParams.Add("astrReportName", lstrReportName);
                adictParams.Remove("ReportName");
            }

            if (adictParams.TryGetValue("ReportEmailID", out string lstrEmailAddress))
            {
                lhstReportParams.Add("astrEmailAddress", lstrEmailAddress);
                adictParams.Remove("ReportEmailID");
            }

            lhstReportParams.Add("adictReportParameters", adictParams);
            Dictionary<string, object> ldictParams = SetParams("wfmReportClientMVVM");
            Hashtable lhstResult = (Hashtable)isrvServers.isrvBusinessTier.ExecuteMethod("InsertReportRecord", lhstReportParams, false, ldictParams);

            HttpResponseMessage lhttpResponseMessage = new HttpResponseMessage();
            lhttpResponseMessage = Request.CreateResponse(HttpStatusCode.OK, lhstResult);

            return lhttpResponseMessage;
        }

        /// <summary>
        /// Get Report Detail
        /// </summary>
        /// <param name="adictParams"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage GetReportDetail(Dictionary<string, string> adictParams)
        {
            if (isrvServers == null)
            {
                isrvServers = new srvServers();
            }

            isrvServers.ConnectToBT("wfmReportClientMVVM");

            Hashtable lhstReportParams = new Hashtable();

            if (adictParams.TryGetValue("ReportName", out string lstrReportName))
            {
                lhstReportParams.Add("astrReportName", lstrReportName);
            }

            Dictionary<string, object> ldictParams = SetParams("wfmReportClientMVVM");
            Hashtable lhstResult = (Hashtable)isrvServers.isrvBusinessTier.ExecuteMethod("GetReportDetail", lhstReportParams, false, ldictParams);

            HttpResponseMessage lHttpResponseMessage = new HttpResponseMessage();
            lHttpResponseMessage = Request.CreateResponse(HttpStatusCode.OK, lhstResult);
            return lHttpResponseMessage;
        }

        /// <summary>
        /// Get Corr Enclosure Info
        /// </summary>
        /// <param name="adictParams"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage GetCorrEnclosureInfo(Dictionary<string, string> adictParams)
        {
            if (isrvServers == null)
            {
                isrvServers = new srvServers();
            }

            isrvServers.ConnectToBT();

            Hashtable lhstParams = new Hashtable();

            lhstParams.Add("adictCorrParams", adictParams);

            Dictionary<string, object> ldictParams = SetParams("wfmReportClientMVVM");
            Hashtable lhstResult = (Hashtable)isrvServers.isrvBusinessTier.ExecuteMethod("GetCorrEnclosureInfo", lhstParams, false, ldictParams);

            HttpResponseMessage httpResponseMessage = new HttpResponseMessage();
            httpResponseMessage = Request.CreateResponse(HttpStatusCode.OK, lhstResult);

            return httpResponseMessage;
        }

        #endregion [FS006 END- Communication related Functions]

        [HttpPost]
        public string FindPersonFromSSNAndMPID(Dictionary<string, object> adictParam)
        {           
            string astrSSN = adictParam["astrSsn"].ToString();
            string astrMPID = adictParam["astrMPID"].ToString();
            string lstrNewPersonMPID = string.Empty;

            Dictionary<string, object> ldictParams = (Dictionary<string, object>)iobjSessionData["dictParams"];
            string lstrUrl = string.Format(ConfigurationManager.AppSettings["BusinessTierUrl"], "srvPerson");
            IBusinessTier lsrvBusinessTier = WCFClient<IBusinessTier>.CreateChannel(lstrUrl);

            Hashtable lhstParam = new Hashtable();

            DataTable ldtbPersonInfo = new DataTable();
            if (astrSSN.IsNotNullOrEmpty())
            {
                astrSSN = astrSSN.Replace("-", "");
                
                lhstParam.Add(MPIPHP.DataObjects.enmSsnMergeHistory.old_ssn.ToString().ToUpper(), astrSSN);
                ldtbPersonInfo = lsrvBusinessTier.ExecuteQuery("cdoSsnMergeHistory.GetOldPersonBySSN", lhstParam, ldictParams);
            }
            else if (astrMPID.IsNotNullOrEmpty())
            {

                lhstParam.Add(MPIPHP.DataObjects.enmSsnMergeHistory.old_mpi_person_id.ToString(), astrMPID);
                ldtbPersonInfo = lsrvBusinessTier.ExecuteQuery("cdoSsnMergeHistory.GetOldPersonByMPID", lhstParam, ldictParams);
            }

            if (ldtbPersonInfo != null && ldtbPersonInfo.Rows.Count > 0)
            {
                if ((ldtbPersonInfo.Rows[0][MPIPHP.DataObjects.enmSsnMergeHistory.new_mpi_person_id.ToString()]).ToString().IsNotNullOrEmpty())
                    lstrNewPersonMPID = Convert.ToString(ldtbPersonInfo.Rows[0][MPIPHP.DataObjects.enmSsnMergeHistory.new_mpi_person_id.ToString()]);
             
            }
            return lstrNewPersonMPID;
        }

        [HttpPost]
        public string InitiateServiceRetirement(Dictionary<string, object> adictParam)
        {
            int aintPersonId =Convert.ToInt32(adictParam["PersonId"]);
            int aintPersonAccountId = Convert.ToInt32(adictParam["PersonAccountId"]);
            string astrControlID = Convert.ToString(adictParam["ControlId"]);

            Dictionary<string, object> ldictParams = (Dictionary<string, object>)iobjSessionData["dictParams"];
            string lstrUrl = string.Format(ConfigurationManager.AppSettings["BusinessTierUrl"], "srvPerson");
            IBusinessTier lsrvBusinessTier = WCFClient<IBusinessTier>.CreateChannel(lstrUrl);

            Hashtable lhstParam = new Hashtable();
            lhstParam.Add("aintPersonId", aintPersonId);
            lhstParam.Add("aintPersonAccountId", aintPersonAccountId);
            lhstParam.Add("astrControlId", astrControlID);
            int ldtbPersonInfo = (int)lsrvBusinessTier.ExecuteMethod("GetActivityInstanceCount", lhstParam, false, ldictParams);

            if (ldtbPersonInfo > 0)
            {
                return "This BPM process already exists for this MPID and plan";
            }
            return string.Empty;
        }
        [HttpPost]
        public void CancelServiceRetirement(Dictionary<string, object> adictParam)
        {
            int aintPersonId = Convert.ToInt32(adictParam["PersonId"]);
            int aintPersonAccountId = Convert.ToInt32(adictParam["PersonAccountId"]);
            string astrControlID = Convert.ToString(adictParam["ControlId"]);

            Dictionary<string, object> ldictParams = (Dictionary<string, object>)iobjSessionData["dictParams"];
            string lstrUrl = string.Format(ConfigurationManager.AppSettings["BusinessTierUrl"], "srvPerson");
            IBusinessTier lsrvBusinessTier = WCFClient<IBusinessTier>.CreateChannel(lstrUrl);

            Hashtable lhstParam = new Hashtable();
            lhstParam.Add("aintPersonId", aintPersonId);
            lhstParam.Add("aintPersonAccountId", aintPersonAccountId);
            lhstParam.Add("astrControlId", astrControlID);
            int ldtbPersonInfo = (int)lsrvBusinessTier.ExecuteMethod("CancelActivityInstance", lhstParam, false, ldictParams);
        }

        [HttpPost]
        public void SetActivityInstanceRefrenceId(Dictionary<string, object> adictParam)
        {
            int aintActivityInstanceRefrenceID = Convert.ToInt32(adictParam["ActivityInstanceRefrenceId"]);

            Dictionary<string, object> ldictParams = (Dictionary<string, object>)iobjSessionData["dictParams"];
            if (ldictParams.ContainsKey("aintActivityInstanceRefrenceId"))
            {
                ldictParams.Remove("aintActivityInstanceRefrenceId");

            }
            ldictParams.Add("aintActivityInstanceRefrenceId", aintActivityInstanceRefrenceID);
        }
            [HttpPost]
        public string GetPlanExists(object Params)
        {
            try
            {
                Dictionary<string, object> AllParams = null;
                var jss = new JavaScriptSerializer();
                AllParams = jss.Deserialize<Dictionary<string, object>>(Params.ToString());
                string astrPlanId = AllParams["astrPlanId"]?.ToString();
                string astrBenType = AllParams["astrBenType"]?.ToString();
                if (astrPlanId != 9.ToString())
                {
                    string lstrResult = "100";

                    isrvServers.ConnectToBT(istrSenderForm);

                    int aintPrimaryKey = 0;
                    if (Convert.ToString(AllParams["aintPrimaryKey"]).IsNotNullOrEmpty())
                    {
                        aintPrimaryKey = Convert.ToInt32(AllParams["aintPrimaryKey"]);
                    }
                    Hashtable lhshTable = new Hashtable();
                    lhshTable.Add("astrFormName", istrSenderForm);
                    lhshTable.Add("aintPrimaryKey", aintPrimaryKey);
                    Dictionary<string, object> ldictParams = (Dictionary<string, object>)iobjSessionData["dictParams"];

                    MPIPHP.BusinessObjects.busPersonBeneficiary lobjMain = (MPIPHP.BusinessObjects.busPersonBeneficiary)isrvServers.isrvBusinessTier.ExecuteMethod("GetBusObjectFromDB", lhshTable, false, ldictParams);

                    if (lobjMain is MPIPHP.BusinessObjects.busPersonBeneficiary)
                    {
                        MPIPHP.BusinessObjects.busPersonBeneficiary lbusPersonBeneficiary = (MPIPHP.BusinessObjects.busPersonBeneficiary)lobjMain;
                        foreach (MPIPHP.BusinessObjects.busPersonAccountBeneficiary lbusPersonAccountBeneficiary in lbusPersonBeneficiary.iclbPersonAccountBeneficiary)
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
                            foreach (MPIPHP.BusinessObjects.busPersonAccountBeneficiary lbusPersonAccountBeneficiary in lbusPersonBeneficiary.iclbPersonAccountBeneficiariesAll)
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

        [HttpPost]
        public ArrayList ChangeBenefitOption(object Params)
        {
            IBusinessTier isrvNeoSpinMSSBusinessTier = null; ArrayList lArrayList = null;
            try
            {
                Dictionary<string, object> AllParams = null; 
                var jss = new JavaScriptSerializer();
                AllParams = jss.Deserialize<Dictionary<string, object>>(Params.ToString());

                string lstrURL = string.Empty; int benefit_application_detail_id = 0; string spousal_consent_flag = string.Empty;
                int intPlan_Id = 0; string istrBenefitOptionValue = string.Empty; int iintJointAnnuaintID = 0;

                if (AllParams.ContainsKey("intPlan_Id") && Convert.ToString(AllParams["intPlan_Id"]).IsNotNullOrEmpty())
                {
                    intPlan_Id = Convert.ToInt32(AllParams["intPlan_Id"]);
                }
                if (AllParams.ContainsKey("istrBenefitOptionValue") && Convert.ToString(AllParams["istrBenefitOptionValue"]).IsNotNullOrEmpty())
                {
                    istrBenefitOptionValue = Convert.ToString(AllParams["istrBenefitOptionValue"]);
                }
                if (AllParams.ContainsKey("benefit_application_detail_id") && Convert.ToString(AllParams["benefit_application_detail_id"]).IsNotNullOrEmpty())
                {
                    benefit_application_detail_id = Convert.ToInt32(AllParams["benefit_application_detail_id"]);
                }
                if (AllParams.ContainsKey("spousal_consent_flag") && Convert.ToString(AllParams["spousal_consent_flag"]).IsNotNullOrEmpty())
                {
                    spousal_consent_flag = Convert.ToString(AllParams["spousal_consent_flag"]);
                }
                if (AllParams.ContainsKey("iintJointAnnuaintID") && Convert.ToString(AllParams["iintJointAnnuaintID"]).IsNotNullOrEmpty())
                {
                    iintJointAnnuaintID = Convert.ToInt32(AllParams["iintJointAnnuaintID"]);
                }
                string lstrUrl = String.Format(ConfigurationManager.AppSettings["BusinessTierUrl"], "srvPerson");
                Dictionary<string, object> ldictParams = (Dictionary<string, object>)iobjSessionData["dictParams"];

                isrvNeoSpinMSSBusinessTier = WCFClient<IBusinessTier>.CreateChannel(lstrUrl);
                Hashtable lhstParam = new Hashtable();
                lhstParam.Add("intPlan_Id", intPlan_Id);
                lhstParam.Add("istrBenefitOptionValue", istrBenefitOptionValue);
                lhstParam.Add("benefit_application_detail_id", benefit_application_detail_id);
                lhstParam.Add("spousal_consent_flag", spousal_consent_flag);
                lhstParam.Add("iintJointAnnuaintID", iintJointAnnuaintID);
                
                lArrayList = (ArrayList)isrvNeoSpinMSSBusinessTier.ExecuteMethod("UpdateBenefitOptionValue", lhstParam, false, ldictParams);
            }
            finally
            {
                HelperFunction.CloseChannel(isrvNeoSpinMSSBusinessTier);
            }
            return lArrayList;
        }

        [HttpPost]
        public string GetLaunchImageViewerUrl(object Params)
        {
            string istrFinalUrl = "http://" + ConfigurationManager.AppSettings["AppXtender_ServerName"] + "/ISubmitQuery.aspx?Appname=" + ConfigurationManager.AppSettings["AppXtender_AppName"] + "&DataSource=" + ConfigurationManager.AppSettings["AppXtender_DataSource"] + "&QueryType=0&SSN=";
           return istrFinalUrl;
        }

        [HttpPost]
        public HttpResponseMessage EditCorrOnLocalTool_Override(object aParams)
        {
            Dictionary<string, string> AllParams = null;
            string astrLastGeneratedCorr = null;
            ArrayList aarrResult = null;
            string lstrEncryptionKey = ControlsHelper2.GetEncryptionKey(iobjSessionData.istrSessionId);
            string lstrEncryptionIV = ControlsHelper2.GetEncryptionIV(iobjSessionData.istrSessionId);
            bool ablnReadOnlyMode = false;

            AllParams = new JavaScriptSerializer().Deserialize<Dictionary<string, string>>(aParams.ToString());
            if (AllParams.ContainsKey("LastGeneratedCorr"))
            {
                astrLastGeneratedCorr = HelperFunction.SagitecDecryptFIPS(AllParams["LastGeneratedCorr"], lstrEncryptionKey, lstrEncryptionIV);
            }
            else
            {
                vm.ResponseMessage = new utlResponseMessage() { istrMessage = "This file is not generated in current session." };
                vm.ValidationSummary.Add(new utlError() { istrErrorMessage = "This file is not generated in current session." });
                return Request.CreateResponse(HttpStatusCode.OK, vm);
            }
            if (AllParams.ContainsKey("LastCorrSecurityLevel"))
            {
                ablnReadOnlyMode = HelperFunction.SagitecDecryptFIPS(AllParams["LastCorrSecurityLevel"], lstrEncryptionKey, lstrEncryptionIV) == "1";
            }

            HttpResponseMessage lHttpResponseMessage = null;
            ArrayList Result = aarrResult ?? new ArrayList();
            string lstrDefaultPrinter = AllParams["DefaultPrinter"].ToString();
            bool lblnShowPrintDialog = AllParams["ShowPrintDialog"].ToString().ToLower() == "true";
            var idictParams = SetParams("wfmLogin");

            if (aarrResult == null && !astrLastGeneratedCorr.Contains("\\"))
            {
                idictParams["PrependPathForCorr"] = "CorrDrafts";
            }

            if (Result.Count == 0)
            {
                isrvServers.ConnectToBT();
                Result = isrvServers.isrvBusinessTier.EditCorrOnLocalTool(astrLastGeneratedCorr, idictParams);
            }

            if (Result.Count > 0 && Result[0] is utlError)
            {
                utlError lutlError = Result[0] as utlError;
                vm.ResponseMessage = new utlResponseMessage() { istrMessage = lutlError.istrErrorMessage };
                vm.ValidationSummary.Add(lutlError);
                lHttpResponseMessage = Request.CreateResponse(HttpStatusCode.OK, vm);
                return lHttpResponseMessage;
            }

            string lBase64String = Result[0].ToString();
            astrLastGeneratedCorr = Result[1] as string;

            string UserID = idictParams[utlConstants.istrConstUserID].ToString();
            int UserSerialID = Convert.ToInt32(idictParams[utlConstants.istrConstUserSerialID].ToString());
            string RequestIPAddress = idictParams["RequestIPAddress"].ToString();

            long lintTrackingID = GetCorrTrackingID(astrLastGeneratedCorr);

            Dictionary<string, Object> data = new Dictionary<string, Object>();
            data.Add("FilePath", astrLastGeneratedCorr);
            data.Add("ReadOnlyMode", ablnReadOnlyMode);
            data.Add("DefaultPrinter", lstrDefaultPrinter);
            data.Add("ShowPrintDialog", lblnShowPrintDialog);

            data.Add("FormID", AllParams["FormID"]);
            data.Add("KeyField", AllParams["KeyField"]);
            data.Add("TemplateName", AllParams["TemplateName"]);
            data.Add(utlConstants.istrConstUserID, UserID);
            data.Add(utlConstants.istrConstUserSerialID, UserSerialID);
            data.Add("TrackingID", lintTrackingID);
            data.Add("RequestIPAddress", idictParams["RequestIPAddress"].ToString());

            //// adding configuration for custom buttons on corr editor tool
            //List<utlCorrToolCustomControl> llstCorrToolCustomControls = null;
            //ModifyCustomButtonConfigurationForCorrTool(data, out llstCorrToolCustomControls);
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

            var IPnPath = astrLastGeneratedCorr + "~" + RequestIPAddress;

            data.Add("MainSessionID", iobjSessionData.istrSessionId);

            string lstrEncryptedString = HelperFunction.SagitecEncryptFIPS(IPnPath, lstrEncryptionKey, lstrEncryptionIV);
            data.Add("EncryptedString", lstrEncryptedString);

            string lstrWebAPIURL = Request.RequestUri.ToString().Replace("EditCorrOnLocalTool", "{0}").Replace("OpenDoc", "{0}");
            data.Add("WebAPIURL", lstrWebAPIURL);

            data.Add("Base64String", lBase64String.SplitBy(32000));
            Dictionary<string, string> Headers = new Dictionary<string, string>();
            if (Request.Headers.Contains("RequestVerificationToken"))
            {
                Headers.Add("RequestVerificationToken", ((string[])(Request.Headers.GetValues("RequestVerificationToken")))[0]);
            }
            if (Request.Headers.Contains(utlConstants.istrWindowName))
            {
                Headers.Add(utlConstants.istrWindowName, ((string[])(Request.Headers.GetValues(utlConstants.istrWindowName)))[0]);
            }
            if (Request.Headers.Contains("X-Requested-With"))
            {
                Headers.Add("X-Requested-With", ((string[])(Request.Headers.GetValues("X-Requested-With")))[0]);
            }
            data.Add("Headers", Headers);
            utlResponseData lutlResponseData = new utlResponseData();
            lutlResponseData.OtherData["CorrData"] = data;
            vm.DomainModel = lutlResponseData;
            vm.ResponseMessage = new utlResponseMessage() { istrMessage = "Correspondence is opened in local tool." };
            vm.Errors = null;
            lHttpResponseMessage = Request.CreateResponse(HttpStatusCode.OK, vm);
            return lHttpResponseMessage;
        }

        [HttpPost]
        public HttpResponseMessage NavigateToMSS(object Params)
        {
            IBusinessTier isrvNeoSpinMSSBusinessTier = null;
            HttpResponseMessage lHttpResponseMessage = null;
            try
            {
                Dictionary<string, object> AllParams = null;
                string lstrURL = string.Empty;
                var jss = new JavaScriptSerializer();
                AllParams = jss.Deserialize<Dictionary<string, object>>(Params.ToString());
                string astrMPID = AllParams["astrMPID"]?.ToString();
                Dictionary<string, Object> data = new Dictionary<string, Object>();
              
                utlResponseData lutlResponseData = new utlResponseData();

                string lstrUrl = String.Format(ConfigurationManager.AppSettings["BusinessTierUrl"], "srvMPIPHPMSS");
                //FM upgrade changes - Remoting to WCF
                //IBusinessTier isrvNeoSpinMSSBusinessTier = (IBusinessTier)Activator.GetObject(typeof(IBusinessTier), lstrUrl);
                Dictionary<string, object> ldictParams = (Dictionary<string, object>)iobjSessionData["dictParams"];

                isrvNeoSpinMSSBusinessTier = WCFClient<IBusinessTier>.CreateChannel(lstrUrl);
                Hashtable lhstParam = new Hashtable();
                lhstParam.Add("astrMpiPersonId", astrMPID);
                MPIPHP.BusinessObjects.busPerson lobjPerson = (MPIPHP.BusinessObjects.busPerson)isrvNeoSpinMSSBusinessTier.ExecuteMethod("LoadPersonWithMPID", lhstParam, false, ldictParams);
                bool lblnCanAccess = true;


                if (lobjPerson.icdoPerson.vip_flag == MPIPHP.BusinessObjects.busConstant.Flag_Yes)
                {
                    if (iobjSessionData["Logged_In_User_is_VIP"].IsNotNull() && Convert.ToString(iobjSessionData["Logged_In_User_is_VIP"]) == "VIPAccessUser")
                    {
                        lblnCanAccess = true;
                    }
                    else
                    {
                        lblnCanAccess = false;
                    }
                }
                if (lblnCanAccess)
                {
                    FormsAuthentication.SetAuthCookie(astrMPID, true);
                    string lstrMssApp = isrvServers.isrvDbCache.GetConstantValue("MSSA");

                    string lstrUserNameCookie = "MPID_MSS_LOGIN_FROM_APPLICATION";
                    HttpCookie lcokNeoSpinCookie = new HttpCookie(lstrUserNameCookie);
                    lcokNeoSpinCookie.Value = astrMPID;
                    HttpContext.Current.Response.Cookies.Add(lcokNeoSpinCookie);

                    //Need to replace localhost with the server name.
                    //ContentPlaceHolder c = (ContentPlaceHolder)base.Master.FindControl("cphCenterMiddle");
                    string lstrMSSWindowName = Guid.NewGuid().ToString();
                    //c.Controls.Add(new PopupWindowControl("/" + lstrMssApp + "/" + "wfmEntryPoint.aspx?FWN=" + lstrMSSWindowName));
                    lstrURL = lstrMssApp + "/" + "wfmEntryPoint.aspx?FWN=" + lstrMSSWindowName;
                    data.Add("NewURL", lstrURL);
                    lutlResponseData.OtherData["Data"] = data;
                    vm.DomainModel = lutlResponseData;
                    vm.ResponseMessage = new utlResponseMessage() { istrMessage = "Url created successfully." };
                    vm.Errors = null;
                }
                else
                {
                    //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "OpenVIPDialog", "OpenVIPDialog();", true);
                    vm.ResponseMessage = new utlResponseMessage() { istrMessage = "Show VIP Dialog" };
                    vm.Errors = null;
                }
                lHttpResponseMessage = Request.CreateResponse(HttpStatusCode.OK, vm);
                return lHttpResponseMessage;
            }
            catch(Exception _ec)
            {
                lHttpResponseMessage = Request.CreateErrorResponse(HttpStatusCode.InternalServerError, _ec.Message);
                return lHttpResponseMessage;
            }
            finally
            {
                HelperFunction.CloseChannel(isrvNeoSpinMSSBusinessTier);
            }
        }

        [HttpPost]
        public string GetTaxWithHoldingScreenConfiguratorColumns(object Params)
        {
            Dictionary<string, object> AllParams = null;
            var jss = new JavaScriptSerializer();
            AllParams = jss.Deserialize<Dictionary<string, object>>(Params.ToString());
            string astrTaxIdentifierVal = AllParams["astrTaxIdentifierVal"]?.ToString();
            string astrBenType = AllParams["astrBenType"]?.ToString();

            IBusinessTier isrvBusinessTier = null;
            IMetaDataCache isrvMetaDataCache = null;
            IDBCache isrvDBCache = null;
            try
            {
                int aintPersonID = 0;
                string strBusinessTierUrl = String.Format(MPIPHP.Common.ApplicationSettings.Instance.BusinessTierUrl, "srvPayeeAccount");
                string strMetaDataUrl = MPIPHP.Common.ApplicationSettings.Instance.MetaDataCacheUrl;
                string strDBCacheUrl = MPIPHP.Common.ApplicationSettings.Instance.DBCacheUrl;

                isrvBusinessTier = WCFClient<IBusinessTier>.CreateChannel(strBusinessTierUrl);
                isrvMetaDataCache = WCFClient<IMetaDataCache>.CreateChannel(strMetaDataUrl);
                isrvDBCache = WCFClient<IDBCache>.CreateChannel(strDBCacheUrl);

                Dictionary<string, object> ldictParams = new Dictionary<string, object>();

                Hashtable lhstParam = new Hashtable();
                lhstParam.Add("TAX_IDENTIFIER_VALUE", astrTaxIdentifierVal);
                lhstParam.Add("BENDISTYPE", astrBenType);

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

        [HttpPost]
        public string GetBankName(Dictionary<string, object> adictParam)
        {
            string astrRoutingNumber = adictParam["astrRoutingNumber"].ToString();
            string lstrOrgName = string.Empty;

            Dictionary<string, object> ldictParams = (Dictionary<string, object>)iobjSessionData["dictParams"];
            string lstrUrl = string.Format(ConfigurationManager.AppSettings["BusinessTierUrl"], "srvPayeeAccount");
            IBusinessTier lsrvBusinessTier = WCFClient<IBusinessTier>.CreateChannel(lstrUrl);

            Hashtable lhstParam = new Hashtable();

            DataTable ldtbOrgBankName = new DataTable();
            if (astrRoutingNumber.IsNotNullOrEmpty())
            {
                lhstParam.Add(MPIPHP.DataObjects.enmOrganization.routing_number.ToString().ToUpper(), astrRoutingNumber);
                ldtbOrgBankName = lsrvBusinessTier.ExecuteQuery("cdoOrganization.GetOrgDetailsByRoutingNumber", lhstParam, ldictParams); 
            }
            if (ldtbOrgBankName != null && ldtbOrgBankName.Rows.Count > 0)
            {
                if ((ldtbOrgBankName.Rows[0][MPIPHP.DataObjects.enmOrganization.org_name.ToString()]).ToString().IsNotNullOrEmpty())
                    lstrOrgName = Convert.ToString(ldtbOrgBankName.Rows[0][MPIPHP.DataObjects.enmOrganization.org_name.ToString()]);
            }

            return lstrOrgName;
        }
        [HttpPost]
        public string GetLaserFicheUrlfromDB(object Params)
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

                Hashtable lhstParam = new Hashtable();

                Dictionary<string, object> ldictParams = new Dictionary<string, object>();
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

        [HttpPost]
        public string GetWebExFlagDB(object Params)
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

                Hashtable lhstParam = new Hashtable();

                Dictionary<string, object> ldictParams = new Dictionary<string, object>();
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
    }
}