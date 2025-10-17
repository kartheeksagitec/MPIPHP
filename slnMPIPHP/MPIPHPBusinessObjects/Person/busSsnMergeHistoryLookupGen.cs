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
	/// Class MPIPHP.BusinessObjects.busSsnMergeHistoryLookupGen:
	/// Inherited from busMainBase,It is used to create new look up business object. 
	/// </summary>

	[Serializable]
	public class busSsnMergeHistoryLookupGen : busMainBase
	{
		/// <summary>
		/// Gets or sets the collection object of type busSsnMergeHistory. 
		/// </summary>
		public Collection<busSsnMergeHistory> iclbSsnMergeHistory { get; set; }


		/// <summary>
		/// MPIPHP.BusinessObjects.busSsnMergeHistoryLookupGen.LoadSsnMergeHistorys(DataTable):
		/// Loads Collection object iclbSsnMergeHistory of type busSsnMergeHistory.
		/// </summary>
		/// <param name="adtbSearchResult">DataTable that holds search result to fill the collection object busSsnMergeHistoryLookupGen.iclbSsnMergeHistory</param>
		public virtual void LoadSsnMergeHistorys(DataTable adtbSearchResult)
		{
			iclbSsnMergeHistory = GetCollection<busSsnMergeHistory>(adtbSearchResult, "icdoSsnMergeHistory");
		}
	}
}
