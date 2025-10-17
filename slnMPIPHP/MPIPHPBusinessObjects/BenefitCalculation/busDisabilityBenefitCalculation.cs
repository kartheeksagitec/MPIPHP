
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
    public class busDisabiltyBenefitCalculation : busBenefitCalculationHeader
    {
        #region PROPERTIES
        public busBenefitApplication ibusBenefitApplicationRetirement { get; set; }

        public string istrLateRetirementFlag { get; set; }
        public DateTime idtLastWorkingDate { get; set; }
        public string istrNewNotes { get; set; }
        public bool iblnEligibleForRegularBenefit { get; set; }
        public bool iblnCheckIfHoursAfterDisabilityRetirementDate { get; set; }
        public bool iblnIsQualifiedSpouse { get; set; }

        public decimal idecLumpSumFactor { get; set; }
        public decimal idecLocalFrozenBenefit { get; set; }
        public decimal idecEEDerivedComponent { get; set; }
        public bool lblIsNew { get; set; }
        public int iintPersonAccountId { get; set; }
        public int iintNonSuspendibleMonths { get; set; }

        //public int iintPartAge { get; set; }
        //public int iintSpouseAge { get; set; }

        public Collection<busNotes> iclbNotes { get; set; }
        decimal idecFinalAccruedBenefitAmount { get; set; }

        decimal idecTotalLocalFrozenBenefit { get; set; }


        decimal idecEEContribution { get; set; }
        decimal idecEEInterest { get; set; }
        decimal idecEEPartialInterest { get; set; }

        decimal idecUVHPContribution { get; set; }
        decimal idecUVHPInterest { get; set; }
        public Dictionary<int, Dictionary<int, decimal>> idictWorkHrsAfterRetirement { get; set; }
        public Collection<cdoDummyWorkData> iclbWorkHrsAfterRetirement { get; set; }

        public int iintAgeAtRetirement { get; set; }

        public string istrCurrentDate { get; set; }
        /// <summary>
        /// ROUNDDOWN(MemAgeR-SpAgeR,0)
        /// </summary>
        public decimal idecAgeDiff { get; set; }


        public decimal idecIAPBalanceAsofSSAOnsetDate { get; set; }
        public decimal idecIAPBalanceAsofAwardedOnDate { get; set; }

        public decimal idecL161BalanceAsofSSAOnsetDate { get; set; }
        public decimal idecL161BalanceAsofAwardedOnDate { get; set; }

        public decimal idecL52BalanceAsofSSAOnsetDate { get; set; }
        public decimal idecL52BalanceAsofAwardedOnDate { get; set; }

        public DateTime idtLatestDate { get; set; }
        public int iintParticipantAgeAsofPaymentDate { get; set; }


        public busDocumentProcessCrossref ibusDocumentProcessCrossref { get; set; }

        //WorkFlow Properties
        public int Early_Retirement_Benefit_Application_Id { get; set; }
        public bool iblnPostRetirementDeath { get; set; }
        #endregion

        #region OVERRIDE

        public override void BeforeValidate(utlPageMode aenmPageMode)
        {
            ibusBenefitApplication.icdoBenefitApplication.disability_onset_date = icdoBenefitCalculationHeader.ssa_disability_onset_date;

            if (this.icdoBenefitCalculationHeader.retirement_date != DateTime.MinValue)
            {
                //Option 1
                //LoadAllRetirementContributions();
                if (ibusPerson.iclbPersonAccount != null && !ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).IsNullOrEmpty())
                {
                    LoadAllRetirementContributions(ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault());
                }
                else
                {
                    LoadAllRetirementContributions(null);
                }

                SetUpDisabilityVariablesForCalculation(this.icdoBenefitCalculationHeader.retirement_date);
                SetupPreRequisites_DisabilityCalculations();
            }

            
            this.iblnCalcualteUVHPBenefit = this.iblnCalculateIAPBenefit = this.iblnCalculateL161SplAccBenefit = this.iblnCalculateL52SplAccBenefit = this.iblnCalculateMPIPPBenefit = true;

            if (this.icdoBenefitCalculationHeader.ienuObjectState == ObjectState.Insert)
                lblIsNew = true;
            else
                lblIsNew = false;

            base.BeforeValidate(aenmPageMode);
        }

        public override void BeforePersistChanges()
        {
            if (this.icdoBenefitCalculationHeader.ienuObjectState != ObjectState.Insert)
            {
                FlushOlderCalculations();
                this.icdoBenefitCalculationHeader.ienuObjectState = ObjectState.Update;
            }
            Setup_Disabilty_Calculations();
            if (this.icdoBenefitCalculationHeader.retirement_date_option_2 != DateTime.MinValue)
            {
                CalculateMPIBenefitOptionTwo();
            }



            base.BeforePersistChanges();
        }

        public override void AfterPersistChanges()
        {
            decimal ldecLocal700GauranteedAmt = 0;
            Collection<busActiveRetireeIncreaseContract> lclbActiveRetireeIncreaseContract = new Collection<busActiveRetireeIncreaseContract>();
            busActiveRetireeIncreaseContract lbusActiveRetireeIncreaseContract = new busActiveRetireeIncreaseContract();

            foreach (busBenefitCalculationDetail lbusBenefitCalculationDetail in iclbBenefitCalculationDetail)
            {
                lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.benefit_calculation_header_id = this.icdoBenefitCalculationHeader.benefit_calculation_header_id;
                lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.Insert();
                //lbusBenefitCalculationDetail.istrDisabilityType = this.icdoBenefitCalculationHeader.istrDisabilityType;
                if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.iap_as_of_date != DateTime.MinValue)
                {
                    lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.iintIAPasYear = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.iap_as_of_date.Year;
                }

                if (!lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.IsNullOrEmpty())
                {
                    foreach (busBenefitCalculationOptions lbusBenefitCalculationOptions in lbusBenefitCalculationDetail.iclbBenefitCalculationOptions)
                    {
                        lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.benefit_calculation_detail_id = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.benefit_calculation_detail_id;
                        lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.Insert();

                        #region Calculate Retiree Increase

                        int lintPayementCount = (int)DBFunction.DBExecuteScalar("cdoPayeeAccount.GetCountOfPaymentMade",
                                                   new object[2] { this.iintOriginalPayeeAccountId, icdoBenefitCalculationHeader.person_id }, iobjPassInfo.iconFramework,
                                                   iobjPassInfo.itrnFramework);

                        //PROD PIR 127
                        if (((icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT
                            && lintPayementCount >= 1) || icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL) &&
                            icdoBenefitCalculationHeader.retirement_date < DateTime.Now && icdoBenefitCalculationHeader.iintPlanId != busConstant.IAP_PLAN_ID
                            )
                        {
                            lclbActiveRetireeIncreaseContract = lbusActiveRetireeIncreaseContract.LoadActiveRetireeIncContractByRetirementDate(icdoBenefitCalculationHeader.retirement_date);

                            foreach (busActiveRetireeIncreaseContract lbusRetireeIncreaseContract in lclbActiveRetireeIncreaseContract)
                            {
                                DataTable ldtbLoadApprovedQDRO = Select("cdoDroApplication.LoadApprovedQDRO", new object[2] { this.icdoBenefitCalculationHeader.person_id, icdoBenefitCalculationHeader.iintPlanId });
                                DateTime ldtDROCommencementDate = new DateTime();
                                string lstrIsAlternatePayeeEntitledToPartBenefit = string.Empty;

                                if (ldtbLoadApprovedQDRO.Rows.Count > 0)
                                {
                                    ldtDROCommencementDate = Convert.ToDateTime(ldtbLoadApprovedQDRO.Rows[0][enmDroApplication.dro_commencement_date.ToString()]);
                                    lstrIsAlternatePayeeEntitledToPartBenefit = Convert.ToString(ldtbLoadApprovedQDRO.Rows[0][enmDroBenefitDetails.is_alt_payee_eligible_for_participant_retiree_increase.ToString()]);
                                }

                                DateTime ldtRetireeIncreaseDate = new DateTime(lbusRetireeIncreaseContract.icdoActiveRetireeIncreaseContract.plan_year, 11, 01);

                                if (ldtRetireeIncreaseDate >= icdoBenefitCalculationHeader.retirement_date && (ldtDROCommencementDate == DateTime.MinValue ||
                                      (ldtDROCommencementDate != DateTime.MinValue && ldtRetireeIncreaseDate < ldtDROCommencementDate) ||
                                    (ldtDROCommencementDate != DateTime.MinValue && ldtRetireeIncreaseDate > ldtDROCommencementDate && lstrIsAlternatePayeeEntitledToPartBenefit != busConstant.FLAG_YES)))
                                {
                                    if (icdoBenefitCalculationHeader.iintPlanId == busConstant.LOCAL_700_PLAN_ID)
                                    {
                                        ldecLocal700GauranteedAmt =
                                            ibusCalculation.GetLocal700GuarentedAmt(lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.person_account_id);
                                    }


                                    //Delete All Records from Disability Retiree Increase table for this calc id
                                    if (iclbDisabilityRetireeIncrease != null && iclbDisabilityRetireeIncrease.Count > 0)
                                    {
                                        foreach (busDisabilityRetireeIncrease lbusDisabilityRetireeIncrease in iclbDisabilityRetireeIncrease)
                                        {
                                            lbusDisabilityRetireeIncrease.icdoDisabilityRetireeIncrease.Delete();
                                        }
                                        iclbDisabilityRetireeIncrease.Clear();
                                    }

                                    ibusCalculation.CalculateAndCreateRetireeIncreasePayeeAccount(lbusBenefitCalculationOptions, null, lbusRetireeIncreaseContract, icdoBenefitCalculationHeader.retirement_date.Year,
                                       icdoBenefitCalculationHeader.benefit_calculation_header_id, 0, ldecLocal700GauranteedAmt,
                                       Convert.ToDecimal(lbusRetireeIncreaseContract.icdoActiveRetireeIncreaseContract.percent_increase_value), busConstant.BENEFIT_TYPE_DISABILITY, this);
                                }
                            }
                        }

                        #endregion
                    }
                }

                if (!lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.IsNullOrEmpty())
                {

                    foreach (busBenefitCalculationYearlyDetail lbusBenefitCalculationYearlyDetail in lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail)
                    {
                        lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.benefit_calculation_detail_id = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.benefit_calculation_detail_id; ;
                        lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.Insert();

                        if (lbusBenefitCalculationYearlyDetail.iclbBenefitCalculationNonsuspendibleDetail != null && lbusBenefitCalculationYearlyDetail.iclbBenefitCalculationNonsuspendibleDetail.Count > 0)
                        {
                            foreach (busBenefitCalculationNonsuspendibleDetail lbusBenefitCalculationNonsuspendibleDetail in lbusBenefitCalculationYearlyDetail.iclbBenefitCalculationNonsuspendibleDetail)
                            {
                                lbusBenefitCalculationNonsuspendibleDetail.icdoBenefitCalculationNonsuspendibleDetail.benefit_calculation_yearly_detail_id = lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.benefit_calculation_yearly_detail_id;
                                lbusBenefitCalculationNonsuspendibleDetail.icdoBenefitCalculationNonsuspendibleDetail.benefit_calculation_detail_id = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.benefit_calculation_detail_id;
                                lbusBenefitCalculationNonsuspendibleDetail.icdoBenefitCalculationNonsuspendibleDetail.Insert();
                            }
                        }
                    }
                }

                DateTime ldtForfietureDate = new DateTime();

                if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.istrPlanCode == busConstant.MPIPP &&
                    this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).Count() > 0)
                {
                    busPersonAccountEligibility lbusPersonAccountEligibility = new busPersonAccountEligibility();
                    lbusPersonAccountEligibility = lbusPersonAccountEligibility.LoadPersonAccEligibilityByPersonAccountId(this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().icdoPersonAccount.person_account_id);
                    if (lbusPersonAccountEligibility != null)
                        ldtForfietureDate = lbusPersonAccountEligibility.icdoPersonAccountEligibility.forfeiture_date;
                }

                lbusBenefitCalculationDetail.LoadBenefitCalculationYearlyDetailsTotal(ldtForfietureDate);
            }

            base.AfterPersistChanges();

            if (this.ibusBaseActivityInstance.IsNotNull())
            {
                this.SetProcessInstanceParameters();
            }
        }

        public ArrayList btn_ApproveCalculation()
        {
            int lintBenefitAccountID = 0;
            int lintPayeeAccountID = 0;
            decimal ldecMinimumGuarantee = 0.0M;
            decimal ldecNonTaxableBeginningBalance = 0.0M;
            DateTime ldteTermCertainEndDate = new DateTime();
            string lstrFamilyRelationshipValue = string.Empty;
            DateTime ldteNextBenefitPaymentDate = new DateTime();
            busPayeeAccount lbusEarlyRetirementPayeeAccount = null;
            busPayeeAccount lbusPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
            bool lblnDeductPrevIAPBalance = false;
            if (this.iarrErrors.IsNull())
                this.iarrErrors = new ArrayList();

            this.iarrErrors = base.btn_ApproveCalculation();

            if (!this.iarrErrors.IsNullOrEmpty())
                return this.iarrErrors;

            if (this.iarrErrors.Count == 0 && this.icdoBenefitCalculationHeader.calculation_type_value != busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE)
            {

                #region PAYEE ACCOUNT RELATED LOGIC (PAYMENT - SPRINT 3.0) -- Abhishek

                //Is Disability application then
                //check if this person is having a receiving payee account for Early retirement application  
                int flag = 0;
                if (flag != 1)  // DONE ON PURPOSE TO AVOID PAYEE ACCOUNT CODE TO BE EXECUTED FOR NOW
                {
                    if (IsCancelledPayeeAccountDoesNotExsits()) //R3view THIS is for Disability (Not sure if Required here)
                    {
                        utlError lobjError = new utlError();
                        lobjError = AddError(2100, ""); //R3view - MessageID here 
                        this.iarrErrors.Add(lobjError);
                    }

                    //R3view - The Logic of this IF condition and whether the LAMBDA is safe 
                    if (this.iclbBenefitCalculationDetail.First().iclbBenefitCalculationOptions.Count() > 0 &&
                        this.iclbBenefitCalculationDetail.First().iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.participant_amount <= Decimal.Zero
                         && !(this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                            icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT))//PIR 985 10262015
                    {
                        utlError lobjError = new utlError();
                        lobjError = AddError(6057, "");//R3view 
                        this.iarrErrors.Add(lobjError);
                    }

                    else if ((this.iclbBenefitCalculationDetail.First().iclbBenefitCalculationOptions.Count() > 0 && this.iclbBenefitCalculationDetail.First().iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.participant_amount > Decimal.Zero)
                        || (this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                            icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT))//PIR 985 10262015
                    {
                        string lstrFundsType = String.Empty;
                        //Variables Required for Benefit updation or Insertion
                        
                        //Benefit Account Related
                        decimal ldecAccountOwnerStartingTaxableAmount = 0.0M;
                        decimal ldecAccountOwnerStartingNonTaxableAmount = 0.0M;
                        decimal ldecAccountOwnerStartingGrossAmount = 0.0M;
                        decimal ldecMinimumGuaranteeAmount = this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.minimum_guarantee_amount;

                        #region Get Early Retirement Payee Account - Adjustment Calc
                        if (this.icdoBenefitCalculationHeader.payee_account_id == 0 && icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT)
                        {
                            busDisabilityApplication lbusDisabilityApplication = new busDisabilityApplication { icdoBenefitApplication = new cdoBenefitApplication() };
                            if (lbusDisabilityApplication.FindBenefitApplication(icdoBenefitCalculationHeader.benefit_application_id))
                            {
                                lbusDisabilityApplication.iclbPayeeAccount = new Collection<busPayeeAccount>();
                                lbusDisabilityApplication.GetPayeeAccountsInReceivingSatus();

                                if (lbusDisabilityApplication.iclbPayeeAccount.Count > 0)
                                {
                                    if (lbusDisabilityApplication.iclbPayeeAccount.Where(item => item.icdoPayeeAccount.iintPlanId == this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.plan_id).Count() > 0)
                                    {
                                        icdoBenefitCalculationHeader.payee_account_id =
                                            lbusDisabilityApplication.iclbPayeeAccount.Where(item => item.icdoPayeeAccount.iintPlanId == this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.plan_id).FirstOrDefault().icdoPayeeAccount.payee_account_id;
                                    }
                                }
                            }
                        }
                        #endregion Get Early Retirement Payee Account

                        busPayeeBenefitAccount lbusPayeeBenefitAccount = new busPayeeBenefitAccount { icdoPayeeBenefitAccount = new cdoPayeeBenefitAccount() };

                        DateTime ldteBenefitBeginDate = this.icdoBenefitCalculationHeader.retirement_date;
                        
                        if (this.icdoBenefitCalculationHeader.payee_account_id > 0)
                        {
                            lbusEarlyRetirementPayeeAccount = new busPayeeAccount();
                            lbusEarlyRetirementPayeeAccount.FindPayeeAccount(this.icdoBenefitCalculationHeader.payee_account_id);
                            lbusEarlyRetirementPayeeAccount.LoadBenefitDetails();
                            lbusEarlyRetirementPayeeAccount.LoadPaymentHistoryHeaderDetails();
                            lbusEarlyRetirementPayeeAccount.LoadNextBenefitPaymentDate();

                        }

                        switch (this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.plan_id)
                        {
                            //R3view - Based on Per Plan we need to set the TAXABLE and NON-TAXABLE ITEMS
                            case busConstant.MPIPP_PLAN_ID:
                                ldecAccountOwnerStartingNonTaxableAmount = this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.vested_ee_amount;

                                ldecAccountOwnerStartingTaxableAmount = this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.vested_ee_interest;

                                ldecAccountOwnerStartingGrossAmount = ldecAccountOwnerStartingNonTaxableAmount + ldecAccountOwnerStartingTaxableAmount;

                                if(lbusEarlyRetirementPayeeAccount != null && lbusEarlyRetirementPayeeAccount.icdoPayeeAccount.payee_account_id > 0)//PIR - 915
                                    ldecMinimumGuaranteeAmount = lbusEarlyRetirementPayeeAccount.idecRemainingMinimumGuaranteeAmount;
                                // Visible only for MPI
                                break;

                            case busConstant.IAP_PLAN_ID:
                                //GROSS - IAP ACCOUNT BALANCE  - TILL DATE
                                ldecAccountOwnerStartingGrossAmount = this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.iap_balance_amount + this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.qdro_offset;
                                break;

                            case busConstant.LOCAL_161_PLAN_ID:

                                break;

                            case busConstant.LOCAL_52_PLAN_ID:

                                break;

                            case busConstant.LOCAL_600_PLAN_ID:

                                break;

                            case busConstant.LOCAL_666_PLAN_ID:

                                break;

                            case busConstant.LOCAL_700_PLAN_ID:

                                break;

                        }

                        //Benefit Account
                        lintBenefitAccountID = busPayeeAccountHelper.IsBenefitAccountExists(this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).First().icdoPersonAccount.person_account_id,
                                                                                             busConstant.BENEFIT_TYPE_DISABILITY, lstrFundsType,
                                                                                             this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.benefit_application_detail_id, 0);  //R3view  the Query and code for this one.

                        lintBenefitAccountID = lbusPayeeBenefitAccount.ManagePayeeBenefitAccount(lintBenefitAccountID, this.icdoBenefitCalculationHeader.person_id,
                                                                          this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).First().icdoPersonAccount.person_account_id,
                                                                          ldecAccountOwnerStartingTaxableAmount, ldecAccountOwnerStartingNonTaxableAmount, ldecAccountOwnerStartingGrossAmount, lstrFundsType);


                        //R3view -- IF Term Year Certain Option FIND the end Date 

                        LoadPlanBenefitsForPlan(this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.plan_id);
                        iintTermCertainMonths = busConstant.ZERO_INT;
                        iintTermCertainMonths = busPayeeAccountHelper.IsTermCertainBenefitOption(this.iclbBenefitCalculationDetail.First().iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.plan_benefit_id, this.iclbcdoPlanBenefit);
                        if (iintTermCertainMonths > 0)
                        {
                            //Related to Disability Conversion Part
                            if (this.icdoBenefitCalculationHeader.payee_account_id > 0)
                            {
                                ldteTermCertainEndDate = lbusEarlyRetirementPayeeAccount.icdoPayeeAccount.term_certain_end_date;
                            }
                            else
                            {
                                ldteTermCertainEndDate = ldteBenefitBeginDate.AddMonths(iintTermCertainMonths);
                                if (ldteTermCertainEndDate != DateTime.MinValue)
                                    ldteTermCertainEndDate = ldteTermCertainEndDate.AddDays(-1);
                            }
                        }


                        if (icdoBenefitCalculationHeader.payee_account_id > 0)
                        {
                            //BR-018-12
                            DataTable ldtblQDROApplications = Select("cdoBenefitApplication.GetSharedQDROInReceivingStatus_Disability",
                                new object[1] { this.icdoBenefitCalculationHeader.person_id });

                            if (ldtblQDROApplications != null && ldtblQDROApplications.Rows.Count > 0)
                            {
                                busWorkflowHelper.InitializeWorkflowIfNotExists(busConstant.QDRO_WORKFLOW_NAME, this.icdoBenefitCalculationHeader.person_id, 0,
                                    0, null);
                            }
                        }

                        if (this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID)
                        {
                            //Disability Conversion BR-18-07 For MEA
                            if (icdoBenefitCalculationHeader.payee_account_id > 0 && lbusEarlyRetirementPayeeAccount != null
                                && this.ibusBenefitApplication.icdoBenefitApplication.retirement_date < lbusEarlyRetirementPayeeAccount.icdoPayeeAccount.idtRetireMentDate)
                            {
                                   ldecNonTaxableBeginningBalance = lbusEarlyRetirementPayeeAccount.idecRemainingNonTaxableBeginningBalance;                                                              
                            }
                            else
                            {
                                this.ibusCalculation.ProcessQDROOffset(this.iclbBenefitCalculationDetail.First(), this.icdoBenefitCalculationHeader.person_id, ref ldecNonTaxableBeginningBalance, false, false,
                                    false, false, busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL);

                                if (ldecAccountOwnerStartingNonTaxableAmount > 0)
                                {
                                    //Ticket#73070
                                    ldecNonTaxableBeginningBalance = ldecAccountOwnerStartingNonTaxableAmount - this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.idecAlt_payee_ee_contribution;
                                }
                            }

                        }

                        bool lblnAdjustmentPaymentFlag = false;
                        if (this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.adjustment_iap_payment_flag == busConstant.FLAG_YES)
                        {
                            lblnAdjustmentPaymentFlag = true;
                        }
                        DateTime adtAdjustFromDate = ldteBenefitBeginDate;


                        busPayeeAccount lbusIAPPayeeAccount = null;

                        //PIR 944
                        if (this.icdoBenefitCalculationHeader.lump_sum_payment == busConstant.FLAG_YES)
                        {
                            DataTable ldtbResult = new DataTable();
                           

                            ldtbResult = busBase.Select<cdoPayeeAccount>(
                                  new string[5] { "PERSON_ID", "ACCOUNT_RELATION_VALUE", "PAYEE_BENEFIT_ACCOUNT_ID", "BENEFIT_ACCOUNT_TYPE_VALUE", "PLAN_BENEFIT_ID" },
                                  new object[5] { this.icdoBenefitCalculationHeader.person_id, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER, lintBenefitAccountID, busConstant.BENEFIT_TYPE_DISABILITY, 9 }, null, null);
                            if (ldtbResult != null && ldtbResult.Rows.Count > 0)
                            {
                                busPayeeAccount lbusTempPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
                                lbusTempPayeeAccount.icdoPayeeAccount.LoadData(ldtbResult.Rows[0]);
                                lintPayeeAccountID = lbusTempPayeeAccount.icdoPayeeAccount.payee_account_id;
                            }
                        }
                        else
                        {
                            lintPayeeAccountID = busPayeeAccountHelper.IsPayeeAccountExists(this.icdoBenefitCalculationHeader.person_id, lintBenefitAccountID, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER, busConstant.BENEFIT_TYPE_DISABILITY, false, this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.plan_id,null,0,null,this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.benefit_application_detail_id); 
                        }

                        //Ticket#92127:  Every time a new Adjustment calculation for Pension Plans is Approved, OPUS need to change the status of any Reimbursement entry (in Overpayment Tab) from Pending to Completed. Also, stop showing any Receivable Amount in Payee Account 
                        if (lintPayeeAccountID > 0 && this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT)
                        {
                            busPayeeAccount lbusPayeeAccountRepayment = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
                            lbusPayeeAccountRepayment.FindPayeeAccount(lintPayeeAccountID);
                            lbusPayeeAccountRepayment.LoadAllRepaymentSchedules();
                            lbusPayeeAccountRepayment.LoadPayeeAccountPaymentItemType();
                            if (lbusPayeeAccountRepayment.iclbRepaymentSchedule != null)
                            {
                                busRepaymentSchedule lbusRepaymentSchedule = lbusPayeeAccountRepayment.iclbRepaymentSchedule
                                        .Where(item => item.icdoRepaymentSchedule.reimbursement_status_value == "PEND").FirstOrDefault();  //There should only be one Pending record. No need to loop.
                                if (lbusRepaymentSchedule != null)
                                {
                                    //First, Change the status of any Payee Account-->OverPayment-->Reimbursement (in Overpayment Tab) from Pending to Completed.
                                    lbusRepaymentSchedule.icdoRepaymentSchedule.reimbursement_status_value = "CMPL";
                                    lbusRepaymentSchedule.icdoRepaymentSchedule.Update();

                                    //Next, populate the end_date on the associated SGT_PAYEE_ACCOUNT_PAYMENT_ITEM_TYPE record.  This makes the "Pension Receivable" value go away in the Payment Breakdown tab of the Payee Account Maintenance screen.
                                    busPayeeAccountPaymentItemType lbusPayeeAccountPaymentItemType = lbusPayeeAccountRepayment.iclbPayeeAccountPaymentItemType
                                        .Where(item => item.icdoPayeeAccountPaymentItemType.payment_item_type_id == 53)  //53 = Pension Receivable
                                        .Where(item => item.icdoPayeeAccountPaymentItemType.amount == lbusRepaymentSchedule.icdoRepaymentSchedule.next_amount_due) //this exta Where is probably not necessary but just in case
                                        .FirstOrDefault();
                                    if (lbusPayeeAccountPaymentItemType != null)
                                    {
                                        lbusPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.end_date = DateTime.Today;
                                        lbusPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.Update();
                                    }
                                }
                            }
                        }

                        if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT)
                        {
                            if (this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID)
                            {
                                if (lintPayeeAccountID > 0)
                                {
                                    lbusIAPPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
                                    lbusIAPPayeeAccount.FindPayeeAccount(lintPayeeAccountID);                               
                                }
                            }
                        }

                        busPayeeAccount lbusOldPayeeDetails = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };

                        if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT)
                        {                            
                            busPlanBenefitXr lbusPlanBenXr = new busPlanBenefitXr();
                            lbusPlanBenXr.FindPlanBenefitXr(this.iclbBenefitCalculationDetail.First().iclbBenefitCalculationOptions.FirstOrDefault().icdoBenefitCalculationOptions.plan_benefit_id);
                            if (lintPayeeAccountID > 0)
                            {
                                lbusOldPayeeDetails.FindPayeeAccount(lintPayeeAccountID);
                                lbusOldPayeeDetails.LoadBenefitDetails();
                                lbusOldPayeeDetails.LoadPaymentHistoryHeaderDetails();

                                if ((lbusOldPayeeDetails.icdoPayeeAccount.istrBenefitOptionValue == busConstant.JOINT_50_PERCENT_POPUP_ANNUITY ||
                                    lbusOldPayeeDetails.icdoPayeeAccount.istrBenefitOptionValue == busConstant.JOINT_100_PERCENT_POPUP_ANNUITY) && lbusPlanBenXr.icdoPlanBenefitXr.benefit_option_value == busConstant.LIFE_ANNUTIY)
                                {

                                    busPerson lbusSpouse = new busPerson();
                                    lbusSpouse.FindPerson(this.icdoBenefitCalculationHeader.beneficiary_person_id);
                                    if (lbusSpouse.icdoPerson.date_of_death != DateTime.MinValue)
                                    {
                                        //RID 115034 commented below line
                                        //adtAdjustFromDate = lbusSpouse.icdoPerson.date_of_death.GetLastDayofMonth().AddDays(1);//If Convert To Benefit Option Executed.

                                        //RID 115034 Fixing adjustment from date selection because for retirement calc we are doing as following. Commented above line.
                                        if (lbusSpouse.icdoPerson.date_of_death.Day == 1)
                                        {
                                            adtAdjustFromDate = lbusSpouse.icdoPerson.date_of_death;
                                        }
                                        else
                                        {
                                            adtAdjustFromDate = lbusSpouse.icdoPerson.date_of_death.GetLastDayofMonth().AddDays(1);//If Convert To Benefit Option Executed.
                                        }

                                    }

                                    //RID 115034
                                    int lintLifeConversionDateExists = (int)DBFunction.DBExecuteScalar("cdoPayeeAccount.CheckIfConvertToLifeDateExists", new object[1] { lintPayeeAccountID },
                                    iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                                    if (lintLifeConversionDateExists == 0)
                                    {
                                        DBFunction.DBExecuteScalar("cdoPayeeAccount.UpdatePopupToLifeConversionDate", new object[3] { adtAdjustFromDate, lbusPlanBenXr.icdoPlanBenefitXr.plan_id, lintPayeeAccountID }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                                    }




                                }
                                //PIR - 915
                                ldecNonTaxableBeginningBalance = lbusOldPayeeDetails.icdoPayeeAccount.nontaxable_beginning_balance; 
                                ldecMinimumGuaranteeAmount = lbusOldPayeeDetails.icdoPayeeAccount.minimum_guarantee_amount;
                            }

                        }

                      
                        string lstrDisabilityConvFlag = string.Empty;
                        if (icdoBenefitCalculationHeader.payee_account_id > 0)
                        {
                            this.icdoBenefitCalculationHeader.istrRetirementType = busConstant.DISABILITY_TYPE_SSA;
                            lstrDisabilityConvFlag = busConstant.FLAG_YES;                            
                        }
                        
                        //PIR 944
                        int lintPlanBenefitId = 0;
                        if (this.icdoBenefitCalculationHeader.lump_sum_payment == busConstant.FLAG_YES)
                        {
                            lintPlanBenefitId = 9;
                        }
                        else
                        {
                            lintPlanBenefitId = this.iclbBenefitCalculationDetail.First().iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.plan_benefit_id;
                        }

                        lintPayeeAccountID = lbusPayeeAccount.ManagePayeeAccount(lintPayeeAccountID, this.icdoBenefitCalculationHeader.person_id, 0,
                                                                                  this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.benefit_application_detail_id,
                                                                                  this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.benefit_calculation_detail_id,
                                                                                  0, 0, lintBenefitAccountID, busConstant.BENEFIT_TYPE_DISABILITY, iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.benefit_subtype_value,
                                                                                  ldteBenefitBeginDate, DateTime.MinValue, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER, lstrFamilyRelationshipValue,
                                                                                  ldecMinimumGuaranteeAmount,
                                                                                  ldecNonTaxableBeginningBalance, lintPlanBenefitId,//PIR 944
                                                                                  ldteTermCertainEndDate, busConstant.FLAG_NO, lstrDisabilityConvFlag, lblnAdjustmentPaymentFlag);

                        decimal ldecTaxableAmount = 0M;
                        decimal ldecNonTaxableAmount = 0M;
                        lbusPayeeAccount.LoadNextBenefitPaymentDate();
                        ldteNextBenefitPaymentDate = lbusPayeeAccount.idtNextBenefitPaymentDate;//R3vview this once with Vinovin
                        //R3view -- First Parameter Should be maybe Retirement or Payment Date Review the Function too   
                        if (this.iclbBenefitCalculationDetail.First().iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.istrBenefitOptionDescription.Contains(busConstant.LUMP_SUM_DESCRIPTION)
                            && this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID)
                        {
                            decimal ldecBenefitAmount = this.iclbBenefitCalculationDetail.First().iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.participant_amount;
                            if (this.iclbBenefitCalculationDetail.First().iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.overridden_benefit_amount > decimal.Zero)
                            {
                                ldecBenefitAmount = this.iclbBenefitCalculationDetail.First().iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.overridden_benefit_amount;
                            }
                            busPayeeAccountHelper.CalculateMonthlyPaymentComponents(DateTime.MinValue, ldecBenefitAmount,
                                                      ref ldecNonTaxableAmount, ref ldecTaxableAmount, ldecNonTaxableBeginningBalance);
                        }
                        else if (this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID)
                        {
                            decimal ldecIapPaid = decimal.Zero;
                            int lintPersonAccountId = 0;
                            decimal ldecIapAdjustedBalance = decimal.Zero;
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
                                //                    lbusIAPPayeeAccount.icdoPayeeAccount.payee_account_id,  lbusIAPPayeeAccount.icdoPayeeAccount.benefit_begin_date,
                                //                    DateTime.Now});

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
                            ldecIapAdjustedBalance = this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.final_monthly_benefit_amount;
                            //PROD PIR 252 Overridden amount fix
                            if (this.iclbBenefitCalculationDetail.First().iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.overridden_benefit_amount > decimal.Zero)
                            {
                                ldecIapAdjustedBalance = this.iclbBenefitCalculationDetail.First().iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.overridden_benefit_amount;
                            }
                            ldecIapAdjustedBalance = ldecIapAdjustedBalance - ldecIapPaid + ldecWithHoldingAmount;

                            if (ldecIapAdjustedBalance > decimal.Zero)
                            {
                                busPayeeAccountHelper.CalculateMonthlyPaymentComponents(DateTime.MinValue, ldecIapAdjustedBalance,
                                                                         ref ldecNonTaxableAmount, ref ldecTaxableAmount, 0);
                            }
                            else
                            {
                                //create overpayment.
                                if (lbusIAPPayeeAccount.IsNotNull())
                                {
                                    //Create Overpayment.
                                    lbusPayeeAccount.CreateOverPayments(lbusPayeeAccount, lbusPayeeAccount.idtNextBenefitPaymentDate, DateTime.MinValue, DateTime.MinValue, -ldecIapAdjustedBalance, decimal.Zero,
                                        busConstant.RETRO_PAYMENT_ADJUSTMENT_BATCH);
                                }
                            }
                        }
                        else
                        {
                            decimal ldecBenefitAmount = this.iclbBenefitCalculationDetail.First().iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.participant_amount;
                            if (this.iclbBenefitCalculationDetail.First().iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.overridden_benefit_amount > decimal.Zero)
                            {
                                ldecBenefitAmount = this.iclbBenefitCalculationDetail.First().iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.overridden_benefit_amount;
                            }
                            busPayeeAccountHelper.CalculateMonthlyPaymentComponents(DateTime.MinValue, ldecBenefitAmount,
                                                          ref ldecNonTaxableAmount, ref ldecTaxableAmount, this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.monthly_exclusion_amount);
                        }


                        if (ldecTaxableAmount > 0M)
                        {
                            if (this.iclbBenefitCalculationDetail.First().iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.istrBenefitOptionDescription.Contains(busConstant.LUMP_SUM_DESCRIPTION))
                                lbusPayeeAccount.CreatePayeeAccountPaymentItemType("ITEM21", ldecTaxableAmount, "0", 0,
                                                                                 ldteNextBenefitPaymentDate, DateTime.MinValue, "N", false);
                            else
                                lbusPayeeAccount.CreatePayeeAccountPaymentItemType("ITEM1", ldecTaxableAmount, "0", 0,
                                                                                ldteNextBenefitPaymentDate, DateTime.MinValue, "N", false);
                        }
                        if (ldecNonTaxableAmount > 0M)
                        {
                            if (this.iclbBenefitCalculationDetail.First().iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.istrBenefitOptionDescription.Contains(busConstant.LUMP_SUM_DESCRIPTION))
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
                        DateTime ldtlastWorkingDate = new DateTime();
                        string lstrEmployerName = string.Empty;
                        Collection<busBenefitMonthwiseAdjustmentDetail> lclbBenefitMonthwiseAdjustmentDetail = new Collection<busBenefitMonthwiseAdjustmentDetail>();
                        if (idictWorkHrsAfterRetirement.IsNullOrEmpty())
                            idictWorkHrsAfterRetirement = ibusCalculation.LoadMPIHoursAfterRetirementDate(this.ibusPerson.icdoPerson.istrSSNNonEncrypted, this.icdoBenefitCalculationHeader.retirement_date, this.icdoBenefitCalculationHeader.iintPlanId, ref ldtlastWorkingDate, ref lstrEmployerName);

                        busBenefitCalculationDetail lbusBenefitCalculationDetail = new busBenefitCalculationDetail { icdoBenefitCalculationDetail = new cdoBenefitCalculationDetail() };
                        lbusBenefitCalculationDetail = this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault();

                        ibusCalculation.FillYearlyDetailSetBenefitAmountForEachYear(this, lbusBenefitCalculationDetail);


                        decimal ldecTaxableAmtPaid = 0, ldecNonTaxableAmtPaid = 0;
                        decimal ldecPopUpBenefitAmount = 0, ldecPopupNonTaxableAmount = 0; //RequestID: 72091 

                        if (this.iclbBenefitCalculationDetail.First().iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.pop_up_benefit_amount > decimal.Zero)
                        {
                            ldecPopUpBenefitAmount = this.iclbBenefitCalculationDetail.First().iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.pop_up_benefit_amount;
                            ldecPopupNonTaxableAmount = Math.Round(this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.popup_monthly_exclusion_amount, 2, MidpointRounding.AwayFromZero);                    
                        }

                        if (lbusEarlyRetirementPayeeAccount != null)
                        {
                            
                            Collection<busBenefitMonthwiseAdjustmentDetail>  lclbEarlyRetirementMonthwiseAdjustmentDetail = new Collection<busBenefitMonthwiseAdjustmentDetail>();

                            #region Under/Over in case of Early to Disability Conversion - Adjustment Calculation
                            if (icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT)
                            {
                                lclbEarlyRetirementMonthwiseAdjustmentDetail = ibusCalculation.GetAmountActuallyPaid(lbusEarlyRetirementPayeeAccount, this.icdoBenefitCalculationHeader.retirement_date, lbusEarlyRetirementPayeeAccount.idtLastBenefitPaymentDate);

                                //Payment Adjustments - Benefit Adjustment Batch
                                bool lblnAdjustmentCalculationForRetireeIncrease = false;
                                if (iclbDisabilityRetireeIncrease != null && iclbDisabilityRetireeIncrease.Count > 0)
                                    lblnAdjustmentCalculationForRetireeIncrease = true;

                                if (lclbEarlyRetirementMonthwiseAdjustmentDetail != null && lclbEarlyRetirementMonthwiseAdjustmentDetail.Count > 0)
                                {
                                    lclbBenefitMonthwiseAdjustmentDetail = ibusCalculation.GetAmountActuallyPaid(lbusPayeeAccount, adtAdjustFromDate, lbusPayeeAccount.idtLastBenefitPaymentDate, lblnAdjustmentCalculationForRetireeIncrease);

                                    foreach (busBenefitMonthwiseAdjustmentDetail lbusEarlyRetirementMonthwiseAdjustmentDetail in lclbEarlyRetirementMonthwiseAdjustmentDetail)
                                    {
                                        if (lclbBenefitMonthwiseAdjustmentDetail.Where(t => t.icdoBenefitMonthwiseAdjustmentDetail.payment_date.Date == lbusEarlyRetirementMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date.Date).Count() > 0)
                                        {
                                            lclbBenefitMonthwiseAdjustmentDetail.Where(t => t.icdoBenefitMonthwiseAdjustmentDetail.payment_date.Date == lbusEarlyRetirementMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date.Date)
                                                .FirstOrDefault().icdoBenefitMonthwiseAdjustmentDetail.taxable_amount_paid +=
                                                    lbusEarlyRetirementMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.taxable_amount_paid;

                                            lclbBenefitMonthwiseAdjustmentDetail.Where(t => t.icdoBenefitMonthwiseAdjustmentDetail.payment_date.Date == lbusEarlyRetirementMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date.Date)
                                                .FirstOrDefault().icdoBenefitMonthwiseAdjustmentDetail.non_taxable_amount_paid +=
                                                lbusEarlyRetirementMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.non_taxable_amount_paid;
                                        }
                                        else
                                        {
                                            lclbBenefitMonthwiseAdjustmentDetail.Add(lbusEarlyRetirementMonthwiseAdjustmentDetail);
                                        }
                                    }
                                }
                                else
                                {
                                    lclbBenefitMonthwiseAdjustmentDetail = ibusCalculation.GetAmountActuallyPaid(lbusPayeeAccount, adtAdjustFromDate, lbusPayeeAccount.idtLastBenefitPaymentDate, lblnAdjustmentCalculationForRetireeIncrease);
                                }

                                lclbBenefitMonthwiseAdjustmentDetail = lclbBenefitMonthwiseAdjustmentDetail.OrderBy(t => t.icdoBenefitMonthwiseAdjustmentDetail.payment_date).ToList().ToCollection();
                            }
                            #endregion Under/Over in case of Early to Disability Conversion - Adjustment Calculation
                            else
                            {
                                lclbBenefitMonthwiseAdjustmentDetail = ibusCalculation.GetAmountActuallyPaid(lbusEarlyRetirementPayeeAccount, this.icdoBenefitCalculationHeader.retirement_date, lbusEarlyRetirementPayeeAccount.idtLastBenefitPaymentDate);
                            }
                            
                            int lintYear = 0;
                            int lintMonth = 0;

                            if (!idictWorkHrsAfterRetirement.IsNullOrEmpty())
                            {
                                DateTime ldtParticipantTurns65 = this.ibusPerson.icdoPerson.date_of_birth.AddYears(65);
                                if (ldtParticipantTurns65.Day != 1)
                                {
                                    ldtParticipantTurns65 = ldtParticipantTurns65.GetLastDayofMonth().AddDays(1);
                                }
                                bool lblnGetAmountForReemployment = false;
                                foreach (busBenefitMonthwiseAdjustmentDetail lbusBenefitMonthwiseAdjustmentDetail in lclbBenefitMonthwiseAdjustmentDetail)
                                {
                                    lblnGetAmountForReemployment = false;
                                    if (lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date >= ldtParticipantTurns65)
                                    {
                                        lblnGetAmountForReemployment = true;
                                    }
                                    lintMonth = lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date.Month;
                                    lintYear = lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date.Year;
                                    if (lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date < this.icdoBenefitCalculationHeader.retirement_date)
                                    {
                                        lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.non_taxable_amount_to_be_paid = decimal.Zero;
                                        lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.taxable_amount_to_be_paid = decimal.Zero;

                                    }
                                    //SuspendibleHoursChange
                                    else if ((idictWorkHrsAfterRetirement.Keys.Contains(lintYear) && idictWorkHrsAfterRetirement[lintYear][lintMonth] >= ibusCalculation.GetSuspendibleHoursValue(lintYear,lintMonth) || ibusCalculation.CheckIfMonthIsSuspendibleMonthFromNonSignatory(this.ibusPerson, lintMonth, lintYear)))
                                    {
                                        //SuspendibleHoursChange
                                        if (idictWorkHrsAfterRetirement.Keys.Contains(lintYear) && idictWorkHrsAfterRetirement[lintYear][lintMonth] >= ibusCalculation.GetSuspendibleHoursValue(lintYear, lintMonth))
                                        {
                                            lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.hours = idictWorkHrsAfterRetirement[lintYear][lintMonth];
                                        }
                                        lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.suspended_flag = busConstant.FLAG_YES;
                                        if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.ee_derived_benefit_amount > 0)
                                        {
                                            //RequestID: 72091
                                            if (this.icdoBenefitCalculationHeader.iblnPopUpToLife && lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date < icdoBenefitCalculationHeader.idtJointAnnuitantDOD)
                                            {
                                                lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.non_taxable_amount_to_be_paid = ldecPopupNonTaxableAmount;
                                                lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.taxable_amount_to_be_paid = (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.ee_derived_benefit_amount * lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.pop_up_option_factor_at_ret) - ldecPopupNonTaxableAmount;
                                            }
                                            else
                                            {
                                                lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.non_taxable_amount_to_be_paid = ldecNonTaxableAmount;
                                                lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.taxable_amount_to_be_paid = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.ee_derived_benefit_amount - ldecNonTaxableAmount;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (!lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.IsNullOrEmpty())
                                        {
                                            ldecTaxableAmtPaid = decimal.Zero;

                                            //RequestID: 72091
                                            if (this.icdoBenefitCalculationHeader.iblnPopUpToLife && lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date < icdoBenefitCalculationHeader.idtJointAnnuitantDOD)
                                                lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.non_taxable_amount_to_be_paid = ldecPopupNonTaxableAmount;
                                            else
                                                lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.non_taxable_amount_to_be_paid = ldecNonTaxableAmount;

                                            //RequestID: 72091
                                            if (this.icdoBenefitCalculationHeader.iblnPopUpToLife && lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date < icdoBenefitCalculationHeader.idtJointAnnuitantDOD)
                                                ldecTaxableAmtPaid = ibusCalculation.GetBenefitAmountOfParticipantFromYearlyDetail(this, lbusBenefitCalculationDetail, lintYear, ablnGetAmountForReemployment: lblnGetAmountForReemployment,ldtPaymentDate: lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date);
                                            else
                                                ldecTaxableAmtPaid = ibusCalculation.GetBenefitAmountOfParticipantFromYearlyDetail(this, lbusBenefitCalculationDetail, lintYear, ablnGetAmountForReemployment: lblnGetAmountForReemployment);

                                            if (ldecTaxableAmtPaid > 0)
                                            {
                                                if (this.icdoBenefitCalculationHeader.iblnPopUpToLife && lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date < icdoBenefitCalculationHeader.idtJointAnnuitantDOD)
                                                    lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.taxable_amount_to_be_paid = ldecTaxableAmtPaid - ldecPopupNonTaxableAmount;
                                                else
                                                    lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.taxable_amount_to_be_paid = ldecTaxableAmtPaid - ldecNonTaxableAmount;
                                            }
                                        }
                                    }
                                }

                                if (lbusPayeeAccount.iclbRetroItemType.IsNullOrEmpty())
                                    lbusPayeeAccount.LoadRetroItemType();
                                if (lbusPayeeAccount.iclbPaymentItemType.IsNullOrEmpty())
                                    lbusPayeeAccount.LoadPaymentItemType();
                            }
                            else
                            {
                                ibusCalculation.CalculateAmountShouldHaveBeenPaid(lbusPayeeAccount, ref lclbBenefitMonthwiseAdjustmentDetail);

                                //RequestID: 72091
                                if (lclbBenefitMonthwiseAdjustmentDetail.Count > 0)
                                {
                                    foreach (busBenefitMonthwiseAdjustmentDetail lbusBenefitMonthwiseAdjustmentDetail in lclbBenefitMonthwiseAdjustmentDetail)
                                    {
                                        if (this.icdoBenefitCalculationHeader.iblnPopUpToLife && lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date < this.icdoBenefitCalculationHeader.idtJointAnnuitantDOD)
                                        {
                                            lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.taxable_amount_to_be_paid = ldecPopUpBenefitAmount;
                                            lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.non_taxable_amount_to_be_paid = ldecPopupNonTaxableAmount;
                                        }
                                    }
                                }
                            }

                            //PROD FIX FOR RETIREE INCREASE PIR 127
                            ibusCalculation.CalculateRetireeIncreaseAmountShouldHaveBeenPaid(lbusPayeeAccount, iclbDisabilityRetireeIncrease, ref lclbBenefitMonthwiseAdjustmentDetail); // PROD PIR 581
                            ibusCalculation.CreateOverpaymentUnderPayment(lbusPayeeAccount, lclbBenefitMonthwiseAdjustmentDetail, busConstant.RETRO_PAYMENT_ADJUSTMENT_BATCH); //PIR - 1036

                            lbusPayeeAccount.CreateReviewPayeeAccountStatus(ablnFromApprovedCalc: true);
                        }
                        else if ((!this.iclbBenefitCalculationDetail.First().iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.istrBenefitOptionDescription.Contains(busConstant.LUMP_SUM_DESCRIPTION))
                            && this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.plan_id != busConstant.IAP_PLAN_ID &&
                        this.icdoBenefitCalculationHeader.calculation_type_value != busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT)
                        {
                            //Create Retro Payment Items 
                            if (lbusPayeeAccount.iclbRetroItemType.IsNullOrEmpty())
                                lbusPayeeAccount.LoadRetroItemType();
                            if (lbusPayeeAccount.iclbPaymentItemType.IsNullOrEmpty())
                                lbusPayeeAccount.LoadPaymentItemType();



                            string istrCorrespondingInitialRetroItemCodeTaxable = string.Empty;
                            string istrCorrespondingInitialRetroItemCodeNonTaxable = string.Empty;
                            Decimal ldecRetroGrossAmt = Decimal.Zero;
                            Decimal ldecRetroTaxableAmt = busConstant.ZERO_DECIMAL;
                            Decimal ldecRetroNonTaxableAmt = Decimal.Zero;

                            int lintAge = Convert.ToInt32(busGlobalFunctions.CalculatePersonAge(this.ibusPerson.icdoPerson.date_of_birth, ldteNextBenefitPaymentDate));
                                                       
                            //R3view - For IAP this will be friday so we might have to Write a code.
                            int lintDiffinMonths = busGlobalFunctions.GetMonthsBetweenTwoDates(this.icdoBenefitCalculationHeader.retirement_date, ldteNextBenefitPaymentDate);
                            //PIR - 758
                            DateTime ldtFromDate = this.icdoBenefitCalculationHeader.retirement_date;
                            lclbBenefitMonthwiseAdjustmentDetail = ibusCalculation.GetAmountActuallyPaid(lbusPayeeAccount, ldtFromDate, ldtFromDate.AddMonths(lintDiffinMonths - 1), false);

                            if (lclbBenefitMonthwiseAdjustmentDetail.Count > 0)
                                lclbBenefitMonthwiseAdjustmentDetail = lclbBenefitMonthwiseAdjustmentDetail.OrderBy(i => i.icdoBenefitMonthwiseAdjustmentDetail.payment_date).ToList().ToCollection();

                            if (!idictWorkHrsAfterRetirement.IsNullOrEmpty())
                            {
                                DateTime ldtParticipantTurns65 = this.ibusPerson.icdoPerson.date_of_birth.AddYears(65);
                                if (ldtParticipantTurns65.Day != 1)
                                {
                                    ldtParticipantTurns65 = ldtParticipantTurns65.GetLastDayofMonth().AddDays(1);
                                }
                                
                                bool lblnGetAmountForReemployment = false;                               
                                int lintMonth = 0;
                                int lintYear = 0;
                                
                                foreach (busBenefitMonthwiseAdjustmentDetail lbusBenefitMonthwiseAdjustmentDetail in lclbBenefitMonthwiseAdjustmentDetail)
                                {
                                    lblnGetAmountForReemployment = false;
                                    if (lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date >= ldtParticipantTurns65)
                                    {
                                        lblnGetAmountForReemployment = true;
                                    }

                                    lintMonth = lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date.Month;
                                    lintYear = lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date.Year;

                                    if ((idictWorkHrsAfterRetirement.Keys.Contains(lintYear) && idictWorkHrsAfterRetirement[lintYear][lintMonth] >= ibusCalculation.GetSuspendibleHoursValue(lintYear, lintMonth)) || ibusCalculation.CheckIfMonthIsSuspendibleMonthFromNonSignatory(this.ibusPerson, lintMonth, lintYear))
                                    {
                                        //SuspendibleHoursChange
                                        if (idictWorkHrsAfterRetirement.Keys.Contains(lintYear) && idictWorkHrsAfterRetirement[lintYear][lintMonth] >= ibusCalculation.GetSuspendibleHoursValue(lintYear, lintMonth))
                                        {
                                            lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.hours = idictWorkHrsAfterRetirement[lintYear][lintMonth];
                                        }
                                        lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.suspended_flag = busConstant.FLAG_YES;
                                        if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.ee_derived_benefit_amount > 0)
                                        {
                                            lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.non_taxable_amount_to_be_paid = ldecNonTaxableAmount;
                                            lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.taxable_amount_to_be_paid = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.ee_derived_benefit_amount - ldecNonTaxableAmount;
                                        }

                                        ldecRetroGrossAmt += lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.ee_derived_benefit_amount;
                                    }
                                    else
                                    {
                                        if (this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID)
                                        {
                                            ldecTaxableAmtPaid = decimal.Zero;
                                            lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.non_taxable_amount_to_be_paid = ldecNonTaxableAmount;
                                            ldecTaxableAmtPaid = ibusCalculation.GetBenefitAmountOfParticipantFromYearlyDetail(this, lbusBenefitCalculationDetail, lintYear, ablnGetAmountForReemployment: lblnGetAmountForReemployment);

                                            if(ldecTaxableAmtPaid > 0)
                                            { 
                                                lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.taxable_amount_to_be_paid = ldecTaxableAmtPaid - ldecNonTaxableAmount;
                                            }
                                            ldecRetroGrossAmt += ibusCalculation.GetBenefitAmountOfParticipantFromYearlyDetail(this, lbusBenefitCalculationDetail, lintYear, ablnGetAmountForReemployment: lblnGetAmountForReemployment);
                                        }
                                        else
                                        {
                                            lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.non_taxable_amount_to_be_paid = ldecNonTaxableAmount;
                                            lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.taxable_amount_to_be_paid = ldecTaxableAmount;

                                            ldecRetroGrossAmt += ldecTaxableAmount;
                                        }
                                    }
                                    
                                }
                                
                                if (this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.monthly_exclusion_amount > 0)
                                {
                                    ldecRetroNonTaxableAmt = this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.monthly_exclusion_amount * lintDiffinMonths;
                                    ldecRetroTaxableAmt = ldecRetroGrossAmt - ldecRetroNonTaxableAmt;
                                }
                                else
                                    ldecRetroTaxableAmt = ldecRetroGrossAmt;
                                
                                ibusCalculation.CreateOverpaymentUnderPayment(lbusPayeeAccount,lclbBenefitMonthwiseAdjustmentDetail,busConstant.RETRO_PAYMENT_INITIAL);
                            }
                            else
                            {
                                lbusPayeeAccount.LoadPayeeAccountPaymentItemType();                                                            
                                ibusCalculation.CalculateAmountShouldHaveBeenPaid(lbusPayeeAccount, ref lclbBenefitMonthwiseAdjustmentDetail);
                                ibusCalculation.CreateOverpaymentUnderPayment(lbusPayeeAccount, lclbBenefitMonthwiseAdjustmentDetail, busConstant.RETRO_PAYMENT_INITIAL);
                            }
                            // PROD PIR 127
                            if (lbusEarlyRetirementPayeeAccount != null && this.icdoBenefitCalculationHeader.payee_account_id > 0)
                            {
                                Early_Retirement_Benefit_Application_Id = lbusEarlyRetirementPayeeAccount.icdoPayeeAccount.iintBenefitApplicationID;
                            }
                            this.CreatePayeeAccountForRetireeIncrease(lbusPayeeAccount,lintBenefitAccountID, lstrFamilyRelationshipValue, ldecNonTaxableBeginningBalance, ldteNextBenefitPaymentDate, Early_Retirement_Benefit_Application_Id, busConstant.BENEFIT_TYPE_DISABILITY);
                        }

                        else if (icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT)
                        {

                            //PIR 985 10262015
                            if (this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                                this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.final_monthly_benefit_amount < 0)
                                ;
                            else
                                lbusPayeeAccount.CreateReviewPayeeAccountStatus(ablnFromApprovedCalc: true);// PIR 1055

                            //PIR 993  added check for Lumpsum benefit type for MPIPP plan.
                            if ((!this.iclbBenefitCalculationDetail.First().iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.istrBenefitOptionDescription.Contains(busConstant.LUMP_SUM_DESCRIPTION))
                                && (this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.plan_id != busConstant.IAP_PLAN_ID))
                            {
                                //Payment Adjustments - Benefit Adjustment Batch
                                bool lblnAdjustmentCalculationForRetireeIncrease = false;
                                if (iclbDisabilityRetireeIncrease != null && iclbDisabilityRetireeIncrease.Count > 0)
                                    lblnAdjustmentCalculationForRetireeIncrease = true;

                                lclbBenefitMonthwiseAdjustmentDetail = ibusCalculation.GetAmountActuallyPaid(lbusPayeeAccount, adtAdjustFromDate, lbusPayeeAccount.idtLastBenefitPaymentDate, lblnAdjustmentCalculationForRetireeIncrease);
                                if (!idictWorkHrsAfterRetirement.IsNullOrEmpty())
                                {
                                    int lintMonth = 0;
                                    int lintYear = 0;
                                    DateTime ldtParticipantTurns65 = this.ibusPerson.icdoPerson.date_of_birth.AddYears(65);
                                    if (ldtParticipantTurns65.Day != 1)
                                    {
                                        ldtParticipantTurns65 = ldtParticipantTurns65.GetLastDayofMonth().AddDays(1);
                                    }
                                    bool lblnGetAmountForReemployment = false;

                                    if(lclbBenefitMonthwiseAdjustmentDetail.Count > 0)
                                        lclbBenefitMonthwiseAdjustmentDetail = lclbBenefitMonthwiseAdjustmentDetail.OrderBy(i => i.icdoBenefitMonthwiseAdjustmentDetail.payment_date).ToList().ToCollection();

                                    foreach (busBenefitMonthwiseAdjustmentDetail lbusBenefitMonthwiseAdjustmentDetail in lclbBenefitMonthwiseAdjustmentDetail)
                                    {
                                        lblnGetAmountForReemployment = false;
                                        if (lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date >= ldtParticipantTurns65)
                                        {
                                            lblnGetAmountForReemployment = true;
                                        }
                                        lintMonth = lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date.Month;
                                        lintYear = lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date.Year;
                                        //SuspendibleHoursChange
                                        if ((idictWorkHrsAfterRetirement.Keys.Contains(lintYear) && idictWorkHrsAfterRetirement[lintYear][lintMonth] >= ibusCalculation.GetSuspendibleHoursValue(lintYear, lintMonth)) || ibusCalculation.CheckIfMonthIsSuspendibleMonthFromNonSignatory(this.ibusPerson, lintMonth, lintYear))
                                        {
                                            //SuspendibleHoursChange
                                            if (idictWorkHrsAfterRetirement.Keys.Contains(lintYear) && idictWorkHrsAfterRetirement[lintYear][lintMonth] >= ibusCalculation.GetSuspendibleHoursValue(lintYear, lintMonth))
                                            {
                                                lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.hours = idictWorkHrsAfterRetirement[lintYear][lintMonth];
                                            }
                                            lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.suspended_flag = busConstant.FLAG_YES;
                                            if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.ee_derived_benefit_amount > 0)
                                            {
                                                lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.non_taxable_amount_to_be_paid = ldecNonTaxableAmount;
                                                lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.taxable_amount_to_be_paid = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.ee_derived_benefit_amount - ldecNonTaxableAmount;
                                            }
                                        }
                                        else
                                        {
                                            if (this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID)
                                            {
                                                if (!lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.IsNullOrEmpty())
                                                {
                                                    ldecTaxableAmtPaid = decimal.Zero;
                                                    //RequestID: 72091
                                                    if(icdoBenefitCalculationHeader.iblnPopUpToLife && lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date < icdoBenefitCalculationHeader.idtJointAnnuitantDOD)
                                                    { 
                                                        lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.non_taxable_amount_to_be_paid = ldecPopupNonTaxableAmount;
                                                        ldecTaxableAmtPaid = ibusCalculation.GetBenefitAmountOfParticipantFromYearlyDetail(this, lbusBenefitCalculationDetail, lintYear, ablnGetAmountForReemployment: lblnGetAmountForReemployment,ldtPaymentDate: lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date);
                                                        if (ldecTaxableAmtPaid > 0)
                                                        {
                                                            lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.taxable_amount_to_be_paid = ldecTaxableAmtPaid - ldecPopupNonTaxableAmount;
                                                        }
                                                    }
                                                    else if(icdoBenefitCalculationHeader.iblnPopUpToLife && lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date > icdoBenefitCalculationHeader.idtJointAnnuitantDOD)
                                                    { 
                                                        lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.non_taxable_amount_to_be_paid = ldecNonTaxableAmount;
                                                        ldecTaxableAmtPaid = ibusCalculation.GetBenefitAmountOfParticipantFromYearlyDetail(this, lbusBenefitCalculationDetail, lintYear, ablnGetAmountForReemployment: lblnGetAmountForReemployment, ldtPaymentDate: lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date);
                                                        if (ldecTaxableAmtPaid > 0)
                                                        {
                                                            lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.taxable_amount_to_be_paid = ldecTaxableAmtPaid - ldecNonTaxableAmount;
                                                        }
                                                    }
                                                    else
                                                    { 
                                                        lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.non_taxable_amount_to_be_paid = ldecNonTaxableAmount;
                                                        ldecTaxableAmtPaid = ibusCalculation.GetBenefitAmountOfParticipantFromYearlyDetail(this, lbusBenefitCalculationDetail, lintYear, ablnGetAmountForReemployment: lblnGetAmountForReemployment);
                                                        if (ldecTaxableAmtPaid > 0)
                                                        {
                                                            lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.taxable_amount_to_be_paid = ldecTaxableAmtPaid - ldecNonTaxableAmount;
                                                        }
                                                    }
                                                                                                   
                                                }
                                            }
                                            else
                                            {
                                                lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.taxable_amount_to_be_paid = ldecTaxableAmount;
                                            }
                                        }
                                    }

                                }

                                else
                                {
                                    ibusCalculation.CalculateAmountShouldHaveBeenPaid(lbusPayeeAccount, ref lclbBenefitMonthwiseAdjustmentDetail);

                                    //RequestID: 72091
                                    foreach(busBenefitMonthwiseAdjustmentDetail lbusBenefitMonthwiseAdjustmentDetail in lclbBenefitMonthwiseAdjustmentDetail)
                                    {
                                        if(icdoBenefitCalculationHeader.iblnPopUpToLife && lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date < icdoBenefitCalculationHeader.idtJointAnnuitantDOD)
                                        {
                                            lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.non_taxable_amount_to_be_paid = ldecPopupNonTaxableAmount;
                                            lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.taxable_amount_to_be_paid = ldecPopUpBenefitAmount - ldecPopupNonTaxableAmount;
                                        }
                                    }
                                }

                                ibusCalculation.CalculateRetireeIncreaseAmountShouldHaveBeenPaid(lbusPayeeAccount, iclbDisabilityRetireeIncrease, ref lclbBenefitMonthwiseAdjustmentDetail); // PROD PIR 581
                                ibusCalculation.CreateOverpaymentUnderPayment(lbusPayeeAccount, lclbBenefitMonthwiseAdjustmentDetail, busConstant.RETRO_PAYMENT_ADJUSTMENT_BATCH);

                                //PIR 607
                                ibusCalculation.CreatePayeeAccountForRetireeIncrease(lintBenefitAccountID, lstrFamilyRelationshipValue, ldecNonTaxableBeginningBalance, ldteNextBenefitPaymentDate,
                                  lintPayeeAccountID, iclbDisabilityRetireeIncrease, this, null, iclbBenefitCalculationDetail, null);
                            }
                            #region ReCalculate Post Retirement DRO

                            DataTable ldtbList = Select("cdoDroApplication.LoadAllExistingQDRO", new object[1] { lbusPayeeAccount.icdoPayeeAccount.person_id });

                            foreach (DataRow ldrDro in ldtbList.Rows)
                            {
                                if (ldrDro[enmDroBenefitDetails.dro_model_value.ToString()].ToString() == busConstant.DRO_MODEL_VALUE_STANDARD_RETIREE_FORMULA)
                                {

                                    busPayeeAccount lbusQDROPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
                                    if (lbusQDROPayeeAccount.FindPayeeAccount(Convert.ToInt32(ldrDro[enmPayeeAccount.payee_account_id.ToString().ToUpper()])))
                                    {
                                        lbusPayeeAccount.ReCalculateBenefitForQDRO(lbusQDROPayeeAccount, abusParticipantPayeeAccount: lbusPayeeAccount);
                                    }
                                }
                            }

                            #endregion

                        }
                        else
                        {
                            // PROD PIR 127
                            if (lbusEarlyRetirementPayeeAccount != null && this.icdoBenefitCalculationHeader.payee_account_id > 0)
                            {
                                Early_Retirement_Benefit_Application_Id = lbusEarlyRetirementPayeeAccount.icdoPayeeAccount.iintBenefitApplicationID;
                            }
                            this.CreatePayeeAccountForRetireeIncrease(lbusPayeeAccount,lintBenefitAccountID, lstrFamilyRelationshipValue, ldecNonTaxableBeginningBalance, ldteNextBenefitPaymentDate, Early_Retirement_Benefit_Application_Id, busConstant.BENEFIT_TYPE_DISABILITY);

                        }


                        if (this.ibusBaseActivityInstance.IsNotNull())
                        {
                            this.SetWFVariables4PayeeAccount(lintPayeeAccountID, this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.plan_id);
                            this.SetProcessInstanceParameters();
                        }
                    }
                }

                #endregion

                #region TO POST PARTIAL INTEREST AND QUATERLY IAP ALLOCATIONS IN THE RETIREMENT CONTRIBUTION TABLE

                if (this.icdoBenefitCalculationHeader.calculation_type_value != busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT)
                {
                    foreach (busBenefitCalculationDetail lbusBenefitCalculationDetail in this.iclbBenefitCalculationDetail)
                    {
                        if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID)
                        {

                            int lintPersonAccountId = 0;

                            #region Reverse the interest already posted to the participants account for Disability Conversion

                            //Interest will get reverted on click of cancel button in application detail.

                            #endregion


                            lintPersonAccountId = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().icdoPersonAccount.person_account_id;
                            // Insert the Partial Interest for EE Contributions in the SGT_PERSON_RETIREMENT_CONTRIBUTION table
                            decimal ldecPriorYearEEInterestAmount = decimal.Zero;
                            decimal ldecPartialEEInterestAmount = ibusCalculation.CalculatePartialEEInterest(this.icdoBenefitCalculationHeader.retirement_date,
                                            this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault(),
                                            true, false, iclbPersonAccountRetirementContribution, out ldecPartialEEInterestAmount);

                            if (ldecPartialEEInterestAmount - ldecPriorYearEEInterestAmount > decimal.Zero)
                            {
                                busPersonAccountRetirementContribution lbusPersonAccountRetirementContribution = new busPersonAccountRetirementContribution() { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };
                                lbusPersonAccountRetirementContribution.InsertPersonAccountRetirementContirbution(lintPersonAccountId, busGlobalFunctions.GetLastDateOfComputationYear(this.icdoBenefitCalculationHeader.retirement_date.Year - 1).AddDays(1),
                                    DateTime.Now, this.icdoBenefitCalculationHeader.retirement_date.Year, adecEEInterestAmount: ldecPartialEEInterestAmount - ldecPriorYearEEInterestAmount, astrTransactionType: busConstant.TRANSACTION_TYPE_INTEREST,
                                    astrContributionType: busConstant.CONTRIBUTION_TYPE_EE, astrContributionSubtype: busConstant.CONTRIBUTION_SUBTYPE_VESTED, aintReferenceID: this.icdoBenefitCalculationHeader.benefit_calculation_header_id);
                            }
                            if (ldecPriorYearEEInterestAmount > decimal.Zero)
                            {
                                busPersonAccountRetirementContribution lbusPersonAccountRetirementContribution = new busPersonAccountRetirementContribution() { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };
                                lbusPersonAccountRetirementContribution.InsertPersonAccountRetirementContirbution(lintPersonAccountId, busGlobalFunctions.GetLastDateOfComputationYear(this.icdoBenefitCalculationHeader.retirement_date.Year - 1).AddDays(1),
                                    DateTime.Now, this.icdoBenefitCalculationHeader.retirement_date.AddYears(-1).Year, adecEEInterestAmount: ldecPriorYearEEInterestAmount, astrTransactionType: busConstant.TRANSACTION_TYPE_INTEREST,
                                    astrContributionType: busConstant.CONTRIBUTION_TYPE_EE, astrContributionSubtype: busConstant.CONTRIBUTION_SUBTYPE_VESTED, aintReferenceID: this.icdoBenefitCalculationHeader.benefit_calculation_header_id);

                            }
                        }

                    }
                }


                // Insert the IAP Quarterly Allocations in the SGT_PERSON_RETIREMENT_CONTRIBUTION table
                if (this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).Count() > 0)
                {
                    bool lblnCalculateIapAllocations = true;
                    if (this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.adjustment_iap_payment_flag == busConstant.FLAG_YES)
                    {
                        lblnCalculateIapAllocations = false;
                    }
                    //PIR 534: OPUS should not be push quarterly allocations and RY allocations when approving Adjustment Calc.
                    //if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT && !lblnDeductPrevIAPBalance)
                    if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT)
                    {
                        lblnCalculateIapAllocations = false;
                    }
                    if (lblnCalculateIapAllocations)
                    {
                        int lintPersonAccountId = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().icdoPersonAccount.person_account_id;

                        if ((from item in this.iclbPersonAccountRetirementContribution
                             where item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId
                             select item.icdoPersonAccountRetirementContribution.iap_balance_amount).Sum() > 0)
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

                if (this.ibusBaseActivityInstance.IsNotNull())
                {
                    this.SetProcessInstanceParameters();
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
                bool lblnInsertEEPartialInterest = busConstant.BOOL_FALSE;
                foreach (busBenefitCalculationDetail lbusBenefitCalculationDetail in this.iclbBenefitCalculationDetail)
                {
                    if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID)
                    {
                        int lintPersonAccountId = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().icdoPersonAccount.person_account_id;
                        // Insert the Partial Interest for EE Contributions in the SGT_PERSON_RETIREMENT_CONTRIBUTION table
                        if (!lblnInsertEEPartialInterest)
                        {
                            DataTable ltblEEPartialInterest = Select("cdoPersonAccountRetirementContribution.GetEE&UVHPPartialInterest", new object[2] {this.icdoBenefitCalculationHeader.benefit_calculation_header_id,
                                        busConstant.CONTRIBUTION_TYPE_EE});
                            if (ltblEEPartialInterest.IsNotNull() && ltblEEPartialInterest.Rows.Count > 0 && Convert.ToString(ltblEEPartialInterest.Rows[0][0]).IsNotNullOrEmpty())
                            {
                                ldecPartialEEInterestAmount = -(Convert.ToDecimal(ltblEEPartialInterest.Rows[0][0]));
                                busPersonAccountRetirementContribution lbusPersonAccountRetirementContribution = new busPersonAccountRetirementContribution() { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };
                                lbusPersonAccountRetirementContribution.InsertPersonAccountRetirementContirbution(lintPersonAccountId, busGlobalFunctions.GetLastDateOfComputationYear(this.icdoBenefitCalculationHeader.retirement_date.Year - 1).AddDays(1),
                                    DateTime.Now, this.icdoBenefitCalculationHeader.retirement_date.Year, adecEEInterestAmount: ldecPartialEEInterestAmount,
                                    //astrTransactionType: busConstant.TRANSACTION_TYPE_CANCELLED_CALCULATION
                                     astrTransactionType: Convert.ToString(ltblEEPartialInterest.Rows[0][1])
                                    , astrContributionType: busConstant.CONTRIBUTION_TYPE_EE, astrContributionSubtype: busConstant.CONTRIBUTION_SUBTYPE_VESTED, aintReferenceID: this.icdoBenefitCalculationHeader.benefit_calculation_header_id);
                                lblnInsertEEPartialInterest = busConstant.BOOL_TRUE;
                            }
                        }

                        // Insert the Partial Interest for UV & HP Contributions in the SGT_PERSON_RETIREMENT_CONTRIBUTION table
                        if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.uvhp_flag == busConstant.FLAG_YES)
                        {
                            DataTable ltblUVHPPartialInterest = Select("cdoPersonAccountRetirementContribution.GetEE&UVHPPartialInterest", new object[2] {this.icdoBenefitCalculationHeader.benefit_calculation_header_id,
                                        busConstant.CONTRIBUTION_TYPE_UVHP});
                            if (ltblUVHPPartialInterest.IsNotNull() && ltblUVHPPartialInterest.Rows.Count > 0 && Convert.ToString(ltblUVHPPartialInterest.Rows[0][0]).IsNotNullOrEmpty())
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

                #region Revert IAP Contribution
                if (this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).Count() > 0)
                {
                    if (this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.reference_id
                        == this.icdoBenefitCalculationHeader.benefit_calculation_header_id).Count() > 0)
                    {
                        this.ibusCalculation.NegateAllocationsCreatedByCalculation(this.icdoBenefitCalculationHeader.benefit_calculation_header_id);
                    }
                }
                #endregion
            }
        }
            return this.iarrErrors;
        }

        public override void ValidateHardErrors(utlPageMode aenmPageMode)
        {
            base.ValidateHardErrors(aenmPageMode);
            utlError lobjError = null;
            if (!this.ibusBenefitApplication.iclbEligiblePlans.Contains(this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().icdoPersonAccount.istrPlanCode))
            {
                lobjError = AddError(5434, "");
                this.iarrErrors.Add(lobjError);
                return;
            }

            //RID 89976 business asked to remove this validation.
            /*
            if (this.icdoBenefitCalculationHeader.istrRetirementType == busConstant.RETIREMENT_TYPE_LATE)
            {
                if (string.IsNullOrEmpty(istrLateRetirementFlag) || (istrLateRetirementFlag == busConstant.FLAG_NO))
                {
                    lobjError = AddError(5435, "");
                    this.iarrErrors.Add(lobjError);
                }
            }
            */
            if (!this.iblnCheckIfHoursAfterDisabilityRetirementDate && this.icdoBenefitCalculationHeader.retirement_date_option_2 != DateTime.MinValue)
            {
                lobjError = AddError(5431, "");
                this.iarrErrors.Add(lobjError);
            }
            else if (this.iblnCheckIfHoursAfterDisabilityRetirementDate && this.icdoBenefitCalculationHeader.retirement_date_option_2 != DateTime.MinValue && this.icdoBenefitCalculationHeader.retirement_date_option_2 <= this.icdoBenefitCalculationHeader.retirement_date)
            {
                lobjError = AddError(5468, "");
                this.iarrErrors.Add(lobjError);
            }
            if (this.icdoBenefitCalculationHeader.iintPlanId != busConstant.MPIPP_PLAN_ID && this.icdoBenefitCalculationHeader.iintPlanId != busConstant.IAP_PLAN_ID)
            {
                if (this.ibusPerson.iclbPersonAccount.Where(local => local.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).Count() > 0)
                {
                    DateTime ldDate = this.icdoBenefitCalculationHeader.ssa_approval_date;
                    if (ldDate == DateTime.MinValue)
                    {
                        ldDate = this.icdoBenefitCalculationHeader.retirement_date;
                    }
                    DateTime ldtMergerDate = this.ibusPerson.iclbPersonAccount.Where(local => local.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().icdoPersonAccount.idtMergerDate;
                    if (ldDate < ldtMergerDate)
                    {
                        lobjError = AddError(5448, "");
                        this.iarrErrors.Add(lobjError);
                    }
                }
            }
        }


        #endregion

        #region PRIVATE

        private void SetupPreRequisites_DisabilityCalculations()
        {
            if (!this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.IsNullOrEmpty())
            {
                this.icdoBenefitCalculationHeader.istrRetirementType = string.Empty;
                this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.ForEach(a => a.icdoPersonAccount.istrRetirementSubType = string.Empty);
                this.ibusBenefitApplicationRetirement = ibusBenefitApplication;
                this.ibusBenefitApplicationRetirement.ibusPerson.iclbPersonAccount.ForEach(a => a.icdoPersonAccount.istrRetirementSubType = string.Empty);
                this.ibusBenefitApplicationRetirement.LoadWorkHistoryandSetupPrerequisites_Retirement();
                this.ibusBenefitApplication.LoadWorkHistoryandSetupPrerequisites_Disability();
                //Not To be Shown on Screen
                this.icdoBenefitCalculationHeader.istrRetirementType = this.ibusBenefitApplicationRetirement.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().icdoPersonAccount.istrRetirementSubType;
                GetEligiblityForRegularBenefit();
            }
        }

        private void Setup_Disabilty_Calculations()
        {
            DateTime ldtVestedDate = DateTime.MinValue;

            #region SETUP BENEFIT CALCULATION DETAIL BASED ON WHAT ESTIMATE HAS BEEN ASKED FOR
            if (this.iclbBenefitCalculationDetail == null)
            {
                this.iclbBenefitCalculationDetail = new Collection<busBenefitCalculationDetail>();
            }
            if (this.icdoBenefitCalculationHeader.iintPlanId == busConstant.MPIPP_PLAN_ID)
            {
                #region Setup Detail Records for MPIs Estimate
                ldtVestedDate = DateTime.MinValue;
                busBenefitCalculationDetail lbusBenefitCalculationDetail = new busBenefitCalculationDetail { icdoBenefitCalculationDetail = new cdoBenefitCalculationDetail() };
                if (this.ibusBenefitApplication.CheckAlreadyVested(busConstant.MPIPP))
                {
                    ldtVestedDate = this.ibusBenefitApplication.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date;
                }
                lbusBenefitCalculationDetail.LoadData(this.icdoBenefitCalculationHeader.benefit_calculation_header_id, this.icdoBenefitCalculationHeader.iintPlanId,
                    this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).First().icdoPersonAccount.person_account_id,
                    ldtVestedDate, this.icdoBenefitCalculationHeader.istrDisabilityType,
                    busConstant.BENEFIT_TYPE_DISABILITY, this.ibusBenefitApplication.icdoBenefitApplication.retirement_date);

                lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.istrPlanCode = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).First().icdoPersonAccount.istrPlanCode;
                if (this.idecFinalAccruedBenefitAmount != busConstant.ZERO_DECIMAL)
                {
                    lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.unreduced_benefit_amount = idecFinalAccruedBenefitAmount;

                }
                lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.ee_derived_benefit_amount = ibusCalculation.CalculateEEDerivedBenefitAsOfRetirementDate(this, null);

                lbusBenefitCalculationDetail.iobjMainCDO = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail;
                this.iclbBenefitCalculationDetail.Add(lbusBenefitCalculationDetail);

                if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE)
                {
                    ldtVestedDate = DateTime.MinValue;
                    busBenefitCalculationDetail lbusBenefitCalculationDetailIAP = new busBenefitCalculationDetail { icdoBenefitCalculationDetail = new cdoBenefitCalculationDetail() };
                    if (this.ibusBenefitApplication.CheckAlreadyVested(busConstant.IAP))
                    {
                        ldtVestedDate = this.ibusBenefitApplication.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date;
                    }
                    lbusBenefitCalculationDetailIAP.LoadData(this.icdoBenefitCalculationHeader.benefit_calculation_header_id, busConstant.IAP_PLAN_ID,
                        this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.IAP_PLAN_ID).First().icdoPersonAccount.person_account_id,
                        ldtVestedDate, this.icdoBenefitCalculationHeader.istrDisabilityType,
                        busConstant.BENEFIT_TYPE_DISABILITY, this.ibusBenefitApplication.icdoBenefitApplication.retirement_date);
                    lbusBenefitCalculationDetailIAP.icdoBenefitCalculationDetail.istrPlanCode = busConstant.IAP;
                    lbusBenefitCalculationDetailIAP.iobjMainCDO = lbusBenefitCalculationDetailIAP.icdoBenefitCalculationDetail;
                    this.iclbBenefitCalculationDetail.Add(lbusBenefitCalculationDetailIAP);
                }

                #endregion
            }
            else if (this.icdoBenefitCalculationHeader.iintPlanId == busConstant.IAP_PLAN_ID)
            {
                #region Setup Detail Record for IAPs Estimate
                ldtVestedDate = DateTime.MinValue;
                busBenefitCalculationDetail lbusBenefitCalculationDetailIAP = new busBenefitCalculationDetail { icdoBenefitCalculationDetail = new cdoBenefitCalculationDetail() };
                lbusBenefitCalculationDetailIAP.iobjMainCDO = lbusBenefitCalculationDetailIAP.icdoBenefitCalculationDetail;
                if (this.ibusBenefitApplication.CheckAlreadyVested(busConstant.IAP))
                {
                    ldtVestedDate = this.ibusBenefitApplication.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date;
                }
                lbusBenefitCalculationDetailIAP.LoadData(this.icdoBenefitCalculationHeader.benefit_calculation_header_id, this.icdoBenefitCalculationHeader.iintPlanId,
                    this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).First().icdoPersonAccount.person_account_id,
                    ldtVestedDate, this.icdoBenefitCalculationHeader.istrDisabilityType,
                    busConstant.BENEFIT_TYPE_DISABILITY, this.ibusBenefitApplication.icdoBenefitApplication.retirement_date);
                lbusBenefitCalculationDetailIAP.icdoBenefitCalculationDetail.istrPlanCode = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).First().icdoPersonAccount.istrPlanCode;
                this.iclbBenefitCalculationDetail.Add(lbusBenefitCalculationDetailIAP);

                #endregion
            }
            else
            {
                #region Setup Detail Record for Locals
                if (this.ibusBenefitApplication.CheckAlreadyVested(this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).First().icdoPersonAccount.istrPlanCode))
                    ldtVestedDate = this.ibusBenefitApplication.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date;
                busBenefitCalculationDetail lbusBenefitCalculationDetail = new busBenefitCalculationDetail { icdoBenefitCalculationDetail = new cdoBenefitCalculationDetail() };
                lbusBenefitCalculationDetail.iobjMainCDO = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail;
                lbusBenefitCalculationDetail.LoadData(this.icdoBenefitCalculationHeader.benefit_calculation_header_id, this.icdoBenefitCalculationHeader.iintPlanId,
                this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).First().icdoPersonAccount.person_account_id,
                ldtVestedDate, this.icdoBenefitCalculationHeader.istrDisabilityType,
                busConstant.BENEFIT_TYPE_DISABILITY, this.ibusBenefitApplication.icdoBenefitApplication.retirement_date);
                lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.istrPlanCode = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).First().icdoPersonAccount.istrPlanCode;
                lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.unreduced_benefit_amount = idecLocalFrozenBenefit;
                this.iclbBenefitCalculationDetail.Add(lbusBenefitCalculationDetail);
                #endregion
            }
            #endregion

            #region SWITCH CASE - INITIATE CALCULATION BASED ON THE REQUIRED PLAN'S ESTIMATE
            if (!this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.IsNullOrEmpty())
            {
                decimal ldecTotalBenefitAmount;
                int lintPersonAccountId = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).First().icdoPersonAccount.person_account_id;
                if (string.IsNullOrEmpty(this.ibusBenefitApplication.icdoBenefitApplication.benefit_type_value))
                {
                    this.ibusBenefitApplication.icdoBenefitApplication.benefit_type_value = busConstant.BENEFIT_TYPE_DISABILITY;
                }
                switch (this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).First().icdoPersonAccount.istrPlanCode)
                {

                    case busConstant.Local_161:
                        CalculateLocal161Benefit(busConstant.CodeValueAll, busConstant.BOOL_FALSE);
                        break;
                    case busConstant.Local_52:
                        CalculateLocal52Benefit(busConstant.CodeValueAll, busConstant.BOOL_FALSE);
                        break;

                    case busConstant.Local_600:
                        CalculateLocal600Benefit(busConstant.CodeValueAll, busConstant.BOOL_FALSE);
                        break;

                    case busConstant.Local_666:
                        CalculateLocal666Benefit(busConstant.CodeValueAll, busConstant.BOOL_FALSE);
                        break;

                    case busConstant.LOCAL_700:
                        CalculateLocal700Benefit(busConstant.CodeValueAll, busConstant.BOOL_FALSE);
                        break;

                    case busConstant.MPIPP:
                        if (this.ibusBenefitApplication.aclbPersonWorkHistory_MPI != null && this.ibusBenefitApplication.aclbPersonWorkHistory_MPI.Count > 0)
                        {
                            this.CalculateMPIBenefitOptions(busConstant.CodeValueAll);
                            //this.CalculateAccruedBenefitForPension(busConstant.CodeValueAll, this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault());
                        }

                        if (this.ibusBenefitApplication.aclbPersonWorkHistory_IAP != null && this.ibusBenefitApplication.aclbPersonWorkHistory_IAP.Count > 0)
                        {
                            this.CalculateIAPBenefitAmount(busConstant.CodeValueAll);
                        }
                        break;

                    case busConstant.IAP:
                        if (this.ibusBenefitApplication.aclbPersonWorkHistory_IAP != null && this.ibusBenefitApplication.aclbPersonWorkHistory_IAP.Count > 0)
                        {
                            this.CalculateIAPBenefitAmount(busConstant.CodeValueAll);
                        }
                        break;
                }
            }
            #endregion

        }

        #region PENSION

        //public void LoadandProcessMPIWorkHistoryTillRetirementDate()
        //{
        //    DataTable ldtbPensionCredits = new DataTable();

        //    utlConnection utlLegacyDBConnetion = HelperFunction.GetDBConnectionProperties("Legacy");
        //    string astrLegacyDBConnetion = utlLegacyDBConnetion.istrConnectionString;

        //    SqlParameter[] parameters = new SqlParameter[3];
        //    SqlParameter param1 = new SqlParameter("@SSN", DbType.String);
        //    SqlParameter param2 = new SqlParameter("@PLANCODE", DbType.String);
        //    SqlParameter param3 = new SqlParameter("@RETIREMENT_DATE", DbType.DateTime);


        //        #region Get MPI Plan Work History Till Disability Retirement Date
        //        if (!this.ibusPerson.icdoPerson.ssn.IsNullOrEmpty())
        //        {
        //            param1.Value = this.ibusPerson.icdoPerson.istrSSNNonEncrypted;
        //            parameters[0] = param1;

        //            param2.Value = busConstant.MPIPP;
        //            parameters[1] = param2;

        //            param3.Value = this.ibusBenefitApplication.icdoBenefitApplication.retirement_date;
        //            parameters[2] = param3;

        //            DataTable ldtPersonWorkHistory_MPI = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetWorkDataTillGivenDate", astrLegacyDBConnetion, parameters);
        //            if (ldtPersonWorkHistory_MPI.Rows.Count > 0)
        //            {
        //                this.iclbMPIWorkHistoryTillDisRetirementDate = cdoDummyWorkData.GetCollection<cdoDummyWorkData>(ldtPersonWorkHistory_MPI);

        //                this.iclbMPIWorkHistoryTillDisRetirementDate = this.ibusBenefitApplication.PaddingForBridgingService(this.iclbMPIWorkHistoryTillDisRetirementDate);
        //                this.ibusBenefitApplication.ProcessWorkHistoryPadding(this.iclbMPIWorkHistoryTillDisRetirementDate, busConstant.MPIPP, false);

        //                this.ibusBenefitApplication.ProcessWorkHistoryforBISandForfieture(this.iclbMPIWorkHistoryTillDisRetirementDate, busConstant.MPIPP,false);

        //                if (this.ibusBenefitApplication.CheckAlreadyVested(busConstant.MPIPP)) //IMPORTANT TO DO THIS HERE
        //                {
        //                    this.ibusBenefitApplication.ProcessWorkHistorytoRemoveUnwantedForFieture(this.iclbMPIWorkHistoryTillDisRetirementDate, busConstant.MPIPP, this.ibusBenefitApplication.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date, false);
        //                }

        //            }
        //        }
        //        #endregion

        //}

        public bool iblnOptionTwoFlag = false; // PROD PIR 504
        private void CalculateMPIBenefitOptionTwo()
        {
            if (this.icdoBenefitCalculationHeader.retirement_date_option_2 != DateTime.MinValue && this.icdoBenefitCalculationHeader.retirement_date_option_2 > this.icdoBenefitCalculationHeader.retirement_date)
            {
                iblnOptionTwoFlag = true; // PROD PIR 504
                SetUpDisabilityVariablesForCalculation(this.icdoBenefitCalculationHeader.retirement_date_option_2);
                SetupPreRequisites_DisabilityCalculations();
                Setup_Disabilty_Calculations();
                iblnOptionTwoFlag = false; // PROD PIR 504
            }
        }

        private decimal CalculateMPIAccuredBenefitAmount()
        {
            idecFinalAccruedBenefitAmount =
            this.ibusCalculation.CalculateUnReducedBenefitAmtForPension(ibusPerson, this.icdoBenefitCalculationHeader.idecParticipantFullAge, this.ibusBenefitApplication.icdoBenefitApplication.retirement_date,
            ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault(),
            ibusBenefitApplication, busConstant.BOOL_FALSE,
            iclbBenefitCalculationDetail, null, ibusBenefitApplication.aclbPersonWorkHistory_MPI, this.icdoBenefitCalculationHeader.istrRetirementType, this.icdoBenefitCalculationHeader.benefit_type_value);

            busPersonAccountEligibility lbusPersonAccountEligibility = new busPersonAccountEligibility();
            lbusPersonAccountEligibility = lbusPersonAccountEligibility.LoadPersonAccEligibilityByPersonAccountId(ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().icdoPersonAccount.person_account_id);

            #region Check if Withdrawal History Exists: Then Acrrued benefit = Accrued Benefit - EE derived
            Collection<cdoPersonAccountRetirementContribution> lclbRetCont = new Collection<cdoPersonAccountRetirementContribution>();
            idecFinalAccruedBenefitAmount = ibusCalculation.CalculateAccruedBenefitForPersonWithWithdrawal(this.icdoBenefitCalculationHeader.istrRetirementType,
                                                            lbusPersonAccountEligibility.icdoPersonAccountEligibility.vested_date,
                                                            idecFinalAccruedBenefitAmount, this.ibusBenefitApplication.ibusPerson, this.ibusBenefitApplication.ibusPerson.iclbPersonAccount,
                                                            icdoBenefitCalculationHeader.retirement_date, this.ibusBenefitApplication.aclbPersonWorkHistory_MPI,
                                                            this.iclbPersonAccountRetirementContribution,
                                                            lbusPersonAccountEligibility.icdoPersonAccountEligibility.forfeiture_date.Year, ref lclbRetCont);

            iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoBenefitCalculationDetail.retirement_date ==
                ibusBenefitApplication.icdoBenefitApplication.retirement_date).FirstOrDefault().icdoBenefitCalculationDetail.unreduced_benefit_amount = idecFinalAccruedBenefitAmount;

            #endregion

            return idecFinalAccruedBenefitAmount;
        }

        private void CalculateMPIBenefitOptions(string astrBenefitOptionValue, bool ablnConvertBenOption = false, string astrOriginalBenefitOption = "") //RequestID: 72091
        {
            CalculateMPIAccuredBenefitAmount();
            if (idecFinalAccruedBenefitAmount != busConstant.ZERO_DECIMAL || this.icdoBenefitCalculationHeader.istrRetirementType == busConstant.RETIREMENT_TYPE_LATE)
            {
                //if (this.icdoBenefitCalculationHeader.payment_date != DateTime.MinValue && iblnCheckIfHoursAfterDisabilityRetirementDate &&
                //    this.icdoBenefitCalculationHeader.retirement_date == this.ibusBenefitApplication.icdoBenefitApplication.retirement_date)
                //{
                //    SetRetroactiveAdjustmentVariables();
                //}
                if (iblnEligibleForRegularBenefit)
                {
                    //RequestID: 72091
                    CalculateMPIBenefitOptionsForRegularAndDisability(idecFinalAccruedBenefitAmount, astrBenefitOptionValue,ablnConvertBenOption,astrOriginalBenefitOption);
                }
                else
                {
                    CalculateMPIBenefitOptionsForDiabledPerson(idecFinalAccruedBenefitAmount, astrBenefitOptionValue, ablnConvertBenOption, astrOriginalBenefitOption);
                }


            }
            CalculateMgAndMea();

            if (this.iblnCalcualteUVHPBenefit)
            {
                decimal ldecTotalBenefitAmount = this.ibusCalculation.FetchUVHPAmountandInterest(false, null, this.iclbBenefitCalculationDetail, this, null, iintPersonAccountId, this.ibusBenefitApplication.icdoBenefitApplication.retirement_date);
                decimal ldecNonVestedEEAndUVHPAmt = ldecTotalBenefitAmount + ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().icdoPersonAccount.idecNonVestedEE +
                                                    ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().icdoPersonAccount.idecNonVestedEEInterest;

                CalculateUVHPBenefitOptions(busConstant.CodeValueAll, ldecTotalBenefitAmount);

            }
        }

        private void CalculateMPIBenefitOptionsForRegularAndDisability(decimal adecFinalAccruedBenefitAmount, string astrBenefitOptionValue, bool ablnConvertBenOption = false, string astrOriginalBenefitOption = "") //RequestID: 72091
        {
            if (iblnEligibleForRegularBenefit)
            {
                #region Reduced_OR_LateAdj_Amount
                decimal ldecLateAdjustmentAmt = new decimal();

                Collection<busBenefitCalculationDetail> lclbDetailCol = null;
                if (this.icdoBenefitCalculationHeader.istrRetirementType == busConstant.RETIREMENT_TYPE_LATE)
                {
                    lclbDetailCol = this.iclbBenefitCalculationDetail;
                }
                busBenefitCalculationOptions lbusBenefitCalculationOptions = new busBenefitCalculationOptions(); ;
                decimal ldecBenefitOptionFactor = busConstant.ZERO_DECIMAL;
                decimal ldecPopupBenefitOptionFactor = busConstant.ZERO_DECIMAL; //RequestID: 72091

                decimal ldecNetRegularBenefitAmount = new decimal();
                decimal ldecPopupNetRegularBenefitAmount = new decimal(); //RequestID: 72091
                decimal ldecReducedBenefitAmount = decimal.Zero;
                int lintTotalQualifiedYear = this.ibusBenefitApplicationRetirement.aclbPersonWorkHistory_MPI.FirstOrDefault().qualified_years_count;


                ldecReducedBenefitAmount = ibusCalculation.CalculateReducedBenefit(ibusPerson, busConstant.BENEFIT_TYPE_RETIREMENT
                                                , this.icdoBenefitCalculationHeader.idecParticipantFullAge,
                                                this.ibusBenefitApplication.icdoBenefitApplication.retirement_date,
                                                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().icdoBenefitCalculationDetail.vested_date,
                                                ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault(),
                                                ibusBenefitApplication, busConstant.BOOL_FALSE,
                                                this.iclbBenefitCalculationDetail, null, lintTotalQualifiedYear, idecFinalAccruedBenefitAmount,
                                                this.icdoBenefitCalculationHeader.istrRetirementType, true, ibusBenefitApplication.aclbPersonWorkHistory_MPI, this.iclbPersonAccountRetirementContribution,
                                                ref ldecLateAdjustmentAmt, this.icdoBenefitCalculationHeader.benefit_type_value, this.ibusPerson.icdoPerson.person_id);


                ldecReducedBenefitAmount = Math.Round(ldecReducedBenefitAmount, 2);


                if (this.icdoBenefitCalculationHeader.istrRetirementType == busConstant.RETIREMENT_TYPE_LATE)
                    adecFinalAccruedBenefitAmount = ldecLateAdjustmentAmt;
                else
                {
                    decimal ldecQdroOffSet = this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoBenefitCalculationDetail.retirement_date == this.ibusBenefitApplication.icdoBenefitApplication.retirement_date).FirstOrDefault().icdoBenefitCalculationDetail.qdro_offset;
                    adecFinalAccruedBenefitAmount = adecFinalAccruedBenefitAmount - ldecQdroOffSet;
                }

                #endregion


                #region Retro
                // PROD PIR 504
                if (this.iblnCheckIfHoursAfterDisabilityRetirementDate && (this.icdoBenefitCalculationHeader.retirement_date == this.ibusBenefitApplication.icdoBenefitApplication.retirement_date || iblnOptionTwoFlag)) 
                {
                    if (!this.ibusBenefitApplication.aclbReEmployedWorkHistory.IsNullOrEmpty())
                        ibusCalculation.CalculateAccruedBenefitForReEmployedParticipant(this, null, this.icdoBenefitCalculationHeader.payment_date, true);


                    if (this.icdoBenefitCalculationHeader.iintPlanId != busConstant.IAP_PLAN_ID)
                    {
                        if (this.icdoBenefitCalculationHeader.payment_date != DateTime.MinValue)
                        {
                            SetRetroactiveAdjustmentVariables();
                        }
                    }

                }
                #endregion

                #region Lump
                if (astrBenefitOptionValue == busConstant.CodeValueAll || astrBenefitOptionValue == busConstant.LUMP_SUM)
                {

                    ldecBenefitOptionFactor = idecLumpSumFactor;
                    ldecNetRegularBenefitAmount = Convert.ToDecimal(Math.Round(ldecReducedBenefitAmount * ldecBenefitOptionFactor, 2));
                    lbusBenefitCalculationOptions = GetDisabilityBenefitOptions(busConstant.LUMP_SUM, adecFinalAccruedBenefitAmount, ldecNetRegularBenefitAmount, ldecBenefitOptionFactor);

                }
                #endregion

                #region JAndS
                if (iblnIsQualifiedSpouse)
                {
                    if (astrBenefitOptionValue == busConstant.CodeValueAll || astrBenefitOptionValue == busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY)
                    {
                        ldecBenefitOptionFactor = GetMPIBenefitOptionFactor(.86M, .005M, .006M);
                        //PIR-940 
                        if (ldecBenefitOptionFactor > 1)
                        {
                            ldecBenefitOptionFactor = 1;
                        }
                        ldecNetRegularBenefitAmount = Convert.ToDecimal(Math.Round(ldecReducedBenefitAmount * ldecBenefitOptionFactor, 2));
                        //if (this.iblnCheckIfHoursAfterDisabilityRetirementDate)
                        //{
                        //  ldecNetRegularBenefitAmount +=  ibusCalculation.GetRegularFactorsForReemployedParticipants(this,  this.ibusPerson.icdoPerson.idtDateofBirth, this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth, busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY);
                        //}
                        lbusBenefitCalculationOptions = GetDisabilityBenefitOptions(busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY, adecFinalAccruedBenefitAmount, ldecNetRegularBenefitAmount, ldecBenefitOptionFactor);
                    }
                    if (this.ibusBenefitApplication.icdoBenefitApplication.retirement_date > busConstant.BenefitCalculation.DISABILITY_JS75_CHECK_DATE)
                    {
                        if (astrBenefitOptionValue == busConstant.CodeValueAll || astrBenefitOptionValue == busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY)
                        {
                            ldecBenefitOptionFactor = GetMPIBenefitOptionFactor(.80M, .01M, .006M);
                            //PIR-940
                            if(ldecBenefitOptionFactor>1)
                            {
                                ldecBenefitOptionFactor = 1;
                            }
                            ldecNetRegularBenefitAmount = Math.Round(ldecReducedBenefitAmount * ldecBenefitOptionFactor, 2);
                            //if (this.iblnCheckIfHoursAfterDisabilityRetirementDate)
                            //{
                            //    ibusCalculation.GetRegularFactorsForReemployedParticipants(this, this.ibusPerson.icdoPerson.idtDateofBirth, this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth, busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY);
                            //}
                            lbusBenefitCalculationOptions = GetDisabilityBenefitOptions(busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, adecFinalAccruedBenefitAmount, ldecNetRegularBenefitAmount, ldecBenefitOptionFactor);
                        }
                    }
                    if (astrBenefitOptionValue == busConstant.CodeValueAll || astrBenefitOptionValue == busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY)
                    {
                        //Regular
                        //double ldblBenefitOptionFactorJnS100 = 0.75 + 0.01 * Convert.ToInt32(this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement - this.icdoBenefitCalculationHeader.age) +
                        //             0.006 * (65 - Convert.ToInt32(this.icdoBenefitCalculationHeader.age));

                        ldecBenefitOptionFactor = GetMPIBenefitOptionFactor(.75M, .01M, .006M);
                        //PIR-940
                        if (ldecBenefitOptionFactor > 1)
                        {
                            ldecBenefitOptionFactor = 1;
                        }
                        //if (this.iblnCheckIfHoursAfterDisabilityRetirementDate)
                        //{
                        //    ibusCalculation.GetRegularFactorsForReemployedParticipants(this, this.ibusPerson.icdoPerson.idtDateofBirth, this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth, busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY);
                        //}
                        ldecNetRegularBenefitAmount = Math.Round(ldecReducedBenefitAmount * ldecBenefitOptionFactor, 2);

                        //Disabilty
                        lbusBenefitCalculationOptions = GetDisabilityBenefitOptions(busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY, adecFinalAccruedBenefitAmount, ldecNetRegularBenefitAmount, ldecBenefitOptionFactor);
                    }
                    if (astrBenefitOptionValue == busConstant.CodeValueAll || astrBenefitOptionValue == busConstant.JOINT_50_PERCENT_POPUP_ANNUITY)
                    {

                        //double ldblBenefitOptionFactorJnS50Pop = 0.83 + 0.007 * Convert.ToInt32(this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement - this.icdoBenefitCalculationHeader.age) +
                        //             0.006 * (65 - Convert.ToInt32(this.icdoBenefitCalculationHeader.age));

                        ldecBenefitOptionFactor = GetMPIBenefitOptionFactor(.83M, .007M, .006M);
                        //PIR-940
                        if (ldecBenefitOptionFactor > 1)
                        {
                            ldecBenefitOptionFactor = 1;
                        }
                        //if (this.iblnCheckIfHoursAfterDisabilityRetirementDate)
                        //{
                        //    ldecNetRegularBenefitAmount += ibusCalculation.GetRegularFactorsForReemployedParticipants(this, this.ibusPerson.icdoPerson.idtDateofBirth, this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth, busConstant.JOINT_50_PERCENT_POPUP_ANNUITY);
                        //}
                        ldecNetRegularBenefitAmount = Math.Round(ldecReducedBenefitAmount * ldecBenefitOptionFactor, 2);

                        lbusBenefitCalculationOptions = GetDisabilityBenefitOptions(busConstant.JOINT_50_PERCENT_POPUP_ANNUITY, adecFinalAccruedBenefitAmount, ldecNetRegularBenefitAmount, ldecBenefitOptionFactor);


                    }

                    if (astrBenefitOptionValue == busConstant.CodeValueAll || astrBenefitOptionValue == busConstant.JOINT_100_PERCENT_POPUP_ANNUITY)
                    {

                        //double ldblBenefitOptionFactorJnS100Pop = 0.71 + 0.01 * Convert.ToInt32(this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement - this.icdoBenefitCalculationHeader.age) +
                        //             0.008 * (65 - Convert.ToInt32(this.icdoBenefitCalculationHeader.age));

                        ldecBenefitOptionFactor = GetMPIBenefitOptionFactor(.71M, .01M, .008M);
                        //PIR-940
                        if(ldecBenefitOptionFactor>1)
                        {
                            ldecBenefitOptionFactor = 1;
                        }
                        //if (this.iblnCheckIfHoursAfterDisabilityRetirementDate)
                        //{
                        //    ldecNetRegularBenefitAmount += ibusCalculation.GetRegularFactorsForReemployedParticipants(this, this.ibusPerson.icdoPerson.idtDateofBirth, this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth, busConstant.JOINT_100_PERCENT_POPUP_ANNUITY);
                        //}
                        ldecNetRegularBenefitAmount = Convert.ToDecimal(Math.Round(ldecReducedBenefitAmount * ldecBenefitOptionFactor, 2));

                        lbusBenefitCalculationOptions = GetDisabilityBenefitOptions(busConstant.JOINT_100_PERCENT_POPUP_ANNUITY, adecFinalAccruedBenefitAmount, ldecNetRegularBenefitAmount, ldecBenefitOptionFactor);
                    }
                }

                #endregion

                #region TEN
                if (astrBenefitOptionValue == busConstant.CodeValueAll || astrBenefitOptionValue == busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY)
                {

                    int lPlanBenefitId = ibusCalculation.GetPlanBenefitId(busConstant.MPIPP_PLAN_ID, busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY);
                    ldecBenefitOptionFactor = ibusCalculation.GetBenefitOptionFactor(busConstant.BENEFIT_TYPE_RETIREMENT, lPlanBenefitId,
                                                      Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge), 0);
                    ldecBenefitOptionFactor = Math.Round(ldecBenefitOptionFactor, 3);
                    //pir-940
                    if (ldecBenefitOptionFactor > 1)
                    {
                        ldecBenefitOptionFactor = 1;
                    }
                    ldecNetRegularBenefitAmount = Convert.ToDecimal(Math.Round(ldecReducedBenefitAmount * ldecBenefitOptionFactor, 2));
                    //if (this.iblnCheckIfHoursAfterDisabilityRetirementDate)
                    //{
                    //    ldecNetRegularBenefitAmount += ibusCalculation.GetRegularFactorsForReemployedParticipants(this, this.ibusPerson.icdoPerson.idtDateofBirth, this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth, busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY);
                    //}

                    lbusBenefitCalculationOptions = GetDisabilityBenefitOptions(busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, adecFinalAccruedBenefitAmount, ldecNetRegularBenefitAmount, ldecBenefitOptionFactor);

                }
                #endregion

                #region LIFE
                if (astrBenefitOptionValue == busConstant.CodeValueAll || astrBenefitOptionValue == busConstant.LIFE_ANNUTIY)
                {
                    ldecBenefitOptionFactor = 1;
                    ldecNetRegularBenefitAmount = Convert.ToDecimal(Math.Round(ldecReducedBenefitAmount * ldecBenefitOptionFactor, 2));
                    
                    //RequestID: 72091
                    if(astrOriginalBenefitOption == busConstant.JOINT_100_PERCENT_POPUP_ANNUITY)
                    {
                        ldecPopupBenefitOptionFactor = GetMPIBenefitOptionFactor(.71M, .01M, .008M);

                        if (ldecPopupBenefitOptionFactor > 1)
                        {
                            ldecPopupBenefitOptionFactor = 1;
                        }

                        ldecPopupNetRegularBenefitAmount = Convert.ToDecimal(Math.Round(ldecReducedBenefitAmount * ldecPopupBenefitOptionFactor, 2));
                    }

                    if(astrOriginalBenefitOption == busConstant.JOINT_50_PERCENT_POPUP_ANNUITY)
                    {
                        ldecPopupBenefitOptionFactor = GetMPIBenefitOptionFactor(.83M, .007M, .006M);
                        
                        if (ldecPopupBenefitOptionFactor > 1)
                        {
                            ldecPopupBenefitOptionFactor = 1;
                        }

                        ldecPopupNetRegularBenefitAmount = Convert.ToDecimal(Math.Round(ldecReducedBenefitAmount * ldecPopupBenefitOptionFactor, 2));
                    }
                    
                    //if (this.iblnCheckIfHoursAfterDisabilityRetirementDate)
                    //{
                    //    ldecNetRegularBenefitAmount += ibusCalculation.GetRegularFactorsForReemployedParticipants(this, this.ibusPerson.icdoPerson.idtDateofBirth, this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth, busConstant.LIFE_ANNUTIY);
                    //}
                    lbusBenefitCalculationOptions = GetDisabilityBenefitOptions(busConstant.LIFE_ANNUTIY, adecFinalAccruedBenefitAmount, ldecNetRegularBenefitAmount, ldecBenefitOptionFactor, ablnConvertBenOption,astrOriginalBenefitOption,adecPopupRegularFactor: ldecPopupBenefitOptionFactor, adecPopupRegularBenefit: ldecPopupNetRegularBenefitAmount); //RequestID: 72091

                }
                #endregion

            }

        }

        private void CalculateMPIBenefitOptionsForDiabledPerson(decimal adecFinalAccruedBenefitAmount, string astrBenefitOptionValue, bool ablnConvertBenOption = false, string astrOriginalBenefitOption = "") //RequestID: 72091
        {
            this.ibusCalculation.ProcessQDROOffset(this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoBenefitCalculationDetail.retirement_date == this.icdoBenefitCalculationHeader.retirement_date).First(), this.ibusPerson.icdoPerson.person_id, ref adecFinalAccruedBenefitAmount);
            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoBenefitCalculationDetail.retirement_date == this.icdoBenefitCalculationHeader.retirement_date)
                            .First().icdoBenefitCalculationDetail.idecBenefitsAfterQDROOffSet = adecFinalAccruedBenefitAmount;

            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoBenefitCalculationDetail.retirement_date == this.icdoBenefitCalculationHeader.retirement_date).FirstOrDefault().icdoBenefitCalculationDetail.early_reduced_benefit_amount = adecFinalAccruedBenefitAmount;

            if ((this.iblnCheckIfHoursAfterDisabilityRetirementDate && !iblnEligibleForRegularBenefit
                && this.icdoBenefitCalculationHeader.retirement_date == this.ibusBenefitApplication.icdoBenefitApplication.retirement_date) || iblnOptionTwoFlag) // PROD PIR 504
                //As Retroactive already calculated in case of regular benefit
            {
                if (!this.ibusBenefitApplication.aclbReEmployedWorkHistory.IsNullOrEmpty() && !iblnOptionTwoFlag) // PROD PIR 504
                    ibusCalculation.CalculateAccruedBenefitForReEmployedParticipant(this, null, this.icdoBenefitCalculationHeader.payment_date, true);

                if (this.icdoBenefitCalculationHeader.iintPlanId != busConstant.IAP_PLAN_ID)
                {
                    if (this.icdoBenefitCalculationHeader.payment_date != DateTime.MinValue &&
                (this.icdoBenefitCalculationHeader.retirement_date == this.ibusBenefitApplication.icdoBenefitApplication.retirement_date || iblnOptionTwoFlag))
                    {
                        SetRetroactiveAdjustmentVariables();
                    }
                }
            }

            busBenefitCalculationOptions lbusBenefitCalculationOptions = new busBenefitCalculationOptions();
            decimal ldecTotalLocalLumpsumAmount = ibusCalculation.GetLocalLumpsumBenefitAmount(icdoBenefitCalculationHeader.idecParticipantFullAge, ibusBenefitApplication,
                                                  ibusPerson, icdoBenefitCalculationHeader.retirement_date, iclbPersonAccountRetirementContribution);
            if (astrBenefitOptionValue == busConstant.CodeValueAll || astrBenefitOptionValue == busConstant.LUMP_SUM)
            {
                lbusBenefitCalculationOptions = this.GetDisabilityBenefitOptions(busConstant.LUMP_SUM, adecFinalAccruedBenefitAmount, 0, 0);
                //this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
            }
            if (iblnIsQualifiedSpouse)
            {
                if (astrBenefitOptionValue == busConstant.CodeValueAll || astrBenefitOptionValue == busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY)
                {
                    lbusBenefitCalculationOptions = this.GetDisabilityBenefitOptions(busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY, adecFinalAccruedBenefitAmount, 0, 0);
                    //this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
                }
                if (this.ibusBenefitApplication.icdoBenefitApplication.retirement_date > busConstant.BenefitCalculation.DISABILITY_JS75_CHECK_DATE)
                {
                    if (astrBenefitOptionValue == busConstant.CodeValueAll || astrBenefitOptionValue == busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY)
                    {
                        lbusBenefitCalculationOptions = this.GetDisabilityBenefitOptions(busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, adecFinalAccruedBenefitAmount, 0, 0);
                        //this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
                    }
                }
                if (astrBenefitOptionValue == busConstant.CodeValueAll || astrBenefitOptionValue == busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY)
                {
                    lbusBenefitCalculationOptions = this.GetDisabilityBenefitOptions(busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY, adecFinalAccruedBenefitAmount, 0, 0);
                    // this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
                }
                if (astrBenefitOptionValue == busConstant.CodeValueAll || astrBenefitOptionValue == busConstant.JOINT_50_PERCENT_POPUP_ANNUITY)
                {
                    lbusBenefitCalculationOptions = this.GetDisabilityBenefitOptions(busConstant.JOINT_50_PERCENT_POPUP_ANNUITY, adecFinalAccruedBenefitAmount, 0, 0);
                    //this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
                }
                if (astrBenefitOptionValue == busConstant.CodeValueAll || astrBenefitOptionValue == busConstant.JOINT_100_PERCENT_POPUP_ANNUITY)
                {
                    lbusBenefitCalculationOptions = this.GetDisabilityBenefitOptions(busConstant.JOINT_100_PERCENT_POPUP_ANNUITY, adecFinalAccruedBenefitAmount, 0, 0);
                    //this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
                }
            }
            if (astrBenefitOptionValue == busConstant.CodeValueAll || astrBenefitOptionValue == busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY)
            {
                lbusBenefitCalculationOptions = this.GetDisabilityBenefitOptions(busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, adecFinalAccruedBenefitAmount, 0, 0);
                // this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
            }
            if (astrBenefitOptionValue == busConstant.CodeValueAll || astrBenefitOptionValue == busConstant.LIFE_ANNUTIY)
            {
                //RequestID: 72091
                lbusBenefitCalculationOptions = this.GetDisabilityBenefitOptions(busConstant.LIFE_ANNUTIY, adecFinalAccruedBenefitAmount, 0, 0,ablnConvertBenOption,astrOriginalBenefitOption);
                //this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
            }

            //#region Calculate Retiree Increase

            //if (icdoBenefitCalculationHeader.retirement_date < DateTime.Now)
            //{
            //    CalculateDisabilityRetireeIncAmount(lbusBenefitCalculationOptions);
            //}

            //#endregion
        }

        public void GetBenefitAmountForEachYearAfterRetirement(busBenefitCalculationHeader abusBenefitCalculationHeader, string astrBenefitOptionValue, DateTime adtBatchRunDate, DateTime adtParticipantDOB, DateTime adtSpouseDOB, DateTime adtBenefitEffectiveDate, bool ablnStartFromRetirement, decimal adecReferenceAccruedBenefit, decimal adecPrevBenefitAmount = decimal.Zero)
        {
            if (abusBenefitCalculationHeader.iclbBenefitCalculationDetail.FirstOrDefault().iclbBenefitCalculationYearlyDetail.IsNullOrEmpty())
                return;

            Collection<busBenefitCalculationYearlyDetail> lclbReemployedYearlyDetail = new Collection<busBenefitCalculationYearlyDetail>();

            decimal ldecBenefitOptionFactor = decimal.Zero;
            decimal ldecSurvivorAmount = decimal.Zero;
            decimal ldecFactor = decimal.Zero;
            decimal ldecBenefitAmount = adecReferenceAccruedBenefit;
            int lintParticipantAgeAtRetirement = Convert.ToInt32(busGlobalFunctions.CalculatePersonAge(adtParticipantDOB, abusBenefitCalculationHeader.icdoBenefitCalculationHeader.retirement_date));
            decimal ldecERF = 1;

            decimal ldecDismAmtAtRetr = decimal.Zero;
            decimal ldecRegAmtAtRerr = decimal.Zero;
            decimal ldecDisbFactor = decimal.Zero;
            decimal ldecRegularFactor = decimal.Zero;
            bool lblnDisabilityFactors = true;
            int lintPlanBenID = ibusCalculation.GetPlanBenefitId(busConstant.MPIPP_PLAN_ID, astrBenefitOptionValue);
            if (!abusBenefitCalculationHeader.iclbBenefitCalculationDetail.IsNullOrEmpty() && !abusBenefitCalculationHeader.iclbBenefitCalculationDetail.FirstOrDefault().iclbBenefitCalculationOptions.IsNullOrEmpty())
            {
                busBenefitCalculationOptions lbusBenefitCalculationOptions = abusBenefitCalculationHeader.iclbBenefitCalculationDetail.FirstOrDefault().iclbBenefitCalculationOptions.FirstOrDefault();
                ldecDismAmtAtRetr = lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.disability_amount;
                ldecRegAmtAtRerr = lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.benefit_amount;
                ldecDisbFactor = lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.disability_factor;
                ldecRegularFactor = lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.benefit_option_factor;
                if (ldecDismAmtAtRetr > ldecRegAmtAtRerr)
                {
                    ldecBenefitAmount = ldecDismAmtAtRetr;
                }
                else
                {
                    lblnDisabilityFactors = false;
                    ldecBenefitAmount = ldecRegAmtAtRerr;
                }
            }

            if (ablnStartFromRetirement && lintParticipantAgeAtRetirement < 65 && (abusBenefitCalculationHeader.iclbBenefitCalculationDetail.FirstOrDefault().icdoBenefitCalculationDetail.benefit_subtype_value == busConstant.RETIREMENT_TYPE_REDUCED_EARLY
                || abusBenefitCalculationHeader.iclbBenefitCalculationDetail.FirstOrDefault().icdoBenefitCalculationDetail.benefit_subtype_value == busConstant.RETIREMENT_TYPE_SPL_REDUCED_EARLY))
            {
                //If Calculation exists we will direct get early reduced benefit amount.
                if (abusBenefitCalculationHeader.iclbBenefitCalculationDetail.FirstOrDefault().icdoBenefitCalculationDetail.early_reduced_benefit_amount == decimal.Zero)
                {
                    ldecERF = ibusCalculation.GetEarlyReductionFactor(busConstant.MPIPP_PLAN_ID, busConstant.BENEFIT_TYPE_RETIREMENT, abusBenefitCalculationHeader.iclbBenefitCalculationDetail.FirstOrDefault().icdoBenefitCalculationDetail.benefit_subtype_value, lintParticipantAgeAtRetirement);
                    ldecBenefitAmount = Math.Round(ldecBenefitAmount * ldecERF, 2);
                    abusBenefitCalculationHeader.iclbBenefitCalculationDetail.FirstOrDefault().icdoBenefitCalculationDetail.early_reduction_factor = ldecERF;
                    abusBenefitCalculationHeader.iclbBenefitCalculationDetail.FirstOrDefault().icdoBenefitCalculationDetail.early_reduced_benefit_amount = ldecBenefitAmount;
                }
                else
                {
                    ldecBenefitAmount = abusBenefitCalculationHeader.iclbBenefitCalculationDetail.FirstOrDefault().icdoBenefitCalculationDetail.early_reduced_benefit_amount;
                }
            }
            if (ablnStartFromRetirement)
            {
                int lintSpouseAge = 0;
                lintSpouseAge = Convert.ToInt32(busGlobalFunctions.CalculatePersonAge(adtSpouseDOB, abusBenefitCalculationHeader.icdoBenefitCalculationHeader.retirement_date));
                ldecFactor = ibusCalculation.GetFactor(this, astrBenefitOptionValue, lintSpouseAge, lintParticipantAgeAtRetirement);
                ldecBenefitAmount = Math.Round(ldecBenefitAmount * ldecFactor, 2);
                abusBenefitCalculationHeader.iclbBenefitCalculationDetail.FirstOrDefault().iclbBenefitCalculationYearlyDetail.OrderByDescending(item => item.icdoBenefitCalculationYearlyDetail.plan_year).Where(
                                    item => !(item.icdoBenefitCalculationYearlyDetail.reemployed_flag == busConstant.FLAG_YES)).FirstOrDefault().icdoBenefitCalculationYearlyDetail.benefit_option_factor = ldecFactor;
                lclbReemployedYearlyDetail = abusBenefitCalculationHeader.iclbBenefitCalculationDetail.FirstOrDefault().iclbBenefitCalculationYearlyDetail.Where(item => item.icdoBenefitCalculationYearlyDetail.reemployed_flag == busConstant.FLAG_YES).ToList().ToCollection();
            }
            else
            {

                ldecBenefitAmount = adecPrevBenefitAmount;
                lclbReemployedYearlyDetail = abusBenefitCalculationHeader.iclbBenefitCalculationDetail.FirstOrDefault().iclbBenefitCalculationYearlyDetail.Where(item => item.icdoBenefitCalculationYearlyDetail.reemployed_flag == busConstant.FLAG_YES && Convert.ToInt32(item.icdoBenefitCalculationYearlyDetail.plan_year) == adtBenefitEffectiveDate.AddYears(-1).Year).ToList().ToCollection();
            }
            if (!lclbReemployedYearlyDetail.IsNullOrEmpty())
            {
                int lintParticipantAge = 0;
                int lintSpouseAge = 0;
                DateTime adtAgeAtDate = new DateTime();
                foreach (busBenefitCalculationYearlyDetail lbusbenefitCalculationYearly in lclbReemployedYearlyDetail)
                {
                    lintParticipantAge = 0;
                    lintSpouseAge = 0;
                    adtAgeAtDate = new DateTime(Convert.ToInt32(lbusbenefitCalculationYearly.icdoBenefitCalculationYearlyDetail.plan_year), 01, 01).AddYears(1);
                    lintParticipantAge = Convert.ToInt32(busGlobalFunctions.CalculatePersonAge(adtParticipantDOB, adtAgeAtDate));
                    lintSpouseAge = Convert.ToInt32(busGlobalFunctions.CalculatePersonAge(adtSpouseDOB, adtAgeAtDate));
                    if (lblnDisabilityFactors)
                    {
                        ldecFactor = ibusCalculation.GetDisabilityFactor(abusBenefitCalculationHeader, astrBenefitOptionValue, lintParticipantAge, lintSpouseAge, lintPlanBenID);
                    }
                    else
                    {
                        ldecFactor = ibusCalculation.GetFactor(abusBenefitCalculationHeader, astrBenefitOptionValue, lintSpouseAge, lintParticipantAge);
                    }
                    ldecBenefitAmount += Math.Round(lbusbenefitCalculationYearly.icdoBenefitCalculationYearlyDetail.accrued_benefit_amount * ldecFactor, 3);
                    lbusbenefitCalculationYearly.icdoBenefitCalculationYearlyDetail.benefit_option_factor = ldecFactor;

                }
            }

            ldecSurvivorAmount = ibusCalculation.GetSurvivorAmountFromBenefitAmount(ldecBenefitAmount, astrBenefitOptionValue);
            busBenefitCalculationOptions lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

            lbusBenefitCalculationOption.LoadDisabilityData(lintPlanBenID, ldecRegularFactor, ldecRegAmtAtRerr, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER, this.icdoBenefitCalculationHeader.person_id,
abusBenefitCalculationHeader.icdoBenefitCalculationHeader.beneficiary_person_id, astrBenefitOptionValue, ldecBenefitAmount, ldecSurvivorAmount, ldecDisbFactor, ldecDismAmtAtRetr, decimal.Zero, ldecERF);


            abusBenefitCalculationHeader.iclbBenefitCalculationDetail.FirstOrDefault().iclbBenefitCalculationOptions.Clear();

            abusBenefitCalculationHeader.iclbBenefitCalculationDetail.FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);

        }


        public busBenefitCalculationOptions GetDisabilityBenefitOptions(string astrBenefitOption, decimal adecAccuredBenefit, decimal adecRegularBenefit, decimal adecRegularFactor, bool ablnConvertBenOption = false, string astrOriginalBenefitOption = "",decimal adecPopupRegularFactor = 0, decimal adecPopupRegularBenefit = 0) //RequestID: 72091
        {
            bool lblnReductionFactorForRetroactive = false;
            decimal ldecBenOptionFactorForRetroactive = decimal.Zero;
            if (astrBenefitOption == busConstant.LUMP_SUM)
            {
                if (this.icdoBenefitCalculationHeader.iintPlanId == busConstant.MPIPP_PLAN_ID)
                {
                    if (adecAccuredBenefit + idecLocalFrozenBenefit > 10000)
                        return null;
                }
                else if (this.icdoBenefitCalculationHeader.iintPlanId != busConstant.MPIPP_PLAN_ID &&
                    this.icdoBenefitCalculationHeader.iintPlanId != busConstant.IAP_PLAN_ID)
                {
                    if (adecAccuredBenefit + idecFinalAccruedBenefitAmount > 10000)
                        return null;
                }
            }
            decimal ldecBenefitOptionFactor = busConstant.ZERO_DECIMAL;//PIR-563
            decimal ldecFinalDisBenefitOptionFactor = decimal.Zero;//PIR-563
            decimal ldecFinalDisBenefitAmount = decimal.Zero;//PIR-563
            decimal ldecDisBenefitOptionFactor = decimal.Zero;
            decimal ldecRetroActiveAmount = decimal.Zero;
            decimal ldecDisBenefitAmount = decimal.Zero;
            decimal ldecParticipantAmount = new decimal();
            decimal ldecSurvivorAmount = new decimal();
            //RequestID: 72091
            decimal ldecPopupBenefitOptionFactor = busConstant.ZERO_DECIMAL;
            decimal ldecDisPopupBenefitOptionFactor = decimal.Zero;
            decimal ldecDisPopupBenefitAmount = decimal.Zero;
            decimal ldecFinalPopupDisBenefitOptionFactor = decimal.Zero;
            decimal ldecFinalPopupDisBenefitAmount = decimal.Zero;
            decimal ldecParticipantPopupAmount = new decimal();
            decimal ldecPopupBenOptionFactorForRetroactive = decimal.Zero;
            


            decimal ldecERF = this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId && item.icdoBenefitCalculationDetail.retirement_date == this.ibusBenefitApplication.icdoBenefitApplication.retirement_date).FirstOrDefault().icdoBenefitCalculationDetail.early_reduction_factor;
            int lintPlanBenefitID = ibusCalculation.GetPlanBenefitId(this.icdoBenefitCalculationHeader.iintPlanId, astrBenefitOption);
            //RequestID: 72091
            int lintPopupPlanBenefitID = ibusCalculation.GetPlanBenefitId(this.icdoBenefitCalculationHeader.iintPlanId, astrOriginalBenefitOption);

            int lintMPIPlanBenefitIDForFactors = ibusCalculation.GetPlanBenefitId(busConstant.MPIPP_PLAN_ID, astrBenefitOption);
            int lintMPIPopupPlanBenefitIDForFactors = ibusCalculation.GetPlanBenefitId(busConstant.MPIPP_PLAN_ID, astrOriginalBenefitOption); //RequestID: 72091
            bool lblnGetFactorsFromDisb = true;

            //Gets Calculated when we calculate Regular Benefit : Needs to be shown in Disability Amount and Factors.
            #region Set Disability Factors
            //RID 89976 Added benefit type disability check.
            if (this.icdoBenefitCalculationHeader.istrRetirementType == busConstant.RETIREMENT_TYPE_LATE && this.icdoBenefitCalculationHeader.benefit_type_value != busConstant.BENEFIT_TYPE_DISABILITY)
            {
                lblnGetFactorsFromDisb = false;
                ldecDisBenefitAmount = adecRegularBenefit;
                ldecDisBenefitOptionFactor = adecRegularFactor;
                adecRegularFactor = 0;
                adecRegularBenefit = 0;
                ldecFinalDisBenefitOptionFactor = adecRegularFactor;
                ldecFinalDisBenefitAmount = adecRegularBenefit;
                ldecDisPopupBenefitAmount = adecPopupRegularBenefit;
                ldecDisPopupBenefitOptionFactor = adecPopupRegularFactor;
            }
            else
            {
                if (astrBenefitOption == busConstant.LUMP_SUM)
                {
                    ldecDisBenefitOptionFactor = idecLumpSumFactor;
                }
                else if (astrBenefitOption == busConstant.LIFE_ANNUTIY || astrBenefitOption == busConstant.FIVE_YEARS_CERTAIN_AND_LIFE_ANNUTIY ||
                    astrBenefitOption == busConstant.THREE_YEARS_CERTAIN_AND_LIFE_ANNUITY || astrBenefitOption == busConstant.TWO_YEARS_CERTAIN_AND_LIFE_ANNUITY)
                {
                    ldecDisBenefitOptionFactor = 1;

                    //RequestID: 72091
                    if(astrBenefitOption == busConstant.LIFE_ANNUTIY && (astrOriginalBenefitOption == busConstant.JOINT_50_PERCENT_POPUP_ANNUITY || astrOriginalBenefitOption == busConstant.JOINT_75_PERCENT_POPUP_ANNUITY || astrOriginalBenefitOption == busConstant.JOINT_100_PERCENT_POPUP_ANNUITY))
                    {
                        if (astrOriginalBenefitOption == busConstant.JOINT_50_PERCENT_POPUP_ANNUITY || astrOriginalBenefitOption == busConstant.JOINT_75_PERCENT_POPUP_ANNUITY || astrOriginalBenefitOption == busConstant.JOINT_100_PERCENT_POPUP_ANNUITY)
                            ldecDisPopupBenefitOptionFactor = ibusCalculation.GetBenefitOptionFactor(this.icdoBenefitCalculationHeader.benefit_type_value, lintMPIPopupPlanBenefitIDForFactors, Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge), Convert.ToInt32(this.icdoBenefitCalculationHeader.idecSurvivorFullAge));

                    }
                }
                else if (astrBenefitOption == busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY)
                {
                    ldecDisBenefitOptionFactor = ibusCalculation.GetBenefitOptionFactor(this.icdoBenefitCalculationHeader.benefit_type_value, lintMPIPlanBenefitIDForFactors, Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge), 0);
                    
                }
                else
                {
                    ldecDisBenefitOptionFactor = ibusCalculation.GetBenefitOptionFactor(this.icdoBenefitCalculationHeader.benefit_type_value, lintMPIPlanBenefitIDForFactors, Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge), Convert.ToInt32(this.icdoBenefitCalculationHeader.idecSurvivorFullAge));
                }
                ldecDisBenefitOptionFactor = Math.Round(ldecDisBenefitOptionFactor, 3);
                ldecDisBenefitAmount = ldecDisBenefitOptionFactor * adecAccuredBenefit;
                ldecDisBenefitAmount = Math.Round(ldecDisBenefitAmount, 2);

                //RequestID: 72091
                ldecDisPopupBenefitOptionFactor = Math.Round(ldecDisPopupBenefitOptionFactor, 3);
                ldecDisPopupBenefitAmount = ldecDisPopupBenefitOptionFactor * adecAccuredBenefit;
                ldecDisPopupBenefitAmount = Math.Round(ldecDisPopupBenefitAmount, 2);

                //PIR- 563 (Benefit Factor and Disability factor, whichever max should be applied for Early)
                if (this.icdoBenefitCalculationHeader.istrRetirementType == busConstant.RETIREMENT_TYPE_UNREDUCED_EARLY || this.icdoBenefitCalculationHeader.istrRetirementType == busConstant.RETIREMENT_TYPE_REDUCED_EARLY || this.icdoBenefitCalculationHeader.istrRetirementType == busConstant.RETIREMENT_TYPE_SPL_REDUCED_EARLY)
                {
                    if (astrBenefitOption == busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY)
                    {
                        ldecBenefitOptionFactor = GetMPIBenefitOptionFactor(.86M, .005M, .006M);
                        if (ldecBenefitOptionFactor > ldecDisBenefitOptionFactor)
                        {
                            ldecFinalDisBenefitOptionFactor = ldecBenefitOptionFactor;
                        }
                        else
                        {
                            ldecFinalDisBenefitOptionFactor = ldecDisBenefitOptionFactor;
                        }
                    }
                    else if (astrBenefitOption == busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY)
                    {
                        ldecBenefitOptionFactor = GetMPIBenefitOptionFactor(.80M, .01M, .006M);
                        if (ldecBenefitOptionFactor > ldecDisBenefitOptionFactor)
                        {
                            ldecFinalDisBenefitOptionFactor = ldecBenefitOptionFactor;
                        }
                        else
                        {
                            ldecFinalDisBenefitOptionFactor = ldecDisBenefitOptionFactor;
                        }
                    }
                    else if (astrBenefitOption == busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY)
                    {
                        ldecBenefitOptionFactor = GetMPIBenefitOptionFactor(.75M, .01M, .006M);
                        if (ldecBenefitOptionFactor > ldecDisBenefitOptionFactor)
                        {
                            ldecFinalDisBenefitOptionFactor = ldecBenefitOptionFactor;
                        }
                        else
                        {
                            ldecFinalDisBenefitOptionFactor = ldecDisBenefitOptionFactor;
                        }
                    }
                    else if (astrBenefitOption == busConstant.JOINT_50_PERCENT_POPUP_ANNUITY)
                    {
                        ldecBenefitOptionFactor = GetMPIBenefitOptionFactor(.83M, .007M, .006M);
                        if (ldecBenefitOptionFactor > ldecDisBenefitOptionFactor)
                        {
                            ldecFinalDisBenefitOptionFactor = ldecBenefitOptionFactor;
                        }
                        else
                        {
                            ldecFinalDisBenefitOptionFactor = ldecDisBenefitOptionFactor;                            
                        }
                    }
                    else
                    {
                        ldecFinalDisBenefitOptionFactor = ldecDisBenefitOptionFactor;
                        ldecFinalPopupDisBenefitOptionFactor = ldecDisPopupBenefitOptionFactor; //RequestID: 72091                        
                    }

                    ldecFinalDisBenefitOptionFactor = Math.Round(ldecFinalDisBenefitOptionFactor, 3);
                    ldecFinalDisBenefitAmount = ldecFinalDisBenefitOptionFactor * adecAccuredBenefit;
                    ldecFinalDisBenefitAmount = Math.Round(ldecDisBenefitAmount, 2);//RequestID: 72091

                    //RequestID: 72091
                    ldecFinalPopupDisBenefitOptionFactor = Math.Round(ldecFinalPopupDisBenefitOptionFactor, 3);
                    ldecFinalPopupDisBenefitAmount = ldecFinalPopupDisBenefitOptionFactor * adecAccuredBenefit;
                    ldecFinalPopupDisBenefitAmount = Math.Round(ldecFinalPopupDisBenefitAmount, 2);

                }
                else
                {
                    ldecFinalDisBenefitOptionFactor = Math.Round(ldecDisBenefitOptionFactor, 3);
                    ldecFinalDisBenefitAmount = ldecDisBenefitOptionFactor * adecAccuredBenefit;
                    ldecFinalDisBenefitAmount = Math.Round(ldecDisBenefitAmount, 2);//RequestID: 72091

                    //RequestID: 72091
                    ldecFinalPopupDisBenefitOptionFactor = Math.Round(ldecDisPopupBenefitOptionFactor, 3);
                    ldecFinalPopupDisBenefitAmount = ldecFinalPopupDisBenefitOptionFactor * adecAccuredBenefit;
                    ldecFinalPopupDisBenefitAmount = Math.Round(ldecFinalPopupDisBenefitAmount, 2);
                }

            }
            ldecParticipantAmount = ldecFinalDisBenefitAmount;
            ldecBenOptionFactorForRetroactive = ldecFinalDisBenefitOptionFactor;

            //RequestID: 72091
            ldecParticipantPopupAmount = ldecFinalPopupDisBenefitAmount;
            ldecPopupBenOptionFactorForRetroactive = ldecFinalPopupDisBenefitOptionFactor;
            #endregion

            #region RegularFactors
            if (ldecDisBenefitAmount < adecRegularBenefit)
            {
                lblnGetFactorsFromDisb = false;
                ldecParticipantAmount = adecRegularBenefit;
                lblnReductionFactorForRetroactive = true;
                ldecBenOptionFactorForRetroactive = adecRegularFactor;
            }

            //RequestID: 72091
            if(ldecDisPopupBenefitAmount < adecPopupRegularBenefit)
            {
                lblnGetFactorsFromDisb = false;
                ldecParticipantPopupAmount = adecPopupRegularBenefit;
                lblnReductionFactorForRetroactive = true;
                ldecPopupBenOptionFactorForRetroactive = adecPopupRegularFactor;
            }
            #endregion

            #region Set Participant Final Amount
            if (this.iintParticipantAgeAsofPaymentDate >= 65)
            {
                ldecParticipantAmount = ibusCalculation.GetRegularOrDisabiltyFactorsForReemployedParticipants(this, this.ibusPerson.icdoPerson.idtDateofBirth,
                                         this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth, astrBenefitOption, ldecParticipantAmount, ldecBenOptionFactorForRetroactive, lblnGetFactorsFromDisb, lintPlanBenefitID);

                //RequestID: 72091
                ldecParticipantPopupAmount = ibusCalculation.GetRegularOrDisabiltyFactorsForReemployedParticipants(this, this.ibusPerson.icdoPerson.idtDateofBirth,
                                         this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth, astrOriginalBenefitOption, ldecParticipantPopupAmount, ldecPopupBenOptionFactorForRetroactive, lblnGetFactorsFromDisb, lintPopupPlanBenefitID);

            }
            if (lblnGetFactorsFromDisb)
            {
                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId && item.icdoBenefitCalculationDetail.retirement_date == this.ibusBenefitApplication.icdoBenefitApplication.retirement_date).FirstOrDefault().icdoBenefitCalculationDetail.ee_derived_benefit_amount =
                 Math.Round(idecEEDerivedComponent * ldecDisBenefitOptionFactor, 2);
            }
            else
            {
                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId && item.icdoBenefitCalculationDetail.retirement_date == this.ibusBenefitApplication.icdoBenefitApplication.retirement_date).FirstOrDefault().icdoBenefitCalculationDetail.ee_derived_benefit_amount =
              Math.Round(Math.Round(idecEEDerivedComponent * this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId && item.icdoBenefitCalculationDetail.retirement_date == this.ibusBenefitApplication.icdoBenefitApplication.retirement_date).FirstOrDefault().icdoBenefitCalculationDetail.early_reduction_factor, 2) * adecRegularFactor, 2);
            }
            #endregion

            #region SurvivorAmount
            if (astrBenefitOption == busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY || astrBenefitOption == busConstant.JOINT_50_PERCENT_POPUP_ANNUITY ||
              astrBenefitOption == busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY || astrBenefitOption == busConstant.JOINT_100_PERCENT_POPUP_ANNUITY ||
              astrBenefitOption == busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY || astrBenefitOption == busConstant.JOINT_66_2by3_PERCENT_SURVIVOR_ANNUITY)
            {
                if (astrBenefitOption == busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY || astrBenefitOption == busConstant.JOINT_50_PERCENT_POPUP_ANNUITY)
                {
                    ldecSurvivorAmount = Math.Round(ldecParticipantAmount * .50m, 2);
                }
                else if (astrBenefitOption == busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY)
                {
                    ldecSurvivorAmount = Math.Round(ldecParticipantAmount * .75m, 2);
                }
                else
                {
                    ldecSurvivorAmount = Math.Round(ldecParticipantAmount, 2);
                }
            }
            #endregion

            #region RetroAmount Or PV
            if (astrBenefitOption != busConstant.LUMP_SUM)
            {
                if (iblnCheckIfHoursAfterDisabilityRetirementDate && this.icdoBenefitCalculationHeader.payment_date != DateTime.MinValue)
                {
                    if (this.icdoBenefitCalculationHeader.retirement_date == this.ibusBenefitApplication.icdoBenefitApplication.retirement_date
                        || this.icdoBenefitCalculationHeader.retirement_date_option_2 == this.ibusBenefitApplication.icdoBenefitApplication.retirement_date) // PROD PIR 504
                    {

                        ldecRetroActiveAmount = GetRetroactiveBenefitAmount(ldecParticipantAmount, ldecBenOptionFactorForRetroactive, lblnReductionFactorForRetroactive);
                    }
                }
            }
            else
            {
                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId && item.icdoBenefitCalculationDetail.retirement_date == this.ibusBenefitApplication.icdoBenefitApplication.retirement_date).FirstOrDefault().icdoBenefitCalculationDetail.present_value_amount = ldecParticipantAmount;
            }
            #endregion

            #region Fill Collections
            if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL ||
                this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT)
            {
                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().icdoBenefitCalculationDetail.elected_benefit_amount = ldecParticipantAmount;
            }
            busBenefitCalculationOptions lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
           lbusBenefitCalculationOptions.LoadDisabilityData(lintPlanBenefitID, adecRegularFactor, adecRegularBenefit, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER, this.icdoBenefitCalculationHeader.person_id,
            this.icdoBenefitCalculationHeader.beneficiary_person_id, astrBenefitOption, ldecParticipantAmount, ldecSurvivorAmount, ldecDisBenefitOptionFactor, ldecDisBenefitAmount, ldecRetroActiveAmount, ldecERF,adecDisabilityPopupFactor: (ldecDisPopupBenefitOptionFactor > adecPopupRegularFactor) ? ldecDisPopupBenefitOptionFactor: adecPopupRegularFactor,adecDisabiltyPopupBenefit: ldecParticipantPopupAmount, adecDisabilityPopupFactoratRet: (ldecDisPopupBenefitOptionFactor > adecPopupRegularFactor) ? ldecDisPopupBenefitOptionFactor : adecPopupRegularFactor); //RequestID: 72091
            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId && item.icdoBenefitCalculationDetail.retirement_date == this.ibusBenefitApplication.icdoBenefitApplication.retirement_date).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
            #endregion

            return lbusBenefitCalculationOptions;


        }

        public void CalculateMEA()
        {
            DataTable ldtbEEContributions = new DataTable();
            //Fill EE Contributions
            ldtbEEContributions = busBase.Select("cdoPersonAccountRetirementContribution.GetEEContributionForPlanYear", new object[] { iintPersonAccountId, this.ibusBenefitApplication.icdoBenefitApplication.retirement_date.Year });
            if (ldtbEEContributions.Rows.Count > 0)
            {
                decimal ldecPriorYearEEInterest = decimal.Zero;
                if (idecEEContribution == 0)
                {
                    idecEEContribution = Convert.ToDecimal(Convert.ToBoolean(ldtbEEContributions.Rows[0][0].IsDBNull()) ? busConstant.ZERO_DECIMAL : ldtbEEContributions.Rows[0][0]);
                    idecEEInterest = Convert.ToDecimal(Convert.ToBoolean(ldtbEEContributions.Rows[0][1].IsDBNull()) ? busConstant.ZERO_DECIMAL : ldtbEEContributions.Rows[0][1]);
                }
                idecEEPartialInterest = ibusCalculation.CalculatePartialEEInterest(this.ibusBenefitApplication.icdoBenefitApplication.retirement_date,
                                        this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).First(),
                                        true, false, iclbPersonAccountRetirementContribution, out ldecPriorYearEEInterest);
                DateTime ldtEEContributionAsOfDate = Convert.ToDateTime(Convert.ToBoolean(ldtbEEContributions.Rows[0][2].IsDBNull()) ? DateTime.MinValue : ldtbEEContributions.Rows[0][2]);


                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId &&
                    item.icdoBenefitCalculationDetail.uvhp_flag.IsNullOrEmpty() && item.icdoBenefitCalculationDetail.retirement_date == this.ibusBenefitApplication.icdoBenefitApplication.retirement_date).
                    FirstOrDefault().icdoBenefitCalculationDetail.vested_ee_amount = idecEEContribution;
                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId &&
                    item.icdoBenefitCalculationDetail.uvhp_flag.IsNullOrEmpty() && item.icdoBenefitCalculationDetail.retirement_date == this.ibusBenefitApplication.icdoBenefitApplication.retirement_date).
                    FirstOrDefault().icdoBenefitCalculationDetail.vested_ee_interest = idecEEInterest + idecEEPartialInterest;
                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId &&
                    item.icdoBenefitCalculationDetail.uvhp_flag.IsNullOrEmpty() && item.icdoBenefitCalculationDetail.retirement_date == this.ibusBenefitApplication.icdoBenefitApplication.retirement_date).
                    FirstOrDefault().icdoBenefitCalculationDetail.ee_as_of_date = ldtEEContributionAsOfDate;


                decimal ldecMea = ibusCalculation.CalculateMEA(idecEEContribution, iblnIsQualifiedSpouse, this.icdoBenefitCalculationHeader.idecParticipantFullAge, this.icdoBenefitCalculationHeader.idecSurvivorFullAge);

                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId && item.icdoBenefitCalculationDetail.retirement_date == this.ibusBenefitApplication.icdoBenefitApplication.retirement_date
                                                         && item.icdoBenefitCalculationDetail.uvhp_flag.IsNullOrEmpty()).FirstOrDefault().icdoBenefitCalculationDetail.monthly_exclusion_amount = ldecMea;

            }
        }

        public void CalculateMgAndMea()
        {
            decimal ldecCreditedMG = 0M;
            busPayeeAccount lbusEarlyRetirementPayeeAccount = null;
            if (this.icdoBenefitCalculationHeader.payee_account_id > 0)
            {
                lbusEarlyRetirementPayeeAccount = new busPayeeAccount();
                lbusEarlyRetirementPayeeAccount.FindPayeeAccount(this.icdoBenefitCalculationHeader.payee_account_id);
                lbusEarlyRetirementPayeeAccount.LoadBenefitDetails();
                lbusEarlyRetirementPayeeAccount.LoadPaymentHistoryHeaderDetails();
            }

            if (icdoBenefitCalculationHeader.payee_account_id > 0 && lbusEarlyRetirementPayeeAccount != null
                               && this.ibusBenefitApplication.icdoBenefitApplication.retirement_date < lbusEarlyRetirementPayeeAccount.icdoPayeeAccount.idtRetireMentDate)
            {
                ldecCreditedMG = lbusEarlyRetirementPayeeAccount.icdoPayeeAccount.minimum_guarantee_amount - lbusEarlyRetirementPayeeAccount.idecRemainingMinimumGuaranteeAmount;
            }

            ibusCalculation.CalculateMEAAndMG(idecFinalAccruedBenefitAmount, ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.MPIPP).First(),
                        idecFinalAccruedBenefitAmount * idecLumpSumFactor, this.icdoBenefitCalculationHeader.iintPlanId,
                        Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge), this.ibusBenefitApplication.icdoBenefitApplication.retirement_date, Convert.ToInt32(this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement),
                        this.iblnIsQualifiedSpouse, busConstant.BOOL_FALSE, null, this.iclbBenefitCalculationDetail, this.iclbPersonAccountRetirementContribution,
                        icdoBenefitCalculationHeader.calculation_type_value, this.icdoBenefitCalculationHeader.person_id,
                        this.icdoBenefitCalculationHeader.beneficiary_person_id, this.icdoBenefitCalculationHeader.benefit_type_value, ldecCreditedMG);

        }

        public bool CheckIfHoursReportedAfterDisabilityDate()
        {
            DateTime ldtLastWorkingDate = new DateTime();
            string lstrEmpName = string.Empty;

            //iblnCheckIfHoursAfterDisabilityRetirementDate = ibusCalculation.CheckIfHoursAfterRetirementDate(this.ibusPerson.icdoPerson.istrSSNNonEncrypted, this.icdoBenefitCalculationHeader.retirement_date);
            idictWorkHrsAfterRetirement = ibusCalculation.LoadMPIHoursAfterRetirementDate(this.ibusPerson.icdoPerson.istrSSNNonEncrypted, this.icdoBenefitCalculationHeader.retirement_date, this.icdoBenefitCalculationHeader.iintPlanId, ref ldtLastWorkingDate, ref lstrEmpName);
            if (idictWorkHrsAfterRetirement.Count > 0)
            {
                iblnCheckIfHoursAfterDisabilityRetirementDate = true;
            }
            return iblnCheckIfHoursAfterDisabilityRetirementDate;
        }

        public void LoadHoursAfterRetirementDate()
        {
            if (idictWorkHrsAfterRetirement.Count > 0)
            {
                this.ibusBenefitApplication.aclbReEmployedWorkHistory = new Collection<cdoDummyWorkData>();

                Collection<cdoDummyWorkData> lclbTopTwoDummyWorkData = new Collection<cdoDummyWorkData>();
                lclbTopTwoDummyWorkData = this.ibusBenefitApplication.aclbPersonWorkHistory_MPI.OrderByDescending(item => item.year).ToList().ToCollection();
                lclbTopTwoDummyWorkData = lclbTopTwoDummyWorkData.Take(2).ToList().ToCollection();
                int lintLastYearIsQualifiedYear = lclbTopTwoDummyWorkData[0].qualified_years_count - lclbTopTwoDummyWorkData[1].qualified_years_count;

                int lintlastComputationYear = this.ibusBenefitApplication.aclbPersonWorkHistory_MPI.Max(item => item.year);
                cdoDummyWorkData lcdoLastComputionalYear = this.ibusBenefitApplication.aclbPersonWorkHistory_MPI.Where(item => item.year == lintlastComputationYear).FirstOrDefault();
                // ArrayList arrYears = new ArrayList();
                decimal ldecHours = decimal.Zero;
                cdoDummyWorkData lcdoDummyWorkData = new cdoDummyWorkData();

                foreach (int lintYear in idictWorkHrsAfterRetirement.Keys)
                {
                    ldecHours = decimal.Zero;
                    ldecHours = idictWorkHrsAfterRetirement[lintYear].Values.Sum();

                    if (ldecHours >= 870 && (lintYear != DateTime.Now.Year || this.iblnPostRetirementDeath))
                    {
                        lcdoDummyWorkData = new cdoDummyWorkData();
                        if (lintYear == lclbTopTwoDummyWorkData[0].year && lintLastYearIsQualifiedYear == 1 && lintlastComputationYear == this.icdoBenefitCalculationHeader.retirement_date.Year)
                        {
                            lcdoDummyWorkData.qualified_years_count = lclbTopTwoDummyWorkData[0].qualified_years_count;
                        }
                        else
                        {
                            lcdoDummyWorkData.qualified_years_count = lclbTopTwoDummyWorkData[0].qualified_years_count + 1;
                        }

                        lcdoDummyWorkData.year = lintYear;
                        lcdoDummyWorkData.qualified_hours = ldecHours;
                        //lcdoDummyWorkData.qualified_years_count 
                        lcdoDummyWorkData.vested_hours = ldecHours;
                        lcdoDummyWorkData.iblnHoursAfterRetirement = true;
                        this.ibusBenefitApplication.aclbReEmployedWorkHistory.Add(lcdoDummyWorkData);

                    }
                }
            }
        }

        public void SetRetroactiveAdjustmentVariables()
        {
            if (this.ibusBenefitApplication.icdoBenefitApplication.retirement_date == this.icdoBenefitCalculationHeader.retirement_date)
            {
                //idecEEDerivedComponent = ldecEEDerivedComponent;
                //iintNonSuspendibleMonths = GetNonSuspendibleMonthsBetweenTwoDates();
                idecEEDerivedComponent = ibusCalculation.CalculateEEDerivedBenefitAsOfRetirementDate(this, null);
                iintNonSuspendibleMonths = GetNonSuspendibleMonthsBetweenTwoDatesAfterRetirement();
                //decimal ldecEEDerivedComponent = new decimal();
                //ibusCalculation.CalculateEEDerivedBenefitTillRetirementDate(this, out ldecEEDerivedComponent);
            }
            else if (iblnOptionTwoFlag) //PROD PIR 504
            {
                idecEEDerivedComponent = ibusCalculation.CalculateEEDerivedBenefitAsOfRetirementDateOptionTwo(this, null);
                iintNonSuspendibleMonths = GetNonSuspendibleMonthsForOptionTwo();            
            }
        }
        /// <summary>
        /// PIR : 257  Both dates retirement date and payment date should be inclusive.
        /// </summary>
        /// <returns></returns>
        public int GetNonSuspendibleMonthsBetweenTwoDates()
        {

            int lintNonSuspendibleMonths = 0;
            int lintFromyear = this.icdoBenefitCalculationHeader.retirement_date.Year;
            int lintToYear = this.icdoBenefitCalculationHeader.payment_date.Year;
            DateTime ldtStartDate = new DateTime();
            DateTime ldtEndDate = new DateTime();
            busBenefitCalculationDetail lbusBenefitApplicationDetail = null;
            if (this.icdoBenefitCalculationHeader.iintPlanId == busConstant.MPIPP_PLAN_ID)
            {
                lbusBenefitApplicationDetail = this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId && this.icdoBenefitCalculationHeader.retirement_date == this.ibusBenefitApplication.icdoBenefitApplication.retirement_date).FirstOrDefault();
            }
            for (int i = lintFromyear; i <= lintToYear; i++)
            {
                if (i == lintToYear && this.icdoBenefitCalculationHeader.payment_date.Month == 1)
                    continue;
                busBenefitCalculationYearlyDetail lbusBenefitCalculationYearlyDetail = null;
                if (lbusBenefitApplicationDetail != null)
                {
                    if (lbusBenefitApplicationDetail.iclbBenefitCalculationYearlyDetail.Where(item => item.icdoBenefitCalculationYearlyDetail.plan_year == i).Count() > 0)
                    {
                        lbusBenefitCalculationYearlyDetail = lbusBenefitApplicationDetail.iclbBenefitCalculationYearlyDetail.Where(item => item.icdoBenefitCalculationYearlyDetail.plan_year == i).FirstOrDefault();
                    }
                }
                int lintNonSusForYear = 0;
                if (lintFromyear == lintToYear && i == this.icdoBenefitCalculationHeader.retirement_date.Year)
                {

                    if (i < 2004)
                    {
                        ldtStartDate = this.icdoBenefitCalculationHeader.retirement_date;
                        ldtEndDate = this.icdoBenefitCalculationHeader.payment_date.GetLastDayofMonth();
                    }
                    else
                    {
                        ldtStartDate = busGlobalFunctions.GetFirstPayrollDayOfMonth(this.icdoBenefitCalculationHeader.retirement_date.Year, this.icdoBenefitCalculationHeader.retirement_date.Month);
                        ldtEndDate = busGlobalFunctions.GetLastPayrollDayOfMonth(this.icdoBenefitCalculationHeader.payment_date.Year, this.icdoBenefitCalculationHeader.payment_date.Month);
                    }
                }
                else if (i == this.icdoBenefitCalculationHeader.retirement_date.Year)
                {
                    if (i < 2004)
                    {
                        ldtStartDate = this.icdoBenefitCalculationHeader.retirement_date;
                        ldtEndDate = new DateTime(i, 12, 31);
                    }
                    else
                    {
                        ldtStartDate = busGlobalFunctions.GetFirstPayrollDayOfMonth(this.icdoBenefitCalculationHeader.retirement_date.Year, this.icdoBenefitCalculationHeader.retirement_date.Month);
                        ldtEndDate = busGlobalFunctions.GetLastPayrollDayOfMonth(i, 12);
                    }
                }
                else if (i == this.icdoBenefitCalculationHeader.payment_date.Year)
                {
                    if (i < 2004)
                    {
                        ldtStartDate = new DateTime(i, 1, 1);
                        ldtEndDate = this.icdoBenefitCalculationHeader.payment_date;
                    }
                    else
                    {
                        ldtStartDate = busGlobalFunctions.GetFirstPayrollDayOfMonth(i, 1);
                        ldtEndDate = busGlobalFunctions.GetLastPayrollDayOfMonth(i, this.icdoBenefitCalculationHeader.payment_date.Month);
                    }
                }
                else
                {

                    if (i < 2004)
                    {
                        ldtStartDate = new DateTime(i, 1, 1);
                        ldtEndDate = new DateTime(i, 12, 31);
                    }
                    else
                    {
                        ldtStartDate = busGlobalFunctions.GetFirstPayrollDayOfMonth(i, 1);
                        ldtEndDate = busGlobalFunctions.GetLastPayrollDayOfMonth(i, 12);
                    }
                }
                if (lbusBenefitApplicationDetail != null && this.icdoBenefitCalculationHeader.iintPlanId == busConstant.MPIPP_PLAN_ID)
                {
                    if (lbusBenefitCalculationYearlyDetail == null)
                    {
                        lbusBenefitCalculationYearlyDetail = new busBenefitCalculationYearlyDetail { icdoBenefitCalculationYearlyDetail = new cdoBenefitCalculationYearlyDetail() };
                    }
                    lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.plan_year = i;
                    lbusBenefitCalculationYearlyDetail.iclbBenefitCalculationNonsuspendibleDetail = new Collection<busBenefitCalculationNonsuspendibleDetail>();
                }
                lintNonSusForYear = ibusCalculation.GetNonSuspendibleMonths(this.ibusPerson.icdoPerson.istrSSNNonEncrypted.ToString(), this.ibusPerson, i,
                                                                     busConstant.MPIPP_PLAN_ID, lbusBenefitCalculationYearlyDetail, ldtStartDate, ldtEndDate, true);
                if (lbusBenefitCalculationYearlyDetail != null)
                {
                    if (!(lbusBenefitApplicationDetail.iclbBenefitCalculationYearlyDetail.Where(item => item.icdoBenefitCalculationYearlyDetail.plan_year == i).Count() > 0))
                    {
                        lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.plan_year = i;
                        lbusBenefitApplicationDetail.iclbBenefitCalculationYearlyDetail.Add(lbusBenefitCalculationYearlyDetail);
                    }
                }
                lintNonSuspendibleMonths = lintNonSuspendibleMonths + lintNonSusForYear;
            }
            return lintNonSuspendibleMonths;
        }

        public int GetNonSuspendibleMonthsBetweenTwoDatesAfterRetirement()
        {
            //Prod PIR 93
            int lintNonSuspendibleMonths = 0;
            int lintSuspendibleMonths = 0;
            int lintYearlySuspendibleMonths = 0;

            int lintFromyear = this.icdoBenefitCalculationHeader.retirement_date.Year;
            int lintToYear = this.icdoBenefitCalculationHeader.payment_date.Year;


            this.ibusPerson.LoadPersonSuspendibleMonth();
            int lintTotalMonths = busGlobalFunctions.DateDiffByMonth(this.icdoBenefitCalculationHeader.retirement_date, this.icdoBenefitCalculationHeader.payment_date);

            busBenefitCalculationDetail lbusBenefitCalculationDetail = null;
            if (this.icdoBenefitCalculationHeader.iintPlanId == busConstant.MPIPP_PLAN_ID)
            {
                lbusBenefitCalculationDetail = this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId && this.icdoBenefitCalculationHeader.retirement_date == this.ibusBenefitApplication.icdoBenefitApplication.retirement_date).FirstOrDefault();
            }
            busBenefitCalculationYearlyDetail lbusBenefitCalculationYearlyDetail = null;
            for (int lintYear = lintFromyear; lintYear <= lintToYear; lintYear++)
            {
                lbusBenefitCalculationYearlyDetail = null;
                lintYearlySuspendibleMonths = 0;
                if (lbusBenefitCalculationDetail != null)
                {
                    busBenefitCalculationYearlyDetail lbusExistingReEmployedYearlyDetail = null;
                    if (lbusBenefitCalculationDetail != null)
                    {
                        if (lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.Where(item => item.icdoBenefitCalculationYearlyDetail.plan_year == lintYear && item.icdoBenefitCalculationYearlyDetail.reemployed_flag == busConstant.FLAG_YES).Count() == 0)
                        {
                            lbusBenefitCalculationYearlyDetail = new busBenefitCalculationYearlyDetail { icdoBenefitCalculationYearlyDetail = new cdoBenefitCalculationYearlyDetail() };
                            lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.reemployed_flag = busConstant.FLAG_YES;
                            lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.plan_year = lintYear;
                            lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.Add(lbusBenefitCalculationYearlyDetail);
                        }
                        else
                        {
                            lbusBenefitCalculationYearlyDetail = lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.Where(item => item.icdoBenefitCalculationYearlyDetail.plan_year == lintYear && item.icdoBenefitCalculationYearlyDetail.reemployed_flag == busConstant.FLAG_YES).FirstOrDefault();
                        }

                    }
                }
                if (idictWorkHrsAfterRetirement.Keys.Contains(lintYear))
                {
                    foreach (int month in idictWorkHrsAfterRetirement[lintYear].Keys)
                    {
                        busBenefitCalculationNonsuspendibleDetail lbusBenefitCalculationsuspendibleDetail = new busBenefitCalculationNonsuspendibleDetail() { icdoBenefitCalculationNonsuspendibleDetail = new cdoBenefitCalculationNonsuspendibleDetail() };
                        //SuspendibleHoursChange
                        if (idictWorkHrsAfterRetirement[lintYear][month] >= ibusCalculation.GetSuspendibleHoursValue(lintYear, month))
                        {
                            if (lintYear == this.icdoBenefitCalculationHeader.retirement_date.Year && month < this.icdoBenefitCalculationHeader.retirement_date.Month)
                                continue;
                            lintYearlySuspendibleMonths++;
                            lbusBenefitCalculationsuspendibleDetail.LoadData(lintYear, month, idictWorkHrsAfterRetirement[lintYear][month]);
                            if (lbusBenefitCalculationYearlyDetail != null)
                            {
                                if (lbusBenefitCalculationYearlyDetail.iclbBenefitCalculationNonsuspendibleDetail.IsNull())
                                    lbusBenefitCalculationYearlyDetail.iclbBenefitCalculationNonsuspendibleDetail = new Collection<busBenefitCalculationNonsuspendibleDetail>();

                                lbusBenefitCalculationYearlyDetail.iclbBenefitCalculationNonsuspendibleDetail.Add(lbusBenefitCalculationsuspendibleDetail);
                            }
                        }
                        else
                        {
                            if (this.ibusPerson.iclbPersonSuspendibleMonth.Where(item => item.icdoPersonSuspendibleMonth.plan_year == lintYear && item.icdoPersonSuspendibleMonth.suspendible_month_value == Convert.ToString(month)).Count() > 0)
                            {
                                lintYearlySuspendibleMonths++;
                            }
                        }
                    }
                    lintSuspendibleMonths = lintSuspendibleMonths + lintYearlySuspendibleMonths;
                    if (lbusBenefitCalculationYearlyDetail.IsNotNull())
                    {
                        lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.suspendible_months_count = lintYearlySuspendibleMonths;
                    }
                }

            }
            if (lbusBenefitCalculationDetail.IsNotNull())
            {
                int lintLastCompYear = Convert.ToInt32(lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.Max(item => item.icdoBenefitCalculationYearlyDetail.plan_year));
                if (!lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.IsNullOrEmpty() &&
                    lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.Where(item => !(item.icdoBenefitCalculationYearlyDetail.reemployed_flag == busConstant.FLAG_YES) && item.icdoBenefitCalculationYearlyDetail.plan_year == lintLastCompYear).Count() > 0)
                {
                    lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.Where(item => !(item.icdoBenefitCalculationYearlyDetail.reemployed_flag == busConstant.FLAG_YES) && item.icdoBenefitCalculationYearlyDetail.plan_year == lintLastCompYear).FirstOrDefault().icdoBenefitCalculationYearlyDetail.ee_derived_amount = idecEEDerivedComponent;
                }
            }

            lintNonSuspendibleMonths = lintTotalMonths - lintSuspendibleMonths;
            return lintNonSuspendibleMonths;
        }

        public DateTime GetLastWorkingDate()
        {
            idtLastWorkingDate = new DateTime();
            IDbConnection lconLegacy = DBFunction.GetDBConnection("Legacy");
            if (lconLegacy != null)
            {
                Collection<IDbDataParameter> lcolParameters = new Collection<IDbDataParameter>();
                IDbDataParameter lobjParameter = DBFunction.GetDBParameter();
                lobjParameter.ParameterName = "@SSN";
                lobjParameter.DbType = DbType.String;
                lobjParameter.Value = this.ibusPerson.icdoPerson.istrSSNNonEncrypted.ToLower();
                lcolParameters.Add(lobjParameter); ;
                DataTable ldataTable = new DataTable();

                IDataReader lDataReader = DBFunction.DBExecuteProcedureResult("usp_GetLastWorkingDate", lcolParameters, lconLegacy, null);
                if (lDataReader != null)
                {
                    ldataTable.Load(lDataReader);
                    if (ldataTable.Rows.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(Convert.ToString(ldataTable.Rows[0][0])))
                        {
                            idtLastWorkingDate = Convert.ToDateTime(ldataTable.Rows[0][0]);
                        }
                    }
                }
            }
            return idtLastWorkingDate;
        }

        #region EE_UVHP

        public void CalculateUVHPBenefitOptions(string astrBenefitOptionValue, decimal ldecTotalBenefitAmount)
        {
            decimal ldecLifeyAnnuityFactor = new decimal();
            decimal ldecJAndS50Factor = new decimal();
            decimal ldecLifeAnnuityAmount = decimal.Zero;
            decimal ldecJandSAnnuityAmount = decimal.Zero;
            int lintParticipantAge = Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge);

            this.ibusCalculation.ProcessQDROOffset(this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoBenefitCalculationDetail.retirement_date == this.ibusBenefitApplication.icdoBenefitApplication.retirement_date).FirstOrDefault(), this.ibusPerson.icdoPerson.person_id, ref ldecTotalBenefitAmount, true, true, false, false, busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE);

            busBenefitCalculationOptions lbusBenefitCalculationOption;

            //We hv factor tables till age 64 for Survivor
            int lintSurvivivorAge = Convert.ToInt32(this.icdoBenefitCalculationHeader.idecSurvivorFullAge);
            if (lintSurvivivorAge > 64)
            {
                lintSurvivivorAge = 64;
            }

            #region Get the Necessary Factors
            DataTable ldtMonthlyLifeAnnuity = Select("cdoBenefitProvisionUvhpLifeFactor.GetUVHPLifeFactor", new object[2] { Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge), this.ibusBenefitApplication.icdoBenefitApplication.retirement_date.Year });

            //Need not confuse the query, its just reused at many places. That's why did not change the name     
            // DataTable ldtMonthlyJS50Annuity = Select("cdoBenefitProvisionUvhpFactor.GetEEUVHPFactor", new object[3] { ibusCalculation.GetPlanBenefitId((busConstant.MPIPP_PLAN_ID), busConstant.QJ50), Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge), Convert.ToInt32(this.icdoBenefitCalculationHeader.idecSurvivorFullAge) });
            if (ldtMonthlyLifeAnnuity.Rows.Count > 0)
            {
                ldecLifeyAnnuityFactor = Convert.ToDecimal(ldtMonthlyLifeAnnuity.Rows[0][0]);
                ldecLifeyAnnuityFactor = Math.Round(ldecLifeyAnnuityFactor, 3);
                ldecLifeAnnuityAmount = Math.Round(ldecTotalBenefitAmount / ldecLifeyAnnuityFactor, 2);
            }
            //if (ldtMonthlyJS50Annuity.Rows.Count > 0)
            //{
            //    ldecJAndS50Factor = Convert.ToDecimal(ldtMonthlyJS50Annuity.Rows[0][0]);
            //    ldecJandSAnnuityAmount = Math.Round(ldecJAndS50Factor * ldecTotalBenefitAmount, 2);
            //}
            ldecJAndS50Factor = Convert.ToDecimal(Math.Round(Math.Min(1, Math.Max(0, 0.86 + 0.005 * (lintSurvivivorAge - lintParticipantAge) + 0.006 * (65 - lintParticipantAge))), 3));
            ldecJandSAnnuityAmount = Math.Round(ldecLifeAnnuityAmount * ldecJAndS50Factor, 2);
            #endregion

            #region Switch Case Based on Benefit Options We have to Calculate
            switch (astrBenefitOptionValue)
            {
                case busConstant.CodeValueAll:
                    #region UVHP LUMP SUM
                    lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                    lbusBenefitCalculationOption.LoadDisabilityData(ibusCalculation.GetPlanBenefitId(busConstant.MPIPP_PLAN_ID, busConstant.LUMP_SUM), decimal.Zero, decimal.Zero, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER, this.icdoBenefitCalculationHeader.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.LUMP_SUM, ldecTotalBenefitAmount, busConstant.ZERO_DECIMAL, decimal.One, ldecTotalBenefitAmount, decimal.Zero, decimal.Zero, true, true);
                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId && item.icdoBenefitCalculationDetail.retirement_date == this.ibusBenefitApplication.icdoBenefitApplication.retirement_date).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);

                    #endregion

                    #region JOINT_50_PERCENT_SURVIVOR_ANNUITY

                    if (ldecTotalBenefitAmount > busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT)
                    {
                        lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                        lbusBenefitCalculationOption.LoadDisabilityData(ibusCalculation.GetPlanBenefitId(busConstant.MPIPP_PLAN_ID, busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY), decimal.Zero, decimal.Zero, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER, this.icdoBenefitCalculationHeader.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY, ldecJandSAnnuityAmount, busConstant.ZERO_DECIMAL, ldecJAndS50Factor, ldecJandSAnnuityAmount, decimal.Zero, decimal.Zero, true, true);
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId && item.icdoBenefitCalculationDetail.retirement_date == this.ibusBenefitApplication.icdoBenefitApplication.retirement_date).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);

                    }
                    #endregion

                    #region LIFE_ANNUTIY

                    if (ldecTotalBenefitAmount > busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT)
                    {
                        lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                        lbusBenefitCalculationOption.LoadDisabilityData(ibusCalculation.GetPlanBenefitId(busConstant.MPIPP_PLAN_ID, busConstant.LIFE_ANNUTIY), decimal.Zero, decimal.Zero, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER, this.icdoBenefitCalculationHeader.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.LIFE_ANNUTIY, ldecLifeAnnuityAmount, busConstant.ZERO_DECIMAL, ldecLifeyAnnuityFactor, ldecLifeAnnuityAmount, decimal.Zero, decimal.Zero, true, true);
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId && item.icdoBenefitCalculationDetail.retirement_date == this.ibusBenefitApplication.icdoBenefitApplication.retirement_date).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);

                    }
                    #endregion
                    break;

                case busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY:
                    #region JOINT_50_PERCENT_SURVIVOR_ANNUITY
                    lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                    lbusBenefitCalculationOption.LoadDisabilityData(ibusCalculation.GetPlanBenefitId(busConstant.MPIPP_PLAN_ID, busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY), decimal.Zero, decimal.Zero, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER, this.icdoBenefitCalculationHeader.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY, ldecJandSAnnuityAmount, busConstant.ZERO_DECIMAL, ldecJAndS50Factor, ldecJandSAnnuityAmount, decimal.Zero, decimal.Zero, true, true);

                    if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL ||
                        this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT)
                    {
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId && item.icdoBenefitCalculationDetail.uvhp_flag.IsNotNullOrEmpty()).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    }
                    else
                    {
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    }
                    #endregion
                    break;

                case busConstant.LIFE_ANNUTIY:
                    #region LIFE_ANNUTIY
                    lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                    lbusBenefitCalculationOption.LoadDisabilityData(ibusCalculation.GetPlanBenefitId(busConstant.MPIPP_PLAN_ID, busConstant.LIFE_ANNUTIY), decimal.Zero, decimal.Zero, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER, this.icdoBenefitCalculationHeader.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.LIFE_ANNUTIY, ldecLifeAnnuityAmount, busConstant.ZERO_DECIMAL, ldecLifeyAnnuityFactor, ldecLifeAnnuityAmount, decimal.Zero, decimal.Zero, true, true);

                    if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL ||
                        this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT)
                    {
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId && item.icdoBenefitCalculationDetail.uvhp_flag.IsNotNullOrEmpty()).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    }
                    else
                    {
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    }
                    #endregion
                    break;

                case busConstant.LUMP_SUM:
                    #region UVHP LUMP SUM
                    lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                    lbusBenefitCalculationOption.LoadDisabilityData(ibusCalculation.GetPlanBenefitId(busConstant.MPIPP_PLAN_ID, busConstant.LUMP_SUM), decimal.Zero, decimal.Zero, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER, this.icdoBenefitCalculationHeader.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.LUMP_SUM, ldecTotalBenefitAmount, busConstant.ZERO_DECIMAL, decimal.One, ldecTotalBenefitAmount, decimal.Zero, decimal.Zero, true, true);

                    if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL ||
                       this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT)
                    {
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId && item.icdoBenefitCalculationDetail.uvhp_flag.IsNotNullOrEmpty()).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    }
                    else
                    {
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    }
                    #endregion
                    break;
            }
            #endregion
        }

        /*
        public void CalculatePartialEEInterest()
        {
            decimal ldecBenefitInterestRate = 1;
            object lobjBenefitInterestRate = DBFunction.DBExecuteScalar("cdoBenefitInterestRate.GetBenefitInterestRate", new object[] { this.ibusBenefitApplication.icdoBenefitApplication.retirement_date.Year },
                                             iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

            if (lobjBenefitInterestRate != null)
            {
                ldecBenefitInterestRate = Convert.ToDecimal(lobjBenefitInterestRate);
            }
            decimal ldecUVHPPartialInterestAmount = Math.Round(((idecUVHPContribution + idecUVHPInterest) * ldecBenefitInterestRate) / 12 * (this.ibusBenefitApplication.icdoBenefitApplication.retirement_date.Month - 1), 2);
            decimal ldecEEPartialInterestAmount = Math.Round(((idecEEContribution + idecEEInterest) * ldecBenefitInterestRate) / 12 * (this.ibusBenefitApplication.icdoBenefitApplication.retirement_date.Month - 1), 2);

            idecEEInterest = idecEEInterest + ldecEEPartialInterestAmount;
            idecUVHPInterest = idecUVHPInterest + ldecUVHPPartialInterestAmount;
        }

        private void FillEEUVHPAmounts()
        {
            if (this.icdoBenefitCalculationHeader.iintPlanId == busConstant.MPIPP_PLAN_ID)
            {
                CalculatePartialEEUVHPInterest();

                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId && item.icdoBenefitCalculationDetail.retirement_date == this.ibusBenefitApplication.icdoBenefitApplication.retirement_date).FirstOrDefault().icdoBenefitCalculationDetail.total_ee_contribution_amount = idecEEContribution;
                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId && item.icdoBenefitCalculationDetail.retirement_date == this.ibusBenefitApplication.icdoBenefitApplication.retirement_date).FirstOrDefault().icdoBenefitCalculationDetail.total_ee_interest_amount = idecEEInterest;
                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId && item.icdoBenefitCalculationDetail.retirement_date == this.ibusBenefitApplication.icdoBenefitApplication.retirement_date).FirstOrDefault().icdoBenefitCalculationDetail.total_uvhp_contribution_amount = idecUVHPContribution;
                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId && item.icdoBenefitCalculationDetail.retirement_date == this.ibusBenefitApplication.icdoBenefitApplication.retirement_date).FirstOrDefault().icdoBenefitCalculationDetail.total_uvhp_interest_amount = idecUVHPInterest;

            }


        }*/

        #endregion

        /// <summary>
        /// As per UAT PIR 257 rounding needs to be done at every step. 
        /// Retirement Date and Payment Date Month are both inclusive.
        /// </summary>
        /// <param name="adecParticipantAmount"></param>
        /// <param name="adecBenefitOptionFactor"></param>
        /// <param name="ablnReductionFactorForRetroactive"></param>
        /// <returns></returns>
        private decimal GetRetroactiveBenefitAmount(decimal adecParticipantAmount, decimal adecBenefitOptionFactor, bool ablnReductionFactorForRetroactive)
        {
            decimal ldecRetroactiveAdjustment = new decimal();
            decimal ldecERF = 1;
            if (ablnReductionFactorForRetroactive)
            {
                ldecERF = this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId && item.icdoBenefitCalculationDetail.retirement_date == this.ibusBenefitApplication.icdoBenefitApplication.retirement_date).FirstOrDefault().icdoBenefitCalculationDetail.early_reduction_factor;
            }
            decimal ldecQdroOffset = this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId && item.icdoBenefitCalculationDetail.retirement_date == this.ibusBenefitApplication.icdoBenefitApplication.retirement_date).FirstOrDefault().icdoBenefitCalculationDetail.qdro_offset;
            decimal ldecEEderivedComponent = Math.Round(adecBenefitOptionFactor * idecEEDerivedComponent * ldecERF, 2);
            decimal ldecERComponent = decimal.Zero;
            int lintFromyear = this.icdoBenefitCalculationHeader.retirement_date.Year;
            int lintToYear = this.icdoBenefitCalculationHeader.payment_date.Year;
            busBenefitCalculationYearlyDetail lbusBenefitCalculationPrevYearlyDetail;

            #region Yearly Detail
            Collection<busBenefitCalculationYearlyDetail> iclbYearlyDetail = this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId && item.icdoBenefitCalculationDetail.retirement_date == this.ibusBenefitApplication.icdoBenefitApplication.retirement_date).FirstOrDefault().iclbBenefitCalculationYearlyDetail;
            if (!iclbYearlyDetail.IsNullOrEmpty())
            {
                iclbYearlyDetail.OrderByDescending(item => item.icdoBenefitCalculationYearlyDetail.plan_year).Where(item => !(item.icdoBenefitCalculationYearlyDetail.reemployed_flag == busConstant.FLAG_YES)).FirstOrDefault().icdoBenefitCalculationYearlyDetail.ee_derived_amount = ldecEEderivedComponent;
            }
            #endregion

            #region Yearly Detail After Retirement
            Collection<busBenefitCalculationYearlyDetail> iclbYearlyDetailAfterRetirement = this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId && item.icdoBenefitCalculationDetail.retirement_date == this.ibusBenefitApplication.icdoBenefitApplication.retirement_date).FirstOrDefault().iclbBenefitCalculationYearlyDetail.Where(item => item.icdoBenefitCalculationYearlyDetail.plan_year >= lintFromyear && item.icdoBenefitCalculationYearlyDetail.reemployed_flag == busConstant.FLAG_YES).ToList().ToCollection();
            if (!iclbYearlyDetailAfterRetirement.IsNullOrEmpty())
            {
                iclbYearlyDetailAfterRetirement = iclbYearlyDetailAfterRetirement.OrderBy(item => item.icdoBenefitCalculationYearlyDetail.plan_year).ToList().ToCollection();
            }
            #endregion

            if (this.iintParticipantAgeAsofPaymentDate >= 65 && this.icdoBenefitCalculationHeader.iintPlanId == busConstant.MPIPP_PLAN_ID)
            {
                foreach (busBenefitCalculationYearlyDetail lbusBenefitCalculationYearlyDetail in iclbYearlyDetailAfterRetirement)
                {
                    int lintYearMonths = 12;
                    int lintNonSuspendibleMonths = 0;
                    ldecERComponent = decimal.Zero;
                    decimal ldecDROOffset = decimal.Zero;
                    busBenefitCalculationDetail lbusBenefitCalculationDetail = this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId && this.icdoBenefitCalculationHeader.retirement_date == this.ibusBenefitApplication.icdoBenefitApplication.retirement_date).FirstOrDefault();
                    ldecDROOffset = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.qdro_offset;

                    lbusBenefitCalculationPrevYearlyDetail = new busBenefitCalculationYearlyDetail { icdoBenefitCalculationYearlyDetail = new cdoBenefitCalculationYearlyDetail() };

                    #region Get Previous Year Yearly Detail Object To get the Accrued Benefit
                    if (lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.plan_year == this.icdoBenefitCalculationHeader.payment_date.Year &&
                        lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.plan_year == this.icdoBenefitCalculationHeader.retirement_date.Year)
                    {
                        lintYearMonths = this.icdoBenefitCalculationHeader.payment_date.Month - this.icdoBenefitCalculationHeader.retirement_date.Month + 1;
                        lbusBenefitCalculationPrevYearlyDetail = iclbYearlyDetail.OrderByDescending(item => item.icdoBenefitCalculationYearlyDetail.plan_year).Where(item => item.icdoBenefitCalculationYearlyDetail.plan_year <= this.icdoBenefitCalculationHeader.retirement_date.Year && !(item.icdoBenefitCalculationYearlyDetail.reemployed_flag == busConstant.FLAG_YES)).FirstOrDefault();
                    }
                    else if (lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.plan_year == this.icdoBenefitCalculationHeader.retirement_date.Year)
                    {
                        lintYearMonths = 12 - this.icdoBenefitCalculationHeader.retirement_date.Month + 1;
                        lbusBenefitCalculationPrevYearlyDetail = iclbYearlyDetail.OrderByDescending(item => item.icdoBenefitCalculationYearlyDetail.plan_year).Where(item => item.icdoBenefitCalculationYearlyDetail.plan_year <= this.icdoBenefitCalculationHeader.retirement_date.Year && !(item.icdoBenefitCalculationYearlyDetail.reemployed_flag == busConstant.FLAG_YES)).FirstOrDefault();

                    }
                    else if (lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.plan_year == this.icdoBenefitCalculationHeader.payment_date.Year)
                    {
                        lintYearMonths = this.icdoBenefitCalculationHeader.payment_date.Month;
                        lbusBenefitCalculationPrevYearlyDetail = iclbYearlyDetail.Where(item => item.icdoBenefitCalculationYearlyDetail.plan_year == (lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.plan_year - 1)).FirstOrDefault();
                    }
                    else
                    {
                        lbusBenefitCalculationPrevYearlyDetail = iclbYearlyDetail.Where(item => item.icdoBenefitCalculationYearlyDetail.plan_year == (lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.plan_year - 1)).FirstOrDefault();
                    }
                    #endregion

                    #region Calculate RetroActive Amount
                    lintNonSuspendibleMonths = lintYearMonths - lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.suspendible_months_count;
                    if (this.icdoBenefitCalculationHeader.istrRetirementType == busConstant.RETIREMENT_TYPE_LATE)
                    {
                        if (lbusBenefitCalculationPrevYearlyDetail.IsNotNull())
                        {
                            ldecERComponent = lbusBenefitCalculationPrevYearlyDetail.icdoBenefitCalculationYearlyDetail.idecTotalBenefitAmount - ldecEEderivedComponent;
                            lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.retroactive_amount = Math.Round(idecEEDerivedComponent * lintYearMonths, 2) + Math.Round(ldecERComponent * lintNonSuspendibleMonths, 2);
                            ldecRetroactiveAdjustment += lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.retroactive_amount;
                        }

                    }
                    else
                    {
                        if (lbusBenefitCalculationPrevYearlyDetail.IsNotNull())
                        {
                            ldecERComponent = lbusBenefitCalculationPrevYearlyDetail.icdoBenefitCalculationYearlyDetail.idecTotalBenefitAmount - ldecEEderivedComponent;
                            lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.retroactive_amount = Math.Round(idecEEDerivedComponent * lintYearMonths, 2) + Math.Round(ldecERComponent * lintNonSuspendibleMonths, 2);
                            ldecRetroactiveAdjustment += lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.retroactive_amount;
                        }

                    }
                    #endregion

                }
            }
            else
            {
                //PROD PIR 504
                int lintTotalMonths = 0;
                if (!iblnOptionTwoFlag)
                    lintTotalMonths = busGlobalFunctions.DateDiffByMonth(this.icdoBenefitCalculationHeader.retirement_date, this.icdoBenefitCalculationHeader.payment_date);
                else
                    lintTotalMonths = busGlobalFunctions.DateDiffByMonth(this.icdoBenefitCalculationHeader.retirement_date_option_2, this.icdoBenefitCalculationHeader.payment_date);
                if (this.icdoBenefitCalculationHeader.iintPlanId == busConstant.MPIPP_PLAN_ID)
                {
                    //ldecRetroactiveAdjustment = (adecParticipantAmount * adecBenefitOptionFactor * ldecERF); //PROD PIR 504
                    ldecRetroactiveAdjustment = adecParticipantAmount * iintNonSuspendibleMonths + ldecEEderivedComponent * (lintTotalMonths - iintNonSuspendibleMonths);

                }
                else
                {
                    ldecRetroactiveAdjustment = adecParticipantAmount * iintNonSuspendibleMonths;
                }
            }
            ldecRetroactiveAdjustment = Math.Round(ldecRetroactiveAdjustment, 2);

            return ldecRetroactiveAdjustment;

        }

        #endregion

        #region IAP

        public void CalculateIAPBenefitAmount(string astrBenefitOptionValue, bool ablnReEmployed = false, string astrAdjustmentFlag = "")
        {
            busBenefitCalculationOptions lbusBenefitCalculationOptions;
            decimal ldecIAPBalance = busConstant.ZERO_DECIMAL;
            decimal ldecLocal52SpecialAccountBalance = busConstant.ZERO_DECIMAL;
            decimal ldecLocal161SpecialAccountBalance = busConstant.ZERO_DECIMAL;

            decimal ldecIAPHours4QtrAlloc = busConstant.ZERO_DECIMAL;
            decimal ldecIAPHoursA2forQtrAlloc = busConstant.ZERO_DECIMAL;
            decimal ldecIAPPercent4forQtrAlloc = busConstant.ZERO_DECIMAL;

            if (!ablnReEmployed)
            {
                #region To Set Values for IAP QTR Allocations

                idtLatestDate = new DateTime();
                bool lblnDiffDate = false;
                busIapAllocationSummary lbusIapAllocationSummary = new busIapAllocationSummary();
                lbusIapAllocationSummary.LoadLatestAllocationSummary();
                int lintFromYear;
                utlConnection utlLegacyDBConnetion = HelperFunction.GetDBConnectionProperties("Legacy");
                string astrLegacyDBConnetion = utlLegacyDBConnetion.istrConnectionString;

                SqlParameter[] parameters = new SqlParameter[3];
                SqlParameter param1 = new SqlParameter("@SSN", DbType.String);
                SqlParameter param2 = new SqlParameter("@FROMDATE", DbType.DateTime);
                SqlParameter param3 = new SqlParameter("@TODATE", DbType.DateTime);

                param1.Value = this.ibusPerson.icdoPerson.istrSSNNonEncrypted;
                parameters[0] = param1;

                busIapAllocationSummary ibusLatestIAPAllocationSummaryAsofYear = new busIapAllocationSummary();
                int lintMaxYear = ibusLatestIAPAllocationSummaryAsofYear.GetMaxAllocationYear();
                bool lblnCalculateIapAllocations = false;
                //151767
                //if (lintMaxYear >= this.icdoBenefitCalculationHeader.awarded_on_date.AddYears(-1).Year &&
                //    lintMaxYear >= this.ibusBenefitApplication.icdoBenefitApplication.retirement_date.AddYears(-1).Year)
                if (lintMaxYear >= HelperUtil.GetMaxDate(this.icdoBenefitCalculationHeader.awarded_on_date, this.ibusBenefitApplication.icdoBenefitApplication.retirement_date).AddYears(-1).Year)
                {
                    if (this.ibusCalculation.CheckIfFactorAvailableForIapAllocation(this))
                    {
                        lblnCalculateIapAllocations = true;
                    }
                    if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT)
                    {
                        //PIR 985
                        //if (astrAdjustmentFlag != busConstant.FLAG_YES)
                        //{
                        lblnCalculateIapAllocations = false;
                        // }
                    }
                }

                if (this.ibusBenefitApplication.icdoBenefitApplication.retirement_date == this.icdoBenefitCalculationHeader.retirement_date)
                {
                    #region BalanceAsofAwardedOnDate
                    //151767
                    if (this.IsOnlyOnePlanAllowed() == false &&  this.icdoBenefitCalculationHeader.awarded_on_date != DateTime.MinValue)
                    {
                        if (lbusIapAllocationSummary.icdoIapAllocationSummary.computation_year >= this.icdoBenefitCalculationHeader.awarded_on_date.Year)
                        {
                            lintFromYear = this.icdoBenefitCalculationHeader.awarded_on_date.Year - 1;
                        }
                        else
                        {
                            lintFromYear = lbusIapAllocationSummary.icdoIapAllocationSummary.computation_year;
                        }
                        param2.Value = busGlobalFunctions.GetLastDateOfComputationYear(lintFromYear);
                        parameters[1] = param2;

                        param3.Value = busGlobalFunctions.GetLastDayOfWeek(this.icdoBenefitCalculationHeader.awarded_on_date); //PROD PIR 113
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

                        if (!this.iclbPersonAccountRetirementContribution.IsNullOrEmpty())
                        {
                            if (this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.IAP).Count() > 0)
                            {
                                int lintPersonAccountId = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.IAP).First().icdoPersonAccount.person_account_id;
                                if (this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Count() > 0)
                                {
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
                                                                                   this.icdoBenefitCalculationHeader.awarded_on_date, ldecIAPHours4QtrAlloc, ldecIAPHoursA2forQtrAlloc, ldecIAPPercent4forQtrAlloc, lblnCalculateIapAllocations);
                                }
                            }
                        }
                        }

                    }
                    #endregion


                    #region BalanceAsOfOnsetDate
                    /*
                    if (this.icdoBenefitCalculationHeader.ssa_disability_onset_date != DateTime.MinValue)
                    {

                        if (lbusIapAllocationSummary.icdoIapAllocationSummary.computation_year >= this.icdoBenefitCalculationHeader.ssa_disability_onset_date.Year)
                        {
                            lintFromYear = this.icdoBenefitCalculationHeader.ssa_disability_onset_date.Year - 1;
                        }
                        else
                        {
                            lintFromYear = lbusIapAllocationSummary.icdoIapAllocationSummary.computation_year;
                        }

                        param2.Value = busGlobalFunctions.GetLastDateOfComputationYear(lintFromYear);
                        parameters[1] = param2;

                        param3.Value = this.icdoBenefitCalculationHeader.ssa_disability_onset_date;
                        parameters[2] = param3;

                        DataTable ldtbIAPInfo = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetIAPHourInfoForQuarterlyAllocation", astrLegacyDBConnetion, parameters);
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

                        if (!this.iclbPersonAccountRetirementContribution.IsNullOrEmpty())
                        {
                            if (this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.IAP).Count() > 0)
                            {
                                int lintPersonAccountId = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.IAP).First().icdoPersonAccount.person_account_id;
                                if (this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Count() > 0)
                                {
                                    ibusCalculation.GetIAPAndSpecialAccountBalance(busConstant.BOOL_FALSE, null, this.iclbBenefitCalculationDetail, this, null,
                                                                                   this.icdoBenefitCalculationHeader.ssa_disability_onset_date, ldecIAPHours4QtrAlloc, ldecIAPHoursA2forQtrAlloc, ldecIAPPercent4forQtrAlloc);
                                }
                            }
                        }

                    }
                    */
                    #endregion

                    #region BalanceAsPerRetirementDate
                    if (this.icdoBenefitCalculationHeader.retirement_date != DateTime.MinValue)
                    {
                        ldecIAPHours4QtrAlloc = decimal.Zero;
                        ldecIAPHoursA2forQtrAlloc = decimal.Zero;
                        ldecIAPPercent4forQtrAlloc = decimal.Zero;
                        if (lbusIapAllocationSummary.icdoIapAllocationSummary.computation_year >= this.icdoBenefitCalculationHeader.retirement_date.Year)
                        {
                            lintFromYear = this.icdoBenefitCalculationHeader.retirement_date.Year - 1;
                        }
                        else
                        {
                            lintFromYear = lbusIapAllocationSummary.icdoIapAllocationSummary.computation_year;
                        }
                        param2.Value = busGlobalFunctions.GetLastDateOfComputationYear(lintFromYear);
                        parameters[1] = param2;

                        param3.Value = busGlobalFunctions.GetLastDayOfWeek(this.icdoBenefitCalculationHeader.retirement_date);//PROD PIR 113
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

                        if (!this.iclbPersonAccountRetirementContribution.IsNullOrEmpty())
                        {
                            if (this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.IAP).Count() > 0)
                            {
                                int lintPersonAccountId = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.IAP).First().icdoPersonAccount.person_account_id;
                                if (this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Count() > 0)
                                {
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
                                                                                   this.icdoBenefitCalculationHeader.retirement_date, ldecIAPHours4QtrAlloc, ldecIAPHoursA2forQtrAlloc, ldecIAPPercent4forQtrAlloc, lblnCalculateIapAllocations);
                                    }

                                    if (!lblnCalculateIapAllocations)
                                    {
                                        if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL ||
                                                  this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT)
                                        {
                                            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                                                    item.icdoBenefitCalculationDetail.l52_spl_acc_flag.IsNullOrEmpty() &&
                                                    item.icdoBenefitCalculationDetail.l161_spl_acc_flag.IsNullOrEmpty()).FirstOrDefault().icdoBenefitCalculationDetail.adjustment_iap_payment_flag = busConstant.FLAG_YES;
                                        }

                                    }

                                }
                            }
                        }

                    }
                    #endregion

                }
                else if (this.ibusBenefitApplication.icdoBenefitApplication.retirement_date == this.icdoBenefitCalculationHeader.retirement_date_option_2)
                {

                    #region Option2

                    if (this.icdoBenefitCalculationHeader.retirement_date_option_2 != DateTime.MinValue)
                    {
                        ldecIAPHours4QtrAlloc = decimal.Zero;
                        ldecIAPHoursA2forQtrAlloc = decimal.Zero;
                        ldecIAPPercent4forQtrAlloc = decimal.Zero;
                        if (lbusIapAllocationSummary.icdoIapAllocationSummary.computation_year >= this.icdoBenefitCalculationHeader.retirement_date_option_2.Year)
                        {
                            lintFromYear = this.icdoBenefitCalculationHeader.retirement_date_option_2.Year - 1;
                        }
                        else
                        {
                            lintFromYear = lbusIapAllocationSummary.icdoIapAllocationSummary.computation_year;
                        }
                        param2.Value = busGlobalFunctions.GetLastDateOfComputationYear(lintFromYear);
                        parameters[1] = param2;

                        param3.Value = busGlobalFunctions.GetLastDayOfWeek(this.icdoBenefitCalculationHeader.retirement_date_option_2);//PROD PIR 113
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

                        if (!this.iclbPersonAccountRetirementContribution.IsNullOrEmpty())
                        {
                            if (this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.IAP).Count() > 0)
                            {
                                int lintPersonAccountId = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.IAP).First().icdoPersonAccount.person_account_id;
                                if (this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Count() > 0)
                                {
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
                                                                                   this.icdoBenefitCalculationHeader.retirement_date_option_2, ldecIAPHours4QtrAlloc, ldecIAPHoursA2forQtrAlloc, ldecIAPPercent4forQtrAlloc, lblnCalculateIapAllocations);
                                    }

                                }
                            }
                        }

                    }

                    #endregion
                }
                #endregion
            }

            if (this.iblnCalculateIAPBenefit)
            {
                ldecIAPBalance = this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID && item.icdoBenefitCalculationDetail.retirement_date == this.ibusBenefitApplication.icdoBenefitApplication.retirement_date).FirstOrDefault().icdoBenefitCalculationDetail.iap_balance_amount;
                if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL ||
                    this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT)
                {
                    // Set the Default Parameters 
                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                                                                    item.icdoBenefitCalculationDetail.l52_spl_acc_flag.IsNullOrEmpty() &&
                                                                    item.icdoBenefitCalculationDetail.l161_spl_acc_flag.IsNullOrEmpty()).FirstOrDefault().icdoBenefitCalculationDetail.unreduced_benefit_amount = ldecIAPBalance;

                    //Process QDRO Offset
                    this.ibusCalculation.ProcessQDROOffset(this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID && item.icdoBenefitCalculationDetail.l52_spl_acc_flag.IsNullOrEmpty() && item.icdoBenefitCalculationDetail.l161_spl_acc_flag.IsNullOrEmpty()).First()
                        , this.ibusPerson.icdoPerson.person_id, ref ldecIAPBalance);

                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).First().
                        icdoBenefitCalculationDetail.idecBenefitsAfterQDROOffSet = ldecIAPBalance;


                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                                                                    item.icdoBenefitCalculationDetail.l52_spl_acc_flag.IsNullOrEmpty() &&
                                                                    item.icdoBenefitCalculationDetail.l161_spl_acc_flag.IsNullOrEmpty()).FirstOrDefault().icdoBenefitCalculationDetail.early_reduction_factor = 1.0m;

                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                                                                    item.icdoBenefitCalculationDetail.l52_spl_acc_flag.IsNullOrEmpty() &&
                                                                    item.icdoBenefitCalculationDetail.l161_spl_acc_flag.IsNullOrEmpty()).FirstOrDefault().icdoBenefitCalculationDetail.early_reduced_benefit_amount = ldecIAPBalance;

                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                                                                    item.icdoBenefitCalculationDetail.l52_spl_acc_flag.IsNullOrEmpty() &&
                                                                    item.icdoBenefitCalculationDetail.l161_spl_acc_flag.IsNullOrEmpty()).FirstOrDefault().icdoBenefitCalculationDetail.final_monthly_benefit_amount = ldecIAPBalance;


                }
                else
                {
                    // Set the Default Parameters 
                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID && item.icdoBenefitCalculationDetail.retirement_date == this.ibusBenefitApplication.icdoBenefitApplication.retirement_date).FirstOrDefault().icdoBenefitCalculationDetail.unreduced_benefit_amount = ldecIAPBalance;
                    //Process QDRO Offset
                    this.ibusCalculation.ProcessQDROOffset(this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID && item.icdoBenefitCalculationDetail.l52_spl_acc_flag.IsNullOrEmpty() && item.icdoBenefitCalculationDetail.l161_spl_acc_flag.IsNullOrEmpty()).First()
                        , this.ibusPerson.icdoPerson.person_id, ref ldecIAPBalance);

                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).First().
                        icdoBenefitCalculationDetail.idecBenefitsAfterQDROOffSet = ldecIAPBalance;


                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID && item.icdoBenefitCalculationDetail.retirement_date == this.ibusBenefitApplication.icdoBenefitApplication.retirement_date).FirstOrDefault().icdoBenefitCalculationDetail.early_reduction_factor = 1.0m;
                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID && item.icdoBenefitCalculationDetail.retirement_date == this.ibusBenefitApplication.icdoBenefitApplication.retirement_date).FirstOrDefault().icdoBenefitCalculationDetail.early_reduced_benefit_amount = ldecIAPBalance;
                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID && item.icdoBenefitCalculationDetail.retirement_date == this.ibusBenefitApplication.icdoBenefitApplication.retirement_date).FirstOrDefault().icdoBenefitCalculationDetail.final_monthly_benefit_amount = ldecIAPBalance;
                }
            }

            if (this.iblnCalculateL52SplAccBenefit)
            {
                ldecLocal52SpecialAccountBalance = this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID && item.icdoBenefitCalculationDetail.retirement_date == this.ibusBenefitApplication.icdoBenefitApplication.retirement_date).FirstOrDefault().icdoBenefitCalculationDetail.local52_special_acct_bal_amount;
                this.ibusCalculation.ProcessQDROOffset(this.iclbBenefitCalculationDetail.Where(i => i.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault(), this.ibusPerson.icdoPerson.person_id, ref ldecLocal52SpecialAccountBalance, false, false, true, false, busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE);

            }

            if (this.iblnCalculateL161SplAccBenefit)
            {
                ldecLocal161SpecialAccountBalance = this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID && item.icdoBenefitCalculationDetail.retirement_date == this.ibusBenefitApplication.icdoBenefitApplication.retirement_date).FirstOrDefault().icdoBenefitCalculationDetail.local161_special_acct_bal_amount;
                this.ibusCalculation.ProcessQDROOffset(this.iclbBenefitCalculationDetail.Where(i => i.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault(), this.ibusPerson.icdoPerson.person_id, ref ldecLocal52SpecialAccountBalance, false, false, false, true, busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE);
            }

            if ((ldecIAPBalance > busConstant.ZERO_DECIMAL || ldecLocal52SpecialAccountBalance > busConstant.ZERO_DECIMAL || ldecLocal161SpecialAccountBalance > busConstant.ZERO_DECIMAL)
                || icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT)//PIR 985 10252015
            {
                if (astrBenefitOptionValue == busConstant.CodeValueAll || astrBenefitOptionValue == busConstant.LUMP_SUM)
                {
                    if (this.iblnCalculateIAPBenefit)
                    {
                        lbusBenefitCalculationOptions = this.GetIAPDisabilityBenefitOptions(busConstant.LUMP_SUM, ldecIAPBalance, false, false, idtLatestDate);
                    }
                    if (this.iblnCalculateL161SplAccBenefit)
                    {
                        lbusBenefitCalculationOptions = this.GetIAPDisabilityBenefitOptions(busConstant.LUMP_SUM, ldecLocal161SpecialAccountBalance, false, true, idtLatestDate);
                    }
                    if (this.iblnCalculateL52SplAccBenefit)
                    {
                        lbusBenefitCalculationOptions = this.GetIAPDisabilityBenefitOptions(busConstant.LUMP_SUM, ldecLocal52SpecialAccountBalance, true, false, idtLatestDate);
                    }
                }
                if (iblnIsQualifiedSpouse)
                {
                    if (astrBenefitOptionValue == busConstant.CodeValueAll || astrBenefitOptionValue == busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY)
                    {
                        if (this.iblnCalculateIAPBenefit)
                        {
                            lbusBenefitCalculationOptions = this.GetIAPDisabilityBenefitOptions(busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY, ldecIAPBalance, false, false, idtLatestDate);
                        }
                        if (this.iblnCalculateL161SplAccBenefit)
                        {
                            lbusBenefitCalculationOptions = this.GetIAPDisabilityBenefitOptions(busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY, ldecLocal161SpecialAccountBalance, false, true, idtLatestDate);
                        }
                        if (this.iblnCalculateL52SplAccBenefit)
                        {
                            lbusBenefitCalculationOptions = this.GetIAPDisabilityBenefitOptions(busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY, ldecLocal52SpecialAccountBalance, true, false, idtLatestDate);
                        }

                    }
                    if (astrBenefitOptionValue == busConstant.CodeValueAll || astrBenefitOptionValue == busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY)
                    {
                        if (this.ibusBenefitApplication.icdoBenefitApplication.retirement_date > busConstant.BenefitCalculation.DISABILITY_JS75_CHECK_DATE)
                        {
                            if (this.iblnCalculateIAPBenefit)
                            {
                                lbusBenefitCalculationOptions = this.GetIAPDisabilityBenefitOptions(busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, ldecIAPBalance, false, false, idtLatestDate);
                            }
                            if (this.iblnCalculateL161SplAccBenefit)
                            {
                                lbusBenefitCalculationOptions = this.GetIAPDisabilityBenefitOptions(busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, ldecLocal161SpecialAccountBalance, false, true, idtLatestDate);
                            }
                            if (this.iblnCalculateL52SplAccBenefit)
                            {
                                lbusBenefitCalculationOptions = this.GetIAPDisabilityBenefitOptions(busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, ldecLocal52SpecialAccountBalance, true, false, idtLatestDate);
                            }
                        }
                    }
                    if (astrBenefitOptionValue == busConstant.CodeValueAll || astrBenefitOptionValue == busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY)
                    {
                        if (this.iblnCalculateIAPBenefit)
                        {
                            lbusBenefitCalculationOptions = this.GetIAPDisabilityBenefitOptions(busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY, ldecIAPBalance, false, false, idtLatestDate);
                        }


                    }
                }
                if (astrBenefitOptionValue == busConstant.CodeValueAll || astrBenefitOptionValue == busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY)
                {
                    if (this.iblnCalculateIAPBenefit)
                    {
                        lbusBenefitCalculationOptions = this.GetIAPDisabilityBenefitOptions(busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, ldecIAPBalance, false, false, idtLatestDate);
                    }

                }
                if (astrBenefitOptionValue == busConstant.CodeValueAll || astrBenefitOptionValue == busConstant.LIFE_ANNUTIY)
                {
                    if (this.iblnCalculateIAPBenefit)
                    {
                        lbusBenefitCalculationOptions = this.GetIAPDisabilityBenefitOptions(busConstant.LIFE_ANNUTIY, ldecIAPBalance, false, false, idtLatestDate);
                    }
                    if (this.iblnCalculateL161SplAccBenefit)
                    {
                        lbusBenefitCalculationOptions = this.GetIAPDisabilityBenefitOptions(busConstant.LIFE_ANNUTIY, ldecLocal161SpecialAccountBalance, false, true, idtLatestDate);
                    }
                    if (this.iblnCalculateL52SplAccBenefit)
                    {
                        lbusBenefitCalculationOptions = this.GetIAPDisabilityBenefitOptions(busConstant.LIFE_ANNUTIY, ldecLocal52SpecialAccountBalance, true, false, idtLatestDate);
                    }

                }
            }
        }

        private busBenefitCalculationOptions GetIAPDisabilityBenefitOptions(string astrBenefitOption, decimal adecBalance, bool ablnL52Flag, bool ablnL161Flag, DateTime adtLatestDate)
        {
            decimal ldecDisBenefitOptionFactor = new decimal();
            decimal ldecDisBenefitAmount = new decimal();
            decimal ldecParticipantAmount = new decimal();
            decimal ldecSurvivorAmount = new decimal();
            decimal ldecRetroactiveAmount = Decimal.Zero;
            decimal ldecAnnunityAdjustmentMultiplier = this.GetAnnunityMultiplier();
            int lintRemainder = 0;
            int lintParticipantAge = Convert.ToInt32(Math.Floor(this.icdoBenefitCalculationHeader.idecParticipantFullAge));
            int lintSpouseAge = Convert.ToInt32(Math.Floor(this.icdoBenefitCalculationHeader.idecSurvivorFullAge));

            if (adtLatestDate != this.icdoBenefitCalculationHeader.retirement_date)
            {
                lintParticipantAge = Convert.ToInt32(Math.Floor(busGlobalFunctions.CalculatePersonAge(this.ibusPerson.icdoPerson.idtDateofBirth, adtLatestDate)));
                lintSpouseAge = Convert.ToInt32(Math.Floor(busGlobalFunctions.CalculatePersonAge(this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth, adtLatestDate)));
            }

            int lintPlanBenefitID = ibusCalculation.GetPlanBenefitId(busConstant.IAP_PLAN_ID, astrBenefitOption);
            if (astrBenefitOption == busConstant.LIFE_ANNUTIY || astrBenefitOption == busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY)
            {
                ldecDisBenefitOptionFactor = ibusCalculation.GetBenefitOptionFactor(this.icdoBenefitCalculationHeader.benefit_type_value, lintPlanBenefitID, lintParticipantAge, 0) * 12;
            }
            else if (astrBenefitOption == busConstant.LUMP_SUM)
            {
                ldecDisBenefitOptionFactor = 1;
            }
            else
            {
                ldecDisBenefitOptionFactor = ibusCalculation.GetBenefitOptionFactor(this.icdoBenefitCalculationHeader.benefit_type_value, lintPlanBenefitID, lintParticipantAge, lintParticipantAge) * 12;
            }

            if (astrBenefitOption != busConstant.LUMP_SUM)
            {
                ldecDisBenefitAmount = Math.Round(adecBalance / ldecDisBenefitOptionFactor);
                if (astrBenefitOption != busConstant.LUMP_SUM)
                {
                    Math.DivRem(Convert.ToInt32(ldecDisBenefitAmount), 10, out lintRemainder);
                    if (lintRemainder > 0)
                    {
                        ldecDisBenefitAmount = ldecDisBenefitAmount - lintRemainder;
                    }
                }
            }
            else
            {
                ldecDisBenefitAmount = Math.Round(adecBalance, 2);

            }
            if (astrBenefitOption != busConstant.LUMP_SUM && astrBenefitOption != busConstant.LIFE && astrBenefitOption != busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY)
            {
                ldecSurvivorAmount = ldecDisBenefitAmount;

                if (astrBenefitOption == busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY || astrBenefitOption == busConstant.JOINT_50_PERCENT_POPUP_ANNUITY)
                {
                    ldecSurvivorAmount = ldecSurvivorAmount * .50m;
                }
                if (astrBenefitOption == busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY)
                {
                    ldecSurvivorAmount = ldecSurvivorAmount * .75m;
                }
            }

            ldecParticipantAmount = ldecDisBenefitAmount;
            if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL ||
                this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT)
            {
                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().icdoBenefitCalculationDetail.elected_benefit_amount = ldecParticipantAmount;
            }
            busBenefitCalculationOptions lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
            if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE)
            {
                lbusBenefitCalculationOptions.LoadDisabilityData(lintPlanBenefitID, new decimal(), new decimal(), busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER, this.icdoBenefitCalculationHeader.person_id,
               this.icdoBenefitCalculationHeader.beneficiary_person_id, astrBenefitOption, astrBenefitOption != busConstant.LUMP_SUM ? ldecAnnunityAdjustmentMultiplier == 0 ? ldecDisBenefitAmount: ldecDisBenefitAmount*ldecAnnunityAdjustmentMultiplier: ldecDisBenefitAmount, astrBenefitOption != busConstant.LUMP_SUM ? ldecAnnunityAdjustmentMultiplier == 0 ? ldecSurvivorAmount: ldecSurvivorAmount*ldecAnnunityAdjustmentMultiplier: ldecSurvivorAmount, ldecDisBenefitOptionFactor, astrBenefitOption != busConstant.LUMP_SUM ? ldecAnnunityAdjustmentMultiplier == 0 ? ldecDisBenefitAmount : ldecDisBenefitAmount * ldecAnnunityAdjustmentMultiplier: ldecDisBenefitAmount, ldecRetroactiveAmount, 1, false, false, ablnL52Flag, ablnL161Flag);

            }
            else
            {
                lbusBenefitCalculationOptions.LoadDisabilityData(lintPlanBenefitID, new decimal(), new decimal(), busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER, this.icdoBenefitCalculationHeader.person_id,
               this.icdoBenefitCalculationHeader.beneficiary_person_id, astrBenefitOption, ldecDisBenefitAmount, ldecSurvivorAmount, ldecDisBenefitOptionFactor, ldecDisBenefitAmount, ldecRetroactiveAmount, 1, false, false, ablnL52Flag, ablnL161Flag);

            }
               
            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID && item.icdoBenefitCalculationDetail.retirement_date == this.ibusBenefitApplication.icdoBenefitApplication.retirement_date).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
            return lbusBenefitCalculationOptions;


        }

        #endregion


        #region LOCAL


        private busBenefitCalculationOptions GetLocalDisabilityBenefitOptions(decimal adecDisabilityFactor, decimal adecDisabilityBenefit, decimal adecRegularBenefit
            , decimal adecRegularFactor, string astrBenefitOption)
        {
            int lintPlanBenID = this.iclbcdoPlanBenefit.Where(lbusPlanBenefitXr => lbusPlanBenefitXr.icdoPlanBenefitXr.benefit_option_value == astrBenefitOption).First().icdoPlanBenefitXr.plan_benefit_id;
            decimal ldecParticipantAmount = 0;
            decimal ldecSurvivorAmount = 0;

            if (astrBenefitOption == busConstant.LUMP_SUM)
            {

            }

            if (adecDisabilityBenefit < adecRegularBenefit)
            {
                ldecParticipantAmount = Math.Round(adecRegularBenefit, 4);
            }
            else
            {
                ldecParticipantAmount = Math.Round(adecDisabilityBenefit, 4);
            }

            if (astrBenefitOption == busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY || astrBenefitOption == busConstant.JOINT_50_PERCENT_POPUP_ANNUITY ||
                astrBenefitOption == busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY || astrBenefitOption == busConstant.JOINT_100_PERCENT_POPUP_ANNUITY ||
                astrBenefitOption == busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY || astrBenefitOption == busConstant.JOINT_66_2by3_PERCENT_SURVIVOR_ANNUITY)
            {
                if (astrBenefitOption == busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY || astrBenefitOption == busConstant.JOINT_50_PERCENT_POPUP_ANNUITY)
                {
                    ldecSurvivorAmount = Math.Round(ldecParticipantAmount * .50m, 4);
                }
                else if (astrBenefitOption == busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY)
                {
                    ldecSurvivorAmount = Math.Round(ldecParticipantAmount * .75m, 4);
                }
                else if (astrBenefitOption == busConstant.JOINT_66_2by3_PERCENT_SURVIVOR_ANNUITY)
                {
                    ldecSurvivorAmount = Math.Round((ldecParticipantAmount * 2) / 3, 4);
                }
            }
            if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL ||
                this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT)
            {
                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().icdoBenefitCalculationDetail.elected_benefit_amount = ldecParticipantAmount;
            }
            busBenefitCalculationOptions lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
            lbusBenefitCalculationOptions.LoadDisabilityData(lintPlanBenID, adecRegularFactor, adecRegularBenefit, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER, this.icdoBenefitCalculationHeader.person_id,
            this.icdoBenefitCalculationHeader.beneficiary_person_id, astrBenefitOption, ldecParticipantAmount, ldecSurvivorAmount, adecDisabilityFactor, adecDisabilityBenefit);
            return lbusBenefitCalculationOptions;



        }

        private void CalculateLocal161Benefit(string astrBeneifitOptionValue, bool ablnFinalCalc)
        {
            if (iblnEligibleForRegularBenefit)
            {
                CalculateL161BenefitOptionsForRegularAndDisabled(astrBeneifitOptionValue, ablnFinalCalc);
            }
            else
            {
                CalculateAllLocalBenefitOptionsForDiabledPerson(astrBeneifitOptionValue, idecLocalFrozenBenefit, 0, 0);
                //CalculateL161BenefitOptionsForDiabledPerson(astrBeneifitOptionValue, idecLocalFrozenBenefit, 0, 0);
            }

        }


        private void CalculateLocal600Benefit(string astrBenefitOptionValue, bool ablnFinalCalc)
        {
            if (iblnEligibleForRegularBenefit)
            {
                CalculateL600BenefitOptionsForRegularAndDisabled(astrBenefitOptionValue, ablnFinalCalc);
            }
            else
            {
                CalculateAllLocalBenefitOptionsForDiabledPerson(astrBenefitOptionValue, idecLocalFrozenBenefit, 0, 0);
                //CalculateL600BenefitOptionsForDiabledPerson(astrBenefitOptionValue, idecLocalFrozenBenefit, 0, 0);
            }
        }


        private void CalculateLocal52Benefit(string astrBenefitOptionValue, bool ablnFinalCalc)
        {
            if (iblnEligibleForRegularBenefit)
            {
                CalculateL52BenefitOptionsForRegularAndDisabled(astrBenefitOptionValue, ablnFinalCalc);
            }
            else
            {
                CalculateAllLocalBenefitOptionsForDiabledPerson(astrBenefitOptionValue, idecLocalFrozenBenefit, 0, 0);

                //CalculateL52BenefitOptionsForDiabledPerson(astrBenefitOptionValue, idecLocalFrozenBenefit, 0, 0);
            }
        }

        private void CalculateLocal666Benefit(string astrBenefitOptionValue, bool ablnFinalCalc)
        {
            if (iblnEligibleForRegularBenefit)
            {
                CalculateL666BenefitOptionsForRegularAndDisabled(astrBenefitOptionValue, ablnFinalCalc);
            }
            else
            {
                CalculateAllLocalBenefitOptionsForDiabledPerson(astrBenefitOptionValue, idecLocalFrozenBenefit, 0, 0);
                //CalculateL666BenefitOptionsForDiabledPerson(astrBenefitOptionValue, idecLocalFrozenBenefit, 0, 0);
            }
        }

        private void CalculateLocal700Benefit(string astrBenefitOptionValue, bool ablnFinalCalc)
        {
            if (iblnEligibleForRegularBenefit)
            {
                CalculateL700BenefitOptionsForRegularAndDisabled(astrBenefitOptionValue, ablnFinalCalc);
            }
            else
            {
                CalculateAllLocalBenefitOptionsForDiabledPerson(astrBenefitOptionValue, idecLocalFrozenBenefit, 0, 0);

                //CalculateL700BenefitOptionsForDiabledPerson(astrBenefitOptionValue, idecLocalFrozenBenefit, 0, 0);
            }
        }

        #region OptionsNoDisability
        /*
        private void CalculateAllLocalBenefitOptionsForRegularAndDiabledPerson(string astrBenefitOption, bool ablnFinalCalc)
        {
            decimal ldecRegularBenefitAfterReductionFactor = this.ibusCalculation.CalculateTotalBenefitAmtForLocal161(this.icdoBenefitCalculationHeader.istrRetirementType,
                                                         this.icdoBenefitCalculationHeader.retirement_date, this.ibusPerson.icdoPerson.idtDateofBirth,
                                                         idecLocalFrozenBenefit,
                                                         false, this.ibusBenefitApplication, this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == iintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_pre_bis_amount),
                                                         this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == iintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_post_bis_amount),
                                                         null, this.iclbBenefitCalculationDetail,
                                                         Convert.ToInt32(this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == iintPersonAccountId).Max(item => item.icdoPersonAccountRetirementContribution.computational_year)),
                                                         this.iclbPersonAccountRetirementContribution, ablnFinalCalc);

            decimal ldecNetRegularBenefitAmount = 1;
            decimal ldecRegularFactor = 1;


            busBenefitCalculationOptions lbusBenefitCalculationOptions = null;

            #region Lump
            if (astrBenefitOptionValue == busConstant.CodeValueAll || astrBenefitOptionValue == busConstant.LUMP_SUM)
            {

                ldecRegularFactor = idecLumpSumFactor;
                ldecNetRegularBenefitAmount = Convert.ToDecimal(Math.Round(ldecRegularBenefitAfterReductionFactor * ldecBenefitOptionFactor, 2));
                lbusBenefitCalculationOptions = GetDisabilityBenefitOptions(busConstant.LUMP_SUM, adecFinalAccruedBenefitAmount, ldecNetRegularBenefitAmount, ldecBenefitOptionFactor);

            }
            #endregion

            #region JAndS
            if (iblnIsQualifiedSpouse)
            {
                if (astrBenefitOptionValue == busConstant.CodeValueAll || astrBenefitOptionValue == busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY)
                {
                    ldecBenefitOptionFactor = GetMPIBenefitOptionFactor(.86M, .005M, .006M);
                    ldecNetRegularBenefitAmount = Convert.ToDecimal(Math.Round(ldecRegularBenefitAfterReductionFactor * ldecBenefitOptionFactor, 2));
                    lbusBenefitCalculationOptions = GetDisabilityBenefitOptions(busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY, adecFinalAccruedBenefitAmount, ldecNetRegularBenefitAmount, ldecBenefitOptionFactor);
                }
                if (this.ibusBenefitApplication.icdoBenefitApplication.retirement_date > busConstant.BenefitCalculation.DISABILITY_JS75_CHECK_DATE)
                {
                    if (astrBenefitOptionValue == busConstant.CodeValueAll || astrBenefitOptionValue == busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY)
                    {
                        ldecBenefitOptionFactor = GetMPIBenefitOptionFactor(.80M, .01M, .006M);
                        ldecNetRegularBenefitAmount = Math.Round(ldecRegularBenefitAfterReductionFactor * ldecBenefitOptionFactor, 2);
                        lbusBenefitCalculationOptions = GetDisabilityBenefitOptions(busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, adecFinalAccruedBenefitAmount, ldecNetRegularBenefitAmount, ldecBenefitOptionFactor);
                    }
                }
                if (astrBenefitOptionValue == busConstant.CodeValueAll || astrBenefitOptionValue == busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY)
                {
                    //Regular
                    //double ldblBenefitOptionFactorJnS100 = 0.75 + 0.01 * Convert.ToInt32(this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement - this.icdoBenefitCalculationHeader.age) +
                    //             0.006 * (65 - Convert.ToInt32(this.icdoBenefitCalculationHeader.age));

                    ldecBenefitOptionFactor = GetMPIBenefitOptionFactor(.75M, .01M, .006M);
                    ldecNetRegularBenefitAmount = Math.Round(ldecRegularBenefitAfterReductionFactor * ldecBenefitOptionFactor, 2);

                    //Disabilty
                    lbusBenefitCalculationOptions = GetDisabilityBenefitOptions(busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY, adecFinalAccruedBenefitAmount, ldecNetRegularBenefitAmount, ldecBenefitOptionFactor);
                }
                if (astrBenefitOptionValue == busConstant.CodeValueAll || astrBenefitOptionValue == busConstant.JOINT_50_PERCENT_POPUP_ANNUITY)
                {

                    //double ldblBenefitOptionFactorJnS50Pop = 0.83 + 0.007 * Convert.ToInt32(this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement - this.icdoBenefitCalculationHeader.age) +
                    //             0.006 * (65 - Convert.ToInt32(this.icdoBenefitCalculationHeader.age));

                    ldecBenefitOptionFactor = GetMPIBenefitOptionFactor(.83M, .007M, .006M);
                    ldecNetRegularBenefitAmount = Math.Round(ldecRegularBenefitAfterReductionFactor * ldecBenefitOptionFactor, 2);

                    lbusBenefitCalculationOptions = GetDisabilityBenefitOptions(busConstant.JOINT_50_PERCENT_POPUP_ANNUITY, adecFinalAccruedBenefitAmount, ldecNetRegularBenefitAmount, ldecBenefitOptionFactor);


                }

                if (astrBenefitOptionValue == busConstant.CodeValueAll || astrBenefitOptionValue == busConstant.JOINT_100_PERCENT_POPUP_ANNUITY)
                {

                    //double ldblBenefitOptionFactorJnS100Pop = 0.71 + 0.01 * Convert.ToInt32(this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement - this.icdoBenefitCalculationHeader.age) +
                    //             0.008 * (65 - Convert.ToInt32(this.icdoBenefitCalculationHeader.age));

                    ldecBenefitOptionFactor = GetMPIBenefitOptionFactor(.71M, .01M, .008M);
                    ldecNetRegularBenefitAmount = Convert.ToDecimal(Math.Round(ldecRegularBenefitAfterReductionFactor * ldecBenefitOptionFactor, 2));

                    lbusBenefitCalculationOptions = GetDisabilityBenefitOptions(busConstant.JOINT_100_PERCENT_POPUP_ANNUITY, adecFinalAccruedBenefitAmount, ldecNetRegularBenefitAmount, ldecBenefitOptionFactor);
                }
            }

            #endregion

            #region TEN
            if (astrBenefitOptionValue == busConstant.CodeValueAll || astrBenefitOptionValue == busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY)
            {

                int lPlanBenefitId = ibusCalculation.GetPlanBenefitId(busConstant.MPIPP_PLAN_ID, busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY);
                ldecBenefitOptionFactor = ibusCalculation.GetBenefitOptionFactor(busConstant.BENEFIT_TYPE_RETIREMENT, lPlanBenefitId,
                                                  Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge), 0);
                ldecBenefitOptionFactor = Math.Round(ldecBenefitOptionFactor, 2);
                ldecNetRegularBenefitAmount = Convert.ToDecimal(Math.Round(ldecRegularBenefitAfterReductionFactor * ldecBenefitOptionFactor, 2));

                lbusBenefitCalculationOptions = GetDisabilityBenefitOptions(busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, adecFinalAccruedBenefitAmount, ldecNetRegularBenefitAmount, ldecBenefitOptionFactor);

            }
            #endregion

            #region LIFE
            if (astrBenefitOptionValue == busConstant.CodeValueAll || astrBenefitOptionValue == busConstant.LIFE_ANNUTIY)
            {
                ldecBenefitOptionFactor = 1;
                ldecNetRegularBenefitAmount = Convert.ToDecimal(Math.Round(ldecReducedBenefitAmount * ldecBenefitOptionFactor, 2));
                lbusBenefitCalculationOptions = GetDisabilityBenefitOptions(busConstant.LIFE_ANNUTIY, adecFinalAccruedBenefitAmount, ldecNetRegularBenefitAmount, ldecBenefitOptionFactor);

            }
            #endregion


        }

        */
        private void CalculateL161BenefitOptionsForRegularAndDisabled(string astrBenefitOption, bool ablnFinalCalc)
        {
            decimal ldecRegularBenefitAfterReductionFactor = this.ibusCalculation.CalculateTotalBenefitAmtForLocal161(this.icdoBenefitCalculationHeader.istrRetirementType,
                                                                    this.ibusBenefitApplication.icdoBenefitApplication.retirement_date, this.ibusPerson.icdoPerson.idtDateofBirth,
                                                                     idecLocalFrozenBenefit,
                                                                     false, this.ibusBenefitApplication, this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == iintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_pre_bis_amount),
                                                                     this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == iintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_post_bis_amount),
                                                                     null, this.iclbBenefitCalculationDetail,
                                                                     Convert.ToInt32(this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == iintPersonAccountId).Max(item => item.icdoPersonAccountRetirementContribution.computational_year)),
                                                                     this.iclbPersonAccountRetirementContribution, ablnFinalCalc, this.ibusPerson.icdoPerson.person_id);

            decimal ldecRegularBenefit = 1;
            decimal ldecRegularFactor = 1;
            // Main Formula : =RoundUp (AccBen*Factor,0)
            //Lump Sum : Not there 
            if (this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoBenefitCalculationDetail.retirement_date == this.ibusBenefitApplication.icdoBenefitApplication.retirement_date).Count() > 0)
            {
                decimal ldecQdroOffSet = this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoBenefitCalculationDetail.retirement_date == this.ibusBenefitApplication.icdoBenefitApplication.retirement_date).FirstOrDefault().icdoBenefitCalculationDetail.qdro_offset;
                idecLocalFrozenBenefit = idecLocalFrozenBenefit - ldecQdroOffSet;
            }

            if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.LUMP_SUM)
            {
                ldecRegularFactor = idecLumpSumFactor;
                ldecRegularBenefit = Math.Ceiling(ldecRegularBenefitAfterReductionFactor * ldecRegularFactor);
                //CalculateL161BenefitOptionsForDiabledPerson(busConstant.LUMP_SUM, idecLocalFrozenBenefit, ldecRegularBenefit, ldecRegularFactor);
                CalculateAllLocalBenefitOptionsForDiabledPerson(busConstant.LUMP_SUM, idecLocalFrozenBenefit, ldecRegularBenefit, ldecRegularFactor);

            }
            if (iblnIsQualifiedSpouse)
            {
                if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY)
                {
                    ldecRegularFactor = 1;
                    ldecRegularFactor = GetBenefitFactorLocal(busConstant.Local_161, busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY, busConstant.BENEFIT_TYPE_RETIREMENT, this.icdoBenefitCalculationHeader.age, this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement);
                    ldecRegularFactor = Math.Round(ldecRegularFactor, 3);
                    ldecRegularBenefit = Math.Ceiling(ldecRegularBenefitAfterReductionFactor * ldecRegularFactor);
                    //CalculateL161BenefitOptionsForDiabledPerson(busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY, idecLocalFrozenBenefit, ldecRegularBenefit, ldecRegularFactor);               
                    CalculateAllLocalBenefitOptionsForDiabledPerson(busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY, idecLocalFrozenBenefit, ldecRegularBenefit, ldecRegularFactor);
                }
                if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.JOINT_50_PERCENT_POPUP_ANNUITY)
                {
                    CalculateAllLocalBenefitOptionsForDiabledPerson(busConstant.JOINT_50_PERCENT_POPUP_ANNUITY, idecLocalFrozenBenefit, 0, 0);
                }
                //Joint & 100% Pop-up Annuity 
                if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.JOINT_100_PERCENT_POPUP_ANNUITY)
                {
                    CalculateAllLocalBenefitOptionsForDiabledPerson(busConstant.JOINT_100_PERCENT_POPUP_ANNUITY, idecLocalFrozenBenefit, 0, 0);
                }

                if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY)
                {
                    CalculateAllLocalBenefitOptionsForDiabledPerson(busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY, idecLocalFrozenBenefit, 0, 0);
                }


                if (this.ibusBenefitApplication.icdoBenefitApplication.retirement_date > busConstant.BenefitCalculation.DISABILITY_JS75_CHECK_DATE)
                {
                    if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY)
                    {
                        ldecRegularFactor = 1;
                        ldecRegularFactor = GetBenefitFactorLocal(busConstant.Local_161, busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, busConstant.BENEFIT_TYPE_RETIREMENT, this.icdoBenefitCalculationHeader.age, this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement);
                        ldecRegularBenefit = Math.Ceiling(ldecRegularBenefitAfterReductionFactor * ldecRegularFactor);
                        //CalculateL161BenefitOptionsForDiabledPerson(busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, idecLocalFrozenBenefit, ldecRegularBenefit, ldecRegularFactor);
                        CalculateAllLocalBenefitOptionsForDiabledPerson(busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, idecLocalFrozenBenefit, ldecRegularBenefit, ldecRegularFactor);

                    }
                }
            }

            //if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY)
            //{
            //    ldecRegularFactor = 1;
            //    ldecRegularBenefit = Math.Ceiling(ldecRegularBenefitAfterReductionFactor * ldecRegularFactor);
            //    //CalculateL161BenefitOptionsForDiabledPerson(busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, idecLocalFrozenBenefit, ldecRegularBenefit, ldecRegularFactor);
            //    CalculateAllLocalBenefitOptionsForDiabledPerson(busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, idecLocalFrozenBenefit, ldecRegularBenefit, ldecRegularFactor);
            //}
            if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY)
            {
                CalculateAllLocalBenefitOptionsForDiabledPerson(busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, idecLocalFrozenBenefit, 0, 0);

            }
            if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.LIFE_ANNUTIY)
            {
                CalculateAllLocalBenefitOptionsForDiabledPerson(busConstant.LIFE_ANNUTIY, idecLocalFrozenBenefit, 0, 0);

            }
        }

        private void CalculateL600BenefitOptionsForRegularAndDisabled(string astrBenefitOption, bool ablnFinalCalc)
        {
            decimal ldecRegularBenefitAfterReductionFactor = this.ibusCalculation.CalculateTotalBenefitAmtForLocal600(this.icdoBenefitCalculationHeader.istrRetirementType,
                                                                     this.ibusBenefitApplication.icdoBenefitApplication.retirement_date, this.ibusPerson.icdoPerson.idtDateofBirth,
                                                                     idecLocalFrozenBenefit,
                                                                     false, this.ibusBenefitApplication, this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == iintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_pre_bis_amount),
                                                                     this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == iintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_post_bis_amount),
                                                                     null, this.iclbBenefitCalculationDetail,
                                                                     Convert.ToInt32(this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == iintPersonAccountId).Max(item => item.icdoPersonAccountRetirementContribution.computational_year)),
                                                                     this.iclbPersonAccountRetirementContribution, ablnFinalCalc, this.ibusPerson.icdoPerson.person_id);



            decimal ldecRegularFactor = 1;
            decimal ldecRegularBenefit = 1;
            if (this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoBenefitCalculationDetail.retirement_date == this.ibusBenefitApplication.icdoBenefitApplication.retirement_date).Count() > 0)
            {
                decimal ldecQdroOffSet = this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoBenefitCalculationDetail.retirement_date == this.ibusBenefitApplication.icdoBenefitApplication.retirement_date).FirstOrDefault().icdoBenefitCalculationDetail.qdro_offset;
                idecLocalFrozenBenefit = idecLocalFrozenBenefit - ldecQdroOffSet;
            }
            //Lump Sum
            if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.LUMP_SUM)
            {
                ldecRegularFactor = idecLumpSumFactor;
                ldecRegularBenefit = Math.Ceiling(ldecRegularBenefitAfterReductionFactor * ldecRegularFactor);
                //CalculateL600BenefitOptionsForDiabledPerson(busConstant.LUMP_SUM, idecLocalFrozenBenefit, ldecRegularBenefit, ldecRegularFactor);
                CalculateAllLocalBenefitOptionsForDiabledPerson(busConstant.LUMP_SUM, idecLocalFrozenBenefit, ldecRegularBenefit, ldecRegularFactor);

            }
            //Qualified Joint & 50% Survivor Annuity 
            if (iblnIsQualifiedSpouse)
            {
                if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY)
                {
                    CalculateAllLocalBenefitOptionsForDiabledPerson(busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY, idecLocalFrozenBenefit, 0, 0);
                }
                if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.JOINT_50_PERCENT_POPUP_ANNUITY)
                {
                    // PIR - 760
                    ldecRegularFactor = ibusCalculation.GetRetirementL600Factor(busConstant.JOINT_50_PERCENT_POPUP_ANNUITY, this.ibusPerson.icdoPerson.idtDateofBirth, this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth);
                    //ldecRegularFactor = GetBenefitOptionFactor(.94M, .005M,idecAgeDiff);
                    ldecRegularBenefit = GetNetBenefitAmount(ldecRegularBenefitAfterReductionFactor, ldecRegularFactor);
                    //CalculateL600BenefitOptionsForDiabledPerson(busConstant.JOINT_50_PERCENT_POPUP_ANNUITY, idecLocalFrozenBenefit, ldecRegularBenefit, ldecRegularFactor);
                    CalculateAllLocalBenefitOptionsForDiabledPerson(busConstant.JOINT_50_PERCENT_POPUP_ANNUITY, idecLocalFrozenBenefit, ldecRegularBenefit, ldecRegularFactor);

                }
                if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY)
                {
                    CalculateAllLocalBenefitOptionsForDiabledPerson(busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY, idecLocalFrozenBenefit, 0, 0);
                }
                if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.JOINT_100_PERCENT_POPUP_ANNUITY)
                {
                    CalculateAllLocalBenefitOptionsForDiabledPerson(busConstant.JOINT_100_PERCENT_POPUP_ANNUITY, idecLocalFrozenBenefit, 0, 0);
                }
                if (this.ibusBenefitApplication.icdoBenefitApplication.retirement_date > busConstant.BenefitCalculation.DISABILITY_JS75_CHECK_DATE)
                {
                    //Joint & 75%  Annuity : Not there 
                    if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY)
                    {
                        // PIR - 760
                        ldecRegularFactor = ibusCalculation.GetRetirementL600Factor(busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, this.ibusPerson.icdoPerson.idtDateofBirth, this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth);

                        //ldecRegularFactor = GetBenefitOptionFactor(.85m, .005m,idecAgeDiff);
                        ldecRegularBenefit = GetNetBenefitAmount(ldecRegularBenefitAfterReductionFactor, ldecRegularFactor);
                        //CalculateL600BenefitOptionsForDiabledPerson(busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, idecLocalFrozenBenefit, ldecRegularBenefit, ldecRegularFactor);
                        CalculateAllLocalBenefitOptionsForDiabledPerson(busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, idecLocalFrozenBenefit, ldecRegularBenefit, ldecRegularFactor);

                    }
                }
            }
            //Ten-Years-Certain and Life Annuity 
            if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY)
            {
                ldecRegularFactor = 1;
                ldecRegularBenefit = GetNetBenefitAmount(ldecRegularBenefitAfterReductionFactor, ldecRegularFactor);
                //CalculateL600BenefitOptionsForDiabledPerson(busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, idecLocalFrozenBenefit, ldecRegularBenefit, ldecRegularFactor);
                CalculateAllLocalBenefitOptionsForDiabledPerson(busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, idecLocalFrozenBenefit, ldecRegularBenefit, ldecRegularFactor);
            }
            if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.LIFE_ANNUTIY)
            {
                CalculateAllLocalBenefitOptionsForDiabledPerson(busConstant.LIFE_ANNUTIY, idecLocalFrozenBenefit, 0, 0);

            }

        }

        private void CalculateL666BenefitOptionsForRegularAndDisabled(string astrBenefitOption, bool ablnFinalCalc)
        {
            decimal ldecRegularBenefitAfterReductionFactor = this.ibusCalculation.CalculateTotalBenefitAmtForLocal666(this.icdoBenefitCalculationHeader.istrRetirementType,
                                                                     this.ibusBenefitApplication.icdoBenefitApplication.retirement_date, this.ibusPerson.icdoPerson.idtDateofBirth,
                                                                     idecLocalFrozenBenefit,
                                                                     false, this.ibusBenefitApplication, this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == iintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_pre_bis_amount),
                                                                     this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == iintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_post_bis_amount), this.icdoBenefitCalculationHeader.idecParticipantFullAge,
                                                                     null,
                                                                     this.iclbBenefitCalculationDetail,
                                                                     Convert.ToInt32(this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == iintPersonAccountId).Max(item => item.icdoPersonAccountRetirementContribution.computational_year)),
                                                                     this.iclbPersonAccountRetirementContribution, ablnFinalCalc, this.ibusPerson.icdoPerson.person_id);



            decimal ldecRegularFactor = 1;
            decimal ldecRegularBenefit = 1;

            if (this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoBenefitCalculationDetail.retirement_date == this.ibusBenefitApplication.icdoBenefitApplication.retirement_date).Count() > 0)
            {
                decimal ldecQdroOffSet = this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoBenefitCalculationDetail.retirement_date == this.ibusBenefitApplication.icdoBenefitApplication.retirement_date).FirstOrDefault().icdoBenefitCalculationDetail.qdro_offset;
                idecLocalFrozenBenefit = idecLocalFrozenBenefit - ldecQdroOffSet;
            }

            //Lump Sum: Not there 
            if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.LUMP_SUM)
            {
                ldecRegularFactor = idecLumpSumFactor;
                ldecRegularBenefit = GetNetBenefitAmount(ldecRegularBenefitAfterReductionFactor, ldecRegularFactor, .5m);
                //CalculateL666BenefitOptionsForDiabledPerson(busConstant.LUMP_SUM, idecLocalFrozenBenefit, ldecRegularBenefit, ldecRegularFactor);
                CalculateAllLocalBenefitOptionsForDiabledPerson(busConstant.LUMP_SUM, idecLocalFrozenBenefit, ldecRegularBenefit, ldecRegularFactor);

            }
            if (iblnIsQualifiedSpouse)
            {
                //Joint & 50% Pop-up Annuity 
                if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.JOINT_50_PERCENT_POPUP_ANNUITY)
                {
                    // PIR - 760
                    ldecRegularFactor = ibusCalculation.GetRetirementL666Factor(busConstant.JOINT_50_PERCENT_POPUP_ANNUITY, this.ibusPerson.icdoPerson.idtDateofBirth, this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth);
                    //ldecRegularFactor = GetBenefitOptionFactor(.89m, .004m,idecAgeDiff);
                    ldecRegularBenefit = GetNetBenefitAmount(ldecRegularBenefitAfterReductionFactor, ldecRegularFactor, .5m);
                    //CalculateL666BenefitOptionsForDiabledPerson(busConstant.JOINT_50_PERCENT_POPUP_ANNUITY, idecLocalFrozenBenefit, ldecRegularBenefit, ldecRegularFactor);
                    CalculateAllLocalBenefitOptionsForDiabledPerson(busConstant.JOINT_50_PERCENT_POPUP_ANNUITY, idecLocalFrozenBenefit, ldecRegularBenefit, ldecRegularFactor);

                }
                if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY)
                {
                    CalculateAllLocalBenefitOptionsForDiabledPerson(busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY, idecLocalFrozenBenefit, 0, 0);
                }
                if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY)
                {
                    CalculateAllLocalBenefitOptionsForDiabledPerson(busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY, idecLocalFrozenBenefit, 0, 0);
                }
                if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.JOINT_100_PERCENT_POPUP_ANNUITY)
                {
                    CalculateAllLocalBenefitOptionsForDiabledPerson(busConstant.JOINT_100_PERCENT_POPUP_ANNUITY, idecLocalFrozenBenefit, 0, 0);
                }
                if (this.ibusBenefitApplication.icdoBenefitApplication.retirement_date > busConstant.BenefitCalculation.DISABILITY_JS75_CHECK_DATE)
                {
                    //Joint & 75%  Annuity: Not there 
                    if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY)
                    {
                        // PIR - 760
                        ldecRegularFactor = ibusCalculation.GetRetirementL666Factor(busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, this.ibusPerson.icdoPerson.idtDateofBirth, this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth);

                        //ldecRegularFactor = GetBenefitOptionFactor(.81m, .004m,idecAgeDiff);
                        ldecRegularBenefit = GetNetBenefitAmount(ldecRegularBenefitAfterReductionFactor, ldecRegularFactor, .5m);
                        //CalculateL666BenefitOptionsForDiabledPerson(busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, idecLocalFrozenBenefit, ldecRegularBenefit, ldecRegularFactor);
                        CalculateAllLocalBenefitOptionsForDiabledPerson(busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, idecLocalFrozenBenefit, ldecRegularBenefit, ldecRegularFactor);
                    }
                }
            }
            //Three-Years-Certain and Life Annuity
            //if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY)
            //{
            //    ldecRegularFactor = 1;
            //    ldecRegularBenefit = GetNetBenefitAmount(ldecRegularBenefitAfterReductionFactor, ldecRegularFactor, .5m);
            //   // CalculateL666BenefitOptionsForDiabledPerson(busConstant.THREE_YEARS_CERTAIN_AND_LIFE_ANNUITY, idecLocalFrozenBenefit, ldecRegularBenefit, ldecRegularFactor);
            //    CalculateAllLocalBenefitOptionsForDiabledPerson(busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, idecLocalFrozenBenefit, ldecRegularBenefit, ldecRegularFactor);
            //}
            if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY)
            {
                CalculateAllLocalBenefitOptionsForDiabledPerson(busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, idecLocalFrozenBenefit, 0, 0);

            }
            if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.LIFE_ANNUTIY)
            {
                CalculateAllLocalBenefitOptionsForDiabledPerson(busConstant.LIFE_ANNUTIY, idecLocalFrozenBenefit, 0, 0);

            }
        }

        private void CalculateL52BenefitOptionsForRegularAndDisabled(string astrBenefitOption, bool ablnFinalCalc)
        {
            decimal ldecRegularBenefitAfterReductionFactor = this.ibusCalculation.CalculateTotalBenefitAmtForLocal52(this.icdoBenefitCalculationHeader.istrRetirementType,
                                                                     this.ibusBenefitApplication.icdoBenefitApplication.retirement_date, this.ibusPerson.icdoPerson.idtDateofBirth,
                                                                     idecLocalFrozenBenefit,
                                                                     false, this.ibusBenefitApplication, this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == iintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_pre_bis_amount),
                                                                     this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == iintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_post_bis_amount),
                                                                     null, this.iclbBenefitCalculationDetail,
                                                                     Convert.ToInt32(this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == iintPersonAccountId).Max(item => item.icdoPersonAccountRetirementContribution.computational_year)),
                                                                     this.iclbPersonAccountRetirementContribution, ablnFinalCalc, this.ibusPerson.icdoPerson.person_id);

            decimal ldecRegularFactor = 1;
            decimal ldecRegularBenefit = 1;
            ////PIR 371
            //decimal ldecDiffAgeFactor = 0;
            //if (this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth > this.ibusPerson.icdoPerson.idtDateofBirth)
            //{
            //    ldecDiffAgeFactor = busGlobalFunctions.CalculatePersonAge(this.ibusPerson.icdoPerson.idtDateofBirth, this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth);
            //}
            //else
            //{
            //    ldecDiffAgeFactor = busGlobalFunctions.CalculatePersonAge(this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth, this.ibusPerson.icdoPerson.idtDateofBirth) * -1;
            //}
            if (this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoBenefitCalculationDetail.retirement_date == this.ibusBenefitApplication.icdoBenefitApplication.retirement_date).Count() > 0)
            {
                decimal ldecQdroOffSet = this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoBenefitCalculationDetail.retirement_date == this.ibusBenefitApplication.icdoBenefitApplication.retirement_date).FirstOrDefault().icdoBenefitCalculationDetail.qdro_offset;
                idecLocalFrozenBenefit = idecLocalFrozenBenefit - ldecQdroOffSet;
            }
            //Lump Sum
            if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.LUMP_SUM)
            {
                ldecRegularFactor = idecLumpSumFactor;
                ldecRegularBenefit = GetNetBenefitAmount(ldecRegularBenefitAfterReductionFactor, ldecRegularFactor);
                //CalculateL52BenefitOptionsForDiabledPerson(busConstant.LUMP_SUM, idecLocalFrozenBenefit, ldecRegularBenefit, ldecRegularFactor);
                CalculateAllLocalBenefitOptionsForDiabledPerson(busConstant.LUMP_SUM, idecLocalFrozenBenefit, ldecRegularBenefit, ldecRegularFactor);

            }
            //Life Annuity 
            if (iblnIsQualifiedSpouse)
            {
                //Joint & 50% Pop-up Annuity 
                if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.JOINT_50_PERCENT_POPUP_ANNUITY)
                {
                    // PIR - 371
                    ldecRegularFactor = ibusCalculation.GetRetirementL52Factor(busConstant.JOINT_50_PERCENT_POPUP_ANNUITY, this.ibusPerson.icdoPerson.idtDateofBirth, this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth);
                    ldecRegularBenefit = GetNetBenefitAmount(ldecRegularBenefitAfterReductionFactor, ldecRegularFactor);
                    // CalculateL52BenefitOptionsForDiabledPerson(busConstant.JOINT_50_PERCENT_POPUP_ANNUITY, idecLocalFrozenBenefit, ldecRegularBenefit, ldecRegularFactor);
                    CalculateAllLocalBenefitOptionsForDiabledPerson(busConstant.JOINT_50_PERCENT_POPUP_ANNUITY, idecLocalFrozenBenefit, ldecRegularBenefit, ldecRegularFactor);

                }
                if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY)
                {
                    CalculateAllLocalBenefitOptionsForDiabledPerson(busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY, idecLocalFrozenBenefit, 0, 0);
                }
                if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY)
                {
                    CalculateAllLocalBenefitOptionsForDiabledPerson(busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY, idecLocalFrozenBenefit, 0, 0);
                }
                if (this.ibusBenefitApplication.icdoBenefitApplication.retirement_date > busConstant.BenefitCalculation.DISABILITY_JS75_CHECK_DATE)
                {
                    //Joint & 75%  Annuity :Not there
                    if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY)
                    {
                        // PIR - 371
                        ldecRegularFactor = ibusCalculation.GetRetirementL52Factor(busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, this.ibusPerson.icdoPerson.idtDateofBirth, this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth);
                        ldecRegularBenefit = GetNetBenefitAmount(ldecRegularBenefitAfterReductionFactor, ldecRegularFactor);
                        //CalculateL52BenefitOptionsForDiabledPerson(busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, idecLocalFrozenBenefit, ldecRegularBenefit, ldecRegularFactor);
                        CalculateAllLocalBenefitOptionsForDiabledPerson(busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, idecLocalFrozenBenefit, ldecRegularBenefit, ldecRegularFactor);

                    }
                }
                //Joint & 100% Pop-up Annuity 
                //Ten-Years-Certain and Life Annuity 
                if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.JOINT_100_PERCENT_POPUP_ANNUITY)
                {
                    // PIR - 371
                    ldecRegularFactor = ibusCalculation.GetRetirementL52Factor(busConstant.JOINT_100_PERCENT_POPUP_ANNUITY, this.ibusPerson.icdoPerson.idtDateofBirth, this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth);

                    //ldecRegularFactor = GetBenefitOptionFactor(.85m, .006m,idecAgeDiff);
                    ldecRegularBenefit = GetNetBenefitAmount(ldecRegularBenefitAfterReductionFactor, ldecRegularFactor);
                    //CalculateL52BenefitOptionsForDiabledPerson(busConstant.JOINT_100_PERCENT_POPUP_ANNUITY, idecLocalFrozenBenefit, ldecRegularBenefit, ldecRegularFactor);
                    CalculateAllLocalBenefitOptionsForDiabledPerson(busConstant.JOINT_100_PERCENT_POPUP_ANNUITY, idecLocalFrozenBenefit, ldecRegularBenefit, ldecRegularFactor);

                }
            }
            if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY)
            {
                ldecRegularFactor = 1;
                ldecRegularBenefit = GetNetBenefitAmount(ldecRegularBenefitAfterReductionFactor, ldecRegularFactor);
                //CalculateL52BenefitOptionsForDiabledPerson(busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, idecLocalFrozenBenefit, ldecRegularBenefit, ldecRegularFactor);
                CalculateAllLocalBenefitOptionsForDiabledPerson(busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, idecLocalFrozenBenefit, ldecRegularBenefit, ldecRegularFactor);

            }
            if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.LIFE_ANNUTIY)
            {
                ldecRegularFactor = 1;
                ldecRegularBenefit = GetNetBenefitAmount(ldecRegularBenefitAfterReductionFactor, ldecRegularFactor);
                //CalculateL52BenefitOptionsForDiabledPerson(busConstant.LIFE_ANNUTIY, idecLocalFrozenBenefit, ldecRegularBenefit, ldecRegularFactor);
                CalculateAllLocalBenefitOptionsForDiabledPerson(busConstant.LIFE_ANNUTIY, idecLocalFrozenBenefit, ldecRegularBenefit, ldecRegularFactor);

            }
        }

        private void CalculateL700BenefitOptionsForRegularAndDisabled(string astrBenefitOption, bool ablnFinalCalc)
        {
            decimal ldecRegularBenefitAfterReductionFactor = this.ibusCalculation.CalculateTotalBenefitAmtForLocal700(this.icdoBenefitCalculationHeader.istrRetirementType,
                                                                     this.ibusBenefitApplication.icdoBenefitApplication.retirement_date, this.ibusPerson.icdoPerson.idtDateofBirth,
                                                                     idecLocalFrozenBenefit,
                                                                     false, this.ibusBenefitApplication, this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == iintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_pre_bis_amount),
                                                                     this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == iintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_post_bis_amount),
                                                                     null, this.iclbBenefitCalculationDetail,
                                                                     Convert.ToInt32(this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == iintPersonAccountId).Max(item => item.icdoPersonAccountRetirementContribution.computational_year)),
                                                                     this.iclbPersonAccountRetirementContribution, ablnFinalCalc, this.ibusPerson.icdoPerson.person_id);



            decimal ldecRegularBenefit = 1;
            decimal ldecRegularFactor = 1;

            if (this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoBenefitCalculationDetail.retirement_date == this.ibusBenefitApplication.icdoBenefitApplication.retirement_date).Count() > 0)
            {
                decimal ldecQdroOffSet = this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoBenefitCalculationDetail.retirement_date == this.ibusBenefitApplication.icdoBenefitApplication.retirement_date).FirstOrDefault().icdoBenefitCalculationDetail.qdro_offset;
                idecLocalFrozenBenefit = idecLocalFrozenBenefit - ldecQdroOffSet;
            }

            //Lump Sum 
            if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.LUMP_SUM)
            {
                ldecRegularFactor = idecLumpSumFactor;
                ldecRegularBenefit = GetNetBenefitAmount(ldecRegularBenefitAfterReductionFactor, ldecRegularFactor, .5m);
                //CalculateL700BenefitOptionsForDiabledPerson(busConstant.LUMP_SUM, idecLocalFrozenBenefit, ldecRegularBenefit, ldecRegularFactor);
                CalculateAllLocalBenefitOptionsForDiabledPerson(busConstant.LUMP_SUM, idecLocalFrozenBenefit, ldecRegularBenefit, ldecRegularFactor);


            }
            if (iblnIsQualifiedSpouse)
            {
                //Qualified Joint & 50% Survivor Annuity 
                if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY)
                {
                    ldecRegularFactor = GetBenefitFactorLocal(busConstant.LOCAL_700, busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY, busConstant.BENEFIT_TYPE_RETIREMENT, this.icdoBenefitCalculationHeader.idecParticipantFullAge, this.icdoBenefitCalculationHeader.idecSurvivorFullAge);
                    ldecRegularBenefit = GetNetBenefitAmount(ldecRegularBenefitAfterReductionFactor, ldecRegularFactor, .5m);
                    //CalculateL700BenefitOptionsForDiabledPerson(busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY, idecLocalFrozenBenefit, ldecRegularBenefit, ldecRegularFactor);
                    CalculateAllLocalBenefitOptionsForDiabledPerson(busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY, idecLocalFrozenBenefit, ldecRegularBenefit, ldecRegularFactor);

                }
                if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.JOINT_50_PERCENT_POPUP_ANNUITY)
                {
                    CalculateAllLocalBenefitOptionsForDiabledPerson(busConstant.JOINT_50_PERCENT_POPUP_ANNUITY, idecLocalFrozenBenefit, 0, 0);
                }

                if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.JOINT_100_PERCENT_POPUP_ANNUITY)
                {
                    CalculateAllLocalBenefitOptionsForDiabledPerson(busConstant.JOINT_100_PERCENT_POPUP_ANNUITY, idecLocalFrozenBenefit, 0, 0);
                }

                //Joint & 66 2/3% Survivor Annuity 
                //if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.JOINT_66_2by3_PERCENT_SURVIVOR_ANNUITY)
                //{
                //    ldecRegularFactor = GetBenefitFactorLocal(busConstant.LOCAL_700, busConstant.JOINT_66_2by3_PERCENT_SURVIVOR_ANNUITY, busConstant.BENEFIT_TYPE_RETIREMENT, this.icdoBenefitCalculationHeader.idecParticipantFullAge, this.icdoBenefitCalculationHeader.idecSurvivorFullAge);
                //    ldecRegularBenefit = GetNetBenefitAmount(ldecRegularBenefitAfterReductionFactor, ldecRegularFactor, .5m);
                //    //CalculateL700BenefitOptionsForDiabledPerson(busConstant.JOINT_66_2by3_PERCENT_SURVIVOR_ANNUITY, idecLocalFrozenBenefit, ldecRegularBenefit, ldecRegularFactor);
                //    CalculateAllLocalBenefitOptionsForDiabledPerson(busConstant.LUMP_SUM, idecLocalFrozenBenefit, ldecRegularBenefit, ldecRegularFactor);

                //}
                //Joint & 100% Survivor Annuity
                if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY)
                {
                    ldecRegularFactor = GetBenefitFactorLocal(busConstant.LOCAL_700, busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY, busConstant.BENEFIT_TYPE_RETIREMENT, this.icdoBenefitCalculationHeader.idecParticipantFullAge, this.icdoBenefitCalculationHeader.idecSurvivorFullAge);
                    ldecRegularBenefit = GetNetBenefitAmount(ldecRegularBenefitAfterReductionFactor, ldecRegularFactor, .5m);
                    //CalculateL700BenefitOptionsForDiabledPerson(busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY, idecLocalFrozenBenefit, ldecRegularBenefit, ldecRegularFactor);
                    CalculateAllLocalBenefitOptionsForDiabledPerson(busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY, idecLocalFrozenBenefit, ldecRegularBenefit, ldecRegularFactor);

                }
                if (this.ibusBenefitApplication.icdoBenefitApplication.retirement_date > busConstant.BenefitCalculation.DISABILITY_JS75_CHECK_DATE)
                {
                    //Joint & 75%  Annuity : Not there 
                    if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY)
                    {
                        ldecRegularFactor = GetBenefitFactorLocal(busConstant.LOCAL_700, busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, busConstant.BENEFIT_TYPE_RETIREMENT, this.icdoBenefitCalculationHeader.idecParticipantFullAge, this.icdoBenefitCalculationHeader.idecSurvivorFullAge);
                        ldecRegularBenefit = GetNetBenefitAmount(ldecRegularBenefitAfterReductionFactor, ldecRegularFactor, .5m);
                        //CalculateL700BenefitOptionsForDiabledPerson(busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, idecLocalFrozenBenefit, ldecRegularBenefit, ldecRegularFactor);
                        CalculateAllLocalBenefitOptionsForDiabledPerson(busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, idecLocalFrozenBenefit, ldecRegularBenefit, ldecRegularFactor);

                    }
                }
            }
            if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.LIFE_ANNUTIY)
            {
                ldecRegularFactor = 1;
                ldecRegularBenefit = GetNetBenefitAmount(ldecRegularBenefitAfterReductionFactor, ldecRegularFactor, .5m);
                CalculateAllLocalBenefitOptionsForDiabledPerson(busConstant.LIFE_ANNUTIY, idecLocalFrozenBenefit, ldecRegularBenefit, ldecRegularFactor);
            }

            if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY)
            {
                ldecRegularFactor = GetBenefitFactorLocal(busConstant.LOCAL_700, busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, busConstant.BENEFIT_TYPE_RETIREMENT, this.icdoBenefitCalculationHeader.idecParticipantFullAge);
                ldecRegularBenefit = GetNetBenefitAmount(ldecRegularBenefitAfterReductionFactor, ldecRegularFactor, .5m);
                CalculateAllLocalBenefitOptionsForDiabledPerson(busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, idecLocalFrozenBenefit, ldecRegularBenefit, ldecRegularFactor);
            }
            //Two-Years-Certain and Life Annuity
            //if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.TWO_YEARS_CERTAIN_AND_LIFE_ANNUITY)
            //{
            //    ldecRegularFactor = GetBenefitFactorLocal(busConstant.LOCAL_700, busConstant.TWO_YEARS_CERTAIN_AND_LIFE_ANNUITY, busConstant.BENEFIT_TYPE_RETIREMENT, this.icdoBenefitCalculationHeader.idecParticipantFullAge, this.icdoBenefitCalculationHeader.idecSurvivorFullAge);
            //    ldecRegularBenefit = GetNetBenefitAmount(ldecRegularBenefitAfterReductionFactor, ldecRegularFactor, .5m);
            //    CalculateL700BenefitOptionsForDiabledPerson(busConstant.TWO_YEARS_CERTAIN_AND_LIFE_ANNUITY, idecLocalFrozenBenefit, ldecRegularBenefit, ldecRegularFactor);
            //}
        }

        #endregion

        #region OptionDisabilty

        private void CalculateAllLocalBenefitOptionsForDiabledPerson(string astrBenefitOption, decimal adecTotalAccuredBenefit, decimal adecRegularBenefit, decimal adecRegularFactor)
        {
            this.ibusCalculation.ProcessQDROOffset(this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId && item.icdoBenefitCalculationDetail.retirement_date == this.icdoBenefitCalculationHeader.retirement_date).First(), this.ibusPerson.icdoPerson.person_id, ref adecTotalAccuredBenefit);
            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId && item.icdoBenefitCalculationDetail.retirement_date == this.icdoBenefitCalculationHeader.retirement_date)
                            .First().icdoBenefitCalculationDetail.idecBenefitsAfterQDROOffSet = adecTotalAccuredBenefit;

            busBenefitCalculationOptions lbusBenefitCalculationOptions = null;
            //Lump Sum
            if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.LUMP_SUM)
            {
                GetDisabilityBenefitOptions(busConstant.LUMP_SUM, adecTotalAccuredBenefit, adecRegularBenefit, adecRegularFactor);
            }
            if (iblnIsQualifiedSpouse)
            {
                if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY)
                {
                    lbusBenefitCalculationOptions = this.GetDisabilityBenefitOptions(busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY, adecTotalAccuredBenefit, adecRegularBenefit, adecRegularFactor);
                }
                if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY)
                {
                    lbusBenefitCalculationOptions = this.GetDisabilityBenefitOptions(busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY, adecTotalAccuredBenefit, adecRegularBenefit, adecRegularFactor);
                }
                //Joint & 50% Pop-up Annuity 

                if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.JOINT_50_PERCENT_POPUP_ANNUITY)
                {
                    GetDisabilityBenefitOptions(busConstant.JOINT_50_PERCENT_POPUP_ANNUITY, adecTotalAccuredBenefit, adecRegularBenefit, adecRegularFactor);
                }
                if (this.ibusBenefitApplication.icdoBenefitApplication.retirement_date > busConstant.BenefitCalculation.DISABILITY_JS75_CHECK_DATE)
                {
                    if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY)
                    {
                        GetDisabilityBenefitOptions(busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, adecTotalAccuredBenefit, adecRegularBenefit, adecRegularFactor);
                    }
                }
                //Joint & 100% Pop-up Annuity 
                if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.JOINT_100_PERCENT_POPUP_ANNUITY)
                {
                    GetDisabilityBenefitOptions(busConstant.JOINT_100_PERCENT_POPUP_ANNUITY, adecTotalAccuredBenefit, adecRegularBenefit, adecRegularFactor);
                }
            }
            //Ten-Years-Certain and Life Annuity 
            if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY)
            {
                GetDisabilityBenefitOptions(busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, adecTotalAccuredBenefit, adecRegularBenefit, adecRegularFactor);
            }
            if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.LIFE_ANNUTIY)
            {
                GetDisabilityBenefitOptions(busConstant.LIFE_ANNUTIY, adecTotalAccuredBenefit, adecRegularBenefit, adecRegularFactor);

            }

        }

        #region OLD_Local_Code_With_Old_Options

        /*
        private void CalculateL600BenefitOptionsForDiabledPerson(string astrBenefitOption, decimal adecTotalAccuredBenefit,decimal adecRegularBenefit, decimal adecRegularFactor)
        {
            decimal ldecDisabilityBenOptionFactor = new decimal();
            decimal ldecDisBenefitAnount = new decimal();
            busBenefitCalculationOptions lbusBenefitCalculationOptions = null;
            // Main Formula : =RoundUp (AccBen*Factor,0)
            //Lump Sum : Not there 
            if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.LUMP_SUM)
            {
                //if (this.icdoBenefitCalculationHeader.istrRetirementType != busConstant.RETIREMENT_TYPE_LATE)
                //{
                     GetDisabilityBenefitOptions(busConstant.LUMP_SUM,adecTotalAccuredBenefit,adecRegularBenefit,adecRegularFactor);
                    //ldecDisabilityBenOptionFactor = idecLumpSumFactor;
                    //ldecDisBenefitAnount = GetNetBenefitAmount(adecTotalAccuredBenefit, idecLumpSumFactor);
                //}
                //lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                //lbusBenefitCalculationOptions = GetLocalDisabilityBenefitOptions(ldecDisabilityBenOptionFactor, ldecDisBenefitAnount, adecRegularBenefit, adecRegularFactor, busConstant.LUMP_SUM);

                //this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);

            }
            if (iblnIsQualifiedSpouse)
            {
                //Joint & 50% Pop-up Annuity =MIN(SUM(0.775-(AgeDiff*0.004)),0.99)
                if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.JOINT_50_PERCENT_POPUP_ANNUITY)
                {
                    GetDisabilityBenefitOptions(busConstant.JOINT_50_PERCENT_POPUP_ANNUITY, adecTotalAccuredBenefit, adecRegularBenefit, adecRegularFactor);
                    //if (this.icdoBenefitCalculationHeader.istrRetirementType != busConstant.RETIREMENT_TYPE_LATE)
                    //{
                    //    ldecDisabilityBenOptionFactor = ibusCalculation.GetBenefitOptionFactor(.775M, .004M, idecAgeDiff);
                    //    ldecDisBenefitAnount = GetNetBenefitAmount(adecTotalAccuredBenefit, ldecDisabilityBenOptionFactor);
                    //}
                    //lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                    //lbusBenefitCalculationOptions = GetLocalDisabilityBenefitOptions(ldecDisabilityBenOptionFactor, ldecDisBenefitAnount, adecRegularBenefit, adecRegularFactor, busConstant.JOINT_50_PERCENT_POPUP_ANNUITY);

                    //this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);

                }
                if (this.ibusBenefitApplication.icdoBenefitApplication.retirement_date > busConstant.BenefitCalculation.DISABILITY_JS75_CHECK_DATE)
                {
                    //Joint & 75%  Annuity : Not there '=MIN(SUM(0.69-(AgeDiff*0.005)),0.99)
                    if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY)
                    {
                        GetDisabilityBenefitOptions(busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, adecTotalAccuredBenefit, adecRegularBenefit, adecRegularFactor);

                        //if (this.icdoBenefitCalculationHeader.istrRetirementType != busConstant.RETIREMENT_TYPE_LATE)
                        //{
                        //    ldecDisabilityBenOptionFactor = ibusCalculation.GetBenefitOptionFactor(.69M, .005M, idecAgeDiff);
                        //    ldecDisBenefitAnount = GetNetBenefitAmount(adecTotalAccuredBenefit, ldecDisabilityBenOptionFactor);
                        //}
                        //lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                        //lbusBenefitCalculationOptions = GetLocalDisabilityBenefitOptions(ldecDisabilityBenOptionFactor, ldecDisBenefitAnount, adecRegularBenefit, adecRegularFactor, busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY);

                        //this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);

                    }
                }
            }
            //Ten-Years-Certain and Life Annuity=  100.0%
            if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY)
            {
                GetDisabilityBenefitOptions(busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, adecTotalAccuredBenefit, adecRegularBenefit, adecRegularFactor);

                //if (this.icdoBenefitCalculationHeader.istrRetirementType != busConstant.RETIREMENT_TYPE_LATE)
                //{
                //    ldecDisabilityBenOptionFactor = 1;
                //    ldecDisBenefitAnount = GetNetBenefitAmount(adecTotalAccuredBenefit, ldecDisabilityBenOptionFactor);
                //}
                //lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                //lbusBenefitCalculationOptions = GetLocalDisabilityBenefitOptions(ldecDisabilityBenOptionFactor, ldecDisBenefitAnount, adecRegularBenefit, adecRegularFactor, busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY);

                //this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);

            }

        }

        private void CalculateL161BenefitOptionsForDiabledPerson(string astrBenefitOption, decimal adecTotalAccuredBenefit, decimal adecRegularBenefit, decimal adecRegularFactor)
        {
            decimal ldecDisabilityBenOptionFactor = new decimal();
            decimal ldecDisBenefitAnount = new decimal();
            ldecDisabilityBenOptionFactor = adecRegularFactor;

            busBenefitCalculationOptions lbusBenefitCalculationOptions = null;
            //Lump Sum
            if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.LUMP_SUM)
            {
                GetDisabilityBenefitOptions(busConstant.LUMP_SUM, adecTotalAccuredBenefit, adecRegularBenefit, adecRegularFactor);
                //if (this.icdoBenefitCalculationHeader.istrRetirementType != busConstant.RETIREMENT_TYPE_LATE)
                //{
                //    ldecDisabilityBenOptionFactor = idecLumpSumFactor;
                //    ldecDisBenefitAnount = Math.Round(adecTotalAccuredBenefit * ldecDisabilityBenOptionFactor, 2);
                //}
                //lbusBenefitCalculationOptions = GetLocalDisabilityBenefitOptions(ldecDisabilityBenOptionFactor, ldecDisBenefitAnount, adecRegularBenefit, adecRegularFactor, busConstant.LUMP_SUM);

                //this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);

            }
            if (iblnIsQualifiedSpouse)
            {
                //Qualified Joint & 50% Survivor Annuity 
                if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY)
                {
                    GetDisabilityBenefitOptions(busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY, adecTotalAccuredBenefit, adecRegularBenefit, adecRegularFactor);
                    //if (this.icdoBenefitCalculationHeader.istrRetirementType != busConstant.RETIREMENT_TYPE_LATE)
                    //{
                    //    if (adecRegularFactor == 0)
                    //    {
                    //        ldecDisabilityBenOptionFactor = GetBenefitFactorLocal(busConstant.Local_161, busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY, busConstant.BENEFIT_TYPE_RETIREMENT, this.icdoBenefitCalculationHeader.age, this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement);
                    //    }

                    //    ldecDisBenefitAnount = Math.Round(adecTotalAccuredBenefit * ldecDisabilityBenOptionFactor, 2);
                    //}
                    //lbusBenefitCalculationOptions = GetLocalDisabilityBenefitOptions(ldecDisabilityBenOptionFactor, ldecDisBenefitAnount, adecRegularBenefit, adecRegularFactor, busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY);

                    //this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);

                }
                if (this.ibusBenefitApplication.icdoBenefitApplication.retirement_date > busConstant.BenefitCalculation.DISABILITY_JS75_CHECK_DATE)
                {
                    //Joint & 75%  Annuity : Not there 
                    if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY)
                    {
                        GetDisabilityBenefitOptions(busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, adecTotalAccuredBenefit, adecRegularBenefit, adecRegularFactor);
                        //if (this.icdoBenefitCalculationHeader.istrRetirementType != busConstant.RETIREMENT_TYPE_LATE)
                        //{
                        //    if (adecRegularFactor == 0)
                        //    {
                        //        ldecDisabilityBenOptionFactor = GetBenefitFactorLocal(busConstant.Local_161, busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, busConstant.BENEFIT_TYPE_RETIREMENT, this.icdoBenefitCalculationHeader.age, this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement);
                        //    }
                        //}
                        //ldecDisBenefitAnount = Math.Round(adecTotalAccuredBenefit * ldecDisabilityBenOptionFactor, 2);
                        //lbusBenefitCalculationOptions = GetLocalDisabilityBenefitOptions(ldecDisabilityBenOptionFactor, ldecDisBenefitAnount, adecRegularBenefit, adecRegularFactor, busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY);
                        //this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
                    }
                }
            }
            //Five-Years-Certain and Life Annuity 
            if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.FIVE_YEARS_CERTAIN_AND_LIFE_ANNUTIY)
            {
                GetDisabilityBenefitOptions(busConstant.FIVE_YEARS_CERTAIN_AND_LIFE_ANNUTIY, adecTotalAccuredBenefit, adecRegularBenefit, adecRegularFactor);
                //if (this.icdoBenefitCalculationHeader.istrRetirementType != busConstant.RETIREMENT_TYPE_LATE)
                //{
                //    if (adecRegularFactor == 0)
                //    {
                //        ldecDisabilityBenOptionFactor = 1;
                //    }
                //    ldecDisBenefitAnount = Math.Round(adecTotalAccuredBenefit * ldecDisabilityBenOptionFactor, 2);
                //}
                //lbusBenefitCalculationOptions = GetLocalDisabilityBenefitOptions(ldecDisabilityBenOptionFactor, ldecDisBenefitAnount, adecRegularBenefit, adecRegularFactor, busConstant.FIVE_YEARS_CERTAIN_AND_LIFE_ANNUTIY);

                //this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);

            }
           
        }

        private void CalculateL666BenefitOptionsForDiabledPerson(string astrBenefitOption, decimal adecTotalAccuredBenefit, decimal adecRegularBenefit, decimal adecRegularFactor)
        {
            decimal ldecDisabilityBenOptionFactor = new decimal();
            decimal ldecDisBenefitAnount = new decimal();
            busBenefitCalculationOptions lbusBenefitCalculationOptions = null;
            //Lump Sum: Not there 
            if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.LUMP_SUM)
            {
                GetDisabilityBenefitOptions(busConstant.LUMP_SUM, adecTotalAccuredBenefit, adecRegularBenefit, adecRegularFactor);

                //if (this.icdoBenefitCalculationHeader.istrRetirementType != busConstant.RETIREMENT_TYPE_LATE)
                //{
                //    ldecDisabilityBenOptionFactor = idecLumpSumFactor;
                //    ldecDisBenefitAnount = Math.Round(adecTotalAccuredBenefit * ldecDisabilityBenOptionFactor, 2);
                //}

                //lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                //lbusBenefitCalculationOptions = GetLocalDisabilityBenefitOptions(ldecDisabilityBenOptionFactor, ldecDisBenefitAnount, adecRegularBenefit, adecRegularFactor, busConstant.LUMP_SUM);

                //this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);

            }
            if (iblnIsQualifiedSpouse)
            {
                //Joint & 50% Pop-up Annuity 
                if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.JOINT_50_PERCENT_POPUP_ANNUITY)
                {
                    GetDisabilityBenefitOptions(busConstant.JOINT_50_PERCENT_POPUP_ANNUITY, adecTotalAccuredBenefit, adecRegularBenefit, adecRegularFactor);

                    //if (this.icdoBenefitCalculationHeader.istrRetirementType != busConstant.RETIREMENT_TYPE_LATE)
                    //{
                    //    ldecDisabilityBenOptionFactor = ibusCalculation.GetBenefitOptionFactor(.79M, .004M, idecAgeDiff);
                    //    ldecDisBenefitAnount = GetNetBenefitAmount(adecTotalAccuredBenefit, ldecDisabilityBenOptionFactor);
                    //}
                    //lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                    //lbusBenefitCalculationOptions = GetLocalDisabilityBenefitOptions(ldecDisabilityBenOptionFactor, ldecDisBenefitAnount, adecRegularBenefit, adecRegularFactor, busConstant.JOINT_50_PERCENT_POPUP_ANNUITY);

                    //this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);

                }
                if (this.ibusBenefitApplication.icdoBenefitApplication.retirement_date > busConstant.BenefitCalculation.DISABILITY_JS75_CHECK_DATE)
                {
                    //Joint&75%  Annuity: Notthere 
                    if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY)
                    {
                        GetDisabilityBenefitOptions(busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, adecTotalAccuredBenefit, adecRegularBenefit, adecRegularFactor);

                        //if (this.icdoBenefitCalculationHeader.istrRetirementType != busConstant.RETIREMENT_TYPE_LATE)
                        //{
                        //    ldecDisabilityBenOptionFactor = ibusCalculation.GetBenefitOptionFactor(.71M, .005M, idecAgeDiff);
                        //    ldecDisBenefitAnount = GetNetBenefitAmount(adecTotalAccuredBenefit, ldecDisabilityBenOptionFactor);
                        //}
                        //lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                        //lbusBenefitCalculationOptions = GetLocalDisabilityBenefitOptions(ldecDisabilityBenOptionFactor, ldecDisBenefitAnount, adecRegularBenefit, adecRegularFactor, busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY);

                        //this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
                    }
                }
            }
            //Three-Years-Certain and Life Annuity
            if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.THREE_YEARS_CERTAIN_AND_LIFE_ANNUITY)
            {
                GetDisabilityBenefitOptions(busConstant.THREE_YEARS_CERTAIN_AND_LIFE_ANNUITY, adecTotalAccuredBenefit, adecRegularBenefit, adecRegularFactor);

                //if (this.icdoBenefitCalculationHeader.istrRetirementType != busConstant.RETIREMENT_TYPE_LATE)
                //{
                //    ldecDisabilityBenOptionFactor = 1;
                //    ldecDisBenefitAnount = GetNetBenefitAmount(adecTotalAccuredBenefit, ldecDisabilityBenOptionFactor);
                //}
                //lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                //lbusBenefitCalculationOptions = GetLocalDisabilityBenefitOptions(ldecDisabilityBenOptionFactor, ldecDisBenefitAnount, adecRegularBenefit, adecRegularFactor, busConstant.THREE_YEARS_CERTAIN_AND_LIFE_ANNUITY);

                //this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);


            }

        }

        private void CalculateL52BenefitOptionsForDiabledPerson(string astrBenefitOption, decimal adecTotalAccuredBenefit, decimal adecRegularBenefit, decimal adecRegularFactor)
        {
            decimal ldecDisabilityBenOptionFactor = new decimal();
            decimal ldecDisBenefitAnount = new decimal();
            int lintPlanBenID = 0;
            busBenefitCalculationOptions lbusBenefitCalculationOptions = null;
            //Lump Sum
            if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.LUMP_SUM)
            {
                GetDisabilityBenefitOptions(busConstant.LUMP_SUM, adecTotalAccuredBenefit, adecRegularBenefit, adecRegularFactor);

                //if (this.icdoBenefitCalculationHeader.istrRetirementType != busConstant.RETIREMENT_TYPE_LATE)
                //{
                //    ldecDisabilityBenOptionFactor = idecLumpSumFactor;
                //    ldecDisBenefitAnount = Math.Round(adecTotalAccuredBenefit * ldecDisabilityBenOptionFactor, 2);
                //}
                //lbusBenefitCalculationOptions = GetLocalDisabilityBenefitOptions(ldecDisabilityBenOptionFactor, ldecDisBenefitAnount, adecRegularBenefit, adecRegularFactor, busConstant.LUMP_SUM);

                //this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);

            }
            //Life Annuity 
            //Joint & 50% Pop-up Annuity 
            if (iblnIsQualifiedSpouse)
            {
                if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.JOINT_50_PERCENT_POPUP_ANNUITY)
                {
                    GetDisabilityBenefitOptions(busConstant.JOINT_50_PERCENT_POPUP_ANNUITY, adecTotalAccuredBenefit, adecRegularBenefit, adecRegularFactor);

                    //if (this.icdoBenefitCalculationHeader.istrRetirementType != busConstant.RETIREMENT_TYPE_LATE)
                    //{
                    //    ldecDisabilityBenOptionFactor = ibusCalculation.GetBenefitOptionFactor(.915M, .004M, idecAgeDiff);
                    //    ldecDisBenefitAnount = GetNetBenefitAmount(adecTotalAccuredBenefit, ldecDisabilityBenOptionFactor);
                    //}
                    //lbusBenefitCalculationOptions = GetLocalDisabilityBenefitOptions(ldecDisabilityBenOptionFactor, ldecDisBenefitAnount, adecRegularBenefit, adecRegularFactor, busConstant.JOINT_50_PERCENT_POPUP_ANNUITY);

                    //this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);

                }
                if (this.ibusBenefitApplication.icdoBenefitApplication.retirement_date > busConstant.BenefitCalculation.DISABILITY_JS75_CHECK_DATE)
                {
                    //Joint & 75%  Annuity :Not there
                    if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY)
                    {
                        GetDisabilityBenefitOptions(busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, adecTotalAccuredBenefit, adecRegularBenefit, adecRegularFactor);

                        //lintPlanBenID = this.iclbcdoPlanBenefit.Where(lbusPlanBenefitXr => lbusPlanBenefitXr.icdoPlanBenefitXr.benefit_option_value == busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY).First().icdoPlanBenefitXr.plan_benefit_id;
                        //if (this.icdoBenefitCalculationHeader.istrRetirementType != busConstant.RETIREMENT_TYPE_LATE)
                        //{
                        //    ldecDisabilityBenOptionFactor = ibusCalculation.GetBenefitOptionFactor(.83M, .005M, idecAgeDiff);
                        //    ldecDisBenefitAnount = GetNetBenefitAmount(adecTotalAccuredBenefit, ldecDisabilityBenOptionFactor);
                        //}
                        //lbusBenefitCalculationOptions = GetLocalDisabilityBenefitOptions(ldecDisabilityBenOptionFactor, ldecDisBenefitAnount, adecRegularBenefit, adecRegularFactor, busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY);
                        //this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);

                    }
                }

                //Joint & 100% Pop-up Annuity 
                if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.JOINT_100_PERCENT_POPUP_ANNUITY)
                {
                    GetDisabilityBenefitOptions(busConstant.JOINT_100_PERCENT_POPUP_ANNUITY, adecTotalAccuredBenefit, adecRegularBenefit, adecRegularFactor);
                    //lintPlanBenID = this.iclbcdoPlanBenefit.Where(lbusPlanBenefitXr => lbusPlanBenefitXr.icdoPlanBenefitXr.benefit_option_value == busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY).First().icdoPlanBenefitXr.plan_benefit_id;
                    //if (this.icdoBenefitCalculationHeader.istrRetirementType != busConstant.RETIREMENT_TYPE_LATE)
                    //{
                    //    if (idecAgeDiff < 0)
                    //    {
                    //        ldecDisabilityBenOptionFactor = ibusCalculation.GetBenefitOptionFactor(.75M, .004M, idecAgeDiff);
                    //    }
                    //    else
                    //    {
                    //        ldecDisabilityBenOptionFactor = ibusCalculation.GetBenefitOptionFactor(.75M, .005M, idecAgeDiff);
                    //    }
                    //}

                    //ldecDisBenefitAnount = GetNetBenefitAmount(adecTotalAccuredBenefit, ldecDisabilityBenOptionFactor);

                    //lbusBenefitCalculationOptions = GetLocalDisabilityBenefitOptions(ldecDisabilityBenOptionFactor, ldecDisBenefitAnount, adecRegularBenefit, adecRegularFactor, busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY);

                    //this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);

                }
            }
            //Ten-Years-Certain and Life Annuity 
            if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY)
            {
                GetDisabilityBenefitOptions(busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, adecTotalAccuredBenefit, adecRegularBenefit, adecRegularFactor);
                //if (this.icdoBenefitCalculationHeader.istrRetirementType != busConstant.RETIREMENT_TYPE_LATE)
                //{
                //    ldecDisabilityBenOptionFactor = 1;
                //    ldecDisBenefitAnount = GetNetBenefitAmount(adecTotalAccuredBenefit, ldecDisabilityBenOptionFactor);
                //}
                //lbusBenefitCalculationOptions = GetLocalDisabilityBenefitOptions(ldecDisabilityBenOptionFactor, ldecDisBenefitAnount, adecRegularBenefit, adecRegularFactor, busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY);
                //this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
            }
            if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.LIFE_ANNUTIY)
            {
                GetDisabilityBenefitOptions(busConstant.LIFE_ANNUTIY, adecTotalAccuredBenefit, adecRegularBenefit, adecRegularFactor);

                //if (this.icdoBenefitCalculationHeader.istrRetirementType != busConstant.RETIREMENT_TYPE_LATE)
                //{
                //    ldecDisabilityBenOptionFactor = 1;
                //    ldecDisBenefitAnount = GetNetBenefitAmount(adecTotalAccuredBenefit, ldecDisabilityBenOptionFactor);
                //}
                //lbusBenefitCalculationOptions = GetLocalDisabilityBenefitOptions(ldecDisabilityBenOptionFactor, ldecDisBenefitAnount, adecRegularBenefit, adecRegularFactor, busConstant.LIFE_ANNUTIY);
                //this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
            }

        }

        private void CalculateL700BenefitOptionsForDiabledPerson(string astrBenefitOption, decimal adecTotalAccuredBenefit, decimal adecRegularBenefit, decimal adecRegularFactor)
        {
            decimal ldecDisabilityBenOptionFactor = new decimal();
            decimal ldecDisBenefitAnount = new decimal();
            busBenefitCalculationOptions lbusBenefitCalculationOptions = null;
            ldecDisabilityBenOptionFactor = adecRegularFactor;
            //Lump Sum 
            if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.LUMP_SUM)
            {
                GetDisabilityBenefitOptions(busConstant.LUMP_SUM, adecTotalAccuredBenefit, adecRegularBenefit, adecRegularFactor);

                //if (this.icdoBenefitCalculationHeader.istrRetirementType != busConstant.RETIREMENT_TYPE_LATE)
                //{
                //    ldecDisabilityBenOptionFactor = idecLumpSumFactor;
                //    ldecDisBenefitAnount = Math.Round(adecTotalAccuredBenefit * ldecDisabilityBenOptionFactor, 2);
                //}
                //lbusBenefitCalculationOptions = GetLocalDisabilityBenefitOptions(ldecDisabilityBenOptionFactor, ldecDisBenefitAnount, adecRegularBenefit, adecRegularFactor, busConstant.LUMP_SUM);
                //this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
            }
            if (iblnIsQualifiedSpouse)
            {
                //Qualified Joint & 50% Survivor Annuity 
                if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY)
                {
                    GetDisabilityBenefitOptions(busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY, adecTotalAccuredBenefit, adecRegularBenefit, adecRegularFactor);

                    //if (this.icdoBenefitCalculationHeader.istrRetirementType != busConstant.RETIREMENT_TYPE_LATE)
                    //{
                    //    if (adecRegularFactor == 0)
                    //    {
                    //        ldecDisabilityBenOptionFactor = GetBenefitFactorLocal(busConstant.LOCAL_700, busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY, busConstant.BENEFIT_TYPE_DISABILITY, this.icdoBenefitCalculationHeader.idecParticipantFullAge, this.icdoBenefitCalculationHeader.idecSurvivorFullAge);
                    //    }
                    //    ldecDisBenefitAnount = GetNetBenefitAmount(idecLocalFrozenBenefit, ldecDisabilityBenOptionFactor, .5m);
                    //}
                    //lbusBenefitCalculationOptions = GetLocalDisabilityBenefitOptions(ldecDisabilityBenOptionFactor, ldecDisBenefitAnount, adecRegularBenefit, adecRegularFactor, busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY);
                    //this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);

                }
                //Joint & 66 2/3% Survivor Annuity 
                if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.JOINT_66_2by3_PERCENT_SURVIVOR_ANNUITY)
                {
                    GetDisabilityBenefitOptions(busConstant.JOINT_66_2by3_PERCENT_SURVIVOR_ANNUITY, adecTotalAccuredBenefit, adecRegularBenefit, adecRegularFactor);

                    //if (this.icdoBenefitCalculationHeader.istrRetirementType != busConstant.RETIREMENT_TYPE_LATE)
                    //{
                    //    if (adecRegularFactor == 0)
                    //    {
                    //        ldecDisabilityBenOptionFactor = GetBenefitFactorLocal(busConstant.LOCAL_700, busConstant.JOINT_66_2by3_PERCENT_SURVIVOR_ANNUITY, busConstant.BENEFIT_TYPE_DISABILITY, this.icdoBenefitCalculationHeader.idecParticipantFullAge, this.icdoBenefitCalculationHeader.idecSurvivorFullAge);
                    //    }
                    //    ldecDisBenefitAnount = GetNetBenefitAmount(idecLocalFrozenBenefit, ldecDisabilityBenOptionFactor, .5m);
                    //}
                    //lbusBenefitCalculationOptions = GetLocalDisabilityBenefitOptions(ldecDisabilityBenOptionFactor, ldecDisBenefitAnount, adecRegularBenefit, adecRegularFactor, busConstant.JOINT_66_2by3_PERCENT_SURVIVOR_ANNUITY);

                    //this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);

                }
                //Joint & 100% Survivor Annuity
                if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY)
                {
                    GetDisabilityBenefitOptions(busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY, adecTotalAccuredBenefit, adecRegularBenefit, adecRegularFactor);

                    //if (this.icdoBenefitCalculationHeader.istrRetirementType != busConstant.RETIREMENT_TYPE_LATE)
                    //{
                    //    if (adecRegularFactor == 0)
                    //    {
                    //        ldecDisabilityBenOptionFactor = GetBenefitFactorLocal(busConstant.LOCAL_700, busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY, busConstant.BENEFIT_TYPE_DISABILITY, this.icdoBenefitCalculationHeader.idecParticipantFullAge, this.icdoBenefitCalculationHeader.idecSurvivorFullAge);
                    //    }
                    //    ldecDisBenefitAnount = GetNetBenefitAmount(idecLocalFrozenBenefit, ldecDisabilityBenOptionFactor, .5m);
                    //}
                    //lbusBenefitCalculationOptions = GetLocalDisabilityBenefitOptions(ldecDisabilityBenOptionFactor, ldecDisBenefitAnount, adecRegularBenefit, adecRegularFactor, busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY);

                    //this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);

                }
                if (this.ibusBenefitApplication.icdoBenefitApplication.retirement_date > busConstant.BenefitCalculation.DISABILITY_JS75_CHECK_DATE)
                {
                    //Joint & 75%  Annuity : Not there 
                    if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY)
                    {
                        GetDisabilityBenefitOptions(busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, adecTotalAccuredBenefit, adecRegularBenefit, adecRegularFactor);
                        //if (this.icdoBenefitCalculationHeader.istrRetirementType != busConstant.RETIREMENT_TYPE_LATE)
                        //{
                        //    if (adecRegularFactor == 0)
                        //    {
                        //        ldecDisabilityBenOptionFactor = GetBenefitFactorLocal(busConstant.LOCAL_700, busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, busConstant.BENEFIT_TYPE_DISABILITY, this.icdoBenefitCalculationHeader.idecParticipantFullAge, this.icdoBenefitCalculationHeader.idecSurvivorFullAge);
                        //    }
                        //    ldecDisBenefitAnount = GetNetBenefitAmount(idecLocalFrozenBenefit, ldecDisabilityBenOptionFactor, .5m);
                        //}
                        //lbusBenefitCalculationOptions = GetLocalDisabilityBenefitOptions(ldecDisabilityBenOptionFactor, ldecDisBenefitAnount, adecRegularBenefit, adecRegularFactor, busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY);

                        //this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);

                    }
                }
            }
            //Two-Years-Certain and Life Annuity 
            if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.TWO_YEARS_CERTAIN_AND_LIFE_ANNUITY)
            {
                GetDisabilityBenefitOptions(busConstant.TWO_YEARS_CERTAIN_AND_LIFE_ANNUITY, adecTotalAccuredBenefit, adecRegularBenefit, adecRegularFactor);

                //if (this.icdoBenefitCalculationHeader.istrRetirementType != busConstant.RETIREMENT_TYPE_LATE)
                //{
                //    if (adecRegularFactor == 0)
                //    {
                //        ldecDisabilityBenOptionFactor = GetBenefitFactorLocal(busConstant.LOCAL_700, busConstant.TWO_YEARS_CERTAIN_AND_LIFE_ANNUITY, busConstant.BENEFIT_TYPE_DISABILITY, this.icdoBenefitCalculationHeader.idecParticipantFullAge, this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement);
                //    }
                //    ldecDisBenefitAnount = GetNetBenefitAmount(idecLocalFrozenBenefit, ldecDisabilityBenOptionFactor, .5m);
                //}
                //lbusBenefitCalculationOptions = GetLocalDisabilityBenefitOptions(ldecDisabilityBenOptionFactor, ldecDisBenefitAnount, adecRegularBenefit, adecRegularFactor, busConstant.TWO_YEARS_CERTAIN_AND_LIFE_ANNUITY);
                //this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);

            }


        }
        */

        #endregion

        #endregion

        #endregion

        #region Common

        public void SetUpDisabilityVariablesForCalculation(DateTime adtRetirementDate, bool ablnPostRetrDeath = false)
        {
            #region Ages
            this.iintAgeAtRetirement = Convert.ToInt32(Math.Floor(busGlobalFunctions.CalculatePersonAge(this.ibusPerson.icdoPerson.idtDateofBirth, this.icdoBenefitCalculationHeader.retirement_date)));
            if (adtRetirementDate != DateTime.MinValue)
            {
                //Need to take this Off as it will come application
                this.ibusBenefitApplication.icdoBenefitApplication.retirement_date = adtRetirementDate;
                //
                this.ibusBenefitApplication.idecAge = busGlobalFunctions.CalculatePersonAgeInDec(this.ibusPerson.icdoPerson.idtDateofBirth, adtRetirementDate);

                this.icdoBenefitCalculationHeader.age = this.ibusBenefitApplication.idecAge; //Load the AGE OF THE MAIN HEADER OBJECT AS WELL

                this.ibusBenefitApplication.LoadandProcessWorkHistory_ForAllPlans();

                if (this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth != DateTime.MinValue)
                {
                    this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement = busGlobalFunctions.CalculatePersonAgeInDec(this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth, adtRetirementDate);
                }

                this.icdoBenefitCalculationHeader.idecParticipantFullAge = Math.Floor(this.ibusBenefitApplication.idecAge);

                this.icdoBenefitCalculationHeader.idecSurvivorFullAge = Math.Floor(this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement);

            }
            #endregion

            #region Option2Check
            if (this.icdoBenefitCalculationHeader.retirement_date == this.ibusBenefitApplication.icdoBenefitApplication.retirement_date)
            {
                this.CheckIfHoursReportedAfterDisabilityDate();

                if (this.iblnCheckIfHoursAfterDisabilityRetirementDate)
                {
                    this.iblnPostRetirementDeath = ablnPostRetrDeath;
                    this.LoadHoursAfterRetirementDate();
                    if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE)
                    {
                        //As Per Pir No. 256 in UAT , Option 2 should be editable.
                        if (this.icdoBenefitCalculationHeader.retirement_date_option_2 == DateTime.MinValue)
                        {
                            this.GetLastWorkingDate();
                            if (this.idtLastWorkingDate != DateTime.MinValue)
                            {
                                this.icdoBenefitCalculationHeader.retirement_date_option_2 = this.idtLastWorkingDate.GetLastDayofMonth().AddDays(1);
                            }
                        }
                        if (this.icdoBenefitCalculationHeader.payment_date == DateTime.MinValue)
                        {
                            this.icdoBenefitCalculationHeader.payment_date = DateTime.Now.GetLastDayofMonth().AddDays(1);
                        }

                    }
                    else
                    {
                        //default it to first of next month of calculation date : Will be covered in 3.0 
                        this.icdoBenefitCalculationHeader.payment_date = DateTime.Now.GetLastDayofMonth().AddDays(1);


                    }

                }
            }
            #endregion

            iintParticipantAgeAsofPaymentDate = Convert.ToInt32(busGlobalFunctions.CalculatePersonAge(this.ibusPerson.icdoPerson.date_of_birth, this.icdoBenefitCalculationHeader.payment_date));
            iblnIsQualifiedSpouse = ibusCalculation.CheckIfSurvivorIsQualifiedSpouse(this.icdoBenefitCalculationHeader.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id);

            //Check if this is the case of Disability Conversion 
            //If in case Participant is married at the time of Early Retirement and has Early Retirement for benefit option of type JS and got divorced before 
            //Disability conversion ,so in this we will allow him to choose JS type benefit option so qualified spouse flag should be true
            if (icdoBenefitCalculationHeader.payee_account_id > 0 && !iblnIsQualifiedSpouse)
            {
                busPayeeAccount lEarlyRetirementPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
                lEarlyRetirementPayeeAccount.FindPayeeAccount(icdoBenefitCalculationHeader.payee_account_id);
                lEarlyRetirementPayeeAccount.LoadBenefitDetails();

                foreach (busBenefitApplicationDetail lbusBenefitApplicationDetail in this.ibusBenefitApplication.iclbBenefitApplicationDetail)
                {
                    if (lbusBenefitApplicationDetail.iintPlan_ID == lEarlyRetirementPayeeAccount.icdoPayeeAccount.iintPlanId)
                    {
                        busCodeValue lbusCodeValue = new busCodeValue();
                        lbusCodeValue.icdoCodeValue = lbusCodeValue.GetCodeValue(busConstant.BENEFIT_OPTION_CODE_ID, lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue);

                        if (lbusCodeValue.icdoCodeValue.data1 == busConstant.Joint_Survivor)
                        {
                            iblnIsQualifiedSpouse = true;
                            break;
                        }
                    }
                }
            }

            //Lum Sum Factor Constant
            //idecLumpSumFactor = ibusCalculation.GetLumpsumBenefitFactorByAGE(Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge)) * 12;
            //Ticket - 61531
            idecLumpSumFactor =Math.Round(GetLumpsumBenefitFactor(Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge), this.ibusBenefitApplication.icdoBenefitApplication.retirement_date.Year) * 12,3);
            if (idecLumpSumFactor == 1)
            {
                idecLumpSumFactor = ibusCalculation.GetLumpsumBenefitFactorByAGE(Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge)) * 12;
                idecLumpSumFactor = Math.Round(idecLumpSumFactor, 3);

            }
            //Frozen Benefit For Locals
            busPersonAccount lbusPersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };

            if (this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).Count() > 0)
            {
                iintPersonAccountId = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).First().icdoPersonAccount.person_account_id;
            }
            if (this.icdoBenefitCalculationHeader.iintPlanId != busConstant.MPIPP_PLAN_ID && this.icdoBenefitCalculationHeader.iintPlanId != busConstant.IAP_PLAN_ID)
            {
                #region Frozen_Local
                if ((!this.iclbPersonAccountRetirementContribution.IsNullOrEmpty()) && this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == iintPersonAccountId).Count() > 0)
                {
                    idecLocalFrozenBenefit = this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == iintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_frozen_benefit_amount);
                }
                #endregion

                #region Mpi_Plus_Local_Check_For_LumpSum_Or_Annuity
                if (this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).Count() > 0)
                {
                    lbusPersonAccount = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault();
                    if (lbusPersonAccount.icdoPersonAccount.istrRetirementSubType == busConstant.RETIREMENT_TYPE_LATE)
                    {

                        int lintTotalQualifiedYear = this.ibusBenefitApplication.aclbPersonWorkHistory_MPI.OrderByDescending(item => item.year).ToList().ToCollection().FirstOrDefault().qualified_years_count;
                        decimal ldecLateAdjustmentAmt = decimal.Zero;
                        idecFinalAccruedBenefitAmount = ibusCalculation.CalculateReducedBenefit(ibusPerson, busConstant.BENEFIT_TYPE_RETIREMENT
                        , this.icdoBenefitCalculationHeader.idecParticipantFullAge,
                        this.ibusBenefitApplication.icdoBenefitApplication.retirement_date,
                        DateTime.MinValue,
                        lbusPersonAccount,
                        ibusBenefitApplication, busConstant.BOOL_FALSE,
                        null, null, lintTotalQualifiedYear, decimal.Zero, lbusPersonAccount.icdoPersonAccount.istrRetirementSubType, true,
                        ibusBenefitApplication.aclbPersonWorkHistory_MPI, this.iclbPersonAccountRetirementContribution, ref ldecLateAdjustmentAmt, String.Empty, this.ibusPerson.icdoPerson.person_id);
                    }
                    else
                    {

                        idecFinalAccruedBenefitAmount = this.ibusCalculation.CalculateUnReducedBenefitAmtForPension(ibusPerson, this.icdoBenefitCalculationHeader.idecParticipantFullAge, this.ibusBenefitApplication.icdoBenefitApplication.retirement_date,
                        ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault(),
                        ibusBenefitApplication, busConstant.BOOL_FALSE,
                        null, null, ibusBenefitApplication.aclbPersonWorkHistory_MPI, this.icdoBenefitCalculationHeader.istrRetirementType, this.icdoBenefitCalculationHeader.benefit_type_value);


                    }
                }
                #endregion

                #region NonSuspendible_Local
                if (iblnCheckIfHoursAfterDisabilityRetirementDate)
                {
                    if (this.icdoBenefitCalculationHeader.payment_date != DateTime.MinValue &&
                    this.icdoBenefitCalculationHeader.retirement_date == this.ibusBenefitApplication.icdoBenefitApplication.retirement_date)
                    {
                        //iintNonSuspendibleMonths = GetNonSuspendibleMonthsBetweenTwoDates();
                        iintNonSuspendibleMonths = GetNonSuspendibleMonthsBetweenTwoDatesAfterRetirement();
                    }
                }
                #endregion

            }
            else if (this.icdoBenefitCalculationHeader.iintPlanId == busConstant.MPIPP_PLAN_ID)
            {
                idecLocalFrozenBenefit = this.ibusCalculation.GetTotalFrozenAmountForDiability(this.icdoBenefitCalculationHeader.idecParticipantFullAge, this.ibusBenefitApplication, this.ibusPerson, this.ibusBenefitApplication.icdoBenefitApplication.retirement_date, this.iclbPersonAccountRetirementContribution);
            }

            //Age Diff : Will be Used in Locals Calc
            idecAgeDiff = this.icdoBenefitCalculationHeader.idecParticipantFullAge - this.icdoBenefitCalculationHeader.idecSurvivorFullAge;


        }

        private decimal GetNetBenefitAmount(decimal adecAccuredBenefit, decimal adecBenefitOptionFactor, decimal adecRoundUpto = 1)
        {
            // =RoundUp (AccBen*Factor,0)

            //L600,666,161
            decimal ldecNetBenefitAmount = new decimal();
            ldecNetBenefitAmount = adecAccuredBenefit * adecBenefitOptionFactor;
            if (adecRoundUpto == 1)
            {
                ldecNetBenefitAmount = Math.Ceiling(ldecNetBenefitAmount);
            }
            else if (adecRoundUpto == .5m)
            {
                ldecNetBenefitAmount = Convert.ToDecimal(Math.Ceiling((ldecNetBenefitAmount) / 0.5M) * 0.5M);
            }
            return ldecNetBenefitAmount;
        }

        private decimal GetDifference(decimal adecPartAge, decimal adecSpouseAge)
        {
            decimal ldecDiff = 0;
            ldecDiff = Math.Floor(Math.Floor(adecPartAge) - Math.Floor(adecSpouseAge));
            return ldecDiff;
        }

        /// <summary>
        /// =MIN(SUM(adecMainFactor-(AgeDiff*adecAgeDiffFactor)),0.99)
        /// EXL600 JP50 : =MIN(SUM(0.775-(AgeDiff*0.004)),0.99)
        /// </summary>
        /// <param name="adecMainFactor"></param>
        /// <param name="adecAgeDiffFactor"></param>
        private decimal GetBenefitOptionFactor(decimal adecMainFactor, decimal adecAgeDiffFactor, decimal adecAgeDiff)
        {
            decimal ldecBenefitOptionFactor = new decimal();
            ldecBenefitOptionFactor = adecMainFactor - (adecAgeDiff * adecAgeDiffFactor);
            ldecBenefitOptionFactor = Math.Min(ldecBenefitOptionFactor, 0.99M);
            return ldecBenefitOptionFactor;

        }

        /// <summary>
        /// For MPIPHP Benefit Options JS50, JS75,JS100
        /// </summary>
        /// <param name="adecMainFactor"></param>
        /// <param name="iintAgeFactor"></param>
        /// <param name="adecNormalRetrFactor"></param>
        /// <returns></returns>
        private decimal GetMPIBenefitOptionFactor(decimal adecMainFactor, decimal adecAgeFactor, decimal adecNormalRetrFactor)
        {
            decimal ldecBenefitOptionFactor = new decimal();
            int lintAgeDiff = Convert.ToInt32(this.icdoBenefitCalculationHeader.idecSurvivorFullAge - this.icdoBenefitCalculationHeader.idecParticipantFullAge);
            ldecBenefitOptionFactor = adecMainFactor + adecAgeFactor * lintAgeDiff + adecNormalRetrFactor * (65 - Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge));
            ldecBenefitOptionFactor = Math.Round(ldecBenefitOptionFactor, 3);
            return ldecBenefitOptionFactor;

        }


        public void GetEligiblityForRegularBenefit()
        {
            iblnEligibleForRegularBenefit = false;
            if (this.icdoBenefitCalculationHeader.istrRetirementType == busConstant.RETIREMENT_TYPE_REDUCED_EARLY ||
               this.icdoBenefitCalculationHeader.istrRetirementType == busConstant.RETIREMENT_TYPE_SPL_REDUCED_EARLY ||
               this.icdoBenefitCalculationHeader.istrRetirementType == busConstant.RETIREMENT_TYPE_UNREDUCED_EARLY ||
                this.icdoBenefitCalculationHeader.istrRetirementType == busConstant.RETIREMENT_TYPE_LATE)
            {
                iblnEligibleForRegularBenefit = true;
            }
        }

        public void CopyApplicationPropertiesToCalculation()
        {
            this.icdoBenefitCalculationHeader.retirement_date = this.ibusBenefitApplication.icdoBenefitApplication.retirement_date;
            this.icdoBenefitCalculationHeader.ssa_approval_date = this.ibusBenefitApplication.icdoBenefitApplication.entitlement_date;
            this.icdoBenefitCalculationHeader.ssa_disability_onset_date = this.ibusBenefitApplication.icdoBenefitApplication.disability_onset_date;
            this.icdoBenefitCalculationHeader.ssa_application_date = this.ibusBenefitApplication.icdoBenefitApplication.ssa_application_date;
            this.icdoBenefitCalculationHeader.terminally_ill_flag = this.ibusBenefitApplication.icdoBenefitApplication.terminally_ill_flag;
            this.icdoBenefitCalculationHeader.awarded_on_date = this.ibusBenefitApplication.icdoBenefitApplication.awarded_on_date;
            if (this.CheckIfHoursReportedAfterDisabilityDate() && this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE)
            {
                if (this.icdoBenefitCalculationHeader.retirement_date_option_2 == DateTime.MinValue)
                {
                    this.GetLastWorkingDate();
                    if (idtLastWorkingDate != DateTime.MinValue)
                    {
                        this.icdoBenefitCalculationHeader.retirement_date_option_2 = idtLastWorkingDate.GetLastDayofMonth().AddDays(1);
                    }
                }
            }
        }

        public bool CheckIfRetrDateIsNotFirstDayofMonthAfterSSAApprovalDate()
        {
            DateTime ldtTime = busGlobalFunctions.GetFirstDayofMonthAfterGivenDate(this.icdoBenefitCalculationHeader.ssa_approval_date);
            if (this.icdoBenefitCalculationHeader.retirement_date >= ldtTime)
            {
                return false;
            }
            return true;
        }

        //151767
        public bool IsOnlyOnePlanAllowed()
        {
            if (this.icdoBenefitCalculationHeader.retirement_date >= busConstant.BenefitCalculation.DISABILITY_IAP_ONLY_EFFECTIVE_DATE)
            { 
                return true; 
            }                                                                                                                        
            return false;
        }

        #endregion

        private void CreateDisabilityPayeeAccountForRetireeInc(int aintBenefitAccountID, string astrFamilyRelationshipValue, decimal adecNonTaxableBeginningBalance,
                                                            DateTime adtNextPaymentDate, int aintEarlyRetirementBenefitApplicationID)
        {
            //Load all existing retiree inc payee acccount
            busPayeeAccount lbusPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
            Collection<busPayeeAccount> lclbEarlyRetirementRetireIncPayeeAccount = new Collection<busPayeeAccount>();
            int lintPayeeAccountID = 0;

            if (aintEarlyRetirementBenefitApplicationID > 0)
            {
                lclbEarlyRetirementRetireIncPayeeAccount = lbusPayeeAccount.LoadRetireeIncFromAppDetail(this.icdoBenefitCalculationHeader.payee_account_id);
            }

            foreach (busDisabilityRetireeIncrease lbusDisabilityRetireeIncrease in iclbDisabilityRetireeIncrease)
            {
                lintPayeeAccountID = lbusPayeeAccount.ManagePayeeAccount(0, icdoBenefitCalculationHeader.person_id, 0,
                                                                                      iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.benefit_application_detail_id,
                                                                                      iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.benefit_calculation_detail_id,
                                                                                      0, 0, aintBenefitAccountID, iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.benefit_subtype_value,
                                                                                      icdoBenefitCalculationHeader.istrRetirementType,
                                                                                      lbusDisabilityRetireeIncrease.icdoDisabilityRetireeIncrease.retiree_increase_date,
                                                                                      DateTime.MinValue, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER, astrFamilyRelationshipValue,
                                                                                      iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.minimum_guarantee_amount,
                                                                                      adecNonTaxableBeginningBalance,
                                                                                      iclbBenefitCalculationDetail.First().iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.plan_benefit_id,
                                                                                      DateTime.MinValue);

                decimal ldecTaxableAmount = 0M;
                decimal ldecNonTaxableAmount = 0M;
                decimal ldecAdjustedRetireeIncAmt = 0;

                if (lclbEarlyRetirementRetireIncPayeeAccount != null && (!lclbEarlyRetirementRetireIncPayeeAccount.Where(item => item.icdoPayeeAccount.benefit_begin_date.Year ==
                                                lbusDisabilityRetireeIncrease.icdoDisabilityRetireeIncrease.retiree_increase_date.Year).IsNullOrEmpty()))
                {
                    busPayeeAccountPaymentItemType lbusPayeeAccountPaymentItemType = new busPayeeAccountPaymentItemType();
                    int lintExistingRetireeIncreaseAmount = lbusPayeeAccountPaymentItemType.LoadAmountItemTypeIdAndPayeeAccountId(lclbEarlyRetirementRetireIncPayeeAccount.Where(item => item.icdoPayeeAccount.benefit_begin_date ==
                                                lbusDisabilityRetireeIncrease.icdoDisabilityRetireeIncrease.retiree_increase_date).FirstOrDefault().icdoPayeeAccount.payee_account_id);
                    ldecAdjustedRetireeIncAmt = lbusDisabilityRetireeIncrease.icdoDisabilityRetireeIncrease.retiree_increase_amount - lintExistingRetireeIncreaseAmount;
                }
                else
                {
                    ldecAdjustedRetireeIncAmt = lbusDisabilityRetireeIncrease.icdoDisabilityRetireeIncrease.retiree_increase_amount;
                }


                //R3view -- First Parameter Should be maybe Retirement or Payment Date Review the Function too   
                if (iclbBenefitCalculationDetail.First().iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.istrBenefitOptionDescription.Contains(busConstant.LUMP_SUM_DESCRIPTION)
                        && iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID)
                {
                    busPayeeAccountHelper.CalculateMonthlyPaymentComponents(DateTime.MinValue, ldecAdjustedRetireeIncAmt,
                                              ref ldecNonTaxableAmount, ref ldecTaxableAmount, adecNonTaxableBeginningBalance);
                }
                else
                {
                    busPayeeAccountHelper.CalculateMonthlyPaymentComponents(DateTime.MinValue, ldecAdjustedRetireeIncAmt,
                                                  ref ldecNonTaxableAmount, ref ldecTaxableAmount, iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.monthly_exclusion_amount);
                }

                if (ldecTaxableAmount > 0M)
                {
                    lbusPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.ITEM48, ldecTaxableAmount, "0", 0,
                                                                          adtNextPaymentDate, DateTime.MinValue, "N", false);
                }

                //Create Payee Account in Review
                lbusPayeeAccount.CreateReviewPayeeAccountStatus();

                if (ldecAdjustedRetireeIncAmt >= 750)
                {
                    if (lbusPayeeAccount.idtNextBenefitPaymentDate == DateTime.MinValue)
                        lbusPayeeAccount.idtNextBenefitPaymentDate = adtNextPaymentDate;
                    lbusPayeeAccount.ProcessTaxWithHoldingDetails(true);
                }
            }
        }



        #endregion

        #region Final
        public void SpawnFinalRetirementCalculation(int aintBenefitApplicationDetailId, int aintPersonAccountId, int aintPlanId, string astrPlanCode, DateTime adtVestedDate, string astrBenefitSubTypeValue, string astrBenefitOptionValue, DateTime adtRetirementDate, string astrIAPAdjustmentFlag = "", bool ablnConvertBenOption = false, string astrOriginalBenefitOption = "") //RequestID: 72091
        {
            this.PopulateInitialDataBenefitCalculationDetails(aintBenefitApplicationDetailId, aintPersonAccountId, aintPlanId, astrPlanCode, adtVestedDate, astrBenefitSubTypeValue, adtRetirementDate);
            this.ibusCalculation.CheckIfSurvivorIsQualifiedSpouse(this.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id);
            this.LoadPlanBenefitsForPlan(aintPlanId);
            this.CopyApplicationPropertiesToCalculation();
            //this.icdoBenefitCalculationHeader.istrRetirementType = astrBenefitSubTypeValue;
            decimal ldecTotalBenefitAmount = new decimal();

            #region SWITCH CASE - INITIATE CALCULATION BASED ON THE PLAN
            if (!this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.IsNullOrEmpty())
            {
                if (string.IsNullOrEmpty(this.ibusBenefitApplication.icdoBenefitApplication.benefit_type_value))
                {
                    this.ibusBenefitApplication.icdoBenefitApplication.benefit_type_value = busConstant.BENEFIT_TYPE_DISABILITY;
                }
                //Switched back to MPI Factors and Reduction factors.
                switch (astrPlanCode)
                {
                    case busConstant.Local_161:
                        this.iclbBenefitCalculationDetail[0].icdoBenefitCalculationDetail.unreduced_benefit_amount = idecLocalFrozenBenefit;
                        CalculateLocal161Benefit(astrBenefitOptionValue, busConstant.BOOL_TRUE);
                        break;

                    case busConstant.Local_52:
                        this.iclbBenefitCalculationDetail[0].icdoBenefitCalculationDetail.unreduced_benefit_amount = idecLocalFrozenBenefit;
                        CalculateLocal52Benefit(astrBenefitOptionValue, busConstant.BOOL_TRUE);
                        break;

                    case busConstant.Local_600:
                        this.iclbBenefitCalculationDetail[0].icdoBenefitCalculationDetail.unreduced_benefit_amount = idecLocalFrozenBenefit;
                        CalculateLocal600Benefit(astrBenefitOptionValue, busConstant.BOOL_TRUE);
                        break;

                    case busConstant.Local_666:
                        this.iclbBenefitCalculationDetail[0].icdoBenefitCalculationDetail.unreduced_benefit_amount = idecLocalFrozenBenefit;
                        CalculateLocal666Benefit(astrBenefitOptionValue, busConstant.BOOL_TRUE);
                        break;

                    case busConstant.LOCAL_700:
                        this.iclbBenefitCalculationDetail[0].icdoBenefitCalculationDetail.unreduced_benefit_amount = idecLocalFrozenBenefit;
                        CalculateLocal700Benefit(astrBenefitOptionValue, busConstant.BOOL_TRUE);
                        break;

                    case busConstant.MPIPP:
                        if (this.ibusBenefitApplication.aclbPersonWorkHistory_MPI != null && this.ibusBenefitApplication.aclbPersonWorkHistory_MPI.Count > 0)
                        {
                            this.CalculateMPIBenefitOptions(astrBenefitOptionValue,ablnConvertBenOption,astrOriginalBenefitOption); //RequestID: 72091

                            //RequestID: 72091
                            if (ablnConvertBenOption && (astrOriginalBenefitOption == busConstant.JOINT_100_PERCENT_POPUP_ANNUITY || astrOriginalBenefitOption == busConstant.JOINT_50_PERCENT_POPUP_ANNUITY)
                                   && this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().iclbBenefitCalculationYearlyDetail.Count > 0
                                   && this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().iclbBenefitCalculationOptions.Count > 0)
                            {

                                foreach (busBenefitCalculationYearlyDetail lbusbenefitcalculationyearlydetail in this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().iclbBenefitCalculationYearlyDetail)
                                {
                                    if (lbusbenefitcalculationyearlydetail.icdoBenefitCalculationYearlyDetail.annual_adjustment_amount != 0 || lbusbenefitcalculationyearlydetail.icdoBenefitCalculationYearlyDetail.plan_year >= this.icdoBenefitCalculationHeader.retirement_date.Year) //RequestID: 72091
                                        lbusbenefitcalculationyearlydetail.icdoBenefitCalculationYearlyDetail.benefit_as_of_det_date = Math.Round(this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().iclbBenefitCalculationOptions.FirstOrDefault().icdoBenefitCalculationOptions.pop_up_benefit_amount, 2);
                                }
                            }
                        }
                        break;

                    case busConstant.IAP:
                        if (this.ibusBenefitApplication.aclbPersonWorkHistory_IAP != null && this.ibusBenefitApplication.aclbPersonWorkHistory_IAP.Count > 0)
                        {
                            this.CalculateIAPBenefitAmount(astrBenefitOptionValue, astrAdjustmentFlag: astrIAPAdjustmentFlag);
                        }
                        break;
                }
            }
            #endregion

            #region Set Accrued At Retirement in Detail Object , will be used for ReEmployment
            if (this.iclbBenefitCalculationDetail.Count > 0)
            {
                this.iclbBenefitCalculationDetail.FirstOrDefault().icdoBenefitCalculationDetail.accrued_at_retirement_amount = this.iclbBenefitCalculationDetail.FirstOrDefault().icdoBenefitCalculationDetail.unreduced_benefit_amount;
                if (this.iblnCheckIfHoursAfterDisabilityRetirementDate)
                {
                    this.iclbBenefitCalculationDetail.FirstOrDefault().icdoBenefitCalculationDetail.reemployed_accrued_benefit_effective_date = this.icdoBenefitCalculationHeader.retirement_date;
                }
            }
            #endregion



        }

        public ArrayList btn_RefreshCalculation()
        {
            ArrayList larrList = new ArrayList();

            #region FLAGS
            //Flags to be used for making sure we donot calculate again if another IAP entry comes along
            bool lblnIAPCalculated = false;
            bool lblnMPIPPCalculated = false;

            bool lblnL52SplFlag = false;
            bool lblnL161SplFlag = false;
            bool lblnUVHPFlag = false;
            bool lblnNonVestedEEFlag = false;

            int lintMPIPPHeaderId = 0;
            int lintIAPHeaderId = 0;
            this.iblnCalcualteUVHPBenefit = this.iblnCalculateIAPBenefit = this.iblnCalculateL161SplAccBenefit = this.iblnCalculateL52SplAccBenefit = this.iblnCalculateMPIPPBenefit = false;
            #endregion



            this.icdoBenefitCalculationHeader.iintPlanId = this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.plan_id;

            if (!lblIsNew)
                FlushOlderCalculations();

            if (this.ibusBenefitApplication.FindBenefitApplication(this.icdoBenefitCalculationHeader.benefit_application_id))
            {



                this.ibusBenefitApplication.iclbBenefitApplicationDetail = new Collection<busBenefitApplicationDetail>();
                this.iclbPersonAccountRetirementContribution = new Collection<busPersonAccountRetirementContribution>();

                this.ibusBenefitApplication.LoadBenefitApplicationDetails();
                //this.LoadAllRetirementContributions();

                if (ibusPerson.iclbPersonAccount != null && !ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).IsNullOrEmpty())
                {
                    LoadAllRetirementContributions(ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault());
                }
                else
                {
                    LoadAllRetirementContributions(null);
                }

                this.SetUpDisabilityVariablesForCalculation(this.icdoBenefitCalculationHeader.retirement_date);
                this.SetupPreRequisites_DisabilityCalculations();
                this.ValidateHardErrors(utlPageMode.Update);

                if (!this.ibusBenefitApplication.iclbBenefitApplicationDetail.IsNullOrEmpty())
                {
                    foreach (busBenefitApplicationDetail lbusBenefitApplicationDetail in this.ibusBenefitApplication.iclbBenefitApplicationDetail)
                    {
                        //this.iclbBenefitCalculationDetail = new Collection<busBenefitCalculationDetail>();
                        if (lbusBenefitApplicationDetail.iintPlan_ID != this.icdoBenefitCalculationHeader.iintPlanId)
                            continue;
                        if (lbusBenefitApplicationDetail.iintPlan_ID == busConstant.IAP_PLAN_ID && this.icdoBenefitCalculationHeader.iintPlanId == busConstant.IAP_PLAN_ID)
                        {
                            #region IAP PLAN FOUND IN GRID
                            if (lbusBenefitApplicationDetail.istrSubPlan.IsNullOrEmpty() && !this.iblnCalculateIAPBenefit)
                            {
                                this.iblnCalculateIAPBenefit = true;

                            }

                            else if (this.ibusBenefitApplication.iclbBenefitApplicationDetail.Where(item => item.istrSubPlan == busConstant.L52_SPL_ACC && item.iintPlan_ID == busConstant.IAP_PLAN_ID).Count() > 0 && !lblnL52SplFlag)
                            {
                                this.iblnCalculateL52SplAccBenefit = true;
                                lblnL52SplFlag = true;
                            }

                            else if (this.ibusBenefitApplication.iclbBenefitApplicationDetail.Where(item => item.istrSubPlan == busConstant.L161_SPL_ACC && item.iintPlan_ID == busConstant.IAP_PLAN_ID).Count() > 0 && !lblnL161SplFlag)
                            {
                                this.iblnCalculateL161SplAccBenefit = true;
                                lblnL161SplFlag = true;
                            }


                            if (lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue.IsNullOrEmpty() && lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrSubPlanBenefitOptionValue.IsNotNullOrEmpty())
                            {
                                lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue = lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrSubPlanBenefitOptionValue;
                            }

                            this.SpawnFinalRetirementCalculation(lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.benefit_application_detail_id,
                                                                                             this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == lbusBenefitApplicationDetail.istrPlanCode).FirstOrDefault().icdoPersonAccount.person_account_id,
                                                                                             lbusBenefitApplicationDetail.iintPlan_ID, lbusBenefitApplicationDetail.istrPlanCode, this.ibusBenefitApplication.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date,
                                                                                             lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.benefit_subtype_value, lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue, this.ibusBenefitApplication.icdoBenefitApplication.retirement_date);
                            try
                            {
                                this.AfterPersistChanges();
                            }
                            catch
                            {
                            }

                            #endregion
                        }

                        else if (lbusBenefitApplicationDetail.iintPlan_ID == busConstant.MPIPP_PLAN_ID && this.icdoBenefitCalculationHeader.iintPlanId == busConstant.MPIPP_PLAN_ID)
                        {
                            #region MPIPP PLAN FOUNd in GRID
                            if (lbusBenefitApplicationDetail.istrSubPlan.IsNullOrEmpty() && !this.iblnCalculateMPIPPBenefit)
                            {
                                this.iblnCalculateMPIPPBenefit = true;

                            }

                            else if (this.ibusBenefitApplication.iclbBenefitApplicationDetail.Where(item => item.istrSubPlan == busConstant.UVHP && item.iintPlan_ID == busConstant.MPIPP_PLAN_ID).Count() > 0 && !lblnUVHPFlag)
                            {
                                this.iblnCalcualteUVHPBenefit = true;
                                lblnUVHPFlag = true;
                            }



                            if (lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue.IsNullOrEmpty() && lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrSubPlanBenefitOptionValue.IsNotNullOrEmpty())
                            {
                                lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue = lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrSubPlanBenefitOptionValue;
                            }

                            this.SpawnFinalRetirementCalculation(lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.benefit_application_detail_id,
                                                                                             this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == lbusBenefitApplicationDetail.istrPlanCode).FirstOrDefault().icdoPersonAccount.person_account_id,
                                                                                             lbusBenefitApplicationDetail.iintPlan_ID, lbusBenefitApplicationDetail.istrPlanCode, this.ibusBenefitApplication.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date,
                                                                                             lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.benefit_subtype_value, lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue, this.ibusBenefitApplication.icdoBenefitApplication.retirement_date);
                            try
                            {
                                this.AfterPersistChanges();
                            }
                            catch
                            {
                            }


                            #endregion
                        }

                        else if (lbusBenefitApplicationDetail.iintPlan_ID != busConstant.MPIPP_PLAN_ID && lbusBenefitApplicationDetail.iintPlan_ID != busConstant.IAP_PLAN_ID)
                        {
                            #region LOCAL PLAN FOUND


                            if (lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue.IsNullOrEmpty() && lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrSubPlanBenefitOptionValue.IsNotNullOrEmpty())
                            {
                                lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue = lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrSubPlanBenefitOptionValue;
                            }

                            this.SpawnFinalRetirementCalculation(lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.benefit_application_detail_id,
                                                                                             this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == lbusBenefitApplicationDetail.istrPlanCode).FirstOrDefault().icdoPersonAccount.person_account_id,
                                                                                             lbusBenefitApplicationDetail.iintPlan_ID, lbusBenefitApplicationDetail.istrPlanCode, this.ibusBenefitApplication.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date,
                                                                                             lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.benefit_subtype_value, lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue, this.ibusBenefitApplication.icdoBenefitApplication.retirement_date);
                            try
                            {
                                this.AfterPersistChanges();
                            }
                            catch
                            {
                            }


                            #endregion
                        }
                    }

                }
                this.EvaluateInitialLoadRules();
                this.iclbBenefitCalculationDetail.First().EvaluateInitialLoadRules();
            }
            larrList.Add(this);
            return larrList;
        }


        #endregion

        #region Load Person Notes
        public void LoadPersonNotes()
        {
            iclbNotes = new Collection<busNotes>();
            DataTable ldtblist = busPerson.Select("cdoNotes.GetNotesForWithdrawalCaclulation", new object[2] { this.icdoBenefitCalculationHeader.person_id, busConstant.DISABILITY_CALCULATION_MAINTAINENCE_FORM });
            iclbNotes = GetCollection<busNotes>(ldtblist, "icdoNotes");
            if (iclbNotes != null)
                iclbNotes = iclbNotes.OrderByDescending(obj => obj.icdoNotes.created_date).ToList().ToCollection<busNotes>();
        }

        public ArrayList AddDSBLNotes()
        {
            ArrayList larrResult = new ArrayList();
            if (istrNewNotes.IsNullOrEmpty())
            {
                utlError lutlError = AddError(4076, "");
                larrResult.Add(lutlError);
                return larrResult;
            }
            cdoNotes lcdoNotes = new cdoNotes();
            lcdoNotes.person_id = this.icdoBenefitCalculationHeader.person_id;
            lcdoNotes.notes = this.istrNewNotes;
            lcdoNotes.form_id = busConstant.Form_ID;
            lcdoNotes.form_value = busConstant.DISABILITY_CALCULATION_MAINTAINENCE_FORM;
            lcdoNotes.created_by = iobjPassInfo.istrUserID;
            lcdoNotes.created_date = DateTime.Now;
            lcdoNotes.Insert();
            this.LoadPersonNotes();
            this.istrNewNotes = string.Empty;
            larrResult.Add(this);
            return larrResult;
        }
        #endregion

        #region HardErrorMethods
        public bool CheckRtrmntDt6MnthGreaterThanDisOnstDt()
        {
            if ((this.icdoBenefitCalculationHeader.retirement_date) <= (this.icdoBenefitCalculationHeader.ssa_disability_onset_date))
            {
                return true;
            }
            else
            {
                int lintMonths = ((this.icdoBenefitCalculationHeader.retirement_date.Year - this.icdoBenefitCalculationHeader.ssa_disability_onset_date.Year) * 12) + (this.icdoBenefitCalculationHeader.retirement_date.Month - this.icdoBenefitCalculationHeader.ssa_disability_onset_date.Month);

                if (lintMonths == 6)
                {
                    if (this.icdoBenefitCalculationHeader.retirement_date.Day < this.icdoBenefitCalculationHeader.ssa_disability_onset_date.Day)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (lintMonths < 6)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool CheckEntitlementDate()
        {
            if (this.icdoBenefitCalculationHeader.ssa_approval_date <= this.icdoBenefitCalculationHeader.ssa_disability_onset_date)
            {
                return true;
            }
            else
            {
                int lintMonths = ((this.icdoBenefitCalculationHeader.ssa_approval_date.Year - this.icdoBenefitCalculationHeader.ssa_disability_onset_date.Year) * 12) + (this.icdoBenefitCalculationHeader.ssa_approval_date.Month - this.icdoBenefitCalculationHeader.ssa_disability_onset_date.Month);

                if (lintMonths == 5)
                {
                    if (this.icdoBenefitCalculationHeader.ssa_approval_date.Day < this.icdoBenefitCalculationHeader.ssa_disability_onset_date.Day)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (lintMonths < 5)
                {
                    return true;
                }
                else
                    return false;
            }
        }

        #endregion

        #region for PROD PIR 504 - Retirement Date Option 2
        public int GetNonSuspendibleMonthsForOptionTwo()
        {
            //Prod PIR 93
            int lintNonSuspendibleMonths = 0;
            int lintSuspendibleMonths = 0;
            int lintYearlySuspendibleMonths = 0;

            int lintFromyear = this.icdoBenefitCalculationHeader.retirement_date_option_2.Year;
            int lintToYear = this.icdoBenefitCalculationHeader.payment_date.Year;


            this.ibusPerson.LoadPersonSuspendibleMonth();
            int lintTotalMonths = busGlobalFunctions.DateDiffByMonth(this.icdoBenefitCalculationHeader.retirement_date_option_2, this.icdoBenefitCalculationHeader.payment_date);

            busBenefitCalculationDetail lbusBenefitCalculationDetail = null;
            if (this.icdoBenefitCalculationHeader.iintPlanId == busConstant.MPIPP_PLAN_ID)
            {
                lbusBenefitCalculationDetail = this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId && this.icdoBenefitCalculationHeader.retirement_date_option_2 > this.icdoBenefitCalculationHeader.retirement_date).FirstOrDefault();
            }
            busBenefitCalculationYearlyDetail lbusBenefitCalculationYearlyDetail = null;
            for (int lintYear = lintFromyear; lintYear <= lintToYear; lintYear++)
            {
                lbusBenefitCalculationYearlyDetail = null;
                lintYearlySuspendibleMonths = 0;
                if (lbusBenefitCalculationDetail != null)
                {
                    busBenefitCalculationYearlyDetail lbusExistingReEmployedYearlyDetail = null;
                    if (lbusBenefitCalculationDetail != null)
                    {
                        if (lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.Where(item => item.icdoBenefitCalculationYearlyDetail.plan_year == lintYear && item.icdoBenefitCalculationYearlyDetail.reemployed_flag == busConstant.FLAG_YES).Count() == 0)
                        {
                            lbusBenefitCalculationYearlyDetail = new busBenefitCalculationYearlyDetail { icdoBenefitCalculationYearlyDetail = new cdoBenefitCalculationYearlyDetail() };
                            lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.reemployed_flag = busConstant.FLAG_YES;
                            lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.plan_year = lintYear;
                            lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.Add(lbusBenefitCalculationYearlyDetail);
                        }
                        else
                        {
                            lbusBenefitCalculationYearlyDetail = lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.Where(item => item.icdoBenefitCalculationYearlyDetail.plan_year == lintYear && item.icdoBenefitCalculationYearlyDetail.reemployed_flag == busConstant.FLAG_YES).FirstOrDefault();
                        }

                    }
                }
                if (idictWorkHrsAfterRetirement.Keys.Contains(lintYear))
                {
                    foreach (int month in idictWorkHrsAfterRetirement[lintYear].Keys)
                    {
                        busBenefitCalculationNonsuspendibleDetail lbusBenefitCalculationsuspendibleDetail = new busBenefitCalculationNonsuspendibleDetail() { icdoBenefitCalculationNonsuspendibleDetail = new cdoBenefitCalculationNonsuspendibleDetail() };
                        //SuspendibleHoursChange
                        if (idictWorkHrsAfterRetirement[lintYear][month] >= ibusCalculation.GetSuspendibleHoursValue(lintYear, month))
                        {
                            if (lintYear == this.icdoBenefitCalculationHeader.retirement_date_option_2.Year && month < this.icdoBenefitCalculationHeader.retirement_date_option_2.Month)
                                continue;
                            lintYearlySuspendibleMonths++;
                            lbusBenefitCalculationsuspendibleDetail.LoadData(lintYear, month, idictWorkHrsAfterRetirement[lintYear][month]);
                            if (lbusBenefitCalculationYearlyDetail != null)
                            {
                                if (lbusBenefitCalculationYearlyDetail.iclbBenefitCalculationNonsuspendibleDetail.IsNull())
                                    lbusBenefitCalculationYearlyDetail.iclbBenefitCalculationNonsuspendibleDetail = new Collection<busBenefitCalculationNonsuspendibleDetail>();

                                lbusBenefitCalculationYearlyDetail.iclbBenefitCalculationNonsuspendibleDetail.Add(lbusBenefitCalculationsuspendibleDetail);
                            }
                        }
                        else
                        {
                            if (this.ibusPerson.iclbPersonSuspendibleMonth.Where(item => item.icdoPersonSuspendibleMonth.plan_year == lintYear && item.icdoPersonSuspendibleMonth.suspendible_month_value == Convert.ToString(month)).Count() > 0)
                            {
                                lintYearlySuspendibleMonths++;
                            }
                        }
                    }
                    lintSuspendibleMonths = lintSuspendibleMonths + lintYearlySuspendibleMonths;
                    if (lbusBenefitCalculationYearlyDetail.IsNotNull())
                    {
                        lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.suspendible_months_count = lintYearlySuspendibleMonths;
                    }
                }

            }
            if (lbusBenefitCalculationDetail.IsNotNull())
            {
                int lintLastCompYear = Convert.ToInt32(lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.Max(item => item.icdoBenefitCalculationYearlyDetail.plan_year));
                if (!lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.IsNullOrEmpty() &&
                    lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.Where(item => !(item.icdoBenefitCalculationYearlyDetail.reemployed_flag == busConstant.FLAG_YES) && item.icdoBenefitCalculationYearlyDetail.plan_year == lintLastCompYear).Count() > 0)
                {
                    lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.Where(item => !(item.icdoBenefitCalculationYearlyDetail.reemployed_flag == busConstant.FLAG_YES) && item.icdoBenefitCalculationYearlyDetail.plan_year == lintLastCompYear).FirstOrDefault().icdoBenefitCalculationYearlyDetail.ee_derived_amount = idecEEDerivedComponent;
                }
            }

            lintNonSuspendibleMonths = lintTotalMonths - lintSuspendibleMonths;
            return lintNonSuspendibleMonths;
        }
        #endregion

    }




}
