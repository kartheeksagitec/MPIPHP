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
	/// Class MPIPHP.BusinessObjects.busActiveRetireeIncreaseContractHistory:
	/// Inherited from busActiveRetireeIncreaseContractHistoryGen, the class is used to customize the business object busActiveRetireeIncreaseContractHistoryGen.
	/// </summary>
	[Serializable]
	public class busActiveRetireeIncreaseContractHistory : busActiveRetireeIncreaseContractHistoryGen  
	{
        public void InsertValuesInActiveRetireeIncreaseContractHistory(int aintRetireeIncContractId, int aintPlanYear, DateTime adtEffectiveStartDate,
                                                                        DateTime adtEffectiveEndDate, string astrPercentInc)
        {
            if (icdoActiveRetireeIncreaseContractHistory == null)
            {
                icdoActiveRetireeIncreaseContractHistory = new cdoActiveRetireeIncreaseContractHistory();
            }

            icdoActiveRetireeIncreaseContractHistory.active_retiree_increase_contract_id = aintRetireeIncContractId;
            icdoActiveRetireeIncreaseContractHistory.plan_year = aintPlanYear;
            icdoActiveRetireeIncreaseContractHistory.effective_start_date = adtEffectiveStartDate;
            icdoActiveRetireeIncreaseContractHistory.effective_end_date = adtEffectiveEndDate;
            icdoActiveRetireeIncreaseContractHistory.percent_increase_id = busConstant.INCREASE_PERCENT;
            icdoActiveRetireeIncreaseContractHistory.percent_increase_value = astrPercentInc;
            icdoActiveRetireeIncreaseContractHistory.Insert();
        }
	}
}
