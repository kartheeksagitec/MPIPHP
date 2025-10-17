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
    /// Class MPIPHP.BusinessObjects.busFedStateDeductionGen:
    /// Inherited from busBase, used to create new business object for main table cdoFedStateDeduction and its children table. 
    /// </summary>
	[Serializable]
	public class busFedStateDeductionGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.BusinessObjects.busFedStateDeductionGen
        /// </summary>
		public busFedStateDeductionGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busFedStateDeductionGen.
        /// </summary>
		public cdoFedStateDeduction icdoFedStateDeduction { get; set; }




        /// <summary>
        /// MPIPHP.busFedStateDeductionGen.FindFedStateDeduction():
        /// Finds a particular record from cdoFedStateDeduction with its primary key. 
        /// </summary>
        /// <param name="aintFedStateDeductionId">A primary key value of type int of cdoFedStateDeduction on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindFedStateDeduction(int aintFedStateDeductionId)
		{
			bool lblnResult = false;
			if (icdoFedStateDeduction == null)
			{
				icdoFedStateDeduction = new cdoFedStateDeduction();
			}
			if (icdoFedStateDeduction.SelectRow(new object[1] { aintFedStateDeductionId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
