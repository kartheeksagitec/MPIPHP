#region Using directives

using System;
using System.Data;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.DBUtility;
using MPIPHP.CustomDataObjects;

#endregion

namespace MPIPHP.BusinessObjects
{
	/// <summary>
	/// Class MPIPHP.BusinessObjects.busPensionActuaryLookupGen:
	/// Inherited from busMainBase,It is used to create new look up business object. 
	/// </summary>

	[Serializable]
	public class busPensionActuaryLookupGen : busMainBase
	{
		/// <summary>
		/// Gets or sets the collection object of type busDataExtractionBatchInfo. 
		/// </summary>
		public Collection<busDataExtractionBatchInfo> iclbDataExtractionBatchInfo { get; set; }


		/// <summary>
		/// MPIPHP.BusinessObjects.busPensionActuaryLookupGen.LoadDataExtractionBatchInfos(DataTable):
		/// Loads Collection object iclbDataExtractionBatchInfo of type busDataExtractionBatchInfo.
		/// </summary>
		/// <param name="adtbSearchResult">DataTable that holds search result to fill the collection object busPensionActuaryLookupGen.iclbDataExtractionBatchInfo</param>
		public virtual void LoadDataExtractionBatchInfos(DataTable adtbSearchResult)
		{
			iclbDataExtractionBatchInfo = GetCollection<busDataExtractionBatchInfo>(adtbSearchResult, "icdoDataExtractionBatchInfo");
		}
	}
}
