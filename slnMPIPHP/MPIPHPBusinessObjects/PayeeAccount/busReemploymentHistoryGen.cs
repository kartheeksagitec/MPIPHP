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
    /// Class MPIPHP.BusinessObjects.busReemploymentHistoryGen:
    /// Inherited from busBase, used to create new business object for main table cdoReemploymentHistory and its children table. 
    /// </summary>
	[Serializable]
	public class busReemploymentHistoryGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.BusinessObjects.busReemploymentHistoryGen
        /// </summary>
		public busReemploymentHistoryGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busReemploymentHistoryGen.
        /// </summary>
		public cdoReemploymentHistory icdoReemploymentHistory { get; set; }




        /// <summary>
        /// MPIPHP.busReemploymentHistoryGen.FindReemploymentHistory():
        /// Finds a particular record from cdoReemploymentHistory with its primary key. 
        /// </summary>
        /// <param name="aintReemploymentHistoryId">A primary key value of type int of cdoReemploymentHistory on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindReemploymentHistory(int aintReemploymentHistoryId)
		{
			bool lblnResult = false;
			if (icdoReemploymentHistory == null)
			{
				icdoReemploymentHistory = new cdoReemploymentHistory();
			}
			if (icdoReemploymentHistory.SelectRow(new object[1] { aintReemploymentHistoryId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
