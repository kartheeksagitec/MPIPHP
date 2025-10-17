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
	/// Class MPIPHP.CustomDataObjects.cdoFedStateFlatTaxRate:
	/// Inherited from doFedStateFlatTaxRate, the class is used to customize the database object doFedStateFlatTaxRate.
	/// </summary>
    [Serializable]
	public class cdoFedStateFlatTaxRate : doFedStateFlatTaxRate
	{
		public cdoFedStateFlatTaxRate() : base()
		{
		}
    } 
} 
