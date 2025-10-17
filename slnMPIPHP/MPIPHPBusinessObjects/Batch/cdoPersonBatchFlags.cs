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
	/// Class MPIPHP.CustomDataObjects.cdoPersonBatchFlags:
	/// Inherited from doPersonBatchFlags, the class is used to customize the database object doPersonBatchFlags.
	/// </summary>
    [Serializable]
	public class cdoPersonBatchFlags : doPersonBatchFlags
	{
		public cdoPersonBatchFlags() : base()
		{
		}
    } 
} 
