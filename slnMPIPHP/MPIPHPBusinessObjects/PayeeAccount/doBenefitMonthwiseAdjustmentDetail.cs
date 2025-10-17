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
	/// Class MPIPHP.DataObjects.doBenefitMonthwiseAdjustmentDetail:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doBenefitMonthwiseAdjustmentDetail : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doBenefitMonthwiseAdjustmentDetail() : base()
         {
         }
         public int benefit_monthwise_adjustment_detail_id { get; set; }
         public int payee_account_retro_payment_id { get; set; }
         public DateTime payment_date { get; set; }
         public Decimal taxable_amount_to_be_paid { get; set; }
         public Decimal non_taxable_amount_to_be_paid { get; set; }
         public Decimal taxable_amount_paid { get; set; }
         public Decimal non_taxable_amount_paid { get; set; }
         public Decimal taxable_amount_difference { get; set; }
         public Decimal non_taxable_amount_difference { get; set; }
         public Decimal hours { get; set; }
         public string suspended_flag { get; set; }
         public int payment_history_header_id { get; set; }
         public Decimal overriden_taxable_amount { get; set; }
         public Decimal overriden_non_taxable_amount { get; set; }
         public Decimal amount_repaid { get; set; }
         public string override_flag { get; set; }
    }
    [Serializable]
    public enum enmBenefitMonthwiseAdjustmentDetail
    {
         benefit_monthwise_adjustment_detail_id ,
         payee_account_retro_payment_id ,
         payment_date ,
         taxable_amount_to_be_paid ,
         non_taxable_amount_to_be_paid ,
         taxable_amount_paid ,
         non_taxable_amount_paid ,
         taxable_amount_difference ,
         non_taxable_amount_difference ,
         hours ,
         suspended_flag ,
         payment_history_header_id ,
         overriden_taxable_amount ,
         overriden_non_taxable_amount ,
         amount_repaid ,
         override_flag ,
    }
}

