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
	/// Class MPIPHP.CustomDataObjects.cdoPersonAddressChklist:
	/// Inherited from doPersonAddressChklist, the class is used to customize the database object doPersonAddressChklist.
	/// </summary>
    [Serializable]
	public class cdoPersonAddressChklist : doPersonAddressChklist
	{
		public cdoPersonAddressChklist() : base()
		{
		}
    } 
} 
