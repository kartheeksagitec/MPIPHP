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
using MPIPHP.BusinessObjects;
using MPIPHP.DataObjects;

#endregion

namespace MPIPHP
{
    /// <summary>
    /// Class MPIPHP.busVipStatusHistoryGen:
    /// Inherited from busBase, used to create new business object for main table cdoVipStatusHistory and its children table. 
    /// </summary>
	[Serializable]
	public class busVipStatusHistoryGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.busVipStatusHistoryGen
        /// </summary>
		public busVipStatusHistoryGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busVipStatusHistoryGen.
        /// </summary>
		public cdoVipStatusHistory icdoVipStatusHistory { get; set; }




        /// <summary>
        /// MPIPHP.busVipStatusHistoryGen.FindVipStatusHistory():
        /// Finds a particular record from cdoVipStatusHistory with its primary key. 
        /// </summary>
        /// <param name="aintVipStatusHistoryId">A primary key value of type int of cdoVipStatusHistory on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindVipStatusHistory(int aintVipStatusHistoryId)
		{
			bool lblnResult = false;
			if (icdoVipStatusHistory == null)
			{
				icdoVipStatusHistory = new cdoVipStatusHistory();
			}
			if (icdoVipStatusHistory.SelectRow(new object[1] { aintVipStatusHistoryId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
