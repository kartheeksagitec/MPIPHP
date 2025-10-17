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
	/// Class MPIPHP.CustomDataObjects.cdoBenefitInterestRate:
	/// Inherited from doBenefitInterestRate, the class is used to customize the database object doBenefitInterestRate.
	/// </summary>
    [Serializable]
	public class cdoBenefitInterestRate : doBenefitInterestRate
	{
		public cdoBenefitInterestRate() : base()
		{
		}
    } 
} 
