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
    /// Class MPIPHP.BusinessObjects.busBenefitCalculationNonsuspendibleDetailGen:
    /// Inherited from busBase, used to create new business object for main table cdoBenefitCalculationNonsuspendibleDetail and its children table. 
    /// </summary>
	[Serializable]
	public class busBenefitCalculationNonsuspendibleDetailGen : busMPIPHPBase
	{
        /// <summary>
        /// Constructor for MPIPHP.BusinessObjects.busBenefitCalculationNonsuspendibleDetailGen
        /// </summary>
		public busBenefitCalculationNonsuspendibleDetailGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busBenefitCalculationNonsuspendibleDetailGen.
        /// </summary>
		public cdoBenefitCalculationNonsuspendibleDetail icdoBenefitCalculationNonsuspendibleDetail { get; set; }

        /// <summary>
        /// Gets or sets the non-collection object of type busBenefitCalculationYearlyDetail.
        /// </summary>
		public busBenefitCalculationYearlyDetail ibusBenefitCalculationYearlyDetail { get; set; }

        /// <summary>
        /// Gets or sets the non-collection object of type busBenefitCalculationDetail.
        /// </summary>
		public busBenefitCalculationDetail ibusBenefitCalculationDetail { get; set; }




        /// <summary>
        /// MPIPHP.busBenefitCalculationNonsuspendibleDetailGen.FindBenefitCalculationNonsuspendibleDetail():
        /// Finds a particular record from cdoBenefitCalculationNonsuspendibleDetail with its primary key. 
        /// </summary>
        /// <param name="aintBenefitCalculationNonsuspendibleDetailId">A primary key value of type int of cdoBenefitCalculationNonsuspendibleDetail on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindBenefitCalculationNonsuspendibleDetail(int aintBenefitCalculationNonsuspendibleDetailId)
		{
			bool lblnResult = false;
			if (icdoBenefitCalculationNonsuspendibleDetail == null)
			{
				icdoBenefitCalculationNonsuspendibleDetail = new cdoBenefitCalculationNonsuspendibleDetail();
			}
			if (icdoBenefitCalculationNonsuspendibleDetail.SelectRow(new object[1] { aintBenefitCalculationNonsuspendibleDetailId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

        /// <summary>
        /// MPIPHP.busBenefitCalculationNonsuspendibleDetailGen.LoadBenefitCalculationYearlyDetail():
        /// Loads non-collection object ibusBenefitCalculationYearlyDetail of type busBenefitCalculationYearlyDetail.
        /// </summary>
		public virtual void LoadBenefitCalculationYearlyDetail()
		{
			if (ibusBenefitCalculationYearlyDetail == null)
			{
				ibusBenefitCalculationYearlyDetail = new busBenefitCalculationYearlyDetail();
			}
			ibusBenefitCalculationYearlyDetail.FindBenefitCalculationYearlyDetail(icdoBenefitCalculationNonsuspendibleDetail.benefit_calculation_yearly_detail_id);
		}

        /// <summary>
        /// MPIPHP.busBenefitCalculationNonsuspendibleDetailGen.LoadBenefitCalculationDetail():
        /// Loads non-collection object ibusBenefitCalculationDetail of type busBenefitCalculationDetail.
        /// </summary>
		public virtual void LoadBenefitCalculationDetail()
		{
			if (ibusBenefitCalculationDetail == null)
			{
				ibusBenefitCalculationDetail = new busBenefitCalculationDetail();
			}
			ibusBenefitCalculationDetail.FindBenefitCalculationDetail(icdoBenefitCalculationNonsuspendibleDetail.benefit_calculation_detail_id);
		}

	}
}
