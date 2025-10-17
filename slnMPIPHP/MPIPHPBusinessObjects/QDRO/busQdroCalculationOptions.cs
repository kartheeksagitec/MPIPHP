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
	/// Class MPIPHP.BusinessObjects.busQdroCalculationOptions:
	/// Inherited from busQdroCalculationOptionsGen, the class is used to customize the business object busQdroCalculationOptionsGen.
	/// </summary>
	[Serializable]
	public class busQdroCalculationOptions : busQdroCalculationOptionsGen
	{
        public void LoadData(int aintPlanBenefitId, decimal adecBenefitOptionFactor, int aintPersonId,
                                int aintBeneficiaryPersonId, decimal adecAltPayeeBenefitAmount, bool ablnNonVestedEEFlag = false, bool ablnUVHPFlag = false, bool ablnL52SpecialAccountFlag = false, bool ablnL161SpecialAccountFlag = false
            ,decimal adecEarlyReductionfactor = 1M ,decimal adecLifeConversionFactor = 1M)
        {
            if (this.icdoQdroCalculationOptions == null)
                this.icdoQdroCalculationOptions = new cdoQdroCalculationOptions();

            this.icdoQdroCalculationOptions.plan_benefit_id = aintPlanBenefitId;
            this.icdoQdroCalculationOptions.benefit_option_factor = adecBenefitOptionFactor;
            this.icdoQdroCalculationOptions.life_conversion_factor = adecLifeConversionFactor;
            this.icdoQdroCalculationOptions.early_reduction_factor = adecEarlyReductionfactor;
            this.icdoQdroCalculationOptions.alt_payee_relationship_id = busConstant.BENEFICIARY_RELATIONSHIP_CODE_ID;
            this.icdoQdroCalculationOptions.account_relationship_id = busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_CODE_ID;
            this.icdoQdroCalculationOptions.alt_payee_benefit_amount = adecAltPayeeBenefitAmount;
            

            if (aintBeneficiaryPersonId != busConstant.ZERO_INT)
            {
                // Beneficiary exists in OPUS. Check if the relationship exsits between the participant and the beneficiary
                DataTable ldtbRelationship = SelectWithOperator<cdoRelationship>(new string[] { enmRelationship.person_id.ToString(), enmRelationship.beneficiary_person_id.ToString(), enmRelationship.relationship_value.ToString() },
                    new string[] { busConstant.DBOperatorEquals, busConstant.DBOperatorEquals, busConstant.DBOperatorNotEquals }, new object[] { aintPersonId, aintBeneficiaryPersonId, busConstant.BENEFICIARY_RELATIONSHIP_OTHER }, null);

                if (ldtbRelationship != null && ldtbRelationship.Rows.Count > 0)
                {
                    busRelationship lbusRelationship = new busRelationship() { icdoRelationship = new cdoRelationship() };
                    lbusRelationship.icdoRelationship.LoadData(ldtbRelationship.Rows[0]);
                    this.icdoQdroCalculationOptions.alt_payee_relationship_value = lbusRelationship.icdoRelationship.relationship_value;
                }
            }


            if (ablnNonVestedEEFlag)
            {
                this.icdoQdroCalculationOptions.ee_flag = busConstant.FLAG_YES;
                this.icdoQdroCalculationOptions.istrBenefitOptionDescription += "(EE)";
            }
            if (ablnUVHPFlag)
            {
                this.icdoQdroCalculationOptions.uvhp_flag = busConstant.FLAG_YES;
                this.icdoQdroCalculationOptions.istrBenefitOptionDescription += "(UVHP)";
            }

            if (ablnL52SpecialAccountFlag)
            {
                this.icdoQdroCalculationOptions.istrBenefitOptionDescription += "(Local 52 Special Account)";
                this.icdoQdroCalculationOptions.l52_spl_acc_flag = busConstant.FLAG_YES;
                
            }

            if (ablnL161SpecialAccountFlag)
            {
                this.icdoQdroCalculationOptions.istrBenefitOptionDescription += "(Local 161 Special Account)";
                this.icdoQdroCalculationOptions.l161_spl_acc_flag = busConstant.FLAG_YES;
            }
        }
	}
}
