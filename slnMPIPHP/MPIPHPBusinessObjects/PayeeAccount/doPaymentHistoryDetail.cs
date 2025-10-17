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
	/// Class MPIPHP.DataObjects.doPaymentHistoryDetail:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPaymentHistoryDetail : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doPaymentHistoryDetail() : base()
         {
         }
         public int payment_history_detail_id { get; set; }
         public int payment_history_header_id { get; set; }
         public int payment_item_type_id { get; set; }
         public Decimal amount { get; set; }
         public int rollover_org_id { get; set; }
         public int vendor_org_id { get; set; }
    }
    [Serializable]
    public enum enmPaymentHistoryDetail
    {
         payment_history_detail_id ,
         payment_history_header_id ,
         payment_item_type_id ,
         amount ,
         rollover_org_id ,
         vendor_org_id ,
    }
}

