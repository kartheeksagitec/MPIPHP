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
	/// Class MPIPHP.CustomDataObjects.cdoReemploymentHistory:
	/// Inherited from doReemploymentHistory, the class is used to customize the database object doReemploymentHistory.
	/// </summary>
    [Serializable]
	public class cdoReemploymentHistory : doReemploymentHistory
	{
		public cdoReemploymentHistory() : base()
		{
		}
    } 
} 
