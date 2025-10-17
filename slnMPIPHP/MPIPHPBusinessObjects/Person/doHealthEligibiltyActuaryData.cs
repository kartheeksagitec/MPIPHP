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
	/// Class MPIPHP.DataObjects.doHealthEligibiltyActuaryData:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doHealthEligibiltyActuaryData : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doHealthEligibiltyActuaryData() : base()
         {
         }
         public int health_actuary_data_id { get; set; }
         public int person_id { get; set; }
         public string person_ssn { get; set; }
         public int plan_year { get; set; }
         public string person_first_name { get; set; }
         public string person_last_name { get; set; }
         public DateTime person_dob { get; set; }
         public DateTime person_dod { get; set; }
         public DateTime retirement_date { get; set; }
         public int qualified_years_till_batch_date { get; set; }
         public Decimal qualified_hours_till_batch_date { get; set; }
         public Decimal current_hours { get; set; }
         public Decimal prior_hours { get; set; }
         public string ret_health_elig_flag { get; set; }
         public string eligible_rule { get; set; }
         public string enrolled_in_local_plan_flag { get; set; }
         public string retirement_type { get; set; }
    }
    [Serializable]
    public enum enmHealthEligibiltyActuaryData
    {
         health_actuary_data_id ,
         person_id ,
         person_ssn ,
         plan_year ,
         person_first_name ,
         person_last_name ,
         person_dob ,
         person_dod ,
         retirement_date ,
         qualified_years_till_batch_date ,
         qualified_hours_till_batch_date ,
         current_hours ,
         prior_hours ,
         ret_health_elig_flag ,
         eligible_rule ,
         enrolled_in_local_plan_flag ,
         retirement_type ,
    }
}

