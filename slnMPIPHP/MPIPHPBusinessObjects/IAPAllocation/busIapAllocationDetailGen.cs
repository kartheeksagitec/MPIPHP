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
    /// Class MPIPHP.BusinessObjects.busIapAllocationDetailGen:
    /// Inherited from busBase, used to create new business object for main table cdoIapAllocationDetail and its children table. 
    /// </summary>
	[Serializable]
	public class busIapAllocationDetailGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.BusinessObjects.busIapAllocationDetailGen
        /// </summary>
		public busIapAllocationDetailGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busIapAllocationDetailGen.
        /// </summary>
		public cdoIapAllocationDetail icdoIapAllocationDetail { get; set; }




        /// <summary>
        /// MPIPHP.busIapAllocationDetailGen.FindIapAllocationDetail():
        /// Finds a particular record from cdoIapAllocationDetail with its primary key. 
        /// </summary>
        /// <param name="aintIapAllocationDetailId">A primary key value of type int of cdoIapAllocationDetail on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindIapAllocationDetail(int aintIapAllocationDetailId)
		{
			bool lblnResult = false;
			if (icdoIapAllocationDetail == null)
			{
				icdoIapAllocationDetail = new cdoIapAllocationDetail();
			}
			if (icdoIapAllocationDetail.SelectRow(new object[1] { aintIapAllocationDetailId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
