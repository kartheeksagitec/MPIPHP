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
	/// Class MPIPHP.CustomDataObjects.cdoDisabilityBenefitHistory:
	/// Inherited from doDisabilityBenefitHistory, the class is used to customize the database object doDisabilityBenefitHistory.
	/// </summary>
    [Serializable]
	public class cdoDisabilityBenefitHistory : doDisabilityBenefitHistory
	{
		public cdoDisabilityBenefitHistory() : base()
		{
		}
        public string istrPlanName { get; set; }
    } 
} 
