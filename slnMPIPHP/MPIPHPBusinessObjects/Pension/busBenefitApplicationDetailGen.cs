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
    /// Class  MPIPHP.BusinessObjects.busBenefitApplicationDetailGen:
    /// Inherited from busBase, used to create new business object for main table cdoBenefitApplicationDetail and its children table. 
    /// </summary>
	[Serializable]
	public class busBenefitApplicationDetailGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for  MPIPHP.BusinessObjects.busBenefitApplicationDetailGen
        /// </summary>
		public busBenefitApplicationDetailGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busBenefitApplicationDetailGen.
        /// </summary>
		public cdoBenefitApplicationDetail icdoBenefitApplicationDetail { get; set; }

        /// <summary>
        /// Gets or sets the non-collection object of type busPlanBenefitXr.
        /// </summary>
		public busPlanBenefitXr ibusPlanBenefitXr { get; set; }

        /// <summary>
        /// Gets or sets the non-collection object of type busBenefitApplication.
        /// </summary>
		public busBenefitApplication ibusBenefitApplication { get; set; }




        /// <summary>
        ///  MPIPHP.busBenefitApplicationDetailGen.FindBenefitApplicationDetail():
        /// Finds a particular record from cdoBenefitApplicationDetail with its primary key. 
        /// </summary>
        /// <param name="aintBenefitApplicationDetailId">A primary key value of type int of cdoBenefitApplicationDetail on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindBenefitApplicationDetail(int aintBenefitApplicationDetailId)
		{
			bool lblnResult = false;
			if (icdoBenefitApplicationDetail == null)
			{
				icdoBenefitApplicationDetail = new cdoBenefitApplicationDetail();
			}
			if (icdoBenefitApplicationDetail.SelectRow(new object[1] { aintBenefitApplicationDetailId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

        /// <summary>
        ///  MPIPHP.busBenefitApplicationDetailGen.LoadPlanBenefitXr():
        /// Loads non-collection object ibusPlanBenefitXr of type busPlanBenefitXr.
        /// </summary>
		public virtual void LoadPlanBenefitXr()
		{
			if (ibusPlanBenefitXr == null)
			{
				ibusPlanBenefitXr = new busPlanBenefitXr();
			}
			ibusPlanBenefitXr.FindPlanBenefitXr(icdoBenefitApplicationDetail.plan_benefit_id);
		}

        /// <summary>
        ///  MPIPHP.busBenefitApplicationDetailGen.LoadBenefitApplication():
        /// Loads non-collection object ibusBenefitApplication of type busBenefitApplication.
        /// </summary>
		public virtual void LoadBenefitApplication()
		{
			if (ibusBenefitApplication == null)
			{
				ibusBenefitApplication = new busBenefitApplication();
			}
			ibusBenefitApplication.FindBenefitApplication(icdoBenefitApplicationDetail.benefit_application_id);
		}

	}
}
