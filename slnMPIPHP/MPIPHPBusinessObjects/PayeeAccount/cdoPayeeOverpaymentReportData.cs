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
	/// Class MPIPHP.CustomDataObjects.cdoPayeeOverpaymentReportData:
	/// Inherited from doPayeeOverpaymentReportData, the class is used to customize the database object doPayeeOverpaymentReportData.
	/// </summary>
    [Serializable]
	public class cdoPayeeOverpaymentReportData : doPayeeOverpaymentReportData
	{
		public cdoPayeeOverpaymentReportData() : base()
		{
		}
    } 
} 
