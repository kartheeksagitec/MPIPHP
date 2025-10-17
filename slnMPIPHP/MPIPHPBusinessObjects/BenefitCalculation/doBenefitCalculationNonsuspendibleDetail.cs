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
	/// Class MPIPHP.DataObjects.doBenefitCalculationNonsuspendibleDetail:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doBenefitCalculationNonsuspendibleDetail : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doBenefitCalculationNonsuspendibleDetail() : base()
         {
         }
         public int benefit_calculation_nonsuspendible_detail_id { get; set; }
         public int benefit_calculation_yearly_detail_id { get; set; }
         public int benefit_calculation_detail_id { get; set; }
         public Decimal calculation_plan_year { get; set; }
         public int nonsuspendible_month_id { get; set; }
         public string nonsuspendible_month_description { get; set; }
         public string nonsuspendible_month_value { get; set; }
         public Decimal pension_hours { get; set; }
    }
    [Serializable]
    public enum enmBenefitCalculationNonsuspendibleDetail
    {
         benefit_calculation_nonsuspendible_detail_id ,
         benefit_calculation_yearly_detail_id ,
         benefit_calculation_detail_id ,
         calculation_plan_year ,
         nonsuspendible_month_id ,
         nonsuspendible_month_description ,
         nonsuspendible_month_value ,
         pension_hours ,
    }
}

