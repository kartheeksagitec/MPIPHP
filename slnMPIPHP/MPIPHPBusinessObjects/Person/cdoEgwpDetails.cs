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
	/// Class MPIPHP.CustomDataObjects.cdoEgwpDetails:
	/// Inherited from doEgwpDetails, the class is used to customize the database object doEgwpDetails.
	/// </summary>
    [Serializable]
	public class cdoEgwpDetails : doEgwpDetails
	{
		public cdoEgwpDetails() : base()
		{
		}
    } 
} 
