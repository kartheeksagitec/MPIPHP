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
	/// Class MPIPHP.DataObjects.doParticipantInformationDataExtraction:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doParticipantInformationDataExtraction : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doParticipantInformationDataExtraction() : base()
         {
         }
         public int participant_information_data_extraction_id { get; set; }
         public int year { get; set; }
         public int process_status_id { get; set; }
         public string process_status_description { get; set; }
         public string process_status_value { get; set; }
         public int person_id { get; set; }
         public string person_name { get; set; }
         public string person_ssn { get; set; }
         public string cat_type { get; set; }
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
         public DateTime beneficiary_date_of_death { get; set; }
         public int payee_account_id { get; set; }
         public string status_value { get; set; }
         public string addr_state_value { get; set; }
         public int plan_id { get; set; }
         public int person_account_id { get; set; }
         public string benefit_account_type_value { get; set; }
         public int retirement_type_id { get; set; }
         public string retirement_type_description { get; set; }
         public string retirement_type_value { get; set; }
         public string benefit_option_code_value { get; set; }
         public DateTime benefit_begin_date { get; set; }
         public DateTime benefit_end_date { get; set; }
         public DateTime term_certain_end_date { get; set; }
         public string funds_type_value { get; set; }
         public Decimal nontaxable_beginning_balance { get; set; }
         public Decimal remaining_non_taxable_from_conversion { get; set; }
         public Decimal minimum_guarantee_amount { get; set; }
         public Decimal gross_amount { get; set; }
         public Decimal amount_paid { get; set; }
         public string reemployed_flag { get; set; }
         public string mpi_person_id { get; set; }
         public int joint_annuitant_id { get; set; }
         public string first_name { get; set; }
         public string last_name { get; set; }
         public string ssn { get; set; }
         public int gender_id { get; set; }
         public string gender_description { get; set; }
         public string gender_value { get; set; }
         public string date_of_birth { get; set; }
         public string date_of_death { get; set; }
         public string forfeiture_date { get; set; }
         public string vested_date { get; set; }
         public string payee_account_status { get; set; }
         public string is_disability_conversion { get; set; }
         public string is_converted_from_popup { get; set; }
         public string dro_model { get; set; }
    }
    [Serializable]
    public enum enmParticipantInformationDataExtraction
    {
         participant_information_data_extraction_id ,
         year ,
         process_status_id ,
         process_status_description ,
         process_status_value ,
         person_id ,
         person_name ,
         person_ssn ,
         cat_type ,
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
         beneficiary_date_of_death ,
         payee_account_id ,
         status_value ,
         addr_state_value ,
         plan_id ,
         person_account_id ,
         benefit_account_type_value ,
         retirement_type_id ,
         retirement_type_description ,
         retirement_type_value ,
         benefit_option_code_value ,
         benefit_begin_date ,
         benefit_end_date ,
         term_certain_end_date ,
         funds_type_value ,
         nontaxable_beginning_balance ,
         remaining_non_taxable_from_conversion ,
         minimum_guarantee_amount ,
         gross_amount ,
         amount_paid ,
         reemployed_flag ,
         mpi_person_id ,
         joint_annuitant_id ,
         first_name ,
         last_name ,
         ssn ,
         gender_id ,
         gender_description ,
         gender_value ,
         date_of_birth ,
         date_of_death ,
         forfeiture_date ,
         vested_date ,
         payee_account_status ,
         is_disability_conversion ,
         is_converted_from_popup ,
         dro_model ,
    }
}

