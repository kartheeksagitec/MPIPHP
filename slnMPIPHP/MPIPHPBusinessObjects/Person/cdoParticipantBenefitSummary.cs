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
	/// Class MPIPHP.CustomDataObjects.cdoParticipantBenefitSummary:
	/// Inherited from doParticipantBenefitSummary, the class is used to customize the database object doParticipantBenefitSummary.
	/// </summary>
    [Serializable]
	public class cdoParticipantBenefitSummary : doParticipantBenefitSummary
	{
		public cdoParticipantBenefitSummary() : base()
		{
		}
    } 
} 
