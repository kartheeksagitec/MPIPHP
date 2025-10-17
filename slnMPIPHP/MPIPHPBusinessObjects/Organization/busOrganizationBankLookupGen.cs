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
	/// Class MPIPHP.BusinessObjects.busOrganizationBankLookupGen:
	/// Inherited from busMainBase,It is used to create new look up business object. 
	/// </summary>

	[Serializable]
	public class busOrganizationBankLookupGen : busMainBase
	{
		/// <summary>
		/// Gets or sets the collection object of type busOrgBank. 
		/// </summary>
		public Collection<busOrgBank> iclbOrgBank { get; set; }


		/// <summary>
		/// MPIPHP.BusinessObjects.busOrganizationBankLookupGen.LoadOrgBanks(DataTable):
		/// Loads Collection object iclbOrgBank of type busOrgBank.
		/// </summary>
		/// <param name="adtbSearchResult">DataTable that holds search result to fill the collection object busOrganizationBankLookupGen.iclbOrgBank</param>
		public virtual void LoadOrgBanks(DataTable adtbSearchResult)
		{
			iclbOrgBank = GetCollection<busOrgBank>(adtbSearchResult, "icdoOrgBank");
		}
	}
}
