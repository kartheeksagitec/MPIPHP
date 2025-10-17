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
    /// Class MPIPHP.BusinessObjects.busHolidayLookupGen:
	/// Inherited from busMainBase,It is used to create new look up business object. 
	/// </summary>

	[Serializable]
	public class busHolidayLookupGen : busMainBase
	{
		/// <summary>
		/// Gets or sets the collection object of type busHoliday. 
		/// </summary>
		public Collection<busHoliday> iclbHoliday { get; set; }


		/// <summary>
        /// MPIPHP.BusinessObjects.busHolidayLookupGen.LoadHolidays(DataTable):
		/// Loads Collection object iclbHoliday of type busHoliday.
		/// </summary>
		/// <param name="adtbSearchResult">DataTable that holds search result to fill the collection object busHolidayLookupGen.iclbHoliday</param>
		public virtual void LoadHolidays(DataTable adtbSearchResult)
		{
			iclbHoliday = GetCollection<busHoliday>(adtbSearchResult, "icdoHoliday");
		}
	}
}
