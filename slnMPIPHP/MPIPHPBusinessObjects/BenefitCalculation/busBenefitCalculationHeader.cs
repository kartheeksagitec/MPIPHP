#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using System.Linq;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using MPIPHP.CustomDataObjects;
using MPIPHP.DataObjects;
using System.Windows.Forms;
using System.Data.SqlClient;

#endregion

namespace MPIPHP.BusinessObjects
{
    /// <summary>
    /// Class MPIPHP.BusinessObjects.busBenefitCalculationHeader:
    /// Inherited from busBenefitCalculationHeaderGen, the class is used to customize the business object busBenefitCalculationHeaderGen.
    /// </summary>
    [Serializable]
    public class busBenefitCalculationHeader : busBenefitCalculationHeaderGen
    {
        public busCalculation ibusCalculation { get; set; }
        public busQdroApplication ibusQdroApplication { get; set; }
        public cdoPerson icdoSurvivorDetails { get; set; }
        public bool iblnWorkHistoryAlreadyFetched { get; set; }
        public busPerson ibusAlternatePayee { get; set; }
        public int sel_benefit_calculation_detail_id { get; set; }
        public busPersonOverview ibusPersonOverview { get; set; }

        public string istrSixtyDaysPriorDate { get; set; }

        public string istrRetirementDt { get; set; }
        public string istrRetirementDtPlus2Mo { get; set; }

        public string istrHealthHours { get; set; }

        public int istrBenefitOptionValueData { get; set; }

        //Ticket 85016 - For PERO-0011 correspondence
        public string istrCurrentLongDate { get; set; }
        public string istrRetirementLongDate { get; set; }

        //not working when setting it in busCalculation
        public int iintIAPPayeeAccountID { get; set; }

        public int intHealthYearCount { get; set; }

        public decimal intHealthHours { get; set; }
        public decimal intTotalHours { get; set; }
        public int iintL52SplAccPayeeAccountID { get; set; }
        public int iintL161SplAccPayeeAccountID { get; set; }
        public int iintMPIPayeeAccountID { get; set; }
        public int iintL52PayeeAccountID { get; set; }
        public int iintL161PayeeAccountID { get; set; }
        public int iintL600PayeeAccountID { get; set; }
        public int iintL666PayeeAccountID { get; set; }
        public int iintL700PayeeAccountID { get; set; }
        public int iintEEUVHPPayeeAccountID { get; set; }
        public string astrFundName { get; set; }
        public int iintOriginalPayeeAccountId { get; set; }

        public bool iblnIAPPayeeAccntCreated { get; set; }
        public bool iblnL52SpecialAccPayeeAccountCreated { get; set; }
        public bool iblnL161SpecialAccPayeeAccountCreated { get; set; }
        public bool iblnEEUVHPPayeeAccountCreated { get; set; }
        public bool iblnMPIPayeeAccountCreated { get; set; }
        public Collection<busPayeeAccountStatus> iclbPayeeAccountStatusByApplication { get; set; }

        public string istrIsUSA { get; set; }
        public string istrBenType { get; set; }

        public busPerson ibusBeneficiary { get; set; }
        public int FLAG { get; set; }
        public int iintTermCertainMonths { get; set; }

        public bool iblnIsParticipant { get; set; }

        public DateTime idtLastWorkingDate { get; set; }

        public int iintSpousePersonId { get; set; }

        public busRetirementApplication ibusRetirementApplication { get; set; }

        // PIR-810
        public string istrParticipantRelationshipWithBeneficiary { get; set; }

        public string istrOrgAddress { get; set; }

        public string istrOrgAddress2 { get; set; }
        public string istrOrgCity { get; set; }
        public string istrOrgState { get; set; }
        public int intOrgZip { get; set; }

        public string istrOrgName { get; set; }

        public string istrOrgMpId { get; set; }

        //PIR-811
        public bool iblnIsDroStatusReceived { get; set; }

        // 1. Check Vesting and Eligibility for the participant and plan
        // 2. Calculate the full accrued benefit amount for the given plan. If estimate and plan = MPIPHP, then calculate for IAP as well
        // 3. Apply the early reduction factor if applicable
        // 4. Using the formulas, calculate the final benefit amount the specific benefit option passed as a parameter (if estimare then calculate for all benefit options available)

        public Collection<busBenefitCalculationYearlyDetail> iclbMDCalculationYearlyDetail { get; set; }

        public DateTime idtRetirementDate { get; set; }
        public DateTime idtDayOneOfNextMonth { get; set; }
        public string istrDayOneOfNextMonth { get; set; }

        public decimal idecTotalPenHrs { get; set; }
        public DateTime idueDate { get; set; }


        #region Properties Specific to Final Calculations
        public bool iblnCalcualteNonVestedEEBenefit { get; set; }
        public bool iblnCalculateMPIPPBenefit { get; set; }
        public bool iblnCalcualteUVHPBenefit { get; set; }
        public bool iblnCalculateIAPBenefit { get; set; }
        public bool iblnCalculateL52SplAccBenefit { get; set; }
        public bool iblnCalculateL161SplAccBenefit { get; set; }
        #endregion

        #region Properties
        public decimal idecSurvivorAgeAtDeath { get; set; }
        // Parameters//COMMENT-ABHISHEK

        // Abhishek - We already have this parameter defined in the Gen.
        // We do not need to redefine it here. I am commeting it out.
        // public busBenefitApplication ibusBenefitApplication { get; set; } //This OBJECT IS CRUCIAL FOR CHECKING ELIGIBILITY                
        public Collection<busPersonAccountRetirementContribution> iclbPersonAccountRetirementContribution { get; set; }

        #region Correspondence Properties

        public Collection<busPlanBenefitXr> iclbcdoPlanBenefit { get; set; }
        public busBenefitCalculationRetirement ibusBenefitCalculationRetirement { get; set; }

        public busDisabiltyBenefitCalculation ibusDisabiltyBenefitCalculation { get; set; }

        public decimal idecParticipantLifeAnnuityBenefitAmt { get; set; }
        public string istrParticipant3YearBenefitAmt { get; set; }
        public string istrParticipantLifeAnnuityBenefitAmt { get; set; }
        public string istrParticipantJSurvivor50BenefitAmt { get; set; }
        public string istrParticipantJPop50BenefitAmt { get; set; }
        public string istrParticipantJSurvivor75BenefitAmt { get; set; }
        public decimal idecParticipantJSurvivor100BenefitAmt { get; set; }
        public string istrParticipantJSurvivor100BenefitAmt { get; set; }
        public string istrParticipantJPopup100BenefitAmt { get; set; }
        public decimal idecParticipantTenYearBenefitAmt { get; set; }
        public string istrParticipantTenYearBenefitAmt { get; set; }

        public string istrSingleParticipantTenYearBenefitAmt { get; set; } //rid 78872
        public decimal idecParticipantLumpSumBenefitAmt { get; set; }//IAP       
        public decimal idecParticipantLumpSumBenefitAmtMPI { get; set; }
        public decimal idecParticipantTwoYearBenefitAmt { get; set; }

        public decimal idecSpecialYear { get; set; } //PIR 847

        public decimal idecParticipantJSurvivor66_2by3BenefitAmt { get; set; } //rid 78872
        public string istrParticipantJSurvivor66_2by3BenefitAmt { get; set; }
        public decimal idecParticipantJLevelIncome { get; set; }
        public string istrParticipantJLevelIncome { get; set; }
        public decimal idecParticipant3YearBenefitAmt { get; set; }
        public decimal idecParticipantUVHP { get; set; }
        public string istrParticipantUVHP { get; set; }
        public decimal idecParticipantLumpSumBenefitAmount { get; set; }//NON IAP
        public decimal idecParticipantLumpSumPlusUVHPBenefitAmount { get; set; }//NON IAP

        public decimal idecUVHPLIFE { get; set; }
        public string istrUVHPLIFE { get; set; }

        public decimal idecParticipantFiveYearBenefitAmt { get; set; }
        public decimal idecLumpSumCumUVHP { get; set; }
        public string istrParticipantLifeAnnuityBenefitAmtForPOPUP { get; set; }

        public decimal idecSurvivorLifeAnnuityBenefitAmt { get; set; }
        public string istrSurvivorJSurvivor50BenefitAmt { get; set; }
        public string istrSurvivorJPop50BenefitAmt { get; set; }
        public string istrSurvivorJSurvivor75BenefitAmt { get; set; }
        public string istrSurvivorJSurvivor100BenefitAmt { get; set; }
        public string istrSurvivorJPopup100BenefitAmt { get; set; }
        public string istrSurvivorJSurvivor66_2by3BenefitAmt { get; set; }
        public string istrParticipantTwoYearBenefitAmt { get; set; }

        public decimal idecParticipantIAPLifeAnnuityBenefitAmt { get; set; }
        public string istrParticipantIAPLifeAnnuityBenefitAmt { get; set; }

        public string istrParticipantIAPLifeAnnuityBenefitAmtForPOPUP { get; set; }

        public string istrParticipantIAPJSurvivor50BenefitAmt { get; set; }

        public string istrParticipantIAPJPop50BenefitAmt { get; set; }
        public string istrParticipantIAPJSurvivor75BenefitAmt { get; set; }
        public string istrParticipantIAPJSurvivor100BenefitAmt { get; set; }
        public string istrParticipantIAPJPopup100BenefitAmt { get; set; }
        public decimal idecParticipantIAPTenYearBenefitAmt { get; set; }
        public string istrParticipantIAPTenYearBenefitAmt { get; set; }
        public decimal idecParticipantIAPTwoYearBenefitAmt { get; set; }
        public decimal iintIAPAsOfDate { get; set; }


        public string istrPartIAPAnnuityJPop50BenefitAmt { get; set; }
        public string istrPartAnnuityJPop100BenefitAmt { get; set; }
        public string istrPartIAPAnnuityJPop100BenefitAmt { get; set; }


        public decimal idecSurvivorIAPLifeAnnuityBenefitAmt { get; set; }
        public string istrSurvivorIAPLifeAnnuityBenefitAmt { get; set; }
        public string istrSurvivorIAPJSurvivor50BenefitAmt { get; set; }
        public string istrSurvivorIAPJPop50BenefitAmt { get; set; }
        public string istrSurvivorIAPJSurvivor75BenefitAmt { get; set; }
        public string istrSurvivorIAPJSurvivor100BenefitAmt { get; set; }
        public string istrSurvivorIAPJPopup100BenefitAmt { get; set; }

        public string istrLocalSpecialAcntPlan { get; set; }
        public string istrPartLifeAnnuityL52_L161SpAcnt { get; set; }
        public string istrPartJS50L52_L161SpAcnt { get; set; }
        public string istrPartJS75L52_L161SpAcnt { get; set; }
        public string istrPartLumpSumL52_L161SpAcnt { get; set; }

        public string istrLifeAnnuityRelativeValue { get; set; }
        public string istrJSPop50RelativeValue { get; set; }
        public string istrJS75RelativeValue { get; set; }
        public string istrJS100RelativeValue { get; set; }
        public string istrJS100PopRelativeValue { get; set; }
        public string istrTenCertainRelativeValue { get; set; }
        public string istrTwoCertainRelativeValue { get; set; }
        public string istrJS66_2by3RelativeValue { get; set; }
        public string istrLevelIncome { get; set; }
        public string istrFiveYearRelativeValue { get; set; }
        public string istrParticipantLumpSumBenefitAmount { get; set; }
        public string istrParticipantLumpSumBenefitAmt { get; set; }
        public string istrParticipantFiveYearBenefitAmt { get; set; }
        public bool iblnNoBeneficiaryExists { get; set; }

        public decimal idecEmployeeContributionBalance { get; set; }
        public string istrEEAsOfDate { get; set; }
        public int iintLumpSumYearByAge { get; set; }
        public int iintLumpSumYear { get; set; } //78634

        public string istrEligibleForRetireeHealth { get; set; }
        public string istrDisabilityOnsetDate { get; set; }
        public string istrBenefitOptionPercent { get; set; }

        public decimal idecEEUVHPTotal { get; set; }
        public decimal idecEEUVHPAmt { get; set; }
        public decimal idecEEUVHPInterest { get; set; }



        public decimal idecUVHPTotal { get; set; }
        public decimal idecEEContri { get; set; }
        public decimal idecEEInt { get; set; }
        public decimal idecTotalEE { get; set; }

        public decimal idecAccntBalance { get; set; }
        public decimal idecPensionAcctBal { get; set; }

        public decimal idecMPILumpSum { get; set; }

        public decimal idecEEUVHPLumSum { get; set; }

        public string istrDistributionType { get; set; }         ///commented by aarti need to ask abhishek
        public string istrNormalRtmtDate { get; set; }
        public string istrBenefitOptionValue { get; set; }

        public string istrCurrentTime { get; set; }
        public string istrPlanLocal { get; set; }
        public string istrIsLocal52 { get; set; }

        public string istrBoth { get; set; }
        public string istrEEFlag { get; set; }
        public string istrUVHPFlag { get; set; }
        public string istrDueDate { get; set; }


        //RETR-0038
        public DateTime idtCrntDate { get; set; }
        public int iintTotalQYrs { get; set; }
        public int iintTotalHealthYrs { get; set; }
        public decimal idecTotalCreditHrs { get; set; }
        public decimal idecTotalHealthHrs { get; set; }
        public decimal idecAcrdBenefitMPI { get; set; }
        public decimal idecAcrdBenefitLocal { get; set; } //rid 78456
        public decimal idecAcrdBenefitIAP { get; set; }
        public decimal idecEEContriAmt { get; set; }
        public decimal idecEEIntAmt { get; set; }
        public decimal idecUVHPContriAmt { get; set; }
        public decimal idecUVHPIntAmt { get; set; }
        public decimal idecTotalEEAmt { get; set; }
        public decimal idecTotalUVHPAmt { get; set; }
        public decimal idecQDROffsetMPI { get; set; }
        public string istrIsQDROffsetMPI { get; set; }
        public decimal idecQDROffsetIAP { get; set; }
        public string istrIsQDROffsetIAP { get; set; }
        public decimal idecIAPBalanceAmt { get; set; }

        public decimal idecLocal52SPABalanceAmt { get; set; } //rid 78456
        public decimal idecLocal161SPABalanceAmt { get; set; } //rid 78456
        public int iintYear { get; set; }
        public bool iblnQualifiedYrs { get; set; }
        public int iintParticipantAge { get; set; }
        public int iintSpouseAge { get; set; }
        public string istrQDROffset { get; set; }
        public Collection<busBenefitCalculationOptions> iclbbusBenefitCalculationOptions { get; set; }

        public int iintVestedYears { get; set; }
        public decimal idecVestedHours { get; set; }



        //payee- 0035
        public string istrParticipantMPIID { get; set; }
        public string istrPlan { get; set; }

        //PIR 861
        public DateTime idtCutoffEnddate { get; set; }
        public string istrVestedHours { get; set; }
        public busPerson ibusParticipant { get; set; }
        public bool iblnIAPFactor { get; set; }
        #endregion

        public string istrParticipantFullName { get; set; }
        #endregion

        #region Public Methods

        public busBenefitCalculationHeader()
        {
            if (this.ibusCalculation.IsNull())
            {
                this.ibusCalculation = new busCalculation();
            }
        }


        public ArrayList CalculateBenefit(int aintPersonId, int aintBenefitApplicationId, int aintBeneficiaryPersonId, string astrCalculationType, string astrBenefitTypeValue, string astrBenefitCalculationStatusValue,
                                                DateTime adtRetirementDate, decimal adecAge, string astrBenefitOptionValue, int aintBenefitApplicationDetailId,
                                                int aintPersonAccountId, int aintPlanId, DateTime adtVestedDate, string astrBenefitSubTypeValue)
        {
            ArrayList larlError = new ArrayList();

            // We cannot check vesting and eligibility until we get the Retirement Date and Age
            //larlError = ConfirmEligibility();
            //if (larlError.IsNotNull() && larlError.Count > 0)
            //    return larlError;

            //int lintPlanId = busGlobalFunctions.GetPlanIdFromPlanCode(astrPlanCode);

            // Populate the Benefit Calculation Header object
            PopulateInitialDataBenefitCalculationHeader(aintPersonId, aintBenefitApplicationId, aintBeneficiaryPersonId, astrBenefitTypeValue, astrCalculationType, adtRetirementDate, adecAge, aintPlanId);
            //PopulateInitialDataBenefitCalculationDetails(this.icdoBenefitCalculationHeader.benefit_calculation_header_id, aintBenefitApplicationDetailId, aintPersonAccountId, aintPlanId, adtVestedDate, astrBenefitSubTypeValue);

            switch (aintPlanId)
            {
                case busConstant.MPIPP_PLAN_ID:
                    break;

                case busConstant.IAP_PLAN_ID:
                    break;

                default:
                    break;

            }

            return larlError;
        }

        public decimal GetEarlyReductionFactor(int aintBenefitProvisionId, string astrBenefitProvisionTypeValue, string astrBenefitProvisionSubTypeValue, int aintAge)
        {
            object lobjERF = DBFunction.DBExecuteScalar("cdoBenefitProvisionBenefitTypeFactor.GetEarlyReductionBenefitFactor", new object[4] { aintBenefitProvisionId, astrBenefitProvisionTypeValue, astrBenefitProvisionSubTypeValue, aintAge },
                                                         iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            if (lobjERF.IsNotNull())
            {
                return (decimal)lobjERF;
            }

            return 1;
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
                        if (!lbusBenefitCalculationYearlyDetail.iclbBenefitCalculationNonsuspendibleDetail.IsNullOrEmpty())
                        {
                            foreach (busBenefitCalculationNonsuspendibleDetail lbusBenefitCalculationNonsuspendibleDetail in lbusBenefitCalculationYearlyDetail.iclbBenefitCalculationNonsuspendibleDetail)
                            {
                                if (lbusBenefitCalculationNonsuspendibleDetail.icdoBenefitCalculationNonsuspendibleDetail.benefit_calculation_yearly_detail_id != 0)
                                {
                                    lbusBenefitCalculationNonsuspendibleDetail.icdoBenefitCalculationNonsuspendibleDetail.Delete();
                                }
                            }
                            lbusBenefitCalculationYearlyDetail.iclbBenefitCalculationNonsuspendibleDetail.Clear();
                        }

                        lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.Delete();
                    }
                    lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.Clear();
                }

                lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.Delete();
            }

            this.iclbBenefitCalculationDetail.Clear();
        }

        public void EmployeeContributionBalance(busPersonAccount abusPersonAccount, DateTime adtRetirementDate)
        {
            int lintPersonAccountId = abusPersonAccount.icdoPersonAccount.person_account_id;
            //decimal ldecVestedEEContributionAmount = busConstant.ZERO_DECIMAL, ldecVestedEEInterestAmount = busConstant.ZERO_DECIMAL, ldecNonVestedEEcontributionAmount = busConstant.ZERO_DECIMAL,
            //    ldecNonVestedEEInterest = busConstant.ZERO_DECIMAL;
            decimal ldecEEInterestAmount = busConstant.ZERO_DECIMAL;
            // Fetch the UV & HP Contribution and Interest Amounts 
            //ataTable ldtbEEContributionsInterest = busBase.Select("cdoPersonAccountRetirementContribution.GetTotalEEContribution", new object[] { lintPersonAccountId });

            if (abusPersonAccount != null && iclbPersonAccountRetirementContribution != null)
            {
                decimal ldecPriorYearInterest = decimal.Zero;
                ldecEEInterestAmount = abusPersonAccount.icdoPersonAccount.idecVestedEEInterest + abusPersonAccount.icdoPersonAccount.idecNonVestedEEInterest +
                                       this.ibusCalculation.CalculatePartialEEInterest(adtRetirementDate, abusPersonAccount, true, true, iclbPersonAccountRetirementContribution, out ldecPriorYearInterest);

                DateTime ldtEEAsOfDate = (from item in iclbPersonAccountRetirementContribution
                                          where item.icdoPersonAccountRetirementContribution.person_account_id == abusPersonAccount.icdoPersonAccount.person_account_id
                                          select item.icdoPersonAccountRetirementContribution.transaction_date).Max();

                istrEEAsOfDate = busGlobalFunctions.ConvertDateIntoDifFormat(ldtEEAsOfDate);

                idecEmployeeContributionBalance = abusPersonAccount.icdoPersonAccount.idecVestedEE + abusPersonAccount.icdoPersonAccount.idecNonVestedEE + ldecEEInterestAmount;
            }
        }

        public ArrayList OverrideBenefitAmount()
        {
            ArrayList larr = new ArrayList();
            //EmergencyOneTimePayment - 03/17/2017
            busBenefitApplication lbusBenApp = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
            lbusBenApp.FindBenefitApplication(this.icdoBenefitCalculationHeader.benefit_application_id);

            if (!this.iclbBenefitCalculationDetail.IsNullOrEmpty())
            {
                if (!this.iclbBenefitCalculationDetail.FirstOrDefault().iclbBenefitCalculationOptions.IsNullOrEmpty())
                {
                    busBenefitCalculationOptions lbusBenefitCalculationOptions = this.iclbBenefitCalculationDetail.FirstOrDefault().iclbBenefitCalculationOptions.FirstOrDefault();
                    if (this.icdoBenefitCalculationHeader.idecOverriddenBenefitAmount > decimal.Zero)
                    {
                        if (lbusBenApp.icdoBenefitApplication.emergency_onetime_payment_flag.IsNullOrEmpty() && lbusBenApp.icdoBenefitApplication.withdrawal_type_value.IsNullOrEmpty())
                        {
                            lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.overridden_benefit_amount = Math.Round(this.icdoBenefitCalculationHeader.idecOverriddenBenefitAmount, 2);
                        }
                        else
                        {
                            lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.overridden_benefit_amount = Math.Round(this.icdoBenefitCalculationHeader.idecOverriddenBenefitAmount, 2);
                        }

                        lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.Update();
                    }
                }
            }

            larr.Add(this);
            return larr;
        }

        //PIR 944
        public ArrayList LumpSumPayment()
        {
            ArrayList larr = new ArrayList();
            icdoBenefitCalculationHeader.lump_sum_payment = busConstant.FLAG_YES;
            icdoBenefitCalculationHeader.Update();

            larr.Add(this);
            return larr;
        }

        #endregion Public Methods

        #region Private Methoods

        private ArrayList ConfirmEligibility()
        {

            ArrayList larlError = new ArrayList();
            busBenefitApplication lbusBenefitApplication = new busBenefitApplication() { icdoBenefitApplication = new cdoBenefitApplication() };
            lbusBenefitApplication.icdoBenefitApplication.person_id = this.icdoBenefitCalculationHeader.person_id;
            lbusBenefitApplication.idecAge = this.icdoBenefitCalculationHeader.age;

            lbusBenefitApplication.LoadPerson();
            lbusBenefitApplication.ibusPerson.LoadPersonAccounts();
            lbusBenefitApplication.LoadWorkHistoryandSetupPrerequisites_Retirement();

            if (lbusBenefitApplication.NotEligible == false) // We need to check one more condition here whether the ELIGBILE plan Collection Contains PENSION PLAN OR NOT
            {
                utlError lobjError = AddError(5103, " ");
                larlError.Add(lobjError);
            }

            return larlError;
        }


        private void LoadIAPPlanPropertiesforCorrespondence(string astrTemplateName)
        {
            Collection<busBenefitCalculationOptions> lclbIAPbusBenefitCalculationOptions =
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().iclbBenefitCalculationOptions.ToList().ToCollection();

            #region Annuity
            int lintPlanBenefitId = ibusCalculation.GetPlanBenefitId(busConstant.IAP_PLAN_ID, busConstant.LIFE_ANNUTIY);
            busBenefitCalculationOptions lbusBeneOption = null;
            if (lclbIAPbusBenefitCalculationOptions.Where(option => option.icdoBenefitCalculationOptions.plan_benefit_id == lintPlanBenefitId).Count() > 0)
            {
                if (lclbIAPbusBenefitCalculationOptions.Where(option => option.icdoBenefitCalculationOptions.plan_benefit_id == lintPlanBenefitId && option.icdoBenefitCalculationOptions.local161_special_acct_bal_flag == busConstant.FLAG_YES).Count() > 0)
                {
                    istrLocalSpecialAcntPlan = "Local 161";
                    decimal ldecParticipantJSurvivor50BenefitAmtIAP = lclbIAPbusBenefitCalculationOptions.Where(option => option.icdoBenefitCalculationOptions.plan_benefit_id == lintPlanBenefitId && option.icdoBenefitCalculationOptions.local161_special_acct_bal_flag == busConstant.FLAG_YES).FirstOrDefault().icdoBenefitCalculationOptions.benefit_amount;
                    if (ldecParticipantJSurvivor50BenefitAmtIAP > 0)
                        istrPartLifeAnnuityL52_L161SpAcnt = AppendDoller(ldecParticipantJSurvivor50BenefitAmtIAP);
                }
                else if (lclbIAPbusBenefitCalculationOptions.Where(option => option.icdoBenefitCalculationOptions.plan_benefit_id == lintPlanBenefitId && option.icdoBenefitCalculationOptions.local52_special_acct_bal_flag == busConstant.FLAG_YES).Count() > 0)
                {
                    istrLocalSpecialAcntPlan = "Local 52";
                    decimal ldecParticipantJS50AmtL52SpAct = lclbIAPbusBenefitCalculationOptions.Where(option => option.icdoBenefitCalculationOptions.plan_benefit_id == lintPlanBenefitId && option.icdoBenefitCalculationOptions.local52_special_acct_bal_flag == busConstant.FLAG_YES).FirstOrDefault().icdoBenefitCalculationOptions.benefit_amount;
                    if (ldecParticipantJS50AmtL52SpAct > 0)
                        istrPartLifeAnnuityL52_L161SpAcnt = AppendDoller(ldecParticipantJS50AmtL52SpAct);
                }


                if (lclbIAPbusBenefitCalculationOptions.Where(option => option.icdoBenefitCalculationOptions.plan_benefit_id == lintPlanBenefitId &&
                     !(option.icdoBenefitCalculationOptions.local52_special_acct_bal_flag == busConstant.FLAG_YES) && !(option.icdoBenefitCalculationOptions.local161_special_acct_bal_flag == busConstant.FLAG_YES)
                    ).Count() > 0)
                {
                    lbusBeneOption = lclbIAPbusBenefitCalculationOptions.Where(option => option.icdoBenefitCalculationOptions.plan_benefit_id == lintPlanBenefitId &&
                        !(option.icdoBenefitCalculationOptions.local52_special_acct_bal_flag == busConstant.FLAG_YES) && !(option.icdoBenefitCalculationOptions.local161_special_acct_bal_flag == busConstant.FLAG_YES)
                        ).FirstOrDefault();
                    idecParticipantIAPLifeAnnuityBenefitAmt = lbusBeneOption.icdoBenefitCalculationOptions.benefit_amount;

                    if (idecParticipantIAPLifeAnnuityBenefitAmt > 0)
                        istrParticipantIAPLifeAnnuityBenefitAmt = AppendDoller(idecParticipantIAPLifeAnnuityBenefitAmt);

                    idecSurvivorIAPLifeAnnuityBenefitAmt = lbusBeneOption.icdoBenefitCalculationOptions.survivor_amount;
                    istrSurvivorIAPLifeAnnuityBenefitAmt = AppendDoller(idecSurvivorIAPLifeAnnuityBenefitAmt);
                }
            }
            #endregion

            #region JS50
            lintPlanBenefitId = ibusCalculation.GetPlanBenefitId(busConstant.IAP_PLAN_ID, busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY);
            lbusBeneOption = null;
            if (lclbIAPbusBenefitCalculationOptions.Where(option => option.icdoBenefitCalculationOptions.plan_benefit_id == lintPlanBenefitId && option.icdoBenefitCalculationOptions.local161_special_acct_bal_flag == busConstant.FLAG_YES).Count() > 0)
            {
                istrLocalSpecialAcntPlan = "Local 161";
                decimal ldecParticipantJSurvivor50BenefitAmtIAP = lclbIAPbusBenefitCalculationOptions.Where(option => option.icdoBenefitCalculationOptions.plan_benefit_id == lintPlanBenefitId && option.icdoBenefitCalculationOptions.local161_special_acct_bal_flag == busConstant.FLAG_YES).FirstOrDefault().icdoBenefitCalculationOptions.benefit_amount;
                if (ldecParticipantJSurvivor50BenefitAmtIAP > 0)
                    istrPartJS50L52_L161SpAcnt = AppendDoller(ldecParticipantJSurvivor50BenefitAmtIAP);
            }
            else if (lclbIAPbusBenefitCalculationOptions.Where(option => option.icdoBenefitCalculationOptions.plan_benefit_id == lintPlanBenefitId && option.icdoBenefitCalculationOptions.local52_special_acct_bal_flag == busConstant.FLAG_YES).Count() > 0)
            {
                istrLocalSpecialAcntPlan = "Local 52";
                decimal ldecParticipantJS50AmtL52SpAct = lclbIAPbusBenefitCalculationOptions.Where(option => option.icdoBenefitCalculationOptions.plan_benefit_id == lintPlanBenefitId && option.icdoBenefitCalculationOptions.local52_special_acct_bal_flag == busConstant.FLAG_YES).FirstOrDefault().icdoBenefitCalculationOptions.benefit_amount;
                if (ldecParticipantJS50AmtL52SpAct > 0)
                    istrPartJS50L52_L161SpAcnt = AppendDoller(ldecParticipantJS50AmtL52SpAct);
            }

            if (lclbIAPbusBenefitCalculationOptions.Where(option => option.icdoBenefitCalculationOptions.plan_benefit_id == lintPlanBenefitId && !(option.icdoBenefitCalculationOptions.local52_special_acct_bal_flag == busConstant.FLAG_YES) && !(option.icdoBenefitCalculationOptions.local161_special_acct_bal_flag == busConstant.FLAG_YES)).Count() > 0)
            {
                lbusBeneOption = lclbIAPbusBenefitCalculationOptions.Where(option => option.icdoBenefitCalculationOptions.plan_benefit_id == lintPlanBenefitId && !(option.icdoBenefitCalculationOptions.local52_special_acct_bal_flag == busConstant.FLAG_YES) && !(option.icdoBenefitCalculationOptions.local161_special_acct_bal_flag == busConstant.FLAG_YES)).FirstOrDefault();
                decimal ldecPartJS50BenAmt = lbusBeneOption.icdoBenefitCalculationOptions.benefit_amount;
                if (ldecPartJS50BenAmt > 0)
                    istrParticipantIAPJSurvivor50BenefitAmt = AppendDoller(ldecPartJS50BenAmt);

                decimal ldecSurvivorIAPJS50BenAmt = lbusBeneOption.icdoBenefitCalculationOptions.survivor_amount;
                if (ldecSurvivorIAPJS50BenAmt > 0)
                    istrSurvivorIAPJSurvivor50BenefitAmt = AppendDoller(ldecSurvivorIAPJS50BenAmt);
            }
            #endregion

            #region JP50 POP
            lintPlanBenefitId = ibusCalculation.GetPlanBenefitId(busConstant.IAP_PLAN_ID, busConstant.JOINT_50_PERCENT_POPUP_ANNUITY);
            lbusBeneOption = null;
            if (lclbIAPbusBenefitCalculationOptions.Where(option => option.icdoBenefitCalculationOptions.plan_benefit_id == lintPlanBenefitId && !(option.icdoBenefitCalculationOptions.local52_special_acct_bal_flag == busConstant.FLAG_YES) && !(option.icdoBenefitCalculationOptions.local161_special_acct_bal_flag == busConstant.FLAG_YES)).Count() > 0)
            {
                lbusBeneOption = lclbIAPbusBenefitCalculationOptions.Where(option => option.icdoBenefitCalculationOptions.plan_benefit_id == lintPlanBenefitId && !(option.icdoBenefitCalculationOptions.local52_special_acct_bal_flag == busConstant.FLAG_YES) && !(option.icdoBenefitCalculationOptions.local161_special_acct_bal_flag == busConstant.FLAG_YES)).FirstOrDefault();
                decimal ldecPartIAPJPop50BenAmt = lbusBeneOption.icdoBenefitCalculationOptions.benefit_amount;

                if (ldecPartIAPJPop50BenAmt > 0)
                    istrParticipantIAPJPop50BenefitAmt = AppendDoller(ldecPartIAPJPop50BenAmt);

                if (istrParticipantIAPLifeAnnuityBenefitAmt.IsNotNullOrEmpty())
                    istrPartIAPAnnuityJPop50BenefitAmt = istrParticipantIAPLifeAnnuityBenefitAmt;

                decimal ldecSurvivorIAPJPop50BenAmt = lbusBeneOption.icdoBenefitCalculationOptions.survivor_amount;

                if (ldecSurvivorIAPJPop50BenAmt > 0)
                    istrSurvivorIAPJPop50BenefitAmt = AppendDoller(ldecSurvivorIAPJPop50BenAmt);

                istrParticipantIAPLifeAnnuityBenefitAmtForPOPUP = istrParticipantLifeAnnuityBenefitAmt;

            }
            #endregion

            #region JS100
            lintPlanBenefitId = ibusCalculation.GetPlanBenefitId(busConstant.IAP_PLAN_ID, busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY);
            lbusBeneOption = null;
            if (lclbIAPbusBenefitCalculationOptions.Where(option => option.icdoBenefitCalculationOptions.plan_benefit_id == lintPlanBenefitId && !(option.icdoBenefitCalculationOptions.local52_special_acct_bal_flag == busConstant.FLAG_YES) && !(option.icdoBenefitCalculationOptions.local161_special_acct_bal_flag == busConstant.FLAG_YES)).Count() > 0)
            {
                lbusBeneOption = lclbIAPbusBenefitCalculationOptions.Where(option => option.icdoBenefitCalculationOptions.plan_benefit_id == lintPlanBenefitId && !(option.icdoBenefitCalculationOptions.local52_special_acct_bal_flag == busConstant.FLAG_YES) && !(option.icdoBenefitCalculationOptions.local161_special_acct_bal_flag == busConstant.FLAG_YES)).FirstOrDefault();
                decimal ldecPartIAPJS100BenAmt = lbusBeneOption.icdoBenefitCalculationOptions.benefit_amount;
                if (ldecPartIAPJS100BenAmt > 0)
                    istrParticipantIAPJSurvivor100BenefitAmt = AppendDoller(ldecPartIAPJS100BenAmt);

                decimal ldecSurvivorIAPJS100BenAmt = lbusBeneOption.icdoBenefitCalculationOptions.survivor_amount;
                if (ldecSurvivorIAPJS100BenAmt > 0)
                    istrSurvivorIAPJSurvivor100BenefitAmt = AppendDoller(ldecSurvivorIAPJS100BenAmt);
            }

            #endregion

            #region JP100 POP
            lintPlanBenefitId = ibusCalculation.GetPlanBenefitId(busConstant.IAP_PLAN_ID, busConstant.JOINT_100_PERCENT_POPUP_ANNUITY);
            lbusBeneOption = null;
            if (lclbIAPbusBenefitCalculationOptions.Where(option => option.icdoBenefitCalculationOptions.plan_benefit_id == lintPlanBenefitId && !(option.icdoBenefitCalculationOptions.local52_special_acct_bal_flag == busConstant.FLAG_YES) && !(option.icdoBenefitCalculationOptions.local161_special_acct_bal_flag == busConstant.FLAG_YES)).Count() > 0)
            {
                lbusBeneOption = lclbIAPbusBenefitCalculationOptions.Where(option => option.icdoBenefitCalculationOptions.plan_benefit_id == lintPlanBenefitId && !(option.icdoBenefitCalculationOptions.local52_special_acct_bal_flag == busConstant.FLAG_YES) && !(option.icdoBenefitCalculationOptions.local161_special_acct_bal_flag == busConstant.FLAG_YES)).FirstOrDefault();
                istrParticipantIAPLifeAnnuityBenefitAmtForPOPUP = istrParticipantLifeAnnuityBenefitAmt;

                decimal ldecPartIAPJPopup100BenAmt = lbusBeneOption.icdoBenefitCalculationOptions.benefit_amount;

                if (ldecPartIAPJPopup100BenAmt > 0)
                    istrParticipantIAPJPopup100BenefitAmt = AppendDoller(ldecPartIAPJPopup100BenAmt);

                if (istrParticipantIAPLifeAnnuityBenefitAmt.IsNotNullOrEmpty())
                    istrPartIAPAnnuityJPop100BenefitAmt = istrParticipantIAPLifeAnnuityBenefitAmt;

                decimal ldecSurvivorIAPJPopup100BenefitAmt = lbusBeneOption.icdoBenefitCalculationOptions.survivor_amount;

                if (ldecSurvivorIAPJPopup100BenefitAmt > 0)
                    istrSurvivorIAPJPopup100BenefitAmt = AppendDoller(ldecSurvivorIAPJPopup100BenefitAmt);
            }
            #endregion

            #region Lump SUMP
            lintPlanBenefitId = ibusCalculation.GetPlanBenefitId(busConstant.IAP_PLAN_ID, busConstant.LUMP_SUM);
            lbusBeneOption = null;
            if (lclbIAPbusBenefitCalculationOptions.Where(option => option.icdoBenefitCalculationOptions.plan_benefit_id == lintPlanBenefitId && option.icdoBenefitCalculationOptions.local161_special_acct_bal_flag == busConstant.FLAG_YES).Count() > 0)
            {
                lbusBeneOption = lclbIAPbusBenefitCalculationOptions.Where(option => option.icdoBenefitCalculationOptions.plan_benefit_id == lintPlanBenefitId && option.icdoBenefitCalculationOptions.local161_special_acct_bal_flag == busConstant.FLAG_YES).FirstOrDefault();

                istrLocalSpecialAcntPlan = "Local 161";
                decimal ldecParticipantLumpsumBenefitAmtIAP = lbusBeneOption.icdoBenefitCalculationOptions.benefit_amount;
                if (ldecParticipantLumpsumBenefitAmtIAP > 0)
                    istrPartLumpSumL52_L161SpAcnt = AppendDoller(ldecParticipantLumpsumBenefitAmtIAP);
                //Ticket#116779
                if (astrTemplateName == busConstant.IAP_SECOND_PAYMENT_LETTER)
                {
                    istrParticipantLumpSumBenefitAmt = istrPartLumpSumL52_L161SpAcnt;
                }
                
            }
            lbusBeneOption = null;
            if (lclbIAPbusBenefitCalculationOptions.Where(option => option.icdoBenefitCalculationOptions.plan_benefit_id == lintPlanBenefitId && option.icdoBenefitCalculationOptions.local52_special_acct_bal_flag == busConstant.FLAG_YES).Count() > 0)
            {
                lbusBeneOption = lclbIAPbusBenefitCalculationOptions.Where(option => option.icdoBenefitCalculationOptions.plan_benefit_id == lintPlanBenefitId && option.icdoBenefitCalculationOptions.local52_special_acct_bal_flag == busConstant.FLAG_YES).FirstOrDefault();

                istrLocalSpecialAcntPlan = "Local 52";
                decimal ldecParticipantLumpSumAmtL52SpAct = lbusBeneOption.icdoBenefitCalculationOptions.benefit_amount;
                if (ldecParticipantLumpSumAmtL52SpAct > 0)
                    istrPartLumpSumL52_L161SpAcnt = AppendDoller(ldecParticipantLumpSumAmtL52SpAct);
                //Ticket#116779
                if (astrTemplateName == busConstant.IAP_SECOND_PAYMENT_LETTER)
                {
                    istrParticipantLumpSumBenefitAmt = istrPartLumpSumL52_L161SpAcnt;
                }

            }
            lbusBeneOption = null;
            if (lclbIAPbusBenefitCalculationOptions.Where(option => option.icdoBenefitCalculationOptions.plan_benefit_id == lintPlanBenefitId && !(option.icdoBenefitCalculationOptions.local52_special_acct_bal_flag == busConstant.FLAG_YES) && !(option.icdoBenefitCalculationOptions.local161_special_acct_bal_flag == busConstant.FLAG_YES)).Count() > 0)
            {
                lbusBeneOption = lclbIAPbusBenefitCalculationOptions.Where(option => option.icdoBenefitCalculationOptions.plan_benefit_id == lintPlanBenefitId && !(option.icdoBenefitCalculationOptions.local52_special_acct_bal_flag == busConstant.FLAG_YES) && !(option.icdoBenefitCalculationOptions.local161_special_acct_bal_flag == busConstant.FLAG_YES)).FirstOrDefault();
                idecParticipantLumpSumBenefitAmt = lbusBeneOption.icdoBenefitCalculationOptions.benefit_amount;

                if (idecParticipantLumpSumBenefitAmt > 0)
                    istrParticipantLumpSumBenefitAmt = AppendDoller(idecParticipantLumpSumBenefitAmt);
            }

            #endregion

            #region TEN YEAR
            lintPlanBenefitId = ibusCalculation.GetPlanBenefitId(busConstant.IAP_PLAN_ID, busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY);
            lbusBeneOption = null;
            if (lclbIAPbusBenefitCalculationOptions.Where(option => option.icdoBenefitCalculationOptions.plan_benefit_id == lintPlanBenefitId && !(option.icdoBenefitCalculationOptions.local52_special_acct_bal_flag == busConstant.FLAG_YES) && !(option.icdoBenefitCalculationOptions.local161_special_acct_bal_flag == busConstant.FLAG_YES)).Count() > 0)
            {
                lbusBeneOption = lclbIAPbusBenefitCalculationOptions.Where(option => option.icdoBenefitCalculationOptions.plan_benefit_id == lintPlanBenefitId && !(option.icdoBenefitCalculationOptions.local52_special_acct_bal_flag == busConstant.FLAG_YES) && !(option.icdoBenefitCalculationOptions.local161_special_acct_bal_flag == busConstant.FLAG_YES)).FirstOrDefault();
                decimal ldecPartIAPTenYearBenAmt = lbusBeneOption.icdoBenefitCalculationOptions.benefit_amount;

                if (ldecPartIAPTenYearBenAmt > 0)
                    istrParticipantIAPTenYearBenefitAmt = AppendDoller(ldecPartIAPTenYearBenAmt);
            }
            #endregion

            #region JS 75
            lintPlanBenefitId = ibusCalculation.GetPlanBenefitId(busConstant.IAP_PLAN_ID, busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY);
            lbusBeneOption = null;
            if (lclbIAPbusBenefitCalculationOptions.Where(option => option.icdoBenefitCalculationOptions.plan_benefit_id == lintPlanBenefitId && option.icdoBenefitCalculationOptions.local161_special_acct_bal_flag == busConstant.FLAG_YES).Count() > 0)
            {
                istrLocalSpecialAcntPlan = "Local 161";
                lbusBeneOption = lclbIAPbusBenefitCalculationOptions.Where(option => option.icdoBenefitCalculationOptions.plan_benefit_id == lintPlanBenefitId && option.icdoBenefitCalculationOptions.local161_special_acct_bal_flag == busConstant.FLAG_YES).FirstOrDefault();

                decimal ldecParticipantJSurvivor75BenefitAmtIAP = lbusBeneOption.icdoBenefitCalculationOptions.benefit_amount;
                if (ldecParticipantJSurvivor75BenefitAmtIAP > 0)
                    istrPartJS75L52_L161SpAcnt = AppendDoller(ldecParticipantJSurvivor75BenefitAmtIAP);
            }
            else if (lclbIAPbusBenefitCalculationOptions.Where(option => option.icdoBenefitCalculationOptions.plan_benefit_id == lintPlanBenefitId && option.icdoBenefitCalculationOptions.local52_special_acct_bal_flag == busConstant.FLAG_YES).Count() > 0)
            {
                istrLocalSpecialAcntPlan = "Local 52";
                lbusBeneOption = lclbIAPbusBenefitCalculationOptions.Where(option => option.icdoBenefitCalculationOptions.plan_benefit_id == lintPlanBenefitId && option.icdoBenefitCalculationOptions.local52_special_acct_bal_flag == busConstant.FLAG_YES).FirstOrDefault();

                decimal ldecParticipantJS75AmtL52SpAct = lbusBeneOption.icdoBenefitCalculationOptions.benefit_amount;
                if (ldecParticipantJS75AmtL52SpAct > 0)
                    istrPartJS75L52_L161SpAcnt = AppendDoller(ldecParticipantJS75AmtL52SpAct);
            }
            lbusBeneOption = null;
            if (lclbIAPbusBenefitCalculationOptions.Where(option => option.icdoBenefitCalculationOptions.plan_benefit_id == lintPlanBenefitId && !(option.icdoBenefitCalculationOptions.local52_special_acct_bal_flag == busConstant.FLAG_YES) && !(option.icdoBenefitCalculationOptions.local161_special_acct_bal_flag == busConstant.FLAG_YES)).Count() > 0)
            {
                lbusBeneOption = lclbIAPbusBenefitCalculationOptions.Where(option => option.icdoBenefitCalculationOptions.plan_benefit_id == lintPlanBenefitId && !(option.icdoBenefitCalculationOptions.local52_special_acct_bal_flag == busConstant.FLAG_YES) && !(option.icdoBenefitCalculationOptions.local161_special_acct_bal_flag == busConstant.FLAG_YES)).FirstOrDefault();
                decimal ldecPartIAPJS75BenAmt = lbusBeneOption.icdoBenefitCalculationOptions.benefit_amount;

                if (ldecPartIAPJS75BenAmt > 0)
                    istrParticipantIAPJSurvivor75BenefitAmt = AppendDoller(ldecPartIAPJS75BenAmt);

                decimal ldecSurvivorIAPJS75BenAmt = lbusBeneOption.icdoBenefitCalculationOptions.survivor_amount;

                if (ldecSurvivorIAPJS75BenAmt > 0)
                    istrSurvivorIAPJSurvivor75BenefitAmt = AppendDoller(ldecSurvivorIAPJS75BenAmt);
            }
            #endregion
        }

        private void LoadPlanPropertiesforCorrespondence(Collection<busBenefitCalculationOptions> aclbBenefitCalculationOptions, int aintPlanId,
                                                            bool ablnGenerateLocalCorresFromBatch, string astrTemplateName)
        {
            LoadPlanBenefitsForPlan(aintPlanId);

            //PIR 847
            if (aintPlanId == busConstant.LOCAL_52_PLAN_ID)
            {
                idecSpecialYear = 0;
                if (this.ibusBenefitApplication.aclbPersonWorkHistory_MPI != null && this.ibusBenefitApplication.aclbPersonWorkHistory_MPI.Count() > 0 &&
                    this.ibusBenefitApplication.aclbPersonWorkHistory_MPI.Where(item => item.year >= busConstant.BenefitCalculation.MERGER_DATE_LOCAL_52.Year && item.qualified_hours >= 400).Count() > 0)
                {
                    idecSpecialYear = this.ibusBenefitApplication.aclbPersonWorkHistory_MPI.Where(item => item.year >= busConstant.BenefitCalculation.MERGER_DATE_LOCAL_52.Year && item.qualified_hours >= 400).Count();
                }
                idecSpecialYear += this.ibusBenefitApplication.Local52_PensionCredits;
            }


            foreach (busBenefitCalculationOptions lbusBenefitCalculationOptions in aclbBenefitCalculationOptions)
            {
                if (lbusBenefitCalculationOptions.ibusPlanBenefitXr.IsNull())
                {
                    lbusBenefitCalculationOptions.ibusPlanBenefitXr = iclbcdoPlanBenefit.Where(planben => planben.icdoPlanBenefitXr.plan_benefit_id == lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.plan_benefit_id).FirstOrDefault();
                }
            }
            foreach (busBenefitCalculationOptions lbusBenefitCalculationOptions in aclbBenefitCalculationOptions)
            {
                if (iclbcdoPlanBenefit.Where(planben => planben.icdoPlanBenefitXr.plan_benefit_id == lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.plan_benefit_id).Count() > 0)
                {
                    string lstrBenOpValue = iclbcdoPlanBenefit.Where(planben => planben.icdoPlanBenefitXr.plan_benefit_id == lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.plan_benefit_id).FirstOrDefault().icdoPlanBenefitXr.benefit_option_value;

                    if (lstrBenOpValue == busConstant.LIFE_ANNUTIY)
                    {
                        //In Retirement Life Annuity Option is valid for both MPI && also for Mpi with EE : Kunal
                        if ((lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.uvhp_flag != busConstant.FLAG_YES && lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.ee_flag != busConstant.FLAG_YES) || astrTemplateName == busConstant.RETIREMENT_BENEFIT_ELECTION_FORM_EE_CONTRIBUTIONS_UVHP_WITHDRAWAL || astrTemplateName == busConstant.Withdrawal_EE_UVHP_Packet || astrTemplateName == busConstant.Withdrawal_EE_UVHP_Retirement_Disablity_Packet || astrTemplateName == busConstant.Withdrawal_EE_UVHP_REFUND_LETTER_C)
                        {
                            if (lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.relative_value.IsNotNullOrEmpty())
                                istrLifeAnnuityRelativeValue = lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.relative_value;
                            idecParticipantLifeAnnuityBenefitAmt = lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.benefit_amount;

                            if (idecParticipantIAPLifeAnnuityBenefitAmt > 0)
                                istrParticipantLifeAnnuityBenefitAmt = AppendDoller(idecParticipantLifeAnnuityBenefitAmt);

                            if (idecParticipantLifeAnnuityBenefitAmt > 0)
                                istrParticipantLifeAnnuityBenefitAmt = AppendDoller(idecParticipantLifeAnnuityBenefitAmt);

                            if (aclbBenefitCalculationOptions.Where(item => item.ibusPlanBenefitXr.icdoPlanBenefitXr.benefit_option_value == busConstant.JOINT_50_PERCENT_POPUP_ANNUITY ||
                                item.ibusPlanBenefitXr.icdoPlanBenefitXr.benefit_option_value == busConstant.JOINT_100_PERCENT_POPUP_ANNUITY || item.ibusPlanBenefitXr.icdoPlanBenefitXr.benefit_option_value == busConstant.JOINT_75_PERCENT_POPUP_ANNUITY).Count() > 0)
                            {
                                if (aintPlanId == 3 || aintPlanId == 7)
                                    istrParticipantLifeAnnuityBenefitAmtForPOPUP = AppendDoller(Math.Ceiling(idecParticipantLifeAnnuityBenefitAmt));

                                else if (aintPlanId == 4 || aintPlanId == 6)
                                    istrParticipantLifeAnnuityBenefitAmtForPOPUP = AppendDoller(Math.Ceiling(idecParticipantLifeAnnuityBenefitAmt / 0.5M) * 0.5M);

                                else
                                    istrParticipantLifeAnnuityBenefitAmtForPOPUP = AppendDoller(idecParticipantLifeAnnuityBenefitAmt);
                            }
                            idecSurvivorLifeAnnuityBenefitAmt = lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.survivor_amount;
                        }
                        else if (lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.uvhp_flag == busConstant.FLAG_YES)
                        {
                            idecUVHPLIFE = lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.benefit_amount;
                            istrUVHPLIFE = AppendDoller(idecUVHPLIFE);
                        }
                    }
                    if (lstrBenOpValue == busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY)
                    {
                        //Ticket#117411
                        //In Retirement Life Annuity Option is valid for both MPI && also for Mpi with EE : Kunal
                        if ((lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.uvhp_flag != busConstant.FLAG_YES && lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.ee_flag != busConstant.FLAG_YES) || astrTemplateName == busConstant.RETIREMENT_BENEFIT_ELECTION_FORM_EE_CONTRIBUTIONS_UVHP_WITHDRAWAL || astrTemplateName == busConstant.Withdrawal_EE_UVHP_Retirement_Disablity_Packet || astrTemplateName == busConstant.Withdrawal_EE_UVHP_REFUND_LETTER_C|| astrTemplateName == busConstant.Withdrawal_EE_UVHP_Packet)
                        {
                            decimal ldecPartJS50BenAmt = lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.benefit_amount;

                            if (ldecPartJS50BenAmt > 0)

                                istrParticipantJSurvivor50BenefitAmt = AppendDoller(lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.benefit_amount);

                            decimal idecSurvivorJS50BenefitAmt = lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.survivor_amount;

                            if (idecSurvivorJS50BenefitAmt > 0)
                                istrSurvivorJSurvivor50BenefitAmt = AppendDoller(lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.survivor_amount);
                        }
                    }

                    if (lstrBenOpValue == busConstant.JOINT_50_PERCENT_POPUP_ANNUITY)
                    {
                        if (lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.relative_value.IsNotNullOrEmpty())
                            istrJSPop50RelativeValue = lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.relative_value;

                        decimal ldecPartJPop50BenAmt = lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.benefit_amount;

                        if (ldecPartJPop50BenAmt > 0)
                            istrParticipantJPop50BenefitAmt = AppendDoller(lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.benefit_amount);

                        decimal ldecSurvivorJPop50BenefitAmt = lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.survivor_amount;

                        if (ldecSurvivorJPop50BenefitAmt > 0)
                            istrSurvivorJPop50BenefitAmt = AppendDoller(lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.survivor_amount);
                    }

                    if (lstrBenOpValue == busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY)
                    {
                        if (lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.relative_value.IsNotNullOrEmpty())
                            istrJS75RelativeValue = lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.relative_value;


                        decimal ldecPartJS75BenAmt = lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.benefit_amount;

                        if (ldecPartJS75BenAmt > 0)
                        {
                            if (aintPlanId == 3 || aintPlanId == 7)
                                istrParticipantJSurvivor75BenefitAmt = AppendDoller(Math.Ceiling(lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.benefit_amount));

                            else if (aintPlanId == 4 || aintPlanId == 6)
                                istrParticipantJSurvivor75BenefitAmt = AppendDoller(Math.Ceiling(lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.benefit_amount / 0.5M) * 0.5M);

                            else
                                istrParticipantJSurvivor75BenefitAmt = AppendDoller(lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.benefit_amount);

                        }
                        decimal ldecSurvivorJS75BenAmt = lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.survivor_amount;

                        if (ldecSurvivorJS75BenAmt > 0)
                            istrSurvivorJSurvivor75BenefitAmt = AppendDoller(lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.survivor_amount);
                    }

                    if (lstrBenOpValue == busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY)
                    {

                        if (lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.relative_value.IsNotNullOrEmpty())
                            istrJS100RelativeValue = lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.relative_value;

                        decimal ldecPartJS100BenAmt = lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.benefit_amount;

                        if (ldecPartJS100BenAmt > 0)
                            istrParticipantJSurvivor100BenefitAmt = AppendDoller(lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.benefit_amount);

                        decimal ldecSurvivorJS100BenAmt = lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.survivor_amount;

                        if (ldecSurvivorJS100BenAmt > 0)
                            istrSurvivorJSurvivor100BenefitAmt = AppendDoller(lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.survivor_amount);
                    }

                    if (lstrBenOpValue == busConstant.JOINT_100_PERCENT_POPUP_ANNUITY)
                    {
                        if (lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.relative_value.IsNotNullOrEmpty())
                            istrJS100PopRelativeValue = lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.relative_value;

                        decimal ldecParJPopup100BenAmt = lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.benefit_amount;

                        if (ldecParJPopup100BenAmt > 0)
                            istrParticipantJPopup100BenefitAmt = AppendDoller(lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.benefit_amount);

                        decimal ldecSurvivorJPopup100BenAmt = lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.survivor_amount;

                        if (ldecSurvivorJPopup100BenAmt > 0)
                            istrSurvivorJPopup100BenefitAmt = AppendDoller(lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.survivor_amount);
                    }

                    if (lstrBenOpValue == busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY)
                    {
                        if (lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.relative_value.IsNotNullOrEmpty())
                            istrTenCertainRelativeValue = lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.relative_value;

                        idecParticipantTenYearBenefitAmt = lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.benefit_amount;


                        if (idecParticipantTenYearBenefitAmt > 0)
                            istrParticipantTenYearBenefitAmt = AppendDoller(idecParticipantTenYearBenefitAmt);

                        if (idecParticipantTenYearBenefitAmt > 0 && ibusBenefitApplication.ibusPerson.icdoPerson.istrMaritalStatus != "Single") //rid 78872
                            istrSingleParticipantTenYearBenefitAmt = AppendDoller(idecParticipantTenYearBenefitAmt);

                        if (aclbBenefitCalculationOptions.Where(item => item.ibusPlanBenefitXr.icdoPlanBenefitXr.benefit_option_value == busConstant.JOINT_50_PERCENT_POPUP_ANNUITY ||
                           item.ibusPlanBenefitXr.icdoPlanBenefitXr.benefit_option_value == busConstant.JOINT_100_PERCENT_POPUP_ANNUITY || item.ibusPlanBenefitXr.icdoPlanBenefitXr.benefit_option_value == busConstant.JOINT_75_PERCENT_POPUP_ANNUITY).Count() > 0)
                        {
                            if (this.icdoBenefitCalculationHeader.iintPlanId == busConstant.LOCAL_600_PLAN_ID)
                            {
                                if (aintPlanId == 3 || aintPlanId == 7)
                                    istrParticipantLifeAnnuityBenefitAmtForPOPUP = AppendDoller(Math.Ceiling(idecParticipantTenYearBenefitAmt));

                                else if (aintPlanId == 4 || aintPlanId == 6)
                                    istrParticipantLifeAnnuityBenefitAmtForPOPUP = AppendDoller(Math.Ceiling(idecParticipantTenYearBenefitAmt / 0.5M) * 0.5M);

                                else
                                    istrParticipantLifeAnnuityBenefitAmtForPOPUP = AppendDoller(idecParticipantTenYearBenefitAmt);
                            }
                        }
                    }
                    if (lstrBenOpValue == busConstant.TWO_YEARS_CERTAIN_AND_LIFE_ANNUITY)
                    {
                        if (lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.relative_value.IsNotNullOrEmpty())
                            istrTwoCertainRelativeValue = lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.relative_value;
                        idecParticipantTwoYearBenefitAmt = lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.benefit_amount;
                        istrParticipantTwoYearBenefitAmt = AppendDoller(idecParticipantTwoYearBenefitAmt);
                    }
                    if (lstrBenOpValue == busConstant.JOINT_66_2by3_PERCENT_SURVIVOR_ANNUITY)
                    {
                        if (lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.relative_value.IsNotNullOrEmpty())
                            istrJS66_2by3RelativeValue = lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.relative_value;
                        istrParticipantJSurvivor66_2by3BenefitAmt = AppendDoller(lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.benefit_amount);
                        istrSurvivorJSurvivor66_2by3BenefitAmt = AppendDoller(lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.survivor_amount);

                        idecParticipantJSurvivor66_2by3BenefitAmt = lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.benefit_amount; //rid 78872
                    }

                    if (lstrBenOpValue == busConstant.LEVEL_INCOME)
                    {
                        if (lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.relative_value.IsNotNullOrEmpty())
                            istrLevelIncome = lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.relative_value;
                        idecParticipantJLevelIncome = lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.benefit_amount;
                        istrParticipantJLevelIncome = AppendDoller(idecParticipantJLevelIncome);
                    }
                    if (lstrBenOpValue == busConstant.THREE_YEARS_CERTAIN_AND_LIFE_ANNUITY)
                    {
                        idecParticipant3YearBenefitAmt = lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.benefit_amount;
                        istrParticipant3YearBenefitAmt = AppendDoller(idecParticipant3YearBenefitAmt);
                        if (aclbBenefitCalculationOptions.Where(item => item.ibusPlanBenefitXr.icdoPlanBenefitXr.benefit_option_value == busConstant.JOINT_50_PERCENT_POPUP_ANNUITY ||
                            item.ibusPlanBenefitXr.icdoPlanBenefitXr.benefit_option_value == busConstant.JOINT_100_PERCENT_POPUP_ANNUITY || item.ibusPlanBenefitXr.icdoPlanBenefitXr.benefit_option_value == busConstant.JOINT_75_PERCENT_POPUP_ANNUITY).Count() > 0)
                        {
                            if (aintPlanId == 3 || aintPlanId == 7)
                                istrParticipantLifeAnnuityBenefitAmtForPOPUP = AppendDoller(Math.Ceiling(idecParticipant3YearBenefitAmt));

                            else if (aintPlanId == 4 || aintPlanId == 6)
                                istrParticipantLifeAnnuityBenefitAmtForPOPUP = AppendDoller(Math.Ceiling(idecParticipant3YearBenefitAmt / 0.5M) * 0.5M);

                            else
                                istrParticipantLifeAnnuityBenefitAmtForPOPUP = AppendDoller(idecParticipant3YearBenefitAmt);
                        }
                    }

                    if (lstrBenOpValue == busConstant.LUMP_SUM)
                    {

                        if (this.icdoBenefitCalculationHeader.benefit_type_value == "WDRL")
                        {
                            idecParticipantLumpSumBenefitAmount = lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.benefit_amount;
                            istrParticipantLumpSumBenefitAmount = AppendDoller(idecParticipantLumpSumBenefitAmount);
                        }
                        else
                        {
                            if (lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.ee_flag != busConstant.FLAG_YES &&
                                            lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.uvhp_flag != busConstant.FLAG_YES)
                            {
                                idecParticipantLumpSumBenefitAmount = lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.benefit_amount;
                                istrParticipantLumpSumBenefitAmount = AppendDoller(idecParticipantLumpSumBenefitAmount);
                            }
                            else if (lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.uvhp_flag == busConstant.FLAG_YES)
                            {
                                idecParticipantUVHP = lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.benefit_amount;
                                istrParticipantUVHP = AppendDoller(idecParticipantUVHP);
                            }
                        }
                    }
                    if (lstrBenOpValue == busConstant.FIVE_YEARS_CERTAIN_AND_LIFE_ANNUTIY)
                    {
                        if (lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.relative_value.IsNotNullOrEmpty())
                            istrFiveYearRelativeValue = lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.relative_value;
                        idecParticipantFiveYearBenefitAmt = lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.benefit_amount;
                        istrParticipantFiveYearBenefitAmt = AppendDoller(idecParticipantFiveYearBenefitAmt);
                    }
                }
            }
        }

        private void LoadIAPPlanDisabilityPropertiesforCorrespondence(string astrTemplateName)
        {
            Collection<busBenefitCalculationOptions> lclbIAPbusBenefitCalculationOptions = new Collection<busBenefitCalculationOptions>();
            //Ticket #101090
            if (astrTemplateName == busConstant.DISABILITY_BENEFIT_ELECTION_MPI_PACKAGE && this.sel_benefit_calculation_detail_id > 0)
            {
                lclbIAPbusBenefitCalculationOptions = this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID && item.icdoBenefitCalculationDetail.retirement_date == icdoBenefitCalculationHeader.retirement_date).FirstOrDefault().iclbBenefitCalculationOptions.ToList().ToCollection();
            }
            else
            {
                lclbIAPbusBenefitCalculationOptions = this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().iclbBenefitCalculationOptions.ToList().ToCollection();
            }

            int lintPlanBenefitId = ibusCalculation.GetPlanBenefitId(busConstant.IAP_PLAN_ID, busConstant.LIFE_ANNUTIY);
            if (lclbIAPbusBenefitCalculationOptions.Where(option => option.icdoBenefitCalculationOptions.plan_benefit_id == lintPlanBenefitId).Count() > 0)
            {
                idecParticipantIAPLifeAnnuityBenefitAmt = lclbIAPbusBenefitCalculationOptions.Where(option => option.icdoBenefitCalculationOptions.plan_benefit_id == lintPlanBenefitId).FirstOrDefault().icdoBenefitCalculationOptions.participant_amount;
                if (idecParticipantIAPLifeAnnuityBenefitAmt > 0)
                    istrParticipantIAPLifeAnnuityBenefitAmt = AppendDoller(idecParticipantIAPLifeAnnuityBenefitAmt);

                idecSurvivorIAPLifeAnnuityBenefitAmt = lclbIAPbusBenefitCalculationOptions.Where(option => option.icdoBenefitCalculationOptions.plan_benefit_id == lintPlanBenefitId).FirstOrDefault().icdoBenefitCalculationOptions.survivor_amount;
            }
            lintPlanBenefitId = ibusCalculation.GetPlanBenefitId(busConstant.IAP_PLAN_ID, busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY);
            if (lclbIAPbusBenefitCalculationOptions.Where(option => option.icdoBenefitCalculationOptions.plan_benefit_id == lintPlanBenefitId).Count() > 0)
            {


                decimal ldecPartJS50BenAmt = lclbIAPbusBenefitCalculationOptions.Where(option => option.icdoBenefitCalculationOptions.plan_benefit_id == lintPlanBenefitId).FirstOrDefault().icdoBenefitCalculationOptions.participant_amount;
                //Ticket 87975
                //if (ldecPartJS50BenAmt > 5000) 
                if (ldecPartJS50BenAmt > 0)
                    istrParticipantIAPJSurvivor50BenefitAmt = AppendDoller(ldecPartJS50BenAmt);

                decimal ldecSurvivorIAPJS50BenAmt = lclbIAPbusBenefitCalculationOptions.Where(option => option.icdoBenefitCalculationOptions.plan_benefit_id == lintPlanBenefitId).FirstOrDefault().icdoBenefitCalculationOptions.survivor_amount;
                if (ldecSurvivorIAPJS50BenAmt > 0)
                    istrSurvivorIAPJSurvivor50BenefitAmt = AppendDoller(ldecSurvivorIAPJS50BenAmt);
            }

            lintPlanBenefitId = ibusCalculation.GetPlanBenefitId(busConstant.IAP_PLAN_ID, busConstant.JOINT_50_PERCENT_POPUP_ANNUITY);
            if (lclbIAPbusBenefitCalculationOptions.Where(option => option.icdoBenefitCalculationOptions.plan_benefit_id == lintPlanBenefitId).Count() > 0)
            {

                decimal ldecPartIAPJPop50BenAmt = lclbIAPbusBenefitCalculationOptions.Where(option => option.icdoBenefitCalculationOptions.plan_benefit_id == lintPlanBenefitId).FirstOrDefault().icdoBenefitCalculationOptions.participant_amount;
                if (ldecPartIAPJPop50BenAmt > 0)
                    istrParticipantIAPJPop50BenefitAmt = AppendDoller(ldecPartIAPJPop50BenAmt);


                decimal ldecSurvivorIAPJPop50BenAmt = lclbIAPbusBenefitCalculationOptions.Where(option => option.icdoBenefitCalculationOptions.plan_benefit_id == lintPlanBenefitId).FirstOrDefault().icdoBenefitCalculationOptions.survivor_amount;
                if (ldecSurvivorIAPJPop50BenAmt > 0)
                    istrSurvivorIAPJPop50BenefitAmt = AppendDoller(ldecSurvivorIAPJPop50BenAmt);

                istrParticipantIAPLifeAnnuityBenefitAmtForPOPUP = istrParticipantLifeAnnuityBenefitAmt;
            }

            lintPlanBenefitId = ibusCalculation.GetPlanBenefitId(busConstant.IAP_PLAN_ID, busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY);
            if (lclbIAPbusBenefitCalculationOptions.Where(option => option.icdoBenefitCalculationOptions.plan_benefit_id == lintPlanBenefitId).Count() > 0)
            {
                decimal ldecPartIAPJS100BenAmt = lclbIAPbusBenefitCalculationOptions.Where(option => option.icdoBenefitCalculationOptions.plan_benefit_id == lintPlanBenefitId).FirstOrDefault().icdoBenefitCalculationOptions.participant_amount;

                if (ldecPartIAPJS100BenAmt > 0)
                    istrParticipantIAPJSurvivor100BenefitAmt = AppendDoller(ldecPartIAPJS100BenAmt);


                decimal ldecSurvivorIAPJS100BenAmt = lclbIAPbusBenefitCalculationOptions.Where(option => option.icdoBenefitCalculationOptions.plan_benefit_id == lintPlanBenefitId).FirstOrDefault().icdoBenefitCalculationOptions.survivor_amount;

                if (ldecSurvivorIAPJS100BenAmt > 0)
                    istrSurvivorIAPJSurvivor100BenefitAmt = AppendDoller(ldecSurvivorIAPJS100BenAmt);
            }

            lintPlanBenefitId = ibusCalculation.GetPlanBenefitId(busConstant.IAP_PLAN_ID, busConstant.JOINT_100_PERCENT_POPUP_ANNUITY);
            if (lclbIAPbusBenefitCalculationOptions.Where(option => option.icdoBenefitCalculationOptions.plan_benefit_id == lintPlanBenefitId).Count() > 0)
            {

                decimal ldecPartIAPJPopup100BenAmt = lclbIAPbusBenefitCalculationOptions.Where(option => option.icdoBenefitCalculationOptions.plan_benefit_id == lintPlanBenefitId).FirstOrDefault().icdoBenefitCalculationOptions.participant_amount;

                if (ldecPartIAPJPopup100BenAmt > 0)
                    istrParticipantIAPJPopup100BenefitAmt = AppendDoller(ldecPartIAPJPopup100BenAmt);

                decimal ldecSurvivorIAPJPopup100BenefitAmt = lclbIAPbusBenefitCalculationOptions.Where(option => option.icdoBenefitCalculationOptions.plan_benefit_id == lintPlanBenefitId).FirstOrDefault().icdoBenefitCalculationOptions.survivor_amount;

                if (ldecSurvivorIAPJPopup100BenefitAmt > 0)
                    istrSurvivorIAPJPopup100BenefitAmt = AppendDoller(ldecSurvivorIAPJPopup100BenefitAmt);
                istrParticipantIAPLifeAnnuityBenefitAmtForPOPUP = istrParticipantLifeAnnuityBenefitAmt;

            }

            lintPlanBenefitId = ibusCalculation.GetPlanBenefitId(busConstant.IAP_PLAN_ID, busConstant.LUMP_SUM);
            if (lclbIAPbusBenefitCalculationOptions.Where(option => option.icdoBenefitCalculationOptions.plan_benefit_id == lintPlanBenefitId).Count() > 0)
            {
                idecParticipantLumpSumBenefitAmt = lclbIAPbusBenefitCalculationOptions.Where(option => option.icdoBenefitCalculationOptions.plan_benefit_id == lintPlanBenefitId).FirstOrDefault().icdoBenefitCalculationOptions.participant_amount;
                if (idecParticipantLumpSumBenefitAmt > 0)
                    istrParticipantLumpSumBenefitAmt = AppendDoller(idecParticipantLumpSumBenefitAmt);
            }
            lintPlanBenefitId = ibusCalculation.GetPlanBenefitId(busConstant.IAP_PLAN_ID, busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY);
            if (lclbIAPbusBenefitCalculationOptions.Where(option => option.icdoBenefitCalculationOptions.plan_benefit_id == lintPlanBenefitId).Count() > 0)
            {

                idecParticipantIAPTenYearBenefitAmt = lclbIAPbusBenefitCalculationOptions.Where(option => option.icdoBenefitCalculationOptions.plan_benefit_id == lintPlanBenefitId).FirstOrDefault().icdoBenefitCalculationOptions.participant_amount;

                if (idecParticipantIAPTenYearBenefitAmt > 0)
                    istrParticipantIAPTenYearBenefitAmt = AppendDoller(idecParticipantIAPTenYearBenefitAmt);
            }
            lintPlanBenefitId = ibusCalculation.GetPlanBenefitId(busConstant.IAP_PLAN_ID, busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY);
            if (lclbIAPbusBenefitCalculationOptions.Where(option => option.icdoBenefitCalculationOptions.plan_benefit_id == lintPlanBenefitId).Count() > 0)
            {

                decimal ldecPartIAPJS75BenAmt = lclbIAPbusBenefitCalculationOptions.Where(option => option.icdoBenefitCalculationOptions.plan_benefit_id == lintPlanBenefitId).FirstOrDefault().icdoBenefitCalculationOptions.participant_amount;

                if (ldecPartIAPJS75BenAmt > 0)
                    istrParticipantIAPJSurvivor75BenefitAmt = AppendDoller(ldecPartIAPJS75BenAmt);

                decimal ldecSurvivorIAPJS75BenAmt = lclbIAPbusBenefitCalculationOptions.Where(option => option.icdoBenefitCalculationOptions.plan_benefit_id == lintPlanBenefitId).FirstOrDefault().icdoBenefitCalculationOptions.survivor_amount;

                if (ldecSurvivorIAPJS75BenAmt > 0)
                    istrSurvivorIAPJSurvivor75BenefitAmt = AppendDoller(ldecSurvivorIAPJS75BenAmt);
            }
        }

        private void LoadPlanDisabilityPropertiesforCorrespondence(Collection<busBenefitCalculationOptions> aclbBenefitCalculationOptions, int aintPlanId, string astrTemplateName)
        {
            if (iclbcdoPlanBenefit.IsNullOrEmpty())
                LoadPlanBenefitsForPlan(aintPlanId);

            foreach (busBenefitCalculationOptions lbusBenefitCalculationOptions in aclbBenefitCalculationOptions)
            {
                if (lbusBenefitCalculationOptions.ibusPlanBenefitXr.IsNull())
                {
                    lbusBenefitCalculationOptions.ibusPlanBenefitXr = iclbcdoPlanBenefit.Where(planben => planben.icdoPlanBenefitXr.plan_benefit_id == lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.plan_benefit_id).FirstOrDefault();
                }
            }

            foreach (busBenefitCalculationOptions lbusBenefitCalculationOptions in aclbBenefitCalculationOptions)
            {
                if (iclbcdoPlanBenefit.Where(planben => planben.icdoPlanBenefitXr.plan_benefit_id == lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.plan_benefit_id).Count() > 0)
                {
                    string lstrBenOpValue = iclbcdoPlanBenefit.Where(planben => planben.icdoPlanBenefitXr.plan_benefit_id == lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.plan_benefit_id).FirstOrDefault().icdoPlanBenefitXr.benefit_option_value;

                    //PIR 847
                    if (aintPlanId == busConstant.LOCAL_52_PLAN_ID)
                    {
                        idecSpecialYear = 0;
                        if (this.ibusBenefitApplication.aclbPersonWorkHistory_MPI != null && this.ibusBenefitApplication.aclbPersonWorkHistory_MPI.Count() > 0 &&
                            this.ibusBenefitApplication.aclbPersonWorkHistory_MPI.Where(item => item.year >= busConstant.BenefitCalculation.MERGER_DATE_LOCAL_52.Year && item.qualified_hours >= 400).Count() > 0)
                        {
                            idecSpecialYear = this.ibusBenefitApplication.aclbPersonWorkHistory_MPI.Where(item => item.year >= busConstant.BenefitCalculation.MERGER_DATE_LOCAL_52.Year && item.qualified_hours >= 400).Count();
                        }
                        idecSpecialYear += this.ibusBenefitApplication.Local52_PensionCredits;
                    }

                    if (lstrBenOpValue == busConstant.LIFE_ANNUTIY)
                    {
                        //In Retirement Life Annuity Option is valid for both MPI && also for Mpi with EE : Kunal
                        if ((lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.uvhp_flag != busConstant.FLAG_YES && lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.ee_flag != busConstant.FLAG_YES) || astrTemplateName == busConstant.RETIREMENT_BENEFIT_ELECTION_FORM_EE_CONTRIBUTIONS_UVHP_WITHDRAWAL)
                        {
                            if (lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.relative_value.IsNotNullOrEmpty())
                                istrLifeAnnuityRelativeValue = lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.relative_value;

                            //Ticket - 76533
                            if (aintPlanId != busConstant.MPIPP_PLAN_ID && aintPlanId != busConstant.IAP_PLAN_ID)
                            {
                                idecParticipantLifeAnnuityBenefitAmt = lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.disability_amount;
                            }
                            else
                            {
                                idecParticipantLifeAnnuityBenefitAmt = lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.participant_amount;
                            }

                            idecSurvivorLifeAnnuityBenefitAmt = lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.survivor_amount;

                            if (idecParticipantLifeAnnuityBenefitAmt > 0)
                                istrParticipantLifeAnnuityBenefitAmt = AppendDoller(idecParticipantLifeAnnuityBenefitAmt);

                            if (aclbBenefitCalculationOptions.Where(item => item.ibusPlanBenefitXr.icdoPlanBenefitXr.benefit_option_value == busConstant.JOINT_50_PERCENT_POPUP_ANNUITY ||
                                item.ibusPlanBenefitXr.icdoPlanBenefitXr.benefit_option_value == busConstant.JOINT_100_PERCENT_POPUP_ANNUITY || item.ibusPlanBenefitXr.icdoPlanBenefitXr.benefit_option_value == busConstant.JOINT_75_PERCENT_POPUP_ANNUITY).Count() > 0)
                            {
                                //if (aintPlanId == 3 || aintPlanId == 7)
                                //    istrParticipantLifeAnnuityBenefitAmtForPOPUP = AppendDoller(Math.Ceiling(idecParticipantLifeAnnuityBenefitAmt));

                                //else if (aintPlanId == 4 || aintPlanId == 6)
                                //    istrParticipantLifeAnnuityBenefitAmtForPOPUP = AppendDoller(Math.Ceiling(idecParticipantLifeAnnuityBenefitAmt / 0.5M) * 0.5M);

                                //else
                                istrParticipantLifeAnnuityBenefitAmtForPOPUP = AppendDoller(idecParticipantLifeAnnuityBenefitAmt);
                            }
                        }
                        else if (lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.uvhp_flag == busConstant.FLAG_YES)
                        {
                            idecUVHPLIFE = lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.benefit_amount;
                            istrUVHPLIFE = AppendDoller(idecUVHPLIFE);
                        }


                    }
                    if (lstrBenOpValue == busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY)
                    {
                        //Ticket#117411
                        if ((lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.uvhp_flag != busConstant.FLAG_YES && lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.ee_flag != busConstant.FLAG_YES) || astrTemplateName == busConstant.RETIREMENT_BENEFIT_ELECTION_FORM_EE_CONTRIBUTIONS_UVHP_WITHDRAWAL|| astrTemplateName == busConstant.Withdrawal_EE_UVHP_Packet)
                        {
                            decimal ldecPartJS50BenAmt = decimal.Zero;

                            //Ticket - 76533
                            if (aintPlanId != busConstant.MPIPP_PLAN_ID && aintPlanId != busConstant.IAP_PLAN_ID)
                            {
                                ldecPartJS50BenAmt = lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.disability_amount;
                            }
                            else
                            {
                                ldecPartJS50BenAmt = lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.participant_amount;
                            }
                                

                            if (ldecPartJS50BenAmt > 0)
                                istrParticipantJSurvivor50BenefitAmt = AppendDoller(ldecPartJS50BenAmt);


                            decimal idecSurvivorJS50BenefitAmt = lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.survivor_amount;

                            if (idecSurvivorJS50BenefitAmt > 0)
                                istrSurvivorJSurvivor50BenefitAmt = AppendDoller(ldecPartJS50BenAmt / 2);
                        }
                    }

                    if (lstrBenOpValue == busConstant.JOINT_50_PERCENT_POPUP_ANNUITY)
                    {
                        if (lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.relative_value.IsNotNullOrEmpty())
                            istrJSPop50RelativeValue = lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.relative_value;


                        decimal ldecPartJPop50BenAmt = decimal.Zero;

                        //Ticket - 76533
                        if (aintPlanId != busConstant.MPIPP_PLAN_ID && aintPlanId != busConstant.IAP_PLAN_ID)
                        {
                            ldecPartJPop50BenAmt = lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.disability_amount;
                        }
                        else
                        {
                            ldecPartJPop50BenAmt = lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.participant_amount;
                        }


                        if (ldecPartJPop50BenAmt > 0)
                            istrParticipantJPop50BenefitAmt = AppendDoller(ldecPartJPop50BenAmt);


                        decimal ldecSurvivorJPop50BenefitAmt = lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.survivor_amount;

                        if (ldecSurvivorJPop50BenefitAmt > 0)
                            istrSurvivorJPop50BenefitAmt = AppendDoller(ldecPartJPop50BenAmt / 2);
                    }

                    if (lstrBenOpValue == busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY)
                    {
                        if (lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.relative_value.IsNotNullOrEmpty())
                            istrJS75RelativeValue = lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.relative_value;


                        decimal ldecPartJS75BenAmt = decimal.Zero;

                        //Ticket - 76533
                        if (aintPlanId != busConstant.MPIPP_PLAN_ID && aintPlanId != busConstant.IAP_PLAN_ID)
                        {
                            ldecPartJS75BenAmt = lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.disability_amount;
                        }
                        else
                        {
                            ldecPartJS75BenAmt = lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.participant_amount;
                        }


                        if (ldecPartJS75BenAmt > 0)
                        {
                            //if (aintPlanId == 3 || aintPlanId == 7)
                            //    istrParticipantJSurvivor75BenefitAmt = AppendDoller(Math.Ceiling(lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.participant_amount));

                            //else if (aintPlanId == 4 || aintPlanId == 6)
                            //    istrParticipantJSurvivor75BenefitAmt = AppendDoller(Math.Ceiling(lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.participant_amount / 0.5M) * 0.5M);

                            //else
                            istrParticipantJSurvivor75BenefitAmt = AppendDoller(ldecPartJS75BenAmt);
                        }

                        decimal ldecSurvivorJS75BenAmt = lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.survivor_amount;

                        if (ldecSurvivorJS75BenAmt > 0)
                            istrSurvivorJSurvivor75BenefitAmt = AppendDoller(ldecPartJS75BenAmt * 3/4);
                    }

                    if (lstrBenOpValue == busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY)
                    {
                        if (lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.relative_value.IsNotNullOrEmpty())
                            istrJS100RelativeValue = lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.relative_value;
                        idecParticipantJSurvivor100BenefitAmt = decimal.Zero;

                        //Ticket - 76533
                        if (aintPlanId != busConstant.MPIPP_PLAN_ID && aintPlanId != busConstant.IAP_PLAN_ID)
                        {
                            idecParticipantJSurvivor100BenefitAmt = lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.disability_amount;
                        }
                        else
                        {
                            idecParticipantJSurvivor100BenefitAmt = lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.participant_amount;
                        }

                        if (idecParticipantJSurvivor100BenefitAmt > 0)
                            istrParticipantJSurvivor100BenefitAmt = AppendDoller(idecParticipantJSurvivor100BenefitAmt);

                        if (istrParticipantLifeAnnuityBenefitAmt.IsNotNullOrEmpty())
                            istrPartAnnuityJPop100BenefitAmt = istrParticipantLifeAnnuityBenefitAmt;

                        decimal ldecSurvivorJS100BenAmt = lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.survivor_amount;

                        if (ldecSurvivorJS100BenAmt > 0)
                            istrSurvivorJSurvivor100BenefitAmt = AppendDoller(idecParticipantJSurvivor100BenefitAmt);
                    }

                    if (lstrBenOpValue == busConstant.JOINT_100_PERCENT_POPUP_ANNUITY)
                    {
                        if (lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.relative_value.IsNotNullOrEmpty())
                            istrJS100PopRelativeValue = lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.relative_value;
                        decimal ldecParJPopup100BenAmt = decimal.Zero;

                        //Ticket - 76533
                        if (aintPlanId != busConstant.MPIPP_PLAN_ID && aintPlanId != busConstant.IAP_PLAN_ID)
                        {
                            ldecParJPopup100BenAmt = lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.disability_amount;
                        }
                        else
                        {
                            ldecParJPopup100BenAmt = lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.participant_amount;
                        }

                        if (ldecParJPopup100BenAmt > 0)
                            istrParticipantJPopup100BenefitAmt = AppendDoller(ldecParJPopup100BenAmt);


                        decimal ldecSurvivorJPopup100BenAmt = lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.survivor_amount;

                        if (ldecSurvivorJPopup100BenAmt > 0)
                            istrSurvivorJPopup100BenefitAmt = AppendDoller(ldecParJPopup100BenAmt);
                    }

                    if (lstrBenOpValue == busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY)
                    {
                        if (lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.relative_value.IsNotNullOrEmpty())
                            istrTenCertainRelativeValue = lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.relative_value;

                        idecParticipantTenYearBenefitAmt = decimal.Zero;

                        //Ticket - 76533
                        if (aintPlanId != busConstant.MPIPP_PLAN_ID && aintPlanId != busConstant.IAP_PLAN_ID)
                        {
                            idecParticipantTenYearBenefitAmt = lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.disability_amount;
                        }
                        else
                        {
                            idecParticipantTenYearBenefitAmt = lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.participant_amount;
                        }

                        if (idecParticipantTenYearBenefitAmt > 0)
                            istrParticipantTenYearBenefitAmt = AppendDoller(idecParticipantTenYearBenefitAmt);

                        if (idecParticipantTenYearBenefitAmt > 0 && ibusBenefitApplication.ibusPerson.icdoPerson.istrMaritalStatus != "Single") //rid 78872
                            istrSingleParticipantTenYearBenefitAmt = AppendDoller(idecParticipantTenYearBenefitAmt);
                    }
                    if (lstrBenOpValue == busConstant.TWO_YEARS_CERTAIN_AND_LIFE_ANNUITY)
                    {
                        if (lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.relative_value.IsNotNullOrEmpty())
                            istrTwoCertainRelativeValue = lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.relative_value;
                        idecParticipantTwoYearBenefitAmt = lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.participant_amount;

                        //Ticket - 76533
                        if (aintPlanId != busConstant.MPIPP_PLAN_ID && aintPlanId != busConstant.IAP_PLAN_ID)
                        {
                            idecParticipantTwoYearBenefitAmt = lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.disability_amount;
                        }
                        else
                        {
                            idecParticipantTwoYearBenefitAmt = lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.participant_amount;
                        }

                        istrParticipantTwoYearBenefitAmt = AppendDoller(idecParticipantTwoYearBenefitAmt);
                    }
                    if (lstrBenOpValue == busConstant.JOINT_66_2by3_PERCENT_SURVIVOR_ANNUITY)
                    {
                        istrJS66_2by3RelativeValue = lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.relative_value;
                        istrParticipantJSurvivor66_2by3BenefitAmt = AppendDoller(lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.participant_amount);
                        istrSurvivorJSurvivor66_2by3BenefitAmt = AppendDoller(lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.survivor_amount);
                    }

                    if (lstrBenOpValue == busConstant.LEVEL_INCOME)
                    {
                        istrLevelIncome = lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.relative_value;
                        idecParticipantJLevelIncome = lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.participant_amount;
                        istrParticipantJLevelIncome = AppendDoller(idecParticipantJLevelIncome);
                    }
                    if (lstrBenOpValue == busConstant.THREE_YEARS_CERTAIN_AND_LIFE_ANNUITY)
                    {
                        idecParticipant3YearBenefitAmt = lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.participant_amount;

                        //Ticket - 76533
                        if (aintPlanId != busConstant.MPIPP_PLAN_ID && aintPlanId != busConstant.IAP_PLAN_ID)
                        {
                            idecParticipant3YearBenefitAmt = lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.disability_amount;
                        }
                        else
                        {
                            idecParticipant3YearBenefitAmt = lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.participant_amount;
                        }

                        istrParticipant3YearBenefitAmt = AppendDoller(idecParticipant3YearBenefitAmt);
                    }

                    if (lstrBenOpValue == busConstant.LUMP_SUM)
                    {
                        if (lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.uvhp_flag == busConstant.FLAG_YES)
                        {
                            idecParticipantUVHP = lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.participant_amount;
                            istrParticipantUVHP = AppendDoller(idecParticipantUVHP);
                        }
                        else if (lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.uvhp_flag != busConstant.FLAG_YES &&
                            lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.ee_flag != busConstant.FLAG_YES)
                        {
                            idecParticipantLumpSumBenefitAmount = lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.participant_amount;
                            istrParticipantLumpSumBenefitAmount = AppendDoller(idecParticipantLumpSumBenefitAmount);
                        }
                    }
                    if (lstrBenOpValue == busConstant.FIVE_YEARS_CERTAIN_AND_LIFE_ANNUTIY)
                    {
                        istrFiveYearRelativeValue = lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.relative_value;
                        idecParticipantFiveYearBenefitAmt = lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.participant_amount;


                        //Ticket - 76533
                        if (aintPlanId != busConstant.MPIPP_PLAN_ID && aintPlanId != busConstant.IAP_PLAN_ID)
                        {
                            idecParticipantFiveYearBenefitAmt = lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.disability_amount;
                        }
                        else
                        {
                            idecParticipantFiveYearBenefitAmt = lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.participant_amount;
                        }

                        istrParticipantFiveYearBenefitAmt = AppendDoller(idecParticipantFiveYearBenefitAmt);
                    }

                }
            }

        }

        public void PopulateInitialDataBenefitCalculationHeaderForRemployment(int aintPersonId, int aintBenefitApplicationId, int aintBeneficiaryPersonId, string astrBenefitTypeValue, string astrCalculationType,
                                                        DateTime adtRetirementDate, decimal adecAge, int aintPlanId, int aintorgid = 0, decimal adecSurvPercent = 100, bool ablnReEvaluationMDBatch = false)
        {
            this.icdoBenefitCalculationHeader.person_id = aintPersonId;
            if (aintBenefitApplicationId.IsNotNull() && aintBenefitApplicationId != 0)
                this.icdoBenefitCalculationHeader.benefit_application_id = aintBenefitApplicationId;

            if (aintBeneficiaryPersonId.IsNotNull() && aintBeneficiaryPersonId != 0)
            {
                busPerson lbusPerson = new busPerson();
                if (lbusPerson.FindPerson(aintBeneficiaryPersonId))
                {
                    this.icdoBenefitCalculationHeader.beneficiary_person_id = lbusPerson.icdoPerson.person_id;
                    this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth = lbusPerson.icdoPerson.idtDateofBirth;
                    this.icdoBenefitCalculationHeader.beneficiary_person_name = lbusPerson.icdoPerson.istrFullName;
                    this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement = busGlobalFunctions.CalculatePersonAgeInDec(this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth, this.ibusBenefitApplication.icdoBenefitApplication.retirement_date);
                }
            }
            if (aintorgid.IsNotNull() && aintorgid != 0)
            {
                busOrganization lbusOrganization = new busOrganization();
                if (lbusOrganization.FindOrganization(aintorgid))
                {
                    this.icdoBenefitCalculationHeader.istrSurvivorTypeValue = busConstant.SURVIVOR_TYPE_ORGN;
                    this.icdoBenefitCalculationHeader.organization_id = lbusOrganization.icdoOrganization.org_id;
                }

            }

            this.icdoBenefitCalculationHeader.retirement_date = adtRetirementDate;
            this.icdoBenefitCalculationHeader.calculation_type_id = busConstant.BenefitCalculation.CALCULATION_TYPE_CODE_ID;
            this.icdoBenefitCalculationHeader.calculation_type_value = astrCalculationType;
            this.icdoBenefitCalculationHeader.benefit_type_id = busConstant.BENEFIT_TYPE_CODE_ID;
            this.icdoBenefitCalculationHeader.benefit_type_value = astrBenefitTypeValue;
            this.icdoBenefitCalculationHeader.status_id = busConstant.BenefitCalculation.CALCULATION_STATUS_TYPE_CODE_ID;
            this.icdoBenefitCalculationHeader.status_value = busConstant.BenefitCalculation.CALCULATION_STATUS_TYPE_APPROVED;
            this.icdoBenefitCalculationHeader.PopulateDescriptions();
            this.icdoBenefitCalculationHeader.iintPlanId = aintPlanId;

        }


        public void PopulateInitialDataBenefitCalculationHeader(int aintPersonId, int aintBenefitApplicationId, int aintBeneficiaryPersonId, string astrBenefitTypeValue, string astrCalculationType,
                                                                DateTime adtRetirementDate, decimal adecAge, int aintPlanId, int aintorgid = 0, decimal adecSurvPercent = 100, bool ablnReEvaluationMDBatch = false)
        {
            this.icdoBenefitCalculationHeader.person_id = aintPersonId;
            if (aintBenefitApplicationId.IsNotNull() && aintBenefitApplicationId != 0)
                this.icdoBenefitCalculationHeader.benefit_application_id = aintBenefitApplicationId;

            //Prod PIR 762
            if (this.ibusBenefitApplication == null)
            {
                this.ibusBenefitApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
                this.ibusBenefitApplication.FindBenefitApplication(aintBenefitApplicationId);
            }
            //Ticket #100420
            this.icdoBenefitCalculationHeader.dro_application_id = this.ibusBenefitApplication.icdoBenefitApplication.dro_application_id;

            if (aintBeneficiaryPersonId.IsNotNull() && aintBeneficiaryPersonId != 0)
            {
                busPerson lbusPerson = new busPerson();
                if (lbusPerson.FindPerson(aintBeneficiaryPersonId))
                {
                    if (astrBenefitTypeValue == busConstant.BENEFIT_TYPE_DEATH_PRE_RETIREMENT)
                    {
                        this.icdoBenefitCalculationHeader.istrSurvivorTypeValue = busConstant.SURVIVOR_TYPE_PER;
                    }
                    this.icdoBenefitCalculationHeader.beneficiary_person_id = lbusPerson.icdoPerson.person_id;
                    this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth = lbusPerson.icdoPerson.idtDateofBirth;
                    this.icdoBenefitCalculationHeader.beneficiary_person_name = lbusPerson.icdoPerson.istrFullName;
                    this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement = busGlobalFunctions.CalculatePersonAgeInDec(this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth, this.ibusBenefitApplication.icdoBenefitApplication.retirement_date);

                    if (astrBenefitTypeValue == busConstant.BENEFIT_TYPE_WITHDRAWAL)
                    {
                        this.icdoBenefitCalculationHeader.age = busGlobalFunctions.CalculatePersonAgeInDec(lbusPerson.icdoPerson.idtDateofBirth, this.ibusBenefitApplication.icdoBenefitApplication.withdrawal_date);
                        this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement = busGlobalFunctions.CalculatePersonAgeInDec(this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth, this.ibusBenefitApplication.icdoBenefitApplication.withdrawal_date);
                    }
                }
            }
            if (aintorgid.IsNotNull() && aintorgid != 0)
            {
                busOrganization lbusOrganization = new busOrganization();
                if (lbusOrganization.FindOrganization(aintorgid))
                {
                    this.icdoBenefitCalculationHeader.istrSurvivorTypeValue = busConstant.SURVIVOR_TYPE_ORGN;
                    this.icdoBenefitCalculationHeader.organization_id = lbusOrganization.icdoOrganization.org_id;
                }

            }
            if (astrBenefitTypeValue == busConstant.BENEFIT_TYPE_DEATH_PRE_RETIREMENT)
            {
                this.icdoBenefitCalculationHeader.date_of_death = this.ibusPerson.icdoPerson.date_of_death;
                this.icdoBenefitCalculationHeader.survivor_percentage = adecSurvPercent;
            }
            if (adtRetirementDate.IsNotNull() && adtRetirementDate != DateTime.MinValue)
                if (astrBenefitTypeValue == busConstant.BENEFIT_TYPE_RETIREMENT || astrBenefitTypeValue == busConstant.BENEFIT_TYPE_WITHDRAWAL ||
                    astrBenefitTypeValue == busConstant.BENEFIT_TYPE_DISABILITY)
                {
                    this.icdoBenefitCalculationHeader.retirement_date = adtRetirementDate;
                }


            if (adecAge.IsNotNull() && (adecAge != busConstant.ZERO_DECIMAL || adecAge != busConstant.ZERO_INT))
                this.icdoBenefitCalculationHeader.age = adecAge;

            this.icdoBenefitCalculationHeader.calculation_type_id = busConstant.BenefitCalculation.CALCULATION_TYPE_CODE_ID;
            this.icdoBenefitCalculationHeader.calculation_type_value = astrCalculationType;
            this.icdoBenefitCalculationHeader.benefit_type_id = busConstant.BENEFIT_TYPE_CODE_ID;
            this.icdoBenefitCalculationHeader.benefit_type_value = astrBenefitTypeValue;
            this.icdoBenefitCalculationHeader.status_id = busConstant.BenefitCalculation.CALCULATION_STATUS_TYPE_CODE_ID;
            #region Prod PIR 619
            //Commenting
            //if (ablnReEvaluationMDBatch)
            //{
            //    this.icdoBenefitCalculationHeader.status_value = busConstant.BenefitCalculation.CALCULATION_STATUS_TYPE_APPROVED;
            //}
            //else
            //{
            //    this.icdoBenefitCalculationHeader.status_value = busConstant.BenefitCalculation.CALCULATION_STATUS_TYPE_PENDING;
            //}
            //New Code
            this.icdoBenefitCalculationHeader.status_value = busConstant.BenefitCalculation.CALCULATION_STATUS_TYPE_PENDING;
            #endregion

            this.icdoBenefitCalculationHeader.PopulateDescriptions();
            this.icdoBenefitCalculationHeader.iintPlanId = aintPlanId;

        }

        //METHOD PopulateInitialDataBenefitCalculationDetails ONLY MEANT FOR FINAL CALCULATIONS -- DO NOT USE IN ESTIMATE 
        public void PopulateInitialDataBenefitCalculationDetails(int aintBenefitApplicationDetailId, int aintPersonAccountId, int aintPlanId, string astrPlanCode, DateTime adtVestedDate, string astrBenefitSubTypeValue
          , DateTime? adtRetirementDate = null, string astrDROModelValue = null)
        {
            switch (astrBenefitSubTypeValue)
            {
                case busConstant.CodeValueAll:
                // Create a collection of benefit calc detail objects
                default:
                    busBenefitCalculationDetail lbusBenefitCalculationDetail = new busBenefitCalculationDetail() { icdoBenefitCalculationDetail = new cdoBenefitCalculationDetail() };
                    lbusBenefitCalculationDetail.iclbBenefitCalculationOptions = new Collection<busBenefitCalculationOptions>();
                    lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail = new Collection<busBenefitCalculationYearlyDetail>();

                    //lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.benefit_calculation_header_id = aintBenefitCalculationHeaderId;
                    lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.benefit_application_detail_id = aintBenefitApplicationDetailId;
                    lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.person_account_id = aintPersonAccountId;
                    lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.plan_id = aintPlanId;
                    lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.istrPlanCode = astrPlanCode;
                    lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.istrPlanDescription = astrPlanCode;
                    lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.dro_model_id = busConstant.DRO_MODEL_ID;
                    lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.dro_model_value = astrDROModelValue;

                    lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.vested_date = adtVestedDate;
                    lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.benefit_subtype_id = busConstant.RETIREMENT_TYPE_CODE_ID;
                    lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.benefit_subtype_value = astrBenefitSubTypeValue;
                    lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.status_id = busConstant.BenefitCalculation.CALCULATION_STATUS_TYPE_CODE_ID;
                    lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.status_value = busConstant.BenefitCalculation.CALCULATION_STATUS_TYPE_PENDING;
                    lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.retirement_date = Convert.ToDateTime(adtRetirementDate);

                    if (this.iblnCalcualteUVHPBenefit)
                    {
                        lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.uvhp_flag = busConstant.FLAG_YES;
                        if (this.icdoBenefitCalculationHeader.benefit_type_value == busConstant.BENEFIT_TYPE_DEATH_PRE_RETIREMENT)
                        {
                            lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.ee_flag = busConstant.FLAG_YES;
                        }
                    }
                    if (this.iblnCalculateL52SplAccBenefit)
                    {
                        lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.l52_spl_acc_flag = busConstant.FLAG_YES;
                    }
                    if (this.iblnCalculateL161SplAccBenefit)
                    {
                        lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.l161_spl_acc_flag = busConstant.FLAG_YES;
                    }
                    if (this.iblnCalcualteNonVestedEEBenefit)
                    {
                        lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.ee_flag = busConstant.FLAG_YES;
                    }

                    // Add the detailed business object into the collection
                    this.iclbBenefitCalculationDetail.Add(lbusBenefitCalculationDetail);
                    break;
            }
        }

        #endregion

        #region Public Methods

        public override void LoadBenefitCalculationDetails()
        {
            base.LoadBenefitCalculationDetails();


            if (this.ibusPerson == null)
            {
                this.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
                this.ibusPerson.FindPerson(this.icdoBenefitCalculationHeader.person_id);
                if (this.ibusPerson.iclbPersonAccount == null || this.ibusPerson.iclbPersonAccount.Count == 0)
                    this.ibusPerson.LoadPersonAccounts();
            }

            foreach (busBenefitCalculationDetail lbusBenefitCalculationDetail in this.iclbBenefitCalculationDetail)
            {
                if (this.icdoBenefitCalculationHeader.dro_application_id > 0)
                {
                    if (this.ibusQdroApplication.ibusParticipant != null && this.ibusQdroApplication.ibusParticipant.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.plan_id).Count() > 0)
                        lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.istrPlanCode = this.ibusQdroApplication.ibusParticipant.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.plan_id).First().icdoPersonAccount.istrPlanCode;
                }
                else
                {
                    lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.istrPlanCode = this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.plan_id).First().icdoPersonAccount.istrPlanCode;
                }

                if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.istrPlanCode == busConstant.IAP)
                {
                    if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.iap_as_of_date != DateTime.MinValue)
                    {
                        lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.iintIAPasYear = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.iap_as_of_date.Year;
                    }
                    #region IAP
                    if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES)
                    {
                        lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.istrPlanDescription = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.istrPlanCode + "(L52 SPL ACC)";
                        lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.idecRemainingBenefits = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.local52_special_acct_bal_amount -
                                lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.qdro_offset;
                    }
                    else if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES)
                    {
                        lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.istrPlanDescription = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.istrPlanCode + "(L161 SPL ACC)";
                        lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.idecRemainingBenefits = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.local161_special_acct_bal_amount -
                                lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.qdro_offset;
                    }
                    else
                    {
                        lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.istrPlanDescription = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.istrPlanCode;
                        lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.idecRemainingBenefits = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.iap_balance_amount -
                                lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.qdro_offset;
                    }
                    #endregion
                }
                else if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.istrPlanCode == busConstant.MPIPP)
                {
                    #region MPIPP
                    if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.uvhp_flag == busConstant.FLAG_YES && lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.ee_flag == busConstant.FLAG_YES)
                    {
                        lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.istrPlanDescription = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.istrPlanCode + "(UV&HP/EE)";
                    }
                    else if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.uvhp_flag == busConstant.FLAG_YES)
                    {
                        lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.istrPlanDescription = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.istrPlanCode + "(UV&HP)";
                        lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.idecRemainingBenefits = (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.total_uvhp_contribution_amount +
                            lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.total_uvhp_interest_amount) - lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.qdro_offset;
                    }
                    else if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.ee_flag == busConstant.FLAG_YES)
                    {
                        lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.istrPlanDescription = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.istrPlanCode + "(EE)";

                        lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.idecRemainingBenefits = (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.non_vested_ee_amount +
                           lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.non_vested_ee_interest) - lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.qdro_offset;
                    }
                    else
                    {
                        lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.istrPlanDescription = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.istrPlanCode;
                    }
                    #endregion
                }
                else
                {
                    lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.istrPlanDescription = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.istrPlanCode;
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


                lbusBenefitCalculationDetail.LoadBenefitCalculationOptionss();
                lbusBenefitCalculationDetail.LoadBenefitCalculationYearlyDetails();
                lbusBenefitCalculationDetail.LoadBenefitCalculationYearlyDetailsTotal(ldtForfietureDate);

                foreach (busBenefitCalculationYearlyDetail lbusBenefitCalculationYearlyDetail in lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail)
                {
                    lbusBenefitCalculationYearlyDetail.LoadBenefitCalculationNonsuspendibleDetails();
                }
                if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT ||
                    this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL)
                {
                    if (!lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.IsNullOrEmpty())
                    {
                        this.icdoBenefitCalculationHeader.idecOverriddenBenefitAmount = lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.FirstOrDefault().icdoBenefitCalculationOptions.overridden_benefit_amount;
                    }
                }
                lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.iintPersonId = this.icdoBenefitCalculationHeader.person_id;
            }

            //10 Percent
            if ((this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT ||
                    this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL)
                    && iclbBenefitCalculationDetail != null && iclbBenefitCalculationDetail.Count() > 0 && iclbBenefitCalculationDetail.FirstOrDefault().icdoBenefitCalculationDetail.benefit_calculation_detail_id > 0)
            {
                DataTable ldtbPayeeAccountId = busBase.Select("cdoBenefitCalculationHeader.GetPayeeAccountIdFromCalcId", new object[2]
                { icdoBenefitCalculationHeader.benefit_type_value,iclbBenefitCalculationDetail.FirstOrDefault().icdoBenefitCalculationDetail.benefit_calculation_detail_id });

                if (ldtbPayeeAccountId != null && ldtbPayeeAccountId.Rows.Count > 0 && Convert.ToString(ldtbPayeeAccountId.Rows[0][0]).IsNotNullOrEmpty())
                {
                    icdoBenefitCalculationHeader.iintPayeeAccountId = Convert.ToInt32(ldtbPayeeAccountId.Rows[0][0]);
                }
            }


        }

        //This Method can also be called in the same way when it gets called from the Benefit Application Screen
        public void LoadRetirementContributionsbyAccountId(int aintPersonAccountId)
        {
            DataTable ldtbList = busBase.Select("cdoPersonAccountRetirementContribution.GetRetirementContributionbyAccountId", new object[1] { aintPersonAccountId });
            iclbPersonAccountRetirementContribution = GetCollection<busPersonAccountRetirementContribution>(ldtbList, "icdoPersonAccountRetirementContribution");
        }

        public void LoadAllRetirementContributions(busPersonAccount abusPersonAccount)
        {
            DataTable ldtbList = busBase.Select("cdoPersonAccountRetirementContribution.GetRetirementContributionsofPerson", new object[1] { this.ibusPerson.icdoPerson.person_id });
            iclbPersonAccountRetirementContribution = GetCollection<busPersonAccountRetirementContribution>(ldtbList, "icdoPersonAccountRetirementContribution");

            LoadVestedNonVestedEE(iclbPersonAccountRetirementContribution, this.ibusPerson.icdoPerson.person_id, abusPersonAccount);
        }

        public Collection<busPersonAccountRetirementContribution> LoadAllRetirementContributions(int aintPersonId, busPersonAccount abusPersonAccount)
        {
            DataTable ldtbList = Select("cdoPersonAccountRetirementContribution.GetRetirementContributionsofPerson",
                                                        new object[1] { aintPersonId });
            Collection<busPersonAccountRetirementContribution> lclbPersonAccountRetirementContribution =
                        GetCollection<busPersonAccountRetirementContribution>(ldtbList, "icdoPersonAccountRetirementContribution");


            LoadVestedNonVestedEE(lclbPersonAccountRetirementContribution, aintPersonId, abusPersonAccount);

            return lclbPersonAccountRetirementContribution;
        }

        public void LoadVestedNonVestedEE(Collection<busPersonAccountRetirementContribution> aclbPersonAccountRetirementContribution, int aintPersonId, busPersonAccount abusPersonAccount)
        {

            #region Get Vested and Non vested EE

            decimal ldecNonVestedEE = 0, ldecVestedEE = 0, ldecNonVestedEEInterest = 0, ldecVestedEEInterest = 0;

            if (aclbPersonAccountRetirementContribution != null && aclbPersonAccountRetirementContribution.Count > 0 && (!aclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.plan_id == busConstant.MPIPP_PLAN_ID &&
                                            item.icdoPersonAccountRetirementContribution.person_id == aintPersonId).IsNullOrEmpty()))
            {
                int lintPersonAccountID = aclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.plan_id == busConstant.MPIPP_PLAN_ID &&
                                                item.icdoPersonAccountRetirementContribution.person_id == aintPersonId).FirstOrDefault().icdoPersonAccountRetirementContribution.person_account_id;
                if (lintPersonAccountID > 0)
                {
                    //Kunal Arora : This code is no longer required we should directly look at the sub type value either Vested or Non Vested.
                    //If not working correctly subtype value is not set correctly.

                    //To Do : To be handled different for disability [cont & int only upto retdate,as it can be retro] & other
                    //busPersonAccountEligibility lbusPersonAccountEligibility = new busPersonAccountEligibility();
                    //lbusPersonAccountEligibility = lbusPersonAccountEligibility = lbusPersonAccountEligibility.LoadPersonAccEligibilityByPersonAccountId(lintPersonAccountID);

                    //if (lbusPersonAccountEligibility != null && lbusPersonAccountEligibility.icdoPersonAccountEligibility != null &&
                    //    lbusPersonAccountEligibility.icdoPersonAccountEligibility.forfeiture_date != DateTime.MinValue)
                    //{
                    //Sort collection in ascending Order
                    aclbPersonAccountRetirementContribution = aclbPersonAccountRetirementContribution.OrderBy(item => item.icdoPersonAccountRetirementContribution.effective_date).ToList().ToCollection();


                    ldecNonVestedEE = aclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountID &&
                                        item.icdoPersonAccountRetirementContribution.contribution_subtype_value == busConstant.CONTRIBUTION_SUBTYPE_NON_VESTED).
                                        Sum(item => item.icdoPersonAccountRetirementContribution.ee_contribution_amount);
                    ldecVestedEE = aclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountID &&
                                        item.icdoPersonAccountRetirementContribution.contribution_subtype_value == busConstant.CONTRIBUTION_SUBTYPE_VESTED).
                                                                        Sum(item => item.icdoPersonAccountRetirementContribution.ee_contribution_amount);

                    ldecNonVestedEEInterest = aclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountID &&
                                        item.icdoPersonAccountRetirementContribution.contribution_subtype_value == busConstant.CONTRIBUTION_SUBTYPE_NON_VESTED).
                                                                        Sum(item => item.icdoPersonAccountRetirementContribution.ee_int_amount);
                    ldecVestedEEInterest = aclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountID &&
                                        item.icdoPersonAccountRetirementContribution.contribution_subtype_value == busConstant.CONTRIBUTION_SUBTYPE_VESTED).
                                                                        Sum(item => item.icdoPersonAccountRetirementContribution.ee_int_amount);

                    if (abusPersonAccount != null)
                    {
                        abusPersonAccount.icdoPersonAccount.idecNonVestedEE = ldecNonVestedEE;
                        abusPersonAccount.icdoPersonAccount.idecVestedEE = ldecVestedEE;
                        abusPersonAccount.icdoPersonAccount.idecNonVestedEEInterest = ldecNonVestedEEInterest;
                        abusPersonAccount.icdoPersonAccount.idecVestedEEInterest = ldecVestedEEInterest;
                    }
                }
                //else
                //{
                //    ldecVestedEE = aclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountID).
                //                                                        Sum(item => item.icdoPersonAccountRetirementContribution.ee_contribution_amount);
                //    ldecVestedEEInterest = aclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountID).
                //                                                          Sum(item => item.icdoPersonAccountRetirementContribution.ee_int_amount);

                //    if (abusPersonAccount != null)
                //    {
                //        abusPersonAccount.icdoPersonAccount.idecVestedEE = ldecVestedEE;
                //        abusPersonAccount.icdoPersonAccount.idecVestedEEInterest = ldecVestedEEInterest;
                //        abusPersonAccount.icdoPersonAccount.idecNonVestedEE = 0;
                //        abusPersonAccount.icdoPersonAccount.idecNonVestedEEInterest = 0;
                //    }
                //}                  
                //}
            }
            #endregion

        }

        public decimal GetBenefitFactorLocal(string astrPlanCode, string astrBenefitOption, string astrBenefitAcountTypeValue, decimal aintMemberAge, decimal aintSpouseAge = busConstant.ZERO_DECIMAL)//This is Optional Parameter has been kept for a special reason
        {
            object ldecFactor;

            aintMemberAge = aintMemberAge.RoundToTwoDecimalPoints();
            aintSpouseAge = aintSpouseAge.RoundToTwoDecimalPoints();
            // Abhishek - Why are we checking the SpouseAge condition 3 times?
            // I think you must be wanting to check the MemberAge and SpouseAge right? Or just the SpouseAge?
            if (aintSpouseAge == null || aintSpouseAge == busConstant.ZERO_DECIMAL || aintSpouseAge == busConstant.ZERO_DECIMAL)
            {
                ldecFactor = DBFunction.DBExecuteScalar("cdoPersonAccountRetirementContribution.GetFactorBasedonMemberAge", new object[4] { astrPlanCode, astrBenefitOption, astrBenefitAcountTypeValue, aintMemberAge }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                if (ldecFactor.IsNotNull())
                    return (decimal)ldecFactor;
                else
                    return Decimal.One;
            }
            else
            {
                ldecFactor = DBFunction.DBExecuteScalar("cdoPersonAccountRetirementContribution.GetFactorBasedonMemberandSpouseAge", new object[5] { astrPlanCode, astrBenefitOption, astrBenefitAcountTypeValue, aintSpouseAge, aintMemberAge }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                if (ldecFactor.IsNotNull())
                    return (decimal)ldecFactor;
                else
                    return Decimal.One;
            }

        }


        public decimal GetLumpsumBenefitFactor(int aintMemberAge, int aintRetirementYear)
        {
            object lobjLumpsumBenefitFactor = null;
            lobjLumpsumBenefitFactor = DBFunction.DBExecuteScalar("cdoBenefitProvisionLumpsumFactor.GetLumpSumFactor", new object[2] { aintMemberAge, aintRetirementYear },
                                                                   iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            ////Ticket - 61531
            if (lobjLumpsumBenefitFactor.IsNotNull())
            {
                return (decimal)lobjLumpsumBenefitFactor;
            }

            return 1;
        }

        public ArrayList btn_CancelCalculation()
        {
            if (this.iarrErrors.IsNull())
                this.iarrErrors = new ArrayList();
            //PIR-843: Allow Cancel if all the records are Cancelled or Reclaimed status
            if (this.icdoBenefitCalculationHeader.benefit_calculation_header_id > 0)
            {
                utlError lobjError = null;
                object lobjCheckDistributionStatusValue = null;
                int lintCheckDistributionStatusValue = 0;
                lobjCheckDistributionStatusValue = DBFunction.DBExecuteScalar("cdoPaymentHistoryHeader.GetActiveDistributionRecord",
                           new object[1] { this.icdoBenefitCalculationHeader.benefit_calculation_header_id }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                if (lobjCheckDistributionStatusValue != null)
                {
                    lintCheckDistributionStatusValue = ((int)lobjCheckDistributionStatusValue);
                }
                if (lintCheckDistributionStatusValue > 0)
                {
                    lobjError = AddError(6102, "");
                    iarrErrors.Add(lobjError);
                    return iarrErrors;
                }
            }

            if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL && this.ibusBenefitApplication.IsNotNull() && this.icdoBenefitCalculationHeader.benefit_application_id.IsNotNull() && this.icdoBenefitCalculationHeader.benefit_application_id > 0)
            {

                //PROD PIR 792
                int iintPaymentCount = 0;
                if (this.icdoBenefitCalculationHeader.beneficiary_person_id == 0)
                    iintPaymentCount = (int)DBFunction.DBExecuteScalar("cdoPersonAccountRetirementContribution.GetPaymentCount", new object[3] { this.icdoBenefitCalculationHeader.person_id,
                    iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.benefit_calculation_header_id == this.icdoBenefitCalculationHeader.benefit_calculation_header_id).FirstOrDefault().icdoBenefitCalculationDetail.plan_id,
                this.iclbBenefitCalculationDetail.FirstOrDefault().icdoBenefitCalculationDetail.benefit_calculation_detail_id},
                                                         iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                else
                    iintPaymentCount = (int)DBFunction.DBExecuteScalar("cdoPersonAccountRetirementContribution.GetPaymentCount", new object[3] { this.icdoBenefitCalculationHeader.beneficiary_person_id,
                        iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.benefit_calculation_header_id == this.icdoBenefitCalculationHeader.benefit_calculation_header_id).FirstOrDefault().icdoBenefitCalculationDetail.plan_id,
                    this.iclbBenefitCalculationDetail.FirstOrDefault().icdoBenefitCalculationDetail.benefit_calculation_detail_id},
                                                         iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                if (iintPaymentCount > 0)
                {
                    utlError lobjError = new utlError();
                    lobjError = AddError(0, "Benefit calculation can not be cancel.");//R3view 
                    this.iarrErrors.Add(lobjError);
                    return iarrErrors;
                }
            }

            this.icdoBenefitCalculationHeader.status_value = busConstant.BenefitCalculation.CALCULATION_STATUS_TYPE_CANCELED;
            this.icdoBenefitCalculationHeader.Update();

            if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL && this.ibusBenefitApplication.IsNotNull() && this.icdoBenefitCalculationHeader.benefit_application_id.IsNotNull() && this.icdoBenefitCalculationHeader.benefit_application_id > 0)
            {
                if (this.ibusBenefitApplication.FindBenefitApplication(this.icdoBenefitCalculationHeader.benefit_application_id))
                {
                    this.ibusBenefitApplication.icdoBenefitApplication.final_calc_flag = busConstant.FLAG_NO;
                    this.ibusBenefitApplication.icdoBenefitApplication.Update();
                }

                foreach (busBenefitCalculationDetail lobjBenefitCalculationDetail in this.iclbBenefitCalculationDetail)
                {
                    this.LoadPayeeAccountStatusByCalculationDetailID(lobjBenefitCalculationDetail.icdoBenefitCalculationDetail.benefit_calculation_detail_id);
                    foreach (busPayeeAccountStatus lobjPayeeAccountStatus in this.iclbPayeeAccountStatusByApplication)
                    {
                        lobjPayeeAccountStatus.icdoPayeeAccountStatus.status_value = busConstant.PAYEE_ACCOUNT_STATUS_CANCELLED;
                        lobjPayeeAccountStatus.icdoPayeeAccountStatus.status_effective_date = DateTime.Now;
                        lobjPayeeAccountStatus.icdoPayeeAccountStatus.Insert();
                    }
                }

            }
            //ibusCalculation.NegateAllocationsCreatedByCalculation(this.icdoBenefitCalculationHeader.benefit_calculation_header_id);
            return iarrErrors;
        }

        public void LoadPayeeAccountStatusByCalculationDetailID(int aintCalculationDetailID)
        {
            DataTable ldtbList = busBase.Select("cdoPayeeAccount.GetPayeeAccountStatusByCalculationDetailID", new object[1] { aintCalculationDetailID });
            iclbPayeeAccountStatusByApplication = GetCollection<busPayeeAccountStatus>(ldtbList, "icdoPayeeAccountStatus");
        }

        public ArrayList btn_ApproveCalculation()
        {
            bool lblnFlag = false;
            if (this.iarrErrors.IsNull())
                this.iarrErrors = new ArrayList();
            DataTable ldtbExistingCalculation = new DataTable();

            //PIR 944
            if (icdoBenefitCalculationHeader.lump_sum_payment == busConstant.FLAG_YES && icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT)
            {
                if (iclbBenefitCalculationDetail != null && iclbBenefitCalculationDetail.Count() > 0
                    && (iclbBenefitCalculationDetail[0].iclbBenefitCalculationOptions == null || iclbBenefitCalculationDetail[0].iclbBenefitCalculationOptions.Count() == 0))
                {
                    busBenefitCalculationOptions lbusBenefitCalculationOptions =
                        new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                    lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.benefit_calculation_detail_id = iclbBenefitCalculationDetail[0].icdoBenefitCalculationDetail.benefit_calculation_detail_id;
                    lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.benefit_option_factor = 9;
                    lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.benefit_amount = iclbBenefitCalculationDetail[0].icdoBenefitCalculationDetail.unreduced_benefit_amount;
                    lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.disability_amount = iclbBenefitCalculationDetail[0].icdoBenefitCalculationDetail.unreduced_benefit_amount;
                    lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.istrBenefitOptionDescription = busConstant.LUMP_SUM_DESCRIPTION;
                    if (iclbBenefitCalculationDetail[0].iclbBenefitCalculationOptions == null)
                        iclbBenefitCalculationDetail[0].iclbBenefitCalculationOptions = new Collection<busBenefitCalculationOptions>();
                    iclbBenefitCalculationDetail[0].iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
                }
            }

            if (this.icdoBenefitCalculationHeader.benefit_type_value == busConstant.BENEFIT_TYPE_DEATH_POST_RETIREMENT || this.icdoBenefitCalculationHeader.benefit_type_value == busConstant.BENEFIT_TYPE_DEATH_PRE_RETIREMENT)
            {
                ldtbExistingCalculation = busBase.Select("cdoBenefitCalculationHeader.GetCalculationForPersonPlanTypeStatusForDDPT", new object[] { this.icdoBenefitCalculationHeader.person_id,this.icdoBenefitCalculationHeader.beneficiary_person_id, this.icdoBenefitCalculationHeader.iintPlanId,
                                                                this.icdoBenefitCalculationHeader.benefit_type_value, this.icdoBenefitCalculationHeader.calculation_type_value, busConstant.BenefitCalculation.CALCULATION_STATUS_TYPE_APPROVED });

                if (ldtbExistingCalculation != null && ldtbExistingCalculation.Rows.Count > 0)
                {
                    string lstrL52Flag = string.Empty;
                    string lstrL161Flag = string.Empty;
                    string lstrEEUVFlag = string.Empty;
                    bool lblnPrevCalcExist = false;

                    foreach (busBenefitCalculationDetail lbusBenefitCalc in this.iclbBenefitCalculationDetail)
                    {
                        foreach (DataRow ldtRow in ldtbExistingCalculation.Rows)
                        {
                            if (this.icdoBenefitCalculationHeader.iintPlanId == busConstant.IAP_PLAN_ID)
                            {
                                lstrL52Flag = Convert.ToString(ldtRow["l52_spl_acc_flag"]);
                                lstrL161Flag = Convert.ToString(ldtRow["l161_spl_acc_flag"]);
                                if (lstrL52Flag.IsNullOrEmpty())
                                {
                                    lstrL52Flag = busConstant.FLAG_NO;
                                }
                                if (lstrL161Flag.IsNullOrEmpty())
                                {
                                    lstrL161Flag = busConstant.FLAG_NO;
                                }
                                if (lbusBenefitCalc.icdoBenefitCalculationDetail.l52_spl_acc_flag != busConstant.FLAG_YES && lbusBenefitCalc.icdoBenefitCalculationDetail.l161_spl_acc_flag != busConstant.FLAG_YES)
                                {
                                    if (lstrL52Flag != busConstant.FLAG_YES && lstrL161Flag != busConstant.FLAG_YES)
                                    {
                                        lblnPrevCalcExist = true;
                                        //break;
                                    }
                                }
                                if (lbusBenefitCalc.icdoBenefitCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES && lbusBenefitCalc.icdoBenefitCalculationDetail.l161_spl_acc_flag != busConstant.FLAG_YES)
                                {
                                    if (lstrL52Flag == busConstant.FLAG_YES)
                                    {
                                        lblnPrevCalcExist = true;
                                        //break;
                                    }
                                }
                                if (lbusBenefitCalc.icdoBenefitCalculationDetail.l52_spl_acc_flag != busConstant.FLAG_YES && lbusBenefitCalc.icdoBenefitCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES)
                                {
                                    if (lstrL161Flag == busConstant.FLAG_YES)
                                    {
                                        lblnPrevCalcExist = true;
                                        //break;
                                    }
                                }
                            }
                            else if (this.icdoBenefitCalculationHeader.iintPlanId == busConstant.MPIPP_PLAN_ID)
                            {
                                lstrEEUVFlag = Convert.ToString(ldtRow[enmBenefitCalculationDetail.ee_flag.ToString()]);
                                if (string.IsNullOrEmpty(lstrEEUVFlag))
                                {
                                    lstrEEUVFlag = busConstant.FLAG_NO;
                                }
                                if (lbusBenefitCalc.icdoBenefitCalculationDetail.ee_flag == busConstant.FLAG_YES || lbusBenefitCalc.icdoBenefitCalculationDetail.uvhp_flag == busConstant.FLAG_YES)
                                {
                                    if (lstrEEUVFlag == busConstant.FLAG_YES)
                                    {
                                        lblnPrevCalcExist = true;
                                        //break;
                                    }
                                }
                                else if (lstrEEUVFlag == busConstant.FLAG_NO)
                                {
                                    lblnPrevCalcExist = true;
                                    //break;
                                }
                            }
                            else
                            {
                                lblnPrevCalcExist = true;
                                //break;
                            }

                            //PIR 853
                            busBenefitCalculationHeader lbusBenefitCalculationHeader = new busBenefitCalculationHeader { icdoBenefitCalculationHeader = new cdoBenefitCalculationHeader() };
                            if (lbusBenefitCalculationHeader.FindBenefitCalculationHeader(Convert.ToInt32(ldtRow[enmBenefitCalculationHeader.benefit_calculation_header_id.ToString().ToUpper()]))
                                && lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT)
                            {
                                lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.status_value = busConstant.BenefitCalculation.CALCULATION_STATUS_TYPE_CANCELED;
                                lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.Update();
                                lblnPrevCalcExist = false;
                            }
                        }
                        if (lblnPrevCalcExist)
                            break;
                    }

                    if (lblnPrevCalcExist)
                    {
                        utlError lobjError = AddError(5181, " ");
                        this.iarrErrors.Add(lobjError);
                        lblnFlag = true;
                    }
                }
            }
            else
            {
                ldtbExistingCalculation = busBase.Select("cdoBenefitCalculationHeader.GetCalculationForPersonPlanTypeStatus", new object[] { this.icdoBenefitCalculationHeader.person_id, this.icdoBenefitCalculationHeader.iintPlanId,
                                                                this.icdoBenefitCalculationHeader.benefit_type_value, this.icdoBenefitCalculationHeader.calculation_type_value, busConstant.BenefitCalculation.CALCULATION_STATUS_TYPE_APPROVED });
            }


            if (ldtbExistingCalculation != null && ldtbExistingCalculation.Rows.Count > 0 &&
                !(this.icdoBenefitCalculationHeader.benefit_type_value == busConstant.BENEFIT_TYPE_DEATH_POST_RETIREMENT || this.icdoBenefitCalculationHeader.benefit_type_value == busConstant.BENEFIT_TYPE_DEATH_PRE_RETIREMENT))
            {
                busBenefitApplication lbus = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
                lbus.FindBenefitApplication(this.icdoBenefitCalculationHeader.benefit_application_id);
                if (lbus.icdoBenefitApplication.converted_min_distribution_flag != busConstant.FLAG_YES
                    && Convert.ToString(ldtbExistingCalculation.Rows[0][enmBenefitCalculationHeader.benefit_calculation_header_id.ToString().ToUpper()]).IsNotNullOrEmpty())//10 Percent
                {

                    busBenefitCalculationHeader lbusBenefitCalculationHeader = new busBenefitCalculationHeader { icdoBenefitCalculationHeader = new cdoBenefitCalculationHeader() };
                    if (lbusBenefitCalculationHeader.FindBenefitCalculationHeader(Convert.ToInt32(ldtbExistingCalculation.Rows[0][enmBenefitCalculationHeader.benefit_calculation_header_id.ToString().ToUpper()]))
                        && lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT)
                    {
                        lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.status_value = busConstant.BenefitCalculation.CALCULATION_STATUS_TYPE_CANCELED;
                        lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.Update();
                    }

                    //utlError lobjError = AddError(5181, " ");
                    //this.iarrErrors.Add(lobjError);
                    //lblnFlag = true;
                }
            }

            // PIR-811
            bool lblnflagISIAP = false;
            string lstrDRO_Status = "";
            DataTable ldtlbQDROQlfdOrCanc = Select("cdoDroApplication.GetDroNotQLF&CancForIAP", new object[1] { this.ibusPerson.icdoPerson.person_id });

            busBenefitApplication lbusBenApp = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
            lbusBenApp.FindBenefitApplication(this.icdoBenefitCalculationHeader.benefit_application_id);

            //PIR 811
            //if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT)
            //{
            //    int lintQDROCalculationCount = 0;
            //    lintQDROCalculationCount = (int)DBFunction.DBExecuteScalar("cdoQdroCalculationHeader.GetQDROCalculationsPendingStatus", new object[1] { this.ibusPerson.icdoPerson.person_id },
            //                                                              iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework, iobjPassInfo.isrvMetaDataCache);
            //    if (lintQDROCalculationCount > 0)
            //    {
            //        utlError lobjError = new utlError();
            //        lobjError = AddError(5470, "");
            //        //lobjError.istrErrorMessage = "Please Approve the QDRO final calculation before approving this calculation";
            //        this.iarrErrors.Add(lobjError);
            //        lblnFlag = true;
            //    }
            //}
            if (this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).Count() > 0)
            {
                // PIR-811
                //int lintCount = 0;
                DataTable ldtlbQRDOOffset = Select("cdoQdroCalculationHeader.CheckIfDROApplicationorPendingFinalCalc", new object[2] { this.ibusPerson.icdoPerson.person_id, busConstant.IAP_PLAN_ID });
                DataTable ldtlbApprovedDROApplAndNoCalculation = Select("cdoQdroCalculationHeader.CheckIfApprovedDROApplAndNoCalculation", new object[2] { this.ibusPerson.icdoPerson.person_id, busConstant.IAP_PLAN_ID });

                //DataTable ldtlbQDROQlfdOrCanc = Select("cdoDroApplication.GetDroNotQLF&CancForIAP", new object[1] { this.ibusPerson.icdoPerson.person_id });
                //busBenefitApplication lbusBenApp = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
                //lbusBenApp.FindBenefitApplication(this.icdoBenefitCalculationHeader.benefit_application_id);

                //if (ldtlbQDROQlfdOrCanc.Rows.Count > 0 && Convert.ToString(ldtlbQDROQlfdOrCanc.Rows[0][0]).IsNotNullOrEmpty())               
                if (ldtlbQDROQlfdOrCanc != null && ldtlbQDROQlfdOrCanc.Rows.Count > 0)
                {
                    //lintCount = Convert.ToInt32(ldtlbQDROQlfdOrCanc.Rows[0][0]);
                    //if (lintCount > 0 && lbusBenApp.icdoBenefitApplication.child_support_flag != busConstant.FLAG_YES)//FOR PIR-531
                    //{
                    //    utlError lobjError = new utlError();
                    //    lobjError = AddError(5471, "");
                    //    lobjError.istrErrorMessage = "Please Qualify or Cancel QDRO Application for this Participant";
                    //    this.iarrErrors.Add(lobjError);
                    //    lblnFlag = true;
                    //}                  

                    //if (lbusBenApp.icdoBenefitApplication.child_support_flag != busConstant.FLAG_YES)
                    //{
                    foreach (DataRow dr in ldtlbQDROQlfdOrCanc.Rows)
                    {
                        if (dr.IsNull("PLAN_ID") && Convert.ToString(dr["DRO_STATUS_VALUE"]) == "RCVD")
                        {
                            lstrDRO_Status = Convert.ToString(dr["DRO_STATUS_VALUE"]);
                            break;
                        }
                        else if (!dr.IsNull("PLAN_ID") && this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == Convert.ToInt32(dr["PLAN_ID"])).Count() > 0)
                        {
                            if (Convert.ToInt32(dr["PLAN_ID"]) == busConstant.IAP_PLAN_ID)
                            {
                                lblnflagISIAP = true;
                            }
                            lstrDRO_Status = Convert.ToString(dr["DRO_STATUS_VALUE"]);
                            if (lblnflagISIAP && lstrDRO_Status == "RCVD")
                            {
                                break;
                            }
                        }
                    }
                    if (lstrDRO_Status == "RCVD")
                    {
                        //PIR 811
                        iblnIsDroStatusReceived = true;
                        //utlError lobjError = new utlError();
                        //lobjError = AddError(6230, "");
                        //this.iarrErrors.Add(lobjError);                       
                        //lblnFlag = true;                        
                    }
                    else if (lblnflagISIAP && lbusBenApp.icdoBenefitApplication.child_support_flag != busConstant.FLAG_YES && lbusBenApp.icdoBenefitApplication.emergency_onetime_payment_flag != busConstant.FLAG_YES && lbusBenApp.icdoBenefitApplication.withdrawal_type_value.IsNullOrEmpty() )//FOR PIR-531 //EmergencyOneTimePayment - 03/17/2020
                    {
                        utlError lobjError = new utlError();
                        lobjError = AddError(5471, "");
                        this.iarrErrors.Add(lobjError);
                        lblnFlag = true;
                    }
                }
                //}
                //PIR 811
                if (((ldtlbQRDOOffset.Rows.Count > 0 && Convert.ToInt32(ldtlbQRDOOffset.Rows[0][0]) > 0) ||
                    (ldtlbApprovedDROApplAndNoCalculation.Rows.Count > 0 && Convert.ToInt32(ldtlbApprovedDROApplAndNoCalculation.Rows[0][0]) > 0))
                    && lbusBenApp.icdoBenefitApplication.child_support_flag != busConstant.FLAG_YES && lbusBenApp.icdoBenefitApplication.emergency_onetime_payment_flag != busConstant.FLAG_YES && lbusBenApp.icdoBenefitApplication.withdrawal_type_value.IsNullOrEmpty() )//FOR PIR-531 //EmergnecyOneTimePayment - 03/17/2020
                {
                    utlError lobjError = new utlError();
                    lobjError = AddError(5470, "");
                    //lobjError.istrErrorMessage = "Please Approve the QDRO final calculation before approving this calculation";
                    this.iarrErrors.Add(lobjError);
                    lblnFlag = true;
                }
            }
            else  // PIR-811
            {
                if (ldtlbQDROQlfdOrCanc != null && ldtlbQDROQlfdOrCanc.Rows.Count > 0)
                {
                    //if (lbusBenApp.icdoBenefitApplication.child_support_flag != busConstant.FLAG_YES)
                    //{                    
                    foreach (DataRow dr in ldtlbQDROQlfdOrCanc.Rows)
                    {
                        if (dr.IsNull("PLAN_ID") && Convert.ToString(dr["DRO_STATUS_VALUE"]) == "RCVD")
                        {
                            lstrDRO_Status = Convert.ToString(dr["DRO_STATUS_VALUE"]);
                            break;
                        }
                        else if (!dr.IsNull("PLAN_ID") && this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == Convert.ToInt32(dr["PLAN_ID"])).Count() > 0)
                        {
                            lstrDRO_Status = Convert.ToString(dr["DRO_STATUS_VALUE"]);
                            if (lstrDRO_Status == "RCVD")
                            {
                                break;
                            }
                        }
                    }
                    if (lstrDRO_Status == "RCVD")
                    {
                        //PIR 811
                        iblnIsDroStatusReceived = true;
                        //utlError lobjError = new utlError();
                        //lobjError = AddError(6230, "");
                        //this.iarrErrors.Add(lobjError);
                        //lblnFlag = true;                        
                    }
                }
                //}
            }

            if (!lblnFlag)
            {
                this.icdoBenefitCalculationHeader.status_value = busConstant.BenefitCalculation.CALCULATION_STATUS_TYPE_APPROVED;
                this.icdoBenefitCalculationHeader.Update();
                this.EvaluateInitialLoadRules();
            }
            //PIR 811
            this.ValidateSoftErrors();
            return this.iarrErrors;
        }

        public cdoPerson GetSpouseDetails(string istrMPIPersonID)
        {
            cdoPerson lcdoPerson = new cdoPerson();
            if (!string.IsNullOrEmpty(istrMPIPersonID))
            {
                DataTable ldtDataTable = Select("cdoPerson.GetPersonDetails", new object[1] { istrMPIPersonID });
                if (ldtDataTable.Rows.Count > 0)
                {
                    lcdoPerson.LoadData(ldtDataTable.Rows[0]);
                }
            }
            return lcdoPerson;
        }

        public cdoPerson GetBeneficiaryDetails(string astrBeneficiaryMPID)
        {
            icdoSurvivorDetails = new cdoPerson();
            //cdoPerson lcdoPerson = new cdoPerson();
            if (!string.IsNullOrEmpty(astrBeneficiaryMPID))
            {
                DataTable ldtDataTable = Select("cdoPerson.GetBeneficiaryDetails", new object[2] { this.ibusPerson.icdoPerson.person_id, astrBeneficiaryMPID });
                if (ldtDataTable.Rows.Count > 0)
                {
                    icdoSurvivorDetails.LoadData(ldtDataTable.Rows[0]);
                    this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth = icdoSurvivorDetails.idtDateofBirth;

                    //PIR-810
                    if (!string.IsNullOrEmpty(Convert.ToString(ldtDataTable.Rows[0]["RELATIONSHIP_VALUE"])))
                        istrParticipantRelationshipWithBeneficiary = Convert.ToString(ldtDataTable.Rows[0]["RELATIONSHIP_VALUE"]);

                    icdoSurvivorDetails.idecSurvivorAgeAtDeath = busGlobalFunctions.CalculatePersonAge(this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth, this.icdoBenefitCalculationHeader.date_of_death);
                    if (this.icdoBenefitCalculationHeader.retirement_date != DateTime.MinValue)
                    {
                        icdoSurvivorDetails.idecAgeAtEarlyRetirement = busGlobalFunctions.CalculatePersonAge(this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth, this.icdoBenefitCalculationHeader.retirement_date);

                    }
                    if (this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth != DateTime.MinValue)
                    {
                        if (this.icdoBenefitCalculationHeader.modified_date != DateTime.MinValue)
                        {
                            DateTime ldtCalculationDate = this.icdoBenefitCalculationHeader.modified_date;
                            ldtCalculationDate = ldtCalculationDate.AddMonths(2);
                            ldtCalculationDate = busGlobalFunctions.GetLastDayofMonth(ldtCalculationDate).AddDays(1);
                            this.icdoSurvivorDetails.idecSurvivorsAgeAsOfCalculationDate = busGlobalFunctions.CalculatePersonAge(this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth, ldtCalculationDate);

                        }
                    }
                }
            }
            return icdoSurvivorDetails;

        }

        public cdoBenefitCalculationHeader GetRetirementDate(string adecMemberAge)
        {
            if (adecMemberAge.IsNotNullOrEmpty())
            {
                this.icdoBenefitCalculationHeader.retirement_date = this.ibusPerson.icdoPerson.idtDateofBirth.AddYears(Convert.ToInt32(Convert.ToDecimal(adecMemberAge)));
                if (this.icdoBenefitCalculationHeader.retirement_date.Day != 1)
                {
                    this.icdoBenefitCalculationHeader.retirement_date = busGlobalFunctions.GetFirstDayofMonth(this.icdoBenefitCalculationHeader.retirement_date);
                    this.icdoBenefitCalculationHeader.retirement_date = this.icdoBenefitCalculationHeader.retirement_date.AddMonths(1);
                }
            }
            return this.icdoBenefitCalculationHeader;
        }

        public cdoBenefitCalculationHeader GetAgeOnRetirementDate(string adtRetrDate)
        {
            if (adtRetrDate.IsNotNullOrEmpty())
            {
                this.icdoBenefitCalculationHeader.age = busGlobalFunctions.CalculatePersonAge(this.ibusPerson.icdoPerson.idtDateofBirth, Convert.ToDateTime(adtRetrDate));
            }
            return this.icdoBenefitCalculationHeader;
        }

        public virtual void LoadPlanBenefitsForPlan(int aintPlanId)
        {
            DataTable ldtbList = Select<cdoPlanBenefitXr>(
                new string[1] { enmPlanBenefitXr.plan_id.ToString() },
                new object[1] { aintPlanId }, null, null);
            iclbcdoPlanBenefit = GetCollection<busPlanBenefitXr>(ldtbList, "icdoPlanBenefitXr");
        }

        public void FillOptionsForCorrespondence(string astrTemplateName, bool ablnLocalCorrespondenceFromBatch = false)
        {
            SetDefaultValues();
            if (this.icdoBenefitCalculationHeader.benefit_type_value == busConstant.BENEFIT_TYPE_DISABILITY)
            {
                if (this.iclbBenefitCalculationDetail.Count > 0)
                {
                    Collection<busBenefitCalculationOptions> lclbBenefitCalculationOptions;
                    //Ticket #101090
                    if (astrTemplateName == busConstant.DISABILITY_BENEFIT_ELECTION_MPI_PACKAGE && this.sel_benefit_calculation_detail_id > 0)
                    {
                        busBenefitCalculationDetail lbusBenefitCalculationDetail = new busBenefitCalculationDetail { icdoBenefitCalculationDetail = new cdoBenefitCalculationDetail() };
                        lbusBenefitCalculationDetail.FindBenefitCalculationDetail(this.sel_benefit_calculation_detail_id);
                        lbusBenefitCalculationDetail.LoadBenefitCalculationOptionss();
                        lclbBenefitCalculationOptions = lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.ToList().ToCollection();
                    }
                    else
                    {
                        lclbBenefitCalculationOptions = this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id ==
                                this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().iclbBenefitCalculationOptions.ToList().ToCollection();
                    }

                    if (lclbBenefitCalculationOptions != null && lclbBenefitCalculationOptions.Count() > 0)
                    {
                        if (this.icdoBenefitCalculationHeader.iintPlanId == busConstant.IAP_PLAN_ID || this.icdoBenefitCalculationHeader.iintPlanId == busConstant.MPIPP_PLAN_ID)
                        {
                            if (this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault() != null)
                            {
                                LoadIAPPlanDisabilityPropertiesforCorrespondence(astrTemplateName);
                            }
                        }
                        if (this.icdoBenefitCalculationHeader.iintPlanId != busConstant.IAP_PLAN_ID)
                        {
                            LoadPlanDisabilityPropertiesforCorrespondence(lclbBenefitCalculationOptions, this.icdoBenefitCalculationHeader.iintPlanId, astrTemplateName);

                        }
                    }
                }
            }
            else
            {
                this.ibusBenefitApplication.istrMinimumDistributionDate =
                                    busGlobalFunctions.ConvertDateIntoDifFormat(icdoBenefitCalculationHeader.retirement_date);

                if (this.iclbBenefitCalculationDetail != null && this.iclbBenefitCalculationDetail.Count > 0)
                {
                    Collection<busBenefitCalculationOptions> lclbBenefitCalculationOptions = this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id ==
                            this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().iclbBenefitCalculationOptions.ToList().ToCollection();

                    if (lclbBenefitCalculationOptions != null && lclbBenefitCalculationOptions.Count() > 0)
                    {
                        if (this.icdoBenefitCalculationHeader.iintPlanId == busConstant.IAP_PLAN_ID || this.icdoBenefitCalculationHeader.iintPlanId == busConstant.MPIPP_PLAN_ID)
                        {
                            if (this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault() != null)
                            {
                                this.iintIAPAsOfDate = this.iclbBenefitCalculationDetail.Where(
                                    item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().icdoBenefitCalculationDetail.iap_as_of_date.Year;
                                LoadIAPPlanPropertiesforCorrespondence(astrTemplateName);
                            }
                        }
                        if (this.icdoBenefitCalculationHeader.iintPlanId != busConstant.IAP_PLAN_ID)
                        {
                            LoadPlanPropertiesforCorrespondence(lclbBenefitCalculationOptions, this.icdoBenefitCalculationHeader.iintPlanId, ablnLocalCorrespondenceFromBatch, astrTemplateName);

                        }
                    }
                }

            }
        }

        //CODE _ AARTI
        public void GetPriorDates()
        {
            if (this.icdoBenefitCalculationHeader.retirement_date != DateTime.MinValue)
            {
                DateTime ldtRetire = this.icdoBenefitCalculationHeader.retirement_date;
                DateTime ldtRtrDt = ldtRetire.AddMonths(-2);
                ldtRtrDt = ldtRtrDt.AddDays(-1);
                this.ibusBenefitApplication.istrSixtyDaysPriorDate = Convert.ToString(ldtRtrDt);
                this.ibusBenefitApplication.istrSixtyDaysPriorDate = busGlobalFunctions.ConvertDateIntoDifFormat(ldtRtrDt);

                DateTime ldtRetire1 = this.icdoBenefitCalculationHeader.retirement_date;
                DateTime ldtRtrDt1 = ldtRetire1.AddMonths(-1);
                DateTime ldtOneDayPriorRtmt = ldtRetire.AddDays(-1);
                ldtRtrDt1 = ldtRtrDt1.AddDays(-1);
                this.ibusBenefitApplication.istrThirtyDaysPriorDate = Convert.ToString(ldtRtrDt1);
                this.ibusBenefitApplication.istrThirtyDaysPriorDate = busGlobalFunctions.ConvertDateIntoDifFormat(ldtRtrDt1);

                this.ibusBenefitApplication.dtSixtyDaysPriorDate = ldtRetire.AddDays(-60);
                this.ibusBenefitApplication.dtThirtyDaysPriorDate = ldtRetire.AddDays(-30);

                this.ibusBenefitApplication.istrOneDayPriorRtDate = busGlobalFunctions.ConvertDateIntoDifFormat(ldtOneDayPriorRtmt);

            }

        }
        //Moved From busBenefitCalculationRetirement--Tushar
        public bool IsCancelledPayeeAccountDoesNotExsits()
        {
            //if (icdoBenefitCalculation.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability)
            //{
            //    if (ibusMember.iclbBenefitApplication.IsNull())
            //        ibusMember.LoadBenefitApplication();
            //    if (ibusMember.iclbPayeeAccount.IsNull())
            //        ibusMember.LoadPayeeAccount(true);
            //    var lenumEarlyRetApplication = ibusMember.iclbBenefitApplication.Where(lobjBA =>
            //        lobjBA.icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypeRetirement &&
            //        lobjBA.icdoBenefitApplication.benefit_sub_type_value == busConstant.ApplicationBenefitSubTypeEarly);

            //    // PROD PIR ID 5356
            //    if (lenumEarlyRetApplication.IsNotNull())
            //    {
            //        foreach (busBenefitApplication lobjApplication in lenumEarlyRetApplication)
            //        {
            //            if (ibusMember.iclbPayeeAccount.Where(lobjPA =>
            //                    lobjPA.icdoPayeeAccount.application_id == lobjApplication.icdoBenefitApplication.benefit_application_id &&
            //                    lobjPA.ibusPayeeAccountActiveStatus.IsStatusNotCancelled()).Any())
            //                return true;
            //        }
            //    }
            //}
            return false;
        }

        #endregion

        #region Overriden Methods

        #region Validate
        public override ArrayList ValidateNew(Hashtable ahstParam)
        {
            ArrayList larrErrors = new ArrayList();

            string astrMpiPersonID = Convert.ToString(ahstParam["astr_person_mpi_id"]); ;
            int aintPlanID = Convert.ToInt32(ahstParam["aint_plan_id"]);
            string astrBenefitTypeValue = Convert.ToString(ahstParam["astr_benefit_type"]);
            int aintbenefitapplicationid = 0;
            if (!string.IsNullOrEmpty(Convert.ToString(ahstParam["aint_benefit_application_id"])))
            {
                aintbenefitapplicationid = Convert.ToInt32(ahstParam["aint_benefit_application_id"]);
            }
            //string astrCalculationTypeValue = Convert.ToString(ahstParam["astr_calculation_type_value"]);
            if (astrBenefitTypeValue == busConstant.BENEFIT_TYPE_DEATH_POST_RETIREMENT)
            {
                utlError lobjError = null;
                lobjError = AddError(6208, " ");
                larrErrors.Add(lobjError);

            }

            if (astrMpiPersonID.IsNullOrEmpty() && astrBenefitTypeValue != busConstant.BENEFIT_TYPE_DISABILITY && astrBenefitTypeValue != busConstant.BENEFIT_TYPE_DEATH_POST_RETIREMENT)
            {
                utlError lobjError = null;
                lobjError = AddError(4075, "");
                larrErrors.Add(lobjError);
            }
            if (!astrMpiPersonID.IsNullOrEmpty())
            {
                if (iobjPassInfo.idictParams.ContainsKey("UserID"))
                {
                    string astrUserSerialID = iobjPassInfo.idictParams["UserID"].ToString();
                    DataTable ldtbParticipantId = busBase.Select("cdoPerson.GetVIPFlagInfo", new object[2] { astrUserSerialID, astrMpiPersonID });
                    if (ldtbParticipantId.Rows.Count > 0)
                    {
                        if (ldtbParticipantId.Rows[0]["istr_IS_LOGGED_IN_USER_VIP"].ToString() == "N" && ldtbParticipantId.Rows[0]["istrRelativeVipFlag"].ToString() == "Y")
                        {
                            utlError lobjError = null;
                            lobjError = AddError(6175, "");
                            larrErrors.Add(lobjError);
                            return larrErrors;
                        }
                    }

                }
            }
            if (aintbenefitapplicationid == 0 && astrBenefitTypeValue == busConstant.BENEFIT_TYPE_DISABILITY)
            {
                utlError lobjError = null;
                lobjError = AddError(5433, "");
                larrErrors.Add(lobjError);
            }

            if ((aintPlanID.IsNull()) || (aintPlanID == 0) && (astrBenefitTypeValue != busConstant.BENEFIT_TYPE_DEATH_POST_RETIREMENT))
            {
                utlError lobjError = null;
                lobjError = AddError(5408, "");
                larrErrors.Add(lobjError);
            }
            if (astrBenefitTypeValue.IsNullOrEmpty())
            {
                utlError lobjError = null;
                lobjError = AddError(5409, "");
                larrErrors.Add(lobjError);
            }

            if (astrBenefitTypeValue == busConstant.BENEFIT_TYPE_DISABILITY && aintbenefitapplicationid != 0)
            {
                object ldtApplicationStatus = DBFunction.DBExecuteScalar("cdoBenefitCalculationHeader.CheckIsApplicationCancelled", new object[2] { Convert.ToString(ahstParam["astr_benefit_type"]), aintbenefitapplicationid },
                                                            iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                if (Convert.ToInt32(ldtApplicationStatus) > 0)
                {
                    utlError lobjError = null;
                    lobjError = AddError(5432, "");
                    larrErrors.Add(lobjError);
                }
                if ((aintbenefitapplicationid.IsNotNull() || aintbenefitapplicationid != 0) && (aintPlanID.IsNotNull() && aintPlanID != 0))
                {
                    object ldtApplicationcount = DBFunction.DBExecuteScalar("cdoBenefitCalculationHeader.CheckIsApplicationIDValid", new object[1] { aintbenefitapplicationid },
                                                                            iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                    if (Convert.ToInt32(ldtApplicationcount) == 0)
                    {
                        utlError lobjError = null;
                        lobjError = AddError(5436, "");
                        larrErrors.Add(lobjError);
                    }
                    else
                    {
                        object ldtplancount = DBFunction.DBExecuteScalar("cdoBenefitCalculationHeader.CheckEnrolledPlanByApplID", new object[2] { aintbenefitapplicationid, aintPlanID },
                                                            iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                        if (Convert.ToInt32(ldtplancount) == 0)
                        {
                            utlError lobjError = null;
                            lobjError = AddError(5412, "");
                            larrErrors.Add(lobjError);
                        }
                    }
                }
            }

            if (astrBenefitTypeValue == busConstant.BENEFIT_TYPE_WITHDRAWAL)
            {
                if (aintPlanID > 2)
                {
                    utlError lobjError = null;
                    lobjError = AddError(5407, "");
                    larrErrors.Add(lobjError);
                }

                if (astrMpiPersonID.IsNotNullOrEmpty() && aintPlanID.IsNotNull() && aintPlanID == busConstant.MPIPP_PLAN_ID)
                {
                    DataTable ldtblCount = busBase.Select("cdoPersonAccountRetirementContribution.CheckEE_UVHP_AmpuntForWithdrawal", new object[2] { astrMpiPersonID, aintPlanID });

                    if (ldtblCount.Rows.Count > 0 && ((Convert.ToDecimal(ldtblCount.Rows[0][0]) > 0) || (Convert.ToDecimal(ldtblCount.Rows[0][1]) > 0)))
                    {

                    }
                    else
                    {
                        utlError lobjError = null;
                        lobjError = AddError(5421, "");
                        larrErrors.Add(lobjError);
                    }
                }

                if (astrMpiPersonID.IsNotNullOrEmpty() && aintPlanID.IsNotNull() && aintPlanID == busConstant.IAP_PLAN_ID)
                {
                    DataTable ldtblCount = busBase.Select("cdoPersonAccountRetirementContribution.CheckIAP_Special_AmountForWithdrawal", new object[2] { astrMpiPersonID, aintPlanID });
                    if (ldtblCount.Rows.Count > 0 && (Convert.ToDecimal(ldtblCount.Rows[0][0]) > 0 || Convert.ToDecimal(ldtblCount.Rows[0][1]) > 0 || Convert.ToDecimal(ldtblCount.Rows[0][2]) > 0))
                    {

                    }
                    else
                    {
                        utlError lobjError = null;
                        lobjError = AddError(5422, "");
                        larrErrors.Add(lobjError);
                    }
                }
            }

            //if (astrCalculationTypeValue == busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL)
            //{
            //    utlError lobjError = null;
            //    lobjError = AddError(5405, "");
            //    larrErrors.Add(lobjError);
            //}


            if (astrMpiPersonID.IsNotNullOrEmpty() && aintPlanID.IsNotNull() && aintPlanID != 0 && astrBenefitTypeValue != busConstant.BENEFIT_TYPE_DISABILITY)
            {
                object ldtplancount = DBFunction.DBExecuteScalar("cdoBenefitCalculationHeader.CheckEnrolledPlan", new object[2] { astrMpiPersonID, aintPlanID },
                                                                        iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                if (Convert.ToInt32(ldtplancount) == 0)
                {
                    utlError lobjError = null;
                    lobjError = AddError(5412, "");
                    larrErrors.Add(lobjError);
                }
            }



            if (astrMpiPersonID.IsNotNullOrEmpty() && aintPlanID.IsNotNull() && aintPlanID != 0)
            {
                bool lblnIsUVHPEEAmountPresent = busConstant.BOOL_FALSE;

                if (astrBenefitTypeValue == busConstant.BENEFIT_TYPE_WITHDRAWAL)
                {
                    DataTable ldtblCount = busBase.Select("cdoPersonAccountRetirementContribution.CheckEE_UVHP_AmpuntForWithdrawal", new object[2] { astrMpiPersonID, busConstant.MPIPP_PLAN_ID });

                    if (ldtblCount.Rows.Count > 0 && ((Convert.ToDecimal(ldtblCount.Rows[0][0]) > 0) || (Convert.ToDecimal(ldtblCount.Rows[0][1]) > 0)))
                    {
                        lblnIsUVHPEEAmountPresent = true;
                    }
                }

                //object ldtplancount = DBFunction.DBExecuteScalar("cdoBenefitCalculationHeader.CheckIsPlanForfieted", new object[2] { astrMpiPersonID, aintPlanID },
                //                                                        iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework, iobjPassInfo.isrvMetaDataCache);
                //if (Convert.ToInt32(ldtplancount) > 0 && !lblnIsUVHPEEAmountPresent)
                //{
                //    utlError lobjError = null;
                //    lobjError = AddError(5413, "");
                //    larrErrors.Add(lobjError);
                //}
            }
            return larrErrors;
        }

        public override void ValidateHardErrors(utlPageMode aenmPageMode)
        {

            base.ValidateHardErrors(aenmPageMode);
            utlError lobjError = null;
            if (this.iarrErrors.IsNull())
                this.iarrErrors = new ArrayList();
            if (this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth != DateTime.MinValue && this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth >= DateTime.Now)
            {
                lobjError = AddError(1131, " ");
                this.iarrErrors.Add(lobjError);
            }

            if (this.icdoBenefitCalculationHeader.benefit_type_value != busConstant.BENEFIT_TYPE_DEATH_PRE_RETIREMENT)
            {
                if (this.icdoBenefitCalculationHeader.iintPlanId != busConstant.MPIPP_PLAN_ID)
                {
                    if (this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).Count() > 0)
                    {
                        int lintPersonAccountId = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).First().icdoPersonAccount.person_account_id;
                        if (this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Count() == 0)
                        {
                            lobjError = AddError(0, "No Retirement Contribution found for the participant.");
                            this.iarrErrors.Add(lobjError);
                        }
                    }
                }
            }
            else
            {
                if (this.icdoBenefitCalculationHeader.iintPlanId == busConstant.LOCAL_600_PLAN_ID ||
                    this.icdoBenefitCalculationHeader.iintPlanId == busConstant.LOCAL_666_PLAN_ID)
                {
                    if (this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).Count() > 0)
                    {
                        int lintPersonAccountId = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).First().icdoPersonAccount.person_account_id;
                        if (this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Count() == 0)
                        {
                            lobjError = AddError(0, "No Retirement Contribution found for the participant.");
                            this.iarrErrors.Add(lobjError);
                        }
                    }
                }
            }
            if (this.icdoBenefitCalculationHeader.benefit_type_value == busConstant.BENEFIT_TYPE_DEATH_PRE_RETIREMENT && this.icdoBenefitCalculationHeader.istrOrganizationId.IsNotNullOrEmpty())
            {
                DataTable ldtblOrganization = busBase.Select("cdoOrganization.GetValidOrgDetails", new object[1] { this.icdoBenefitCalculationHeader.istrOrganizationId });
                if (ldtblOrganization.Rows.Count > 0)
                { }
                else
                {
                    lobjError = AddError(5452, "");
                    this.iarrErrors.Add(lobjError);
                }

            }

            if (this.icdoBenefitCalculationHeader.beneficiary_person_id != 0 && this.icdoBenefitCalculationHeader.istrSurvivorTypeValue != busConstant.SURVIVOR_TYPE_ORGN)
            {
                DataTable ldtblBeneficiary = busBase.Select("cdoPersonAccountBeneficiary.GetBeneficiaryByBeneficiaryID", new object[1] { this.icdoBenefitCalculationHeader.beneficiary_person_id });

                if (ldtblBeneficiary.Rows.Count > 0)
                {
                    string strName = string.Empty;
                    string strExisting = string.Empty;
                    if (!string.IsNullOrEmpty(Convert.ToString(ldtblBeneficiary.Rows[0]["istrFullName"])))
                    {
                        strName = Convert.ToString(ldtblBeneficiary.Rows[0]["istrFullName"]).Trim();
                    }
                    if (!string.IsNullOrEmpty(this.icdoBenefitCalculationHeader.beneficiary_person_name))
                    {
                        strExisting = this.icdoBenefitCalculationHeader.beneficiary_person_name.Trim();
                    }

                    if ((Convert.ToString(ldtblBeneficiary.Rows[0]["DATE_OF_BIRTH"]).IsNotNullOrEmpty() && this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth != Convert.ToDateTime(ldtblBeneficiary.Rows[0]["DATE_OF_BIRTH"]))
                                || strExisting != strName)
                    {
                        lobjError = AddError(5430, "");
                        this.iarrErrors.Add(lobjError);
                    }
                }
            }
        }

        #endregion


        public void GetDistributionType()
        {
            string lstrCurrentDatePlusSixtyDays = busGlobalFunctions.ConvertDateIntoDifFormat(DateTime.Now.AddDays(60));

            if (this.icdoBenefitCalculationHeader.benefit_type_value == busConstant.BENEFIT_TYPE_WITHDRAWAL)
            {
                if (this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().icdoBenefitCalculationDetail.uvhp_flag == busConstant.FLAG_YES &&
                    (this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().icdoBenefitCalculationDetail.ee_flag == busConstant.FLAG_NO ||
                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().icdoBenefitCalculationDetail.ee_flag.IsNullOrEmpty()))
                {
                    istrDistributionType = "Refund of UV&HP plus interest. This form must be received by MPI within 60 days of your withdrawal request.";

                }
                else if (this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().icdoBenefitCalculationDetail.uvhp_flag == busConstant.FLAG_NO &&
                    (this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().icdoBenefitCalculationDetail.ee_flag == busConstant.FLAG_YES ||
                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().icdoBenefitCalculationDetail.ee_flag.IsNullOrEmpty()))
                {
                    istrDistributionType = "Refund of employee contributions. This form must be received by MPI within 60 days of your withdrawal request.";
                }
                else if (this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().icdoBenefitCalculationDetail.uvhp_flag == busConstant.FLAG_YES &&
                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().icdoBenefitCalculationDetail.ee_flag == busConstant.FLAG_YES)
                {
                    istrDistributionType = "Refund of employee contributions and UV&HP plus interest. This form must be received by MPI within 60 days of your withdrawal request.";
                }
                else if (this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).Count() > 0)
                {
                    istrDistributionType = "IAP Withdrawal. This form must be received by MPI within 60 days of your withdrawal request.";
                }
            }

            else if (this.icdoBenefitCalculationHeader.benefit_type_value == busConstant.BENEFIT_TYPE_RETIREMENT)
            {
                if (this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).Count() > 0)
                {
                    istrDistributionType = "IAP Retirement. This form must be received by MPI no later than " + lstrCurrentDatePlusSixtyDays + ".";  //Review {due date}
                }
                else if (this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).Count() > 0)
                {
                    LoadPlanBenefitsForPlan(busConstant.MPIPP_PLAN_ID);
                    int lintCount = (from item in this.iclbcdoPlanBenefit
                                     where item.icdoPlanBenefitXr.benefit_option_value == busConstant.LUMP_SUM
                                     select item).Count();
                    if (lintCount > 0)
                    {
                        istrDistributionType = "Pension Benefit Single Lump Sum Payment. This form must be received by MPI no later than " + lstrCurrentDatePlusSixtyDays + ".";  //Review {due date}
                    }
                }
            }

            else if (this.icdoBenefitCalculationHeader.payee_account_id > 0)
            {
                busPayeeAccount lbusPayeeAccount = new busPayeeAccount();
                if (lbusPayeeAccount.FindPayeeAccount(this.icdoBenefitCalculationHeader.payee_account_id))
                {
                    if (lbusPayeeAccount.icdoPayeeAccount.retiree_incr_flag == busConstant.FLAG_YES)
                    {
                        int lintYear = System.DateTime.Now.Year;
                        istrDistributionType = "Additional 13th and 14th Monthly Benefit Payment. This form must be received by MPI no later than October 15, " + lintYear + " or my payment will be defaulted to Option 1 under Section A. ";
                    }
                }
            }
        }

        //CODE _ AARTI
        public override busBase GetCorPerson()
        {
            this.ibusPerson.LoadPersonAddresss();
            this.ibusPerson.LoadPersonContacts();
            this.ibusPerson.LoadCorrAddress();
            return this.ibusPerson;
        }

        //CODE- AARTI
        public override void LoadCorresProperties(string astrTemplateName)
        {
            //Ticket #101090
            if (astrTemplateName == busConstant.DISABILITY_BENEFIT_ELECTION_MPI_PACKAGE && this.sel_benefit_calculation_detail_id > 0)
            {
                busBenefitCalculationDetail lbusBenefitCalculationDetail = new busBenefitCalculationDetail { icdoBenefitCalculationDetail = new cdoBenefitCalculationDetail() };
                lbusBenefitCalculationDetail.FindBenefitCalculationDetail(this.sel_benefit_calculation_detail_id);
                icdoBenefitCalculationHeader.retirement_date = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.retirement_date;
            }

            DateTime ldtCurrentDate = System.DateTime.Now;
            idtCrntDate = ldtCurrentDate.Date;
            this.ibusPerson.istrCurrentDate = busGlobalFunctions.ConvertDateIntoDifFormat(ldtCurrentDate);
            //Ticket 85016 - For PERO-0011 correspondence
            istrCurrentLongDate = busGlobalFunctions.ConvertDateIntoDifFormat(ldtCurrentDate);
            istrRetirementLongDate = busGlobalFunctions.ConvertDateIntoDifFormat(icdoBenefitCalculationHeader.retirement_date);
            bool lblnRetirementTypeMinDist = false;
            int lintTotalQualifiedYear = 0;

            base.LoadCorresProperties(astrTemplateName);

            istrCurrentTime = DateTime.Now.ToString("h:mm tt");

            this.LoadBenefitApplication();
            GetPriorDates();

            //PIR-940
            DateTime ldtForfDate = DateTime.MinValue;
            DateTime ldtWithdrwlDate = DateTime.MinValue;
            DateTime ldtEffectiveDate = DateTime.MinValue;

            istrBenType = icdoBenefitCalculationHeader.benefit_type_description;

            if (iclbBenefitCalculationDetail != null && iclbBenefitCalculationDetail.Where(t => t.icdoBenefitCalculationDetail.istrPlanCode == busConstant.MPIPP).Count() > 0 &&
                this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).Count() > 0)
            {
                busPersonAccountEligibility lbusPersonAccountEligibility = new busPersonAccountEligibility();
                lbusPersonAccountEligibility = lbusPersonAccountEligibility.LoadPersonAccEligibilityByPersonAccountId(this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().icdoPersonAccount.person_account_id);
                if (lbusPersonAccountEligibility != null)
                    ldtForfDate = lbusPersonAccountEligibility.icdoPersonAccountEligibility.forfeiture_date;

            }
            DataTable ldtbCheckPersonWithdrawal = busBase.Select("cdoBenefitApplication.CheckPersonHasWithdrawal", new object[1] { this.ibusPerson.icdoPerson.person_id });
            if (ldtbCheckPersonWithdrawal != null && ldtbCheckPersonWithdrawal.Rows.Count > 0)
            {
                ldtWithdrwlDate = (from item in ldtbCheckPersonWithdrawal.AsEnumerable()
                                   where item.Field<string>("CONTRIBUTION_TYPE_VALUE") == "EE"
                                   orderby item.Field<DateTime>("WITHDRAWAL_DATE") descending
                                   select item.Field<DateTime>("WITHDRAWAL_DATE")).FirstOrDefault();
            }

            if (ldtForfDate >= ldtWithdrwlDate)
                ldtEffectiveDate = ldtForfDate;
            else
                ldtEffectiveDate = ldtWithdrwlDate;

            if (this.ibusBenefitApplication.aclbPersonWorkHistory_MPI.IsNullOrEmpty() || this.ibusBenefitApplication.aclbPersonWorkHistory_IAP.IsNullOrEmpty())
            {
                //If there are 2 valid retirement date options, AND one of the Plans has a retirement date equal to the 2nd retirement date option in the calculation, AND the Plan type is MPIPP
                if (this.icdoBenefitCalculationHeader.retirement_date != DateTime.MinValue && this.icdoBenefitCalculationHeader.retirement_date_option_2 != DateTime.MinValue
                    && this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.retirement_date == this.icdoBenefitCalculationHeader.retirement_date_option_2 && item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).Count() > 0)//LA Sunset - Corr Tracking
                {
                    Collection<busBenefitCalculationYearlyDetail> lcol = this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.retirement_date == this.icdoBenefitCalculationHeader.retirement_date_option_2 && item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().iclbBenefitCalculationYearlyDetail;
                    if (!lcol.IsNullOrEmpty())
                    {
                        this.ibusBenefitApplication.iintVestedYears = lcol.Where(t => t.icdoBenefitCalculationYearlyDetail.qualified_years_count > 0).OrderByDescending(item => item.icdoBenefitCalculationYearlyDetail.plan_year).FirstOrDefault().icdoBenefitCalculationYearlyDetail.qualified_years_count;
                        this.ibusBenefitApplication.idecVestedHours = (from item in lcol where item.icdoBenefitCalculationYearlyDetail.plan_year > ldtEffectiveDate.Year select item.icdoBenefitCalculationYearlyDetail.vested_hours).Sum();

                    }
                }
                else if (this.icdoBenefitCalculationHeader.retirement_date != DateTime.MinValue)
                {
                    if (this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault() != null)
                    {
                        Collection<busBenefitCalculationYearlyDetail> lcol = this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().iclbBenefitCalculationYearlyDetail;
                        if (!lcol.IsNullOrEmpty())
                        {
                            this.ibusBenefitApplication.iintVestedYears = lcol.Where(t => t.icdoBenefitCalculationYearlyDetail.qualified_years_count > 0).OrderByDescending(item => item.icdoBenefitCalculationYearlyDetail.plan_year).FirstOrDefault().icdoBenefitCalculationYearlyDetail.qualified_years_count;
                            this.ibusBenefitApplication.idecVestedHours = (from item in lcol where item.icdoBenefitCalculationYearlyDetail.plan_year > ldtEffectiveDate.Year select item.icdoBenefitCalculationYearlyDetail.vested_hours).Sum();
                            idecTotalPenHrs = (from item in lcol where item.icdoBenefitCalculationYearlyDetail.plan_year > ldtEffectiveDate.Year select item.icdoBenefitCalculationYearlyDetail.idecTotalPensionHours).Sum();

                        }
                    }
                }

            }

            //Ticket#76117
            if (astrTemplateName == busConstant.Retirement_Benefit_Election_Packet_MPI || astrTemplateName == busConstant.DISABILITY_BENEFIT_ELECTION_MPI_PACKAGE || astrTemplateName == busConstant.MEDICARE_COORDINATION_ACKNOWLEDGEMENT || astrTemplateName == busConstant.MD_TO_RD_PACKET)
            {
                Collection<busBenefitCalculationYearlyDetail> lcol = this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().iclbBenefitCalculationYearlyDetail;
                if (!lcol.IsNullOrEmpty())
                {
                    idecTotalPenHrs = (from item in lcol where item.icdoBenefitCalculationYearlyDetail.plan_year > ldtEffectiveDate.Year select item.icdoBenefitCalculationYearlyDetail.annual_hours).Sum();
                    iintTotalQYrs = lcol.Where(t => t.icdoBenefitCalculationYearlyDetail.qualified_years_count > 0).OrderByDescending(item => item.icdoBenefitCalculationYearlyDetail.plan_year).FirstOrDefault().icdoBenefitCalculationYearlyDetail.qualified_years_count;

                    //  intHealthHours = (from item in lcol where item.icdoBenefitCalculationYearlyDetail.plan_year > ldtEffectiveDate.Year select item.icdoBenefitCalculationYearlyDetail.idecTotalHealthHours).Sum();
                    intHealthHours = lcol.Sum(i => i.icdoBenefitCalculationYearlyDetail.health_hours);
                    intHealthYearCount = lcol.Where(t => t.icdoBenefitCalculationYearlyDetail.qualified_years_count > 0).OrderByDescending(item => item.icdoBenefitCalculationYearlyDetail.plan_year).FirstOrDefault().icdoBenefitCalculationYearlyDetail.health_years_count;

                }




            }

            if (astrTemplateName == busConstant.ACTIVE_DEATH_PENSION_BENEFIT_ELECTION_COVER_LETTER_DEFAULTED_ANNUITY || astrTemplateName == busConstant.DIRECT_DEPOSIT_AUTHORIZATION
            || astrTemplateName == busConstant.ACTIVE_DEATH_BENEFICIARY_PACKAGE_COVER_LETTER_PENSION_AND_IAP ||
            astrTemplateName == busConstant.ACTIVE_DEATH_BENEFICIARY_PACKAGE_COVER_LETTER_PENSION_ONLY
            || astrTemplateName == busConstant.Active_Death_Beneficiary_Package_Pension_Iap
            || astrTemplateName == busConstant.Active_Death_Beneficiary_Package_Pension_Only
            || astrTemplateName == busConstant.Active_Death_Beneficiary_Package_Defaulted_Annunity
            || astrTemplateName == busConstant.Active_Death_Beneficiary_Package_LUMPSUM)
            {
                this.FillBeneficiaryDetails();
                DateTime NormalRtmtDt = this.ibusPerson.icdoPerson.date_of_birth.AddYears(65);
                istrNormalRtmtDate = String.Format("{0:MMMM dd, yyyy}", NormalRtmtDt);
            }

            #region Load distribution type and in case of Death Pre retirement load Plan type

            if (astrTemplateName == busConstant.LUMP_SUM_DISTRIBUTION_ELECTION_NON_SPOUCE || astrTemplateName == busConstant.LUMP_SUM_DISTRIBUTION_ELECTION)
            {
                istrParticipantMPIID = ibusBenefitApplication.ibusPerson.icdoPerson.mpi_person_id;

                if (icdoBenefitCalculationHeader.benefit_type_value == busConstant.BENEFIT_TYPE_DEATH_PRE_RETIREMENT || icdoBenefitCalculationHeader.benefit_type_value == busConstant.BENEFIT_TYPE_DEATH_POST_RETIREMENT)
                {
                    this.FillBeneficiaryDetails();
                    this.ibusPerson = this.ibusBeneficiary;
                    istrSurvivorFullName = this.icdoBenefitCalculationHeader.beneficiary_person_name;
                    istrParticipantFullName = this.ibusBenefitApplication.ibusPerson.icdoPerson.first_name.ToUpper() + " " + this.ibusBenefitApplication.ibusPerson.icdoPerson.last_name.ToUpper();


                }
            }
            #endregion

            if (astrTemplateName == busConstant.ACTIVE_DEATH_NON_SPOUSE_BENE || astrTemplateName == busConstant.Pre_Retirement_Death_non_spouse_Packet)
            {
                if (!this.iclbBenefitCalculationDetail.IsNullOrEmpty())
                {
                    if (this.iclbBenefitCalculationDetail.FirstOrDefault() != null)
                    {
                        this.iclbBenefitCalculationDetail.FirstOrDefault().LoadPersonAccount();
                        if (this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault() != null)
                        {
                            idecAccntBalance = this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().icdoBenefitCalculationDetail.iap_balance_amount;
                            if (icdoBenefitCalculationHeader.survivor_percentage > 0)
                            {
                                idecAccntBalance = (idecAccntBalance * icdoBenefitCalculationHeader.survivor_percentage) / 100;
                            }
                        }
                        if (this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault() != null)
                        {
                            busBenefitCalculationDetail lbusBenefitCalculationDetailForMPI = new busBenefitCalculationDetail();
                            lbusBenefitCalculationDetailForMPI = this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault();
                            idecEEUVHPAmt = lbusBenefitCalculationDetailForMPI.icdoBenefitCalculationDetail.total_uvhp_contribution_amount;// + lbusBenefitCalculationDetailForMPI.icdoBenefitCalculationDetail.total_uvhp_interest_amount;
                            idecEEUVHPAmt = idecEEUVHPAmt +
                                                         lbusBenefitCalculationDetailForMPI.icdoBenefitCalculationDetail.vested_ee_amount +
                                                         +lbusBenefitCalculationDetailForMPI.icdoBenefitCalculationDetail.non_vested_ee_amount;
                            idecPensionAcctBal = idecEEUVHPAmt;
                            idecEEUVHPInterest = lbusBenefitCalculationDetailForMPI.icdoBenefitCalculationDetail.vested_ee_interest +
                                                         +lbusBenefitCalculationDetailForMPI.icdoBenefitCalculationDetail.non_vested_ee_interest
                                                         + lbusBenefitCalculationDetailForMPI.icdoBenefitCalculationDetail.total_uvhp_interest_amount;
                            idecEEUVHPTotal = idecEEUVHPAmt + idecEEUVHPInterest;
                            if (icdoBenefitCalculationHeader.survivor_percentage > 0)
                            {
                                idecEEUVHPAmt = (idecEEUVHPAmt * icdoBenefitCalculationHeader.survivor_percentage) / 100;
                                idecEEUVHPInterest = (idecEEUVHPInterest * icdoBenefitCalculationHeader.survivor_percentage) / 100;
                                idecEEUVHPTotal = (idecEEUVHPTotal * icdoBenefitCalculationHeader.survivor_percentage) / 100;
                            }

                            if (lbusBenefitCalculationDetailForMPI.iclbBenefitCalculationOptions.
                                    Where(t => (t.icdoBenefitCalculationOptions.uvhp_flag == busConstant.FLAG_YES || t.icdoBenefitCalculationOptions.ee_flag == busConstant.FLAG_YES) && t.icdoBenefitCalculationOptions.istrBenefitOptionDescription.Contains("Lump")).Count() > 0)
                            {
                                idecEEUVHPLumSum = lbusBenefitCalculationDetailForMPI.iclbBenefitCalculationOptions.
                                    Where(t => (t.icdoBenefitCalculationOptions.uvhp_flag == busConstant.FLAG_YES || t.icdoBenefitCalculationOptions.ee_flag == busConstant.FLAG_YES) && t.icdoBenefitCalculationOptions.istrBenefitOptionDescription.Contains("Lump")).FirstOrDefault().icdoBenefitCalculationOptions.survivor_amount;
                            }

                            if (lbusBenefitCalculationDetailForMPI.iclbBenefitCalculationOptions.
                                    Where(t => (t.icdoBenefitCalculationOptions.uvhp_flag != busConstant.FLAG_YES && t.icdoBenefitCalculationOptions.ee_flag != busConstant.FLAG_YES) && t.icdoBenefitCalculationOptions.istrBenefitOptionDescription.Contains("Lump")).Count() > 0)
                            {
                                idecMPILumpSum = lbusBenefitCalculationDetailForMPI.iclbBenefitCalculationOptions.
                                    Where(t => (t.icdoBenefitCalculationOptions.uvhp_flag != busConstant.FLAG_YES && t.icdoBenefitCalculationOptions.ee_flag != busConstant.FLAG_YES) && t.icdoBenefitCalculationOptions.istrBenefitOptionDescription.Contains("Lump")).FirstOrDefault().icdoBenefitCalculationOptions.survivor_amount;
                            }
                        }
                    }
                }
            }
            if (astrTemplateName == busConstant.ACTIVE_DEATH_NON_SPOUSE_BENE || astrTemplateName == busConstant.Pre_Retirement_Death_non_spouse_Packet || astrTemplateName == busConstant.Beneficiary_IAP_Second_Payment_packet)
            {
                DataTable ldtbGetSurvivorDetails = busBase.Select<cdoPerson>(
                  new string[1] { enmPerson.person_id.ToString() },
                  new object[1] { this.icdoBenefitCalculationHeader.beneficiary_person_id }, null, null);

                if (ldtbGetSurvivorDetails.Rows.Count > 0)
                {
                    istrSurvivorFullName = (this.icdoBenefitCalculationHeader.beneficiary_person_name).ToUpper();
                    istrSurvivorFullNameInProperCase = this.icdoBenefitCalculationHeader.beneficiary_person_name;
                    istrSurvivorLastName = ldtbGetSurvivorDetails.Rows[0][enmPerson.last_name.ToString()].ToString();
                    istrSurvivorPrefix = ldtbGetSurvivorDetails.Rows[0][enmPerson.name_prefix_value.ToString()].ToString();
                    istrSurvivorMPID = ldtbGetSurvivorDetails.Rows[0][enmPerson.mpi_person_id.ToString()].ToString();
                }
                else
                {
                    ldtbGetSurvivorDetails = busBase.Select<cdoOrganization>(
                    new string[1] { enmOrganization.org_id.ToString() },
                    new object[1] { this.icdoBenefitCalculationHeader.organization_id }, null, null);

                    if (ldtbGetSurvivorDetails.Rows.Count > 0)
                    {
                        istrSurvivorFullName = ldtbGetSurvivorDetails.Rows[0][enmOrganization.org_name.ToString()].ToString();
                        istrSurvivorLastName = ldtbGetSurvivorDetails.Rows[0][enmOrganization.org_name.ToString()].ToString();
                        istrSurvivorMPID = ldtbGetSurvivorDetails.Rows[0][enmOrganization.mpi_org_id.ToString()].ToString();
                    }
                }
            }

            if (astrTemplateName == busConstant.EMPLOYEE_CONTRIBUTION_BALANCE)
            {
                if (this.ibusPerson.iclbPersonAccount != null && this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).Count() > 0)
                {
                    EmployeeContributionBalance(this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault(),
                                                this.icdoBenefitCalculationHeader.retirement_date);
                }
            }

            if (astrTemplateName == busConstant.RETIREMENT_BENEFIT_ELECTION_FORM_LOCAL_52 || astrTemplateName == busConstant.Retirement_Benefit_Election_Packet_L52
                || astrTemplateName == busConstant.Retirement_Benefit_Election_Packet_L161 || astrTemplateName == busConstant.Retirement_Benefit_Election_Packet_L600
                || astrTemplateName == busConstant.Retirement_Benefit_Election_Packet_L666 || astrTemplateName == busConstant.Retirement_Benefit_Election_Packet_L700
                || astrTemplateName == busConstant.Disability_Benefit_Election_Packet_L52 || astrTemplateName == busConstant.Disability_Benefit_Election_Packet_L161
                || astrTemplateName == busConstant.Disability_Benefit_Election_Packet_L600 || astrTemplateName == busConstant.Disability_Benefit_Election_Packet_L666
                || astrTemplateName == busConstant.Disability_Benefit_Election_Packet_L700)
            {
                this.ibusBenefitApplication.istrPlanDesc = this.iclbBenefitCalculationDetail.FirstOrDefault().icdoBenefitCalculationDetail.istrPlanDescription;

            }
            if (astrTemplateName == busConstant.REQUEST_OF_ANNUITY_QUOTE_OR_ESTIMATE_TO_INSURANCE_AGENCY || astrTemplateName == busConstant.IAP_ANNUITY_OPTION_ELECTION_CONFIRMATION || astrTemplateName == busConstant.Beneficiary_IAP_Second_Payment_packet)
            {
                //Kunal : should be generated only in case of final calculations.

                busOrganization lbusOrganization = new busOrganization();
                if (lbusOrganization.FindOrganization(this.icdoBenefitCalculationHeader.organization_id))
                {
                    istrOrgName = lbusOrganization.icdoOrganization.org_name;
                    istrOrgMpId = lbusOrganization.icdoOrganization.mpi_org_id;

                }

                
                               

                if (!this.iclbBenefitCalculationDetail.IsNullOrEmpty() && this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).Count() > 0)
                {
                    if (this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID && item.icdoBenefitCalculationDetail.local52_special_acct_bal_amount == 0 && item.icdoBenefitCalculationDetail.local161_special_acct_bal_amount == 0).Count() > 0)
                    {
                        idecAccntBalance = this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID && item.icdoBenefitCalculationDetail.local52_special_acct_bal_amount == 0 && item.icdoBenefitCalculationDetail.local161_special_acct_bal_amount == 0).FirstOrDefault().icdoBenefitCalculationDetail.iap_balance_amount;

                    }
                    else if (this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID && item.icdoBenefitCalculationDetail.local52_special_acct_bal_amount > 0 && item.icdoBenefitCalculationDetail.local161_special_acct_bal_amount > 0).Count() > 0)
                    {
                        idecAccntBalance = this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID && item.icdoBenefitCalculationDetail.local52_special_acct_bal_amount > 0 && item.icdoBenefitCalculationDetail.local161_special_acct_bal_amount > 0).FirstOrDefault().icdoBenefitCalculationDetail.local161_special_acct_bal_amount +
                                            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID && item.icdoBenefitCalculationDetail.local52_special_acct_bal_amount == 0 && item.icdoBenefitCalculationDetail.local161_special_acct_bal_amount == 0).FirstOrDefault().icdoBenefitCalculationDetail.local52_special_acct_bal_amount;

                    }
                    else if (this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID && item.icdoBenefitCalculationDetail.local52_special_acct_bal_amount > 0 && item.icdoBenefitCalculationDetail.local161_special_acct_bal_amount == 0).Count() > 0)
                    {
                        idecAccntBalance = this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID && item.icdoBenefitCalculationDetail.local52_special_acct_bal_amount > 0 && item.icdoBenefitCalculationDetail.local161_special_acct_bal_amount == 0).FirstOrDefault().icdoBenefitCalculationDetail.local52_special_acct_bal_amount;

                    }
                    else if (this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID && item.icdoBenefitCalculationDetail.local52_special_acct_bal_amount == 0 && item.icdoBenefitCalculationDetail.local161_special_acct_bal_amount > 0).Count() > 0)
                    {
                        idecAccntBalance = this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID && item.icdoBenefitCalculationDetail.local52_special_acct_bal_amount == 0 && item.icdoBenefitCalculationDetail.local161_special_acct_bal_amount > 0).FirstOrDefault().icdoBenefitCalculationDetail.local161_special_acct_bal_amount;

                    }
                    if (!this.iclbBenefitCalculationDetail.FirstOrDefault().iclbBenefitCalculationOptions.IsNullOrEmpty())
                    {
                        istrBenefitOptionValue = this.iclbBenefitCalculationDetail.FirstOrDefault().iclbBenefitCalculationOptions.FirstOrDefault().icdoBenefitCalculationOptions.istrBenefitOptionDescription;
                    if (istrBenefitOptionValue != "Life Annuity" && istrBenefitOptionValue != "Ten-Years-Certain and Life Annuity")
                        {
                            istrBenefitOptionValueData = 1;
                            this.ibusPerson.FindPerson(this.icdoBenefitCalculationHeader.person_id);
;                        } 
                    }
                    //Ticket#136246
                    if (astrTemplateName == busConstant.Beneficiary_IAP_Second_Payment_packet && this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT)
                    {
                        if(this.iclbBenefitCalculationDetail.FirstOrDefault().iclbBenefitCalculationOptions.FirstOrDefault().icdoBenefitCalculationOptions.survivor_amount > 0)
                        {
                            idecMPILumpSum = this.iclbBenefitCalculationDetail.FirstOrDefault().iclbBenefitCalculationOptions.FirstOrDefault().icdoBenefitCalculationOptions.survivor_amount -
                                this.iclbBenefitCalculationDetail.FirstOrDefault().iclbBenefitCalculationOptions.FirstOrDefault().icdoBenefitCalculationOptions.paid_amount;

                        }
                        
                    }

                }
                
            }
            if (!this.iclbBenefitCalculationDetail.IsNullOrEmpty())
            {
                this.ibusBenefitApplication.istrRetirementType = this.iclbBenefitCalculationDetail.FirstOrDefault().icdoBenefitCalculationDetail.benefit_subtype_description;
                if (this.iclbBenefitCalculationDetail.FirstOrDefault().icdoBenefitCalculationDetail.benefit_subtype_value == busConstant.RETIREMENT_TYPE_MINIMUM_DISTRIBUTION)
                    lblnRetirementTypeMinDist = true;
            }

            this.ibusBenefitApplication.istrCurrentDate = busGlobalFunctions.ConvertDateIntoDifFormat(ldtCurrentDate);
            this.ibusBenefitApplication.idueDate = ldtCurrentDate.AddDays(90);
            this.ibusBenefitApplication.istrDueDate = busGlobalFunctions.ConvertDateIntoDifFormat(this.ibusBenefitApplication.idueDate);
            this.ibusBenefitApplication.istrMinimumDistributionDate = string.Empty;
            //  if(astrTemplateName != busConstant.Withdrawal_EE_UVHP_REFUND_LETTER_A && astrTemplateName != busConstant.Withdrawal_EE_UVHP_REFUND_LETTER_B)
            //  {
            if (astrTemplateName != busConstant.RETIREMENT_APPLICATION_IAP_WITHDRAWAL &&
                astrTemplateName != busConstant.RE_EMPLOYMENT_RULES_ACKNOWLEDGEMENT && astrTemplateName != busConstant.MEDICARE_COORDINATION_ACKNOWLEDGEMENT &&
                astrTemplateName != busConstant.RETIREMENT_COUNSELING_SUMMARY_OF_PENDING_ITEMS && astrTemplateName != busConstant.DIRECT_DEPOSIT_AUTHORIZATION &&
                astrTemplateName != busConstant.TAX_WITHHOLDING_FORM && astrTemplateName != busConstant.EMPLOYEE_CONTRIBUTION_BALANCE && astrTemplateName != busConstant.NON_RESIDENT_ALIEN_STATUS &&
                astrTemplateName != busConstant.LUMP_SUM_DISTRIBUTION_ELECTION_NON_SPOUCE && astrTemplateName != busConstant.LUMP_SUM_DISTRIBUTION_ELECTION
                || astrTemplateName != busConstant.Retirement_Benefit_Election_Packet_MPI || astrTemplateName != busConstant.Disability_Benefit_Election_Packet_MPI
                || astrTemplateName == busConstant.Active_Death_Beneficiary_Package_Pension_Iap
                || astrTemplateName == busConstant.Active_Death_Beneficiary_Package_Pension_Only
                || astrTemplateName == busConstant.Active_Death_Beneficiary_Package_LUMPSUM
                || astrTemplateName == busConstant.Withdrawal_EE_UVHP_Retirement_Disablity_Packet)
            {

                FillOptionsForCorrespondence(astrTemplateName, lblnRetirementTypeMinDist);
                iblnNoBeneficiaryExists = !ibusCalculation.CheckIfSurvivorIsQualifiedSpouse(this.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id);
            }
            // }


            //to keep the code clean added this if condition. since for these two correspondence we need to show either IAP or MPIPP plan or Local
            if (astrTemplateName == busConstant.LUMP_SUM_DISTRIBUTION_ELECTION_NON_SPOUCE || astrTemplateName == busConstant.LUMP_SUM_DISTRIBUTION_ELECTION
                || astrTemplateName == busConstant.IAP_Withdrawal_Packet || astrTemplateName == busConstant.IAP_Disability_Packet
                || astrTemplateName == busConstant.IAP_Disability_Terminally_Ill_Packet || astrTemplateName == busConstant.IAP_Special_Accounts_Packet
                || astrTemplateName == busConstant.Withdrawal_EE_UVHP_Packet || astrTemplateName == busConstant.IAP_Election_Packet
                || astrTemplateName == busConstant.Retirement_Benefit_Election_Packet_MPI || astrTemplateName == busConstant.Disability_Benefit_Election_Packet_MPI
                || astrTemplateName == busConstant.Withdrawal_EE_UVHP_Retirement_Disablity_Packet || astrTemplateName == busConstant.Pre_Retirement_Death_non_spouse_Packet
                || astrTemplateName == busConstant.Withdrawal_EE_UVHP_REFUND_LETTER_A
                || astrTemplateName == busConstant.Withdrawal_EE_UVHP_REFUND_LETTER_B
                || astrTemplateName == busConstant.Withdrawal_EE_UVHP_REFUND_LETTER_C
                || astrTemplateName == busConstant.Active_Death_Beneficiary_Package_Pension_Iap
                || astrTemplateName == busConstant.Active_Death_Beneficiary_Package_Pension_Only || astrTemplateName == busConstant.MD_TO_RD_PACKET || astrTemplateName == busConstant.IAP_RMD_PACKET
                || astrTemplateName == busConstant.RETIREMENT_BENEFIT_ELECTION_FORM_MINIMUM_DISTRIBUTION
                || astrTemplateName == busConstant.RETIREMENT_BENEFIT_ELECTION_FORM_MINIMUM_DISTRIBUTION_REVISED
                || astrTemplateName == busConstant.IAP_SECOND_PAYMENT_LETTER)//Ticket#114490
            {
                FillAmountForLumpSumDistribution();
                //Ticket#157821
                this.ibusBenefitApplication.istrRetirementDate = Convert.ToString(this.ibusBenefitApplication.icdoBenefitApplication.retirement_date);

                if ((icdoBenefitCalculationHeader.benefit_type_value == busConstant.BENEFIT_TYPE_RETIREMENT ||
                        icdoBenefitCalculationHeader.benefit_type_value == busConstant.BENEFIT_TYPE_DISABILITY ||
                        icdoBenefitCalculationHeader.benefit_type_value == busConstant.BENEFIT_TYPE_WITHDRAWAL) && icdoBenefitCalculationHeader.iintPlanId != busConstant.IAP_PLAN_ID)
                {
                    iintLumpSumYearByAge = icdoBenefitCalculationHeader.retirement_date.Year;
                }
                //&&
                //            icdoBenefitCalculationHeader.iintPlanId != busConstant.IAP_PLAN_ID)
                else if (icdoBenefitCalculationHeader.benefit_type_value == busConstant.BENEFIT_TYPE_DEATH_PRE_RETIREMENT)
                {       //Ticket#132277
                    if(this.iclbBenefitCalculationDetail.Where(i=>i.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).Count() > 0)
                    {
                        iintLumpSumYearByAge = this.iclbBenefitCalculationDetail.Where(
                                    item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().icdoBenefitCalculationDetail.iap_as_of_date.Year;

                    }
                    else
                    {
                        iintLumpSumYearByAge = icdoBenefitCalculationHeader.benefit_commencement_date.Year;
                    }
                    
                }

                if (icdoBenefitCalculationHeader.iintPlanId == busConstant.IAP_PLAN_ID)
                {
                    iintLumpSumYearByAge = this.iclbBenefitCalculationDetail.Where(
                                    item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().icdoBenefitCalculationDetail.iap_as_of_date.Year;
                }

                if (astrTemplateName == busConstant.Active_Death_Beneficiary_Package_Pension_Iap ||
                    astrTemplateName == busConstant.Retirement_Benefit_Election_Packet_MPI ||
                    astrTemplateName == busConstant.RETIREMENT_BENEFIT_ELECTION_FORM_MINIMUM_DISTRIBUTION ||
                    astrTemplateName == busConstant.RETIREMENT_BENEFIT_ELECTION_FORM_MINIMUM_DISTRIBUTION_REVISED ||
                    astrTemplateName == busConstant.DISABILITY_BENEFIT_ELECTION_MPI_PACKAGE ||
                    astrTemplateName == busConstant.MD_TO_RD_PACKET)
                {
                    //Ticket #101090
                    busBenefitCalculationDetail iintLumpSumYearTemp;
                    if (astrTemplateName == busConstant.DISABILITY_BENEFIT_ELECTION_MPI_PACKAGE && this.sel_benefit_calculation_detail_id > 0)
                    {

                        iintLumpSumYearTemp = this.iclbBenefitCalculationDetail.Where(
                                            item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID && item.icdoBenefitCalculationDetail.retirement_date == icdoBenefitCalculationHeader.retirement_date).FirstOrDefault();
                    }
                    else
                    {
                        iintLumpSumYearTemp = this.iclbBenefitCalculationDetail.Where(
                                            item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault();
                    }


                    if (iintLumpSumYearTemp != null)
                    {
                        iintLumpSumYear = iintLumpSumYearTemp.icdoBenefitCalculationDetail.iap_as_of_date.Year;
                    }
                }
            }
            //Ticket#102627
            if (astrTemplateName == busConstant.Withdrawal_IAP_RETIREMENT_PACKET)
            {

                iintLumpSumYearByAge = this.iclbBenefitCalculationDetail.Where(
                                item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().icdoBenefitCalculationDetail.iap_as_of_date.Year;


            }

            if (astrTemplateName == busConstant.Withdrawal_EE_UVHP_REFUND_LETTER_A || astrTemplateName == busConstant.Withdrawal_EE_UVHP_REFUND_LETTER_B || astrTemplateName == busConstant.Withdrawal_EE_UVHP_REFUND_LETTER_C)
            {
                iintLumpSumYearByAge = icdoBenefitCalculationHeader.retirement_date.Year;

            }
            if (astrTemplateName == busConstant.RETIREMENT_BENEFIT_ESTIMATE_SUMMARY)
            {
                if (this.ibusPerson.icdoPerson.health_eligible_flag == busConstant.HEALTH_ELIGIBLE_FLAG_YES)
                {
                    istrEligibleForRetireeHealth = "YES";
                }
                else if (this.ibusPerson.icdoPerson.health_eligible_flag == busConstant.HEALTH_ELIGIBLE_FLAG_NO)
                {
                    istrEligibleForRetireeHealth = "NO";
                }
                else if (this.ibusPerson.icdoPerson.health_eligible_flag.IsNullOrEmpty())
                {
                    istrEligibleForRetireeHealth = "";
                }
            }

            if (astrTemplateName == busConstant.NON_RESIDENT_ALIEN_STATUS)
            {
                if ((this.icdoBenefitCalculationHeader.benefit_type_value == busConstant.BENEFIT_TYPE_RETIREMENT) || (this.icdoBenefitCalculationHeader.benefit_type_value == busConstant.BENEFIT_TYPE_DISABILITY)
                    || (this.icdoBenefitCalculationHeader.benefit_type_value == busConstant.BENEFIT_TYPE_WITHDRAWAL && this.icdoBenefitCalculationHeader.dro_application_id == 0))
                {
                    iblnIsParticipant = busConstant.BOOL_TRUE;
                    this.ibusAlternatePayee = new busPerson { icdoPerson = new cdoPerson() };
                    this.ibusAlternatePayee.FindPerson(this.icdoBenefitCalculationHeader.person_id);
                    this.ibusAlternatePayee.LoadInitialData();
                    this.ibusAlternatePayee.LoadPersonAddresss();
                    this.ibusAlternatePayee.LoadPersonContacts();
                    this.ibusAlternatePayee.LoadCorrAddress();
                    if (Convert.ToInt32(this.ibusAlternatePayee.ibusPersonAddressForCorr.icdoPersonAddress.addr_country_value) == busConstant.USA)
                    {
                        istrIsUSA = "1";
                    }
                }
                else
                {
                    iblnIsParticipant = busConstant.BOOL_FALSE;
                    this.ibusAlternatePayee = new busPerson { icdoPerson = new cdoPerson() };
                    this.ibusAlternatePayee.FindPerson(this.icdoBenefitCalculationHeader.beneficiary_person_id);
                    this.ibusAlternatePayee.LoadInitialData();
                    this.ibusAlternatePayee.LoadPersonAddresss();
                    this.ibusAlternatePayee.LoadPersonContacts();
                    this.ibusAlternatePayee.LoadCorrAddress();
                    if (Convert.ToInt32(this.ibusAlternatePayee.ibusPersonAddressForCorr.icdoPersonAddress.addr_country_value) == busConstant.USA)
                    {
                        istrIsUSA = "1";
                    }
                }

            }


            if (astrTemplateName == busConstant.IAP_Disability_Packet || astrTemplateName == busConstant.IAP_Disability_Terminally_Ill_Packet)
            {
                DateTime currentDate = System.DateTime.Now;
                idtRetirementDate = this.icdoBenefitCalculationHeader.retirement_date;

                DateTime ldtNextMonth = currentDate.AddMonths(1);
                ldtNextMonth = ldtNextMonth.GetFirstDayofMonth();

                idtDayOneOfNextMonth = ldtNextMonth;
                istrDayOneOfNextMonth = busGlobalFunctions.ConvertDateIntoDifFormat(idtDayOneOfNextMonth);

                idueDate = idtDayOneOfNextMonth.AddDays(-1);
                istrDueDate = busGlobalFunctions.ConvertDateIntoDifFormat(idueDate);
            }

            // rid 78872
            busAnnualBenefitSummaryOverview ibusAnnualBenefitSummaryOverview = new busAnnualBenefitSummaryOverview();
            ibusAnnualBenefitSummaryOverview.icdoPerson = ibusBenefitApplication.ibusPerson.icdoPerson;
            ibusAnnualBenefitSummaryOverview.LoadWorkHistory(true);
            //busBenefitApplication lbusBenefitApplication = new busBenefitApplication();
            //lbusBenefitApplication = ibusAnnualBenefitSummaryOverview.lbusBenefitApplication;
            

            System.Collections.Generic.List<BenLocal> lbenlocal = new System.Collections.Generic.List<BenLocal>();



            //int lintAccumHealthYear = 0;

            int lintPrevYear = 0;
            foreach (cdoDummyWorkData dwd in ibusAnnualBenefitSummaryOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI)
            {
                BenLocal bl = new BenLocal();

                bl.HealthHour = dwd.idecTotalHealthHours;
                bl.Year = dwd.year;
                bl.QualifiedYear = dwd.qualified_years_count;
                bl.QualifiedHour = dwd.qualified_hours;
                bl.HealthYear = dwd.iintHealthCount;
                bl.TempQHour = dwd.idecTempqualified_hours;
                bl.Comment = dwd.comments;
                lbenlocal.Add(bl);

                lintPrevYear = bl.Year;
            }
            //Ticket#95918
            //if (this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().iclbBenefitCalculationYearlyDetail.IsNotNull() && this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().iclbBenefitCalculationYearlyDetail.Count > 0)
            //{
            //    Collection<busBenefitCalculationYearlyDetail> lcolHealthHours = this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().iclbBenefitCalculationYearlyDetail;




            if (this.icdoBenefitCalculationHeader.benefit_type_value == "DSBL")
            {
                if (astrTemplateName == busConstant.Disability_Benefit_Election_Packet_L700
                    || astrTemplateName == busConstant.Disability_Benefit_Election_Packet_L52
                    || astrTemplateName == busConstant.Disability_Benefit_Election_Packet_L600
                    || astrTemplateName == busConstant.Disability_Benefit_Election_Packet_L161
                    || astrTemplateName == busConstant.Disability_Benefit_Election_Packet_L666
                    || astrTemplateName == busConstant.MEDICARE_COORDINATION_ACKNOWLEDGEMENT
                    || astrTemplateName == busConstant.DISABILITY_BENEFIT_ELECTION_MPI_PACKAGE)
                {
                    if (intHealthHours == 0)
                    {
                        if (ibusAnnualBenefitSummaryOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Select(i => i.idecTotalHealthHours).Sum() == ibusAnnualBenefitSummaryOverview.aclbPersonWorkHistory_Local.Select(i => i.idecTotalHealthHours).Sum())
                        {
                            intHealthHours = ibusAnnualBenefitSummaryOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Select(i => i.idecTotalHealthHours).Sum();

                        }
                        else
                        {
                            intHealthHours = ibusAnnualBenefitSummaryOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Select(i => i.idecTotalHealthHours).Sum() - ibusAnnualBenefitSummaryOverview.aclbPersonWorkHistory_Local.Select(i => i.idecTotalHealthHours).Sum();
                        }

                    }
                }
                if(intHealthYearCount == 0)
                {
                    intHealthYearCount = ibusAnnualBenefitSummaryOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Select(i => i.iintHealthCount).Last();

                }
                
            }

            if (this.icdoBenefitCalculationHeader.benefit_type_value == "RTMT")
            {
                if (astrTemplateName == busConstant.Retirement_Benefit_Election_Packet_L52
               || astrTemplateName == busConstant.Retirement_Benefit_Election_Packet_L161 || astrTemplateName == busConstant.Retirement_Benefit_Election_Packet_L600
               || astrTemplateName == busConstant.Retirement_Benefit_Election_Packet_L666 || astrTemplateName == busConstant.Retirement_Benefit_Election_Packet_L700
               || astrTemplateName == busConstant.Retirement_Benefit_Election_Packet_MPI || astrTemplateName == busConstant.MD_TO_RD_PACKET || astrTemplateName == busConstant.MEDICARE_COORDINATION_ACKNOWLEDGEMENT
               || astrTemplateName == busConstant.Disability_Benefit_Election_Packet_L700)
                {
                    ibusAnnualBenefitSummaryOverview.LoadPlanDetails();
                    if(intHealthHours == 0)
                    {
                        intHealthHours = Convert.ToDecimal(ibusAnnualBenefitSummaryOverview.iclcdoPersonAccountOverview.Where(i => i.istrHealthHoursPO != "N/A").Select(j => j.istrHealthHoursPO).SingleOrDefault());
                    }
                    if(intHealthYearCount == 0)
                    {
                        if (ibusAnnualBenefitSummaryOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Count > 0)
                        {
                            intHealthYearCount = ibusAnnualBenefitSummaryOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Select(i => i.iintHealthCount).Last();
                        }
                        else
                        {
                            intHealthYearCount = 0;
                        }

                    }
                                      
                    if (astrTemplateName == busConstant.Retirement_Benefit_Election_Packet_MPI)
                    {
                        if ((ibusAnnualBenefitSummaryOverview.iclcdoPersonAccountOverview.Where(i => i.plan_id == 2 && i.istrVested == true).Count() > 0) && (ibusAnnualBenefitSummaryOverview.iclcdoPersonAccountOverview.Where(i => i.plan_id > 2 && i.istrVested == true).Count() > 0))
                        {
                            intTotalHours = Convert.ToDecimal(ibusAnnualBenefitSummaryOverview.iclcdoPersonAccountOverview.Where(i => i.plan_id != 1).Select(y => y.istrTotalHours).Sum());
                            intHealthHours = intTotalHours;
                        }
                        else
                        {
                            intTotalHours = idecTotalPenHrs;//Convert.ToDecimal(ibusAnnualBenefitSummaryOverview.iclcdoPersonAccountOverview.Where(i => i.plan_id == 2).Select(y => y.istrTotalHours).Sum());

                        }

                    }


                }

            }



            switch (this.icdoBenefitCalculationHeader.iintPlanId)
            {
                case busConstant.MPIPP_PLAN_ID:
                    if (this.ibusBenefitApplication.icdoBenefitApplication.istrIsPersonVestedinMPI == busConstant.FLAG_YES)
                    {
                        if (this.ibusBenefitApplication.aclbPersonWorkHistory_MPI != null && this.ibusBenefitApplication.aclbPersonWorkHistory_MPI.Count > 0)
                        {
                            this.ibusBenefitApplication.iintVestedYears = this.ibusBenefitApplication.aclbPersonWorkHistory_MPI.Last().vested_years_count;
                            //Ticket 85016 - Correspondence RETR-0054
                            this.ibusBenefitApplication.iintQualifiedYears = this.ibusBenefitApplication.aclbPersonWorkHistory_MPI.Last().qualified_years_count;
                            //PIR 861
                            this.ibusBenefitApplication.idecVestedHours = (from item in this.ibusBenefitApplication.aclbPersonWorkHistory_MPI select item.vested_hours).Sum();
                            //Ticket 85016 - Correspondence RETR-0054
                            this.ibusBenefitApplication.ldecTotalCreditedHrs = ibusAnnualBenefitSummaryOverview.ldecTotalCreditedHours;

                            if (astrTemplateName == busConstant.RETIREMENT_BENEFIT_ELECTION_FORM_MINIMUM_DISTRIBUTION_REVISED || astrTemplateName == busConstant.RETIREMENT_BENEFIT_ELECTION_FORM_MINIMUM_DISTRIBUTION || astrTemplateName == busConstant.Retirement_Benefit_Election_Packet_MPI || astrTemplateName == busConstant.DISABILITY_BENEFIT_ELECTION_MPI_PACKAGE || astrTemplateName == busConstant.MD_TO_RD_PACKET)
                            {
                                //Ticket : 66063
                                // this.idtCutoffEnddate = System.DateTime.Now;
                                utlConnection utlLegacyDBConnetion = HelperFunction.GetDBConnectionProperties("Legacy");
                                string astrLegacyDBConnetion = utlLegacyDBConnetion.istrConnectionString;
                                DataTable ldtbCheckLastReportedDate = new DataTable();
                                SqlParameter[] parameters = new SqlParameter[1];
                                SqlParameter param1 = new SqlParameter("@SSN", DbType.String);
                                param1.Value = this.ibusPerson.icdoPerson.istrSSNNonEncrypted;
                                parameters[0] = param1;
                                ldtbCheckLastReportedDate = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_PensionInterface4OPUS", astrLegacyDBConnetion, null, parameters);

                                DataRow lastRow = ldtbCheckLastReportedDate.Rows[ldtbCheckLastReportedDate.Rows.Count - 1];
                                this.idtCutoffEnddate = Convert.ToDateTime(lastRow["Todate"]);
                                this.iintYear = System.DateTime.Today.Year; //PIR 861
                                DateTime ldtForfeitureDate = DateTime.MinValue;
                                if (this.ibusBenefitApplication.ibusTempPersonAccountEligibility != null)
                                    ldtForfeitureDate = this.ibusBenefitApplication.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.forfeiture_date;
                                this.istrVestedHours = Convert.ToString((from items in this.ibusBenefitApplication.aclbPersonWorkHistory_MPI
                                                                         where items.year > ldtForfeitureDate.Year
                                                                         select items.vested_hours).Sum().ToString("#.##"));
                            }
                        }
                    }
                    //  intHealthHours = (from items in ibusAnnualBenefitSummaryOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI select items.idecTotalHealthHours).Sum();
                    //  intHealthYearCount = (from items in ibusAnnualBenefitSummaryOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI select items.qualified_years_count).Last();

                    break;

                case busConstant.IAP_PLAN_ID:
                    if (this.ibusBenefitApplication.icdoBenefitApplication.istrIsPersonVestedinIAP == busConstant.FLAG_YES)
                    {
                        if (this.ibusBenefitApplication.aclbPersonWorkHistory_IAP != null && this.ibusBenefitApplication.aclbPersonWorkHistory_IAP.Count > 0)
                        {
                            this.ibusBenefitApplication.iintVestedYears = this.ibusBenefitApplication.aclbPersonWorkHistory_IAP.Last().vested_years_count;
                            this.ibusBenefitApplication.idecVestedHours = (from item in this.ibusBenefitApplication.aclbPersonWorkHistory_IAP select item.vested_hours).Sum();
                        }
                    }
                    break;
                //PIR 847
                case busConstant.LOCAL_52_PLAN_ID:
                    this.ibusBenefitApplication.CheckAlreadyVested(busConstant.Local_52);
                    if (this.ibusBenefitApplication != null && this.ibusBenefitApplication.ibusTempPersonAccountEligibility != null)
                    {
                        this.ibusBenefitApplication.iintVestedYears = this.ibusBenefitApplication.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.local_qualified_years;
                        this.ibusBenefitApplication.idecVestedHours = this.ibusBenefitApplication.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.local_frozen_hours;
                    }

                    //PIR-66093
                    if (astrTemplateName == busConstant.RETIREMENT_BENEFIT_ELECTION_FORM_LOCAL_52 || astrTemplateName == busConstant.Retirement_Benefit_Election_Packet_L52 || astrTemplateName == busConstant.Disability_Benefit_Election_Packet_L52)
                    {
                        busPersonAccountEligibility lbusTempPersonAccountEligibility = new busPersonAccountEligibility { icdoPersonAccountEligibility = new cdoPersonAccountEligibility() };
                        int lintAccountId = 0;
                        if (this.ibusBenefitApplication.ibusPerson.iclbPersonAccount != null)
                        {
                            if (this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).Count() > 0)
                            {
                                lintAccountId = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).First().icdoPersonAccount.person_account_id;
                            }
                        }

                        if (lintAccountId > 0)
                        {
                            if (astrTemplateName == busConstant.Retirement_Benefit_Election_Packet_L52)
                            {
                                ibusBenefitApplication.iintVestedYears = ibusAnnualBenefitSummaryOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().qualified_years_count;
                                ibusBenefitApplication.idecVestedHours = (from items in ibusAnnualBenefitSummaryOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI select items.qualified_hours).Sum();
                            }
                            else
                            {
                                if ((ibusAnnualBenefitSummaryOverview.iclcdoPersonAccountOverview.Where(i => i.plan_id == 2 && i.istrVested == true).Count() > 0) && (ibusAnnualBenefitSummaryOverview.iclcdoPersonAccountOverview.Where(i => i.plan_id > 2 && i.istrVested == true).Count() > 0))
                                {
                                    if (astrTemplateName == busConstant.Disability_Benefit_Election_Packet_L52)
                                    {
                                        ibusBenefitApplication.iintVestedYears = ibusAnnualBenefitSummaryOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().qualified_years_count;
                                        ibusBenefitApplication.idecVestedHours = (from items in ibusAnnualBenefitSummaryOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI select items.qualified_hours).Sum();


                                    }
                                    else
                                    {
                                        GetVestedYearsAndHours(astrTemplateName, lintAccountId, ibusAnnualBenefitSummaryOverview);

                                    }

                                }
                                else
                                {
                                    GetVestedYearsAndHours(astrTemplateName, lintAccountId, ibusAnnualBenefitSummaryOverview);

                                }
                            }
                        }
                        //  intHealthHours = (from items in ibusAnnualBenefitSummaryOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI select items.idecTotalHealthHours).Sum();
                        ///   intHealthYearCount = (from items in ibusAnnualBenefitSummaryOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI select items.qualified_years_count).Last();

                    }
                    break;
                //PIR 847
                case busConstant.LOCAL_161_PLAN_ID:
                    if (this.ibusBenefitApplication != null && this.ibusBenefitApplication.ibusTempPersonAccountEligibility != null)
                    {
                        this.ibusBenefitApplication.iintVestedYears = this.ibusBenefitApplication.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.local_qualified_years;
                        this.ibusBenefitApplication.idecVestedHours = this.ibusBenefitApplication.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.local_frozen_hours;
                    }

                    if (astrTemplateName == busConstant.RETIREMENT_BENEFIT_ELECTION_FORM_LOCAL_161 || astrTemplateName == busConstant.Retirement_Benefit_Election_Packet_L161
                        || astrTemplateName == busConstant.Disability_Benefit_Election_Packet_L161)
                    {
                        busPersonAccountEligibility lbusTempPersonAccountEligibility = new busPersonAccountEligibility { icdoPersonAccountEligibility = new cdoPersonAccountEligibility() };
                        int lintAccountId = 0;
                        if (this.ibusBenefitApplication.ibusPerson.iclbPersonAccount != null)
                        {
                            if (this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).Count() > 0)
                            {
                                lintAccountId = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).First().icdoPersonAccount.person_account_id;
                            }
                        }

                        if (lintAccountId > 0)
                        {
                            if (astrTemplateName == busConstant.Retirement_Benefit_Election_Packet_L161)
                            {
                                ibusBenefitApplication.iintVestedYears = ibusAnnualBenefitSummaryOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().qualified_years_count;
                                ibusBenefitApplication.idecVestedHours = (from items in ibusAnnualBenefitSummaryOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI select items.qualified_hours).Sum();
                            }
                            else
                            {
                                if ((ibusAnnualBenefitSummaryOverview.iclcdoPersonAccountOverview.Where(i => i.plan_id == 2 && i.istrVested == true).Count() > 0)&& (ibusAnnualBenefitSummaryOverview.iclcdoPersonAccountOverview.Where(i => i.plan_id > 2 && i.istrVested == true).Count() > 0))
                                {
                                    if (astrTemplateName == busConstant.Disability_Benefit_Election_Packet_L161)
                                    {
                                        ibusBenefitApplication.iintVestedYears = ibusAnnualBenefitSummaryOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().qualified_years_count;
                                        ibusBenefitApplication.idecVestedHours = (from items in ibusAnnualBenefitSummaryOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI select items.qualified_hours).Sum();


                                    }
                                    else
                                    {
                                        GetVestedYearsAndHours(astrTemplateName, lintAccountId, ibusAnnualBenefitSummaryOverview);

                                    }

                                }
                                else
                                {
                                    GetVestedYearsAndHours(astrTemplateName, lintAccountId, ibusAnnualBenefitSummaryOverview);

                                }
                            }
                        }
                        //  intHealthHours = (from items in ibusAnnualBenefitSummaryOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI select items.idecTotalHealthHours).Sum();
                        //   intHealthYearCount = (from items in ibusAnnualBenefitSummaryOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI select items.qualified_years_count).Last();

                    }
                    //if (this.ibusBenefitApplication.aclbPersonWorkHistory_MPI != null && this.ibusBenefitApplication.aclbPersonWorkHistory_MPI.Count > 0)
                    //{
                    //    this.ibusBenefitApplication.iintVestedYears = (from item in this.ibusBenefitApplication.aclbPersonWorkHistory_MPI where item.L161_Hours >= busConstant.MIN_HOURS_FOR_VESTED_YEAR select item.year).Count();
                    //    this.ibusBenefitApplication.idecVestedHours = (from item in this.ibusBenefitApplication.aclbPersonWorkHistory_MPI where item.L161_Hours >= busConstant.MIN_HOURS_FOR_VESTED_YEAR select item.L161_Hours).Sum();
                    //}
                    break;
                //PIR 847
                case busConstant.LOCAL_600_PLAN_ID:

                    if (this.ibusBenefitApplication != null && this.ibusBenefitApplication.ibusTempPersonAccountEligibility != null)
                    {
                        this.ibusBenefitApplication.iintVestedYears = this.ibusBenefitApplication.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.local_qualified_years;
                        this.ibusBenefitApplication.idecVestedHours = this.ibusBenefitApplication.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.local_frozen_hours;
                    }

                    if (astrTemplateName == busConstant.RETIREMENT_BENEFIT_ELECTION_FORM_LOCAL_600 || astrTemplateName == busConstant.Retirement_Benefit_Election_Packet_L600
                       || astrTemplateName == busConstant.Disability_Benefit_Election_Packet_L600)
                    {
                        busPersonAccountEligibility lbusTempPersonAccountEligibility = new busPersonAccountEligibility { icdoPersonAccountEligibility = new cdoPersonAccountEligibility() };
                        int lintAccountId = 0;
                        if (this.ibusBenefitApplication.ibusPerson.iclbPersonAccount != null)
                        {
                            if (this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).Count() > 0)
                            {
                                lintAccountId = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).First().icdoPersonAccount.person_account_id;
                            }
                        }

                        if (lintAccountId > 0)
                        {
                            if (astrTemplateName == busConstant.Retirement_Benefit_Election_Packet_L600)
                            {
                                ibusBenefitApplication.iintVestedYears = ibusAnnualBenefitSummaryOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().qualified_years_count;
                                ibusBenefitApplication.idecVestedHours = (from items in ibusAnnualBenefitSummaryOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI select items.qualified_hours).Sum();
                            }
                            else
                            {
                                if ((ibusAnnualBenefitSummaryOverview.iclcdoPersonAccountOverview.Where(i => i.plan_id == 2 && i.istrVested == true).Count() > 0) && (ibusAnnualBenefitSummaryOverview.iclcdoPersonAccountOverview.Where(i => i.plan_id > 2 && i.istrVested == true).Count() > 0))
                                {
                                    if (astrTemplateName == busConstant.Disability_Benefit_Election_Packet_L600)
                                    {
                                        ibusBenefitApplication.iintVestedYears = ibusAnnualBenefitSummaryOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().qualified_years_count;
                                        ibusBenefitApplication.idecVestedHours = (from items in ibusAnnualBenefitSummaryOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI select items.qualified_hours).Sum();


                                    }
                                    else
                                    {
                                        GetVestedYearsAndHours(astrTemplateName, lintAccountId, ibusAnnualBenefitSummaryOverview);

                                    }

                                }
                                else
                                {
                                    GetVestedYearsAndHours(astrTemplateName, lintAccountId, ibusAnnualBenefitSummaryOverview);

                                }


                                    
                            }
                        }
                        //  intHealthHours = (from items in ibusAnnualBenefitSummaryOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI select items.idecTotalHealthHours).Sum();
                        //  intHealthYearCount = (from items in ibusAnnualBenefitSummaryOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI select items.qualified_years_count).Last();

                    }
                    //if (this.ibusBenefitApplication.aclbPersonWorkHistory_MPI != null && this.ibusBenefitApplication.aclbPersonWorkHistory_MPI.Count > 0)
                    //{
                    //    this.ibusBenefitApplication.iintVestedYears = (from item in this.ibusBenefitApplication.aclbPersonWorkHistory_MPI where item.L600_Hours >= busConstant.MIN_HOURS_FOR_VESTED_YEAR select item.year).Count();
                    //    this.ibusBenefitApplication.idecVestedHours = (from item in this.ibusBenefitApplication.aclbPersonWorkHistory_MPI where item.L600_Hours >= busConstant.MIN_HOURS_FOR_VESTED_YEAR select item.L600_Hours).Sum();
                    //}
                    break;
                //PIR 847
                case busConstant.LOCAL_666_PLAN_ID:

                    if (this.ibusBenefitApplication != null && this.ibusBenefitApplication.ibusTempPersonAccountEligibility != null)
                    {
                        this.ibusBenefitApplication.iintVestedYears = this.ibusBenefitApplication.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.local_qualified_years;
                        this.ibusBenefitApplication.idecVestedHours = this.ibusBenefitApplication.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.local_frozen_hours;
                    }

                    if (astrTemplateName == busConstant.RETIREMENT_BENEFIT_ELECTION_FORM_LOCAL_666 || astrTemplateName == busConstant.Retirement_Benefit_Election_Packet_L666
                        || astrTemplateName == busConstant.Disability_Benefit_Election_Packet_L666)
                    {
                        busPersonAccountEligibility lbusTempPersonAccountEligibility = new busPersonAccountEligibility { icdoPersonAccountEligibility = new cdoPersonAccountEligibility() };
                        int lintAccountId = 0;
                        if (this.ibusBenefitApplication.ibusPerson.iclbPersonAccount != null)
                        {
                            if (this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).Count() > 0)
                            {
                                lintAccountId = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).First().icdoPersonAccount.person_account_id;
                            }
                        }

                        if (lintAccountId > 0)
                        {
                            if (astrTemplateName == busConstant.Retirement_Benefit_Election_Packet_L666)
                            {
                                ibusBenefitApplication.iintVestedYears = ibusAnnualBenefitSummaryOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().qualified_years_count;
                                ibusBenefitApplication.idecVestedHours = (from items in ibusAnnualBenefitSummaryOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI select items.qualified_hours).Sum();
                            }
                            else
                            {
                                if ((ibusAnnualBenefitSummaryOverview.iclcdoPersonAccountOverview.Where(i => i.plan_id == 2 && i.istrVested == true).Count() > 0) && (ibusAnnualBenefitSummaryOverview.iclcdoPersonAccountOverview.Where(i => i.plan_id > 2 && i.istrVested == true).Count() > 0))
                                {
                                    if (astrTemplateName == busConstant.Disability_Benefit_Election_Packet_L666)
                                    {
                                        ibusBenefitApplication.iintVestedYears = ibusAnnualBenefitSummaryOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().qualified_years_count;
                                        ibusBenefitApplication.idecVestedHours = (from items in ibusAnnualBenefitSummaryOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI select items.qualified_hours).Sum();


                                    }
                                    else
                                    {
                                        GetVestedYearsAndHours(astrTemplateName, lintAccountId, ibusAnnualBenefitSummaryOverview);

                                    }

                                }
                                else
                                {
                                    GetVestedYearsAndHours(astrTemplateName, lintAccountId, ibusAnnualBenefitSummaryOverview);

                                }
                            }
                        }
                        //  intHealthHours = (from items in ibusAnnualBenefitSummaryOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI select items.idecTotalHealthHours).Sum();
                        //  intHealthYearCount = (from items in ibusAnnualBenefitSummaryOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI select items.qualified_years_count).Last();

                    }
                    //if (this.ibusBenefitApplication.aclbPersonWorkHistory_MPI != null && this.ibusBenefitApplication.aclbPersonWorkHistory_MPI.Count > 0)
                    //{
                    //    this.ibusBenefitApplication.iintVestedYears = (from item in this.ibusBenefitApplication.aclbPersonWorkHistory_MPI where item.L666_Hours >= busConstant.MIN_HOURS_FOR_VESTED_YEAR select item.year).Count();
                    //    this.ibusBenefitApplication.idecVestedHours = (from item in this.ibusBenefitApplication.aclbPersonWorkHistory_MPI where item.L666_Hours >= busConstant.MIN_HOURS_FOR_VESTED_YEAR select item.L666_Hours).Sum();
                    //}
                    break;
                //PIR 847
                case busConstant.LOCAL_700_PLAN_ID:

                    if (this.ibusBenefitApplication != null && this.ibusBenefitApplication.ibusTempPersonAccountEligibility != null)
                    {
                        this.ibusBenefitApplication.iintVestedYears = this.ibusBenefitApplication.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.local_qualified_years;
                        this.ibusBenefitApplication.idecVestedHours = this.ibusBenefitApplication.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.local_frozen_hours;
                    }

                    if (astrTemplateName == busConstant.RETIREMENT_BENEFIT_ELECTION_FORM_LOCAL_700 || astrTemplateName == busConstant.Retirement_Benefit_Election_Packet_L700
                        || astrTemplateName == busConstant.Disability_Benefit_Election_Packet_L700)
                    {
                        busPersonAccountEligibility lbusTempPersonAccountEligibility = new busPersonAccountEligibility { icdoPersonAccountEligibility = new cdoPersonAccountEligibility() };
                        int lintAccountId = 0;
                        if (this.ibusBenefitApplication.ibusPerson.iclbPersonAccount != null)
                        {
                            if (this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).Count() > 0)
                            {
                                lintAccountId = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).First().icdoPersonAccount.person_account_id;
                            }
                        }

                        if (lintAccountId > 0)
                        {
                            if (astrTemplateName == busConstant.Retirement_Benefit_Election_Packet_L700)
                            {
                                ibusBenefitApplication.iintVestedYears = ibusAnnualBenefitSummaryOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().qualified_years_count;
                                ibusBenefitApplication.idecVestedHours = (from items in ibusAnnualBenefitSummaryOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI select items.qualified_hours).Sum();
                            }
                            else
                            {
                                if ((ibusAnnualBenefitSummaryOverview.iclcdoPersonAccountOverview.Where(i => i.plan_id == 2 && i.istrVested == true).Count() > 0) && (ibusAnnualBenefitSummaryOverview.iclcdoPersonAccountOverview.Where(i => i.plan_id > 2 && i.istrVested == true).Count() > 0))
                                {
                                    if (astrTemplateName == busConstant.Disability_Benefit_Election_Packet_L700)
                                    {
                                        ibusBenefitApplication.iintVestedYears = ibusAnnualBenefitSummaryOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().qualified_years_count;
                                        ibusBenefitApplication.idecVestedHours = (from items in ibusAnnualBenefitSummaryOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI select items.qualified_hours).Sum();


                                    }
                                    else
                                    {
                                        GetVestedYearsAndHours(astrTemplateName, lintAccountId, ibusAnnualBenefitSummaryOverview);

                                    }

                                }
                                else
                                {
                                    GetVestedYearsAndHours(astrTemplateName, lintAccountId, ibusAnnualBenefitSummaryOverview);

                                }
                            }
                            //    DataTable ldtbPersonAccountEligibility = busBase.Select("cdoPersonAccountEligibility.GetLocalEligibilityInfoFromPersonID", new object[1] { ibusBenefitApplication.ibusPerson.icdoPerson.person_id });
                            //if (ldtbPersonAccountEligibility.Rows.Count > 0)
                            //{
                            //    lbusTempPersonAccountEligibility.icdoPersonAccountEligibility.LoadData(ldtbPersonAccountEligibility.Rows[0]);
                            //    ibusBenefitApplication.iintVestedYears = ibusBenefitApplication.aclbPersonWorkHistory_MPI.Last().vested_years_count;
                            //    ibusBenefitApplication.idecVestedHours = lbusTempPersonAccountEligibility.icdoPersonAccountEligibility.local_frozen_hours + (from items in ibusBenefitApplication.aclbPersonWorkHistory_MPI select items.qualified_hours).Sum();
                            //}
                            //   intHealthHours = (from items in ibusAnnualBenefitSummaryOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI select items.idecTotalHealthHours).Sum();
                            //   intHealthYearCount = (from items in ibusAnnualBenefitSummaryOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI select items.qualified_years_count).Last();
                        }
                    }

                    //rid 78872
                    decimal dresult = idecParticipantJSurvivor66_2by3BenefitAmt * decimal.Parse(".66667");
                    istrSurvivorJSurvivor66_2by3BenefitAmt = AppendDoller(dresult);

                    //if (this.ibusBenefitApplication.aclbPersonWorkHistory_MPI != null && this.ibusBenefitApplication.aclbPersonWorkHistory_MPI.Count > 0)
                    //{
                    //    this.ibusBenefitApplication.iintVestedYears = (from item in this.ibusBenefitApplication.aclbPersonWorkHistory_MPI where item.L700_Hours >= busConstant.MIN_HOURS_FOR_VESTED_YEAR select item.year).Count();
                    //    this.ibusBenefitApplication.idecVestedHours = (from item in this.ibusBenefitApplication.aclbPersonWorkHistory_MPI where item.L700_Hours >= busConstant.MIN_HOURS_FOR_VESTED_YEAR select item.L700_Hours).Sum();
                    //}
                    break;
            }

            if (astrTemplateName == busConstant.MD_TO_RD_PACKET)
            {
                GetPriorDates();

                istrSixtyDaysPriorDate = this.ibusBenefitApplication.istrSixtyDaysPriorDate;

                istrRetirementDt = String.Format("{0:MMMM dd, yyyy}", this.ibusBenefitApplication.icdoBenefitApplication.retirement_date);
                istrRetirementDtPlus2Mo = this.ibusBenefitApplication.icdoBenefitApplication.retirement_date.AddMonths(2).ToShortDateString();
                iintVestedYears = this.ibusBenefitApplication.iintVestedYears;
                idecVestedHours = this.ibusBenefitApplication.idecVestedHours;
                if (this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault() != null)
                {
                    idecIAPBalanceAmt = this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().icdoBenefitCalculationDetail.iap_balance_amount;
                }
                else
                {
                    idecIAPBalanceAmt = 0;
                }

                //Ticket#154137
                intHealthHours = (from items in ibusAnnualBenefitSummaryOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI select items.idecTotalHealthHours).Sum();
                intHealthYearCount = (from items in ibusAnnualBenefitSummaryOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI select items.qualified_years_count).Last();


            }

            //PIR 861
            if (astrTemplateName == busConstant.RETIREMENT_BENEFIT_ELECTION_FORM_LOCAL_700 ||
               astrTemplateName == busConstant.RETIREMENT_BENEFIT_ELECTION_FORM_LOCAL_52 ||
               astrTemplateName == busConstant.RETIREMENT_BENEFIT_ELECTION_FORM_LOCAL_161 ||
               astrTemplateName == busConstant.RETIREMENT_BENEFIT_ELECTION_FORM_LOCAL_666 ||
               astrTemplateName == busConstant.RETIREMENT_BENEFIT_ELECTION_FORM_LOCAL_600 ||
               astrTemplateName == busConstant.Retirement_Benefit_Election_Packet_L52 ||
               astrTemplateName == busConstant.Retirement_Benefit_Election_Packet_L161 ||
               astrTemplateName == busConstant.Retirement_Benefit_Election_Packet_L600 ||
               astrTemplateName == busConstant.Retirement_Benefit_Election_Packet_L666 ||
               astrTemplateName == busConstant.Retirement_Benefit_Election_Packet_L700 ||
               astrTemplateName == busConstant.Disability_Benefit_Election_Packet_L52 ||
               astrTemplateName == busConstant.Disability_Benefit_Election_Packet_L161 ||
               astrTemplateName == busConstant.Disability_Benefit_Election_Packet_L600 ||
               astrTemplateName == busConstant.Disability_Benefit_Election_Packet_L666 ||
               astrTemplateName == busConstant.Disability_Benefit_Election_Packet_L700 ||
               astrTemplateName == busConstant.Disability_Benefit_Election_Packet_MPI ||
               astrTemplateName == busConstant.Active_Death_Beneficiary_Package_Pension_Iap
               || astrTemplateName == busConstant.Active_Death_Beneficiary_Package_Pension_Only
               || astrTemplateName == busConstant.Active_Death_Beneficiary_Package_Defaulted_Annunity
               || astrTemplateName == busConstant.Active_Death_Beneficiary_Package_LUMPSUM
               || astrTemplateName == busConstant.Retirement_Benefit_Election_Packet_MPI)
            {
                this.iintYear = DateTime.Today.Year;
                if (astrTemplateName == busConstant.Retirement_Benefit_Election_Packet_L52)
                {
                    this.ibusPerson.LoadBeneficiaries();
                    // int lintSpousePersonID = 0;
                    foreach (busPersonAccountBeneficiary lbusPersonAccountBeneficiary in this.ibusPerson.iclbPersonAccountBeneficiary)
                    {
                        if (lbusPersonAccountBeneficiary.ibusRelationship.icdoRelationship.relationship_value == "SPOU")
                        {
                            iintSpousePersonId = lbusPersonAccountBeneficiary.ibusRelationship.icdoRelationship.person_id;
                        }
                    }

                }
            }

            if (this.icdoBenefitCalculationHeader.retirement_date != DateTime.MinValue)
            {
                DateTime lRetirementDt = this.icdoBenefitCalculationHeader.retirement_date;
                DateTime lRetrDateTemp = lRetirementDt.AddMonths(-2);
                this.ibusBenefitApplication.istrPriorToRetirement = Convert.ToString(lRetrDateTemp.AddDays(-1));
                this.ibusBenefitApplication.istrPriorToRetirement = String.Format("{0:MMMM dd, yyyy}", lRetrDateTemp.AddDays(-1));
                this.ibusBenefitApplication.istrRetirementDt = String.Format("{0:MMMM dd, yyyy}", this.icdoBenefitCalculationHeader.retirement_date);

                if (this.ibusBenefitApplication.icdoBenefitApplication.disability_onset_date != DateTime.MinValue)
                {
                    istrDisabilityOnsetDate = busGlobalFunctions.ConvertDateIntoDifFormat(this.ibusBenefitApplication.icdoBenefitApplication.disability_onset_date);
                    if (this.ibusBenefitApplication.icdoBenefitApplication.disability_onset_date < this.ibusBenefitApplication.icdoBenefitApplication.retirement_date)
                    {
                        this.ibusBenefitApplication.iblnSetDateFlag = true;
                    }
                }
            }

            if (this.ibusBenefitApplication.icdoBenefitApplication.disability_conversion_date != DateTime.MinValue)
            {
                this.ibusBenefitApplication.iblnDisConvDate = true;
            }

            if (astrTemplateName == busConstant.RETIREMENT_APPLICATION_LOCAL_161 || astrTemplateName == busConstant.RETIREMENT_APPLICATION_LOCAL_161_WITHDRAWAL
                || astrTemplateName == busConstant.Retirement_Application_Packet_Local_161 || astrTemplateName == busConstant.IAP_Special_Accounts_Packet)
            {
                foreach (busBenefitCalculationDetail lbusBenefitCalculationDetail in this.iclbBenefitCalculationDetail)
                {
                    busPersonAccountEligibility lbusPersonAccountEligibility = new busPersonAccountEligibility();

                    if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.istrPlanCode == "Local52"
                        || (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.istrPlanCode == busConstant.IAP && istrLocalSpecialAcntPlan.IsNotNullOrEmpty() && istrLocalSpecialAcntPlan.Replace(" ", string.Empty) == "Local52"))
                    {
                        istrPlanLocal = "Local 52";
                        istrIsLocal52 = busConstant.FLAG_YES;

                        if (this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.LOCAL_52_PLAN_ID).Count() > 0)
                        {
                            lbusPersonAccountEligibility = lbusPersonAccountEligibility.LoadPersonAccEligibilityByPersonAccountId(this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.LOCAL_52_PLAN_ID).FirstOrDefault().icdoPersonAccount.person_account_id);
                            if (lbusPersonAccountEligibility != null)
                                this.ibusBenefitApplication.idecVestedHours += lbusPersonAccountEligibility.icdoPersonAccountEligibility.local_frozen_hours;
                        }
                    }
                    else if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.istrPlanCode == "Local161"
                        || (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.istrPlanCode == busConstant.IAP && istrLocalSpecialAcntPlan.IsNotNullOrEmpty() && istrLocalSpecialAcntPlan.Replace(" ", string.Empty) == "Local161"))
                    {
                        istrPlanLocal = "Local 161";

                        if (this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.LOCAL_161_PLAN_ID).Count() > 0)
                        {
                            lbusPersonAccountEligibility = lbusPersonAccountEligibility.LoadPersonAccEligibilityByPersonAccountId(this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.LOCAL_161_PLAN_ID).FirstOrDefault().icdoPersonAccount.person_account_id);
                            if (lbusPersonAccountEligibility != null)
                                this.ibusBenefitApplication.idecVestedHours += lbusPersonAccountEligibility.icdoPersonAccountEligibility.local_frozen_hours;
                        }

                    }
                }

            }
            if (astrTemplateName == busConstant.GENERAL_BENEFIT_ESTIMATE || astrTemplateName == busConstant.GENERAL_BENEFIT_ESTIMATE_52 || astrTemplateName == busConstant.GENERAL_BENEFIT_ESTIMATE_161
                || astrTemplateName == busConstant.GENERAL_BENEFIT_ESTIMATE_600 || astrTemplateName == busConstant.GENERAL_BENEFIT_ESTIMATE_666 || astrTemplateName == busConstant.GENERAL_BENEFIT_ESTIMATE_700|| astrTemplateName == busConstant.MPI_Retirement_Workshop ) //rid 78456
            {
                iintTotalQYrs = 0;
                iintTotalHealthYrs = 0;
                idecTotalCreditHrs = 0.0M;
                idecTotalHealthHrs = 0.0M;
                idecEEContriAmt = 0.0M;
                idecEEIntAmt = 0.0M;
                idecUVHPContriAmt = 0.0M;
                idecUVHPIntAmt = 0.0M;
                idecTotalEEAmt = 0.0M;
                idecTotalUVHPAmt = 0.0M;
                foreach (busBenefitCalculationDetail lbusBenefitCalculationDetail in this.iclbBenefitCalculationDetail)
                {
                    if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.istrPlanCode == busConstant.MPIPP)
                    {
                        if ((lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.Last().icdoBenefitCalculationYearlyDetail.qualified_years_count) > 0)
                            iintTotalQYrs = lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.Last().icdoBenefitCalculationYearlyDetail.qualified_years_count;

                        if ((lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.Last().icdoBenefitCalculationYearlyDetail.health_years_count) > 0)
                            iintTotalHealthYrs = lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.Last().icdoBenefitCalculationYearlyDetail.health_years_count;
                        ////PIR-940
                        //DateTime ldtForfietureDate = new DateTime();
                        //DateTime ldtWithdrawlDate = new DateTime();
                        //if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.istrPlanCode == busConstant.MPIPP &&
                        //    this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).Count() > 0)
                        //{
                        //    busPersonAccountEligibility lbusPersonAccountEligibility = new busPersonAccountEligibility();
                        //    lbusPersonAccountEligibility = lbusPersonAccountEligibility.LoadPersonAccEligibilityByPersonAccountId(this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().icdoPersonAccount.person_account_id);
                        //    if (lbusPersonAccountEligibility != null)
                        //        ldtForfietureDate = lbusPersonAccountEligibility.icdoPersonAccountEligibility.forfeiture_date;

                        //}
                        //DataTable ldtbCheckPersonHasWithdrawal = busBase.Select("cdoBenefitApplication.CheckPersonHasWithdrawal", new object[1] { this.ibusPerson.icdoPerson.person_id });
                        //if (ldtbCheckPersonHasWithdrawal != null && ldtbCheckPersonHasWithdrawal.Rows.Count > 0)
                        //{
                        //    ldtWithdrawlDate = (from item in ldtbCheckPersonHasWithdrawal.AsEnumerable()
                        //                        where item.Field<string>("CONTRIBUTION_TYPE_VALUE") == "EE"
                        //                        orderby item.Field<DateTime>("WITHDRAWAL_DATE") descending
                        //                        select item.Field<DateTime>("WITHDRAWAL_DATE")).FirstOrDefault();
                        //}
                        //if (ldtForfietureDate != DateTime.MinValue && ldtForfietureDate >= ldtWithdrawlDate && lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.Where(item => item.icdoBenefitCalculationYearlyDetail.plan_year > ldtForfietureDate.Year).Count() > 0)
                        //{
                        //    idecTotalCreditHrs = lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.Where(item => item.icdoBenefitCalculationYearlyDetail.plan_year > ldtForfietureDate.Year).Sum(item => item.icdoBenefitCalculationYearlyDetail.annual_hours);
                        //}

                        //else if (ldtWithdrawlDate != DateTime.MinValue && ldtWithdrawlDate >= ldtForfietureDate && lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.Where(item => item.icdoBenefitCalculationYearlyDetail.plan_year > ldtWithdrawlDate.Year).Count() > 0)
                        //{
                        //    idecTotalCreditHrs = lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.Where(item => item.icdoBenefitCalculationYearlyDetail.plan_year > ldtWithdrawlDate.Year).Sum(item => item.icdoBenefitCalculationYearlyDetail.annual_hours);
                        //}
                        //else
                        //{
                        //    if ((lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.Sum(item => item.icdoBenefitCalculationYearlyDetail.annual_hours)) > 0)
                        //        idecTotalCreditHrs = lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.Sum(item => item.icdoBenefitCalculationYearlyDetail.annual_hours);
                        //}
                        ////End : 940

                        //if ((lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.Sum(item => item.icdoBenefitCalculationYearlyDetail.health_hours)) > 0)
                        //    idecTotalHealthHrs = lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.Sum(item => item.icdoBenefitCalculationYearlyDetail.health_hours);

                        //Ticket 85016 - For RETR-0038 correspondence
                        idecTotalCreditHrs = (from items in ibusAnnualBenefitSummaryOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI select items.qualified_hours).Sum();
                        idecTotalHealthHrs = (from items in ibusAnnualBenefitSummaryOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI select items.idecTotalHealthHours).Sum();

                        idecEEContriAmt = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.vested_ee_amount;
                        idecEEIntAmt = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.vested_ee_interest;
                        idecUVHPContriAmt = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.total_uvhp_contribution_amount;
                        idecUVHPIntAmt = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.total_uvhp_interest_amount;

                        if (idecEEContriAmt > 0 || idecEEIntAmt > 0)
                            idecTotalEEAmt = idecEEContriAmt + idecEEIntAmt;

                        if (idecUVHPContriAmt > 0 || idecUVHPIntAmt > 0)
                            idecTotalUVHPAmt = idecUVHPContriAmt + idecUVHPIntAmt;


                        idecAcrdBenefitMPI = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.unreduced_benefit_amount;
                        idecQDROffsetMPI = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.qdro_offset;
                        if (idecQDROffsetMPI > 0)
                        {
                            istrIsQDROffsetMPI = busConstant.FLAG_YES;
                            istrQDROffset = busConstant.FLAG_YES;
                        }
                    }
                    else if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.istrPlanCode == busConstant.IAP)
                    {
                        idecQDROffsetIAP = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.qdro_offset;
                        if (idecQDROffsetIAP > 0)
                        {
                            istrIsQDROffsetIAP = busConstant.FLAG_YES;
                            istrQDROffset = busConstant.FLAG_YES;
                        }
                        idecIAPBalanceAmt = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.iap_balance_amount;
                        iintYear = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.iap_as_of_date.Year;
                    }
                    else if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.istrPlanCode == busConstant.Local_52) //rid 78456
                    {
                        iintTotalHealthYrs = lbenlocal.Last().HealthYear;
                        idecTotalHealthHrs = lbenlocal.Sum(x => x.HealthHour);

                        iintTotalQYrs = ibusAnnualBenefitSummaryOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().qualified_years_count;
                        idecTotalCreditHrs = (from items in ibusAnnualBenefitSummaryOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI select items.qualified_hours).Sum();

                        idecAcrdBenefitLocal = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.unreduced_benefit_amount;

                        // For Local52 Special Account
                        int person_account_id = 0;
                        Collection<busIapAllocationDetailCalculation> iclbL52SpecialAccountDetailPersonOverview = new Collection<busIapAllocationDetailCalculation>();

                        DataTable ldtbGetPersonAcntId = busBase.Select("cdoPersonAccount.GetPersonAccountID", new object[2] { busConstant.IAP_PLAN_ID, ibusAnnualBenefitSummaryOverview.icdoPerson.person_id });
                        if (ldtbGetPersonAcntId.Rows.Count > 0)
                            person_account_id = Convert.ToInt32(ldtbGetPersonAcntId.Rows[0]["PERSON_ACCOUNT_ID"]);

                        DataTable ldtbLocal52Details = busBase.Select("cdoPersonAccountRetirementContribution.GetLocal52SpecialAccountDetailsForPersonOverview", new object[1] { person_account_id });
                        if (ldtbLocal52Details.Rows.Count > 0)
                        {
                            iclbL52SpecialAccountDetailPersonOverview = GetCollection<busIapAllocationDetailCalculation>(ldtbLocal52Details, "icdoIapallocationDetailPersonoverview");
                            decimal dTotalAlloc = 0;
                            foreach (busIapAllocationDetailCalculation item in iclbL52SpecialAccountDetailPersonOverview)
                            {
                                dTotalAlloc += item.icdoIapallocationDetailPersonoverview.alloc1;
                            }

                            idecLocal52SPABalanceAmt = dTotalAlloc + iclbL52SpecialAccountDetailPersonOverview[0].icdoIapallocationDetailPersonoverview.alloc4 + iclbL52SpecialAccountDetailPersonOverview.Sum(x => x.icdoIapallocationDetailPersonoverview.total_payment);
                            if (idecLocal52SPABalanceAmt > 0)
                            {
                                iintYear = (int)iclbL52SpecialAccountDetailPersonOverview.LastOrDefault().icdoIapallocationDetailPersonoverview.computational_year;
                            }
                        }
                    }
                    else if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.istrPlanCode == busConstant.Local_161) //rid 78456
                    {
                        iintTotalHealthYrs = lbenlocal.Last().HealthYear;
                        idecTotalHealthHrs = lbenlocal.Sum(x => x.HealthHour);

                        iintTotalQYrs = ibusAnnualBenefitSummaryOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().qualified_years_count;
                        idecTotalCreditHrs = (from items in ibusAnnualBenefitSummaryOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI select items.qualified_hours).Sum();

                        idecAcrdBenefitLocal = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.unreduced_benefit_amount;


                        // For Local161 Special Account
                        int person_account_id = 0;
                        Collection<busIapAllocationDetailCalculation> iclbL161SpecialAccountDetailPersonOverview = new Collection<busIapAllocationDetailCalculation>();

                        DataTable ldtbGetPersonAcntId = busBase.Select("cdoPersonAccount.GetPersonAccountID", new object[2] { busConstant.IAP_PLAN_ID, ibusAnnualBenefitSummaryOverview.icdoPerson.person_id });
                        if (ldtbGetPersonAcntId.Rows.Count > 0)
                            person_account_id = Convert.ToInt32(ldtbGetPersonAcntId.Rows[0]["PERSON_ACCOUNT_ID"]);

                        DataTable ldtbLocal161Details = busBase.Select("cdoPersonAccountRetirementContribution.GetLocal161SpecialAccountDetailsForPersonOverview", new object[1] { person_account_id });
                        if (ldtbLocal161Details.Rows.Count > 0)
                        {
                            iclbL161SpecialAccountDetailPersonOverview = GetCollection<busIapAllocationDetailCalculation>(ldtbLocal161Details, "icdoIapallocationDetailPersonoverview");
                            decimal dTotalAlloc = 0;
                            foreach (busIapAllocationDetailCalculation item in iclbL161SpecialAccountDetailPersonOverview)
                            {
                                dTotalAlloc += item.icdoIapallocationDetailPersonoverview.alloc1;
                            }

                            idecLocal161SPABalanceAmt = dTotalAlloc + iclbL161SpecialAccountDetailPersonOverview[0].icdoIapallocationDetailPersonoverview.alloc4 + iclbL161SpecialAccountDetailPersonOverview.Sum(x => x.icdoIapallocationDetailPersonoverview.total_payment);
                            if (idecLocal161SPABalanceAmt > 0)
                            {
                                iintYear = (int)iclbL161SpecialAccountDetailPersonOverview.LastOrDefault().icdoIapallocationDetailPersonoverview.computational_year;
                            }
                        }
                    }
                    else if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.istrPlanCode == busConstant.Local_600) //rid 78456
                    {
                        iintTotalHealthYrs = lbenlocal.Last().HealthYear;
                        idecTotalHealthHrs = lbenlocal.Sum(x => x.HealthHour);

                        iintTotalQYrs = ibusAnnualBenefitSummaryOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().qualified_years_count;
                        idecTotalCreditHrs = (from items in ibusAnnualBenefitSummaryOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI select items.qualified_hours).Sum();

                        idecAcrdBenefitLocal = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.unreduced_benefit_amount;
                    }
                    else if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.istrPlanCode == busConstant.Local_666) //rid 78456
                    {
                        iintTotalHealthYrs = lbenlocal.Last().HealthYear;
                        idecTotalHealthHrs = lbenlocal.Sum(x => x.HealthHour);

                        iintTotalQYrs = ibusAnnualBenefitSummaryOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().qualified_years_count;
                        idecTotalCreditHrs = (from items in ibusAnnualBenefitSummaryOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI select items.qualified_hours).Sum();

                        idecAcrdBenefitLocal = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.unreduced_benefit_amount;
                    }
                    else if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.istrPlanCode == busConstant.LOCAL_700) //rid 78456
                    {
                        iintTotalHealthYrs = lbenlocal.Last().HealthYear;
                        idecTotalHealthHrs = lbenlocal.Sum(x => x.HealthHour);

                        iintTotalQYrs = ibusAnnualBenefitSummaryOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().qualified_years_count;
                        idecTotalCreditHrs = (from items in ibusAnnualBenefitSummaryOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI select items.qualified_hours).Sum(); // - lbenlocal.Sum(x => x.TempQHour);

                        idecAcrdBenefitLocal = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.unreduced_benefit_amount;
                    }
                }
                this.ibusPerson.LoadBeneficiaries();
                int lintSpousePersonID = 0;
                foreach (busPersonAccountBeneficiary lbusPersonAccountBeneficiary in this.ibusPerson.iclbPersonAccountBeneficiary)
                {
                    if (lbusPersonAccountBeneficiary.ibusRelationship.icdoRelationship.relationship_value == "SPOU")
                    {
                        lintSpousePersonID = lbusPersonAccountBeneficiary.ibusRelationship.icdoRelationship.person_id;
                    }
                }

                busPerson lbusSpouse = new busPerson { icdoPerson = new cdoPerson() };
                lbusSpouse.FindPerson(lintSpousePersonID);
                //PIR 916
                if (lbusSpouse.icdoPerson.date_of_birth != DateTime.MinValue)
                {
                    //iintSpouseAge = busGlobalFunctions.AgeInYearsAsOfDate(lbusSpouse.icdoPerson.date_of_birth, this.icdoBenefitCalculationHeader.retirement_date);
                    iintSpouseAge = busGlobalFunctions.AgeInYearsAsOfDate(lbusSpouse.icdoPerson.date_of_birth, DateTime.Now);
                }

                this.iclbbusBenefitCalculationOptions = new Collection<busBenefitCalculationOptions>();
                string lstrRetirementType = string.Empty;
                //PIR 916
                //int lintPartcipantage = busGlobalFunctions.AgeInYearsAsOfDate(this.ibusBenefitApplication.ibusPerson.icdoPerson.idtDateofBirth, this.icdoBenefitCalculationHeader.retirement_date);
                int lintPartcipantage = busGlobalFunctions.AgeInYearsAsOfDate(this.ibusBenefitApplication.ibusPerson.icdoPerson.idtDateofBirth, DateTime.Now);
                iintParticipantAge = lintPartcipantage;
                int lintSpouseAge = 0;
                if (this.ibusPerson.ibusBeneficiaryPerson != null)
                {
                    lintSpouseAge = busGlobalFunctions.AgeInYearsAsOfDate(this.ibusPerson.ibusBeneficiaryPerson.icdoPerson.date_of_birth, this.icdoBenefitCalculationHeader.retirement_date);
                }
                //  this.ibusBenefitApplication.ibusPerson.// if (lbPersonAccountOverview.istrPlanCode == "MPIPP" && lbPersonAccountOverview.istrVested == true)
                int lintPersonAccountId = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).First().icdoPersonAccount.person_account_id;
            }

            //rid 79506
            if (astrTemplateName == busConstant.MEDICARE_COORDINATION_ACKNOWLEDGEMENT || astrTemplateName == busConstant.MD_TO_RD_PACKET
                || astrTemplateName == busConstant.Retirement_Benefit_Election_Packet_MPI || astrTemplateName == busConstant.Disability_Benefit_Election_Packet_MPI)
            {
                iintTotalHealthYrs = 0;
                idecTotalHealthHrs = 0.0M;
                busBenefitCalculationDetail lbusBenefitCalculationDetail = iclbBenefitCalculationDetail.FirstOrDefault();
                if (lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.Count() > 0)
                {
                    iintTotalHealthYrs = lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.Last().icdoBenefitCalculationYearlyDetail.health_years_count;
                    idecTotalHealthHrs = lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.Sum(item => item.icdoBenefitCalculationYearlyDetail.health_hours);
                }
                else
                {
                    throw new Exception("Eligible Health data not available for calc id = " + lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.benefit_calculation_header_id);
                }
            }
        }

        public void FillBeneficiaryDetails()
        {
            this.ibusBeneficiary = new busPerson { icdoPerson = new cdoPerson() };
            this.ibusBeneficiary.FindPerson(this.icdoBenefitCalculationHeader.beneficiary_person_id);
            this.ibusBeneficiary.LoadInitialData();
            this.ibusBeneficiary.LoadCorrAddress();

            if (Convert.ToInt32(this.ibusBeneficiary.ibusPersonAddressForCorr.icdoPersonAddress.addr_country_value) == busConstant.USA)
            {
                istrIsUSA = "1";
            }
        }

        public void GetLumpSumYearByAge(decimal adecParticipantAge)
        {
            DataTable ldtbProvisionLumpSumYear = busBase.Select("cdoBenefitProvisionLumpsumFactor.GetLumpSumYearByAge", new object[] { adecParticipantAge });
            iintLumpSumYearByAge = Convert.ToInt32(ldtbProvisionLumpSumYear.Rows[0][0]);
        }

        public string AppendDoller(decimal adecAmount)
        {
            //iblnNoBeneficiaryExists = !ibusCalculation.CheckIfSurvivorIsQualifiedSpouse(this.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id);
            //  if (iblnNoBeneficiaryExists != true)
            //{
            if (adecAmount.IsNotNull() || adecAmount != 0)
            {
                string lstrAmount = String.Format("{0:c}", adecAmount);
                return lstrAmount;
            }
            else
            {
                return "N/A";
            }
            //}
            //return "n/a";
        }

        public void SetWFVariables4PayeeAccount(int aintPayeeAccountId, int aintPlanId, bool ablnEEFlag = false, bool ablnUVHPFlag = false, bool ablnL52SplAccFlag = false, bool ablnL161SplAccFlag = false)
        {
            switch (aintPlanId)
            {
                case busConstant.MPIPP_PLAN_ID:
                    if (ablnEEFlag && ablnUVHPFlag)
                    {
                        iintEEUVHPPayeeAccountID = aintPayeeAccountId;
                        astrFundName = "EEUVHP";
                        iblnEEUVHPPayeeAccountCreated = true;
                    }
                    else
                    {
                        iintMPIPayeeAccountID = aintPayeeAccountId;
                        iblnMPIPayeeAccountCreated = true;
                    }
                    break;

                case busConstant.IAP_PLAN_ID:
                    if (ablnL52SplAccFlag)
                    {
                        iintL52SplAccPayeeAccountID = aintPayeeAccountId;
                        astrFundName = "L52SPLACC";
                        iblnL52SpecialAccPayeeAccountCreated = true;
                    }
                    else if (ablnL161SplAccFlag)
                    {
                        iintL161SplAccPayeeAccountID = aintPayeeAccountId;
                        astrFundName = "L161SPLACC";
                        iblnL161SpecialAccPayeeAccountCreated = true;
                    }
                    else
                    {
                        iintIAPPayeeAccountID = aintPayeeAccountId;
                        iblnIAPPayeeAccntCreated = true;
                    }
                    break;

                case busConstant.LOCAL_52_PLAN_ID:
                    iintL52PayeeAccountID = aintPayeeAccountId;
                    break;

                case busConstant.LOCAL_161_PLAN_ID:
                    iintL161PayeeAccountID = aintPayeeAccountId;
                    break;

                case busConstant.LOCAL_600_PLAN_ID:
                    iintL600PayeeAccountID = aintPayeeAccountId;
                    break;

                case busConstant.LOCAL_666_PLAN_ID:
                    iintL666PayeeAccountID = aintPayeeAccountId;
                    break;

                case busConstant.LOCAL_700_PLAN_ID:
                    iintL700PayeeAccountID = aintPayeeAccountId;
                    break;
            }

        }


        #endregion

        public string istrSurvivorFullName { get; set; }

        public string istrSurvivorLastName { get; set; }

        public string istrSurvivorPrefix { get; set; }

        public string istrSurvivorMPID { get; set; }
        public string istrSurvivorFullNameInProperCase { get; set; }
        public void SetDefaultValues()
        {

            istrParticipantTwoYearBenefitAmt = "N/A";
            istrSurvivorJSurvivor66_2by3BenefitAmt = "N/A";
            istrParticipantJSurvivor50BenefitAmt = "N/A";
            istrParticipantJPop50BenefitAmt = "N/A";
            istrParticipantJSurvivor75BenefitAmt = "N/A";
            istrParticipantJSurvivor100BenefitAmt = "N/A";
            istrSurvivorJSurvivor75BenefitAmt = "N/A";
            istrSurvivorJPop50BenefitAmt = "N/A";
            istrSurvivorJSurvivor50BenefitAmt = "N/A";
            istrSurvivorJSurvivor100BenefitAmt = "N/A";
            istrParticipantJPopup100BenefitAmt = "N/A";
            istrSurvivorJPopup100BenefitAmt = "N/A";
            istrParticipantTenYearBenefitAmt = "N/A";
            istrSingleParticipantTenYearBenefitAmt = "N/A"; //rid78872
            istrParticipantJSurvivor66_2by3BenefitAmt = "N/A";
            istrParticipantJLevelIncome = "N/A";
            istrParticipantLumpSumBenefitAmount = "N/A";
            istrParticipantLifeAnnuityBenefitAmtForPOPUP = "N/A";
            istrLifeAnnuityRelativeValue = "N/A";
            istrJSPop50RelativeValue = "N/A";
            istrJS75RelativeValue = "N/A";
            istrJS100RelativeValue = "N/A";
            istrJS100PopRelativeValue = "N/A";
            istrTenCertainRelativeValue = "N/A";
            istrTwoCertainRelativeValue = "N/A";
            istrJS66_2by3RelativeValue = "N/A";
            istrLevelIncome = "N/A";
            istrFiveYearRelativeValue = "N/A";
            istrParticipantLumpSumBenefitAmt = "N/A";
            istrParticipantLifeAnnuityBenefitAmt = "N/A";
            istrParticipantUVHP = "N/A";
            istrParticipantFiveYearBenefitAmt = "N/A";
            istrParticipant3YearBenefitAmt = "N/A";


            //IAP Plan Props
            istrParticipantIAPLifeAnnuityBenefitAmt = "N/A";
            istrSurvivorIAPJSurvivor50BenefitAmt = "N/A";
            istrParticipantIAPJSurvivor50BenefitAmt = "N/A";
            istrParticipantIAPJPop50BenefitAmt = "N/A";
            istrSurvivorIAPJPop50BenefitAmt = "N/A";
            istrParticipantIAPJSurvivor100BenefitAmt = "N/A";
            istrSurvivorIAPJSurvivor100BenefitAmt = "N/A";
            istrParticipantIAPJPopup100BenefitAmt = "N/A";
            istrSurvivorIAPJPopup100BenefitAmt = "N/A";
            istrParticipantIAPJSurvivor75BenefitAmt = "N/A";
            istrSurvivorIAPJSurvivor75BenefitAmt = "N/A";
            istrSurvivorIAPLifeAnnuityBenefitAmt = "N/A";
            istrParticipantIAPTenYearBenefitAmt = "N/A";
            istrParticipantIAPLifeAnnuityBenefitAmtForPOPUP = "N/A";

            istrPartIAPAnnuityJPop50BenefitAmt = "N/A";
            istrPartAnnuityJPop100BenefitAmt = "N/A";
            istrPartIAPAnnuityJPop100BenefitAmt = "N/A";

            istrPartLifeAnnuityL52_L161SpAcnt = "N/A";
            istrPartJS50L52_L161SpAcnt = "N/A";
            istrPartJS75L52_L161SpAcnt = "N/A";
            istrPartLumpSumL52_L161SpAcnt = "N/A";
        }

        /// <summary>
        /// Method is only used for template Retr-0033 and payee-0035
        /// </summary>
        /// <param name="astrTemplateName"></param>
        private void FillAmountForLumpSumDistribution()
        {
            int lintPlanBenefitId = 0;
            if (istrPlan.IsNullOrEmpty())
            {
                if (icdoBenefitCalculationHeader.iintPlanId == busConstant.MPIPP_PLAN_ID || icdoBenefitCalculationHeader.iintPlanId == busConstant.LOCAL_161_PLAN_ID ||
                    icdoBenefitCalculationHeader.iintPlanId == busConstant.LOCAL_600_PLAN_ID || icdoBenefitCalculationHeader.iintPlanId == busConstant.LOCAL_600_PLAN_ID ||
                    icdoBenefitCalculationHeader.iintPlanId == busConstant.LOCAL_666_PLAN_ID || icdoBenefitCalculationHeader.iintPlanId == busConstant.LOCAL_52_PLAN_ID ||
                    icdoBenefitCalculationHeader.iintPlanId == busConstant.LOCAL_700_PLAN_ID)
                {
                    istrPlan = busConstant.MPIPP;
                }
                else if (icdoBenefitCalculationHeader.iintPlanId == busConstant.IAP_PLAN_ID)
                {
                    istrPlan = busConstant.IAP;
                }
            }

            if (istrPlan.IsNotNullOrEmpty() && (istrPlan.Contains(busConstant.MPIPP) || istrPlan.Contains("EE/UVHP")))
            {
                Collection<busBenefitCalculationOptions> lclbBenefitCalculationOptions = new Collection<busBenefitCalculationOptions>();

                #region set Benefit option depending on plan

                if (icdoBenefitCalculationHeader.iintPlanId == busConstant.MPIPP_PLAN_ID)
                {
                    lclbBenefitCalculationOptions =
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().iclbBenefitCalculationOptions.ToList().ToCollection();

                    lintPlanBenefitId = ibusCalculation.GetPlanBenefitId(busConstant.MPIPP_PLAN_ID, busConstant.LUMP_SUM);
                }
                else if (icdoBenefitCalculationHeader.iintPlanId == busConstant.LOCAL_161_PLAN_ID)
                {
                    lclbBenefitCalculationOptions =
                       this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_161_PLAN_ID).FirstOrDefault().iclbBenefitCalculationOptions.ToList().ToCollection();

                    lintPlanBenefitId = ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_161_PLAN_ID, busConstant.LUMP_SUM);
                }
                else if (icdoBenefitCalculationHeader.iintPlanId == busConstant.LOCAL_600_PLAN_ID)
                {
                    lclbBenefitCalculationOptions =
                       this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_600_PLAN_ID).FirstOrDefault().iclbBenefitCalculationOptions.ToList().ToCollection();

                    lintPlanBenefitId = ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_600_PLAN_ID, busConstant.LUMP_SUM);
                }
                else if (icdoBenefitCalculationHeader.iintPlanId == busConstant.LOCAL_666_PLAN_ID)
                {
                    lclbBenefitCalculationOptions =
                       this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_666_PLAN_ID).FirstOrDefault().iclbBenefitCalculationOptions.ToList().ToCollection();

                    lintPlanBenefitId = ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_666_PLAN_ID, busConstant.LUMP_SUM);
                }
                else if (icdoBenefitCalculationHeader.iintPlanId == busConstant.LOCAL_52_PLAN_ID)
                {
                    lclbBenefitCalculationOptions =
                       this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_52_PLAN_ID).FirstOrDefault().iclbBenefitCalculationOptions.ToList().ToCollection();

                    lintPlanBenefitId = ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_52_PLAN_ID, busConstant.LUMP_SUM);
                }
                else if (icdoBenefitCalculationHeader.iintPlanId == busConstant.LOCAL_700_PLAN_ID)
                {
                    lclbBenefitCalculationOptions =
                       this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_700_PLAN_ID).FirstOrDefault().iclbBenefitCalculationOptions.ToList().ToCollection();

                    lintPlanBenefitId = ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_700_PLAN_ID, busConstant.LUMP_SUM);
                }

                #endregion

                if (lclbBenefitCalculationOptions.Where(option => option.icdoBenefitCalculationOptions.plan_benefit_id == lintPlanBenefitId && option.icdoBenefitCalculationOptions.ee_flag.IsNullOrEmpty() &&
                            option.icdoBenefitCalculationOptions.uvhp_flag.IsNullOrEmpty()).Count() > 0 && istrPlan.Contains("MPIPP"))
                {
                    if (icdoBenefitCalculationHeader.benefit_type_value == busConstant.BENEFIT_TYPE_DISABILITY)
                    {
                        idecParticipantLumpSumBenefitAmount = lclbBenefitCalculationOptions.Where(option =>
                            option.icdoBenefitCalculationOptions.plan_benefit_id == lintPlanBenefitId && option.icdoBenefitCalculationOptions.ee_flag.IsNullOrEmpty() &&
                            option.icdoBenefitCalculationOptions.uvhp_flag.IsNullOrEmpty()).FirstOrDefault().icdoBenefitCalculationOptions.participant_amount;
                        istrParticipantLumpSumBenefitAmount = AppendDoller(idecParticipantLumpSumBenefitAmount);
                    }
                    else if (icdoBenefitCalculationHeader.benefit_type_value == busConstant.BENEFIT_TYPE_DEATH_PRE_RETIREMENT)
                    {
                        idecParticipantLumpSumBenefitAmount = lclbBenefitCalculationOptions.Where(option =>
                            option.icdoBenefitCalculationOptions.plan_benefit_id == lintPlanBenefitId && option.icdoBenefitCalculationOptions.ee_flag.IsNullOrEmpty() &&
                            option.icdoBenefitCalculationOptions.uvhp_flag.IsNullOrEmpty()).FirstOrDefault().icdoBenefitCalculationOptions.survivor_amount;
                        istrParticipantLumpSumBenefitAmount = AppendDoller(idecParticipantLumpSumBenefitAmount);
                    }
                    else
                    {
                        idecParticipantLumpSumBenefitAmount = lclbBenefitCalculationOptions.Where(option =>
                           option.icdoBenefitCalculationOptions.plan_benefit_id == lintPlanBenefitId && option.icdoBenefitCalculationOptions.ee_flag.IsNullOrEmpty() &&
                           option.icdoBenefitCalculationOptions.uvhp_flag.IsNullOrEmpty()).FirstOrDefault().icdoBenefitCalculationOptions.benefit_amount;
                        istrParticipantLumpSumBenefitAmount = AppendDoller(idecParticipantLumpSumBenefitAmount);
                    }
                }
                else if (lclbBenefitCalculationOptions.Where(option => option.icdoBenefitCalculationOptions.plan_benefit_id == lintPlanBenefitId &&
                           option.icdoBenefitCalculationOptions.uvhp_flag == busConstant.FLAG_YES).Count() > 0 && istrPlan.Contains("EE/UVHP"))
                {
                    if (icdoBenefitCalculationHeader.benefit_type_value == busConstant.BENEFIT_TYPE_DISABILITY)
                    {
                        idecParticipantLumpSumBenefitAmount = lclbBenefitCalculationOptions.Where(option =>
                            option.icdoBenefitCalculationOptions.plan_benefit_id == lintPlanBenefitId &&
                            option.icdoBenefitCalculationOptions.uvhp_flag == busConstant.FLAG_YES).FirstOrDefault().icdoBenefitCalculationOptions.participant_amount;
                        istrParticipantLumpSumBenefitAmount = AppendDoller(idecParticipantLumpSumBenefitAmount);
                    }
                    else if (icdoBenefitCalculationHeader.benefit_type_value == busConstant.BENEFIT_TYPE_DEATH_PRE_RETIREMENT)
                    {
                        idecParticipantLumpSumBenefitAmount = lclbBenefitCalculationOptions.Where(option =>
                            option.icdoBenefitCalculationOptions.plan_benefit_id == lintPlanBenefitId &&
                            option.icdoBenefitCalculationOptions.uvhp_flag == busConstant.FLAG_YES).FirstOrDefault().icdoBenefitCalculationOptions.survivor_amount;
                        istrParticipantLumpSumBenefitAmount = AppendDoller(idecParticipantLumpSumBenefitAmount);
                    }
                    else
                    {
                        idecParticipantLumpSumBenefitAmount = lclbBenefitCalculationOptions.Where(option =>
                           option.icdoBenefitCalculationOptions.plan_benefit_id == lintPlanBenefitId &&
                           option.icdoBenefitCalculationOptions.uvhp_flag == busConstant.FLAG_YES).FirstOrDefault().icdoBenefitCalculationOptions.benefit_amount;
                        istrParticipantLumpSumBenefitAmount = AppendDoller(idecParticipantLumpSumBenefitAmount);
                    }
                }
            }
            else if (istrPlan.IsNotNullOrEmpty() && istrPlan.Contains(busConstant.IAP) &&
                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).Count() > 0)
            {
                Collection<busBenefitCalculationOptions> lclbBenefitCalculationOptions = new Collection<busBenefitCalculationOptions>();

                lclbBenefitCalculationOptions =
                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().iclbBenefitCalculationOptions.ToList().ToCollection();

                lintPlanBenefitId = ibusCalculation.GetPlanBenefitId(busConstant.IAP_PLAN_ID, busConstant.LUMP_SUM);
                if (lclbBenefitCalculationOptions.Where(option => option.icdoBenefitCalculationOptions.plan_benefit_id == lintPlanBenefitId &&
                            option.icdoBenefitCalculationOptions.local161_special_acct_bal_flag.IsNullOrEmpty() &&
                            option.icdoBenefitCalculationOptions.local52_special_acct_bal_flag.IsNullOrEmpty()).Count() > 0)
                {
                    if (icdoBenefitCalculationHeader.benefit_type_value == busConstant.BENEFIT_TYPE_DISABILITY)
                    {
                        idecParticipantLumpSumBenefitAmount = lclbBenefitCalculationOptions.Where(option =>
                            option.icdoBenefitCalculationOptions.plan_benefit_id == lintPlanBenefitId &&
                            option.icdoBenefitCalculationOptions.local161_special_acct_bal_flag.IsNullOrEmpty() &&
                            option.icdoBenefitCalculationOptions.local52_special_acct_bal_flag.IsNullOrEmpty()).FirstOrDefault().icdoBenefitCalculationOptions.participant_amount;
                        istrParticipantLumpSumBenefitAmount = AppendDoller(idecParticipantLumpSumBenefitAmount);
                    }
                    else if (icdoBenefitCalculationHeader.benefit_type_value == busConstant.BENEFIT_TYPE_DEATH_PRE_RETIREMENT)
                    {
                        idecParticipantLumpSumBenefitAmount = lclbBenefitCalculationOptions.Where(option =>
                            option.icdoBenefitCalculationOptions.plan_benefit_id == lintPlanBenefitId &&
                             option.icdoBenefitCalculationOptions.local161_special_acct_bal_flag.IsNullOrEmpty() &&
                            option.icdoBenefitCalculationOptions.local52_special_acct_bal_flag.IsNullOrEmpty()).FirstOrDefault().icdoBenefitCalculationOptions.survivor_amount;
                        istrParticipantLumpSumBenefitAmount = AppendDoller(idecParticipantLumpSumBenefitAmount);
                    }
                    else
                    {
                        idecParticipantLumpSumBenefitAmount = lclbBenefitCalculationOptions.Where(option =>
                           option.icdoBenefitCalculationOptions.plan_benefit_id == lintPlanBenefitId &&
                            option.icdoBenefitCalculationOptions.local161_special_acct_bal_flag.IsNullOrEmpty() &&
                            option.icdoBenefitCalculationOptions.local52_special_acct_bal_flag.IsNullOrEmpty()).FirstOrDefault().icdoBenefitCalculationOptions.benefit_amount;
                        istrParticipantLumpSumBenefitAmount = AppendDoller(idecParticipantLumpSumBenefitAmount);
                    }
                }
            }
            else if (istrPlan.IsNotNullOrEmpty() && istrPlan.Contains("161") &&
                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).Count() > 0)
            {
                Collection<busBenefitCalculationOptions> lclbBenefitCalculationOptions = new Collection<busBenefitCalculationOptions>();

                lclbBenefitCalculationOptions =
                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().iclbBenefitCalculationOptions.ToList().ToCollection();

                lintPlanBenefitId = ibusCalculation.GetPlanBenefitId(busConstant.IAP_PLAN_ID, busConstant.LUMP_SUM);
                if (lclbBenefitCalculationOptions.Where(option => option.icdoBenefitCalculationOptions.plan_benefit_id == lintPlanBenefitId &&
                            option.icdoBenefitCalculationOptions.local161_special_acct_bal_flag == busConstant.FLAG_YES &&
                            option.icdoBenefitCalculationOptions.local52_special_acct_bal_flag.IsNullOrEmpty()).Count() > 0)
                {
                    if (icdoBenefitCalculationHeader.benefit_type_value == busConstant.BENEFIT_TYPE_DISABILITY)
                    {
                        idecParticipantLumpSumBenefitAmount = lclbBenefitCalculationOptions.Where(option =>
                            option.icdoBenefitCalculationOptions.plan_benefit_id == lintPlanBenefitId &&
                            option.icdoBenefitCalculationOptions.local161_special_acct_bal_flag == busConstant.FLAG_YES &&
                            option.icdoBenefitCalculationOptions.local52_special_acct_bal_flag.IsNullOrEmpty()).FirstOrDefault().icdoBenefitCalculationOptions.participant_amount;
                        istrParticipantLumpSumBenefitAmount = AppendDoller(idecParticipantLumpSumBenefitAmount);
                    }
                    else if (icdoBenefitCalculationHeader.benefit_type_value == busConstant.BENEFIT_TYPE_DEATH_PRE_RETIREMENT)
                    {
                        idecParticipantLumpSumBenefitAmount = lclbBenefitCalculationOptions.Where(option =>
                            option.icdoBenefitCalculationOptions.plan_benefit_id == lintPlanBenefitId &&
                             option.icdoBenefitCalculationOptions.local161_special_acct_bal_flag == busConstant.FLAG_YES &&
                            option.icdoBenefitCalculationOptions.local52_special_acct_bal_flag.IsNullOrEmpty()).FirstOrDefault().icdoBenefitCalculationOptions.survivor_amount;
                        istrParticipantLumpSumBenefitAmount = AppendDoller(idecParticipantLumpSumBenefitAmount);
                    }
                    else
                    {
                        idecParticipantLumpSumBenefitAmount = lclbBenefitCalculationOptions.Where(option =>
                           option.icdoBenefitCalculationOptions.plan_benefit_id == lintPlanBenefitId &&
                            option.icdoBenefitCalculationOptions.local161_special_acct_bal_flag == busConstant.FLAG_YES &&
                            option.icdoBenefitCalculationOptions.local52_special_acct_bal_flag.IsNullOrEmpty()).FirstOrDefault().icdoBenefitCalculationOptions.benefit_amount;
                        istrParticipantLumpSumBenefitAmount = AppendDoller(idecParticipantLumpSumBenefitAmount);
                    }
                }
            }
            else if (istrPlan.IsNotNullOrEmpty() && istrPlan.Contains("52") &&
                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).Count() > 0)
            {
                Collection<busBenefitCalculationOptions> lclbBenefitCalculationOptions = new Collection<busBenefitCalculationOptions>();

                lclbBenefitCalculationOptions =
                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().iclbBenefitCalculationOptions.ToList().ToCollection();

                lintPlanBenefitId = ibusCalculation.GetPlanBenefitId(busConstant.IAP_PLAN_ID, busConstant.LUMP_SUM);
                if (lclbBenefitCalculationOptions.Where(option => option.icdoBenefitCalculationOptions.plan_benefit_id == lintPlanBenefitId &&
                            option.icdoBenefitCalculationOptions.local161_special_acct_bal_flag.IsNullOrEmpty() &&
                            option.icdoBenefitCalculationOptions.local52_special_acct_bal_flag == busConstant.FLAG_YES).Count() > 0)
                {
                    if (icdoBenefitCalculationHeader.benefit_type_value == busConstant.BENEFIT_TYPE_DISABILITY)
                    {
                        idecParticipantLumpSumBenefitAmount = lclbBenefitCalculationOptions.Where(option =>
                            option.icdoBenefitCalculationOptions.plan_benefit_id == lintPlanBenefitId &&
                            option.icdoBenefitCalculationOptions.local161_special_acct_bal_flag.IsNullOrEmpty() &&
                            option.icdoBenefitCalculationOptions.local52_special_acct_bal_flag == busConstant.FLAG_YES).FirstOrDefault().icdoBenefitCalculationOptions.participant_amount;
                        istrParticipantLumpSumBenefitAmount = AppendDoller(idecParticipantLumpSumBenefitAmount);
                    }
                    else if (icdoBenefitCalculationHeader.benefit_type_value == busConstant.BENEFIT_TYPE_DEATH_PRE_RETIREMENT)
                    {
                        idecParticipantLumpSumBenefitAmount = lclbBenefitCalculationOptions.Where(option =>
                            option.icdoBenefitCalculationOptions.plan_benefit_id == lintPlanBenefitId &&
                             option.icdoBenefitCalculationOptions.local161_special_acct_bal_flag.IsNullOrEmpty() &&
                            option.icdoBenefitCalculationOptions.local52_special_acct_bal_flag == busConstant.FLAG_YES).FirstOrDefault().icdoBenefitCalculationOptions.survivor_amount;
                        istrParticipantLumpSumBenefitAmount = AppendDoller(idecParticipantLumpSumBenefitAmount);
                    }
                    else
                    {
                        idecParticipantLumpSumBenefitAmount = lclbBenefitCalculationOptions.Where(option =>
                           option.icdoBenefitCalculationOptions.plan_benefit_id == lintPlanBenefitId &&
                            option.icdoBenefitCalculationOptions.local161_special_acct_bal_flag.IsNullOrEmpty() &&
                            option.icdoBenefitCalculationOptions.local52_special_acct_bal_flag == busConstant.FLAG_YES).FirstOrDefault().icdoBenefitCalculationOptions.benefit_amount;
                        istrParticipantLumpSumBenefitAmount = AppendDoller(idecParticipantLumpSumBenefitAmount);
                    }
                }
            }
        }

        #region // PROD PIR 127
        public void CreatePayeeAccountForRetireeIncrease(busPayeeAccount abusPayeeAccount, int aintBenefitAccountID, string astrFamilyRelationshipValue, decimal adecNonTaxableBeginningBalance,
                                                    DateTime adtNextPaymentDate, int aintEarlyRetirementBenefitApplicationID, string astrBenefitAccountTypeValue, string astrAccountRelationshipType = null)
        {

            string lstrAccountRelationShipType = string.Empty;
            bool ablnCreatePayeeAccount;

            //Load all existing retiree inc payee acccount
            busPlanBenefitXr lbusPlanBenefitXr = new busPlanBenefitXr();
            busPayeeAccount lbusPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
            Collection<busPayeeAccount> lclbEarlyRetirementRetireIncPayeeAccount = new Collection<busPayeeAccount>();
            int lintPayeeAccountID = 0;

            if (aintEarlyRetirementBenefitApplicationID > 0)
            {
                lclbEarlyRetirementRetireIncPayeeAccount = lbusPayeeAccount.LoadRetireeIncFromAppDetail(this.icdoBenefitCalculationHeader.payee_account_id);
            }

            if (astrAccountRelationshipType.IsNotNullOrEmpty())
            {
                lstrAccountRelationShipType = astrAccountRelationshipType;
            }
            else
            {
                lstrAccountRelationShipType = busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER;
            }

            if (iclbDisabilityRetireeIncrease.IsNullOrEmpty() || iclbDisabilityRetireeIncrease == null)
                this.LoadDisabilityRetireeIncreases();

            if (iclbBenefitCalculationDetail.IsNullOrEmpty() || iclbBenefitCalculationDetail == null)
                this.LoadBenefitCalculationDetails();

            DataTable ldtbResult = new DataTable();


            foreach (busDisabilityRetireeIncrease lbusDisabilityRetireeIncrease in iclbDisabilityRetireeIncrease)
            {
                ablnCreatePayeeAccount = true;

                //Ticket - 71304
                lintPayeeAccountID = 0;

                if (abusPayeeAccount.icdoPayeeAccount.org_id == 0)
                {
                    if (abusPayeeAccount.icdoPayeeAccount.dro_application_detail_id != 0)
                        ldtbResult = busBase.Select<cdoPayeeAccount>(
                                        new string[7] { "PERSON_ID", "ACCOUNT_RELATION_VALUE", "PAYEE_BENEFIT_ACCOUNT_ID", "BENEFIT_ACCOUNT_TYPE_VALUE", "dro_application_detail_id", enmPayeeAccount.retiree_incr_flag.ToString().ToUpper(), enmPayeeAccount.benefit_begin_date.ToString().ToUpper() },
                                        new object[7] { abusPayeeAccount.icdoPayeeAccount.person_id, abusPayeeAccount.icdoPayeeAccount.account_relation_value,
                                                    abusPayeeAccount.icdoPayeeAccount.payee_benefit_account_id, abusPayeeAccount.icdoPayeeAccount.benefit_account_type_value,
                                                    abusPayeeAccount.icdoPayeeAccount.dro_application_detail_id, "Y", lbusDisabilityRetireeIncrease.icdoDisabilityRetireeIncrease.retiree_increase_date }, null, null);
                    else
                        ldtbResult = busBase.Select<cdoPayeeAccount>(
                                    new string[6] { "PERSON_ID", "ACCOUNT_RELATION_VALUE", "PAYEE_BENEFIT_ACCOUNT_ID", "BENEFIT_ACCOUNT_TYPE_VALUE", enmPayeeAccount.retiree_incr_flag.ToString().ToUpper(), enmPayeeAccount.benefit_begin_date.ToString().ToUpper() },
                                    new object[6] { abusPayeeAccount.icdoPayeeAccount.person_id, abusPayeeAccount.icdoPayeeAccount.account_relation_value,
                                                    abusPayeeAccount.icdoPayeeAccount.payee_benefit_account_id, abusPayeeAccount.icdoPayeeAccount.benefit_account_type_value,
                                                    "Y", lbusDisabilityRetireeIncrease.icdoDisabilityRetireeIncrease.retiree_increase_date }, null, null);
                }
                else
                {
                    ldtbResult = busBase.Select<cdoPayeeAccount>(
                                new string[6] { "ORG_ID", "ACCOUNT_RELATION_VALUE", "PAYEE_BENEFIT_ACCOUNT_ID", "BENEFIT_ACCOUNT_TYPE_VALUE", enmPayeeAccount.retiree_incr_flag.ToString().ToUpper(), enmPayeeAccount.benefit_begin_date.ToString().ToUpper() },
                                new object[6] { abusPayeeAccount.icdoPayeeAccount.person_id, abusPayeeAccount.icdoPayeeAccount.account_relation_value,
                                                abusPayeeAccount.icdoPayeeAccount.payee_benefit_account_id, abusPayeeAccount.icdoPayeeAccount.benefit_account_type_value,
                                                "Y", lbusDisabilityRetireeIncrease.icdoDisabilityRetireeIncrease.retiree_increase_date }, null, null);
                }

                foreach (DataRow dr in ldtbResult.Rows)
                {
                    busPayeeAccount lbusTempPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
                    lbusTempPayeeAccount.icdoPayeeAccount.LoadData(dr);

                    if (lbusTempPayeeAccount.ibusPayee.IsNull())
                    {
                        lbusTempPayeeAccount.ibusPayee = new busPerson { icdoPerson = new cdoPerson() };
                        lbusTempPayeeAccount.ibusPayee.FindPerson(abusPayeeAccount.icdoPayeeAccount.person_id);
                    }

                    if (lbusTempPayeeAccount.ibusCurrentActivePayeeAccount.IsNull())
                        lbusTempPayeeAccount.ibusCurrentActivePayeeAccount = new busPayeeAccountStatus { icdoPayeeAccountStatus = new cdoPayeeAccountStatus() };

                    lbusTempPayeeAccount.LoadPayeeAccountStatuss();

                    if (!lbusTempPayeeAccount.iclbPayeeAccountStatus.IsNullOrEmpty())
                    {
                        lbusTempPayeeAccount.ibusCurrentActivePayeeAccount = lbusTempPayeeAccount.iclbPayeeAccountStatus[0];


                        if (lbusTempPayeeAccount.ibusCurrentActivePayeeAccount.IsStatusCompleted())
                        {
                            object lobjActiveDistributionRecords = null;
                            int lintActiveDistributionRecords = 0;

                            if (lbusTempPayeeAccount.ibusCurrentActivePayeeAccount.IsStatusCompleted())
                            {
                                lobjActiveDistributionRecords = DBFunction.DBExecuteScalar("cdoPaymentHistoryHeader.GetActiveDistributionRecordFromPayeeAccnt",
                                                                    new object[1] { lbusTempPayeeAccount.icdoPayeeAccount.payee_account_id }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

                                if (lobjActiveDistributionRecords != null)
                                {
                                    lintActiveDistributionRecords = ((int)lobjActiveDistributionRecords);
                                }
                            }

                            if (lintActiveDistributionRecords > 0)
                            {
                                ablnCreatePayeeAccount = false;
                                continue;
                            }
                        }
                        else if (!lbusTempPayeeAccount.ibusCurrentActivePayeeAccount.IsStatusCancelled())
                        {
                            ablnCreatePayeeAccount = true;
                            lintPayeeAccountID = lbusTempPayeeAccount.icdoPayeeAccount.payee_account_id;
                            break;
                        }
                    }
                }

                if (!ablnCreatePayeeAccount)
                    continue;

                int aint_person_ID = 0;
                if (astrBenefitAccountTypeValue == busConstant.BENEFIT_TYPE_DEATH_POST_RETIREMENT || astrBenefitAccountTypeValue == busConstant.BENEFIT_TYPE_DEATH_PRE_RETIREMENT)
                    aint_person_ID = icdoBenefitCalculationHeader.beneficiary_person_id;
                else
                    aint_person_ID = icdoBenefitCalculationHeader.person_id;

                lintPayeeAccountID = lbusPayeeAccount.ManagePayeeAccount(lintPayeeAccountID, abusPayeeAccount.icdoPayeeAccount.person_id, abusPayeeAccount.icdoPayeeAccount.org_id,
                                                                                      iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.benefit_application_detail_id,
                                                                                      iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.benefit_calculation_detail_id,
                                                                                      0, 0, aintBenefitAccountID, astrBenefitAccountTypeValue,
                                                                                      iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.benefit_subtype_value,
                                                                                      lbusDisabilityRetireeIncrease.icdoDisabilityRetireeIncrease.retiree_increase_date,
                                                                                      DateTime.MinValue, lstrAccountRelationShipType, astrFamilyRelationshipValue,
                                                                                      0.0M, adecNonTaxableBeginningBalance,
                                                                                      lbusPlanBenefitXr.GetPlanBenefitId(iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.plan_id, busConstant.LUMP_SUM),
                                                                                      DateTime.MinValue, busConstant.FLAG_YES, aintReferenceId: abusPayeeAccount.icdoPayeeAccount.payee_account_id);

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

                if (ldecAdjustedRetireeIncAmt > 0M)
                {
                    lbusPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.ITEM48, ldecAdjustedRetireeIncAmt, "0", 0,
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

        //PIR 944
        public bool LumpSumPaymentCheckBoxVisiblily()
        {
            if (icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT
                && this.iclbBenefitCalculationDetail != null
                && this.iclbBenefitCalculationDetail[0].icdoBenefitCalculationDetail.plan_id == 1
                && (this.iclbBenefitCalculationDetail[0].iclbBenefitCalculationOptions == null ||
                this.iclbBenefitCalculationDetail[0].iclbBenefitCalculationOptions.Count() == 0 ||
                (this.iclbBenefitCalculationDetail[0].iclbBenefitCalculationOptions.Count() > 0 &&
                this.iclbBenefitCalculationDetail[0].iclbBenefitCalculationOptions[0].icdoBenefitCalculationOptions.plan_benefit_id != 9)))
            {
                return true;
            }

            return false;
        }

        public bool CheckIfManager()
        {
            busUserRoles lbusUserRoles = new busUserRoles { icdoRoles = new cdoRoles() };
            if (lbusUserRoles.FindUserRoles(iobjPassInfo.iintUserSerialID, busConstant.Role.MANAGER_ROLE))
            {
                return true;
            }
            return false;
        }

        public Decimal GetAnnunityMultiplier()
        {
            decimal ldecAnnunityMultiplier = 0.00m;
            DataTable ldtbAnnunityMultiplier = new DataTable();
            ldtbAnnunityMultiplier = busBase.Select("cdoIapAnnunityAdjustmentMultiplier.GetIAPAnnunityMultiplier", new object[0] { });
            if (ldtbAnnunityMultiplier.Rows.Count>0 && !ldtbAnnunityMultiplier.Rows[0][0].IsNull())
            {
                ldecAnnunityMultiplier = Convert.ToDecimal(ldtbAnnunityMultiplier.Rows[0][0]);
            }


            return ldecAnnunityMultiplier;
        }

        public void GetVestedYearsAndHours(string astrTemplateName, int lintAccountId, busAnnualBenefitSummaryOverview abusAnnualBenefitSummaryOverview)
        {
            DataTable ldtbPersonAccountEligibility = busBase.Select("cdoPersonAccountEligibility.GetEligibilityInfoFromAccountID", new object[1] { lintAccountId });
            busPersonAccountEligibility lbusTempPersonAccountEligibility = new busPersonAccountEligibility { icdoPersonAccountEligibility = new cdoPersonAccountEligibility() };
            if ((astrTemplateName == busConstant.RETIREMENT_BENEFIT_ELECTION_FORM_LOCAL_52 || astrTemplateName == busConstant.RETIREMENT_BENEFIT_ELECTION_FORM_LOCAL_161 ||
                astrTemplateName == busConstant.RETIREMENT_BENEFIT_ELECTION_FORM_LOCAL_600 || astrTemplateName == busConstant.RETIREMENT_BENEFIT_ELECTION_FORM_LOCAL_666 ||
                astrTemplateName == busConstant.RETIREMENT_BENEFIT_ELECTION_FORM_LOCAL_700) && ldtbPersonAccountEligibility.Rows.Count > 0)
            {
                lbusTempPersonAccountEligibility.icdoPersonAccountEligibility.LoadData(ldtbPersonAccountEligibility.Rows[0]);
                if (lbusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date.IsNotNull() && lbusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date != DateTime.MinValue)
                {
                    this.ibusBenefitApplication.iintVestedYears = lbusTempPersonAccountEligibility.icdoPersonAccountEligibility.local_qualified_years;
                    this.ibusBenefitApplication.idecVestedHours = lbusTempPersonAccountEligibility.icdoPersonAccountEligibility.local_frozen_hours;
                }
            }

            if ((astrTemplateName == busConstant.Disability_Benefit_Election_Packet_L52 || astrTemplateName == busConstant.Disability_Benefit_Election_Packet_L161 ||
                astrTemplateName == busConstant.Disability_Benefit_Election_Packet_L600 || astrTemplateName == busConstant.Disability_Benefit_Election_Packet_L666 ||
                astrTemplateName == busConstant.Disability_Benefit_Election_Packet_L700) && ldtbPersonAccountEligibility.Rows.Count > 0)
            {
                int lintLocalQualifiedYears = 0;
                decimal ldecLocalFrozenHours = 0;
                int lintMPIQualifiedYears = 0;
                decimal ldecPensionHours = 0;

                int lintOverviewQualifiedYears = 0;
                decimal ldecOverviewQualifiedHours = 0;

                System.Collections.Generic.List<cdoDummyWorkData> llobjDWD = abusAnnualBenefitSummaryOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.ToList();

                foreach (cdoDummyWorkData dwd in llobjDWD)
                {
                    if (dwd.comments.IsNotNullOrEmpty() && dwd.comments.Contains("Frozen Service"))
                    {
                        lintOverviewQualifiedYears = dwd.qualified_years_count;
                        ldecOverviewQualifiedHours = dwd.qualified_hours;
                    }
                }

                lbusTempPersonAccountEligibility.icdoPersonAccountEligibility.LoadData(ldtbPersonAccountEligibility.Rows[0]);
                if (lbusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date.IsNotNull() && lbusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date != DateTime.MinValue)
                {
                    lintLocalQualifiedYears = lbusTempPersonAccountEligibility.icdoPersonAccountEligibility.local_qualified_years;
                    ldecLocalFrozenHours = lbusTempPersonAccountEligibility.icdoPersonAccountEligibility.local_frozen_hours;
                }

                if (lintOverviewQualifiedYears == 0 && ldecOverviewQualifiedHours == 0)
                {
                    lintOverviewQualifiedYears = lintLocalQualifiedYears;
                    ldecOverviewQualifiedHours = ldecLocalFrozenHours;
                }

                string lstrMPID = this.ibusBenefitApplication.ibusPerson.icdoPerson.istrMPID;
                DataTable ldtbMpiCalc = busBase.Select("cdoBenefitCalculationHeader.GetCalcForMPI", new object[1] { lstrMPID });
                if (ldtbMpiCalc.Rows.Count > 0)
                {
                    int lintCalcHeaderId = Convert.ToInt32(ldtbMpiCalc.Rows[0]["BENEFIT_CALCULATION_HEADER_ID"]);
                    int lintCalcDetailId = Convert.ToInt32(ldtbMpiCalc.Rows[0]["BENEFIT_CALCULATION_DETAIL_ID"]);

                    DateTime ldtForfietureDate = new DateTime();

                    busPersonAccountEligibility lbusPersonAccountEligibility = new busPersonAccountEligibility();
                    lbusPersonAccountEligibility = lbusPersonAccountEligibility.LoadPersonAccEligibilityByPersonAccountId(this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().icdoPersonAccount.person_account_id);
                    if (lbusPersonAccountEligibility != null)
                        ldtForfietureDate = lbusPersonAccountEligibility.icdoPersonAccountEligibility.forfeiture_date;

                    busBenefitCalculationDetail lbusBenefitCalculationDetail = new busBenefitCalculationDetail { icdoBenefitCalculationDetail = new cdoBenefitCalculationDetail() };
                    lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.benefit_calculation_header_id = lintCalcHeaderId;
                    lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.benefit_calculation_detail_id = lintCalcDetailId;

                    lbusBenefitCalculationDetail.LoadBenefitCalculationYearlyDetails();
                    lbusBenefitCalculationDetail.LoadBenefitCalculationYearlyDetailsTotal(ldtForfietureDate);

                    lintMPIQualifiedYears = lbusBenefitCalculationDetail.iclbAnnualBenfitSummayOverviewTotal.FirstOrDefault().icdoBenefitCalculationYearlyDetail.iintQualifiedYears;
                    ldecPensionHours = lbusBenefitCalculationDetail.iclbAnnualBenfitSummayOverviewTotal.FirstOrDefault().icdoBenefitCalculationYearlyDetail.idecTotalPensionHours;

                    ibusBenefitApplication.iintVestedYears = lintMPIQualifiedYears;
                    ibusBenefitApplication.idecVestedHours = ldecPensionHours + ldecOverviewQualifiedHours;
                }
                else
                {
                    ibusBenefitApplication.iintVestedYears = lintOverviewQualifiedYears;
                    ibusBenefitApplication.idecVestedHours = ldecOverviewQualifiedHours;
                }
            }
        }
    }

    // rid 78456
    public struct BenLocal
    {
        public int Year { get; set; }
        public int QualifiedYear { get; set; }
        public decimal QualifiedHour { get; set; }
        public int HealthYear { get; set; }

        public decimal HealthHour { get; set; }

        public decimal TempQHour { get; set; }

        public string Comment { get; set; }
    }
}
