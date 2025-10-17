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
	/// Class MPIPHP.CustomDataObjects.cdoWithholdingInformationHistoryDetail:
	/// Inherited from doWithholdingInformationHistoryDetail, the class is used to customize the database object doWithholdingInformationHistoryDetail.
	/// </summary>
    [Serializable]
	public class cdoWithholdingInformationHistoryDetail : doWithholdingInformationHistoryDetail
	{
		public cdoWithholdingInformationHistoryDetail() : base()
		{
		}
    } 
} 
