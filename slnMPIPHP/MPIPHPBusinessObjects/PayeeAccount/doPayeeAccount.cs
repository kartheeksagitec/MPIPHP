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
	/// Class MPIPHP.DataObjects.doPayeeAccount:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPayeeAccount : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doPayeeAccount() : base()
         {
         }
         public int payee_account_id { get; set; }
         public int person_id { get; set; }
         public int org_id { get; set; }
         public int benefit_application_detail_id { get; set; }
         public int benefit_calculation_detail_id { get; set; }
         public int dro_application_detail_id { get; set; }
         public int dro_calculation_detail_id { get; set; }
         public int payee_benefit_account_id { get; set; }
         public int benefit_account_type_id { get; set; }
         public string benefit_account_type_description { get; set; }
         public string benefit_account_type_value { get; set; }
         public int retirement_type_id { get; set; }
         public string retirement_type_description { get; set; }
         public string retirement_type_value { get; set; }
         public DateTime benefit_begin_date { get; set; }
         public DateTime benefit_end_date { get; set; }
         public int account_relation_id { get; set; }
         public string account_relation_description { get; set; }
         public string account_relation_value { get; set; }
         public int family_relation_id { get; set; }
         public string family_relation_description { get; set; }
         public string family_relation_value { get; set; }
         public Decimal minimum_guarantee_amount { get; set; }
         public Decimal nontaxable_beginning_balance { get; set; }
         public int plan_benefit_id { get; set; }
         public DateTime term_certain_end_date { get; set; }
         public int transfer_org_id { get; set; }
         public string verified_flag { get; set; }
         public string adjustment_payment_eligible_flag { get; set; }
         public string include_in_adhoc_flag { get; set; }
         public string review_payee_acc_for_retiree_inc_form { get; set; }
         public string reemployed_flag { get; set; }
         public int reference_id { get; set; }
         public string reemployed_flag_from_eadb { get; set; }
         public DateTime reemployed_flag_as_of_date { get; set; }
         public string disability_conversion_flag { get; set; }
         public string accrued_benefit_to_be_paid_reeval_flag { get; set; }
         public string reemployment_override_flag { get; set; }
         public Decimal unreduced_benefit_amount { get; set; }
         public Decimal ee_derived_benefit_amount { get; set; }
         public Decimal remaining_non_taxable_from_conversion { get; set; }
         public string retiree_incr_flag { get; set; }
         public string adverse_interest_flag { get; set; }
        public string transfer_org_contact_name { get; set; }
         public string verified_by { get; set; }
         public DateTime verified_date { get; set; }
         public DateTime converted_to_life_date { get; set; }
        public string onetime_payment_flag { get; set; }
    }
    [Serializable]
    public enum enmPayeeAccount
    {
         payee_account_id ,
         person_id ,
         org_id ,
         benefit_application_detail_id ,
         benefit_calculation_detail_id ,
         dro_application_detail_id ,
         dro_calculation_detail_id ,
         payee_benefit_account_id ,
         benefit_account_type_id ,
         benefit_account_type_description ,
         benefit_account_type_value ,
         retirement_type_id ,
         retirement_type_description ,
         retirement_type_value ,
         benefit_begin_date ,
         benefit_end_date ,
         account_relation_id ,
         account_relation_description ,
         account_relation_value ,
         family_relation_id ,
         family_relation_description ,
         family_relation_value ,
         minimum_guarantee_amount ,
         nontaxable_beginning_balance ,
         plan_benefit_id ,
         term_certain_end_date ,
         transfer_org_id ,
         verified_flag ,
         adjustment_payment_eligible_flag ,
         include_in_adhoc_flag ,
         review_payee_acc_for_retiree_inc_form ,
         reemployed_flag ,
         reference_id ,
         reemployed_flag_from_eadb ,
         reemployed_flag_as_of_date ,
         disability_conversion_flag ,
         accrued_benefit_to_be_paid_reeval_flag ,
         reemployment_override_flag ,
         unreduced_benefit_amount ,
         ee_derived_benefit_amount ,
         remaining_non_taxable_from_conversion ,
         retiree_incr_flag ,
         transfer_org_contact_name ,
         verified_by ,
         verified_date ,
         converted_to_life_date ,
        onetime_payment_flag,
        adverse_interest_flag,
    }
}


