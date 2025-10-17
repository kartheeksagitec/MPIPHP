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

#endregion

namespace  MPIPHP.BusinessObjects
{
    /// <summary>
    /// Class  MPIPHP.BusinessObjects.busBenefitCalculationOptionsGen:
    /// Inherited from busBase, used to create new business object for main table cdoBenefitCalculationOptions and its children table. 
    /// </summary>
	[Serializable]
	public class busBenefitCalculationOptionsGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for  MPIPHP.BusinessObjects.busBenefitCalculationOptionsGen
        /// </summary>
		public busBenefitCalculationOptionsGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busBenefitCalculationOptionsGen.
        /// </summary>
		public cdoBenefitCalculationOptions icdoBenefitCalculationOptions { get; set; }

        /// <summary>
        /// Gets or sets the non-collection object of type busBenefitCalculationDetail.
        /// </summary>
		public busBenefitCalculationDetail ibusBenefitCalculationDetail { get; set; }

        /// <summary>
        /// Gets or sets the non-collection object of type busPlanBenefitXr.
        /// </summary>
		public busPlanBenefitXr ibusPlanBenefitXr { get; set; }




        /// <summary>
        ///  MPIPHP.busBenefitCalculationOptionsGen.FindBenefitCalculationOptions():
        /// Finds a particular record from cdoBenefitCalculationOptions with its primary key. 
        /// </summary>
        /// <param name="aintbenefitcalculationoptionid">A primary key value of type int of cdoBenefitCalculationOptions on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindBenefitCalculationOptions(int aintbenefitcalculationoptionid)
		{
			bool lblnResult = false;
			if (icdoBenefitCalculationOptions == null)
			{
				icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions();
			}
			if (icdoBenefitCalculationOptions.SelectRow(new object[1] { aintbenefitcalculationoptionid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

        /// <summary>
        ///  MPIPHP.busBenefitCalculationOptionsGen.LoadBenefitCalculationDetail():
        /// Loads non-collection object ibusBenefitCalculationDetail of type busBenefitCalculationDetail.
        /// </summary>
		public virtual void LoadBenefitCalculationDetail()
		{
			if (ibusBenefitCalculationDetail == null)
			{
				ibusBenefitCalculationDetail = new busBenefitCalculationDetail();
			}
			ibusBenefitCalculationDetail.FindBenefitCalculationDetail(icdoBenefitCalculationOptions.benefit_calculation_detail_id);
		}

        /// <summary>
        ///  MPIPHP.busBenefitCalculationOptionsGen.LoadPlanBenefitXr():
        /// Loads non-collection object ibusPlanBenefitXr of type busPlanBenefitXr.
        /// </summary>
		public virtual void LoadPlanBenefitXr()
		{
			if (ibusPlanBenefitXr == null)
			{
				ibusPlanBenefitXr = new busPlanBenefitXr();
			}
			ibusPlanBenefitXr.FindPlanBenefitXr(icdoBenefitCalculationOptions.plan_benefit_id);
		}

	}
}
