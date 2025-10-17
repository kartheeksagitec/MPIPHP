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
	/// Class MPIPHP.CustomDataObjects.cdoPaymentCheckBook:
	/// Inherited from doPaymentCheckBook, the class is used to customize the database object doPaymentCheckBook.
	/// </summary>
    [Serializable]
	public class cdoPaymentCheckBook : doPaymentCheckBook
	{
		public cdoPaymentCheckBook() : base()
		{
		}
     
    } 
} 
