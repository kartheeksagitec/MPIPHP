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
    /// Class MPIPHP.busIapRecalculationCopyGen:
    /// Inherited from busBase, used to create new business object for main table cdoIapRecalculationCopy and its children table. 
    /// </summary>
	[Serializable]
	public class busIapRecalculationCopyGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.busIapRecalculationCopyGen
        /// </summary>
		public busIapRecalculationCopyGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busIapRecalculationCopyGen.
        /// </summary>
		public cdoIapRecalculationCopy icdoIapRecalculationCopy { get; set; }




        /// <summary>
        /// MPIPHP.busIapRecalculationCopyGen.FindIapRecalculationCopy():
        /// Finds a particular record from cdoIapRecalculationCopy with its primary key. 
        /// </summary>
        /// <param name="aintIapRecalculationCopyId">A primary key value of type int of cdoIapRecalculationCopy on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindIapRecalculationCopy(int aintIapRecalculationCopyId)
		{
			bool lblnResult = false;
			if (icdoIapRecalculationCopy == null)
			{
				icdoIapRecalculationCopy = new cdoIapRecalculationCopy();
			}
			if (icdoIapRecalculationCopy.SelectRow(new object[1] { aintIapRecalculationCopyId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
