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
	/// Class MPIPHP.CustomDataObjects.cdoIapAnnunityAdjustmentMultiplier:
	/// Inherited from doIapAnnunityAdjustmentMultiplier, the class is used to customize the database object doIapAnnunityAdjustmentMultiplier.
	/// </summary>
    [Serializable]
	public class cdoIapAnnunityAdjustmentMultiplier : doIapAnnunityAdjustmentMultiplier
	{
		public cdoIapAnnunityAdjustmentMultiplier() : base()
		{
		}

		public long iintAPrimarKey { get; set; }
    } 
} 
