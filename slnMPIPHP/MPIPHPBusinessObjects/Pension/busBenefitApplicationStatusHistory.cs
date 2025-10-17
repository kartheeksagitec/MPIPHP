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
	/// Class MPIPHP.BusinessObjects.busBenefitApplicationStatusHistory:
	/// Inherited from busBenefitApplicationStatusHistoryGen, the class is used to customize the business object busBenefitApplicationStatusHistoryGen.
	/// </summary>
	[Serializable]
	public class busBenefitApplicationStatusHistory : busBenefitApplicationStatusHistoryGen
	{
        public bool InsertStatusHistory(int aintBenefitApplicationId, string astrStatusValue, DateTime adtStatusDate, string astrModifiedBy)
        {
            bool lblnResult = false;

            if (icdoBenefitApplicationStatusHistory == null)
                icdoBenefitApplicationStatusHistory = new cdoBenefitApplicationStatusHistory();

            icdoBenefitApplicationStatusHistory.benefit_application_id = aintBenefitApplicationId;
            icdoBenefitApplicationStatusHistory.status_id = busConstant.BENEFIT_APPLICATION_STATUS_CODE_ID;
            icdoBenefitApplicationStatusHistory.status_value = astrStatusValue;
            icdoBenefitApplicationStatusHistory.status_date = adtStatusDate;
            icdoBenefitApplicationStatusHistory.modified_by = astrModifiedBy;
            icdoBenefitApplicationStatusHistory.Insert();

            return lblnResult;
        }
	}
}
