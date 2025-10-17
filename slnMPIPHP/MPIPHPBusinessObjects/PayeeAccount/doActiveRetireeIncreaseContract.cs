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
	/// Class MPIPHP.DataObjects.doActiveRetireeIncreaseContract:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doActiveRetireeIncreaseContract : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doActiveRetireeIncreaseContract() : base()
         {
         }
         public int active_retiree_increase_contract_id { get; set; }
         public int plan_year { get; set; }
         public DateTime effective_start_date { get; set; }
         public DateTime effective_end_date { get; set; }
         public DateTime retirement_date_from { get; set; }
         public DateTime retirement_date_to { get; set; }
         public int percent_increase_id { get; set; }
         public string percent_increase_description { get; set; }
         public string percent_increase_value { get; set; }
         public int contract_status_id { get; set; }
         public string contract_status_description { get; set; }
         public string contract_status_value { get; set; }
         public string approved_by { get; set; }
         public string notes { get; set; }
    }
    [Serializable]
    public enum enmActiveRetireeIncreaseContract
    {
         active_retiree_increase_contract_id ,
         plan_year ,
         effective_start_date ,
         effective_end_date ,
         retirement_date_from ,
         retirement_date_to ,
         percent_increase_id ,
         percent_increase_description ,
         percent_increase_value ,
         contract_status_id ,
         contract_status_description ,
         contract_status_value ,
         approved_by ,
         notes ,
    }
}

