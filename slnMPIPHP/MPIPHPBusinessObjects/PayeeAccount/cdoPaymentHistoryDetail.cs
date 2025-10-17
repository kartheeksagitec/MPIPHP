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
	/// Class MPIPHP.CustomDataObjects.cdoPaymentHistoryDetail:
	/// Inherited from doPaymentHistoryDetail, the class is used to customize the database object doPaymentHistoryDetail.
	/// </summary>
    [Serializable]
	public class cdoPaymentHistoryDetail : doPaymentHistoryDetail
	{
		public cdoPaymentHistoryDetail() : base()
		{
          
		}
        public decimal SIGNED_AMOUNT { get; set; }  
    } 
} 
