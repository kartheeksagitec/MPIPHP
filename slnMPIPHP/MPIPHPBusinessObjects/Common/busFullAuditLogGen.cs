#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using MPIPHP.CustomDataObjects;
using MPIPHP.DataObjects;

#endregion

namespace MPIPHP.BusinessObjects
{
    /// <summary>
    /// Class MPIPHP.BusinessObjects.busFullAuditLogGen:
    /// Inherited from busBase, used to create new business object for main table cdoFullAuditLog and its children table. 
    /// </summary>
	[Serializable]
	public class busFullAuditLogGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.BusinessObjects.busFullAuditLogGen
        /// </summary>
		public busFullAuditLogGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busFullAuditLogGen.
        /// </summary>
		public cdoFullAuditLog icdoFullAuditLog { get; set; }


        /// <summary>
        /// Gets or sets the collection object of type busFullAuditLogDetail. 
        /// </summary>
		public Collection<busFullAuditLogDetail> iclbFullAuditLogDetail { get; set; }



        /// <summary>
        /// MPIPHP.busFullAuditLogGen.FindFullAuditLog():
        /// Finds a particular record from cdoFullAuditLog with its primary key. 
        /// </summary>
        /// <param name="aintAuditLogId">A primary key value of type int of cdoFullAuditLog on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindFullAuditLog(int aintAuditLogId)
		{
			bool lblnResult = false;
			if (icdoFullAuditLog == null)
			{
				icdoFullAuditLog = new cdoFullAuditLog();
			}
			if (icdoFullAuditLog.SelectRow(new object[1] { aintAuditLogId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

        /// <summary>
        /// MPIPHP.busFullAuditLogGen.LoadFullAuditLogDetails():
        /// Loads Collection object iclbFullAuditLogDetail of type busFullAuditLogDetail.
        /// </summary>
		public virtual void LoadFullAuditLogDetails()
		{
			DataTable ldtbList = Select<cdoFullAuditLogDetail>(
				new string[1] { enmFullAuditLogDetail.audit_log_id.ToString() },
				new object[1] { icdoFullAuditLog.audit_log_id }, null, null);
			iclbFullAuditLogDetail = GetCollection<busFullAuditLogDetail>(ldtbList, "icdoFullAuditLogDetail");
		}

	}
}
