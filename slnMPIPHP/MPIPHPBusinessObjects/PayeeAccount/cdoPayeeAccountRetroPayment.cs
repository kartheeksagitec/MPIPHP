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
	/// Class MPIPHP.CustomDataObjects.cdoPayeeAccountRetroPayment:
	/// Inherited from doPayeeAccountRetroPayment, the class is used to customize the database object doPayeeAccountRetroPayment.
	/// </summary>
    [Serializable]
	public class cdoPayeeAccountRetroPayment : doPayeeAccountRetroPayment
	{
		public cdoPayeeAccountRetroPayment() : base()
		{
		}
    } 
} 
