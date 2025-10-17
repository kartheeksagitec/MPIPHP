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
	/// Class MPIPHP.CustomDataObjects.cdoCustomSecurity:
	/// Inherited from doCustomSecurity, the class is used to customize the database object doCustomSecurity.
	/// </summary>
    [Serializable]
	public class cdoCustomSecurity : doCustomSecurity
	{
		public cdoCustomSecurity() : base()
		{
		}
    } 
} 
