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
    /// Class MPIPHP.BusinessObjects.busBenefitMonthwiseAdjustmentDetailGen:
    /// Inherited from busBase, used to create new business object for main table cdoBenefitMonthwiseAdjustmentDetail and its children table. 
    /// </summary>
	[Serializable]
	public class busBenefitMonthwiseAdjustmentDetailGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.BusinessObjects.busBenefitMonthwiseAdjustmentDetailGen
        /// </summary>
		public busBenefitMonthwiseAdjustmentDetailGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busBenefitMonthwiseAdjustmentDetailGen.
        /// </summary>
		public cdoBenefitMonthwiseAdjustmentDetail icdoBenefitMonthwiseAdjustmentDetail { get; set; }




        /// <summary>
        /// MPIPHP.busBenefitMonthwiseAdjustmentDetailGen.FindBenefitMonthwiseAdjustmentDetail():
        /// Finds a particular record from cdoBenefitMonthwiseAdjustmentDetail with its primary key. 
        /// </summary>
        /// <param name="aintBenefitMonthwiseAdjustmentDetailId">A primary key value of type int of cdoBenefitMonthwiseAdjustmentDetail on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindBenefitMonthwiseAdjustmentDetail(int aintBenefitMonthwiseAdjustmentDetailId)
		{
			bool lblnResult = false;
			if (icdoBenefitMonthwiseAdjustmentDetail == null)
			{
				icdoBenefitMonthwiseAdjustmentDetail = new cdoBenefitMonthwiseAdjustmentDetail();
			}
			if (icdoBenefitMonthwiseAdjustmentDetail.SelectRow(new object[1] { aintBenefitMonthwiseAdjustmentDetailId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
