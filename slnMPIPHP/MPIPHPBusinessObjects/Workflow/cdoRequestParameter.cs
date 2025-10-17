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
	/// Class MPIPHP.CustomDataObjects.cdoRequestParameter:
	/// Inherited from doRequestParameter, the class is used to customize the database object doRequestParameter.
	/// </summary>
    [Serializable]
	public class cdoRequestParameter : doRequestParameter
	{
		public cdoRequestParameter() : base()
		{
		}
    } 
} 
