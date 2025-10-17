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

namespace MPIPHP
{
	/// <summary>
	/// Class MPIPHP.busBenefitInterestRateLookupGen:
	/// Inherited from busMainBase,It is used to create new look up business object. 
	/// </summary>

	[Serializable]
	public class busBenefitInterestRateLookupGen : busMainBase
	{
		/// <summary>
		/// Gets or sets the collection object of type busBenefitInterestRate. 
		/// </summary>
		public Collection<busBenefitInterestRate> iclbBenefitInterestRate { get; set; }


		/// <summary>
		/// MPIPHP.busBenefitInterestRateLookupGen.LoadBenefitInterestRates(DataTable):
		/// Loads Collection object iclbBenefitInterestRate of type busBenefitInterestRate.
		/// </summary>
		/// <param name="adtbSearchResult">DataTable that holds search result to fill the collection object busBenefitInterestRateLookupGen.iclbBenefitInterestRate</param>
		public virtual void LoadBenefitInterestRates(DataTable adtbSearchResult)
		{
			iclbBenefitInterestRate = GetCollection<busBenefitInterestRate>(adtbSearchResult, "icdoBenefitInterestRate");
		}
	}
}
