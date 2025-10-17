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
	/// Class MPIPHP.DataObjects.doReimbursementDetails:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doReimbursementDetails : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doReimbursementDetails() : base()
         {
         }
         public int reimbursement_details_id { get; set; }
         public int repayment_schedule_id { get; set; }
         public DateTime posted_date { get; set; }
         public string check_number { get; set; }
         public Decimal amount_paid { get; set; }
         public int payment_option_id { get; set; }
         public string payment_option_description { get; set; }
         public string payment_option_value { get; set; }
         public Decimal gross_amount { get; set; }
         public Decimal state_tax { get; set; }
         public Decimal fed_tax { get; set; }
    }
    [Serializable]
    public enum enmReimbursementDetails
    {
         reimbursement_details_id ,
         repayment_schedule_id ,
         posted_date ,
         check_number ,
         amount_paid ,
         payment_option_id ,
         payment_option_description ,
         payment_option_value ,
         gross_amount ,
         state_tax ,
         fed_tax ,
    }
}

