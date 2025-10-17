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
	/// Class MPIPHP.CustomDataObjects.cdoActiveRetireeIncreaseContractHistory:
	/// Inherited from doActiveRetireeIncreaseContractHistory, the class is used to customize the database object doActiveRetireeIncreaseContractHistory.
	/// </summary>
    [Serializable]
	public class cdoActiveRetireeIncreaseContractHistory : doActiveRetireeIncreaseContractHistory
	{
		public cdoActiveRetireeIncreaseContractHistory() : base()
		{
		}
    } 
} 
