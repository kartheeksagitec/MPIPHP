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
	/// Class MPIPHP.DataObjects.doBenefitProvisionEligibility:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doBenefitProvisionEligibility : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doBenefitProvisionEligibility() : base()
         {
         }
         public int benefit_provision_eligibility_id { get; set; }
         public int benefit_provision_id { get; set; }
         public int eligibility_rule_id { get; set; }
         public string eligibilty_rule_description { get; set; }
         public string rule_grouping_operator { get; set; }
         public int benefit_account_type_id { get; set; }
         public string benefit_account_type_description { get; set; }
         public string benefit_account_type_value { get; set; }
         public int eligibility_type_id { get; set; }
         public string eligibility_type_description { get; set; }
         public string eligibility_type_value { get; set; }
         public DateTime effective_date { get; set; }
         public Decimal min_age { get; set; }
         public int min_age_operator_id { get; set; }
         public string min_age_operator_description { get; set; }
         public string min_age_operator_value { get; set; }
         public Decimal max_age { get; set; }
         public int max_age_operator_id { get; set; }
         public string max_age_operator_description { get; set; }
         public string max_age_operator_value { get; set; }
         public int credited_hours { get; set; }
         public int qualified_years { get; set; }
         public int special_years { get; set; }
         public int vested_years { get; set; }
         public int anniversary_years { get; set; }
         public string inc_forfieted_period { get; set; }
         public string inc_withdrawal { get; set; }
         public string bis_participant { get; set; }
         public string non_affliate_participant { get; set; }
         public Decimal balance { get; set; }
         public int pension_credits { get; set; }
         public int retirement_credits { get; set; }
         public DateTime merger_date { get; set; }
         public string retired_flag { get; set; }
         public int effective_year { get; set; }
    }
    [Serializable]
    public enum enmBenefitProvisionEligibility
    {
         benefit_provision_eligibility_id ,
         benefit_provision_id ,
         eligibility_rule_id ,
         eligibilty_rule_description ,
         rule_grouping_operator ,
         benefit_account_type_id ,
         benefit_account_type_description ,
         benefit_account_type_value ,
         eligibility_type_id ,
         eligibility_type_description ,
         eligibility_type_value ,
         effective_date ,
         min_age ,
         min_age_operator_id ,
         min_age_operator_description ,
         min_age_operator_value ,
         max_age ,
         max_age_operator_id ,
         max_age_operator_description ,
         max_age_operator_value ,
         credited_hours ,
         qualified_years ,
         special_years ,
         vested_years ,
         anniversary_years ,
         inc_forfieted_period ,
         inc_withdrawal ,
         bis_participant ,
         non_affliate_participant ,
         balance ,
         pension_credits ,
         retirement_credits ,
         merger_date ,
         retired_flag ,
         effective_year ,
    }
}

