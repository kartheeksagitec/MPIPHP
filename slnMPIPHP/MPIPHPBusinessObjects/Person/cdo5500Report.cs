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
	/// Class MPIPHP.CustomDataObjects.cdo5500Report:
	/// Inherited from do5500Report, the class is used to customize the database object do5500Report.
	/// </summary>
    [Serializable]
	public class cdo5500Report : do5500Report
	{
		public cdo5500Report() : base()
		{
		}
    } 
} 
