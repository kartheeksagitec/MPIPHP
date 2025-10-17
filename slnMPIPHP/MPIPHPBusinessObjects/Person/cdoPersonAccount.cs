#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using MPIPHP.DataObjects;
using MPIPHP.BusinessObjects;

#endregion

namespace MPIPHP.CustomDataObjects
{
	/// <summary>
	/// Class MPIPHP.CustomDataObjects.cdoPersonAccount:
	/// Inherited from doPersonAccount, the class is used to customize the database object doPersonAccount.
	/// </summary>
    [Serializable]
	public class cdoPersonAccount : doPersonAccount
	{
		public cdoPersonAccount() : base()
		{
		}

        public string istrPlan { get; set; }
        public string istrPlanCode { get; set; }
        public string istrPlanDesc { get; set; }
        public string istrRetirementSubType { get; set; }
        public DateTime idtMergerDate { get; set; }
        public DateTime idtVestedDate { get; set; }

        // ---- Used in Overview 
        public string istrPlanParticipantStatus { get; set; }
        public bool istrVested { get; set; }
        public DateTime dtVestedDate { get; set; }
        public DateTime dtForfeitureDate { get; set; }
        public DateTime dtWithDrawlDate { get; set; }
        public decimal istrTotalHours { get; set; }
        public int istrTotalQualifiedYears { get; set; }
        public int istrTotalVestedYears { get; set; }
        public decimal idecVestedEE { get; set; }
        public decimal idecNonVestedEE { get; set; }
        public decimal idecVestedEEInterest { get; set; }
        public decimal idecNonVestedEEInterest { get; set; }
        public decimal idecAccruedBenefit { get; set; }

        public string istrSpecialAccountFlag
        {
            get
            {
                string lstrSpecial = special_account;
                if (lstrSpecial == busConstant.Flag_Yes)
                {
                    lstrSpecial = busConstant.YES;
                }
                else if (lstrSpecial == busConstant.FLAG_NO)
                {
                    lstrSpecial = busConstant.NO;
                }
                return lstrSpecial;
            }
        
        }

        public string idecTotalAccruedBenefit { get; set; }
        public string idecSpecialAccountBalance { get; set; }
        public decimal istrHealthHours { get; set; }
        public decimal iintTotalHealthYears { get; set; }
        public string istrPensionCredit { get; set; }
        public int iinlocalQualifiedYearsCount { get; set; }
        public string istrAllocationEndYear { get; set; }
        public string istrHealthHoursPO { get; set; }
        public string istrTotalHealthYearsPO { get; set; }
        // ---- Used in Overview
    } 
} 
