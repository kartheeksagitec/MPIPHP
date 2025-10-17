#region Using directives

using System;
using System.Data;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.CustomDataObjects;

#endregion

namespace MPIPHP.BusinessObjects
{
	/// <summary>
	/// Class MPIPHP.busPacketContentLookupGen:
	/// Inherited from busMainBase,It is used to create new look up business object. 
	/// </summary>

	[Serializable]
	public class busPacketContentLookupGen : busMainBase
	{
		/// <summary>
		/// Gets or sets the collection object of type busCorPacketContent. 
		/// </summary>
		public Collection<busCorPacketContent> iclbCorPacketContent { get; set; }


		/// <summary>
		/// MPIPHP.busPacketContentLookupGen.LoadCorPacketContents(DataTable):
		/// Loads Collection object iclbCorPacketContent of type busCorPacketContent.
		/// </summary>
		/// <param name="adtbSearchResult">DataTable that holds search result to fill the collection object busPacketContentLookupGen.iclbCorPacketContent</param>
		public virtual void LoadCorPacketContents(DataTable adtbSearchResult)
		{
			iclbCorPacketContent = GetCollection<busCorPacketContent>(adtbSearchResult, "icdoCorPacketContent");
		}
	}
}
