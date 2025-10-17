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
	/// Class MPIPHP.BusinessObjects.busQdroCalculationDetail:
	/// Inherited from busQdroCalculationDetailGen, the class is used to customize the business object busQdroCalculationDetailGen.
	/// </summary>
	[Serializable]
	public class busQdroCalculationDetail : busQdroCalculationDetailGen
    {
        public decimal idecParticipantTotalEE { get; set; }
        public void LoadChildGridForQDROCalculationMaintenance()
        {
            LoadQdroCalculationOptionss();
            LoadQdroCalculationYearlyDetails();
            LoadQdroIapAllocationDetails();
        }
        public override void LoadQdroCalculationOptionss()
        {
            base.LoadQdroCalculationOptionss();
            foreach (busQdroCalculationOptions lbusQdroCalculationOptions in iclbQdroCalculationOptions)
            {
                lbusQdroCalculationOptions.LoadPlanBenefitXr();
                lbusQdroCalculationOptions.ibusPlanBenefitXr.icdoPlanBenefitXr.PopulateDescriptions();
                lbusQdroCalculationOptions.icdoQdroCalculationOptions.istrBenefitOptionDescription =
                                    lbusQdroCalculationOptions.ibusPlanBenefitXr.icdoPlanBenefitXr.benefit_option_description;

                if (lbusQdroCalculationOptions.icdoQdroCalculationOptions.ee_flag == busConstant.FLAG_YES)
                {
                    lbusQdroCalculationOptions.icdoQdroCalculationOptions.istrBenefitOptionDescription += "(EE)";
                }

                if (lbusQdroCalculationOptions.icdoQdroCalculationOptions.uvhp_flag == busConstant.FLAG_YES)
                {
                    lbusQdroCalculationOptions.icdoQdroCalculationOptions.istrBenefitOptionDescription += "(UVHP)";
                }

                if (lbusQdroCalculationOptions.icdoQdroCalculationOptions.l52_spl_acc_flag == busConstant.FLAG_YES)
                {
                    lbusQdroCalculationOptions.icdoQdroCalculationOptions.istrBenefitOptionDescription += "(Local 52 Special Account)";
                }

                if (lbusQdroCalculationOptions.icdoQdroCalculationOptions.l161_spl_acc_flag == busConstant.FLAG_YES)
                {
                    lbusQdroCalculationOptions.icdoQdroCalculationOptions.istrBenefitOptionDescription += "(Local 161 Special Account)";
                }
            }
        }

        public void LoadData(int aintBenefitCalculationHeaderId, int aintPlanId,string astrPlanCode, int aintPersonAccountId, DateTime adtVestedDate, string astrRetirementSubType,
                            int aintDROApplicationDetailId, string astrUVHPFlag, string astrEEFlag, string astrL52SpecialAccountFlag, string astrL161SpecialAccountFlag, bool ablnNewRecord = false,decimal adecRetiredParticipantAmount = decimal.Zero)
        {
            this.iclbQdroCalculationOptions = new Collection<busQdroCalculationOptions>();
            this.iclbQdroCalculationYearlyDetail = new Collection<busQdroCalculationYearlyDetail>();
            this.iclbQdroIapAllocationDetail = new Collection<busQdroIapAllocationDetail>();
            this.icdoQdroCalculationDetail.qdro_calculation_header_id = aintBenefitCalculationHeaderId;
            this.icdoQdroCalculationDetail.person_account_id = aintPersonAccountId;
            this.icdoQdroCalculationDetail.plan_id = aintPlanId;
            this.icdoQdroCalculationDetail.istrPlanCode = astrPlanCode;
            this.icdoQdroCalculationDetail.vested_date = adtVestedDate;
            this.icdoQdroCalculationDetail.benefit_subtype_id = busConstant.RETIREMENT_TYPE_CODE_ID;
            this.icdoQdroCalculationDetail.benefit_subtype_value = astrRetirementSubType;
            this.icdoQdroCalculationDetail.status_id = busConstant.BenefitCalculation.CALCULATION_STATUS_TYPE_CODE_ID;
            this.icdoQdroCalculationDetail.status_value = busConstant.BenefitCalculation.CALCULATION_STATUS_TYPE_PENDING;
            this.icdoQdroCalculationDetail.qdro_application_detail_id = aintDROApplicationDetailId;
            this.icdoQdroCalculationDetail.istrRetirementTypeDisability = busConstant.BENEFIT_TYPE_DISABILITY_DESC;
            this.icdoQdroCalculationDetail.uvhp_flag = astrUVHPFlag;
            this.icdoQdroCalculationDetail.ee_flag = astrEEFlag;
            this.icdoQdroCalculationDetail.l161_spl_acc_flag = astrL161SpecialAccountFlag;
            this.icdoQdroCalculationDetail.l52_spl_acc_flag = astrL52SpecialAccountFlag;
            this.icdoQdroCalculationDetail.iblnIsNewRecord = ablnNewRecord;
            this.icdoQdroCalculationDetail.retired_participant_amount = adecRetiredParticipantAmount;
        }

        public void LoadPlanDescription()
        {
            if (this.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_YES)
            {
                icdoQdroCalculationDetail.istrPlanCode += "(EE)";
            }

            if (this.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_YES)
            {
                icdoQdroCalculationDetail.istrPlanCode += "(UVHP)";
            }

            if (this.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES)
            {
                icdoQdroCalculationDetail.istrPlanCode += "(Local 52 Special Account)";
            }

            if (this.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES)
            {
                icdoQdroCalculationDetail.istrPlanCode += "(Local 161 Special Account)";
            }
        }
	}
}
