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
	/// Class MPIPHP.CustomDataObjects.cdoQdroCalculationYearlyDetail:
	/// Inherited from doQdroCalculationYearlyDetail, the class is used to customize the database object doQdroCalculationYearlyDetail.
	/// </summary>
    [Serializable]
	public class cdoQdroCalculationYearlyDetail : doQdroCalculationYearlyDetail
	{
		public cdoQdroCalculationYearlyDetail() : base()
		{
		}
    } 
} 
