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
using MPIPHP.DataObjects;

#endregion

namespace MPIPHP.BusinessObjects
{
    /// <summary>
    /// Class MPIPHP.BusinessObjects.busPaymentStepRefGen:
    /// Inherited from busBase, used to create new business object for main table cdoPaymentStepRef and its children table. 
    /// </summary>
	[Serializable]
	public class busPaymentStepRefGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.BusinessObjects.busPaymentStepRefGen
        /// </summary>
		public busPaymentStepRefGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busPaymentStepRefGen.
        /// </summary>
		public cdoPaymentStepRef icdoPaymentStepRef { get; set; }


        /// <summary>
        /// Gets or sets the collection object of type busPaymentScheduleStep. 
        /// </summary>
		public Collection<busPaymentScheduleStep> iclbPaymentScheduleStep { get; set; }



        /// <summary>
        /// MPIPHP.busPaymentStepRefGen.FindPaymentStepRef():
        /// Finds a particular record from cdoPaymentStepRef with its primary key. 
        /// </summary>
        /// <param name="aintPaymentStepId">A primary key value of type int of cdoPaymentStepRef on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindPaymentStepRef(int aintPaymentStepId)
		{
			bool lblnResult = false;
			if (icdoPaymentStepRef == null)
			{
				icdoPaymentStepRef = new cdoPaymentStepRef();
			}
			if (icdoPaymentStepRef.SelectRow(new object[1] { aintPaymentStepId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

        /// <summary>
        /// MPIPHP.busPaymentStepRefGen.LoadPaymentScheduleSteps():
        /// Loads Collection object iclbPaymentScheduleStep of type busPaymentScheduleStep.
        /// </summary>
		public virtual void LoadPaymentScheduleSteps()
		{
			DataTable ldtbList = Select<cdoPaymentScheduleStep>(
				new string[1] { enmPaymentScheduleStep.payment_step_id.ToString() },
				new object[1] { icdoPaymentStepRef.payment_step_id }, null, null);
			iclbPaymentScheduleStep = GetCollection<busPaymentScheduleStep>(ldtbList, "icdoPaymentScheduleStep");
		}

	}
}
