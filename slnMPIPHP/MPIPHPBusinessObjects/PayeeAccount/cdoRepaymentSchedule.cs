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
	/// Class MPIPHP.CustomDataObjects.cdoRepaymentSchedule:
	/// Inherited from doRepaymentSchedule, the class is used to customize the database object doRepaymentSchedule.
	/// </summary>
    [Serializable]
	public class cdoRepaymentSchedule : doRepaymentSchedule
	{

        public decimal idecRemainingOverPaymentAmount { get; set; }
        public DateTime idtPaymentDate { get; set; }
        public string istrSuspendibleMonth { get; set; }
        public decimal idecEEDerived { get; set; }
        public decimal idecERDerived { get; set; }
        public decimal idecTotal { get; set; }
        public decimal idecRepaymentToTheplan { get; set; }
        public decimal idecPayableForTheMonth { get; set; }
        public decimal idecReimbrAmtInCurrentMonth { get; set; }
        public decimal idecToDateReimbrAmount { get; set; }

        public decimal idecFlatPercent { get; set; }
        public decimal idecMonthlyAmtOfReduction { get; set; }

		public cdoRepaymentSchedule() : base()
		{
		}

    } 
} 
