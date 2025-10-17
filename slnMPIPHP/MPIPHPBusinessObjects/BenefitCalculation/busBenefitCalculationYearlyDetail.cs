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
	/// Class MPIPHP.BusinessObjects.busBenefitCalculationYearlyDetail:
	/// Inherited from busBenefitCalculationYearlyDetailGen, the class is used to customize the business object busBenefitCalculationYearlyDetailGen.
	/// </summary>
	[Serializable]
	public class busBenefitCalculationYearlyDetail : busBenefitCalculationYearlyDetailGen
	{
        //public Collection<busBenefitCalculationNonsuspendibleDetail> iclbBenefitCalculationNonsuspendibleDetail { get; set; }


        public void LoadData(decimal adecAnnualHours, int aintBreakYearsCount, decimal adecPlanYear, int aintQualifiedYearsCount, decimal adecVestedHours, int aintVestedYearsCount,
                             decimal adecBenefitRate, decimal adecAccruedBenefitAmount, decimal adecTotalHealthHours, int aintHealthCount, decimal adecUnreducedAccruedBenefitAmount,decimal adecTotalAccruedBenefitAmount = 0.0m,string astrReEmployed = busConstant.FLAG_NO ,decimal adecTableBfactor = 0.0m, decimal adecTableAfactor = 0.0m, decimal adecEEContribution = 0.0m, decimal adecEEInterest = 0.0m)
        {
            if (this.icdoBenefitCalculationYearlyDetail == null)
            {
                this.icdoBenefitCalculationYearlyDetail = new cdoBenefitCalculationYearlyDetail();
            }

            
            this.icdoBenefitCalculationYearlyDetail.actuarial_equivalent_amount = busConstant.ZERO_DECIMAL;
            this.icdoBenefitCalculationYearlyDetail.annual_adjustment_amount = busConstant.ZERO_DECIMAL;
            //this.icdoBenefitCalculationYearlyDetail.ee_contribution_amount = busConstant.ZERO_DECIMAL;
            this.icdoBenefitCalculationYearlyDetail.ee_derived_amount = busConstant.ZERO_DECIMAL;
            //this.icdoBenefitCalculationYearlyDetail.ee_interest_amount = busConstant.ZERO_DECIMAL;
            this.icdoBenefitCalculationYearlyDetail.er_derived_amount = busConstant.ZERO_DECIMAL;
            this.icdoBenefitCalculationYearlyDetail.local_credited_days = busConstant.ZERO_DECIMAL;
            this.icdoBenefitCalculationYearlyDetail.pension_credit = busConstant.ZERO_DECIMAL;
            this.icdoBenefitCalculationYearlyDetail.suspendible_months_count = busConstant.ZERO_INT;
            this.icdoBenefitCalculationYearlyDetail.uvhp_contribution_amount = busConstant.ZERO_DECIMAL;
            this.icdoBenefitCalculationYearlyDetail.uvhp_interest_amount = busConstant.ZERO_DECIMAL;

            this.icdoBenefitCalculationYearlyDetail.annual_hours = adecAnnualHours;
            this.icdoBenefitCalculationYearlyDetail.break_years_count = aintBreakYearsCount;
            this.icdoBenefitCalculationYearlyDetail.plan_year = adecPlanYear;
            this.icdoBenefitCalculationYearlyDetail.qualified_years_count = aintQualifiedYearsCount;
            this.icdoBenefitCalculationYearlyDetail.vested_hours = adecVestedHours;
            this.icdoBenefitCalculationYearlyDetail.vested_years_count = aintVestedYearsCount;
            this.icdoBenefitCalculationYearlyDetail.benefit_rate = adecBenefitRate;
            this.icdoBenefitCalculationYearlyDetail.accrued_benefit_amount = adecAccruedBenefitAmount;
            this.icdoBenefitCalculationYearlyDetail.actuarial_accrued_benenfit = adecUnreducedAccruedBenefitAmount;
            this.icdoBenefitCalculationYearlyDetail.health_hours = adecTotalHealthHours;
            this.icdoBenefitCalculationYearlyDetail.health_years_count = aintHealthCount;
            this.icdoBenefitCalculationYearlyDetail.total_accrued_benefit = adecTotalAccruedBenefitAmount;

            //this.icdoBenefitCalculationYearlyDetail.iblnHoursAfterRetirement = ablnHoursAfterRetirement;
            this.icdoBenefitCalculationYearlyDetail.reemployed_flag = astrReEmployed;

            //PIR-557
            this.icdoBenefitCalculationYearlyDetail.table_b_factor = adecTableBfactor;
            this.icdoBenefitCalculationYearlyDetail.table_a_factor = adecTableAfactor;

            this.icdoBenefitCalculationYearlyDetail.ee_contribution_amount = adecEEContribution;
            this.icdoBenefitCalculationYearlyDetail.ee_interest_amount = adecEEInterest;

        }
        //PIR 914
        public override void LoadBenefitCalculationNonsuspendibleDetails()
        {
            DataTable ldtbList = Select("cdoBenefitCalculationNonsuspendibleDetail.LoadBenefitNonSuspendibleDetails",new object[1] { icdoBenefitCalculationYearlyDetail.benefit_calculation_yearly_detail_id});

            this.iclbBenefitCalculationNonsuspendibleDetail = GetCollection<busBenefitCalculationNonsuspendibleDetail>(ldtbList, "icdoBenefitCalculationNonsuspendibleDetail");
            this.iclbBenefitCalculationNonsuspendibleDetail.ForEach(item => item.icdoBenefitCalculationNonsuspendibleDetail.PopulateDescriptions());
        }
    }
}
