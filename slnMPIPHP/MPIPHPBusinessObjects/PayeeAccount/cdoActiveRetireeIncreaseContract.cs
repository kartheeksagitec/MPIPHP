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
	/// Class MPIPHP.CustomDataObjects.cdoActiveRetireeIncreaseContract:
	/// Inherited from doActiveRetireeIncreaseContract, the class is used to customize the database object doActiveRetireeIncreaseContract.
	/// </summary>
    [Serializable]
	public class cdoActiveRetireeIncreaseContract : doActiveRetireeIncreaseContract 
	{
		public cdoActiveRetireeIncreaseContract() : base()
		{
		}
    } 
} 
