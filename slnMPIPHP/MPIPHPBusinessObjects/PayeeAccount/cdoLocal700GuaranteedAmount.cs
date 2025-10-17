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
	/// Class MPIPHP.CustomDataObjects.cdoLocal700GuaranteedAmount:
	/// Inherited from doLocal700GuaranteedAmount, the class is used to customize the database object doLocal700GuaranteedAmount.
	/// </summary>
    [Serializable]
	public class cdoLocal700GuaranteedAmount : doLocal700GuaranteedAmount
	{
		public cdoLocal700GuaranteedAmount() : base()
		{
		}
    } 
} 
