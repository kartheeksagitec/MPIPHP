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
	/// Class MPIPHP.CustomDataObjects.cdoIapRecalculationCopy:
	/// Inherited from doIapRecalculationCopy, the class is used to customize the database object doIapRecalculationCopy.
	/// </summary>
    [Serializable]
	public class cdoIapRecalculationCopy : doIapRecalculationCopy
	{
		public cdoIapRecalculationCopy() : base()
		{
		}
        public string CreatedBy { get; set; }
    } 
} 
