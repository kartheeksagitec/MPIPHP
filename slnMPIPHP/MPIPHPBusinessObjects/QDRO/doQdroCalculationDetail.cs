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
	/// Class MPIPHP.DataObjects.doQdroCalculationDetail:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doQdroCalculationDetail : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doQdroCalculationDetail() : base()
         {
         }
         public int qdro_calculation_detail_id { get; set; }
         public int qdro_calculation_header_id { get; set; }
         public int qdro_application_detail_id { get; set; }
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
         public Decimal special_account_balance_amount { get; set; }
         public Decimal minimum_guarantee_amount { get; set; }
         public Decimal member_exclusion_amount { get; set; }
         public int status_id { get; set; }
         public string status_description { get; set; }
         public string status_value { get; set; }
         public int qdro_model_id { get; set; }
         public string qdro_model_description { get; set; }
         public string qdro_model_value { get; set; }
         public int benefit_calculation_based_on_id { get; set; }
         public string benefit_calculation_based_on_description { get; set; }
         public string benefit_calculation_based_on_value { get; set; }
         public Decimal total_service { get; set; }
         public Decimal community_property_service { get; set; }
         public Decimal overriden_total_value { get; set; }
         public Decimal qdro_percent { get; set; }
         public Decimal flat_amount { get; set; }
         public Decimal flat_percent { get; set; }
         public string is_alt_payee_early_ret_flag { get; set; }
         public Decimal alt_payee_fraction { get; set; }
         public Decimal alt_payee_amt_before_conversion { get; set; }
         public Decimal participant_ee_contribution { get; set; }
         public Decimal alt_payee_ee_contribution { get; set; }
         public Decimal alt_payee_interest_amount { get; set; }
         public Decimal overriden_community_property_period { get; set; }
         public DateTime ee_as_of_date { get; set; }
         public Decimal total_uvhp_contribution_amount { get; set; }
         public Decimal total_uvhp_interest_amount { get; set; }
         public DateTime uvhp_as_of_date { get; set; }
         public Decimal alt_payee_uvhp { get; set; }
         public Decimal alt_payee_uvhp_interest { get; set; }
         public DateTime community_property_end_date { get; set; }
         public Decimal overriden_uvhp_amount { get; set; }
         public Decimal overriden_uv_uvhp_int_amount { get; set; }
         public int balance_as_of_plan_year { get; set; }
         public int alt_payee_benefit_cap_year { get; set; }
         public DateTime net_investment_from_date { get; set; }
         public DateTime net_investment_to_date { get; set; }
         public Decimal participant_benefit_amount { get; set; }
         public Decimal local52_special_acct_bal_amount { get; set; }
         public string ee_flag { get; set; }
         public string uvhp_flag { get; set; }
         public string l52_spl_acc_flag { get; set; }
         public string l161_spl_acc_flag { get; set; }
         public Decimal local161_special_acct_bal_amount { get; set; }
         public DateTime iap_as_of_date { get; set; }
         public Decimal alt_payee_l52_spl_acc_amt { get; set; }
         public Decimal alt_payee_l161_spl_acc_amt { get; set; }
         public Decimal alt_payee_l52_spl_acc_fraction { get; set; }
         public Decimal alt_payee_l161_spl_acc_fraction { get; set; }
         public Decimal alt_payee_uvhp_fraction { get; set; }
         public Decimal alt_payee_ee_fraction { get; set; }
         public Decimal participant_l52_spl_acc_amt { get; set; }
         public Decimal participant_l161_spl_acc_amt { get; set; }
         public Decimal alt_payee_minimum_guarantee_amount { get; set; }
         public Decimal alt_payee_member_exclusion_amount { get; set; }
         public Decimal adjustment_amt { get; set; }
         public Decimal accrued_benefit_amt { get; set; }
         public Decimal vested_ee_amount { get; set; }
         public Decimal vested_ee_interest { get; set; }
         public Decimal non_vested_ee_amount { get; set; }
         public Decimal non_vested_ee_interest { get; set; }
         public Decimal non_vested_altpayee_ee_amount { get; set; }
         public Decimal non_vested_altpayee_ee_interest { get; set; }
         public Decimal adjusment_payment { get; set; }
         public string adjustment_iap_payment_flag { get; set; }
         public Decimal retired_participant_amount { get; set; }
         public int referenece_participant_payee_account_id { get; set; }
         public string adjustment_l52spl_payment_flag { get; set; }
         public string adjustment_l161spl_payment_flag { get; set; }
    }
    [Serializable]
    public enum enmQdroCalculationDetail
    {
         qdro_calculation_detail_id ,
         qdro_calculation_header_id ,
         qdro_application_detail_id ,
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
         special_account_balance_amount ,
         minimum_guarantee_amount ,
         member_exclusion_amount ,
         status_id ,
         status_description ,
         status_value ,
         qdro_model_id ,
         qdro_model_description ,
         qdro_model_value ,
         benefit_calculation_based_on_id ,
         benefit_calculation_based_on_description ,
         benefit_calculation_based_on_value ,
         total_service ,
         community_property_service ,
         overriden_total_value ,
         qdro_percent ,
         flat_amount ,
         flat_percent ,
         is_alt_payee_early_ret_flag ,
         alt_payee_fraction ,
         alt_payee_amt_before_conversion ,
         participant_ee_contribution ,
         alt_payee_ee_contribution ,
         alt_payee_interest_amount ,
         overriden_community_property_period ,
         ee_as_of_date ,
         total_uvhp_contribution_amount ,
         total_uvhp_interest_amount ,
         uvhp_as_of_date ,
         alt_payee_uvhp ,
         alt_payee_uvhp_interest ,
         community_property_end_date ,
         overriden_uvhp_amount ,
         overriden_uv_uvhp_int_amount ,
         balance_as_of_plan_year ,
         alt_payee_benefit_cap_year ,
         net_investment_from_date ,
         net_investment_to_date ,
         participant_benefit_amount ,
         local52_special_acct_bal_amount ,
         ee_flag ,
         uvhp_flag ,
         l52_spl_acc_flag ,
         l161_spl_acc_flag ,
         local161_special_acct_bal_amount ,
         iap_as_of_date ,
         alt_payee_l52_spl_acc_amt ,
         alt_payee_l161_spl_acc_amt ,
         alt_payee_l52_spl_acc_fraction ,
         alt_payee_l161_spl_acc_fraction ,
         alt_payee_uvhp_fraction ,
         alt_payee_ee_fraction ,
         participant_l52_spl_acc_amt ,
         participant_l161_spl_acc_amt ,
         alt_payee_minimum_guarantee_amount ,
         alt_payee_member_exclusion_amount ,
         adjustment_amt ,
         accrued_benefit_amt ,
         vested_ee_amount ,
         vested_ee_interest ,
         non_vested_ee_amount ,
         non_vested_ee_interest ,
         non_vested_altpayee_ee_amount ,
         non_vested_altpayee_ee_interest ,
         adjusment_payment ,
         adjustment_iap_payment_flag ,
         retired_participant_amount ,
         referenece_participant_payee_account_id ,
         adjustment_l52spl_payment_flag ,
         adjustment_l161spl_payment_flag ,
    }
}

