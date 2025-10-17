#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using MPIPHP.DataObjects;

#endregion

namespace MPIPHP.CustomDataObjects
{
	/// <summary>
	/// Class MPIPHP.CustomDataObjects.cdoPaymentScheduleStep:
	/// Inherited from doPaymentScheduleStep, the class is used to customize the database object doPaymentScheduleStep.
	/// </summary>
    [Serializable]
	public class cdoPaymentScheduleStep : doPaymentScheduleStep
	{
		public cdoPaymentScheduleStep() : base()
		{
            
		}
        public int run_sequence { get; set; }
        public int batch_schedule_id { get; set; }
        public string istrStepName { get; set; }
        public string istrActiveFlag { get; set; }
    } 
} 
