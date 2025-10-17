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
    /// Class MPIPHP.BusinessObjects.busQdroIapAllocationDetailGen:
    /// Inherited from busBase, used to create new business object for main table cdoQdroIapAllocationDetail and its children table. 
    /// </summary>
	[Serializable]
	public class busQdroIapAllocationDetailGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.BusinessObjects.busQdroIapAllocationDetailGen
        /// </summary>
		public busQdroIapAllocationDetailGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busQdroIapAllocationDetailGen.
        /// </summary>
		public cdoQdroIapAllocationDetail icdoQdroIapAllocationDetail { get; set; }




        /// <summary>
        /// MPIPHP.busQdroIapAllocationDetailGen.FindQdroIapAllocationDetail():
        /// Finds a particular record from cdoQdroIapAllocationDetail with its primary key. 
        /// </summary>
        /// <param name="aintQdroIapAllocationDetailId">A primary key value of type int of cdoQdroIapAllocationDetail on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindQdroIapAllocationDetail(int aintQdroIapAllocationDetailId)
		{
			bool lblnResult = false;
			if (icdoQdroIapAllocationDetail == null)
			{
				icdoQdroIapAllocationDetail = new cdoQdroIapAllocationDetail();
			}
			if (icdoQdroIapAllocationDetail.SelectRow(new object[1] { aintQdroIapAllocationDetailId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
