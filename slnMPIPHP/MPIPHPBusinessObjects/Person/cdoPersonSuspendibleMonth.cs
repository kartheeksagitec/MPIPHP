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
	/// Class MPIPHP.CustomDataObjects.cdoPersonSuspendibleMonth:
	/// Inherited from doPersonSuspendibleMonth, the class is used to customize the database object doPersonSuspendibleMonth.
	/// </summary>
    [Serializable]
	public class cdoPersonSuspendibleMonth : doPersonSuspendibleMonth
	{
		public cdoPersonSuspendibleMonth() : base()
		{
		}
       
    } 
} 
