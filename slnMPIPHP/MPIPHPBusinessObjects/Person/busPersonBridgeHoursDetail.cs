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
	/// Class MPIPHP.BusinessObjects.busPersonBridgeHoursDetail:
	/// Inherited from busPersonBridgeHoursDetailGen, the class is used to customize the business object busPersonBridgeHoursDetailGen.
	/// </summary>
	[Serializable]
	public class busPersonBridgeHoursDetail : busPersonBridgeHoursDetailGen
	{
        public void InsertDataInPersonBridgedTable(int aintPersonBridgeId, int aintComputationYear, decimal adecHours,
                                                    DateTime adtFromDate, DateTime adtTodate)
        {
            if (icdoPersonBridgeHoursDetail == null)
            {
                icdoPersonBridgeHoursDetail = new cdoPersonBridgeHoursDetail();
            }

            icdoPersonBridgeHoursDetail.person_bridge_id = aintPersonBridgeId;
            icdoPersonBridgeHoursDetail.computation_year = aintComputationYear;
            icdoPersonBridgeHoursDetail.hours = adecHours;
            icdoPersonBridgeHoursDetail.from_date = adtFromDate;
            icdoPersonBridgeHoursDetail.to_date = adtTodate;
            icdoPersonBridgeHoursDetail.Insert();
        }
        
	}
}
