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
	/// Class MPIPHP.DataObjects.doPensionActuary:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPensionActuary : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doPensionActuary() : base()
         {
         }
         public int pension_actuary_id { get; set; }
         public int computational_year { get; set; }
         public string person_name { get; set; }
         public string person_ssn { get; set; }
         public int person_gender_id { get; set; }
         public string person_gender_description { get; set; }
         public string person_gender_value { get; set; }
         public DateTime person_date_of_birth { get; set; }
         public string beneficiary_flag { get; set; }
         public string beneficiary_name { get; set; }
         public string beneficiary_ssn { get; set; }
         public int beneficiary_gender_id { get; set; }
         public string beneficiary_gender_description { get; set; }
         public string beneficiary_gender_value { get; set; }
         public DateTime beneficiary_date_of_birth { get; set; }
         public string union_code { get; set; }
         public int status_code_id { get; set; }
         public string status_code_description { get; set; }
         public string status_code_value { get; set; }
         public DateTime participant_date_of_death { get; set; }
         public string plan_name { get; set; }
         public int total_qualified_years { get; set; }
         public string state_of_rsdnc { get; set; }
         public int break_years { get; set; }
         public Decimal non_eligible_benefit { get; set; }
         public Decimal benefit_in_year { get; set; }
         public Decimal total_benefit { get; set; }
         public Decimal total_ee_contribution_amt { get; set; }
         public Decimal total_uvhp_amt { get; set; }
         public Decimal credited_hr_ytd { get; set; }
         public Decimal credited_hours_total { get; set; }
         public Decimal credited_hours_last_year { get; set; }
         public Decimal total_ee_interest_amt { get; set; }
         public Decimal total_uvhp_interest_amt { get; set; }
         public Decimal monthly_benefit_amt { get; set; }
         public Decimal remaining_mg { get; set; }
         public string benefit_option_code_value { get; set; }
         public string return_to_work_flag { get; set; }
         public DateTime date_of_retr_or_dsbl { get; set; }
         public DateTime beneficiary_first_payment_receive_date { get; set; }
         public DateTime pension_stop_date { get; set; }
         public int total_qualified_years_at_ret { get; set; }
         public Decimal total_qualified_hours_at_ret { get; set; }
         public DateTime beneficiary_date_of_death { get; set; }
         public Decimal cashout_amt { get; set; }
         public int retirement_type_id { get; set; }
         public string retirement_type_description { get; set; }
         public string retirement_type_value { get; set; }
         public Decimal life_annuity_amt { get; set; }
         public string md_flag { get; set; }
         public string is_disability_conversion { get; set; }
         public string is_converted_from_popup { get; set; }
         public string dro_model { get; set; }
    }
    [Serializable]
    public enum enmPensionActuary
    {
         pension_actuary_id ,
         computational_year ,
         person_name ,
         person_ssn ,
         person_gender_id ,
         person_gender_description ,
         person_gender_value ,
         person_date_of_birth ,
         beneficiary_flag ,
         beneficiary_name ,
         beneficiary_ssn ,
         beneficiary_gender_id ,
         beneficiary_gender_description ,
         beneficiary_gender_value ,
         beneficiary_date_of_birth ,
         union_code ,
         status_code_id ,
         status_code_description ,
         status_code_value ,
         participant_date_of_death ,
         plan_name ,
         total_qualified_years ,
         state_of_rsdnc ,
         break_years ,
         non_eligible_benefit ,
         benefit_in_year ,
         total_benefit ,
         total_ee_contribution_amt ,
         total_uvhp_amt ,
         credited_hr_ytd ,
         credited_hours_total ,
         credited_hours_last_year ,
         total_ee_interest_amt ,
         total_uvhp_interest_amt ,
         monthly_benefit_amt ,
         remaining_mg ,
         benefit_option_code_value ,
         return_to_work_flag ,
         date_of_retr_or_dsbl ,
         beneficiary_first_payment_receive_date ,
         pension_stop_date ,
         total_qualified_years_at_ret ,
         total_qualified_hours_at_ret ,
         beneficiary_date_of_death ,
         cashout_amt ,
         retirement_type_id ,
         retirement_type_description ,
         retirement_type_value ,
         life_annuity_amt ,
         md_flag ,
         is_disability_conversion ,
         is_converted_from_popup ,
         dro_model ,
    }
}

