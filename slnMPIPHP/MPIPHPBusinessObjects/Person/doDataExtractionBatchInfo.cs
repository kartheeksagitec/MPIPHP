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
	/// Class MPIPHP.DataObjects.doDataExtractionBatchInfo:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doDataExtractionBatchInfo : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doDataExtractionBatchInfo() : base()
         {
         }
         public int data_extraction_batch_info_id { get; set; }
         public int year_end_data_extraction_header_id { get; set; }
         public int person_id { get; set; }
         public string person_name { get; set; }
         public string person_ssn { get; set; }
         public int person_gender_id { get; set; }
         public string person_gender_description { get; set; }
         public string person_gender_value { get; set; }
         public DateTime person_dob { get; set; }
         public DateTime participant_date_of_death { get; set; }
         public int beneficiary_id { get; set; }
         public string beneficiary_flag { get; set; }
         public string beneficiary_name { get; set; }
         public string beneficiary_ssn { get; set; }
         public int beneficiary_gender_id { get; set; }
         public string beneficiary_gender_description { get; set; }
         public string beneficiary_gender_value { get; set; }
         public DateTime beneficiary_dob { get; set; }
         public int status_code_id { get; set; }
         public string status_code_description { get; set; }
         public string status_code_value { get; set; }
         public int total_qualified_years { get; set; }
         public int participant_state_id { get; set; }
         public string participant_state_description { get; set; }
         public string participant_state_value { get; set; }
         public int last_qf_yr_before_bis { get; set; }
         public Decimal non_eligible_benefit { get; set; }
         public Decimal accrued_benefit_for_prior_year { get; set; }
         public Decimal accrued_benefit_till_last_comp_year { get; set; }
         public Decimal total_ee_contribution_amt { get; set; }
         public Decimal total_uvhp_amt { get; set; }
         public Decimal total_ee_interest_amt { get; set; }
         public Decimal total_uvhp_interest_amt { get; set; }
         public Decimal ytd_hours_for_last_comp_year { get; set; }
         public Decimal total_hours { get; set; }
         public Decimal ytd_hours_before_last_comp_year { get; set; }
         public string local_600_flag { get; set; }
         public Decimal local_600_premerger_total_qualified_years { get; set; }
         public Decimal local_600_premerger_benefit { get; set; }
         public string local_666_flag { get; set; }
         public Decimal local_666_premerger_total_qualified_years { get; set; }
         public Decimal local_666_premerger_benefit { get; set; }
         public string local_700_flag { get; set; }
         public Decimal local_700_premerger_total_qualified_years { get; set; }
         public Decimal local_700_premerger_benefit { get; set; }
         public string local_52_flag { get; set; }
         public Decimal local_52_premerger_total_qualified_years { get; set; }
         public Decimal local_52_premerger_benefit { get; set; }
         public string local_161_flag { get; set; }
         public Decimal local_161_premerger_total_qualified_years { get; set; }
         public Decimal local_161_premerger_benefit { get; set; }
         public Decimal monthly_benefit_amt { get; set; }
         public Decimal non_taxable_amt_left { get; set; }
         public string return_to_work_flag { get; set; }
         public DateTime determination_date { get; set; }
         public DateTime beneficiary_first_payment_receive_date { get; set; }
         public DateTime pension_stop_date { get; set; }
         public int total_qualified_years_at_ret { get; set; }
         public Decimal total_qualified_hours_at_ret { get; set; }
         public DateTime beneficiary_date_of_death { get; set; }
         public Decimal lump_amt_taken_in_last_comp_yr { get; set; }
         public int retirement_type_id { get; set; }
         public string retirement_type_description { get; set; }
         public string retirement_type_value { get; set; }
         public Decimal ee_amt_prior_year { get; set; }
         public Decimal uvhp_amt_prior_year { get; set; }
         public int benefit_option_code_id { get; set; }
         public string benefit_option_code_description { get; set; }
         public string benefit_option_code_value { get; set; }
         public Decimal total_qf_yr_begining_of_last_comp_year { get; set; }
         public Decimal ee_contribution_amt { get; set; }
         public Decimal uvhp_contribution_amt { get; set; }
         public string mpi_person_id { get; set; }
         public Decimal local_52_pension_credits { get; set; }
         public Decimal local_52_credited_hours { get; set; }
         public Decimal local_600_pension_credits { get; set; }
         public Decimal local_600_credited_hours { get; set; }
         public Decimal local_666_pension_credits { get; set; }
         public Decimal local_666_credited_hours { get; set; }
         public Decimal local_700_pension_credits { get; set; }
         public Decimal local_700_credited_hours { get; set; }
         public Decimal local_161_pension_credits { get; set; }
         public Decimal local_161_credited_hours { get; set; }
         public Decimal ytd_hours_for_year_before_last_comp_year { get; set; }
         public Decimal accrued_benefit_till_previous_year { get; set; }
         public int total_qf_yr_end_of_last_comp_year { get; set; }
         public Decimal diff_accrued_benfit_for_late_hour { get; set; }
         public Decimal late_ee_contribution { get; set; }
         public Decimal life_annuity_amt { get; set; }
         public string md_flag { get; set; }
         public int plan_id { get; set; }
         public int total_vested_years { get; set; }
         public Decimal vested_hours_for_last_comp_year { get; set; }
         public Decimal mpi_late_hours_in_last_comp_year_for_prior_years { get; set; }
         public string mpi_5500_status_code { get; set; }
         public string eligible_active_incr_flag { get; set; }
         public string is_disability_conversion { get; set; }
         public string is_converted_from_popup { get; set; }
         public string dro_model { get; set; }
    }
    [Serializable]
    public enum enmDataExtractionBatchInfo
    {
         data_extraction_batch_info_id ,
         year_end_data_extraction_header_id ,
         person_id ,
         person_name ,
         person_ssn ,
         person_gender_id ,
         person_gender_description ,
         person_gender_value ,
         person_dob ,
         participant_date_of_death ,
         beneficiary_id ,
         beneficiary_flag ,
         beneficiary_name ,
         beneficiary_ssn ,
         beneficiary_gender_id ,
         beneficiary_gender_description ,
         beneficiary_gender_value ,
         beneficiary_dob ,
         status_code_id ,
         status_code_description ,
         status_code_value ,
         total_qualified_years ,
         participant_state_id ,
         participant_state_description ,
         participant_state_value ,
         last_qf_yr_before_bis ,
         non_eligible_benefit ,
         accrued_benefit_for_prior_year ,
         accrued_benefit_till_last_comp_year ,
         total_ee_contribution_amt ,
         total_uvhp_amt ,
         total_ee_interest_amt ,
         total_uvhp_interest_amt ,
         ytd_hours_for_last_comp_year ,
         total_hours ,
         ytd_hours_before_last_comp_year ,
         local_600_flag ,
         local_600_premerger_total_qualified_years ,
         local_600_premerger_benefit ,
         local_666_flag ,
         local_666_premerger_total_qualified_years ,
         local_666_premerger_benefit ,
         local_700_flag ,
         local_700_premerger_total_qualified_years ,
         local_700_premerger_benefit ,
         local_52_flag ,
         local_52_premerger_total_qualified_years ,
         local_52_premerger_benefit ,
         local_161_flag ,
         local_161_premerger_total_qualified_years ,
         local_161_premerger_benefit ,
         monthly_benefit_amt ,
         non_taxable_amt_left ,
         return_to_work_flag ,
         determination_date ,
         beneficiary_first_payment_receive_date ,
         pension_stop_date ,
         total_qualified_years_at_ret ,
         total_qualified_hours_at_ret ,
         beneficiary_date_of_death ,
         lump_amt_taken_in_last_comp_yr ,
         retirement_type_id ,
         retirement_type_description ,
         retirement_type_value ,
         ee_amt_prior_year ,
         uvhp_amt_prior_year ,
         benefit_option_code_id ,
         benefit_option_code_description ,
         benefit_option_code_value ,
         total_qf_yr_begining_of_last_comp_year ,
         ee_contribution_amt ,
         uvhp_contribution_amt ,
         mpi_person_id ,
         local_52_pension_credits ,
         local_52_credited_hours ,
         local_600_pension_credits ,
         local_600_credited_hours ,
         local_666_pension_credits ,
         local_666_credited_hours ,
         local_700_pension_credits ,
         local_700_credited_hours ,
         local_161_pension_credits ,
         local_161_credited_hours ,
         ytd_hours_for_year_before_last_comp_year ,
         accrued_benefit_till_previous_year ,
         total_qf_yr_end_of_last_comp_year ,
         diff_accrued_benfit_for_late_hour ,
         late_ee_contribution ,
         life_annuity_amt ,
         md_flag ,
         plan_id ,
         total_vested_years ,
         vested_hours_for_last_comp_year ,
         mpi_late_hours_in_last_comp_year_for_prior_years ,
         mpi_5500_status_code ,
         eligible_active_incr_flag ,
         is_disability_conversion ,
         is_converted_from_popup ,
         dro_model ,
    }
}

