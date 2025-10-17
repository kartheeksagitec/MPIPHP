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
	/// Class MPIPHP.CustomDataObjects.cdoBenefitProvisionEligibility:
	/// Inherited from doBenefitProvisionEligibility, the class is used to customize the database object doBenefitProvisionEligibility.
	/// </summary>
    [Serializable]
	public class cdoBenefitProvisionEligibility : doBenefitProvisionEligibility
	{
		public cdoBenefitProvisionEligibility() : base()
		{
            
		}
        public string istrPlanCode { get; set; }
    } 
} 
