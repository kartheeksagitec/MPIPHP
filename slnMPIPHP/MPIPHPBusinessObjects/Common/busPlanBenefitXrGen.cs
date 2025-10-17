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
    /// Class MPIPHP.BusinessObjects.busPlanBenefitXrGen:
    /// Inherited from busBase, used to create new business object for main table cdoPlanBenefitXr and its children table. 
    /// </summary>
	[Serializable]
	public class busPlanBenefitXrGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.BusinessObjects.busPlanBenefitXrGen
        /// </summary>
		public busPlanBenefitXrGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busPlanBenefitXrGen.
        /// </summary>
		public cdoPlanBenefitXr icdoPlanBenefitXr { get; set; }




        /// <summary>
        /// MPIPHP.busPlanBenefitXrGen.FindPlanBenefitXr():
        /// Finds a particular record from cdoPlanBenefitXr with its primary key. 
        /// </summary>
        /// <param name="aintplanbenefitid">A primary key value of type int of cdoPlanBenefitXr on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindPlanBenefitXr(int aintplanbenefitid)
		{
			bool lblnResult = false;
			if (icdoPlanBenefitXr == null)
			{
				icdoPlanBenefitXr = new cdoPlanBenefitXr();
			}
			if (icdoPlanBenefitXr.SelectRow(new object[1] { aintplanbenefitid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
