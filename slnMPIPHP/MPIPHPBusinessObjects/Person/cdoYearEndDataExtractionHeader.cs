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
	/// Class MPIPHP.CustomDataObjects.cdoYearEndDataExtractionHeader:
	/// Inherited from doYearEndDataExtractionHeader, the class is used to customize the database object doYearEndDataExtractionHeader.
	/// </summary>
    [Serializable]
	public class cdoYearEndDataExtractionHeader : doYearEndDataExtractionHeader
	{
		public cdoYearEndDataExtractionHeader() : base()
		{
		}
    } 
} 
