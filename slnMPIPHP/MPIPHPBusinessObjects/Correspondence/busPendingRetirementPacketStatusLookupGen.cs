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
using MPIPHP.DataObjects;

#endregion

namespace MPIPHP.BusinessObjects
{
	/// <summary>
	/// Class MPIPHP.busPendingRetirementPacketStatusLookupGen:
	/// Inherited from busMainBase,It is used to create new look up business object. 
	/// </summary>

	[Serializable]
	public class busPendingRetirementPacketStatusLookupGen : busMainBase
	{
		/// <summary>
		/// Gets or sets the collection object of type busCorPacketContentTracking. 
		/// </summary>
		public Collection<busCorPacketContentTracking> iclbCorPacketContentTracking { get; set; }


		/// <summary>
		/// MPIPHP.busPendingRetirementPacketStatusLookupGen.LoadCorPacketContentTrackings(DataTable):
		/// Loads Collection object iclbCorPacketContentTracking of type busCorPacketContentTracking.
		/// </summary>
		/// <param name="adtbSearchResult">DataTable that holds search result to fill the collection object busPendingRetirementPacketStatusLookupGen.iclbCorPacketContentTracking</param>
		public virtual void LoadCorPacketContentTrackings(DataTable adtbSearchResult)
		{
			iclbCorPacketContentTracking = GetCollection<busCorPacketContentTracking>(adtbSearchResult, "icdoCorPacketContentTracking");
		}
	}
}
