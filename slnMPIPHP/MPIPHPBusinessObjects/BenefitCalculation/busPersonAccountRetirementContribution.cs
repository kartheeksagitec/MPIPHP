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
using System.Linq;

#endregion

namespace MPIPHP.BusinessObjects
{
	/// <summary>
	/// Class MPIPHP.BusinessObjects.busPersonAccountRetirementContribution:
	/// Inherited from busPersonAccountRetirementContributionGen, the class is used to customize the business object busPersonAccountRetirementContributionGen.
	/// </summary>
	[Serializable]
	public class busPersonAccountRetirementContribution : busPersonAccountRetirementContributionGen
	{

        public busPerson ibusPerson { get; set; }

        public void InsertPersonAccountRetirementContirbution(int aintPersonAccountID, DateTime adtEffectiveDate, DateTime adtTransactionDate, int aintComputationYear, decimal adecIAPBalanceAmount = 0.00M, decimal adec52SplAccountBalance = 0.00M,
          decimal adec161SplAccountBalance = 0.00M, string astrTransactionType = null, decimal adecEEContrAmount = 0.00M, decimal adecUVHPAmount = 0.00M, decimal adecEEInterestAmount = 0.00M, decimal adecUVHPInterestAmount = 0.00M,
          decimal adecLocalFrozenAmount = 0.00M, string astrContributionType = null, string astrContributionSubtype = null, decimal adecLocalPreBISAmount = 0.00M, decimal adecLocalPostBISAmount = 0.00M, int aintReferenceID = 0, string astrRecordFreezeFlag = null)
        {


            icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution();
            icdoPersonAccountRetirementContribution.person_account_id = aintPersonAccountID;
            icdoPersonAccountRetirementContribution.effective_date = adtEffectiveDate;
            icdoPersonAccountRetirementContribution.transaction_date = adtTransactionDate;
            icdoPersonAccountRetirementContribution.computational_year = aintComputationYear;

            if (busGlobalFunctions.CheckIfPrecisionExceeds(adecIAPBalanceAmount, 11))
                return;
            else
                icdoPersonAccountRetirementContribution.iap_balance_amount = adecIAPBalanceAmount;

            if (busGlobalFunctions.CheckIfPrecisionExceeds(adec52SplAccountBalance, 11))
                return;
            else
                icdoPersonAccountRetirementContribution.local52_special_acct_bal_amount = Math.Round(adec52SplAccountBalance, 2);

            if (busGlobalFunctions.CheckIfPrecisionExceeds(adec161SplAccountBalance, 11))
                return;
            else
                icdoPersonAccountRetirementContribution.local161_special_acct_bal_amount = Math.Round(adec161SplAccountBalance, 2);
            icdoPersonAccountRetirementContribution.transaction_type_value = astrTransactionType;

            if (busGlobalFunctions.CheckIfPrecisionExceeds(adecEEContrAmount, 11))
                return;
            else
                icdoPersonAccountRetirementContribution.ee_contribution_amount = Math.Round(adecEEContrAmount, 2);

            if (busGlobalFunctions.CheckIfPrecisionExceeds(adecUVHPAmount, 11))
                return;
            else
                icdoPersonAccountRetirementContribution.uvhp_amount = Math.Round(adecUVHPAmount, 2);

            if (busGlobalFunctions.CheckIfPrecisionExceeds(adecEEInterestAmount, 11))
                return;
            else
                icdoPersonAccountRetirementContribution.ee_int_amount = Math.Round(adecEEInterestAmount, 2);

            if (busGlobalFunctions.CheckIfPrecisionExceeds(adecUVHPInterestAmount, 11))
                return;
            else
                icdoPersonAccountRetirementContribution.uvhp_int_amount = Math.Round(adecUVHPInterestAmount, 2);

            if (busGlobalFunctions.CheckIfPrecisionExceeds(adecLocalFrozenAmount, 11))
                return;
            else
                icdoPersonAccountRetirementContribution.local_frozen_benefit_amount = Math.Round(adecLocalFrozenAmount, 2);

            icdoPersonAccountRetirementContribution.contribution_type_value = astrContributionType;
            icdoPersonAccountRetirementContribution.contribution_subtype_value = astrContributionSubtype;

            if (busGlobalFunctions.CheckIfPrecisionExceeds(adecLocalPreBISAmount, 11))
                return;
            else
                icdoPersonAccountRetirementContribution.local_pre_bis_amount = Math.Round(adecLocalPreBISAmount, 2);

            if (busGlobalFunctions.CheckIfPrecisionExceeds(adecLocalPostBISAmount, 11))
                return;
            else
                icdoPersonAccountRetirementContribution.local_post_bis_amount = Math.Round(adecLocalPostBISAmount, 2);

            icdoPersonAccountRetirementContribution.reference_id = aintReferenceID;
            icdoPersonAccountRetirementContribution.record_freeze_flag = astrRecordFreezeFlag;
            icdoPersonAccountRetirementContribution.Insert();
        }


        public override void BeforePersistChanges()
        {
            base.BeforePersistChanges();
            if (this.ibusPerson.iclbPersonAccount.Where(t => t.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).Count() > 0)
            {
                this.icdoPersonAccountRetirementContribution.person_account_id =
                    this.ibusPerson.iclbPersonAccount.Where(t => t.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().icdoPersonAccount.person_account_id;
            }

            this.icdoPersonAccountRetirementContribution.effective_date = busGlobalFunctions.GetLastDateOfComputationYear(Convert.ToInt32(this.icdoPersonAccountRetirementContribution.computational_year)).AddDays(1);
            this.icdoPersonAccountRetirementContribution.transaction_date = DateTime.Now;
            this.icdoPersonAccountRetirementContribution.computational_year = this.icdoPersonAccountRetirementContribution.computational_year;
            this.icdoPersonAccountRetirementContribution.transaction_type_id = busConstant.TRANSACTION_TYPE_CODE_ID;
            this.icdoPersonAccountRetirementContribution.contribution_type_id = busConstant.CONTRIBUTION_TYPE_CODE_ID;
            this.icdoPersonAccountRetirementContribution.contribution_type_value = busConstant.UVHP;
            this.icdoPersonAccountRetirementContribution.contribution_subtype_id = busConstant.CONTRIBUTION_SUBTYPE_CODE_ID;

        }
	}
}
