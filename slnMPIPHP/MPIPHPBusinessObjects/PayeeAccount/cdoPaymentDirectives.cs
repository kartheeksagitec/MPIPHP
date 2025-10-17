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
	/// Class MPIPHP.CustomDataObjects.cdoPaymentDirectives:
	/// Inherited from doPaymentDirectives, the class is used to customize the database object doPaymentDirectives.
	/// </summary>
    [Serializable]
	public class cdoPaymentDirectives : doPaymentDirectives
	{
		public cdoPaymentDirectives() : base()
		{
		}
    } 
} 
