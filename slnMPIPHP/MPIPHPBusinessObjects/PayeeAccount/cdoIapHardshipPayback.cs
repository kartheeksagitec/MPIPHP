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
	/// Class MPIPHP.CustomDataObjects.cdoIapHardshipPayback:
	/// Inherited from doIapHardshipPayback, the class is used to customize the database object doIapHardshipPayback.
	/// </summary>
    [Serializable]
	public class cdoIapHardshipPayback : doIapHardshipPayback
	{
		public cdoIapHardshipPayback() : base()
		{
		}

        public decimal idecRunningIAPPaybackBalance { get; set; }
        public string istrIAPPaybackYear { get; set; }
        public decimal idecTotalIAPPaybacksReceived { get; set; }
    } 
} 
