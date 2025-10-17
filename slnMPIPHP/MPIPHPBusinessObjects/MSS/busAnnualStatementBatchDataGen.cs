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
    /// Class MPIPHP.BusinessObjects.busAnnualStatementBatchDataGen:
    /// Inherited from busBase, used to create new business object for main table cdoAnnualStatementBatchData and its children table. 
    /// </summary>
	[Serializable]
	public class busAnnualStatementBatchDataGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.BusinessObjects.busAnnualStatementBatchDataGen
        /// </summary>
		public busAnnualStatementBatchDataGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busAnnualStatementBatchDataGen.
        /// </summary>
		public cdoAnnualStatementBatchData icdoAnnualStatementBatchData { get; set; }




        /// <summary>
        /// MPIPHP.busAnnualStatementBatchDataGen.FindAnnualStatementBatchData():
        /// Finds a particular record from cdoAnnualStatementBatchData with its primary key. 
        /// </summary>
        /// <param name="aintannualstatementbatchdataid">A primary key value of type int of cdoAnnualStatementBatchData on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindAnnualStatementBatchData(int aintannualstatementbatchdataid)
		{
			bool lblnResult = false;
			if (icdoAnnualStatementBatchData == null)
			{
				icdoAnnualStatementBatchData = new cdoAnnualStatementBatchData();
			}
			if (icdoAnnualStatementBatchData.SelectRow(new object[1] { aintannualstatementbatchdataid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
