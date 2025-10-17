using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sagitec.Common;
using MPIPHP.CustomDataObjects;

namespace MPIPHP.BusinessObjects
{
    [Serializable]
    public class utlProcessInstance
    {
        public int iintProcessInstanceID { get; set; }
        public long iintReferenceID { get; set; }
        public string istrCreatedBy { get; set; }
        public Dictionary<string, utlProcessMaintainance.utlActivity> idctActivities { get; set; }
        public string istrReturnFromAuditFlag { get; set; }
        public Dictionary<string, object> idctAdditionalParameters { get; set; }
        public string GetParameterValue(string astrParameterName)
        {
            busProcessInstance lbusPrecessInstance = new busProcessInstance();
            lbusPrecessInstance.icdoProcessInstance = new cdoProcessInstance();
            lbusPrecessInstance.icdoProcessInstance.process_instance_id = iintProcessInstanceID;
            return lbusPrecessInstance.GetParameterValue(astrParameterName);
        }
    }
}
