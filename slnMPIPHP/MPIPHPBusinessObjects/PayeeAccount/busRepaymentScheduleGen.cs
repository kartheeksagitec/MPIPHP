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
    /// Class MPIPHP.BusinessObjects.busRepaymentScheduleGen:
    /// Inherited from busBase, used to create new business object for main table cdoRepaymentSchedule and its children table. 
    /// </summary>
	[Serializable]
	public class busRepaymentScheduleGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.BusinessObjects.busRepaymentScheduleGen
        /// </summary>
        public busRepaymentScheduleGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busRepaymentScheduleGen.
        /// </summary>
        public cdoRepaymentSchedule icdoRepaymentSchedule { get; set; }




        /// <summary>
        /// MPIPHP.busRepaymentScheduleGen.FindRepaymentSchedule():
        /// Finds a particular record from cdoRepaymentSchedule with its primary key. 
        /// </summary>
        /// <param name="aintRepaymentScheduleId">A primary key value of type int of cdoRepaymentSchedule on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
        public virtual bool FindRepaymentSchedule(int aintRepaymentScheduleId)
		{
			bool lblnResult = false;
            if (icdoRepaymentSchedule == null)
			{
                icdoRepaymentSchedule = new cdoRepaymentSchedule();
			}
            if (icdoRepaymentSchedule.SelectRow(new object[1] { aintRepaymentScheduleId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
