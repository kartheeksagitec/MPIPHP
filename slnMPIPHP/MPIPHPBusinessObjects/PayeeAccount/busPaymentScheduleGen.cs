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
    /// Class MPIPHP.BusinessObjects.busPaymentScheduleGen:
    /// Inherited from busBase, used to create new business object for main table cdoPaymentSchedule and its children table. 
    /// </summary>
	[Serializable]
	public class busPaymentScheduleGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.BusinessObjects.busPaymentScheduleGen
        /// </summary>
		public busPaymentScheduleGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busPaymentScheduleGen.
        /// </summary>
		public cdoPaymentSchedule icdoPaymentSchedule { get; set; }




        /// <summary>
        /// MPIPHP.busPaymentScheduleGen.FindPaymentSchedule():
        /// Finds a particular record from cdoPaymentSchedule with its primary key. 
        /// </summary>
        /// <param name="aintPaymentScheduleId">A primary key value of type int of cdoPaymentSchedule on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindPaymentSchedule(int aintPaymentScheduleId)
		{
			bool lblnResult = false;
			if (icdoPaymentSchedule == null)
			{
				icdoPaymentSchedule = new cdoPaymentSchedule();
			}
			if (icdoPaymentSchedule.SelectRow(new object[1] { aintPaymentScheduleId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
