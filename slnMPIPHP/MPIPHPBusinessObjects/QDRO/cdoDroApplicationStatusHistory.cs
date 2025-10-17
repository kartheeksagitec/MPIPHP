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
	/// Class MPIPHP.CustomDataObjects.cdoDroApplicationStatusHistory:
	/// Inherited from doDroApplicationStatusHistory, the class is used to customize the database object doDroApplicationStatusHistory.
	/// </summary>
    [Serializable]
	public class cdoDroApplicationStatusHistory : doDroApplicationStatusHistory
	{
		public cdoDroApplicationStatusHistory() : base()
		{
		}
    } 
} 
