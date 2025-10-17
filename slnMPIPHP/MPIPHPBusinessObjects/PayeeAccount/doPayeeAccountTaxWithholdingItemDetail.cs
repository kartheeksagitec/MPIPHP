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
	/// Class MPIPHP.DataObjects.doPayeeAccountTaxWithholdingItemDetail:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPayeeAccountTaxWithholdingItemDetail : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doPayeeAccountTaxWithholdingItemDetail() : base()
         {
         }
         public int payee_account_tax_withholding_item_dtl_id { get; set; }
         public int payee_account_tax_withholding_id { get; set; }
         public int payee_account_payment_item_type_id { get; set; }
         public int payment_item_type_id { get; set; }
         public Decimal amount { get; set; }
    }
    [Serializable]
    public enum enmPayeeAccountTaxWithholdingItemDetail
    {
         payee_account_tax_withholding_item_dtl_id ,
         payee_account_tax_withholding_id ,
         payee_account_payment_item_type_id ,
         payment_item_type_id ,
         amount ,
    }
}

