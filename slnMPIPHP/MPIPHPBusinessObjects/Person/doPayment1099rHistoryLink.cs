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
	/// Class MPIPHP.DataObjects.doPayment1099rHistoryLink:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPayment1099rHistoryLink : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doPayment1099rHistoryLink() : base()
         {
         }
         public int payment_1099r_history_link_id { get; set; }
         public int payment_1099r_id { get; set; }
         public int payment_history_header_id { get; set; }
         public int reimbursement_details_id { get; set; }
    }
    [Serializable]
    public enum enmPayment1099rHistoryLink
    {
         payment_1099r_history_link_id ,
         payment_1099r_id ,
         payment_history_header_id ,
         reimbursement_details_id ,
    }
}

