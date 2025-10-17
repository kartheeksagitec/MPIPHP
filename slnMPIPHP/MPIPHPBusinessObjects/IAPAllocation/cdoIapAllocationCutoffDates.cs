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
	/// Class MPIPHP.CustomDataObjects.cdoIapAllocationCutoffDates:
	/// Inherited from doIapAllocationCutoffDates, the class is used to customize the database object doIapAllocationCutoffDates.
	/// </summary>
    [Serializable]
	public class cdoIapAllocationCutoffDates : doIapAllocationCutoffDates
	{
		public cdoIapAllocationCutoffDates() : base()
		{
		}
    } 
} 
