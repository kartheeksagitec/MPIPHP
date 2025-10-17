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
	/// Class MPIPHP.BusinessObjects.busBenefitCalculationDetailLookupGen:
	/// Inherited from busMainBase,It is used to create new look up business object. 
	/// </summary>

	[Serializable]
	public class busBenefitCalculationDetailLookupGen : busMainBase
	{
		/// <summary>
		/// Gets or sets the collection object of type busBenefitCalculationDetail. 
		/// </summary>
		public Collection<busBenefitCalculationDetail> iclbBenefitCalculationDetail { get; set; }


		/// <summary>
		/// MPIPHP.BusinessObjects.busBenefitCalculationDetailLookupGen.LoadBenefitCalculationDetails(DataTable):
		/// Loads Collection object iclbBenefitCalculationDetail of type busBenefitCalculationDetail.
		/// </summary>
		/// <param name="adtbSearchResult">DataTable that holds search result to fill the collection object busBenefitCalculationDetailLookupGen.iclbBenefitCalculationDetail</param>
		public virtual void LoadBenefitCalculationDetails(DataTable adtbSearchResult)
		{
			iclbBenefitCalculationDetail = GetCollection<busBenefitCalculationDetail>(adtbSearchResult, "icdoBenefitCalculationDetail");
		}
	}
}
