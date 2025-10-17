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
	/// Class MPIPHP.BusinessObjects.busDroApplicationStatusHistory:
	/// Inherited from busDroApplicationStatusHistoryGen, the class is used to customize the business object busDroApplicationStatusHistoryGen.
	/// </summary>
	[Serializable]
	public class busDroApplicationStatusHistory : busDroApplicationStatusHistoryGen
	{
        public bool InsertStatusHistory(int aintDroApplicationId, string astrStatusValue, DateTime adtStatusDate, string astrModifiedBy)
        {
            bool lblnResult = false;

            if (icdoDroApplicationStatusHistory == null)
                icdoDroApplicationStatusHistory = new cdoDroApplicationStatusHistory();

            icdoDroApplicationStatusHistory.dro_application_id = aintDroApplicationId;
            icdoDroApplicationStatusHistory.status_id = busConstant.DRO_APPLICATION_STATUS_CODE_ID;
            icdoDroApplicationStatusHistory.status_value = astrStatusValue;
            icdoDroApplicationStatusHistory.status_date = adtStatusDate;
            icdoDroApplicationStatusHistory.modified_by = astrModifiedBy;
            icdoDroApplicationStatusHistory.Insert();

            return lblnResult;
        }
	}
}
