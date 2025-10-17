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
	/// Class MPIPHP.BusinessObjects.busDisabilityRetireeIncrease:
	/// Inherited from busDisabilityRetireeIncreaseGen, the class is used to customize the business object busDisabilityRetireeIncreaseGen.
	/// </summary>
	[Serializable]
	public class busDisabilityRetireeIncrease : busDisabilityRetireeIncreaseGen
	{
        /// <summary>
        /// Insert values in disablity Retiree Increasae table
        /// </summary>
        /// <param name="aintBenefitCalculationHeaderId"></param>
        /// <param name="adecRetireeIcAmt"></param>
        /// <param name="adtRetireeIncDate"></param>
        public void InsertValuesInDisabiltyRetireeIncrease(int aintBenefitCalculationHeaderId, int aintQdroCalculationHeaderId, decimal adecRetireeIcAmt, 
                                                            DateTime adtRetireeIncDate)
        {
            if (icdoDisabilityRetireeIncrease == null)
            {
                icdoDisabilityRetireeIncrease = new cdoDisabilityRetireeIncrease();
            }

            icdoDisabilityRetireeIncrease.benefit_calculation_header_id = aintBenefitCalculationHeaderId;
            icdoDisabilityRetireeIncrease.qdro_calculation_header_id = aintQdroCalculationHeaderId;
            icdoDisabilityRetireeIncrease.retiree_increase_amount = adecRetireeIcAmt;
            icdoDisabilityRetireeIncrease.retiree_increase_date = adtRetireeIncDate;
            icdoDisabilityRetireeIncrease.Insert();
        }
	}
}
