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
	/// Class MPIPHP.BusinessObjects.busBenefitCalculationHeaderLookupGen:
	/// Inherited from busMainBase,It is used to create new look up business object. 
	/// </summary>

	[Serializable]
	public class busBenefitCalculationHeaderLookupGen : busMainBase
	{
		/// <summary>
		/// Gets or sets the collection object of type busBenefitCalculationHeader. 
		/// </summary>
		public Collection<busBenefitCalculationHeader> iclbBenefitCalculationHeader { get; set; }


		/// <summary>
		/// MPIPHP.BusinessObjects.busBenefitCalculationHeaderLookupGen.LoadBenefitCalculationHeaders(DataTable):
		/// Loads Collection object iclbBenefitCalculationHeader of type busBenefitCalculationHeader.
		/// </summary>
		/// <param name="adtbSearchResult">DataTable that holds search result to fill the collection object busBenefitCalculationHeaderLookupGen.iclbBenefitCalculationHeader</param>
		public virtual void LoadBenefitCalculationHeaders(DataTable adtbSearchResult)
		{
			iclbBenefitCalculationHeader = GetCollection<busBenefitCalculationHeader>(adtbSearchResult, "icdoBenefitCalculationHeader");
		}
	}
}
