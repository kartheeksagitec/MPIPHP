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
	/// Class MPIPHP.CustomDataObjects.cdoRetroItemType:
	/// Inherited from doRetroItemType, the class is used to customize the database object doRetroItemType.
	/// </summary>
    [Serializable]
	public class cdoRetroItemType : doRetroItemType
	{
		public cdoRetroItemType() : base()
		{
		}
    } 
} 
