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
	/// Class MPIPHP.CustomDataObjects.cdoFullAuditLogDetail:
	/// Inherited from doFullAuditLogDetail, the class is used to customize the database object doFullAuditLogDetail.
	/// </summary>
    [Serializable]
	public class cdoFullAuditLogDetail : doFullAuditLogDetail
	{
        public string istrMODIFIED_BY { get; set; }
        public string idtMODIFIED_DATE { get; set; }
		
        public cdoFullAuditLogDetail() : base()
		{
           
		}
    } 
} 
