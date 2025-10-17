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

namespace MPIPHP.BusinessObjects
{
    /// <summary>
    /// Class MPIPHP.busPensionVerificationHistoryGen:
    /// Inherited from busBase, used to create new business object for main table cdoPensionVerificationHistory and its children table. 
    /// </summary>
	[Serializable]
	public class busPensionVerificationHistoryGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.busPensionVerificationHistoryGen
        /// </summary>
		public busPensionVerificationHistoryGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busPensionVerificationHistoryGen.
        /// </summary>
		public cdoPensionVerificationHistory icdoPensionVerificationHistory { get; set; }




        /// <summary>
        /// MPIPHP.busPensionVerificationHistoryGen.FindPensionVerificationHistory():
        /// Finds a particular record from cdoPensionVerificationHistory with its primary key. 
        /// </summary>
        /// <param name="aintPensionVerificationHistoryId">A primary key value of type int of cdoPensionVerificationHistory on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindPensionVerificationHistory(int aintPensionVerificationHistoryId)
		{
			bool lblnResult = false;
			if (icdoPensionVerificationHistory == null)
			{
				icdoPensionVerificationHistory = new cdoPensionVerificationHistory();
			}
			if (icdoPensionVerificationHistory.SelectRow(new object[1] { aintPensionVerificationHistoryId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
