#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using  MPIPHP.CustomDataObjects;
using MPIPHP.DataObjects;

#endregion

namespace  MPIPHP.BusinessObjects
{
    /// <summary>
    /// Class  MPIPHP.BusinessObjects.busBenefitCalculationYearlyDetailGen:
    /// Inherited from busBase, used to create new business object for main table cdoBenefitCalculationYearlyDetail and its children table. 
    /// </summary>
	[Serializable]
	public class busBenefitCalculationYearlyDetailGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for  MPIPHP.BusinessObjects.busBenefitCalculationYearlyDetailGen
        /// </summary>
		public busBenefitCalculationYearlyDetailGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busBenefitCalculationYearlyDetailGen.
        /// </summary>
		public cdoBenefitCalculationYearlyDetail icdoBenefitCalculationYearlyDetail { get; set; }

        /// <summary>
        /// Gets or sets the non-collection object of type busBenefitCalculationDetail.
        /// </summary>
		public busBenefitCalculationDetail ibusBenefitCalculationDetail { get; set; }


        /// <summary>
        /// Gets or sets the collection object of type busBenefitCalculationNonsuspendibleDetail. 
        /// </summary>
		public Collection<busBenefitCalculationNonsuspendibleDetail> iclbBenefitCalculationNonsuspendibleDetail { get; set; }



        /// <summary>
        ///  MPIPHP.busBenefitCalculationYearlyDetailGen.FindBenefitCalculationYearlyDetail():
        /// Finds a particular record from cdoBenefitCalculationYearlyDetail with its primary key. 
        /// </summary>
        /// <param name="aintBenefitCalculationYearlyDetailId">A primary key value of type int of cdoBenefitCalculationYearlyDetail on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindBenefitCalculationYearlyDetail(int aintBenefitCalculationYearlyDetailId)
		{
			bool lblnResult = false;
			if (icdoBenefitCalculationYearlyDetail == null)
			{
				icdoBenefitCalculationYearlyDetail = new cdoBenefitCalculationYearlyDetail();
			}
			if (icdoBenefitCalculationYearlyDetail.SelectRow(new object[1] { aintBenefitCalculationYearlyDetailId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

        /// <summary>
        ///  MPIPHP.busBenefitCalculationYearlyDetailGen.LoadBenefitCalculationDetail():
        /// Loads non-collection object ibusBenefitCalculationDetail of type busBenefitCalculationDetail.
        /// </summary>
		public virtual void LoadBenefitCalculationDetail()
		{
			if (ibusBenefitCalculationDetail == null)
			{
				ibusBenefitCalculationDetail = new busBenefitCalculationDetail();
			}
			ibusBenefitCalculationDetail.FindBenefitCalculationDetail(icdoBenefitCalculationYearlyDetail.benefit_calculation_detail_id);
		}

        /// <summary>
        ///  MPIPHP.busBenefitCalculationYearlyDetailGen.LoadBenefitCalculationNonsuspendibleDetails():
        /// Loads Collection object iclbBenefitCalculationNonsuspendibleDetail of type busBenefitCalculationNonsuspendibleDetail.
        /// </summary>
		public virtual void LoadBenefitCalculationNonsuspendibleDetails()
		{
			DataTable ldtbList = Select<cdoBenefitCalculationNonsuspendibleDetail>(
				new string[1] { enmBenefitCalculationNonsuspendibleDetail.benefit_calculation_yearly_detail_id.ToString() },
				new object[1] { icdoBenefitCalculationYearlyDetail.benefit_calculation_yearly_detail_id }, null, enmBenefitCalculationNonsuspendibleDetail.benefit_calculation_yearly_detail_id.ToString());

			this.iclbBenefitCalculationNonsuspendibleDetail = GetCollection<busBenefitCalculationNonsuspendibleDetail>(ldtbList, "icdoBenefitCalculationNonsuspendibleDetail");
            this.iclbBenefitCalculationNonsuspendibleDetail.ForEach(item => item.icdoBenefitCalculationNonsuspendibleDetail.PopulateDescriptions());
		}

	}
}
