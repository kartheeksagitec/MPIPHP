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
	/// Class MPIPHP.CustomDataObjects.cdoPersonBridgeHours:
	/// Inherited from doPersonBridgeHours, the class is used to customize the database object doPersonBridgeHours.
	/// </summary>
    [Serializable]
	public class cdoPersonBridgeHours : doPersonBridgeHours
	{
		public cdoPersonBridgeHours() : base()
		{
		}
    } 
} 
