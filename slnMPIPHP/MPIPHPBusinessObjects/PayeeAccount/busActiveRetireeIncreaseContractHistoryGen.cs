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
    /// Class MPIPHP.BusinessObjects.busActiveRetireeIncreaseContractHistoryGen:
    /// Inherited from busBase, used to create new business object for main table cdoActiveRetireeIncreaseContractHistory and its children table. 
    /// </summary>
	[Serializable]
	public class busActiveRetireeIncreaseContractHistoryGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.BusinessObjects.busActiveRetireeIncreaseContractHistoryGen
        /// </summary>
		public busActiveRetireeIncreaseContractHistoryGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busActiveRetireeIncreaseContractHistoryGen.
        /// </summary>
		public cdoActiveRetireeIncreaseContractHistory icdoActiveRetireeIncreaseContractHistory { get; set; }




        /// <summary>
        /// MPIPHP.busActiveRetireeIncreaseContractHistoryGen.FindActiveRetireeIncreaseContractHistory():
        /// Finds a particular record from cdoActiveRetireeIncreaseContractHistory with its primary key. 
        /// </summary>
        /// <param name="aintActiveRetireeIncreaseContractHistoryId">A primary key value of type int of cdoActiveRetireeIncreaseContractHistory on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindActiveRetireeIncreaseContractHistory(int aintActiveRetireeIncreaseContractHistoryId)
		{
			bool lblnResult = false;
			if (icdoActiveRetireeIncreaseContractHistory == null)
			{
				icdoActiveRetireeIncreaseContractHistory = new cdoActiveRetireeIncreaseContractHistory();
			}
			if (icdoActiveRetireeIncreaseContractHistory.SelectRow(new object[1] { aintActiveRetireeIncreaseContractHistoryId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
