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
	/// Class MPIPHP.BusinessObjects.busRecipientRolloverOrganisationLookupGen:
	/// Inherited from busMainBase,It is used to create new look up business object. 
	/// </summary>

	[Serializable]
	public class busRecipientRolloverOrganisationLookupGen : busMainBase
	{
		/// <summary>
		/// Gets or sets the collection object of type busPayeeAccountRolloverDetail. 
		/// </summary>
		public Collection<busPayeeAccountRolloverDetail> iclbPayeeAccountRolloverDetail { get; set; }


		/// <summary>
		/// MPIPHP.BusinessObjects.busRecipientRolloverOrganisationLookupGen.LoadPayeeAccountRolloverDetails(DataTable):
		/// Loads Collection object iclbPayeeAccountRolloverDetail of type busPayeeAccountRolloverDetail.
		/// </summary>
		/// <param name="adtbSearchResult">DataTable that holds search result to fill the collection object busRecipientRolloverOrganisationLookupGen.iclbPayeeAccountRolloverDetail</param>
		public virtual void LoadPayeeAccountRolloverDetails(DataTable adtbSearchResult)
		{
			iclbPayeeAccountRolloverDetail = GetCollection<busPayeeAccountRolloverDetail>(adtbSearchResult, "icdoPayeeAccountRolloverDetail");
		}
	}
}
