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
	/// Class MPIPHP.CustomDataObjects.cdoOrgBank:
	/// Inherited from doOrgBank, the class is used to customize the database object doOrgBank.
	/// </summary>
    [Serializable]
	public class cdoOrgBank : doOrgBank
	{
		public cdoOrgBank() : base()
		{
		}
        public string istrOrgBankName { get; set; }
        public string istrRoutingNumber { get; set; }
    } 
} 
