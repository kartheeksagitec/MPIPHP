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
	/// Class MPIPHP.DataObjects.doBenefitCalculationHeader:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doBenefitCalculationHeader : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doBenefitCalculationHeader() : base()
         {
         }
         public int benefit_calculation_header_id { get; set; }
         public int benefit_application_id { get; set; }
         public int person_id { get; set; }
         public int beneficiary_person_id { get; set; }
         public string beneficiary_person_name { get; set; }
         public DateTime beneficiary_person_date_of_birth { get; set; }
         public int calculation_type_id { get; set; }
         public string calculation_type_description { get; set; }
         public string calculation_type_value { get; set; }
         public int benefit_type_id { get; set; }
         public string benefit_type_description { get; set; }
         public string benefit_type_value { get; set; }
         public int status_id { get; set; }
         public string status_description { get; set; }
         public string status_value { get; set; }
         public DateTime retirement_date { get; set; }
         public Decimal age { get; set; }
         public DateTime benefit_commencement_date { get; set; }
         public DateTime date_of_death { get; set; }
         public DateTime payment_date { get; set; }
         public DateTime awarded_on_date { get; set; }
         public DateTime ssa_application_date { get; set; }
         public DateTime ssa_disability_onset_date { get; set; }
         public DateTime ssa_approval_date { get; set; }
         public string terminally_ill_flag { get; set; }
         public string withdraw_ee_flag { get; set; }
         public string withdraw_uvhp_flag { get; set; }
         public string withdraw_iap_flag { get; set; }
         public string withdraw_l52_spl_acc_flag { get; set; }
         public string withdraw_l161_spl_acc_flag { get; set; }
         public DateTime retirement_date_option_2 { get; set; }
         public int error_status_id { get; set; }
         public string error_status_description { get; set; }
         public string error_status_value { get; set; }
         public string suppress_warnings_flag { get; set; }
         public int dro_application_id { get; set; }
         public int organization_id { get; set; }
         public Decimal survivor_percentage { get; set; }
         public Decimal survivor_percentage_iap { get; set; }
         public int payee_account_id { get; set; }
         public string mss_flag { get; set; }
         public string lump_sum_payment { get; set; }
    }
    [Serializable]
    public enum enmBenefitCalculationHeader
    {
         benefit_calculation_header_id ,
         benefit_application_id ,
         person_id ,
         beneficiary_person_id ,
         beneficiary_person_name ,
         beneficiary_person_date_of_birth ,
         calculation_type_id ,
         calculation_type_description ,
         calculation_type_value ,
         benefit_type_id ,
         benefit_type_description ,
         benefit_type_value ,
         status_id ,
         status_description ,
         status_value ,
         retirement_date ,
         age ,
         benefit_commencement_date ,
         date_of_death ,
         payment_date ,
         awarded_on_date ,
         ssa_application_date ,
         ssa_disability_onset_date ,
         ssa_approval_date ,
         terminally_ill_flag ,
         withdraw_ee_flag ,
         withdraw_uvhp_flag ,
         withdraw_iap_flag ,
         withdraw_l52_spl_acc_flag ,
         withdraw_l161_spl_acc_flag ,
         retirement_date_option_2 ,
         error_status_id ,
         error_status_description ,
         error_status_value ,
         suppress_warnings_flag ,
         dro_application_id ,
         organization_id ,
         survivor_percentage ,
         survivor_percentage_iap ,
         payee_account_id ,
         mss_flag ,
         lump_sum_payment ,
    }
}

