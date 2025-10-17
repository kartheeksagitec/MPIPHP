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
    /// Class MPIPHP.BusinessObjects.busBenefitProvisionEligibilityGen:
    /// Inherited from busBase, used to create new business object for main table cdoBenefitProvisionEligibility and its children table. 
    /// </summary>
	[Serializable]
	public class busBenefitProvisionEligibilityGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.BusinessObjects.busBenefitProvisionEligibilityGen
        /// </summary>
		public busBenefitProvisionEligibilityGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busBenefitProvisionEligibilityGen.
        /// </summary>
		public cdoBenefitProvisionEligibility icdoBenefitProvisionEligibility { get; set; }




        /// <summary>
        /// MPIPHP.busBenefitProvisionEligibilityGen.FindBenefitProvisionEligibility():
        /// Finds a particular record from cdoBenefitProvisionEligibility with its primary key. 
        /// </summary>
        /// <param name="aintbenefitprovisioneligibilityid">A primary key value of type int of cdoBenefitProvisionEligibility on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindBenefitProvisionEligibility(int aintbenefitprovisioneligibilityid)
		{
			bool lblnResult = false;
			if (icdoBenefitProvisionEligibility == null)
			{
				icdoBenefitProvisionEligibility = new cdoBenefitProvisionEligibility();
			}
			if (icdoBenefitProvisionEligibility.SelectRow(new object[1] { aintbenefitprovisioneligibilityid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
