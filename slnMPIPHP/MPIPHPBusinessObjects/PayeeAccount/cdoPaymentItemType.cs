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
	/// Class MPIPHP.CustomDataObjects.cdoPaymentItemType:
	/// Inherited from doPaymentItemType, the class is used to customize the database object doPaymentItemType.
	/// </summary>
    [Serializable]
	public class cdoPaymentItemType : doPaymentItemType
	{
		public cdoPaymentItemType() : base()
		{
		}
    } 
} 
