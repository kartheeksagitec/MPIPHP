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
    /// Class MPIPHP.BusinessObjects.busUserSecurityHistoryGen:
    /// Inherited from busBase, used to create new business object for main table cdoUserSecurityHistory and its children table. 
    /// </summary>
	[Serializable]
	public class busUserSecurityHistoryGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.BusinessObjects.busUserSecurityHistoryGen
        /// </summary>
		public busUserSecurityHistoryGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busUserSecurityHistoryGen.
        /// </summary>
		public cdoUserSecurityHistory icdoUserSecurityHistory { get; set; }

        /// <summary>
        /// MPIPHP.busUserSecurityHistoryGen.FindUserSecurityHistory():
        /// Finds a particular record from cdoUserSecurityHistory with its primary key. 
        /// </summary>
        /// <param name="aintsgsusersecurityhistoryid">A primary key value of type int of cdoUserSecurityHistory on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindUserSecurityHistory(int aintsgsusersecurityhistoryid)
		{
			bool lblnResult = false;
			if (icdoUserSecurityHistory == null)
			{
				icdoUserSecurityHistory = new cdoUserSecurityHistory();
			}
			if (icdoUserSecurityHistory.SelectRow(new object[1] { aintsgsusersecurityhistoryid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
