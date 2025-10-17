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
	/// Class MPIPHP.DataObjects.doDroBenefitDetails:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doDroBenefitDetails : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doDroBenefitDetails() : base()
         {
         }
         public int dro_benefit_id { get; set; }
         public int dro_application_id { get; set; }
         public int dro_model_id { get; set; }
         public string dro_model_description { get; set; }
         public string dro_model_value { get; set; }
         public int plan_id { get; set; }
         public Decimal benefit_perc { get; set; }
         public Decimal benefit_amt { get; set; }
         public string alt_payee_increase { get; set; }
         public string alt_payee_early_ret { get; set; }
         public Decimal benefit_flat_perc { get; set; }
         public int plan_benefit_id { get; set; }
         public string uvhp_flag { get; set; }
         public string ee_flag { get; set; }
         public string l52_spl_acc_flag { get; set; }
         public string l161_spl_acc_flag { get; set; }
         public Decimal adjustment_amt { get; set; }
         public int status_id { get; set; }
         public string status_description { get; set; }
         public string status_value { get; set; }
         public Decimal dro_withheld_perc { get; set; }
         public int balance_as_of_plan_year { get; set; }
         public int alt_payee_benefit_cap_year { get; set; }
         public DateTime net_investment_from_date { get; set; }
         public DateTime net_investment_to_date { get; set; }
         public string is_alt_payee_eligible_for_participant_retiree_increase { get; set; }
    }
    [Serializable]
    public enum enmDroBenefitDetails
    {
         dro_benefit_id ,
         dro_application_id ,
         dro_model_id ,
         dro_model_description ,
         dro_model_value ,
         plan_id ,
         benefit_perc ,
         benefit_amt ,
         alt_payee_increase ,
         alt_payee_early_ret ,
         benefit_flat_perc ,
         plan_benefit_id ,
         uvhp_flag ,
         ee_flag ,
         l52_spl_acc_flag ,
         l161_spl_acc_flag ,
         adjustment_amt ,
         status_id ,
         status_description ,
         status_value ,
         dro_withheld_perc ,
         balance_as_of_plan_year ,
         alt_payee_benefit_cap_year ,
         net_investment_from_date ,
         net_investment_to_date ,
         is_alt_payee_eligible_for_participant_retiree_increase ,
    }
}

