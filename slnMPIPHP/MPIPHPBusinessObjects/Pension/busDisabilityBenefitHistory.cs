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
	/// Class MPIPHP.BusinessObjects.busDisabilityBenefitHistory:
	/// Inherited from busDisabilityBenefitHistoryGen, the class is used to customize the business object busDisabilityBenefitHistoryGen.
	/// </summary>
	[Serializable]
	public class busDisabilityBenefitHistory : busDisabilityBenefitHistoryGen
	{
        public Collection<cdoPlan> GetPlanValues()
        {
            busDisabilityApplication lbusDisabilityApplication = new busDisabilityApplication();
            return lbusDisabilityApplication.GetPlanValues();
        }
	}
}
