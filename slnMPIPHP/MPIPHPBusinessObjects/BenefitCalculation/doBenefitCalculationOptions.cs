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
	/// Class MPIPHP.DataObjects.doBenefitCalculationOptions:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doBenefitCalculationOptions : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doBenefitCalculationOptions() : base()
         {
         }
         public int benefit_calculation_option_id { get; set; }
         public int benefit_calculation_detail_id { get; set; }
         public int plan_benefit_id { get; set; }
         public Decimal benefit_option_factor { get; set; }
         public Decimal benefit_amount { get; set; }
         public int account_relationship_id { get; set; }
         public string account_relationship_description { get; set; }
         public string account_relationship_value { get; set; }
         public int survivor_relationship_id { get; set; }
         public string survivor_relationship_description { get; set; }
         public string survivor_relationship_value { get; set; }
         public Decimal survivor_amount { get; set; }
         public string ee_flag { get; set; }
         public string uvhp_flag { get; set; }
         public string local52_special_acct_bal_flag { get; set; }
         public string local161_special_acct_bal_flag { get; set; }
         public string relative_value { get; set; }
         public Decimal disability_factor { get; set; }
         public Decimal disability_amount { get; set; }
         public Decimal participant_amount { get; set; }
         public Decimal retroactive_amount { get; set; }
         public Decimal survivor_percent_amount { get; set; }
         public Decimal overridden_benefit_amount { get; set; }
         public Decimal paid_amount { get; set; }
         public Decimal pop_up_option_factor { get; set; }
         public Decimal pop_up_benefit_amount { get; set; }
         public Decimal pop_up_option_factor_at_ret { get; set; }
    }
    [Serializable]
    public enum enmBenefitCalculationOptions
    {
         benefit_calculation_option_id ,
         benefit_calculation_detail_id ,
         plan_benefit_id ,
         benefit_option_factor ,
         benefit_amount ,
         account_relationship_id ,
         account_relationship_description ,
         account_relationship_value ,
         survivor_relationship_id ,
         survivor_relationship_description ,
         survivor_relationship_value ,
         survivor_amount ,
         ee_flag ,
         uvhp_flag ,
         local52_special_acct_bal_flag ,
         local161_special_acct_bal_flag ,
         relative_value ,
         disability_factor ,
         disability_amount ,
         participant_amount ,
         retroactive_amount ,
         survivor_percent_amount ,
         overridden_benefit_amount ,
         paid_amount ,
         pop_up_option_factor ,
         pop_up_benefit_amount ,
         pop_up_option_factor_at_ret ,
    }
}

