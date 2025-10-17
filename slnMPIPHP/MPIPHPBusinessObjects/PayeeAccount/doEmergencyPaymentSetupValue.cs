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
	/// Class MPIPHP.DataObjects.doEmergencyPaymentSetupValue:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doEmergencyPaymentSetupValue : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doEmergencyPaymentSetupValue() : base()
         {
         }
         public int emergency_payment_setup_id { get; set; }
         public DateTime effective_date { get; set; }
         public DateTime end_date { get; set; }
         public Decimal percentage { get; set; }
         public Decimal minlimit { get; set; }
         public Decimal maxlimit { get; set; }
         public int iap_balance_as_of_year { get; set; }
         public DateTime payback_begin_date { get; set; }
    }
    [Serializable]
    public enum enmEmergencyPaymentSetupValue
    {
         emergency_payment_setup_id ,
         effective_date ,
         end_date ,
         percentage ,
         minlimit ,
         maxlimit ,
         iap_balance_as_of_year ,
         payback_begin_date ,
    }
}

