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
	/// Class MPIPHP.CustomDataObjects.cdoRate:
	/// Inherited from doRate, the class is used to customize the database object doRate.
	/// </summary>
    [Serializable]
	public class cdoRate : doRate
	{
		public cdoRate() : base()
		{
		}
    } 
} 
