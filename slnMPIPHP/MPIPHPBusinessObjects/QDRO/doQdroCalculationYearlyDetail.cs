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
	/// Class MPIPHP.DataObjects.doQdroCalculationYearlyDetail:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doQdroCalculationYearlyDetail : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doQdroCalculationYearlyDetail() : base()
         {
         }
         public int qdro_calculation_yearly_detail_id { get; set; }
         public int qdro_calculation_detail_id { get; set; }
         public int plan_year { get; set; }
         public Decimal annual_hours { get; set; }
         public int qualified_years_count { get; set; }
         public Decimal vested_hours { get; set; }
         public int vested_years_count { get; set; }
         public int break_years_count { get; set; }
         public Decimal health_hours { get; set; }
         public int health_years_count { get; set; }
         public Decimal local_credited_days { get; set; }
         public Decimal pension_credit { get; set; }
         public Decimal benefit_rate { get; set; }
         public Decimal accrued_benefit_amount { get; set; }
         public Decimal ee_contribution_amount { get; set; }
         public Decimal ee_interest_amount { get; set; }
         public int suspendible_months_count { get; set; }
         public Decimal er_derived_amount { get; set; }
         public Decimal ee_derived_amount { get; set; }
         public Decimal actuarial_accrued_benenfit { get; set; }
         public Decimal actuarial_equivalent_amount { get; set; }
         public Decimal annual_adjustment_amount { get; set; }
         public Decimal uvhp_contribution_amount { get; set; }
         public Decimal uvhp_interest_amount { get; set; }
         public Decimal qdro_hours { get; set; }
         public Decimal thru79hours { get; set; }
    }
    [Serializable]
    public enum enmQdroCalculationYearlyDetail
    {
         qdro_calculation_yearly_detail_id ,
         qdro_calculation_detail_id ,
         plan_year ,
         annual_hours ,
         qualified_years_count ,
         vested_hours ,
         vested_years_count ,
         break_years_count ,
         health_hours ,
         health_years_count ,
         local_credited_days ,
         pension_credit ,
         benefit_rate ,
         accrued_benefit_amount ,
         ee_contribution_amount ,
         ee_interest_amount ,
         suspendible_months_count ,
         er_derived_amount ,
         ee_derived_amount ,
         actuarial_accrued_benenfit ,
         actuarial_equivalent_amount ,
         annual_adjustment_amount ,
         uvhp_contribution_amount ,
         uvhp_interest_amount ,
         qdro_hours ,
         thru79hours ,
    }
}

