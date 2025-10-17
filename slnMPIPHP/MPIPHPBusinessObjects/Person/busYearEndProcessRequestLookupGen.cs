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
	/// Class MPIPHP.BusinessObjects.busYearEndProcessRequestLookupGen:
	/// Inherited from busMainBase,It is used to create new look up business object. 
	/// </summary>

	[Serializable]
	public class busYearEndProcessRequestLookupGen : busMainBase
	{
		/// <summary>
		/// Gets or sets the collection object of type busYearEndProcessRequest. 
		/// </summary>
		public Collection<busYearEndProcessRequest> iclbYearEndProcessRequest { get; set; }


		/// <summary>
		/// MPIPHP.BusinessObjects.busYearEndProcessRequestLookupGen.LoadYearEndProcessRequests(DataTable):
		/// Loads Collection object iclbYearEndProcessRequest of type busYearEndProcessRequest.
		/// </summary>
		/// <param name="adtbSearchResult">DataTable that holds search result to fill the collection object busYearEndProcessRequestLookupGen.iclbYearEndProcessRequest</param>
		public virtual void LoadYearEndProcessRequests(DataTable adtbSearchResult)
		{
			iclbYearEndProcessRequest = GetCollection<busYearEndProcessRequest>(adtbSearchResult, "icdoYearEndProcessRequest");
		}
	}
}
