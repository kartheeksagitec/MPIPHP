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
    /// Class MPIPHP.busIapAnnunityAdjustmentMultiplierGen:
    /// Inherited from busBase, used to create new business object for main table cdoIapAnnunityAdjustmentMultiplier and its children table. 
    /// </summary>
	[Serializable]
	public class busIapAnnunityAdjustmentMultiplierGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.busIapAnnunityAdjustmentMultiplierGen
        /// </summary>
		public busIapAnnunityAdjustmentMultiplierGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busIapAnnunityAdjustmentMultiplierGen.
        /// </summary>
		public cdoIapAnnunityAdjustmentMultiplier icdoIapAnnunityAdjustmentMultiplier { get; set; }




        /// <summary>
        /// MPIPHP.busIapAnnunityAdjustmentMultiplierGen.FindIapAnnunityAdjustmentMultiplier():
        /// Finds a particular record from cdoIapAnnunityAdjustmentMultiplier with its primary key. 
        /// </summary>
        /// <param name="aintIapAnnunityAdjustmentMultiplierId">A primary key value of type int of cdoIapAnnunityAdjustmentMultiplier on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindIapAnnunityAdjustmentMultiplier()
		{
			bool lblnResult = false;
			if (icdoIapAnnunityAdjustmentMultiplier == null)
			{
				icdoIapAnnunityAdjustmentMultiplier = new cdoIapAnnunityAdjustmentMultiplier();
			}
			if (icdoIapAnnunityAdjustmentMultiplier.SelectRow(new object[0] {}))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
