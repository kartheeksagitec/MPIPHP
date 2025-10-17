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
	/// Class MPIPHP.CustomDataObjects.cdoPayeeAccount:
	/// Inherited from doPayeeAccount, the class is used to customize the database object doPayeeAccount.
	/// </summary>
    [Serializable]
	public class cdoPayeeAccount : doPayeeAccount
	{
		public cdoPayeeAccount() : base()
		{
		}
        public int iintBenefitApplicationID { get; set; }
        public int iintBenefitCalculationID { get; set; }
        public int iintDROApplicationID { get; set; }
        public int iintDROCalculationID { get; set; }
        public string istrBenefitOption { get; set; }
        public string istrBenefitOptionValue { get; set; }
        //PIR 894
        public string istrOriginalBenefitOptionValue { get; set; }
        public DateTime idtSpouseDateOfDeath { get; set; }
        public int istrBenefitOptionValueData { get; set; }
        public string istrBenefitOptionJNS50 { get; set; }
        public DateTime idtRetireMentDate { get; set; }
        public DateTime idtRTMTDate { get; set; }
        public string istrOrgMPID { get; set; }
        public string istrOrgName { get; set; }
        public string istrPlanCode { get; set; }
        public int iintPlanId { get; set; }

        public DateTime idtMin_Distribution_Date { get; set; }
        public string istrDeathNotificationStatus{ get; set; }
        public decimal idecLastFederalTax { get; set; }
        public decimal idecCurrentStateTax { get; set; }
        public decimal idecCurrentFederalTax { get; set; }
        public decimal idecLastStateTax { get; set; }
        public decimal idecAddFlatFederalTax { get; set; }
        public decimal idecAddFlatStateTax { get; set; }
        public decimal idecOverriddenBenefitAmt { get; set; }
        public string istrCalculationType { get; set; }
        public string istrPrefix { get; set; }
        public string istrLastName { get; set; }
        public string istrAddrLine1 { get; set; }
        public string istrAddrLine2 { get; set; }
        public string istrCity { get; set; }
        public string istrState { get; set; }
        public string istrZipCode { get; set; }
        public string istrStatusDesc { get; set; }
        public string istrStatus { get; set; }
        public string IS_ROLLOVER { get; set; }
        public string istrRelativeVipFlag { get; set; }
        public string istrPercentIncrease { get; set; }
        public string istrChildSupportFlag { get; set; }

        public string istrSavingMode { get; set; }

        public string istrTaxIdentifier { get; set; }
        //rid 80131
        public string istrBenefitDistributionType { get; set; }
        public string istrTaxOption { get; set; }

        public string istrFundType { get; set; }//PIR 969

        public string istrCovidFlag { get; set; } //RequestID: 99406
        public string istrHardshipWithdrawal { get; set; } //RequestID: 99406

        #region Death Notification related

        public string istrPayeeName { get; set; }
        public string istrPayeeAccountCurrentStatus { get; set; }
        public string istrTerminationReason { get; set; }
        public string istrMonthlyPayeeAccountCurrentStatus { get; set; }
        #endregion Death Notification related


        #region Retiree Increase Report
        public string istrMPID { get; set; }
        public string istrParticipantName { get; set; }
        public int intPlanYear { get; set; }
        public string istrPlanDescription { get; set; }
        public string istrMDAge { get; set; }
        public decimal idecStateTax { get; set; }
        public decimal idecFederalTax { get; set; }
        public decimal idecNetAmount { get; set; }
        public string istrRetireeIncreaseEligible { get; set; }
        public string istrRolloverEligible { get; set; }
        public string istrRolloverGroup { get; set; }
        public string istrContactName { get; set; }
        public string istrPaymentMethod { get; set; }
        public string istrPersonType { get; set; }
        public decimal idecRetireeIncAmt { get; set; }
        public decimal idecGrossAmt { get; set; }
        public decimal idecDeductionAmt { get; set; }
        public DateTime idtRolloverCraetedDate { get; set; }
        public DateTime idtMDDate { get; set; }  // PIR RID 71870

        public string istrMaritalStatusValue { get; set; }

        public string istrtax_option_value { get; set; }

        #endregion

        #region  SSA Disability Certification Batch
        public string istrFirsttName { get; set; }
        public decimal idecAge { get; set; }
        #endregion

        public string VALID_ADDR_FLAG { get; set; } //PIR 337

        public string more_information { get; set; }
    } 
} 

