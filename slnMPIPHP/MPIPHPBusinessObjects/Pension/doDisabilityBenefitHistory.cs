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
	/// Class MPIPHP.DataObjects.doDisabilityBenefitHistory:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doDisabilityBenefitHistory : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doDisabilityBenefitHistory() : base()
         {
         }
         public int disability_benefit_history_id { get; set; }
         public int benefit_application_id { get; set; }
         public DateTime disability_cont_letter_date { get; set; }
         public int plan_id { get; set; }
         public string received { get; set; }
         public string sent { get; set; }
    }
    [Serializable]
    public enum enmDisabilityBenefitHistory
    {
         disability_benefit_history_id ,
         benefit_application_id ,
         disability_cont_letter_date ,
         plan_id ,
         received ,
         sent ,
    }
}

