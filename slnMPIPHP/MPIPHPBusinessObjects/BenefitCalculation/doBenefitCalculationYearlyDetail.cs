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
	/// Class MPIPHP.DataObjects.doBenefitCalculationYearlyDetail:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doBenefitCalculationYearlyDetail : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doBenefitCalculationYearlyDetail() : base()
         {
         }
         public int benefit_calculation_yearly_detail_id { get; set; }
         public int benefit_calculation_detail_id { get; set; }
         public Decimal plan_year { get; set; }
         public Decimal annual_hours { get; set; }
         public int qualified_years_count { get; set; }
         public Decimal vested_hours { get; set; }
         public int vested_years_count { get; set; }
         public int break_years_count { get; set; }
         public Decimal health_hours { get; set; }
         public int health_years_count { get; set; }
         public Decimal local_credited_days { get; set; }
         public Decimal pension_credit { get; set; }
         public Decimal benefit_rate { get; set; }
         public Decimal accrued_benefit_amount { get; set; }
         public Decimal ee_contribution_amount { get; set; }
         public Decimal ee_interest_amount { get; set; }
         public int suspendible_months_count { get; set; }
         public Decimal er_derived_amount { get; set; }
         public Decimal ee_derived_amount { get; set; }
         public Decimal actuarial_accrued_benenfit { get; set; }
         public Decimal actuarial_equivalent_amount { get; set; }
         public Decimal annual_adjustment_amount { get; set; }
         public Decimal uvhp_contribution_amount { get; set; }
         public Decimal uvhp_interest_amount { get; set; }
         public Decimal total_accrued_benefit { get; set; }
         public Decimal table_b_factor { get; set; }
         public Decimal ee_act_inc_amt { get; set; }
         public Decimal er_act_inc_amt { get; set; }
         public Decimal max_ee_derv_amt { get; set; }
         public Decimal age { get; set; }
         public Decimal value_benefit_paid { get; set; }
         public Decimal gam_71_factor { get; set; }
         public Decimal active_retiree_inc { get; set; }
         public DateTime inc_effective_date { get; set; }
         public Decimal retroactive_amount { get; set; }
         public string reemployed_flag { get; set; }
         public Decimal benefit_option_factor { get; set; }
         public Decimal table_a_factor { get; set; }
         public Decimal cumulative_accrd_ben { get; set; }
         public Decimal er_current_year { get; set; }
         public int sus_months_before_incr { get; set; }
         public int sus_months_after_incr { get; set; }
         public Decimal benefit_as_of_det_date { get; set; }
         public Decimal popup_to_life_amount { get; set; }
    }
    [Serializable]
    public enum enmBenefitCalculationYearlyDetail
    {
         benefit_calculation_yearly_detail_id ,
         benefit_calculation_detail_id ,
         plan_year ,
         annual_hours ,
         qualified_years_count ,
         vested_hours ,
         vested_years_count ,
         break_years_count ,
         health_hours ,
         health_years_count ,
         local_credited_days ,
         pension_credit ,
         benefit_rate ,
         accrued_benefit_amount ,
         ee_contribution_amount ,
         ee_interest_amount ,
         suspendible_months_count ,
         er_derived_amount ,
         ee_derived_amount ,
         actuarial_accrued_benenfit ,
         actuarial_equivalent_amount ,
         annual_adjustment_amount ,
         uvhp_contribution_amount ,
         uvhp_interest_amount ,
         total_accrued_benefit ,
         table_b_factor ,
         ee_act_inc_amt ,
         er_act_inc_amt ,
         max_ee_derv_amt ,
         age ,
         value_benefit_paid ,
         gam_71_factor ,
         active_retiree_inc ,
         inc_effective_date ,
         retroactive_amount ,
         reemployed_flag ,
         benefit_option_factor ,
         table_a_factor ,
         cumulative_accrd_ben ,
         er_current_year ,
         sus_months_before_incr ,
         sus_months_after_incr ,
         benefit_as_of_det_date ,
         popup_to_life_amount ,
    }
}

