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
	/// Class MPIPHP.CustomDataObjects.cdoBenefitMonthwiseAdjustmentDetail:
	/// Inherited from doBenefitMonthwiseAdjustmentDetail, the class is used to customize the database object doBenefitMonthwiseAdjustmentDetail.
	/// </summary>
    [Serializable]
	public class cdoBenefitMonthwiseAdjustmentDetail : doBenefitMonthwiseAdjustmentDetail
	{
        public decimal idecAlternatePayeeMonthlyTaxableOffset { get; set; }
        public decimal idecAlternatePayeeMonthlyNonTaxableOffset { get; set; }

        
        public decimal idecMonthlyCummulativeTillDate { get; set; }

        public decimal idecEEDerivedShouldHavePaid { get; set; }
        public decimal idecERDerivedShouldHavePaid { get; set; }
        public decimal idecTotalShouldHavePaid { get; set; }
        public decimal idecAcctualPaidForTheMonth { get; set; }
        public decimal idecOverUnderPaymentPerMonth { get; set; }
        public decimal idecToDateOverUnderPayment { get; set; }

      
        


		public cdoBenefitMonthwiseAdjustmentDetail() : base()
		{
		}

        public decimal idecNonTaxableAmountDifference
        {
            get
            {
                if (this.overriden_non_taxable_amount > decimal.Zero)
                {
                    return this.overriden_non_taxable_amount - this.non_taxable_amount_paid;
                }
                else
                {
                    return this.non_taxable_amount_to_be_paid - this.non_taxable_amount_paid;

                }
            }
        }

        public decimal idecTaxableAmountDifference
        {
            get
            {
                if (this.overriden_taxable_amount > decimal.Zero)
                {
                    return this.overriden_taxable_amount - (this.taxable_amount_paid - this.amount_repaid);
                }
                else
                {
                    return this.taxable_amount_to_be_paid - (this.taxable_amount_paid - this.amount_repaid);
                }
            }
        }
    } 
} 
