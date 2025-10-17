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
	/// Class MPIPHP.DataObjects.doActiveRetireeIncreaseContractHistory:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doActiveRetireeIncreaseContractHistory : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doActiveRetireeIncreaseContractHistory() : base()
         {
         }
         public int active_retiree_increase_contract_history_id { get; set; }
         public int active_retiree_increase_contract_id { get; set; }
         public int plan_year { get; set; }
         public DateTime effective_start_date { get; set; }
         public DateTime effective_end_date { get; set; }
         public int percent_increase_id { get; set; }
         public string percent_increase_description { get; set; }
         public string percent_increase_value { get; set; }
    }
    [Serializable]
    public enum enmActiveRetireeIncreaseContractHistory
    {
         active_retiree_increase_contract_history_id ,
         active_retiree_increase_contract_id ,
         plan_year ,
         effective_start_date ,
         effective_end_date ,
         percent_increase_id ,
         percent_increase_description ,
         percent_increase_value ,
    }
}

