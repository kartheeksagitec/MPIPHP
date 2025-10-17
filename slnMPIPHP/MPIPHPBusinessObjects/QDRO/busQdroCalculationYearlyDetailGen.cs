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
    /// Class MPIPHP.BusinessObjects.busQdroCalculationYearlyDetailGen:
    /// Inherited from busBase, used to create new business object for main table cdoQdroCalculationYearlyDetail and its children table. 
    /// </summary>
	[Serializable]
	public class busQdroCalculationYearlyDetailGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.BusinessObjects.busQdroCalculationYearlyDetailGen
        /// </summary>
		public busQdroCalculationYearlyDetailGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busQdroCalculationYearlyDetailGen.
        /// </summary>
		public cdoQdroCalculationYearlyDetail icdoQdroCalculationYearlyDetail { get; set; }




        /// <summary>
        /// MPIPHP.busQdroCalculationYearlyDetailGen.FindQdroCalculationYearlyDetail():
        /// Finds a particular record from cdoQdroCalculationYearlyDetail with its primary key. 
        /// </summary>
        /// <param name="aintQdroCalculationYearlyDetailId">A primary key value of type int of cdoQdroCalculationYearlyDetail on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindQdroCalculationYearlyDetail(int aintQdroCalculationYearlyDetailId)
		{
			bool lblnResult = false;
			if (icdoQdroCalculationYearlyDetail == null)
			{
				icdoQdroCalculationYearlyDetail = new cdoQdroCalculationYearlyDetail();
			}
			if (icdoQdroCalculationYearlyDetail.SelectRow(new object[1] { aintQdroCalculationYearlyDetailId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
