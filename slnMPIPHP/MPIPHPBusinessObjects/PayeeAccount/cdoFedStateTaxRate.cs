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
	/// Class MPIPHP.CustomDataObjects.cdoFedStateTaxRate:
	/// Inherited from doFedStateTaxRate, the class is used to customize the database object doFedStateTaxRate.
	/// </summary>
    [Serializable]
	public class cdoFedStateTaxRate : doFedStateTaxRate
	{
		public cdoFedStateTaxRate() : base()
		{
		}
    } 
} 
