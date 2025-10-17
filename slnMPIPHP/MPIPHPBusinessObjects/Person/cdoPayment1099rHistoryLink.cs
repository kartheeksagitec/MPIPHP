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
	/// Class MPIPHP.CustomDataObjects.cdoPayment1099rHistoryLink:
	/// Inherited from doPayment1099rHistoryLink, the class is used to customize the database object doPayment1099rHistoryLink.
	/// </summary>
    [Serializable]
	public class cdoPayment1099rHistoryLink : doPayment1099rHistoryLink
	{
		public cdoPayment1099rHistoryLink() : base()
		{
		}
    } 
} 
