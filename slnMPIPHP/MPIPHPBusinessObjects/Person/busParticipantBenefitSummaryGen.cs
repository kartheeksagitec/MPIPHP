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
using MPIPHP.BusinessObjects;
using MPIPHP.DataObjects;

#endregion

namespace MPIPHP.BusinessObjects
{
    /// <summary>
    /// Class MPIPHP.busParticipantBenefitSummaryGen:
    /// Inherited from busBase, used to create new business object for main table cdoParticipantBenefitSummary and its children table. 
    /// </summary>
	[Serializable]
	public class busParticipantBenefitSummaryGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.busParticipantBenefitSummaryGen
        /// </summary>
		public busParticipantBenefitSummaryGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busParticipantBenefitSummaryGen.
        /// </summary>
		public cdoParticipantBenefitSummary icdoParticipantBenefitSummary { get; set; }




        /// <summary>
        /// MPIPHP.busParticipantBenefitSummaryGen.FindParticipantBenefitSummary():
        /// Finds a particular record from cdoParticipantBenefitSummary with its primary key. 
        /// </summary>
        /// <param name="aintParticipantBenefitSummaryId">A primary key value of type int of cdoParticipantBenefitSummary on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindParticipantBenefitSummary(int aintParticipantBenefitSummaryId)
		{
			bool lblnResult = false;
			if (icdoParticipantBenefitSummary == null)
			{
				icdoParticipantBenefitSummary = new cdoParticipantBenefitSummary();
			}
			if (icdoParticipantBenefitSummary.SelectRow(new object[1] { aintParticipantBenefitSummaryId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
