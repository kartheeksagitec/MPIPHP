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
	/// Class MPIPHP.CustomDataObjects.cdoDummyWorkData:
	/// Inherited from doDummyWorkData, the class is used to customize the database object doDummyWorkData.
	/// </summary>
    [Serializable]
	public class cdoDummyWorkData : doDummyWorkData
	{
		public cdoDummyWorkData() : base()
		{
		}

        public decimal idcPensionHours_healthBatch { get; set; }

        public decimal idcIAPHours_healthBatch { get; set; }

        public DateTime PlanStartDate { get; set; }

        // The following properties are used for Pension Retirement Benefit Calculation
        //public bool iblnBIS_Flag { get; set; }
        public int iintPlanYear { get; set; }
        public decimal idecBenefitRate { get; set; }
        public decimal idecBenefitAmount { get; set; }
        public decimal idecBenefitAmountLocal { get; set; }
        public decimal idecPlanYearAccruedBenefit { get; set; }
        public bool iblnHoursAfterRetirement { get; set; }
        public decimal idecIAPHours { get; set; }
        public int iintLateHourCount { get; set; }

        // Public Properties required for Late Retirement
        public decimal idecEEDerivedBenefit { get; set; }
        public decimal idecEEActurialIncrease { get; set; }
        public decimal idecMaxEEDerivedBenefit { get; set; }
        public decimal idecERDerivedBenefit { get; set; }
        public decimal idecERActurialIncrease { get; set; }
        public decimal idecTotalERDerivedBenefit { get; set; }
        public decimal idecLateRetirementAdjustment { get; set; }
        public decimal idecAnnualMax { get; set; }
        public int iintNonSuspendibleMonths { get; set; }
        public DateTime firstHourReported { get; set; }

        //For Annual benefit summary overview
        public decimal idecTotalPensionHours { get; set; }
        public decimal idecTotalIAPHours { get; set; }
        public decimal idecTotalHealthHours { get; set; }
        public int iintHealthCount { get; set; }
        public decimal idecTotalVestedHours { get; set; }
        public int iintQualifiedYears { get; set; }
        public int iiVestedYears { get; set; }
        public decimal idecTotalAccruedBenefit { get; set; }
        public decimal idecLocalAccruedBenefit { get; set; }
        //Mss
        public Decimal Withdrawal_Hours { get; set; }
        public Decimal Forfieture_Hours { get; set; }

        public string CheckVesting { get; set; }
        public bool iblnWithdrawalReset { get; set; }

        //Public Properties for the Showing the Work History and Including the Locals for MPI vesting and eligibility
        //New twist from MPI
        public Decimal L600_Hours { get; set; }
        public Decimal L666_Hours { get; set; }
        public Decimal L700_Hours { get; set; }
        public Decimal L161_Hours { get; set; }
        public Decimal L52_Hours { get; set; }
        public Decimal IAP_HOURSA2 { get; set; }
        public Decimal IAP_PERCENT { get; set; }

        //PIR 1035
        public Decimal idecBenefitRateAtMDAge { get; set; }


        public Decimal L600_PensionCredits { get; set; }
        public Decimal L666_PensionCredits { get; set; }
        public Decimal L161_PensionCredits { get; set; }
        public Decimal L700_PensionCredits { get; set; }
        public Decimal L52_PensionCredits { get; set; }

        public decimal idecQdroHours { get; set; }
        public decimal iintAgetoShow { get; set; }
        public string istrForfietureFlag { get; set; }
        public string istrBisParticipantFlag { get; set; }
        public int iintPersonId { get; set; }

        //For Corr RETR-0018
        public DateTime BeginingDate { get; set; }
        public DateTime EndingDate { get; set; }
        public int PlanYear { get; set; }
        public decimal IAPHours { get; set; }
        public decimal MPIHours { get; set; }
        public string Employer { get; set; }
        public int UnionCode { get; set; }
        public string UnionCodeDesc { get; set; }
        public string istrHealthEligibilty { get; set; }

        //Enhancement 
        public decimal idecEEContribution { get; set; }
        public decimal idecEEInterest { get; set; }
        public decimal idecUVHPContribution { get; set; }
        public decimal idecUVHPInterest { get; set; }
        public decimal idecWithdrawalHours { get; set; }
        public decimal idectotalBenefitAmount { get; set; }
        public string istrPlanCode { get; set; }
        public Decimal idecTempqualified_hours { get; set; }
        public DateTime idtWithdrawalDate { get; set; }
        public decimal intSequenceNumber { get; set; }
        public int aintAfterWDRLCount { get; set; }

        public int iintNonQualifiedYears { get; set; } //PIR 970

        //10 PERCENT
        public decimal idecCummBalance { get; set; }
        //Ticket#85664
        public bool iblnBridgeServiceFlag { get; set; }

    } 
} 
