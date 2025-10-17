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
    /// Class MPIPHP.BusinessObjects.busPersonSuspendibleMonthGen:
    /// Inherited from busBase, used to create new business object for main table cdoPersonSuspendibleMonth and its children table. 
    /// </summary>
	[Serializable]
	public class busPersonSuspendibleMonthGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.BusinessObjects.busPersonSuspendibleMonthGen
        /// </summary>
		public busPersonSuspendibleMonthGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busPersonSuspendibleMonthGen.
        /// </summary>
		public cdoPersonSuspendibleMonth icdoPersonSuspendibleMonth { get; set; }




        /// <summary>
        /// MPIPHP.busPersonSuspendibleMonthGen.FindPersonSuspendibleMonth():
        /// Finds a particular record from cdoPersonSuspendibleMonth with its primary key. 
        /// </summary>
        /// <param name="aintsgtpersonsuspendiblemonthid">A primary key value of type int of cdoPersonSuspendibleMonth on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
        public virtual bool FindPersonSuspendibleMonth(int aintPersonSuspendibleMonthID)
		{
			bool lblnResult = false;
			if (icdoPersonSuspendibleMonth == null)
			{
				icdoPersonSuspendibleMonth = new cdoPersonSuspendibleMonth();
			}
            if (icdoPersonSuspendibleMonth.SelectRow(new object[1] { aintPersonSuspendibleMonthID }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
