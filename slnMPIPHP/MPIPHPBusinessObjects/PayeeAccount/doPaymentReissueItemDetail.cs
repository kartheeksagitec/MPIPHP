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
	/// Class MPIPHP.DataObjects.doPaymentReissueItemDetail:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPaymentReissueItemDetail : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doPaymentReissueItemDetail() : base()
         {
         }
         public int payment_reissue_item_detail_id { get; set; }
         public int payment_reissue_detail_id { get; set; }
         public int payee_account_payment_item_type_id { get; set; }
    }
    [Serializable]
    public enum enmPaymentReissueItemDetail
    {
         payment_reissue_item_detail_id ,
         payment_reissue_detail_id ,
         payee_account_payment_item_type_id ,
    }
}

