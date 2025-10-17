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
    /// Class MPIPHP.BusinessObjects.busDataExtractionBatchHourInfoGen:
    /// Inherited from busBase, used to create new business object for main table cdoDataExtractionBatchHourInfo and its children table. 
    /// </summary>
	[Serializable]
	public class busDataExtractionBatchHourInfoGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.BusinessObjects.busDataExtractionBatchHourInfoGen
        /// </summary>
		public busDataExtractionBatchHourInfoGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busDataExtractionBatchHourInfoGen.
        /// </summary>
		public cdoDataExtractionBatchHourInfo icdoDataExtractionBatchHourInfo { get; set; }




        /// <summary>
        /// MPIPHP.busDataExtractionBatchHourInfoGen.FindDataExtractionBatchHourInfo():
        /// Finds a particular record from cdoDataExtractionBatchHourInfo with its primary key. 
        /// </summary>
        /// <param name="aintdataextractionbatchhourinfoid">A primary key value of type int of cdoDataExtractionBatchHourInfo on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindDataExtractionBatchHourInfo(int aintdataextractionbatchhourinfoid)
		{
			bool lblnResult = false;
			if (icdoDataExtractionBatchHourInfo == null)
			{
				icdoDataExtractionBatchHourInfo = new cdoDataExtractionBatchHourInfo();
			}
			if (icdoDataExtractionBatchHourInfo.SelectRow(new object[1] { aintdataextractionbatchhourinfoid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
