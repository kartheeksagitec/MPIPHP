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
	/// Class MPIPHP.DataObjects.doBenefitCalculationDetail:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doBenefitCalculationDetail : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doBenefitCalculationDetail() : base()
         {
         }
         public int benefit_calculation_detail_id { get; set; }
         public int benefit_calculation_header_id { get; set; }
         public int benefit_application_detail_id { get; set; }
         public int person_account_id { get; set; }
         public int plan_id { get; set; }
         public DateTime vested_date { get; set; }
         public int benefit_subtype_id { get; set; }
         public string benefit_subtype_description { get; set; }
         public string benefit_subtype_value { get; set; }
         public Decimal unreduced_benefit_amount { get; set; }
         public Decimal final_monthly_benefit_amount { get; set; }
         public Decimal early_reduction_factor { get; set; }
         public Decimal early_reduced_benefit_amount { get; set; }
         public Decimal additional_accrued_benefit_amount { get; set; }
         public Decimal actuarial_accrued_benefit_amount { get; set; }
         public Decimal iap_balance_amount { get; set; }
         public Decimal minimum_guarantee_amount { get; set; }
         public Decimal monthly_exclusion_amount { get; set; }
         public int status_id { get; set; }
         public string status_description { get; set; }
         public string status_value { get; set; }
         public Decimal total_uvhp_contribution_amount { get; set; }
         public Decimal total_uvhp_interest_amount { get; set; }
         public DateTime ee_as_of_date { get; set; }
         public DateTime uvhp_as_of_date { get; set; }
         public DateTime iap_as_of_date { get; set; }
         public Decimal local52_special_acct_bal_amount { get; set; }
         public Decimal local161_special_acct_bal_amount { get; set; }
         public DateTime special_account_as_of_date { get; set; }
         public DateTime retirement_date { get; set; }
         public string ee_flag { get; set; }
         public string uvhp_flag { get; set; }
         public string l52_spl_acc_flag { get; set; }
         public string l161_spl_acc_flag { get; set; }
         public Decimal elected_benefit_amount { get; set; }
         public Decimal present_value_amount { get; set; }
         public Decimal qdro_offset { get; set; }
         public Decimal l161_spl_acc_qdro_offset { get; set; }
         public Decimal l52_spl_acc_qdro_offset { get; set; }
         public Decimal total_uvhp_contribution_qdro_offset { get; set; }
         public Decimal total_ee_contribution_qdro_offset { get; set; }
         public int dro_model_id { get; set; }
         public string dro_model_description { get; set; }
         public string dro_model_value { get; set; }
         public int local52_rule_id { get; set; }
         public string local52_rule_description { get; set; }
         public string local52_rule_value { get; set; }
         public Decimal alternate_payee_pure_contribution { get; set; }
         public Decimal total_ee_interest_qdro_offset { get; set; }
         public Decimal total_uvhp_interest_qdro_offset { get; set; }
         public Decimal vested_ee_amount { get; set; }
         public Decimal vested_ee_interest { get; set; }
         public Decimal non_vested_ee_amount { get; set; }
         public Decimal non_vested_ee_interest { get; set; }
         public Decimal adjusment_payment { get; set; }
         public string adjustment_iap_payment_flag { get; set; }
         public string adjustment_l52spl_payment_flag { get; set; }
         public string adjustment_l161spl_payment_flag { get; set; }
         public Decimal ee_derived_benefit_amount { get; set; }
         public DateTime reemployed_accrued_benefit_effective_date { get; set; }
         public Decimal accrued_at_retirement_amount { get; set; }
         public Decimal popup_monthly_exclusion_amount { get; set; }
    }
    [Serializable]
    public enum enmBenefitCalculationDetail
    {
         benefit_calculation_detail_id ,
         benefit_calculation_header_id ,
         benefit_application_detail_id ,
         person_account_id ,
         plan_id ,
         vested_date ,
         benefit_subtype_id ,
         benefit_subtype_description ,
         benefit_subtype_value ,
         unreduced_benefit_amount ,
         final_monthly_benefit_amount ,
         early_reduction_factor ,
         early_reduced_benefit_amount ,
         additional_accrued_benefit_amount ,
         actuarial_accrued_benefit_amount ,
         iap_balance_amount ,
         minimum_guarantee_amount ,
         monthly_exclusion_amount ,
         status_id ,
         status_description ,
         status_value ,
         total_uvhp_contribution_amount ,
         total_uvhp_interest_amount ,
         ee_as_of_date ,
         uvhp_as_of_date ,
         iap_as_of_date ,
         local52_special_acct_bal_amount ,
         local161_special_acct_bal_amount ,
         special_account_as_of_date ,
         retirement_date ,
         ee_flag ,
         uvhp_flag ,
         l52_spl_acc_flag ,
         l161_spl_acc_flag ,
         elected_benefit_amount ,
         present_value_amount ,
         qdro_offset ,
         l161_spl_acc_qdro_offset ,
         l52_spl_acc_qdro_offset ,
         total_uvhp_contribution_qdro_offset ,
         total_ee_contribution_qdro_offset ,
         dro_model_id ,
         dro_model_description ,
         dro_model_value ,
         local52_rule_id ,
         local52_rule_description ,
         local52_rule_value ,
         alternate_payee_pure_contribution ,
         total_ee_interest_qdro_offset ,
         total_uvhp_interest_qdro_offset ,
         vested_ee_amount ,
         vested_ee_interest ,
         non_vested_ee_amount ,
         non_vested_ee_interest ,
         adjusment_payment ,
         adjustment_iap_payment_flag ,
         adjustment_l52spl_payment_flag ,
         adjustment_l161spl_payment_flag ,
         ee_derived_benefit_amount ,
         reemployed_accrued_benefit_effective_date ,
         accrued_at_retirement_amount ,
         popup_monthly_exclusion_amount ,
    }
}

