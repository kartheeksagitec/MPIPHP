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
	/// Class MPIPHP.BusinessObjects.busBenefitProvisionEligibilityLookupGen:
	/// Inherited from busMainBase,It is used to create new look up business object. 
	/// </summary>

	[Serializable]
	public class busBenefitProvisionEligibilityLookupGen : busMainBase
	{
		/// <summary>
		/// Gets or sets the collection object of type busBenefitProvisionEligibility. 
		/// </summary>
		public Collection<busBenefitProvisionEligibility> iclbBenefitProvisionEligibility { get; set; }


		/// <summary>
		/// MPIPHP.BusinessObjects.busBenefitProvisionEligibilityLookupGen.LoadBenefitProvisionEligibilitys(DataTable):
		/// Loads Collection object iclbBenefitProvisionEligibility of type busBenefitProvisionEligibility.
		/// </summary>
		/// <param name="adtbSearchResult">DataTable that holds search result to fill the collection object busBenefitProvisionEligibilityLookupGen.iclbBenefitProvisionEligibility</param>
		public virtual void LoadBenefitProvisionEligibilitys(DataTable adtbSearchResult)
		{
			iclbBenefitProvisionEligibility = GetCollection<busBenefitProvisionEligibility>(adtbSearchResult, "icdoBenefitProvisionEligibility");
		}
	}
}
