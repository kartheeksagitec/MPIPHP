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
    /// Class MPIPHP.BusinessObjects.busPaymentScheduleStepGen:
    /// Inherited from busBase, used to create new business object for main table cdoPaymentScheduleStep and its children table. 
    /// </summary>
	[Serializable]
	public class busPaymentScheduleStepGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.BusinessObjects.busPaymentScheduleStepGen
        /// </summary>
		public busPaymentScheduleStepGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busPaymentScheduleStepGen.
        /// </summary>
		public cdoPaymentScheduleStep icdoPaymentScheduleStep { get; set; }




        /// <summary>
        /// MPIPHP.busPaymentScheduleStepGen.FindPaymentScheduleStep():
        /// Finds a particular record from cdoPaymentScheduleStep with its primary key. 
        /// </summary>
        /// <param name="aintPaymentScheduleStepId">A primary key value of type int of cdoPaymentScheduleStep on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindPaymentScheduleStep(int aintPaymentScheduleStepId)
		{
			bool lblnResult = false;
			if (icdoPaymentScheduleStep == null)
			{
				icdoPaymentScheduleStep = new cdoPaymentScheduleStep();
			}
			if (icdoPaymentScheduleStep.SelectRow(new object[1] { aintPaymentScheduleStepId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
