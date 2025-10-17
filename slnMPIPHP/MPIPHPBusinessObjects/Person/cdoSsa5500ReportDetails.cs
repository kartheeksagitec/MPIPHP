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
	/// Class MPIPHP.CustomDataObjects.cdoSsa5500ReportDetails:
	/// Inherited from doSsa5500ReportDetails, the class is used to customize the database object doSsa5500ReportDetails.
	/// </summary>
    [Serializable]
	public class cdoSsa5500ReportDetails : doSsa5500ReportDetails
	{
		public cdoSsa5500ReportDetails() : base()
		{
		}
    } 
} 
