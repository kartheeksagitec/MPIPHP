#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using MPIPHP.CustomDataObjects;

#endregion

namespace MPIPHP.BusinessObjects
{
	/// <summary>
	/// Class MPIPHP.BusinessObjects.busQdroCalculationYearlyDetail:
	/// Inherited from busQdroCalculationYearlyDetailGen, the class is used to customize the business object busQdroCalculationYearlyDetailGen.
	/// </summary>
	[Serializable]
	public class busQdroCalculationYearlyDetail : busQdroCalculationYearlyDetailGen
	{
        public void LoadData(decimal adecAnnualHours, int aintBreakYearsCount, int aintPlanYear, int aintQualifiedYearsCount, decimal adecVestedHours, int aintVestedYearsCount,
                             decimal adecBenefitRate, decimal adecAccruedBenefitAmount, decimal adecTotalHealthHours, int aintHealthCount, decimal adecUnreducedAccruedBenefitAmount,
                             decimal adecQdroHours,decimal adecThru79Hr = 0.0m, decimal adecTotalAccruedBenefitAmount = 0.0m)
        {
            if (this.icdoQdroCalculationYearlyDetail == null)
            {
                this.icdoQdroCalculationYearlyDetail = new cdoQdroCalculationYearlyDetail();
            }


            this.icdoQdroCalculationYearlyDetail.actuarial_equivalent_amount = busConstant.ZERO_DECIMAL;
            this.icdoQdroCalculationYearlyDetail.annual_adjustment_amount = busConstant.ZERO_DECIMAL;
            this.icdoQdroCalculationYearlyDetail.ee_contribution_amount = busConstant.ZERO_DECIMAL;
            this.icdoQdroCalculationYearlyDetail.ee_derived_amount = busConstant.ZERO_DECIMAL;
            this.icdoQdroCalculationYearlyDetail.ee_interest_amount = busConstant.ZERO_DECIMAL;
            this.icdoQdroCalculationYearlyDetail.er_derived_amount = busConstant.ZERO_DECIMAL;
            this.icdoQdroCalculationYearlyDetail.local_credited_days = busConstant.ZERO_DECIMAL;
            this.icdoQdroCalculationYearlyDetail.pension_credit = busConstant.ZERO_DECIMAL;
            this.icdoQdroCalculationYearlyDetail.suspendible_months_count = busConstant.ZERO_INT;
            this.icdoQdroCalculationYearlyDetail.uvhp_contribution_amount = busConstant.ZERO_DECIMAL;
            this.icdoQdroCalculationYearlyDetail.uvhp_interest_amount = busConstant.ZERO_DECIMAL;

            this.icdoQdroCalculationYearlyDetail.annual_hours = adecAnnualHours;
            this.icdoQdroCalculationYearlyDetail.break_years_count = aintBreakYearsCount;
            this.icdoQdroCalculationYearlyDetail.plan_year = aintPlanYear;
            this.icdoQdroCalculationYearlyDetail.qualified_years_count = aintQualifiedYearsCount;
            this.icdoQdroCalculationYearlyDetail.vested_hours = adecVestedHours;
            this.icdoQdroCalculationYearlyDetail.vested_years_count = aintVestedYearsCount;
            this.icdoQdroCalculationYearlyDetail.benefit_rate = adecBenefitRate;
            this.icdoQdroCalculationYearlyDetail.accrued_benefit_amount = adecAccruedBenefitAmount;
            this.icdoQdroCalculationYearlyDetail.actuarial_accrued_benenfit = adecUnreducedAccruedBenefitAmount;
            this.icdoQdroCalculationYearlyDetail.health_hours = adecTotalHealthHours;
            this.icdoQdroCalculationYearlyDetail.health_years_count = aintHealthCount;
            this.icdoQdroCalculationYearlyDetail.qdro_hours = adecQdroHours;
            this.icdoQdroCalculationYearlyDetail.thru79hours = adecThru79Hr;
            //this.icdoQdroCalculationYearlyDetail.total_accrued_benefit = adecTotalAccruedBenefitAmount; //TODO Need to add column
        }
	}
}
