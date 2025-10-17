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
	/// Class MPIPHP.BusinessObjects.busPaymentScheduleStep:
	/// Inherited from busPaymentScheduleStepGen, the class is used to customize the business object busPaymentScheduleStepGen.
	/// </summary>
	[Serializable]
	public class busPaymentScheduleStep : busPaymentScheduleStepGen
	{
        public bool iblnStepFailedIndicator { get; set; }
        public busPaymentStepRef ibusPaymentStepRef { get; set; }
	}
}
