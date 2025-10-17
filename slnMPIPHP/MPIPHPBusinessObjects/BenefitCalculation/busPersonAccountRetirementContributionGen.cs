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
    /// Class MPIPHP.BusinessObjects.busPersonAccountRetirementContributionGen:
    /// Inherited from busBase, used to create new business object for main table cdoPersonAccountRetirementContribution and its children table. 
    /// </summary>
	[Serializable]
	public class busPersonAccountRetirementContributionGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.BusinessObjects.busPersonAccountRetirementContributionGen
        /// </summary>
		public busPersonAccountRetirementContributionGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busPersonAccountRetirementContributionGen.
        /// </summary>
		public cdoPersonAccountRetirementContribution icdoPersonAccountRetirementContribution { get; set; }




        /// <summary>
        /// MPIPHP.busPersonAccountRetirementContributionGen.FindPersonAccountRetirementContribution():
        /// Finds a particular record from cdoPersonAccountRetirementContribution with its primary key. 
        /// </summary>
        /// <param name="aintpersonaccountretirementcontributionid">A primary key value of type int of cdoPersonAccountRetirementContribution on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindPersonAccountRetirementContribution(int aintpersonaccountretirementcontributionid)
		{
			bool lblnResult = false;
			if (icdoPersonAccountRetirementContribution == null)
			{
				icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution();
			}
			if (icdoPersonAccountRetirementContribution.SelectRow(new object[1] { aintpersonaccountretirementcontributionid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
