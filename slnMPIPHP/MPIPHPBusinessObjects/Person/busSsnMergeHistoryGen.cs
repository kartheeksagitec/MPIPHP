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
    /// Class MPIPHP.BusinessObjects.busSsnMergeHistoryGen:
    /// Inherited from busBase, used to create new business object for main table cdoSsnMergeHistory and its children table. 
    /// </summary>
	[Serializable]
	public class busSsnMergeHistoryGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.BusinessObjects.busSsnMergeHistoryGen
        /// </summary>
		public busSsnMergeHistoryGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busSsnMergeHistoryGen.
        /// </summary>
		public cdoSsnMergeHistory icdoSsnMergeHistory { get; set; }




        /// <summary>
        /// MPIPHP.busSsnMergeHistoryGen.FindSsnMergeHistory():
        /// Finds a particular record from cdoSsnMergeHistory with its primary key. 
        /// </summary>
        /// <param name="aintSsnMergeHistoryId">A primary key value of type int of cdoSsnMergeHistory on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindSsnMergeHistory(int aintSsnMergeHistoryId)
		{
			bool lblnResult = false;
			if (icdoSsnMergeHistory == null)
			{
				icdoSsnMergeHistory = new cdoSsnMergeHistory();
			}
			if (icdoSsnMergeHistory.SelectRow(new object[1] { aintSsnMergeHistoryId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
