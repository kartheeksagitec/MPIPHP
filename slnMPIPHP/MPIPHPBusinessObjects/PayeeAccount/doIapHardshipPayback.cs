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
	/// Class MPIPHP.DataObjects.doIapHardshipPayback:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doIapHardshipPayback : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doIapHardshipPayback() : base()
         {
         }
         public int iap_hardship_payback_id { get; set; }
         public int payee_account_id { get; set; }
         public DateTime payment_posted_date { get; set; }
         public DateTime check_date { get; set; }
         public string check_number { get; set; }
         public Decimal check_amount { get; set; }
    }
    [Serializable]
    public enum enmIapHardshipPayback
    {
         iap_hardship_payback_id ,
         payee_account_id ,
         payment_posted_date ,
         check_date ,
         check_number ,
         check_amount ,
    }
}

