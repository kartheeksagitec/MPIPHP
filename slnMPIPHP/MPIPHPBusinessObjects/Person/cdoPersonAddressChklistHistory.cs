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
	/// Class MPIPHP.CustomDataObjects.cdoPersonAddressChklistHistory:
	/// Inherited from doPersonAddressChklistHistory, the class is used to customize the database object doPersonAddressChklistHistory.
	/// </summary>
    [Serializable]
	public class cdoPersonAddressChklistHistory : doPersonAddressChklistHistory
	{
		public cdoPersonAddressChklistHistory() : base()
		{
		}
    } 
} 
