#region Using directives

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Sagitec.Common;
using Sagitec.DataObjects;

#endregion

namespace MPIPHP.DataObjects
{
	/// <summary>
	/// Class MPIPHP.DataObjects.doBenefitApplicationEligiblePlans:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doBenefitApplicationEligiblePlans : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doBenefitApplicationEligiblePlans() : base()
         {

         }
         public int benefit_application_eligible_id { get; set; }
         public int benefit_application_id { get; set; }
         public string mpipp_flag { get; set; }
         public string mpipp_eligibleflag { get; set; }
         public string iap_flag { get; set; }
         public string iap_eligibleflag { get; set; }
         public string local600_flag { get; set; }
         public string local600_eligibleflag { get; set; }
         public string local666_flag { get; set; }
         public string local666_eligibleflag { get; set; }
         public string local700_flag { get; set; }
         public string local700_eligibleflag { get; set; }
         public string local52_flag { get; set; }
         public string local52_eligibleflag { get; set; }
         public string local161_flag { get; set; }
         public string local161_eligibleflag { get; set; }
    }
    [Serializable]
    public enum enmBenefitApplicationEligiblePlans
    {
         benefit_application_eligible_id ,
         benefit_application_id ,
         mpipp_flag ,
         mpipp_eligibleflag ,
         iap_flag ,
         iap_eligibleflag ,
         local600_flag ,
         local600_eligibleflag ,
         local666_flag ,
         local666_eligibleflag ,
         local700_flag ,
         local700_eligibleflag ,
         local52_flag ,
         local52_eligibleflag ,
         local161_flag ,
         local161_eligibleflag ,
    }
}

