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
	/// Class MPIPHP.DataObjects.doPayeeOverpaymentReportData:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPayeeOverpaymentReportData : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doPayeeOverpaymentReportData() : base()
         {
         }
         public int payee_overpayment_report_data_id { get; set; }
         public DateTime start_date { get; set; }
         public DateTime end_date { get; set; }
         public string mpi_person_id { get; set; }
         public string ssn { get; set; }
         public string payee_name { get; set; }
         public DateTime date_of_death { get; set; }
         public string plan_code { get; set; }
         public Decimal next_gross_payment { get; set; }
         public Decimal next_amt_due { get; set; }
         public int payee_account_id { get; set; }
         public int overpayment_id { get; set; }
         public DateTime effective_date { get; set; }
         public Decimal total_op_amount { get; set; }
         public Decimal new_overpayment { get; set; }
         public Decimal reimbursement_amt_paid { get; set; }
         public int month { get; set; }
         public Decimal adjustment { get; set; }
         public Decimal new_outstanding_balance { get; set; }
         public DateTime estimated_end_date { get; set; }
         public string status_value { get; set; }
    }
    [Serializable]
    public enum enmPayeeOverpaymentReportData
    {
         payee_overpayment_report_data_id ,
         start_date ,
         end_date ,
         mpi_person_id ,
         ssn ,
         payee_name ,
         date_of_death ,
         plan_code ,
         next_gross_payment ,
         next_amt_due ,
         payee_account_id ,
         overpayment_id ,
         effective_date ,
         total_op_amount ,
         new_overpayment ,
         reimbursement_amt_paid ,
         month ,
         adjustment ,
         new_outstanding_balance ,
         estimated_end_date ,
         status_value ,
    }
}

