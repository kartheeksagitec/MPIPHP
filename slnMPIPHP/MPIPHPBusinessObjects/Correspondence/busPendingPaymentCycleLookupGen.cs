#region Using directives

using System;
using System.Data;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.DBUtility;

#endregion

namespace MPIPHP.BusinessObjects
{
	/// <summary>
	/// Class MPIPHP.busPendingPaymentCycleLookupGen:
	/// Inherited from busMainBase,It is used to create new look up business object. 
	/// </summary>

	[Serializable]
	public class busPendingPaymentCycleLookupGen : busMainBase
	{
		/// <summary>
		/// Gets or sets the collection object of type busCorPacketContentTracking. 
		/// </summary>
		public Collection<busCorPacketContentTracking> iclbCorPacketContentTracking { get; set; }


		/// <summary>
		/// MPIPHP.busPendingPaymentCycleLookupGen.LoadCorPacketContentTrackings(DataTable):
		/// Loads Collection object iclbCorPacketContentTracking of type busCorPacketContentTracking.
		/// </summary>
		/// <param name="adtbSearchResult">DataTable that holds search result to fill the collection object busPendingPaymentCycleLookupGen.iclbCorPacketContentTracking</param>
		public virtual void LoadCorPacketContentTrackings(DataTable adtbSearchResult)
		{
			iclbCorPacketContentTracking = GetCollection<busCorPacketContentTracking>(adtbSearchResult, "icdoCorPacketContentTracking");
		}
	}
}
