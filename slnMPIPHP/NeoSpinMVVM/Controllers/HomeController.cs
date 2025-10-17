using MPIPHP.BusinessObjects;
using NeoBase.Common;
using NeoBase.SystemManagement.DataObjects;
using NeoSpin.BusinessObjects;
using NeoSpinConstants;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.MVVMClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;
using System.Xml.Linq;

namespace Neo.Controllers
{
    public class HomeController : AccountControllerBase
    {    
        #region[Private]       
        /// <summary>
        /// Method to get Parameters related to show on the footer of master page
        /// </summary>        
        private void GetFooterDetails()
        {
            Dictionary<string, object> ldicMasterParam = null;
            try
            {
                ldicMasterParam = new Dictionary<string, object>();
                // Sagitec.Common.utlServerDetail lobjBusinessTier = (Sagitec.Common.utlServerDetail)isrvServers.isrvBusinessTier.GetBusinessTierDetails();
                string lstrProductVersion = isrvServers.isrvMetaDataCache.GetProductVersion();
                string lstrProductTitle = UiHelperFunction.GetProductTitle();
                string lstrNeospinVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                string lstrReleaseDate = "Framework : " + lstrProductVersion + ", " + lstrProductTitle + " : " + lstrNeospinVersion + ", Solution : " + ServiceHelper.idteReleaseDate.ToLocalTime().ToString();

                string lstrMachineName = HttpContext.Server.MachineName;
                ldicMasterParam.Add(UIConstants.RELEASE_DATE, lstrReleaseDate);
                ldicMasterParam.Add(UIConstants.REQUEST_MACHINE_NAME, lstrMachineName);

                IPAddress[] larrAddresslist = Dns.GetHostAddresses(Convert.ToString(GetIP()));
                if (larrAddresslist != null && larrAddresslist.Length > 0)
                {
                    ldicMasterParam.Add(UIConstants.REQUEST_IP_ADDRESS, Convert.ToString(larrAddresslist[0]));
                }
                if (string.IsNullOrEmpty(ServiceHelper.iobjTraceInfo.istrAppServer) && idictParams.ContainsKey("istrAppServer"))
                {
                    ldicMasterParam.Add(UIConstants.REQUEST_APP_SERVER, idictParams["istrAppServer"]);
                }
                else
                {
                  ldicMasterParam.Add(UIConstants.REQUEST_APP_SERVER, ServiceHelper.iobjTraceInfo.istrAppServer);
                }
                
                ldicMasterParam.Add(UIConstants.REGION, istrRegionValue);
                ldicMasterParam.Add(UIConstants.PRODUCT_REGION, istrProductRegion);             
                ldicMasterParam.Add(UIConstants.BATCH_DATE, Convert.ToString(DateTime.Now));
                iobjSessionData[UIConstants.DICT_MASTER_PARAMS] = ldicMasterParam;
            }
            finally
            {
                ldicMasterParam = null;
            }
        }
        /// <summary>
        /// Get message based on message Id.
        /// </summary>
        /// <param name="messageId">Message Id</param>
        /// <returns>Reponse message object</returns>
        private utlResponseMessage GetMessage(int messageId)
        {
            utlResponseMessage responseMessage = new utlResponseMessage();
            responseMessage = base.GetMessage(messageId);
            //If not found in SGS_FWK_MESSAGES table then pick it from SGS_MESSAGES table.
            if (responseMessage == null)
            {
                responseMessage = new utlResponseMessage()
                {
                    istrMessageID = messageId.ToString(),
                    istrMessage = isrvServers.isrvDbCache.GetMessageText(messageId)
                };
            }
            return responseMessage;
        }

        
        /// <summary>
        /// Validation Summary
        /// </summary>
        /// <param name="vmViewModel"></param>
        /// <param name="lhstParams"></param>
        /// <param name="lstrFileType"></param>
        /// <returns></returns>
        private utlResponseMessage ValidateModel(ViewModelObj vmViewModel, ref Hashtable lhstParams, string lstrFileType)
        {
            utlResponseMessage lobjResponseMessage;
            try
            {
                ArrayList larrErrors = (ArrayList)isrvServers.isrvBusinessTier.ExecuteMethod("UploadFile", lhstParams, false, idictParams);
                // int lbusFileHdr = larrErrors[5] as busFileHdr;
                //yamini: Add comments


                //Yamini:- 
                if (larrErrors.Count > 0 && !(larrErrors[0] is busFileHdr))
                {
                    vmViewModel.ValidationSummary = new ArrayList();
                    foreach (utlError lutlError in larrErrors.OfType<utlError>().ToList())
                    {
                        vmViewModel.ValidationSummary.Add(lutlError);
                    }
                    lobjResponseMessage = GetMessage(NeoConstant.File.FileUpload.Msg_File_Uploaderror);
                }
                else
                {
                    if (lstrFileType == "5001" || lstrFileType == "13" || lstrFileType == "15003") //constant. ONLY for ER file
                    {
                        larrErrors = AddJobScheduleParams(vmViewModel, out lobjResponseMessage, out lhstParams, lstrFileType, larrErrors);
                    }
                    else
                    {
                        lobjResponseMessage = GetMessage(NeoConstant.File.FileUpload.Msg_File_Uploaded);
                        vmViewModel.ValidationSummary = new ArrayList();
                    }
                }
            }
            catch (Exception ex)
            {
                lobjResponseMessage = GetMessage(NeoConstant.File.FileUpload.Msg_File_Uploaderror, new object[1] { ex });
                vmViewModel.DomainModel = new utlResponseData();
                vmViewModel.ValidationSummary = new ArrayList();
            }

            return lobjResponseMessage;
        }

        

        /// <summary>
        /// Add Job schedule parametors for ER File
        /// </summary>
        /// <param name="vmViewModel"></param>
        /// <param name="lobjResponseMessage"></param>
        /// <param name="lhstParams"></param>
        /// <param name="lstrFileType"></param>
        /// <param name="larrErrors"></param>
        /// <returns>ArrayList</returns>
        private ArrayList AddJobScheduleParams(ViewModelObj vmViewModel, out utlResponseMessage lobjResponseMessage, out Hashtable lhstParams, string lstrFileType, ArrayList larrErrors)
        {
            busFileHdr lbusFileHdr = larrErrors[larrErrors.Count - 1] as busFileHdr;
            int lintFileHdrID = lbusFileHdr.icdoFileHdr.file_hdr_id;

            lobjResponseMessage = UiHelperFunction.GetMessage(NeoConstant.File.FileUpload.Msg_Er_File_Uploaded);
            vmViewModel.ValidationSummary = new ArrayList();

            lhstParams = new Hashtable();
            if (lstrFileType == "5001")
                lhstParams.Add("astrJobScheduleCode", JobServiceCodes.PROCESS_EMPLOYER_REPORTING_FILE);//schedule code
            else if (lstrFileType == "15003")
                lhstParams.Add("astrJobScheduleCode", JobServiceCodes.PROCESS_FUND_BALANCE_INBOUND_FILE);//schedule code
            else
                lhstParams.Add("astrJobScheduleCode", JobServiceCodes.PROCESS_PERSON_ENROLLMENT_FILE);//schedule code
            Hashtable lhstJobScheduleParams = new Hashtable();
            //YAMINI :-  Pass file header ID as job schedule parameters                                                   
            lhstJobScheduleParams.Add("FILE_HDR_ID", lintFileHdrID);
            lhstParams.Add("ahstJobScheduleParams", lhstJobScheduleParams);//schedule code
            larrErrors = (ArrayList)isrvServers.isrvBusinessTier.ExecuteMethod("QueueJobSchedule", lhstParams, false, idictParams);
            return larrErrors;
        }
        
        /// <summary>
        ///  Upload File from Fime maintenace  
        /// </summary>
        /// <param name="vmViewModel"></param>
        /// <param name="fileAttachment"></param>
        /// <param name="aintFileUploadLimit"></param>
        /// <param name="aobjResponseData"></param>
        /// <param name="aobjResponseMessage"></param>
        /// <param name="astrFormName"></param>
        /// <param name="astrFileInfoName"></param>
        /// <param name="arrBytes"></param>
        /// <param name="ahstParams"></param>
        /// <param name="jsonResult">JsonResult</param>
        private JsonResult FileUpload(ViewModelObj vmViewModel, HttpPostedFileBase fileAttachment, int aintFileUploadLimit, utlResponseData aobjResponseData,
            utlResponseMessage aobjResponseMessage, string astrFormName, string astrFileInfoName, byte[] arrBytes, Hashtable ahstParams)
        {
            JsonResult ljsonResult = null;
            if (fileAttachment.ContentLength <= aintFileUploadLimit)
            {                
                string lstrOrgCode = aobjResponseData.HeaderData["MaintenanceData"].ContainsKey("txtOrgCode") ?
                    Convert.ToString(aobjResponseData.HeaderData["MaintenanceData"]["txtOrgCode"]) : string.Empty;
                int lintOrgId = UIConstants.ZERO;
                int lintFileTypeID = UIConstants.ZERO;
                string lstrFileType = aobjResponseData.HeaderData["MaintenanceData"]["ddlFileType"].ToString();
                vmViewModel.ValidationSummary = new ArrayList();
                if (lstrFileType == null || lstrFileType == "" || Convert.ToInt32(lstrFileType) <= 0)
                {
                    aobjResponseMessage = GetMessage(NeoConstant.File.FileUpload.Msg_Er_Upload);                    
                    utlError lutlError = new utlError();
                    lutlError.istrErrorID = NeoConstant.File.FileUpload.Msg_Fileupload_Select_Filetype.ToString();
                    string lstrErrorMessage = GetMessage(NeoConstant.File.FileUpload.Msg_Fileupload_Select_Filetype).istrMessage;
                    lutlError.istrErrorMessage = lstrErrorMessage;
                    vmViewModel.ValidationSummary.Add(lutlError);
                }
                
                if (vmViewModel.ValidationSummary.Count > 0)
                {
                    vmViewModel.DomainModel = new utlResponseData();
                    vmViewModel.ResponseMessage = aobjResponseMessage;
                    vmViewModel.ExtraInfoFields["FormId"] = astrFormName;
                    return ljsonResult=Json(vmViewModel, JsonRequestBehavior.AllowGet);
                    
                }

                if (lstrFileType != "")
                {
                    lintFileTypeID = Convert.ToInt32(lstrFileType);
                }
                ahstParams = new Hashtable
                                        {
                                            { "aintOrgID", lintOrgId },
                                            { "aintFileTypeID", lintFileTypeID },
                                            { "aarrAttachmentData", arrBytes },
                                            { "astrFileName", astrFileInfoName },
                                            { "astrUserID", idictParams["UserID"] }
                                        };

                aobjResponseMessage = ValidateModel(vmViewModel, ref ahstParams, lstrFileType);
            }
            return ljsonResult;
        }

        #endregion
        #region [Public]
        /// <summary>
        /// Submit File
        /// </summary>
        /// <param name="files"></param>
        /// <param name="data"></param>
        /// <returns>JsonResult</returns>
        [HttpPost]
        public JsonResult SubmitFile([System.Web.Http.FromBody] IEnumerable<HttpPostedFileBase> files, [System.Web.Http.FromBody] object data)
        {
            var jss = new JavaScriptSerializer();
            Dictionary<string, object> ldictParams = (Dictionary<string, object>)iobjSessionData["dictParams"];
            ViewModelObj vmViewModel = new ViewModelObj();
            utlResponseData lobjResponseData = null;
            utlResponseMessage lobjResponseMessage = new utlResponseMessage();
            string strData = string.Empty;
            string lstrFormName = string.Empty;
            string lstrFileUploadPath = string.Empty;
            try
            {
                strData = ((string[])data)[0];
            }
            catch (Exception E)
            {
                throw E;
            }
            lobjResponseData = (utlResponseData)jss.Deserialize<utlResponseData>(strData);
            if (lobjResponseData != null)
            {
                lstrFormName = lobjResponseData.istrFormName;
                if (files != null && files.Count() > 0)
                {
                    #region Set Initial Common Operation

                    string lstrFileInfoName = Path.GetFileName(Request.Files[0].FileName);
                    byte[] larrBytes = new byte[Request.Files[0].ContentLength];
                    Request.Files[0].InputStream.Read(larrBytes, 0, Request.Files[0].ContentLength);

                    //Defaulting File Upload Limit to 10 MB if no setting found in web.config o.w. reads from web.config.
                    string lstrFileUploadLimit = System.Configuration.ConfigurationManager.AppSettings["FILE_UPLOAD_LIMIT"];

                    int lintFileUploadLimit = 10485760;
                    if (!int.TryParse(lstrFileUploadLimit, out lintFileUploadLimit))
                    {
                        lintFileUploadLimit = 10485760;
                    }
                    if (idictParams.ContainsKey("IsErrorOccured"))
                    {
                        idictParams.Remove("IsErrorOccured");
                    }
                    idictParams[utlConstants.istrConstFormName] = lstrFileInfoName;
                    isrvServers.ConnectToBT(lstrFormName);
                    string lstrFileType = string.Empty;
                    String lstrMessage = String.Empty;
                    utlError lutlError = new utlError();
                    #endregion Set Initial Common Operation

                    switch (lstrFormName)
                    {
                        case UIConstants.UploadFileMaintenance:
                            {
                                try
                                {
                                    if (lobjResponseData.HeaderData["MaintenanceData"].ContainsKey("ddlFileType"))
                                    {
                                        lstrFileType = Convert.ToString(lobjResponseData.HeaderData["MaintenanceData"]["ddlFileType"]);
                                    }
                                    else
                                    {
                                        lstrFileType = "0";
                                    }
                                    if (Request.Files[0].ContentLength > 0)
                                    {
                                        // Write data into a file
                                        Hashtable lhstParams = new Hashtable();
                                        lhstParams.Add("astrUserId", Convert.ToString(idictParams["UserID"]).Trim());
                                        //lhstParams.Add("aintReferenceId", lintOrgID);
                                        lhstParams.Add("aintFileType", Convert.ToInt32(lstrFileType));
                                        lhstParams.Add("astrFileName", lstrFileInfoName);
                                        lhstParams.Add("aBuffer", larrBytes);
                                        lhstParams.Add("astrMailFrom", "MPIPHP.UploadFile@sagitec.com");
                                        lhstParams.Add("astrEmailId", "jaswinder.singh@sagitec.com");
                                        lhstParams.Add("astrSubject", lstrFileInfoName + " has been successfully received ");
                                        lhstParams.Add("astrMessage", lstrFileInfoName + " will soon be uploaded into MPIPHP system and all the transaction edits will be run. " +
                                            "Once the upload and edits are completed you will be informed via email");
                                        lhstParams.Add("ablnAlwaysCreateFileHeader", true);

                                        ArrayList larrErrors = (ArrayList)isrvServers.isrvBusinessTier.ExecuteMethod("ValidateAndWriteToFile", lhstParams, false, idictParams);
                                        if (larrErrors.Count > 0)
                                        {
                                            Hashtable lshtErrors = new Hashtable();

                                            foreach (utlError lobjError in larrErrors)
                                            {
                                                if (!lshtErrors.ContainsKey(lobjError.istrErrorID))
                                                {
                                                    lshtErrors.Add(lobjError.istrErrorID, lobjError.istrErrorMessage);
                                                }
                                            }

                                            foreach (DictionaryEntry hashEntry in lshtErrors)
                                            {
                                                lutlError.istrErrorID = hashEntry.Key.ToString();
                                                lutlError.istrErrorMessage = hashEntry.Value.ToString();
                                                vmViewModel.ValidationSummary.Add(lutlError);
                                                ldictParams["IsErrorOccured"] = vmViewModel.ValidationSummary;
                                            }
                                            lobjResponseMessage.istrMessage = "Filename : " + lstrFileInfoName + "&nbsp;&nbsp" + " Size : " + Request.Files[0].ContentLength.ToString() + ", unable to load due to errors";
                                        }
                                        else
                                        {
                                            lobjResponseMessage.istrMessage = "Filename : " + lstrFileInfoName + "&nbsp;&nbsp" + " Size : " + Request.Files[0].ContentLength.ToString() + " successfully loaded";
                                        }
                                    }
                                    if (Request.Files[0].ContentLength == 0)
                                    {
                                        lobjResponseMessage.istrMessage = " Uploading the File Failed as the File is empty";
                                        throw new Exception(lobjResponseMessage.istrMessage);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    lutlError.istrErrorID = "";
                                    lutlError.istrErrorMessage = "Error Occurred: "+ ex;
                                    vmViewModel.ValidationSummary.Add(lutlError);
                                    ldictParams["IsErrorOccured"] = vmViewModel.ValidationSummary;
                                }
                            }
                            break;
                        // F/W Upgrade : code conversion of btnUploadFile_Click method.
                        case UIConstants.PirMaintenance:
                            {
                                Hashtable lhstParams = new Hashtable();
                                if (Request.Files[0].ContentLength <= 5 * 1024 * 1024) // File size upto 5 MB allowed.
                                {
                                    string lstrPIRID = lobjResponseData.HeaderData["MaintenanceData"]["lblPirId"].ToString();
                                    lhstParams.Add("aintPIRID", Convert.ToInt32(lstrPIRID));
                                    lhstParams.Add("astrFileName", lstrFileInfoName);
                                    lhstParams.Add("aobjAttachmentData", larrBytes);
                                    lhstParams.Add("astrMimeType", Request.Files[0].ContentType);
                                    lhstParams.Add("astrUserID", idictParams["UserID"]);
                                    ArrayList larrErrors = (ArrayList)isrvServers.isrvBusinessTier.ExecuteMethod("InsertAttachment", lhstParams, false, idictParams);
                                    if (larrErrors != null && larrErrors.Count > 0)
                                    {
                                        vmViewModel.ValidationSummary.Add((utlError)larrErrors[0]);
                                    }
                                }
                            }
                            break;

                        default:
                            break;
                    }

                }
                else
                {
                    utlError lutlError = new utlError
                    {
                        istrErrorID = busNeoBaseConstants.File.MSG_FILE_UPLOAD_SELECT.ToString(),
                        istrErrorMessage = GetMessage(busNeoBaseConstants.File.MSG_FILE_UPLOAD_SELECT).istrMessage,
                    };
                    vmViewModel.ValidationSummary.Add(lutlError);
                    lobjResponseMessage = GetMessage(busNeoBaseConstants.File.MSG_FILE_UPLOAD_SELECT);
                }
            }
            vmViewModel.DomainModel = new utlResponseData();
            vmViewModel.ResponseMessage = lobjResponseMessage;
            vmViewModel.ExtraInfoFields["FormId"] = lstrFormName;
            return Json(vmViewModel, JsonRequestBehavior.AllowGet);
        }        
        #endregion
        #region [Overriden Methods]
        /// <summary>
        /// override Index method for load footer details.
        /// </summary>
        /// <returns></returns>
        public override ActionResult Index()
        {
            iobjSessionData["SetWindowNameInMaster"] = ConfigurationManager.AppSettings["SetWindowNameInMaster"] != null && Convert.ToBoolean(ConfigurationManager.AppSettings["SetWindowNameInMaster"]);
            GetFooterDetails();
            return base.Index();
        }
        #endregion

        // F/W Upgrade : Code conversion of btn_OpnPDF and btn_OpenPDF method.
        [HttpPost]
        public ActionResult OpenPDF()
        {
            byte[] lbytFileBytes = null;
            try
            {
                Dictionary<string, object> AllParams = null;
                
                var jss = new JavaScriptSerializer();
                AllParams = jss.Deserialize<Dictionary<string, object>>(HttpContext.Request.Params["aobjDownload"].ToString());
                var istrSenderForm = HttpContext.Request.Params["SenderForm"];

                isrvServers.ConnectToBT(istrSenderForm);

                Dictionary<string, object> ldictParams = (Dictionary<string, object>)iobjSessionData["dictParams"];

                Hashtable lhshTable = new Hashtable();
                string lstrButtonInfo = Convert.ToString(AllParams["lstrButtonInfo"]);

                if (lstrButtonInfo == "Open File")
                {
                    var astr_filename = Convert.ToString(AllParams["FileName"]);
                    lhshTable.Add("astrFormName", istrSenderForm);
                    lhshTable.Add("astr_filename", astr_filename);
                    
                    lbytFileBytes = (byte[])isrvServers.isrvBusinessTier.ExecuteMethod("RenderPDF", lhshTable, false, ldictParams);
                }
                else
                {
                    int aintPersonID = Convert.ToString(AllParams["aintPersonID"]) != string.Empty ? Convert.ToInt32(AllParams["aintPersonID"]) : 0;

                    lhshTable.Add("astrFormName", istrSenderForm);
                    lhshTable.Add("aintPersonID", aintPersonID);
                }

                if (lstrButtonInfo == "Generate Payment Directive" || lstrButtonInfo == "Retrieve Payment Directive" || lstrButtonInfo == "Retrieve Deleted Payment Directive")
                {
                    int aintPayeeAccountId = Convert.ToString(AllParams["aintPayeeAccountId"]) != string.Empty ? Convert.ToInt32(AllParams["aintPayeeAccountId"]) : 0;
                    string astrSpecialInstructions = Convert.ToString(AllParams["astrSpecialInstructions"]);
                    DateTime adtAdhocPaymentDate = Convert.ToString(AllParams["adtAdhocPaymentDate"]) != string.Empty ? Convert.ToDateTime(AllParams["adtAdhocPaymentDate"]) : DateTime.MinValue; ;
                    string astrModifiedBy = Convert.ToString(AllParams["astrModifiedBy"]);

                    lhshTable.Add("aintPayeeAccountId", aintPayeeAccountId);
                    lhshTable.Add("astrSpecialInstructions", astrSpecialInstructions);
                    lhshTable.Add("adtAdhocPaymentDate", adtAdhocPaymentDate);
                    lhshTable.Add("astrModifiedBy", astrModifiedBy);
                }

                if (lstrButtonInfo == "Generate Annual Statement" || lstrButtonInfo == "Retrieve Annual Statement")
                {
                    lbytFileBytes = (byte[])isrvServers.isrvBusinessTier.ExecuteMethod("CreateMemberAnnualStatement", lhshTable, false, ldictParams);
                }
                else if (lstrButtonInfo == "Month Of Suspendible Service Report")
                {
                    lbytFileBytes = (byte[])isrvServers.isrvBusinessTier.ExecuteMethod("MonthOfSuspendibleServiceReport", lhshTable, false, ldictParams);
                }
                else if (lstrButtonInfo == "Generate Payment Directive")
                {
                    lbytFileBytes = (byte[])isrvServers.isrvBusinessTier.ExecuteMethod("GeneratePaymentDirective", lhshTable, false, ldictParams);
                }
                else if (lstrButtonInfo == "Retrieve Payment Directive" || lstrButtonInfo == "Retrieve Deleted Payment Directive")
                {
                    DateTime adtPaymentCycleDate = Convert.ToString(AllParams["adtPaymentCycleDate"]) != string.Empty ? Convert.ToDateTime(AllParams["adtPaymentCycleDate"]) : DateTime.MinValue;
                    int aintDeletedPaymentDirectiveId = Convert.ToString(AllParams["aintDeletedPaymentDirectiveId"]) != string.Empty ? Convert.ToInt32(AllParams["aintDeletedPaymentDirectiveId"]) : 0;

                    lhshTable.Add("adtPaymentCycleDate", adtPaymentCycleDate);
                    lhshTable.Add("aintDeletedPaymentDirectiveId", aintDeletedPaymentDirectiveId);

                    lbytFileBytes = (byte[])isrvServers.isrvBusinessTier.ExecuteMethod("RetrievePaymentDirective", lhshTable, false, ldictParams);
                }
                else if (lstrButtonInfo == "Open Month Of Suspendible Service Report")
                {
                    int lintTrackingId = Convert.ToString(AllParams["aintTrackingId"]) != string.Empty ? Convert.ToInt32(AllParams["aintTrackingId"]) : 0;
                    lhshTable.Add("aintTrackingId", lintTrackingId);
                    lbytFileBytes = (byte[])isrvServers.isrvBusinessTier.ExecuteMethod("OpenMonthOfSuspendibleServiceReport", lhshTable, false, ldictParams);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return File(lbytFileBytes, "application/pdf");
        }
        [HttpGet]
        public ActionResult MapRender()
        {
            LoginModel objLoginModel = new LoginModel();

            SetAntiForgeryToken(objLoginModel);

            if (iobjSessionData["dictParams"] != null)
            {
                Dictionary<string, object> dicParams = (Dictionary<string, object>)iobjSessionData["dictParams"];

                objLoginModel.LoginWindowName = dicParams["WindowName"].ToString();

            }
            return View(objLoginModel);
        }

        [HttpGet]
        public ActionResult BPMNMap()
        {
            LoginModel objLoginModel = new LoginModel();

            SetAntiForgeryToken(objLoginModel);

            if (iobjSessionData["dictParams"] != null)
            {
                Dictionary<string, object> dicParams = (Dictionary<string, object>)iobjSessionData["dictParams"];

                objLoginModel.LoginWindowName = dicParams["WindowName"].ToString();

            }
            return View(objLoginModel);
        }


        [HttpGet]
        public ActionResult BPMNReadOnlyMap()
        {
            LoginModel objLoginModel = new LoginModel();

            SetAntiForgeryToken(objLoginModel);

            if (Session["dictParams"] != null)
            {
                Dictionary<string, object> dicParams = (Dictionary<string, object>)Session["dictParams"];

                objLoginModel.LoginWindowName = Convert.ToString(dicParams["WindowName"]);

            }
            return View(objLoginModel);
        }
    }
}
