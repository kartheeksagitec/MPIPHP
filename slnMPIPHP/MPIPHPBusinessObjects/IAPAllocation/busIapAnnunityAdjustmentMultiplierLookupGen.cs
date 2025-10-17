#region Using directives

using System;
using System.Data;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using MPIPHP.CustomDataObjects;

#endregion

namespace MPIPHP
{
	/// <summary>
	/// Class MPIPHP.busIapAnnunityAdjustmentMultiplierLookupGen:
	/// Inherited from busMainBase,It is used to create new look up business object. 
	/// </summary>

	[Serializable]
	public class busIapAnnunityAdjustmentMultiplierLookupGen : busMainBase
	{
		/// <summary>
		/// Gets or sets the collection object of type busIapAnnunityAdjustmentMultiplier. 
		/// </summary>
		public Collection<busIapAnnunityAdjustmentMultiplier> iclbIapAnnunityAdjustmentMultiplier { get; set; }


		/// <summary>
		/// MPIPHP.busIapAnnunityAdjustmentMultiplierLookupGen.LoadIapAnnunityAdjustmentMultipliers(DataTable):
		/// Loads Collection object iclbIapAnnunityAdjustmentMultiplier of type busIapAnnunityAdjustmentMultiplier.
		/// </summary>
		/// <param name="adtbSearchResult">DataTable that holds search result to fill the collection object busIapAnnunityAdjustmentMultiplierLookupGen.iclbIapAnnunityAdjustmentMultiplier</param>
		public virtual void LoadIapAnnunityAdjustmentMultipliers(DataTable adtbSearchResult)
		{
			iclbIapAnnunityAdjustmentMultiplier = GetCollection<busIapAnnunityAdjustmentMultiplier>(adtbSearchResult, "icdoIapAnnunityAdjustmentMultiplier");
		}
	}
}
