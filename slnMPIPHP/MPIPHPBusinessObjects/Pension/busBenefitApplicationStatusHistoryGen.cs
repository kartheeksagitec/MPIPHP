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
    /// Class MPIPHP.BusinessObjects.busBenefitApplicationStatusHistoryGen:
    /// Inherited from busBase, used to create new business object for main table cdoBenefitApplicationStatusHistory and its children table. 
    /// </summary>
	[Serializable]
	public class busBenefitApplicationStatusHistoryGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.BusinessObjects.busBenefitApplicationStatusHistoryGen
        /// </summary>
		public busBenefitApplicationStatusHistoryGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busBenefitApplicationStatusHistoryGen.
        /// </summary>
		public cdoBenefitApplicationStatusHistory icdoBenefitApplicationStatusHistory { get; set; }

        /// <summary>
        /// Gets or sets the non-collection object of type busBenefitApplication.
        /// </summary>
		public busBenefitApplication ibusBenefitApplication { get; set; }




        /// <summary>
        /// MPIPHP.busBenefitApplicationStatusHistoryGen.FindBenefitApplicationStatusHistory():
        /// Finds a particular record from cdoBenefitApplicationStatusHistory with its primary key. 
        /// </summary>
        /// <param name="aintBenefitApplicationStatusHistoryId">A primary key value of type int of cdoBenefitApplicationStatusHistory on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindBenefitApplicationStatusHistory(int aintBenefitApplicationStatusHistoryId)
		{
			bool lblnResult = false;
			if (icdoBenefitApplicationStatusHistory == null)
			{
				icdoBenefitApplicationStatusHistory = new cdoBenefitApplicationStatusHistory();
			}
			if (icdoBenefitApplicationStatusHistory.SelectRow(new object[1] { aintBenefitApplicationStatusHistoryId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

        /// <summary>
        /// MPIPHP.busBenefitApplicationStatusHistoryGen.LoadBenefitApplication():
        /// Loads non-collection object ibusBenefitApplication of type busBenefitApplication.
        /// </summary>
		public virtual void LoadBenefitApplication()
		{
			if (ibusBenefitApplication == null)
			{
				ibusBenefitApplication = new busBenefitApplication();
			}
			ibusBenefitApplication.FindBenefitApplication(icdoBenefitApplicationStatusHistory.benefit_application_id);
		}

	}
}
