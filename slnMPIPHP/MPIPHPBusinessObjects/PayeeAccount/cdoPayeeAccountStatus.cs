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
	/// Class MPIPHP.CustomDataObjects.cdoPayeeAccountStatus:
	/// Inherited from doPayeeAccountStatus, the class is used to customize the database object doPayeeAccountStatus.
	/// </summary>
    [Serializable]
	public class cdoPayeeAccountStatus : doPayeeAccountStatus
	{
		public cdoPayeeAccountStatus() : base()
		{
		}

        public int PLAN_BENEFIT_ID { get; set; }
    } 
} 
