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
	/// Class MPIPHP.DataObjects.doRepaymentSchedule:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doRepaymentSchedule : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doRepaymentSchedule() : base()
         {
         }
         public int repayment_schedule_id { get; set; }
         public int payee_account_retro_payment_id { get; set; }
         public Decimal reimbursement_amount { get; set; }
         public int reimbursement_status_id { get; set; }
         public string reimbursement_status_description { get; set; }
         public string reimbursement_status_value { get; set; }
         public int repayment_type_id { get; set; }
         public string repayment_type_description { get; set; }
         public string repayment_type_value { get; set; }
         public int payment_option_id { get; set; }
         public string payment_option_description { get; set; }
         public string payment_option_value { get; set; }
         public Decimal next_amount_due { get; set; }
         public Decimal reimbursement_amount_paid { get; set; }
         public DateTime estimated_end_date { get; set; }
         public int payee_account_id { get; set; }
         public Decimal flat_percentage { get; set; }
         public DateTime effective_date { get; set; }
         public Decimal original_reimbursement_amount { get; set; }
    }
    [Serializable]
    public enum enmRepaymentSchedule
    {
         repayment_schedule_id ,
         payee_account_retro_payment_id ,
         reimbursement_amount ,
         reimbursement_status_id ,
         reimbursement_status_description ,
         reimbursement_status_value ,
         repayment_type_id ,
         repayment_type_description ,
         repayment_type_value ,
         payment_option_id ,
         payment_option_description ,
         payment_option_value ,
         next_amount_due ,
         reimbursement_amount_paid ,
         estimated_end_date ,
         payee_account_id ,
         flat_percentage ,
         effective_date ,
         original_reimbursement_amount ,
    }
}

