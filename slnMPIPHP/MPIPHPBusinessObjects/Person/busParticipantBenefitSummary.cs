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
	/// Class MPIPHP.busParticipantBenefitSummary:
	/// Inherited from busParticipantBenefitSummaryGen, the class is used to customize the business object busParticipantBenefitSummaryGen.
	/// </summary>
	[Serializable]
	public class busParticipantBenefitSummary : busParticipantBenefitSummaryGen
	{

        public bool FindParticipantBenefitSummary(int aintPersonId,int aintPlanId)
        {
            bool lblnResult = false;
            if (icdoParticipantBenefitSummary == null)
            {
                icdoParticipantBenefitSummary = new cdoParticipantBenefitSummary();
            }
            if (icdoParticipantBenefitSummary.SelectRow(new object[2] { aintPersonId, aintPlanId }))
            {
                lblnResult = true;
            }
            return lblnResult;
        }

    }
}
