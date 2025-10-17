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
	/// Class MPIPHP.CustomDataObjects.cdoPirHistory:
	/// Inherited from doPirHistory, the class is used to customize the database object doPirHistory.
	/// </summary>
    [Serializable]
	public class cdoPirHistory : doPirHistory
	{
		public cdoPirHistory() : base()
		{
		}
    } 
} 
