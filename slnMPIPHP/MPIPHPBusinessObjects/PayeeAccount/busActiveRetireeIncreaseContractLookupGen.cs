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
	/// Class MPIPHP.BusinessObjects.busActiveRetireeIncreaseContractLookupGen:
	/// Inherited from busMainBase,It is used to create new look up business object. 
	/// </summary>

	[Serializable]
	public class busActiveRetireeIncreaseContractLookupGen : busMainBase
	{
		/// <summary>
		/// Gets or sets the collection object of type busActiveRetireeIncreaseContract. 
		/// </summary>
		public Collection<busActiveRetireeIncreaseContract> iclbActiveRetireeIncreaseContract { get; set; }


		/// <summary>
		/// MPIPHP.BusinessObjects.busActiveRetireeIncreaseContractLookupGen.LoadActiveRetireeIncreaseContracts(DataTable):
		/// Loads Collection object iclbActiveRetireeIncreaseContract of type busActiveRetireeIncreaseContract.
		/// </summary>
		/// <param name="adtbSearchResult">DataTable that holds search result to fill the collection object busActiveRetireeIncreaseContractLookupGen.iclbActiveRetireeIncreaseContract</param>
		public virtual void LoadActiveRetireeIncreaseContracts(DataTable adtbSearchResult)
		{
			iclbActiveRetireeIncreaseContract = GetCollection<busActiveRetireeIncreaseContract>(adtbSearchResult, "icdoActiveRetireeIncreaseContract");
		}
	}
}
