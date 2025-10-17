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
	/// Class MPIPHP.CustomDataObjects.cdoIapAllocationFactor:
	/// Inherited from doIapAllocationFactor, the class is used to customize the database object doIapAllocationFactor.
	/// </summary>
    [Serializable]
	public class cdoIapAllocationFactor : doIapAllocationFactor
	{
		public cdoIapAllocationFactor() : base()
		{
		}
    } 
} 
