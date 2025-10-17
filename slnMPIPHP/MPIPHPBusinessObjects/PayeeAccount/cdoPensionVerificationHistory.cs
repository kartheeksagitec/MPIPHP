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
	/// Class MPIPHP.CustomDataObjects.cdoPensionVerificationHistory:
	/// Inherited from doPensionVerificationHistory, the class is used to customize the database object doPensionVerificationHistory.
	/// </summary>
    [Serializable]
	public class cdoPensionVerificationHistory : doPensionVerificationHistory
	{
		public cdoPensionVerificationHistory() : base()
		{
		}
    } 
} 
