#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using MPIPHP.DataObjects;

#endregion

namespace MPIPHP.CustomDataObjects
{
	/// <summary>
	/// Class MPIPHP.CustomDataObjects.cdoPaymentHistoryHeader:
	/// Inherited from doPaymentHistoryHeader, the class is used to customize the database object doPaymentHistoryHeader.
	/// </summary>
    [Serializable]
	public class cdoPaymentHistoryHeader : doPaymentHistoryHeader
	{
        public string istrPlanDescription { get; set; }
        public string istrPaymentMethod { get; set; }
        public string istrDistributionCode { get; set; }
        public string istrCheckNumber { get; set; }
        public DateTime idtPaymentDate { get; set; }
        public string istrDistributionCodeDescription { get; set; }
        public string istrPaymentType { get; set; }
        public string istrRelativeVipFlag { get; set; }

        //these properties used for Load Payment history tab in Payee account maitenance screen
        public int payment_year { get; set; }
        public decimal gross_amount { get; set; }
        public decimal taxable_amount { get; set; }
        public decimal NonTaxable_Amount { get; set; }
        public decimal deduction_amount { get; set; }
        public decimal net_amount { get; set; }
        public DateTime PaymentYearStartDate { get; set; }
        public DateTime PaymentYearEndDate { get; set; }
        public decimal taxable_rollover_amount { get; set; }
        public decimal nontaxable_rollover_amount { get; set; }

        //Mss
        public decimal retro_amount { get; set; }
        public string istrFundsType { get; set; }
        public string istrBankName { get; set; }
        public string istrBenefitType { get; set; }

        public decimal idecfedraltax { get; set; }
        public decimal idecstatetax { get; set; }

        //PIR 888
        public string istrOriginalCheckNumber { get; set; }
        public int iintOriginalHeaderId { get; set; }

		public cdoPaymentHistoryHeader() : base()
		{
		}

        //public int plan_year { get; set; }
    } 
} 
