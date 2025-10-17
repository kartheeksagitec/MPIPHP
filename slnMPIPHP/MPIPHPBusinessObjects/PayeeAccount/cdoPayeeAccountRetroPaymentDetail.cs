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
	/// Class MPIPHP.CustomDataObjects.cdoPayeeAccountRetroPaymentDetail:
	/// Inherited from doPayeeAccountRetroPaymentDetail, the class is used to customize the database object doPayeeAccountRetroPaymentDetail.
	/// </summary>
    [Serializable]
	public class cdoPayeeAccountRetroPaymentDetail : doPayeeAccountRetroPaymentDetail
	{
        public string istrPaymentItemTypedesription { get; set; }
		public cdoPayeeAccountRetroPaymentDetail() : base()
		{
		}
    } 
} 
