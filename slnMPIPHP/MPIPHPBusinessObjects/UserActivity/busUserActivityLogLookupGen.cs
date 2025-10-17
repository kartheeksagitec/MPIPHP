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
	/// Class MPIPHP.BusinessObjects.busUserActivityLogLookupGen:
	/// Inherited from busMainBase,It is used to create new look up business object. 
	/// </summary>

	[Serializable]
	public class busUserActivityLogLookupGen : busMainBase
	{
		/// <summary>
		/// Gets or sets the collection object of type busUserActivityLog. 
		/// </summary>
		public Collection<busUserActivityLog> iclbUserActivityLog { get; set; }


		/// <summary>
		/// MPIPHP.BusinessObjects.busUserActivityLogLookupGen.LoadUserActivityLogs(DataTable):
		/// Loads Collection object iclbUserActivityLog of type busUserActivityLog.
		/// </summary>
		/// <param name="adtbSearchResult">DataTable that holds search result to fill the collection object busUserActivityLogLookupGen.iclbUserActivityLog</param>
		public virtual void LoadUserActivityLogs(DataTable adtbSearchResult)
		{
			iclbUserActivityLog = GetCollection<busUserActivityLog>(adtbSearchResult, "icdoUserActivityLog");
		}
	}
}
