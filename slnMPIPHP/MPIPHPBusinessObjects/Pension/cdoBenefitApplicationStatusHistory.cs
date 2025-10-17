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
	/// Class MPIPHP.CustomDataObjects.cdoBenefitApplicationStatusHistory:
	/// Inherited from doBenefitApplicationStatusHistory, the class is used to customize the database object doBenefitApplicationStatusHistory.
	/// </summary>
    [Serializable]
	public class cdoBenefitApplicationStatusHistory : doBenefitApplicationStatusHistory
	{
		public cdoBenefitApplicationStatusHistory() : base()
		{
		}
    } 
} 
