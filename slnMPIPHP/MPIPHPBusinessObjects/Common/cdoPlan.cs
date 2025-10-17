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
	/// Class MPIPHP.CustomDataObjects.cdoPlan:
	/// Inherited from doPlan, the class is used to customize the database object doPlan.
	/// </summary>
    [Serializable]
	public class cdoPlan : doPlan
	{
		public cdoPlan() : base()
		{
		}
    } 
} 
