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
using MPIPHP.BusinessObjects;
using MPIPHP.DataObjects;

#endregion

namespace MPIPHP.BusinessObjects
{
    /// <summary>
    /// Class MPIPHP.busParticipantInformationDataExtractionGen:
    /// Inherited from busBase, used to create new business object for main table cdoParticipantInformationDataExtraction and its children table. 
    /// </summary>
	[Serializable]
	public class busParticipantInformationDataExtractionGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.busParticipantInformationDataExtractionGen
        /// </summary>
		public busParticipantInformationDataExtractionGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busParticipantInformationDataExtractionGen.
        /// </summary>
		public cdoParticipantInformationDataExtraction icdoParticipantInformationDataExtraction { get; set; }




        /// <summary>
        /// MPIPHP.busParticipantInformationDataExtractionGen.FindParticipantInformationDataExtraction():
        /// Finds a particular record from cdoParticipantInformationDataExtraction with its primary key. 
        /// </summary>
        /// <param name="aintParticipantInformationDataExtractionId">A primary key value of type int of cdoParticipantInformationDataExtraction on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindParticipantInformationDataExtraction(int aintParticipantInformationDataExtractionId)
		{
			bool lblnResult = false;
			if (icdoParticipantInformationDataExtraction == null)
			{
				icdoParticipantInformationDataExtraction = new cdoParticipantInformationDataExtraction();
			}
			if (icdoParticipantInformationDataExtraction.SelectRow(new object[1] { aintParticipantInformationDataExtractionId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
