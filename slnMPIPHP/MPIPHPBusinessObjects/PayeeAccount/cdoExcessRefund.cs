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
	/// Class MPIPHP.CustomDataObjects.cdoExcessRefund:
	/// Inherited from doExcessRefund, the class is used to customize the database object doExcessRefund.
	/// </summary>
    [Serializable]
	public class cdoExcessRefund : doExcessRefund
	{
		public cdoExcessRefund() : base()
		{
		}

        public string istrRelativeVipFlag { get; set; }
    } 
} 
