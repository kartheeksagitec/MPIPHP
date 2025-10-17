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
	/// Class MPIPHP.CustomDataObjects.cdoProviderReportPayment:
	/// Inherited from doProviderReportPayment, the class is used to customize the database object doProviderReportPayment.
	/// </summary>
    [Serializable]
	public class cdoProviderReportPayment : doProviderReportPayment
	{
		public cdoProviderReportPayment() : base()
		{
		}
    } 
} 
