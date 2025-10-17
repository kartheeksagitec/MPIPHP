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
	/// Class MPIPHP.BusinessObjects.busRetireeIncreaseLookupGen:
	/// Inherited from busMainBase,It is used to create new look up business object. 
	/// </summary>

	[Serializable]
	public class busRetireeIncreaseLookupGen : busMainBase
	{
		/// <summary>
		/// Gets or sets the collection object of type busPayeeAccount. 
		/// </summary>
		public Collection<busPayeeAccount> iclbPayeeAccount { get; set; }


		/// <summary>
		/// MPIPHP.BusinessObjects.busRetireeIncreaseLookupGen.LoadPayeeAccounts(DataTable):
		/// Loads Collection object iclbPayeeAccount of type busPayeeAccount.
		/// </summary>
		/// <param name="adtbSearchResult">DataTable that holds search result to fill the collection object busRetireeIncreaseLookupGen.iclbPayeeAccount</param>
		public virtual void LoadPayeeAccounts(DataTable adtbSearchResult)
		{
			iclbPayeeAccount = GetCollection<busPayeeAccount>(adtbSearchResult, "icdoPayeeAccount");
		}
	}
}
