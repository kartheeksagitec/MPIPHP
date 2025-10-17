using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.Interface;
using Sagitec.WebClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Script.Serialization;
/// <summary>
/// Summary description for CorrEditorController
/// </summary>
namespace Neo
{
    public class CorrEditorController : wfmBaseController
    {
        [HttpPost]
        public HttpResponseMessage UpdateCorrespondenceStatus_Override(object aParams)
        {
            IBusinessTier lsrvBusinessTier = null;
            Dictionary<string, object> AllParams = null;
            try
            {
                var jss = new JavaScriptSerializer();
                AllParams = (Dictionary<string, object>)jss.Deserialize<Dictionary<string, object>>(aParams.ToString());

                string lstrRemoteObject = "srvCommon";
                string lstrBusinessTierUrl = System.Configuration.ConfigurationManager.AppSettings["BusinessTierUrl"].Split(new char[1] { ';' })[0];
                lstrBusinessTierUrl = lstrBusinessTierUrl.Substring(0, lstrBusinessTierUrl.LastIndexOf('/'));
                lstrBusinessTierUrl += "/" + lstrRemoteObject;
                //FM upgrade: 6.0.0.30 changes
                //lsrvBusinessTier = srvMainDBAccessProxy.GetObject(null, lstrBusinessTierUrl);
                lsrvBusinessTier = WCFClient<IBusinessTier>.CreateChannel(lstrBusinessTierUrl);

                string Status = (string)AllParams["Status"];

                string StatusCopy = Status;

                if(Status == "PRINT")
                {
                    Status = "PRNT";
                    StatusCopy = Status;
                }

                if (Status == "SAVE")
                {
                    Status = "GENR";
                }

                string FileName = (string)AllParams["FileName"];

                Dictionary<string, object> ldictParams = new Dictionary<string, object>();

                ldictParams["UserID"] = AllParams["UserID"].ToString();

                int lintTrackingId = GetTrackingIDFromFileName(FileName);
                if (lintTrackingId > 0)
                {
                    //FM upgrade: 6.0.10.0 changes
                    //lsrvBusinessTier.UpdateCorrespondenceTrackingStatus(lintTrackingId, Status, ldictParams);
                    lsrvBusinessTier.UpdateCorrTrackingStatus(lintTrackingId, Status, ldictParams);
                }

                if (AllParams.ContainsKey("CorrFileData"))
                {
                    lsrvBusinessTier.UpdateGeneratedCorrespondence(FileName, AllParams["CorrFileData"].ToString(), ldictParams);
                }

                if (StatusCopy == "SAVE")
                    return Request.CreateResponse(HttpStatusCode.OK, "Correspondence Successfully Saved");
                if (StatusCopy == "PRNT")
                    return Request.CreateResponse(HttpStatusCode.OK, "Correspondence Successfully Printed");

                return Request.CreateResponse(HttpStatusCode.OK, "success");

            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, e.Message);
            }
            finally
            {
                HelperFunction.CloseChannel(lsrvBusinessTier);
            }
        }

        [HttpPost]
        public HttpResponseMessage CorrToolCustomButtonClick_Override(object aParams)
        {
            IBusinessTier lsrvBusinessTier = null;
            Dictionary<string, object> AllParams = null;
            try
            {
                var jss = new JavaScriptSerializer();
                AllParams = (Dictionary<string, object>)jss.Deserialize<Dictionary<string, object>>(aParams.ToString());

                string lstrRemoteObject = "srvCommon";
                string lstrBusinessTierUrl = System.Configuration.ConfigurationManager.AppSettings["BusinessTierUrl"].Split(new char[1] { ';' })[0];
                lstrBusinessTierUrl = lstrBusinessTierUrl.Substring(0, lstrBusinessTierUrl.LastIndexOf('/'));
                lstrBusinessTierUrl += "/" + lstrRemoteObject;
                //FM upgrade: 6.0.0.30 changes
                //lsrvBusinessTier = srvMainDBAccessProxy.GetObject(null, lstrBusinessTierUrl);
                lsrvBusinessTier = WCFClient<IBusinessTier>.CreateChannel(lstrBusinessTierUrl);
                string TemplateName = (string)AllParams["TemplateName"];
                string FileName = (string)AllParams["FileName"];

                Dictionary<string, object> ldictParams = new Dictionary<string, object>();
                ldictParams["UserID"] = AllParams["UserID"].ToString();
                ldictParams["UserSerialID"] = Convert.ToInt32(AllParams["UserSerialID"].ToString());
                int lintTrackingID = Convert.ToInt32(AllParams["TrackingID"].ToString());

                ArrayList larrResult = null;
                string lstrCustomMethod = AllParams["CustomMethod"].ToString();

                //FM upgrade: 6.0.0.25 changes
                //if (lstrCustomMethod == "CorrToolCustomButton1")
                //{
                //    //larrResult = lsrvBusinessTier.CorrToolCustomButton1(AllParams, ldictParams);
                //}
                //else if (lstrCustomMethod == "CorrToolCustomButton2")
                //{
                //    //larrResult = lsrvBusinessTier.CorrToolCustomButton2(AllParams, ldictParams);
                //}
                if (lstrCustomMethod == "CorrToolCustomButton")
                {
                    //Hashtable lhstParams = new Hashtable();
                    //lhstParams.Add("adictCorrInfo", AllParams);
                    //larrResult = (ArrayList)lsrvBusinessTier.ExecuteMethod("CorrToolCustomButton", lhstParams, false, ldictParams);
                    
                    larrResult = lsrvBusinessTier.CorrespondenceToolCustomButton(AllParams, ldictParams);
                }

                if (larrResult != null && larrResult.Count > 0 && larrResult[0] is utlError)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, ((utlError)larrResult[0]).istrErrorMessage);
                }

                var lResult = Request.CreateResponse(HttpStatusCode.OK, "Method Successfully Executed.");
                return lResult;
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, e.Message);
            }
            finally
            {
                HelperFunction.CloseChannel(lsrvBusinessTier);
            }
        }

        public virtual int GetTrackingIDFromFileName(string astrFileName)
        {
            int lintTrackingId = 0;
            if (astrFileName.Contains("-"))
            {
                lintTrackingId = Convert.ToInt32(astrFileName.Substring(astrFileName.LastIndexOf("-") + 1).Substring(0, astrFileName.LastIndexOf(".") - astrFileName.LastIndexOf("-") - 1));
            }
            return lintTrackingId;
        }

    }
}
