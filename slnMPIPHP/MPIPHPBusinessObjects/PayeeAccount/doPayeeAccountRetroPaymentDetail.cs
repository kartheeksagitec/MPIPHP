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
	/// Class MPIPHP.DataObjects.doPayeeAccountRetroPaymentDetail:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPayeeAccountRetroPaymentDetail : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doPayeeAccountRetroPaymentDetail() : base()
         {
         }
         public int payee_account_retro_payment_detail_id { get; set; }
         public int payee_account_retro_payment_id { get; set; }
         public int payment_item_type_id { get; set; }
         public Decimal amount { get; set; }
         public int vendor_org_id { get; set; }
         public int original_payment_item_type_id { get; set; }
         public int payee_account_payment_item_type_id { get; set; }
    }
    [Serializable]
    public enum enmPayeeAccountRetroPaymentDetail
    {
         payee_account_retro_payment_detail_id ,
         payee_account_retro_payment_id ,
         payment_item_type_id ,
         amount ,
         vendor_org_id ,
         original_payment_item_type_id ,
         payee_account_payment_item_type_id ,
    }
}

