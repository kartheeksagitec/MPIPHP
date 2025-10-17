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
    /// Class MPIPHP.BusinessObjects.busPlanGen:
    /// Inherited from busBase, used to create new business object for main table cdoPlan and its children table. 
    /// </summary>
	[Serializable]
	public class busPlanGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.BusinessObjects.busPlanGen
        /// </summary>
		public busPlanGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busPlanGen.
        /// </summary>
		public cdoPlan icdoPlan { get; set; }




        /// <summary>
        /// MPIPHP.busPlanGen.FindPlan():
        /// Finds a particular record from cdoPlan with its primary key. 
        /// </summary>
        /// <param name="aintplanid">A primary key value of type int of cdoPlan on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindPlan(int aintplanid)
		{
			bool lblnResult = false;
			if (icdoPlan == null)
			{
				icdoPlan = new cdoPlan();
			}
			if (icdoPlan.SelectRow(new object[1] { aintplanid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
