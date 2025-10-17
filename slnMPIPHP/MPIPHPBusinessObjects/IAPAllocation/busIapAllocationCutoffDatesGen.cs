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
    /// Class MPIPHP.BusinessObjects.busIapAllocationCutoffDatesGen:
    /// Inherited from busBase, used to create new business object for main table cdoIapAllocationCutoffDates and its children table. 
    /// </summary>
	[Serializable]
	public class busIapAllocationCutoffDatesGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.BusinessObjects.busIapAllocationCutoffDatesGen
        /// </summary>
		public busIapAllocationCutoffDatesGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busIapAllocationCutoffDatesGen.
        /// </summary>
		public cdoIapAllocationCutoffDates icdoIapAllocationCutoffDates { get; set; }




        /// <summary>
        /// MPIPHP.busIapAllocationCutoffDatesGen.FindIapAllocationCutoffDates():
        /// Finds a particular record from cdoIapAllocationCutoffDates with its primary key. 
        /// </summary>
        /// <param name="aintiapallocationcutoffdatesid">A primary key value of type int of cdoIapAllocationCutoffDates on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindIapAllocationCutoffDates(int aintiapallocationcutoffdatesid)
		{
			bool lblnResult = false;
			if (icdoIapAllocationCutoffDates == null)
			{
				icdoIapAllocationCutoffDates = new cdoIapAllocationCutoffDates();
			}
			if (icdoIapAllocationCutoffDates.SelectRow(new object[1] { aintiapallocationcutoffdatesid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
