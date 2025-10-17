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
	/// Class MPIPHP.CustomDataObjects.cdoVipStatusHistory:
	/// Inherited from doVipStatusHistory, the class is used to customize the database object doVipStatusHistory.
	/// </summary>
    [Serializable]
	public class cdoVipStatusHistory : doVipStatusHistory
	{
		public cdoVipStatusHistory() : base()
		{
		}
    } 
} 
