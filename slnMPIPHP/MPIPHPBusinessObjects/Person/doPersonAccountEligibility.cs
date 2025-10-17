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
	/// Class MPIPHP.DataObjects.doPersonAccountEligibility:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPersonAccountEligibility : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doPersonAccountEligibility() : base()
         {
         }
         public int person_account_eligibility_id { get; set; }
         public int person_account_id { get; set; }
         public DateTime vested_date { get; set; }
         public string vesting_rule { get; set; }
         public DateTime forfeiture_date { get; set; }
         public DateTime last_evaluated_date { get; set; }
         public int status_id { get; set; }
         public string status_description { get; set; }
         public string status_value { get; set; }
         public Decimal pension_credits { get; set; }
         public int local_qualified_years { get; set; }
         public Decimal local_frozen_hours { get; set; }
    }
    [Serializable]
    public enum enmPersonAccountEligibility
    {
         person_account_eligibility_id ,
         person_account_id ,
         vested_date ,
         vesting_rule ,
         forfeiture_date ,
         last_evaluated_date ,
         status_id ,
         status_description ,
         status_value ,
         pension_credits ,
         local_qualified_years ,
         local_frozen_hours ,
    }
}

