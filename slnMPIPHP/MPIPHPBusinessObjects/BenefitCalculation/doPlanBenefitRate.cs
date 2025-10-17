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
	/// Class MPIPHP.DataObjects.doPlanBenefitRate:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPlanBenefitRate : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doPlanBenefitRate() : base()
         {
         }
         public int plan_benefit_rate_id { get; set; }
         public int rate_type_id { get; set; }
         public string rate_type_description { get; set; }
         public string rate_type_value { get; set; }
         public int qualified_year_limit_id { get; set; }
         public string qualified_year_limit_description { get; set; }
         public string qualified_year_limit_value { get; set; }
         public DateTime effective_date { get; set; }
         public Decimal plan_year { get; set; }
         public Decimal break_in_service_year { get; set; }
         public Decimal rate { get; set; }
         public Decimal increase_percentage { get; set; }
         public Decimal effective_end_year { get; set; }
         public DateTime reemployed_accrual_eligible_from_retr_dt { get; set; }
         public DateTime minimum_distribution_effective_date { get; set; }
    }
    [Serializable]
    public enum enmPlanBenefitRate
    {
         plan_benefit_rate_id ,
         rate_type_id ,
         rate_type_description ,
         rate_type_value ,
         qualified_year_limit_id ,
         qualified_year_limit_description ,
         qualified_year_limit_value ,
         effective_date ,
         plan_year ,
         break_in_service_year ,
         rate ,
         increase_percentage ,
         effective_end_year ,
         reemployed_accrual_eligible_from_retr_dt ,
         minimum_distribution_effective_date ,
    }
}

