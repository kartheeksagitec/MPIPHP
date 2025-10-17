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
    /// Class MPIPHP.BusinessObjects.busIapAllocationFactorGen:
    /// Inherited from busBase, used to create new business object for main table cdoIapAllocationFactor and its children table. 
    /// </summary>
	[Serializable]
	public class busIapAllocationFactorGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.BusinessObjects.busIapAllocationFactorGen
        /// </summary>
		public busIapAllocationFactorGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busIapAllocationFactorGen.
        /// </summary>
		public cdoIapAllocationFactor icdoIapAllocationFactor { get; set; }




        /// <summary>
        /// MPIPHP.busIapAllocationFactorGen.FindIapAllocationFactor():
        /// Finds a particular record from cdoIapAllocationFactor with its primary key. 
        /// </summary>
        /// <param name="aintIapAllocationFactorId">A primary key value of type int of cdoIapAllocationFactor on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindIapAllocationFactor(int aintIapAllocationFactorId)
		{
			bool lblnResult = false;
			if (icdoIapAllocationFactor == null)
			{
				icdoIapAllocationFactor = new cdoIapAllocationFactor();
			}
			if (icdoIapAllocationFactor.SelectRow(new object[1] { aintIapAllocationFactorId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
