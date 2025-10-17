#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using MPIPHP.DataObjects;

#endregion

namespace MPIPHP.CustomDataObjects
{
	/// <summary>
	/// Class MPIPHP.CustomDataObjects.cdoParticipantBenefitSummaryBatchRunDetail:
	/// Inherited from doParticipantBenefitSummaryBatchRunDetail, the class is used to customize the database object doParticipantBenefitSummaryBatchRunDetail.
	/// </summary>
    [Serializable]
	public class cdoParticipantBenefitSummaryBatchRunDetail : doParticipantBenefitSummaryBatchRunDetail
	{
		public cdoParticipantBenefitSummaryBatchRunDetail() : base()
		{
		}
    } 
} 
