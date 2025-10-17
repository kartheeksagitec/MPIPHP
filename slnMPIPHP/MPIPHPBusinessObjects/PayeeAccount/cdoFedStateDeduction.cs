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
	/// Class MPIPHP.CustomDataObjects.cdoFedStateDeduction:
	/// Inherited from doFedStateDeduction, the class is used to customize the database object doFedStateDeduction.
	/// </summary>
    [Serializable]
	public class cdoFedStateDeduction : doFedStateDeduction
	{
		public cdoFedStateDeduction() : base()
		{
		}
    } 
} 
