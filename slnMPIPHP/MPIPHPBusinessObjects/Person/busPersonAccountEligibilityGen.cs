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
    /// Class MPIPHP.BusinessObjects.busPersonAccountEligibilityGen:
    /// Inherited from busBase, used to create new business object for main table cdoPersonAccountEligibility and its children table. 
    /// </summary>
	[Serializable]
	public class busPersonAccountEligibilityGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.BusinessObjects.busPersonAccountEligibilityGen
        /// </summary>
		public busPersonAccountEligibilityGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busPersonAccountEligibilityGen.
        /// </summary>
		public cdoPersonAccountEligibility icdoPersonAccountEligibility { get; set; }




        /// <summary>
        /// MPIPHP.busPersonAccountEligibilityGen.FindPersonAccountEligibility():
        /// Finds a particular record from cdoPersonAccountEligibility with its primary key. 
        /// </summary>
        /// <param name="aintpersonaccounteligibilityid">A primary key value of type int of cdoPersonAccountEligibility on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindPersonAccountEligibility(int aintpersonaccounteligibilityid)
		{
			bool lblnResult = false;
			if (icdoPersonAccountEligibility == null)
			{
				icdoPersonAccountEligibility = new cdoPersonAccountEligibility();
			}
			if (icdoPersonAccountEligibility.SelectRow(new object[1] { aintpersonaccounteligibilityid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
