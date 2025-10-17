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
    /// Class  MPIPHP.BusinessObjects.busQdroCalculationOptionsGen:
    /// Inherited from busBase, used to create new business object for main table cdoQdroCalculationOptions and its children table. 
    /// </summary>
	[Serializable]
	public class busQdroCalculationOptionsGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for  MPIPHP.BusinessObjects.busQdroCalculationOptionsGen
        /// </summary>
		public busQdroCalculationOptionsGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busQdroCalculationOptionsGen.
        /// </summary>
		public cdoQdroCalculationOptions icdoQdroCalculationOptions { get; set; }

        /// <summary>
        /// Gets or sets the non-collection object of type busPlanBenefitXr.
        /// </summary>
		public busPlanBenefitXr ibusPlanBenefitXr { get; set; }




        /// <summary>
        ///  MPIPHP.busQdroCalculationOptionsGen.FindQdroCalculationOptions():
        /// Finds a particular record from cdoQdroCalculationOptions with its primary key. 
        /// </summary>
        /// <param name="aintQdroCalculationOptionId">A primary key value of type int of cdoQdroCalculationOptions on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindQdroCalculationOptions(int aintQdroCalculationOptionId)
		{
			bool lblnResult = false;
			if (icdoQdroCalculationOptions == null)
			{
				icdoQdroCalculationOptions = new cdoQdroCalculationOptions();
			}
			if (icdoQdroCalculationOptions.SelectRow(new object[1] { aintQdroCalculationOptionId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

        /// <summary>
        ///  MPIPHP.busQdroCalculationOptionsGen.LoadPlanBenefitXr():
        /// Loads non-collection object ibusPlanBenefitXr of type busPlanBenefitXr.
        /// </summary>
		public virtual void LoadPlanBenefitXr()
		{
			if (ibusPlanBenefitXr == null)
			{
				ibusPlanBenefitXr = new busPlanBenefitXr();
			}
			ibusPlanBenefitXr.FindPlanBenefitXr(icdoQdroCalculationOptions.plan_benefit_id);
		}

	}
}
