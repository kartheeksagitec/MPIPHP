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
	/// Class MPIPHP.CustomDataObjects.cdoIapAllocationDetail:
	/// Inherited from doIapAllocationDetail, the class is used to customize the database object doIapAllocationDetail.
	/// </summary>
    [Serializable]
	public class cdoIapAllocationDetail : doIapAllocationDetail
	{
		public cdoIapAllocationDetail() : base()
		{
		}
    } 
} 
