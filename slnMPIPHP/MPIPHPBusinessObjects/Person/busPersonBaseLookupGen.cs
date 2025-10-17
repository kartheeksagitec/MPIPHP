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
	/// Class MPIPHP.BusinessObjects.busPersonBaseLookupGen:
	/// Inherited from busMainBase,It is used to create new look up business object. 
	/// </summary>

	[Serializable]
	public class busPersonBaseLookupGen : busMainBase
	{
		/// <summary>
		/// Gets or sets the collection object of type busPersonBase. 
		/// </summary>
		public Collection<busPersonBase> iclbPersonBase { get; set; }


		/// <summary>
		/// MPIPHP.BusinessObjects.busPersonBaseLookupGen.LoadPersonBases(DataTable):
		/// Loads Collection object iclbPersonBase of type busPersonBase.
		/// </summary>
		/// <param name="adtbSearchResult">DataTable that holds search result to fill the collection object busPersonBaseLookupGen.iclbPersonBase</param>
		public virtual void LoadPersonBases(DataTable adtbSearchResult)
		{
			iclbPersonBase = GetCollection<busPersonBase>(adtbSearchResult, "icdoPersonBase");
		}
	}
}
