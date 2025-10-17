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
	/// Class MPIPHP.CustomDataObjects.cdoPersonBridgeHoursDetail:
	/// Inherited from doPersonBridgeHoursDetail, the class is used to customize the database object doPersonBridgeHoursDetail.
	/// </summary>
    [Serializable]
	public class cdoPersonBridgeHoursDetail : doPersonBridgeHoursDetail
	{
		public cdoPersonBridgeHoursDetail() : base()
		{
		}
        public string BRIDGE_TYPE_VALUE { get; set; }
    } 
} 
