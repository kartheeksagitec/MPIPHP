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
	/// Class MPIPHP.CustomDataObjects.cdoPlanBenefitRate:
	/// Inherited from doPlanBenefitRate, the class is used to customize the database object doPlanBenefitRate.
	/// </summary>
    [Serializable]
	public class cdoPlanBenefitRate : doPlanBenefitRate
	{
		public cdoPlanBenefitRate() : base()
		{
		}
    } 
} 
