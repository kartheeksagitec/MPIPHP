#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using Sagitec.CustomDataObjects;
using MPIPHP.CustomDataObjects;

#endregion

namespace MPIPHP.BusinessObjects
{
	/// <summary>
	/// Class MPIPHP.BusinessObjects.busBenefitCalculationNonsuspendibleDetail:
	/// Inherited from busBenefitCalculationNonsuspendibleDetailGen, the class is used to customize the business object busBenefitCalculationNonsuspendibleDetailGen.
	/// </summary>
	[Serializable]
	public class busBenefitCalculationNonsuspendibleDetail : busBenefitCalculationNonsuspendibleDetailGen
	{
        public void LoadData(int aintCalculationPlanYear, int aintNonsuspendibleMonth, decimal adecPensionHours)
        {
            //this.icdoBenefitCalculationNonsuspendibleDetail.benefit_calculation_yearly_detail_id = aintBenefitCalculationYearlyDetailId;
            //this.icdoBenefitCalculationNonsuspendibleDetail.benefit_calculation_detail_id = aintBenefitCalculationDetailId;
            this.icdoBenefitCalculationNonsuspendibleDetail.calculation_plan_year = aintCalculationPlanYear;
            this.icdoBenefitCalculationNonsuspendibleDetail.nonsuspendible_month_id = busConstant.CODE_ID_MONTHS;
            this.icdoBenefitCalculationNonsuspendibleDetail.pension_hours = adecPensionHours;

            busCodeValue lbusCodeValue = new busCodeValue() { icdoCodeValue = new cdoCodeValue() };
            DataTable ldtbCodeValue = busBase.Select<cdoCodeValue>(new string[] { "CODE_ID", "CODE_VALUE_ORDER" }, new object[] { busConstant.CODE_ID_MONTHS, aintNonsuspendibleMonth }, null, null);
            if (ldtbCodeValue != null && ldtbCodeValue.Rows.Count > 0)
            {
                lbusCodeValue.icdoCodeValue.LoadData(ldtbCodeValue.Rows[0]);
                this.icdoBenefitCalculationNonsuspendibleDetail.nonsuspendible_month_value = lbusCodeValue.icdoCodeValue.code_value;
                this.icdoBenefitCalculationNonsuspendibleDetail.nonsuspendible_month_description = lbusCodeValue.icdoCodeValue.description;
            }

            //this.icdoBenefitCalculationNonsuspendibleDetail.Insert();
        }
	}
}
