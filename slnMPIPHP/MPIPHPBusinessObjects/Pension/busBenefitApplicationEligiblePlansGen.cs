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

#endregion

namespace MPIPHP
{
    /// <summary>
    /// Class MPIPHP.busBenefitApplicationEligiblePlansGen:
    /// Inherited from busBase, used to create new business object for main table cdoBenefitApplicationEligiblePlans and its children table. 
    /// </summary>
	[Serializable]
	public class busBenefitApplicationEligiblePlansGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.busBenefitApplicationEligiblePlansGen
        /// </summary>
		public busBenefitApplicationEligiblePlansGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busBenefitApplicationEligiblePlansGen.
        /// </summary>
		public cdoBenefitApplicationEligiblePlans icdoBenefitApplicationEligiblePlans { get; set; }


        /// <summary>
        /// MPIPHP.busBenefitApplicationEligiblePlansGen.FindBenefitApplicationEligiblePlans():
        /// Finds a particular record from cdoBenefitApplicationEligiblePlans with its primary key. 
        /// </summary>
        /// <param name="aintBenefitApplicationEligibleId">A primary key value of type int of cdoBenefitApplicationEligiblePlans on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindBenefitApplicationEligiblePlans(int aintBenefitApplicationEligibleId)
		{
			bool lblnResult = false;
			if (icdoBenefitApplicationEligiblePlans == null)
			{
				icdoBenefitApplicationEligiblePlans = new cdoBenefitApplicationEligiblePlans();
			}
			if (icdoBenefitApplicationEligiblePlans.SelectRow(new object[1] { aintBenefitApplicationEligibleId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
