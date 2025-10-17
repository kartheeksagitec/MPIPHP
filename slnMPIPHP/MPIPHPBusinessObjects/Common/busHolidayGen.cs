#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using MPIPHP.CustomDataObjects;

#endregion

namespace MPIPHP.BusinessObjects
{
    /// <summary>
    /// Class MPIPHP.BusinessObjects.busHolidayGen:
    /// Inherited from busBase, used to create new business object for main table cdoHoliday and its children table. 
    /// </summary>
	[Serializable]
	public class busHolidayGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.BusinessObjects.busHolidayGen
        /// </summary>
		public busHolidayGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busHolidayGen.
        /// </summary>
		public cdoHoliday icdoHoliday { get; set; }




        /// <summary>
        /// MPIPHP.busHolidayGen.FindHoliday():
        /// Finds a particular record from cdoHoliday with its primary key. 
        /// </summary>
        /// <param name="aintholidayid">A primary key value of type int of cdoHoliday on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindHoliday(int aintholidayid)
		{
			bool lblnResult = false;
			if (icdoHoliday == null)
			{
				icdoHoliday = new cdoHoliday();
			}
			if (icdoHoliday.SelectRow(new object[1] { aintholidayid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
