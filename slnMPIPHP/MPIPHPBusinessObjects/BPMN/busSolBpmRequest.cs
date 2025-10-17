#region Using directives

using NeoBase.BPM;
using NeoBase.Common;
using NeoBase.Common.DataObjects;
using NeoSpin.Common;
using NeoSpin.Communication;
using NeoSpin.DataObjects;
using NeoSpinConstants;
using Sagitec.Bpm;
using Sagitec.Common;
using Sagitec.DBUtility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

#endregion

namespace NeoSpin.BusinessObjects
{
    /// <summary>
    /// Class NeoSpin.BusinessObjects.busBpmRequest:
    /// Inherited from busBpmRequestGen, the class is used to customize the business object busBpmRequestGen.
    /// </summary>
    [Serializable]

    public class busSolBpmRequest : busNeobaseBpmRequest //busBpmRequest
    {
        /// Developer - Rahul Mane
        /// Date - 09-24-2021
        /// Iteration - Main-Iteration10 
        /// Comment - This method call from busNeobaseBpmRequest 
        /// <summary>
        /// Calls  the IncomingDocumentRecevied method to notify communication module for the incoming document.

        public override void IncomingDocumentReceived(busBpmRequest abusBpmRequest)
        {
            if (abusBpmRequest.icdoBpmRequest.tracking_id > 0 && abusBpmRequest.icdoBpmRequest.reason_value.IsNotNullOrEmpty())
            {
                new busCommunication().IncomingDocumentReceived(abusBpmRequest.icdoBpmRequest.tracking_id, abusBpmRequest.icdoBpmRequest.reason_value);
            }
        }
        public int AddRequest(Hashtable ahstRequestParameters)
        {
            Dictionary<string, object> idctRequiredParameters = new Dictionary<string, object>();

            if (ahstRequestParameters != null && ahstRequestParameters.Count > 0)
            {
                if (iclbRequestParametersForOnlineInitiation.IsNullOrEmpty())
                {
                    iclbRequestParametersForOnlineInitiation = new Collection<busBpmRequestParameter>();
                }
                else
                {
                    iclbRequestParametersForOnlineInitiation.Clear();
                }
                foreach (DictionaryEntry lRequestParameter in ahstRequestParameters)
                {
                    busBpmRequestParameter lbusBpmRequestParameter = new busBpmRequestParameter();
                    lbusBpmRequestParameter.icdoBpmRequestParameter.parameter_name = lRequestParameter.Key.ToString();
                    lbusBpmRequestParameter.icdoBpmRequestParameter.parameter_value = lRequestParameter.Value.ToString();
                    iclbRequestParametersForOnlineInitiation.Add(lbusBpmRequestParameter);
                }
            }
            if (iclbRequestParametersForOnlineInitiation != null)
            {
                foreach (busBpmRequestParameter lbusBpmRequestParameter in iclbRequestParametersForOnlineInitiation)
                {
                    string lstrParamName = lbusBpmRequestParameter.icdoBpmRequestParameter.parameter_name;
                    if (!idctRequiredParameters.ContainsKey(lstrParamName))
                        idctRequiredParameters.Add(lstrParamName, lbusBpmRequestParameter.icdoBpmRequestParameter.parameter_value);
                }
            }
            this.LoadBpmProcess();
            this.ibusBpmProcess.ibusBpmCase = busBpmCase.GetBpmCase(this.ibusBpmProcess.icdoBpmProcess.case_id);

            this.icdoBpmRequest.source_value = BpmRequestSource.Online;
            this.icdoBpmRequest.status_value = BpmRequestStatus.NotProcessed;
            this.icdoBpmRequest.created_by = iobjPassInfo.istrUserID;

            this.icdoBpmRequest.Insert();

            if (iclbRequestParametersForOnlineInitiation != null && iclbRequestParametersForOnlineInitiation.Count > 0)
            {
                foreach (KeyValuePair<string, object> requestParameter in idctRequiredParameters)
                {
                    busBpmRequestParameter lbusBpmRequestParameter = new busBpmRequestParameter();
                    lbusBpmRequestParameter.icdoBpmRequestParameter.request_id = this.icdoBpmRequest.request_id;
                    lbusBpmRequestParameter.icdoBpmRequestParameter.parameter_name = requestParameter.Key;
                    lbusBpmRequestParameter.icdoBpmRequestParameter.parameter_value = requestParameter.Value.ToString();
                    lbusBpmRequestParameter.icdoBpmRequestParameter.Insert();
                    this.iclbBpmRequestParameter.Add(lbusBpmRequestParameter);
                }
            }
            return icdoBpmRequest.request_id;
        }
    }
}
