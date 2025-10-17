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
    /// Class MPIPHP.BusinessObjects.busDisabilityBenefitHistoryGen:
    /// Inherited from busBase, used to create new business object for main table cdoDisabilityBenefitHistory and its children table. 
    /// </summary>
	[Serializable]
	public class busDisabilityBenefitHistoryGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.BusinessObjects.busDisabilityBenefitHistoryGen
        /// </summary>
		public busDisabilityBenefitHistoryGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busDisabilityBenefitHistoryGen.
        /// </summary>
		public cdoDisabilityBenefitHistory icdoDisabilityBenefitHistory { get; set; }




        /// <summary>
        /// MPIPHP.busDisabilityBenefitHistoryGen.FindDisabilityBenefitHistory():
        /// Finds a particular record from cdoDisabilityBenefitHistory with its primary key. 
        /// </summary>
        /// <param name="aintdisabilitybenefithistoryid">A primary key value of type int of cdoDisabilityBenefitHistory on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindDisabilityBenefitHistory(int aintdisabilitybenefithistoryid)
		{
			bool lblnResult = false;
			if (icdoDisabilityBenefitHistory == null)
			{
				icdoDisabilityBenefitHistory = new cdoDisabilityBenefitHistory();
			}
			if (icdoDisabilityBenefitHistory.SelectRow(new object[1] { aintdisabilitybenefithistoryid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
