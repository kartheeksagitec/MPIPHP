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
	/// Class MPIPHP.DataObjects.doBenefitApplication:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doBenefitApplication : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doBenefitApplication() : base()
         {
         }
         public int benefit_application_id { get; set; }
         public int person_id { get; set; }
         public int org_id { get; set; }
         public int benefit_type_id { get; set; }
         public string benefit_type_description { get; set; }
         public string benefit_type_value { get; set; }
         public DateTime retirement_date { get; set; }
         public DateTime application_received_date { get; set; }
         public int application_status_id { get; set; }
         public string application_status_description { get; set; }
         public string application_status_value { get; set; }
         public DateTime disability_onset_date { get; set; }
         public string terminally_ill_flag { get; set; }
         public DateTime entitlement_date { get; set; }
         public DateTime disability_conversion_date { get; set; }
         public string min_distribution_flag { get; set; }
         public DateTime min_distribution_date { get; set; }
         public DateTime death_notification_received_date { get; set; }
         public int death_notification_id { get; set; }
         public int cancellation_reason_id { get; set; }
         public string cancellation_reason_description { get; set; }
         public string cancellation_reason_value { get; set; }
         public string reason_description { get; set; }
         public DateTime withdrawal_date { get; set; }
         public DateTime ssa_application_date { get; set; }
         public DateTime awarded_on_date { get; set; }
         public int status_id { get; set; }
         public string status_description { get; set; }
         public string status_value { get; set; }
         public string final_calc_flag { get; set; }
         public int alternate_payee_id { get; set; }
         public int dro_application_id { get; set; }
         public string converted_min_distribution_flag { get; set; }
         public string child_support_flag { get; set; }
         public DateTime effective_date { get; set; }
         public string change_benefit_option_flag { get; set; }
         public string emergency_onetime_payment_flag { get; set; }
         public Decimal covid_withdrawal_amount { get; set; }
         public Decimal covid_federal_tax_percentage { get; set; }
         public Decimal covid_state_tax_percentage { get; set; }
         public int covid_option_id { get; set; }
         public string covid_option_description { get; set; }
         public string covid_option_value { get; set; }
         public int withdrawal_type_id { get; set; }
         public string withdrawal_type_description { get; set; }
         public string withdrawal_type_value { get; set; }
    }
    [Serializable]
    public enum enmBenefitApplication
    {
         benefit_application_id ,
         person_id ,
         org_id ,
         benefit_type_id ,
         benefit_type_description ,
         benefit_type_value ,
         retirement_date ,
         application_received_date ,
         application_status_id ,
         application_status_description ,
         application_status_value ,
         disability_onset_date ,
         terminally_ill_flag ,
         entitlement_date ,
         disability_conversion_date ,
         min_distribution_flag ,
         min_distribution_date ,
         death_notification_received_date ,
         death_notification_id ,
         cancellation_reason_id ,
         cancellation_reason_description ,
         cancellation_reason_value ,
         reason_description ,
         withdrawal_date ,
         ssa_application_date ,
         awarded_on_date ,
         status_id ,
         status_description ,
         status_value ,
         final_calc_flag ,
         alternate_payee_id ,
         dro_application_id ,
         converted_min_distribution_flag ,
         child_support_flag ,
         effective_date ,
         change_benefit_option_flag ,
         emergency_onetime_payment_flag ,
         covid_withdrawal_amount ,
         covid_federal_tax_percentage ,
         covid_state_tax_percentage ,
         covid_option_id ,
         covid_option_description ,
         covid_option_value ,
         withdrawal_type_id ,
         withdrawal_type_description ,
         withdrawal_type_value ,
    }
}

