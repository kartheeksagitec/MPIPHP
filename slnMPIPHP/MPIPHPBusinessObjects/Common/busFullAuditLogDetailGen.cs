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

#endregion

namespace MPIPHP.BusinessObjects
{
    /// <summary>
    /// Class MPIPHP.BusinessObjects.busFullAuditLogDetailGen:
    /// Inherited from busBase, used to create new business object for main table cdoFullAuditLogDetail and its children table. 
    /// </summary>
	[Serializable]
	public class busFullAuditLogDetailGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.BusinessObjects.busFullAuditLogDetailGen
        /// </summary>
		public busFullAuditLogDetailGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busFullAuditLogDetailGen.
        /// </summary>
		public cdoFullAuditLogDetail icdoFullAuditLogDetail { get; set; }




        /// <summary>
        /// MPIPHP.busFullAuditLogDetailGen.FindFullAuditLogDetail():
        /// Finds a particular record from cdoFullAuditLogDetail with its primary key. 
        /// </summary>
        /// <param name="aintAuditLogDetailId">A primary key value of type int of cdoFullAuditLogDetail on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindFullAuditLogDetail(int aintAuditLogDetailId)
		{
			bool lblnResult = false;
			if (icdoFullAuditLogDetail == null)
			{
				icdoFullAuditLogDetail = new cdoFullAuditLogDetail();
			}
			if (icdoFullAuditLogDetail.SelectRow(new object[1] { aintAuditLogDetailId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
