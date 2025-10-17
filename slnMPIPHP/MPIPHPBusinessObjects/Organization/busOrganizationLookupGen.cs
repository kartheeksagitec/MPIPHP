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
	/// Class MPIPHP.BusinessObjects.busOrganizationLookupGen:
	/// Inherited from busMainBase,It is used to create new look up business object. 
	/// </summary>

	[Serializable]
	public class busOrganizationLookupGen : busMainBase
	{
		/// <summary>
		/// Gets or sets the collection object of type busOrganization. 
		/// </summary>
		public Collection<busOrganization> iclbOrganization { get; set; }
       

		/// <summary>
		/// MPIPHP.BusinessObjects.busOrganizationLookupGen.LoadOrganizations(DataTable):
		/// Loads Collection object iclbOrganization of type busOrganization.
		/// </summary>
		/// <param name="adtbSearchResult">DataTable that holds search result to fill the collection object busOrganizationLookupGen.iclbOrganization</param>
		public virtual void LoadOrganizations(DataTable adtbSearchResult)
		{
			iclbOrganization = GetCollection<busOrganization>(adtbSearchResult, "icdoOrganization");
		}
	}
}
