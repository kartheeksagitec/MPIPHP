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
	/// Class MPIPHP.CustomDataObjects.cdoHealthEligibiltyActuaryData:
	/// Inherited from doHealthEligibiltyActuaryData, the class is used to customize the database object doHealthEligibiltyActuaryData.
	/// </summary>
    [Serializable]
	public class cdoHealthEligibiltyActuaryData : doHealthEligibiltyActuaryData
	{
		public cdoHealthEligibiltyActuaryData() : base()
		{
		}
    } 
} 
