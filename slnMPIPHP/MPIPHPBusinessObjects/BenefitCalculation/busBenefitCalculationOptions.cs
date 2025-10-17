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
using MPIPHP.DataObjects;

#endregion

namespace MPIPHP.BusinessObjects
{
	/// <summary>
	/// Class MPIPHP.BusinessObjects.busBenefitCalculationOptions:
	/// Inherited from busBenefitCalculationOptionsGen, the class is used to customize the business object busBenefitCalculationOptionsGen.
	/// </summary>
	[Serializable]
	public class busBenefitCalculationOptions : busBenefitCalculationOptionsGen
	{
        public int iintParticipantAge { get; set; }
        public int iintSpouseAge { get; set; }
        public string istrRetirementType { get; set; }
        public string istrInitialOne { get; set; }
        public decimal idecRegularReductionFactor { get; set; }

        public DateTime idtEarliestBenefitCommencementDate { get; set; }
        public string istrJandSAnnuityFactorForLumpSum { get; set; }

        public string istrJS50 { get; set; }
        public string istrJS100 { get; set; }
        public string istrLife { get; set; }
        public string istrJP50 { get; set; }
        public string istrTenYear { get; set; }
        public string istrJP100 { get; set; }
        public string istrJS75 { get; set; }
        public string istrERFPercentage { get; set; }

        //start rid 78456
        public string istrTwoYear { get; set; }
        public string istrJS66 { get; set; }
        public string istrThreeYear { get; set; }
        public string istrFiveYear { get; set; }
        //end rid 78456

        //Post Retirement Death


        public void LoadData(int aintPlanBenefitId, decimal adecBenefitOptionFactor, decimal adecBenefitAmount, string astrAccountRelationship, int aintPersonId, int aintBeneficiaryPersonId, string strBenefitOptionDescription,
                             decimal adecSurvivorBenefitAmount = busConstant.ZERO_DECIMAL, bool ablnEEFlag = false, bool ablnUVHPFlag = false, bool ablnL52SpecialAccountBenefitFlag = false, bool ablnL161SpecialAccountBenefitFlag = false, decimal adecPopupoptionfactor = busConstant.ZERO_DECIMAL,
                             decimal adecpopupbenefitamount = busConstant.ZERO_DECIMAL, decimal adecpopupoptionfactoratret = busConstant.ZERO_DECIMAL)
        {
            this.icdoBenefitCalculationOptions.plan_benefit_id = aintPlanBenefitId;
            this.icdoBenefitCalculationOptions.benefit_option_factor = adecBenefitOptionFactor.RoundDecimalPoints(3);
            this.icdoBenefitCalculationOptions.benefit_amount = adecBenefitAmount;
            this.icdoBenefitCalculationOptions.account_relationship_id = busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_CODE_ID;
            this.icdoBenefitCalculationOptions.account_relationship_value = astrAccountRelationship;
            this.icdoBenefitCalculationOptions.survivor_relationship_id = busConstant.BENEFICIARY_RELATIONSHIP_CODE_ID;
            this.icdoBenefitCalculationOptions.survivor_amount = adecSurvivorBenefitAmount;
            this.icdoBenefitCalculationOptions.istrBenefitOptionDescription = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(1504, strBenefitOptionDescription);
            this.icdoBenefitCalculationOptions.pop_up_option_factor = adecPopupoptionfactor;
            this.icdoBenefitCalculationOptions.pop_up_benefit_amount = adecpopupbenefitamount;
            this.icdoBenefitCalculationOptions.pop_up_option_factor_at_ret = adecpopupoptionfactoratret;

            if (aintBeneficiaryPersonId != busConstant.ZERO_INT)
            {
                // Beneficiary exists in OPUS. Check if the relationship exsits between the participant and the beneficiary
                DataTable ldtbRelationship = SelectWithOperator<cdoRelationship>(new string[] { enmRelationship.person_id.ToString(), enmRelationship.beneficiary_person_id.ToString(), enmRelationship.relationship_value.ToString() },
                    new string[] { busConstant.DBOperatorEquals, busConstant.DBOperatorEquals, busConstant.DBOperatorNotEquals }, new object[] { aintPersonId, aintBeneficiaryPersonId, busConstant.BENEFICIARY_RELATIONSHIP_OTHER }, null);

                if (ldtbRelationship != null && ldtbRelationship.Rows.Count > 0)
                {
                    busRelationship lbusRelationship = new busRelationship() { icdoRelationship = new cdoRelationship() };
                    lbusRelationship.icdoRelationship.LoadData(ldtbRelationship.Rows[0]);
                    this.icdoBenefitCalculationOptions.survivor_relationship_value = lbusRelationship.icdoRelationship.relationship_value;
                }

            }
            
            //this.LoadPlanBenefitXr();
            //this.ibusPlanBenefitXr.icdoPlanBenefitXr.PopulateDescriptions();
            //this.icdoBenefitCalculationOptions.istrBenefitOptionDescription = this.ibusPlanBenefitXr.icdoPlanBenefitXr.benefit_option_description;
            if (ablnEEFlag && !ablnUVHPFlag)
            {
                this.icdoBenefitCalculationOptions.ee_flag = busConstant.FLAG_YES;
                this.icdoBenefitCalculationOptions.istrBenefitOptionDescription += "(EE)";
            }
            if (ablnUVHPFlag && !ablnEEFlag)
            {
                this.icdoBenefitCalculationOptions.uvhp_flag = busConstant.FLAG_YES;
                this.icdoBenefitCalculationOptions.istrBenefitOptionDescription += "(UVHP)";
            }

            if (ablnUVHPFlag && ablnEEFlag)//Death
            {
                this.icdoBenefitCalculationOptions.uvhp_flag = busConstant.FLAG_YES;
                this.icdoBenefitCalculationOptions.ee_flag = busConstant.FLAG_YES;
                this.icdoBenefitCalculationOptions.istrBenefitOptionDescription += "(EE and UVHP)";

            }

            if (ablnL52SpecialAccountBenefitFlag)
            {
                this.icdoBenefitCalculationOptions.istrBenefitOptionDescription += "(Local 52 Special Account)";
                this.icdoBenefitCalculationOptions.local52_special_acct_bal_flag = busConstant.FLAG_YES;
            }

            if (ablnL161SpecialAccountBenefitFlag)
            {
                this.icdoBenefitCalculationOptions.istrBenefitOptionDescription += "(Local 161 Special Account)";
                this.icdoBenefitCalculationOptions.local161_special_acct_bal_flag = busConstant.FLAG_YES;
            }
        }

        public void LoadDisabilityData(int aintPlanBenefitId, decimal adecBenefitOptionFactor, decimal adecBenefitAmount, string astrAccountRelationship, int aintPersonId, int aintBeneficiaryPersonId, string strBenefitOptionDescription, decimal adecParticipantAmount,
                             decimal adecSurvivorBenefitAmount = busConstant.ZERO_DECIMAL, decimal adecDisabilityFactor =busConstant.ZERO_DECIMAL ,decimal adecDisabiltyBenefit =  busConstant.ZERO_DECIMAL,decimal adecRetroActiveAmount = busConstant.ZERO_DECIMAL,decimal adecRegularReductionFactor = decimal.Zero,bool ablnEEFlag = false, bool ablnUVHPFlag = false, bool ablnL52SpecialAccountBenefitFlag = false, bool ablnL161SpecialAccountBenefitFlag = false, 
                             decimal adecDisabilityPopupFactor = busConstant.ZERO_DECIMAL, decimal adecDisabiltyPopupBenefit = busConstant.ZERO_DECIMAL,
                             decimal adecDisabilityPopupFactoratRet = busConstant.ZERO_DECIMAL) //RequestID: 72091
        {
            this.icdoBenefitCalculationOptions.plan_benefit_id = aintPlanBenefitId;
            this.icdoBenefitCalculationOptions.benefit_option_factor = adecBenefitOptionFactor.RoundDecimalPoints(3);
            this.icdoBenefitCalculationOptions.benefit_amount = adecBenefitAmount;
            this.icdoBenefitCalculationOptions.account_relationship_id = busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_CODE_ID;
            this.icdoBenefitCalculationOptions.account_relationship_value = astrAccountRelationship;
            this.icdoBenefitCalculationOptions.survivor_relationship_id = busConstant.BENEFICIARY_RELATIONSHIP_CODE_ID;
            this.icdoBenefitCalculationOptions.survivor_amount = adecSurvivorBenefitAmount;
            this.icdoBenefitCalculationOptions.istrBenefitOptionDescription = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(1504, strBenefitOptionDescription);
            this.icdoBenefitCalculationOptions.disability_amount = adecDisabiltyBenefit;
            //RequestID:72091
            this.icdoBenefitCalculationOptions.pop_up_option_factor = adecDisabilityPopupFactor;
            this.icdoBenefitCalculationOptions.pop_up_benefit_amount = adecDisabiltyPopupBenefit;
            //Kunal - Chech Rounding of adecDisabilityFactor -Need change in future
            this.icdoBenefitCalculationOptions.disability_factor = adecDisabilityFactor.RoundDecimalPoints(3);
            this.icdoBenefitCalculationOptions.participant_amount = adecParticipantAmount;
            this.icdoBenefitCalculationOptions.retroactive_amount = adecRetroActiveAmount;
            this.idecRegularReductionFactor = adecRegularReductionFactor;
            this.icdoBenefitCalculationOptions.pop_up_option_factor_at_ret = adecDisabilityPopupFactoratRet;

            if (aintBeneficiaryPersonId != busConstant.ZERO_INT)
            {
                DataTable ldtbRelationship = SelectWithOperator<cdoRelationship>(new string[] { enmRelationship.person_id.ToString(), enmRelationship.beneficiary_person_id.ToString(), enmRelationship.relationship_value.ToString() },
                    new string[] { busConstant.DBOperatorEquals, busConstant.DBOperatorEquals, busConstant.DBOperatorNotEquals }, new object[] { aintPersonId, aintBeneficiaryPersonId, busConstant.BENEFICIARY_RELATIONSHIP_OTHER }, null);

            if (ldtbRelationship != null && ldtbRelationship.Rows.Count > 0)
                {
                    busRelationship lbusRelationship = new busRelationship() { icdoRelationship = new cdoRelationship() };
                    lbusRelationship.icdoRelationship.LoadData(ldtbRelationship.Rows[0]);
                    this.icdoBenefitCalculationOptions.survivor_relationship_value = lbusRelationship.icdoRelationship.relationship_value;
                }

            }
            if (ablnEEFlag)
            {
                this.icdoBenefitCalculationOptions.ee_flag = busConstant.FLAG_YES;
                this.icdoBenefitCalculationOptions.istrBenefitOptionDescription += "(EE)";
            }
            if (ablnUVHPFlag)
            {
                this.icdoBenefitCalculationOptions.uvhp_flag = busConstant.FLAG_YES;
                this.icdoBenefitCalculationOptions.istrBenefitOptionDescription += "(UVHP)";
            }

            if (ablnL52SpecialAccountBenefitFlag)
            {
                this.icdoBenefitCalculationOptions.istrBenefitOptionDescription += "(Local 52 Special Account)";
                this.icdoBenefitCalculationOptions.local52_special_acct_bal_flag = busConstant.FLAG_YES;
            }

            if (ablnL161SpecialAccountBenefitFlag)
            {
                this.icdoBenefitCalculationOptions.istrBenefitOptionDescription += "(Local 161 Special Account)";
                this.icdoBenefitCalculationOptions.local161_special_acct_bal_flag = busConstant.FLAG_YES;
            }
        }
    }
}
