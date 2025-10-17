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
	/// Class MPIPHP.CustomDataObjects.cdoDataExtractionBatchHourInfo:
	/// Inherited from doDataExtractionBatchHourInfo, the class is used to customize the database object doDataExtractionBatchHourInfo.
	/// </summary>
    [Serializable]
	public class cdoDataExtractionBatchHourInfo : doDataExtractionBatchHourInfo
	{
		public cdoDataExtractionBatchHourInfo() : base()
		{
		}
    } 
} 
