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
	//Fw upgrade: PIR ID : 28526: Inheriting cdoFullAuditLog from fw doFullAuditLog
	//because solution side doFullAuditLog does not contain audit_details column (which is added in 6.0)
	//and hence we get exception while inserting data in sgs_full_audit_log
	/// <summary>
	/// Class MPIPHP.CustomDataObjects.cdoFullAuditLog:
	/// Inherited from doFullAuditLog, the class is used to customize the database object doFullAuditLog.
	/// </summary>
	[Serializable]
	public class cdoFullAuditLog : Sagitec.DataObjects.doFullAuditLog
	{
		public cdoFullAuditLog() : base()
		{
		}
		//Fw upgrade: PIR ID : 28660: New implementation of Audit History using audit_details
		public string istrMODIFIED_BY { get; set; }
		public string idtMODIFIED_DATE { get; set; }
	} 
} 
