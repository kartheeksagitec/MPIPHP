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
	/// Class MPIPHP.DataObjects.doPaymentDirectives:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPaymentDirectives : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doPaymentDirectives() : base()
         {
         }
         public int payment_directives_id { get; set; }
         public int payee_account_id { get; set; }
         public string is_deleted { get; set; }
         public string is_org_payment { get; set; }
         public string payee_account_status { get; set; }
         public int payment_directive_type { get; set; }
         public int plan_id { get; set; }
         public string plan_code { get; set; }
         public DateTime payment_cycle_date { get; set; }
         public DateTime payment_begin_date { get; set; }
         public DateTime payment_end_date { get; set; }
         public int payment_category_id { get; set; }
         public string payment_category_description { get; set; }
         public string payment_category_value { get; set; }
         public int retirement_type_id { get; set; }
         public string retirement_type_description { get; set; }
         public string retirement_type_value { get; set; }
         public int payment_type_id { get; set; }
         public string payment_type_description { get; set; }
         public string payment_type_value { get; set; }
         public string payee_ssn { get; set; }
         public string payee_mpid { get; set; }
         public string payee_name { get; set; }
         public DateTime payee_dob { get; set; }
         public DateTime payee_dod { get; set; }
         public int account_relationship_id { get; set; }
         public string account_relationship_description { get; set; }
         public string account_relationship_value { get; set; }
         public string parti_ssn { get; set; }
         public string parti_mpid { get; set; }
         public string parti_name { get; set; }
         public DateTime parti_dob { get; set; }
         public DateTime parti_dod { get; set; }
         public DateTime minimum_distribution_date { get; set; }
         public DateTime retirement_date { get; set; }
         public DateTime awarded_on_date { get; set; }
         public DateTime retro_adjustment_date { get; set; }
         public int payment_option_id { get; set; }
         public string payment_option_description { get; set; }
         public string payment_option_value { get; set; }
         public int ucd { get; set; }
         public Decimal gross_amount { get; set; }
         public Decimal deduction_amt { get; set; }
         public Decimal ee_contri_amount { get; set; }
         public Decimal ee_interest_amount { get; set; }
         public Decimal uvhp_amount { get; set; }
         public Decimal uvhp_interest_amount { get; set; }
         public Decimal misc_payment_amt { get; set; }
         public Decimal pension_receivable_amt { get; set; }
         public DateTime repayment_esti_end_date { get; set; }
         public Decimal net_amount { get; set; }
         public Decimal iap_bal_sp_accnt_amt { get; set; }
         public Decimal over_under_paymt_adj_amt { get; set; }
         public Decimal retro_plus_curr_mnt_annu_amt { get; set; }
         public int total_months { get; set; }
         public Decimal retro_plus_curr_mnt_mea { get; set; }
         public int fed_marital_status_id { get; set; }
         public string fed_marital_status_description { get; set; }
         public string fed_marital_status_value { get; set; }
         public int fed_tax_option_id { get; set; }
         public string fed_tax_option_description { get; set; }
         public string fed_tax_option_value { get; set; }
         public int fed_tax_allowance { get; set; }
         public Decimal fed_flat_perc { get; set; }
         public Decimal fed_additional_tax_amount { get; set; }
         public Decimal fed_total_tax_amount { get; set; }
         public int st_marital_status_id { get; set; }
         public string st_marital_status_description { get; set; }
         public string st_marital_status_value { get; set; }
         public int st_tax_option_id { get; set; }
         public string st_tax_option_description { get; set; }
         public string st_tax_option_value { get; set; }
         public int st_tax_allowance { get; set; }
         public Decimal st_flat_perc { get; set; }
         public Decimal st_additional_tax_amount { get; set; }
         public Decimal st_total_tax_amount { get; set; }
         public string ach_flag { get; set; }
         public string bank { get; set; }
         public int bank_account_type_id { get; set; }
         public string bank_account_type_description { get; set; }
         public string bank_account_type_value { get; set; }
         public string bank_account_number { get; set; }
         public int routing_number { get; set; }
         public DateTime effetive_date { get; set; }
         public string split_payments_flag { get; set; }
         public Decimal payment_to_payee { get; set; }
         public Decimal direct_rollover { get; set; }
         public int distribution_code_id { get; set; }
         public string distribution_code_description { get; set; }
         public string distribution_code_value { get; set; }
         public string rollover_lump_sum_flag { get; set; }
         public string rollover_bank { get; set; }
         public string rollover_address { get; set; }
         public string rollover_account_number { get; set; }
         public Decimal withholding_percentage { get; set; }
         public string transfer_org_id { get; set; }
         public string transfer_org_name { get; set; }
         public string transfer_org_contact_name { get; set; }
         public string adjustment_payment_flag { get; set; }
         public string child_support_flag { get; set; }
         public string retiree_incr { get; set; }
         public string fund_type { get; set; }
         public string special_instructions { get; set; }
         public string more_information { get; set; }
         public string adhoc_flag { get; set; }
         public string approved_by { get; set; }
         public DateTime approved_date { get; set; }
         public string verification_rq { get; set; }
         public string verified_by { get; set; }
         public DateTime verified_date { get; set; }
         public string emergency_onetime_payment_flag { get; set; }
         public string wire_flag { get; set; }
         public string aba_swift_bank_code { get; set; }
         public int withdrawal_type_id { get; set; }
         public string withdrawal_type_description { get; set; }
         public string withdrawal_type_value { get; set; }
         public string tax_identifier_value { get; set; }
    }
    [Serializable]
    public enum enmPaymentDirectives
    {
         payment_directives_id ,
         payee_account_id ,
         is_deleted ,
         is_org_payment ,
         payee_account_status ,
         payment_directive_type ,
         plan_id ,
         plan_code ,
         payment_cycle_date ,
         payment_begin_date ,
         payment_end_date ,
         payment_category_id ,
         payment_category_description ,
         payment_category_value ,
         retirement_type_id ,
         retirement_type_description ,
         retirement_type_value ,
         payment_type_id ,
         payment_type_description ,
         payment_type_value ,
         payee_ssn ,
         payee_mpid ,
         payee_name ,
         payee_dob ,
         payee_dod ,
         account_relationship_id ,
         account_relationship_description ,
         account_relationship_value ,
         parti_ssn ,
         parti_mpid ,
         parti_name ,
         parti_dob ,
         parti_dod ,
         minimum_distribution_date ,
         retirement_date ,
         awarded_on_date ,
         retro_adjustment_date ,
         payment_option_id ,
         payment_option_description ,
         payment_option_value ,
         ucd ,
         gross_amount ,
         deduction_amt ,
         ee_contri_amount ,
         ee_interest_amount ,
         uvhp_amount ,
         uvhp_interest_amount ,
         misc_payment_amt ,
         pension_receivable_amt ,
         repayment_esti_end_date ,
         net_amount ,
         iap_bal_sp_accnt_amt ,
         over_under_paymt_adj_amt ,
         retro_plus_curr_mnt_annu_amt ,
         total_months ,
         retro_plus_curr_mnt_mea ,
         fed_marital_status_id ,
         fed_marital_status_description ,
         fed_marital_status_value ,
         fed_tax_option_id ,
         fed_tax_option_description ,
         fed_tax_option_value ,
         fed_tax_allowance ,
         fed_flat_perc ,
         fed_additional_tax_amount ,
         fed_total_tax_amount ,
         st_marital_status_id ,
         st_marital_status_description ,
         st_marital_status_value ,
         st_tax_option_id ,
         st_tax_option_description ,
         st_tax_option_value ,
         st_tax_allowance ,
         st_flat_perc ,
         st_additional_tax_amount ,
         st_total_tax_amount ,
         ach_flag ,
         bank ,
         bank_account_type_id ,
         bank_account_type_description ,
         bank_account_type_value ,
         bank_account_number ,
         routing_number ,
         effetive_date ,
         split_payments_flag ,
         payment_to_payee ,
         direct_rollover ,
         distribution_code_id ,
         distribution_code_description ,
         distribution_code_value ,
         rollover_lump_sum_flag ,
         rollover_bank ,
         rollover_address ,
         rollover_account_number ,
         withholding_percentage ,
         transfer_org_id ,
         transfer_org_name ,
         transfer_org_contact_name ,
         adjustment_payment_flag ,
         child_support_flag ,
         retiree_incr ,
         fund_type ,
         special_instructions ,
         more_information ,
         adhoc_flag ,
         approved_by ,
         approved_date ,
         verification_rq ,
         verified_by ,
         verified_date ,
         emergency_onetime_payment_flag ,
         wire_flag ,
         aba_swift_bank_code ,
         withdrawal_type_id ,
         withdrawal_type_description ,
         withdrawal_type_value ,
         tax_identifier_value ,
    }
}
