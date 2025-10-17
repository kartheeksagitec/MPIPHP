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
	/// Class MPIPHP.BusinessObjects.busBenefitApplicationLookupGen:
	/// Inherited from busMainBase,It is used to create new look up business object. 
	/// </summary>

	[Serializable]
	public class busBenefitApplicationLookupGen : busMainBase
	{
		/// <summary>
		/// Gets or sets the collection object of type busBenefitApplication. 
		/// </summary>
		public Collection<busBenefitApplication> iclbBenefitApplication { get; set; }


		/// <summary>
		/// MPIPHP.BusinessObjects.busBenefitApplicationLookupGen.LoadBenefitApplications(DataTable):
		/// Loads Collection object iclbBenefitApplication of type busBenefitApplication.
		/// </summary>
		/// <param name="adtbSearchResult">DataTable that holds search result to fill the collection object busBenefitApplicationLookupGen.iclbBenefitApplication</param>
		public virtual void LoadBenefitApplications(DataTable adtbSearchResult)
		{
			iclbBenefitApplication = GetCollection<busBenefitApplication>(adtbSearchResult, "icdoBenefitApplication");
		}
	}
}
