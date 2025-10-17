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
    /// Class MPIPHP.CustomDataObjects.cdoHoliday:
	/// Inherited from doHoliday, the class is used to customize the database object doHoliday.
	/// </summary>
    [Serializable]
	public class cdoHoliday : doHoliday
	{
		public cdoHoliday() : base()
		{
		}
    } 
} 
