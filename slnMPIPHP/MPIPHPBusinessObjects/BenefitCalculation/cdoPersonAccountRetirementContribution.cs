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
	/// Class MPIPHP.CustomDataObjects.cdoPersonAccountRetirementContribution:
	/// Inherited from doPersonAccountRetirementContribution, the class is used to customize the database object doPersonAccountRetirementContribution.
	/// </summary>
    [Serializable]
	public class cdoPersonAccountRetirementContribution : doPersonAccountRetirementContribution
	{
		public cdoPersonAccountRetirementContribution() : base()
		{

		}

        public int plan_id { get; set; }
        public int person_id { get; set; }
        public decimal idecEEDerivedBenefitForYear { get; set; }
        public bool iblnWithdrawalDateAfterVesting { get; set; }
    } 
} 
