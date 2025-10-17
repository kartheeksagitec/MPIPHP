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
	/// Class MPIPHP.CustomDataObjects.cdoBenefitCalculationNonsuspendibleDetail:
	/// Inherited from doBenefitCalculationNonsuspendibleDetail, the class is used to customize the database object doBenefitCalculationNonsuspendibleDetail.
	/// </summary>
    [Serializable]
	public class cdoBenefitCalculationNonsuspendibleDetail : doBenefitCalculationNonsuspendibleDetail
	{
		public cdoBenefitCalculationNonsuspendibleDetail() : base()
		{
		}
    } 
} 
