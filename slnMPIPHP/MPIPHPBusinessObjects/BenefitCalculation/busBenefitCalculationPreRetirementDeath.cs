
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
    /// <summary>05/11/2012 : Kunal Arora
    /// Lump Sum Eligibility As of Earliest Retirement Date.
    /// Life Annuity : Eligibility As of Benefit Com Date.
    /// Post Retirement Death : All Local : MPI Plan Formulas.
    /// </summary>
    [Serializable]
    public class busBenefitCalculationPreRetirementDeath : busBenefitCalculationHeader
    {

        #region properties
        public Collection<cdoPlanBenefitRate> iclbcdoPlanBenefitRate { get; set; }
        public Collection<busBenefitCalculationOptions> iclbbusBenefitCalculationOptions { get; set; }

        public DateTime idtBenefitCommencementDate { get; set; }
        public bool lblIsNew { get; set; }

        /// <summary>
        /// UVHP & EE Contribution variables 
        /// </summary>

        public decimal idecUVHPInterest { get; set; }
        public decimal idecUVHPPartialInterest { get; set; }

        public decimal idecNonVestedEEPartialInterest { get; set; }
        public decimal idecEEPartialInterest { get; set; }
        public decimal idecVestedEEInterest { get; set; }
        public decimal idecNonVestedEEInterest { get; set; }
        public decimal idecEEPlusInterest { get; set; }

        public decimal idecVestedEEContribution { get; set; }
        public decimal idecNonVestedEEContribution { get; set; }
        public decimal idecUVHPContribution { get; set; }
        public decimal idecUVHPPlusInterest { get; set; }

        public decimal idecSurvivorAmountForCor { get; set; }

        public decimal idecTotalEEUVHP { get; set; }

        public decimal idecFinalPercentSurvivorAccrued { get; set; }

        public decimal idecFinalPercentSurvivorEEAmount { get; set; }

        public decimal idecFinalPercentSurviviorEEUVHPAmount { get; set; }

        public decimal idecFinalPercentSurviviorIAPAmount { get; set; }
        public decimal idecFinalPercentSurviviorL161SpecialAccountAmount { get; set; }
        public decimal idecFinalPercentSurviviorL52SpecialAccountAmount { get; set; }

        public bool iblnQualifiedYrs { get; set; }
        /// <summary>
        /// 1 yr rule valid for all plans,except L700
        /// </summary>
        public bool iblnCheckIfSurvivorIsSpouse { get; set; }

        public decimal idecAgeDiff { get; set; }//Spouse- part(Full Age)

        public int iintParticipantEarliestRetrAge { get; set; }
        public int iintSpouseEarliestRetrAge { get; set; }

        public bool iblnCheckIfPreRetPostElection { get; set; }
        public busPayeeAccount ibusPayeeAccount { get; set; }

        public busDocumentProcessCrossref ibusDocumentProcessCrossref { get; set; }

        public string istrSurvivorFullName { get; set; }
        public string istrSurvivorLastName { get; set; }
        public string istrSurvivorPrefix { get; set; }
        public string istrEstimatedMailDate { get; set; }

        public string istrShowAnnuity { get; set; }
        public string istrHasLifeOptionAnyPlan { get; set; }
        public string istrHasLumpOptionAnyPlan { get; set; }

        public string istrHasLumpMPI { get; set; }
        public string istrHasLifeMPI { get; set; }
        public string istrL52 { get; set; }

        public string istrHasLifeIAP { get; set; }
        public string istrHasLumpIAP { get; set; }

        public string istrHasBothEEUV { get; set; }
        public string istrHasOnlyEE { get; set; }
        public string istrHasonlyUVHP { get; set; }


        public string istrNormalRetrDt { get; set; }

        public string ldtBenName  { get; set; }
        public string ldtBenPrefix  { get; set; }
        public string ldtBenAddrLine1  { get; set; }
        public string ldtBenAddrLine2 { get; set; }
        public string ldtBenAddrCity   { get; set; }
        public string ldtBenAddrState  { get; set; }
        public string ldtBenAddrCountry  { get; set; }
        public string ldtBenAddrZip { get; set; }
        //PIR 862
        public DateTime idtPaymentDate { get; set; }

        #region Calculation Detail Letter

        public decimal idecParticipantAccruedBenefit { get; set; }
        public decimal idecERF { get; set; }
       
        public decimal idecJSFactor { get; set; }
        public decimal idecPartJS50 { get; set; }        
        public decimal idecPresentValueFactor { get; set; }
        public decimal idecSpouseJS { get; set; }
        public decimal idecLump { get; set; }

        #endregion Calculation Detail Letter

        public string istrSurvivorAdrCorStreet1 { get; set; }
        public string istrSurvivorAdrCorStreet2 { get; set; }
        public string istrSurvivorAdrCountryDesc { get; set; }

        public string istrPlanDesc { get; set; }

        #endregion

        #region override

        public override void BeforeValidate(Sagitec.Common.utlPageMode aenmPageMode)
        {
            this.ibusBenefitApplication.CheckIfQualifiedSpouseinDeath();
            this.ibusBenefitApplication.DetermineVesting();

            this.CheckQualifiedSpouseExists();

            if (this.icdoBenefitCalculationHeader.istrSurvivorTypeValue == busConstant.SURVIVOR_TYPE_PER && this.icdoBenefitCalculationHeader.istrSurvivorMPID.IsNotNullOrEmpty())
            {
                this.icdoBenefitCalculationHeader.organization_id = 0;
                this.icdoBenefitCalculationHeader.istrOrganizationName = string.Empty;
            }
            else
            {
                this.icdoBenefitCalculationHeader.beneficiary_person_id = 0;
                this.icdoBenefitCalculationHeader.beneficiary_person_name = string.Empty;
                if (this.icdoSurvivorDetails != null)
                {
                    this.icdoSurvivorDetails.idecAgeAtEarlyRetirement = 0;
                    this.icdoSurvivorDetails.idecSurvivorAgeAtDeath = 0;
                    this.icdoSurvivorDetails.idtDateOfMarriage = DateTime.MinValue;
                }
            }
            if (!string.IsNullOrEmpty(this.icdoBenefitCalculationHeader.istrSurvivorMPID))
            {
                GetBeneficiaryDetails(this.icdoBenefitCalculationHeader.istrSurvivorMPID);
            }
            if (this.ibusBenefitApplication.icdoBenefitApplication.benefit_type_value.IsNullOrEmpty() && this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE)
            {
                this.ibusBenefitApplication.icdoBenefitApplication.benefit_type_value = busConstant.BENEFIT_TYPE_DEATH_PRE_RETIREMENT;
            }

            this.icdoBenefitCalculationHeader.benefit_commencement_date = DateTime.Now.GetLastDayofMonth().AddDays(1);

            if (this.icdoBenefitCalculationHeader.date_of_death != DateTime.MinValue)
            {
                if (this.icdoBenefitCalculationHeader.retirement_date != DateTime.MinValue)
                {
                    LoadPreRetirementDeathInitialData();
                    SetUpVariablesForDeath(this.icdoBenefitCalculationHeader.benefit_commencement_date);
                }
                else
                {
                    LoadPreRetirementDeathInitialData();
                    SetUpVariablesForDeath(this.icdoBenefitCalculationHeader.benefit_commencement_date);


                }
            }

            //Earliest Retirement Age To be shown on maintenance form , icdoBenefitCalculationHeader.age: age as of benefit commencement date
            if (this.icdoBenefitCalculationHeader.ienuObjectState == ObjectState.Insert)
                lblIsNew = true;
            else
                lblIsNew = false;

            //LoadAllRetirementContributions();

            if (ibusPerson.iclbPersonAccount != null && !ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).IsNullOrEmpty())
            {
                LoadAllRetirementContributions(ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault());
            }
            else
            {
                LoadAllRetirementContributions(null);
            }
            this.iblnCalculateMPIPPBenefit = this.iblnCalculateIAPBenefit = true;

            Collection<cdoPlan> lclbFunds = new Collection<cdoPlan>();
            if (this.icdoBenefitCalculationHeader.iintPlanId == busConstant.MPIPP_PLAN_ID)
            {
                lclbFunds = this.ibusBenefitApplication.GetSubPlan(busConstant.MPIPP_PLAN_ID);
                if (!lclbFunds.IsNullOrEmpty() && lclbFunds.Where(i => i.plan_code == busConstant.EE_UVHP).Count() > 0)
                    this.iblnCalcualteUVHPBenefit = this.iblnCalcualteNonVestedEEBenefit = true;

                lclbFunds = this.ibusBenefitApplication.GetSubPlan(busConstant.IAP_PLAN_ID);
                if (!lclbFunds.IsNullOrEmpty() && lclbFunds.Where(i => i.plan_code == busConstant.L52_SPL_ACC).Count() > 0)
                    this.iblnCalculateL52SplAccBenefit = true;

                if (!lclbFunds.IsNullOrEmpty() && lclbFunds.Where(i => i.plan_code == busConstant.L161_SPL_ACC).Count() > 0)
                    this.iblnCalculateL161SplAccBenefit = true;
            }

            if (this.icdoBenefitCalculationHeader.iintPlanId == busConstant.IAP_PLAN_ID)
            {
                lclbFunds = this.ibusBenefitApplication.GetSubPlan(busConstant.IAP_PLAN_ID);
                if (!lclbFunds.IsNullOrEmpty() && lclbFunds.Where(i => i.plan_code == busConstant.L52_SPL_ACC).Count() > 0)
                    this.iblnCalculateL52SplAccBenefit = true;

                if (!lclbFunds.IsNullOrEmpty() && lclbFunds.Where(i => i.plan_code == busConstant.L161_SPL_ACC).Count() > 0)
                    this.iblnCalculateL161SplAccBenefit = true;
            }

            base.BeforeValidate(aenmPageMode);
        }


        public override void BeforePersistChanges()
        {
            if (!this.lblIsNew)
            {
                FlushOlderCalculations();
            }

            Setup_Death_Calculations();
            base.BeforePersistChanges();
        }

        public override void AfterPersistChanges()
        {
            decimal ldecLocal700GauranteedAmt = 0;
            busActiveRetireeIncreaseContract lbusActiveRetireeIncreaseContract = new busActiveRetireeIncreaseContract();
            Collection<busActiveRetireeIncreaseContract> lclbActiveRetireeIncreaseContract = new Collection<busActiveRetireeIncreaseContract>();

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

                        #region Calculate Retiree Increase

                        int lintPayementCount = (int)DBFunction.DBExecuteScalar("cdoPayeeAccount.GetCountOfPaymentMade",
                                                    new object[2] { this.iintOriginalPayeeAccountId, icdoBenefitCalculationHeader.person_id }, iobjPassInfo.iconFramework,
                                                    iobjPassInfo.itrnFramework);


                        //PROD PIR 127
                        if (((icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT
                            && lintPayementCount >= 1) || icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL) &&
                            icdoBenefitCalculationHeader.retirement_date < DateTime.Now && icdoBenefitCalculationHeader.iintPlanId != busConstant.IAP_PLAN_ID)
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

                                if (ldtRetireeIncreaseDate >= icdoBenefitCalculationHeader.benefit_commencement_date && (ldtDROCommencementDate == DateTime.MinValue ||
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
                                        Convert.ToDecimal(lbusRetireeIncreaseContract.icdoActiveRetireeIncreaseContract.percent_increase_value), busConstant.BENEFIT_TYPE_DEATH_PRE_RETIREMENT);
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
            GetAgeAsOfCalculationDate();

            // Check Eligibility

            if (this.ibusBaseActivityInstance.IsNotNull())
            {
                this.SetProcessInstanceParameters();
            }
        }


        public override void ValidateHardErrors(utlPageMode aenmPageMode)
        {
            base.ValidateHardErrors(aenmPageMode);
            utlError lobjError = null;
            if (this.icdoBenefitCalculationHeader.iintPlanId != busConstant.MPIPP_PLAN_ID && this.icdoBenefitCalculationHeader.iintPlanId != busConstant.IAP_PLAN_ID)
            {
                if (this.ibusPerson.iclbPersonAccount.Where(local => local.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).Count() > 0)
                {
                    DateTime ldDate = this.icdoBenefitCalculationHeader.date_of_death;
                    if (ldDate != DateTime.MinValue)
                    {
                        if (!this.ibusPerson.iclbPersonAccount.IsNullOrEmpty() && this.ibusPerson.iclbPersonAccount.Where(local => local.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).Count() > 0)
                        {
                            DateTime ldtMergerDate = this.ibusPerson.iclbPersonAccount.Where(local => local.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().icdoPersonAccount.idtMergerDate;
                            if (ldDate < ldtMergerDate)
                            {
                                lobjError = AddError(5454, "");
                                this.iarrErrors.Add(lobjError);
                            }
                        }
                    }
                }
            }
            if (this.icdoBenefitCalculationHeader.survivor_percentage != 100 && this.iblnCheckIfSurvivorIsSpouse)
            {
                lobjError = AddError(0, "Survivor is a qualified spouse.Percentage cannot be less than 100.");
                this.iarrErrors.Add(lobjError);
            }
        }


        public ArrayList btn_ApproveCalculation()
        {
            if (this.iarrErrors.IsNull())
                this.iarrErrors = new ArrayList();

            this.iarrErrors = base.btn_ApproveCalculation();

            if (!this.iarrErrors.IsNullOrEmpty())
                return this.iarrErrors;

            if (this.iarrErrors.Count == 0 && this.icdoBenefitCalculationHeader.calculation_type_value != busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE)
            {
                bool lblnDeductPrevIAPBalance = false;
                #region PAYEE ACCOUNT RELATED LOGIC (PAYMENT - SPRINT 3.0) -- Abhishek
                int flag = 0;
                if (flag != 1)  // DONE ON PURPOSE TO AVOID PAYEE ACCOUNT CODE TO BE EXECUTED FOR NOW
                {
                    //this.ValidateHardErrors(utlPageMode.All);

                    //R3view - The Logic of this IF condition and whether the LAMBDA is safe 

                    foreach (busBenefitCalculationDetail lbusBenefitCalculationDetail in this.iclbBenefitCalculationDetail)
                    {
                        if (lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.Count > 0 &&
                            lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.survivor_amount <= Decimal.Zero
                            && !(lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                            icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT))//PIR 985 10262015
                        {
                            utlError lobjError = new utlError();
                            lobjError = AddError(6057, "");//R3view 
                            this.iarrErrors.Add(lobjError);
                        }
                        else if ((lbusBenefitCalculationDetail.iclbBenefitCalculationOptions != null && lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.Count > 0 && lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.survivor_amount > Decimal.Zero)
                            || (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                            icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT))//PIR 985 10262015
                        {
                            int lintBenefitAccountID = 0;
                            int lintPayeeAccountID = 0;
                            string lstrFundsType = String.Empty;

                            //Variables Required for Benefit updation or Insertion
                           
                            //Benefit Account Related
                            decimal ldecAccountOwnerStartingTaxableAmount = 0.0M;
                            decimal ldecAccountOwnerStartingNonTaxableAmount = 0.0M;
                            decimal ldecAccountOwnerStartingGrossAmount = 0.0M;
                            decimal ldecTotalInterestAmount = 0.0M;

                            busPayeeAccount lbusPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
                            busPayeeBenefitAccount lbusPayeeBenefitAccount = new busPayeeBenefitAccount { icdoPayeeBenefitAccount = new cdoPayeeBenefitAccount() };

                            DateTime ldteBenefitBeginDate = this.icdoBenefitCalculationHeader.benefit_commencement_date;
                            if (ldteBenefitBeginDate == DateTime.MinValue)
                            {
                                ldteBenefitBeginDate = this.ibusBenefitApplication.icdoBenefitApplication.retirement_date;
                            }

                            switch (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.plan_id)
                            {
                                //R3view - Based on Per Plan we need to set the TAXABLE and NON-TAXABLE ITEMS
                                case busConstant.MPIPP_PLAN_ID:
                                    if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.ee_flag == busConstant.FLAG_YES && lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.uvhp_flag == busConstant.FLAG_YES)
                                    {
                                        if (this.ibusBenefitApplication.CheckAlreadyVested(busConstant.MPIPP) && iblnCheckIfSurvivorIsSpouse)
                                        {
                                            ldecAccountOwnerStartingNonTaxableAmount = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.non_vested_ee_amount +
                                                                                       lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.total_uvhp_contribution_amount;

                                            //ldecAccountOwnerStartingTaxableAmount = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.non_vested_ee_interest +
                                            //                                        lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.total_uvhp_interest_amount;

                                            ldecTotalInterestAmount = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.non_vested_ee_interest +
                                            lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.total_uvhp_interest_amount;

                                            ldecAccountOwnerStartingTaxableAmount = ldecTotalInterestAmount;

                                        }
                                        else
                                        {
                                            //Prod PIR 243 : As discussed no check required at the time of payee account creation will be handled manually by adding no tax item in tax withholding.

                                            ldecAccountOwnerStartingNonTaxableAmount = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.non_vested_ee_amount +
                                           lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.total_uvhp_contribution_amount + lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.vested_ee_amount;

                                            //ldecAccountOwnerStartingTaxableAmount = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.non_vested_ee_interest +
                                            //                                        lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.total_uvhp_interest_amount + lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.vested_ee_interest;

                                            ldecTotalInterestAmount = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.non_vested_ee_interest +
                                            lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.total_uvhp_interest_amount + lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.vested_ee_interest;

                                            ldecAccountOwnerStartingTaxableAmount = ldecTotalInterestAmount;

                                        }

                                        ldecAccountOwnerStartingGrossAmount = ldecAccountOwnerStartingNonTaxableAmount + ldecAccountOwnerStartingTaxableAmount;
                                        lstrFundsType = busConstant.FundTypeEEandUVHPCombined;

                                    }
                                    else
                                    {
                                        ldecAccountOwnerStartingNonTaxableAmount = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.vested_ee_amount;

                                        //ldecAccountOwnerStartingTaxableAmount = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.vested_ee_interest;

                                        ldecTotalInterestAmount = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.vested_ee_interest;

                                        //PROD PIR 612
                                        decimal idecSurvivorBenefitAmount = 0M;
                                        if (lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.Count() > 0 && lbusBenefitCalculationDetail.iclbBenefitCalculationOptions != null)
                                            idecSurvivorBenefitAmount = lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.FirstOrDefault().icdoBenefitCalculationOptions.survivor_amount;
                                        if (idecSurvivorBenefitAmount > 200M)
                                            ldecAccountOwnerStartingTaxableAmount = idecSurvivorBenefitAmount - lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.vested_ee_amount;
                                        else if (idecSurvivorBenefitAmount < 200M)
                                            ldecAccountOwnerStartingTaxableAmount = idecSurvivorBenefitAmount - (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.vested_ee_amount + lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.vested_ee_interest);

                                      
                                        ldecAccountOwnerStartingGrossAmount = ldecAccountOwnerStartingNonTaxableAmount + ldecAccountOwnerStartingTaxableAmount;
                                    }
                                    // Visible only for MPI
                                    break;

                                case busConstant.IAP_PLAN_ID:
                                    //GROSS - IAP ACCOUNT BALANCE  - TILL DATE
                                    if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.l52_spl_acc_flag != busConstant.FLAG_YES && lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.l161_spl_acc_flag != busConstant.FLAG_YES)
                                    {
                                        ldecAccountOwnerStartingTaxableAmount = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.early_reduced_benefit_amount + lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.qdro_offset;
                                        ldecAccountOwnerStartingGrossAmount = ldecAccountOwnerStartingTaxableAmount;
                                    }
                                    else if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES)
                                    {

                                        ldecAccountOwnerStartingTaxableAmount = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.early_reduced_benefit_amount + lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.qdro_offset;
                                        ldecAccountOwnerStartingGrossAmount = ldecAccountOwnerStartingTaxableAmount;
                                        lstrFundsType = busConstant.FundTypeLocal52SpecialAccount;
                                    }
                                    else if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES)
                                    {
                                        ldecAccountOwnerStartingTaxableAmount = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.early_reduced_benefit_amount + lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.qdro_offset;
                                        ldecAccountOwnerStartingGrossAmount = ldecAccountOwnerStartingTaxableAmount;
                                        lstrFundsType = busConstant.FundTypeLocal161SpecialAccount;
                                    }
                                    break;

                                default:
                                    break;

                            }
                           
                            //Benefit Account
                            lintBenefitAccountID = busPayeeAccountHelper.IsBenefitAccountExists(lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.person_account_id,
                                                                                                 busConstant.BENEFIT_TYPE_DEATH_PRE_RETIREMENT, lstrFundsType,
                                                                                                 this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.benefit_application_detail_id, 0);  //R3view  the Query and code for this one.

                            lintBenefitAccountID = lbusPayeeBenefitAccount.ManagePayeeBenefitAccount(lintBenefitAccountID, this.icdoBenefitCalculationHeader.person_id,
                                                                              lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.person_account_id,
                                                                              ldecAccountOwnerStartingTaxableAmount, ldecAccountOwnerStartingNonTaxableAmount, ldecAccountOwnerStartingGrossAmount, lstrFundsType);
                                                        
                            //Payee Account //R3view this code
                            if (this.icdoBenefitCalculationHeader.beneficiary_person_id > 0)
                                lintPayeeAccountID = busPayeeAccountHelper.IsPayeeAccountExists(this.icdoBenefitCalculationHeader.beneficiary_person_id, lintBenefitAccountID, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_BENEFICIARY, busConstant.BENEFIT_TYPE_DEATH_PRE_RETIREMENT, false, lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.plan_id,
                                    astrBenefitOption : this.iclbBenefitCalculationDetail.First().iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.istrBenefitOptionDescription,
                                    0, null, this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.benefit_application_detail_id); //10 Percent

                            else if (this.icdoBenefitCalculationHeader.organization_id > 0)
                                lintPayeeAccountID = busPayeeAccountHelper.IsPayeeAccountExists(this.icdoBenefitCalculationHeader.organization_id, lintBenefitAccountID, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_BENEFICIARY, busConstant.BENEFIT_TYPE_DEATH_PRE_RETIREMENT, true, lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.plan_id,
                                    astrBenefitOption:  this.iclbBenefitCalculationDetail.First().iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.istrBenefitOptionDescription,
                                    0, null, this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.benefit_application_detail_id); //10 Percent

                            lblnDeductPrevIAPBalance = false;
                            busPayeeAccount lbusIAPPayeeAccount = null;


                            if (this.icdoBenefitCalculationHeader.lump_sum_payment == busConstant.FLAG_YES)
                            {
                                DataTable ldtbResult = new DataTable();

                                ldtbResult = busBase.Select<cdoPayeeAccount>(
                                      new string[5] { "PERSON_ID", "ACCOUNT_RELATION_VALUE", "PAYEE_BENEFIT_ACCOUNT_ID", "BENEFIT_ACCOUNT_TYPE_VALUE", "PLAN_BENEFIT_ID" },
                                      new object[5] { this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_BENEFICIARY, lintBenefitAccountID, busConstant.DEATH_PRE_RETIREMENT_MAINTANENCE_FORM, this.iclbBenefitCalculationDetail.First().iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.plan_benefit_id }, null, null);
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
                            decimal ldecMinimumGuaranteeAmount = 0M;

                            if (this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID)
                            {
                                if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.ee_flag == busConstant.FLAG_YES && lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.uvhp_flag == busConstant.FLAG_YES)
                                {
                                    this.ibusCalculation.ProcessQDROOffset(lbusBenefitCalculationDetail, this.icdoBenefitCalculationHeader.person_id, ref ldecNonTaxableBeginningBalance, true, true,
                                        false, false, icdoBenefitCalculationHeader.calculation_type_value);

                                    if (ldecAccountOwnerStartingNonTaxableAmount > 0)
                                    {
                                        //Ticket#73070
                                        ldecNonTaxableBeginningBalance = ldecAccountOwnerStartingNonTaxableAmount - lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.idecAlt_payee_ee_contribution;
                                        GetSurvivorPercentAmount(ref ldecNonTaxableBeginningBalance, this.icdoBenefitCalculationHeader.iintPlanId);
                                    }
                                }
                                else
                                {
                                    this.ibusCalculation.ProcessQDROOffset(lbusBenefitCalculationDetail, this.icdoBenefitCalculationHeader.person_id, ref ldecNonTaxableBeginningBalance, true, false,
                                                                           false, false, icdoBenefitCalculationHeader.calculation_type_value);
                                    if (ldecAccountOwnerStartingNonTaxableAmount > 0)
                                    {
                                        //Ticket#73070
                                        ldecNonTaxableBeginningBalance = ldecAccountOwnerStartingNonTaxableAmount - lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.idecAlt_payee_ee_contribution;
                                        GetSurvivorPercentAmount(ref ldecNonTaxableBeginningBalance, this.icdoBenefitCalculationHeader.iintPlanId);
                                    }
                                }
                            }

                            if (this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID)
                            {
                                ldecMinimumGuaranteeAmount = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.vested_ee_amount + lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.vested_ee_interest;
                                GetSurvivorPercentAmount(ref ldecMinimumGuaranteeAmount, this.icdoBenefitCalculationHeader.iintPlanId);
                            }

                            DateTime ldteTermCertainEndDate = new DateTime();
                            string lstrFamilyRelationshipValue = string.Empty;

                            //R3view -- IF Term Year Certain Option FIND the end Date 
                            LoadPlanBenefitsForPlan(this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.plan_id);
                            iintTermCertainMonths = busConstant.ZERO_INT;
                            iintTermCertainMonths = busPayeeAccountHelper.IsTermCertainBenefitOption(lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.plan_benefit_id, this.iclbcdoPlanBenefit);
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

                            //Family Relationship value
                            //DataTable ldtbRelationshipValue = busBase.Select("cdoRelationship.GetRelationType", new object[2] { this.icdoBenefitCalculationHeader.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id });
                            //if (ldtbRelationshipValue.Rows.Count > 0)
                            //{
                            //    lstrFamilyRelationshipValue = Convert.ToString(ldtbRelationshipValue.Rows[0]["RELATIONSHIP_VALUE"]);
                            //}

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

                            //CHECK QDRO here in CONTEXT OF EE to determine PAYEE's starting NON-TAXABLE Amount

                            //R3view -- NonTaxable Beginning Balance   
                            if (this.icdoBenefitCalculationHeader.beneficiary_person_id > 0)
                            {
                                string lstrAccountRelationship = string.Empty;

                                //Below Code commented as in Pre Retirement Account Relationship should always be Beneficiary.

                                //if (!this.iblnCheckIfSurvivorIsSpouse)
                                //    this.CheckQualifiedSpouseExists();
                                //if (this.iblnCheckIfSurvivorIsSpouse)
                                //{
                                //    lstrAccountRelationship = busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_JOINT_ANNUITANT;
                                //}
                                //else
                                //{
                                lstrAccountRelationship = busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_BENEFICIARY;
                                //}
                                DataTable ldtbRelationshipValue = busBase.Select("cdoRelationship.GetRelationType", new object[2] { this.icdoBenefitCalculationHeader.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id });
                                if (ldtbRelationshipValue.Rows.Count > 0)
                                {
                                    lstrFamilyRelationshipValue = Convert.ToString(ldtbRelationshipValue.Rows[0]["RELATIONSHIP_VALUE"]);
                                }
                                lintPayeeAccountID = lbusPayeeAccount.ManagePayeeAccount(lintPayeeAccountID, this.icdoBenefitCalculationHeader.beneficiary_person_id, 0,
                                                                                      lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.benefit_application_detail_id,
                                                                                      lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.benefit_calculation_detail_id,
                                                                                      0, 0, lintBenefitAccountID, busConstant.BENEFIT_TYPE_DEATH_PRE_RETIREMENT, this.icdoBenefitCalculationHeader.istrRetirementType,
                                                                                      ldteBenefitBeginDate, DateTime.MinValue, lstrAccountRelationship, lstrFamilyRelationshipValue,
                                                                                      ldecMinimumGuaranteeAmount, ldecNonTaxableBeginningBalance, lintPlanBenefitId,//lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.plan_benefit_id,
                                                                                      ldteTermCertainEndDate, busConstant.FLAG_NO, busConstant.FLAG_NO, lblnAdjustmentPaymentFlag);
                            }
                            else if (this.icdoBenefitCalculationHeader.organization_id > 0)
                            {
                                DataTable ldtbRelationshipValue = busBase.Select("cdoRelationship.GetOrgRelationType", new object[2] { this.icdoBenefitCalculationHeader.person_id, this.icdoBenefitCalculationHeader.organization_id });
                                if (ldtbRelationshipValue.Rows.Count > 0)
                                {
                                    lstrFamilyRelationshipValue = Convert.ToString(ldtbRelationshipValue.Rows[0]["RELATIONSHIP_VALUE"]);
                                }
                                lintPayeeAccountID = lbusPayeeAccount.ManagePayeeAccount(lintPayeeAccountID, 0, this.icdoBenefitCalculationHeader.organization_id,
                                                                                          lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.benefit_application_detail_id,
                                                                                          lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.benefit_calculation_detail_id,
                                                                                          0, 0, lintBenefitAccountID, busConstant.BENEFIT_TYPE_DEATH_PRE_RETIREMENT, this.icdoBenefitCalculationHeader.istrRetirementType,
                                                                                          ldteBenefitBeginDate, DateTime.MinValue, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_BENEFICIARY, lstrFamilyRelationshipValue,
                                                                                          ldecMinimumGuaranteeAmount, ldecNonTaxableBeginningBalance, lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.FirstOrDefault().icdoBenefitCalculationOptions.plan_benefit_id,
                                                                                          ldteTermCertainEndDate, busConstant.FLAG_NO, busConstant.FLAG_NO, lblnAdjustmentPaymentFlag);

                            }

                            lbusPayeeAccount.LoadNextBenefitPaymentDate();
                            DateTime ldteNextBenefitPaymentDate = lbusPayeeAccount.idtNextBenefitPaymentDate;//R3vview this once with Vinovin


                            decimal ldecTaxableAmount = 0M;
                            decimal ldecNonTaxableAmount = 0M;

                            //R3view -- First Parameter Should be maybe Retirement or Payment Date Review the Function too 

                            if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.ee_flag == busConstant.FLAG_YES || lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.uvhp_flag == busConstant.FLAG_YES)
                            {
                                //Prod PIR 36 : 9th July 2013
                                //decimal ldecNonTaxableInterest = (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.non_vested_ee_interest - lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.total_ee_interest_qdro_offset) + (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.total_uvhp_interest_amount - lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.total_uvhp_interest_qdro_offset);

                                //if (ldecNonTaxableInterest > 200)
                                //    ldecNonTaxableInterest = 0;
                                busPayeeAccountHelper.CalculateMonthlyPaymentComponents(DateTime.MinValue, lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.survivor_amount,
                                                      ref ldecNonTaxableAmount, ref ldecTaxableAmount, lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.istrBenefitOptionDescription.Contains(busConstant.LUMP_SUM_DESCRIPTION) ? ldecNonTaxableBeginningBalance : 0);
                            }
                            else if (lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.istrBenefitOptionDescription.Contains(busConstant.LUMP_SUM_DESCRIPTION)
                                  && this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID)
                            {
                                //10 Percent
                                if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT)
                                {
                                    if (lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.survivor_amount
                                        - lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.paid_amount > 0)
                                        busPayeeAccountHelper.CalculateMonthlyPaymentComponents(DateTime.MinValue, lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.survivor_amount
                                            - lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.paid_amount,
                                                                                   ref ldecNonTaxableAmount, ref ldecTaxableAmount, ldecNonTaxableBeginningBalance);
                                }
                                else
                                    busPayeeAccountHelper.CalculateMonthlyPaymentComponents(DateTime.MinValue, lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.survivor_amount,
                                                                               ref ldecNonTaxableAmount, ref ldecTaxableAmount, ldecNonTaxableBeginningBalance);
                            }
                            else if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID)
                            {
                                //Ticket# 68700
                                //decimal ldecTotalGross = lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.survivor_amount
                                //            - lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.paid_amount;
                                decimal ldecTotalGross = lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.survivor_percent_amount
                                            - lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.paid_amount;

                                if (ldecTotalGross > decimal.Zero)
                                {
                                    busPayeeAccountHelper.CalculateMonthlyPaymentComponents(DateTime.MinValue, ldecTotalGross,
                                                                             ref ldecNonTaxableAmount, ref ldecTaxableAmount, 0);
                                }
                                else
                                {
                                    //Create OverPayment.
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
                                busPayeeAccountHelper.CalculateMonthlyPaymentComponents(DateTime.MinValue, lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.survivor_amount,
                                    ref ldecNonTaxableAmount, ref ldecTaxableAmount, lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.monthly_exclusion_amount.IsNull() ? 0 : lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.monthly_exclusion_amount);
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

                            if (ldteBenefitBeginDate < ldteNextBenefitPaymentDate && (!lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.istrBenefitOptionDescription.Contains(busConstant.LUMP_SUM_DESCRIPTION))
                                && lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.plan_id != busConstant.IAP_PLAN_ID &&
                            this.icdoBenefitCalculationHeader.calculation_type_value != busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT)
                            {
                                lbusPayeeAccount.LoadPayeeAccountPaymentItemType();
                                lbusPayeeAccount.CreateRetroPayments(lbusPayeeAccount, ldteNextBenefitPaymentDate, ldteBenefitBeginDate, lintPayeeAccountID, busConstant.RETRO_PAYMENT_INITIAL);
                                
                                // PROD PIR 127
                                this.CreatePayeeAccountForRetireeIncrease(lbusPayeeAccount,lintBenefitAccountID, lstrFamilyRelationshipValue, ldecNonTaxableBeginningBalance, ldteNextBenefitPaymentDate, 0, busConstant.BENEFIT_TYPE_DEATH_PRE_RETIREMENT, lbusPayeeAccount.icdoPayeeAccount.account_relation_value);
                            }

                            if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT)
                            {
                                //PIR 985 10262015
                                if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID && lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.early_reduced_benefit_amount < 0)
                                    ;
                                else
                                    lbusPayeeAccount.CreateReviewPayeeAccountStatus(ablnFromApprovedCalc: true);// PIR 1055

                                //PIR 993  added check for Lumpsum benefit type for MPIPP plan.
                                if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.plan_id != busConstant.IAP_PLAN_ID 
                                    && (!lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.istrBenefitOptionDescription.Contains(busConstant.LUMP_SUM_DESCRIPTION)))
                                {
                                    //Payment Adjustments - Benefit Adjustment Batch
                                    bool lblnAdjustmentCalculationForRetireeIncrease = false;
                                    if (iclbDisabilityRetireeIncrease != null && iclbDisabilityRetireeIncrease.Count > 0)
                                        lblnAdjustmentCalculationForRetireeIncrease = true;

                                    Collection<busBenefitMonthwiseAdjustmentDetail> lclbBenefitMonthwiseAdjustmentDetail = new Collection<busBenefitMonthwiseAdjustmentDetail>();
                                    lclbBenefitMonthwiseAdjustmentDetail = ibusCalculation.GetAmountActuallyPaid(lbusPayeeAccount, lbusPayeeAccount.icdoPayeeAccount.benefit_begin_date, lbusPayeeAccount.idtLastBenefitPaymentDate, lblnAdjustmentCalculationForRetireeIncrease);
                                    ibusCalculation.CalculateAmountShouldHaveBeenPaid(lbusPayeeAccount, ref lclbBenefitMonthwiseAdjustmentDetail);
                                    ibusCalculation.CalculateRetireeIncreaseAmountShouldHaveBeenPaid(lbusPayeeAccount, iclbDisabilityRetireeIncrease, ref lclbBenefitMonthwiseAdjustmentDetail); // PROD PIR 581
                                    ibusCalculation.CreateOverpaymentUnderPayment(lbusPayeeAccount, lclbBenefitMonthwiseAdjustmentDetail, busConstant.RETRO_PAYMENT_ADJUSTMENT_BATCH);
                                }
                                //ibusCalculation.CreatePayeeAccountForRetireeIncrease(lintBenefitAccountID, lstrFamilyRelationshipValue, ldecNonTaxableBeginningBalance, ldteNextBenefitPaymentDate,
                                // lintPayeeAccountID, iclbDisabilityRetireeIncrease, this, null, iclbBenefitCalculationDetail, null);
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
                }

                #endregion

                #region POST Entries in Retirement Contribution Table
                if (icdoBenefitCalculationHeader.calculation_type_value != busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT)
                {
                    foreach (busBenefitCalculationDetail lbusBenefitCalculationDetail in this.iclbBenefitCalculationDetail)
                    {
                        LoadPlanBenefitsForPlan(this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.plan_id);

                        if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID)
                        {
                            int lintPersonAccountId = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().icdoPersonAccount.person_account_id;

                            if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.ee_flag != busConstant.FLAG_YES
                                && lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.uvhp_flag != busConstant.FLAG_YES)
                            {
                                if (this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.computational_year == this.icdoBenefitCalculationHeader.benefit_commencement_date.Year && item.icdoPersonAccountRetirementContribution.contribution_type_value == "EE" && item.icdoPersonAccountRetirementContribution.transaction_type_value == "INTR" && item.icdoPersonAccountRetirementContribution.contribution_subtype_value == "VEST").Count() == 0)
                                {
                                    decimal ldecPriorYearInterest = decimal.Zero;

                                    decimal ldecPartialEEInterestAmount = ibusCalculation.CalculatePartialEEInterest(this.ibusBenefitApplication.icdoBenefitApplication.retirement_date, this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault(),
                                                                            true, false, iclbPersonAccountRetirementContribution, out ldecPriorYearInterest);
                                    busPersonAccountRetirementContribution lbusPersonAccountRetirementContribution = null;

                                    if (ldecPartialEEInterestAmount > decimal.Zero)
                                    {
                                        lbusPersonAccountRetirementContribution = new busPersonAccountRetirementContribution() { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };
                                        lbusPersonAccountRetirementContribution.InsertPersonAccountRetirementContirbution(lintPersonAccountId, busGlobalFunctions.GetLastDateOfComputationYear(this.icdoBenefitCalculationHeader.retirement_date.Year - 1).AddDays(1),
                                               DateTime.Now, this.ibusBenefitApplication.icdoBenefitApplication.retirement_date.Year, adecEEInterestAmount: ldecPartialEEInterestAmount-ldecPriorYearInterest,
                                               astrTransactionType: busConstant.TRANSACTION_TYPE_INTEREST, astrContributionType: busConstant.CONTRIBUTION_TYPE_EE, astrContributionSubtype: busConstant.CONTRIBUTION_SUBTYPE_VESTED, aintReferenceID: this.icdoBenefitCalculationHeader.benefit_calculation_header_id);
                                    }
                                    if (ldecPriorYearInterest > decimal.Zero)
                                    {
                                        lbusPersonAccountRetirementContribution = new busPersonAccountRetirementContribution() { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };
                                        lbusPersonAccountRetirementContribution.InsertPersonAccountRetirementContirbution(lintPersonAccountId, busGlobalFunctions.GetLastDateOfComputationYear(this.icdoBenefitCalculationHeader.retirement_date.Year - 1).AddDays(1),
                                               DateTime.Now, this.ibusBenefitApplication.icdoBenefitApplication.retirement_date.AddYears(-1).Year, adecEEInterestAmount:  ldecPriorYearInterest,
                                               astrTransactionType: busConstant.TRANSACTION_TYPE_INTEREST, astrContributionType: busConstant.CONTRIBUTION_TYPE_EE, astrContributionSubtype: busConstant.CONTRIBUTION_SUBTYPE_VESTED, aintReferenceID: this.icdoBenefitCalculationHeader.benefit_calculation_header_id);
                                    
                                    }
                                }
                            }
                            else
                            {
                                if (this.ibusBenefitApplication.CheckAlreadyVested(busConstant.MPIPP) && iblnCheckIfSurvivorIsSpouse)
                                {
                                    if (this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.computational_year == this.icdoBenefitCalculationHeader.benefit_commencement_date.Year && item.icdoPersonAccountRetirementContribution.contribution_type_value == busConstant.CONTRIBUTION_TYPE_UVHP && item.icdoPersonAccountRetirementContribution.transaction_type_value == "INTR").Count() == 0)
                                    {
                                        decimal ldecPriorYearUVHPInterest = decimal.Zero;
                                        decimal ldecPartialUVHPInterestAmount = ibusCalculation.CalculatePartialUVHPInterest(this.ibusBenefitApplication.icdoBenefitApplication.retirement_date, lintPersonAccountId,out ldecPriorYearUVHPInterest);
                                        if (ldecPartialUVHPInterestAmount - ldecPriorYearUVHPInterest > decimal.Zero)
                                        {
                                            busPersonAccountRetirementContribution lbusPersonAccountRetirementContribution = new busPersonAccountRetirementContribution() { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };
                                            lbusPersonAccountRetirementContribution.InsertPersonAccountRetirementContirbution(lintPersonAccountId, busGlobalFunctions.GetLastDateOfComputationYear(this.icdoBenefitCalculationHeader.retirement_date.Year - 1).AddDays(1),
                                                     DateTime.Now, this.ibusBenefitApplication.icdoBenefitApplication.retirement_date.Year, adecUVHPInterestAmount: ldecPartialUVHPInterestAmount-ldecPriorYearUVHPInterest, astrTransactionType: busConstant.TRANSACTION_TYPE_INTEREST
                                                     , astrContributionType: busConstant.CONTRIBUTION_TYPE_UVHP, aintReferenceID: this.icdoBenefitCalculationHeader.benefit_calculation_header_id);
                                        }
                                        if (ldecPriorYearUVHPInterest > decimal.Zero)
                                        {
                                            busPersonAccountRetirementContribution lbusPersonAccountRetirementContribution = new busPersonAccountRetirementContribution() { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };
                                            lbusPersonAccountRetirementContribution.InsertPersonAccountRetirementContirbution(lintPersonAccountId, busGlobalFunctions.GetLastDateOfComputationYear(this.icdoBenefitCalculationHeader.retirement_date.Year - 1).AddDays(1),
                                                     DateTime.Now, this.ibusBenefitApplication.icdoBenefitApplication.retirement_date.AddYears(-1).Year, adecUVHPInterestAmount: ldecPriorYearUVHPInterest, astrTransactionType: busConstant.TRANSACTION_TYPE_INTEREST
                                                     , astrContributionType: busConstant.CONTRIBUTION_TYPE_UVHP, aintReferenceID: this.icdoBenefitCalculationHeader.benefit_calculation_header_id);
                                        
                                        }
                                    }
                                }
                                else
                                {
                                    busPersonAccountRetirementContribution lbusPersonAccountRetirementContribution = new busPersonAccountRetirementContribution() { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };

                                    #region Post Vested EE Contribution
                                    if (this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.computational_year == this.icdoBenefitCalculationHeader.benefit_commencement_date.Year && item.icdoPersonAccountRetirementContribution.contribution_type_value == busConstant.CONTRIBUTION_TYPE_EE && item.icdoPersonAccountRetirementContribution.transaction_type_value == "INTR" && item.icdoPersonAccountRetirementContribution.contribution_subtype_value == busConstant.CONTRIBUTION_SUBTYPE_VESTED).Count() == 0)
                                    {
                                        decimal ldecPriorYearInterest = decimal.Zero;
                                        decimal ldecPartialVestedEEInterestAmount = ibusCalculation.CalculatePartialEEInterest(this.ibusBenefitApplication.icdoBenefitApplication.retirement_date,
                                            this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault(),
                                            true, false, iclbPersonAccountRetirementContribution,out ldecPriorYearInterest);

                                        if (ldecPartialVestedEEInterestAmount-ldecPriorYearInterest > decimal.Zero)
                                        {
                                            lbusPersonAccountRetirementContribution.InsertPersonAccountRetirementContirbution(lintPersonAccountId, busGlobalFunctions.GetLastDateOfComputationYear(this.ibusBenefitApplication.icdoBenefitApplication.retirement_date.Year - 1).AddDays(1),
                                               DateTime.Now, this.ibusBenefitApplication.icdoBenefitApplication.retirement_date.Year, adecEEInterestAmount: ldecPartialVestedEEInterestAmount - ldecPriorYearInterest,
                                               astrTransactionType: busConstant.TRANSACTION_TYPE_INTEREST, astrContributionType: busConstant.CONTRIBUTION_TYPE_EE, astrContributionSubtype: busConstant.CONTRIBUTION_SUBTYPE_VESTED, aintReferenceID: this.icdoBenefitCalculationHeader.benefit_calculation_header_id);
                                        }
                                        if (ldecPriorYearInterest > decimal.Zero)
                                        {
                                            lbusPersonAccountRetirementContribution.InsertPersonAccountRetirementContirbution(lintPersonAccountId, busGlobalFunctions.GetLastDateOfComputationYear(this.ibusBenefitApplication.icdoBenefitApplication.retirement_date.Year - 1).AddDays(1),
                                            DateTime.Now, this.ibusBenefitApplication.icdoBenefitApplication.retirement_date.AddYears(-1).Year, adecEEInterestAmount: ldecPriorYearInterest,
                                            astrTransactionType: busConstant.TRANSACTION_TYPE_INTEREST, astrContributionType: busConstant.CONTRIBUTION_TYPE_EE, astrContributionSubtype: busConstant.CONTRIBUTION_SUBTYPE_VESTED, aintReferenceID: this.icdoBenefitCalculationHeader.benefit_calculation_header_id);

                                        }
                                    }
                                    #endregion

                                    #region Post Non Vested EE Contribution
                                    if (this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.computational_year == this.icdoBenefitCalculationHeader.benefit_commencement_date.Year && item.icdoPersonAccountRetirementContribution.contribution_type_value == busConstant.CONTRIBUTION_TYPE_EE && item.icdoPersonAccountRetirementContribution.transaction_type_value == "INTR" && item.icdoPersonAccountRetirementContribution.contribution_subtype_value == busConstant.CONTRIBUTION_SUBTYPE_NON_VESTED).Count() == 0)
                                    {
                                        decimal ldecPriorYearInterest = decimal.Zero;
                                        decimal ldecPartialNonVestedEEInterestAmount = ibusCalculation.CalculatePartialEEInterest(this.ibusBenefitApplication.icdoBenefitApplication.retirement_date,
                                            this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault(),
                                            false, true, iclbPersonAccountRetirementContribution,out ldecPriorYearInterest);

                                        if (ldecPartialNonVestedEEInterestAmount-ldecPriorYearInterest > decimal.Zero)
                                        {
                                            lbusPersonAccountRetirementContribution = new busPersonAccountRetirementContribution() { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };
                                            lbusPersonAccountRetirementContribution.InsertPersonAccountRetirementContirbution(lintPersonAccountId, busGlobalFunctions.GetLastDateOfComputationYear(this.ibusBenefitApplication.icdoBenefitApplication.retirement_date.Year - 1).AddDays(1),
                                               DateTime.Now, this.ibusBenefitApplication.icdoBenefitApplication.retirement_date.Year, adecEEInterestAmount: ldecPartialNonVestedEEInterestAmount-ldecPriorYearInterest,
                                               astrTransactionType: busConstant.TRANSACTION_TYPE_INTEREST, astrContributionType: busConstant.CONTRIBUTION_TYPE_EE, astrContributionSubtype: busConstant.CONTRIBUTION_SUBTYPE_NON_VESTED, aintReferenceID: this.icdoBenefitCalculationHeader.benefit_calculation_header_id);
                                        }
                                        if (ldecPriorYearInterest > decimal.Zero)
                                        {
                                            lbusPersonAccountRetirementContribution = new busPersonAccountRetirementContribution() { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };
                                            lbusPersonAccountRetirementContribution.InsertPersonAccountRetirementContirbution(lintPersonAccountId, busGlobalFunctions.GetLastDateOfComputationYear(this.ibusBenefitApplication.icdoBenefitApplication.retirement_date.Year - 1).AddDays(1),
                                               DateTime.Now, this.ibusBenefitApplication.icdoBenefitApplication.retirement_date.AddYears(-1).Year, adecEEInterestAmount: ldecPriorYearInterest,
                                               astrTransactionType: busConstant.TRANSACTION_TYPE_INTEREST, astrContributionType: busConstant.CONTRIBUTION_TYPE_EE, astrContributionSubtype: busConstant.CONTRIBUTION_SUBTYPE_NON_VESTED, aintReferenceID: this.icdoBenefitCalculationHeader.benefit_calculation_header_id);

                                        }
                                    }
                                    #endregion
                                    if (this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.computational_year == this.icdoBenefitCalculationHeader.benefit_commencement_date.Year && item.icdoPersonAccountRetirementContribution.contribution_type_value == busConstant.CONTRIBUTION_TYPE_UVHP && item.icdoPersonAccountRetirementContribution.transaction_type_value == "INTR").Count() == 0)
                                    {
                                        decimal ldecPriorYearUVHPInterest = decimal.Zero;
                                        decimal ldecPartialUVHPInterestAmount = ibusCalculation.CalculatePartialUVHPInterest(this.ibusBenefitApplication.icdoBenefitApplication.retirement_date, lintPersonAccountId,out ldecPriorYearUVHPInterest);

                                        if (ldecPartialUVHPInterestAmount-ldecPriorYearUVHPInterest > decimal.Zero)
                                        {
                                            lbusPersonAccountRetirementContribution = new busPersonAccountRetirementContribution() { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };
                                            lbusPersonAccountRetirementContribution.InsertPersonAccountRetirementContirbution(lintPersonAccountId, busGlobalFunctions.GetLastDateOfComputationYear(this.ibusBenefitApplication.icdoBenefitApplication.retirement_date.Year - 1).AddDays(1),
                                                    DateTime.Now, this.ibusBenefitApplication.icdoBenefitApplication.retirement_date.Year, adecUVHPInterestAmount: ldecPartialUVHPInterestAmount - ldecPriorYearUVHPInterest, astrTransactionType: busConstant.TRANSACTION_TYPE_INTEREST
                                                    , astrContributionType: busConstant.CONTRIBUTION_TYPE_UVHP, aintReferenceID: this.icdoBenefitCalculationHeader.benefit_calculation_header_id);
                                        }
                                        if (ldecPriorYearUVHPInterest > decimal.Zero)
                                        {
                                            lbusPersonAccountRetirementContribution = new busPersonAccountRetirementContribution() { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };
                                            lbusPersonAccountRetirementContribution.InsertPersonAccountRetirementContirbution(lintPersonAccountId, busGlobalFunctions.GetLastDateOfComputationYear(this.ibusBenefitApplication.icdoBenefitApplication.retirement_date.Year - 1).AddDays(1),
                                                    DateTime.Now, this.ibusBenefitApplication.icdoBenefitApplication.retirement_date.AddYears(-1).Year, adecUVHPInterestAmount: ldecPriorYearUVHPInterest, astrTransactionType: busConstant.TRANSACTION_TYPE_INTEREST
                                                    , astrContributionType: busConstant.CONTRIBUTION_TYPE_UVHP, aintReferenceID: this.icdoBenefitCalculationHeader.benefit_calculation_header_id);

                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).Count() > 0)
                {
                    bool lblnInsert = true;
                    if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT)// && !lblnDeductPrevIAPBalance) //PIR 985
                    {
                        lblnInsert = false;
                    }
                    if (lblnInsert)
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
                            int lintPersonAccountId = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().icdoPersonAccount.person_account_id;

                            if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.ee_flag != busConstant.FLAG_YES
                                && lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.uvhp_flag != busConstant.FLAG_YES)
                            {

                                DataTable ltblEEPartialInterest = Select("cdoPersonAccountRetirementContribution.GetEE&UVHPPartialInterest", new object[2] {this.icdoBenefitCalculationHeader.benefit_calculation_header_id,
                                        busConstant.CONTRIBUTION_TYPE_EE});
                                if (ltblEEPartialInterest.Rows.Count > 0 && Convert.ToString(ltblEEPartialInterest.Rows[0][0]).IsNotNullOrEmpty())
                                {
                                    ldecPartialEEInterestAmount = -(Convert.ToDecimal(ltblEEPartialInterest.Rows[0][0]));
                                    busPersonAccountRetirementContribution lbusPersonAccountRetirementContribution = new busPersonAccountRetirementContribution() { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };
                                    lbusPersonAccountRetirementContribution.InsertPersonAccountRetirementContirbution(lintPersonAccountId, busGlobalFunctions.GetLastDateOfComputationYear(this.icdoBenefitCalculationHeader.retirement_date.Year - 1).AddDays(1),
                                        DateTime.Now, this.icdoBenefitCalculationHeader.retirement_date.Year, adecEEInterestAmount: ldecPartialEEInterestAmount,
                                        //astrTransactionType: busConstant.TRANSACTION_TYPE_CANCELLED_CALCULATION,
                                        astrTransactionType: Convert.ToString(ltblEEPartialInterest.Rows[0][1]),
                                        astrContributionType: busConstant.CONTRIBUTION_TYPE_EE, astrContributionSubtype: busConstant.CONTRIBUTION_SUBTYPE_VESTED, aintReferenceID: this.icdoBenefitCalculationHeader.benefit_calculation_header_id);
                                }
                            }
                            else
                            {
                                if (this.ibusBenefitApplication.CheckAlreadyVested(busConstant.MPIPP) && this.ibusBenefitApplication.QualifiedSpouseExists)
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
                                             astrTransactionType: Convert.ToString(ltblUVHPPartialInterest.Rows[0][1]),
                                            astrContributionType: busConstant.CONTRIBUTION_TYPE_UVHP, aintReferenceID: this.icdoBenefitCalculationHeader.benefit_calculation_header_id);
                                    }
                                }
                                else
                                {
                                    //DataTable ltblPartialInterest = Select("cdoPersonAccountRetirementContribution.GetEE&UVHPPartialInterest", new object[2] {this.icdoBenefitCalculationHeader.benefit_calculation_header_id,
                                    //    busConstant.CONTRIBUTION_TYPE_EE});
                                    //if (ltblPartialInterest.IsNotNull() && ltblPartialInterest.Rows.Count > 0 && Convert.ToString(ltblPartialInterest.Rows[0][0]).IsNotNullOrEmpty())
                                    //{
                                    //    ldecPartialEEInterestAmount = -(Convert.ToDecimal(ltblPartialInterest.Rows[0][0]));
                                    //    busPersonAccountRetirementContribution lbusPersonAccountRetirementContribution = new busPersonAccountRetirementContribution() { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };
                                    //    lbusPersonAccountRetirementContribution.InsertPersonAccountRetirementContirbution(lintPersonAccountId, busGlobalFunctions.GetLastDateOfComputationYear(this.icdoBenefitCalculationHeader.retirement_date.Year - 1).AddDays(1),
                                    //        DateTime.Now, this.icdoBenefitCalculationHeader.retirement_date.Year, adecEEInterestAmount: ldecPartialEEInterestAmount,
                                    //         //astrTransactionType: busConstant.TRANSACTION_TYPE_CANCELLED_CALCULATION,
                                    //         astrTransactionType: Convert.ToString(ltblPartialInterest.Rows[0][1]),
                                    //        astrContributionType: busConstant.CONTRIBUTION_TYPE_EE, astrContributionSubtype: busConstant.CONTRIBUTION_SUBTYPE_NON_VESTED, aintReferenceID: this.icdoBenefitCalculationHeader.benefit_calculation_header_id);
                                    //}


                                    // ltblPartialInterest.Clear();
                                    DataTable ltblPartialInterest = Select("cdoPersonAccountRetirementContribution.GetEE&UVHPPartialInterest", new object[2] {this.icdoBenefitCalculationHeader.benefit_calculation_header_id,
                                        busConstant.CONTRIBUTION_TYPE_UVHP});
                                    if (ltblPartialInterest.Rows.Count > 0 && Convert.ToString(ltblPartialInterest.Rows[0][0]).IsNotNullOrEmpty())
                                    {
                                        ldecPartialUVHPInterestAmount = -(Convert.ToDecimal(ltblPartialInterest.Rows[0][0]));
                                        busPersonAccountRetirementContribution lbusPersonAccountRetirementContribution = new busPersonAccountRetirementContribution() { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };
                                        lbusPersonAccountRetirementContribution.InsertPersonAccountRetirementContirbution(lintPersonAccountId, busGlobalFunctions.GetLastDateOfComputationYear(this.icdoBenefitCalculationHeader.retirement_date.Year - 1).AddDays(1),
                                            DateTime.Now, this.icdoBenefitCalculationHeader.retirement_date.Year, adecUVHPInterestAmount: ldecPartialUVHPInterestAmount,
                                             //astrTransactionType: busConstant.TRANSACTION_TYPE_CANCELLED_CALCULATION
                                             astrTransactionType: Convert.ToString(ltblPartialInterest.Rows[0][1])
                                            , astrContributionType: busConstant.CONTRIBUTION_TYPE_UVHP, aintReferenceID: this.icdoBenefitCalculationHeader.benefit_calculation_header_id);
                                    }
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

        public override void LoadCorresProperties(string astrTemplateName)
        {
            base.LoadCorresProperties(astrTemplateName);
            LoadDeathCollectionForCorrespondence();
            LoadPropertiesForDeath(astrTemplateName);
            //Ticket#136246
            if (astrTemplateName == busConstant.ACTIVE_DEATH_NON_SPOUSE_BENE || astrTemplateName == busConstant.Pre_Retirement_Death_non_spouse_Packet || astrTemplateName == busConstant.Beneficiary_IAP_Second_Payment_packet)
            {
                DataTable ldtbGetSurvivorDetails = busBase.Select<cdoPerson>(
                  new string[1] { enmPerson.person_id.ToString() },
                  new object[1] { this.icdoBenefitCalculationHeader.beneficiary_person_id }, null, null);

                if (ldtbGetSurvivorDetails.Rows.Count > 0)
                {
                    istrSurvivorFullName = (this.icdoBenefitCalculationHeader.beneficiary_person_name).ToUpper();
                    istrSurvivorLastName = ldtbGetSurvivorDetails.Rows[0][enmPerson.last_name.ToString()].ToString();
                    istrSurvivorPrefix = ldtbGetSurvivorDetails.Rows[0][enmPerson.name_prefix_value.ToString()].ToString();
                }
                else
                {
                  ldtbGetSurvivorDetails = busBase.Select<cdoOrganization>(
                  new string[1] { enmOrganization.org_id.ToString() },
                  new object[1] { this.icdoBenefitCalculationHeader. organization_id}, null, null);

                    if (ldtbGetSurvivorDetails.Rows.Count > 0)
                    {
                        istrSurvivorFullName = ldtbGetSurvivorDetails.Rows[0][enmOrganization.org_name.ToString()].ToString();
                        istrSurvivorLastName = ldtbGetSurvivorDetails.Rows[0][enmOrganization.org_name.ToString()].ToString();
                        istrSurvivorMPID = ldtbGetSurvivorDetails.Rows[0][enmOrganization.mpi_org_id.ToString()].ToString();
                    }
                }
                                

            }
            if (astrTemplateName == busConstant.ACTIVE_DEATH_PENSION_BENEFIT_ELECTION_COVER_LETTER_DEFAULTED_ANNUITY|| astrTemplateName == busConstant.Active_Death_Beneficiary_Package_Defaulted_Annunity|| astrTemplateName == busConstant.Active_Death_Beneficiary_Package_LUMPSUM)
            {
                DataTable ldtbGenratedDate = busBase.Select("cdoBenefitCalculationHeader.GetGeneratedDate", new object[4] { this.icdoBenefitCalculationHeader.person_id, 138, 97, 44 });
                if (ldtbGenratedDate.Rows.Count > 0)
                {
                    DateTime ldtGeneratedt = Convert.ToDateTime(ldtbGenratedDate.Rows[0]["GENERATED_DATE"]);
                    istrEstimatedMailDate = String.Format("{0:MMMM dd, yyyy}", ldtGeneratedt);
                }
            }
            //Ticket#136246
            if (astrTemplateName == busConstant.ACTIVE_DEATH_BENEFICIARY_PACKAGE_COVER_LETTER_IAP_ONLY || astrTemplateName == busConstant.Pre_Retirement_Death_non_spouse_Packet || astrTemplateName == busConstant.Beneficiary_IAP_Second_Payment_packet)
            {
                DataTable ldtbBenDetails = Select("cdoPerson.GetBenificiaryAddress", new object[1] { this.icdoBenefitCalculationHeader.beneficiary_person_id });
                if (ldtbBenDetails.Rows.Count > 0)
                {
                     ldtBenName = Convert.ToString(ldtbBenDetails.Rows[0]["FIRST_NAME"]) + " " + Convert.ToString(ldtbBenDetails.Rows[0]["LAST_NAME"]);
                     ldtBenPrefix = Convert.ToString(ldtbBenDetails.Rows[0]["NAME_PREFIX_VALUE"]);
                     ldtBenAddrLine1 = Convert.ToString(ldtbBenDetails.Rows[0]["addr_line_1"]);
                     ldtBenAddrLine2 = Convert.ToString(ldtbBenDetails.Rows[0]["addr_line_2"]);
                     ldtBenAddrCity = Convert.ToString(ldtbBenDetails.Rows[0]["addr_city"]);
                     ldtBenAddrState = Convert.ToString(ldtbBenDetails.Rows[0]["addr_state_value"]);
                     ldtBenAddrCountry = Convert.ToString(ldtbBenDetails.Rows[0]["ADDR_COUNTRY_VALUE"]);
                     ldtBenAddrZip = Convert.ToString(ldtbBenDetails.Rows[0]["ADDR_ZIP_CODE"]);// +"-" + Convert.ToString(ldtbBenDetails.Rows[0]["ADDR_ZIP_4_CODE"]);

                     istrSurvivorAdrCorStreet1 = Convert.ToString(ldtbBenDetails.Rows[0]["addr_line_1"]);
                     istrSurvivorAdrCorStreet2 = Convert.ToString(ldtbBenDetails.Rows[0]["addr_line_2"]);
                     istrSurvivorAdrCountryDesc = Convert.ToString(ldtbBenDetails.Rows[0]["addr_city"]) + " " + Convert.ToString(ldtbBenDetails.Rows[0]["addr_state_value"]) + " " + Convert.ToString(ldtbBenDetails.Rows[0]["ADDR_ZIP_CODE"]);

                }
            }

        }


        #endregion

        #region public

        public void SetUpVariablesForDeath(DateTime adtCalDate)
        {
            if (this.icdoBenefitCalculationHeader.istrSurvivorTypeValue == busConstant.SURVIVOR_TYPE_PER)
            {
                this.idecSurvivorAgeAtDeath = busGlobalFunctions.CalculatePersonAge(this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth, this.icdoBenefitCalculationHeader.date_of_death);
                this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement = busGlobalFunctions.CalculatePersonAge(this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth, adtCalDate);
                this.icdoBenefitCalculationHeader.idecSurvivorFullAge = Math.Floor(this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement);
            }
            //this.ibusBenefitApplication.icdoBenefitApplication.retirement_date = adtCalDate;
            //this.ibusBenefitApplication.idecAge = busGlobalFunctions.CalculatePersonAge(this.ibusPerson.icdoPerson.idtDateofBirth, this.ibusBenefitApplication.icdoBenefitApplication.retirement_date);
            this.icdoBenefitCalculationHeader.age = busGlobalFunctions.CalculatePersonAge(this.ibusPerson.icdoPerson.idtDateofBirth, adtCalDate); //Load the AGE OF THE MAIN HEADER OBJECT AS WELL
            this.iintSpouseEarliestRetrAge = Convert.ToInt32(busGlobalFunctions.CalculatePersonAge(this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth, this.icdoBenefitCalculationHeader.retirement_date));
            //l600,666
            this.iintParticipantEarliestRetrAge = Convert.ToInt32(Math.Floor(this.ibusPerson.icdoPerson.idecAgeAtEarlyRetirement));
            this.icdoBenefitCalculationHeader.idecParticipantFullAge = Math.Floor(this.ibusBenefitApplication.idecAge);//@Commencement Date
            if (this.iblnCheckIfSurvivorIsSpouse && this.icdoBenefitCalculationHeader.survivor_percentage == decimal.Zero)
            {
                this.icdoBenefitCalculationHeader.survivor_percentage = 100;
            }
            else if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT) //PIR #534- added else condition and query in xml
            {
                //10 Percent
                DataTable ldtSurPer = Select("cdoBenefitCalculationDetail.GetBeneficiaryDistPercentage", new object[2] { this.icdoBenefitCalculationHeader.beneficiary_person_id,icdoBenefitCalculationHeader.iintPlanId });
                if (ldtSurPer.Rows.Count > 0)
                {
                    this.icdoBenefitCalculationHeader.survivor_percentage = Convert.ToDecimal(ldtSurPer.Rows[0]["DIST_PERCENT"]);
                }
            }
            if (this.icdoBenefitCalculationHeader.date_of_death != DateTime.MinValue)
            {
                this.icdoBenefitCalculationHeader.iintParticipantAgeAtDeath = Convert.ToInt32(busGlobalFunctions.CalculatePersonAge(this.ibusPerson.icdoPerson.idtDateofBirth, this.icdoBenefitCalculationHeader.date_of_death));
            }
            //idecAgeDiff = this.icdoBenefitCalculationHeader.idecParticipantFullAge - this.icdoBenefitCalculationHeader.idecSurvivorFullAge;
            if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL && this.icdoBenefitCalculationHeader.benefit_commencement_date != DateTime.MinValue)
            {
                this.ibusBenefitApplication.icdoBenefitApplication.retirement_date = adtCalDate;
                this.ibusBenefitApplication.idecAge = busGlobalFunctions.CalculatePersonAge(this.ibusPerson.icdoPerson.idtDateofBirth, this.ibusBenefitApplication.icdoBenefitApplication.retirement_date);
                SetupPreRequisites_DeathCalculations();
            }
        }

        public void SetupPreRequisites_DeathCalculations()
        {
            if (!this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.IsNullOrEmpty())
            {
                this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.ForEach(a => a.icdoPersonAccount.istrRetirementSubType = string.Empty);
                this.ibusBenefitApplication.LoadWorkHistoryandSetupPrerequisites_PreRetirementDeath();
            }
            this.icdoBenefitCalculationHeader.istrRetirementType = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().icdoPersonAccount.istrRetirementSubType;

        }

        public void Setup_Death_Calculations()
        {
            DateTime ldtVestedDate = DateTime.MinValue;
            decimal ldecTotalBenefitAmount = new decimal();
            decimal ldecLocalFrozenBenefit = new decimal();
            decimal ldecPreBisAmount = new decimal();
            decimal ldecPostBisAmount = new decimal();

            // Abhishek - The NotEligible if-condition needs to be removed. 
            // See the comments below in ValidateHardErros()
            if (!this.ibusBenefitApplication.NotEligible) //THIS IS CONSIDERING THAT ONE EACH TIME FOR LOCALS, LOCALS WILL NEVER BE GROUPED IN THE CONDITION HERE
            {
                #region SETUP BENEFIT CALCULATION DETAIL BASED ON WHAT ESTIMATE HAS BEEN ASKED FOR

                if (this.iclbBenefitCalculationDetail == null)
                {
                    this.iclbBenefitCalculationDetail = new Collection<busBenefitCalculationDetail>();
                }

                if (this.icdoBenefitCalculationHeader.iintPlanId == busConstant.MPIPP_PLAN_ID)
                {
                    #region Setup Detail Records for MPIs Estimate

                    // Create one Detail Record for MPIPP
                    ldtVestedDate = DateTime.MinValue;
                    busBenefitCalculationDetail lbusBenefitCalculationDetail = new busBenefitCalculationDetail { icdoBenefitCalculationDetail = new cdoBenefitCalculationDetail() };
                    lbusBenefitCalculationDetail.iobjMainCDO = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail;

                    //Get the VESTED DATE FROM PERSON ACCOUNT ELIGIBILITY
                    if (this.ibusBenefitApplication.CheckAlreadyVested(busConstant.MPIPP))
                    {
                        ldtVestedDate = this.ibusBenefitApplication.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date;
                    }

                    lbusBenefitCalculationDetail.LoadData(this.icdoBenefitCalculationHeader.benefit_calculation_header_id, this.icdoBenefitCalculationHeader.iintPlanId,
                        this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).First().icdoPersonAccount.person_account_id,
                        ldtVestedDate, this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().icdoPersonAccount.istrRetirementSubType, busConstant.BENEFIT_TYPE_DEATH_PRE_RETIREMENT);
                    lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.istrPlanCode = busConstant.MPIPP;

                    lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.ee_derived_benefit_amount = ibusCalculation.CalculateEEDerivedBenefitAsOfRetirementDate(this, null);

                    this.iclbBenefitCalculationDetail.Add(lbusBenefitCalculationDetail);

                    if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE)
                    {
                        ldtVestedDate = DateTime.MinValue;
                        busBenefitCalculationDetail lbusBenefitCalculationDetailIAP = new busBenefitCalculationDetail { icdoBenefitCalculationDetail = new cdoBenefitCalculationDetail() };
                        lbusBenefitCalculationDetailIAP.iobjMainCDO = lbusBenefitCalculationDetailIAP.icdoBenefitCalculationDetail;

                        if (this.ibusBenefitApplication.CheckAlreadyVested(busConstant.IAP))
                        {
                            ldtVestedDate = this.ibusBenefitApplication.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date;
                        }
                        lbusBenefitCalculationDetailIAP.LoadData(this.icdoBenefitCalculationHeader.benefit_calculation_header_id, busConstant.IAP_PLAN_ID,
                            this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.IAP_PLAN_ID).First().icdoPersonAccount.person_account_id,
                            ldtVestedDate, this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().icdoPersonAccount.istrRetirementSubType, busConstant.BENEFIT_TYPE_DEATH_PRE_RETIREMENT);
                        lbusBenefitCalculationDetailIAP.icdoBenefitCalculationDetail.istrPlanCode = busConstant.IAP;
                        this.iclbBenefitCalculationDetail.Add(lbusBenefitCalculationDetailIAP);
                    }

                    #endregion
                }
                else if (this.icdoBenefitCalculationHeader.iintPlanId == busConstant.IAP_PLAN_ID)
                {
                    #region Setup Detail Record for IAPs Estimate

                    // Create one Detail Record for IAP Plan 
                    ldtVestedDate = DateTime.MinValue;
                    busBenefitCalculationDetail lbusBenefitCalculationDetailIAP = new busBenefitCalculationDetail { icdoBenefitCalculationDetail = new cdoBenefitCalculationDetail() };
                    lbusBenefitCalculationDetailIAP.iobjMainCDO = lbusBenefitCalculationDetailIAP.icdoBenefitCalculationDetail;
                    //Get the VESTED DATE FROM PERSON ACCOUNT ELIGIBILITY
                    if (this.ibusBenefitApplication.CheckAlreadyVested(busConstant.IAP))
                    {
                        ldtVestedDate = this.ibusBenefitApplication.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date;
                    }

                    lbusBenefitCalculationDetailIAP.LoadData(this.icdoBenefitCalculationHeader.benefit_calculation_header_id, this.icdoBenefitCalculationHeader.iintPlanId,
                        this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).First().icdoPersonAccount.person_account_id,
                        ldtVestedDate, this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().icdoPersonAccount.istrRetirementSubType, busConstant.BENEFIT_TYPE_DEATH_PRE_RETIREMENT);
                    lbusBenefitCalculationDetailIAP.icdoBenefitCalculationDetail.istrPlanCode = busConstant.IAP;
                    this.iclbBenefitCalculationDetail.Add(lbusBenefitCalculationDetailIAP);

                    #endregion
                }
                else
                {

                    #region Setup Detail Record for Locals
                    //BENEFIT-CALCULATION-DETAILS COLLECTION INITIAL LOAD CAN HAPPEN HERE SINCE WE NEED VESTED DATE A LOT OF STUFF AS WELL> 
                    busBenefitCalculationDetail lbusBenefitCalculationDetail = new busBenefitCalculationDetail { icdoBenefitCalculationDetail = new cdoBenefitCalculationDetail() };
                    lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail = new Collection<busBenefitCalculationYearlyDetail>();
                    lbusBenefitCalculationDetail.iclbBenefitCalculationOptions = new Collection<busBenefitCalculationOptions>(); //VERY IMP Required at many places
                    lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.benefit_calculation_header_id = this.icdoBenefitCalculationHeader.benefit_calculation_header_id;
                    lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.person_account_id = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).First().icdoPersonAccount.person_account_id;
                    //lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.plan_id = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.First().icdoPersonAccount.plan_id;
                    lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.istrPlanCode = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).First().icdoPersonAccount.istrPlanCode;
                    //FOR LOCAL THIS WORK FINE SINCE I HAVE TO CREATE ONLY ONE CALC DETAIL OBJECT
                    lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.plan_id = this.icdoBenefitCalculationHeader.iintPlanId;
                    //Get the VESTED DATE FROM PERSON ACCOUNT ELIGIBILITY
                    string lstrPlanCode = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).First().icdoPersonAccount.istrPlanCode;
                    if (this.ibusBenefitApplication.CheckAlreadyVested(this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).First().icdoPersonAccount.istrPlanCode))
                        lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.vested_date = this.ibusBenefitApplication.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date;
                    lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.benefit_subtype_id = busConstant.RETIREMENT_TYPE_CODE_ID;
                    lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.benefit_subtype_value = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == lstrPlanCode).First().icdoPersonAccount.istrRetirementSubType;
                    lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.status_id = busConstant.BenefitCalculation.CALCULATION_STATUS_TYPE_CODE_ID;
                    lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.status_value = busConstant.BenefitCalculation.CALCULATION_STATUS_TYPE_PENDING;

                    this.iclbBenefitCalculationDetail.Add(lbusBenefitCalculationDetail);
                    #endregion

                }
                #endregion

                #region SWITCH CASE - INITIATE CALCULATION BASED ON THE REQUIRED PLAN'S ESTIMATE
                if (!this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.IsNullOrEmpty())
                {
                    int lintPersonAccountId = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).First().icdoPersonAccount.person_account_id;
                    switch (this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).First().icdoPersonAccount.istrPlanCode)
                    {
                        case busConstant.Local_161:
                            CalculateLocalBenefits(busConstant.CodeValueAll, busConstant.BOOL_FALSE);

                            break;

                        case busConstant.Local_52:
                            CalculateLocal52Benefit(busConstant.CodeValueAll, this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_52).First().icdoPersonAccount.istrRetirementSubType);
                            break;

                        case busConstant.Local_600:
                            CalculateLocalBenefits(busConstant.CodeValueAll, busConstant.BOOL_FALSE);
                            break;

                        case busConstant.Local_666:
                            CalculateLocalBenefits(busConstant.CodeValueAll, busConstant.BOOL_FALSE);
                            break;

                        case busConstant.LOCAL_700:
                            CalculateLocalBenefits(busConstant.CodeValueAll, busConstant.BOOL_FALSE);
                            break;

                        case busConstant.MPIPP:
                            if (this.ibusBenefitApplication.aclbPersonWorkHistory_MPI != null && this.ibusBenefitApplication.aclbPersonWorkHistory_MPI.Count > 0)
                            {
                                this.CalculateMPIBenefitOptions(busConstant.CodeValueAll);
                                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().icdoBenefitCalculationDetail.ee_derived_benefit_amount = ibusCalculation.CalculateEEDerivedBenefitAsOfRetirementDate(this, null);
                            }

                            if (this.ibusBenefitApplication.aclbPersonWorkHistory_IAP != null && this.ibusBenefitApplication.aclbPersonWorkHistory_IAP.Count > 0 && this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE)
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
        }

        #region Local Calculations Business Logic
        /*
        public void CalculateLocal161Benefit(string astrBenefitOption, string astrRetirementSubType)
        {
            //First STEP GET THE FROZEN AMOUNT and OTHER INFORMATION FROM SOMEWHERE lIKE EE,IAP ACC BALANCE etc 
            //LOAD THAT BUSINESS OBJECT WHICH SHOULD BE A PART OF busBenefitCalculationHeader
            //And also the other BUSINESS OBJECTS THAT NEED TO BE LOADED
            if (!this.iclbPersonAccountRetirementContribution.IsNullOrEmpty())
            {
                int lintPersonAccountId = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_161).First().icdoPersonAccount.person_account_id;
                decimal ldecFrozenBenefitAmount = this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).First().icdoPersonAccountRetirementContribution.local_frozen_benefit_amount;
                //CHECK ON THIS WITH DEBRA
                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_161_PLAN_ID).First().icdoBenefitCalculationDetail.unreduced_benefit_amount = ldecFrozenBenefitAmount;

                decimal ldecTotalBenefitAmount = new decimal();
                //This Amount Will Probably also go into the Calculation Detail Table as one of the fields                

                if (astrRetirementSubType == busConstant.RETIREMENT_TYPE_REDUCED_EARLY ||
            astrRetirementSubType == busConstant.RETIREMENT_TYPE_SPL_REDUCED_EARLY)
                {
                    // This is for Early Retirement. Fetch the ERF from the lookup table
                    decimal ldecERF = this.GetEarlyReductionFactor(this.icdoBenefitCalculationHeader.iintPlanId, busConstant.BENEFIT_TYPE_RETIREMENT,
                                                                    astrRetirementSubType, Convert.ToInt32(this.icdoBenefitCalculationHeader.age));
                    ldecTotalBenefitAmount = ldecFrozenBenefitAmount * ldecERF;

                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_161_PLAN_ID).First().icdoBenefitCalculationDetail.early_reduction_factor = ldecERF;
                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_161_PLAN_ID).First().icdoBenefitCalculationDetail.early_reduced_benefit_amount = ldecTotalBenefitAmount;

                }
                else if (astrRetirementSubType == busConstant.RETIREMENT_TYPE_NORMAL ||
                         astrRetirementSubType == busConstant.RETIREMENT_TYPE_UNREDUCED_EARLY)
                {
                    // Normal Or Unreduced Retirement 
                    ldecTotalBenefitAmount = ldecFrozenBenefitAmount;
                }
                else
                {
                    // Retirement Type is Late or MD
                    // To Do - confirm if the Final Benefit Amount will be the same as Unreduced Beneft Amount.
                    ldecTotalBenefitAmount = ldecFrozenBenefitAmount;
                }

                decimal ldecBenefitOptionFactor = busConstant.ZERO_DECIMAL;
                busBenefitCalculationOptions lbusBenefitCalculationOptions;
                decimal ldecimalLocalLumpSump = busConstant.ZERO_DECIMAL;
                decimal ldecSurvivorAmount = busConstant.ZERO_DECIMAL;

                if (this.ibusBenefitApplication.QualifiedSpouseExists)
                {
                    //For Calculation Life Annity signifies QJ 50 
                    // Qualified Joint And 50% Survivor Annuity Benefit Option = LIFE 
                    CalculateBenefitOptionsForLocalForQualSpouse(busConstant.LOCAL_161_PLAN_ID, ldecTotalBenefitAmount, busConstant.CodeValueAll);

                }
                else
                {
                    if (this.ibusBenefitApplication.Local161_PensionCredits > 0)
                    {
                        ldecimalLocalLumpSump = 250 * this.ibusBenefitApplication.Local161_PensionCredits;
                        if (ldecimalLocalLumpSump > 5000)
                        {
                            ldecimalLocalLumpSump = 5000;
                        }
                        lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                        lbusBenefitCalculationOptions.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_161_PLAN_ID, busConstant.LUMP_SUM), ldecBenefitOptionFactor,
                                                                Convert.ToDecimal(Math.Round(ldecimalLocalLumpSump, 2)), busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id);

                    }
                }

                
            }
        }

        public void CalculateLocal52Benefit(string astrBenefitOption, string astrRetirementSubType)
        {
            if (!this.iclbPersonAccountRetirementContribution.IsNullOrEmpty())
            {
                int lintPersonAccountId = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_161).First().icdoPersonAccount.person_account_id;
                decimal ldecFrozenBenefitAmount = this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).First().icdoPersonAccountRetirementContribution.local_frozen_benefit_amount;
                //CHECK ON THIS WITH DEBRA
                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_600_PLAN_ID).First().icdoBenefitCalculationDetail.unreduced_benefit_amount = ldecFrozenBenefitAmount;

                decimal ldecTotalBenefitAmount = new decimal();
                //This Amount Will Probably also go into the Calculation Detail Table as one of the fields                

                if (astrRetirementSubType == busConstant.RETIREMENT_TYPE_REDUCED_EARLY ||
            astrRetirementSubType == busConstant.RETIREMENT_TYPE_SPL_REDUCED_EARLY)
                {
                    // This is for Early Retirement. Fetch the ERF from the lookup table
                    decimal ldecERF = this.GetEarlyReductionFactor(this.icdoBenefitCalculationHeader.iintPlanId, busConstant.BENEFIT_TYPE_RETIREMENT,
                                                                    astrRetirementSubType, Convert.ToInt32(this.icdoBenefitCalculationHeader.age));
                    ldecTotalBenefitAmount = ldecFrozenBenefitAmount * ldecERF;

                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_600_PLAN_ID).First().icdoBenefitCalculationDetail.early_reduction_factor = ldecERF;
                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_600_PLAN_ID).First().icdoBenefitCalculationDetail.early_reduced_benefit_amount = ldecTotalBenefitAmount;

                }
                else if (astrRetirementSubType == busConstant.RETIREMENT_TYPE_NORMAL ||
                         astrRetirementSubType == busConstant.RETIREMENT_TYPE_UNREDUCED_EARLY)
                {
                    // Normal Or Unreduced Retirement 
                    ldecTotalBenefitAmount = ldecFrozenBenefitAmount;
                }
                else
                {
                    // Retirement Type is Late or MD
                    ldecTotalBenefitAmount = ldecFrozenBenefitAmount;
                }

                #region Variables Required in Switch Case
                decimal ldecFactor = new decimal();
                busBenefitCalculationOptions lbusBenefitCalculationOption;
                #endregion
                switch (astrBenefitOption)
                {
                    case busConstant.CodeValueAll:
                        foreach (cdoPlanBenefitXr lcdoPlanBenefitXr in this.ibusBenefitApplication.lclcL52BenOptionsPreDeath)
                        {
                            if (lcdoPlanBenefitXr.benefit_option_value == busConstant.LUMP_SUM)//Qual Spouse
                            {
                                CalculateBenefitOptionsForLocalForQualSpouse(busConstant.LOCAL_52_PLAN_ID, ldecTotalBenefitAmount, lcdoPlanBenefitXr.benefit_option_value);
                            }
                            if (lcdoPlanBenefitXr.benefit_option_value == busConstant.LIFE_ANNUTIY)////Qual Spouse
                            {
                                CalculateBenefitOptionsForLocalForQualSpouse(busConstant.LOCAL_52_PLAN_ID, ldecTotalBenefitAmount, lcdoPlanBenefitXr.benefit_option_value);
                            }
                            if (lcdoPlanBenefitXr.benefit_option_value == busConstant.QJ50)//Qual Spouse
                            {
                                CalculateBenefitOptionsForLocalForQualSpouse(busConstant.LOCAL_52_PLAN_ID, ldecTotalBenefitAmount, lcdoPlanBenefitXr.benefit_option_value);
                            }
                            if (lcdoPlanBenefitXr.benefit_option_value == busConstant.J100)//Qual Spouse
                            {
                                CalculateBenefitOptionsForLocalForQualSpouse(busConstant.LOCAL_52_PLAN_ID, ldecTotalBenefitAmount, lcdoPlanBenefitXr.benefit_option_value);
                            }
                            if (lcdoPlanBenefitXr.benefit_option_value == busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY)
                            {
                                if (this.ibusBenefitApplication.QualifiedSpouseExists)
                                {

                                }
                                else if (this.icdoBenefitCalculationHeader.age >= 55)
                                {

                                }
                                else
                                {

                                }
                            }
                        }
                        break;

                    case busConstant.LUMP_SUM://Qual Spouse
                        CalculateBenefitOptionsForLocalForQualSpouse(busConstant.LOCAL_52_PLAN_ID, ldecTotalBenefitAmount, busConstant.LUMP_SUM);
                        break;
                    case busConstant.LIFE_ANNUTIY:////Qual Spouse
                        CalculateBenefitOptionsForLocalForQualSpouse(busConstant.LOCAL_52_PLAN_ID, ldecTotalBenefitAmount, busConstant.LIFE_ANNUTIY);
                        break;
                    case busConstant.QJ50://Qual Spouse
                        CalculateBenefitOptionsForLocalForQualSpouse(busConstant.LOCAL_52_PLAN_ID, ldecTotalBenefitAmount, busConstant.QJ50);
                        break;
                    case busConstant.J100://Qual Spouse
                        CalculateBenefitOptionsForLocalForQualSpouse(busConstant.LOCAL_52_PLAN_ID, ldecTotalBenefitAmount, busConstant.J100);
                        break;
                    case busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY:
                        if (this.ibusBenefitApplication.QualifiedSpouseExists)
                        {

                        }
                        else if (this.icdoBenefitCalculationHeader.age >= 55)
                        {

                        }
                        else
                        {

                        }
                        break;

                }

            }
        }

        public void CalculateLocal600Benefit(string astrBenefitOption, string astrRetirementSubType)
        {
            if (!this.iclbPersonAccountRetirementContribution.IsNullOrEmpty())
            {
                int lintPersonAccountId = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_161).First().icdoPersonAccount.person_account_id;
                decimal ldecFrozenBenefitAmount = this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).First().icdoPersonAccountRetirementContribution.local_frozen_benefit_amount;
                //CHECK ON THIS WITH DEBRA
                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_600_PLAN_ID).First().icdoBenefitCalculationDetail.unreduced_benefit_amount = ldecFrozenBenefitAmount;

                decimal ldecTotalBenefitAmount = new decimal();
                //This Amount Will Probably also go into the Calculation Detail Table as one of the fields                

                if (astrRetirementSubType == busConstant.RETIREMENT_TYPE_REDUCED_EARLY ||
            astrRetirementSubType == busConstant.RETIREMENT_TYPE_SPL_REDUCED_EARLY)
                {
                    // This is for Early Retirement. Fetch the ERF from the lookup table
                    decimal ldecERF = this.GetEarlyReductionFactor(this.icdoBenefitCalculationHeader.iintPlanId, busConstant.BENEFIT_TYPE_RETIREMENT,
                                                                    astrRetirementSubType, Convert.ToInt32(this.icdoBenefitCalculationHeader.age));
                    ldecTotalBenefitAmount = ldecFrozenBenefitAmount * ldecERF;

                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_600_PLAN_ID).First().icdoBenefitCalculationDetail.early_reduction_factor = ldecERF;
                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_600_PLAN_ID).First().icdoBenefitCalculationDetail.early_reduced_benefit_amount = ldecTotalBenefitAmount;

                }
                else if (astrRetirementSubType == busConstant.RETIREMENT_TYPE_NORMAL ||
                         astrRetirementSubType == busConstant.RETIREMENT_TYPE_UNREDUCED_EARLY)
                {
                    // Normal Or Unreduced Retirement 
                    ldecTotalBenefitAmount = ldecFrozenBenefitAmount;
                }
                else
                {
                    // Retirement Type is Late or MD
                    ldecTotalBenefitAmount = ldecFrozenBenefitAmount;
                }

                //decimal ldecBenefitOptionFactor = busConstant.ZERO_DECIMAL;
                //busBenefitCalculationOptions lbusBenefitCalculationOptions;
                //decimal ldecimalLocalLumpSump = busConstant.ZERO_DECIMAL;
                //decimal ldecSurvivorAmount = busConstant.ZERO_DECIMAL;

                if (this.ibusBenefitApplication.QualifiedSpouseExists)
                {
                    //For Calculation Life Annity signifies QJ 50 
                    // Qualified Joint And 50% Survivor Annuity Benefit Option = LIFE 
                    CalculateBenefitOptionsForLocalForQualSpouse(busConstant.LOCAL_600_PLAN_ID, ldecTotalBenefitAmount, busConstant.CodeValueAll);
                }
                else if (this.ibusBenefitApplication.ibusPerson.icdoPerson.date_of_death > Convert.ToDateTime(busConstant.BenefitCalculation.DATE_01_01_1978))
                {
                    if (this.ibusBenefitApplication.Local600_PensionCredits >= 12)
                    {

                        switch (astrBenefitOption)
                        {
                            case busConstant.LIFE:

                                break;

                            case busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY:

                                break;
                        }

                    }
                }
               
            }

        }

        public void CalculateLocal666Benefit(string astrBenefitOption, string astrRetirementSubType)
        {
            if (!this.iclbPersonAccountRetirementContribution.IsNullOrEmpty())
            {
                int lintPersonAccountId = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_666).First().icdoPersonAccount.person_account_id;
                decimal ldecFrozenBenefitAmount = this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).First().icdoPersonAccountRetirementContribution.local_frozen_benefit_amount;
                //CHECK ON THIS WITH DEBRA
                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_666_PLAN_ID).First().icdoBenefitCalculationDetail.unreduced_benefit_amount = ldecFrozenBenefitAmount;

                decimal ldecTotalBenefitAmount = new decimal();

                if (astrRetirementSubType == busConstant.RETIREMENT_TYPE_REDUCED_EARLY ||
                           astrRetirementSubType == busConstant.RETIREMENT_TYPE_SPL_REDUCED_EARLY)
                {
                    // This is for Early Retirement. Fetch the ERF from the lookup table
                    decimal ldecERF = this.GetEarlyReductionFactor(this.icdoBenefitCalculationHeader.iintPlanId, busConstant.BENEFIT_TYPE_RETIREMENT,
                                                                    astrRetirementSubType, Convert.ToInt32(this.icdoBenefitCalculationHeader.age));
                    ldecTotalBenefitAmount = ldecFrozenBenefitAmount * ldecERF;

                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_666_PLAN_ID).First().icdoBenefitCalculationDetail.early_reduction_factor = ldecERF;
                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_666_PLAN_ID).First().icdoBenefitCalculationDetail.early_reduced_benefit_amount = ldecTotalBenefitAmount;

                }
                else if (astrRetirementSubType == busConstant.RETIREMENT_TYPE_NORMAL ||
                         astrRetirementSubType == busConstant.RETIREMENT_TYPE_UNREDUCED_EARLY)
                {
                    // Normal Or Unreduced Retirement 
                    ldecTotalBenefitAmount = ldecFrozenBenefitAmount;
                }
                else
                {
                    // Retirement Type is Late or MD
                    // To Do - confirm if the Final Benefit Amount will be the same as Unreduced Beneft Amount.
                    ldecTotalBenefitAmount = ldecFrozenBenefitAmount;
                }

                if (this.ibusBenefitApplication.QualifiedSpouseExists)
                {
                    CalculateBenefitOptionsForLocalForQualSpouse(busConstant.LOCAL_600_PLAN_ID, ldecTotalBenefitAmount, busConstant.CodeValueAll);
                }

            }
        }

        public void CalculateLocal700Benefit(string astrBenefitOption, string astrRetirementSubType)
        {
            //First STEP GET THE FROZEN AMOUNT and OTHER INFORMATION FROM SOMEWHERE lIKE EE,IAP ACC BALANCE etc 
            //LOAD THAT BUSINESS OBJECT WHICH SHOULD BE A PART OF busBenefitCalculationHeader
            //And also the other BUSINESS OBJECTS THAT NEED TO BE LOADED
            if (!this.iclbPersonAccountRetirementContribution.IsNullOrEmpty())
            {
                int lintPersonAccountId = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.LOCAL_700).First().icdoPersonAccount.person_account_id;
                decimal ldecFrozenBenefitAmount = this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).First().icdoPersonAccountRetirementContribution.local_frozen_benefit_amount;
                //CHECK ON THIS WITH DEBRA
                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_700_PLAN_ID).First().icdoBenefitCalculationDetail.unreduced_benefit_amount = ldecFrozenBenefitAmount;

                decimal ldecTotalBenefitAmount = new decimal();
                int CompareAge = 0;

                if (astrRetirementSubType == busConstant.RETIREMENT_TYPE_REDUCED_EARLY ||
            astrRetirementSubType == busConstant.RETIREMENT_TYPE_SPL_REDUCED_EARLY)
                {
                    // This is for Early Retirement. Fetch the ERF from the lookup table
                    decimal ldecERF = this.GetEarlyReductionFactor(this.icdoBenefitCalculationHeader.iintPlanId, busConstant.BENEFIT_TYPE_RETIREMENT,
                                                                    astrRetirementSubType, Convert.ToInt32(this.icdoBenefitCalculationHeader.age));
                    ldecTotalBenefitAmount = ldecFrozenBenefitAmount * ldecERF;

                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_700_PLAN_ID).First().icdoBenefitCalculationDetail.early_reduction_factor = ldecERF;
                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_700_PLAN_ID).First().icdoBenefitCalculationDetail.early_reduced_benefit_amount = ldecTotalBenefitAmount;

                }
                else if (astrRetirementSubType == busConstant.RETIREMENT_TYPE_NORMAL ||
                         astrRetirementSubType == busConstant.RETIREMENT_TYPE_UNREDUCED_EARLY)
                {
                    // Normal Or Unreduced Retirement 
                    ldecTotalBenefitAmount = ldecFrozenBenefitAmount;
                }
                else
                {
                    // Retirement Type is Late or MD
                    // To Do - confirm if the Final Benefit Amount will be the same as Unreduced Beneft Amount.
                    ldecTotalBenefitAmount = ldecFrozenBenefitAmount;
                }

                decimal ldecBenefitOptionFactor = busConstant.ZERO_DECIMAL;
                busBenefitCalculationOptions lbusBenefitCalculationOptions;
                decimal ldecimalLocalLumpSump = busConstant.ZERO_DECIMAL;
                decimal ldecSurvivorAmount = busConstant.ZERO_DECIMAL;

                if (this.ibusBenefitApplication.QualifiedSpouseExists)
                {
                    //For Calculation Life Annity signifies QJ 50 
                    // Qualified Joint And 50% Survivor Annuity Benefit Option = LIFE 
                    CalculateBenefitOptionsForLocalForQualSpouse(busConstant.LOCAL_700_PLAN_ID, ldecTotalBenefitAmount, busConstant.CodeValueAll);

                }
                else
                {
                    decimal ldecSpecialYears = this.ibusBenefitApplication.aclbPersonWorkHistory_MPI.Last().qualified_years_count + this.ibusBenefitApplication.Local700_PensionCredits;
                    ldecimalLocalLumpSump = 500 * ldecSpecialYears;
                    if (ldecimalLocalLumpSump > 10000)
                    {
                        ldecimalLocalLumpSump = 10000;
                    }
                    else if (ldecimalLocalLumpSump < 1000)
                    {
                        ldecimalLocalLumpSump = 1000;
                    }
                    lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                    lbusBenefitCalculationOptions.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_161_PLAN_ID, busConstant.LUMP_SUM), ldecBenefitOptionFactor,
                                                            Convert.ToDecimal(Math.Round(ldecimalLocalLumpSump, 2)), busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                            this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, Convert.ToDecimal(Math.Round(ldecimalLocalLumpSump, 2)));

                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_700_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);

                }

            }

        }
        */
        #endregion Local


        #region Local Calculation New Business Logic


        public void CalculateLocal52Benefit(string astrBenefitOption, string astrRetirementSubType)
        {
            decimal adecSurvivorLifeAnnuityAmount = busConstant.ZERO_DECIMAL;

            if (!this.iclbPersonAccountRetirementContribution.IsNullOrEmpty())
            {
                bool lblnFinal = false;
                if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL)
                {
                    lblnFinal = true;
                }
                int lintPersonAccountId = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_52).First().icdoPersonAccount.person_account_id;
                decimal ldecFrozenBenefitAmount = this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_frozen_benefit_amount);

                //CHECK ON THIS WITH DEBRA
                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_52_PLAN_ID).First().icdoBenefitCalculationDetail.unreduced_benefit_amount = ldecFrozenBenefitAmount;

                #region Reduced Benefit For R1,R2,R3,R6 : MPI RULES
                decimal ldecTotalBenefitAmount = ldecFrozenBenefitAmount;
                if (this.ibusBenefitApplication.istrLocal52RuleForDeathCalculation == busConstant.L52_RULE_1 || this.ibusBenefitApplication.istrLocal52RuleForDeathCalculation == busConstant.L52_RULE_3 ||
                    this.ibusBenefitApplication.istrLocal52RuleForDeathCalculation == busConstant.L52_RULE_2 || this.ibusBenefitApplication.istrLocal52RuleForDeathCalculation == busConstant.L52_RULE_6)
                {
                    if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE)
                    {
                        ldecTotalBenefitAmount = this.ibusCalculation.CalculateTotalBenefitAmtForDeathLocals(this.icdoBenefitCalculationHeader.istrEarliestRetirementType,
                               this.icdoBenefitCalculationHeader.retirement_date, this.ibusPerson.icdoPerson.idtDateofBirth,
                               this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_frozen_benefit_amount),
                               false, this.ibusBenefitApplication, this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_pre_bis_amount),
                              this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_post_bis_amount),
                               null, this.iclbBenefitCalculationDetail, this.icdoBenefitCalculationHeader.iintPlanId, this.iintParticipantEarliestRetrAge,
                   Convert.ToInt32(this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Max(item => item.icdoPersonAccountRetirementContribution.computational_year)),
                   this.iclbPersonAccountRetirementContribution, lblnFinal, this.ibusPerson.icdoPerson.person_id,adtEarliestRetirementDate : icdoBenefitCalculationHeader.retirement_date);//PIR 862
                    }
                    else
                    {
                        ldecTotalBenefitAmount = this.ibusCalculation.CalculateTotalBenefitAmtForDeathLocals(this.icdoBenefitCalculationHeader.istrRetirementType,
                        this.ibusBenefitApplication.icdoBenefitApplication.retirement_date, this.ibusPerson.icdoPerson.idtDateofBirth,
                        this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_frozen_benefit_amount),
                        false, this.ibusBenefitApplication, this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_pre_bis_amount),
                        this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_post_bis_amount),
                        null, this.iclbBenefitCalculationDetail, this.icdoBenefitCalculationHeader.iintPlanId,  this.iintParticipantEarliestRetrAge ,//PROD PIR 609
                        Convert.ToInt32(this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Max(item => item.icdoPersonAccountRetirementContribution.computational_year)),
                        this.iclbPersonAccountRetirementContribution, lblnFinal, this.ibusPerson.icdoPerson.person_id, adtEarliestRetirementDate: icdoBenefitCalculationHeader.retirement_date);//PIR 862

                    }
                }
                GetSurvivorPercentAmount(ref ldecTotalBenefitAmount, this.icdoBenefitCalculationHeader.iintPlanId);

                #endregion

                #region Variables Required in Switch Case
                busBenefitCalculationOptions lbusBenefitCalculationOption;
                #endregion
                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_52_PLAN_ID).FirstOrDefault().icdoBenefitCalculationDetail.local52_rule_value = this.ibusBenefitApplication.istrLocal52RuleForDeathCalculation;

                #region RULE 1
                if (this.ibusBenefitApplication.istrLocal52RuleForDeathCalculation == busConstant.L52_RULE_1 && iblnCheckIfSurvivorIsSpouse)
                {
                    //lbusBenefitCalculationOption = this.CalculateLocal52JAndS50(ldecTotalBenefitAmount);
                    if ((astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.LIFE_ANNUTIY) && (this.ibusBenefitApplication.iblnEligible4L52BenefitPreDeath))
                    {
                        //JS 50
                        this.CalculateLocalLifeAnnuityOptions(lintPersonAccountId, lblnFinal, ldecTotalBenefitAmount);
                    }
                    if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.LUMP_SUM)
                    {
                        //LumpSum
                        decimal ldecLumpSumBenefitOptionFactor = decimal.Zero;
                        if ((this.icdoBenefitCalculationHeader.benefit_commencement_date >= this.icdoBenefitCalculationHeader.retirement_date) ||
                            this.icdoBenefitCalculationHeader.benefit_commencement_date == DateTime.MinValue)
                        {
                            ldecLumpSumBenefitOptionFactor = ibusCalculation.GetLumpsumBenefitFactor(Convert.ToInt32(this.icdoBenefitCalculationHeader.idecSurvivorFullAge), this.icdoBenefitCalculationHeader.benefit_commencement_date.Year) * 12;
                            ldecLumpSumBenefitOptionFactor = Math.Round(ldecLumpSumBenefitOptionFactor, 3);
                        }
                        else
                        {
                            ldecLumpSumBenefitOptionFactor = DeferredLumpSumFactor(Convert.ToInt32(this.icdoBenefitCalculationHeader.idecSurvivorFullAge), iintSpouseEarliestRetrAge, this.icdoBenefitCalculationHeader.benefit_commencement_date.Year);//PROD PIR 816
                        }
                        bool lblnFinalCalc = false;

                        CalculateLocalLumpSumOptions(lintPersonAccountId, ldecLumpSumBenefitOptionFactor, ldecTotalBenefitAmount);
                    }
                }
                #endregion

                #region RULE 2
                else if (this.ibusBenefitApplication.istrLocal52RuleForDeathCalculation == busConstant.L52_RULE_2 && iblnCheckIfSurvivorIsSpouse)
                {
                    if ((astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.LIFE_ANNUTIY) && (this.ibusBenefitApplication.iblnEligible4L52BenefitPreDeath))
                    {
                        this.CalculateLocal52JAndS100(ldecTotalBenefitAmount);
                    }

                    if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.TEN_YEARS_TERM_CERTAIN)
                    {
                        if (this.iclbBenefitCalculationDetail.FirstOrDefault().icdoBenefitCalculationDetail.benefit_subtype_value == busConstant.RETIREMENT_TYPE_LATE)
                        {
                            this.CalculateLocal52TenYearCertainUnreduced(ldecTotalBenefitAmount);
                        }
                        else
                        {
                            //TEN YEAR Unreduced
                            this.CalculateLocal52TenYearCertainUnreduced(ldecFrozenBenefitAmount);
                        }
                    }

                }
                #endregion

                #region RULE 3
                else if (this.ibusBenefitApplication.istrLocal52RuleForDeathCalculation == busConstant.L52_RULE_3 && iblnCheckIfSurvivorIsSpouse)
                {
                    decimal ldecERF = 1;
                    decimal ldecTenYearTotalAmount = ldecTotalBenefitAmount;

                    if ((astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.LIFE_ANNUTIY) && (this.ibusBenefitApplication.iblnEligible4L52BenefitPreDeath))
                    {
                        //JS 50
                        this.CalculateLocalLifeAnnuityOptions(lintPersonAccountId, lblnFinal, ldecTotalBenefitAmount);
                    }

                    if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.TEN_YEARS_TERM_CERTAIN)
                    {
                        if (this.iclbBenefitCalculationDetail.FirstOrDefault().icdoBenefitCalculationDetail.benefit_subtype_value == busConstant.RETIREMENT_TYPE_LATE)
                        {
                            this.CalculateLocal52TenYearCertainReduced(ldecTotalBenefitAmount);
                        }
                        else
                        {
                            //Ten Year Reduced
                            this.CalculateLocal52TenYearCertainReduced(ldecFrozenBenefitAmount);
                        }
                    }
                }
                #endregion

                #region RULE 4
                else if (this.ibusBenefitApplication.istrLocal52RuleForDeathCalculation == busConstant.L52_RULE_4)
                {
                    if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.TEN_YEARS_TERM_CERTAIN)
                    {
                        if (this.iclbBenefitCalculationDetail.FirstOrDefault().icdoBenefitCalculationDetail.benefit_subtype_value == busConstant.RETIREMENT_TYPE_LATE)
                        {
                            this.CalculateLocal52TenYearCertainUnreduced(ldecTotalBenefitAmount);
                        }
                        else
                        {
                            this.CalculateLocal52TenYearCertainUnreduced(ldecFrozenBenefitAmount);
                        }
                    }
                }
                #endregion

                #region RULE 5
                else if (this.ibusBenefitApplication.istrLocal52RuleForDeathCalculation == busConstant.L52_RULE_5)
                {
                    if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.TEN_YEARS_TERM_CERTAIN)
                    {
                        if (this.iclbBenefitCalculationDetail.FirstOrDefault().icdoBenefitCalculationDetail.benefit_subtype_value == busConstant.RETIREMENT_TYPE_LATE)
                        {
                            lbusBenefitCalculationOption = this.CalculateLocal52TenYearCertainReduced(ldecTotalBenefitAmount);
                        }
                        else
                        {
                            lbusBenefitCalculationOption = this.CalculateLocal52TenYearCertainReduced(ldecFrozenBenefitAmount);
                        }
                    }
                }
                #endregion

                #region RULE 6
                else if (this.ibusBenefitApplication.istrLocal52RuleForDeathCalculation == busConstant.L52_RULE_6)
                {
                    if ((astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.LIFE_ANNUTIY) && (this.ibusBenefitApplication.iblnEligible4L52BenefitPreDeath))
                    {
                        this.CalculateLocalLifeAnnuityOptions(lintPersonAccountId, lblnFinal, ldecTotalBenefitAmount);
                    }
                    if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.TEN_YEARS_TERM_CERTAIN)
                    {
                        if (this.iclbBenefitCalculationDetail.FirstOrDefault().icdoBenefitCalculationDetail.benefit_subtype_value == busConstant.RETIREMENT_TYPE_LATE)
                        {
                            this.CalculateLocal52TenYearCertainUnreduced(ldecTotalBenefitAmount);
                        }
                        else
                        {
                            this.CalculateLocal52TenYearCertainUnreduced(ldecFrozenBenefitAmount);
                        }
                    }
                }
                #endregion

                #region Pre RetirementDeath Post Election
                if (this.iblnCheckIfPreRetPostElection)
                {
                    GetMPIBenefitOptionsForDeathPostElection(astrBenefitOption, ldecTotalBenefitAmount, decimal.Zero, decimal.Zero);
                }
                #endregion
            }
        }

        public void CalculateLocal700Benefit(string astrBenefitOption, decimal adecTotalBenefitAmount)
        {
            if (!this.iclbPersonAccountRetirementContribution.IsNullOrEmpty())
            {

                #region Variables Required in Switch Case
                decimal ldecLifeAnnuityFactor = new decimal();
                decimal ldecLumpSumFactor = new decimal();

                decimal ldecSurvivorLifeAnnuityAmount = new decimal();
                decimal ldecSurvivorLumpSumAmount = new decimal();


                decimal ldecLumpSumPensionCredits = new decimal();

                if (this.iblnCheckIfSurvivorIsSpouse && this.ibusBenefitApplication.CheckAlreadyVested(busConstant.LOCAL_700))
                {
                    //To compare Pension Credit LumpSum
                    //ldecLifeAnnuityFactor = GetBenefitFactorLocal(busConstant.LOCAL_700,busConstant.LIFE_ANNUTIY, this.icdoBenefitCalculationHeader.benefit_type_value, this.icdoBenefitCalculationHeader.idecParticipantFullAge, this.icdoBenefitCalculationHeader.idecSurvivorFullAge);

                    ldecLifeAnnuityFactor = CalculateMPIPlanLifeAnnuityFactor(Convert.ToInt32(this.icdoBenefitCalculationHeader.idecSurvivorFullAge), Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge));

                    ldecSurvivorLifeAnnuityAmount = Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecLifeAnnuityFactor) / 0.5M) * 0.5M);
                    ldecSurvivorLifeAnnuityAmount = Convert.ToDecimal(ldecSurvivorLifeAnnuityAmount * 50 / 100);

                    //ldecSurvivorLifeAnnuityAmount = CalculateLifeAnnuityBenefit(adecTotalBenefitAmount, Convert.ToInt32(this.icdoBenefitCalculationHeader.idecSurvivorFullAge), Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge));
                    //Ticket : 61531
                    ldecLumpSumFactor = Math.Round(ibusCalculation.GetLumpsumBenefitFactorByAGE(Convert.ToInt32(this.icdoBenefitCalculationHeader.idecSurvivorFullAge)) * 12,3);//Survivor Lump Sum Age
                    ldecSurvivorLumpSumAmount = Convert.ToDecimal(Math.Ceiling((ldecSurvivorLifeAnnuityAmount * ldecLumpSumFactor) / 0.5M) * 0.5M);
                }
                ldecLumpSumPensionCredits = CalculationBasedOnPensionCredits(busConstant.LOCAL_700_PLAN_ID);
                busBenefitCalculationOptions lbusBenefitCalculationOption;
                #endregion

                if (this.iblnCheckIfSurvivorIsSpouse && this.ibusBenefitApplication.CheckAlreadyVested(busConstant.LOCAL_700))
                {
                    #region Swtich Case to determine the Factors and Amounts
                    switch (astrBenefitOption)
                    {

                        case busConstant.CodeValueAll:
                            #region Life+LumpSum
                            // adding Life Annuity
                            if (this.ibusBenefitApplication.iblnEligbile4L700BenefitPreDeath)
                            {
                                lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                                lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_700_PLAN_ID, busConstant.LIFE_ANNUTIY), ldecLifeAnnuityFactor, new decimal(), busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_BENEFICIARY, this.icdoBenefitCalculationHeader.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.LIFE_ANNUTIY, Convert.ToDecimal(Math.Ceiling(ldecSurvivorLifeAnnuityAmount)));
                                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_700_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                            }
                            if (ldecLumpSumPensionCredits > ldecSurvivorLumpSumAmount)
                            {
                                ldecLumpSumFactor = 1;
                                ldecSurvivorLumpSumAmount = ldecLumpSumPensionCredits;
                            }
                            //adding lumpsum
                            lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                            lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_700_PLAN_ID, busConstant.LUMP_SUM), ldecLumpSumFactor, new decimal(), busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_BENEFICIARY, this.icdoBenefitCalculationHeader.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.LUMP_SUM, ldecSurvivorLumpSumAmount);
                            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_700_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                            #endregion
                            break;

                        case busConstant.LIFE_ANNUTIY:
                            if (this.ibusBenefitApplication.iblnEligbile4L700BenefitPreDeath)
                            {
                                lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                                lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_700_PLAN_ID, busConstant.LIFE_ANNUTIY), ldecLifeAnnuityFactor, new decimal(), busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_BENEFICIARY, this.icdoBenefitCalculationHeader.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.LIFE_ANNUTIY, Convert.ToDecimal(Math.Ceiling(ldecSurvivorLifeAnnuityAmount)));
                                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_700_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                            }
                            break;

                        case busConstant.LUMP_SUM:
                            if (ldecLumpSumPensionCredits > ldecSurvivorLumpSumAmount)
                            {
                                ldecLumpSumFactor = 1;
                                ldecSurvivorLumpSumAmount = ldecLumpSumPensionCredits;
                            }
                            //adding lumpsum
                            lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                            lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_700_PLAN_ID, busConstant.LUMP_SUM), ldecLumpSumFactor, new decimal(), busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_BENEFICIARY, this.icdoBenefitCalculationHeader.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.LUMP_SUM, ldecSurvivorLumpSumAmount);
                            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_700_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                            break;
                    }
                    #endregion
                }
                else
                {
                    switch (astrBenefitOption)
                    {
                        case busConstant.CodeValueAll:
                            lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                            lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_700_PLAN_ID, busConstant.LUMP_SUM), 1, new decimal(), busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_BENEFICIARY, this.icdoBenefitCalculationHeader.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.LUMP_SUM, ldecLumpSumPensionCredits);
                            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_700_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                            break;

                        case busConstant.LUMP_SUM:
                            lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                            lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_700_PLAN_ID, busConstant.LUMP_SUM), 1, new decimal(), busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_BENEFICIARY, this.icdoBenefitCalculationHeader.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.LUMP_SUM, ldecLumpSumPensionCredits);
                            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_700_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                            break;
                    }

                }
            }


        }

        public void CalculateLocalBenefits(string astrBenefitOption, bool ablnFinalCalc)
        {
            decimal ldecSurvivorLifeAnnuityAmount = decimal.Zero;
            if (this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).Count() > 0)
            {
                string lstrPlanCode = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().icdoPersonAccount.istrPlanCode;
                if ((iblnCheckIfSurvivorIsSpouse && this.ibusBenefitApplication.CheckAlreadyVested(lstrPlanCode) && (this.icdoBenefitCalculationHeader.iintPlanId == busConstant.LOCAL_161_PLAN_ID || this.icdoBenefitCalculationHeader.iintPlanId == busConstant.LOCAL_600_PLAN_ID || this.icdoBenefitCalculationHeader.iintPlanId == busConstant.LOCAL_666_PLAN_ID ||
                     this.icdoBenefitCalculationHeader.iintPlanId == busConstant.LOCAL_700_PLAN_ID))
                     ||
                    iblnCheckIfPreRetPostElection)
                {
                    decimal ldecTotalBenefitAmount = decimal.Zero;
                    int lintPersonAccountId = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).First().icdoPersonAccount.person_account_id;
                    if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE || astrBenefitOption == busConstant.LUMP_SUM)
                    {
                        if (this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Count() > 0)
                        {
                            ldecTotalBenefitAmount = this.ibusCalculation.CalculateTotalBenefitAmtForDeathLocals(this.icdoBenefitCalculationHeader.istrEarliestRetirementType,
                             this.icdoBenefitCalculationHeader.retirement_date, this.ibusPerson.icdoPerson.idtDateofBirth,
                             this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_frozen_benefit_amount),
                             false, this.ibusBenefitApplication, this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_pre_bis_amount),
                            this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_post_bis_amount),
                             null, this.iclbBenefitCalculationDetail, this.icdoBenefitCalculationHeader.iintPlanId, this.iintParticipantEarliestRetrAge,
                             Convert.ToInt32(this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Max(item => item.icdoPersonAccountRetirementContribution.computational_year)),
                              this.iclbPersonAccountRetirementContribution, ablnFinalCalc, this.ibusPerson.icdoPerson.person_id, adtEarliestRetirementDate: icdoBenefitCalculationHeader.retirement_date);//PIR 862
                        }
                    }
                    else
                    {
                        if (this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Count() > 0)
                        {
                            ldecTotalBenefitAmount = this.ibusCalculation.CalculateTotalBenefitAmtForDeathLocals(this.icdoBenefitCalculationHeader.istrRetirementType,
                            this.ibusBenefitApplication.icdoBenefitApplication.retirement_date, this.ibusPerson.icdoPerson.idtDateofBirth,
                            this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_frozen_benefit_amount),
                                false, this.ibusBenefitApplication, this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_pre_bis_amount),
    this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_post_bis_amount),
    null, this.iclbBenefitCalculationDetail, this.icdoBenefitCalculationHeader.iintPlanId, Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge),
    Convert.ToInt32(this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Max(item => item.icdoPersonAccountRetirementContribution.computational_year)),
    this.iclbPersonAccountRetirementContribution, ablnFinalCalc, this.ibusPerson.icdoPerson.person_id, adtEarliestRetirementDate: icdoBenefitCalculationHeader.retirement_date);//PIR 862;
                        }

                    }
                    if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.LUMP_SUM)
                    {
                        decimal ldecLumpSumBenefitOptionFactor = decimal.Zero;
                        if ((this.icdoBenefitCalculationHeader.benefit_commencement_date >= this.icdoBenefitCalculationHeader.retirement_date) ||
                            this.icdoBenefitCalculationHeader.benefit_commencement_date == DateTime.MinValue)
                        {
                            ldecLumpSumBenefitOptionFactor = ibusCalculation.GetLumpsumBenefitFactor(Convert.ToInt32(this.icdoBenefitCalculationHeader.idecSurvivorFullAge), this.icdoBenefitCalculationHeader.benefit_commencement_date.Year) * 12;
                            ldecLumpSumBenefitOptionFactor = Math.Round(ldecLumpSumBenefitOptionFactor, 3);
                        }
                        else
                        {
                            ldecLumpSumBenefitOptionFactor = DeferredLumpSumFactor(Convert.ToInt32(this.icdoBenefitCalculationHeader.idecSurvivorFullAge), iintSpouseEarliestRetrAge, this.icdoBenefitCalculationHeader.benefit_commencement_date.Year); // PROD PIR 816
                        }
                        CalculateLocalLumpSumOptions(lintPersonAccountId, ldecLumpSumBenefitOptionFactor, ldecTotalBenefitAmount);
                    }
                    if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.LIFE_ANNUTIY)
                    {
                        if (CheckIfEligibleForLocalPlan())
                        {
                            CalculateLocalLifeAnnuityOptions(lintPersonAccountId, ablnFinalCalc, ldecTotalBenefitAmount);
                        }
                    }
                    if (iblnCheckIfPreRetPostElection && astrBenefitOption != busConstant.LIFE_ANNUTIY && astrBenefitOption != busConstant.LUMP_SUM)
                    {
                        GetMPIBenefitOptionsForDeathPostElection(astrBenefitOption, ldecTotalBenefitAmount, decimal.Zero, decimal.Zero);
                    }
                }
                else if (this.icdoBenefitCalculationHeader.iintPlanId == busConstant.LOCAL_161_PLAN_ID)
                {
                    if (this.ibusBenefitApplication.CheckAlreadyVested(busConstant.Local_161))
                    {
                        if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.LUMP_SUM)
                        {
                            decimal ldecLumpSumPensionCredits = CalculationBasedOnPensionCredits(busConstant.LOCAL_161_PLAN_ID);
                            busBenefitCalculationOptions lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                            lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_161_PLAN_ID, busConstant.LUMP_SUM), 1, new decimal(), busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_BENEFICIARY, this.icdoBenefitCalculationHeader.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.LUMP_SUM, ldecLumpSumPensionCredits);
                            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().icdoBenefitCalculationDetail.present_value_amount = ldecLumpSumPensionCredits;
                            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_161_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);

                        }
                    }
                }
                else if (this.icdoBenefitCalculationHeader.iintPlanId == busConstant.LOCAL_700_PLAN_ID)
                {
                    if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.LUMP_SUM)
                    {
                        decimal ldecLumpSumPensionCredits = CalculationBasedOnPensionCredits(busConstant.LOCAL_700_PLAN_ID);
                        busBenefitCalculationOptions lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                        lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_700_PLAN_ID, busConstant.LUMP_SUM), 1, new decimal(), busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_BENEFICIARY, this.icdoBenefitCalculationHeader.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.LUMP_SUM, ldecLumpSumPensionCredits);
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().icdoBenefitCalculationDetail.present_value_amount = ldecLumpSumPensionCredits;
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_700_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    }
                }
            }
        }

        public bool CheckIfEligibleForLocalPlan()
        {
            bool lblnCheck = false;

            if ((this.ibusBenefitApplication.iblnEligbile4L161BenefitPreDeath && this.icdoBenefitCalculationHeader.iintPlanId == busConstant.LOCAL_161_PLAN_ID)
                || (this.ibusBenefitApplication.iblnEligbile4L600BenefitPreDeath && this.icdoBenefitCalculationHeader.iintPlanId == busConstant.LOCAL_600_PLAN_ID)
                || (this.ibusBenefitApplication.iblnEligbile4L666BenefitPreDeath && this.icdoBenefitCalculationHeader.iintPlanId == busConstant.LOCAL_666_PLAN_ID)
                || (this.ibusBenefitApplication.iblnEligbile4L700BenefitPreDeath && this.icdoBenefitCalculationHeader.iintPlanId == busConstant.LOCAL_700_PLAN_ID))
            {
                lblnCheck = true;
            }
            return lblnCheck;
        }

        public decimal CalculationBasedOnPensionCredits(int aintPlanID)
        {
            decimal ldecimalLocalLumpSump = new decimal();
            if (aintPlanID == busConstant.LOCAL_161_PLAN_ID)
            {
                if (this.ibusBenefitApplication.Local161_PensionCredits > 0)
                {
                    ldecimalLocalLumpSump = 250 * this.ibusBenefitApplication.Local161_PensionCredits;
                    if (ldecimalLocalLumpSump > 5000)
                    {
                        ldecimalLocalLumpSump = 5000;
                    }
                    else if (ldecimalLocalLumpSump < 1000 & ldecimalLocalLumpSump > 0)
                    {
                        ldecimalLocalLumpSump = 1000;
                    }


                }
            }
            if (aintPlanID == busConstant.LOCAL_700_PLAN_ID)
            {
                decimal ldecSpecialYears = this.ibusBenefitApplication.aclbPersonWorkHistory_MPI.Last().qualified_years_count + this.ibusBenefitApplication.Local700_PensionCredits;
                ldecimalLocalLumpSump = 500 * ldecSpecialYears;
                if (ldecimalLocalLumpSump > 10000)
                {
                    ldecimalLocalLumpSump = 10000;
                }
                else if (ldecimalLocalLumpSump < 1000 & ldecimalLocalLumpSump > 0)
                {
                    ldecimalLocalLumpSump = 1000;
                }
            }
            GetSurvivorPercentAmount(ref ldecimalLocalLumpSump, aintPlanID);
            return ldecimalLocalLumpSump;

        }

        public busBenefitCalculationOptions CalculateLocalLumpSumOptions(int aintPersonAccountId, decimal adecLumpSumBenefitOptionFactor, decimal ldecTotalBenefitAmount)
        {
            decimal ldecLumpSumPensionCredits = decimal.Zero;

            decimal ldecLumpSumAnnuityFactor = decimal.Zero;
            decimal ldecimalSurvivorLumpSum = decimal.Zero;
            busBenefitCalculationOptions lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

            if (this.iclbPersonAccountRetirementContribution.Count > 0)
            {
                ldecLumpSumAnnuityFactor = CalculateMPIPlanLifeAnnuityFactor(iintSpouseEarliestRetrAge, iintParticipantEarliestRetrAge);

                //idecJandSAnnuityFactorForLumpSum = ldecLumpSumAnnuityFactor;
                ldecimalSurvivorLumpSum = Convert.ToDecimal(ldecLumpSumAnnuityFactor * ldecTotalBenefitAmount) / 2;

                ldecimalSurvivorLumpSum = Math.Round(adecLumpSumBenefitOptionFactor * ldecimalSurvivorLumpSum, 3);
            }
            if (this.icdoBenefitCalculationHeader.iintPlanId == busConstant.LOCAL_161_PLAN_ID ||
                this.icdoBenefitCalculationHeader.iintPlanId == busConstant.LOCAL_700_PLAN_ID)
            {
                ldecLumpSumPensionCredits = CalculationBasedOnPensionCredits(this.icdoBenefitCalculationHeader.iintPlanId);
                if (ldecLumpSumPensionCredits > ldecimalSurvivorLumpSum)
                {
                    adecLumpSumBenefitOptionFactor = 1;
                    ldecimalSurvivorLumpSum = ldecLumpSumPensionCredits;
                }
            }
            lbusBenefitCalculationOptions.LoadData(ibusCalculation.GetPlanBenefitId(this.icdoBenefitCalculationHeader.iintPlanId, busConstant.LUMP_SUM), adecLumpSumBenefitOptionFactor, decimal.Zero, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_BENEFICIARY, this.icdoBenefitCalculationHeader.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.LUMP_SUM, ldecimalSurvivorLumpSum);

            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().icdoBenefitCalculationDetail.present_value_amount = ldecimalSurvivorLumpSum;

            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
            return lbusBenefitCalculationOptions;
        }

        public busBenefitCalculationOptions CalculateLocalLifeAnnuityOptions(int aintPersonAccountId, bool ablnFinalCalc, decimal ldecTotalBenefitAmount)
        {
            busBenefitCalculationOptions lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
            decimal ldecAnnuityBenefitOptionFactor = decimal.Zero;
            decimal ldecSurvivorLifeAnnuityAmount = decimal.Zero;

            if (this.iclbPersonAccountRetirementContribution.Count > 0)
            {
                if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE)
                {
                    ldecAnnuityBenefitOptionFactor = CalculateMPIPlanLifeAnnuityFactor(this.iintSpouseEarliestRetrAge, this.iintParticipantEarliestRetrAge);
                }
                else
                {
                    ldecAnnuityBenefitOptionFactor = CalculateMPIPlanLifeAnnuityFactor(Convert.ToInt32(this.icdoBenefitCalculationHeader.idecSurvivorFullAge), Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge));
                }
                ldecSurvivorLifeAnnuityAmount = Convert.ToDecimal(Math.Round(ldecTotalBenefitAmount * ldecAnnuityBenefitOptionFactor * 0.50m, 2));
            }
            lbusBenefitCalculationOptions.LoadData(ibusCalculation.GetPlanBenefitId(this.icdoBenefitCalculationHeader.iintPlanId, busConstant.LIFE_ANNUTIY), ldecAnnuityBenefitOptionFactor, ldecSurvivorLifeAnnuityAmount * 2, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_BENEFICIARY, this.icdoBenefitCalculationHeader.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.LIFE_ANNUTIY, ldecSurvivorLifeAnnuityAmount);
            lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.survivor_percent_amount = ldecTotalBenefitAmount;
            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
            return lbusBenefitCalculationOptions;

        }

        #endregion


        public void CalculateTotalEEContributionAndInterest()
        {
            idecVestedEEContribution = busConstant.ZERO_DECIMAL;
            idecNonVestedEEContribution = busConstant.ZERO_DECIMAL;
            idecVestedEEInterest = busConstant.ZERO_DECIMAL;
            idecNonVestedEEInterest = busConstant.ZERO_DECIMAL;
            idecUVHPContribution = busConstant.ZERO_DECIMAL;
            idecUVHPInterest = busConstant.ZERO_DECIMAL;
            idecTotalEEUVHP = busConstant.ZERO_DECIMAL;

            int lintPersonAccountID = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(PersonAccount => PersonAccount.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID && PersonAccount.icdoPersonAccount.person_id == this.icdoBenefitCalculationHeader.person_id).First().icdoPersonAccount.person_account_id;

            idecVestedEEContribution = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(PersonAccount => PersonAccount.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID &&
                                 PersonAccount.icdoPersonAccount.person_id == this.icdoBenefitCalculationHeader.person_id).First().icdoPersonAccount.idecVestedEE;

            idecNonVestedEEContribution = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(PersonAccount => PersonAccount.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID &&
                                 PersonAccount.icdoPersonAccount.person_id == this.icdoBenefitCalculationHeader.person_id).First().icdoPersonAccount.idecNonVestedEE;

            idecVestedEEInterest = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(PersonAccount => PersonAccount.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID &&
                                 PersonAccount.icdoPersonAccount.person_id == this.icdoBenefitCalculationHeader.person_id).First().icdoPersonAccount.idecVestedEEInterest;

            idecNonVestedEEInterest = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(PersonAccount => PersonAccount.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID &&
                                 PersonAccount.icdoPersonAccount.person_id == this.icdoBenefitCalculationHeader.person_id).First().icdoPersonAccount.idecNonVestedEEInterest;

            //Prod Issue 07012013
            idecUVHPContribution = (from item in iclbPersonAccountRetirementContribution where item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountID select item.icdoPersonAccountRetirementContribution.uvhp_amount).Sum();
            idecUVHPInterest = (from item in iclbPersonAccountRetirementContribution where item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountID select item.icdoPersonAccountRetirementContribution.uvhp_int_amount).Sum();

            idecTotalEEUVHP = idecUVHPContribution + idecUVHPInterest + idecVestedEEContribution + idecNonVestedEEContribution + idecVestedEEInterest + idecNonVestedEEInterest;

            #region CommentedCode
            /*
            DataTable ldtbRetrContribution = Select<cdoPersonAccountRetirementContribution>(
                    new string[1] { enmPersonAccountRetirementContribution.person_account_id.ToString() },
                    new object[1] { this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(PersonAccount => PersonAccount.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId && PersonAccount.icdoPersonAccount.person_id == this.icdoBenefitCalculationHeader.person_id).First().icdoPersonAccount.person_account_id }, null, enmPersonAccountRetirementContribution.computational_year.ToString()
                    );
            if (ldtbRetrContribution.Rows.Count > 0)
            {

                foreach (DataRow ldtrow in ldtbRetrContribution.Rows)
                {
                    if (ldtrow[enmPersonAccountRetirementContribution.ee_contribution_amount.ToString()] != null && !string.IsNullOrEmpty(Convert.ToString(ldtrow[enmPersonAccountRetirementContribution.ee_contribution_amount.ToString()])))
                    {
                        idecEEContribution += Convert.ToDecimal(ldtrow[enmPersonAccountRetirementContribution.ee_contribution_amount.ToString()]);
                    }
                    if (ldtrow[enmPersonAccountRetirementContribution.uvhp_amount.ToString()] != null && !string.IsNullOrEmpty(Convert.ToString(ldtrow[enmPersonAccountRetirementContribution.uvhp_amount.ToString()])))
                    {
                        idecUVHPContribution += Convert.ToDecimal(ldtrow[enmPersonAccountRetirementContribution.uvhp_amount.ToString()]);
                    }
                    if (ldtrow[enmPersonAccountRetirementContribution.uvhp_int_amount.ToString()] != null && !string.IsNullOrEmpty(Convert.ToString(ldtrow[enmPersonAccountRetirementContribution.uvhp_int_amount.ToString()])))
                    {
                        idecUVHPInterest += Convert.ToDecimal(ldtrow[enmPersonAccountRetirementContribution.uvhp_int_amount.ToString()]);
                        //idecEEInterest = ldtbRetrContribution.AsEnumerable().Sum(x => x.Field<decimal>(enmPersonAccountRetirementContribution.ee_int_amount.ToString()) );
                    }
                    if (ldtrow[enmPersonAccountRetirementContribution.ee_int_amount.ToString()] != null && !string.IsNullOrEmpty(Convert.ToString(ldtrow[enmPersonAccountRetirementContribution.ee_int_amount.ToString()])))
                    {
                        idecEEInterest += Convert.ToDecimal(ldtrow[enmPersonAccountRetirementContribution.ee_int_amount.ToString()]);
                        //idecEEInterest = ldtbRetrContribution.AsEnumerable().Sum(x => x.Field<decimal>(enmPersonAccountRetirementContribution.ee_int_amount.ToString()) );
                    }
                    //idecUVHPInterest = Convert.ToDecimal(ldtbRetrContribution.Compute("SUM(" + enmPersonAccountRetirementContribution.uvhp_int_amount.ToString() + ")","uvhp_int_amount IsNot Nothing"));
                    //idecUVHPInterest = ldtbRetrContribution.AsEnumerable().Sum(x => x.Field<decimal>(enmPersonAccountRetirementContribution.uvhp_int_amount.ToString()));
                    idecTotalEEUVHP = idecUVHPContribution + idecUVHPInterest + idecEEContribution + idecEEInterest;
                }



            }*/

            #endregion

        }

        public void CalculatePartialEEUVHPInterest()
        {
            decimal ldecBenefitInterestRate = 1;
            object lobjBenefitInterestRate = DBFunction.DBExecuteScalar("cdoBenefitInterestRate.GetBenefitInterestRate", new object[] { this.ibusBenefitApplication.icdoBenefitApplication.retirement_date.Year },
                                             iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

            if (lobjBenefitInterestRate != null)
            {
                ldecBenefitInterestRate = Convert.ToDecimal(lobjBenefitInterestRate);
            }
            decimal ldecUVHPPartialInterestAmount = Math.Round(((idecUVHPContribution + idecUVHPInterest) * ldecBenefitInterestRate) / 12 * (this.ibusBenefitApplication.icdoBenefitApplication.retirement_date.Month - 1), 2);
            decimal ldecEEPartialInterestAmount = Math.Round(((idecVestedEEContribution + idecNonVestedEEContribution + idecVestedEEInterest + idecNonVestedEEInterest) * ldecBenefitInterestRate) / 12 * (this.ibusBenefitApplication.icdoBenefitApplication.retirement_date.Month - 1), 2);
            decimal ldecTotalInterest = idecVestedEEInterest + idecNonVestedEEInterest;

            ldecTotalInterest = idecVestedEEInterest + idecNonVestedEEInterest + ldecEEPartialInterestAmount;
            idecUVHPInterest = idecUVHPInterest + ldecUVHPPartialInterestAmount;
        }

        #endregion

        #region Pension Calculation Business Logic
        /*
        private void CalculateAccruedBenefitForPension(string astrBenefitOptionValue, busPersonAccount abusPersonAccount)
        {
            decimal ldecERF = busConstant.ZERO_DECIMAL;
            decimal ldecFinalAccruedBenefitAmount = busConstant.ZERO_DECIMAL;
            decimal ldecUnreducedAccruedBenefitAmount = busConstant.ZERO_DECIMAL;
            int lintTotalQualifiedYear = busConstant.ZERO_INT;

            // Accrued Benefit = SUM( Qualified Hours for the plan year * Rate )
            // Step 1. Determine whether to use Plan B / Plan C / Plan Ca Rate table
            string lstrRateType = string.Empty;
            lstrRateType = ibusCalculation.DeterminePlanRate(this.ibusBenefitApplication.ibusPerson, this.ibusBenefitApplication.aclbPersonWorkHistory_MPI);

            // Step 3. Read the Plan tables to avoid a DB hit each time
            if (iclbcdoPlanBenefitRate == null)
                iclbcdoPlanBenefitRate = new Collection<cdoPlanBenefitRate>();

            DataTable ldtbPlanBenefitRate = busBase.Select("cdoPlanBenefitRate.Lookup", new object[] { });
            iclbcdoPlanBenefitRate = cdoDummyWorkData.GetCollection<cdoPlanBenefitRate>(ldtbPlanBenefitRate);

            // Step 4. Determine the Rate that needs to be used for Accured Benefit Calculation
            // Step 4. Determine the Rate that needs to be used for Accured Benefit Calculation
            // Sort the Pension WorkHistory Collection (aclbPersonWorkHistory_MPI) in the decending order of the Qualified Years
            this.ibusBenefitApplication.aclbPersonWorkHistory_MPI = this.ibusBenefitApplication.aclbPersonWorkHistory_MPI.OrderByDescending(item => item.year).ToList().ToCollection();
            ibusCalculation.GetRateForBenefitCalculation(this.ibusPerson.icdoPerson.istrSSNNonEncrypted, lstrRateType, this.icdoBenefitCalculationHeader.retirement_date, this.iclbcdoPlanBenefitRate, this.ibusBenefitApplication.aclbPersonWorkHistory_MPI);
            lintTotalQualifiedYear = this.ibusBenefitApplication.aclbPersonWorkHistory_MPI.FirstOrDefault().qualified_years_count;


            // Step 5. Iterate through the collection to calculate the Accrued Benefit Amount
            if (abusPersonAccount.icdoPersonAccount.istrRetirementSubType != busConstant.RETIREMENT_TYPE_LATE &&
                abusPersonAccount.icdoPersonAccount.istrRetirementSubType != busConstant.RETIREMENT_TYPE_MINIMUM_DISTRIBUTION)
            {
                foreach (cdoDummyWorkData lcdoDummyWorkData in this.ibusBenefitApplication.aclbPersonWorkHistory_MPI)
                {
                    // Calculate the Benefit Amount for the Plan Year
                    if (lcdoDummyWorkData.iintPlanYear != busConstant.BenefitCalculation.YEAR_1987)
                    {
                        // To Do 
                        // For MPI Plan you need to consider the Total Qualifies Years for MPI and Locals
                        if (lcdoDummyWorkData.qualified_years_count < Convert.ToInt32(busConstant.BenefitCalculation.QUALIFIED_YEARS_20) &&
                            lcdoDummyWorkData.qualified_hours < busConstant.MIN_HOURS_FOR_VESTED_YEAR)
                        {
                            // These Hours will not be counted for in the calculation of accrued benefit amount
                            lcdoDummyWorkData.idecBenefitAmount = busConstant.ZERO_DECIMAL;
                        }
                        else
                        {
                            lcdoDummyWorkData.idecBenefitAmount = Math.Round(lcdoDummyWorkData.qualified_hours * lcdoDummyWorkData.idecBenefitRate, 2);
                        }
                    }

                    ldecUnreducedAccruedBenefitAmount = ldecUnreducedAccruedBenefitAmount + lcdoDummyWorkData.idecBenefitAmount;

                    // Step 6. Update the YTD Collection
                    if (abusPersonAccount.icdoPersonAccount.istrRetirementSubType != busConstant.RETIREMENT_TYPE_LATE &&
                        abusPersonAccount.icdoPersonAccount.istrRetirementSubType != busConstant.RETIREMENT_TYPE_MINIMUM_DISTRIBUTION)
                    {
                        busBenefitCalculationYearlyDetail lbusBenefitCalculationYearlyDetail = new busBenefitCalculationYearlyDetail();
                        lbusBenefitCalculationYearlyDetail.LoadData(lcdoDummyWorkData.qualified_hours, lcdoDummyWorkData.bis_years_count, Convert.ToDecimal(lcdoDummyWorkData.year),
                                                                    lcdoDummyWorkData.qualified_years_count, lcdoDummyWorkData.vested_hours, lcdoDummyWorkData.vested_years_count, lcdoDummyWorkData.idecBenefitRate,
                                                                    lcdoDummyWorkData.idecBenefitAmount, lcdoDummyWorkData.idecTotalHealthHours, lcdoDummyWorkData.iintHealthCount, ldecUnreducedAccruedBenefitAmount);

                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().iclbBenefitCalculationYearlyDetail.Add(lbusBenefitCalculationYearlyDetail);
                    }
                }
            }

            int lintPlanId = busConstant.ZERO_INT;
            // Step 7. Find the Early Retirement Factor 
            switch (abusPersonAccount.icdoPersonAccount.istrRetirementSubType)
            {
                case busConstant.RETIREMENT_TYPE_REDUCED_EARLY:
                case busConstant.RETIREMENT_TYPE_SPL_REDUCED_EARLY:
                    busPersonAccount lbusPersonAccount = new busPersonAccount();
                    int lintLocalPersonAccountId = busConstant.ZERO_INT;
                    lbusPersonAccount = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.LOCAL_700_PLAN_ID || item.icdoPersonAccount.plan_id == busConstant.LOCAL_52_PLAN_ID ||
                        item.icdoPersonAccount.plan_id == busConstant.LOCAL_161_PLAN_ID).FirstOrDefault();
                    if (lbusPersonAccount.icdoPersonAccount != null && lbusPersonAccount.icdoPersonAccount.person_account_id > 0)
                    {
                        lintPlanId = lbusPersonAccount.icdoPersonAccount.plan_id;
                        lintLocalPersonAccountId = lbusPersonAccount.icdoPersonAccount.person_account_id;
                        ldecERF = ibusCalculation.CheckGrandfatheredRule(lintPlanId, lintLocalPersonAccountId, lintTotalQualifiedYear, 
                            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().icdoBenefitCalculationDetail.vested_date,
                            this.ibusPerson.icdoPerson.idtDateofBirth, this.icdoBenefitCalculationHeader.retirement_date, this.icdoBenefitCalculationHeader.age, this.ibusBenefitApplication);
                    }
                    if (ldecERF == busConstant.ZERO_DECIMAL)
                    {
                        ldecERF = this.GetEarlyReductionFactor(this.icdoBenefitCalculationHeader.iintPlanId, busConstant.BENEFIT_TYPE_RETIREMENT,
                                                                        abusPersonAccount.icdoPersonAccount.istrRetirementSubType, Convert.ToInt32(Math.Floor(this.icdoBenefitCalculationHeader.age)));
                    }
                    ldecFinalAccruedBenefitAmount = ldecUnreducedAccruedBenefitAmount * (ldecERF);
                    break;

                case busConstant.RETIREMENT_TYPE_NORMAL:
                case busConstant.RETIREMENT_TYPE_UNREDUCED_EARLY:
                    // Normal Or Unreduced Retirement 
                    ldecFinalAccruedBenefitAmount = ldecUnreducedAccruedBenefitAmount;
                    ldecERF = 1;
                    break;

                case busConstant.RETIREMENT_TYPE_LATE:
                case busConstant.RETIREMENT_TYPE_MINIMUM_DISTRIBUTION:
                    // Retirement Type is Late or MD
                    // Sort the Collection in Ascending order of the Plan Year
                    ldecUnreducedAccruedBenefitAmount = this.CalculateLateRetirementAccruedBenefitAmount(abusPersonAccount.icdoPersonAccount.person_account_id);
                    ldecFinalAccruedBenefitAmount = ldecUnreducedAccruedBenefitAmount;
                    ldecERF = 1;
                    break;

                default:
                    break;
            }

            // Step 8. Calculate the Benefits for all the Benefit Options
            this.CalculateFinalBenefitForPensionBenefitOptions(ldecFinalAccruedBenefitAmount, astrBenefitOptionValue);

            // Step 9. Calculate the Monthly Exclusion Amount & Minimum Guarantee
            this.CalculateMEAAndMG(ldecFinalAccruedBenefitAmount, abusPersonAccount.icdoPersonAccount.person_account_id);

            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().icdoBenefitCalculationDetail.unreduced_benefit_amount = ldecUnreducedAccruedBenefitAmount;
            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().icdoBenefitCalculationDetail.early_reduction_factor = ldecERF;
            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().icdoBenefitCalculationDetail.early_reduced_benefit_amount = ldecFinalAccruedBenefitAmount;
        }
        */

        private void CalculateIAPBenefitAmount(string astrBenefitOptionValue, string astrAdjustmentFlag = "")
        {
            decimal ldecIAPBalance = busConstant.ZERO_DECIMAL;
            decimal ldecL52SpecialAccountBalance = busConstant.ZERO_DECIMAL;
            decimal ldecL161SpecialAccount = busConstant.ZERO_DECIMAL;

            decimal ldecAnnunityAdjustmentMultiplier = this.GetAnnunityMultiplier();

            //Prod Issue 07012013
            DateTime ldtGivenDate = icdoBenefitCalculationHeader.benefit_commencement_date;
            //Prod Issue 07012013

            //Prod PIR- 634 : For lump Sum payment option it should not take last payment date while re-calculate, it should be based on retirement date only whenever recalculated.
            //if (icdoBenefitCalculationHeader.calculation_type_value != busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE)
            if (icdoBenefitCalculationHeader.calculation_type_value != busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE && astrBenefitOptionValue == busConstant.LUMP_SUM)
            {
                DataTable ldtFirstPaymentDate = Select("cdoBenefitCalculationHeader.GetFirstPaymentDateForBeneficiary", new object[1] { icdoBenefitCalculationHeader.person_id });
                if (ldtFirstPaymentDate != null && ldtFirstPaymentDate.Rows.Count > 0)
                {
                    ldtGivenDate = Convert.ToDateTime(ldtFirstPaymentDate.Rows[0][0]);
                }
            }

            decimal ldecIAPAnnuity = new decimal();

            decimal ldecIAPHours4QtrAlloc = busConstant.ZERO_DECIMAL;
            decimal ldecIAPHoursA2forQtrAlloc = busConstant.ZERO_DECIMAL;
            decimal ldecIAPPercent4forQtrAlloc = busConstant.ZERO_DECIMAL;

            #region To Set Values for IAP QTR Allocations
            if (this.icdoBenefitCalculationHeader.calculation_type_value != busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE)
            {
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
            }
            #endregion

            if (!this.iclbPersonAccountRetirementContribution.IsNullOrEmpty())
            {
                if (this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.IAP).Count() > 0)
                {
                    int lintPersonAccountId = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.IAP).First().icdoPersonAccount.person_account_id;
                    if (this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Count() > 0)
                    {
                        bool lblnExecuteIAPAlloc = true;
                        if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE)
                        {
                            lblnExecuteIAPAlloc = false;
                        }
                        else if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT)
                        {
                            //PIR 985
                            //if (astrAdjustmentFlag != busConstant.FLAG_YES)
                            //{
                            lblnExecuteIAPAlloc = false;
                            //}
                        }
                        busCalculation lbusCalculation = new busCalculation();
                        //PIR-534-If below condition satisfies directly fetch the IAP balance from table and there will be no allocations.
                        busIapAllocationSummary lbusIapAllocationSummary = new busIapAllocationSummary();
                        lbusIapAllocationSummary.LoadLatestAllocationSummary();
                        if (lbusIapAllocationSummary.icdoIapAllocationSummary.computation_year >= this.ibusBenefitApplication.icdoBenefitApplication.retirement_date.Year && this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT)
                        {
                            DataTable ldtbIAPBalance = new DataTable();
                          
                            //Ticket#104194
                            ldtbIAPBalance = busBase.Select("cdoPersonAccountRetirementContribution.GetIAPBalanceAndPreRetirementDeathBalance",
                                       new object[1] { lintPersonAccountId });
                            //ldecIAPBalance = Convert.ToDecimal(ldtbIAPBalance.Rows[0]["IAP_BALANCE_AMOUNT"]);
                            //this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().icdoBenefitCalculationDetail.iap_balance_amount = ldecIAPBalance;
                            ///PIR-534 Complete fixes
                            if (ldtbIAPBalance != null && ldtbIAPBalance.Rows.Count > 0)
                            {
                               ldecIAPBalance = Convert.ToDecimal(Convert.ToBoolean(ldtbIAPBalance.Rows[0]["IAP_BALANCE_AMOUNT_PRE_RTMT_DEATH"].IsDBNull()) ? busConstant.ZERO_DECIMAL : ldtbIAPBalance.Rows[0]["IAP_BALANCE_AMOUNT_PRE_RTMT_DEATH"]);
                               ldecL52SpecialAccountBalance = Convert.ToDecimal(Convert.ToBoolean(ldtbIAPBalance.Rows[0]["LOCAL52_SPECIAL_ACCT_BAL_AMOUNT_PRE_RTMT_DEATH"].IsDBNull()) ? busConstant.ZERO_DECIMAL : ldtbIAPBalance.Rows[0]["LOCAL52_SPECIAL_ACCT_BAL_AMOUNT_PRE_RTMT_DEATH"]);
                               ldecL161SpecialAccount = Convert.ToDecimal(Convert.ToBoolean(ldtbIAPBalance.Rows[0]["LOCAL161_SPECIAL_ACCT_BAL_AMOUNT_PRE_RTMT_DEATH"].IsDBNull()) ? busConstant.ZERO_DECIMAL : ldtbIAPBalance.Rows[0]["LOCAL161_SPECIAL_ACCT_BAL_AMOUNT_PRE_RTMT_DEATH"]);
                            }

                            if (this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().icdoBenefitCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES)
                                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().icdoBenefitCalculationDetail.local52_special_acct_bal_amount = ldecL52SpecialAccountBalance;
                            else if (this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().icdoBenefitCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES)
                                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().icdoBenefitCalculationDetail.local161_special_acct_bal_amount = ldecL161SpecialAccount;
                            else
                                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().icdoBenefitCalculationDetail.iap_balance_amount = ldecIAPBalance;
                        }
                        else
                        {
                            lbusCalculation.GetIAPAndSpecialAccountBalance(busConstant.BOOL_FALSE, null, this.iclbBenefitCalculationDetail, this, null,
                                                                           ldtGivenDate, ldecIAPHours4QtrAlloc, ldecIAPHoursA2forQtrAlloc, ldecIAPPercent4forQtrAlloc, lblnExecuteIAPAlloc);  //Prod Issue 07012013
                        }
                    }
                }
            }

            if (this.iblnCalculateIAPBenefit)
            {

                if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL ||
                    this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT)
                {
                    ldecIAPBalance = this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                                                                    item.icdoBenefitCalculationDetail.l52_spl_acc_flag.IsNullOrEmpty() &&
                                                                    item.icdoBenefitCalculationDetail.l161_spl_acc_flag.IsNullOrEmpty()).FirstOrDefault().icdoBenefitCalculationDetail.iap_balance_amount;

                    // Set the Default Parameters 
                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                                                                    item.icdoBenefitCalculationDetail.l52_spl_acc_flag.IsNullOrEmpty() &&
                                                                    item.icdoBenefitCalculationDetail.l161_spl_acc_flag.IsNullOrEmpty()).FirstOrDefault().icdoBenefitCalculationDetail.unreduced_benefit_amount = ldecIAPBalance;

                    //Process QDRO Offset
                    this.ibusCalculation.ProcessQDROOffset(this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID && item.icdoBenefitCalculationDetail.l52_spl_acc_flag.IsNullOrEmpty() && item.icdoBenefitCalculationDetail.l161_spl_acc_flag.IsNullOrEmpty()).First()
                        , this.ibusPerson.icdoPerson.person_id, ref ldecIAPBalance, false,false,false,false,"", this.icdoBenefitCalculationHeader.benefit_type_value,false); //Ticket - 68700

                    GetSurvivorPercentAmount(ref ldecIAPBalance, busConstant.IAP_PLAN_ID);

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


                    //this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                    //                            item.icdoBenefitCalculationDetail.l52_spl_acc_flag.IsNullOrEmpty() &&
                    //                            item.icdoBenefitCalculationDetail.l161_spl_acc_flag.IsNullOrEmpty()).FirstOrDefault().idecFinalPercentSurviviorIAPAmount = ldecIAPBalance;


                }
                else
                {
                    ldecIAPBalance = this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().icdoBenefitCalculationDetail.iap_balance_amount;

                    // Set the Default Parameters 
                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().icdoBenefitCalculationDetail.unreduced_benefit_amount = ldecIAPBalance;
                    //Process QDRO Offset
                    this.ibusCalculation.ProcessQDROOffset(this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID && item.icdoBenefitCalculationDetail.l52_spl_acc_flag.IsNullOrEmpty() && item.icdoBenefitCalculationDetail.l161_spl_acc_flag.IsNullOrEmpty()).First()
                        , this.ibusPerson.icdoPerson.person_id, ref ldecIAPBalance);

                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).First().
                        icdoBenefitCalculationDetail.idecBenefitsAfterQDROOffSet = ldecIAPBalance;

                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().icdoBenefitCalculationDetail.early_reduction_factor = 1.0m;
                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().icdoBenefitCalculationDetail.early_reduced_benefit_amount = ldecIAPBalance;
                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().icdoBenefitCalculationDetail.final_monthly_benefit_amount = ldecIAPBalance;
                    GetSurvivorPercentAmount(ref ldecIAPBalance, busConstant.IAP_PLAN_ID);
                    //this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().idecFinalPercentSurviviorIAPAmount = ldecIAPBalance;

                }

            }
            if (this.iblnCalculateL52SplAccBenefit)
            {
                if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL ||
                    this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT)
                {

                    ldecL52SpecialAccountBalance = this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID && item.icdoBenefitCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES).FirstOrDefault().icdoBenefitCalculationDetail.local52_special_acct_bal_amount;
                    this.ibusCalculation.ProcessQDROOffset(this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID && item.icdoBenefitCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES).FirstOrDefault(), this.ibusPerson.icdoPerson.person_id, ref ldecL52SpecialAccountBalance, false, false, true, false);
                    GetSurvivorPercentAmount(ref ldecL52SpecialAccountBalance, busConstant.IAP_PLAN_ID);
                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID && item.icdoBenefitCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES).FirstOrDefault().icdoBenefitCalculationDetail.early_reduced_benefit_amount = ldecL52SpecialAccountBalance;
                    //this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID && item.icdoBenefitCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES).FirstOrDefault().idecFinalPercentSurviviorL52SpecialAccountAmount = ldecL52SpecialAccountBalance;
                }
                else
                {
                    ldecL52SpecialAccountBalance = this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().icdoBenefitCalculationDetail.local52_special_acct_bal_amount;
                    this.ibusCalculation.ProcessQDROOffset(this.iclbBenefitCalculationDetail.Where(i => i.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault(), this.ibusPerson.icdoPerson.person_id, ref ldecL52SpecialAccountBalance, false, false, true, false, busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE);
                    GetSurvivorPercentAmount(ref ldecL52SpecialAccountBalance, busConstant.IAP_PLAN_ID);
                    //this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().idecFinalPercentSurviviorL52SpecialAccountAmount = ldecL52SpecialAccountBalance;

                }

            }

            if (this.iblnCalculateL161SplAccBenefit)
            {
                if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL ||
                    this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT)
                {
                    ldecL161SpecialAccount = this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID && item.icdoBenefitCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES).FirstOrDefault().icdoBenefitCalculationDetail.local161_special_acct_bal_amount;
                    this.ibusCalculation.ProcessQDROOffset(this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID && item.icdoBenefitCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES).FirstOrDefault(), this.ibusPerson.icdoPerson.person_id, ref ldecL161SpecialAccount, false, false, false, true);
                    GetSurvivorPercentAmount(ref ldecL161SpecialAccount, busConstant.IAP_PLAN_ID);
                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID && item.icdoBenefitCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES).FirstOrDefault().icdoBenefitCalculationDetail.early_reduced_benefit_amount = ldecL161SpecialAccount;
                    //this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID && item.icdoBenefitCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES).FirstOrDefault().idecFinalPercentSurviviorL161SpecialAccountAmount = ldecL161SpecialAccount;
                }
                else
                {
                    ldecL161SpecialAccount = this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().icdoBenefitCalculationDetail.local161_special_acct_bal_amount;
                    this.ibusCalculation.ProcessQDROOffset(this.iclbBenefitCalculationDetail.Where(i => i.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault(), this.ibusPerson.icdoPerson.person_id, ref ldecL161SpecialAccount, false, false, false, true, busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE);
                    GetSurvivorPercentAmount(ref ldecL161SpecialAccount, busConstant.IAP_PLAN_ID);
                    //this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().idecFinalPercentSurviviorL161SpecialAccountAmount = ldecL161SpecialAccount;
                }

            }


            //To confirm whether we need to do this way or not.
            //Subtract Set Up Fee & then subtract tax:Not required
            //ldecIAPBalance = ldecIAPBalance - busConstant.BenefitCalculation.IAP_SET_UP_FEE;
            //ldecL52SpecialAccountBalance = ldecL52SpecialAccountBalance - busConstant.BenefitCalculation.IAP_SET_UP_FEE;
            //ldecIAPBalance = ldecIAPBalance - Math.Round(ldecIAPBalance * .005M, 2);

            //ldecL52SpecialAccountBalance = ldecL52SpecialAccountBalance - Math.Round(ldecL52SpecialAccountBalance * .005M, 2);

            int lintPlanBenefitId = ibusCalculation.GetPlanBenefitId(busConstant.IAP_PLAN_ID, busConstant.LIFE_ANNUTIY);

            decimal ldecAnnuityBenefitOptionFactor = ibusCalculation.GetBenefitOptionFactor(busConstant.BENEFIT_TYPE_DEATH_PRE_RETIREMENT, lintPlanBenefitId,
                                                      Convert.ToInt32(this.iintSpouseEarliestRetrAge), 0) * 12;
            decimal ldecLumpSumBenefitOptionFactor = 1;
            busBenefitCalculationOptions lbusBenefitCalculationOptions;
            int lintRemainder = 0;
            decimal ldecL52SpecialAccountAnnuity = new decimal();
            decimal ldecL161SpecialAccountAnnuity = new decimal();

            if (ldecIAPBalance > busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT)
            {
                if (this.iblnCalculateIAPBenefit)
                {
                    ldecIAPAnnuity = busConstant.ZERO_DECIMAL;
                    ldecIAPAnnuity = Math.Round(ldecIAPBalance / ldecAnnuityBenefitOptionFactor);
                    Math.DivRem(Convert.ToInt32(ldecIAPAnnuity), 10, out lintRemainder);
                    if (lintRemainder > 0)
                    {
                        ldecIAPAnnuity = ldecIAPAnnuity - lintRemainder;
                    }
                }

            }

            if (ldecL52SpecialAccountBalance > busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT)
            {
                if (this.iblnCalculateL52SplAccBenefit)
                {
                    lintRemainder = 0;
                    ldecL52SpecialAccountAnnuity = Math.Round(ldecL52SpecialAccountBalance / ldecAnnuityBenefitOptionFactor, 0);
                    Math.DivRem(Convert.ToInt32(ldecL52SpecialAccountAnnuity), 10, out lintRemainder);
                    if (lintRemainder > 0)
                    {
                        ldecL52SpecialAccountAnnuity = ldecL52SpecialAccountAnnuity - lintRemainder;
                    }
                }

            }

            if (ldecL161SpecialAccount > busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT)
            {
                if (this.iblnCalculateL161SplAccBenefit)
                {
                    lintRemainder = 0;
                    ldecL161SpecialAccountAnnuity = Math.Round(ldecL161SpecialAccount / ldecAnnuityBenefitOptionFactor, 0);
                    Math.DivRem(Convert.ToInt32(ldecL161SpecialAccountAnnuity), 10, out lintRemainder);
                    if (lintRemainder > 0)
                    {
                        ldecL161SpecialAccountAnnuity = ldecL161SpecialAccountAnnuity - lintRemainder;
                    }
                }
            }


            if ((ldecIAPBalance > busConstant.ZERO_DECIMAL || ldecL52SpecialAccountBalance > busConstant.ZERO_DECIMAL || ldecL161SpecialAccount > busConstant.ZERO_DECIMAL)
                || icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT) //PIR 985 10252015
            {
                switch (astrBenefitOptionValue)
                {
                    #region ALL
                    case busConstant.CodeValueAll:
                        // Life Annuity
                        if (iblnCheckIfSurvivorIsSpouse && this.ibusBenefitApplication.iblnEligible4IAPBenefitPreDeath)
                        {
                            if (ldecIAPBalance > busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT)
                            {
                                lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                                if(icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE)
                                {
                                    
                                    lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, ldecAnnuityBenefitOptionFactor, new decimal(), busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_BENEFICIARY,
                                            this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                             busConstant.LIFE_ANNUTIY, ldecAnnunityAdjustmentMultiplier == 0?  ldecIAPAnnuity: ldecIAPAnnuity * ldecAnnunityAdjustmentMultiplier);
                                }
                                else
                                {
                                    lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, ldecAnnuityBenefitOptionFactor, new decimal(), busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_BENEFICIARY,
                                            this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                             busConstant.LIFE_ANNUTIY, ldecIAPAnnuity);
                                }
                                
                                lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.survivor_percent_amount = ldecIAPBalance;
                                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
                            }
                            if (ldecL52SpecialAccountBalance > busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT)
                            {
                                #region L52
                                lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                                if (icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE)
                                {
                                    lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, ldecAnnuityBenefitOptionFactor, new decimal(), busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_BENEFICIARY,
                                this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                 busConstant.LIFE_ANNUTIY, ldecAnnunityAdjustmentMultiplier == 0 ? ldecL52SpecialAccountAnnuity: ldecL52SpecialAccountAnnuity * ldecAnnunityAdjustmentMultiplier, false, false, true);
                                }
                                else
                                {
                                    lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, ldecAnnuityBenefitOptionFactor, new decimal(), busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_BENEFICIARY,
                                this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                 busConstant.LIFE_ANNUTIY, ldecL52SpecialAccountAnnuity, false, false, true);

                                }
                                    
                                lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.survivor_percent_amount = ldecL52SpecialAccountBalance;

                                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
                                #endregion
                            }
                            if (ldecL161SpecialAccount > busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT)
                            {
                                #region L161
                                lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                                if (icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE)
                                {
                                    lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, ldecAnnuityBenefitOptionFactor, new decimal(), busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_BENEFICIARY,
                                this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                 busConstant.LIFE_ANNUTIY, ldecAnnunityAdjustmentMultiplier == 0 ? ldecL161SpecialAccountAnnuity: ldecL161SpecialAccountAnnuity * ldecAnnunityAdjustmentMultiplier, false, false, false, true);
                                }
                                else
                                {
                                    lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, ldecAnnuityBenefitOptionFactor, new decimal(), busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_BENEFICIARY,
                                this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                 busConstant.LIFE_ANNUTIY, ldecL161SpecialAccountAnnuity, false, false, false, true);
                                }
                                lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.survivor_percent_amount = ldecL161SpecialAccount;

                                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);

                                #endregion

                            }
                        }
                        // Lumpsum Benefit Option
                        lintPlanBenefitId = ibusCalculation.GetPlanBenefitId(busConstant.IAP_PLAN_ID, busConstant.LUMP_SUM);

                        lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                        lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, 1, ldecIAPBalance, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_BENEFICIARY,
                                                                this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.LUMP_SUM, ldecIAPBalance, false, false);
                        lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.survivor_percent_amount = ldecIAPBalance;

                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);

                        if (this.iblnCalculateL52SplAccBenefit)
                        {
                            lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                            lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, 1, new decimal(), busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_BENEFICIARY,
                                                                    this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.LUMP_SUM, ldecL52SpecialAccountBalance, false, false, true);
                            lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.survivor_percent_amount = ldecL52SpecialAccountBalance;

                            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
                        }

                        if (this.iblnCalculateL161SplAccBenefit)
                        {
                            lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                            lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, 1, new decimal(), busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_BENEFICIARY,
                                                                    this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.LUMP_SUM, ldecL161SpecialAccount, false, false, false, true);

                            lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.survivor_percent_amount = ldecL161SpecialAccount;
                            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
                        }

                        break;
                    #endregion

                    #region Annuity
                    case busConstant.LIFE_ANNUTIY:
                        //IAP
                        if (this.iblnCalculateIAPBenefit && iblnCheckIfSurvivorIsSpouse && ldecIAPBalance > busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT && this.ibusBenefitApplication.iblnEligible4IAPBenefitPreDeath)
                        {
                            lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                            if (icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE)
                            {
                                lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, ldecAnnuityBenefitOptionFactor, new decimal(), busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_BENEFICIARY,
                                        this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                        busConstant.LIFE_ANNUTIY, ldecAnnunityAdjustmentMultiplier == 0 ? ldecIAPAnnuity : ldecIAPAnnuity * ldecAnnunityAdjustmentMultiplier, false, false);
                            }
                            else
                            {
                                lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, ldecAnnuityBenefitOptionFactor, new decimal(), busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_BENEFICIARY,
                                        this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                        busConstant.LIFE_ANNUTIY, ldecIAPAnnuity, false, false);

                            }
                                
                            lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.survivor_percent_amount = ldecIAPBalance;

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

                        //L52 Special Account
                        if (this.iblnCalculateL52SplAccBenefit && iblnCheckIfSurvivorIsSpouse && ldecL52SpecialAccountBalance > busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT && this.ibusBenefitApplication.iblnEligible4IAPBenefitPreDeath)
                        {
                            lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                            if (icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE)
                            {
                                lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, ldecAnnuityBenefitOptionFactor, new decimal(), busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_BENEFICIARY,
                                                                    this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                    busConstant.LIFE_ANNUTIY, ldecAnnunityAdjustmentMultiplier == 0 ? ldecL52SpecialAccountAnnuity : ldecL52SpecialAccountAnnuity * ldecAnnunityAdjustmentMultiplier, false, false, true);
                            }
                            else
                            {
                                lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, ldecAnnuityBenefitOptionFactor, new decimal(), busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_BENEFICIARY,
                                                                    this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                    busConstant.LIFE_ANNUTIY, ldecL52SpecialAccountAnnuity, false, false, true);

                            }
                                

                            lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.survivor_percent_amount = ldecL52SpecialAccountBalance;

                            if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL)
                            {
                                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID && item.icdoBenefitCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
                            }
                            else
                            {
                                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
                            }
                        }

                        //L161 Special Account 
                        if (this.iblnCalculateL161SplAccBenefit && iblnCheckIfSurvivorIsSpouse && ldecL161SpecialAccount > busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT && this.ibusBenefitApplication.iblnEligible4IAPBenefitPreDeath)
                        {
                            lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                            if (icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE)
                            {
                                lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, ldecAnnuityBenefitOptionFactor, new decimal(), busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_BENEFICIARY,
                                                                   this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                    busConstant.LIFE_ANNUTIY, ldecAnnunityAdjustmentMultiplier == 0 ? ldecL161SpecialAccountAnnuity : ldecL161SpecialAccountAnnuity * ldecAnnunityAdjustmentMultiplier, false, false, false, true);
                            }
                            else
                            {
                                lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, ldecAnnuityBenefitOptionFactor, new decimal(), busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_BENEFICIARY,
                                                                   this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                    busConstant.LIFE_ANNUTIY, ldecL161SpecialAccountAnnuity, false, false, false, true);
                            }
                               

                            lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.survivor_percent_amount = ldecL161SpecialAccount;

                            if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL)
                            {
                                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID && item.icdoBenefitCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
                            }
                            else
                            {
                                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
                            }
                        }

                        break;
                    #endregion

                    #region Lump
                    case busConstant.LUMP_SUM:
                        lintPlanBenefitId = ibusCalculation.GetPlanBenefitId(busConstant.IAP_PLAN_ID, busConstant.LUMP_SUM);
                        //IAP
                        if (this.iblnCalculateIAPBenefit)
                        {
                            lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                            lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, 1, ldecIAPBalance, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_BENEFICIARY,
                                                                    this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.LUMP_SUM, ldecIAPBalance, false, false);

                            lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.survivor_percent_amount = ldecIAPBalance;
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

                        //L52 SPL ACC
                        if (this.iblnCalculateL52SplAccBenefit)
                        {
                            lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                            lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, 1, ldecIAPBalance, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_BENEFICIARY,
                                                                    this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.LUMP_SUM, ldecL52SpecialAccountBalance, false, false, true);

                            lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.survivor_percent_amount = ldecL52SpecialAccountBalance;

                            if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL)
                            {
                                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID && item.icdoBenefitCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
                            }
                            else
                            {
                                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
                            }
                        }

                        //L161 SPL ACC
                        if (this.iblnCalculateL161SplAccBenefit)
                        {
                            lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                            lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, 1, ldecIAPBalance, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_BENEFICIARY,
                                                                    this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.LUMP_SUM, ldecL161SpecialAccount, false, false, false, true);

                            lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.survivor_percent_amount = ldecL161SpecialAccount;
                            if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL)
                            {
                                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID && item.icdoBenefitCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
                            }
                            else
                            {
                                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
                            }
                        }
                        break;
                    #endregion

                    /*
                    #region JS100
                    case busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY:
                        GetJAndSBenefitOptionForIAP(ldecIAPBalance, ldecL52SpecialAccountBalance, ldecL161SpecialAccount, busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY);
                        break;
                    #endregion

                    #region JS75
                    case busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY:
                        GetJAndSBenefitOptionForIAP(ldecIAPBalance, ldecL52SpecialAccountBalance, ldecL161SpecialAccount, busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY);
                        break;
                    #endregion

                    #region TENYLA
                    case busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY:
                        GetJAndSBenefitOptionForIAP(ldecIAPBalance, ldecL52SpecialAccountBalance, ldecL161SpecialAccount, busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY);
                        break;
                    #endregion
                        */
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Death Pre Retirement Post Election.
        /// </summary>
        /// <param name="adecIAPBalance"></param>
        public void GetJAndSBenefitOptionForIAP(decimal adecIAPBalance, decimal adecL52SpecialAccountBalance, decimal adecL161SpecialAccount, string astrBenefitOptionValue)
        {
            int lintPlanBenefitId = ibusCalculation.GetPlanBenefitId(busConstant.IAP_PLAN_ID, astrBenefitOptionValue);
            decimal ldecBenefitOptionFactor = 0;
            if (astrBenefitOptionValue == busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY)
            {
                ldecBenefitOptionFactor = ibusCalculation.GetBenefitOptionFactor(busConstant.BENEFIT_TYPE_RETIREMENT, lintPlanBenefitId, Convert.ToInt32(this.icdoBenefitCalculationHeader.idecSurvivorFullAge), busConstant.ZERO_INT) * 12;
            }
            else
            {
                ldecBenefitOptionFactor = ibusCalculation.GetBenefitOptionFactor(busConstant.BENEFIT_TYPE_RETIREMENT, lintPlanBenefitId,
                                              Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge), Convert.ToInt32(this.icdoBenefitCalculationHeader.idecSurvivorFullAge)) * 12;
            }
            int lintRemainder = 0;
            busBenefitCalculationOptions lbusBenefitCalculationOptions;
            if (this.iblnCalculateIAPBenefit && iblnCheckIfSurvivorIsSpouse && adecIAPBalance > busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT)
            {

                decimal ldecIAPAnnuity = busConstant.ZERO_DECIMAL;
                ldecIAPAnnuity = Math.Round(adecIAPBalance / ldecBenefitOptionFactor);
                Math.DivRem(Convert.ToInt32(ldecIAPAnnuity), 10, out lintRemainder);
                if (lintRemainder > 0)
                {
                    ldecIAPAnnuity = ldecIAPAnnuity - lintRemainder;
                    if (astrBenefitOptionValue == busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY)
                    {
                        ldecIAPAnnuity = ldecIAPAnnuity * .75M;
                    }
                }

                lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, ldecBenefitOptionFactor, new decimal(), busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_BENEFICIARY,
                            this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                            astrBenefitOptionValue, ldecIAPAnnuity, false, false);
                lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.survivor_percent_amount = adecIAPBalance;

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

            //L52 Special Account
            if (this.iblnCalculateL52SplAccBenefit && iblnCheckIfSurvivorIsSpouse && adecL52SpecialAccountBalance > busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT)
            {
                decimal ldecL52SpecialAccountAnnuity = busConstant.ZERO_DECIMAL;
                ldecL52SpecialAccountAnnuity = Math.Round(adecL52SpecialAccountBalance / ldecBenefitOptionFactor);
                Math.DivRem(Convert.ToInt32(ldecL52SpecialAccountAnnuity), 10, out lintRemainder);
                if (lintRemainder > 0)
                {
                    ldecL52SpecialAccountAnnuity = ldecL52SpecialAccountAnnuity - lintRemainder;
                    if (astrBenefitOptionValue == busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY)
                    {
                        ldecL52SpecialAccountAnnuity = ldecL52SpecialAccountAnnuity * .75M;
                    }
                }


                lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, ldecBenefitOptionFactor, new decimal(), busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_BENEFICIARY,
                                                        this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                        busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY, ldecL52SpecialAccountAnnuity, false, false, true);

                lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.survivor_percent_amount = adecL52SpecialAccountBalance;

                if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL)
                {
                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID && item.icdoBenefitCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
                }
                else
                {
                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
                }
            }

            //L161 Special Account 
            if (this.iblnCalculateL161SplAccBenefit && iblnCheckIfSurvivorIsSpouse && adecL161SpecialAccount > busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT)
            {
                decimal ldecL161SpecialAccountAnnuity = busConstant.ZERO_DECIMAL;
                ldecL161SpecialAccountAnnuity = Math.Round(adecL161SpecialAccount / ldecBenefitOptionFactor);
                Math.DivRem(Convert.ToInt32(ldecL161SpecialAccountAnnuity), 10, out lintRemainder);
                if (lintRemainder > 0)
                {
                    ldecL161SpecialAccountAnnuity = ldecL161SpecialAccountAnnuity - lintRemainder;
                    if (astrBenefitOptionValue == busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY)
                    {
                        ldecL161SpecialAccountAnnuity = ldecL161SpecialAccountAnnuity * .75M;
                    }
                }


                lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, ldecBenefitOptionFactor, new decimal(), busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_BENEFICIARY,
                                                        this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                         busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY, ldecL161SpecialAccountAnnuity, false, false, false, true);

                lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.survivor_percent_amount = adecL161SpecialAccount;

                if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL)
                {
                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID && item.icdoBenefitCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
                }
                else
                {
                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
                }
            }
        }

        public decimal GetNormalRetirementAge(int aintPlanId)
        {
            decimal ldecNormalRetirementAge = busConstant.BenefitCalculation.NORMAL_RETIREMENT_AGE_MPIPP;
            switch (aintPlanId)
            {
                case busConstant.MPIPP_PLAN_ID:
                    ldecNormalRetirementAge = busConstant.BenefitCalculation.NORMAL_RETIREMENT_AGE_MPIPP;
                    break;
                case busConstant.IAP_PLAN_ID:
                    ldecNormalRetirementAge = busConstant.BenefitCalculation.NORMAL_RETIREMENT_AGE_IAP;
                    break;
                case busConstant.LOCAL_52_PLAN_ID:
                    ldecNormalRetirementAge = busConstant.BenefitCalculation.NORMAL_RETIREMENT_AGE_LOCAL_52;
                    break;
                case busConstant.LOCAL_600_PLAN_ID:
                    ldecNormalRetirementAge = busConstant.BenefitCalculation.NORMAL_RETIREMENT_AGE_LOCAL_600;
                    break;
                case busConstant.LOCAL_666_PLAN_ID:
                    ldecNormalRetirementAge = busConstant.BenefitCalculation.NORMAL_RETIREMENT_AGE_LOCAL_666;
                    break;
                case busConstant.LOCAL_161_PLAN_ID:
                    ldecNormalRetirementAge = busConstant.BenefitCalculation.NORMAL_RETIREMENT_AGE_LOCAL_161;
                    break;
                case busConstant.LOCAL_700_PLAN_ID:
                    ldecNormalRetirementAge = busConstant.BenefitCalculation.NORMAL_RETIREMENT_AGE_LOCAL_700;
                    break;
            }

            return ldecNormalRetirementAge;
        }

        public void CalculateMEA()
        {

            // Monthly Exclusion Amount = EE CONTRIBUTION / NUMBER OF RECOVERY MONTHS
            int lintRecoveryMonths = busConstant.ZERO_INT;
            int lintAge = busConstant.ZERO_INT;

            if (!this.iblnCheckIfSurvivorIsSpouse)
            {
                lintAge = Convert.ToInt32(Math.Floor(this.icdoBenefitCalculationHeader.age));

                if (lintAge <= 55)
                {
                    lintRecoveryMonths = 360;
                }
                else if (lintAge <= 60)
                {
                    lintRecoveryMonths = 310;
                }
                else if (lintAge <= 65)
                {
                    lintRecoveryMonths = 260;
                }
                else if (lintAge <= 70)
                {
                    lintRecoveryMonths = 210;
                }
                else
                {
                    lintRecoveryMonths = 160;
                }
            }
            else
            {
                // Calculate based on Single Life
                // Combined AGES                            Number of Recovery Months
                // Not more than 110                                410
                // More than 110, but not more than 120             360
                // More than 120, but not more than 130             310
                // More than 130, but not more than 140             260
                // More than 140                                    210

                lintAge = Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge + this.icdoBenefitCalculationHeader.idecSurvivorFullAge);
                if (lintAge <= 110)
                {
                    lintRecoveryMonths = 410;
                }
                else if (lintAge <= 120)
                {
                    lintRecoveryMonths = 360;
                }
                else if (lintAge <= 130)
                {
                    lintRecoveryMonths = 310;
                }
                else if (lintAge <= 140)
                {
                    lintRecoveryMonths = 260;
                }
                else
                {
                    lintRecoveryMonths = 210;
                }
            }

            decimal ldecVestedNonEEContribution = this.idecVestedEEContribution + this.idecNonVestedEEContribution;
            decimal ldecQDROOffset = decimal.Zero;
            if (ldecVestedNonEEContribution > decimal.Zero)
            {
                if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE)
                {
                    ldecQDROOffset = this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().icdoBenefitCalculationDetail.total_ee_contribution_qdro_offset;
                }
                else
                {
                    if (this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId && item.icdoBenefitCalculationDetail.uvhp_flag.IsNullOrEmpty()).Count() > 0)
                    {
                        ldecQDROOffset = this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId && item.icdoBenefitCalculationDetail.uvhp_flag.IsNullOrEmpty()).FirstOrDefault().icdoBenefitCalculationDetail.total_ee_contribution_qdro_offset;
                    }
                }
                ldecVestedNonEEContribution = ldecVestedNonEEContribution - ldecQDROOffset;

                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().icdoBenefitCalculationDetail.monthly_exclusion_amount =
                        Math.Round((ldecVestedNonEEContribution) / lintRecoveryMonths, 2);
            }

        }


        private void CalculateFinalBenefitForPensionBenefitOptions(string astrBenefitOptionValue)
        {
            if (this.ibusBenefitApplication.CheckAlreadyVested(busConstant.MPIPP) && this.iblnCheckIfSurvivorIsSpouse)
            {
                decimal ldecLumpSumBenefitOptionFactor = busConstant.ZERO_DECIMAL;
                decimal ldecLateAdjustmentAmt = decimal.Zero;
                int lintParticipantBenComDate = Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge);
                int lintSpouseBenComDate = Convert.ToInt32(Math.Floor(this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement));
                // Calculate the Final Benefit Amounts for all Benefit Options
                busPersonAccount lbusPersonAccount = ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault();
                decimal ldecAccruedBenefitAmount = decimal.Zero;
                if (astrBenefitOptionValue == busConstant.CodeValueAll || astrBenefitOptionValue == busConstant.LUMP_SUM)
                {
                    if (this.icdoBenefitCalculationHeader.benefit_commencement_date >= this.icdoBenefitCalculationHeader.retirement_date || this.icdoBenefitCalculationHeader.benefit_commencement_date == DateTime.MinValue)
                    {
                        ldecLumpSumBenefitOptionFactor = ibusCalculation.GetLumpsumBenefitFactor(Convert.ToInt32(this.icdoBenefitCalculationHeader.idecSurvivorFullAge), this.icdoBenefitCalculationHeader.benefit_commencement_date.Year) * 12;
                        ldecLumpSumBenefitOptionFactor = Math.Round(ldecLumpSumBenefitOptionFactor, 3);
                    }
                    else
                    {
                        ldecLumpSumBenefitOptionFactor = DeferredLumpSumFactor(lintSpouseBenComDate, iintSpouseEarliestRetrAge, this.icdoBenefitCalculationHeader.benefit_commencement_date.Year); // PROD PIR 816
                    }

                    ldecLumpSumBenefitOptionFactor = Math.Round(ldecLumpSumBenefitOptionFactor, 3);
                }
                //passing astrcalculation type as estimate always to set ee offset for mea & min guarantee
                decimal ldecEETotal = idecVestedEEContribution + idecVestedEEInterest + idecNonVestedEEContribution + idecNonVestedEEInterest + idecEEPartialInterest + idecNonVestedEEPartialInterest;
                if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE || astrBenefitOptionValue == busConstant.LUMP_SUM)
                {
                    ldecAccruedBenefitAmount = ibusCalculation.CalculateBenefitAmtForPension(ibusPerson, busConstant.BENEFIT_TYPE_RETIREMENT, iintParticipantEarliestRetrAge, this.ibusBenefitApplication.icdoBenefitApplication.retirement_date,
                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().icdoBenefitCalculationDetail.vested_date,
                    lbusPersonAccount,
                    ibusBenefitApplication, busConstant.BOOL_FALSE,
                    this.iclbBenefitCalculationDetail, this.iclbPersonAccountRetirementContribution, null, true, this.icdoBenefitCalculationHeader.istrRetirementType, ref ldecLateAdjustmentAmt, this.ibusPerson.icdoPerson.person_id, astrCalculationType: busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE,
                    adtEarliestRetirementDate:this.icdoBenefitCalculationHeader.retirement_date);

                    #region Check if Withdrawal History Exists: Then Acrrued benefit = Accrued Benefit - EE derived

                    //ldecAccruedBenefitAmount = ibusCalculation.CalculateAccruedBenefitForPersonWithWithdrawal(
                    //                                        this.ibusBenefitApplication.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date,
                    //                                        ldecAccruedBenefitAmount, this.ibusBenefitApplication.ibusPerson, this.ibusBenefitApplication.ibusPerson.iclbPersonAccount,
                    //                                        icdoBenefitCalculationHeader.retirement_date, this.ibusBenefitApplication.aclbPersonWorkHistory_MPI,
                    //                                        this.ibusBenefitApplication.ibusPerson.iclbPersonAccountRetirementContribution,
                    //                                        this.ibusBenefitApplication.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.forfeiture_date.Year);

                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).
                                                                                  FirstOrDefault().icdoBenefitCalculationDetail.early_reduced_benefit_amount = ldecAccruedBenefitAmount;
                    #endregion
                }
                else
                {
                    ldecAccruedBenefitAmount = ibusCalculation.CalculateBenefitAmtForPension(ibusPerson, busConstant.BENEFIT_TYPE_RETIREMENT, Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge), this.ibusBenefitApplication.icdoBenefitApplication.retirement_date,
                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().icdoBenefitCalculationDetail.vested_date,
                     lbusPersonAccount,
                     ibusBenefitApplication, busConstant.BOOL_FALSE,
                     iclbBenefitCalculationDetail, this.iclbPersonAccountRetirementContribution, null, true, lbusPersonAccount.icdoPersonAccount.istrRetirementSubType, ref ldecLateAdjustmentAmt, this.ibusPerson.icdoPerson.person_id, astrCalculationType: busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE,
                     adtEarliestRetirementDate: this.icdoBenefitCalculationHeader.retirement_date);

                    #region Check if Withdrawal History Exists: Then Acrrued benefit = Accrued Benefit - EE derived

                    //ldecAccruedBenefitAmount = ibusCalculation.CalculateAccruedBenefitForPersonWithWithdrawal(
                    //                                        this.ibusBenefitApplication.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date,
                    //                                        ldecAccruedBenefitAmount, this.ibusBenefitApplication.ibusPerson, this.ibusBenefitApplication.ibusPerson.iclbPersonAccount, icdoBenefitCalculationHeader.retirement_date,
                    //                                        this.ibusBenefitApplication.aclbPersonWorkHistory_MPI, this.ibusBenefitApplication.ibusPerson.iclbPersonAccountRetirementContribution,
                    //                                        this.ibusBenefitApplication.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.forfeiture_date.Year);

                    if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE)
                    {
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).
                                                                                      FirstOrDefault().icdoBenefitCalculationDetail.early_reduced_benefit_amount = ldecAccruedBenefitAmount;
                    }
                    else
                    {
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId && item.icdoBenefitCalculationDetail.uvhp_flag.IsNullOrEmpty()).FirstOrDefault().icdoBenefitCalculationDetail.early_reduced_benefit_amount = ldecAccruedBenefitAmount;
                    }
                    #endregion

                }
                if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE)
                {
                    ldecEETotal = ldecEETotal -
(this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().icdoBenefitCalculationDetail.total_ee_contribution_qdro_offset +
this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().icdoBenefitCalculationDetail.total_ee_interest_qdro_offset);

                }
                else
                {
                    ldecEETotal = ldecEETotal -
    (this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId && item.icdoBenefitCalculationDetail.uvhp_flag.IsNullOrEmpty()).FirstOrDefault().icdoBenefitCalculationDetail.total_ee_contribution_qdro_offset +
    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId && item.icdoBenefitCalculationDetail.uvhp_flag.IsNullOrEmpty()).FirstOrDefault().icdoBenefitCalculationDetail.total_ee_interest_qdro_offset);

                }
                GetSurvivorPercentAmount(ref ldecAccruedBenefitAmount, this.icdoBenefitCalculationHeader.iintPlanId);
                GetSurvivorPercentAmount(ref ldecEETotal, this.icdoBenefitCalculationHeader.iintPlanId);
                if (astrBenefitOptionValue == busConstant.CodeValueAll || astrBenefitOptionValue == busConstant.LIFE_ANNUTIY)
                {
                    if (this.ibusBenefitApplication.iblnEligbile4MPIBenefitPreDeath)
                    {
                        CalculateMpiPlanLifeAnnuityOptions(ldecAccruedBenefitAmount);
                    }
                }
                if (astrBenefitOptionValue == busConstant.CodeValueAll || astrBenefitOptionValue == busConstant.LUMP_SUM)
                {
                    CalculateMpiPlanLumpSumOptions(lbusPersonAccount, ldecLumpSumBenefitOptionFactor, ldecEETotal, ldecAccruedBenefitAmount, true);
                }
                if (iblnCheckIfPreRetPostElection)
                {
                    GetMPIBenefitOptionsForDeathPostElection(astrBenefitOptionValue, ldecAccruedBenefitAmount, ldecLumpSumBenefitOptionFactor, ldecEETotal);
                }
            }
        }


        private void CalculateMPIBenefitOptions(string astrBenefitOptionValue)
        {
            this.CalculateEEInterestAsOfRetirementDate(this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault());

            if (this.iblnCalculateMPIPPBenefit)
            {
                CalculateFinalBenefitForPensionBenefitOptions(astrBenefitOptionValue);
                if (!this.iblnCheckIfPreRetPostElection)
                {
                    this.CalculateMEA();
                }
            }
            if (this.iblnCalcualteUVHPBenefit)
            {
                CalculateEEUVHPBenefitOptions(astrBenefitOptionValue);
            }

        }

        private void CalculateEEUVHPBenefitOptions(string astrBenefitOptionValue)
        {
            if (this.ibusBenefitApplication.CheckAlreadyVested(busConstant.MPIPP) && this.iblnCheckIfSurvivorIsSpouse)
            {
                //PIR 998
                decimal ldecTotalUVHP = idecUVHPContribution + idecUVHPInterest + idecUVHPPartialInterest + idecNonVestedEEInterest + idecNonVestedEEPartialInterest + idecNonVestedEEContribution;
                if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL)
                {
                    this.ibusCalculation.ProcessQDROOffset(this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoBenefitCalculationDetail.uvhp_flag.IsNotNullOrEmpty()).FirstOrDefault(), this.ibusPerson.icdoPerson.person_id, ref ldecTotalUVHP
                        , false, true, false, false, this.icdoBenefitCalculationHeader.calculation_type_value);

                }
                else
                {

                    this.ibusCalculation.ProcessQDROOffset(this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault(), this.ibusPerson.icdoPerson.person_id, ref ldecTotalUVHP
                        , false, true, false, false, this.icdoBenefitCalculationHeader.calculation_type_value);

                }
                GetSurvivorPercentAmount(ref ldecTotalUVHP, this.icdoBenefitCalculationHeader.iintPlanId);

                //Only UVHP To be Given along with Accrued Benefit
                switch (astrBenefitOptionValue)
                {
                    case busConstant.CodeValueAll:
                        if (this.idecUVHPContribution + idecUVHPInterest + idecUVHPPartialInterest + idecNonVestedEEContribution + idecNonVestedEEInterest + idecVestedEEContribution + idecVestedEEInterest + idecNonVestedEEPartialInterest > busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT) //PIR 998
                        {
                            CalculateUVHPAnnuityOption(ldecTotalUVHP);
                            CalculateUVHPLumpSumOption(ldecTotalUVHP);
                        }
                        else
                        {
                            CalculateUVHPLumpSumOption(ldecTotalUVHP);
                        }
                        break;

                    case busConstant.LIFE_ANNUTIY:
                        if (this.idecUVHPContribution + idecUVHPInterest + idecUVHPPartialInterest + idecNonVestedEEContribution + idecNonVestedEEInterest + idecVestedEEContribution + idecVestedEEInterest + idecNonVestedEEPartialInterest > busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT) //PIR 998
                        {
                            CalculateUVHPAnnuityOption(ldecTotalUVHP);
                        }
                        break;
                    case busConstant.LUMP_SUM:
                        CalculateUVHPLumpSumOption(ldecTotalUVHP);
                        break;
                }
            }
            else
            {
                decimal ldecTotalEEUVHP = idecTotalEEUVHP;
                if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL)
                {
                    this.ibusCalculation.ProcessQDROOffset(this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoBenefitCalculationDetail.uvhp_flag.IsNotNullOrEmpty()).FirstOrDefault(), this.ibusPerson.icdoPerson.person_id, ref ldecTotalEEUVHP
                        , true, true, false, false, this.icdoBenefitCalculationHeader.calculation_type_value, ablnPreRetiremntDeathNonSpouse: true); //PIR 83(added last parameter)
                }
                else
                {
                    this.ibusCalculation.ProcessQDROOffset(this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault(), this.ibusPerson.icdoPerson.person_id, ref ldecTotalEEUVHP
                        , true, true, false, false, this.icdoBenefitCalculationHeader.calculation_type_value, ablnPreRetiremntDeathNonSpouse: true); //PIR 83(added last parameter)
                }
                GetSurvivorPercentAmount(ref ldecTotalEEUVHP, this.icdoBenefitCalculationHeader.iintPlanId);

                //EE + UVHP : Not Eligible for Accrued
                switch (astrBenefitOptionValue)
                {
                    case busConstant.CodeValueAll:
                        if (this.idecTotalEEUVHP > busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT)
                        {
                            if (this.iblnCheckIfSurvivorIsSpouse)
                            {
                                CalculateEEUVHPAnnuityOption(ldecTotalEEUVHP);
                            }
                            CalculateEEUVHPLumpSumOption(ldecTotalEEUVHP);
                        }
                        else
                        {
                            CalculateEEUVHPLumpSumOption(ldecTotalEEUVHP);
                        }
                        break;

                    case busConstant.LIFE_ANNUTIY:
                        if (this.idecTotalEEUVHP > busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT && this.iblnCheckIfSurvivorIsSpouse)
                        {
                            CalculateEEUVHPAnnuityOption(ldecTotalEEUVHP);
                        }
                        break;
                    case busConstant.LUMP_SUM:
                        CalculateEEUVHPLumpSumOption(ldecTotalEEUVHP);
                        break;
                }
            }
        }

        #endregion

        #region Common
        //Earliest Retirement Date.
        public void LoadPreRetirementDeathInitialData()
        {

            this.icdoBenefitCalculationHeader.retirement_date = DateTime.MinValue;
            //PIR 862
            this.idtPaymentDate = ibusBenefitApplication.icdoBenefitApplication.retirement_date;

            if (!this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.IsNullOrEmpty() && !this.ibusBenefitApplication.aclbPersonWorkHistory_MPI.IsNullOrEmpty())
            {
                decimal ldecParticipantCurrentAge = busGlobalFunctions.CalculatePersonAge(this.ibusPerson.icdoPerson.idtDateofBirth, this.icdoBenefitCalculationHeader.date_of_death);
                this.ibusBenefitApplication.aclbPersonWorkHistory_MPI = this.ibusBenefitApplication.aclbPersonWorkHistory_MPI.OrderBy(item => item.year).ToList().ToCollection();

                //As Per The Rule in Correspondence.
                this.ibusBenefitApplication.idecAgeAtDeath = busGlobalFunctions.CalculatePersonAge(this.ibusPerson.icdoPerson.idtDateofBirth, this.icdoBenefitCalculationHeader.date_of_death.GetLastDayofMonth().AddDays(1));
                this.ibusBenefitApplication.idecAgeAtL52Merger = busGlobalFunctions.CalculatePersonAge(this.ibusPerson.icdoPerson.idtDateofBirth, Convert.ToDateTime(busConstant.MERGER_DATE_STRING));
                string lstrPlanCode = String.Empty;
                DataTable ldtbPlanCode = busBase.Select("cdoPlan.GetPlanCodebyId", new object[1] { this.icdoBenefitCalculationHeader.iintPlanId });
                if (ldtbPlanCode.Rows.Count > 0)
                {
                    lstrPlanCode = ldtbPlanCode.Rows[0][0].ToString();
                }

                #region Seed for getting Earliest RTMT date
                int lintAgeToStartChecking = Convert.ToInt32(Math.Floor(ldecParticipantCurrentAge));
                int lintAgeToStopChecking = 66;
                if (lintAgeToStartChecking > lintAgeToStopChecking)
                {
                    lintAgeToStopChecking = lintAgeToStartChecking;
                }

                if (lintAgeToStartChecking <= 55 && this.icdoBenefitCalculationHeader.iintPlanId != busConstant.LOCAL_700_PLAN_ID)
                {
                    lintAgeToStartChecking = 55;
                }
                else if (this.icdoBenefitCalculationHeader.iintPlanId == busConstant.LOCAL_700_PLAN_ID && lintAgeToStartChecking <= 52)
                {
                    lintAgeToStartChecking = 52;
                }
                #endregion

                #region DetermineRetirementType

                for (int i = lintAgeToStartChecking; i <= lintAgeToStopChecking; i++)
                {
                    if (this.ibusPerson.icdoPerson.idtDateofBirth.Day == 1)
                    {
                        this.ibusBenefitApplication.icdoBenefitApplication.retirement_date = this.ibusPerson.icdoPerson.idtDateofBirth.AddYears(i);
                    }
                    else
                    {
                        this.ibusBenefitApplication.icdoBenefitApplication.retirement_date = busGlobalFunctions.GetLastDayofMonth(this.ibusPerson.icdoPerson.idtDateofBirth.AddYears(i)).AddDays(1);
                    }

                    if (this.ibusBenefitApplication.icdoBenefitApplication.retirement_date < this.icdoBenefitCalculationHeader.date_of_death)
                    {
                        this.ibusBenefitApplication.icdoBenefitApplication.retirement_date = busGlobalFunctions.GetLastDayofMonth(this.icdoBenefitCalculationHeader.date_of_death).AddDays(1);
                    }

                    this.icdoBenefitCalculationHeader.age = busGlobalFunctions.CalculatePersonAge(this.ibusPerson.icdoPerson.idtDateofBirth, this.ibusBenefitApplication.icdoBenefitApplication.retirement_date);
                    this.ibusBenefitApplication.idecAge = this.icdoBenefitCalculationHeader.age;
                    this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement = busGlobalFunctions.CalculatePersonAge(this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth, this.ibusBenefitApplication.icdoBenefitApplication.retirement_date);

                    this.ibusBenefitApplication.SetupPrerequisitesDeath();

                    if (this.icdoBenefitCalculationHeader.iintPlanId == busConstant.MPIPP_PLAN_ID)
                    {
                        if (!(this.ibusBenefitApplication.NotEligible) && this.ibusBenefitApplication.iblnEligbile4MPIBenefitPreDeath)
                        {
                            this.icdoBenefitCalculationHeader.retirement_date = this.ibusBenefitApplication.icdoBenefitApplication.retirement_date;
                            this.icdoBenefitCalculationHeader.istrRetirementType = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().icdoPersonAccount.istrRetirementSubType;
                            this.icdoBenefitCalculationHeader.istrEarliestRetirementType = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().icdoPersonAccount.istrRetirementSubType;

                            break;
                        }
                    }
                    else if (this.icdoBenefitCalculationHeader.iintPlanId == busConstant.LOCAL_700_PLAN_ID)
                    {
                        if (!(this.ibusBenefitApplication.NotEligible) && this.ibusBenefitApplication.iblnEligbile4L700BenefitPreDeath)
                        {
                            this.icdoBenefitCalculationHeader.retirement_date = this.ibusBenefitApplication.icdoBenefitApplication.retirement_date;
                            this.icdoBenefitCalculationHeader.istrRetirementType = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().icdoPersonAccount.istrRetirementSubType;
                            this.icdoBenefitCalculationHeader.istrEarliestRetirementType = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().icdoPersonAccount.istrRetirementSubType;

                            break;
                        }
                    }
                    else if (this.icdoBenefitCalculationHeader.iintPlanId == busConstant.LOCAL_161_PLAN_ID)
                    {
                        if (!(this.ibusBenefitApplication.NotEligible) && this.ibusBenefitApplication.iblnEligbile4L161BenefitPreDeath)
                        {
                            this.icdoBenefitCalculationHeader.retirement_date = this.ibusBenefitApplication.icdoBenefitApplication.retirement_date;
                            this.icdoBenefitCalculationHeader.istrRetirementType = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().icdoPersonAccount.istrRetirementSubType;
                            this.icdoBenefitCalculationHeader.istrEarliestRetirementType = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().icdoPersonAccount.istrRetirementSubType;

                            break;
                        }
                    }
                    else if (this.icdoBenefitCalculationHeader.iintPlanId == busConstant.LOCAL_666_PLAN_ID)
                    {
                        if (!(this.ibusBenefitApplication.NotEligible) && this.ibusBenefitApplication.iblnEligbile4L666BenefitPreDeath)
                        {
                            this.icdoBenefitCalculationHeader.retirement_date = this.ibusBenefitApplication.icdoBenefitApplication.retirement_date;
                            this.icdoBenefitCalculationHeader.istrRetirementType = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().icdoPersonAccount.istrRetirementSubType;
                            this.icdoBenefitCalculationHeader.istrEarliestRetirementType = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().icdoPersonAccount.istrRetirementSubType;

                            break;
                        }
                    }
                    else if (this.icdoBenefitCalculationHeader.iintPlanId == busConstant.LOCAL_600_PLAN_ID)
                    {
                        if (!(this.ibusBenefitApplication.NotEligible) && this.ibusBenefitApplication.iblnEligbile4L600BenefitPreDeath)
                        {
                            this.icdoBenefitCalculationHeader.retirement_date = this.ibusBenefitApplication.icdoBenefitApplication.retirement_date;
                            this.icdoBenefitCalculationHeader.istrRetirementType = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().icdoPersonAccount.istrRetirementSubType;
                            this.icdoBenefitCalculationHeader.istrEarliestRetirementType = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().icdoPersonAccount.istrRetirementSubType;

                            break;
                        }
                    }
                    else if (this.icdoBenefitCalculationHeader.iintPlanId == busConstant.LOCAL_52_PLAN_ID)
                    {
                        if (!(this.ibusBenefitApplication.NotEligible) && this.ibusBenefitApplication.iblnEligible4L52BenefitPreDeath)
                        {
                            this.icdoBenefitCalculationHeader.retirement_date = this.ibusBenefitApplication.icdoBenefitApplication.retirement_date;
                            this.icdoBenefitCalculationHeader.istrRetirementType = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().icdoPersonAccount.istrRetirementSubType;
                            this.icdoBenefitCalculationHeader.istrEarliestRetirementType = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().icdoPersonAccount.istrRetirementSubType;
                            break;
                        }
                    }
                    else if (this.icdoBenefitCalculationHeader.iintPlanId == busConstant.IAP_PLAN_ID)
                    {
                        if (!(this.ibusBenefitApplication.NotEligible) && this.ibusBenefitApplication.iblnEligible4IAPBenefitPreDeath)
                        {
                            this.icdoBenefitCalculationHeader.retirement_date = this.ibusBenefitApplication.icdoBenefitApplication.retirement_date;
                            this.icdoBenefitCalculationHeader.istrRetirementType = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().icdoPersonAccount.istrRetirementSubType;
                            this.icdoBenefitCalculationHeader.istrEarliestRetirementType = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().icdoPersonAccount.istrRetirementSubType;

                            break;
                        }
                    }
                    else if (!(this.ibusBenefitApplication.NotEligible) && this.ibusBenefitApplication.iclbEligiblePlans.Where(plan => plan == lstrPlanCode).Count() > 0)
                    {
                        this.icdoBenefitCalculationHeader.retirement_date = this.ibusBenefitApplication.icdoBenefitApplication.retirement_date;
                        this.icdoBenefitCalculationHeader.istrRetirementType = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().icdoPersonAccount.istrRetirementSubType;
                        this.icdoBenefitCalculationHeader.istrEarliestRetirementType = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().icdoPersonAccount.istrRetirementSubType;
                        break;
                    }

                }
                #endregion
            }
            //else
            //{
            //    this.ibusBenefitApplication.icdoBenefitApplication.retirement_date = busGlobalFunctions.GetLastDayofMonth(this.ibusPerson.icdoPerson.idtDateofBirth.AddYears(Convert.ToInt32(ldecParticipantCurrentAge))).AddDays(1);
            //    if (this.ibusBenefitApplication.icdoBenefitApplication.retirement_date <= this.icdoBenefitCalculationHeader.date_of_death)
            //        this.ibusBenefitApplication.icdoBenefitApplication.retirement_date = busGlobalFunctions.GetLastDayofMonth(this.icdoBenefitCalculationHeader.date_of_death).AddDays(1);

            //    this.icdoBenefitCalculationHeader.retirement_date = this.ibusBenefitApplication.icdoBenefitApplication.retirement_date;
            //    this.ibusBenefitApplication.idecAge = busGlobalFunctions.CalculatePersonAge(this.ibusPerson.icdoPerson.idtDateofBirth, this.icdoBenefitCalculationHeader.retirement_date);
            //    this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement = busGlobalFunctions.CalculatePersonAge(this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth, this.icdoBenefitCalculationHeader.retirement_date);
            //    this.icdoBenefitCalculationHeader.age = this.ibusBenefitApplication.idecAge;
            //    SetupPreRequisites_DeathCalculations();
            //}

            
            GetAgeAsOfEarliestRetirementDate();
            GetAgeAsOfCalculationDate();
        }


        public void CalculateRetirementDate(DateTime adtVesteddate, DateTime adtDateOfDeath)
        {
            if (adtDateOfDeath != DateTime.MinValue)
            {
                if (adtVesteddate > adtDateOfDeath)
                {
                    this.icdoBenefitCalculationHeader.retirement_date = adtVesteddate.GetLastDayofMonth().AddDays(1);
                }
                else
                {
                    this.icdoBenefitCalculationHeader.retirement_date = adtDateOfDeath.GetLastDayofMonth().AddDays(1);
                }
            }
        }

        public void GetAgeAsOfEarliestRetirementDate()
        {
            if (this.icdoSurvivorDetails == null)
            {
                icdoSurvivorDetails = new cdoPerson();
            }
            if (this.icdoBenefitCalculationHeader.retirement_date != DateTime.MinValue)
            {
                //PIR 862
                if (icdoBenefitCalculationHeader.calculation_type_value != busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE && iclbBenefitCalculationDetail != null && iclbBenefitCalculationDetail.Count > 0
                        && iclbBenefitCalculationDetail[0].iclbBenefitCalculationOptions != null && iclbBenefitCalculationDetail[0].iclbBenefitCalculationOptions.Count > 0
                        && iclbBenefitCalculationDetail[0].iclbBenefitCalculationOptions[0].ibusPlanBenefitXr != null
                        && iclbBenefitCalculationDetail[0].iclbBenefitCalculationOptions[0].ibusPlanBenefitXr.icdoPlanBenefitXr.benefit_option_value == busConstant.LIFE)
                {
                    this.ibusPerson.icdoPerson.idecAgeAtEarlyRetirement = busGlobalFunctions.CalculatePersonAge(this.ibusPerson.icdoPerson.idtDateofBirth, this.icdoBenefitCalculationHeader.benefit_commencement_date);
                }
                else
                {
                    this.ibusPerson.icdoPerson.idecAgeAtEarlyRetirement = busGlobalFunctions.CalculatePersonAge(this.ibusPerson.icdoPerson.idtDateofBirth, this.icdoBenefitCalculationHeader.retirement_date);
                }
            }
            if (this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth != DateTime.MinValue)
            {
                if (this.icdoBenefitCalculationHeader.retirement_date != DateTime.MinValue)
                {
                    //PIR 862
                    if (icdoBenefitCalculationHeader.calculation_type_value != busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE && iclbBenefitCalculationDetail != null && iclbBenefitCalculationDetail.Count > 0
                            && iclbBenefitCalculationDetail[0].iclbBenefitCalculationOptions != null && iclbBenefitCalculationDetail[0].iclbBenefitCalculationOptions.Count > 0 
                            && iclbBenefitCalculationDetail[0].iclbBenefitCalculationOptions[0].ibusPlanBenefitXr != null
                            && iclbBenefitCalculationDetail[0].iclbBenefitCalculationOptions[0].ibusPlanBenefitXr.icdoPlanBenefitXr.benefit_option_value == busConstant.LIFE)
                    {
                        this.icdoSurvivorDetails.idecAgeAtEarlyRetirement = busGlobalFunctions.CalculatePersonAge(this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth, this.icdoBenefitCalculationHeader.benefit_commencement_date);
                    }
                    else
                    {
                        this.icdoSurvivorDetails.idecAgeAtEarlyRetirement = busGlobalFunctions.CalculatePersonAge(this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth, this.icdoBenefitCalculationHeader.retirement_date);
                    }
                }
                this.icdoSurvivorDetails.idecSurvivorAgeAtDeath = busGlobalFunctions.CalculatePersonAge(this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth, this.icdoBenefitCalculationHeader.date_of_death);
            }

        }

        public void GetAgeAsOfCalculationDate()
        {
            if (this.icdoSurvivorDetails == null)
            {
                icdoSurvivorDetails = new cdoPerson();
            }
            
            if (this.icdoBenefitCalculationHeader.modified_date != DateTime.MinValue)
            {
                //PIR 862
                this.ibusPerson.icdoPerson.idecParticipantsAgeAsOfCalculationDate = 
                    busGlobalFunctions.CalculatePersonAge(this.ibusPerson.icdoPerson.idtDateofBirth, this.idtPaymentDate == DateTime.MinValue ? ibusBenefitApplication.icdoBenefitApplication.retirement_date : idtPaymentDate);
            }
            if (this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth != DateTime.MinValue)
            {
                if (this.icdoBenefitCalculationHeader.modified_date != DateTime.MinValue)
                {
                    //PIR 862
                    this.icdoSurvivorDetails.idecSurvivorsAgeAsOfCalculationDate = 
                        busGlobalFunctions.CalculatePersonAge(this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth, this.idtPaymentDate == DateTime.MinValue ? ibusBenefitApplication.icdoBenefitApplication.retirement_date : idtPaymentDate);

                }
            }
        }

        //PIR 862
        public bool CheckIfLifeFinalCalculation()
        {
            if (icdoBenefitCalculationHeader.calculation_type_value != busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE && iclbBenefitCalculationDetail != null && iclbBenefitCalculationDetail.Count > 0
                           && iclbBenefitCalculationDetail[0].iclbBenefitCalculationOptions != null && iclbBenefitCalculationDetail[0].iclbBenefitCalculationOptions.Count > 0
                           && iclbBenefitCalculationDetail[0].iclbBenefitCalculationOptions[0].ibusPlanBenefitXr != null
                           && iclbBenefitCalculationDetail[0].iclbBenefitCalculationOptions[0].ibusPlanBenefitXr.icdoPlanBenefitXr.benefit_option_value == busConstant.LIFE)
            {
                return true;
            }
            return false;
        }

        #endregion

        #region EE_UVHP
        public decimal CalculateEEUVHPAnnuityOption(decimal adecTotalEEUVHP)
        {
            decimal ldecMonthlyAnnuity = 1;
            decimal ldecJAndS50Factor = 1;
            DataTable ldtMonthlyLifeAnnuity = new DataTable();
            DataTable ldtMonthlyJS50Annuity = new DataTable();
            int lintParticipantAge = Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge);
            int lintSurvivorAge = Convert.ToInt32(Math.Floor(icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement));


            decimal ldecTotalEEUVHPAnnuity = new decimal();
            if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL)
            {
                //We hv factor tables till age 64 for Survivor
                int lintSurvivivorAge = Convert.ToInt32(this.icdoBenefitCalculationHeader.idecSurvivorFullAge);
                if (lintSurvivivorAge > 64)
                {
                    lintSurvivivorAge = 64;
                }
                ldtMonthlyLifeAnnuity = Select("cdoBenefitProvisionUvhpLifeFactor.GetUVHPLifeFactor", new object[2] { Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge), this.icdoBenefitCalculationHeader.benefit_commencement_date.Year });

                // ldtMonthlyJS50Annuity = Select("cdoBenefitProvisionUvhpFactor.GetEEUVHPFactor", new object[3] { ibusCalculation.GetPlanBenefitId((busConstant.MPIPP_PLAN_ID), busConstant.QJ50), Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge), lintSurvivivorAge });
            }
            else
            {
                int lintSurvivivorAge = Convert.ToInt32(this.iintSpouseEarliestRetrAge);
                if (lintSurvivivorAge > 64)
                {
                    lintSurvivivorAge = 64;
                }
                ldtMonthlyLifeAnnuity = Select("cdoBenefitProvisionUvhpLifeFactor.GetUVHPLifeFactor", new object[2] { Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge), this.icdoBenefitCalculationHeader.benefit_commencement_date.Year });

                // ldtMonthlyJS50Annuity = Select("cdoBenefitProvisionUvhpFactor.GetEEUVHPFactor", new object[3] { ibusCalculation.GetPlanBenefitId((busConstant.MPIPP_PLAN_ID), busConstant.QJ50), Convert.ToInt32(this.iintParticipantEarliestRetrAge), lintSurvivivorAge });

            }
            if (ldtMonthlyLifeAnnuity.Rows.Count > 0)
            {
                ldecMonthlyAnnuity = Convert.ToDecimal(ldtMonthlyLifeAnnuity.Rows[0][0]);
                ldecMonthlyAnnuity = Math.Round(ldecMonthlyAnnuity, 3);
            }
            //if (ldtMonthlyJS50Annuity.Rows.Count > 0)
            //{
            //    ldecJAndS50Factor = Convert.ToDecimal(ldtMonthlyJS50Annuity.Rows[0][0]);
            //}
            ldecJAndS50Factor = Convert.ToDecimal(Math.Round(Math.Min(1, Math.Max(0, 0.86 + 0.005 * (lintSurvivorAge - lintParticipantAge) + 0.006 * (65 - lintParticipantAge))), 3));

            ldecTotalEEUVHPAnnuity = (adecTotalEEUVHP * ldecJAndS50Factor) / ldecMonthlyAnnuity;

            busBenefitCalculationOptions lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
            lbusBenefitCalculationOptions.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.MPIPP_PLAN_ID, busConstant.LIFE), ldecJAndS50Factor / ldecMonthlyAnnuity, ldecTotalEEUVHPAnnuity, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_BENEFICIARY, this.icdoBenefitCalculationHeader.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.LIFE, ldecTotalEEUVHPAnnuity, true, true);
            lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.survivor_percent_amount = adecTotalEEUVHP;
            if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL)
            {
                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId && item.icdoBenefitCalculationDetail.uvhp_flag.IsNotNullOrEmpty()).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
            }
            else
            {
                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
            }
            return ldecTotalEEUVHPAnnuity;

        }

        public void CalculateEEUVHPLumpSumOption(decimal adecTotalEEUVHP)
        {
            busBenefitCalculationOptions lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
            lbusBenefitCalculationOptions.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.MPIPP_PLAN_ID, busConstant.LUMP_SUM), new decimal(), idecTotalEEUVHP, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_BENEFICIARY, this.icdoBenefitCalculationHeader.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.LUMP_SUM, adecTotalEEUVHP, true, true);
            lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.survivor_percent_amount = adecTotalEEUVHP;

            if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL)
            {

                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId && item.icdoBenefitCalculationDetail.uvhp_flag.IsNotNullOrEmpty()).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
                //Minimum Guarantee : EE + Interest
                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId && item.icdoBenefitCalculationDetail.uvhp_flag.IsNotNullOrEmpty()).FirstOrDefault().icdoBenefitCalculationDetail.minimum_guarantee_amount =
                    idecVestedEEContribution + idecNonVestedEEContribution + idecVestedEEInterest + idecNonVestedEEInterest + idecEEPartialInterest + idecNonVestedEEPartialInterest;
            }
            else
            {

                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
                //Minimum Guarantee : EE + Interest
                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().icdoBenefitCalculationDetail.minimum_guarantee_amount =
                    idecVestedEEContribution + idecNonVestedEEContribution + idecVestedEEInterest + idecNonVestedEEInterest + idecEEPartialInterest + idecNonVestedEEPartialInterest;
            }
        }

        public void CalculateUVHPAnnuityOption(decimal adecTotalUVHP)
        {
            decimal ldecMonthlyAnnuity = 1;
            decimal ldecJAndS50Factor = 1;
            DataTable ldtMonthlyLifeAnnuity = new DataTable();
            DataTable ldtMonthlyJS50Annuity = new DataTable();
            int lintParticipantAge = Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge);
            int lintSurvivorAge = Convert.ToInt32(Math.Floor(icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement));

            decimal ldecTotalUVHPAnnuity = new decimal();

            if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL)
            {
                int lintSurvivivorAge = Convert.ToInt32(this.icdoBenefitCalculationHeader.idecSurvivorFullAge);
                if (lintSurvivivorAge > 64)
                {
                    lintSurvivivorAge = 64;
                }
                ldtMonthlyLifeAnnuity = Select("cdoBenefitProvisionUvhpLifeFactor.GetUVHPLifeFactor", new object[2] { Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge), this.icdoBenefitCalculationHeader.benefit_commencement_date.Year });

                //ldtMonthlyJS50Annuity = Select("cdoBenefitProvisionUvhpFactor.GetEEUVHPFactor", new object[3] { ibusCalculation.GetPlanBenefitId((busConstant.MPIPP_PLAN_ID), busConstant.QJ50), Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge), lintSurvivivorAge });
            }
            else
            {
                int lintSurvivivorAge = Convert.ToInt32(this.iintSpouseEarliestRetrAge);
                if (lintSurvivivorAge > 64)
                {
                    lintSurvivivorAge = 64;
                }
                ldtMonthlyLifeAnnuity = Select("cdoBenefitProvisionUvhpLifeFactor.GetUVHPLifeFactor", new object[2] { Convert.ToInt32(this.iintParticipantEarliestRetrAge), this.icdoBenefitCalculationHeader.benefit_commencement_date.Year });

                // ldtMonthlyJS50Annuity = Select("cdoBenefitProvisionUvhpFactor.GetEEUVHPFactor", new object[3] { ibusCalculation.GetPlanBenefitId((busConstant.MPIPP_PLAN_ID), busConstant.QJ50), Convert.ToInt32(this.iintParticipantEarliestRetrAge), lintSurvivivorAge });
            }
            if (ldtMonthlyLifeAnnuity.Rows.Count > 0)
            {
                ldecMonthlyAnnuity = Convert.ToDecimal(ldtMonthlyLifeAnnuity.Rows[0][0]);
                ldecMonthlyAnnuity = Math.Round(ldecMonthlyAnnuity, 3);
            }
            //if (ldtMonthlyJS50Annuity.Rows.Count > 0)
            //{
            //    ldecJAndS50Factor = Convert.ToDecimal(ldtMonthlyJS50Annuity.Rows[0][0]);
            //}
            ldecJAndS50Factor = Convert.ToDecimal(Math.Round(Math.Min(1, Math.Max(0, 0.86 + 0.005 * (lintSurvivorAge - lintParticipantAge) + 0.006 * (65 - lintParticipantAge))), 3));
            ldecTotalUVHPAnnuity = (adecTotalUVHP * ldecJAndS50Factor) / ldecMonthlyAnnuity;

            busBenefitCalculationOptions lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
            lbusBenefitCalculationOptions.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.MPIPP_PLAN_ID, busConstant.LIFE), ldecJAndS50Factor / ldecMonthlyAnnuity, ldecTotalUVHPAnnuity, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_BENEFICIARY, this.icdoBenefitCalculationHeader.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.LIFE, ldecTotalUVHPAnnuity, true, true);
            lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.survivor_percent_amount = adecTotalUVHP;
            if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL)
            {
                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId && item.icdoBenefitCalculationDetail.uvhp_flag.IsNotNullOrEmpty()).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
            }
            else
            {
                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
            }
        }

        public void CalculateUVHPLumpSumOption(decimal adecTotalUVHP)
        {
            busBenefitCalculationOptions lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
            lbusBenefitCalculationOptions.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.MPIPP_PLAN_ID, busConstant.LUMP_SUM), new decimal(), adecTotalUVHP, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_BENEFICIARY, this.icdoBenefitCalculationHeader.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.LUMP_SUM, adecTotalUVHP, true, true);
            lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.survivor_percent_amount = adecTotalUVHP;

            if (adecTotalUVHP > 0)
            {
                if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL)
                {
                    //this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId && item.icdoBenefitCalculationDetail.uvhp_flag.IsNotNullOrEmpty()).FirstOrDefault().idecFinalPercentSurviviorEEUVHPAmount = ldecTotalUVHP;
                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId && item.icdoBenefitCalculationDetail.uvhp_flag.IsNotNullOrEmpty()).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
                }
                else
                {
                    //this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().idecFinalPercentSurviviorEEUVHPAmount = ldecTotalUVHP;
                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
                }
            }
        }

        #endregion

        #region Local52

        public busBenefitCalculationOptions CalculateLocal52JAndS100(decimal ldecTotalBenefitAmount)
        {
            busBenefitCalculationOptions lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
            decimal ldecLifeAnnuityFactor = new decimal();
            decimal ldecSurvivorLifeAnnuity = new decimal();

            if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE)
            {
                ldecLifeAnnuityFactor = this.CalculateLifeAnnuityBasedonMPIJandS100(this.iintSpouseEarliestRetrAge, this.iintParticipantEarliestRetrAge);
            }
            else
            {
                ldecLifeAnnuityFactor = this.CalculateLifeAnnuityBasedonMPIJandS100(Convert.ToInt32(this.icdoBenefitCalculationHeader.idecSurvivorFullAge), Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge));
            }

            ldecSurvivorLifeAnnuity = Math.Round(ldecTotalBenefitAmount * ldecLifeAnnuityFactor * .5M, 2);
            lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_52_PLAN_ID, busConstant.LIFE_ANNUTIY), ldecLifeAnnuityFactor, ldecLifeAnnuityFactor, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_BENEFICIARY, this.icdoBenefitCalculationHeader.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.LIFE_ANNUTIY, ldecSurvivorLifeAnnuity);
            lbusBenefitCalculationOption.icdoBenefitCalculationOptions.survivor_percent_amount = ldecTotalBenefitAmount;

            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);

            return lbusBenefitCalculationOption;
        }

        public busBenefitCalculationOptions CalculateLocal52TenYearCertainUnreduced(decimal adecTotalBenefitAmount)
        {
            int lintPlanBenefitID = ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_52_PLAN_ID, busConstant.TEN_YEARS_TERM_CERTAIN);

            busBenefitCalculationOptions lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() }; ;
            //decimal ldecBenefitOptionFactor = ibusCalculation.GetBenefitOptionFactor(this.icdoBenefitCalculationHeader.benefit_type_value, lintPlanBenefitID,Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge), busConstant.ZERO_INT);
            decimal ldecTotalBenefitAmount = Math.Round(adecTotalBenefitAmount, 2);
            lbusBenefitCalculationOption.LoadData(lintPlanBenefitID, decimal.One, new decimal(), busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_BENEFICIARY, this.icdoBenefitCalculationHeader.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.TEN_YEARS_TERM_CERTAIN, ldecTotalBenefitAmount);
            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_52_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);

            return lbusBenefitCalculationOption;
        }

        public busBenefitCalculationOptions CalculateLocal52TenYearCertainReduced(decimal adecTotalBenefitAmount)
        {
            int lintPlanBenefitID = ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_52_PLAN_ID, busConstant.TEN_YEARS_TERM_CERTAIN);
            //decimal ldecBenefitOptionFactor = ibusCalculation.GetBenefitOptionFactor(this.icdoBenefitCalculationHeader.benefit_type_value, lintPlanBenefitID, Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge), busConstant.ZERO_INT);
            busBenefitCalculationOptions lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() }; ;

            decimal ldecSurvivorAmount = Math.Round(adecTotalBenefitAmount * .5333M, 2);

            lbusBenefitCalculationOption.LoadData(lintPlanBenefitID, .5333M, new decimal(), busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_BENEFICIARY, this.icdoBenefitCalculationHeader.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.TEN_YEARS_TERM_CERTAIN, ldecSurvivorAmount);
            lbusBenefitCalculationOption.icdoBenefitCalculationOptions.survivor_percent_amount = adecTotalBenefitAmount;
            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_52_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
            return lbusBenefitCalculationOption;

        }

        public busBenefitCalculationOptions CalculateLocal52LumpSumBasedOnJAndS50(decimal ldecTotalBenefitAmount)
        {
            busBenefitCalculationOptions lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() }; ;
            decimal ldecLifeAnnuityFactor = new decimal();
            decimal ldecSurvivorLifeAnnuityAmount = new decimal();

            decimal ldecLumpSumFactor = new decimal();
            decimal ldecSurvivorLumpSumAmout = new decimal();

            ldecLifeAnnuityFactor = CalculateMPIPlanLifeAnnuityFactor(Convert.ToInt32(this.icdoBenefitCalculationHeader.idecSurvivorFullAge), Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge));
            ldecSurvivorLifeAnnuityAmount = Convert.ToDecimal(Math.Round(ldecTotalBenefitAmount * ldecLifeAnnuityFactor, 0, MidpointRounding.AwayFromZero) * .50M);

            ldecLumpSumFactor = ibusCalculation.GetLumpsumBenefitFactor(Convert.ToInt32(this.icdoBenefitCalculationHeader.idecSurvivorFullAge), this.icdoBenefitCalculationHeader.benefit_commencement_date.Year) * 12;
            ldecLumpSumFactor = Math.Round(ldecLumpSumFactor, 3);
            ldecSurvivorLumpSumAmout = Convert.ToDecimal(Math.Ceiling(ldecSurvivorLifeAnnuityAmount * ldecLumpSumFactor));
            lbusBenefitCalculationOption.icdoBenefitCalculationOptions.istrBenefitOptionDescription = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(1504, busConstant.JOINT_50_PERCENT_POPUP_ANNUITY);
            lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_52_PLAN_ID, busConstant.LUMP_SUM), ldecLumpSumFactor, new decimal(), busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_BENEFICIARY, this.icdoBenefitCalculationHeader.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.LUMP_SUM, ldecSurvivorLumpSumAmout);
            return lbusBenefitCalculationOption;
        }

        public busBenefitCalculationOptions CalculateLocal52LumpSumBasedOnJAndS100(decimal ldecTotalBenefitAmount)
        {
            busBenefitCalculationOptions lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() }; ;

            decimal ldecLifeAnnuityFactor = new decimal();
            decimal ldecSurvivorLifeAnnuityAmount = new decimal();

            decimal ldecLumpSumFactor = new decimal();
            decimal ldecSurvivorLumpSumAmout = new decimal();

            ldecLifeAnnuityFactor = this.CalculateLifeAnnuityBasedonMPIJandS100(Convert.ToInt32(this.icdoBenefitCalculationHeader.idecSurvivorFullAge), Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge));
            ldecSurvivorLifeAnnuityAmount = Convert.ToDecimal(Math.Round(ldecTotalBenefitAmount * ldecLifeAnnuityFactor, 0, MidpointRounding.AwayFromZero));

            ldecLumpSumFactor = ibusCalculation.GetLumpsumBenefitFactor(Convert.ToInt32(this.icdoBenefitCalculationHeader.idecSurvivorFullAge), this.icdoBenefitCalculationHeader.benefit_commencement_date.Year) * 12;
            ldecLumpSumFactor = Math.Round(ldecLumpSumFactor, 3);

            ldecSurvivorLumpSumAmout = Convert.ToDecimal(Math.Ceiling(ldecSurvivorLifeAnnuityAmount * ldecLumpSumFactor));

            lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_52_PLAN_ID, busConstant.LUMP_SUM), ldecLumpSumFactor, new decimal(), busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_BENEFICIARY, this.icdoBenefitCalculationHeader.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.LUMP_SUM, ldecSurvivorLumpSumAmout);
            return lbusBenefitCalculationOption;
        }

        public decimal CalculateLifeAnnuityBasedonMPIJandS100(int aintSurvivorAge, int aintParticipantAge)
        {
            decimal ldecAnnuityBenefitOptionFactor = new decimal();

            double ldblBenefitOptionFactor = 0.75 + 0.01 * (aintSurvivorAge - aintParticipantAge) +
                      0.006 * (65 - aintParticipantAge);
            ldecAnnuityBenefitOptionFactor = Convert.ToDecimal(ldblBenefitOptionFactor);
            ldecAnnuityBenefitOptionFactor = Math.Min(1, ldecAnnuityBenefitOptionFactor);

            ldecAnnuityBenefitOptionFactor = Math.Round(ldecAnnuityBenefitOptionFactor, 3);
            return ldecAnnuityBenefitOptionFactor;
        }

        #endregion

        public void SpawnFinalRetirementCalculation(int aintBenefitApplicationDetailId, int aintPersonAccountId, int aintPlanId, string astrPlanCode, DateTime adtVestedDate, string astrBenefitSubTypeValue, string astrBenefitOptionValue, string astrAdjustmentFlag = "")
        {
            this.PopulateInitialDataBenefitCalculationDetails(aintBenefitApplicationDetailId, aintPersonAccountId, aintPlanId, astrPlanCode, adtVestedDate, astrBenefitSubTypeValue);
            this.CheckQualifiedSpouseExists();
            this.icdoBenefitCalculationHeader.istrRetirementType = astrBenefitSubTypeValue;
            decimal ldecTotalBenefitAmount = new decimal();

            #region SWITCH CASE - INITIATE CALCULATION BASED ON THE PLAN
            if (!this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.IsNullOrEmpty())
            {
                //Switched back to MPI Factors and Reduction factors.
                switch (astrPlanCode)
                {
                    case busConstant.Local_161:
                        CalculateLocalBenefits(astrBenefitOptionValue, busConstant.BOOL_TRUE);
                        break;

                    case busConstant.Local_52:
                        CalculateLocal52Benefit(astrBenefitOptionValue, astrBenefitSubTypeValue);
                        break;

                    case busConstant.Local_600:
                        CalculateLocalBenefits(astrBenefitOptionValue, busConstant.BOOL_TRUE);
                        break;

                    case busConstant.Local_666:
                        CalculateLocalBenefits(astrBenefitOptionValue, busConstant.BOOL_TRUE);
                        break;

                    case busConstant.LOCAL_700:
                        CalculateLocalBenefits(astrBenefitOptionValue, busConstant.BOOL_TRUE);
                        break;

                    case busConstant.MPIPP:
                        if (this.ibusBenefitApplication.aclbPersonWorkHistory_MPI != null && this.ibusBenefitApplication.aclbPersonWorkHistory_MPI.Count > 0)
                        {
                            this.CalculateMPIBenefitOptions(astrBenefitOptionValue);
                            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().icdoBenefitCalculationDetail.ee_derived_benefit_amount = ibusCalculation.CalculateEEDerivedBenefitAsOfRetirementDate(this, null);

                        }
                        break;

                    case busConstant.IAP:
                        if (this.ibusBenefitApplication.aclbPersonWorkHistory_IAP != null && this.ibusBenefitApplication.aclbPersonWorkHistory_IAP.Count > 0)
                        {
                            this.CalculateIAPBenefitAmount(astrBenefitOptionValue, astrAdjustmentFlag);
                        }
                        break;
                }
            }
            #endregion
        }

        /// <summary>
        /// Holds True for everone except L700
        /// </summary>
        public void CheckQualifiedSpouseExists()
        {
            this.iblnCheckIfSurvivorIsSpouse = false;
            if (this.icdoBenefitCalculationHeader.istrSurvivorTypeValue == busConstant.SURVIVOR_TYPE_PER)
            {
                if (this.icdoBenefitCalculationHeader.iintPlanId != busConstant.LOCAL_700_PLAN_ID)
                {
                    if (this.icdoBenefitCalculationHeader.date_of_death != DateTime.MinValue)
                    {
                        DataTable ldtbLatestSpouseInfo = busBase.Select("cdoRelationship.CheckIfQualifiedSpouseExists", new object[3] { this.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, this.icdoBenefitCalculationHeader.date_of_death });
                        if (ldtbLatestSpouseInfo.Rows.Count > 0 && ldtbLatestSpouseInfo.Rows[0][enmRelationship.beneficiary_person_id.ToString()].ToString().IsNotNullOrEmpty())
                        {
                            int Spouse_Person_ID = Convert.ToInt32(ldtbLatestSpouseInfo.Rows[0][enmRelationship.beneficiary_person_id.ToString()]);

                            int QualifiedDROExists = (int)DBFunction.DBExecuteScalar("cdoDroApplication.CheckCountofApprovedDROforPersonandPayee", new object[2] { Spouse_Person_ID, this.ibusPerson.icdoPerson.person_id }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                            //IF COUNT query Returns 1 then we know there is a DRO and hence no Qualified Spouse Exists
                            if (QualifiedDROExists == 0)
                            {
                                this.iblnCheckIfSurvivorIsSpouse = true;
                            }
                        }
                    }
                }
                else
                {
                    DataTable ldtbLatestSpouseInfo = busBase.Select("cdoRelationship.GetDateOfMarriage", new object[2] { this.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id });
                    if (ldtbLatestSpouseInfo.Rows.Count > 0 && ldtbLatestSpouseInfo.Rows[0][enmRelationship.date_of_marriage.ToString()].ToString().IsNotNullOrEmpty())
                    {
                        int QualifiedDROExists = (int)DBFunction.DBExecuteScalar("cdoDroApplication.CheckCountofApprovedDROforPersonandPayee", new object[2] { this.icdoBenefitCalculationHeader.beneficiary_person_id, this.ibusPerson.icdoPerson.person_id }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                        //IF COUNT query Returns 1 then we know there is a DRO and hence no Qualified Spouse Exists
                        if (QualifiedDROExists == 0)
                        {
                            this.iblnCheckIfSurvivorIsSpouse = true;
                        }
                    }
                }
            }
        }

        public void LoadDeathCollectionForCorrespondence()
        {
            this.iclbbusBenefitCalculationOptions = new Collection<busBenefitCalculationOptions>();
            decimal ldecLateAdjustmentAmt = busConstant.ZERO_DECIMAL;
            decimal ldecSurvivorLifeAnnuityAmount = decimal.Zero;
            istrShowAnnuity = "See Annuity Election Form";
            this.istrHasLifeMPI = busConstant.FLAG_NO;
            this.istrL52 = busConstant.FLAG_NO;
            if (this.iclbBenefitCalculationDetail.Count() > 0)
            {

                if (this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_52_PLAN_ID).Count() > 0)
                {
                    busBenefitCalculationDetail lbus = this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_52_PLAN_ID).FirstOrDefault();

                    if (lbus.icdoBenefitCalculationDetail.local52_rule_value == busConstant.L52_RULE_4 ||
                        lbus.icdoBenefitCalculationDetail.local52_rule_value == busConstant.L52_RULE_5)
                    {
                        istrShowAnnuity = "N/A";
                    }
                    if (lbus.iclbBenefitCalculationOptions.Count() > 0)
                    {
                        foreach (busBenefitCalculationOptions lbusOption in lbus.iclbBenefitCalculationOptions)
                        {
                            if (lbusOption.ibusPlanBenefitXr.IsNull())
                                lbusOption.ibusPlanBenefitXr.FindPlanBenefitXr(lbusOption.icdoBenefitCalculationOptions.plan_benefit_id);
                            if (lbus.icdoBenefitCalculationDetail.local52_rule_value == busConstant.L52_RULE_2 || lbus.icdoBenefitCalculationDetail.local52_rule_value == busConstant.L52_RULE_3 || lbus.icdoBenefitCalculationDetail.local52_rule_value == busConstant.L52_RULE_4 ||
                                    lbus.icdoBenefitCalculationDetail.local52_rule_value == busConstant.L52_RULE_5 || lbus.icdoBenefitCalculationDetail.local52_rule_value == busConstant.L52_RULE_6)
                            {
                                if (lbusOption.ibusPlanBenefitXr.icdoPlanBenefitXr.benefit_option_value == busConstant.TEN_YEARS_TERM_CERTAIN)
                                {

                                    this.istrHasLifeMPI = busConstant.FLAG_YES;
                                    this.istrL52 = busConstant.FLAG_YES;
                                    //Kunal : For Rule 2 to 6 there is no lump sum its tEn yeat term certain.
                                    idecParticipantLumpSumPlusUVHPBenefitAmount = lbusOption.icdoBenefitCalculationOptions.survivor_amount;
                                }
                            }
                        }
                    }
                }
            }

            int lintTotalQualifiedYear = 0;
            this.CheckQualifiedSpouseExists();
            this.ibusBenefitApplication.DetermineVesting();
            this.ibusBenefitApplication.CheckIfQualifiedSpouseinDeath();
            if (this.icdoBenefitCalculationHeader.iintPlanId == busConstant.LOCAL_161_PLAN_ID || this.icdoBenefitCalculationHeader.iintPlanId == busConstant.LOCAL_600_PLAN_ID ||
                this.icdoBenefitCalculationHeader.iintPlanId == busConstant.LOCAL_666_PLAN_ID || this.icdoBenefitCalculationHeader.iintPlanId == busConstant.LOCAL_700_PLAN_ID ||
                this.icdoBenefitCalculationHeader.iintPlanId == busConstant.LOCAL_52_PLAN_ID)
            {
                //this.LoadAllRetirementContributions();

                if (ibusPerson.iclbPersonAccount != null && !ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).IsNullOrEmpty())
                {
                    LoadAllRetirementContributions(ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault());
                }
                else
                {
                    LoadAllRetirementContributions(null);
                }
            }

            string lstrPlanCode = String.Empty;
            DataTable ldtbPlanCode = busBase.Select("cdoPlan.GetPlanCodebyId", new object[1] { this.icdoBenefitCalculationHeader.iintPlanId });
            if (ldtbPlanCode.Rows.Count > 0)
            {
                lstrPlanCode = ldtbPlanCode.Rows[0][0].ToString();
            }

            string lstrRetirementType = string.Empty;
            int lintPartcipantage = busGlobalFunctions.AgeInYearsAsOfDate(this.ibusBenefitApplication.ibusPerson.icdoPerson.idtDateofBirth, this.icdoBenefitCalculationHeader.retirement_date);
            int lintSpouseAge = busGlobalFunctions.AgeInYearsAsOfDate(this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth, this.icdoBenefitCalculationHeader.retirement_date);
            int lintPersonAccountId = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).First().icdoPersonAccount.person_account_id;
            //WorkItem#17261
            this.istrPlanLocal = lstrPlanCode;
            //if (lstrPlanCode != busConstant.MPIPP && lstrPlanCode != busConstant.IAP)
            //{
            //    this.istrPlanLocal = lstrPlanCode;
            //}

            //this.ibusBenefitApplication.LoadandProcessWorkHistory_ForAllPlans();
            busBenefitCalculationOptions lbusBenefitCalculationOptions = null;
            if (this.ibusBenefitApplication.aclbPersonWorkHistory_MPI.IsNotNull())
            {
                lintTotalQualifiedYear = this.ibusBenefitApplication.aclbPersonWorkHistory_MPI.OrderByDescending(item => item.year).ToList().ToCollection().FirstOrDefault().qualified_years_count;
                if (lintTotalQualifiedYear > 10)
                {
                    iblnQualifiedYrs = busConstant.BOOL_TRUE;
                }
            }

            if (this.icdoBenefitCalculationHeader.date_of_death != DateTime.MinValue)
                this.ibusBenefitApplication.idecAgeAtDeath = busGlobalFunctions.CalculatePersonAge(this.ibusPerson.icdoPerson.idtDateofBirth, this.icdoBenefitCalculationHeader.date_of_death.GetLastDayofMonth().AddDays(1));
            this.ibusBenefitApplication.idecAgeAtL52Merger = busGlobalFunctions.CalculatePersonAge(this.ibusPerson.icdoPerson.idtDateofBirth, Convert.ToDateTime(busConstant.MERGER_DATE_STRING));


            for (int i = lintPartcipantage; i <= 65; i++)
            {
                #region Eligibility Parameters

                idecSurvivorAmountForCor = new decimal();
                lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                this.ibusBenefitApplication.idecAge = i;
                if (i != lintPartcipantage)
                {
                    if (this.ibusPerson.icdoPerson.idtDateofBirth.Day == 1)
                    {
                        this.ibusBenefitApplication.icdoBenefitApplication.retirement_date = this.ibusPerson.icdoPerson.idtDateofBirth.AddYears(i);
                    }
                    else
                    {
                        this.ibusBenefitApplication.icdoBenefitApplication.retirement_date = busGlobalFunctions.GetLastDayofMonth(this.ibusPerson.icdoPerson.idtDateofBirth.AddYears(i)).AddDays(1);
                    }
                }
                else
                {
                    this.ibusBenefitApplication.icdoBenefitApplication.retirement_date = this.icdoBenefitCalculationHeader.retirement_date;
                }

                lintSpouseAge = busGlobalFunctions.AgeInYearsAsOfDate(this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth, this.ibusBenefitApplication.icdoBenefitApplication.retirement_date);
                this.ibusBenefitApplication.SetupPrerequisitesDeath();

                if (this.icdoBenefitCalculationHeader.iintPlanId == busConstant.MPIPP_PLAN_ID)
                {
                    if (!(this.ibusBenefitApplication.NotEligible) && this.ibusBenefitApplication.iblnEligbile4MPIBenefitPreDeath)
                    {
                        lstrRetirementType = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().icdoPersonAccount.istrRetirementSubType;

                    }
                }
                else if (this.icdoBenefitCalculationHeader.iintPlanId == busConstant.LOCAL_700_PLAN_ID)
                {
                    if (!(this.ibusBenefitApplication.NotEligible) && this.ibusBenefitApplication.iblnEligbile4L700BenefitPreDeath)
                    {
                        lstrRetirementType = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().icdoPersonAccount.istrRetirementSubType;
                    }
                }
                else if (this.icdoBenefitCalculationHeader.iintPlanId == busConstant.LOCAL_161_PLAN_ID)
                {
                    if (!(this.ibusBenefitApplication.NotEligible) && this.ibusBenefitApplication.iblnEligbile4L161BenefitPreDeath)
                    {
                        lstrRetirementType = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().icdoPersonAccount.istrRetirementSubType;
                    }
                }
                else if (this.icdoBenefitCalculationHeader.iintPlanId == busConstant.LOCAL_666_PLAN_ID)
                {
                    if (!(this.ibusBenefitApplication.NotEligible) && this.ibusBenefitApplication.iblnEligbile4L666BenefitPreDeath)
                    {
                        lstrRetirementType = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().icdoPersonAccount.istrRetirementSubType;
                    }
                }
                else if (this.icdoBenefitCalculationHeader.iintPlanId == busConstant.LOCAL_600_PLAN_ID)
                {
                    if (!(this.ibusBenefitApplication.NotEligible) && this.ibusBenefitApplication.iblnEligbile4L600BenefitPreDeath)
                    {
                        lstrRetirementType = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().icdoPersonAccount.istrRetirementSubType;
                    }
                }
                else if (this.icdoBenefitCalculationHeader.iintPlanId == busConstant.LOCAL_52_PLAN_ID)
                {
                    if (!(this.ibusBenefitApplication.NotEligible) && !string.IsNullOrEmpty(this.ibusBenefitApplication.istrLocal52RuleForDeathCalculation))
                    {
                        lstrRetirementType = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().icdoPersonAccount.istrRetirementSubType;
                    }
                }

                #endregion

                switch (lstrPlanCode)
                {
                    #region MPI
                    case busConstant.MPIPP:
                        if (this.ibusBenefitApplication.aclbPersonWorkHistory_MPI != null && this.ibusBenefitApplication.aclbPersonWorkHistory_MPI.Count > 0)
                        {
                            ldecLateAdjustmentAmt = busConstant.ZERO_DECIMAL;

                            idecSurvivorAmountForCor = ibusCalculation.CalculateReducedBenefit(ibusPerson, busConstant.BENEFIT_TYPE_RETIREMENT, i, this.ibusBenefitApplication.icdoBenefitApplication.retirement_date,
                            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().icdoBenefitCalculationDetail.vested_date,
                            ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault(),
                            ibusBenefitApplication, busConstant.BOOL_FALSE,
                            new Collection<busBenefitCalculationDetail>(), null, lintTotalQualifiedYear, this.iclbBenefitCalculationDetail[0].icdoBenefitCalculationDetail.unreduced_benefit_amount, lstrRetirementType, false,
                            ibusBenefitApplication.aclbPersonWorkHistory_MPI, this.iclbPersonAccountRetirementContribution, ref ldecLateAdjustmentAmt);
                            if (lstrRetirementType == busConstant.RETIREMENT_TYPE_LATE)
                                idecSurvivorAmountForCor = ldecLateAdjustmentAmt;

                            idecSurvivorAmountForCor = this.CalculateLifeAnnuityBenefit(idecSurvivorAmountForCor, lintSpouseAge, i);
                        }
                        break;

                    #endregion

                    #region Locals
                    case busConstant.Local_600:
                    case busConstant.Local_666:
                    case busConstant.LOCAL_700:
                    case busConstant.Local_161:
                        idecSurvivorAmountForCor = this.ibusCalculation.CalculateTotalBenefitAmtForDeathLocals(lstrRetirementType,
                        this.ibusBenefitApplication.icdoBenefitApplication.retirement_date, this.ibusPerson.icdoPerson.idtDateofBirth,
                        this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_frozen_benefit_amount),
                        false, this.ibusBenefitApplication, this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_pre_bis_amount),
                        this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_post_bis_amount),
                        null, null, this.icdoBenefitCalculationHeader.iintPlanId, i,
                        Convert.ToInt32(this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Max(item => item.icdoPersonAccountRetirementContribution.computational_year)),
                        this.iclbPersonAccountRetirementContribution, busConstant.BOOL_FALSE, this.ibusPerson.icdoPerson.person_id, adtEarliestRetirementDate: icdoBenefitCalculationHeader.retirement_date);//PIR 862
                        idecSurvivorAmountForCor = this.CalculateLifeAnnuityBenefit(idecSurvivorAmountForCor, lintSpouseAge, i);
                        break;

                    #endregion

                    #region Local52
                    case busConstant.Local_52:
                        idecSurvivorAmountForCor = this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_frozen_benefit_amount);
                        if (this.ibusBenefitApplication.istrLocal52RuleForDeathCalculation == busConstant.L52_RULE_1 || this.ibusBenefitApplication.istrLocal52RuleForDeathCalculation == busConstant.L52_RULE_3 ||
                  this.ibusBenefitApplication.istrLocal52RuleForDeathCalculation == busConstant.L52_RULE_2 || this.ibusBenefitApplication.istrLocal52RuleForDeathCalculation == busConstant.L52_RULE_6)
                        {
                            idecSurvivorAmountForCor = this.ibusCalculation.CalculateTotalBenefitAmtForDeathLocals(lstrRetirementType,
                                                   this.ibusBenefitApplication.icdoBenefitApplication.retirement_date, this.ibusPerson.icdoPerson.idtDateofBirth,
                                                   this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_frozen_benefit_amount),
                                                   false, this.ibusBenefitApplication, this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_pre_bis_amount),
                                                   this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_post_bis_amount),
                                                   null, null, this.icdoBenefitCalculationHeader.iintPlanId, i,
                                                   Convert.ToInt32(this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Max(item => item.icdoPersonAccountRetirementContribution.computational_year)),
                                                   this.iclbPersonAccountRetirementContribution, busConstant.BOOL_FALSE, this.ibusPerson.icdoPerson.person_id, adtEarliestRetirementDate: icdoBenefitCalculationHeader.retirement_date);//PIR 862
                        }
                        if (this.ibusBenefitApplication.istrLocal52RuleForDeathCalculation == busConstant.L52_RULE_1 || this.ibusBenefitApplication.istrLocal52RuleForDeathCalculation == busConstant.L52_RULE_3 || this.ibusBenefitApplication.istrLocal52RuleForDeathCalculation == busConstant.L52_RULE_6)
                        {
                            idecSurvivorAmountForCor = this.CalculateLifeAnnuityBenefit(idecSurvivorAmountForCor, lintSpouseAge, i);
                        }
                        else if (this.ibusBenefitApplication.istrLocal52RuleForDeathCalculation == busConstant.L52_RULE_2)
                        {
                            idecSurvivorAmountForCor = idecSurvivorAmountForCor * CalculateLifeAnnuityBasedonMPIJandS100(lintSpouseAge, i) * .5M;
                            idecSurvivorAmountForCor = Math.Round(idecSurvivorAmountForCor, 2);
                        }
                        //if (this.ibusBenefitApplication.icdoBenefitApplication.retirement_date == this.icdoBenefitCalculationHeader.retirement_date &&
                        //    this.ibusBenefitApplication.istrLocal52RuleForDeathCalculation != busConstant.L52_RULE_1)
                        //{
                        //    busBenefitCalculationOptions lbusBenefitCalculationOptionTen = null;
                        //    lbusBenefitCalculationOptionTen = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                        //    decimal ldecTenYearCertainAmount = this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_frozen_benefit_amount);
                        //    if (this.ibusBenefitApplication.istrLocal52RuleForDeathCalculation == busConstant.L52_RULE_5 || this.ibusBenefitApplication.istrLocal52RuleForDeathCalculation == busConstant.L52_RULE_3 || this.ibusBenefitApplication.istrLocal52RuleForDeathCalculation == busConstant.L52_RULE_6)
                        //    {
                        //        ldecTenYearCertainAmount = ldecTenYearCertainAmount * .5333M;
                        //    }
                        //    //Kunal : No Need to show ages for participant and spouse as per Avie for Ten Year Certain
                        //    lbusBenefitCalculationOptionTen.istrRetirementType = "Ten Year Certain";
                        //    lbusBenefitCalculationOptionTen.icdoBenefitCalculationOptions.survivor_amount = ldecTenYearCertainAmount;
                        //    lbusBenefitCalculationOptionTen.istrInitialOne = string.Empty;
                        //    lbusBenefitCalculationOptionTen.idtEarliestBenefitCommencementDate = this.ibusBenefitApplication.icdoBenefitApplication.retirement_date;
                        //    iclbbusBenefitCalculationOptions.Add(lbusBenefitCalculationOptionTen);
                        //}


                        break;

                    #endregion

                }

                #region Load Correspondence Collection
                lbusBenefitCalculationOptions.iintParticipantAge = i;
                lbusBenefitCalculationOptions.iintSpouseAge = lintSpouseAge;
                lbusBenefitCalculationOptions.istrRetirementType = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(1501, lstrRetirementType);
                if (this.icdoBenefitCalculationHeader.iintPlanId == busConstant.LOCAL_52_PLAN_ID)
                {
                    lbusBenefitCalculationOptions.istrRetirementType = "Life Annuity - " + lbusBenefitCalculationOptions.istrRetirementType;
                }
                lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.survivor_amount = idecSurvivorAmountForCor;
                lbusBenefitCalculationOptions.istrInitialOne = string.Empty;
                lbusBenefitCalculationOptions.idtEarliestBenefitCommencementDate = this.ibusBenefitApplication.icdoBenefitApplication.retirement_date;
                iclbbusBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
                #endregion
            }

            if (lbusBenefitCalculationOptions != null)
                istrNormalRetrDt = String.Format("{0:MMMM dd, yyyy}", lbusBenefitCalculationOptions.idtEarliestBenefitCommencementDate);
        }

        public void LoadPropertiesForDeath(string astrTemplateName)
        {
            foreach (busBenefitCalculationDetail lbusBenefitCalcDetail in this.iclbBenefitCalculationDetail)
            {
                if (lbusBenefitCalcDetail.icdoBenefitCalculationDetail.plan_id != busConstant.IAP_PLAN_ID)
                {
                    idecParticipantLumpSumBenefitAmount = decimal.Zero;
                    foreach (busBenefitCalculationOptions lbusCalcOption in lbusBenefitCalcDetail.iclbBenefitCalculationOptions)
                    {
                        if (lbusCalcOption.ibusPlanBenefitXr.IsNull())
                            lbusCalcOption.ibusPlanBenefitXr.FindPlanBenefitXr(lbusCalcOption.icdoBenefitCalculationOptions.plan_benefit_id);
                        if (lbusCalcOption.ibusPlanBenefitXr.icdoPlanBenefitXr.benefit_option_value == busConstant.LUMP_SUM && lbusBenefitCalcDetail.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID)
                        {
                            idecParticipantLumpSumPlusUVHPBenefitAmount += lbusCalcOption.icdoBenefitCalculationOptions.survivor_amount;
                        }
                        else if (lbusCalcOption.ibusPlanBenefitXr.icdoPlanBenefitXr.benefit_option_value == busConstant.LUMP_SUM)
                        {
                            idecParticipantLumpSumPlusUVHPBenefitAmount += lbusCalcOption.icdoBenefitCalculationOptions.survivor_amount;
                        }
                    }
                }
            }
            if (astrTemplateName == busConstant.ACTIVE_DEATH_BENEFICIARY_PACKAGE_COVER_LETTER_PENSION_AND_IAP ||
                astrTemplateName == busConstant.ACTIVE_DEATH_BENEFICIARY_PACKAGE_COVER_LETTER_PENSION_ONLY
                || astrTemplateName == busConstant.SURVIVING_SPOUSE_BENEFIT_CALCULATION_DETAILS
                || astrTemplateName == busConstant.Active_Death_Beneficiary_Package_Pension_Iap
                || astrTemplateName == busConstant.Active_Death_Beneficiary_Package_Pension_Only
                || astrTemplateName == busConstant.Active_Death_Beneficiary_Package_LUMPSUM)
            {
                istrHasLifeMPI = busConstant.FLAG_NO;
                istrHasLumpMPI = busConstant.FLAG_NO;
                istrHasLifeIAP = busConstant.FLAG_NO;
                istrHasLumpIAP = busConstant.FLAG_NO;
                istrHasLifeOptionAnyPlan = busConstant.FLAG_NO;
                istrHasLumpOptionAnyPlan = busConstant.FLAG_NO;

                istrHasBothEEUV = busConstant.FLAG_NO;
                istrHasOnlyEE = busConstant.FLAG_NO;
                istrHasonlyUVHP = busConstant.FLAG_NO;


                if (!this.iclbBenefitCalculationDetail.IsNullOrEmpty())
                {
                    foreach (busBenefitCalculationDetail lbusBenefitCalcDetail in this.iclbBenefitCalculationDetail)
                    {
                        foreach (busBenefitCalculationOptions lbusCalcOptions in lbusBenefitCalcDetail.iclbBenefitCalculationOptions)
                        {
                            if (lbusBenefitCalcDetail.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID)
                            {

                                idecParticipantAccruedBenefit = lbusBenefitCalcDetail.icdoBenefitCalculationDetail.idecBenefitsAfterQDROOffSet;
                                idecERF = Math.Round(lbusBenefitCalcDetail.icdoBenefitCalculationDetail.early_reduction_factor,3);

                                if (lbusCalcOptions.ibusPlanBenefitXr.icdoPlanBenefitXr.benefit_option_value == busConstant.LUMP_SUM && lbusCalcOptions.icdoBenefitCalculationOptions.ee_flag != busConstant.FLAG_YES)
                                {
                                    istrHasLumpMPI = busConstant.FLAG_YES;
                                    istrHasLumpOptionAnyPlan = busConstant.FLAG_YES;

                                    idecPresentValueFactor = Math.Round(lbusCalcOptions.icdoBenefitCalculationOptions.benefit_option_factor,3);
                                    idecLump = lbusCalcOptions.icdoBenefitCalculationOptions.survivor_amount;

                                }
                                if (lbusCalcOptions.ibusPlanBenefitXr.icdoPlanBenefitXr.benefit_option_value == busConstant.LIFE_ANNUTIY && lbusCalcOptions.icdoBenefitCalculationOptions.ee_flag != busConstant.FLAG_YES)
                                {
                                    istrHasLifeMPI = busConstant.FLAG_YES;
                                    istrHasLifeOptionAnyPlan = busConstant.FLAG_YES;

                                    idecJSFactor = Math.Round(lbusCalcOptions.icdoBenefitCalculationOptions.benefit_option_factor,3);
                                    idecSpouseJS = Math.Round(lbusCalcOptions.icdoBenefitCalculationOptions.survivor_amount,2);

                                    idecPartJS50 = Math.Round(idecParticipantAccruedBenefit * idecERF * idecJSFactor,2);

                                }
                                if (lbusCalcOptions.icdoBenefitCalculationOptions.ee_flag == busConstant.FLAG_YES)
                                {
                                    if (lbusBenefitCalcDetail.icdoBenefitCalculationDetail.early_reduced_benefit_amount > decimal.Zero)
                                    {
                                        if ((lbusBenefitCalcDetail.icdoBenefitCalculationDetail.non_vested_ee_amount > decimal.Zero)
                                            && lbusBenefitCalcDetail.icdoBenefitCalculationDetail.total_uvhp_contribution_amount > decimal.Zero)
                                        {
                                            //If the participant is elible for accrued benefit then he is only eligible for nonvested EE && UVHP
                                            istrHasBothEEUV = busConstant.FLAG_YES;
                                            idecEEContriAmt = lbusBenefitCalcDetail.icdoBenefitCalculationDetail.non_vested_ee_amount;
                                            idecEEIntAmt = lbusBenefitCalcDetail.icdoBenefitCalculationDetail.non_vested_ee_interest;


                                            idecUVHPContriAmt = lbusBenefitCalcDetail.icdoBenefitCalculationDetail.total_uvhp_contribution_amount;
                                            idecUVHPIntAmt = lbusBenefitCalcDetail.icdoBenefitCalculationDetail.total_uvhp_interest_amount;

                                            idecEEUVHPInterest = idecEEIntAmt + idecUVHPIntAmt;
                                            idecEEUVHPTotal = idecEEContriAmt + idecUVHPContriAmt + idecEEUVHPInterest;

                                        }
                                        else if (lbusBenefitCalcDetail.icdoBenefitCalculationDetail.total_uvhp_contribution_amount > decimal.Zero)
                                        {
                                            istrHasonlyUVHP = busConstant.FLAG_YES;
                                            idecUVHPContriAmt = lbusBenefitCalcDetail.icdoBenefitCalculationDetail.total_uvhp_contribution_amount;
                                            idecEEUVHPInterest = lbusBenefitCalcDetail.icdoBenefitCalculationDetail.total_uvhp_interest_amount;
                                            idecEEUVHPTotal = idecUVHPContriAmt + idecEEUVHPInterest;
                                        }
                                        else if (lbusBenefitCalcDetail.icdoBenefitCalculationDetail.non_vested_ee_amount > decimal.Zero)
                                        {
                                            istrHasOnlyEE = busConstant.FLAG_YES;
                                            idecEEContriAmt = lbusBenefitCalcDetail.icdoBenefitCalculationDetail.non_vested_ee_amount;
                                            idecEEUVHPInterest = lbusBenefitCalcDetail.icdoBenefitCalculationDetail.non_vested_ee_interest;
                                            idecEEUVHPTotal = idecEEContriAmt + idecEEUVHPInterest;
                                        }

                                    }
                                    else if ((lbusBenefitCalcDetail.icdoBenefitCalculationDetail.non_vested_ee_amount > decimal.Zero || lbusBenefitCalcDetail.icdoBenefitCalculationDetail.vested_ee_amount > decimal.Zero)
                                            && lbusBenefitCalcDetail.icdoBenefitCalculationDetail.total_uvhp_contribution_amount > decimal.Zero)
                                    {
                                        istrHasLumpMPI = busConstant.FLAG_YES;
                                        istrHasBothEEUV = busConstant.FLAG_YES;
                                        idecEEContriAmt = lbusBenefitCalcDetail.icdoBenefitCalculationDetail.non_vested_ee_amount + lbusBenefitCalcDetail.icdoBenefitCalculationDetail.vested_ee_amount;
                                        idecEEIntAmt = lbusBenefitCalcDetail.icdoBenefitCalculationDetail.non_vested_ee_interest + lbusBenefitCalcDetail.icdoBenefitCalculationDetail.vested_ee_interest;

                                        idecUVHPContriAmt = lbusBenefitCalcDetail.icdoBenefitCalculationDetail.total_uvhp_contribution_amount;
                                        idecUVHPIntAmt = lbusBenefitCalcDetail.icdoBenefitCalculationDetail.total_uvhp_interest_amount;

                                        idecEEUVHPInterest = idecEEIntAmt + idecUVHPIntAmt;
                                        idecEEUVHPTotal = idecEEContriAmt + idecUVHPContriAmt + idecEEUVHPInterest;
                                    }
                                    else if (lbusBenefitCalcDetail.icdoBenefitCalculationDetail.non_vested_ee_amount > decimal.Zero || lbusBenefitCalcDetail.icdoBenefitCalculationDetail.vested_ee_amount > decimal.Zero)
                                    {
                                        istrHasLumpMPI = busConstant.FLAG_YES;
                                        istrHasOnlyEE = busConstant.FLAG_YES;
                                        idecEEContriAmt = lbusBenefitCalcDetail.icdoBenefitCalculationDetail.non_vested_ee_amount + lbusBenefitCalcDetail.icdoBenefitCalculationDetail.vested_ee_amount;
                                        idecEEUVHPInterest = lbusBenefitCalcDetail.icdoBenefitCalculationDetail.non_vested_ee_interest + lbusBenefitCalcDetail.icdoBenefitCalculationDetail.vested_ee_interest;

                                        idecEEUVHPTotal = idecEEContriAmt + idecEEUVHPInterest;
                                    }
                                    else if (lbusBenefitCalcDetail.icdoBenefitCalculationDetail.total_uvhp_contribution_amount > decimal.Zero)
                                    {
                                        istrHasLumpMPI = busConstant.FLAG_YES;
                                        istrHasonlyUVHP = busConstant.FLAG_YES;

                                        idecUVHPContriAmt = lbusBenefitCalcDetail.icdoBenefitCalculationDetail.total_uvhp_contribution_amount;
                                        idecEEUVHPInterest = lbusBenefitCalcDetail.icdoBenefitCalculationDetail.total_uvhp_interest_amount;

                                        idecEEUVHPTotal = idecUVHPContriAmt + idecEEUVHPInterest;
                                    }

                                    if (astrTemplateName == busConstant.SURVIVING_SPOUSE_BENEFIT_CALCULATION_DETAILS
                                        || astrTemplateName == busConstant.Active_Death_Beneficiary_Package_Pension_Only
                                        || astrTemplateName == busConstant.Active_Death_Beneficiary_Package_Pension_Iap)
                                    {
                                        idecUVHPTotal = idecUVHPContriAmt + idecUVHPIntAmt;
                                        idecEEContri =  lbusBenefitCalcDetail.icdoBenefitCalculationDetail.vested_ee_amount;
                                        idecEEInt = lbusBenefitCalcDetail.icdoBenefitCalculationDetail.vested_ee_interest;
                                        idecTotalEE = idecEEContri + idecEEInt;
                                    }
                                }
                            }
                            if (lbusBenefitCalcDetail.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID)
                            {
                                if (astrTemplateName == busConstant.ACTIVE_DEATH_BENEFICIARY_PACKAGE_COVER_LETTER_PENSION_AND_IAP
                                    || astrTemplateName == busConstant.Active_Death_Beneficiary_Package_Pension_Iap || astrTemplateName == busConstant.Active_Death_Beneficiary_Package_LUMPSUM)
                                {
                                    idecIAPBalanceAmt = lbusBenefitCalcDetail.icdoBenefitCalculationDetail.iap_balance_amount;
                                    if (lbusCalcOptions.ibusPlanBenefitXr.icdoPlanBenefitXr.benefit_option_value == busConstant.LUMP_SUM && lbusCalcOptions.icdoBenefitCalculationOptions.local161_special_acct_bal_flag != busConstant.FLAG_YES
                                        && lbusCalcOptions.icdoBenefitCalculationOptions.local52_special_acct_bal_flag != busConstant.FLAG_YES)
                                    {
                                        istrHasLumpIAP = busConstant.FLAG_YES;
                                        istrHasLumpOptionAnyPlan = busConstant.FLAG_YES;

                                    }
                                    if (lbusCalcOptions.ibusPlanBenefitXr.icdoPlanBenefitXr.benefit_option_value == busConstant.LIFE_ANNUTIY && lbusCalcOptions.icdoBenefitCalculationOptions.local161_special_acct_bal_flag != busConstant.FLAG_YES
                                        && lbusCalcOptions.icdoBenefitCalculationOptions.local52_special_acct_bal_flag != busConstant.FLAG_YES)
                                    {
                                        istrHasLifeIAP = busConstant.FLAG_YES;
                                        istrHasLifeOptionAnyPlan = busConstant.FLAG_YES;

                                        idecSurvivorIAPLifeAnnuityBenefitAmt = lbusCalcOptions.icdoBenefitCalculationOptions.survivor_amount;
                                        istrSurvivorIAPLifeAnnuityBenefitAmt = AppendDoller(idecSurvivorIAPLifeAnnuityBenefitAmt);

                                        this.iintIAPAsOfDate = this.iclbBenefitCalculationDetail.Where(
                                                        item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().icdoBenefitCalculationDetail.iap_as_of_date.Year;
                                    }
                                }
                            }

                        }
                    }

                }
            }
        }

        public decimal CalculateMPIPlanLifeAnnuityFactor(int aintSurvivorAge, int aintParticipantAge)
        {
            decimal ldecAnnuityBenefitOptionFactor = new decimal();

            double ldblBenefitOptionFactor = 0.86 + 0.005 * (aintSurvivorAge - aintParticipantAge) +
                      0.006 * (65 - aintParticipantAge);
            ldecAnnuityBenefitOptionFactor = Convert.ToDecimal(ldblBenefitOptionFactor);

            ldecAnnuityBenefitOptionFactor = Math.Min(1, ldecAnnuityBenefitOptionFactor);

            return ldecAnnuityBenefitOptionFactor;
        }

        public decimal CalculateLifeAnnuityBenefit(decimal adecFinalAccruedBenefitAmount, int aintSurvivorAge, int aintParticipantAge)
        {
            decimal ldecAnnuityBenefitOptionFactor = new decimal();
            decimal ldecSurvivorLifeAnnuityAmount = new decimal();

            double ldblBenefitOptionFactor = 0.86 + 0.005 * (aintSurvivorAge - aintParticipantAge) +
                      0.006 * (65 - aintParticipantAge);
            ldecAnnuityBenefitOptionFactor = Convert.ToDecimal(ldblBenefitOptionFactor);
            ldecSurvivorLifeAnnuityAmount = Convert.ToDecimal(Math.Round(adecFinalAccruedBenefitAmount * ldecAnnuityBenefitOptionFactor * 0.50m, 2));

            return ldecSurvivorLifeAnnuityAmount;
        }

        public void CalculateEEInterestAsOfRetirementDate(busPersonAccount abusPersonAccount)
        {
            CalculateTotalEEContributionAndInterest();
            decimal ldecPriorYearInterest = decimal.Zero;
            decimal ldecPriorYearUVHPInterest = decimal.Zero;
            if (icdoBenefitCalculationHeader.calculation_type_value != busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT)
            {
                if (this.icdoBenefitCalculationHeader.benefit_commencement_date == DateTime.MinValue)
                    this.icdoBenefitCalculationHeader.benefit_commencement_date = DateTime.Now.GetLastDayofMonth().AddDays(1);

                if (this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.computational_year == this.icdoBenefitCalculationHeader.benefit_commencement_date.Year && item.icdoPersonAccountRetirementContribution.contribution_type_value == busConstant.CONTRIBUTION_TYPE_EE && item.icdoPersonAccountRetirementContribution.transaction_type_value == busConstant.TRANSACTION_TYPE_INTEREST && item.icdoPersonAccountRetirementContribution.contribution_subtype_value == busConstant.CONTRIBUTION_SUBTYPE_VESTED).Count() == 0)
                {
                    this.idecEEPartialInterest = ibusCalculation.CalculatePartialEEInterest(this.icdoBenefitCalculationHeader.benefit_commencement_date, abusPersonAccount,
                                                 true, false, iclbPersonAccountRetirementContribution,out ldecPriorYearInterest);
                }
                if (this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.computational_year == this.icdoBenefitCalculationHeader.benefit_commencement_date.Year && item.icdoPersonAccountRetirementContribution.contribution_type_value == busConstant.CONTRIBUTION_TYPE_EE && item.icdoPersonAccountRetirementContribution.transaction_type_value == busConstant.TRANSACTION_TYPE_INTEREST && item.icdoPersonAccountRetirementContribution.contribution_subtype_value == busConstant.CONTRIBUTION_SUBTYPE_NON_VESTED).Count() == 0)
                {
                    this.idecNonVestedEEPartialInterest = ibusCalculation.CalculatePartialEEInterest(this.icdoBenefitCalculationHeader.benefit_commencement_date, abusPersonAccount,
                                                 false, true, iclbPersonAccountRetirementContribution, out ldecPriorYearInterest);
                }
                if (this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.computational_year == this.icdoBenefitCalculationHeader.benefit_commencement_date.Year && item.icdoPersonAccountRetirementContribution.contribution_type_value == busConstant.CONTRIBUTION_TYPE_UVHP && item.icdoPersonAccountRetirementContribution.transaction_type_value == busConstant.TRANSACTION_TYPE_INTEREST).Count() == 0)
                {
                    this.idecUVHPPartialInterest = ibusCalculation.CalculatePartialUVHPInterest(this.icdoBenefitCalculationHeader.benefit_commencement_date, abusPersonAccount.icdoPersonAccount.person_account_id,out ldecPriorYearUVHPInterest);
                }
            }

            idecTotalEEUVHP = idecTotalEEUVHP + this.idecEEPartialInterest + idecNonVestedEEPartialInterest + this.idecUVHPPartialInterest;
            if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL)
            {
                if (this.iblnCalcualteUVHPBenefit)
                {
                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoBenefitCalculationDetail.uvhp_flag.IsNotNull()).FirstOrDefault().
                        icdoBenefitCalculationDetail.total_uvhp_contribution_amount = this.idecUVHPContribution;
                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoBenefitCalculationDetail.uvhp_flag.IsNotNull()).FirstOrDefault().
                        icdoBenefitCalculationDetail.total_uvhp_interest_amount = this.idecUVHPInterest + this.idecUVHPPartialInterest;
                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoBenefitCalculationDetail.uvhp_flag.IsNotNull()).FirstOrDefault().
                        icdoBenefitCalculationDetail.non_vested_ee_amount = this.idecNonVestedEEContribution;
                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoBenefitCalculationDetail.uvhp_flag.IsNotNull()).FirstOrDefault().
                        icdoBenefitCalculationDetail.vested_ee_amount = this.idecVestedEEContribution;
                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoBenefitCalculationDetail.uvhp_flag.IsNotNull()).FirstOrDefault().
                        icdoBenefitCalculationDetail.vested_ee_interest = this.idecVestedEEInterest + this.idecEEPartialInterest;
                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoBenefitCalculationDetail.uvhp_flag.IsNotNull()).FirstOrDefault().
                        icdoBenefitCalculationDetail.non_vested_ee_interest = this.idecNonVestedEEInterest + this.idecNonVestedEEPartialInterest;

                }
                if (this.iblnCalculateMPIPPBenefit)
                {
                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoBenefitCalculationDetail.uvhp_flag.IsNullOrEmpty()).FirstOrDefault().
                        icdoBenefitCalculationDetail.vested_ee_amount = this.idecVestedEEContribution;
                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoBenefitCalculationDetail.uvhp_flag.IsNullOrEmpty()).FirstOrDefault().
                       icdoBenefitCalculationDetail.non_vested_ee_amount = this.idecNonVestedEEContribution;
                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoBenefitCalculationDetail.uvhp_flag.IsNullOrEmpty()).FirstOrDefault().
                        icdoBenefitCalculationDetail.vested_ee_interest = this.idecVestedEEInterest + this.idecEEPartialInterest;
                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoBenefitCalculationDetail.uvhp_flag.IsNullOrEmpty()).FirstOrDefault().
                       icdoBenefitCalculationDetail.non_vested_ee_interest = this.idecNonVestedEEInterest + this.idecNonVestedEEPartialInterest;
                }

            }

            else
            {
                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().icdoBenefitCalculationDetail.total_uvhp_contribution_amount = this.idecUVHPContribution;
                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().icdoBenefitCalculationDetail.total_uvhp_interest_amount = this.idecUVHPInterest + this.idecUVHPPartialInterest;

                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().
                    icdoBenefitCalculationDetail.vested_ee_amount = this.idecVestedEEContribution;
                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().
                    icdoBenefitCalculationDetail.non_vested_ee_amount = this.idecNonVestedEEContribution;

                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().
                    icdoBenefitCalculationDetail.vested_ee_interest = this.idecVestedEEInterest + this.idecEEPartialInterest;
                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().
                icdoBenefitCalculationDetail.non_vested_ee_interest = this.idecNonVestedEEInterest + this.idecNonVestedEEPartialInterest;
            }
        }
        //PROD PIR 816
        public decimal DeferredLumpSumFactor(int aintAgeAtBenefitCommencement, int aintAgeAtEarliestRetirement, int aintBenefitCommencementYear)
        {
            //DEFF_FACTOR = ROUND(S1FACTOR + S2FACTOR + S3FACTOR)*12, 3)
            decimal ldecDeffFactor = new decimal();

            decimal ldecS1 = decimal.Zero;
            decimal ldecS2 = decimal.Zero;
            decimal ldecS3 = decimal.Zero;

            int aintDefAge = aintAgeAtEarliestRetirement;
            int aintBenAge = aintAgeAtBenefitCommencement;
            int aintBenAgePlus5 = aintAgeAtBenefitCommencement + 5;
            int aintBenAgePlus20 = aintAgeAtBenefitCommencement + 20;

            //Get 3 Segment Factors for all this ages

            DataTable ldt3Segment = busBase.Select("cdoBenefitCalculationHeader.GetDeferredLumpSumFactors", new object[5] { aintDefAge, aintBenAge, aintBenAgePlus5, aintBenAgePlus20, aintBenefitCommencementYear }); //PROD PIR 816

            if (ldt3Segment.Rows.Count > 0)
            {

                #region S1
                if (aintBenAge <= (aintDefAge - 5))
                {
                    ldecS1 = decimal.Zero;
                }
                else if (aintBenAge <= aintDefAge)
                {
                    decimal ldecN12X1DefAge = decimal.Zero;
                    decimal ldecN12X1BenAgePlus5 = decimal.Zero;
                    decimal ldecDx1BenAge = decimal.Zero;

                    //DataRow ldtRow = ldt3Segment.Rows
                    //N12X1@aintDefAge
                    ldecN12X1DefAge = Convert.ToDecimal(ldt3Segment.AsEnumerable().Where(ldtRow => Convert.ToDecimal(ldtRow["AGE"]) == aintDefAge).FirstOrDefault()["N(12)X1"]);
                    ldecN12X1BenAgePlus5 = Convert.ToDecimal(ldt3Segment.AsEnumerable().Where(ldtRow => Convert.ToDecimal(ldtRow["AGE"].ToString()) == aintBenAgePlus5).FirstOrDefault()["N(12)X1"]);
                    ldecDx1BenAge = Convert.ToDecimal(ldt3Segment.AsEnumerable().Where(ldtRow => Convert.ToDecimal(ldtRow["AGE"].ToString()) == aintBenAge).FirstOrDefault()["DX1"]);

                    ldecS1 = ldecN12X1DefAge - ldecN12X1BenAgePlus5;

                    ldecS1 = ldecS1 / ldecDx1BenAge;

                }
                #endregion

                #region S2
                if (aintBenAge <= (aintDefAge - 20))
                {
                    ldecS2 = 0;
                }
                else if (aintBenAge <= (aintDefAge - 5))
                {
                    decimal ldecN12X2DefAge = decimal.Zero;
                    decimal ldecN12X2BenAgePlus20 = decimal.Zero;
                    decimal ldecDx2BenAge = decimal.Zero;

                    //DataRow ldtRow = ldt3Segment.Rows
                    //N12X1@aintDefAge
                    ldecN12X2DefAge = Convert.ToDecimal(ldt3Segment.AsEnumerable().Where(ldtRow => Convert.ToDecimal(ldtRow["AGE"].ToString()) == aintDefAge).FirstOrDefault()["N(12)X2"]);
                    ldecN12X2BenAgePlus20 = Convert.ToDecimal(ldt3Segment.AsEnumerable().Where(ldtRow => Convert.ToDecimal(ldtRow["AGE"].ToString()) == aintBenAgePlus20).FirstOrDefault()["N(12)X2"]);
                    ldecDx2BenAge = Convert.ToDecimal(ldt3Segment.AsEnumerable().Where(ldtRow => Convert.ToDecimal(ldtRow["AGE"].ToString()) == aintBenAge).FirstOrDefault()["DX2"]);

                    ldecS2 = ldecN12X2DefAge - ldecN12X2BenAgePlus20;

                    ldecS2 = ldecS2 / ldecDx2BenAge;

                }
                else
                {
                    decimal ldecN12X2BenAgePlus5 = decimal.Zero;
                    decimal ldecN12X2BenAgePlus20 = decimal.Zero;
                    decimal ldecDx2BenAge = decimal.Zero;

                    //DataRow ldtRow = ldt3Segment.Rows
                    //N12X1@aintDefAge
                    ldecN12X2BenAgePlus5 = Convert.ToDecimal(ldt3Segment.AsEnumerable().Where(ldtRow => Convert.ToDecimal(ldtRow["AGE"].ToString()) == aintBenAgePlus5).FirstOrDefault()["N(12)X2"]);
                    ldecN12X2BenAgePlus20 = Convert.ToDecimal(ldt3Segment.AsEnumerable().Where(ldtRow => Convert.ToDecimal(ldtRow["AGE"].ToString()) == aintBenAgePlus20).FirstOrDefault()["N(12)X2"]);
                    ldecDx2BenAge = Convert.ToDecimal(ldt3Segment.AsEnumerable().Where(ldtRow => Convert.ToDecimal(ldtRow["AGE"].ToString()) == aintBenAge).FirstOrDefault()["DX2"]);

                    ldecS2 = ldecN12X2BenAgePlus5 - ldecN12X2BenAgePlus20;

                    ldecS2 = ldecS2 / ldecDx2BenAge;
                }

                #endregion

                #region S3

                if (aintBenAge <= (aintDefAge - 20))
                {
                    decimal ldecN12X3DefAge = decimal.Zero;
                    decimal ldecDx3BenAge = decimal.Zero;

                    ldecN12X3DefAge = Convert.ToDecimal(ldt3Segment.AsEnumerable().Where(ldtRow => Convert.ToDecimal(ldtRow["AGE"].ToString()) == aintDefAge).FirstOrDefault()["N(12)X3"]);
                    ldecDx3BenAge = Convert.ToDecimal(ldt3Segment.AsEnumerable().Where(ldtRow => Convert.ToDecimal(ldtRow["AGE"].ToString()) == aintBenAge).FirstOrDefault()["DX3"]);

                    ldecS3 = ldecN12X3DefAge / ldecDx3BenAge;
                }
                else
                {
                    decimal ldecN12X3BenAgePlus20 = decimal.Zero;
                    decimal ldecDx3BenAge = decimal.Zero;

                    ldecN12X3BenAgePlus20 = Convert.ToDecimal(ldt3Segment.AsEnumerable().Where(ldtRow => Convert.ToDecimal(ldtRow["AGE"].ToString()) == aintBenAgePlus20).FirstOrDefault()["N(12)X3"]);
                    ldecDx3BenAge = Convert.ToDecimal(ldt3Segment.AsEnumerable().Where(ldtRow => Convert.ToDecimal(ldtRow["AGE"].ToString()) == aintBenAge).FirstOrDefault()["DX3"]);

                    ldecS3 = ldecN12X3BenAgePlus20 / ldecDx3BenAge;
                }

                #endregion

                #region DeffFactor
                ldecDeffFactor = Math.Round((ldecS1 + ldecS2 + ldecS3) * 12, 3);
                #endregion
            }
            return ldecDeffFactor;
        }

        public busBenefitCalculationOptions CalculateMpiPlanLifeAnnuityOptions(decimal adecAccruedBenefitAmount)
        {
            busBenefitCalculationOptions lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

            decimal ldecAnnuityBenefitOptionFactor = decimal.Zero;
            if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE)
            {
                ldecAnnuityBenefitOptionFactor = CalculateMPIPlanLifeAnnuityFactor(iintSpouseEarliestRetrAge, iintParticipantEarliestRetrAge);

            }
            else
            {
                ldecAnnuityBenefitOptionFactor = CalculateMPIPlanLifeAnnuityFactor(Convert.ToInt32(this.icdoBenefitCalculationHeader.idecSurvivorFullAge), Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge));
            }
            decimal ldecSurvivorLifeAnnuityAmount = Convert.ToDecimal(Math.Round(adecAccruedBenefitAmount * ldecAnnuityBenefitOptionFactor * 0.50m, 2));
            lbusBenefitCalculationOptions.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.MPIPP_PLAN_ID, busConstant.LIFE_ANNUTIY), ldecAnnuityBenefitOptionFactor, ldecSurvivorLifeAnnuityAmount * 2, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_BENEFICIARY, this.icdoBenefitCalculationHeader.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.LIFE_ANNUTIY, ldecSurvivorLifeAnnuityAmount);
            lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.survivor_percent_amount = adecAccruedBenefitAmount;
            if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL)
            {
                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId && item.icdoBenefitCalculationDetail.uvhp_flag.IsNullOrEmpty()).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
                if (this.icdoBenefitCalculationHeader.iintPlanId == busConstant.MPIPP_PLAN_ID)
                {
                    GetMinimumGuarantee(adecAccruedBenefitAmount);
                }
            }
            else
            {
                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
            }
            return lbusBenefitCalculationOptions;

        }

        //Min Guarantee , PV getting set
        public busBenefitCalculationOptions CalculateMpiPlanLumpSumOptions(busPersonAccount abusPersonAccount, decimal adecLumpSumBenefitOptionFactor, decimal adecEETotal, decimal adecAccruedBenefitAmount, bool ablnFillDetailCollection = true)
        {
            //To Set Minimum Guarantee.
            decimal ldecMinimumG = decimal.Zero;
            decimal ldecLumpSumAnnuity = decimal.Zero;
            //decimal ldecLocalLumpSum = ibusCalculation.GetTotalLocalBenefitAmountForDeath(this.iintParticipantEarliestRetrAge, this.ibusBenefitApplication, this.ibusPerson, this.icdoBenefitCalculationHeader.retirement_date, this.iclbPersonAccountRetirementContribution);

            busBenefitCalculationOptions lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
            ldecLumpSumAnnuity = CalculateMPIPlanLifeAnnuityFactor(iintSpouseEarliestRetrAge, iintParticipantEarliestRetrAge);



            decimal ldecimalSurvivorLumpSum = Convert.ToDecimal(ldecLumpSumAnnuity * adecAccruedBenefitAmount) / 2;

            ldecimalSurvivorLumpSum = Math.Round(ldecimalSurvivorLumpSum, 2);

            ldecimalSurvivorLumpSum = Math.Round(adecLumpSumBenefitOptionFactor * ldecimalSurvivorLumpSum, 2);

            if (ldecimalSurvivorLumpSum > adecEETotal)
            {
                //Lump Sum
                if (ablnFillDetailCollection)
                {
                    lbusBenefitCalculationOptions.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.MPIPP_PLAN_ID, busConstant.LUMP_SUM), adecLumpSumBenefitOptionFactor, ldecimalSurvivorLumpSum * 2, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_BENEFICIARY, this.icdoBenefitCalculationHeader.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.LUMP_SUM, ldecimalSurvivorLumpSum);
                    lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.survivor_percent_amount = adecAccruedBenefitAmount;
                }
                ldecMinimumG = ldecimalSurvivorLumpSum;
            }
            else
            {
                if (ablnFillDetailCollection)
                {
                    lbusBenefitCalculationOptions.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.MPIPP_PLAN_ID, busConstant.LUMP_SUM), 1, adecEETotal, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_BENEFICIARY, this.icdoBenefitCalculationHeader.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.LUMP_SUM, adecEETotal);
                    lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.survivor_percent_amount = adecEETotal;
                }
                ldecMinimumG = adecEETotal;
            }
            lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.survivor_percent_amount = adecAccruedBenefitAmount;
            if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL)
            {
                if (ablnFillDetailCollection)
                {
                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId && item.icdoBenefitCalculationDetail.uvhp_flag.IsNullOrEmpty()).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
                }
                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId && item.icdoBenefitCalculationDetail.uvhp_flag.IsNullOrEmpty()).FirstOrDefault().icdoBenefitCalculationDetail.minimum_guarantee_amount = ldecMinimumG;
                if (this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId && item.icdoBenefitCalculationDetail.uvhp_flag.IsNullOrEmpty()).Count() > 0)
                {
                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId && item.icdoBenefitCalculationDetail.uvhp_flag.IsNullOrEmpty()).FirstOrDefault().icdoBenefitCalculationDetail.present_value_amount = ldecimalSurvivorLumpSum;
                }
            }
            else
            {
                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().icdoBenefitCalculationDetail.minimum_guarantee_amount = ldecMinimumG;
                if (this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).Count() > 0)
                {
                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().icdoBenefitCalculationDetail.present_value_amount = ldecimalSurvivorLumpSum;
                }

            }

            return lbusBenefitCalculationOptions;
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
            int intBenefitApplication_detail_id;
            int lintMPIPPHeaderId = 0;
            int lintIAPHeaderId = 0;
            this.iblnCalcualteUVHPBenefit = this.iblnCalculateIAPBenefit = this.iblnCalculateL161SplAccBenefit = this.iblnCalculateL52SplAccBenefit = this.iblnCalculateMPIPPBenefit = false;
            #endregion

            this.icdoBenefitCalculationHeader.iintPlanId = this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.plan_id;
            //Ticket#122258
            intBenefitApplication_detail_id = this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.benefit_application_detail_id;

            if (!lblIsNew )
            {
                FlushOlderCalculations();
            }

           
            if (this.ibusBenefitApplication.FindBenefitApplication(this.icdoBenefitCalculationHeader.benefit_application_id))
            {
                this.ibusBenefitApplication.iclbBenefitApplicationDetail = new Collection<busBenefitApplicationDetail>();
                this.iclbPersonAccountRetirementContribution = new Collection<busPersonAccountRetirementContribution>();

                this.ibusBenefitApplication.LoadBenefitApplicationDetails();


                this.ibusBenefitApplication.CheckIfQualifiedSpouseinDeath();
                this.ibusBenefitApplication.DetermineVesting();

                this.CheckQualifiedSpouseExists();
              
                if (ibusPerson.iclbPersonAccount != null && !ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).IsNullOrEmpty())
                {
                    LoadAllRetirementContributions(ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault());
                }
                else
                {
                    LoadAllRetirementContributions(null);
                }

                DateTime ldtBenefitComDate = this.ibusBenefitApplication.icdoBenefitApplication.retirement_date;
                this.LoadPreRetirementDeathInitialData();
                this.ibusBenefitApplication.icdoBenefitApplication.retirement_date = ldtBenefitComDate;
                this.icdoBenefitCalculationHeader.benefit_commencement_date = ldtBenefitComDate;
                this.SetUpVariablesForDeath(this.ibusBenefitApplication.icdoBenefitApplication.retirement_date);

                //this.LoadPreRetirementDeathInitialData();
                //this.icdoBenefitCalculationHeader.benefit_commencement_date = this.ibusBenefitApplication.icdoBenefitApplication.retirement_date;
                //SetUpVariablesForDeath(this.ibusBenefitApplication.icdoBenefitApplication.retirement_date);
                //this.SetupPreRequisites_DeathCalculations();
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
                            //Ticket#122258
                            if (intBenefitApplication_detail_id != lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.benefit_application_detail_id)
                                continue;


                            this.iblnCalculateIAPBenefit = this.iblnCalculateL161SplAccBenefit = this.iblnCalculateL52SplAccBenefit = this.iblnCalculateMPIPPBenefit = false;
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

                            if(this.icdoBenefitCalculationHeader.lump_sum_payment == "Y")
                            {
                                lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue = "LUMP";

                            }
                            
                                this.SpawnFinalRetirementCalculation(lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.benefit_application_detail_id,
                                                                                             this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == lbusBenefitApplicationDetail.istrPlanCode).FirstOrDefault().icdoPersonAccount.person_account_id,
                                                                                             lbusBenefitApplicationDetail.iintPlan_ID, lbusBenefitApplicationDetail.istrPlanCode, this.ibusBenefitApplication.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date,
                                                                                             this.icdoBenefitCalculationHeader.istrRetirementType, lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue);
                            
                            #endregion
                        }

                        else if (lbusBenefitApplicationDetail.iintPlan_ID == busConstant.MPIPP_PLAN_ID && this.icdoBenefitCalculationHeader.iintPlanId == busConstant.MPIPP_PLAN_ID)
                        {
                            #region MPIPP PLAN FOUNd in GRID
                            this.iblnCalcualteUVHPBenefit = this.iblnCalculateMPIPPBenefit = false;
                            if (lbusBenefitApplicationDetail.istrSubPlan.IsNullOrEmpty() && !this.iblnCalculateMPIPPBenefit)
                            {
                                this.iblnCalculateMPIPPBenefit = true;

                            }

                            else if (this.ibusBenefitApplication.iclbBenefitApplicationDetail.Where(item => item.istrSubPlan == busConstant.EE_UVHP && item.iintPlan_ID == busConstant.MPIPP_PLAN_ID).Count() > 0 && !lblnUVHPFlag)
                            {
                                this.iblnCalcualteUVHPBenefit = true;
                                lblnUVHPFlag = true;
                            }

                            //if (this.ibusBenefitApplication.CheckAlreadyVested(lbusBenefitApplicationDetail.istrPlanCode))
                            //{

                            if (lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue.IsNullOrEmpty() && lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrSubPlanBenefitOptionValue.IsNotNullOrEmpty())
                            {
                                lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue = lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrSubPlanBenefitOptionValue;
                            }

                            this.SpawnFinalRetirementCalculation(lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.benefit_application_detail_id,
                                                                                             this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == lbusBenefitApplicationDetail.istrPlanCode).FirstOrDefault().icdoPersonAccount.person_account_id,
                                                                                             lbusBenefitApplicationDetail.iintPlan_ID, lbusBenefitApplicationDetail.istrPlanCode, this.ibusBenefitApplication.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date,
                                                                                             this.icdoBenefitCalculationHeader.istrRetirementType, lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue);

                            //}

                            #endregion
                        }

                        else if (lbusBenefitApplicationDetail.iintPlan_ID != busConstant.MPIPP_PLAN_ID && lbusBenefitApplicationDetail.iintPlan_ID != busConstant.IAP_PLAN_ID)
                        {
                            #region LOCAL PLAN FOUND
                            if (this.ibusBenefitApplication.CheckAlreadyVested(lbusBenefitApplicationDetail.istrPlanCode))
                            {

                                if (lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue.IsNullOrEmpty() && lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrSubPlanBenefitOptionValue.IsNotNullOrEmpty())
                                {
                                    lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue = lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrSubPlanBenefitOptionValue;
                                }

                                this.SpawnFinalRetirementCalculation(lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.benefit_application_detail_id,
                                                                                                 this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == lbusBenefitApplicationDetail.istrPlanCode).FirstOrDefault().icdoPersonAccount.person_account_id,
                                                                                                 lbusBenefitApplicationDetail.iintPlan_ID, lbusBenefitApplicationDetail.istrPlanCode, this.ibusBenefitApplication.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date,
                                                                                                 this.icdoBenefitCalculationHeader.istrRetirementType, lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue);

                            }

                            #endregion
                        }
                    }
                    try
                    {
                        this.AfterPersistChanges();
                    }
                    catch
                    {
                    }

                }


                this.EvaluateInitialLoadRules();
                this.iclbBenefitCalculationDetail.First().EvaluateInitialLoadRules();
            }

            larrList.Add(this);
            return larrList;
        }

        public void GetSurvivorPercentAmount(ref decimal adecAmount, int aintPlanID)
        {
            //if (aintPlanID == 1)
            //{
            //    adecAmount = adecAmount * this.icdoBenefitCalculationHeader.survivor_percentage_iap;
            //}
            //else 
            if (this.icdoBenefitCalculationHeader.survivor_percentage > 0)
            {
                adecAmount = adecAmount * (this.icdoBenefitCalculationHeader.survivor_percentage / 100);
            }
        }

        public void GetMinimumGuarantee(decimal adecAccruedBenefit)
        {
            decimal ldecLateAdjustmentAmt = decimal.Zero;
            busPersonAccount lbusPersonAccount = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault();
            decimal ldecAccruedBenefitAmount = adecAccruedBenefit;
            if (ldecAccruedBenefitAmount == decimal.Zero)
            {
                ldecAccruedBenefitAmount = ibusCalculation.CalculateBenefitAmtForPension(ibusPerson, busConstant.BENEFIT_TYPE_RETIREMENT, iintParticipantEarliestRetrAge, this.ibusBenefitApplication.icdoBenefitApplication.retirement_date,
                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().icdoBenefitCalculationDetail.vested_date,
                lbusPersonAccount,
                ibusBenefitApplication, busConstant.BOOL_FALSE,
                this.iclbBenefitCalculationDetail, this.iclbPersonAccountRetirementContribution, null, true, this.icdoBenefitCalculationHeader.istrEarliestRetirementType, ref ldecLateAdjustmentAmt, this.ibusPerson.icdoPerson.person_id);
                if (this.icdoBenefitCalculationHeader.istrEarliestRetirementType == busConstant.RETIREMENT_TYPE_LATE)
                    ldecAccruedBenefitAmount = ldecLateAdjustmentAmt;

                #region Check if Withdrawal History Exists: Then Acrrued benefit = Accrued Benefit - EE derived

                //ldecAccruedBenefitAmount = ibusCalculation.CalculateAccruedBenefitForPersonWithWithdrawal(
                //                                            this.ibusBenefitApplication.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date,
                //                                            ldecAccruedBenefitAmount, this.ibusBenefitApplication.ibusPerson, this.ibusBenefitApplication.ibusPerson.iclbPersonAccount,
                //                                            icdoBenefitCalculationHeader.retirement_date, this.ibusBenefitApplication.aclbPersonWorkHistory_MPI,
                //                                            this.ibusBenefitApplication.ibusPerson.iclbPersonAccountRetirementContribution,
                //                                            this.ibusBenefitApplication.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.forfeiture_date.Year);

                if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE)
                {
                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).
                                                                                  FirstOrDefault().icdoBenefitCalculationDetail.early_reduced_benefit_amount = ldecAccruedBenefitAmount;
                }
                else
                {
                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoBenefitCalculationDetail.uvhp_flag.IsNullOrEmpty()).
                                                              FirstOrDefault().icdoBenefitCalculationDetail.early_reduced_benefit_amount = ldecAccruedBenefitAmount;

                }
                #endregion
            }

            decimal ldecLumpSumBenefitOptionFactor = decimal.Zero;
            if (this.icdoBenefitCalculationHeader.benefit_commencement_date >= this.icdoBenefitCalculationHeader.retirement_date || this.icdoBenefitCalculationHeader.benefit_commencement_date == DateTime.MinValue)
            {
                ldecLumpSumBenefitOptionFactor = ibusCalculation.GetLumpsumBenefitFactor(Convert.ToInt32(this.icdoBenefitCalculationHeader.idecSurvivorFullAge), this.icdoBenefitCalculationHeader.benefit_commencement_date.Year) * 12;
                ldecLumpSumBenefitOptionFactor = Math.Round(ldecLumpSumBenefitOptionFactor, 3);
            }
            else
            {
                ldecLumpSumBenefitOptionFactor = DeferredLumpSumFactor(Convert.ToInt32(this.icdoBenefitCalculationHeader.idecSurvivorFullAge), iintSpouseEarliestRetrAge, this.icdoBenefitCalculationHeader.benefit_commencement_date.Year); //PROD PIR 816
            }
            CalculateMpiPlanLumpSumOptions(lbusPersonAccount, ldecLumpSumBenefitOptionFactor, idecVestedEEContribution +
                idecNonVestedEEInterest + idecVestedEEInterest + idecEEPartialInterest + idecNonVestedEEPartialInterest, ldecAccruedBenefitAmount, false);

        }

        #region Death_Pre_Retirement_Post_Election

        public void GetMPIBenefitOptionsForDeathPostElection(string astrBenefitOption, decimal adecFinalAccruedBenefit, decimal adecLumpSumBenefitOptionFactor, decimal adecEETotal)
        {
            string lstrBenefitOptionValue = string.Empty;
            int lintPlanBenefitID = 0;
            busPersonAccount lbusPersonAccount = ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault();
            busPayeeAccount lbusPayeeAccount = null;
            lbusPayeeAccount = (from lbusPayee in this.ibusBenefitApplication.iclbPayeeAccount
                                where lbusPayee.icdoPayeeAccount.iintPlanId ==
                                 this.icdoBenefitCalculationHeader.iintPlanId
                                select lbusPayee).FirstOrDefault();
            lintPlanBenefitID = lbusPayeeAccount.icdoPayeeAccount.plan_benefit_id;
            if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE)
            {
                lbusPayeeAccount.LoadBenefitDetails();
                if (lbusPayeeAccount.icdoPayeeAccount.idtRetireMentDate != DateTime.MinValue)
                {
                    astrBenefitOption = lbusPayeeAccount.icdoPayeeAccount.istrBenefitOptionValue;
                    if (this.ibusPerson.icdoPerson.date_of_death.AddDays(90) >= lbusPayeeAccount.icdoPayeeAccount.idtRetireMentDate)
                    {
                        if (astrBenefitOption == busConstant.JOINT_50_PERCENT_POPUP_ANNUITY || astrBenefitOption == busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY)
                        {
                            return;
                        }
                        else if (astrBenefitOption == busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY)
                        {
                            if (this.ibusPerson.icdoPerson.date_of_death.AddDays(30) < lbusPayeeAccount.icdoPayeeAccount.idtRetireMentDate)
                            {
                                return;
                            }
                        }
                    }
                    else
                    {
                        return;
                    }
                    if (this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().iclbBenefitCalculationOptions.Where(item => item.icdoBenefitCalculationOptions.plan_benefit_id == lintPlanBenefitID).Count() > 0)
                    {
                        return;
                    }
                }
            }

            lbusPayeeAccount.LoadPayeeAccountPaymentItemType();
            decimal ldecTaxableAnount = decimal.Zero;
            decimal ldecNonTaxableAmount = decimal.Zero;
            decimal ldecGrossParticipantAmount = decimal.Zero;
            foreach (busPayeeAccountPaymentItemType lbusPayeeAccountPaymentItemType in lbusPayeeAccount.iclbPayeeAccountPaymentItemType)
            {
                if (lbusPayeeAccountPaymentItemType.ibusPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.ITEM1)
                {
                    ldecTaxableAnount += lbusPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.amount;
                }
                else if (lbusPayeeAccountPaymentItemType.ibusPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.ITEM2)
                {
                    ldecNonTaxableAmount += lbusPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.amount;
                }
            }

            //ibusCalculation.GetTaxableAndNonTaxableAmountFromPaymentItemTypes(lbusPayeeAccount, out ldecTaxableAnount, out ldecNonTaxableAmount);
            busBenefitCalculationOptions lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
            ldecGrossParticipantAmount = ldecTaxableAnount + ldecNonTaxableAmount;
            GetSurvivorPercentAmount(ref ldecGrossParticipantAmount, this.icdoBenefitCalculationHeader.iintPlanId);

            if (this.icdoBenefitCalculationHeader.iintPlanId == busConstant.MPIPP_PLAN_ID || this.icdoBenefitCalculationHeader.iintPlanId == busConstant.LOCAL_161_PLAN_ID ||
                this.icdoBenefitCalculationHeader.iintPlanId == busConstant.LOCAL_666_PLAN_ID || this.icdoBenefitCalculationHeader.iintPlanId == busConstant.LOCAL_700_PLAN_ID
                || this.icdoBenefitCalculationHeader.iintPlanId == busConstant.LOCAL_600_PLAN_ID || this.icdoBenefitCalculationHeader.iintPlanId == busConstant.LOCAL_52_PLAN_ID)
            {
                if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY)
                {
                    decimal ldecFinalBenefitAmount = Math.Round(ldecGrossParticipantAmount * .75M, 2);
                    lbusBenefitCalculationOptions.LoadData(lintPlanBenefitID, decimal.Zero, ldecGrossParticipantAmount,
                                                            busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_BENEFICIARY, this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                            busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, ldecFinalBenefitAmount);
                    lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.survivor_percent_amount = ldecGrossParticipantAmount;
                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);

                }
            }
            if (this.icdoBenefitCalculationHeader.iintPlanId == busConstant.MPIPP_PLAN_ID || this.icdoBenefitCalculationHeader.iintPlanId == busConstant.LOCAL_666_PLAN_ID ||
                this.icdoBenefitCalculationHeader.iintPlanId == busConstant.LOCAL_52_PLAN_ID)
            {

                if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY)
                {
                    decimal ldecFinalBenefitAmount = Math.Round(ldecGrossParticipantAmount, 2);
                    lbusBenefitCalculationOptions.LoadData(lintPlanBenefitID, decimal.Zero, ldecGrossParticipantAmount,
                                                            busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_BENEFICIARY, this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                            busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY, ldecFinalBenefitAmount);
                    lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.survivor_percent_amount = ldecGrossParticipantAmount;
                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);

                }
            }
            if (this.icdoBenefitCalculationHeader.iintPlanId == busConstant.MPIPP_PLAN_ID || this.icdoBenefitCalculationHeader.iintPlanId == busConstant.LOCAL_600_PLAN_ID ||
     this.icdoBenefitCalculationHeader.iintPlanId == busConstant.LOCAL_666_PLAN_ID || this.icdoBenefitCalculationHeader.iintPlanId == busConstant.LOCAL_700_PLAN_ID ||
                this.icdoBenefitCalculationHeader.iintPlanId == busConstant.LOCAL_52_PLAN_ID)
            {

                if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.JOINT_100_PERCENT_POPUP_ANNUITY)
                {
                    decimal ldecFinalBenefitAmount = Math.Round(ldecGrossParticipantAmount, 2);
                    lbusBenefitCalculationOptions.LoadData(lintPlanBenefitID, decimal.Zero, ldecGrossParticipantAmount,
                                                            busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_BENEFICIARY, this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                            busConstant.JOINT_100_PERCENT_POPUP_ANNUITY, ldecFinalBenefitAmount);
                    lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.survivor_percent_amount = ldecGrossParticipantAmount;
                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);

                }
            }
            if (this.icdoBenefitCalculationHeader.iintPlanId == busConstant.MPIPP_PLAN_ID || this.icdoBenefitCalculationHeader.iintPlanId == busConstant.LOCAL_600_PLAN_ID ||
                 this.icdoBenefitCalculationHeader.iintPlanId == busConstant.LOCAL_666_PLAN_ID || this.icdoBenefitCalculationHeader.iintPlanId == busConstant.LOCAL_700_PLAN_ID ||
                this.icdoBenefitCalculationHeader.iintPlanId == busConstant.LOCAL_52_PLAN_ID)
            {

                if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY)
                {
                    decimal ldecFinalBenefitAmount = Math.Round(ldecGrossParticipantAmount, 2);
                    lbusBenefitCalculationOptions.LoadData(lintPlanBenefitID, decimal.Zero, ldecGrossParticipantAmount,
                                                            busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_BENEFICIARY, this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                            busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, ldecFinalBenefitAmount);
                    lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.survivor_percent_amount = ldecGrossParticipantAmount;
                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);

                }
            }
            if (this.icdoBenefitCalculationHeader.iintPlanId == busConstant.MPIPP_PLAN_ID && this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL)
            {
                GetMinimumGuarantee(decimal.Zero);
                GetSurvivorPercentAmount(ref ldecNonTaxableAmount, this.icdoBenefitCalculationHeader.iintPlanId);
                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().icdoBenefitCalculationDetail.monthly_exclusion_amount = ldecNonTaxableAmount;
            }


            //Kunal : As per Debra's mail made these changes
            //I confirmed these facts for Pre-retirement Post Election
            //If participant dies prior to receiving his/her  first payment. The additional option available to the survivor would be the option elected by the participant.
            //In the example for Dennis Haley the survivor is entitled to the J&S 100% Popup Survivor benefit amount. Nothing about this amount will change regardless of commencement date.
            //The other options Life Annuity and Lump Sum follow the Pre-retirement Death calculations
            #region Commented
            /*
            if (this.icdoBenefitCalculationHeader.iintPlanId == busConstant.MPIPP_PLAN_ID || this.icdoBenefitCalculationHeader.iintPlanId == busConstant.LOCAL_161_PLAN_ID ||
                this.icdoBenefitCalculationHeader.iintPlanId == busConstant.LOCAL_666_PLAN_ID || this.icdoBenefitCalculationHeader.iintPlanId == busConstant.LOCAL_700_PLAN_ID
                || this.icdoBenefitCalculationHeader.iintPlanId == busConstant.LOCAL_600_PLAN_ID || this.icdoBenefitCalculationHeader.iintPlanId == busConstant.LOCAL_52_PLAN_ID)
            {
                if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY)
                {
                    CalculateAnnuityForMPIJAndS75(adecFinalAccruedBenefit);
                }
            }
            if (this.icdoBenefitCalculationHeader.iintPlanId == busConstant.MPIPP_PLAN_ID || this.icdoBenefitCalculationHeader.iintPlanId == busConstant.LOCAL_666_PLAN_ID ||
                this.icdoBenefitCalculationHeader.iintPlanId == busConstant.LOCAL_52_PLAN_ID)
            {

                if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY)
                {
                    CalculateAnnuityForMPIJAndS100(adecFinalAccruedBenefit);
                }
            }
            if (this.icdoBenefitCalculationHeader.iintPlanId == busConstant.MPIPP_PLAN_ID || this.icdoBenefitCalculationHeader.iintPlanId == busConstant.LOCAL_600_PLAN_ID ||
     this.icdoBenefitCalculationHeader.iintPlanId == busConstant.LOCAL_666_PLAN_ID || this.icdoBenefitCalculationHeader.iintPlanId == busConstant.LOCAL_700_PLAN_ID ||
                this.icdoBenefitCalculationHeader.iintPlanId == busConstant.LOCAL_52_PLAN_ID)
            {

                if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.JOINT_100_PERCENT_POPUP_ANNUITY)
                {
                    CalculateAnnuityForMPIJPOP100(adecFinalAccruedBenefit);
                }
            }
            if (this.icdoBenefitCalculationHeader.iintPlanId == busConstant.MPIPP_PLAN_ID || this.icdoBenefitCalculationHeader.iintPlanId == busConstant.LOCAL_600_PLAN_ID ||
                 this.icdoBenefitCalculationHeader.iintPlanId == busConstant.LOCAL_666_PLAN_ID || this.icdoBenefitCalculationHeader.iintPlanId == busConstant.LOCAL_700_PLAN_ID ||
                this.icdoBenefitCalculationHeader.iintPlanId == busConstant.LOCAL_52_PLAN_ID)
            {

                if (astrBenefitOption == busConstant.CodeValueAll || astrBenefitOption == busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY)
                {
                    CalculateAnnuityForMPITenYear(adecFinalAccruedBenefit);
                }
            }
            */
            #endregion
        }

        /*
        public void CalculateAnnuityForMPIJPOP100(decimal adecFinalAccruedBenefitAmount)
        {
            busBenefitCalculationOptions lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

            decimal ldecBenefitOptionFactor = 1;
            double ldblBenefitOptionFactorJnS100Pop = 0.71 + 0.01 * (Convert.ToInt32(Math.Floor(this.icdoBenefitCalculationHeader.idecSurvivorFullAge)) - Convert.ToInt32(Math.Floor(this.icdoBenefitCalculationHeader.idecParticipantFullAge))) + 0.008 * (65 - Convert.ToInt32(Math.Floor(this.icdoBenefitCalculationHeader.idecParticipantFullAge)));
            ldecBenefitOptionFactor = Convert.ToDecimal(Math.Round(ldblBenefitOptionFactorJnS100Pop, 3));
            decimal ldecFinalBenefitAmount = Convert.ToDecimal(Math.Round(adecFinalAccruedBenefitAmount * ldecBenefitOptionFactor, 2));
            lbusBenefitCalculationOptions.LoadData(ibusCalculation.GetPlanBenefitId(this.icdoBenefitCalculationHeader.iintPlanId, busConstant.JOINT_100_PERCENT_POPUP_ANNUITY), ldecBenefitOptionFactor, ldecFinalBenefitAmount,
                                                    busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_BENEFICIARY, this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                    busConstant.JOINT_100_PERCENT_POPUP_ANNUITY, Convert.ToDecimal(Math.Round(adecFinalAccruedBenefitAmount * ldecBenefitOptionFactor, 2)));
            lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.survivor_percent_amount = adecFinalAccruedBenefitAmount;

            if (this.icdoBenefitCalculationHeader.iintPlanId == busConstant.MPIPP_PLAN_ID && this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL)
            {
                GetMinimumGuarantee(adecFinalAccruedBenefitAmount);
            }
            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
        }

        public void CalculateAnnuityForMPIJAndS75(decimal adecFinalAccruedBenefitAmount)
        {
            busBenefitCalculationOptions lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
            decimal ldecBenefitOptionFactor = 1;
            double ldblBenefitOptionFactorJnS75 = 0.80 + 0.01 * (Convert.ToInt32(Math.Floor(this.icdoBenefitCalculationHeader.idecSurvivorFullAge)) - Convert.ToInt32(Math.Floor(this.icdoBenefitCalculationHeader.idecParticipantFullAge))) + 0.006 * (65 - Convert.ToInt32(Math.Floor(this.icdoBenefitCalculationHeader.idecParticipantFullAge)));
            ldecBenefitOptionFactor = Convert.ToDecimal(Math.Round(ldblBenefitOptionFactorJnS75, 3));

            decimal ldecFinalBenefitAmount = Convert.ToDecimal(Math.Round(adecFinalAccruedBenefitAmount * ldecBenefitOptionFactor, 2));
            ldecFinalBenefitAmount = Math.Round(ldecFinalBenefitAmount * .75M, 2);
            lbusBenefitCalculationOptions.LoadData(ibusCalculation.GetPlanBenefitId(this.icdoBenefitCalculationHeader.iintPlanId, busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY), ldecBenefitOptionFactor, ldecFinalBenefitAmount,
                                                    busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_BENEFICIARY, this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                    busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, ldecFinalBenefitAmount);
            lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.survivor_percent_amount = adecFinalAccruedBenefitAmount;
            if (this.icdoBenefitCalculationHeader.iintPlanId == busConstant.MPIPP_PLAN_ID && this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL)
            {
                GetMinimumGuarantee(adecFinalAccruedBenefitAmount);
            }
            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
        }

        public void CalculateAnnuityForMPITenYear(decimal adecFinalAccruedBenefitAmount)
        {
            busBenefitCalculationOptions lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

            int lPlanBenefitId = ibusCalculation.GetPlanBenefitId(this.icdoBenefitCalculationHeader.iintPlanId, busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY);
            decimal ldecBenefitOptionFactor = ibusCalculation.GetBenefitOptionFactor(busConstant.BENEFIT_TYPE_RETIREMENT, lPlanBenefitId, Convert.ToInt32(Math.Floor(this.icdoBenefitCalculationHeader.idecParticipantFullAge)), busConstant.ZERO_INT);

            decimal ldecFinalBenefitAmount = Convert.ToDecimal(Math.Round(adecFinalAccruedBenefitAmount * ldecBenefitOptionFactor, 2));
            lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
            lbusBenefitCalculationOptions.LoadData(lPlanBenefitId, ldecBenefitOptionFactor, ldecFinalBenefitAmount,
                                                    busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_BENEFICIARY, this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                    busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, Convert.ToDecimal(Math.Round(adecFinalAccruedBenefitAmount * ldecBenefitOptionFactor, 2)));
            lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.survivor_percent_amount = adecFinalAccruedBenefitAmount;
            if (this.icdoBenefitCalculationHeader.iintPlanId == busConstant.MPIPP_PLAN_ID && this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL)
            {
                GetMinimumGuarantee(adecFinalAccruedBenefitAmount);
            }
            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
        }

        public void CalculateAnnuityForMPIJAndS100(decimal adecFinalAccruedBenefitAmount)
        {
            busBenefitCalculationOptions lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
            decimal ldecLifeAnnuityFactor = new decimal();
            decimal ldecSurvivorLifeAnnuity = new decimal();

            if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE)
            {
                ldecLifeAnnuityFactor = this.CalculateLifeAnnuityBasedonMPIJandS100(this.iintSpouseEarliestRetrAge, this.iintParticipantEarliestRetrAge);
            }
            else
            {
                ldecLifeAnnuityFactor = this.CalculateLifeAnnuityBasedonMPIJandS100(Convert.ToInt32(this.icdoBenefitCalculationHeader.idecSurvivorFullAge), Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge));
            }

            ldecSurvivorLifeAnnuity = Math.Round(adecFinalAccruedBenefitAmount * ldecLifeAnnuityFactor, 2);
            lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(this.icdoBenefitCalculationHeader.iintPlanId, busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY), ldecLifeAnnuityFactor, ldecLifeAnnuityFactor, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_BENEFICIARY, this.icdoBenefitCalculationHeader.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY, ldecSurvivorLifeAnnuity);
            lbusBenefitCalculationOption.icdoBenefitCalculationOptions.survivor_percent_amount = adecFinalAccruedBenefitAmount;
            if (this.icdoBenefitCalculationHeader.iintPlanId == busConstant.MPIPP_PLAN_ID && this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL)
            {
                GetMinimumGuarantee(adecFinalAccruedBenefitAmount);
            }
            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);


        }
        */

        public void SetUpDeathPostElection()
        {

        }

        public void SetUpPreRequistesPostElection()
        {
            if (!this.ibusBenefitApplication.iclbPayeeAccount.IsNullOrEmpty())
            {
                if (this.ibusBenefitApplication.iclbPayeeAccount.Where(lbusP => lbusP.icdoPayeeAccount.iintPlanId == this.icdoBenefitCalculationHeader.iintPlanId).Count() > 0)
                {
                    ibusPayeeAccount = (from lbusPayeeAccount in this.ibusBenefitApplication.iclbPayeeAccount
                                        where lbusPayeeAccount.icdoPayeeAccount.iintPlanId ==
                                            this.icdoBenefitCalculationHeader.iintPlanId
                                        select lbusPayeeAccount).FirstOrDefault();
                    ibusPayeeAccount.LoadBenefitDetails();

                }
            }
        }
        public ArrayList LumpSumPaymentPreDeath()
        {
            ArrayList larr = new ArrayList();
           this.icdoBenefitCalculationHeader.lump_sum_payment = busConstant.FLAG_YES;
            this.icdoBenefitCalculationHeader.Update();

            if (this.icdoBenefitCalculationHeader.lump_sum_payment == busConstant.FLAG_YES)
            {
                if (this.icdoBenefitCalculationHeader.benefit_type_value == busConstant.BENEFIT_TYPE_DEATH_PRE_RETIREMENT)
                {
                    this.btn_RefreshCalculation();

                }

            }

            larr.Add(this);
            return larr;
        }
        public void LoadCalculationHeaderDetails()
        {

        }

        #endregion

        //public void LoadSurvivorPercentageAmounts()
        //{
        //    foreach (busBenefitCalculationDetail lbusBenefitCalculationDetail in this.iclbBenefitCalculationDetail)
        //    {
        //        if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID)
        //        {

        //        }
        //        else if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID)
        //        {
        //            lbusBenefitCalculationDetail.idecFinalPercentSurvivorAccrued = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.unreduced_benefit_amount * (this.icdoBenefitCalculationHeader.survivor_percentage/100);

        //        }
        //        else
        //        {

        //        }

        //    }
        //}
    }

}                     
