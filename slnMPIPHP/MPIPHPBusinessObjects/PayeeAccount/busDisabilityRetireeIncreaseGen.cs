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
    /// Class MPIPHP.BusinessObjects.busDisabilityRetireeIncreaseGen:
    /// Inherited from busBase, used to create new business object for main table cdoDisabilityRetireeIncrease and its children table. 
    /// </summary>
	[Serializable]
	public class busDisabilityRetireeIncreaseGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.BusinessObjects.busDisabilityRetireeIncreaseGen
        /// </summary>
		public busDisabilityRetireeIncreaseGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busDisabilityRetireeIncreaseGen.
        /// </summary>
		public cdoDisabilityRetireeIncrease icdoDisabilityRetireeIncrease { get; set; }




        /// <summary>
        /// MPIPHP.busDisabilityRetireeIncreaseGen.FindDisabilityRetireeIncrease():
        /// Finds a particular record from cdoDisabilityRetireeIncrease with its primary key. 
        /// </summary>
        /// <param name="aintSgtDisabilityRetireeIncreaseId">A primary key value of type int of cdoDisabilityRetireeIncrease on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindDisabilityRetireeIncrease(int aintDisabilityRetireeIncreaseId)
		{
			bool lblnResult = false;
			if (icdoDisabilityRetireeIncrease == null)
			{
				icdoDisabilityRetireeIncrease = new cdoDisabilityRetireeIncrease();
			}
			if (icdoDisabilityRetireeIncrease.SelectRow(new object[1] { aintDisabilityRetireeIncreaseId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
