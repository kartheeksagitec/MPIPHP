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
    /// Class MPIPHP.BusinessObjects.busYearEndDataExtractionHeaderGen:
    /// Inherited from busBase, used to create new business object for main table cdoYearEndDataExtractionHeader and its children table. 
    /// </summary>
	[Serializable]
	public class busYearEndDataExtractionHeaderGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.BusinessObjects.busYearEndDataExtractionHeaderGen
        /// </summary>
		public busYearEndDataExtractionHeaderGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busYearEndDataExtractionHeaderGen.
        /// </summary>
		public cdoYearEndDataExtractionHeader icdoYearEndDataExtractionHeader { get; set; }




        /// <summary>
        /// MPIPHP.busYearEndDataExtractionHeaderGen.FindYearEndDataExtractionHeader():
        /// Finds a particular record from cdoYearEndDataExtractionHeader with its primary key. 
        /// </summary>
        /// <param name="aintYearEndDataExtractionHeaderId">A primary key value of type int of cdoYearEndDataExtractionHeader on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindYearEndDataExtractionHeader(int aintYearEndDataExtractionHeaderId)
		{
			bool lblnResult = false;
			if (icdoYearEndDataExtractionHeader == null)
			{
				icdoYearEndDataExtractionHeader = new cdoYearEndDataExtractionHeader();
			}
			if (icdoYearEndDataExtractionHeader.SelectRow(new object[1] { aintYearEndDataExtractionHeaderId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
