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
    /// Class MPIPHP.busBenefitInterestRateGen:
    /// Inherited from busBase, used to create new business object for main table cdoBenefitInterestRate and its children table. 
    /// </summary>
	[Serializable]
	public class busBenefitInterestRateGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.busBenefitInterestRateGen
        /// </summary>
		public busBenefitInterestRateGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busBenefitInterestRateGen.
        /// </summary>
		public cdoBenefitInterestRate icdoBenefitInterestRate { get; set; }




        /// <summary>
        /// MPIPHP.busBenefitInterestRateGen.FindBenefitInterestRate():
        /// Finds a particular record from cdoBenefitInterestRate with its primary key. 
        /// </summary>
        /// <param name="aintBenefitInterestRateId">A primary key value of type int of cdoBenefitInterestRate on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindBenefitInterestRate(int aintBenefitInterestRateId)
		{
			bool lblnResult = false;
			if (icdoBenefitInterestRate == null)
			{
				icdoBenefitInterestRate = new cdoBenefitInterestRate();
			}
			if (icdoBenefitInterestRate.SelectRow(new object[1] { aintBenefitInterestRateId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
