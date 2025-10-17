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
    /// Class MPIPHP.BusinessObjects.busDataExtractionBatchInfoGen:
    /// Inherited from busBase, used to create new business object for main table cdoDataExtractionBatchInfo and its children table. 
    /// </summary>
	[Serializable]
	public class busDataExtractionBatchInfoGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.BusinessObjects.busDataExtractionBatchInfoGen
        /// </summary>
		public busDataExtractionBatchInfoGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busDataExtractionBatchInfoGen.
        /// </summary>
		public cdoDataExtractionBatchInfo icdoDataExtractionBatchInfo { get; set; }




        /// <summary>
        /// MPIPHP.busDataExtractionBatchInfoGen.FindDataExtractionBatchInfo():
        /// Finds a particular record from cdoDataExtractionBatchInfo with its primary key. 
        /// </summary>
        /// <param name="aintdataextractionbatchinfoid">A primary key value of type int of cdoDataExtractionBatchInfo on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindDataExtractionBatchInfo(int aintdataextractionbatchinfoid)
		{
			bool lblnResult = false;
			if (icdoDataExtractionBatchInfo == null)
			{
				icdoDataExtractionBatchInfo = new cdoDataExtractionBatchInfo();
			}
			if (icdoDataExtractionBatchInfo.SelectRow(new object[1] { aintdataextractionbatchinfoid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
