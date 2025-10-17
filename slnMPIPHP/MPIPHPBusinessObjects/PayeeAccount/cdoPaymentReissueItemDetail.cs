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
	/// Class MPIPHP.CustomDataObjects.cdoPaymentReissueItemDetail:
	/// Inherited from doPaymentReissueItemDetail, the class is used to customize the database object doPaymentReissueItemDetail.
	/// </summary>
    [Serializable]
	public class cdoPaymentReissueItemDetail : doPaymentReissueItemDetail
	{
		public cdoPaymentReissueItemDetail() : base()
		{
		}
    } 
} 
