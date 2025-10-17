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
	/// Class MPIPHP.BusinessObjects.busQdroCalculationLookupGen:
	/// Inherited from busMainBase,It is used to create new look up business object. 
	/// </summary>

	[Serializable]
	public class busQdroCalculationLookupGen : busMainBase
	{
		/// <summary>
		/// Gets or sets the collection object of type busQdroCalculationHeader. 
		/// </summary>
		public Collection<busQdroCalculationHeader> iclbQdroCalculationHeader { get; set; }


		/// <summary>
		/// MPIPHP.BusinessObjects.busQdroCalculationLookupGen.LoadQdroCalculationHeaders(DataTable):
		/// Loads Collection object iclbQdroCalculationHeader of type busQdroCalculationHeader.
		/// </summary>
		/// <param name="adtbSearchResult">DataTable that holds search result to fill the collection object busQdroCalculationLookupGen.iclbQdroCalculationHeader</param>
		public virtual void LoadQdroCalculationHeaders(DataTable adtbSearchResult)
		{
			iclbQdroCalculationHeader = GetCollection<busQdroCalculationHeader>(adtbSearchResult, "icdoQdroCalculationHeader");
			int i = 1;
			foreach (busQdroCalculationHeader lbusQdroCalculationHeader in iclbQdroCalculationHeader)
			{
				lbusQdroCalculationHeader.icdoQdroCalculationHeader.iintAPrimaryKey = i;
				i++;
			}
		}
	}
}
