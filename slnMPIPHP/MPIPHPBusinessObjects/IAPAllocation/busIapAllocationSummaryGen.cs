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
    /// Class MPIPHP.BusinessObjects.busIapAllocationSummaryGen:
    /// Inherited from busBase, used to create new business object for main table cdoIapAllocationSummary and its children table. 
    /// </summary>
	[Serializable]
	public class busIapAllocationSummaryGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.BusinessObjects.busIapAllocationSummaryGen
        /// </summary>
		public busIapAllocationSummaryGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busIapAllocationSummaryGen.
        /// </summary>
		public cdoIapAllocationSummary icdoIapAllocationSummary { get; set; }




        /// <summary>
        /// MPIPHP.busIapAllocationSummaryGen.FindIapAllocationSummary():
        /// Finds a particular record from cdoIapAllocationSummary with its primary key. 
        /// </summary>
        /// <param name="aintIapAllocationSummaryId">A primary key value of type int of cdoIapAllocationSummary on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindIapAllocationSummary(int aintIapAllocationSummaryId)
		{
			bool lblnResult = false;
			if (icdoIapAllocationSummary == null)
			{
				icdoIapAllocationSummary = new cdoIapAllocationSummary();
			}
			if (icdoIapAllocationSummary.SelectRow(new object[1] { aintIapAllocationSummaryId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
