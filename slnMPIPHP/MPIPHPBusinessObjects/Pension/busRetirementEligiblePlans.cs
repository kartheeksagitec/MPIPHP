#region Using directives
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using System.Linq;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using MPIPHP.CustomDataObjects;
using MPIPHP.DataObjects;
#endregion

namespace MPIPHP.BusinessObjects
{
    //PIR-799
    [Serializable]
    public class busRetirementEligiblePlans : busMPIPHPBase
    {        
        public string istrPlanName { get; set; }
        public int iintPlanId { get; set; }
        public string istrIsSelected { get; set; }
        public string istrIsPreviouslySelected { get; set; }
    }
}
