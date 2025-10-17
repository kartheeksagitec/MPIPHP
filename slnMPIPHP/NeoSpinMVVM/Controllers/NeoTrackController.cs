using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Script.Serialization;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.Interface;
using Sagitec.ViewModelMetaData;
using Sagitec.WebControls_new;

namespace NeoTrack.Controllers
{
    // [Authorize]
    public class NeoTrackController : ApiController
    {
        private srvServers isrvServers;

        private static Dictionary<int, Dictionary<string, string>> iDictCodeValues = new Dictionary<int, Dictionary<string, string>>();

        public NeoTrackController()
        {
            isrvServers = new srvServers();
        }

        [HttpGet]
        public HttpResponseMessage GetMenu()
        {
            string lstrMenu = HtmlGen.GetMenu();
            ViewModelObj vm = new ViewModelObj();
            vm.ResponseMessage = GetMessage(5);
            vm.MenuTemplate = lstrMenu;
            var lResult = Request.CreateResponse(HttpStatusCode.OK, vm);
            return lResult;
        }

        [HttpGet]
        public HttpResponseMessage GetTemplate(string astrFormID, bool ablnIsCenterLeft)
        {
            ViewModelObj vm = new ViewModelObj();
            vm.Errors = new List<string>();
            Dictionary<string, utlObjectData> lobjLookup = null;

            HtmlGen html = new HtmlGen(isrvServers);
            vm.Template = html.RenderHtml(out lobjLookup, astrFormID, ablnIsCenterLeft);
            if (lobjLookup != null)
                vm.DomainModel.HeaderData = lobjLookup;
            vm.ExtraInfoFields = (Dictionary<string, string>)HttpContext.Current.Session[astrFormID + "_ExtraInfoFields"];
            vm.ExtraInfoFields["FormId"] = astrFormID;
            //vm.ExtraInfoFields["FormType"] = astrFormID.EndsWith("Lookup") ? "Lookup" : "Maintenance";
            vm.ExtraInfoFields["TempleteHash"] = GetHashCodeForString(vm.Template);
            vm.ExtraInfoFields["IsCenterLeft"] = ablnIsCenterLeft.ToString();
            vm.ClientVisibility = (Dictionary<string, utlObjectData>)HttpContext.Current.Session[astrFormID + "_ClientVisibility"];
            vm.ControlAttribites = (Dictionary<string, utlObjectData>)HttpContext.Current.Session[astrFormID + "_ControlAttributes"];
            if (astrFormID.Contains("Lookup") || astrFormID.Contains("wfmUploadFile"))
            {
                vm.DomainModel.HeaderData.Add("ControlList", GetHiddenControlListForLookup(astrFormID));
            }
            var lResult = Request.CreateResponse(HttpStatusCode.OK, vm);
            return lResult;
        }

        public utlObjectData GetHiddenControlListForLookup(string astrFormID)
        {
            isrvServers.ConnectToBT(astrFormID);

            Dictionary<string, object> ldictParams = new Dictionary<string, object>();
            ldictParams[utlConstants.istrConstFormName] = astrFormID;
            ldictParams[utlConstants.istrConstUserID] = HttpContext.Current.Session["UserID"].ToString();
            ldictParams["SessionID"] = HttpContext.Current.Session.SessionID;
            ArrayList larrGridItems = new ArrayList();

            utlObjectData hdnControls = isrvServers.isrvBusinessTier.GetHiddenControlForLookup(ldictParams);

            return hdnControls;
        }

        [HttpPost]
        public HttpResponseMessage GetCorrespondenceDropDown(IDictionary<string, string> aParams)
        {
            string astrFormID = aParams["FormID"];
            isrvServers.ConnectToBT(astrFormID);
            Dictionary<string, object> ldictParams = SetParams(astrFormID);
            ldictParams["PrimaryKey"] = aParams["KeyField"];
            ArrayList larrResult = isrvServers.isrvBusinessTier.MVVMGetCorrespondenceDropDown(ldictParams);
            ViewModelObj vm = new ViewModelObj();
            vm.ExtraInfoFields = new Dictionary<string, string>();
            vm.ExtraInfoFields["KeyField"] = aParams["KeyField"];
            vm.ExtraInfoFields["FormId"] = astrFormID;
            if (larrResult != null)
                vm.DomainModel = (utlResponseData)larrResult[0];
            var lResult = Request.CreateResponse(HttpStatusCode.OK, vm);
            return lResult;
        }

        [HttpPost]
        public HttpResponseMessage GenerateCorrespondence(object aParams)
        {
            var jss = new JavaScriptSerializer();
            Dictionary<string, object> AllParams = (Dictionary<string, object>)jss.Deserialize<Dictionary<string, object>>(aParams.ToString());
            string astrFormID = AllParams["FormID"].ToString();
            isrvServers.ConnectToBT(astrFormID);
            Dictionary<string, object> ldictParams = SetParams(astrFormID);
            ldictParams["PrimaryKey"] = AllParams["KeyField"].ToString();
            ldictParams["TemplateName"] = AllParams["TemplateName"].ToString();

            Hashtable QueryBkmksValue = new Hashtable();

            if (AllParams.ContainsKey("QueryBkmksValue"))
            {
                Dictionary<string, object> Bookmarks = (Dictionary<string, object>)AllParams["QueryBkmksValue"];

                foreach (KeyValuePair<string, object> kvp in Bookmarks)
                {
                    QueryBkmksValue.Add(kvp.Key, kvp.Value.ToString());
                }
            }

            ArrayList larrResult = isrvServers.isrvBusinessTier.MVVMGenerateCorrespondence(QueryBkmksValue, ldictParams);
            ViewModelObj vm = new ViewModelObj();
            vm.ExtraInfoFields = new Dictionary<string, string>();
            vm.ExtraInfoFields["FormId"] = astrFormID;
            vm.DomainModel = new utlResponseData();
            vm.ResponseMessage = new utlResponseMessage();
            if (larrResult != null && larrResult.Count > 0 && larrResult[0] is utlError)
            {
                vm.ResponseMessage.istrMessage = ((utlError)larrResult[0]).istrErrorMessage;
                vm.ValidationSummary.Add(vm.ResponseMessage.istrMessage);
            }
            else
            {
                vm.ResponseMessage.istrMessage = larrResult[1].ToString();
                vm.ExtraInfoFields["CorrFilePath"] = larrResult[0].ToString();
                vm.ExtraInfoFields["KeyField"] = AllParams["KeyField"].ToString();
            }
            var lResult = Request.CreateResponse(HttpStatusCode.OK, vm);
            return lResult;
        }

        [HttpPost]
        public HttpResponseMessage GetChartData(IDictionary<string, string> aSearchParams)
        {
            return GetSearchResult(aSearchParams, null);
        }

        //POST action
        [HttpPost]
        public HttpResponseMessage GetSearchResult(IDictionary<string, string> aSearchParams)
        {
            return GetSearchResult(aSearchParams, null);
        }

        public HttpResponseMessage GetSearchResult(IDictionary<string, string> aSearchParams, Collection<utlWhereClause> lclbWhereClaues = null)
        {
            string astrFormID = aSearchParams["FormID"];
            isrvServers.ConnectToBT(astrFormID);
            Dictionary<string, object> ldictParams;
            try
            {
                ldictParams = SetParams(astrFormID);
            }
            catch (Exception ex)
            {
                var response = Request.CreateResponse(HttpStatusCode.Moved);
                response.Headers.Location = new Uri("http://localhost/NeoTrack/");
                return response;
            }
            ArrayList larrGridItems = new ArrayList();

            ArrayList larrResult = isrvServers.isrvBusinessTier.MVVMSearch((Dictionary<string, string>)aSearchParams, larrGridItems, ldictParams, lclbWhereClaues);
            int lintRecCount = (int)larrResult[0];  // First item contains the Count
            utlResponseMessage lobjResponseMessage;

            ViewModelObj vm = new ViewModelObj();

            if (astrFormID.IndexOf("ChartLookup") > 0)
            {
                vm.ChartLookupData = (List<Dictionary<string, string>>)larrResult[2];
            }
            else
            {
                vm.DomainModel = (utlResponseData)larrResult[2];
            }
            vm.Errors = null;
            vm.Template = null;
            vm.ExtraInfoFields["formId"] = astrFormID;

            string sfwMaxCount = vm.ExtraInfoFields.ContainsKey("sfwMaxCount") ? vm.ExtraInfoFields["sfwMaxCount"].ToString() : "";
            int MaxCount = string.IsNullOrEmpty(sfwMaxCount) ? 100 : int.Parse(sfwMaxCount);


            if (lintRecCount == 0)
            {
                lobjResponseMessage = GetMessage(2);
            }
            else
            {
                object[] larrParam = new object[2];
                if (lintRecCount > MaxCount)
                {
                    larrParam[0] = lintRecCount;
                    larrParam[1] = MaxCount;
                    lobjResponseMessage = GetMessage(3, larrParam);
                }
                else
                {
                    larrParam[0] = lintRecCount;
                    lobjResponseMessage = GetMessage(1, larrParam);
                }
            }

            vm.ResponseMessage = lobjResponseMessage;
            vm.ExtraInfoFields["FormId"] = astrFormID;
            var lResult = Request.CreateResponse(HttpStatusCode.OK, vm);

            return lResult;
        }

        [HttpPost]
        public HttpResponseMessage SaveData([FromUri] string astrFormID, [FromUri] string astrKeyValue, [FromUri] string IsNewForm, [FromBody] object aobjSaveData)
        {
            var jss = new JavaScriptSerializer();
            utlResponseData AllParams = (utlResponseData)jss.Deserialize<utlResponseData>(aobjSaveData.ToString());

            //CalculateMD5Hash(aobjSaveData);
            isrvServers.ConnectToBT(astrFormID);
            Dictionary<string, object> ldictParams = SetParams(astrFormID);
            ldictParams["PrimaryKey"] = astrKeyValue;
            ArrayList larrlstSummary = isrvServers.isrvBusinessTier.MVVMSaveData(AllParams, ldictParams);

            ViewModelObj vm = new ViewModelObj();
            vm.ExtraInfoFields = new Dictionary<string, string>();
            vm.ExtraInfoFields["KeyField"] = astrKeyValue;

            vm.ResponseMessage = new utlResponseMessage();
            if (larrlstSummary != null && larrlstSummary.Count > 0 && larrlstSummary[0] is utlError)
            {
                vm.ValidationSummary = larrlstSummary;
                vm.ResponseMessage = GetMessage(30); // not saved
            }
            else
            {
                vm.ResponseMessage = GetMessage(8); // Default values saved successfully
                vm.DomainModel = (utlResponseData)larrlstSummary[0];
                vm.ExtraInfoFields["KeyField"] = vm.DomainModel.KeysData["PrimaryKey"];
            }

            vm.Errors = null;
            vm.ExtraInfoFields["FormId"] = astrFormID;
            if (IsNewForm == "true")
            {
                vm.ExtraInfoFields["IsNewFormSaved"] = "true";
            }
            var lResult = Request.CreateResponse(HttpStatusCode.OK, vm);
            return lResult;
        }

        private List<string> NotRequiredParams = new List<string>() { "PrimaryKey", "ActiveForm", "Title", "ToolTip" };

        [HttpPost]
        public HttpResponseMessage ExecuteObjectMethod([FromUri] string astrFormID, [FromUri] string astrMethodName, [FromUri] bool ablnValidate, [FromUri] string astrKeyValue, [FromBody] object aParams)
        {
            var jss = new JavaScriptSerializer();
            utlExecuteMethodData aobjData = (utlExecuteMethodData)jss.Deserialize<utlExecuteMethodData>(aParams.ToString());

            isrvServers.ConnectToBT(astrFormID);
            Dictionary<string, object> ldictParams = SetParams(astrFormID);
            ldictParams["PrimaryKey"] = astrKeyValue;
            utlResponseData aobjSaveData = (utlResponseData)aobjData.ResponseData;
            Hashtable lhstSelectedRow = new Hashtable();
            var ldctRow = (Dictionary<string, string>)aobjData.NavigationParams;
            foreach (string astrParamName in ldctRow.Keys)
            {
                if (!NotRequiredParams.Contains(astrParamName))
                    lhstSelectedRow.Add(astrParamName, ldctRow[astrParamName]);
            }

            Hashtable lhstRefreshParams = new Hashtable();
            ldctRow = (Dictionary<string, string>)aobjData.RefreshObjParams;
            foreach (string astrParamName in ldctRow.Keys)
            {
                if (!NotRequiredParams.Contains(astrParamName))
                    lhstRefreshParams.Add(astrParamName, ldctRow[astrParamName]);
            }

            ldictParams["RefreshObjParams"] = lhstRefreshParams;

            Object lobjResponse = null;
            if (ablnValidate)
            {
                lobjResponse = isrvServers.isrvBusinessTier.MVVMValidateExecuteObjectMethod(aobjSaveData, astrMethodName, lhstSelectedRow, ldictParams);
            }
            else
            {
                lobjResponse = isrvServers.isrvBusinessTier.MVVMExecuteObjectMethod(aobjSaveData, astrMethodName, lhstSelectedRow, ldictParams);
            }

            ViewModelObj vm = new ViewModelObj();
            vm.ExtraInfoFields = new Dictionary<string, string>();

            if (lobjResponse != null && lobjResponse is ArrayList && ((ArrayList)lobjResponse).Count > 0)
            {
                vm.ValidationSummary = (ArrayList)lobjResponse;
                vm.ResponseMessage = new utlResponseMessage();
            }
            else
            {
                vm.DomainModel = (utlResponseData)lobjResponse;
                vm.ResponseMessage = GetMessage(8); // Default values saved successfully
                vm.ExtraInfoFields["KeyField"] = vm.DomainModel.KeysData["PrimaryKey"];
            }

            vm.Errors = null;
            vm.ExtraInfoFields["FormId"] = astrFormID;
            var lResult = Request.CreateResponse(HttpStatusCode.OK, vm);
            return lResult;
        }

        private static Dictionary<string, object> SetParams(string astrFormID)
        {
            Dictionary<string, object> ldictParams = new Dictionary<string, object>();
            ldictParams["SessionID"] = HttpContext.Current.Session.SessionID;
            ldictParams[utlConstants.istrConstFormName] = astrFormID;
            int lintUserSerialID = int.Parse(HttpContext.Current.Session["UserSerialID"].ToString());
            ldictParams[utlConstants.istrConstUserSerialID] = lintUserSerialID;
            ldictParams[utlConstants.istrConstUserID] = HttpContext.Current.Session["UserID"].ToString();
            return ldictParams;
        }

        [HttpPost]
        public HttpResponseMessage GetFormForOpen([FromUri] string astrFormID, [FromBody] List<Dictionary<string, string>> alstParams)
        {

            ArrayList larrSelectedRows = new ArrayList();
            foreach (var ldctRow in alstParams)
            {
                Hashtable lhstSelectedRow = new Hashtable();
                foreach (string astrParamName in ldctRow.Keys)
                {
                    lhstSelectedRow.Add(astrParamName, ldctRow[astrParamName]);
                }
                larrSelectedRows.Add(lhstSelectedRow);
            }
            HttpContext.Current.Session["SelectedForm"] = astrFormID;
            HttpContext.Current.Session[astrFormID + "SelectedIndex"] = 0;
            HttpContext.Current.Session[astrFormID + "SelectedRows"] = larrSelectedRows;
            ViewModelObj vm;
            try
            {
                vm = GetData(utlPageMode.Update);
            }
            catch (NullReferenceException ex)
            {
                var response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                return response;
            }
            vm.ResponseMessage = GetMessage(7);
            vm.ExtraInfoFields = new Dictionary<string, string>();
            vm.ExtraInfoFields["FormId"] = astrFormID;
            vm.ExtraInfoFields["TotalRowsSelected"] = larrSelectedRows.Count.ToString();
            vm.ExtraInfoFields["SelectedIndex"] = "0";
            vm.ExtraInfoFields["KeyField"] = vm.DomainModel.KeysData["PrimaryKey"];

            var lResult = Request.CreateResponse(HttpStatusCode.OK, vm);
            return lResult;
        }

        [HttpPost]
        public HttpResponseMessage UpdateCorrespondenceStatus(object aParams)
        {

            var jss = new JavaScriptSerializer();
            Dictionary<string, object> AllParams = (Dictionary<string, object>)jss.Deserialize<Dictionary<string, object>>(aParams.ToString());
            string astrFormID = AllParams["FormID"].ToString();
            isrvServers.ConnectToBT(astrFormID);
            string TemplateName = AllParams["TemplateName"].ToString();
            string Status = AllParams["Status"].ToString();
            string FileName = AllParams["FileName"].ToString();

            Dictionary<string, object> ldictParams = SetParams(astrFormID);
            ldictParams["PrimaryKey"] = AllParams["KeyField"].ToString();
            ldictParams["TemplateName"] = TemplateName;

            int lintTrackingId = Convert.ToInt32(FileName.Substring(FileName.LastIndexOf("-") + 1).Substring(0, FileName.LastIndexOf(".") - FileName.LastIndexOf("-") - 1));

            isrvServers.isrvBusinessTier.UpdateCorrespondenceTrackingStatus(lintTrackingId, Status, ldictParams);
            ViewModelObj vm = new ViewModelObj();
            vm.ExtraInfoFields = new Dictionary<string, string>();
            vm.ExtraInfoFields["FormId"] = astrFormID;
            vm.DomainModel = new utlResponseData();
            vm.ResponseMessage = new utlResponseMessage();

            if (Status == "")
                vm.ResponseMessage.istrMessage = "Correspondence Successfully Saved";
            if (Status == "PRNT")
                vm.ResponseMessage.istrMessage = "Correspondence Successfully Saved";

            vm.ExtraInfoFields["KeyField"] = AllParams["KeyField"].ToString();
            var lResult = Request.CreateResponse(HttpStatusCode.OK, vm);
            return lResult;
        }

        [HttpGet]
        public HttpResponseMessage GetFormForNext()
        {
            string lstrFormID = (string)HttpContext.Current.Session["SelectedForm"];
            HttpContext.Current.Session[lstrFormID + "SelectedIndex"] = (int)HttpContext.Current.Session[lstrFormID + "SelectedIndex"] + 1;
            ViewModelObj vm = GetData(utlPageMode.Update);
            vm.ResponseMessage = GetMessage(11);
            vm.ExtraInfoFields = new Dictionary<string, string>();
            int lintTotal = ((ArrayList)HttpContext.Current.Session[lstrFormID + "SelectedRows"]).Count;
            int lintSelectedIdx = (int)HttpContext.Current.Session[lstrFormID + "SelectedIndex"];
            vm.ExtraInfoFields["TotalRowsSelected"] = lintTotal.ToString();
            vm.ExtraInfoFields["SelectedIndex"] = lintSelectedIdx.ToString();
            vm.ExtraInfoFields["FormId"] = lstrFormID;
            vm.ExtraInfoFields["KeyField"] = vm.DomainModel.KeysData["PrimaryKey"];
            var lResult = Request.CreateResponse(HttpStatusCode.OK, vm);
            return lResult;
        }

        [HttpGet]
        public HttpResponseMessage GetFormForPrev()
        {
            string lstrFormID = (string)HttpContext.Current.Session["SelectedForm"];
            HttpContext.Current.Session[lstrFormID + "SelectedIndex"] = ((int)HttpContext.Current.Session[lstrFormID + "SelectedIndex"] - 1);
            ViewModelObj vm = GetData(utlPageMode.Update);
            vm.ResponseMessage = GetMessage(12);
            vm.ExtraInfoFields = new Dictionary<string, string>();
            int lintTotal = ((ArrayList)HttpContext.Current.Session[lstrFormID + "SelectedRows"]).Count;
            int lintSelectedIdx = (int)HttpContext.Current.Session[lstrFormID + "SelectedIndex"];
            vm.ExtraInfoFields["TotalRowsSelected"] = lintTotal.ToString();
            vm.ExtraInfoFields["SelectedIndex"] = lintSelectedIdx.ToString();
            vm.ExtraInfoFields["FormId"] = lstrFormID;
            vm.ExtraInfoFields["KeyField"] = vm.DomainModel.KeysData["PrimaryKey"];
            var lResult = Request.CreateResponse(HttpStatusCode.OK, vm);
            return lResult;
        }

        [HttpPost]
        public ViewModelObj DeleteRecord([FromUri] string astrFormID, [FromBody] IDictionary<string, object> aDeleteParams)
        {
            //string astrFormID = aDeleteParams["FormID"].ToString();
            int[] SelectedRowsIndex = aDeleteParams["SelectedRows"].ToString().Replace("[", "").Replace("]", "").Split(',').Select(i => int.Parse(i)).ToArray<int>();
            string lstrBusinessTierUrl = (string)ConfigurationManager.AppSettings["BusinessTierUrl"];
            lstrBusinessTierUrl = lstrBusinessTierUrl.Substring(0, lstrBusinessTierUrl.LastIndexOf('/'));
            lstrBusinessTierUrl += "/" + "srvAdmin";

            IBusinessTier lsrvBusinessTier = (IBusinessTier)Activator.GetObject(typeof(IBusinessTier), lstrBusinessTierUrl);
            Dictionary<string, object> ldictParams = new Dictionary<string, object>();
            Hashtable lhstUserDefaults = new Hashtable();

            ldictParams["SessionID"] = HttpContext.Current.Session.SessionID;
            ldictParams["ObjectID"] = aDeleteParams["ObjectID"];
            ldictParams["CollectionOf"] = aDeleteParams["CollectionOf"];
            ldictParams[utlConstants.istrConstFormName] = astrFormID;
            int lintUserSerialID = int.Parse(HttpContext.Current.Session["UserSerialID"].ToString());
            ldictParams[utlConstants.istrConstUserSerialID] = lintUserSerialID;

            ArrayList larrlstSummary = lsrvBusinessTier.MVVMDeleteRecord(SelectedRowsIndex, ldictParams);

            ViewModelObj vm = new ViewModelObj();
            if (larrlstSummary != null && larrlstSummary.Count > 0)
            {
                vm.ValidationSummary = larrlstSummary;
                //vm.ResponseMessage = new utlResponseMessage();
                vm.ResponseMessage = GetMessage(30);
            }
            else
            {
                vm.ResponseMessage = GetMessage(28); // Default values saved successfully
            }
            vm.ExtraInfoFields["FormId"] = astrFormID;

            return vm;
        }

        [HttpPost]
        public ViewModelObj StoreUserDefaults(IDictionary<string, string> aSearchParams)
        {
            string astrFormID = aSearchParams["FormID"];
            string astrParentTable = aSearchParams["ParentTable"];


            string lstrBusinessTierUrl = (string)ConfigurationManager.AppSettings["BusinessTierUrl"];
            lstrBusinessTierUrl = lstrBusinessTierUrl.Substring(0, lstrBusinessTierUrl.LastIndexOf('/'));
            lstrBusinessTierUrl += "/" + "srvAdmin";

            IBusinessTier lsrvBusinessTier = (IBusinessTier)Activator.GetObject(typeof(IBusinessTier), lstrBusinessTierUrl);
            Dictionary<string, object> ldictParams = new Dictionary<string, object>();
            Hashtable lhstUserDefaults = new Hashtable();

            foreach (KeyValuePair<string, string> control in aSearchParams)
            {
                if (control.Key == "FormID" || control.Key == "ParentTable")
                    continue;

                lhstUserDefaults.Add(control.Key, control.Value);
            }


            //TO DO : Amol : get activity log level & uncomment following
            //utlUserActivityLogParam lobjUserLogParameter;
            //if (iintUserActivityLogLevel > 1)
            //{
            //    foreach (object lobjKey in lhstUserDefaults.Keys)
            //    {
            //        if (lhstUserDefaults[lobjKey].ToString().Trim() != "")
            //        {
            //            lobjUserLogParameter = new utlUserActivityLogParam();
            //            lobjUserLogParameter.parameter_name = lobjKey.ToString();
            //            lobjUserLogParameter.parameter_value = lhstUserDefaults[lobjKey].ToString();
            //            icolActionParams.Add(lobjUserLogParameter);
            //        }
            //    }
            //}

            ldictParams[utlConstants.istrConstFormName] = astrFormID;
            int lintUserSerialID = int.Parse(HttpContext.Current.Session["UserSerialID"].ToString());
            ldictParams[utlConstants.istrConstUserSerialID] = lintUserSerialID;

            lsrvBusinessTier.StoreUserDefaults(astrParentTable, "Default", lhstUserDefaults, ldictParams);

            ViewModelObj vm = new ViewModelObj();
            vm.ResponseMessage = GetMessage(35); // Default values saved successfully

            vm.ExtraInfoFields["FormId"] = astrFormID;

            return vm;
        }

        private ViewModelObj GetData(utlPageMode aenmPageMode)
        {
            string lstrFormID = (string)HttpContext.Current.Session["SelectedForm"];
            string lstrRemoteObject = isrvServers.isrvMetaDataCache.GetRemoteObjectName(lstrFormID);

            string lstrBusinessTierUrl = (string)ConfigurationManager.AppSettings["BusinessTierUrl"];
            lstrBusinessTierUrl = lstrBusinessTierUrl.Substring(0, lstrBusinessTierUrl.LastIndexOf('/'));
            lstrBusinessTierUrl += "/" + lstrRemoteObject;

            IBusinessTier lsrvBusinessTier = (IBusinessTier)Activator.GetObject(typeof(IBusinessTier), lstrBusinessTierUrl);
            int lintSelectedIndex = (int)HttpContext.Current.Session[lstrFormID + "SelectedIndex"];
            ArrayList larrSelectedRows = (ArrayList)HttpContext.Current.Session[lstrFormID + "SelectedRows"];

            Dictionary<string, object> ldictParams = new Dictionary<string, object>();
            ldictParams[utlConstants.istrConstFormName] = lstrFormID;
            ldictParams[utlConstants.istrConstUserID] = HttpContext.Current.Session["UserID"].ToString();
            if (aenmPageMode == null || aenmPageMode == utlPageMode.All)
            {
                aenmPageMode = utlPageMode.Update;
            }
            ldictParams["PageMode"] = aenmPageMode;
            ldictParams["kendo"] = true;
            ldictParams["SessionID"] = HttpContext.Current.Session.SessionID;
            Hashtable lhstParams = (Hashtable)larrSelectedRows[lintSelectedIndex];
            ArrayList larrGridItems = new ArrayList();

            ArrayList larrResult = lsrvBusinessTier.MVVMGetInitialData(lhstParams, null, ldictParams);

            ViewModelObj vm = new ViewModelObj();

            vm.DomainModel = (utlResponseData)larrResult[0];
            vm.Errors = null;
            return vm;
        }

        private utlResponseMessage GetMessage(int aintMessageID, object[] aarrParam = null)
        {
            string lstrDBCacheUrl = ConfigurationManager.AppSettings["DBCacheUrl"].Split(new char[1] { ';' })[0];
            IDBCache lsrvDBCache = (IDBCache)Activator.GetObject(typeof(IDBCache), lstrDBCacheUrl);
            DataTable ldtbMessageInfo = lsrvDBCache.GetMessageInfo(aintMessageID);
            if (ldtbMessageInfo.Rows.Count == 0)
                return null;
            utlResponseMessage lobjResponseMessage = new utlResponseMessage();
            lobjResponseMessage.istrMessageID = "Msg ID : " + aintMessageID.ToString();
            if (aarrParam == null)
            {
                lobjResponseMessage.istrMessage = " [ " + ldtbMessageInfo.Rows[0]["display_message"].ToString() + " ]";
            }
            else
            {
                lobjResponseMessage.istrMessage = " [ " + String.Format(ldtbMessageInfo.Rows[0]["display_message"].ToString() + " ]", aarrParam);
            }

            return lobjResponseMessage;
        }

        [HttpPost]
        public void WriteAttachment(object adictFileContent)
        {
            HttpResponse Response = System.Web.HttpContext.Current.Response;
            Response.Clear();
            Response.ClearHeaders();
            Response.ClearContent();
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + "temp.xls");
            Response.ContentType = "application/vnd.ms-excel";
            try
            {
                Response.Write(System.Web.HttpContext.Current.Request.Form["content"].ToString());
            }
            catch (Exception ex)
            {
                Response.Write(System.Web.HttpContext.Current.Request.Form["content"].ToString());
            }
            Response.End();
        }

        private string CalculateMD5Hash(utlResponseData input)
        {
            var lobjMaintData = input.HeaderData["MaintenanceData"];
            StringBuilder values = new StringBuilder();
            foreach (var lstrKey in lobjMaintData.Keys)
            {
                values.Append(lstrKey + ":" + lobjMaintData[lstrKey].ToString() + ";");
            }

            // step 1, calculate MD5 hash from input
            return GetHashCodeForString(values.ToString());
        }

        private string GetHashCodeForString(string InputString)
        {
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(InputString);
            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString());
            }
            return sb.ToString();
        }

        [HttpPost]
        public List<string> getChangedFormsTemplates(Dictionary<string, string>[] FormHashcodes)
        {
            ViewModelObj vm = new ViewModelObj();
            List<string> FormsChanged = new List<string>();

            string template;
            string hash;
            HtmlGen html = new HtmlGen(isrvServers);

            foreach (Dictionary<string, string> FormInfo in FormHashcodes)
            {
                if (FormInfo["Type"] == "Form")
                {
                    template = html.GetHtmlTemplate(FormInfo["FormId"], bool.Parse(FormInfo["IsCenterLeft"]));
                    hash = GetHashCodeForString(template);
                    if (hash != FormInfo["HashCode"])
                    {
                        FormsChanged.Add(FormInfo["StorageKey"]);
                    }
                }
                else if (FormInfo["Type"] == "CodeValue")
                {
                    int codeid = Convert.ToInt32(FormInfo["CodeID"]);
                    string LastModificationDate = isrvServers.isrvDbCache.GetLastModifiedDateForCoveGroup(codeid);
                    if (LastModificationDate != FormInfo["HashCode"])
                    {
                        FormsChanged.Add(FormInfo["StorageKey"]);
                    }
                }
            }
            return FormsChanged;
        }

        [HttpPost]
        public HttpResponseMessage ValidateNew([FromUri] string astrFormID, [FromUri] string astrActiveForm, [FromBody] IDictionary<string, string> adctNavigationParams)
        {
            isrvServers.ConnectToBT(astrFormID);

            Dictionary<string, object> ldictParams = new Dictionary<string, object>();
            ldictParams[utlConstants.istrConstFormName] = astrFormID;
            ldictParams[utlConstants.istrConstUserID] = HttpContext.Current.Session["UserID"].ToString();
            ldictParams["SessionID"] = HttpContext.Current.Session.SessionID;
            Hashtable lhstParams = new Hashtable();
            foreach (string lstrKey in adctNavigationParams.Keys)
            {
                lhstParams[lstrKey] = adctNavigationParams[lstrKey];
            }

            ArrayList larrlstErrors = isrvServers.isrvBusinessTier.ValidateNew(lhstParams, ldictParams);
            if (larrlstErrors == null)
                larrlstErrors = new ArrayList();
            ViewModelObj vm = new ViewModelObj();
            if (larrlstErrors.Count == 0)
            {
                isrvServers.ConnectToBT(astrActiveForm);
                HttpContext.Current.Session["SelectedForm"] = astrActiveForm;
                HttpContext.Current.Session[astrActiveForm + "SelectedIndex"] = 0;
                HttpContext.Current.Session[astrActiveForm + "SelectedRows"] = new ArrayList { lhstParams };
                vm = GetData(utlPageMode.New);
                vm.ExtraInfoFields["FormId"] = astrActiveForm;
                vm.ExtraInfoFields["IsNewForm"] = "true";
            }
            else
            {
                vm.ValidationSummary = larrlstErrors;
                vm.ResponseMessage = new utlResponseMessage();
                vm.ExtraInfoFields["FormId"] = astrFormID;
            }
            var lResult = Request.CreateResponse(HttpStatusCode.OK, vm);
            return lResult;
        }

        [HttpPost]
        public HttpResponseMessage GetAutoCompFormResult(Dictionary<string, string>[] SearchInfo)
        {
            //To Do Add Code To reutrn data wfmCenterLeftOrganizationLookup

            Collection<utlWhereClause> lcolWhereClause = new Collection<utlWhereClause>();
            utlWhereClause lobjWhereClause;

            string lstrFormID = SearchInfo[0]["FormID"].ToString();
            string lstrQueryID = SearchInfo[0]["QueryID"].ToString();

            for (int i = 1; i < SearchInfo.Length; i++)
            {
                if (string.IsNullOrEmpty(SearchInfo[i]["value"]))
                    continue;

                lobjWhereClause = new utlWhereClause();
                lobjWhereClause.iobjValue1 = SearchInfo[i]["value"].Trim();
                lobjWhereClause.iobjValue2 = "";
                lobjWhereClause.istrQueryId = lstrQueryID;
                lobjWhereClause.istrFieldName = SearchInfo[i]["field"];
                lobjWhereClause.istrDataType = SearchInfo[i]["type"];
                lobjWhereClause.istrOperator = SearchInfo[i]["operator"];

                if (lcolWhereClause.Count > 0)
                    lobjWhereClause.istrCondition = "and";

                lcolWhereClause.Add(lobjWhereClause);
            }

            Dictionary<string, string> FormInfo = new Dictionary<string, string>();
            FormInfo.Add("FormID", SearchInfo[0]["FormID"]);

            HttpResponseMessage resp = GetSearchResult(FormInfo, lcolWhereClause);

            return resp;
        }

        [HttpPost]
        public Dictionary<string, string> GetCodeValues(Dictionary<string, string> SearchInfo)
        {
            int CodeID = int.Parse(SearchInfo["CodeGroupID"]);
            if (iDictCodeValues.ContainsKey(CodeID))
                return iDictCodeValues[CodeID];
            else
            {
                Hashtable lhstParamValues = new Hashtable();
                string CodeValueQuery = "SELECT CODE_VALUE,DESCRIPTION FROM SGS_CODE_VALUE where CODE_ID=" + CodeID.ToString();
                lhstParamValues["astrQuery"] = CodeValueQuery;
                DataTable CodeValueTable = (DataTable)isrvServers.isrvDbCache.GetCodeValues(CodeID);
                Dictionary<string, string> ldictCodeValue = new Dictionary<string, string>();
                foreach (DataRow dr in CodeValueTable.Rows)
                {
                    ldictCodeValue.Add(dr["CODE_VALUE"].ToString(), dr["DESCRIPTION"].ToString());
                }

                iDictCodeValues.Add(CodeID, ldictCodeValue);
                return ldictCodeValue;
            }
        }

        [HttpPost]
        public HttpResponseMessage GetReportDropDown(Dictionary<string, string> ReportParams)
        {
            string istrReportCategory = "";//Get from query string
            Hashtable lhstReports = isrvServers.isrvMetaDataCache.GetListOfReports();
            string lstrReportCategory = istrReportCategory;
            if (string.IsNullOrEmpty(lstrReportCategory))
            {
                lstrReportCategory = ReportParams["ReportCategory"];
            }

            utlObjectData DropDownValue = null;
            utlObjectData ListContainer = null;
            List<utlObjectData> ListForReport = new List<utlObjectData>();

            foreach (object lobjTemp in lhstReports.Keys)
            {
                utlReportDetails lobjReportDetails = (utlReportDetails)lhstReports[lobjTemp.ToString()];
                // Don't load the reports which do not have report definition file under webserver reports folder
                // This is added to aviod Load Report Failed exception
                if (!File.Exists(HostingEnvironment.MapPath("~/reports") + "\\" + lobjReportDetails.istrReportName))
                {
                    continue;
                }
                if (!string.IsNullOrEmpty(lstrReportCategory) && (lstrReportCategory != lobjReportDetails.istrCategory))
                {
                    continue;
                }
                if (lobjReportDetails.iintResourceID > 0)
                {
                    if ((int)GetSecurity(lobjReportDetails.iintResourceID)[0] <= 0)
                    {
                        continue;
                    }
                }

                DropDownValue = new utlObjectData();
                DropDownValue.Add("text", lobjReportDetails.istrDescription);
                DropDownValue.Add("value", lobjTemp.ToString());
                ListForReport.Add(DropDownValue);
            }
            ListContainer = new utlObjectData();
            ListContainer.Add("FilteredValues", ListForReport);

            ViewModelObj vm = new ViewModelObj();
            vm.DomainModel = new utlResponseData();
            vm.DomainModel.HeaderData.Add("RptValues", ListContainer);

            var lResult = Request.CreateResponse(HttpStatusCode.OK, vm);
            return lResult;
        }

        protected virtual object[] GetSecurity(int aintResourceID)
        {
            object[] larrResult = new object[2] { 0, "" };
            DataTable ldtbUserSecurity = (DataTable)HttpContext.Current.Session["UserSecurity"];
            if (ldtbUserSecurity == null)
            {
                // DisplayError("Error occured, unable to find UserSecurity", null);
                return larrResult;
            }
            DataRow[] ldtrCacheRows = ldtbUserSecurity.Select("resource_id = " + aintResourceID.ToString());

            if (ldtrCacheRows.Length == 1)
            {
                larrResult[0] = Convert.ToInt32(ldtrCacheRows[0]["security_level"]);
                larrResult[1] = (string)ldtrCacheRows[0]["resource_description"];
            }

            return larrResult;
        }

        //Get Reoprt Data n Put it in session
        [HttpPost]
        public HttpResponseMessage GenerateReportData(object aParams)
        {
            isrvServers.ConnectToBT("wfmReportClient");

            var jss = new JavaScriptSerializer();
            Dictionary<string, object> AllParams = (Dictionary<string, object>)jss.Deserialize<Dictionary<string, object>>(aParams.ToString());

            Dictionary<string, object> ldictParams = SetParams("wfmReportClient");

            Hashtable lhstReportParams = new Hashtable();

            foreach (KeyValuePair<string, object> kvp in AllParams)
            {
                if (kvp.Key != "astrQueryID" && kvp.Key != "astrMethodName")
                    lhstReportParams.Add(kvp.Key, kvp.Value.ToString());
            }

            ViewModelObj vm = new ViewModelObj();

            DataSet ldstResult = null;
            if (AllParams.ContainsKey("astrQueryID"))
            {
                string astrQueryID = AllParams["astrQueryID"].ToString();
                DataTable ldtbResult = isrvServers.isrvBusinessTier.ExecuteQuery(astrQueryID, lhstReportParams, ldictParams);
                ldtbResult.TableName = "ReportTable01";
                ldstResult = new DataSet();
                ldstResult.Tables.Add(ldtbResult);
            }
            else if (AllParams.ContainsKey("astrMethodName"))
            {
                string astrMethodName = AllParams["astrMethodName"].ToString();
                ldstResult = (DataSet)isrvServers.isrvBusinessTier.ExecuteMethod(astrMethodName, lhstReportParams, false, ldictParams);
            }
            else
            {

                vm.ValidationSummary.Add("No Query or Method is specified; report could not be generated.");
                //DisplayError("No Query or Method is specified; report could not be generated.", null);
            }

            ArrayList larrResult = new ArrayList() { ldstResult };
            if (ldstResult.Tables.Count > 0)
                vm.ExtraInfoFields["DataGeneratd"] = "true";
            else
                vm.ExtraInfoFields["DataGeneratd"] = "false";

            HttpContext.Current.Session["MVVMReportDataSource"] = ldstResult;
            vm.ResponseMessage = new utlResponseMessage();
            vm.ResponseMessage.istrMessage = "Report Successfully Generated";
            // DisplayMessage("Report Successfully Generated");
            var lResult = Request.CreateResponse(HttpStatusCode.OK, vm);
            return lResult;
        }

        [HttpPost]
        public HttpResponseMessage SubmitFile()
        {
            // Verify that this is an HTML Form file upload request
            if (!Request.Content.IsMimeMultipartContent("form-data"))
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            var appData = HostingEnvironment.MapPath("~/App_Data");

            // Create a stream provider for setting up output streams
            MultipartFormDataStreamProvider streamProvider = new MultipartFormDataStreamProvider(appData);

            // Read the MIME multipart content using the stream provider we just created.
            var bodyparts = Request.Content.ReadAsMultipartAsync();

            // Get a dictionary of local file names from stream provider.
            // The filename parameters provided in Content-Disposition header fields are the keys.
            // The local file names where the files are stored are the values.

            var lResult = Request.CreateResponse(HttpStatusCode.OK);
            return lResult;
        }

        [HttpPost]
        public HttpResponseMessage GetDropDownValues(Dictionary<string, string> DropDownParams)
        {
            bool ablnIsDropDownList = DropDownParams["isdropdown"].ToLower() == "true" ? true : false;

            if (!DropDownParams.ContainsKey("sfwaddemptyitem"))
            {
                DropDownParams.Add("sfwaddemptyitem", "true");
            }

            utlObjectData DropDownValue = null;
            utlObjectData ListContainer = null;
            List<utlObjectData> ListForDropDown = new List<utlObjectData>();
            ViewModelObj vm = new ViewModelObj();

            DataTable ldtOptions = null;

            string InputType = "";

            if (DropDownParams.ContainsKey("controltype"))
            {
                switch (DropDownParams["controltype"])
                {
                    case "sfwCheckBoxList": InputType = "checkbox";
                        break;
                    case "sfwRadioButtonList": InputType = "radio";
                        break;
                }
            }

            string ControlID;
            if (DropDownParams.ContainsKey("id"))
            {
                ControlID = DropDownParams["id"];
            }
            else
            {
                ControlID = "";
            }

            string ParentSection = "MaintenanceData";

            if (DropDownParams["formname"].IndexOf("Lookup") > 0)
                ParentSection = DropDownParams["parenttable"];

            string DrpSelected = "<option selected=\"true\" value=\"{0}\">{1}</option>";
            string DrpNormal = "<option value=\"{0}\">{1}</option>";

            string RdoSelected = "<label><input type=\"" + InputType + "\" data-bind=\"checked:{0}\" checked=\"true\" name=\"{1}\" value=\"{2}\"/>{3}</label>";
            string RdoNormal = "<label><input type=\"" + InputType + "\" data-bind=\"checked:{0}\" name=\"{1}\" value=\"{2}\"/>{3}</label>";

            StringBuilder lsbObtions = new StringBuilder();

            string lstrDataTextField = "description";
            string lstrDataValueField = "code_value";
            string lstrDefaultValue = "";

            string LastModificationDate = "";
            if (DropDownParams.ContainsKey("sfwdefaultvalue"))
                lstrDefaultValue = DropDownParams["sfwdefaultvalue"];

            if (DropDownParams.ContainsKey("datatextfield"))
                lstrDataTextField = DropDownParams["datatextfield"];

            if (DropDownParams.ContainsKey("datavaluefield"))
                lstrDataValueField = DropDownParams["datavaluefield"];

            if (DropDownParams.ContainsKey("sfwcodetable"))
            {
                ldtOptions = BindListControl(DropDownParams, ablnIsDropDownList);
            }
            else if ((DropDownParams.ContainsKey("sfwcodemethod")) && (DropDownParams.ContainsKey("sfwobjectid")))
            {
                ldtOptions = PopulateListFromMethod(DropDownParams, ablnIsDropDownList);
            }
            else if (DropDownParams.ContainsKey("sfwcodegroup") && DropDownParams["sfwcodegroup"] != "0")
            {
                int codeid = Convert.ToInt32(DropDownParams["sfwcodegroup"]);
                ldtOptions = PopulateListFromCodeGroup(DropDownParams, ablnIsDropDownList);

                LastModificationDate = isrvServers.isrvDbCache.GetLastModifiedDateForCoveGroup(codeid);
                if ((DropDownParams["sfwcodegroup"] == "3") || (DropDownParams["sfwcodegroup"] == "4") || (DropDownParams["sfwcodegroup"] == "8"))
                {
                    lstrDataValueField = "data1";
                    if (DropDownParams["sfwcodegroup"] == "8")
                    {
                        lstrDefaultValue = "in";
                    }
                    else
                    {
                        lstrDefaultValue = "=";
                    }
                }
            }

            if (ldtOptions != null && ldtOptions.Rows.Count > 0)
            {
                int i = 0;
                //lsbObtions.Append("<AllOptions>");

                foreach (DataRow dr in ldtOptions.Rows)
                {
                    if (ablnIsDropDownList)
                    {

                        DropDownValue = new utlObjectData();
                        DropDownValue.Add("text", dr[lstrDataTextField].ToString());
                        DropDownValue.Add("value", dr[lstrDataValueField].ToString());
                        ListForDropDown.Add(DropDownValue);

                        if (i == 0 || lstrDefaultValue == dr[lstrDataValueField].ToString())
                            lsbObtions.Append(string.Format(DrpSelected, dr[lstrDataValueField], dr[lstrDataTextField]));
                        else
                            lsbObtions.Append(string.Format(DrpNormal, dr[lstrDataValueField], dr[lstrDataTextField]));
                        i++;
                    }
                    else
                    {
                        int RepeatColumns = 1;
                        if (DropDownParams.ContainsKey("repeatcolumns"))
                        {
                            RepeatColumns = int.Parse(DropDownParams["repeatcolumns"]);
                        }

                        if (lstrDefaultValue == dr[lstrDataValueField].ToString())
                            lsbObtions.Append(string.Format(RdoSelected, ParentSection + "." + ControlID, ControlID, dr[lstrDataValueField], dr[lstrDataTextField]));
                        else
                            lsbObtions.Append(string.Format(RdoNormal, ParentSection + "." + ControlID, ControlID, dr[lstrDataValueField], dr[lstrDataTextField]));

                        i++;

                        if (i % RepeatColumns == 0 && i < ldtOptions.Rows.Count)
                        {
                            lsbObtions.Append("<br/><br/>");
                        }
                        else
                        {
                            lsbObtions.Append("<span class='CheckBoxListGap'> </span>");
                        }
                    }
                }
                // lsbObtions.Append("</AllOptions>");
            }

            ListContainer = new utlObjectData();
            ListContainer.Add("Options", ListForDropDown);

            vm.DomainModel = new utlResponseData();
            vm.DomainModel.HeaderData.Add("DropDownValues", ListContainer);
            vm.ExtraInfoFields = new Dictionary<string, string>();
            vm.Template = lsbObtions.ToString();
            vm.ExtraInfoFields.Add("LastModificationDate", LastModificationDate);
            var lResult = Request.CreateResponse(HttpStatusCode.OK, vm);
            return lResult;
        }

        public DataTable PopulateListFromCodeGroup(Dictionary<string, string> attributes, bool ablnIsDropDownList)
        {
            string islookup = attributes["islookup"];
            DataTable ldtbCodeValue = isrvServers.isrvDbCache.GetCodeValues(int.Parse(attributes["sfwcodegroup"]));

            string DataTextField = "description", DataValueField = "code_value";

            if (ldtbCodeValue.Rows.Count > 0)
            {
                if (attributes.ContainsKey("datatextfield"))
                    DataTextField = attributes["datatextfield"];
                if (attributes.ContainsKey("datavaluefield"))
                    DataValueField = attributes["datavaluefield"];

                string lstrFirstValue;

                DataTable ldtbCode = isrvServers.isrvDbCache.GetCodeInfo(int.Parse(attributes["sfwcodegroup"]));

                if (ablnIsDropDownList)
                {
                    if (attributes.ContainsKey("sfwaddemptyitem") && attributes["sfwaddemptyitem"] == "true")
                    {
                        if (attributes.ContainsKey("sfwfirstitemtext"))
                        {
                            lstrFirstValue = attributes["sfwfirstitemtext"];
                        }
                        else if (islookup == "true")
                        {
                            if (!Convert.IsDBNull(ldtbCode.Rows[0]["first_lookup_item"]))
                            {
                                lstrFirstValue = ldtbCode.Rows[0]["first_lookup_item"].ToString();
                            }
                            else
                            {
                                lstrFirstValue = "All";
                            }
                        }
                        else
                        {
                            if (!Convert.IsDBNull(ldtbCode.Rows[0]["first_maintenance_item"]))
                            {
                                lstrFirstValue = ldtbCode.Rows[0]["first_maintenance_item"].ToString();
                            }
                            else
                            {
                                lstrFirstValue = "";
                            }
                        }

                        // If the value is 'NA' in database, don't add the first item
                        if (lstrFirstValue != "NA")
                        {
                            DataRow ldtrZero = ldtbCodeValue.NewRow();
                            ldtrZero[DataTextField] = lstrFirstValue;

                            if (ldtbCodeValue.Columns[DataValueField].DataType.Name.StartsWith("Int"))
                            {
                                ldtrZero[DataValueField] = 0;
                            }
                            else
                            {
                                ldtrZero[DataValueField] = "";
                            }

                            ldtbCodeValue.Rows.InsertAt(ldtrZero, 0);
                        }
                    }
                }
            }

            return ldtbCodeValue;
        }

        /// <summary>
        /// Populates a list control using a method
        /// </summary>
        /// <param name="attributes">Specifies list control</param>
        protected DataTable PopulateListFromMethod(Dictionary<string, string> attributes, bool ablnIsDropDownList)
        {
            Hashtable lhstParam = new Hashtable();
            Dictionary<string, object> ldictParams = new Dictionary<string, object>();

            string FormName = attributes["formname"];
            string Mode = "All";

            ldictParams[utlConstants.istrConstFormName] = FormName;
            ldictParams[utlConstants.istrConstPageMode] = Mode;
            ldictParams["SessionID"] = HttpContext.Current.Session.SessionID.ToString();

            isrvServers.ConnectToBT(FormName);

            ICollection lcolDataValue = null;

            if (attributes.ContainsKey("sfwcodemethod") && attributes.ContainsKey("sfwobjectid"))
            {
                ldictParams["GetObjectFromSession"] = true;
                if (attributes.ContainsKey("primarykey"))
                    ldictParams[utlConstants.istrBOPrimaryKey] = attributes["primarykey"];
                else
                    ldictParams[utlConstants.istrBOPrimaryKey] = 0;

                lcolDataValue = (ICollection)isrvServers.isrvBusinessTier.ExecuteMethod(attributes["sfwobjectid"], attributes["sfwcodemethod"], new ArrayList(), lhstParam, ldictParams);
            }
            else
            {
                lcolDataValue = (ICollection)isrvServers.isrvBusinessTier.ExecuteMethod(attributes["sfwcodemethod"], lhstParam, false, ldictParams);
            }

            DataTable ldtOptions = new DataTable();
            string DataTextField = attributes["datatextfield"];
            string DataValueField = attributes["datavaluefield"];

            ldtOptions.Columns.Add(DataTextField);
            if (!ldtOptions.Columns.Contains(DataValueField))
                ldtOptions.Columns.Add(DataValueField);

            if (lcolDataValue != null)
            {
                foreach (object item in lcolDataValue)
                {
                    DataRow row = ldtOptions.NewRow();
                    row[DataTextField] = item.GetType().GetProperty(DataTextField).GetValue(item, null);
                    row[DataValueField] = item.GetType().GetProperty(DataValueField).GetValue(item, null);
                    ldtOptions.Rows.Add(row);
                }
            }

            if (ablnIsDropDownList)
            {
                if ((lcolDataValue != null) && (lcolDataValue.Count > 0))
                {
                    DataRow row = ldtOptions.NewRow();
                    if (attributes["sfwaddemptyitem"] == null || attributes["sfwaddemptyitem"] == "true")
                    {
                        if (attributes.ContainsKey("sfwfirstitemtext"))
                        {
                            row[DataTextField] = attributes["sfwfirstitemtext"];
                            row[DataValueField] = "";

                        }
                        else if (FormName.Contains("Lookup"))
                        {
                            row[DataTextField] = "Any";
                            row[DataValueField] = "";
                        }
                        else
                        {
                            row[DataTextField] = "None Selected";
                            row[DataValueField] = "";
                        }
                    }
                    ldtOptions.Rows.Insert(0, row);
                }
            }
            return ldtOptions;
        }


        /// <summary>
        /// Populates a list control using a query
        /// </summary>
        /// <param name="attributes">Specifies list control</param>
        public DataTable BindListControl(Dictionary<string, string> attributes, bool ablnIsDropDownList)
        {

            string FormName = attributes["formname"];

            Dictionary<string, object> ldictParams = new Dictionary<string, object>();
            ldictParams[utlConstants.istrConstFormName] = FormName;

            isrvServers.ConnectToBT(FormName);

            DataTable ldtbCodeValue = isrvServers.isrvBusinessTier.ExecuteQuery(attributes["sfwcodetable"], ldictParams);

            if (ablnIsDropDownList)
            {
                if (ldtbCodeValue.Rows.Count > 0)
                {
                    if (attributes.ContainsKey("sfwaddemptyitem") && attributes["sfwaddemptyitem"] == "true")
                    {
                        DataRow ldtrZero = ldtbCodeValue.NewRow();
                        if (ldtbCodeValue.Columns[attributes["datavaluefield"]].DataType.Name.ToLower().Contains("decimal"))
                        {
                            ldtrZero[attributes["datavaluefield"]] = (decimal)0;
                        }
                        else if (ldtbCodeValue.Columns[attributes["datavaluefield"]].DataType.Name.StartsWith("Int"))
                        {
                            ldtrZero[attributes["datavaluefield"]] = 0;
                        }
                        else
                        {
                            ldtrZero[attributes["datavaluefield"]] = "";
                        }

                        if (attributes.ContainsKey("sfwfirstitemtext"))
                        {
                            ldtrZero[attributes["datatextfield"]] = attributes["sfwfirstitemtext"];
                        }
                        else if (FormName.Contains("Lookup"))
                        {
                            ldtrZero[attributes["datatextfield"]] = "All";
                        }
                        else
                        {
                            if (ldtbCodeValue.Columns[attributes["datavaluefield"]].DataType.Name.ToLower().Contains("decimal"))
                            {
                                ldtrZero[attributes["datavaluefield"]] = (decimal)0;
                            }
                            else if (ldtbCodeValue.Columns[attributes["datatextfield"]].DataType.Name.StartsWith("Int"))
                            {
                                ldtrZero[attributes["datatextfield"]] = 0;
                            }
                            else
                            {
                                ldtrZero[attributes["datatextfield"]] = "";
                            }
                        }
                        ldtbCodeValue.Rows.InsertAt(ldtrZero, 0);
                    }
                }
            }
            return ldtbCodeValue;

        }

        [HttpPost]
        public HttpResponseMessage AddNewChild(Dictionary<string, string> AllParams)
        {
            string astrFormID = AllParams["FormID"].ToString();
            isrvServers.ConnectToBT(astrFormID);
            Dictionary<string, object> ldictParams = SetParams(astrFormID);
            ldictParams["PrimaryKey"] = AllParams["PrimaryKey"].ToString();
            ldictParams["GridObjectID"] = AllParams["GridObjectID"].ToString();
            ldictParams["GridID"] = AllParams["GridID"].ToString();

            Hashtable lhstParams = new Hashtable();

            ArrayList larrResult = isrvServers.isrvBusinessTier.MVVMCreateObject(lhstParams, ldictParams);

            ViewModelObj vm = new ViewModelObj();

            vm.ResponseMessage = new utlResponseMessage();
            if (larrResult != null && larrResult.Count > 0 && larrResult[0] is utlError)
            {
                vm.ResponseMessage.istrMessage = ((utlError)larrResult[0]).istrErrorMessage;
                vm.ValidationSummary.Add(vm.ResponseMessage.istrMessage);
            }
            else
            {
                vm.DomainModel = (utlResponseData)larrResult[0];
                vm.ResponseMessage.istrMessage = "New row added to the grid.";
                vm.Errors = null;
            }

            var lResult = Request.CreateResponse(HttpStatusCode.OK, vm);
            return lResult;
        }
    }
}