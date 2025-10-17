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
using Sagitec.DataObjects;
using System.Linq;
using Sagitec.CustomDataObjects;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.Sql;

namespace MPIPHP.BusinessObjects
{
    [Serializable]
    public class busBenefitCalculationWithdrawal : busBenefitCalculationHeader
    {
        public bool lblIsNew { get; set; }

        public decimal idecUVHPInterest { get; set; }
        // public decimal idecEEInterest { get; set; }
        public decimal idecEEPlusInterest { get; set; }
        public string istrNewNotes { get; set; }
        //public decimal idecEEContribution { get; set; }
        public decimal idecNonVestedEEContribution { get; set; }
        public decimal idecNonVestedEEInterest { get; set; }
        public decimal idecUVHPContribution { get; set; }
        public decimal idecTotalInterest { get; set; }
        public decimal idecLumpSumBenefitAmount { get; set; }
        public decimal idecTotalEEUVHP { get; set; }
        public string istr60DaysAfterCurrentDt { get; set; }
        public string istrUVHPAmt { get; set; }
        public Collection<busNotes> iclbNotes { get; set; }

        public DateTime idtDayBeforeWidrwlDate { get; set; }

        public busBenefitCalculationHeader lbusBenefitCalculationHeader { get; set; }
        public busBenefitApplication ibusBenefitApplicationRetirement { get; set; }

        public busDocumentProcessCrossref ibusDocumentProcessCrossref { get; set; }

        //for PIR-531
        public string istrChildSupportFlg { get; set; }

        public string istrSubPlanDesc { get; set; }
        public string istrIsSpecialAcnt { get; set; }
        public string istrIsIAPSpecial { get; set; }
        public string istrEmergencyOneTimePaymentFlag { get; set; } //EmergencyOneTimePayment - 03/17/2020

        public override void BeforePersistChanges()
        {
            if (!this.lblIsNew)
            {
                FlushOlderCalculations();
            }

            Setup_Withdrawal_Calculations();

            //this.icdoBenefitCalculationHeader.error_status_id = busConstant.ERROR_STATUS_ID;
            this.icdoBenefitCalculationHeader.error_status_value = busConstant.STATUS_REVIEW;


            base.BeforePersistChanges();
        }

        public override void ValidateHardErrors(utlPageMode aenmPageMode)
        {
            utlError lobjError = null;
            if (this.iarrErrors.IsNull())
                this.iarrErrors = new ArrayList();

            this.EvaluateInitialLoadRules();
            base.ValidateHardErrors(aenmPageMode);

            if (this.icdoBenefitCalculationHeader.retirement_date == DateTime.MinValue)
            {
                lobjError = AddError(5411, " ");
                this.iarrErrors.Add(lobjError);
                return;
            }
            if (this.icdoBenefitCalculationHeader.retirement_date != DateTime.MinValue && icdoBenefitCalculationHeader.retirement_date.Day != 1)
            {
                lobjError = AddError(5441, "");
                this.iarrErrors.Add(lobjError);
            }

            if ((this.icdoBenefitCalculationHeader.iintPlanId == busConstant.IAP_PLAN_ID) && (this.ibusBenefitApplication.iclbEligiblePlans == null || this.ibusBenefitApplication.iclbEligiblePlans.Where(item => item == busConstant.IAP).Count() <= 0))
            {
                lobjError = AddError(5462, "");
                this.iarrErrors.Add(lobjError);

            }

            if ((this.icdoBenefitCalculationHeader.iintPlanId == busConstant.MPIPP_PLAN_ID) && (this.ibusBenefitApplication.iclbEligiblePlans == null || this.ibusBenefitApplication.iclbEligiblePlans.Where(item => item == busConstant.MPIPP).Count() <= 0))
            {
                lobjError = AddError(5461, "");
                this.iarrErrors.Add(lobjError);
            }

        }

        public override void BeforeValidate(Sagitec.Common.utlPageMode aenmPageMode)
        {
            if (this.icdoBenefitCalculationHeader.retirement_date != DateTime.MinValue)
            {

                this.ibusBenefitApplication.icdoBenefitApplication.retirement_date = this.icdoBenefitCalculationHeader.retirement_date;
                this.ibusBenefitApplication.icdoBenefitApplication.withdrawal_date = this.icdoBenefitCalculationHeader.retirement_date;
                this.ibusBenefitApplication.idecAge = busGlobalFunctions.CalculatePersonAgeInDec(this.ibusPerson.icdoPerson.idtDateofBirth, this.icdoBenefitCalculationHeader.retirement_date);
                this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement = busGlobalFunctions.CalculatePersonAgeInDec(this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth, this.icdoBenefitCalculationHeader.retirement_date);
                this.icdoBenefitCalculationHeader.age = this.ibusBenefitApplication.idecAge; //Load the AGE OF THE MAIN HEADER OBJECT AS WELL
                this.icdoBenefitCalculationHeader.idecParticipantFullAge = Math.Floor(this.ibusBenefitApplication.idecAge);
                this.icdoBenefitCalculationHeader.idecSurvivorFullAge = Math.Floor(this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement);
                if (this.icdoBenefitCalculationHeader.ienuObjectState == ObjectState.Insert)
                    lblIsNew = true;
                else
                    lblIsNew = false;

                this.iblnCalcualteUVHPBenefit = this.iblnCalculateIAPBenefit = this.iblnCalculateL161SplAccBenefit = this.iblnCalculateL52SplAccBenefit = this.iblnCalculateMPIPPBenefit = true;

                SetupPreRequisites_WithdrawalCalculations();
            }
            // LoadAllRetirementContributions();

            if (icdoBenefitCalculationHeader.dro_application_id == 0)
            {
                if (!ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).IsNullOrEmpty())
                {
                    LoadAllRetirementContributions(ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault());
                }
                else
                {
                    LoadAllRetirementContributions(null);
                }
            }
            else
            {
                if (!ibusQdroApplication.ibusParticipant.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).IsNullOrEmpty())
                {
                    LoadAllRetirementContributions(ibusQdroApplication.ibusParticipant.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault());
                }
                else
                {
                    LoadAllRetirementContributions(null);
                }
            }

            base.BeforeValidate(aenmPageMode);
        }

        public override void LoadCorresProperties(string astrTemplateName)
        {
            base.LoadCorresProperties(astrTemplateName);

            idtDayBeforeWidrwlDate = icdoBenefitCalculationHeader.retirement_date.AddDays(-1);

            if (astrTemplateName == busConstant.RETIREMENT_APPLICATION_EE_UVHP_WITHDRAWAL ||
                astrTemplateName == busConstant.Withdrawal_EE_UVHP_Packet || astrTemplateName == busConstant.Withdrawal_EE_UVHP_Retirement_Disablity_Packet || astrTemplateName == busConstant.Withdrawal_EE_UVHP_REFUND_LETTER_A
                || astrTemplateName == busConstant.Withdrawal_EE_UVHP_REFUND_LETTER_B || astrTemplateName == busConstant.Withdrawal_EE_UVHP_REFUND_LETTER_C)
            {
                DateTime ldtCurrentDt = System.DateTime.Now.AddDays(60);
                istr60DaysAfterCurrentDt = busGlobalFunctions.ConvertDateIntoDifFormat(ldtCurrentDt);
            }
            //TICKET-68159 AND 68160
            if (astrTemplateName == busConstant.EE_CONTRIBUTIONS_AND_UVHP_WITHDRAWAL_REFUND_COVER_LETTER)
            {
                //Setup_Withdrawal_Calculations();
                DateTime ldtDueDate = icdoBenefitCalculationHeader.retirement_date.AddDays(-1);
                istrDueDate = busGlobalFunctions.ConvertDateIntoDifFormat(ldtDueDate);
                if (this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).Count() > 0)
                {
                    busBenefitCalculationDetail lbusBenefitCalculationDetail = this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault();
                    idecNonVestedEEContribution = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.non_vested_ee_amount;
                    idecUVHPContribution = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.total_uvhp_contribution_amount;
                    idecEEUVHPInterest = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.total_uvhp_interest_amount + lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.non_vested_ee_interest;
                    idecNonVestedEEInterest = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.non_vested_ee_interest;
                    idecEEUVHPTotal = idecNonVestedEEContribution + idecUVHPContribution + idecEEUVHPInterest;

                    if (idecUVHPContribution > 0)
                        istrUVHPAmt = busConstant.FLAG_YES;

                    if (idecNonVestedEEContribution > decimal.Zero && idecUVHPContribution > decimal.Zero)
                    {
                        istrBoth = busConstant.FLAG_YES;
                    }
                    else if (idecNonVestedEEContribution > decimal.Zero)
                    {
                        istrEEFlag = busConstant.FLAG_YES;
                    }
                    else if (idecUVHPContribution > decimal.Zero)
                    {
                        istrUVHPFlag = busConstant.FLAG_YES;
                    }
                }

            }
            if (astrTemplateName == busConstant.EE_CONTRIBUTIONS_AND_UVHP_REFUND_COVER_LETTER || astrTemplateName == busConstant.Withdrawal_EE_UVHP_Packet
                || astrTemplateName == busConstant.Withdrawal_EE_UVHP_Retirement_Disablity_Packet || astrTemplateName == busConstant.Withdrawal_EE_UVHP_Retirement_Disablity_Packet || astrTemplateName == busConstant.Withdrawal_EE_UVHP_REFUND_LETTER_A
                || astrTemplateName == busConstant.Withdrawal_EE_UVHP_REFUND_LETTER_B || astrTemplateName == busConstant.Withdrawal_EE_UVHP_REFUND_LETTER_C)
            {
                //Setup_Withdrawal_Calculations();
                if (this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).Count() > 0)
                {


                    busBenefitCalculationDetail lbusBenefitCalculationDetail = this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault();
                    idecNonVestedEEContribution = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.non_vested_ee_amount;

                    idecUVHPContribution = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.total_uvhp_contribution_amount;
                    idecEEUVHPInterest = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.total_uvhp_interest_amount + lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.non_vested_ee_interest;
                    idecNonVestedEEInterest = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.non_vested_ee_interest;
                   
                    idecEEUVHPTotal = idecNonVestedEEContribution + idecUVHPContribution + idecEEUVHPInterest;

                    if (idecUVHPContribution > 0)
                        istrUVHPAmt = busConstant.FLAG_YES;

                    if (idecNonVestedEEContribution > decimal.Zero && idecUVHPContribution > decimal.Zero)
                    {
                        istrBoth = busConstant.FLAG_YES;
                    }
                    else if (idecNonVestedEEContribution > decimal.Zero)
                    {
                        istrEEFlag = busConstant.FLAG_YES;
                    }
                    else if (idecUVHPContribution > decimal.Zero)
                    {
                        istrUVHPFlag = busConstant.FLAG_YES;
                    }
                }
            }
        }

        public void SetSubPlanDescription(string astrIsIAPSpecial, string astrSubPlan, string astrIsSpecialAcnt)
        {
            istrIsIAPSpecial = astrIsIAPSpecial;
            istrIsSpecialAcnt = astrIsSpecialAcnt;
            istrSubPlanDesc = astrSubPlan;
        }

        public ArrayList btn_ApproveCalculation()
        {

            if (this.iarrErrors.IsNull())
                this.iarrErrors = new ArrayList();

            this.iarrErrors = base.btn_ApproveCalculation();

            if (!this.iarrErrors.IsNullOrEmpty())
                return this.iarrErrors;

            if (this.iarrErrors.Count == 0 && (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL ||
                this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT))
            {
                #region PAYEE ACCOUNT RELATED LOGIC (PAYMENT - SPRINT 3.0) -- Abhishek
                bool lblnDeductPrevIAPBalance = false;
                int flag = 0;
                if (flag != 1)  // DONE ON PURPOSE TO AVOID PAYEE ACCOUNT CODE TO BE EXECUTED FOR NOW
                {
                    //this.ValidateHardErrors(utlPageMode.All);


                    if (this.icdoBenefitCalculationHeader.dro_application_id > 0)
                    {
                        //Benefit Account Related
                        decimal ldecAccountOwnerStartingGrossAmount = 0.0M;
                        decimal ldecAccountOwnerStartingTaxableAmount = 0.0M;
                        decimal ldecAccountOwnerStartingNonTaxableAmount = 0.0M;
                        decimal ldecTotalInterestAmount = 0.0M;

                        #region WithDrawal For AlterNate Payee
                        foreach (busBenefitCalculationDetail lbusBenefitCalculationDetail in this.iclbBenefitCalculationDetail)
                        {
                            if (lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.Count() > 0 && lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.benefit_amount <= Decimal.Zero
                                && !(lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                            icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT))//PIR 985 10262015
                            {
                                utlError lobjError = new utlError();
                                lobjError = AddError(6057, "");//R3view 
                                this.iarrErrors.Add(lobjError);
                            }
                            else if ((lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.Count() > 0 && lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.benefit_amount > Decimal.Zero)
                                || (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                            icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT))//PIR 985 10262015
                            {
                                int lintBenefitAccountID = 0;
                                int lintPayeeAccountID = 0;
                                string lstrFundsType = String.Empty;

                                busPayeeAccount lbusPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
                                busPayeeBenefitAccount lbusPayeeBenefitAccount = new busPayeeBenefitAccount { icdoPayeeBenefitAccount = new cdoPayeeBenefitAccount() };

                                DateTime ldteBenefitBeginDate = this.icdoBenefitCalculationHeader.retirement_date;


                                switch (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.plan_id)
                                {
                                    //R3view - Based on Per Plan we need to set the TAXABLE and NON-TAXABLE ITEMS
                                    case busConstant.MPIPP_PLAN_ID:
                                        if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.ee_flag == busConstant.FLAG_YES)
                                        {
                                            ldecAccountOwnerStartingNonTaxableAmount += lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.non_vested_ee_amount;

                                            //ldecAccountOwnerStartingTaxableAmount += lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.non_vested_ee_interest;
                                            //Prod PIR 243 : As discussed no check required at the time of payee account creation will be handled manually by adding no tax item in tax withholding.
                                            ldecTotalInterestAmount = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.non_vested_ee_interest;

                                            ldecAccountOwnerStartingTaxableAmount = ldecTotalInterestAmount;

                                            ldecAccountOwnerStartingGrossAmount = ldecAccountOwnerStartingNonTaxableAmount + ldecAccountOwnerStartingTaxableAmount;

                                            lstrFundsType = busConstant.FundTypePureEE;
                                        }
                                        if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.uvhp_flag == busConstant.FLAG_YES)
                                        {
                                            ldecAccountOwnerStartingNonTaxableAmount += lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.total_uvhp_contribution_amount;

                                            //ldecAccountOwnerStartingTaxableAmount += lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.total_uvhp_interest_amount;

                                            ldecTotalInterestAmount = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.total_uvhp_interest_amount;
                                            //Prod PIR 243 : As discussed no check required at the time of payee account creation will be handled manually by adding no tax item in tax withholding.
                                            ldecAccountOwnerStartingTaxableAmount = ldecTotalInterestAmount;

                                            ldecAccountOwnerStartingGrossAmount = ldecAccountOwnerStartingNonTaxableAmount + ldecAccountOwnerStartingTaxableAmount;

                                            lstrFundsType = busConstant.FundTypePureUVHP;
                                        }
                                        break;

                                    case busConstant.IAP_PLAN_ID:
                                        //GROSS - IAP ACCOUNT BALANCE  - TILL DATE
                                        if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.l52_spl_acc_flag != busConstant.FLAG_YES && lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.l161_spl_acc_flag != busConstant.FLAG_YES)
                                        {
                                            ldecAccountOwnerStartingTaxableAmount = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.iap_balance_amount + lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.qdro_offset;
                                            ldecAccountOwnerStartingGrossAmount = ldecAccountOwnerStartingTaxableAmount;
                                        }
                                        else if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES)
                                        {

                                            ldecAccountOwnerStartingTaxableAmount = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.local52_special_acct_bal_amount;
                                            ldecAccountOwnerStartingGrossAmount = ldecAccountOwnerStartingTaxableAmount;
                                            lstrFundsType = busConstant.FundTypeLocal52SpecialAccount;
                                        }
                                        else if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES)
                                        {
                                            ldecAccountOwnerStartingTaxableAmount = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.local161_special_acct_bal_amount;
                                            ldecAccountOwnerStartingGrossAmount = ldecAccountOwnerStartingTaxableAmount;
                                            lstrFundsType = busConstant.FundTypeLocal161SpecialAccount;
                                        }
                                        break;

                                    default:
                                        break;

                                }

                                //Benefit Account
                                lintBenefitAccountID = busPayeeAccountHelper.IsBenefitAccountExists(lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.person_account_id,
                                                                                                     busConstant.BENEFIT_TYPE_WITHDRAWAL, lstrFundsType,
                                                                                                     lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.benefit_application_detail_id, 0);  //R3view  the Query and code for this one.

                                lintBenefitAccountID = lbusPayeeBenefitAccount.ManagePayeeBenefitAccount(lintBenefitAccountID, this.ibusQdroApplication.icdoDroApplication.person_id,
                                                                                      lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.person_account_id,
                                                                                      ldecAccountOwnerStartingTaxableAmount, ldecAccountOwnerStartingNonTaxableAmount, ldecAccountOwnerStartingGrossAmount, lstrFundsType);



                                //Payee Account
                                //R3view this code
                                lintPayeeAccountID = busPayeeAccountHelper.IsPayeeAccountExists(this.icdoBenefitCalculationHeader.person_id, lintBenefitAccountID, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_ALTERNATE_PAYEE, busConstant.BENEFIT_TYPE_WITHDRAWAL, false, lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.plan_id,
                                                                                null, 0, null, lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.benefit_application_detail_id);

                                decimal ldecNonTaxableBeginningBalance = 0.0M;

                                if (this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID)
                                {
                                    if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.ee_flag == busConstant.FLAG_YES)
                                    {
                                        ldecNonTaxableBeginningBalance = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.non_vested_ee_amount;
                                    }
                                    else if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.uvhp_flag == busConstant.FLAG_YES)
                                    {
                                        ldecNonTaxableBeginningBalance = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.total_uvhp_contribution_amount;
                                    }
                                }

                                DateTime ldteTermCertainEndDate = new DateTime();
                                string lstrFamilyRelationshipValue = string.Empty;

                                //R3view -- IF Term Year Certain Option FIND the end Date 
                                LoadPlanBenefitsForPlan(this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.plan_id);
                                iintTermCertainMonths = busConstant.ZERO_INT;
                                iintTermCertainMonths = busPayeeAccountHelper.IsTermCertainBenefitOption(this.iclbBenefitCalculationDetail.First().iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.plan_benefit_id, this.iclbcdoPlanBenefit);
                                if (iintTermCertainMonths > 0)
                                {
                                    ldteTermCertainEndDate = ldteBenefitBeginDate.AddMonths(iintTermCertainMonths);
                                    if (ldteTermCertainEndDate != DateTime.MinValue)
                                        ldteTermCertainEndDate = ldteTermCertainEndDate.AddDays(-1);
                                }
                                //Family Relationship value
                                DataTable ldtbRelationshipValue = busBase.Select("cdoRelationship.GetRelationType", new object[2] { this.icdoBenefitCalculationHeader.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id });
                                if (ldtbRelationshipValue.Rows.Count > 0)
                                {
                                    lstrFamilyRelationshipValue = Convert.ToString(ldtbRelationshipValue.Rows[0]["RELATIONSHIP_VALUE"]);
                                }

                                bool lblnAdjustmentPaymentFlag = false;
                                if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.adjustment_iap_payment_flag == busConstant.FLAG_YES ||
                                    lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.adjustment_l52spl_payment_flag == busConstant.FLAG_YES ||
                                    lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.adjustment_l161spl_payment_flag == busConstant.FLAG_YES)
                                {
                                    lblnAdjustmentPaymentFlag = true;
                                }

                                //R3view -- NonTaxable Beginning Balance
                                lintPayeeAccountID = lbusPayeeAccount.ManagePayeeAccount(lintPayeeAccountID, this.icdoBenefitCalculationHeader.person_id, 0,
                                                                                         lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.benefit_application_detail_id,
                                                                                         lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.benefit_calculation_detail_id,
                                                                                         0, 0, lintBenefitAccountID, busConstant.BENEFIT_TYPE_WITHDRAWAL, null,
                                                                                         ldteBenefitBeginDate, DateTime.MinValue, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_ALTERNATE_PAYEE, lstrFamilyRelationshipValue,
                                                                                         0.0M, ldecNonTaxableBeginningBalance, this.iclbBenefitCalculationDetail.First().iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.plan_benefit_id,
                                                                                         ldteTermCertainEndDate, busConstant.FLAG_NO, busConstant.FLAG_NO, lblnAdjustmentPaymentFlag);

                                lbusPayeeAccount.LoadNextBenefitPaymentDate();
                                DateTime ldteNextBenefitPaymentDate = lbusPayeeAccount.idtNextBenefitPaymentDate;//R3vview this once with Vinovin
                                decimal ldecTaxableAmount = 0M;
                                decimal ldecNonTaxableAmount = 0M;

                                //R3view -- First Parameter Should be maybe Retirement or Payment Date Review the Function too 
                                if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.ee_flag == busConstant.FLAG_YES || lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.uvhp_flag == busConstant.FLAG_YES)
                                {
                                    //Prod PIR 243 : As discussed no check required at the time of payee account creation will be handled manually by adding no tax item in tax withholding.

                                    decimal ldecNonTaxableInterest = (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.total_ee_interest_qdro_offset) + (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.total_uvhp_interest_qdro_offset);

                                    busPayeeAccountHelper.CalculateMonthlyPaymentComponents(DateTime.MinValue, lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.benefit_amount,
                                        ref ldecNonTaxableAmount, ref ldecTaxableAmount, lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.istrBenefitOptionDescription.Contains(busConstant.LUMP_SUM_DESCRIPTION) ? ldecNonTaxableBeginningBalance + ldecNonTaxableInterest : decimal.Zero);
                                }
                                else if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID)
                                {
                                    if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.l52_spl_acc_flag != busConstant.FLAG_YES
                                   && lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.l161_spl_acc_flag != busConstant.FLAG_YES)
                                    {
                                        busPayeeAccountHelper.CalculateMonthlyPaymentComponents(DateTime.MinValue, lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.iap_balance_amount,
                                                                                 ref ldecNonTaxableAmount, ref ldecTaxableAmount, 0);
                                    }
                                    else if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES)
                                    {
                                        busPayeeAccountHelper.CalculateMonthlyPaymentComponents(DateTime.MinValue, lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.local52_special_acct_bal_amount,
                                                                                ref ldecNonTaxableAmount, ref ldecTaxableAmount, 0);
                                    }
                                    else if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES)
                                    {
                                        busPayeeAccountHelper.CalculateMonthlyPaymentComponents(DateTime.MinValue, lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.local161_special_acct_bal_amount,
                                                                                ref ldecNonTaxableAmount, ref ldecTaxableAmount, 0);
                                    }
                                }
                                else
                                {
                                    busPayeeAccountHelper.CalculateMonthlyPaymentComponents(DateTime.MinValue, lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.benefit_amount,
                                                             ref ldecNonTaxableAmount, ref ldecTaxableAmount, lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.monthly_exclusion_amount.IsNull() ? 0 : this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.monthly_exclusion_amount);
                                }

                                if (ldecTaxableAmount > 0M)
                                {
                                    if (lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.istrBenefitOptionDescription.Contains(busConstant.LUMP_SUM_DESCRIPTION)
                                    || lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID)                                    
                                        lbusPayeeAccount.CreatePayeeAccountPaymentItemType("ITEM21", ldecTaxableAmount, "0", 0,
                                                                    ldteNextBenefitPaymentDate, DateTime.MinValue, "N", false);                                                                        
                                    else
                                        lbusPayeeAccount.CreatePayeeAccountPaymentItemType("ITEM1", ldecTaxableAmount, "0", 0,
                                                                     ldteNextBenefitPaymentDate, DateTime.MinValue, "N", false);
                                }
                                if (ldecNonTaxableAmount > 0M)
                                {
                                    if (lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.istrBenefitOptionDescription.Contains(busConstant.LUMP_SUM_DESCRIPTION)
                                    || lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID)
                                        lbusPayeeAccount.CreatePayeeAccountPaymentItemType("ITEM22", ldecNonTaxableAmount, "0", 0,
                                                                    ldteNextBenefitPaymentDate, DateTime.MinValue, "N", false);
                                    else
                                        lbusPayeeAccount.CreatePayeeAccountPaymentItemType("ITEM2", ldecNonTaxableAmount, "0", 0,
                                                                    ldteNextBenefitPaymentDate, DateTime.MinValue, "N", false);
                                }


                                //Create Payee Account in Review
                                if (this.icdoBenefitCalculationHeader.calculation_type_value != busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT)
                                    lbusPayeeAccount.CreateReviewPayeeAccountStatus();

                                //Retro Calculation Items to be Created
                                //if (this.icdoBenefitCalculationHeader.payment_date != DateTime.MinValue)
                                //{
                                if (this.icdoBenefitCalculationHeader.retirement_date < ldteNextBenefitPaymentDate && (!lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.istrBenefitOptionDescription.Contains(busConstant.LUMP_SUM_DESCRIPTION))
                                    && lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.plan_id != busConstant.IAP_PLAN_ID &&
                                          this.icdoBenefitCalculationHeader.calculation_type_value != busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT)
                                {
                                    lbusPayeeAccount.CreateRetroPayments(lbusPayeeAccount, ldecTaxableAmount, ldecNonTaxableAmount, ldteNextBenefitPaymentDate, this.icdoBenefitCalculationHeader.retirement_date, lintPayeeAccountID);
                                }
                                //}

                                if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT)
                                {
                                    lbusPayeeAccount.CreateReviewPayeeAccountStatus(ablnFromApprovedCalc: true);// PIR 1055
                                    lbusPayeeAccount.ProcessTaxWithHoldingDetails();
                                }

                                if (this.ibusBaseActivityInstance.IsNotNull())
                                {
                                    if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.ee_flag == busConstant.FLAG_YES && lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.uvhp_flag == busConstant.FLAG_YES)
                                        this.SetWFVariables4PayeeAccount(lintPayeeAccountID, lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.plan_id, true, true);

                                    else if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES)
                                        this.SetWFVariables4PayeeAccount(lintPayeeAccountID, lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.plan_id, false, false, false, true);

                                    else if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES)
                                        this.SetWFVariables4PayeeAccount(lintPayeeAccountID, lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.plan_id, false, false, true);

                                    else
                                        this.SetWFVariables4PayeeAccount(lintPayeeAccountID, lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.plan_id);


                                    this.SetProcessInstanceParameters();
                                }

                            }
                        }
                        #endregion
                    }
                    else
                    {
                        //Benefit Account Related
                        decimal ldecAccountOwnerStartingTaxableAmount = 0.0M;
                        decimal ldecAccountOwnerStartingNonTaxableAmount = 0.0M;
                        decimal ldecAccountOwnerStartingGrossAmount = 0.0M;
                        decimal ldecTotalInterestAmount = 0.0M;

                        #region WithDrawal For Participant Himself
                        foreach (busBenefitCalculationDetail lbusBenefitCalculationDetail in this.iclbBenefitCalculationDetail)
                        {
                            if (lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.Count() > 0 &&
                                lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.benefit_amount <= Decimal.Zero
                                && !(lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                            icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT))//PIR 985 10262015
                            {
                                utlError lobjError = new utlError();
                                lobjError = AddError(6057, "");//R3view 
                                this.iarrErrors.Add(lobjError);
                            }
                            else if ((lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.Count() > 0 && lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.benefit_amount > Decimal.Zero)
                                || (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                            icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT))//PIR 985 10262015
                            {
                                int lintBenefitAccountID = 0;
                                int lintPayeeAccountID = 0;
                                string lstrFundsType = String.Empty;

                                busPayeeAccount lbusPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
                                busPayeeBenefitAccount lbusPayeeBenefitAccount = new busPayeeBenefitAccount { icdoPayeeBenefitAccount = new cdoPayeeBenefitAccount() };

                                DateTime ldteBenefitBeginDate = this.icdoBenefitCalculationHeader.retirement_date;
                                DateTime ldteNextBenefitPaymentDate = busPayeeAccountHelper.GetLastBenefitPaymentDate(lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.plan_id).AddMonths(1);

                                switch (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.plan_id)
                                {
                                    //R3view - Based on Per Plan we need to set the TAXABLE and NON-TAXABLE ITEMS
                                    case busConstant.MPIPP_PLAN_ID:
                                        if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.ee_flag == busConstant.FLAG_YES &&
                                            lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.uvhp_flag == busConstant.FLAG_YES)
                                        {
                                            ldecAccountOwnerStartingNonTaxableAmount += lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.non_vested_ee_amount +
                                                lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.total_uvhp_contribution_amount;

                                            ldecTotalInterestAmount = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.non_vested_ee_interest +
                                            lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.total_uvhp_interest_amount;
                                            //Prod PIR 243 : As discussed no check required at the time of payee account creation will be handled manually by adding no tax item in tax withholding.

                                            ldecAccountOwnerStartingTaxableAmount = ldecTotalInterestAmount;

                                            ldecAccountOwnerStartingGrossAmount = ldecAccountOwnerStartingNonTaxableAmount + ldecAccountOwnerStartingTaxableAmount;

                                            lstrFundsType = busConstant.FundTypeEEandUVHPCombined;
                                        }
                                        break;

                                    case busConstant.IAP_PLAN_ID:
                                        //GROSS - IAP ACCOUNT BALANCE  - TILL DATE
                                        if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.l52_spl_acc_flag != busConstant.FLAG_YES && lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.l161_spl_acc_flag != busConstant.FLAG_YES)
                                        {
                                            ldecAccountOwnerStartingTaxableAmount = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.iap_balance_amount + lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.qdro_offset;
                                            ldecAccountOwnerStartingGrossAmount = ldecAccountOwnerStartingTaxableAmount;
                                        }
                                        else if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES)
                                        {

                                            ldecAccountOwnerStartingTaxableAmount = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.local52_special_acct_bal_amount + lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.qdro_offset;
                                            ldecAccountOwnerStartingGrossAmount = ldecAccountOwnerStartingTaxableAmount;
                                            lstrFundsType = busConstant.FundTypeLocal52SpecialAccount;
                                        }
                                        else if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES)
                                        {
                                            ldecAccountOwnerStartingTaxableAmount = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.local161_special_acct_bal_amount + lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.qdro_offset;
                                            ldecAccountOwnerStartingGrossAmount = ldecAccountOwnerStartingTaxableAmount;
                                            lstrFundsType = busConstant.FundTypeLocal161SpecialAccount;
                                        }
                                        break;

                                    default:
                                        break;

                                }

                                //Benefit Account
                                lintBenefitAccountID = busPayeeAccountHelper.IsBenefitAccountExists(lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.person_account_id,
                                                                                                     busConstant.BENEFIT_TYPE_WITHDRAWAL, lstrFundsType,
                                                                                                     lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.benefit_application_detail_id, 0);  //R3view  the Query and code for this one.

                                lintBenefitAccountID = lbusPayeeBenefitAccount.ManagePayeeBenefitAccount(lintBenefitAccountID, this.icdoBenefitCalculationHeader.person_id,
                                                                                      lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.person_account_id,
                                                                                      ldecAccountOwnerStartingTaxableAmount, ldecAccountOwnerStartingNonTaxableAmount, ldecAccountOwnerStartingGrossAmount, lstrFundsType);

                                //Payee Account
                                //R3view this code
                                lintPayeeAccountID = busPayeeAccountHelper.IsPayeeAccountExists(this.icdoBenefitCalculationHeader.person_id, lintBenefitAccountID, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER, busConstant.BENEFIT_TYPE_WITHDRAWAL, false, lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.plan_id,
                                    this.iclbBenefitCalculationDetail.First().iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.istrBenefitOptionDescription,0,null,lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.benefit_application_detail_id); //10 Percent

                                //START: Ticket 100185 - 10/6/2020.  While Approving an IAP withdrawal calc be sure to prevent associating the calculation with any possible previously created Withdrawal Payee Account (that's tied to a Covid Application)
                                if (lintPayeeAccountID > 0 
                                    && this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID) 
                                {
                                    busPayeeAccount lbusPayeeAccountTemp = null;
                                    lbusPayeeAccountTemp = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
                                    lbusPayeeAccountTemp.FindPayeeAccount(lintPayeeAccountID);
                                    lbusPayeeAccountTemp.LoadBenefitDetails();
                                    if (lbusPayeeAccountTemp.isCOVID19PayFlag == busConstant.FLAG_YES)
                                    {
                                        lintPayeeAccountID = 0;  //Resetting this value to 0 will force a new Payee Account to be created in the ManagePayeeAccount() method below
                                    }
                                }
                                //END: Ticket 100185

                                lblnDeductPrevIAPBalance = false;
                                busPayeeAccount lbusIAPPayeeAccount = null;

                                if (this.icdoBenefitCalculationHeader.lump_sum_payment == busConstant.FLAG_YES)
                                {
                                    DataTable ldtbResult = new DataTable();

                                    ldtbResult = busBase.Select<cdoPayeeAccount>(
                                          new string[5] { "PERSON_ID", "ACCOUNT_RELATION_VALUE", "PAYEE_BENEFIT_ACCOUNT_ID", "BENEFIT_ACCOUNT_TYPE_VALUE", "PLAN_BENEFIT_ID" },
                                          new object[5] { this.icdoBenefitCalculationHeader.person_id, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_BENEFICIARY, lintBenefitAccountID, busConstant.DEATH_PRE_RETIREMENT_MAINTANENCE_FORM, this.iclbBenefitCalculationDetail.First().iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.plan_benefit_id }, null, null);
                                    if (ldtbResult != null && ldtbResult.Rows.Count > 0)
                                    {
                                        busPayeeAccount lbusTempPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
                                        lbusTempPayeeAccount.icdoPayeeAccount.LoadData(ldtbResult.Rows[0]);
                                        lintPayeeAccountID = lbusTempPayeeAccount.icdoPayeeAccount.payee_account_id;
                                    }
                                }
                                else
                                {
                                    //10 Percent
                                    lintPayeeAccountID = busPayeeAccountHelper.IsPayeeAccountExists(this.icdoBenefitCalculationHeader.person_id, lintBenefitAccountID, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER, busConstant.BENEFIT_TYPE_RETIREMENT, false,
                                        this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.plan_id, astrBenefitOption: this.iclbBenefitCalculationDetail.First().iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.istrBenefitOptionDescription, 0, astrRetirementType: this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.benefit_subtype_value, this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.benefit_application_detail_id);//RID 60954
                                }

                                //PIR 985 10222015
                                if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT)
                                {
                                    if (this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID)
                                    {
                                        lbusIAPPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
                                        if (lintPayeeAccountID > 0)
                                        {
                                            lbusIAPPayeeAccount.FindPayeeAccount(lintPayeeAccountID);
                                            //if (lbusIAPPayeeAccount.icdoPayeeAccount.adjustment_payment_eligible_flag == busConstant.FLAG_YES)
                                            //{
                                            //    lblnDeductPrevIAPBalance = true;
                                            //}
                                        }
                                    }
                                }
                                decimal ldecNonTaxableBeginningBalance = 0.0M;

                                if (this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID)
                                {
                                    if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.ee_flag == busConstant.FLAG_YES &&
                                                    lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.uvhp_flag == busConstant.FLAG_YES)
                                    {
                                        this.ibusCalculation.ProcessQDROOffset(lbusBenefitCalculationDetail, this.icdoBenefitCalculationHeader.person_id, ref ldecNonTaxableBeginningBalance, true, true,
                                            false, false, icdoBenefitCalculationHeader.calculation_type_value);

                                        if (ldecAccountOwnerStartingNonTaxableAmount > 0)
                                        {
                                            ldecNonTaxableBeginningBalance = ldecAccountOwnerStartingNonTaxableAmount - lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.idecAlternatePayeePurecontribution;
                                        }
                                    }
                                }

                                DateTime ldteTermCertainEndDate = new DateTime();
                                string lstrFamilyRelationshipValue = string.Empty;

                                //R3view -- IF Term Year Certain Option FIND the end Date 
                                LoadPlanBenefitsForPlan(this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.plan_id);
                                iintTermCertainMonths = busConstant.ZERO_INT;
                                iintTermCertainMonths = busPayeeAccountHelper.IsTermCertainBenefitOption(this.iclbBenefitCalculationDetail.First().iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.plan_benefit_id, this.iclbcdoPlanBenefit);
                                if (iintTermCertainMonths > 0)
                                {
                                    ldteTermCertainEndDate = ldteBenefitBeginDate.AddMonths(iintTermCertainMonths);
                                    if (ldteTermCertainEndDate != DateTime.MinValue)
                                        ldteTermCertainEndDate = ldteTermCertainEndDate.AddDays(-1);
                                }

                                bool lblnAdjustmentPaymentFlag = false;
                                if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.adjustment_iap_payment_flag == busConstant.FLAG_YES ||
                                    lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.adjustment_l52spl_payment_flag == busConstant.FLAG_YES ||
                                    lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.adjustment_l161spl_payment_flag == busConstant.FLAG_YES)
                                {
                                    lblnAdjustmentPaymentFlag = true;
                                }


                                int lintPlanBenefitId = 0;
                                if (this.icdoBenefitCalculationHeader.lump_sum_payment == busConstant.FLAG_YES)
                                {
                                    lintPlanBenefitId = 9;
                                    lintPayeeAccountID = 0;
                                }
                                else
                                {
                                    lintPlanBenefitId = this.iclbBenefitCalculationDetail.First().iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.plan_benefit_id;
                                }
                                //Family Relationship value
                                //DataTable ldtbRelationshipValue = busBase.Select("cdoRelationship.GetRelationType", new object[2] { this.icdoBenefitCalculationHeader.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id });
                                //if (ldtbRelationshipValue.Rows.Count > 0)
                                //{
                                //    lstrFamilyRelationshipValue = Convert.ToString(ldtbRelationshipValue.Rows[0]["RELATIONSHIP_VALUE"]);
                                //}

                                //R3view -- NonTaxable Beginning Balance
                                lintPayeeAccountID = lbusPayeeAccount.ManagePayeeAccount(lintPayeeAccountID, this.icdoBenefitCalculationHeader.person_id, 0,
                                                                                         lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.benefit_application_detail_id,
                                                                                         lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.benefit_calculation_detail_id,
                                                                                         0, 0, lintBenefitAccountID, busConstant.BENEFIT_TYPE_WITHDRAWAL, null,
                                                                                         ldteBenefitBeginDate, DateTime.MinValue, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER, lstrFamilyRelationshipValue,
                                                                                         0.0M, ldecNonTaxableBeginningBalance, lintPlanBenefitId,//lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.plan_benefit_id,
                                                                                         ldteTermCertainEndDate, busConstant.FLAG_NO, busConstant.FLAG_NO, lblnAdjustmentPaymentFlag);

                                decimal ldecBenefitAmount = decimal.Zero;
                                decimal ldecTaxableAmount = 0M;
                                decimal ldecNonTaxableAmount = 0M;

                                decimal ldecTotalGross = decimal.Zero;//PIR 985 10262015

                                //for PIR-531
                                busBenefitApplication lbusBenefitApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
                                lbusBenefitCalculationHeader = new busBenefitCalculationHeader { icdoBenefitCalculationHeader = new cdoBenefitCalculationHeader() };
                                lbusBenefitCalculationHeader.FindBenefitCalculationHeader(lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.benefit_calculation_header_id);
                                lbusBenefitApplication.FindBenefitApplication(lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.benefit_application_id);

                                lbusPayeeAccount.LoadNextBenefitPaymentDate();
                                ldteNextBenefitPaymentDate = lbusPayeeAccount.idtNextBenefitPaymentDate;//R3vview this once with Vinovin
                                //R3view -- First Parameter Should be maybe Retirement or Payment Date Review the Function too 
                                if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.ee_flag == busConstant.FLAG_YES || lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.uvhp_flag == busConstant.FLAG_YES)
                                {
                                    //Prod PIR 243 : As discussed no check required at the time of payee account creation will be handled manually by adding no tax item in tax withholding.

                                    busPayeeAccountHelper.CalculateMonthlyPaymentComponents(DateTime.MinValue, lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.benefit_amount,
                                        ref ldecNonTaxableAmount, ref ldecTaxableAmount, lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.istrBenefitOptionDescription.Contains(busConstant.LUMP_SUM_DESCRIPTION) ? ldecNonTaxableBeginningBalance : decimal.Zero);
                                }
                                else if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID && (lbusBenefitApplication.icdoBenefitApplication.child_support_flag == busConstant.FLAG_YES || lbusBenefitApplication.icdoBenefitApplication.emergency_onetime_payment_flag == busConstant.FLAG_YES || lbusBenefitApplication.icdoBenefitApplication.withdrawal_type_value.IsNotNullOrEmpty())) //for PIR-531 //EmergencyOneTimePayment - 03/17/2020
                                {
                                    //ldecBenefitAmount = this.iclbBenefitCalculationDetail.First().iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.benefit_amount;
                                    ldecBenefitAmount = lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.Where(item => item.icdoBenefitCalculationOptions.benefit_calculation_detail_id == lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.benefit_calculation_detail_id).First().icdoBenefitCalculationOptions.benefit_amount; //EmergencyOneTimePayment - 03/17/2020
                                    if (this.iclbBenefitCalculationDetail.First().iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.overridden_benefit_amount > decimal.Zero)
                                    {
                                        ldecBenefitAmount = this.iclbBenefitCalculationDetail.First().iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.overridden_benefit_amount;                                         
                                    }
                                    busPayeeAccountHelper.CalculateMonthlyPaymentComponents(DateTime.MinValue, ldecBenefitAmount, ref ldecNonTaxableAmount, ref ldecTaxableAmount,
                                        lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.istrBenefitOptionDescription.Contains(busConstant.LUMP_SUM_DESCRIPTION) ? ldecNonTaxableBeginningBalance : decimal.Zero);
                                }
                                else if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID)
                                {

                                    if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.l52_spl_acc_flag != busConstant.FLAG_YES
                                    && lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.l161_spl_acc_flag != busConstant.FLAG_YES)
                                    {
                                        ldecTotalGross = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.iap_balance_amount;
                                        //busPayeeAccountHelper.CalculateMonthlyPaymentComponents(DateTime.MinValue, lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.iap_balance_amount,
                                        //                                      ref ldecNonTaxableAmount, ref ldecTaxableAmount, lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.istrBenefitOptionDescription.Contains(busConstant.LUMP_SUM_DESCRIPTION) ? ldecNonTaxableBeginningBalance : 0);
                                    }
                                    else if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES)
                                    {
                                        ldecTotalGross = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.local52_special_acct_bal_amount;

                                        //busPayeeAccountHelper.CalculateMonthlyPaymentComponents(DateTime.MinValue, lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.local52_special_acct_bal_amount,
                                        //                                      ref ldecNonTaxableAmount, ref ldecTaxableAmount, lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.istrBenefitOptionDescription.Contains(busConstant.LUMP_SUM_DESCRIPTION) ? ldecNonTaxableBeginningBalance : 0);
                                    }
                                    else if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES)
                                    {
                                        ldecTotalGross = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.local161_special_acct_bal_amount;
                                        // busPayeeAccountHelper.CalculateMonthlyPaymentComponents(DateTime.MinValue, lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.local161_special_acct_bal_amount,
                                        //                                        ref ldecNonTaxableAmount, ref ldecTaxableAmount, lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.istrBenefitOptionDescription.Contains(busConstant.LUMP_SUM_DESCRIPTION) ? ldecNonTaxableBeginningBalance : 0);
                                    }
                                    decimal ldecIapPaid = decimal.Zero;
                                    int lintPersonAccountId = 0;
                                    decimal ldecWithHoldingAmount = decimal.Zero;

                                    //GROSS - IAP ACCOUNT BALANCE  - TILL DATE
                                    if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT)
                                    {
                                        #region //PIR 985 10222015
                                        //if (lblnDeductPrevIAPBalance)
                                        //{
                                        //    lintPersonAccountId = this.iclbBenefitCalculationDetail.FirstOrDefault().icdoBenefitCalculationDetail.person_account_id;
                                        //    DataTable ldtbIAPBalancePaid = busBase.Select("cdoPaymentHistoryDistribution.GetAmntPaidForThePlan",
                                        //        new object[2] { lintBenefitAccountID, this.icdoBenefitCalculationHeader.retirement_date });
                                        //    if (ldtbIAPBalancePaid.IsNotNull() && ldtbIAPBalancePaid.Rows.Count > 0)
                                        //    {
                                        //        ldecIapPaid = Convert.ToDecimal(Convert.ToBoolean(ldtbIAPBalancePaid.Rows[0][0].IsDBNull()) ? busConstant.ZERO_DECIMAL : ldtbIAPBalancePaid.Rows[0][0]);
                                        //    }
                                        //}
                                        //else if (lbusIAPPayeeAccount.IsNotNull())
                                        //{
                                        //    DataTable ldtblGetWithheldAmount = busBase.Select("cdoWithholdingInformation.GetTotalWithheldAmount", new object[3] {
                                        //        lbusIAPPayeeAccount.icdoPayeeAccount.payee_account_id,  lbusIAPPayeeAccount.icdoPayeeAccount.benefit_begin_date,
                                        //        DateTime.Now});

                                        //    if (ldtblGetWithheldAmount.Rows.Count > 0)
                                        //    {
                                        //        ldecWithHoldingAmount = Convert.ToDecimal(ldtblGetWithheldAmount.Rows[0]["TAXABLE_WITHHOLD_FLAT_AMOUNT"]) +
                                        //            Convert.ToDecimal(ldtblGetWithheldAmount.Rows[0]["NON_TAXABLE_WITHHOLD_FLAT_AMOUNT"]);
                                        //        if (lbusIAPPayeeAccount.IsNotNull())
                                        //            ibusCalculation.EndDateWithholding(lbusIAPPayeeAccount);
                                        //    }
                                        //}
                                        #endregion //PIR 985 10222015
                                    }
                                    ldecTotalGross = ldecTotalGross - ldecIapPaid + ldecWithHoldingAmount;
                                    if (ldecTotalGross > decimal.Zero)
                                    {
                                        busPayeeAccountHelper.CalculateMonthlyPaymentComponents(DateTime.MinValue, ldecTotalGross,
                                                                              ref ldecNonTaxableAmount, ref ldecTaxableAmount, lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.istrBenefitOptionDescription.Contains(busConstant.LUMP_SUM_DESCRIPTION) ? ldecNonTaxableBeginningBalance : 0);
                                    }
                                    else
                                    {
                                        if (lbusIAPPayeeAccount.IsNotNull())
                                        {
                                            //Create Overpayment.
                                            lbusPayeeAccount.CreateOverPayments(lbusPayeeAccount, lbusPayeeAccount.idtNextBenefitPaymentDate, DateTime.MinValue, DateTime.MinValue, -ldecTotalGross, decimal.Zero,
                                                busConstant.RETRO_PAYMENT_ADJUSTMENT_BATCH);
                                        }
                                    }
                                }
                                else
                                {
                                    busPayeeAccountHelper.CalculateMonthlyPaymentComponents(DateTime.MinValue, lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.benefit_amount,
                                                                ref ldecNonTaxableAmount, ref ldecTaxableAmount, lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.monthly_exclusion_amount.IsNull() ? 0 : this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.monthly_exclusion_amount);
                                }

                                if (ldecTaxableAmount > 0M)
                                {
                                    if (lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.istrBenefitOptionDescription.Contains(busConstant.LUMP_SUM_DESCRIPTION)
                                   || lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID)
                                    {
                                        lbusPayeeAccount.CreatePayeeAccountPaymentItemType("ITEM21", ldecTaxableAmount, "0", 0,
                                                                    ldteNextBenefitPaymentDate, DateTime.MinValue, "N", false);

                                        //EmergencyOneTimePayment - 03/17/2020
                                        if (lbusBenefitApplication.icdoBenefitApplication.emergency_onetime_payment_flag == busConstant.FLAG_YES || lbusBenefitApplication.icdoBenefitApplication.withdrawal_type_value.IsNotNullOrEmpty())
                                        {
                                            lbusPayeeAccount.CreateFedStateTaxForIAPLumpSumPayment(ldecTaxableAmount, lbusBenefitApplication.icdoBenefitApplication.covid_federal_tax_percentage, lbusBenefitApplication.icdoBenefitApplication.covid_state_tax_percentage, ldteNextBenefitPaymentDate);
                                        }
                                    }
                                    else
                                        lbusPayeeAccount.CreatePayeeAccountPaymentItemType("ITEM1", ldecTaxableAmount, "0", 0,
                                                                    ldteNextBenefitPaymentDate, DateTime.MinValue, "N", false);
                                }
                                if (ldecNonTaxableAmount > 0M)
                                {
                                    if (lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.istrBenefitOptionDescription.Contains(busConstant.LUMP_SUM_DESCRIPTION)
                                   || lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID)
                                        lbusPayeeAccount.CreatePayeeAccountPaymentItemType("ITEM22", ldecNonTaxableAmount, "0", 0,
                                                                    ldteNextBenefitPaymentDate, DateTime.MinValue, "N", false);
                                    else
                                        lbusPayeeAccount.CreatePayeeAccountPaymentItemType("ITEM2", ldecNonTaxableAmount, "0", 0,
                                                                    ldteNextBenefitPaymentDate, DateTime.MinValue, "N", false);
                                }


                                //Create Payee Account in Review
                                if (this.icdoBenefitCalculationHeader.calculation_type_value != busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT)
                                    lbusPayeeAccount.CreateReviewPayeeAccountStatus();

                                //Retro Calculation Items to be Created
                                if (this.icdoBenefitCalculationHeader.retirement_date < ldteNextBenefitPaymentDate && (!lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.istrBenefitOptionDescription.Contains(busConstant.LUMP_SUM_DESCRIPTION))
                                    && lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.plan_id != busConstant.IAP_PLAN_ID &&
                            this.icdoBenefitCalculationHeader.calculation_type_value != busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT)
                                {
                                    lbusPayeeAccount.CreateRetroPayments(lbusPayeeAccount, ldecTaxableAmount, ldecNonTaxableAmount, ldteNextBenefitPaymentDate, this.icdoBenefitCalculationHeader.retirement_date, lintPayeeAccountID);
                                }

                                if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT)
                                {
                                    //PIR 985 10262015
                                    if (this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                                        ldecTotalGross < 0)
                                        ;
                                    else
                                        lbusPayeeAccount.CreateReviewPayeeAccountStatus(ablnFromApprovedCalc: true);// PIR 1055

                                    lbusPayeeAccount.ProcessTaxWithHoldingDetails();
                                }


                                if (this.ibusBaseActivityInstance.IsNotNull())
                                {
                                    if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.ee_flag == busConstant.FLAG_YES && lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.uvhp_flag == busConstant.FLAG_YES)
                                        this.SetWFVariables4PayeeAccount(lintPayeeAccountID, lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.plan_id, true, true);

                                    else if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES)
                                        this.SetWFVariables4PayeeAccount(lintPayeeAccountID, lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.plan_id, false, false, false, true);

                                    else if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES)
                                        this.SetWFVariables4PayeeAccount(lintPayeeAccountID, lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.plan_id, false, false, true);

                                    else
                                        this.SetWFVariables4PayeeAccount(lintPayeeAccountID, lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.plan_id);


                                    this.SetProcessInstanceParameters();
                                }
                            }
                        }
                        #endregion
                    }
                }



                #endregion

                if (this.icdoBenefitCalculationHeader.dro_application_id == 0)
                {
                    #region TO POST PARTIAL INTEREST AND  QUATERLY IAP ALLOCATIONS(Remaining) IN THE RETIREMENT CONTRIBUTION TABLE
                    foreach (busBenefitCalculationDetail lbusBenefitCalculationDetail in this.iclbBenefitCalculationDetail)
                    {
                        if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID)
                        {
                            int lintPersonAccountId = 0;
                            if (this.icdoBenefitCalculationHeader.dro_application_id > 0)
                            {
                                lintPersonAccountId = this.ibusQdroApplication.ibusParticipant.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().icdoPersonAccount.person_account_id;
                            }
                            else
                            {
                                lintPersonAccountId = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().icdoPersonAccount.person_account_id;
                            }


                            // Insert the Partial Interest for EE Contributions in the SGT_PERSON_RETIREMENT_CONTRIBUTION table
                            if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.ee_flag == busConstant.FLAG_YES)
                            {
                                decimal ldecPartialEEInterestAmount = 0, ldecPartialUVHPInterestAmount = 0;
                                decimal ldecPriorYearEEInterest = decimal.Zero;
                                decimal ldecPriorYearUVHPInterest = decimal.Zero;
                                if (this.icdoBenefitCalculationHeader.dro_application_id > 0)
                                {
                                    ldecPartialEEInterestAmount = ibusCalculation.CalculatePartialEEInterest(this.icdoBenefitCalculationHeader.retirement_date,
                                                                                 this.ibusQdroApplication.ibusParticipant.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault(),
                                                                                 false, true, iclbPersonAccountRetirementContribution, out ldecPriorYearEEInterest);
                                    ldecPartialUVHPInterestAmount = ibusCalculation.CalculatePartialUVHPInterest(this.icdoBenefitCalculationHeader.retirement_date, lintPersonAccountId, out ldecPriorYearUVHPInterest);
                                }
                                else
                                {
                                    ldecPartialEEInterestAmount = ibusCalculation.CalculatePartialEEInterest(this.icdoBenefitCalculationHeader.retirement_date,
                                                                               this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault(),
                                                                               false, true, iclbPersonAccountRetirementContribution, out ldecPriorYearEEInterest);
                                    ldecPartialUVHPInterestAmount = ibusCalculation.CalculatePartialUVHPInterest(this.icdoBenefitCalculationHeader.retirement_date, lintPersonAccountId, out ldecPriorYearUVHPInterest);

                                }
                                busPersonAccountRetirementContribution lbusPersonAccountRetirementContribution = null;
                                if (ldecPriorYearEEInterest > decimal.Zero)
                                {
                                    lbusPersonAccountRetirementContribution = new busPersonAccountRetirementContribution() { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };
                                    lbusPersonAccountRetirementContribution.InsertPersonAccountRetirementContirbution(lintPersonAccountId, busGlobalFunctions.GetLastDateOfComputationYear(this.icdoBenefitCalculationHeader.retirement_date.Year - 1).AddDays(1),
                                        DateTime.Now, this.icdoBenefitCalculationHeader.retirement_date.AddYears(-1).Year, adecEEInterestAmount: ldecPriorYearEEInterest, astrTransactionType: busConstant.TRANSACTION_TYPE_INTEREST,
                                        astrContributionType: busConstant.CONTRIBUTION_TYPE_EE, astrContributionSubtype: busConstant.CONTRIBUTION_SUBTYPE_NON_VESTED, aintReferenceID: this.icdoBenefitCalculationHeader.benefit_calculation_header_id);

                                }
                                if (ldecPartialEEInterestAmount - ldecPriorYearEEInterest > decimal.Zero)
                                {
                                    lbusPersonAccountRetirementContribution = new busPersonAccountRetirementContribution() { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };
                                    lbusPersonAccountRetirementContribution.InsertPersonAccountRetirementContirbution(lintPersonAccountId, busGlobalFunctions.GetLastDateOfComputationYear(this.icdoBenefitCalculationHeader.retirement_date.Year - 1).AddDays(1),
                                        DateTime.Now, this.icdoBenefitCalculationHeader.retirement_date.Year, adecEEInterestAmount: ldecPartialEEInterestAmount - ldecPriorYearEEInterest, astrTransactionType: busConstant.TRANSACTION_TYPE_INTEREST,
                                        astrContributionType: busConstant.CONTRIBUTION_TYPE_EE, astrContributionSubtype: busConstant.CONTRIBUTION_SUBTYPE_NON_VESTED, aintReferenceID: this.icdoBenefitCalculationHeader.benefit_calculation_header_id);
                                }
                                if (ldecPriorYearUVHPInterest > decimal.Zero)
                                {
                                    lbusPersonAccountRetirementContribution = new busPersonAccountRetirementContribution() { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };
                                    lbusPersonAccountRetirementContribution.InsertPersonAccountRetirementContirbution(lintPersonAccountId, busGlobalFunctions.GetLastDateOfComputationYear(this.icdoBenefitCalculationHeader.retirement_date.Year - 1).AddDays(1),
    DateTime.Now, this.icdoBenefitCalculationHeader.retirement_date.AddYears(-1).Year, adecUVHPInterestAmount: ldecPriorYearUVHPInterest, astrTransactionType: busConstant.TRANSACTION_TYPE_INTEREST,
    astrContributionType: busConstant.CONTRIBUTION_TYPE_UVHP, aintReferenceID: this.icdoBenefitCalculationHeader.benefit_calculation_header_id);


                                }
                                if (ldecPartialUVHPInterestAmount - ldecPriorYearUVHPInterest > decimal.Zero)
                                {
                                    lbusPersonAccountRetirementContribution = new busPersonAccountRetirementContribution() { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };

                                    lbusPersonAccountRetirementContribution.InsertPersonAccountRetirementContirbution(lintPersonAccountId, busGlobalFunctions.GetLastDateOfComputationYear(this.icdoBenefitCalculationHeader.retirement_date.Year - 1).AddDays(1),
                                        DateTime.Now, this.icdoBenefitCalculationHeader.retirement_date.Year, adecUVHPInterestAmount: ldecPartialUVHPInterestAmount - ldecPriorYearUVHPInterest, astrTransactionType: busConstant.TRANSACTION_TYPE_INTEREST,
                                        astrContributionType: busConstant.CONTRIBUTION_TYPE_UVHP, aintReferenceID: this.icdoBenefitCalculationHeader.benefit_calculation_header_id);
                                }
                            }

                            // Insert the Partial Interest for UV & HP Contributions in the SGT_PERSON_RETIREMENT_CONTRIBUTION table
                            else if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.uvhp_flag == busConstant.FLAG_YES)
                            {
                                decimal ldecPriorYearUVHPInterest = decimal.Zero;
                                decimal ldecPartialUVHPInterestAmount = ibusCalculation.CalculatePartialUVHPInterest(this.icdoBenefitCalculationHeader.retirement_date, lintPersonAccountId, out ldecPriorYearUVHPInterest);

                                if (ldecPartialUVHPInterestAmount - ldecPriorYearUVHPInterest > decimal.Zero)
                                {
                                    busPersonAccountRetirementContribution lbusPersonAccountRetirementContribution = new busPersonAccountRetirementContribution() { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };
                                    lbusPersonAccountRetirementContribution.InsertPersonAccountRetirementContirbution(lintPersonAccountId, busGlobalFunctions.GetLastDateOfComputationYear(this.icdoBenefitCalculationHeader.retirement_date.Year - 1).AddDays(1),
                                        DateTime.Now, this.icdoBenefitCalculationHeader.retirement_date.Year, adecUVHPInterestAmount: ldecPartialUVHPInterestAmount - ldecPriorYearUVHPInterest, astrTransactionType: busConstant.TRANSACTION_TYPE_INTEREST,
                                        astrContributionType: busConstant.CONTRIBUTION_TYPE_UVHP, aintReferenceID: this.icdoBenefitCalculationHeader.benefit_calculation_header_id);
                                }
                                if (ldecPriorYearUVHPInterest > decimal.Zero)
                                {
                                    busPersonAccountRetirementContribution lbusPersonAccountRetirementContribution = new busPersonAccountRetirementContribution() { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };
                                    lbusPersonAccountRetirementContribution.InsertPersonAccountRetirementContirbution(lintPersonAccountId, busGlobalFunctions.GetLastDateOfComputationYear(this.icdoBenefitCalculationHeader.retirement_date.Year - 1).AddDays(1),
                                        DateTime.Now, this.icdoBenefitCalculationHeader.retirement_date.Year, adecUVHPInterestAmount: ldecPriorYearUVHPInterest, astrTransactionType: busConstant.TRANSACTION_TYPE_INTEREST,
                                        astrContributionType: busConstant.CONTRIBUTION_TYPE_UVHP, aintReferenceID: this.icdoBenefitCalculationHeader.benefit_calculation_header_id);

                                }
                            }
                        }
                    }


                    if (this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).Count() > 0)
                    {
                        bool lblnInsert = true;
                        //PIR 534: OPUS should not be push quarterly allocations and RY allocations when approving Adjustment Calc.
                        //if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT && !lblnDeductPrevIAPBalance)

                        busBenefitApplication lbusBenefitApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
                        lbusBenefitApplication.FindBenefitApplication(this.icdoBenefitCalculationHeader.benefit_application_id);
                        //Ticket #102753: Besides preventing quarterly allocations for Adjustment Calculations also prevent quarterly allocations for Covid-19 IAP withdrawals
                        if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT || lbusBenefitApplication.icdoBenefitApplication.emergency_onetime_payment_flag == busConstant.FLAG_YES || lbusBenefitApplication.icdoBenefitApplication.withdrawal_type_value.IsNotNullOrEmpty() )
                        {
                            lblnInsert = false;
                        }
                        if (lblnInsert)
                        {
                            int lintPersonAccountId = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().icdoPersonAccount.person_account_id;

                            if ((from item in this.iclbPersonAccountRetirementContribution
                                 where item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId
                                 select item.icdoPersonAccountRetirementContribution.iap_balance_amount).Sum() > 0
                                || (from item in this.iclbPersonAccountRetirementContribution
                                    where item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId
                                    select item.icdoPersonAccountRetirementContribution.local52_special_acct_bal_amount).Sum() > 0
                                || (from item in this.iclbPersonAccountRetirementContribution
                                    where item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId
                                    select item.icdoPersonAccountRetirementContribution.local161_special_acct_bal_amount).Sum() > 0)
                            {

                                DataTable ldtlbQRDOOffset = Select("cdoQdroCalculationHeader.CheckIfDROApplicationorPendingFinalCalc", new object[2] { this.ibusPerson.icdoPerson.person_id, busConstant.IAP_PLAN_ID });

                                if (ldtlbQRDOOffset.Rows.Count > 0)
                                {
                                    utlError lobjError = new utlError();
                                    lobjError.istrErrorMessage = "UNAPPROVED FINAL QDRO Calculation Exists, Please Approve QDRO Calculation for IAP First";
                                    this.iarrErrors.Add(lobjError);
                                }
                                else
                                {
                                    this.ibusCalculation.InsertIAPQuarterlyAllocations(this.iclbBenefitCalculationDetail, this);
                                }
                            }
                        }
                    }

                    #endregion
                }
            }

            return this.iarrErrors;
        }

        public ArrayList btn_CancelCalculation()
        {
            decimal ldecPartialEEInterestAmount = 0.0M;
            decimal ldecPartialUVHPInterestAmount = 0.0M;

            base.btn_CancelCalculation();
            if (this.iarrErrors.IsNull())
                this.iarrErrors = new ArrayList();

            if (this.iarrErrors.Count == 0) //PROD PIR 792
            {
                if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL)
                {
                    foreach (busBenefitCalculationDetail lbusBenefitCalculationDetail in this.iclbBenefitCalculationDetail)
                    {
                        if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID)
                        {
                            int lintPersonAccountId = 0;
                            if (this.icdoBenefitCalculationHeader.dro_application_id > 0)
                            {
                                lintPersonAccountId = this.ibusQdroApplication.ibusParticipant.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().icdoPersonAccount.person_account_id;
                            }
                            else
                            {
                                lintPersonAccountId = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().icdoPersonAccount.person_account_id;
                            }
                            // Insert the Partial Interest for EE Contributions in the SGT_PERSON_RETIREMENT_CONTRIBUTION table
                            if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.ee_flag == busConstant.FLAG_YES)
                            {
                                DataTable ltblEEPartialInterest = Select("cdoPersonAccountRetirementContribution.GetEE&UVHPPartialInterest", new object[2] {this.icdoBenefitCalculationHeader.benefit_calculation_header_id,
                                        busConstant.CONTRIBUTION_TYPE_EE});
                                if (ltblEEPartialInterest.Rows.Count > 0 && Convert.ToString(ltblEEPartialInterest.Rows[0][0]).IsNotNullOrEmpty())
                                {
                                    ldecPartialEEInterestAmount = -(Convert.ToDecimal(ltblEEPartialInterest.Rows[0][0]));
                                    busPersonAccountRetirementContribution lbusPersonAccountRetirementContribution = new busPersonAccountRetirementContribution() { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };
                                    lbusPersonAccountRetirementContribution.InsertPersonAccountRetirementContirbution(lintPersonAccountId, busGlobalFunctions.GetLastDateOfComputationYear(this.icdoBenefitCalculationHeader.retirement_date.Year - 1).AddDays(1),
                                        DateTime.Now, this.icdoBenefitCalculationHeader.retirement_date.Year, adecEEInterestAmount: ldecPartialEEInterestAmount,
                                       // astrTransactionType: busConstant.TRANSACTION_TYPE_CANCELLED_CALCULATION
                                       astrTransactionType: Convert.ToString(ltblEEPartialInterest.Rows[0][1])
                                        , astrContributionType: busConstant.CONTRIBUTION_TYPE_EE, astrContributionSubtype: busConstant.CONTRIBUTION_SUBTYPE_NON_VESTED, aintReferenceID: this.icdoBenefitCalculationHeader.benefit_calculation_header_id);
                                }
                            }

                            // Insert the Partial Interest for UV & HP Contributions in the SGT_PERSON_RETIREMENT_CONTRIBUTION table
                            if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.uvhp_flag == busConstant.FLAG_YES)
                            {
                                DataTable ltblUVHPPartialInterest = Select("cdoPersonAccountRetirementContribution.GetEE&UVHPPartialInterest", new object[2] {this.icdoBenefitCalculationHeader.benefit_calculation_header_id,
                                        busConstant.CONTRIBUTION_TYPE_UVHP});
                                if (ltblUVHPPartialInterest.Rows.Count > 0 && Convert.ToString(ltblUVHPPartialInterest.Rows[0][0]).IsNotNullOrEmpty())
                                {
                                    ldecPartialUVHPInterestAmount = -(Convert.ToDecimal(ltblUVHPPartialInterest.Rows[0][0]));
                                    busPersonAccountRetirementContribution lbusPersonAccountRetirementContribution = new busPersonAccountRetirementContribution() { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };
                                    lbusPersonAccountRetirementContribution.InsertPersonAccountRetirementContirbution(lintPersonAccountId, busGlobalFunctions.GetLastDateOfComputationYear(this.icdoBenefitCalculationHeader.retirement_date.Year - 1).AddDays(1),
                                        DateTime.Now, this.icdoBenefitCalculationHeader.retirement_date.Year, adecUVHPInterestAmount: ldecPartialUVHPInterestAmount,
                                         //astrTransactionType: busConstant.TRANSACTION_TYPE_CANCELLED_CALCULATION
                                         astrTransactionType: Convert.ToString(ltblUVHPPartialInterest.Rows[0][1])
                                        , astrContributionType: busConstant.CONTRIBUTION_TYPE_UVHP, aintReferenceID: this.icdoBenefitCalculationHeader.benefit_calculation_header_id);
                                }
                            }
                        }
                    }
                }
            }
            return this.iarrErrors;
        }


        public ArrayList btn_RefreshCalculation()
        {
            ArrayList larrList = new ArrayList();

            #region FLAGS
            //Flags to be used for making sure we donot calculate again if another IAP entry comes along

            this.iblnCalcualteUVHPBenefit = this.iblnCalculateIAPBenefit = this.iblnCalculateL161SplAccBenefit = this.iblnCalculateL52SplAccBenefit = this.iblnCalculateMPIPPBenefit = false;

            #endregion

            this.icdoBenefitCalculationHeader.iintPlanId = this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.plan_id;

            if (this.iclbBenefitCalculationDetail.Count() > 0)
            {
                this.icdoBenefitCalculationHeader.iintPlanId = this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.plan_id;
            }

            if (!lblIsNew)
                FlushOlderCalculations();

            ibusBenefitApplication = new busBenefitApplication();
            if (this.ibusBenefitApplication.FindBenefitApplication(this.icdoBenefitCalculationHeader.benefit_application_id))
            {
                this.ibusBenefitApplication.iclbBenefitApplicationDetail = new Collection<busBenefitApplicationDetail>();
                this.iclbPersonAccountRetirementContribution = new Collection<busPersonAccountRetirementContribution>();

                this.ibusBenefitApplication.LoadBenefitApplicationDetails();

                ibusBenefitApplication.ibusQdroApplication = this.ibusQdroApplication;

                ibusBenefitApplication.icdoBenefitApplication.retirement_date = this.ibusBenefitApplication.icdoBenefitApplication.withdrawal_date;
                if (ibusBenefitApplication.icdoBenefitApplication.dro_application_id > 0)
                {
                    ibusBenefitApplication.ibusQdroApplication.LoadBenefitDetails();
                    ibusBenefitApplication.ibusPerson = this.ibusQdroApplication.ibusParticipant;
                    ibusBenefitApplication.ibusPerson.iclbPersonAccount = this.ibusQdroApplication.ibusParticipant.iclbPersonAccount;
                    if (!ibusQdroApplication.ibusParticipant.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).IsNullOrEmpty())
                    {
                        iclbPersonAccountRetirementContribution =
                            LoadAllRetirementContributions(ibusQdroApplication.icdoDroApplication.person_id, ibusQdroApplication.ibusParticipant.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault());
                    }
                    else
                    {
                        iclbPersonAccountRetirementContribution = LoadAllRetirementContributions(ibusQdroApplication.icdoDroApplication.person_id, null);
                    }
                }
                else
                {
                    ibusBenefitApplication.ibusPerson = this.ibusPerson;
                    ibusBenefitApplication.ibusPerson.iclbPersonAccount = this.ibusPerson.iclbPersonAccount;

                    if (!ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).IsNullOrEmpty())
                    {
                        LoadAllRetirementContributions(ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault());
                    }
                    else
                    {
                        LoadAllRetirementContributions(null);
                    }
                }

                icdoBenefitCalculationHeader.age = busGlobalFunctions.CalculatePersonAgeInDec(this.ibusPerson.icdoPerson.idtDateofBirth, this.ibusBenefitApplication.icdoBenefitApplication.withdrawal_date);
                ibusBenefitApplication.LoadandProcessWorkHistory_ForAllPlans();

                SetupPreRequisites_WithdrawalCalculations();

                if (!this.ibusBenefitApplication.iclbBenefitApplicationDetail.IsNullOrEmpty())
                {
                    foreach (busBenefitApplicationDetail lbusBenefitApplicationDetail in this.ibusBenefitApplication.iclbBenefitApplicationDetail)
                    {
                        if (lbusBenefitApplicationDetail.iintPlan_ID != this.icdoBenefitCalculationHeader.iintPlanId)
                            continue;

                        this.iblnCalcualteUVHPBenefit = this.iblnCalcualteNonVestedEEBenefit = this.iblnCalculateIAPBenefit = this.iblnCalculateL161SplAccBenefit = this.iblnCalculateL52SplAccBenefit = this.iblnCalculateMPIPPBenefit = false;

                        if (lbusBenefitApplicationDetail.iintPlan_ID == busConstant.IAP_PLAN_ID && this.icdoBenefitCalculationHeader.iintPlanId == busConstant.IAP_PLAN_ID)
                        {
                            #region IAP PLAN FOUND IN GRID
                            if (lbusBenefitApplicationDetail.istrSubPlan.IsNullOrEmpty() && !this.iblnCalculateIAPBenefit)
                            {
                                this.iblnCalculateIAPBenefit = true;

                            }
                            else if (lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.l52_spl_acc_flag == busConstant.FLAG_YES)
                            {
                                this.iblnCalculateL52SplAccBenefit = true;
                            }
                            else if (lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.l161_spl_acc_flag == busConstant.FLAG_YES)
                            {
                                this.iblnCalculateL161SplAccBenefit = true;
                            }

                            if (lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue.IsNullOrEmpty() && lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrSubPlanBenefitOptionValue.IsNotNullOrEmpty())
                            {
                                lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue = lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrSubPlanBenefitOptionValue;
                            }

                            #endregion
                        }
                        else if (lbusBenefitApplicationDetail.iintPlan_ID == busConstant.MPIPP_PLAN_ID && this.icdoBenefitCalculationHeader.iintPlanId == busConstant.MPIPP_PLAN_ID)
                        {
                            #region MPIPP PLAN FOUNd in GRID
                            //if (lbusBenefitApplicationDetail.istrSubPlan.IsNullOrEmpty() && !this.iblnCalculateMPIPPBenefit)
                            //{
                            //    this.iblnCalculateMPIPPBenefit = true;

                            //}
                            if (ibusBenefitApplication.icdoBenefitApplication.dro_application_id > 0)
                            {
                                if (lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.uvhp_flag == busConstant.FLAG_YES)
                                {
                                    this.iblnCalcualteUVHPBenefit = true;
                                }
                                else if (lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.ee_flag == busConstant.FLAG_YES)
                                {
                                    this.iblnCalcualteNonVestedEEBenefit = true;
                                }
                            }
                            else
                            {
                                this.iblnCalcualteUVHPBenefit = true;
                                this.iblnCalcualteNonVestedEEBenefit = true;
                            }

                            #endregion

                        }

                        #region Execute Spawn Final method
                        if (lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue.IsNullOrEmpty() && lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrSubPlanBenefitOptionValue.IsNotNullOrEmpty())
                        {
                            lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue = lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrSubPlanBenefitOptionValue;
                        }

                        if (ibusQdroApplication == null)
                        {
                            this.SpawnFinalWithdrawalCalculation(lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.benefit_application_detail_id,
                                                                                                this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == lbusBenefitApplicationDetail.istrPlanCode).FirstOrDefault().icdoPersonAccount.person_account_id,
                                                                                                lbusBenefitApplicationDetail.iintPlan_ID, lbusBenefitApplicationDetail.istrPlanCode,
                                                                                                lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue);
                        }
                        else
                        {
                            this.SpawnFinalWithdrawalCalculationForAlternatePayee(ibusQdroApplication, lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.benefit_application_detail_id,
                                    this.ibusQdroApplication.ibusParticipant.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == lbusBenefitApplicationDetail.istrPlanCode).FirstOrDefault().icdoPersonAccount.person_account_id,
                                                                                                lbusBenefitApplicationDetail.iintPlan_ID, lbusBenefitApplicationDetail.istrPlanCode,
                                                                                                lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue,
                                                                                                lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.dro_model_value);
                        }

                        #endregion

                    }

                    try
                    {
                        this.AfterPersistChanges();
                    }
                    catch
                    {
                    }

                }

            }
            LoadBenefitCalculationDetails();
            this.EvaluateInitialLoadRules();
            if (this.iclbBenefitCalculationDetail.Count() > 0)
            {
                this.iclbBenefitCalculationDetail.First().EvaluateInitialLoadRules();
                this.iclbBenefitCalculationDetail.First().iobjMainCDO = this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail;
            }
            this.ValidateHardErrors(utlPageMode.Update);

            larrList.Add(this);
            return larrList;
        }


        public void SetupPreRequisites_WithdrawalCalculations()
        {
            if (!this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.IsNullOrEmpty())
            {
                switch (this.icdoBenefitCalculationHeader.benefit_type_value)
                {
                    case busConstant.BENEFIT_TYPE_WITHDRAWAL:
                        this.ibusBenefitApplication.LoadWorkHistoryandSetupPrerequisites_Withdrawal();
                        break;
                    default:
                        this.ibusBenefitApplication.LoadWorkHistoryandSetupPrerequisites_Withdrawal();
                        break;
                }

            }
        }

        public void Setup_Withdrawal_Calculations()
        {
            DateTime ldtVestedDate = DateTime.MinValue;
            decimal ldecPartialUVHPInterest = 0;
            decimal ldecPartialEEInterest = 0;

            #region SETUP BENEFIT CALCULATION DETAIL BASED ON WHAT ESTIMATE HAS BEEN ASKED FOR

            if (this.iclbBenefitCalculationDetail == null)
            {
                this.iclbBenefitCalculationDetail = new Collection<busBenefitCalculationDetail>();
            }

            if (this.icdoBenefitCalculationHeader.iintPlanId == busConstant.IAP_PLAN_ID)
            {
                #region IAP Benefit Details

                if (this.ibusBenefitApplication.iclbEligiblePlans != null && this.ibusBenefitApplication.iclbEligiblePlans.Where(item => item == busConstant.IAP).Count() > 0)
                {
                    ldtVestedDate = DateTime.MinValue;
                    busBenefitCalculationDetail lbusBenefitCalculationDetailIAP = new busBenefitCalculationDetail { icdoBenefitCalculationDetail = new cdoBenefitCalculationDetail() };
                    lbusBenefitCalculationDetailIAP.iobjMainCDO = lbusBenefitCalculationDetailIAP.icdoBenefitCalculationDetail;
                    if (this.ibusBenefitApplication.CheckAlreadyVested(busConstant.IAP))
                    {
                        ldtVestedDate = this.ibusBenefitApplication.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date;
                    }

                    lbusBenefitCalculationDetailIAP.LoadData(this.icdoBenefitCalculationHeader.benefit_calculation_header_id, busConstant.IAP_PLAN_ID, this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.IAP_PLAN_ID).First().icdoPersonAccount.person_account_id,
                                                             ldtVestedDate, null, busConstant.BENEFIT_TYPE_WITHDRAWAL);
                    this.iclbBenefitCalculationDetail.Add(lbusBenefitCalculationDetailIAP);
                }

                # endregion
            }
            else if (this.icdoBenefitCalculationHeader.iintPlanId == busConstant.MPIPP_PLAN_ID)
            {
                if (this.ibusBenefitApplication.iclbEligiblePlans != null && this.ibusBenefitApplication.iclbEligiblePlans.Where(item => item == busConstant.MPIPP).Count() > 0)
                {
                    #region UVHP/EE Benefit Details

                    ldtVestedDate = DateTime.MinValue;
                    busBenefitCalculationDetail lbusBenefitCalculationDetailMPIPP = new busBenefitCalculationDetail { icdoBenefitCalculationDetail = new cdoBenefitCalculationDetail() };
                    lbusBenefitCalculationDetailMPIPP.iobjMainCDO = lbusBenefitCalculationDetailMPIPP.icdoBenefitCalculationDetail;

                    if (this.ibusBenefitApplication.CheckAlreadyVested(busConstant.MPIPP))
                    {
                        ldtVestedDate = this.ibusBenefitApplication.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date;
                    }

                    lbusBenefitCalculationDetailMPIPP.LoadData(this.icdoBenefitCalculationHeader.benefit_calculation_header_id, busConstant.MPIPP_PLAN_ID,
                        this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).First().icdoPersonAccount.person_account_id,
                        ldtVestedDate, null, busConstant.BENEFIT_TYPE_WITHDRAWAL);

                    int lintPersonAccountId = this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).First().icdoPersonAccount.person_account_id;

                    DateTime ldtWithdrawalDate = this.icdoBenefitCalculationHeader.retirement_date;
                    decimal ldecPriorYearUvhpInterest = decimal.Zero;
                    decimal ldecPriorYearEEInterest = decimal.Zero;
                    ldecPartialUVHPInterest = this.ibusCalculation.CalculatePartialUVHPInterest(ldtWithdrawalDate, lintPersonAccountId, out ldecPriorYearUvhpInterest);
                    ldecPartialEEInterest = this.ibusCalculation.CalculatePartialEEInterest(ldtWithdrawalDate,
                                            this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).First(),
                                            false, true, iclbPersonAccountRetirementContribution, out ldecPartialEEInterest);

                    lbusBenefitCalculationDetailMPIPP.icdoBenefitCalculationDetail.ee_derived_benefit_amount = ibusCalculation.CalculateEEDerivedBenefitAsOfRetirementDate(this, null);

                    #region UVHP Benefit Details

                    if (iclbPersonAccountRetirementContribution != null
                        && iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId
                        && item.icdoPersonAccountRetirementContribution.uvhp_amount > Convert.ToDecimal(0)).Count() > 0)
                    {
                        lbusBenefitCalculationDetailMPIPP.icdoBenefitCalculationDetail.total_uvhp_contribution_amount = idecUVHPContribution =
                             (from contribution in this.iclbPersonAccountRetirementContribution
                              where contribution.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId &&
                                  contribution.icdoPersonAccountRetirementContribution.computational_year <= ldtWithdrawalDate.Year
                              select contribution.icdoPersonAccountRetirementContribution.uvhp_amount).Sum();

                        lbusBenefitCalculationDetailMPIPP.icdoBenefitCalculationDetail.total_uvhp_interest_amount = idecUVHPInterest =
                            (from contribution in this.iclbPersonAccountRetirementContribution
                             where contribution.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId &&
                                 contribution.icdoPersonAccountRetirementContribution.computational_year <= ldtWithdrawalDate.Year
                             select contribution.icdoPersonAccountRetirementContribution.uvhp_int_amount).Sum() + ldecPartialUVHPInterest;

                    }

                    # endregion

                    #region EE Benefit Details
                    //if (iclbPersonAccountRetirementContribution != null
                    //    && iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId
                    //     && item.icdoPersonAccountRetirementContribution.ee_contribution_amif (this.ibusPerson.iclbPersonAccount != null && (!this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.person_account_id == lintPersonAccountId).IsNullOrEmpty()) &&
                    //this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.person_account_id == lintPersonAccountId).First().icdoPersonAccount.idecNonVestedEE > 0
                    //&& (ldtVestedDate.IsNull() || ldtVestedDate == DateTime.MinValue))ount > Convert.ToDecimal(0)).Count() > 0 && (ldtVestedDate.IsNull() || ldtVestedDate==DateTime.MinValue))

                    if (this.ibusPerson.iclbPersonAccount != null && (!this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.person_account_id == lintPersonAccountId).IsNullOrEmpty()) &&
                         this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.person_account_id == lintPersonAccountId).First().icdoPersonAccount.idecNonVestedEE > 0)
                    // && (ldtVestedDate.IsNull() || ldtVestedDate == DateTime.MinValue))
                    {
                        //lbusBenefitCalculationDetailMPIPP.icdoBenefitCalculationDetail.total_ee_contribution_amount = idecEEContribution =
                        //    (from contribution in this.iclbPersonAccountRetirementContribution where contribution.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId select contribution.icdoPersonAccountRetirementContribution.ee_contribution_amount).Sum();

                        //lbusBenefitCalculationDetailMPIPP.icdoBenefitCalculationDetail.total_ee_interest_amount = idecEEInterest =
                        //    (from contribution in this.iclbPersonAccountRetirementContribution where contribution.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId select contribution.icdoPersonAccountRetirementContribution.ee_int_amount).Sum() + ldecPartialEEInterest;

                        lbusBenefitCalculationDetailMPIPP.icdoBenefitCalculationDetail.non_vested_ee_amount = idecNonVestedEEContribution =
                            this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.person_account_id == lintPersonAccountId).First().icdoPersonAccount.idecNonVestedEE;

                        lbusBenefitCalculationDetailMPIPP.icdoBenefitCalculationDetail.non_vested_ee_interest = idecNonVestedEEInterest =
                            this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.person_account_id == lintPersonAccountId).First().icdoPersonAccount.idecNonVestedEEInterest + ldecPartialEEInterest;

                        this.iblnCalcualteNonVestedEEBenefit = true;
                    }
                    # endregion

                    # endregion

                    this.iclbBenefitCalculationDetail.Add(lbusBenefitCalculationDetailMPIPP);

                }

                if (this.ibusBenefitApplication.iclbEligiblePlans != null && this.ibusBenefitApplication.iclbEligiblePlans.Where(item => item == busConstant.IAP).Count() > 0)
                {
                    #region IAP Calculation
                    busBenefitCalculationDetail lbusBenefitCalculationDetailIAP = new busBenefitCalculationDetail { icdoBenefitCalculationDetail = new cdoBenefitCalculationDetail() };
                    lbusBenefitCalculationDetailIAP.iobjMainCDO = lbusBenefitCalculationDetailIAP.icdoBenefitCalculationDetail;

                    if (this.ibusBenefitApplication.CheckAlreadyVested(busConstant.IAP))
                    {
                        ldtVestedDate = this.ibusBenefitApplication.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date;
                    }

                    lbusBenefitCalculationDetailIAP.LoadData(this.icdoBenefitCalculationHeader.benefit_calculation_header_id, busConstant.IAP_PLAN_ID, this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.IAP_PLAN_ID).First().icdoPersonAccount.person_account_id,
                                                             ldtVestedDate, null, busConstant.BENEFIT_TYPE_WITHDRAWAL);
                    this.iclbBenefitCalculationDetail.Add(lbusBenefitCalculationDetailIAP);
                    #endregion
                }
            }

            #endregion


            #region SWITCH CASE - INITIATE CALCULATION BASED ON THE REQUIRED PLAN'S ESTIMATE
            if (!this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.IsNullOrEmpty())
            {
                switch (this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).First().icdoPersonAccount.istrPlanCode)
                {

                    case busConstant.IAP:
                        if (this.ibusBenefitApplication.iclbEligiblePlans != null && this.ibusBenefitApplication.iclbEligiblePlans.Where(item => item == busConstant.IAP).Count() > 0)
                        {
                            this.CalculateIAPBenefitAmount(busConstant.CodeValueAll);
                        }
                        break;

                    case busConstant.MPIPP:
                        if (this.ibusBenefitApplication.iclbEligiblePlans != null && this.ibusBenefitApplication.iclbEligiblePlans.Where(item => item == busConstant.MPIPP).Count() > 0)
                        {
                            this.CalculateFinalBenefitForUVHPBenefitOptions(busConstant.CodeValueAll, this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault());
                            if (iclbBenefitCalculationDetail != null && iclbBenefitCalculationDetail.Count > 0 && (this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).Count()) > 0)
                            {
                                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().icdoBenefitCalculationDetail.ee_derived_benefit_amount = ibusCalculation.CalculateEEDerivedBenefitAsOfRetirementDate(this, null);
                            }
                        }


                        if (this.ibusBenefitApplication.iclbEligiblePlans != null && this.ibusBenefitApplication.iclbEligiblePlans.Where(item => item == busConstant.IAP).Count() > 0)
                        {

                            this.CalculateIAPBenefitAmount(busConstant.CodeValueAll);
                        }

                        break;
                }
            }
            #endregion

        }

        private void CalculateIAPBenefitAmount(string astrBenefitOptionValue, string astrAdjustmentFlag = "")
        {

            decimal ldecIAPBalance = busConstant.ZERO_DECIMAL;
            decimal ldecLocal52SpecialAccountBalance = busConstant.ZERO_DECIMAL;
            decimal ldecLocal161SpecialAccountBalance = busConstant.ZERO_DECIMAL;

            decimal ldecIAPAnnuity = new decimal();
            decimal ldecLocal52SpecialAccountAnnuity = new decimal();
            decimal ldecLocal161SpecialAccountAnnuity = new decimal();

            decimal ldecIAPHours4QtrAlloc = busConstant.ZERO_DECIMAL;
            decimal ldecIAPHoursA2forQtrAlloc = busConstant.ZERO_DECIMAL;
            decimal ldecIAPPercent4forQtrAlloc = busConstant.ZERO_DECIMAL;

            decimal ldecAnnunityAdjustmentMultiplier = this.GetAnnunityMultiplier();

            if (this.ibusBenefitApplication.icdoBenefitApplication.child_support_flag != busConstant.FLAG_YES && this.ibusBenefitApplication.icdoBenefitApplication.emergency_onetime_payment_flag != busConstant.FLAG_YES && this.ibusBenefitApplication.icdoBenefitApplication.withdrawal_type_value.IsNullOrEmpty() ) //for PIR-531 //EmergencyOneTimePayment - 03/17/2020
            {
                #region To Set Values for IAP QTR Allocations
                utlConnection utlLegacyDBConnetion = HelperFunction.GetDBConnectionProperties("Legacy");
                string astrLegacyDBConnetion = utlLegacyDBConnetion.istrConnectionString;

                SqlParameter[] parameters = new SqlParameter[3];
                SqlParameter param1 = new SqlParameter("@SSN", DbType.String);
                SqlParameter param2 = new SqlParameter("@FROMDATE", DbType.DateTime);
                SqlParameter param3 = new SqlParameter("@TODATE", DbType.DateTime);

                param1.Value = this.ibusPerson.icdoPerson.istrSSNNonEncrypted;
                parameters[0] = param1;

                busIapAllocationSummary lbusIapAllocationSummary = new busIapAllocationSummary();
                lbusIapAllocationSummary.LoadLatestAllocationSummary();

                param2.Value = busGlobalFunctions.GetLastDateOfComputationYear(lbusIapAllocationSummary.icdoIapAllocationSummary.computation_year);
                parameters[1] = param2;

                param3.Value = busGlobalFunctions.GetLastDayOfWeek(this.ibusBenefitApplication.icdoBenefitApplication.retirement_date); //PROD PIR 113
                parameters[2] = param3;

                DataTable ldtbIAPInfo = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetIAPHourInfoForQuarterlyAllocation", astrLegacyDBConnetion, null, parameters);
                if (ldtbIAPInfo.Rows.Count > 0)
                {
                    //if (ldtbIAPInfo.Rows[0]["IAPHours"] != DBNull.Value)
                    //    ldecIAPHours4QtrAlloc = Convert.ToDecimal(ldtbIAPInfo.Rows[0]["IAPHours"]);

                    //if (ldtbIAPInfo.Rows[0]["IAPHoursA2"] != DBNull.Value)
                    //    ldecIAPHoursA2forQtrAlloc = Convert.ToDecimal(ldtbIAPInfo.Rows[0]["IAPHoursA2"]);

                    //if (ldtbIAPInfo.Rows[0]["IAPPercent"] != DBNull.Value)
                    //    ldecIAPPercent4forQtrAlloc = Convert.ToDecimal(ldtbIAPInfo.Rows[0]["IAPPercent"]);
                    if (ldtbIAPInfo.AsEnumerable().Sum(item => item.Field<decimal>("IAPHours")) > 0)
                        ldecIAPHours4QtrAlloc = Convert.ToDecimal(ldtbIAPInfo.AsEnumerable().Sum(item => item.Field<decimal>("IAPHours")));

                    if (ldtbIAPInfo.AsEnumerable().Sum(item => item.Field<decimal>("IAPHoursA2")) > 0)
                        ldecIAPHoursA2forQtrAlloc = Convert.ToDecimal(ldtbIAPInfo.AsEnumerable().Sum(item => item.Field<decimal>("IAPHoursA2")));

                    DataTable ldtIAPFiltered;
                    busIAPAllocationHelper aobjIAPAllocationHelper = new busIAPAllocationHelper();
                    foreach (DataRow ldrIAPPercent in ldtbIAPInfo.Rows)
                    {
                        if (ldrIAPPercent["IAPPercent"] != DBNull.Value && Convert.ToString(ldrIAPPercent["IAPPercent"]).IsNotNullOrEmpty() &&
                            Convert.ToDecimal(ldrIAPPercent["IAPPercent"]) > 0)
                        {
                            ldtIAPFiltered = new DataTable();
                            ldtIAPFiltered = ldtbIAPInfo.AsEnumerable().Where(o => o.Field<Int16?>("ComputationYear") == Convert.ToInt16(ldrIAPPercent["ComputationYear"])
                                && o.Field<int?>("EmpAccountNo") == Convert.ToInt32(ldrIAPPercent["EmpAccountNo"])).CopyToDataTable();

                            ldecIAPPercent4forQtrAlloc += aobjIAPAllocationHelper.CalculateAllocation4Amount(Convert.ToInt32(ldrIAPPercent["ComputationYear"]), ldtIAPFiltered);
                        }

                    }
                }
                #endregion

                if (!this.iclbPersonAccountRetirementContribution.IsNullOrEmpty())
                {
                    if (this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.IAP).Count() > 0)
                    {
                        int lintPersonAccountId = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.IAP).First().icdoPersonAccount.person_account_id;
                        if (this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Count() > 0)
                        {
                            bool lblnCalculateAllocations = true;
                            if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT)
                            {
                                //PIR 985
                                //if (astrAdjustmentFlag != busConstant.FLAG_YES)
                                //{
                                lblnCalculateAllocations = false;
                                //}
                            }

                            //PIR-534-If below condition satisfies directly fetch the IAP balance from table and there will be no allocations.
                            if (lbusIapAllocationSummary.icdoIapAllocationSummary.computation_year >= this.ibusBenefitApplication.icdoBenefitApplication.retirement_date.Year && this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT)
                            {
                                DataTable ldtbIAPBalance = new DataTable();
                                ldtbIAPBalance = busBase.Select("cdoPersonAccountRetirementContribution.GetIAPBalanceForYear",
                                           new object[1] { lintPersonAccountId });
                                //ldecIAPBalance = Convert.ToDecimal(ldtbIAPBalance.Rows[0]["IAP_BALANCE_AMOUNT"]);
                                //this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().icdoBenefitCalculationDetail.iap_balance_amount = ldecIAPBalance;

                                //PIR-534 Complete fixes
                                if (ldtbIAPBalance != null && ldtbIAPBalance.Rows.Count > 0)
                                {
                                    ldecIAPBalance = Convert.ToDecimal(Convert.ToBoolean(ldtbIAPBalance.Rows[0][0].IsDBNull()) ? busConstant.ZERO_DECIMAL : ldtbIAPBalance.Rows[0][0]);
                                    ldecLocal52SpecialAccountBalance = Convert.ToDecimal(Convert.ToBoolean(ldtbIAPBalance.Rows[0][1].IsDBNull()) ? busConstant.ZERO_DECIMAL : ldtbIAPBalance.Rows[0][1]);
                                    ldecLocal161SpecialAccountBalance = Convert.ToDecimal(Convert.ToBoolean(ldtbIAPBalance.Rows[0][2].IsDBNull()) ? busConstant.ZERO_DECIMAL : ldtbIAPBalance.Rows[0][2]);
                                }

                                if (this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().icdoBenefitCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES)
                                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().icdoBenefitCalculationDetail.local52_special_acct_bal_amount = ldecLocal52SpecialAccountBalance;
                                else if (this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().icdoBenefitCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES)
                                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().icdoBenefitCalculationDetail.local161_special_acct_bal_amount = ldecLocal161SpecialAccountBalance;
                                else
                                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().icdoBenefitCalculationDetail.iap_balance_amount = ldecIAPBalance;
                            }
                            else
                            {
                                ibusCalculation.GetIAPAndSpecialAccountBalance(busConstant.BOOL_FALSE, null, this.iclbBenefitCalculationDetail, this, null,
                                    this.ibusBenefitApplication.icdoBenefitApplication.retirement_date, ldecIAPHours4QtrAlloc, ldecIAPHoursA2forQtrAlloc, ldecIAPPercent4forQtrAlloc, ablnExecuteIAPAllocation: lblnCalculateAllocations, ablnLocal161SpecialAccount: lblnCalculateAllocations, ablnLocal52SpecialAccount: lblnCalculateAllocations);
                            }
                        }
                    }
                }
            }
            //if (this.iblnCalculateIAPBenefit)
            //forPIR-531
            if (this.ibusBenefitApplication.icdoBenefitApplication.child_support_flag == busConstant.FLAG_YES || this.ibusBenefitApplication.icdoBenefitApplication.emergency_onetime_payment_flag == busConstant.FLAG_YES || this.ibusBenefitApplication.icdoBenefitApplication.withdrawal_type_value.IsNotNullOrEmpty() ) //EmergencyOneTimePayment - 03/17/2020
            {
                DataTable ldtbIAPBalance = new DataTable();
                busCalculation lbusCalculation = new busCalculation();
                int lintPersonAccountId = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.IAP).First().icdoPersonAccount.person_account_id;
                lbusCalculation.LoadIAPAllocationSummaryAsofYear(this.ibusBenefitApplication.icdoBenefitApplication.retirement_date.Year - 1);

                if (this.ibusBenefitApplication.icdoBenefitApplication.child_support_flag == busConstant.FLAG_YES)
                {
                    ldtbIAPBalance = busBase.Select("cdoPersonAccountRetirementContribution.GetIAPBalanceAsofYear",
                                   new object[2] { lintPersonAccountId, lbusCalculation.ibusLatestIAPAllocationSummaryAsofYear.icdoIapAllocationSummary.computation_year });
                    ldecIAPBalance = Convert.ToDecimal(ldtbIAPBalance.Rows[0]["IAP_BALANCE_AMOUNT"]);                    
                }
                //EmergencyOneTimePayment - 03/17/2020
                if(this.ibusBenefitApplication.icdoBenefitApplication.emergency_onetime_payment_flag == busConstant.Flag_Yes || this.ibusBenefitApplication.icdoBenefitApplication.withdrawal_type_value.IsNotNullOrEmpty())
                {
                    DataTable ldtbEmergencyPaymentSetupValue = new DataTable();
                    ldtbEmergencyPaymentSetupValue = busBase.Select("cdoEmergencyPaymentSetupValue.GetEmergencyPaymentSetupValue", new object[1] { DateTime.Now});
                    decimal idecFactor = 0;
                    decimal ldecGrossAmount = 0;
                    ldtbIAPBalance = busBase.Select("cdoPersonAccountRetirementContribution.GetIAPBalanceAsofYear",
                                   new object[2] { lintPersonAccountId, Convert.ToInt32(ldtbEmergencyPaymentSetupValue.Rows[0]["IAP_BALANCE_AS_OF_YEAR"]) });
                    //ldecIAPBalance = Convert.ToDecimal(ldtbIAPBalance.Rows[0]["IAP_BALANCE_AMOUNT"]);

                    //Calculate Gross Amount for this payment based on the selected OPTION
                    //COVID, HS23, HS24 were grossing up withdrawal amount, but HARDSHIP_2025 is not grossing up.
                    if ((this.ibusBenefitApplication.icdoBenefitApplication.covid_federal_tax_percentage != 0 || this.ibusBenefitApplication.icdoBenefitApplication.covid_state_tax_percentage != 0)
                        && (this.ibusBenefitApplication.icdoBenefitApplication.emergency_onetime_payment_flag == busConstant.Flag_Yes ||
                            this.ibusBenefitApplication.icdoBenefitApplication.withdrawal_type_value == "HS23" ||
                            this.ibusBenefitApplication.icdoBenefitApplication.withdrawal_type_value == "HS24"
                            ))
                    {
                        ldecGrossAmount = Math.Round(this.ibusBenefitApplication.icdoBenefitApplication.covid_withdrawal_amount / ((100 - (this.ibusBenefitApplication.icdoBenefitApplication.covid_federal_tax_percentage + this.ibusBenefitApplication.icdoBenefitApplication.covid_state_tax_percentage)) / 100), 2);
                    }
                    else
                    {
                        ldecGrossAmount = Math.Round(this.ibusBenefitApplication.icdoBenefitApplication.covid_withdrawal_amount, 2);
                    }

                    //Calculate Factor if multiple plans are used to pay the gross amount:
                    if (this.ibusBenefitApplication.icdoBenefitApplication.covid_option_value == busConstant.COVID_L52_L161_SPL_AC_ONLY_OPTION)
                    {
                        idecFactor = ldecGrossAmount / Math.Round((Convert.ToDecimal(ldtbIAPBalance.Rows[0]["LOCAL52_SPECIAL_ACCT_BAL_AMOUNT"]) + Convert.ToDecimal(ldtbIAPBalance.Rows[0]["LOCAL161_SPECIAL_ACCT_BAL_AMOUNT"])), 2);
                    }
                    else if(this.ibusBenefitApplication.icdoBenefitApplication.covid_option_value == busConstant.COVID_ALL_OPTION)
                    {
                        idecFactor = ldecGrossAmount / Math.Round(Convert.ToDecimal(ldtbIAPBalance.Rows[0]["IAP_BALANCE_AMOUNT"]) + Convert.ToDecimal(ldtbIAPBalance.Rows[0]["LOCAL52_SPECIAL_ACCT_BAL_AMOUNT"]) + Convert.ToDecimal(ldtbIAPBalance.Rows[0]["LOCAL161_SPECIAL_ACCT_BAL_AMOUNT"]), 2);
                    }

                    if (this.ibusBenefitApplication.icdoBenefitApplication.covid_option_value == busConstant.COVID_IAP_ONLY_OPTION)
                    {
                        ldecIAPBalance = ldecGrossAmount;                        
                    }
                    else if(this.ibusBenefitApplication.icdoBenefitApplication.covid_option_value == busConstant.COVID_ALL_OPTION)
                    {
                        ldecIAPBalance = ldecGrossAmount;                        
                        ldecIAPBalance = ldecIAPBalance * idecFactor;
                    }                    

                    if (this.iblnCalculateL52SplAccBenefit)
                    {
                        //ldecLocal52SpecialAccountBalance = Convert.ToDecimal(ldtbIAPBalance.Rows[0]["LOCAL52_SPECIAL_ACCT_BAL_AMOUNT"]);

                        if (this.ibusBenefitApplication.icdoBenefitApplication.covid_option_value == busConstant.COVID_L52_SPL_AC_ONLY_OPTION)
                        {
                            ldecLocal52SpecialAccountBalance = ldecGrossAmount;
                        }
                        else if(this.ibusBenefitApplication.icdoBenefitApplication.covid_option_value == busConstant.COVID_L52_L161_SPL_AC_ONLY_OPTION || this.ibusBenefitApplication.icdoBenefitApplication.covid_option_value == busConstant.COVID_ALL_OPTION)
                        {
                            ldecLocal52SpecialAccountBalance = ldecGrossAmount;
                            ldecLocal52SpecialAccountBalance = ldecLocal52SpecialAccountBalance * idecFactor;
                        }
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().icdoBenefitCalculationDetail.local52_special_acct_bal_amount = ldecLocal52SpecialAccountBalance;
                    }

                    if (this.iblnCalculateL161SplAccBenefit)
                    {
                        //ldecLocal161SpecialAccountBalance = Convert.ToDecimal(ldtbIAPBalance.Rows[0]["LOCAL161_SPECIAL_ACCT_BAL_AMOUNT"]);

                        if (this.ibusBenefitApplication.icdoBenefitApplication.covid_option_value == busConstant.COVID_L161_SPL_AC_ONLY_OPTION)
                        {
                            ldecLocal161SpecialAccountBalance = ldecGrossAmount;
                        }
                        else if (this.ibusBenefitApplication.icdoBenefitApplication.covid_option_value == busConstant.COVID_L52_L161_SPL_AC_ONLY_OPTION || this.ibusBenefitApplication.icdoBenefitApplication.covid_option_value == busConstant.COVID_ALL_OPTION)
                        {
                            ldecLocal161SpecialAccountBalance = ldecGrossAmount;
                            ldecLocal161SpecialAccountBalance = ldecLocal161SpecialAccountBalance * idecFactor;
                        }

                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().icdoBenefitCalculationDetail.local161_special_acct_bal_amount = ldecLocal161SpecialAccountBalance;
                    }
                }

                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().icdoBenefitCalculationDetail.iap_balance_amount = ldecIAPBalance;
            }
            else if (this.iblnCalculateIAPBenefit)
            {
                ldecIAPBalance = this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().icdoBenefitCalculationDetail.iap_balance_amount;

                //Process QDRO Offset
                this.ibusCalculation.ProcessQDROOffset(this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).First()
                    , this.icdoBenefitCalculationHeader.person_id, ref ldecIAPBalance, astrCalculationType: this.icdoBenefitCalculationHeader.calculation_type_value);
                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().icdoBenefitCalculationDetail.idecRemainingBenefits = ldecIAPBalance;
            }

            if (this.iblnCalculateL52SplAccBenefit)
            {
                ldecLocal52SpecialAccountBalance = this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().icdoBenefitCalculationDetail.local52_special_acct_bal_amount;
                //Process QDRO Offset
                this.ibusCalculation.ProcessQDROOffset(this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).First()
                    , this.icdoBenefitCalculationHeader.person_id, ref ldecLocal52SpecialAccountBalance, ablnL52SplAccFlag: true, astrCalculationType: this.icdoBenefitCalculationHeader.calculation_type_value);
                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().icdoBenefitCalculationDetail.idecRemainingBenefits = ldecLocal52SpecialAccountBalance;
            }

            if (this.iblnCalculateL161SplAccBenefit)
            {
                ldecLocal161SpecialAccountBalance = this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().icdoBenefitCalculationDetail.local161_special_acct_bal_amount;

                //Process QDRO Offset
                this.ibusCalculation.ProcessQDROOffset(this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).First()
                    , this.icdoBenefitCalculationHeader.person_id, ref ldecLocal161SpecialAccountBalance, ablnL161SplAccFlag: true, astrCalculationType: this.icdoBenefitCalculationHeader.calculation_type_value);
                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().icdoBenefitCalculationDetail.idecRemainingBenefits = ldecLocal161SpecialAccountBalance;
            }

            busBenefitCalculationOptions lbusBenefitCalculationOptions;

            // Step 2. Multiply the amount with the Benefit Option Factors and store it in the BenefitCalculationOption Collection
            if ((ldecIAPBalance > busConstant.ZERO_DECIMAL || ldecLocal52SpecialAccountBalance > busConstant.ZERO_DECIMAL || ldecLocal161SpecialAccountBalance > busConstant.ZERO_DECIMAL)
                || icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT)//PIR 985 10252015
            {
                int lintPlanBenefitId = busConstant.ZERO_INT;

                //Check If Spouse
                bool lblnCheckIfSpouse = busConstant.BOOL_FALSE;
                lblnCheckIfSpouse = this.ibusCalculation.CheckIfSpouse(this.icdoBenefitCalculationHeader.person_id,
                        this.icdoBenefitCalculationHeader.beneficiary_person_id);

                switch (astrBenefitOptionValue)
                {
                    case busConstant.CodeValueAll:
                        // Calculate the Benefit Amounts for all Benefit Options Availble for IAP
                        // Life Annuity
                        if (ldecIAPBalance > busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT)
                        {
                            LoadCalculationOptionsForIAP(busConstant.LIFE, ldecIAPAnnuity, ldecIAPBalance, ldecAnnunityAdjustmentMultiplier);

                            if (lblnCheckIfSpouse)
                            {
                                LoadCalculationOptionsForIAP(busConstant.QJ50, ldecIAPAnnuity, ldecIAPBalance, ldecAnnunityAdjustmentMultiplier);
                                LoadCalculationOptionsForIAP(busConstant.JS75, ldecIAPAnnuity, ldecIAPBalance, ldecAnnunityAdjustmentMultiplier);
                                LoadCalculationOptionsForIAP(busConstant.J100, ldecIAPAnnuity, ldecIAPBalance, ldecAnnunityAdjustmentMultiplier);

                            }
                            LoadCalculationOptionsForIAP(busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, ldecIAPAnnuity, ldecIAPBalance, ldecAnnunityAdjustmentMultiplier);
                        }
                        // Lumpsum Benefit Option
                        // No factor. The Lump sum for IAP will be the total IAP Balance itself.
                        //if (ldecIAPBalance <= busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT)
                        if (ldecIAPBalance > 0)
                        {
                            lintPlanBenefitId = ibusCalculation.GetPlanBenefitId(busConstant.IAP_PLAN_ID, busConstant.LUMP_SUM);

                            lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                            lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, 1, ldecIAPBalance, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                    this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.LUMP_SUM, 0, false, false);

                            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
                        }

                        if (ldecLocal52SpecialAccountBalance > busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT)
                        {
                            LoadCalculationOptionsForLocal52SpecialAccounts(busConstant.LIFE, ldecLocal52SpecialAccountAnnuity, ldecLocal52SpecialAccountBalance);
                            if (lblnCheckIfSpouse)
                            {
                                LoadCalculationOptionsForLocal52SpecialAccounts(busConstant.QJ50, ldecLocal52SpecialAccountAnnuity, ldecLocal52SpecialAccountBalance);
                                if (this.ibusBenefitApplication.icdoBenefitApplication.retirement_date >= busConstant.BenefitCalculation.DISABILITY_JS75_CHECK_DATE)
                                {
                                    LoadCalculationOptionsForLocal52SpecialAccounts(busConstant.JS75, ldecLocal52SpecialAccountAnnuity, ldecLocal52SpecialAccountBalance);
                                }

                                //LoadCalculationOptionsForLocal52SpecialAccounts(busConstant.J100, ldecLocal52SpecialAccountAnnuity, ldecLocal52SpecialAccountBalance); //RID #108577
                            }
                            //LoadCalculationOptionsForLocal52SpecialAccounts(busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, ldecLocal52SpecialAccountAnnuity, ldecLocal52SpecialAccountBalance); //RID #108577
                        }
                        //if (ldecLocal52SpecialAccountBalance <= busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT)
                        if (ldecLocal52SpecialAccountBalance > 0)
                        {
                            lintPlanBenefitId = ibusCalculation.GetPlanBenefitId(busConstant.IAP_PLAN_ID, busConstant.LUMP_SUM);

                            lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                            lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, 1, ldecLocal52SpecialAccountBalance, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                    this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.LUMP_SUM, new decimal(), false, false, true);
                            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
                        }


                        if (ldecLocal161SpecialAccountBalance > busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT)
                        {
                            LoadCalculationOptionsForLocal161SpecialAccounts(busConstant.LIFE, ldecLocal161SpecialAccountAnnuity, ldecLocal161SpecialAccountBalance);
                            if (lblnCheckIfSpouse)
                            {
                                LoadCalculationOptionsForLocal161SpecialAccounts(busConstant.QJ50, ldecLocal161SpecialAccountAnnuity, ldecLocal161SpecialAccountBalance);
                                if (this.ibusBenefitApplication.icdoBenefitApplication.retirement_date >= busConstant.BenefitCalculation.DISABILITY_JS75_CHECK_DATE)
                                {
                                    LoadCalculationOptionsForLocal161SpecialAccounts(busConstant.JS75, ldecLocal161SpecialAccountAnnuity, ldecLocal161SpecialAccountBalance);
                                }
                                //LoadCalculationOptionsForLocal161SpecialAccounts(busConstant.J100, ldecLocal161SpecialAccountAnnuity, ldecLocal161SpecialAccountBalance); //RID #108577
                            }
                            //LoadCalculationOptionsForLocal161SpecialAccounts(busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, ldecLocal161SpecialAccountAnnuity, ldecLocal161SpecialAccountBalance); //RID #108577

                        }

                        //if (ldecLocal161SpecialAccountBalance <= busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT)
                        if (ldecLocal161SpecialAccountBalance > 0)
                        {
                            lintPlanBenefitId = ibusCalculation.GetPlanBenefitId(busConstant.IAP_PLAN_ID, busConstant.LUMP_SUM);

                            lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                            lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, 1, ldecLocal161SpecialAccountBalance, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                    this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.LUMP_SUM, new decimal(), false, false, false, true);
                            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
                        }

                        break;

                    case busConstant.LIFE_ANNUTIY:
                        // Life Annuity
                        if (ldecIAPBalance > busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT)
                        {
                            if (this.iblnCalculateIAPBenefit)
                            {
                                LoadCalculationOptionsForIAP(busConstant.LIFE, ldecIAPAnnuity, ldecIAPBalance,ldecAnnunityAdjustmentMultiplier);
                            }
                        }

                        if (ldecLocal52SpecialAccountBalance > busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT)
                        {
                            if (this.iblnCalculateL52SplAccBenefit)
                            {
                                LoadCalculationOptionsForLocal52SpecialAccounts(busConstant.LIFE, ldecLocal52SpecialAccountAnnuity, ldecLocal52SpecialAccountBalance);
                            }
                        }

                        if (ldecLocal161SpecialAccountBalance > busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT)
                        {
                            if (this.iblnCalculateL161SplAccBenefit)
                            {
                                LoadCalculationOptionsForLocal161SpecialAccounts(busConstant.LIFE, ldecLocal161SpecialAccountAnnuity, ldecLocal161SpecialAccountBalance);
                            }
                        }
                        break;

                    case busConstant.QJ50:

                        if (ldecIAPBalance > busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT && lblnCheckIfSpouse)
                        {
                            if (this.iblnCalculateIAPBenefit)
                            {
                                LoadCalculationOptionsForIAP(busConstant.QJ50, ldecIAPAnnuity, ldecIAPBalance,ldecAnnunityAdjustmentMultiplier);
                            }
                        }

                        if (ldecLocal52SpecialAccountBalance > busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT && lblnCheckIfSpouse)
                        {
                            if (this.iblnCalculateL52SplAccBenefit)
                            {
                                LoadCalculationOptionsForLocal52SpecialAccounts(busConstant.QJ50, ldecLocal52SpecialAccountAnnuity, ldecLocal52SpecialAccountBalance);
                            }
                        }

                        if (ldecLocal161SpecialAccountBalance > busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT && lblnCheckIfSpouse)
                        {
                            if (this.iblnCalculateL161SplAccBenefit)
                            {
                                LoadCalculationOptionsForLocal161SpecialAccounts(busConstant.QJ50, ldecLocal161SpecialAccountAnnuity, ldecLocal161SpecialAccountBalance);
                            }
                        }
                        break;

                    case busConstant.J100:

                        if (ldecIAPBalance > busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT && lblnCheckIfSpouse)
                        {
                            if (this.iblnCalculateIAPBenefit)
                            {
                                LoadCalculationOptionsForIAP(busConstant.J100, ldecIAPAnnuity, ldecIAPBalance,ldecAnnunityAdjustmentMultiplier);
                            }
                        }

                        if (ldecLocal52SpecialAccountBalance > busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT && lblnCheckIfSpouse)
                        {
                            if (this.iblnCalculateL52SplAccBenefit)
                            {
                                LoadCalculationOptionsForLocal52SpecialAccounts(busConstant.J100, ldecLocal52SpecialAccountAnnuity, ldecLocal52SpecialAccountBalance);
                            }
                        }

                        if (ldecLocal161SpecialAccountBalance > busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT && lblnCheckIfSpouse)
                        {
                            if (this.iblnCalculateL161SplAccBenefit)
                            {
                                LoadCalculationOptionsForLocal161SpecialAccounts(busConstant.J100, ldecLocal161SpecialAccountAnnuity, ldecLocal161SpecialAccountBalance);
                            }
                        }
                        break;

                    case busConstant.JS75:

                        if (ldecIAPBalance > busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT && lblnCheckIfSpouse)
                        {
                            if (this.iblnCalculateIAPBenefit)
                            {
                                LoadCalculationOptionsForIAP(busConstant.JS75, ldecIAPAnnuity, ldecIAPBalance,ldecAnnunityAdjustmentMultiplier);
                            }
                        }

                        if (this.ibusBenefitApplication.icdoBenefitApplication.retirement_date >= busConstant.BenefitCalculation.DISABILITY_JS75_CHECK_DATE)
                        {

                            if (ldecLocal52SpecialAccountBalance > busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT && lblnCheckIfSpouse)
                            {
                                if (this.iblnCalculateL52SplAccBenefit)
                                {
                                    LoadCalculationOptionsForLocal52SpecialAccounts(busConstant.JS75, ldecLocal52SpecialAccountAnnuity, ldecLocal52SpecialAccountBalance);
                                }
                            }

                            if (ldecLocal161SpecialAccountBalance > busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT && lblnCheckIfSpouse)
                            {
                                if (this.iblnCalculateL161SplAccBenefit)
                                {
                                    LoadCalculationOptionsForLocal161SpecialAccounts(busConstant.JS75, ldecLocal161SpecialAccountAnnuity, ldecLocal161SpecialAccountBalance);
                                }
                            }
                        }
                        break;

                    case busConstant.LUMP_SUM:
                        //if (ldecIAPBalance <= busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT)
                        //{
                        if (this.iblnCalculateIAPBenefit)
                        {
                            lintPlanBenefitId = ibusCalculation.GetPlanBenefitId(busConstant.IAP_PLAN_ID, busConstant.LUMP_SUM);
                            lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                            lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, 1, ldecIAPBalance, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                    this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.LUMP_SUM, 0, false, false);

                            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                                                          item.icdoBenefitCalculationDetail.l52_spl_acc_flag.IsNullOrEmpty() &&
                                                          item.icdoBenefitCalculationDetail.l161_spl_acc_flag.IsNullOrEmpty()).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
                        }
                        //}

                        //if (ldecLocal52SpecialAccountBalance <= busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT)
                        //{
                        if (this.iblnCalculateL52SplAccBenefit)
                        {
                            lintPlanBenefitId = ibusCalculation.GetPlanBenefitId(busConstant.IAP_PLAN_ID, busConstant.LUMP_SUM);
                            lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                            lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, 1, ldecLocal52SpecialAccountBalance, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                    this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.LUMP_SUM, busConstant.ZERO_DECIMAL, false, false, true);

                            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID && item.icdoBenefitCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
                        }
                        //}

                        //if (ldecLocal161SpecialAccountBalance <= busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT)
                        //{
                        if (this.iblnCalculateL161SplAccBenefit)
                        {
                            lintPlanBenefitId = ibusCalculation.GetPlanBenefitId(busConstant.IAP_PLAN_ID, busConstant.LUMP_SUM);
                            lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                            lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, 1, ldecLocal161SpecialAccountBalance, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                    this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.LUMP_SUM, busConstant.ZERO_DECIMAL, false, false, false, true);

                            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID && item.icdoBenefitCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
                        }
                        //}
                        break;

                    case busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY:
                        // Life Annuity
                        if (ldecIAPBalance > busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT)
                        {
                            if (this.iblnCalculateIAPBenefit)
                            {
                                LoadCalculationOptionsForIAP(busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, ldecIAPAnnuity, ldecIAPBalance,ldecAnnunityAdjustmentMultiplier);
                            }
                        }

                        if (ldecLocal52SpecialAccountBalance > busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT)
                        {
                            if (this.iblnCalculateL52SplAccBenefit)
                            {
                                LoadCalculationOptionsForLocal52SpecialAccounts(busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, ldecLocal52SpecialAccountAnnuity, ldecLocal52SpecialAccountBalance);
                            }
                        }

                        if (ldecLocal161SpecialAccountBalance > busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT)
                        {
                            if (this.iblnCalculateL161SplAccBenefit)
                            {
                                LoadCalculationOptionsForLocal161SpecialAccounts(busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, ldecLocal161SpecialAccountAnnuity, ldecLocal161SpecialAccountBalance);
                            }
                        }
                        break;

                    default:
                        break;
                }
            }
        }

        private void CalculateIAPBenefitAmountForAlternatePayee(busQdroApplication abusQdroApplication, string astrBenefitOptionValue)
        {

            decimal ldecIAPBalance = busConstant.ZERO_DECIMAL;
            decimal ldecLocal52SpecialAccountBalance = busConstant.ZERO_DECIMAL;
            decimal ldecLocal161SpecialAccountBalance = busConstant.ZERO_DECIMAL;

            decimal ldecIAPAnnuity = new decimal();
            decimal ldecLocal52SpecialAccountAnnuity = new decimal();
            decimal ldecLocal161SpecialAccountAnnuity = new decimal();

            decimal ldecTotalHrsWorkedBetTwoDates = 0;
            decimal ldecTotalHours = 0;
            decimal ldecAltPayeeFraction = 0;
            decimal ldecQdroPercent = 0;
            decimal ldecFlatAmount = 0;
            decimal ldecFlatPercent = 0;
            int ldecParticipantAge = 0;
            int ldecAlternatePayeeAge = 0;
            decimal ldecBenefitBeforeConversion = 0;
            decimal ldecLifeConversionFactor = 1;
            decimal ldecAlternatePayeeBenefitAmt = 0;
            decimal idecThru79Hours = 0;
            decimal ldecIAPHours4QtrAlloc = 0.0M;
            decimal ldecIAPHoursA2forQtrAlloc = 0.0M;
            decimal ldecIAPPercent4forQtrAlloc = 0.0M;


            #region Get Alternate Payee Balance
            ldecParticipantAge = busGlobalFunctions.CalulateAge(abusQdroApplication.ibusParticipant.icdoPerson.idtDateofBirth, abusQdroApplication.icdoDroApplication.dro_commencement_date);
            ldecAlternatePayeeAge = busGlobalFunctions.CalulateAge(abusQdroApplication.ibusAlternatePayee.icdoPerson.idtDateofBirth, abusQdroApplication.icdoDroApplication.dro_commencement_date);

            int lintPersonAccountId = abusQdroApplication.ibusParticipant.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.IAP_PLAN_ID).First().icdoPersonAccount.person_account_id;
            DateTime ldtWithdrawalDate = this.icdoBenefitCalculationHeader.retirement_date;
            //Get 
            // DataTable ldtbIAPBalance = busBase.Select("cdoPersonAccountRetirementContribution.GetIAPBalanceAsofYear",
            // new object[2] { lintPersonAccountId,abusQdroApplication.icdoDroApplication.dro_commencement_date.Year});

            utlConnection utlLegacyDBConnetion = HelperFunction.GetDBConnectionProperties("Legacy");
            string astrLegacyDBConnetion = utlLegacyDBConnetion.istrConnectionString;

            SqlParameter[] parameters = new SqlParameter[3];
            SqlParameter param1 = new SqlParameter("@SSN", DbType.String);
            SqlParameter param2 = new SqlParameter("@FROMDATE", DbType.DateTime);
            SqlParameter param3 = new SqlParameter("@TODATE", DbType.DateTime);

            param1.Value = this.ibusPerson.icdoPerson.istrSSNNonEncrypted;
            parameters[0] = param1;

            busIapAllocationSummary lbusIapAllocationSummary = new busIapAllocationSummary();
            lbusIapAllocationSummary.LoadLatestAllocationSummary();

            param2.Value = busGlobalFunctions.GetLastDateOfComputationYear(lbusIapAllocationSummary.icdoIapAllocationSummary.computation_year);
            parameters[1] = param2;

            param3.Value = busGlobalFunctions.GetLastDayOfWeek(this.ibusBenefitApplication.icdoBenefitApplication.retirement_date); //PROD PIR 113
            parameters[2] = param3;

            DataTable ldtbIAPInfo = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetIAPHourInfoForQuarterlyAllocation", astrLegacyDBConnetion, null, parameters);
            if (ldtbIAPInfo.Rows.Count > 0)
            {
                //if (ldtbIAPInfo.Rows[0]["IAPHours"] != DBNull.Value)
                //    ldecIAPHours4QtrAlloc = Convert.ToDecimal(ldtbIAPInfo.Rows[0]["IAPHours"]);

                //if (ldtbIAPInfo.Rows[0]["IAPHoursA2"] != DBNull.Value)
                //    ldecIAPHoursA2forQtrAlloc = Convert.ToDecimal(ldtbIAPInfo.Rows[0]["IAPHoursA2"]);

                //if (ldtbIAPInfo.Rows[0]["IAPPercent"] != DBNull.Value)
                //    ldecIAPPercent4forQtrAlloc = Convert.ToDecimal(ldtbIAPInfo.Rows[0]["IAPPercent"]);

                if (ldtbIAPInfo.AsEnumerable().Sum(item => item.Field<decimal>("IAPHours")) > 0)
                    ldecIAPHours4QtrAlloc = Convert.ToDecimal(ldtbIAPInfo.AsEnumerable().Sum(item => item.Field<decimal>("IAPHours")));

                if (ldtbIAPInfo.AsEnumerable().Sum(item => item.Field<decimal>("IAPHoursA2")) > 0)
                    ldecIAPHoursA2forQtrAlloc = Convert.ToDecimal(ldtbIAPInfo.AsEnumerable().Sum(item => item.Field<decimal>("IAPHoursA2")));

                DataTable ldtIAPFiltered;
                busIAPAllocationHelper aobjIAPAllocationHelper = new busIAPAllocationHelper();
                foreach (DataRow ldrIAPPercent in ldtbIAPInfo.Rows)
                {
                    if (ldrIAPPercent["IAPPercent"] != DBNull.Value && Convert.ToString(ldrIAPPercent["IAPPercent"]).IsNotNullOrEmpty() &&
                        Convert.ToDecimal(ldrIAPPercent["IAPPercent"]) > 0)
                    {
                        ldtIAPFiltered = new DataTable();
                        ldtIAPFiltered = ldtbIAPInfo.AsEnumerable().Where(o => o.Field<Int16?>("ComputationYear") == Convert.ToInt16(ldrIAPPercent["ComputationYear"])
                            && o.Field<int?>("EmpAccountNo") == Convert.ToInt32(ldrIAPPercent["EmpAccountNo"])).CopyToDataTable();

                        ldecIAPPercent4forQtrAlloc += aobjIAPAllocationHelper.CalculateAllocation4Amount(Convert.ToInt32(ldrIAPPercent["ComputationYear"]), ldtIAPFiltered);
                    }

                }
            }

            this.ibusCalculation.GetIAPAndSpecialAccountBalance(busConstant.BOOL_FALSE, null, this.iclbBenefitCalculationDetail, this, null, this.icdoBenefitCalculationHeader.retirement_date, ldecIAPHours4QtrAlloc, ldecIAPHoursA2forQtrAlloc, ldecIAPPercent4forQtrAlloc);

            if (this.iblnCalculateL52SplAccBenefit)
            {
                ldecLocal52SpecialAccountBalance = this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().icdoBenefitCalculationDetail.local52_special_acct_bal_amount;
            }
            else
            {
                ldecLocal161SpecialAccountBalance = this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().icdoBenefitCalculationDetail.local161_special_acct_bal_amount;
            }


            ldecFlatAmount = abusQdroApplication.iclbDroBenefitDetails.Where(item => item.icdoDroBenefitDetails.plan_id == busConstant.IAP_PLAN_ID).First().
               icdoDroBenefitDetails.benefit_amt;
            ldecFlatPercent = abusQdroApplication.iclbDroBenefitDetails.Where(item => item.icdoDroBenefitDetails.plan_id == busConstant.IAP_PLAN_ID).First().
                 icdoDroBenefitDetails.benefit_flat_perc;
            ldecQdroPercent = abusQdroApplication.iclbDroBenefitDetails.Where(item => item.icdoDroBenefitDetails.plan_id == busConstant.IAP_PLAN_ID).First().
                 icdoDroBenefitDetails.benefit_perc;



            //cdoDummyWorkData lcdoWorkData1979 = null;
            //if (ibusBenefitApplication.aclbPersonWorkHistory_IAP.Where(o => o.year == 1979).Count() > 0)
            //{
            //    lcdoWorkData1979 = ibusBenefitApplication.aclbPersonWorkHistory_IAP.Where(o => o.year == 1979).FirstOrDefault();
            //}
            ////IF participant was on BIS as of 1979, then prior hours should not be counted towards Thru1979

            ////Hours for year 1979 only
            //if (lcdoWorkData1979 != null && lcdoWorkData1979.bis_years_count < 2)
            //{
            //    if (ibusBenefitApplication.aclbPersonWorkHistory_IAP.Where(o => o.year == 1979).Count() > 0)
            //    {
            //        idecThru79Hours = ibusBenefitApplication.aclbPersonWorkHistory_IAP.Where(o => o.year == 1979).FirstOrDefault().qualified_hours;
            //    }
            //}


            #region IF participant was on BIS as of 1979, then prior hours should not be counted towards Thru1979

            //PIR 0971
            busPersonT79hours lbusPersonT79hours = new busPersonT79hours { icdoPersonT79hours = new cdoPersonT79hours() };
            if (lbusPersonT79hours.FindPersonT79hours(lintPersonAccountId) && lbusPersonT79hours.icdoPersonT79hours.approved_flag == busConstant.FLAG_YES)
            {
                idecThru79Hours = lbusPersonT79hours.icdoPersonT79hours.t79_hours;
            }
            else
            { 

                //Remove history for any forfieture year 1979
                if (ibusBenefitApplication.aclbPersonWorkHistory_IAP != null && ibusBenefitApplication.aclbPersonWorkHistory_IAP.Where(o => o.year <= busConstant.BenefitCalculation.YEAR_1979).Count() > 0)
                {
                    if (ibusBenefitApplication.aclbPersonWorkHistory_IAP.Where(o => o.year <= busConstant.BenefitCalculation.YEAR_1979 && o.istrForfietureFlag == busConstant.FLAG_YES).Count() > 0)
                    {
                        int lintMaxForfietureYearBefore1979 = ibusBenefitApplication.aclbPersonWorkHistory_IAP.Where(item => item.year <= busConstant.BenefitCalculation.YEAR_1979 && item.istrForfietureFlag == busConstant.FLAG_YES).Max(t => t.year);
                        ibusBenefitApplication.aclbPersonWorkHistory_IAP = ibusBenefitApplication.aclbPersonWorkHistory_IAP.Where(item => item.year > lintMaxForfietureYearBefore1979).ToList().ToCollection();
                    }
                }

                if (ibusBenefitApplication.aclbPersonWorkHistory_IAP != null && ibusBenefitApplication.aclbPersonWorkHistory_IAP.Where(o => o.year == busConstant.BenefitCalculation.YEAR_1979).Count() > 0)
                {
                    decimal ldecPreviousYearPaidIAPAccountBalance = 0M;
                    cdoDummyWorkData lcdoWorkData1979 = ibusBenefitApplication.aclbPersonWorkHistory_IAP.Where(o => o.year == busConstant.BenefitCalculation.YEAR_1979).FirstOrDefault();
                    //IF participant was on BIS as of 1979, then prior hours should not be counted towards Thru1979
                    if (lcdoWorkData1979 != null && lcdoWorkData1979.bis_years_count < 2)
                    {
                        int lintPaymentYear = 0;
                        DataTable ldtblPaymentYear = busBase.Select("cdoPersonAccountRetirementContribution.GetMaxPaymentYearOnOrBefore1979", new object[1] { ibusBenefitApplication.icdoBenefitApplication.person_id });
                        if (ldtblPaymentYear != null && ldtblPaymentYear.Rows.Count > 0 && Convert.ToString(ldtblPaymentYear.Rows[0][0]).IsNotNullOrEmpty())
                        {
                            lintPaymentYear = Convert.ToInt32(ldtblPaymentYear.Rows[0][0]);
                        }
                        if (lintPaymentYear == 0)
                        {

                            idecThru79Hours = ibusBenefitApplication.aclbPersonWorkHistory_IAP.Where(o => o.year <= busConstant.BenefitCalculation.YEAR_1979).Sum(o => o.qualified_hours);

                        }
                        else
                        {
                            if (ibusBenefitApplication.aclbPersonWorkHistory_IAP.Where(o => o.year <= busConstant.BenefitCalculation.YEAR_1979 && o.year > lintPaymentYear).Count() > 0)
                            {
                                idecThru79Hours = ibusBenefitApplication.aclbPersonWorkHistory_IAP.Where(o => o.year <= busConstant.BenefitCalculation.YEAR_1979 && o.year > lintPaymentYear).Sum(o => o.qualified_hours);
                            }
                        }

                        idecThru79Hours += ldecPreviousYearPaidIAPAccountBalance;
                        if (idecThru79Hours < 0)
                            idecThru79Hours = 0;
                    }
                }
            }

            if (ibusBenefitApplication.aclbPersonWorkHistory_IAP != null && ibusBenefitApplication.aclbPersonWorkHistory_IAP.Where(o => o.year >= busConstant.BenefitCalculation.YEAR_1979).Count() > 0)
            {
                ibusBenefitApplication.aclbPersonWorkHistory_IAP = ibusBenefitApplication.aclbPersonWorkHistory_IAP.Where(o => o.year >= busConstant.BenefitCalculation.YEAR_1979).ToList().ToCollection();
            }

            #endregion

            DateTime ldtForfeitureDate = new DateTime();

            if (this.ibusBenefitApplication.ibusTempPersonAccountEligibility != null)
                ldtForfeitureDate = this.ibusBenefitApplication.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.forfeiture_date;

            this.ibusCalculation.GetTotalHoursWorked(abusQdroApplication.ibusParticipant.icdoPerson.istrSSNNonEncrypted, busConstant.IAP,
                                                     abusQdroApplication.icdoDroApplication.dro_commencement_date, ldtForfeitureDate, idecThru79Hours, ref ldecTotalHours);
            //Prodfix_02/07/2013_4
            //Prod PIR : 81 : 01/21/2013
            ldecTotalHrsWorkedBetTwoDates = this.ibusCalculation.GetProratedHoursBetweenTwoDates(abusQdroApplication.icdoDroApplication.date_of_marriage, abusQdroApplication.icdoDroApplication.date_of_divorce,
                                            this.ibusBenefitApplication.aclbPersonWorkHistory_IAP, busConstant.IAP, ldtForfeitureDate, abusQdroApplication.ibusParticipant.icdoPerson.istrSSNNonEncrypted, busConstant.IAP_PLAN_ID);

            ldecAltPayeeFraction = Math.Round(((ldecTotalHrsWorkedBetTwoDates / ldecTotalHours) * ldecQdroPercent) / 100, 3);//PIR 963

            if (this.iblnCalculateL52SplAccBenefit)
            {
                ldecBenefitBeforeConversion = this.ibusCalculation.CalculateBenefitAmtBeforeConversion(ldecLocal52SpecialAccountBalance, ldecAltPayeeFraction, ldecFlatAmount, ldecFlatPercent);

            }
            else if (this.iblnCalculateL161SplAccBenefit)
            {
                ldecBenefitBeforeConversion = this.ibusCalculation.CalculateBenefitAmtBeforeConversion(ldecLocal161SpecialAccountBalance, ldecAltPayeeFraction, ldecFlatAmount, ldecFlatPercent);

            }

            //if (abusQdroApplication.icdoDroApplication.life_conversion_factor_flag == busConstant.FLAG_YES)
            //{
            //    ldecLifeConversionFactor = this.ibusCalculation.CalculateLifeConversionFactor(ldecParticipantAge, ldecAlternatePayeeAge);
            //}

            ldecAlternatePayeeBenefitAmt = ldecBenefitBeforeConversion;

            if (this.iblnCalculateL52SplAccBenefit)
            {

                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().icdoBenefitCalculationDetail.local52_special_acct_bal_amount = ldecAlternatePayeeBenefitAmt;
            }
            else
            {
                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().icdoBenefitCalculationDetail.local161_special_acct_bal_amount = ldecAlternatePayeeBenefitAmt;
            }


            #endregion


            busBenefitCalculationOptions lbusBenefitCalculationOptions;

            // Step 2. Multiply the amount with the Benefit Option Factors and store it in the BenefitCalculationOption Collection
            if (ldecLocal52SpecialAccountBalance > busConstant.ZERO_DECIMAL || ldecLocal161SpecialAccountBalance > busConstant.ZERO_DECIMAL)
            {

                switch (astrBenefitOptionValue)
                {
                    case busConstant.LIFE_ANNUTIY:

                        if (ldecAlternatePayeeBenefitAmt > busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT)
                        {
                            if (this.iblnCalculateL52SplAccBenefit)
                            {
                                LoadCalculationOptionsForLocal52SpecialAccountsForAlternatePayee(busConstant.LIFE, 0, ldecAlternatePayeeBenefitAmt, ldecLifeConversionFactor);
                            }
                        }

                        if (ldecAlternatePayeeBenefitAmt > busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT)
                        {
                            if (this.iblnCalculateL161SplAccBenefit)
                            {
                                LoadCalculationOptionsForLocal161SpecialAccountsForAlternatePayee(busConstant.LIFE, 0, ldecAlternatePayeeBenefitAmt, ldecLifeConversionFactor);
                            }
                        }
                        break;


                    case busConstant.LUMP_SUM:


                        if (this.iblnCalculateL52SplAccBenefit)
                        {
                            LoadCalculationOptionsForLocal52SpecialAccountsForAlternatePayee(busConstant.LUMP_SUM, abusQdroApplication.icdoDroApplication.dro_commencement_date.Year, ldecAlternatePayeeBenefitAmt, ldecLifeConversionFactor);
                        }

                        if (this.iblnCalculateL161SplAccBenefit)
                        {
                            LoadCalculationOptionsForLocal161SpecialAccountsForAlternatePayee(busConstant.LUMP_SUM, abusQdroApplication.icdoDroApplication.dro_commencement_date.Year, ldecAlternatePayeeBenefitAmt, ldecLifeConversionFactor);
                        }
                        /*
                        //if (ldecLocal52SpecialAccountBalance <= busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT)
                        //{
                        if (this.iblnCalculateL52SplAccBenefit)
                        {
                            lintPlanBenefitId = ibusCalculation.GetPlanBenefitId(busConstant.IAP_PLAN_ID, busConstant.LUMP_SUM);
                            lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                            lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, 1, ldecAlternatePayeeBenefitAmt, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                    this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.LUMP_SUM, busConstant.ZERO_DECIMAL, false, false, true);

                            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID && item.icdoBenefitCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
                        }
                        //}

                        //if (ldecLocal161SpecialAccountBalance <= busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT)
                        //{
                        if (this.iblnCalculateL161SplAccBenefit)
                        {
                            lintPlanBenefitId = ibusCalculation.GetPlanBenefitId(busConstant.IAP_PLAN_ID, busConstant.LUMP_SUM);
                            lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                            lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, 1, ldecAlternatePayeeBenefitAmt, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                    this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.LUMP_SUM, busConstant.ZERO_DECIMAL, false, false, false, true);

                            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID && item.icdoBenefitCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
                        }
                        //}*/
                        break;

                    case busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY:
                        if (ldecAlternatePayeeBenefitAmt > busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT)
                        {
                            if (this.iblnCalculateL52SplAccBenefit)
                            {
                                LoadCalculationOptionsForLocal52SpecialAccountsForAlternatePayee(busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, 0, ldecAlternatePayeeBenefitAmt, ldecLifeConversionFactor);
                            }

                            if (this.iblnCalculateL161SplAccBenefit)
                            {
                                LoadCalculationOptionsForLocal161SpecialAccountsForAlternatePayee(busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, 0, ldecAlternatePayeeBenefitAmt, ldecLifeConversionFactor);
                            }
                        }

                        break;

                    default:
                        break;
                }
            }
        }

        private void LoadCalculationOptionsForIAP(string astrBenefitOptionValue, decimal adecIAPAnnuity, decimal adecIAPBalance, decimal ldecAnnunityAdjustmentMultiplier)
        {
            decimal ldecIAPAnnuity = new decimal();
            decimal ldecIAPBalance = new decimal();
            ldecIAPAnnuity = adecIAPAnnuity;
            ldecIAPBalance = adecIAPBalance;

            ibusCalculation = new busCalculation();
            busBenefitCalculationOptions lbusBenefitCalculationOptions;
            int lintPlanBenefitId = busConstant.ZERO_INT;
            //We do not need this --since there is a property called as idecSurvivorFullAge which has the FULL AGE
            //int lintBeneficiaryAge = busConstant.ZERO_INT;

            decimal ldecBenefitOptionFactor = 1;
            int lintRemainder = 0;

            decimal ldecSurvivorFactor = 1;
            if (astrBenefitOptionValue == busConstant.QJ50)
            {
                ldecSurvivorFactor = 0.5M;
            }
            else if (astrBenefitOptionValue == busConstant.JS75)
            {
                ldecSurvivorFactor = 0.75M;
            }
            else if (astrBenefitOptionValue == busConstant.J100)
            {
                ldecSurvivorFactor = 1.0M;
            }
            else if (astrBenefitOptionValue == busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY)
            {
                ldecSurvivorFactor = 0.0M;
            }
            else if (astrBenefitOptionValue == busConstant.LIFE)
            {
                ldecSurvivorFactor = 0.0M;
            }

            ldecBenefitOptionFactor = 1;
            lintPlanBenefitId = ibusCalculation.GetPlanBenefitId(busConstant.IAP_PLAN_ID, astrBenefitOptionValue);

            if (astrBenefitOptionValue == busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY || astrBenefitOptionValue == busConstant.LIFE)
            {
                ldecBenefitOptionFactor = ibusCalculation.GetBenefitOptionFactor(busConstant.BENEFIT_TYPE_WITHDRAWAL, lintPlanBenefitId,
                                                                    Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge), busConstant.ZERO_INT) * 12;
            }
            else
            {
                ldecBenefitOptionFactor = ibusCalculation.GetBenefitOptionFactor(busConstant.BENEFIT_TYPE_WITHDRAWAL, lintPlanBenefitId,
                                                                      Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge), Convert.ToInt16(this.icdoBenefitCalculationHeader.idecSurvivorFullAge)) * 12;
            }

            lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
            ldecIAPAnnuity = busConstant.ZERO_DECIMAL;
            ldecIAPAnnuity = Math.Round(ldecIAPBalance / ldecBenefitOptionFactor);
            Math.DivRem(Convert.ToInt32(ldecIAPAnnuity), 10, out lintRemainder);
            if (lintRemainder > 0)
            {
                ldecIAPAnnuity = ldecIAPAnnuity - lintRemainder;
            }
            if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE)
            {
                lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, ldecBenefitOptionFactor, ldecAnnunityAdjustmentMultiplier == 0? ldecIAPAnnuity: ldecIAPAnnuity * ldecAnnunityAdjustmentMultiplier, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                       this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                        astrBenefitOptionValue, ldecIAPAnnuity * ldecSurvivorFactor);
            }
            else
            {
                lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, ldecBenefitOptionFactor, ldecIAPAnnuity, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                       this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                        astrBenefitOptionValue, ldecIAPAnnuity * ldecSurvivorFactor);
            }
               

            if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL)
            {
                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                                                           item.icdoBenefitCalculationDetail.l52_spl_acc_flag.IsNullOrEmpty() &&
                                                           item.icdoBenefitCalculationDetail.l161_spl_acc_flag.IsNullOrEmpty()).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
            }
            else
            {
                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
            }

        }


        private void LoadCalculationOptionsForLocal52SpecialAccounts(string astrBenefitOptionValue, decimal adecLocal52SpecialAccountAnnuity, decimal adecLocal52SpecialAccountBalance)
        {
            decimal ldecLocal52SpecialAccountAnnuity = new decimal();
            decimal ldecLocal52SpecialAccountBalance = new decimal();

            decimal ldecSurvivorFactor = 1;
            decimal ldecAnnunityAdjustmentMultiplier = this.GetAnnunityMultiplier();
            if (astrBenefitOptionValue == busConstant.QJ50)
            {
                ldecSurvivorFactor = 0.5M;
            }
            else if (astrBenefitOptionValue == busConstant.JS75)
            {
                ldecSurvivorFactor = 0.75M;
            }
            else if (astrBenefitOptionValue == busConstant.LIFE || astrBenefitOptionValue == busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY)
            {
                ldecSurvivorFactor = 0.0M;
            }

            ldecLocal52SpecialAccountAnnuity = adecLocal52SpecialAccountAnnuity;
            ldecLocal52SpecialAccountBalance = adecLocal52SpecialAccountBalance;

            decimal ldecBenefitOptionFactor = 1;
            busBenefitCalculationOptions lbusBenefitCalculationOptions;
            int lintPlanBenefitId = busConstant.ZERO_INT;

            lintPlanBenefitId = ibusCalculation.GetPlanBenefitId(busConstant.IAP_PLAN_ID, astrBenefitOptionValue);

            if (astrBenefitOptionValue == busConstant.LIFE || astrBenefitOptionValue == busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY)
            {
                ldecBenefitOptionFactor = ibusCalculation.GetBenefitOptionFactor(busConstant.BENEFIT_TYPE_WITHDRAWAL, lintPlanBenefitId,
                                                      Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge), 0) * 12;
            }
            else
            {
                ldecBenefitOptionFactor = ibusCalculation.GetBenefitOptionFactor(busConstant.BENEFIT_TYPE_WITHDRAWAL, lintPlanBenefitId,
                                                                      Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge), Convert.ToInt32(this.icdoBenefitCalculationHeader.idecSurvivorFullAge)) * 12;
            }

            int lintRemainder = 0;

            lintRemainder = 0;
            ldecLocal52SpecialAccountAnnuity = busConstant.ZERO_DECIMAL;
            ldecLocal52SpecialAccountAnnuity = Math.Round(ldecLocal52SpecialAccountBalance / ldecBenefitOptionFactor, 0);
            Math.DivRem(Convert.ToInt32(ldecLocal52SpecialAccountAnnuity), 10, out lintRemainder);
            if (lintRemainder > 0)
            {
                ldecLocal52SpecialAccountAnnuity = ldecLocal52SpecialAccountAnnuity - lintRemainder;
            }
            lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
            if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE)
            {
                lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, ldecBenefitOptionFactor, ldecAnnunityAdjustmentMultiplier == 0? ldecLocal52SpecialAccountAnnuity: ldecLocal52SpecialAccountAnnuity * ldecAnnunityAdjustmentMultiplier, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
           this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
           astrBenefitOptionValue, ldecLocal52SpecialAccountAnnuity * ldecSurvivorFactor, false, false, true);
            }
            else
            {
                lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, ldecBenefitOptionFactor, ldecLocal52SpecialAccountAnnuity, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
           this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
           astrBenefitOptionValue, ldecLocal52SpecialAccountAnnuity * ldecSurvivorFactor, false, false, true);

            }
              

            if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL)
            {
                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID && item.icdoBenefitCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
            }
            else
            {
                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
            }

        }

        private void LoadCalculationOptionsForLocal161SpecialAccounts(string astrBenefitOptionValue, decimal adecLocal161SpecialAccountAnnuity, decimal adecLocal161SpecialAccountBalance)
        {
            decimal ldecLocal161SpecialAccountAnnuity = new decimal();
            decimal ldecLocal161SpecialAccountBalance = new decimal();
            decimal ldecAnnunityAdjustmentMultiplier = this.GetAnnunityMultiplier();
            decimal ldecSurvivorFactor = 1;
            if (astrBenefitOptionValue == busConstant.QJ50)
            {
                ldecSurvivorFactor = 0.5M;
            }
            else if (astrBenefitOptionValue == busConstant.JS75)
            {
                ldecSurvivorFactor = 0.75M;
            }
            else if (astrBenefitOptionValue == busConstant.LIFE || astrBenefitOptionValue == busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY)
            {
                ldecSurvivorFactor = 0.0M;
            }


            ldecLocal161SpecialAccountAnnuity = adecLocal161SpecialAccountAnnuity;
            ldecLocal161SpecialAccountBalance = adecLocal161SpecialAccountBalance;

            busBenefitCalculationOptions lbusBenefitCalculationOptions;
            int lintPlanBenefitId = busConstant.ZERO_INT;
            decimal ldecBenefitOptionFactor = 1;
            int lintRemainder = 0;

            lintPlanBenefitId = ibusCalculation.GetPlanBenefitId(busConstant.IAP_PLAN_ID, astrBenefitOptionValue);

            if (astrBenefitOptionValue == busConstant.LIFE || astrBenefitOptionValue == busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY)
            {
                ldecBenefitOptionFactor = ibusCalculation.GetBenefitOptionFactor(busConstant.BENEFIT_TYPE_WITHDRAWAL, lintPlanBenefitId,
                                                      Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge), 0) * 12;
            }
            else
            {
                ldecBenefitOptionFactor = ibusCalculation.GetBenefitOptionFactor(busConstant.BENEFIT_TYPE_WITHDRAWAL, lintPlanBenefitId,
                                                                      Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge), Convert.ToInt32(this.icdoBenefitCalculationHeader.idecSurvivorFullAge)) * 12;
            }



            lintRemainder = 0;
            ldecLocal161SpecialAccountAnnuity = busConstant.ZERO_DECIMAL;
            ldecLocal161SpecialAccountAnnuity = Math.Round(ldecLocal161SpecialAccountBalance / ldecBenefitOptionFactor, 0);
            Math.DivRem(Convert.ToInt32(ldecLocal161SpecialAccountAnnuity), 10, out lintRemainder);
            if (lintRemainder > 0)
            {
                ldecLocal161SpecialAccountAnnuity = ldecLocal161SpecialAccountAnnuity - lintRemainder;
            }
            lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
            if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE)
            {
                lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, ldecBenefitOptionFactor, ldecAnnunityAdjustmentMultiplier ==0 ? ldecLocal161SpecialAccountAnnuity: ldecLocal161SpecialAccountAnnuity * ldecAnnunityAdjustmentMultiplier, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
             this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
            astrBenefitOptionValue, ldecLocal161SpecialAccountAnnuity * ldecSurvivorFactor, false, false, false, true);
            }
            else
            {
                lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, ldecBenefitOptionFactor, ldecLocal161SpecialAccountAnnuity, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
             this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
            astrBenefitOptionValue, ldecLocal161SpecialAccountAnnuity * ldecSurvivorFactor, false, false, false, true);
            }
                

            if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL)
            {
                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID && item.icdoBenefitCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
            }
            else
            {
                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
            }

        }


        private void LoadCalculationOptionsForLocal52SpecialAccountsForAlternatePayee(string astrBenefitOptionValue, int aintDROCommencementYear, decimal adecLocal52SpecialAccountBalance, decimal adecLifeConversionFactor)
        {
            decimal ldecLocal52SpecialAccountBalance = new decimal();


            ldecLocal52SpecialAccountBalance = adecLocal52SpecialAccountBalance;

            decimal ldecBenefitOptionFactor = 1;
            busBenefitCalculationOptions lbusBenefitCalculationOptions;
            int lintPlanBenefitId = busConstant.ZERO_INT;

            lintPlanBenefitId = ibusCalculation.GetPlanBenefitId(busConstant.IAP_PLAN_ID, astrBenefitOptionValue);

            //if (astrBenefitOptionValue == busConstant.LUMP_SUM)
            //{
            //    DataTable ldtblBenefitOptionFactor = Select("cdoBenefitProvisionLumpsumFactor.GetLumpSumFactor", new object[2] { Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge), aintDROCommencementYear });
            //    if (ldtblBenefitOptionFactor.Rows.Count > 0)
            //    {
            //        ldecBenefitOptionFactor = Convert.ToDecimal(ldtblBenefitOptionFactor.Rows[0][0]) * 12;
            //    }
            //}
            //else if (astrBenefitOptionValue == busConstant.LIFE)
            //{
            //    ldecBenefitOptionFactor = 1;
            //}
            //else
            //{
            //    DataTable ldtblBenefitOptionFactor = Select("cdoQdroFactor.LoadQdroFactorByAgeandBenefitOption", new object[2] { astrBenefitOptionValue, Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge) });
            //    if (ldtblBenefitOptionFactor.Rows.Count > 0)
            //    {
            //        ldecBenefitOptionFactor = Convert.ToDecimal(ldtblBenefitOptionFactor.Rows[0][0]);
            //    }
            //}

            lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };


            lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, ldecBenefitOptionFactor, ldecLocal52SpecialAccountBalance * ldecBenefitOptionFactor, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_BENEFICIARY,
             this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
             astrBenefitOptionValue, new decimal(), false, false, true);


            if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL)
            {
                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID && item.icdoBenefitCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
            }
            else
            {
                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
            }

        }

        private void LoadCalculationOptionsForLocal161SpecialAccountsForAlternatePayee(string astrBenefitOptionValue, int aintDROCommencementYear, decimal adecLocal161SpecialAccountBalance, decimal adecLifeConversionFactor)
        {
            decimal ldecLocal161SpecialAccountBalance = new decimal();

            ldecLocal161SpecialAccountBalance = adecLocal161SpecialAccountBalance;

            busBenefitCalculationOptions lbusBenefitCalculationOptions;
            int lintPlanBenefitId = busConstant.ZERO_INT;
            decimal ldecBenefitOptionFactor = 1;

            lintPlanBenefitId = ibusCalculation.GetPlanBenefitId(busConstant.IAP_PLAN_ID, astrBenefitOptionValue);

            //if (astrBenefitOptionValue == busConstant.LUMP_SUM)
            //{
            //    DataTable ldtblBenefitOptionFactor = Select("cdoBenefitProvisionLumpsumFactor.GetLumpSumFactor", new object[2] { Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge), aintDROCommencementYear });
            //    if (ldtblBenefitOptionFactor.Rows.Count > 0)
            //    {
            //        ldecBenefitOptionFactor = Convert.ToDecimal(ldtblBenefitOptionFactor.Rows[0][0]) * 12;
            //    }
            //}
            //else if (astrBenefitOptionValue == busConstant.LIFE)
            //{
            //    ldecBenefitOptionFactor = 1;
            //}
            //else
            //{
            //    DataTable ldtblBenefitOptionFactor = Select("cdoQdroFactor.LoadQdroFactorByAgeandBenefitOption", new object[2] { astrBenefitOptionValue, Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge) });
            //    if (ldtblBenefitOptionFactor.Rows.Count > 0)
            //    {
            //        ldecBenefitOptionFactor = Convert.ToDecimal(ldtblBenefitOptionFactor.Rows[0][0]);
            //    }
            //}


            lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };


            lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, ldecBenefitOptionFactor, ldecLocal161SpecialAccountBalance * ldecBenefitOptionFactor, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_BENEFICIARY,
             this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
            astrBenefitOptionValue, new decimal(), false, false, false, true);

            if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL)
            {
                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID && item.icdoBenefitCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
            }
            else
            {
                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
            }

        }


        public void SpawnFinalWithdrawalCalculation(int aintBenefitApplicationDetailId, int aintPersonAccountId, int aintPlanId, string astrPlanCode, string astrBenefitOptionValue, string astrAdjustmentFlag = "")
        {
            this.PopulateInitialDataBenefitCalculationDetails(aintBenefitApplicationDetailId, aintPersonAccountId, aintPlanId, astrPlanCode, DateTime.MinValue, null);
            decimal ldecPartialUVHPInterest = 0;
            decimal ldecPartialEEInterest = 0;

            #region SWITCH CASE - INITIATE CALCULATION BASED ON THE PLAN
            if (!this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.IsNullOrEmpty())
            {
                switch (astrPlanCode)
                {
                    case busConstant.IAP:
                        this.CalculateIAPBenefitAmount(astrBenefitOptionValue, astrAdjustmentFlag);
                        break;

                    case busConstant.MPIPP:
                        int lintPersonAccountId = this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).First().icdoPersonAccount.person_account_id;

                        DateTime ldtWithdrawalDate = this.icdoBenefitCalculationHeader.retirement_date;
                        decimal ldecPriorYearUVHPInterest = decimal.Zero;
                        decimal ldecPriorYearEEInterest = decimal.Zero;
                        ldecPartialUVHPInterest = this.ibusCalculation.CalculatePartialUVHPInterest(ldtWithdrawalDate, lintPersonAccountId, out ldecPartialUVHPInterest);
                        ldecPartialEEInterest = this.ibusCalculation.CalculatePartialEEInterest(ldtWithdrawalDate,
                                            this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).First(),
                                            false, true, iclbPersonAccountRetirementContribution, out ldecPriorYearEEInterest);

                        if (this.iblnCalcualteUVHPBenefit || this.iblnCalcualteNonVestedEEBenefit)
                        {
                            #region UVHP Benefit Details
                            if (iclbPersonAccountRetirementContribution != null
                                && iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId
                                && item.icdoPersonAccountRetirementContribution.uvhp_amount > Convert.ToDecimal(0)).Count() > 0)
                            {
                                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).First().
                                 icdoBenefitCalculationDetail.total_uvhp_contribution_amount = idecUVHPContribution =
                                        (from contribution in this.iclbPersonAccountRetirementContribution
                                         where contribution.icdoPersonAccountRetirementContribution.person_account_id ==
                                             lintPersonAccountId &&
                                         contribution.icdoPersonAccountRetirementContribution.computational_year <= icdoBenefitCalculationHeader.retirement_date.Year
                                         select contribution.icdoPersonAccountRetirementContribution.uvhp_amount).Sum();



                                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).First().
                                    icdoBenefitCalculationDetail.total_uvhp_interest_amount = idecUVHPInterest =
                                    (from contribution in this.iclbPersonAccountRetirementContribution
                                     where contribution.icdoPersonAccountRetirementContribution.person_account_id ==
                                         lintPersonAccountId &&
                                         contribution.icdoPersonAccountRetirementContribution.computational_year <= icdoBenefitCalculationHeader.retirement_date.Year
                                     select contribution.icdoPersonAccountRetirementContribution.uvhp_int_amount).Sum() + ldecPartialUVHPInterest;


                            }
                            # endregion

                            #region EE Benefit Details

                            if (this.ibusPerson.iclbPersonAccount != null && (!this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.person_account_id == lintPersonAccountId).IsNullOrEmpty()) &&
                                this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.person_account_id == lintPersonAccountId).First().icdoPersonAccount.idecNonVestedEE > 0)
                            {
                                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).First().icdoBenefitCalculationDetail.non_vested_ee_amount = idecNonVestedEEContribution =
                                       this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.person_account_id == lintPersonAccountId).First().icdoPersonAccount.idecNonVestedEE;

                                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).First().icdoBenefitCalculationDetail.non_vested_ee_interest = idecNonVestedEEInterest =
                                    this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.person_account_id == lintPersonAccountId).First().icdoPersonAccount.idecNonVestedEEInterest + ldecPartialEEInterest;

                            }

                            # endregion
                        }

                        this.CalculateFinalBenefitForUVHPBenefitOptions(astrBenefitOptionValue, this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault());

                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().icdoBenefitCalculationDetail.ee_derived_benefit_amount = ibusCalculation.CalculateEEDerivedBenefitAsOfRetirementDate(this, null);

                        break;
                }
            }

            #endregion
        }

        public void SpawnFinalWithdrawalCalculationForAlternatePayee(busQdroApplication abusQdroApplication, int aintBenefitApplicationDetailId, int aintPersonAccountId, int aintPlanId, string astrPlanCode, string astrBenefitOptionValue
            , string astrDROModelValue)
        {
            this.PopulateInitialDataBenefitCalculationDetails(aintBenefitApplicationDetailId, aintPersonAccountId, aintPlanId, astrPlanCode, DateTime.MinValue, null, null, astrDROModelValue);

            #region SWITCH CASE - INITIATE CALCULATION BASED ON THE PLAN
            //Ticket #100420
            if (this.ibusBenefitApplication.ibusQdroApplication.IsNull())
            {
                this.ibusBenefitApplication.ibusQdroApplication = abusQdroApplication;
            }
            if (!this.ibusBenefitApplication.ibusQdroApplication.ibusParticipant.iclbPersonAccount.IsNullOrEmpty())
            {
                busPersonAccountEligibility lbusPersonAccountEligibility = new busPersonAccountEligibility();
                DateTime ldtForfeitureDate = new DateTime();

                switch (astrPlanCode)
                {
                    case busConstant.IAP:

                        this.CalculateIAPBenefitAmountForAlternatePayee(abusQdroApplication, astrBenefitOptionValue);
                        break;

                    case busConstant.MPIPP:

                        this.CalculateUVHPBenefitAmountForAlternatePayee(abusQdroApplication, astrBenefitOptionValue);
                        break;
                }
            }
            #endregion
        }

        private void CalculateUVHPBenefitAmountForAlternatePayee(busQdroApplication abusQdroApplication, string astrBenefitOptionValue)
        {
            decimal ldecTotalHrsWorkedBetTwoDates = 0, ldecTotalHours = 0, ldecAltPayeeFraction = 0, ldecQdroPercent = 0, ldecFlatAmount = 0,
                    ldecFlatPercent = 0, ldecTotalUVHPContribution = 0, ldecTotalEEContribution = 0;
            int ldecParticipantAge = 0, ldecAlternatePayeeAge = 0;
            decimal ldecBenefitBeforeConversion = 0, ldecLifeConversionFactor = 1, ldecAlternatePayeeBenefitAmt = 0;

            ldecParticipantAge = busGlobalFunctions.CalulateAge(abusQdroApplication.ibusParticipant.icdoPerson.idtDateofBirth, abusQdroApplication.icdoDroApplication.dro_commencement_date);
            ldecAlternatePayeeAge = busGlobalFunctions.CalulateAge(abusQdroApplication.ibusAlternatePayee.icdoPerson.idtDateofBirth, abusQdroApplication.icdoDroApplication.dro_commencement_date);

            int lintPersonAccountId = abusQdroApplication.ibusParticipant.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).First().icdoPersonAccount.person_account_id;
            DateTime ldtWithdrawalDate = this.icdoBenefitCalculationHeader.retirement_date;

            if (this.iblnCalcualteUVHPBenefit)
            {
                ldecTotalUVHPContribution = this.ibusCalculation.FetchUVHPAmountandInterest(busConstant.BOOL_FALSE, null, this.iclbBenefitCalculationDetail, this, null,
                                            lintPersonAccountId, ldtWithdrawalDate);


                ldecFlatAmount = abusQdroApplication.iclbDroBenefitDetails.Where(item => item.icdoDroBenefitDetails.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoDroBenefitDetails.uvhp_flag == busConstant.FLAG_YES).First().
                      icdoDroBenefitDetails.benefit_amt;
                ldecFlatPercent = abusQdroApplication.iclbDroBenefitDetails.Where(item => item.icdoDroBenefitDetails.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoDroBenefitDetails.uvhp_flag == busConstant.FLAG_YES).First().
                        icdoDroBenefitDetails.benefit_flat_perc;
                ldecQdroPercent = abusQdroApplication.iclbDroBenefitDetails.Where(item => item.icdoDroBenefitDetails.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoDroBenefitDetails.uvhp_flag == busConstant.FLAG_YES).First().
                        icdoDroBenefitDetails.benefit_perc;
            }
            else
            {
                ldecTotalEEContribution = this.ibusCalculation.FetchEEAmountandInterest(busConstant.BOOL_FALSE, null, this.iclbBenefitCalculationDetail, this, null,
                                          abusQdroApplication.ibusParticipant.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).First(),
                                          ldtWithdrawalDate, iclbPersonAccountRetirementContribution, icdoBenefitCalculationHeader.calculation_type_value);

                ldecFlatAmount = abusQdroApplication.iclbDroBenefitDetails.Where(item => item.icdoDroBenefitDetails.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoDroBenefitDetails.ee_flag == busConstant.FLAG_YES).First().
                        icdoDroBenefitDetails.benefit_amt;
                ldecFlatPercent = abusQdroApplication.iclbDroBenefitDetails.Where(item => item.icdoDroBenefitDetails.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoDroBenefitDetails.ee_flag == busConstant.FLAG_YES).First().
                        icdoDroBenefitDetails.benefit_flat_perc;
                ldecQdroPercent = abusQdroApplication.iclbDroBenefitDetails.Where(item => item.icdoDroBenefitDetails.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoDroBenefitDetails.ee_flag == busConstant.FLAG_YES).First().
                        icdoDroBenefitDetails.benefit_perc;
            }

            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().icdoBenefitCalculationDetail.ee_derived_benefit_amount = ibusCalculation.CalculateEEDerivedBenefitAsOfRetirementDate(this, null);
            #region Get Alternate Payee Balance

            DateTime ldtForfeitureDate = new DateTime();

            if (this.ibusBenefitApplication.ibusTempPersonAccountEligibility != null)
                ldtForfeitureDate = this.ibusBenefitApplication.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.forfeiture_date;

            this.ibusCalculation.GetTotalHoursWorked(abusQdroApplication.ibusParticipant.icdoPerson.istrSSNNonEncrypted, busConstant.MPIPP,
                                abusQdroApplication.icdoDroApplication.dro_commencement_date, ldtForfeitureDate, 0, ref ldecTotalHours);
            //Prodfix_02/07/2013_4
            //Prod PIR : 81 : 01/21/2013
            ldecTotalHrsWorkedBetTwoDates = this.ibusCalculation.GetProratedHoursBetweenTwoDates(abusQdroApplication.icdoDroApplication.date_of_marriage, abusQdroApplication.icdoDroApplication.date_of_divorce,
                this.ibusBenefitApplication.aclbPersonWorkHistory_MPI, busConstant.MPIPP, ldtForfeitureDate, abusQdroApplication.ibusParticipant.icdoPerson.istrSSNNonEncrypted, busConstant.MPIPP_PLAN_ID);

            //Calculate Alternate Payee Fraction

            ldecAltPayeeFraction = Math.Round(((ldecTotalHrsWorkedBetTwoDates / ldecTotalHours) * ldecQdroPercent) / 100, 3);//PIR 963


            if (this.iblnCalcualteUVHPBenefit)
            {
                iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoBenefitCalculationDetail
                   .uvhp_flag == busConstant.FLAG_YES).FirstOrDefault().icdoBenefitCalculationDetail.total_uvhp_contribution_amount =
                   this.ibusCalculation.CalculateBenefitAmtBeforeConversion(iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoBenefitCalculationDetail
                   .uvhp_flag == busConstant.FLAG_YES).FirstOrDefault().icdoBenefitCalculationDetail.total_uvhp_contribution_amount, ldecAltPayeeFraction, ldecFlatAmount, ldecFlatPercent);

                iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoBenefitCalculationDetail
                  .uvhp_flag == busConstant.FLAG_YES).FirstOrDefault().icdoBenefitCalculationDetail.total_uvhp_interest_amount =
                  this.ibusCalculation.CalculateBenefitAmtBeforeConversion(iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoBenefitCalculationDetail
                  .uvhp_flag == busConstant.FLAG_YES).FirstOrDefault().icdoBenefitCalculationDetail.total_uvhp_interest_amount, ldecAltPayeeFraction, ldecFlatAmount, ldecFlatPercent);

                ldecBenefitBeforeConversion = iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoBenefitCalculationDetail
                   .uvhp_flag == busConstant.FLAG_YES).FirstOrDefault().icdoBenefitCalculationDetail.total_uvhp_contribution_amount +
                   iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoBenefitCalculationDetail
                  .uvhp_flag == busConstant.FLAG_YES).FirstOrDefault().icdoBenefitCalculationDetail.total_uvhp_interest_amount;
            }
            else
            {
                iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoBenefitCalculationDetail
                   .ee_flag == busConstant.FLAG_YES).FirstOrDefault().icdoBenefitCalculationDetail.non_vested_ee_amount =
                   this.ibusCalculation.CalculateBenefitAmtBeforeConversion(iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoBenefitCalculationDetail
                   .ee_flag == busConstant.FLAG_YES).FirstOrDefault().icdoBenefitCalculationDetail.non_vested_ee_amount, ldecAltPayeeFraction, ldecFlatAmount, ldecFlatPercent);

                iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoBenefitCalculationDetail
                  .ee_flag == busConstant.FLAG_YES).FirstOrDefault().icdoBenefitCalculationDetail.non_vested_ee_interest =

                  this.ibusCalculation.CalculateBenefitAmtBeforeConversion(iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoBenefitCalculationDetail
                  .ee_flag == busConstant.FLAG_YES).FirstOrDefault().icdoBenefitCalculationDetail.non_vested_ee_interest, ldecAltPayeeFraction, ldecFlatAmount, ldecFlatPercent);

                ldecBenefitBeforeConversion = iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoBenefitCalculationDetail
                   .ee_flag == busConstant.FLAG_YES).FirstOrDefault().icdoBenefitCalculationDetail.non_vested_ee_amount +
                   iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoBenefitCalculationDetail
                  .ee_flag == busConstant.FLAG_YES).FirstOrDefault().icdoBenefitCalculationDetail.non_vested_ee_interest;   //PIR 1054 
            }

            if (abusQdroApplication.icdoDroApplication.life_conversion_factor_flag == busConstant.FLAG_YES)
            {
                ldecLifeConversionFactor = this.ibusCalculation.CalculateLifeConversionFactor(ldecParticipantAge, ldecAlternatePayeeAge);
            }

            ldecAlternatePayeeBenefitAmt = ldecBenefitBeforeConversion * ldecLifeConversionFactor;

            if (this.iblnCalcualteUVHPBenefit)
            {
                iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoBenefitCalculationDetail
                   .uvhp_flag == busConstant.FLAG_YES).FirstOrDefault().icdoBenefitCalculationDetail.alternate_payee_pure_contribution = ldecBenefitBeforeConversion * ldecAltPayeeFraction;

                //iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoBenefitCalculationDetail
                //   .uvhp_flag == busConstant.FLAG_YES).FirstOrDefault().icdoBenefitCalculationDetail.total_uvhp_contribution_amount = ldecBenefitBeforeConversion;
            }
            else
            {
                iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoBenefitCalculationDetail
                   .ee_flag == busConstant.FLAG_YES).FirstOrDefault().icdoBenefitCalculationDetail.alternate_payee_pure_contribution = ldecBenefitBeforeConversion * ldecAltPayeeFraction;

                ////iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().icdoBenefitCalculationDetail
                ////    .non_vested_ee_amount = ldecBenefitBeforeConversion;
            }

            this.CalculateFinalBenefitForUVHPBenefitOptionsForAlternatePayee(astrBenefitOptionValue, ldecBenefitBeforeConversion * ldecLifeConversionFactor, ldecBenefitBeforeConversion,
                  abusQdroApplication.icdoDroApplication.dro_commencement_date.Year, ldecLifeConversionFactor);

            #endregion

        }

        private void CalculateFinalBenefitForUVHPBenefitOptions(string astrBenefitOptionValue, busPersonAccount abusPersonAccount)
        {
            busBenefitCalculationOptions lbusBenefitCalculationOptions;
            // decimal ldecEEAnnuity;
            decimal ldecEEUVHPAnnuity;
            decimal ldecMonthlyAnnuity;
            //decimal ldecTotalEE = new decimal();
            //decimal ldecTotalUVHP = new decimal();
            decimal ldecTotalEEUVHP = new decimal();
            bool lblnCheckIfSpouse = busConstant.BOOL_FALSE;

            lblnCheckIfSpouse = this.ibusCalculation.CheckIfSpouse(this.icdoBenefitCalculationHeader.person_id,
                    this.icdoBenefitCalculationHeader.beneficiary_person_id);

            // ldecTotalEE = idecNonVestedEEContribution + idecNonVestedEEInterest;

            ldecTotalEEUVHP = idecTotalEEUVHP = idecUVHPContribution + idecUVHPInterest + idecNonVestedEEContribution + idecNonVestedEEInterest;
            if (idecTotalEEUVHP > 0)
            {
                // Ticket#70143
                  DataTable ldtAlternateParticipantIsPaid = Select("cdoBenefitCalculationDetail.CheckAlternateParticipantIsPaid", new object[1] { this.icdoBenefitCalculationHeader.person_id });
                
                if (ldtAlternateParticipantIsPaid != null && ldtAlternateParticipantIsPaid.Rows.Count > 0 && Convert.ToInt32(ldtAlternateParticipantIsPaid.Rows[0][0]) > 0)
                {; }
                else
                {
                    if (iclbBenefitCalculationDetail != null && iclbBenefitCalculationDetail.Count > 0 && (this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).Count() > 0))
                    {
                        //Process QDRO Offset
                        this.ibusCalculation.ProcessQDROOffset(this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).First()
                               , this.icdoBenefitCalculationHeader.person_id, ref ldecTotalEEUVHP, ablnEEFlag: true, ablnUVHPFlag: true, astrCalculationType: this.icdoBenefitCalculationHeader.calculation_type_value);
                    }

                }

            }



            //ldecTotalUVHP = idecUVHPInterest + idecUVHPContribution;
            //if (ldecTotalUVHP > 0)
            //{
            //    //Process QDRO Offset
            //    this.ibusCalculation.ProcessQDROOffset(this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).First()
            //        , this.icdoBenefitCalculationHeader.person_id, ref ldecTotalUVHP, ablnUVHPFlag: true, astrCalculationType: this.icdoBenefitCalculationHeader.calculation_type_value);

            //    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().icdoBenefitCalculationDetail.idecRemainingBenefits = ldecTotalUVHP;

            //}


            //idecTotalEEUVHP = idecUVHPContribution + idecUVHPInterest + idecNonVestedEEContribution + idecNonVestedEEInterest;
            switch (astrBenefitOptionValue)
            {

                case busConstant.CodeValueAll:

                    if (this.idecTotalEEUVHP > busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT)
                    {
                        //ldecEEAnnuity = new decimal();
                        ldecEEUVHPAnnuity = new decimal();
                        ldecMonthlyAnnuity = new decimal();

                        CalculateEEUVHPAnnuityOption(busConstant.LIFE, ldecTotalEEUVHP, out ldecEEUVHPAnnuity, out ldecMonthlyAnnuity);
                        //if (this.iblnCalcualteNonVestedEEBenefit && this.iblnCalcualteUVHPBenefit)
                        //{                              
                        lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                        lbusBenefitCalculationOptions.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.MPIPP_PLAN_ID, busConstant.LIFE), ldecMonthlyAnnuity,
                        ldecEEUVHPAnnuity, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER, this.icdoBenefitCalculationHeader.person_id,
                        this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.LIFE, new decimal(), true, true);

                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
                        // }

                        //lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                        //lbusBenefitCalculationOptions.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.MPIPP_PLAN_ID, busConstant.LIFE), ldecMonthlyAnnuity, ldecUVHPAnnuity, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER, this.icdoBenefitCalculationHeader.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.LIFE, new decimal(), false, true);
                        //this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);

                        if (this.icdoBenefitCalculationHeader.beneficiary_person_id > 0 && lblnCheckIfSpouse)
                        {
                            CalculateEEUVHPAnnuityOption(busConstant.QJ50, ldecTotalEEUVHP, out ldecEEUVHPAnnuity, out ldecMonthlyAnnuity);
                            //if (this.iblnCalcualteNonVestedEEBenefit && this.iblnCalcualteUVHPBenefit)
                            // {

                            lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                            lbusBenefitCalculationOptions.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.MPIPP_PLAN_ID, busConstant.QJ50), ldecMonthlyAnnuity, ldecEEUVHPAnnuity,
                                busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER, this.icdoBenefitCalculationHeader.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                busConstant.QJ50, (ldecEEUVHPAnnuity * 50) / 100, true, true);
                            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
                            // }

                            //lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                            //lbusBenefitCalculationOptions.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.MPIPP_PLAN_ID, busConstant.QJ50), ldecMonthlyAnnuity, ldecUVHPAnnuity, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER, this.icdoBenefitCalculationHeader.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.QJ50, (ldecUVHPAnnuity * 50) / 100, false, true);
                            //this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
                        }

                    }
                    //else
                    //{
                    //if (this.iblnCalcualteNonVestedEEBenefit)
                    //{
                    lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                    lbusBenefitCalculationOptions.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.MPIPP_PLAN_ID, busConstant.LUMP_SUM), 1.0M, ldecTotalEEUVHP,
                     busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER, this.icdoBenefitCalculationHeader.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                     busConstant.LUMP_SUM, new decimal(), true, true);
                    if (iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).Count() > 0)
                    {
                        iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
                    }
                    // }

                    //lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                    //lbusBenefitCalculationOptions.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.MPIPP_PLAN_ID, busConstant.LUMP_SUM), 1.0M, ldecTotalUVHP, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER, this.icdoBenefitCalculationHeader.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.LUMP_SUM, new decimal(), false, true);
                    //iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
                    //}
                    break;

                case busConstant.LIFE_ANNUTIY:
                    //idecTotalEEUVHP = idecUVHPContribution + idecUVHPInterest + idecNonVestedEEContribution + idecNonVestedEEInterest;
                    if (this.idecTotalEEUVHP > busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT)
                    {
                        //ldecEEAnnuity = new decimal();
                        ldecEEUVHPAnnuity = new decimal();
                        ldecMonthlyAnnuity = new decimal();
                        CalculateEEUVHPAnnuityOption(busConstant.LIFE, ldecTotalEEUVHP, out ldecEEUVHPAnnuity, out ldecMonthlyAnnuity);

                        //if (this.iblnCalcualteNonVestedEEBenefit && this.iblnCalcualteUVHPBenefit)
                        //{
                        lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                        lbusBenefitCalculationOptions.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.MPIPP_PLAN_ID, busConstant.LIFE), ldecMonthlyAnnuity,
                        ldecEEUVHPAnnuity, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER, this.icdoBenefitCalculationHeader.person_id,
                        this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.LIFE, new decimal(), true, true);

                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoBenefitCalculationDetail.ee_flag == busConstant.FLAG_YES).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
                        //}

                        //if (this.iblnCalcualteUVHPBenefit)
                        //{
                        //    lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                        //    lbusBenefitCalculationOptions.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.MPIPP_PLAN_ID, busConstant.LIFE), ldecMonthlyAnnuity, ldecEEUVHPAnnuity, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER, this.icdoBenefitCalculationHeader.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.LIFE, new decimal(), false, true);
                        //    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoBenefitCalculationDetail.uvhp_flag == busConstant.FLAG_YES).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
                        //}
                    }
                    break;

                case busConstant.LUMP_SUM:
                    // if (this.iblnCalcualteNonVestedEEBenefit && this.iblnCalcualteUVHPBenefit)
                    // {
                    lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                    lbusBenefitCalculationOptions.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.MPIPP_PLAN_ID, busConstant.LUMP_SUM), 1.0M, ldecTotalEEUVHP,
                    busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER, this.icdoBenefitCalculationHeader.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                    busConstant.LUMP_SUM, new decimal(), true, true);

                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoBenefitCalculationDetail.ee_flag == busConstant.FLAG_YES).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
                    //}

                    //if (this.iblnCalcualteUVHPBenefit)
                    //{
                    //    lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                    //    lbusBenefitCalculationOptions.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.MPIPP_PLAN_ID, busConstant.LUMP_SUM), 1.0M, ldecTotalUVHP, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER, this.icdoBenefitCalculationHeader.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.LUMP_SUM, new decimal(), false, true);
                    //    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoBenefitCalculationDetail.uvhp_flag == busConstant.FLAG_YES).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
                    //}

                    break;

                case busConstant.QJ50:
                    if (this.idecTotalEEUVHP > busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT && lblnCheckIfSpouse)
                    {
                        //ldecEEAnnuity = new decimal();
                        ldecEEUVHPAnnuity = new decimal();
                        ldecMonthlyAnnuity = new decimal();
                        CalculateEEUVHPAnnuityOption(busConstant.QJ50, ldecTotalEEUVHP, out ldecEEUVHPAnnuity, out ldecMonthlyAnnuity);

                        //if (this.iblnCalcualteNonVestedEEBenefit && this.iblnCalcualteUVHPBenefit)
                        // {
                        lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                        lbusBenefitCalculationOptions.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.MPIPP_PLAN_ID, busConstant.QJ50), ldecMonthlyAnnuity,
                            ldecEEUVHPAnnuity, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER, this.icdoBenefitCalculationHeader.person_id,
                            this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.QJ50, (ldecEEUVHPAnnuity * 50) / 100, true, false);

                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoBenefitCalculationDetail.ee_flag == busConstant.FLAG_YES).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
                        // }

                        //if (this.iblnCalcualteUVHPBenefit)
                        //{
                        //    lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                        //    lbusBenefitCalculationOptions.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.MPIPP_PLAN_ID, busConstant.QJ50), ldecMonthlyAnnuity, ldecEEUVHPAnnuity, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER, this.icdoBenefitCalculationHeader.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.QJ50, (ldecUVHPAnnuity * 50) / 100, false, true);
                        //    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoBenefitCalculationDetail.uvhp_flag == busConstant.FLAG_YES).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
                        //}
                    }
                    break;
                default:
                    break;
            }
        }

        private void CalculateFinalBenefitForUVHPBenefitOptionsForAlternatePayee(string astrBenefitOptionValue, decimal adecAlternatePayeeBenefitAmt, decimal adecLumpSumBenefitAmount,
                                                                                    int aintDROCommencementYear, decimal adecLifeConversionFactor)
        {
            busBenefitCalculationOptions lbusBenefitCalculationOptions;
            decimal ldecEEAnnuity;
            decimal ldecUVHPAnnuity;
            decimal ldecMonthlyAnnuity;

            idecTotalEEUVHP = adecAlternatePayeeBenefitAmt;
            switch (astrBenefitOptionValue)
            {
                case busConstant.LIFE_ANNUTIY:

                    if (this.idecTotalEEUVHP > busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT)
                    {
                        ldecEEAnnuity = new decimal();
                        ldecUVHPAnnuity = new decimal();
                        ldecMonthlyAnnuity = new decimal();
                        //CalculateEEUVHPAnnuityOptionForAlternatePayee(busConstant.LIFE, adecAlternatePayeeBenefitAmt, 0, adecLifeConversionFactor, out ldecEEAnnuity, out ldecUVHPAnnuity, out ldecMonthlyAnnuity);

                        if (this.iblnCalcualteNonVestedEEBenefit)
                        {
                            lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                            lbusBenefitCalculationOptions.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.MPIPP_PLAN_ID, busConstant.LIFE), adecLifeConversionFactor, adecAlternatePayeeBenefitAmt,
                            busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_BENEFICIARY, this.icdoBenefitCalculationHeader.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                            busConstant.LIFE, new decimal(), true, false);

                            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID &&
                                item.icdoBenefitCalculationDetail.ee_flag == busConstant.FLAG_YES).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
                        }

                        if (this.iblnCalcualteUVHPBenefit)
                        {
                            lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                            lbusBenefitCalculationOptions.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.MPIPP_PLAN_ID, busConstant.LIFE), adecLifeConversionFactor,
                        adecAlternatePayeeBenefitAmt, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_BENEFICIARY, this.icdoBenefitCalculationHeader.person_id,
                        this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.LIFE, new decimal(), false, true);
                            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoBenefitCalculationDetail.uvhp_flag == busConstant.FLAG_YES).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
                        }
                    }
                    break;

                case busConstant.LUMP_SUM:

                    ldecEEAnnuity = new decimal();
                    ldecUVHPAnnuity = new decimal();
                    ldecMonthlyAnnuity = 1;
                    //CalculateEEUVHPAnnuityOptionForAlternatePayee(busConstant.LUMP_SUM, adecLumpSumBenefitAmount, aintDROCommencementYear, adecLifeConversionFactor, out ldecEEAnnuity, out ldecUVHPAnnuity, out ldecMonthlyAnnuity);
                    if (this.iblnCalcualteNonVestedEEBenefit)
                    {
                        lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                        lbusBenefitCalculationOptions.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.MPIPP_PLAN_ID, busConstant.LUMP_SUM), 1, adecLumpSumBenefitAmount,
                        busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_BENEFICIARY, this.icdoBenefitCalculationHeader.person_id,
                        this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.LUMP_SUM, new decimal(), true, false);

                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID &&
                            item.icdoBenefitCalculationDetail.ee_flag == busConstant.FLAG_YES).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
                    }

                    if (this.iblnCalcualteUVHPBenefit)
                    {
                        lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                        lbusBenefitCalculationOptions.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.MPIPP_PLAN_ID, busConstant.LUMP_SUM), 1,
                            adecLumpSumBenefitAmount, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_BENEFICIARY, this.icdoBenefitCalculationHeader.person_id,
                            this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.LUMP_SUM, new decimal(), false, true);

                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID &&
                            item.icdoBenefitCalculationDetail.uvhp_flag == busConstant.FLAG_YES).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
                    }

                    break;
                default:
                    break;
            }
        }

        //Need To Move
        public void CalculateEEUVHPAnnuityOption(string astrBenefitOption, decimal adecTotalEEUVHP, out decimal adecEEUVHPAnnuity, out decimal adecMonthlyAnnuityFactor)
        {
            adecMonthlyAnnuityFactor = 1;
            //adecEEAnnuity = busConstant.ZERO_DECIMAL;
            adecEEUVHPAnnuity = busConstant.ZERO_DECIMAL;
            decimal ldecJAndS50Factor = new decimal();
            int lintParticipantAge = Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge);
            int lintSurvivorAge = Convert.ToInt32(Math.Floor(icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement));

            if (astrBenefitOption == busConstant.LIFE)
            {
                DataTable ldtMonthlyLifeAnnuity = Select("cdoBenefitProvisionUvhpLifeFactor.GetUVHPLifeFactor", new object[2] { lintParticipantAge, this.icdoBenefitCalculationHeader.retirement_date.Year });
                //RID 76077
                if (ldtMonthlyLifeAnnuity.Rows.Count == 0)
                {
                    ldtMonthlyLifeAnnuity = Select("cdoBenefitProvisionUvhpLifeFactor.GetUVHPLifeFactor", new object[2] { lintParticipantAge, DateTime.Now.Year });
                }

                if (ldtMonthlyLifeAnnuity.Rows.Count > 0)
                {
                    adecMonthlyAnnuityFactor = Convert.ToDecimal(ldtMonthlyLifeAnnuity.Rows[0][0]);
                    adecMonthlyAnnuityFactor = Math.Round(adecMonthlyAnnuityFactor, 3);
                }
                //EEAnnuity
                //adecEEAnnuity = adecTotalEE / adecMonthlyAnnuityFactor;
                //UVHPAnnuity
                if (adecMonthlyAnnuityFactor > decimal.Zero)
                {
                    adecEEUVHPAnnuity = Math.Round(adecTotalEEUVHP / adecMonthlyAnnuityFactor, 2);
                }
            }

            if (astrBenefitOption == busConstant.QJ50)
            {
                DataTable ldtMonthlyLifeAnnuity = Select("cdoBenefitProvisionUvhpLifeFactor.GetUVHPLifeFactor", new object[2] { lintParticipantAge, this.icdoBenefitCalculationHeader.retirement_date.Year });
                //RID 76077
                if (ldtMonthlyLifeAnnuity.Rows.Count == 0)
                {
                    ldtMonthlyLifeAnnuity = Select("cdoBenefitProvisionUvhpLifeFactor.GetUVHPLifeFactor", new object[2] { lintParticipantAge, DateTime.Now.Year });
                }

                if (ldtMonthlyLifeAnnuity.Rows.Count > 0)
                {
                    decimal ldecMonthlyAnnuityFactor = Convert.ToDecimal(ldtMonthlyLifeAnnuity.Rows[0][0]);
                    ldecMonthlyAnnuityFactor = Math.Round(ldecMonthlyAnnuityFactor, 3);
                    if (ldecMonthlyAnnuityFactor > decimal.Zero)
                    {
                        ldecJAndS50Factor = Convert.ToDecimal(Math.Round(Math.Min(1, Math.Max(0, 0.86 + 0.005 * (lintSurvivorAge - lintParticipantAge) + 0.006 * (65 - lintParticipantAge))), 3));
                        decimal ldecAnnuityAmount = Math.Round(adecTotalEEUVHP / ldecMonthlyAnnuityFactor, 2);
                        //EEAnnuity
                        //adecEEAnnuity = adecTotalEE * ldecJAndS50Factor;
                        //UVHPAnnuity
                        adecEEUVHPAnnuity = Math.Round(ldecAnnuityAmount * ldecJAndS50Factor, 2);
                        adecMonthlyAnnuityFactor = ldecJAndS50Factor;
                    }
                }
            }
        }

        public override void AfterPersistChanges()
        {
            decimal ldecLocal700GauranteedAmt = 0;
            busActiveRetireeIncreaseContract lbusActiveRetireeIncreaseContract = new busActiveRetireeIncreaseContract();
            foreach (busBenefitCalculationDetail lbusBenefitCalculationDetail in iclbBenefitCalculationDetail)
            {
                lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.benefit_calculation_header_id = this.icdoBenefitCalculationHeader.benefit_calculation_header_id;
                lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.Insert();

                if (!lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.IsNullOrEmpty())
                {
                    foreach (busBenefitCalculationOptions lbusBenefitCalculationOptions in lbusBenefitCalculationDetail.iclbBenefitCalculationOptions)
                    {
                        lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.benefit_calculation_detail_id = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.benefit_calculation_detail_id;
                        lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.Insert();
                    }
                }

                if (!lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.IsNullOrEmpty())
                {
                    foreach (busBenefitCalculationYearlyDetail lbusBenefitCalculationYearlyDetail in lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail)
                    {
                        lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.benefit_calculation_detail_id = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.benefit_calculation_detail_id; ;
                        lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.Insert();
                    }
                }
            }

            base.AfterPersistChanges();

            if (this.ibusBaseActivityInstance.IsNotNull())
            {
                this.SetProcessInstanceParameters();
            }
        }


        public void FlushOlderCalculations()
        {
            foreach (busBenefitCalculationDetail lbusBenefitCalculationDetail in this.iclbBenefitCalculationDetail)
            {
                if (!lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.IsNullOrEmpty())
                {
                    foreach (busBenefitCalculationOptions lbusBenefitCalculationOptions in lbusBenefitCalculationDetail.iclbBenefitCalculationOptions)
                    {
                        lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.Delete();
                    }
                    lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.Clear();
                }

                if (!lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.IsNullOrEmpty())
                {
                    foreach (busBenefitCalculationYearlyDetail lbusBenefitCalculationYearlyDetail in lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail)
                    {
                        lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.Delete();
                    }
                    lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.Clear();
                }

                lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.Delete();
            }

            this.iclbBenefitCalculationDetail.Clear();
        }

        public bool IsPaymentDateNull()
        {
            if (this.icdoBenefitCalculationHeader.payment_date == DateTime.MinValue)
            {
                return true;
            }

            return false;
        }

        public bool IsPaymentDateNotFirstOfMonth()
        {
            if (this.icdoBenefitCalculationHeader.payment_date != DateTime.MinValue && this.icdoBenefitCalculationHeader.payment_date != this.icdoBenefitCalculationHeader.payment_date.GetFirstDayofMonth())
            {
                return true;
            }

            return false;
        }

        public bool IsPaymentDateNotFriday()
        {
            if (this.icdoBenefitCalculationHeader.payment_date != DateTime.MinValue && this.icdoBenefitCalculationHeader.payment_date.DayOfWeek != DayOfWeek.Friday)
            {
                return true;
            }

            return false;
        }

        public void LoadPersonNotes()
        {
            iclbNotes = new Collection<busNotes>();
            DataTable ldtblist = busPerson.Select("cdoNotes.GetNotesForWithdrawalCaclulation", new object[2] { this.icdoBenefitCalculationHeader.person_id, busConstant.WITHDRAWL_CALCULATION_MAINTAINENCE_FORM });
            iclbNotes = GetCollection<busNotes>(ldtblist, "icdoNotes");
            if (iclbNotes != null)
                iclbNotes = iclbNotes.OrderByDescending(obj => obj.icdoNotes.created_date).ToList().ToCollection<busNotes>();
        }

        public ArrayList AddWDRLNotes()
        {
            ArrayList larrResult = new ArrayList();

            if (istrNewNotes.IsNullOrEmpty())
            {
                utlError lutlError = AddError(4076, "");
                larrResult.Add(lutlError);
                return larrResult;
            }

            cdoNotes lcdoNotes = new cdoNotes();
            lcdoNotes.notes = this.istrNewNotes;
            lcdoNotes.person_id = this.icdoBenefitCalculationHeader.person_id;
            lcdoNotes.form_id = busConstant.Form_ID;
            lcdoNotes.form_value = busConstant.WITHDRAWL_CALCULATION_MAINTAINENCE_FORM;
            lcdoNotes.created_by = iobjPassInfo.istrUserID;
            lcdoNotes.created_date = DateTime.Now;
            lcdoNotes.Insert();
            this.LoadPersonNotes();
            istrNewNotes = string.Empty;
            larrResult.Add(this);
            return larrResult;
        }
    }
}
