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
	/// Class MPIPHP.CustomDataObjects.cdoAuditLogDetail:
	/// Inherited from doAuditLogDetail, the class is used to customize the database object doAuditLogDetail.
	/// </summary>
    [Serializable]
	public class cdoAuditLogDetail : doAuditLogDetail
	{
		public cdoAuditLogDetail() : base()
		{
		}
    } 
} 
