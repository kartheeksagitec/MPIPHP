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
	/// Class MPIPHP.DataObjects.doDeathOverpaymentReportBalances:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doDeathOverpaymentReportBalances : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doDeathOverpaymentReportBalances() : base()
         {
         }
         public int death_overpayment_report_balances_id { get; set; }
         public DateTime balance_as_of_date { get; set; }
         public Decimal balance_as_of_prev_period { get; set; }
         public Decimal additional_op_curr_period { get; set; }
         public Decimal amount_reimbursed_curr_period { get; set; }
         public Decimal outstanding_balance { get; set; }
         public Decimal write_off_balance_as_of_prev_period { get; set; }
         public Decimal write_off_balance_as_of_year { get; set; }
         public Decimal write_off_balance_curr_period { get; set; }
         public Decimal write_off_balance { get; set; }
         public Decimal adjustment_amount { get; set; }
    }
    [Serializable]
    public enum enmDeathOverpaymentReportBalances
    {
         death_overpayment_report_balances_id ,
         balance_as_of_date ,
         balance_as_of_prev_period ,
         additional_op_curr_period ,
         amount_reimbursed_curr_period ,
         outstanding_balance ,
         write_off_balance_as_of_prev_period ,
         write_off_balance_as_of_year ,
         write_off_balance_curr_period ,
         write_off_balance ,
         adjustment_amount ,
    }
}

