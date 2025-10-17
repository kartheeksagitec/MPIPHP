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
	/// Class MPIPHP.CustomDataObjects.cdoUserSecurityHistory:
	/// Inherited from doUserSecurityHistory, the class is used to customize the database object doUserSecurityHistory.
	/// </summary>
    [Serializable]
	public class cdoUserSecurityHistory : doUserSecurityHistory
	{
		public cdoUserSecurityHistory() : base()
		{
		}
    } 
} 
