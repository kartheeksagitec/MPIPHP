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
	/// Class MPIPHP.DataObjects.doExcessRefund:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doExcessRefund : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doExcessRefund() : base()
         {
         }
         public int excess_refund_id { get; set; }
         public int payee_account_id { get; set; }
         public int tax_year { get; set; }
         public Decimal refund_amount { get; set; }
    }
    [Serializable]
    public enum enmExcessRefund
    {
         excess_refund_id ,
         payee_account_id ,
         tax_year ,
         refund_amount ,
    }
}

