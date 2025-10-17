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
	/// Class MPIPHP.CustomDataObjects.cdoQdroCalculationHeader:
	/// Inherited from doQdroCalculationHeader, the class is used to customize the database object doQdroCalculationHeader.
	/// </summary>
    [Serializable]
	public class cdoQdroCalculationHeader : doQdroCalculationHeader
	{
		public cdoQdroCalculationHeader() : base()
		{
		}

        public string istrMemeber_Fullname { set; get; }
        public string istrAlternate_Payee_Fullname { set; get; }
        public int iintPlanId { get; set; }
        public string istrRelativeVipFlag { get; set; }
        public string istrPlanDescription { get; set; }
        public decimal idecAlternatePayeeAgeAtRetirement { get; set; }
        public string istrSurvivorMPID { get; set; }
        public int iintAltPayeeAgeAtRetirement { get; set; }
        public int iintParticipantAtRetirement { get; set; }
        public string istrRetirementType { get; set; }
        public string QDROModelDescription { get; set; }
        public string CASE_NUMBER { get; set; }
        public DateTime RECEIVED_DATE { get; set; }
        public DateTime QUALIFIED_DATE { get; set; }
        public long iintAPrimaryKey { get; set; }
        //10 Percent
        public int iintPayeeAccountId { get; set; }
     } 
} 
