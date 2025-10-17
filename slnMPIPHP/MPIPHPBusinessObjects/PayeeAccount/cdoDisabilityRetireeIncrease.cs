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
	/// Class MPIPHP.CustomDataObjects.cdoDisabilityRetireeIncrease:
	/// Inherited from doDisabilityRetireeIncrease, the class is used to customize the database object doDisabilityRetireeIncrease.
	/// </summary>
    [Serializable]
	public class cdoDisabilityRetireeIncrease : doDisabilityRetireeIncrease
	{
		public cdoDisabilityRetireeIncrease() : base()
		{
		}
    } 
} 
