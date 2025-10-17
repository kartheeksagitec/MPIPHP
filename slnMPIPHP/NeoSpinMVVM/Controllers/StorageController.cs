#region [Using directives]
using Newtonsoft.Json;
using Sagitec.Common;
using Sagitec.Interface;
using Sagitec.MVVMClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Script.Serialization;
#endregion [Using directives]

namespace Neo.Controllers
{
    /// <summary>
    /// Class Neo.StorageController
    /// </summary>
    public class StorageController : StorageControllerBase
    {

        #region [Overriden Methods]
        /// <summary>
        /// Overriding  method to save 'save for later' flag when user clicks on save button on correspondence editor Tool.
        /// </summary>
        /// <param name="aParams">Object Parameters</param>
        /// <returns>Http Responce message</returns>
        [HttpPost]
        public override HttpResponseMessage UpdateCorrespondenceStatus(object aParams)
        {
            IBusinessTier lsrvBusinessTier = null;
            Dictionary<string, object> AllParams = null;
            try
            {
                var jss = new JavaScriptSerializer();
                AllParams = (Dictionary<string, object>)jss.Deserialize<Dictionary<string, object>>(aParams.ToString());

                string lstrBusinessTierUrl = string.Format(srvServers.istrConfigBusinessTierURL, "srvCommon");

                lsrvBusinessTier = WCFClient<IBusinessTier>.CreateChannel(lstrBusinessTierUrl);

                string Status = (string)AllParams["Status"];

                string StatusCopy = Status;

                if (Status == "PRINT")
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
        public override HttpResponseMessage CorrToolCustomButtonClick(object aParams)
        {
            IBusinessTier lsrvBusinessTier = null;
            Dictionary<string, object> AllParams = null;
            try
            {
                var jss = new JavaScriptSerializer();
                AllParams = (Dictionary<string, object>)jss.Deserialize<Dictionary<string, object>>(aParams.ToString());

                string lstrBusinessTierUrl = string.Format(srvServers.istrConfigBusinessTierURL, "srvCommon");

                lsrvBusinessTier = WCFClient<IBusinessTier>.CreateChannel(lstrBusinessTierUrl);

                string TemplateName = (string)AllParams["TemplateName"];
                string FileName = (string)AllParams["FileName"];

                Dictionary<string, object> ldictParams = new Dictionary<string, object>();
                ldictParams["UserID"] = AllParams["UserID"].ToString();
                ldictParams["UserSerialID"] = Convert.ToInt32(AllParams["UserSerialID"].ToString());
                int lintTrackingID = Convert.ToInt32(AllParams["TrackingID"].ToString());

                ArrayList larrResult = null;
                string lstrCustomMethod = AllParams["CustomMethod"].ToString();

                if (lstrCustomMethod == "CorrToolCustomButton")
                {
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

        #endregion
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