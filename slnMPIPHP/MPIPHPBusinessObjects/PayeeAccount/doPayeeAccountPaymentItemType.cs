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
	/// Class MPIPHP.DataObjects.doPayeeAccountPaymentItemType:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPayeeAccountPaymentItemType : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doPayeeAccountPaymentItemType() : base()
         {
         }
         public int payee_account_payment_item_type_id { get; set; }
         public int payee_account_id { get; set; }
         public int payment_item_type_id { get; set; }
         public Decimal amount { get; set; }
         public DateTime start_date { get; set; }
         public DateTime end_date { get; set; }
         public string account_number { get; set; }
         public string reissue_item_flag { get; set; }
         public int vendor_org_id { get; set; }
    }
    [Serializable]
    public enum enmPayeeAccountPaymentItemType
    {
         payee_account_payment_item_type_id ,
         payee_account_id ,
         payment_item_type_id ,
         amount ,
         start_date ,
         end_date ,
         account_number ,
         reissue_item_flag ,
         vendor_org_id ,
    }
}

