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
	/// Class MPIPHP.CustomDataObjects.cdoIapallocationDetailPersonoverview:
	/// Inherited from doIapallocationDetailPersonoverview, the class is used to customize the database object doIapallocationDetailPersonoverview.
	/// </summary>
    [Serializable]
	public class cdoIapallocationDetailPersonoverview : doIapallocationDetailPersonoverview
	{
		public cdoIapallocationDetailPersonoverview() : base()
		{
		}

        public Decimal L52ALLOC1 { get; set; }
        public Decimal L161ALLOC1 { get; set; }
        public string affiliate { get; set; }
        public decimal ytdhours { get; set; }
        public DateTime effective_date { get; set; }
        public string transaction_type_value { get; set; }
        public string record_freeze_flag { get; set; }
        public Decimal alloc5 { get; set; }
        public decimal idecYTDHoursA2 { get; set; }
        public string istrSource { get; set; }
        public string istrSSN { get; set; }
        public decimal idecTotal { get; set; }
        public decimal idecOverrideTotal { get; set; }
        public string istrCalculateAlloc5 { get; set; }
        public string istrForfietureFlag { get; set; }
        public bool iblnReemploymentFlag { get; set; }
        public string istrFundType { get; set; } //PIR 19
        public decimal idecRunningIAPAllocationBalance { get; set; }  //PIR RID 73750 
       
    } 

} 
