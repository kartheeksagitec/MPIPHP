
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
using System.Globalization;    //PIR 916
using NeoSpin.BusinessObjects;
using Sagitec.Bpm;

namespace MPIPHP.BusinessObjects
{
    [Serializable]
    public class busBenefitCalculationRetirement : busBenefitCalculationHeader
    {
        private readonly decimal ldecFiftyCents = Convert.ToDecimal(0.50); //rid 82228

        #region Public Properties

        public Collection<cdoPlanBenefitRate> iclbcdoPlanBenefitRate { get; set; }
        //public decimal idecLumpSumBenefitAmount = busConstant.ZERO_DECIMAL;
        public decimal idecEEContributionAmount = busConstant.ZERO_DECIMAL;
        public decimal idecEEInterestAmount = busConstant.ZERO_DECIMAL;
        public decimal idecQualifiedJointAndSurvivorAnnuity50;
        public decimal idecLateAdjustmentAmount = busConstant.ZERO_DECIMAL;
        public bool lblIsNew { get; set; }
        public decimal idecAgeDiff = busConstant.ZERO_DECIMAL;

        public string istrAsofNowIapBalance { get; set; }
        public int iintPlanYear { get; set; }
        public cdoPerson icdoPerson { get; set; }
        public busBenefitCalculationHeader lbusBenefitCalculationHeader { get; set; }
        public busBenefitApplication ibusBenefitApplicationRetirement { get; set; }
        public Dictionary<int, Dictionary<int, decimal>> idictWorkHrsAfterRetirement { get; set; }
        public Collection<busBenefitCalculationOptions> iclbBenefitCalculationOptions { get; set; }
        public busRetirementApplication ibusRetirementApplication { get; set; }

        

        //Ticket# 68545
        //   public int iintYear { get; set; }
        public string istrMinDistriDate { get; set; }
        public string istr72MinDistriDate { get; set; }  //RID 118418 for PER-0006 and PER-0016
        public string istrMDOptionDueDt { get; set; } //RID 118418 for PER-0016
        public string istrFebDate { get; set; }
        public string istrBenefitOption { get; set; }
        public string istrMarchDt { get; set; } //For PER-0006(RASHMI)

        public string istrCurrentDate { get; set; }
        public string istrRetirementDate { get; set; }
        public string istrPersonAddress { get; set; }

        public string istrNewMergedMPIID { get; set; }
        public int iintCurrentYear { get; set; }
        public string istrBenefitTypeDescription { get; set; }
        public string istrIsVested { get; set; }

        public string istrSurvivorFullName { get; set; }
        public string istrParticipantFullName { get; set; }


        #endregion Public Properties

        #region Public Methods

        public void SetupPreRequisites_RetirementCalculations()
        {
            if (!this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.IsNullOrEmpty())
            {
                this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.ForEach(a => a.icdoPersonAccount.istrRetirementSubType = string.Empty);

                this.ibusBenefitApplication.LoadWorkHistoryandSetupPrerequisites_Retirement();

                //Sid Jain 06252012
                //if (this.lblIsNew == false)
                //{
                //switch (this.icdoBenefitCalculationHeader.benefit_type_value)
                //{
                //    case busConstant.BENEFIT_TYPE_RETIREMENT:
                //        this.ibusBenefitApplication.LoadWorkHistoryandSetupPrerequisites_Retirement();
                //        break;
                //    default:
                //        this.ibusBenefitApplication.LoadWorkHistoryandSetupPrerequisites_Retirement();
                //        break;
                //}
                //}
                //else
                //{
                //    this.ibusBenefitApplication.DetermineBenefitSubTypeandEligibility_Retirement();
                //}

                if (this.icdoBenefitCalculationHeader.iintPlanId.IsNotNull() && this.icdoBenefitCalculationHeader.iintPlanId > 0 && this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().icdoPersonAccount.istrRetirementSubType.IsNotNullOrEmpty())
                {
                    //PIR-854
                    string istrBenefitsubtype = string.Empty;
                    if (this.iclbBenefitCalculationDetail != null && this.iclbBenefitCalculationDetail.Count > 0)
                    {
                        istrBenefitsubtype = this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().icdoBenefitCalculationDetail.benefit_subtype_value;
                    }
                    if (istrBenefitsubtype == busConstant.RETIREMENT_TYPE_MINIMUM_DISTRIBUTION)
                    {
                        foreach (string lstrPlanCode in this.ibusBenefitApplication.iclbEligiblePlans)
                        {
                            this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.istrPlanCode == lstrPlanCode).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_MINIMUM_DISTRIBUTION;
                        }
                        //PIR 854 On the Minimum Distribution Calculations OPUS should not include the accrued benefit for hours worked in the year of MD date
                        if (this.ibusBenefitApplication.aclbPersonWorkHistory_MPI != null && this.ibusBenefitApplication.aclbPersonWorkHistory_MPI.Count > 0
                            && this.ibusBenefitApplication.aclbPersonWorkHistory_MPI.Where(item => item.year < this.icdoBenefitCalculationHeader.retirement_date.Year).Count() > 0)
                        {
                            this.ibusBenefitApplication.aclbPersonWorkHistory_MPI = this.ibusBenefitApplication.aclbPersonWorkHistory_MPI.Where(item => item.year < this.icdoBenefitCalculationHeader.retirement_date.Year).ToList().ToCollection();
                        }
                        istrBenefitsubtype = string.Empty;
                    }

                    this.icdoBenefitCalculationHeader.istrRetirementType = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().icdoPersonAccount.istrRetirementSubType;
                }

            }
        }

        public void Setup_Retirement_Calculations(bool ablnIsMindistribution = false) //Mahua Min Distribution batch
        {
            DateTime ldtVestedDateMpi = DateTime.MinValue;
            DateTime ldtVestedDateIAP = DateTime.MinValue;
            // Abhishek - The NotEligible if-condition needs to be removed. 
            // See the comments below in ValidateHardErros()
            //if (!this.ibusBenefitApplication.NotEligible && this.ibusBenefitApplication.iclbEligiblePlans.Where(plan => plan == (this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).First().icdoPersonAccount.istrPlanCode)).Count() > 0) //THIS IS CONSIDERING THAT ONE EACH TIME FOR LOCALS, LOCALS WILL NEVER BE GROUPED IN THE CONDITION HERE
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
                    ldtVestedDateMpi = DateTime.MinValue;
                    busBenefitCalculationDetail lbusBenefitCalculationDetail = new busBenefitCalculationDetail { icdoBenefitCalculationDetail = new cdoBenefitCalculationDetail() };
                    lbusBenefitCalculationDetail.iobjMainCDO = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail;

                    //Get the VESTED DATE FROM PERSON ACCOUNT ELIGIBILITY
                    if (this.ibusBenefitApplication.CheckAlreadyVested(busConstant.MPIPP))
                    {
                        ldtVestedDateMpi = this.ibusBenefitApplication.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date;
                    }

                    lbusBenefitCalculationDetail.LoadData(this.icdoBenefitCalculationHeader.benefit_calculation_header_id, this.icdoBenefitCalculationHeader.iintPlanId,
                        this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).First().icdoPersonAccount.person_account_id,
                        ldtVestedDateMpi, this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().icdoPersonAccount.istrRetirementSubType,
                        busConstant.BENEFIT_TYPE_RETIREMENT);

                    lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.ee_derived_benefit_amount =ibusCalculation.CalculateEEDerivedBenefitAsOfRetirementDate(this,null);

                    this.iclbBenefitCalculationDetail.Add(lbusBenefitCalculationDetail);

                    if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE)
                    {
                        // Create one Detail Record for IAP Plan as well
                        ldtVestedDateIAP = DateTime.MinValue;
                        busBenefitCalculationDetail lbusBenefitCalculationDetailIAP = new busBenefitCalculationDetail { icdoBenefitCalculationDetail = new cdoBenefitCalculationDetail() };
                        lbusBenefitCalculationDetailIAP.iobjMainCDO = lbusBenefitCalculationDetailIAP.icdoBenefitCalculationDetail;

                        //Get the VESTED DATE FROM PERSON ACCOUNT ELIGIBILITY
                        if (this.ibusBenefitApplication.CheckAlreadyVested(busConstant.IAP))
                        {
                            ldtVestedDateIAP = this.ibusBenefitApplication.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date;
                        }

                        lbusBenefitCalculationDetailIAP.LoadData(this.icdoBenefitCalculationHeader.benefit_calculation_header_id, busConstant.IAP_PLAN_ID,
                            this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.IAP_PLAN_ID).First().icdoPersonAccount.person_account_id,
                            ldtVestedDateIAP, this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().icdoPersonAccount.istrRetirementSubType,
                            busConstant.BENEFIT_TYPE_RETIREMENT);

                        this.iclbBenefitCalculationDetail.Add(lbusBenefitCalculationDetailIAP);
                    }

                    #endregion
                }
                else if (this.icdoBenefitCalculationHeader.iintPlanId == busConstant.IAP_PLAN_ID)
                {
                    #region Setup Detail Record for IAPs Estimate

                    // Create one Detail Record for IAP Plan 
                    ldtVestedDateIAP = DateTime.MinValue;
                    busBenefitCalculationDetail lbusBenefitCalculationDetailIAP = new busBenefitCalculationDetail { icdoBenefitCalculationDetail = new cdoBenefitCalculationDetail() };
                    lbusBenefitCalculationDetailIAP.iobjMainCDO = lbusBenefitCalculationDetailIAP.icdoBenefitCalculationDetail;
                    //Get the VESTED DATE FROM PERSON ACCOUNT ELIGIBILITY
                    if (this.ibusBenefitApplication.CheckAlreadyVested(busConstant.IAP))
                    {
                        ldtVestedDateIAP = this.ibusBenefitApplication.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date;
                    }

                    lbusBenefitCalculationDetailIAP.LoadData(this.icdoBenefitCalculationHeader.benefit_calculation_header_id, this.icdoBenefitCalculationHeader.iintPlanId,
                        this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).First().icdoPersonAccount.person_account_id,
                        ldtVestedDateIAP, this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().icdoPersonAccount.istrRetirementSubType,
                        busConstant.BENEFIT_TYPE_RETIREMENT);

                    this.iclbBenefitCalculationDetail.Add(lbusBenefitCalculationDetailIAP);

                    #endregion
                }
                else
                {
                    #region Setup Detail Record for Locals
                    //BENEFIT-CALCULATION-DETAILS COLLECTION INITIAL LOAD CAN HAPPEN HERE SINCE WE NEED VESTED DATE A LOT OF STUFF AS WELL> 
                    if (!this.iclbPersonAccountRetirementContribution.IsNullOrEmpty())
                    {
                        busBenefitCalculationDetail lbusBenefitCalculationDetail = new busBenefitCalculationDetail { icdoBenefitCalculationDetail = new cdoBenefitCalculationDetail() };
                        lbusBenefitCalculationDetail.iclbBenefitCalculationOptions = new Collection<busBenefitCalculationOptions>(); //VERY IMP Required at many places
                        lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail = new Collection<busBenefitCalculationYearlyDetail>();
                        lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.benefit_calculation_header_id = this.icdoBenefitCalculationHeader.benefit_calculation_header_id;
                        lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.person_account_id = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).First().icdoPersonAccount.person_account_id;
                        //lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.plan_id = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.First().icdoPersonAccount.plan_id;
                        lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.istrPlanCode = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).First().icdoPersonAccount.istrPlanCode;
                        lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.istrPlanDescription = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.istrPlanCode;
                        //FOR LOCAL THIS WORK FINE SINCE I HAVE TO CREATE ONLY ONE CALC DETAIL OBJECT
                        lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.plan_id = this.icdoBenefitCalculationHeader.iintPlanId;
                        //Get the VESTED DATE FROM PERSON ACCOUNT ELIGIBILITY
                        if (this.ibusBenefitApplication.CheckAlreadyVested(this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).First().icdoPersonAccount.istrPlanCode))
                            lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.vested_date = this.ibusBenefitApplication.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date;
                        lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.benefit_subtype_id = busConstant.RETIREMENT_TYPE_CODE_ID;
                        lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.benefit_subtype_value = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().icdoPersonAccount.istrRetirementSubType;
                        lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.status_id = busConstant.BenefitCalculation.CALCULATION_STATUS_TYPE_CODE_ID;
                        lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.benefit_subtype_value = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).First().icdoPersonAccount.istrRetirementSubType;
                        lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.status_value = busConstant.BenefitCalculation.CALCULATION_STATUS_TYPE_PENDING;
                        this.iclbBenefitCalculationDetail.Add(lbusBenefitCalculationDetail);
                    }
                    #endregion
                }
                #endregion

                #region SWITCH CASE - INITIATE CALCULATION BASED ON THE REQUIRED PLAN'S ESTIMATE
                if (!this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.IsNullOrEmpty())
                {
                    decimal ldecTotalBenefitAmount = busConstant.ZERO_DECIMAL, ldecFinalAccruedBenefitAmount = busConstant.ZERO_DECIMAL;
                    int lintPersonAccountId = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).First().icdoPersonAccount.person_account_id;

                    if (!this.iclbPersonAccountRetirementContribution.IsNullOrEmpty())
                    {//Mahua: Need to put this check since Code failing for local if retirement contribution is null
                        switch (this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).First().icdoPersonAccount.istrPlanCode)
                        {

                            case busConstant.Local_161:
                                ldecTotalBenefitAmount = this.ibusCalculation.CalculateTotalBenefitAmtForLocal161(this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_161).First().icdoPersonAccount.istrRetirementSubType,
                                                                                         this.icdoBenefitCalculationHeader.retirement_date, this.ibusPerson.icdoPerson.idtDateofBirth,
                                                                                         this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_frozen_benefit_amount),
                                                                                         false, this.ibusBenefitApplication, this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_pre_bis_amount),
                                                                                         this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_post_bis_amount),
                                                                                         null, this.iclbBenefitCalculationDetail,
                                                                                         Convert.ToInt32(this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Max(item => item.icdoPersonAccountRetirementContribution.computational_year)),
                                                                                         this.iclbPersonAccountRetirementContribution, busConstant.BOOL_FALSE, this.ibusPerson.icdoPerson.person_id);

                                //Commented on 2/14/2013 as per Debra. After she questioned Avie about it -- Abhishek
                                //if (ablnIsMindistribution || 
                                //    this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => 
                                //        item.icdoPersonAccount.istrPlanCode == busConstant.Local_161).First().icdoPersonAccount.istrRetirementSubType == busConstant.RETIREMENT_TYPE_LATE)
                                //{
                                //    CalculateFinalBenefitForPensionBenefitOptions(ldecTotalBenefitAmount, busConstant.CodeValueAll, busConstant.LOCAL_161_PLAN_ID);
                                //}
                                //else
                                //{
                                CalculateLocal161BenefitOptions(busConstant.CodeValueAll, ldecTotalBenefitAmount);
                                //}

                                break;
                            case busConstant.Local_52:
                                ldecTotalBenefitAmount = this.ibusCalculation.CalculateTotalBenefitAmtForLocal52(this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_52).First().icdoPersonAccount.istrRetirementSubType,
                                                             this.icdoBenefitCalculationHeader.retirement_date, this.ibusPerson.icdoPerson.idtDateofBirth,
                                                             this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_frozen_benefit_amount),
                                                             false, this.ibusBenefitApplication, this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_pre_bis_amount),
                                                             this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_post_bis_amount),
                                                             null, this.iclbBenefitCalculationDetail,
                                                             Convert.ToInt32(this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Max(item => item.icdoPersonAccountRetirementContribution.computational_year)),
                                                             this.iclbPersonAccountRetirementContribution, busConstant.BOOL_FALSE, this.ibusPerson.icdoPerson.person_id);

                                //Commented on 2/14/2013 as per Debra. After she questioned Avie about it -- Abhishek
                                //if (ablnIsMindistribution ||
                                //    this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item =>
                                //        item.icdoPersonAccount.istrPlanCode == busConstant.Local_52).First().icdoPersonAccount.istrRetirementSubType == busConstant.RETIREMENT_TYPE_LATE)
                                //{
                                //    CalculateFinalBenefitForPensionBenefitOptions(ldecTotalBenefitAmount, busConstant.CodeValueAll, busConstant.LOCAL_52_PLAN_ID);
                                //}
                                //else
                                //{
                                CalculateLocal52BenefitOptions(busConstant.CodeValueAll, ldecTotalBenefitAmount);
                                //}

                                break;

                            case busConstant.Local_600:
                                ldecTotalBenefitAmount = this.ibusCalculation.CalculateTotalBenefitAmtForLocal600(this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_600).First().icdoPersonAccount.istrRetirementSubType,
                                                                                                                  this.icdoBenefitCalculationHeader.retirement_date, this.ibusPerson.icdoPerson.idtDateofBirth,
                                                                                                                  this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_frozen_benefit_amount),
                                                                                                                   false, this.ibusBenefitApplication, this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_pre_bis_amount),
                                                                                                                   this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_post_bis_amount),
                                                                                                                   null, this.iclbBenefitCalculationDetail,
                                                                                                                   Convert.ToInt32(this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Max(item => item.icdoPersonAccountRetirementContribution.computational_year)),
                                                                                                                   this.iclbPersonAccountRetirementContribution, busConstant.BOOL_FALSE, this.ibusPerson.icdoPerson.person_id);

                                //Commented on 2/14/2013 as per Debra. After she questioned Avie about it -- Abhishek
                                //if (ablnIsMindistribution ||
                                //    this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item =>
                                //        item.icdoPersonAccount.istrPlanCode == busConstant.Local_600).First().icdoPersonAccount.istrRetirementSubType == busConstant.RETIREMENT_TYPE_LATE)
                                //{
                                //    CalculateFinalBenefitForPensionBenefitOptions(ldecTotalBenefitAmount, busConstant.CodeValueAll, busConstant.LOCAL_600_PLAN_ID);
                                //}
                                //else
                                //{
                                CalculateLocal600BenefitOptions(busConstant.CodeValueAll, ldecTotalBenefitAmount);
                                //}
                                break;

                            case busConstant.Local_666:
                                ldecTotalBenefitAmount = this.ibusCalculation.CalculateTotalBenefitAmtForLocal666(this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_666).First().icdoPersonAccount.istrRetirementSubType,
                                                                                      this.icdoBenefitCalculationHeader.retirement_date, this.ibusPerson.icdoPerson.idtDateofBirth,
                                                                                      this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_frozen_benefit_amount),
                                                                                       false, this.ibusBenefitApplication, this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_pre_bis_amount),
                                                                                       this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_post_bis_amount), this.icdoBenefitCalculationHeader.age,
                                                                                       null, this.iclbBenefitCalculationDetail,
                                                                                       Convert.ToInt32(this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Max(item => item.icdoPersonAccountRetirementContribution.computational_year)),
                                                                                       this.iclbPersonAccountRetirementContribution, busConstant.BOOL_FALSE, this.ibusPerson.icdoPerson.person_id);

                                //Commented on 2/14/2013 as per Debra. After she questioned Avie about it -- Abhishek
                                //if (ablnIsMindistribution ||
                                //    this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item =>
                                //        item.icdoPersonAccount.istrPlanCode == busConstant.Local_666).First().icdoPersonAccount.istrRetirementSubType == busConstant.RETIREMENT_TYPE_LATE)
                                //{
                                //    CalculateFinalBenefitForPensionBenefitOptions(ldecTotalBenefitAmount, busConstant.CodeValueAll, busConstant.LOCAL_666_PLAN_ID);
                                //}
                                //else
                                //{
                                CalculateLocal666BenefitOptions(busConstant.CodeValueAll, ldecTotalBenefitAmount);
                                //}
                                //CalculateLocal666Benefit(busConstant.CodeValueAll, this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_666).First().icdoPersonAccount.istrRetirementSubType);
                                break;

                            case busConstant.LOCAL_700:                                
                                ldecTotalBenefitAmount = this.ibusCalculation.CalculateTotalBenefitAmtForLocal700(this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.LOCAL_700).First().icdoPersonAccount.istrRetirementSubType,
                                                          this.icdoBenefitCalculationHeader.retirement_date, this.ibusPerson.icdoPerson.idtDateofBirth,
                                                          this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_frozen_benefit_amount),
                                                           false, this.ibusBenefitApplication, this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_pre_bis_amount),
                                                           this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_post_bis_amount),
                                                           null, this.iclbBenefitCalculationDetail,
                                                           Convert.ToInt32(this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Max(item => item.icdoPersonAccountRetirementContribution.computational_year)),
                                                           this.iclbPersonAccountRetirementContribution, busConstant.BOOL_FALSE, this.ibusPerson.icdoPerson.person_id);

                                //Commented on 2/14/2013 as per Debra. After she questioned Avie about it -- Abhishek
                                //if (ablnIsMindistribution ||
                                //    this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item =>
                                //        item.icdoPersonAccount.istrPlanCode == busConstant.LOCAL_700).First().icdoPersonAccount.istrRetirementSubType == busConstant.RETIREMENT_TYPE_LATE)
                                //{
                                //    CalculateFinalBenefitForPensionBenefitOptions(ldecTotalBenefitAmount, busConstant.CodeValueAll, busConstant.LOCAL_700_PLAN_ID);
                                //}
                                //else
                                //{
                                CalculateLocal700BenefitOptions(busConstant.CodeValueAll, ldecTotalBenefitAmount);
                                //}
                                //CalculateLocal700Benefit(busConstant.CodeValueAll, this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.LOCAL_700).First().icdoPersonAccount.istrRetirementSubType);
                                break;
                        }
                    }

                    switch (this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).First().icdoPersonAccount.istrPlanCode)
                    {
                        case busConstant.MPIPP:
                            if (this.ibusBenefitApplication.aclbPersonWorkHistory_MPI != null && this.ibusBenefitApplication.aclbPersonWorkHistory_MPI.Count > 0)
                            {

                                busPersonAccount lbusPersonAccount = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault();
                                decimal ldecLateAdjustmentAmt = busConstant.ZERO_DECIMAL;
                                decimal ldecBenefitAmtForPension = ibusCalculation.CalculateBenefitAmtForPension(ibusBenefitApplication.ibusPerson,
                                             busConstant.BENEFIT_TYPE_RETIREMENT, //Sid Jain 05092012
                                             icdoBenefitCalculationHeader.age, icdoBenefitCalculationHeader.retirement_date, ldtVestedDateMpi,
                                             lbusPersonAccount,
                                             this.ibusBenefitApplication, false, iclbBenefitCalculationDetail, this.iclbPersonAccountRetirementContribution, null, true, lbusPersonAccount.icdoPersonAccount.istrRetirementSubType, ref ldecLateAdjustmentAmt, this.ibusPerson.icdoPerson.person_id);

                                #region Check if Withdrawal History Exists: Then Acrrued benefit = Accrued Benefit - EE derived

                                //ldecBenefitAmtForPension = ibusCalculation.CalculateAccruedBenefitForPersonWithWithdrawal(
                                //                             this.ibusBenefitApplication.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date,
                                //                             ldecBenefitAmtForPension, this.ibusBenefitApplication.ibusPerson, this.ibusBenefitApplication.ibusPerson.iclbPersonAccount,
                                //                             icdoBenefitCalculationHeader.retirement_date, this.ibusBenefitApplication.aclbPersonWorkHistory_MPI,
                                //                             this.ibusBenefitApplication.ibusPerson.iclbPersonAccountRetirementContribution, this.ibusBenefitApplication.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.forfeiture_date.Year);

                                if (ldecBenefitAmtForPension != 0)
                                {
                                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).
                                                               FirstOrDefault().icdoBenefitCalculationDetail.early_reduced_benefit_amount = ldecBenefitAmtForPension;

                                    //this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).
                                    //                          FirstOrDefault().icdoBenefitCalculationDetail.unreduced_benefit_amount = ldecBenefitAmtForPension;

                                    #endregion


                                    if (this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.MPIPP).First().icdoPersonAccount.istrRetirementSubType == busConstant.RETIREMENT_TYPE_LATE)
                                    {
                                        ldecFinalAccruedBenefitAmount = CalculateFinalBenefitForPensionBenefitOptions(ldecLateAdjustmentAmt, busConstant.CodeValueAll, busConstant.MPIPP_PLAN_ID);
                                    }
                                    else
                                    {
                                        ldecFinalAccruedBenefitAmount = CalculateFinalBenefitForPensionBenefitOptions(ldecBenefitAmtForPension, busConstant.CodeValueAll, busConstant.MPIPP_PLAN_ID);
                                    }

                                    if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL)
                                    {
                                        // Set the Final Benefit Calc Value
                                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id ==
                                            this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().icdoBenefitCalculationDetail.final_monthly_benefit_amount = ldecFinalAccruedBenefitAmount;
                                    }


                                    #region UVHP calculation in ESTIMATE
                                    ldecTotalBenefitAmount = this.ibusCalculation.FetchUVHPAmountandInterest(false, null, this.iclbBenefitCalculationDetail, this, null, this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).First().icdoPersonAccount.person_account_id, this.icdoBenefitCalculationHeader.retirement_date);
                                    decimal ldecEEUVHPAmount = ldecTotalBenefitAmount + lbusPersonAccount.icdoPersonAccount.idecNonVestedEE + lbusPersonAccount.icdoPersonAccount.idecNonVestedEEInterest;
                                    this.ibusCalculation.ProcessQDROOffset(this.iclbBenefitCalculationDetail.Where(i => i.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault(), this.ibusPerson.icdoPerson.person_id, ref ldecEEUVHPAmount, true, true, false, false, busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE);

                                    CalculateUVHPBenefitOptions(busConstant.CodeValueAll, ldecEEUVHPAmount);
                                    #endregion

                                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().icdoBenefitCalculationDetail.ee_derived_benefit_amount = ibusCalculation.CalculateEEDerivedBenefitAsOfRetirementDate(this, null);
                                }

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
        }


        #region Local Benefit Options Calculations Business Logic

        public void CalculateLocal161BenefitOptions(string astrBenefitOption, decimal adecTotalBenefitAmount)
        {
            #region Variables Required in Switch Case
            decimal ldecFactor = new decimal();
            busBenefitCalculationOptions lbusBenefitCalculationOption;

            #endregion


            bool lblnCheckIfSpouse = busConstant.BOOL_FALSE;
            lblnCheckIfSpouse = this.ibusCalculation.CheckIfSpouse(this.icdoBenefitCalculationHeader.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id);

            int lintYears;
            int lintMonths;
            int lintDays;
            int lintBeneficiaryYears;
            int lintBeneficiaryMonths;
            int lintBeneficiaryDays;



            #region Swtich Case to determine the Factors and Amounts
            switch (astrBenefitOption)
            {
                case busConstant.CodeValueAll:

                    #region Local 161 LUMP SUM
                    //Ticket - 61531
                    ldecFactor = Math.Round(ibusCalculation.GetLumpsumBenefitFactorByAGE(Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge)) * 12,3);
                    lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                    //Query to SGT_PLAN_BENEFIT_XR to get PLAN_BENEFIT_ID
                    //lbusBenefitCalculationOption.icdoBenefitCalculationOptions.istrBenefitOptionDescription = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(1504, busConstant.LUMP_SUM);
                    //lbusBenefitCalculationOption.icdoBenefitCalculationOptions.plan_benefit_id = ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_161_PLAN_ID, busConstant.LUMP_SUM);
                    //lbusBenefitCalculationOption.icdoBenefitCalculationOptions.benefit_option_factor = ldecFactor;
                    //lbusBenefitCalculationOption.icdoBenefitCalculationOptions.benefit_amount = Convert.ToDecimal(Math.Round(ldecTotalBenefitAmount * ldecFactor, 2, MidpointRounding.AwayFromZero));

                    lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_161_PLAN_ID, busConstant.LUMP_SUM), ldecFactor,
                                                            Convert.ToDecimal(adecTotalBenefitAmount * ldecFactor),
                                                            busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                            this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                            busConstant.LUMP_SUM, busConstant.ZERO_DECIMAL);



                    decimal ldecTotalLumpSum = GetMPIPHPLumpSum() + lbusBenefitCalculationOption.icdoBenefitCalculationOptions.benefit_amount;
                    if (ldecTotalLumpSum <= busConstant.BenefitCalculation.LUMP_SUM_LIMIT)
                    {
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_161_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    }
                    #endregion

                    if (ldecTotalLumpSum > busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT)
                    {
                        if (lblnCheckIfSpouse)
                        {
                            #region JOINT_50_PERCENT_SURVIVOR_ANNUITY
                            busGlobalFunctions.GetDetailTimeSpan(this.ibusBenefitApplication.icdoBenefitApplication.retirement_date, this.ibusPerson.icdoPerson.idtDateofBirth, out lintYears, out lintMonths, out lintDays);
                            busGlobalFunctions.GetDetailTimeSpan(this.ibusBenefitApplication.icdoBenefitApplication.retirement_date, this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth, out lintBeneficiaryYears, out lintBeneficiaryMonths, out lintBeneficiaryDays);

                            ldecFactor = ibusCalculation.GetBenefitProvisionBenefitOptionFactor(busConstant.LOCAL_161_PLAN_ID, busConstant.BENEFIT_TYPE_RETIREMENT, ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_161_PLAN_ID, busConstant.QJ50), lintYears, lintMonths, lintBeneficiaryYears, lintBeneficiaryMonths);

                            //ldecFactor = GetBenefitFactorLocal(busConstant.Local_161, busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY, busConstant.BENEFIT_TYPE_RETIREMENT, this.icdoBenefitCalculationHeader.age, this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement);
                            lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                            this.idecQualifiedJointAndSurvivorAnnuity50 = Math.Round(adecTotalBenefitAmount * ldecFactor, 2);
                            //PIR 229 : Local 161 
                            lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_161_PLAN_ID, busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY), ldecFactor,
                                            Math.Round(adecTotalBenefitAmount * ldecFactor, 2),
                                            busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                            this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                            busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY, Math.Round(this.idecQualifiedJointAndSurvivorAnnuity50 * 0.5M, 2));

                            // No Need to show the Relative Value for JOINT_50_PERCENT_SURVIVOR_ANNUITY Benefit Option
                            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_161_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                            #endregion


                            #region JOINT_75_PERCENT_SURVIVOR_ANNUITY
                            ldecFactor = ibusCalculation.GetBenefitProvisionBenefitOptionFactor(busConstant.LOCAL_161_PLAN_ID, busConstant.BENEFIT_TYPE_RETIREMENT, ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_161_PLAN_ID, busConstant.JS75), lintYears, lintMonths, lintBeneficiaryYears, lintBeneficiaryMonths);

                            lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                            lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_161_PLAN_ID, busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY), ldecFactor,
                                        Convert.ToDecimal(adecTotalBenefitAmount * ldecFactor),
                                        busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                        this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                        busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, Math.Round(Math.Round(adecTotalBenefitAmount * ldecFactor, 2) * 75 / 100, 2));


                            //if (Math.Round(Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor)) / this.idecQualifiedJointAndSurvivorAnnuity50, 2) == 0.95m)
                            //{
                            //    lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = busConstant.BenefitCalculation.RELATIVE_VALUE_APPROX_EQUAL;
                            //}
                            //else
                            //{
                            //    lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = Math.Round(Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor)) / this.idecQualifiedJointAndSurvivorAnnuity50 * 100, 2).ToString() + " %";
                            //}

                            lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, this.icdoBenefitCalculationHeader.age, this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement);
                            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_161_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                            #endregion
                        }

                        #region FIVE_YEARS_CERTAIN_AND_LIFE_ANNUTIY
                        lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                        lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_161_PLAN_ID, busConstant.FIVE_YEARS_CERTAIN_AND_LIFE_ANNUTIY), Decimal.One,
                                                                Convert.ToDecimal(adecTotalBenefitAmount * Decimal.One),
                                                                busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                busConstant.FIVE_YEARS_CERTAIN_AND_LIFE_ANNUTIY, busConstant.ZERO_DECIMAL);

                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_161_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                        #endregion
                    }

                    break;

                case busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY:
                    if (lblnCheckIfSpouse)
                    {



                        busGlobalFunctions.GetDetailTimeSpan(this.ibusBenefitApplication.icdoBenefitApplication.retirement_date, this.ibusPerson.icdoPerson.idtDateofBirth, out lintYears, out lintMonths, out lintDays);
                        busGlobalFunctions.GetDetailTimeSpan(this.ibusBenefitApplication.icdoBenefitApplication.retirement_date, this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth, out lintBeneficiaryYears, out lintBeneficiaryMonths, out lintBeneficiaryDays);

                        ldecFactor = ibusCalculation.GetBenefitProvisionBenefitOptionFactor(busConstant.LOCAL_161_PLAN_ID, busConstant.BENEFIT_TYPE_RETIREMENT, ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_161_PLAN_ID, busConstant.QJ50), lintYears, lintMonths, lintBeneficiaryYears, lintBeneficiaryMonths);

                        //ldecFactor = GetBenefitFactorLocal(busConstant.Local_161, busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY, busConstant.BENEFIT_TYPE_RETIREMENT, this.icdoBenefitCalculationHeader.age, this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement);
                        lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                        lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_161_PLAN_ID, busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY), ldecFactor,
                                                                Math.Round(adecTotalBenefitAmount * ldecFactor, 2),
                                                                busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY, Math.Round(Math.Round(adecTotalBenefitAmount * ldecFactor, 2) * 50 / 100, 2));

                        // No need to show the Relative Value for this Benefit Option
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_161_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    }
                    break;

                case busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY:
                    if (lblnCheckIfSpouse)
                    {
                        busGlobalFunctions.GetDetailTimeSpan(this.ibusBenefitApplication.icdoBenefitApplication.retirement_date, this.ibusPerson.icdoPerson.idtDateofBirth, out lintYears, out lintMonths, out lintDays);
                        busGlobalFunctions.GetDetailTimeSpan(this.ibusBenefitApplication.icdoBenefitApplication.retirement_date, this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth, out lintBeneficiaryYears, out lintBeneficiaryMonths, out lintBeneficiaryDays);

                        ldecFactor = ibusCalculation.GetBenefitProvisionBenefitOptionFactor(busConstant.LOCAL_161_PLAN_ID, busConstant.BENEFIT_TYPE_RETIREMENT, ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_161_PLAN_ID, busConstant.JS75), lintYears, lintMonths, lintBeneficiaryYears, lintBeneficiaryMonths);

                        //ldecFactor = GetBenefitFactorLocal(busConstant.Local_161, busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, busConstant.BENEFIT_TYPE_RETIREMENT, this.icdoBenefitCalculationHeader.idecParticipantFullAge, this.icdoBenefitCalculationHeader.idecSurvivorFullAge);
                        lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                        lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_161_PLAN_ID, busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY), ldecFactor,
                                                                Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor)),
                                                                busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, Math.Round(Math.Round(adecTotalBenefitAmount * ldecFactor, 2) * 75 / 100, 2));

                        lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, this.icdoBenefitCalculationHeader.age, this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement);
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_161_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    }
                    break;

                case busConstant.FIVE_YEARS_CERTAIN_AND_LIFE_ANNUTIY:
                    lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                    lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_161_PLAN_ID, busConstant.FIVE_YEARS_CERTAIN_AND_LIFE_ANNUTIY), Decimal.One,
                                                            Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * Decimal.One)),
                                                            busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                            this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                            busConstant.FIVE_YEARS_CERTAIN_AND_LIFE_ANNUTIY, busConstant.ZERO_DECIMAL);

                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_161_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    break;

                case busConstant.LUMP_SUM:
                    //Ticket - 61531
                    ldecFactor = Math.Round(ibusCalculation.GetLumpsumBenefitFactorByAGE(Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge)) * 12,3);
                    lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                    lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_161_PLAN_ID, busConstant.LUMP_SUM), ldecFactor,
                                                            Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor)),
                                                            busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                            this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                            busConstant.LUMP_SUM, busConstant.ZERO_DECIMAL);

                    ldecTotalLumpSum = GetMPIPHPLumpSum() + lbusBenefitCalculationOption.icdoBenefitCalculationOptions.benefit_amount;
                    if (ldecTotalLumpSum < busConstant.BenefitCalculation.LUMP_SUM_LIMIT)
                    {
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_161_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    }
                    break;

            }

            decimal ldecLumpSumFactor = ldecFactor = ibusCalculation.GetLumpsumBenefitFactorByAGE(Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge)) * 12;
            ldecLumpSumFactor = Math.Round(ldecLumpSumFactor, 3);
            decimal ldecPresentValue = Math.Round(adecTotalBenefitAmount * ldecLumpSumFactor, 3);
            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_161_PLAN_ID).First().icdoBenefitCalculationDetail.present_value_amount = ldecPresentValue;
            #endregion
        }

        public void CalculateLocal52BenefitOptions(string astrBenefitOption, decimal adecTotalBenefitAmount, bool ablnConvertBenefitOption = false,string astrOriginalBenefitOption = "")//PIR 894
        {

            #region Variables Required in Switch Case
            decimal ldecFactor = new decimal();
            busBenefitCalculationOptions lbusBenefitCalculationOption;
            #endregion

            #region SwitchCase Based on BenefitOptions to be Calculated
            bool lblnCheckIfSpouse = busConstant.BOOL_FALSE;
            lblnCheckIfSpouse = this.ibusCalculation.CheckIfSpouse(this.icdoBenefitCalculationHeader.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id);
            decimal ldecSpecialYears = this.ibusBenefitApplication.aclbPersonWorkHistory_MPI.Where(item => item.year >= busConstant.BenefitCalculation.MERGER_DATE_LOCAL_52.Year && item.qualified_hours >= 400).Count() + this.ibusBenefitApplication.Local52_PensionCredits;
            //PIR 371
            decimal ldecDiffAgeFactor = 0;
            if (this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth > this.ibusPerson.icdoPerson.idtDateofBirth)
            {
                ldecDiffAgeFactor = busGlobalFunctions.CalculatePersonAge(this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth, this.ibusPerson.icdoPerson.idtDateofBirth);
            }
            else
            {
                ldecDiffAgeFactor = busGlobalFunctions.CalculatePersonAge(this.ibusPerson.icdoPerson.idtDateofBirth, this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth) * -1;
            }
            switch (astrBenefitOption)
            {
                case busConstant.CodeValueAll:

                    #region Local 52 LUMP SUM
                    //Ticket - 61531
                    ldecFactor = Math.Round(ibusCalculation.GetLumpsumBenefitFactorByAGE(Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge)) * 12,3);
                    lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                    lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_52_PLAN_ID, busConstant.LUMP_SUM), ldecFactor,
                                                            Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor)),
                                                            busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                            this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                            busConstant.LUMP_SUM, busConstant.ZERO_DECIMAL);

                    decimal ldecTotalLumpSum = GetMPIPHPLumpSum() + lbusBenefitCalculationOption.icdoBenefitCalculationOptions.benefit_amount;
                    if (ldecTotalLumpSum <= busConstant.BenefitCalculation.LUMP_SUM_LIMIT)
                    {
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_52_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    }
                    #endregion

                    if (ldecTotalLumpSum > busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT)
                    {
                        if (lblnCheckIfSpouse)
                        {
                            #region JOINT_50_PERCENT_POPUP_ANNUITY
                            //ldecFactor = Math.Min((Math.Round((this.icdoBenefitCalculationHeader.age - this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement), 0) * Convert.ToDecimal(0.005)) + Convert.ToDecimal(0.94), Convert.ToDecimal(0.99));
                            // PIR - 371
                            ldecFactor = ibusCalculation.GetRetirementL52Factor(busConstant.JOINT_50_PERCENT_POPUP_ANNUITY, this.ibusPerson.icdoPerson.idtDateofBirth, this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth);
                            lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                            this.idecQualifiedJointAndSurvivorAnnuity50 = Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor));
                            lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_52_PLAN_ID, busConstant.JOINT_50_PERCENT_POPUP_ANNUITY), ldecFactor,
                                                                    Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor)),
                                                                    busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                    this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                    busConstant.JOINT_50_PERCENT_POPUP_ANNUITY, Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor)) * 50 / 100));

                            // No need to show the Relative Value for this Benefit Option
                            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_52_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                            #endregion

                            #region JOINT_75_PERCENT_SURVIVOR_ANNUITY
                            //ldecFactor = Math.Min((Math.Round((this.icdoBenefitCalculationHeader.age - this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement), 0) * Convert.ToDecimal(0.006)) + Convert.ToDecimal(0.895), Convert.ToDecimal(0.99));
                            // PIR - 371
                            ldecFactor = ibusCalculation.GetRetirementL52Factor(busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, this.ibusPerson.icdoPerson.idtDateofBirth, this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth);
                            lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                            lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_52_PLAN_ID, busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY), ldecFactor,
                                                                    Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor)),
                                                                    busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                    this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                    busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, Convert.ToDecimal(Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor)) * 75 / 100));

                            //if (Math.Round(Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor)) / this.idecQualifiedJointAndSurvivorAnnuity50, 2) == 0.95m)
                            //{
                            //    lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = busConstant.BenefitCalculation.RELATIVE_VALUE_APPROX_EQUAL;
                            //}
                            //else
                            //{
                            //    lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = Math.Round(Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor)) / this.idecQualifiedJointAndSurvivorAnnuity50 * 100, 2).ToString() + " %";
                            //}

                            lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, this.icdoBenefitCalculationHeader.age, this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement);
                            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_52_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                            #endregion

                            #region JOINT_100_PERCENT_POPUP_ANNUITY
                            //ldecFactor = Math.Min((Math.Round((this.icdoBenefitCalculationHeader.age - this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement), 0) * Convert.ToDecimal(0.006)) + Convert.ToDecimal(0.85), Convert.ToDecimal(0.99));
                            // PIR - 371
                            ldecFactor = ibusCalculation.GetRetirementL52Factor(busConstant.JOINT_100_PERCENT_POPUP_ANNUITY, this.ibusPerson.icdoPerson.idtDateofBirth, this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth);
                            lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                            lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_52_PLAN_ID, busConstant.JOINT_100_PERCENT_POPUP_ANNUITY), ldecFactor,
                                                                    Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor)),
                                                                    busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                    this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                    busConstant.JOINT_100_PERCENT_POPUP_ANNUITY, Convert.ToDecimal(Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor)) * 100 / 100));

                            //if (Math.Round(Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor)) / this.idecQualifiedJointAndSurvivorAnnuity50, 2) == 0.95m)
                            //{
                            //    lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = busConstant.BenefitCalculation.RELATIVE_VALUE_APPROX_EQUAL;
                            //}
                            //else
                            //{
                            //    lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = Math.Round(Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor)) / this.idecQualifiedJointAndSurvivorAnnuity50 * 100, 2).ToString() + " %";
                            //}

                            lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.JOINT_100_PERCENT_POPUP_ANNUITY, this.icdoBenefitCalculationHeader.age, this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement);
                            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_52_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                            #endregion
                        }

                        #region TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY
                        if (ldecSpecialYears >= 15)
                        {
                            lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                            lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_52_PLAN_ID, busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY), Decimal.One,
                                                                    Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount)),
                                                                    busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                    this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                    busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, busConstant.ZERO_DECIMAL);

                            lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, this.icdoBenefitCalculationHeader.age, this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement);
                            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_52_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                        }
                        #endregion

                        #region LIFE_ANNUTIY
                        if (ldecSpecialYears < 15)
                        {
                            lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                            lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_52_PLAN_ID, busConstant.LIFE_ANNUTIY), Decimal.One,
                                                                     Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount)),
                                                                    busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                    this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                    busConstant.LIFE_ANNUTIY, busConstant.ZERO_DECIMAL);

                            lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.LIFE_ANNUTIY, this.icdoBenefitCalculationHeader.age, this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement);
                            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_52_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                        }
                        #endregion
                    }
                    break;

                case busConstant.LIFE_ANNUTIY:
                    if (ldecSpecialYears < 15 || ablnConvertBenefitOption)  //PIR 894
                    {
                        lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                        lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_52_PLAN_ID, busConstant.LIFE_ANNUTIY), Decimal.One,
                                                                Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount)),
                                                                busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                busConstant.LIFE_ANNUTIY, busConstant.ZERO_DECIMAL);

                        //PIR 894
                        if(ablnConvertBenefitOption)
                        {
                            if(astrOriginalBenefitOption == busConstant.JOINT_50_PERCENT_POPUP_ANNUITY)
                            {
                                lbusBenefitCalculationOption.icdoBenefitCalculationOptions.pop_up_option_factor
                                    = ibusCalculation.GetRetirementL52Factor(busConstant.JOINT_50_PERCENT_POPUP_ANNUITY, this.ibusPerson.icdoPerson.idtDateofBirth, this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth);

                                lbusBenefitCalculationOption.icdoBenefitCalculationOptions.pop_up_benefit_amount = Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * lbusBenefitCalculationOption.icdoBenefitCalculationOptions.pop_up_option_factor));
                            }
                            else if(astrOriginalBenefitOption == busConstant.JOINT_100_PERCENT_POPUP_ANNUITY)
                            {
                                lbusBenefitCalculationOption.icdoBenefitCalculationOptions.pop_up_option_factor
                                    = ibusCalculation.GetRetirementL52Factor(busConstant.JOINT_100_PERCENT_POPUP_ANNUITY, this.ibusPerson.icdoPerson.idtDateofBirth, this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth);

                                lbusBenefitCalculationOption.icdoBenefitCalculationOptions.pop_up_option_factor = Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * lbusBenefitCalculationOption.icdoBenefitCalculationOptions.pop_up_option_factor));
                            }
                        }

                        lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.LIFE_ANNUTIY, this.icdoBenefitCalculationHeader.age, this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement);
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_52_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    }
                    break;

                case busConstant.JOINT_50_PERCENT_POPUP_ANNUITY:
                    if (lblnCheckIfSpouse)
                    {
                        //ldecFactor = Math.Min((Math.Round((this.icdoBenefitCalculationHeader.age - this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement), 0) * Convert.ToDecimal(0.005)) + Convert.ToDecimal(0.94), Convert.ToDecimal(0.99));
                        // PIR - 371
                        ldecFactor = ibusCalculation.GetRetirementL52Factor(busConstant.JOINT_50_PERCENT_POPUP_ANNUITY, this.ibusPerson.icdoPerson.idtDateofBirth, this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth);
                        lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                        lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_52_PLAN_ID, busConstant.JOINT_50_PERCENT_POPUP_ANNUITY), ldecFactor,
                                                                Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor)),
                                                                busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                busConstant.JOINT_50_PERCENT_POPUP_ANNUITY, Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) * 50 / 100)));

                        lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.JOINT_50_PERCENT_POPUP_ANNUITY, this.icdoBenefitCalculationHeader.age, this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement);
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_52_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    } break;

                case busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY:
                    if (lblnCheckIfSpouse)
                    {
                        // PIR - 371
                        ldecFactor = ibusCalculation.GetRetirementL52Factor(busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, this.ibusPerson.icdoPerson.idtDateofBirth, this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth);
                        lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                        lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_52_PLAN_ID, busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY), ldecFactor,
                                                                Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor)),
                                                                busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) * 75 / 100)));

                        lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, this.icdoBenefitCalculationHeader.age, this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement);
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_52_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    } break;

                case busConstant.JOINT_100_PERCENT_POPUP_ANNUITY:
                    if (lblnCheckIfSpouse)
                    {
                        // PIR - 371
                        ldecFactor = ibusCalculation.GetRetirementL52Factor(busConstant.JOINT_100_PERCENT_POPUP_ANNUITY, this.ibusPerson.icdoPerson.idtDateofBirth, this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth);
                        lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                        lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_52_PLAN_ID, busConstant.JOINT_100_PERCENT_POPUP_ANNUITY), ldecFactor,
                                                                Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor)),
                                                                busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                busConstant.JOINT_100_PERCENT_POPUP_ANNUITY, Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) * 100 / 100)));

                        lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.JOINT_100_PERCENT_POPUP_ANNUITY, this.icdoBenefitCalculationHeader.age, this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement);
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_52_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    } break;

                case busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY:
                    if (ldecSpecialYears >= 15)
                    {
                        lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                        lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_52_PLAN_ID, busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY), Decimal.One,
                                                                Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * Decimal.One)),
                                                                busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, busConstant.ZERO_DECIMAL);

                        lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, this.icdoBenefitCalculationHeader.age, this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement);
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_52_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    }
                    break;

                case busConstant.LUMP_SUM:
                    //Ticket - 61531
                    ldecFactor = Math.Round(ibusCalculation.GetLumpsumBenefitFactorByAGE(Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge)) * 12,3);
                    lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                    lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_52_PLAN_ID, busConstant.LUMP_SUM), ldecFactor,
                                                            Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor)),
                                                            busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                            this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                            busConstant.LUMP_SUM, busConstant.ZERO_DECIMAL);

                    ldecTotalLumpSum = GetMPIPHPLumpSum() + lbusBenefitCalculationOption.icdoBenefitCalculationOptions.benefit_amount;
                    if (ldecTotalLumpSum < busConstant.BenefitCalculation.LUMP_SUM_LIMIT)
                    {
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_52_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    }
                    break;
            }
            #endregion
            
            decimal ldecLumpSumFactor = ldecFactor = ibusCalculation.GetLumpsumBenefitFactorByAGE(Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge)) * 12;
            ldecLumpSumFactor = Math.Round(ldecLumpSumFactor, 3);
            decimal ldecPresentValue = Math.Round(adecTotalBenefitAmount * ldecLumpSumFactor, 3);
            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_52_PLAN_ID).First().icdoBenefitCalculationDetail.present_value_amount = ldecPresentValue;
        }

        public void CalculateLocal600BenefitOptions(string astrBenefitOption, decimal adecTotalBenefitAmount, bool ablnConvertBenefitOption = false, string astrOriginalBenefitOption = "")//PIR 894
        {
            #region Variables Required in Switch Case
            decimal ldecFactor = new decimal();
            busBenefitCalculationOptions lbusBenefitCalculationOption;
            #endregion

            #region SwitchCase Based on BenefitOptions to be Calculated
            bool lblnCheckIfSpouse = busConstant.BOOL_FALSE;
            lblnCheckIfSpouse = this.ibusCalculation.CheckIfSpouse(this.icdoBenefitCalculationHeader.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id);

            switch (astrBenefitOption)
            {
                case busConstant.CodeValueAll:

                    #region Local 600 LUMP SUM
                    //Ticket - 61531
                    ldecFactor = Math.Round(ibusCalculation.GetLumpsumBenefitFactorByAGE(Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge)) * 12,3);
                    lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                    lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_600_PLAN_ID, busConstant.LUMP_SUM), ldecFactor,
                                                        Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor)),
                                                            busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                            this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                            busConstant.LUMP_SUM, busConstant.ZERO_DECIMAL);

                    decimal ldecTotalLumpSum = GetMPIPHPLumpSum() + lbusBenefitCalculationOption.icdoBenefitCalculationOptions.benefit_amount;
                    if (ldecTotalLumpSum <= busConstant.BenefitCalculation.LUMP_SUM_LIMIT)
                    {
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_600_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    }
                    #endregion

                    if (ldecTotalLumpSum > busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT)
                    {
                        if (lblnCheckIfSpouse)
                        {
                            #region JOINT_50_PERCENT_POPUP_ANNUITY
                            //ldecFactor = Math.Min((Math.Round((this.icdoBenefitCalculationHeader.age - this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement), 0) * Convert.ToDecimal(0.005)) + Convert.ToDecimal(0.94), Convert.ToDecimal(0.99));
                            // PIR - 760
                            ldecFactor = ibusCalculation.GetRetirementL600Factor(busConstant.JOINT_50_PERCENT_POPUP_ANNUITY, this.ibusPerson.icdoPerson.idtDateofBirth, this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth);
                            lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                            this.idecQualifiedJointAndSurvivorAnnuity50 = Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor));
                            lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_600_PLAN_ID, busConstant.JOINT_50_PERCENT_POPUP_ANNUITY), ldecFactor,
                                                                    Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor)),
                                                                    busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                    this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                    busConstant.JOINT_50_PERCENT_POPUP_ANNUITY, Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) * 50 / 100)));//PIR-1064


                            // No  need to show the Relative Value for this Benefit Option.
                            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_600_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                            #endregion

                            #region JOINT_75_PERCENT_SURVIVOR_ANNUITY
                            //ldecFactor = Math.Min((Math.Round((this.icdoBenefitCalculationHeader.age - this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement), 0) * Convert.ToDecimal(0.005)) + Convert.ToDecimal(0.85), Convert.ToDecimal(1.0));
                            // PIR - 760
                            ldecFactor = ibusCalculation.GetRetirementL600Factor(busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, this.ibusPerson.icdoPerson.idtDateofBirth, this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth);
                            lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                            lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_600_PLAN_ID, busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY), ldecFactor,
                                                                    Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor)),
                                                                    busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                    this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                    busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) * 75 / 100)));//PIR-1064

                            //if (Math.Round(Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor)) / this.idecQualifiedJointAndSurvivorAnnuity50, 2) == 0.95m)
                            //{
                            //    lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = busConstant.BenefitCalculation.RELATIVE_VALUE_APPROX_EQUAL;
                            //}
                            //else
                            //{
                            //    lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = Math.Round(Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor)) / this.idecQualifiedJointAndSurvivorAnnuity50 * 100, 2).ToString() + " %";
                            //}

                            lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, this.icdoBenefitCalculationHeader.age, this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement);
                            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_600_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                            #endregion
                        }

                        #region TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY
                        lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                        lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_600_PLAN_ID, busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY), Decimal.One,
                                                                Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount)),
                                                                busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, busConstant.ZERO_DECIMAL);

                        lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, this.icdoBenefitCalculationHeader.age, this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement);
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_600_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                        #endregion
                    }
                    break;
                case busConstant.JOINT_50_PERCENT_POPUP_ANNUITY:
                    if (lblnCheckIfSpouse)
                    {
                        //ldecFactor = Math.Min((Math.Round((this.icdoBenefitCalculationHeader.age - this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement), 0) * Convert.ToDecimal(0.005)) + Convert.ToDecimal(0.94), Convert.ToDecimal(0.99));
                        // PIR - 760
                        ldecFactor = ibusCalculation.GetRetirementL600Factor(busConstant.JOINT_50_PERCENT_POPUP_ANNUITY, this.ibusPerson.icdoPerson.idtDateofBirth, this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth);
                        lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                        lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_600_PLAN_ID, busConstant.JOINT_50_PERCENT_POPUP_ANNUITY), ldecFactor,
                                                                Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor)),
                                                                busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                busConstant.JOINT_50_PERCENT_POPUP_ANNUITY, Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) * 50 / 100)));//PIR-1064

                        lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.JOINT_50_PERCENT_POPUP_ANNUITY, this.icdoBenefitCalculationHeader.age, this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement);
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_600_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    } break;

                case busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY:
                    if (lblnCheckIfSpouse)
                    {
                        //ldecFactor = Math.Min((Math.Round((this.icdoBenefitCalculationHeader.age - this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement), 0) * Convert.ToDecimal(0.006)) + Convert.ToDecimal(0.895), Convert.ToDecimal(0.99));
                        // PIR - 760
                        ldecFactor = ibusCalculation.GetRetirementL600Factor(busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, this.ibusPerson.icdoPerson.idtDateofBirth, this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth);
                        lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                        lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_600_PLAN_ID, busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY), ldecFactor,
                                                                Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor)),
                                                                busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) * 75 / 100)));//PIR-1064

                        lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, this.icdoBenefitCalculationHeader.age, this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement);
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_600_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    } break;

                case busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY:
                    lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                    lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_600_PLAN_ID, busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY), Decimal.One,
                                                            Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * Decimal.One)),
                                                            busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                            this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                            busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, busConstant.ZERO_DECIMAL);

                    lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, this.icdoBenefitCalculationHeader.age, this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement);
                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_600_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    break;

                case busConstant.LUMP_SUM:
                    //Ticket - 61531
                    ldecFactor = Math.Round(ibusCalculation.GetLumpsumBenefitFactorByAGE(Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge)) * 12,3);
                    lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                    lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_600_PLAN_ID, busConstant.LUMP_SUM), ldecFactor,
                                    Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor)),
                                    busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                    this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                    busConstant.LUMP_SUM, busConstant.ZERO_DECIMAL);

                    ldecTotalLumpSum = GetMPIPHPLumpSum() + lbusBenefitCalculationOption.icdoBenefitCalculationOptions.benefit_amount;
                    if (ldecTotalLumpSum < busConstant.BenefitCalculation.LUMP_SUM_LIMIT)
                    {
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_600_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    }
                    break;

                //PIR 894
                case busConstant.LIFE_ANNUTIY:
                    if (ablnConvertBenefitOption)  //PIR 894
                    {
                        lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                        lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_600_PLAN_ID, busConstant.LIFE_ANNUTIY), Decimal.One,
                                                                Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount)),
                                                                busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                busConstant.LIFE_ANNUTIY, busConstant.ZERO_DECIMAL);

                        //PIR 894
                        if (ablnConvertBenefitOption)
                        {
                            if (astrOriginalBenefitOption == busConstant.JOINT_50_PERCENT_POPUP_ANNUITY)
                            {
                                lbusBenefitCalculationOption.icdoBenefitCalculationOptions.pop_up_option_factor
                                    = ibusCalculation.GetRetirementL600Factor(busConstant.JOINT_50_PERCENT_POPUP_ANNUITY, this.ibusPerson.icdoPerson.idtDateofBirth, this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth);

                                lbusBenefitCalculationOption.icdoBenefitCalculationOptions.pop_up_benefit_amount = Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * lbusBenefitCalculationOption.icdoBenefitCalculationOptions.pop_up_option_factor));
                            }
                        }

                        lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.LIFE_ANNUTIY, this.icdoBenefitCalculationHeader.age, this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement);
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_600_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    }
                    break;
            }
            #endregion

            decimal ldecLumpSumFactor = ldecFactor = ibusCalculation.GetLumpsumBenefitFactorByAGE(Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge)) * 12;
            ldecLumpSumFactor = Math.Round(ldecLumpSumFactor, 3);
            decimal ldecPresentValue = Math.Round(adecTotalBenefitAmount * ldecLumpSumFactor, 3);
            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_600_PLAN_ID).First().icdoBenefitCalculationDetail.present_value_amount = ldecPresentValue;
        }

        public void CalculateLocal666BenefitOptions(string astrBenefitOption, decimal adecTotalBenefitAmount, bool ablnConvertBenefitOption = false, string astrOriginalBenefitOption = "")//PIR 894
        {
            #region Variables Required in Switch Case
            decimal ldecFactor = new decimal();
            busBenefitCalculationOptions lbusBenefitCalculationOption;
            #endregion

            #region Switch Case Based on Benefit Options We have to Calculate
            bool lblnCheckIfSpouse = busConstant.BOOL_FALSE;
            lblnCheckIfSpouse = this.ibusCalculation.CheckIfSpouse(this.icdoBenefitCalculationHeader.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id);

            switch (astrBenefitOption)
            {
                case busConstant.CodeValueAll:

                    #region Local 666 LUMP SUM
                    //Ticket - 61531
                    ldecFactor = Math.Round(ibusCalculation.GetLumpsumBenefitFactorByAGE(Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge)) * 12,3);
                    lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                    lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_666_PLAN_ID, busConstant.LUMP_SUM), ldecFactor,
                                                            Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) / 0.5M) * 0.5M),
                                                            busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                            this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                            busConstant.LUMP_SUM, busConstant.ZERO_DECIMAL);

                    decimal ldecTotalLumpSum = GetMPIPHPLumpSum() + lbusBenefitCalculationOption.icdoBenefitCalculationOptions.benefit_amount;
                    if (ldecTotalLumpSum <= busConstant.BenefitCalculation.LUMP_SUM_LIMIT)
                    {
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_666_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    }
                    #endregion

                    if (ldecTotalLumpSum > busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT)
                    {
                        if (lblnCheckIfSpouse)
                        {
                            #region JOINT_50_PERCENT_POPUP_ANNUITY
                            //ldecFactor = Math.Min((Math.Round((this.icdoBenefitCalculationHeader.age - this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement), 0) * Convert.ToDecimal(0.004)) + Convert.ToDecimal(0.89), Convert.ToDecimal(0.99));
                            // PIR - 760
                            ldecFactor = ibusCalculation.GetRetirementL666Factor(busConstant.JOINT_50_PERCENT_POPUP_ANNUITY, this.ibusPerson.icdoPerson.idtDateofBirth, this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth);
                            lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                            this.idecQualifiedJointAndSurvivorAnnuity50 = Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) / 0.5M) * 0.5M);
                            lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_666_PLAN_ID, busConstant.JOINT_50_PERCENT_POPUP_ANNUITY), ldecFactor,
                                            Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) / 0.5M) * 0.5M),
                                            busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                            this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                            busConstant.JOINT_50_PERCENT_POPUP_ANNUITY, Convert.ToDecimal(Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) / 0.5M) * 0.5M) * 0.5M));

                            // No need to set Relative Value for this Benefit option
                            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_666_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                            #endregion

                            #region JOINT_75_PERCENT_SURVIVOR_ANNUITY
                            //ldecFactor = Math.Min((Math.Round((this.icdoBenefitCalculationHeader.age - this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement), 0) * Convert.ToDecimal(0.004)) + Convert.ToDecimal(0.81), Convert.ToDecimal(1));
                            // PIR - 760
                            ldecFactor = ibusCalculation.GetRetirementL666Factor(busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, this.ibusPerson.icdoPerson.idtDateofBirth, this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth);
                            lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                            lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_666_PLAN_ID, busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY), ldecFactor,
                                                                    Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) / 0.5M) * 0.5M),
                                                                    busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                    this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                    busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, Convert.ToDecimal(Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) / 0.5M) * 0.5M) * 0.75M));

                            //if (Math.Round(Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) / 0.5M) * 0.5M) / this.idecQualifiedJointAndSurvivorAnnuity50, 2) == 0.95m)
                            //{
                            //    lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = busConstant.BenefitCalculation.RELATIVE_VALUE_APPROX_EQUAL;
                            //}
                            //else
                            //{
                            //    lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = Math.Round(Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor)) / this.idecQualifiedJointAndSurvivorAnnuity50 * 100, 2).ToString() + " %";
                            //}

                            lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, this.icdoBenefitCalculationHeader.age, this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement);
                            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_666_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                            #endregion
                        }

                        #region THREE_YEARS_CERTAIN_AND_LIFE_ANNUITY
                        lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                        lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_666_PLAN_ID, busConstant.THREE_YEARS_CERTAIN_AND_LIFE_ANNUITY), Decimal.One,
                                                                Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount / 0.5M) * 0.5M),
                                                                busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                busConstant.THREE_YEARS_CERTAIN_AND_LIFE_ANNUITY, busConstant.ZERO_DECIMAL);


                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_666_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                        #endregion
                    }
                    break;

                case busConstant.JOINT_50_PERCENT_POPUP_ANNUITY:
                    if (lblnCheckIfSpouse)
                    {
                        //ldecFactor = Math.Min((Math.Round((this.icdoBenefitCalculationHeader.age - this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement), 0) * Convert.ToDecimal(0.004)) + Convert.ToDecimal(0.89), Convert.ToDecimal(0.99));
                        // PIR - 760
                        ldecFactor = ibusCalculation.GetRetirementL666Factor(busConstant.JOINT_50_PERCENT_POPUP_ANNUITY, this.ibusPerson.icdoPerson.idtDateofBirth, this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth);
                        lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                        lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_666_PLAN_ID, busConstant.JOINT_50_PERCENT_POPUP_ANNUITY), ldecFactor,
                                                                Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) / 0.5M) * 0.5M),
                                                                busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                busConstant.JOINT_50_PERCENT_POPUP_ANNUITY, Convert.ToDecimal(Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) / 0.5M) * 0.5M) * 50 / 100));

                        lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.JOINT_50_PERCENT_POPUP_ANNUITY, this.icdoBenefitCalculationHeader.age, this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement);
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_666_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    }
                    break;

                case busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY:
                    if (lblnCheckIfSpouse)
                    {
                        //ldecFactor = Math.Min((Math.Round((this.icdoBenefitCalculationHeader.age - this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement), 0) * Convert.ToDecimal(0.004)) + Convert.ToDecimal(0.81), Convert.ToDecimal(1));
                        // PIR - 760
                        ldecFactor = ibusCalculation.GetRetirementL666Factor(busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, this.ibusPerson.icdoPerson.idtDateofBirth, this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth);
                        lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                        lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_666_PLAN_ID, busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY), ldecFactor,
                                                                Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) / 0.5M) * 0.5M),
                                                                busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, Convert.ToDecimal(Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) / 0.5M) * 0.5M) * 75 / 100));

                        lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, this.icdoBenefitCalculationHeader.age, this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement);
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_666_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    }
                    break;

                case busConstant.THREE_YEARS_CERTAIN_AND_LIFE_ANNUITY:
                    lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                    lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_666_PLAN_ID, busConstant.THREE_YEARS_CERTAIN_AND_LIFE_ANNUITY), Decimal.One,
                                                            Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount / 0.5M) * 0.5M),
                                                            busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                            this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                            busConstant.THREE_YEARS_CERTAIN_AND_LIFE_ANNUITY, busConstant.ZERO_DECIMAL);

                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_666_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    break;

                case busConstant.LUMP_SUM:
                    //Ticket - 61531
                    ldecFactor = Math.Round(ibusCalculation.GetLumpsumBenefitFactorByAGE(Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge)) * 12,3);
                    lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                    lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_666_PLAN_ID, busConstant.LUMP_SUM), ldecFactor,
                                    Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor)),
                                    busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                    this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                    busConstant.LUMP_SUM, busConstant.ZERO_DECIMAL);

                    ldecTotalLumpSum = GetMPIPHPLumpSum() + lbusBenefitCalculationOption.icdoBenefitCalculationOptions.benefit_amount;
                    if (ldecTotalLumpSum < busConstant.BenefitCalculation.LUMP_SUM_LIMIT)
                    {
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_666_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    }
                    break;
                
                //PIR 894
                case busConstant.LIFE_ANNUTIY:
                    if (ablnConvertBenefitOption)  //PIR 894
                    {
                        lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                        lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_666_PLAN_ID, busConstant.LIFE_ANNUTIY), Decimal.One,
                                                                Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount)),
                                                                busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                busConstant.LIFE_ANNUTIY, busConstant.ZERO_DECIMAL);

                        //PIR 894
                        if (ablnConvertBenefitOption)
                        {
                            if (astrOriginalBenefitOption == busConstant.JOINT_50_PERCENT_POPUP_ANNUITY)
                            {
                                lbusBenefitCalculationOption.icdoBenefitCalculationOptions.pop_up_option_factor
                                    = ibusCalculation.GetRetirementL666Factor(busConstant.JOINT_50_PERCENT_POPUP_ANNUITY, this.ibusPerson.icdoPerson.idtDateofBirth, this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth);

                                lbusBenefitCalculationOption.icdoBenefitCalculationOptions.pop_up_benefit_amount = Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * lbusBenefitCalculationOption.icdoBenefitCalculationOptions.pop_up_option_factor));
                            }                            
                        }

                        lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.LIFE_ANNUTIY, this.icdoBenefitCalculationHeader.age, this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement);
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_666_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    }
                    break;
            }
            #endregion
            decimal ldecLumpSumFactor = ldecFactor = ibusCalculation.GetLumpsumBenefitFactorByAGE(Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge)) * 12;
            ldecLumpSumFactor = Math.Round(ldecLumpSumFactor, 3);
            decimal ldecPresentValue = Math.Round(adecTotalBenefitAmount * ldecLumpSumFactor, 3);
            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_666_PLAN_ID).First().icdoBenefitCalculationDetail.present_value_amount = ldecPresentValue;

        }

        public void CalculateLocal700BenefitOptions(string astrBenefitOption, decimal adecTotalBenefitAmount)
        {
            #region Variables Required in Switch Case
            decimal ldecFactor = new decimal();
            busBenefitCalculationOptions lbusBenefitCalculationOption;
            #endregion

            #region Switch Case Based on Benefit Options We have to Calculate

            bool lblnCheckIfSpouse = busConstant.BOOL_FALSE;
            lblnCheckIfSpouse = this.ibusCalculation.CheckIfSpouse(this.icdoBenefitCalculationHeader.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id);

            switch (astrBenefitOption)
            {
                case busConstant.CodeValueAll:

                    #region Local 700 LUMP SUM
                    //Ticket - 61531
                    ldecFactor = Math.Round(ibusCalculation.GetLumpsumBenefitFactorByAGE(Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge)) * 12,3);
                    lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                    lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_700_PLAN_ID, busConstant.LUMP_SUM), ldecFactor,
                                                            Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) / 0.5M) * 0.5M),
                                                            busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                            this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                            busConstant.LUMP_SUM, busConstant.ZERO_DECIMAL);

                    decimal ldecTotalLumpSum = GetMPIPHPLumpSum() + lbusBenefitCalculationOption.icdoBenefitCalculationOptions.benefit_amount;
                    if (ldecTotalLumpSum <= busConstant.BenefitCalculation.LUMP_SUM_LIMIT)
                    {
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_700_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    }
                    #endregion

                    if (ldecTotalLumpSum > busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT)
                    {
                        if (lblnCheckIfSpouse)
                        {
                            #region JOINT_50_PERCENT_SURVIVOR_ANNUITY
                                                      
                            ldecFactor = GetBenefitFactorLocal(busConstant.LOCAL_700, busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY, busConstant.BENEFIT_TYPE_RETIREMENT, this.icdoBenefitCalculationHeader.idecParticipantFullAge, this.icdoBenefitCalculationHeader.idecSurvivorFullAge);                                                       
                            lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                            this.idecQualifiedJointAndSurvivorAnnuity50 = Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) / 0.5M) * 0.5M);
                            lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_700_PLAN_ID, busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY), ldecFactor,
                                                                    Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) / 0.5M) * 0.5M),
                                                                    busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                    this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY,
                                                                    Convert.ToDecimal(Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) / 0.5M) * 0.5M) * 50 / 100));

                            // No Need to show the Relative Value for JOINT_50_PERCENT_SURVIVOR_ANNUITY Benefit Option
                            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_700_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                            #endregion

                            #region JOINT_100_PERCENT_SURVIVOR_ANNUITY
                            ldecFactor = GetBenefitFactorLocal(busConstant.LOCAL_700, busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY, busConstant.BENEFIT_TYPE_RETIREMENT, this.icdoBenefitCalculationHeader.idecParticipantFullAge, this.icdoBenefitCalculationHeader.idecSurvivorFullAge);
                            lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                            lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_700_PLAN_ID, busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY), ldecFactor,
                                                                    Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) / 0.5M) * 0.5M),
                                                                    busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                    this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                    busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY,
                                                                    Convert.ToDecimal(Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) / 0.5M) * 0.5M) * 100 / 100));

                            //if (Math.Round(Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) / 0.5M) * 0.5M) / this.idecQualifiedJointAndSurvivorAnnuity50, 2) == 0.95m)
                            //{
                            //    lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = busConstant.BenefitCalculation.RELATIVE_VALUE_APPROX_EQUAL;
                            //}
                            //else
                            //{
                            //    lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = Math.Round(Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor)) / this.idecQualifiedJointAndSurvivorAnnuity50 * 100, 2).ToString() + " %";
                            //}

                            lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY, this.icdoBenefitCalculationHeader.age, this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement);
                            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_700_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                            #endregion

                            #region JOINT_66_2by3_PERCENT_SURVIVOR_ANNUITY
                            ldecFactor = GetBenefitFactorLocal(busConstant.LOCAL_700, busConstant.JOINT_66_2by3_PERCENT_SURVIVOR_ANNUITY, busConstant.BENEFIT_TYPE_RETIREMENT, this.icdoBenefitCalculationHeader.idecParticipantFullAge, this.icdoBenefitCalculationHeader.idecSurvivorFullAge);
                            lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                            lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_700_PLAN_ID, busConstant.JOINT_66_2by3_PERCENT_SURVIVOR_ANNUITY), ldecFactor,
                                                                    Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) / 0.5M) * 0.5M),
                                                                    busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                    this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                    busConstant.JOINT_66_2by3_PERCENT_SURVIVOR_ANNUITY, Convert.ToDecimal(Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) / 0.5M) * 0.5M) * 44 / 100));

                            lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.JOINT_66_2by3_PERCENT_SURVIVOR_ANNUITY, this.icdoBenefitCalculationHeader.age, this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement);
                            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_700_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                            #endregion

                            #region JOINT_75_PERCENT_SURVIVOR_ANNUITY
                            ldecFactor = GetBenefitFactorLocal(busConstant.LOCAL_700, busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, busConstant.BENEFIT_TYPE_RETIREMENT, this.icdoBenefitCalculationHeader.idecParticipantFullAge, this.icdoBenefitCalculationHeader.idecSurvivorFullAge);
                            lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                            lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_700_PLAN_ID, busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY), ldecFactor,
                                                                    Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) / 0.5M) * 0.5M),
                                                                    busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                    this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                    busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, Convert.ToDecimal(Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) / 0.5M) * 0.5M) * 75 / 100));

                            //if (Math.Round(Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) / 0.5M) * 0.5M) / this.idecQualifiedJointAndSurvivorAnnuity50, 2) == 0.95m)
                            //{
                            //    lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = busConstant.BenefitCalculation.RELATIVE_VALUE_APPROX_EQUAL;
                            //}
                            //else
                            //{
                            //    lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = Math.Round(Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor)) / this.idecQualifiedJointAndSurvivorAnnuity50 * 100, 2).ToString() + " %";
                            //}
                            lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, this.icdoBenefitCalculationHeader.age, this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement);
                            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_700_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                            #endregion
                        }

                        #region TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY
                        ldecFactor = GetBenefitFactorLocal(busConstant.LOCAL_700, busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, busConstant.BENEFIT_TYPE_RETIREMENT, this.icdoBenefitCalculationHeader.idecParticipantFullAge);
                        lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                        lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_700_PLAN_ID, busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY), ldecFactor,
                                                                Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) / 0.5M) * 0.5M),
                                                                busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, busConstant.ZERO_DECIMAL);

                        lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, this.icdoBenefitCalculationHeader.age, this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement);
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_700_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                        #endregion

                        #region TWO_YEARS_CERTAIN_AND_LIFE_ANNUITY
                        ldecFactor = GetBenefitFactorLocal(busConstant.LOCAL_700, busConstant.TWO_YEARS_CERTAIN_AND_LIFE_ANNUITY, busConstant.BENEFIT_TYPE_RETIREMENT, this.icdoBenefitCalculationHeader.idecParticipantFullAge);
                        lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                        lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_700_PLAN_ID, busConstant.TWO_YEARS_CERTAIN_AND_LIFE_ANNUITY), ldecFactor,
                                                                Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount / ldecFactor) / 0.5M) * 0.5M),
                                                                busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                busConstant.TWO_YEARS_CERTAIN_AND_LIFE_ANNUITY, busConstant.ZERO_DECIMAL);


                        //lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.TWO_YEARS_CERTAIN_AND_LIFE_ANNUITY, this.icdoBenefitCalculationHeader.age, this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement);
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_700_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                        #endregion

                        #region LIFE_ANNUTIY
                        lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                        lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_700_PLAN_ID, busConstant.LIFE_ANNUTIY), Decimal.One,
                                                                Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * Decimal.One) / 0.5M) * 0.5M),
                                                                busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                busConstant.LIFE_ANNUTIY, busConstant.ZERO_DECIMAL);

                        lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.LIFE_ANNUTIY, this.icdoBenefitCalculationHeader.age, this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement);
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_700_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                        #endregion
                    }
                    break;
                case busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY:
                    if (lblnCheckIfSpouse)
                    {
                        ldecFactor = GetBenefitFactorLocal(busConstant.LOCAL_700, busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY, busConstant.BENEFIT_TYPE_RETIREMENT, this.icdoBenefitCalculationHeader.idecParticipantFullAge, this.icdoBenefitCalculationHeader.idecSurvivorFullAge);
                        lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                        lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_700_PLAN_ID, busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY), ldecFactor,
                                                                Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) / 0.5M) * 0.5M),
                                                                busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY, Convert.ToDecimal(Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) / 0.5M) * 0.5M) * 50 / 100));

                        // No Need to show the Relative Value for JOINT_50_PERCENT_SURVIVOR_ANNUITY Benefit Option
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_700_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    }
                    break;

                case busConstant.JOINT_66_2by3_PERCENT_SURVIVOR_ANNUITY:
                    if (lblnCheckIfSpouse)
                    {
                        ldecFactor = GetBenefitFactorLocal(busConstant.LOCAL_700, busConstant.JOINT_66_2by3_PERCENT_SURVIVOR_ANNUITY, busConstant.BENEFIT_TYPE_RETIREMENT, this.icdoBenefitCalculationHeader.idecParticipantFullAge, this.icdoBenefitCalculationHeader.idecSurvivorFullAge);
                        lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                        lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_700_PLAN_ID, busConstant.JOINT_66_2by3_PERCENT_SURVIVOR_ANNUITY), ldecFactor,
                                                                Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) / 0.5M) * 0.5M),
                                                                busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                busConstant.JOINT_66_2by3_PERCENT_SURVIVOR_ANNUITY, Convert.ToDecimal(Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) / 0.5M) * 0.5M) * 44 / 100));

                        lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.JOINT_66_2by3_PERCENT_SURVIVOR_ANNUITY, this.icdoBenefitCalculationHeader.idecParticipantFullAge, this.icdoBenefitCalculationHeader.idecSurvivorFullAge);
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_700_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    }
                    break;

                case busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY:
                    if (lblnCheckIfSpouse)
                    {
                        ldecFactor = GetBenefitFactorLocal(busConstant.LOCAL_700, busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, busConstant.BENEFIT_TYPE_RETIREMENT, this.icdoBenefitCalculationHeader.idecParticipantFullAge, this.icdoBenefitCalculationHeader.idecSurvivorFullAge);
                        lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                        lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_700_PLAN_ID, busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY), ldecFactor,
                                                                Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) / 0.5M) * 0.5M),
                                                                busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, Convert.ToDecimal(Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) / 0.5M) * 0.5M) * 75 / 100));

                        lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, this.icdoBenefitCalculationHeader.idecParticipantFullAge, this.icdoBenefitCalculationHeader.idecSurvivorFullAge);
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_700_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    }
                    break;

                case busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY:
                    if (lblnCheckIfSpouse)
                    {
                        ldecFactor = GetBenefitFactorLocal(busConstant.LOCAL_700, busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY, busConstant.BENEFIT_TYPE_RETIREMENT, this.icdoBenefitCalculationHeader.age, this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement);
                        lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                        lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_700_PLAN_ID, busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY), ldecFactor,
                                        Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) / 0.5M) * 0.5M),
                                        busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                        this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                        busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY, Convert.ToDecimal(Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) / 0.5M) * 0.5M) * 100 / 100));

                        lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY, this.icdoBenefitCalculationHeader.idecParticipantFullAge, this.icdoBenefitCalculationHeader.idecSurvivorFullAge);
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_700_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    }
                    break;

                case busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY:
                    ldecFactor = GetBenefitFactorLocal(busConstant.LOCAL_700, busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, busConstant.BENEFIT_TYPE_RETIREMENT, this.icdoBenefitCalculationHeader.idecParticipantFullAge);
                    lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                    lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_700_PLAN_ID, busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY), ldecFactor,
                                                            Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) / 0.5M) * 0.5M),
                                                            busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                            this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                            busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, busConstant.ZERO_DECIMAL);

                    lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, this.icdoBenefitCalculationHeader.idecParticipantFullAge, this.icdoBenefitCalculationHeader.idecSurvivorFullAge);
                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_700_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    break;

                case busConstant.TWO_YEARS_CERTAIN_AND_LIFE_ANNUITY:
                    ldecFactor = GetBenefitFactorLocal(busConstant.LOCAL_700, busConstant.TWO_YEARS_CERTAIN_AND_LIFE_ANNUITY, busConstant.BENEFIT_TYPE_RETIREMENT, this.icdoBenefitCalculationHeader.idecParticipantFullAge);
                    lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                    lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_700_PLAN_ID, busConstant.TWO_YEARS_CERTAIN_AND_LIFE_ANNUITY), ldecFactor,
                                    Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount / ldecFactor) / 0.5M) * 0.5M),  //PIR 833 should be divide instead of multiply by factor
                                    busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                    this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                    busConstant.TWO_YEARS_CERTAIN_AND_LIFE_ANNUITY, busConstant.ZERO_DECIMAL);

                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_700_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    break;

                case busConstant.LIFE_ANNUTIY:
                    lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                    lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_700_PLAN_ID, busConstant.LIFE_ANNUTIY), Decimal.One,
                                    Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * Decimal.One) / 0.5M) * 0.5M),
                                    busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                    this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                    busConstant.LIFE_ANNUTIY, busConstant.ZERO_DECIMAL);

                    lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.LIFE_ANNUTIY, this.icdoBenefitCalculationHeader.idecParticipantFullAge, this.icdoBenefitCalculationHeader.idecSurvivorFullAge);
                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_700_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    break;

                case busConstant.LUMP_SUM:
                    //Ticket - 61531
                    ldecFactor = Math.Round(ibusCalculation.GetLumpsumBenefitFactorByAGE(Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge)) * 12,3);
                    lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                    lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_700_PLAN_ID, busConstant.LUMP_SUM), ldecFactor,
                                    Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor)),
                                    busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                    this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                    busConstant.LUMP_SUM, busConstant.ZERO_DECIMAL);

                    ldecTotalLumpSum = GetMPIPHPLumpSum() + lbusBenefitCalculationOption.icdoBenefitCalculationOptions.benefit_amount;
                    if (ldecTotalLumpSum < busConstant.BenefitCalculation.LUMP_SUM_LIMIT)
                    {
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_700_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    }
                    break;
            }
            #endregion

            decimal ldecLumpSumFactor = ldecFactor = ibusCalculation.GetLumpsumBenefitFactorByAGE(Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge)) * 12;
            ldecLumpSumFactor = Math.Round(ldecLumpSumFactor, 3);
            decimal ldecPresentValue = Math.Round(adecTotalBenefitAmount * ldecLumpSumFactor, 3);
            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_700_PLAN_ID).First().icdoBenefitCalculationDetail.present_value_amount = ldecPresentValue;

        }

        public bool IsRetirementDateFirstOfMonth()
        {
            if (this.icdoBenefitCalculationHeader.retirement_date != DateTime.MinValue &&
                this.icdoBenefitCalculationHeader.retirement_date.Day != 1)
                return false;
            return true;
        }

        #endregion Local

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

            this.SetupPreRequisites_RetirementCalculations();

            this.ValidateHardErrors(utlPageMode.Update);

            if (this.iclbBenefitCalculationDetail.Count() > 0)
            {
                this.icdoBenefitCalculationHeader.iintPlanId = this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.plan_id;
            }

            if (!lblIsNew)
                FlushOlderCalculations();

            if (this.ibusBenefitApplication.FindBenefitApplication(this.icdoBenefitCalculationHeader.benefit_application_id))
            {
                this.ibusBenefitApplication.iclbBenefitApplicationDetail = new Collection<busBenefitApplicationDetail>();
                this.iclbPersonAccountRetirementContribution = new Collection<busPersonAccountRetirementContribution>();

                this.ibusBenefitApplication.LoadBenefitApplicationDetails();
                
                if (!ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).IsNullOrEmpty())
                {
                    LoadAllRetirementContributions(ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault());
                }
                else
                {
                    LoadAllRetirementContributions(null);
                }

                if (!this.ibusBenefitApplication.iclbBenefitApplicationDetail.IsNullOrEmpty())
                {
                    foreach (busBenefitApplicationDetail lbusBenefitApplicationDetail in this.ibusBenefitApplication.iclbBenefitApplicationDetail)
                    {
                       
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

                            if (this.ibusBenefitApplication.CheckAlreadyVested(lbusBenefitApplicationDetail.istrPlanCode))
                            {

                                if (lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue.IsNullOrEmpty() && lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrSubPlanBenefitOptionValue.IsNotNullOrEmpty())
                                {
                                    lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue = lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrSubPlanBenefitOptionValue;
                                }

                                this.SpawnFinalRetirementCalculation(lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.benefit_application_detail_id,
                                                                                                 this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == lbusBenefitApplicationDetail.istrPlanCode).FirstOrDefault().icdoPersonAccount.person_account_id,
                                                                                                 lbusBenefitApplicationDetail.iintPlan_ID, lbusBenefitApplicationDetail.istrPlanCode, this.ibusBenefitApplication.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date,
                                                                                                 lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.benefit_subtype_value, lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue);
                                try
                                {
                                    this.AfterPersistChanges();
                                }
                                catch
                                {
                                }
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

                            if (this.ibusBenefitApplication.CheckAlreadyVested(lbusBenefitApplicationDetail.istrPlanCode))
                            {

                                if (lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue.IsNullOrEmpty() && lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrSubPlanBenefitOptionValue.IsNotNullOrEmpty())
                                {
                                    lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue = lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrSubPlanBenefitOptionValue;
                                }

                                this.SpawnFinalRetirementCalculation(lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.benefit_application_detail_id,
                                                                                                 this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == lbusBenefitApplicationDetail.istrPlanCode).FirstOrDefault().icdoPersonAccount.person_account_id,
                                                                                                 lbusBenefitApplicationDetail.iintPlan_ID, lbusBenefitApplicationDetail.istrPlanCode, this.ibusBenefitApplication.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date,
                                                                                                 lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.benefit_subtype_value, lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue);
                                try
                                {
                                    this.AfterPersistChanges();
                                }
                                catch
                                {
                                }
                            }

                            #endregion
                        }

                        else if (lbusBenefitApplicationDetail.iintPlan_ID != busConstant.MPIPP_PLAN_ID && lbusBenefitApplicationDetail.iintPlan_ID != busConstant.IAP_PLAN_ID
                                 && this.icdoBenefitCalculationHeader.iintPlanId != busConstant.IAP_PLAN_ID && this.icdoBenefitCalculationHeader.iintPlanId != busConstant.MPIPP_PLAN_ID)
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
                                                                                                 lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.benefit_subtype_value, lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue);
                                try
                                {
                                    this.AfterPersistChanges();
                                }
                                catch
                                {
                                }
                            }

                            #endregion
                        }
                    }

                }

            }
            this.EvaluateInitialLoadRules();
            if (this.iclbBenefitCalculationDetail.Count() > 0)
            {
                this.iclbBenefitCalculationDetail.First().EvaluateInitialLoadRules();
                this.iclbBenefitCalculationDetail.First().iobjMainCDO = this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail;
            }

            larrList.Add(this);
            return larrList;
        }


        public void SpawnFinalRetirementCalculation(int aintBenefitApplicationDetailId, int aintPersonAccountId, int aintPlanId, string astrPlanCode,
                                        DateTime adtVestedDate, string astrBenefitSubTypeValue, string astrBenefitOptionValue, bool ablnReEvaluationMDBatch = false, bool ablnReemployed = false, string astrAdjustmentFlag = "",bool ablnConvertBenOption = false,string astrOriginalBenefitOption = "")
        {
            decimal ldecLateAdjustmentAmt = busConstant.ZERO_DECIMAL;
                     
            this.PopulateInitialDataBenefitCalculationDetails(aintBenefitApplicationDetailId, aintPersonAccountId, aintPlanId, astrPlanCode, adtVestedDate, astrBenefitSubTypeValue);
            
            decimal ldecTotalBenefitAmount = decimal.Zero;
            this.idecAgeDiff = this.icdoBenefitCalculationHeader.idecParticipantFullAge - this.icdoBenefitCalculationHeader.idecSurvivorFullAge;

            #region SWITCH CASE - INITIATE CALCULATION BASED ON THE PLAN
            if (!this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.IsNullOrEmpty())
            {
                int lintMaxComputationYear = 0;

                switch (astrPlanCode)
                {
                    case busConstant.Local_161:
                        if (this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == aintPersonAccountId).Count() > 0)
                        {
                            lintMaxComputationYear = Convert.ToInt32(this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == aintPersonAccountId).Max(item => item.icdoPersonAccountRetirementContribution.computational_year));
                        }

                        ldecTotalBenefitAmount = this.ibusCalculation.CalculateTotalBenefitAmtForLocal161(this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_161).First().icdoPersonAccount.istrRetirementSubType,
                                                                                 this.icdoBenefitCalculationHeader.retirement_date, this.ibusPerson.icdoPerson.idtDateofBirth,
                                                                                 this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == aintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_frozen_benefit_amount),
                                                                                 false, this.ibusBenefitApplication, this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == aintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_pre_bis_amount),
                                                                                 this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == aintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_post_bis_amount),
                                                                                 null, this.iclbBenefitCalculationDetail, lintMaxComputationYear, this.iclbPersonAccountRetirementContribution, busConstant.BOOL_TRUE, this.ibusPerson.icdoPerson.person_id);

                        
                        CalculateLocal161BenefitOptions(astrBenefitOptionValue, ldecTotalBenefitAmount);
                        
                        break;

                    case busConstant.Local_52:

                        if (this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == aintPersonAccountId).Count() > 0)
                        {
                            lintMaxComputationYear = Convert.ToInt32(this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == aintPersonAccountId).Max(item => item.icdoPersonAccountRetirementContribution.computational_year));
                        }

                        ldecTotalBenefitAmount = this.ibusCalculation.CalculateTotalBenefitAmtForLocal52(this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_52).First().icdoPersonAccount.istrRetirementSubType,
                                                     this.icdoBenefitCalculationHeader.retirement_date, this.ibusPerson.icdoPerson.idtDateofBirth,
                                                     this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == aintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_frozen_benefit_amount),
                                                     false, this.ibusBenefitApplication, this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == aintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_pre_bis_amount),
                                                     this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == aintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_post_bis_amount),
                                                     null, this.iclbBenefitCalculationDetail, lintMaxComputationYear,
                                                     this.iclbPersonAccountRetirementContribution, busConstant.BOOL_TRUE, this.ibusPerson.icdoPerson.person_id);

                        
                        CalculateLocal52BenefitOptions(astrBenefitOptionValue, ldecTotalBenefitAmount, ablnConvertBenefitOption : ablnConvertBenOption,astrOriginalBenefitOption : astrOriginalBenefitOption);//PIR 894
                        
                        break;

                    case busConstant.Local_600:

                        if (this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == aintPersonAccountId).Count() > 0)
                        {
                            lintMaxComputationYear = Convert.ToInt32(this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == aintPersonAccountId).Max(item => item.icdoPersonAccountRetirementContribution.computational_year));
                        }

                        ldecTotalBenefitAmount = this.ibusCalculation.CalculateTotalBenefitAmtForLocal600(this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_600).First().icdoPersonAccount.istrRetirementSubType,
                                                                                                          this.icdoBenefitCalculationHeader.retirement_date, this.ibusPerson.icdoPerson.idtDateofBirth,
                                                                                                          this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == aintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_frozen_benefit_amount),
                                                                                                           false, this.ibusBenefitApplication, this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == aintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_pre_bis_amount),
                                                                                                           this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == aintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_post_bis_amount),
                                                                                                           null, this.iclbBenefitCalculationDetail, lintMaxComputationYear,
                                                                                                           this.iclbPersonAccountRetirementContribution, busConstant.BOOL_TRUE, this.ibusPerson.icdoPerson.person_id);

                       
                        CalculateLocal600BenefitOptions(astrBenefitOptionValue, ldecTotalBenefitAmount, ablnConvertBenefitOption: ablnConvertBenOption, astrOriginalBenefitOption: astrOriginalBenefitOption); //894
                        
                        break;

                    case busConstant.Local_666:

                        if (this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == aintPersonAccountId).Count() > 0)
                        {
                            lintMaxComputationYear = Convert.ToInt32(this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == aintPersonAccountId).Max(item => item.icdoPersonAccountRetirementContribution.computational_year));
                        }

                        ldecTotalBenefitAmount = this.ibusCalculation.CalculateTotalBenefitAmtForLocal666(this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_666).First().icdoPersonAccount.istrRetirementSubType,
                                                                              this.icdoBenefitCalculationHeader.retirement_date, this.ibusPerson.icdoPerson.idtDateofBirth,
                                                                              this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == aintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_frozen_benefit_amount),
                                                                               false, this.ibusBenefitApplication, this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == aintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_pre_bis_amount),
                                                                               this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == aintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_post_bis_amount), this.icdoBenefitCalculationHeader.age,
                                                                               null, this.iclbBenefitCalculationDetail, lintMaxComputationYear,
                                                                               this.iclbPersonAccountRetirementContribution, busConstant.BOOL_TRUE, this.ibusPerson.icdoPerson.person_id);

                        
                        CalculateLocal666BenefitOptions(astrBenefitOptionValue, ldecTotalBenefitAmount,ablnConvertBenefitOption: ablnConvertBenOption, astrOriginalBenefitOption: astrOriginalBenefitOption);//PIR 894

                        break;

                    case busConstant.LOCAL_700:

                        if (this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == aintPersonAccountId).Count() > 0)
                        {
                            lintMaxComputationYear = Convert.ToInt32(this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == aintPersonAccountId).Max(item => item.icdoPersonAccountRetirementContribution.computational_year));
                        }

                        ldecTotalBenefitAmount = this.ibusCalculation.CalculateTotalBenefitAmtForLocal700(this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.LOCAL_700).First().icdoPersonAccount.istrRetirementSubType,
                                                  this.icdoBenefitCalculationHeader.retirement_date, this.ibusPerson.icdoPerson.idtDateofBirth,
                                                  this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == aintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_frozen_benefit_amount),
                                                   false, this.ibusBenefitApplication, this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == aintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_pre_bis_amount),
                                                   this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == aintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_post_bis_amount),
                                                   null, this.iclbBenefitCalculationDetail, lintMaxComputationYear,
                                                   this.iclbPersonAccountRetirementContribution, busConstant.BOOL_TRUE, this.ibusPerson.icdoPerson.person_id);

                       
                        CalculateLocal700BenefitOptions(astrBenefitOptionValue, ldecTotalBenefitAmount);
                        
                        break;

                    case busConstant.MPIPP:
                        if (this.ibusBenefitApplication.aclbPersonWorkHistory_MPI != null && this.ibusBenefitApplication.aclbPersonWorkHistory_MPI.Count > 0)
                        {
                            //PIR 1053 --Benefit Calculation for MD should be done up to end of plan year before MD date. Following code is added.
                            if ((ablnReEvaluationMDBatch == false &&
                                this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.MPIPP).Count() > 0 &&
                                this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.MPIPP).FirstOrDefault().icdoPersonAccount.istrRetirementSubType == busConstant.RETIREMENT_TYPE_MINIMUM_DISTRIBUTION)
                                                && this.ibusBenefitApplication.aclbPersonWorkHistory_MPI.Where(item => item.year < this.icdoBenefitCalculationHeader.retirement_date.Year).Count() > 0)
                            {
                                this.ibusBenefitApplication.aclbPersonWorkHistory_MPI = this.ibusBenefitApplication.aclbPersonWorkHistory_MPI.Where(item => item.year < this.icdoBenefitCalculationHeader.retirement_date.Year).ToList().ToCollection();
                            }


                            if (this.iblnCalculateMPIPPBenefit)
                            {
                                busPersonAccount lbusPersonAccount = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.MPIPP).FirstOrDefault();
                                decimal ldecBenefitAmt = ibusCalculation.CalculateBenefitAmtForPension(ibusPerson, busConstant.BENEFIT_TYPE_RETIREMENT, icdoBenefitCalculationHeader.age, icdoBenefitCalculationHeader.retirement_date,
                                    this.ibusBenefitApplication.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date,
                                    this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.MPIPP).FirstOrDefault(),
                                    this.ibusBenefitApplication, false, iclbBenefitCalculationDetail, this.iclbPersonAccountRetirementContribution, null, true, lbusPersonAccount.icdoPersonAccount.istrRetirementSubType, ref ldecLateAdjustmentAmt, this.ibusPerson.icdoPerson.person_id, ablnReEvaluationMDBatch);

                                ldecBenefitAmt = Math.Round(ldecBenefitAmt, 2);//PIR 627 10292015
                                CalculateFinalBenefitForPensionBenefitOptions(ldecBenefitAmt, astrBenefitOptionValue, busConstant.MPIPP_PLAN_ID,ablnConvertBenefitOption : ablnConvertBenOption,astrOriginalBenefitOption : astrOriginalBenefitOption);//PIR 894

                                //PIR 894
                                if(ablnConvertBenOption && (astrOriginalBenefitOption == busConstant.JOINT_100_PERCENT_POPUP_ANNUITY || astrOriginalBenefitOption == busConstant.JOINT_50_PERCENT_POPUP_ANNUITY)
                                   && this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().iclbBenefitCalculationYearlyDetail.Count > 0
                                   && this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().iclbBenefitCalculationOptions.Count > 0)
                                {

                                    foreach(busBenefitCalculationYearlyDetail lbusbenefitcalculationyearlydetail in this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().iclbBenefitCalculationYearlyDetail)
                                    {
                                        if(lbusbenefitcalculationyearlydetail.icdoBenefitCalculationYearlyDetail.annual_adjustment_amount != 0 || lbusbenefitcalculationyearlydetail.icdoBenefitCalculationYearlyDetail.plan_year >= this.icdoBenefitCalculationHeader.retirement_date.Year) //RequestID: 72091
                                            lbusbenefitcalculationyearlydetail.icdoBenefitCalculationYearlyDetail.benefit_as_of_det_date = Math.Round(this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().iclbBenefitCalculationOptions.FirstOrDefault().icdoBenefitCalculationOptions.pop_up_benefit_amount, 2);
                                    }
                                }
                                decimal ldecEEderivedAmount = 0M;
                                //changes for Late Retirement EE derived amount while pushing in Benefit calculation Detail's grid
                                if (lbusPersonAccount.icdoPersonAccount.istrRetirementSubType != busConstant.RETIREMENT_TYPE_LATE)
                                {
                                    ldecEEderivedAmount = ibusCalculation.CalculateEEDerivedBenefitAsOfRetirementDate(this, null);
                                }
                                else
                                {
                                    if (this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId) != null
                                        && this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).Count() > 0
                                        && this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().iclbBenefitCalculationYearlyDetail != null
                                        && this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().iclbBenefitCalculationYearlyDetail.Count > 0)
                                    {
                                        ldecEEderivedAmount = this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().
                                            iclbBenefitCalculationYearlyDetail.LastOrDefault().icdoBenefitCalculationYearlyDetail.max_ee_derv_amt;
                                    }
                                }

                                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().icdoBenefitCalculationDetail.ee_derived_benefit_amount = Math.Round(
                                    Math.Round(this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().icdoBenefitCalculationDetail.early_reduction_factor * ldecEEderivedAmount, 2) *
                                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().iclbBenefitCalculationOptions.FirstOrDefault().icdoBenefitCalculationOptions.benefit_option_factor, 2);
                            }
                        }

                        break;

                    case busConstant.IAP:
                        if (this.ibusBenefitApplication.aclbPersonWorkHistory_IAP != null && this.ibusBenefitApplication.aclbPersonWorkHistory_IAP.Count > 0)
                        {
                            this.CalculateIAPBenefitAmount(astrBenefitOptionValue, ablnReemployed, astrAdjustmentFlag);
                        }
                        break;
                }
            }
            #endregion

            #region Set Accrued At Retirement in Detail Object , will be used for ReEmployment
            if (this.iclbBenefitCalculationDetail.Count > 0)
            {
                if (ldecLateAdjustmentAmt == decimal.Zero)
                {
                    this.iclbBenefitCalculationDetail.FirstOrDefault().icdoBenefitCalculationDetail.accrued_at_retirement_amount = ldecTotalBenefitAmount;
                }
                else
                {
                    this.iclbBenefitCalculationDetail.FirstOrDefault().icdoBenefitCalculationDetail.accrued_at_retirement_amount = ldecLateAdjustmentAmt;
                }
            }
            #endregion
        }

        /// <summary>
        /// set values for correspondence
        /// </summary>
        /// <param name="abusPerson"></param>
        /// <returns></returns>
        public busBenefitCalculationRetirement GenerateMinDistributionEstiFromBatch(busPerson abusPerson, DateTime adtBatchRunDate, string astrPlanCode)
        {
            int lintBeneficiaryPersonId = 0;
            busBenefitCalculationRetirement lbusBenefitCalculationHeader =
                new busBenefitCalculationRetirement { icdoBenefitCalculationHeader = new cdoBenefitCalculationHeader() };

            lbusBenefitCalculationHeader.ibusPerson = abusPerson;
            lbusBenefitCalculationHeader.ibusPerson.LoadPersonAccounts();

            lbusBenefitCalculationHeader.iclbPersonAccountRetirementContribution = new Collection<busPersonAccountRetirementContribution>();
            //lbusBenefitCalculationHeader.LoadAllRetirementContributions();

            if (!lbusBenefitCalculationHeader.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).IsNullOrEmpty())
            {
                lbusBenefitCalculationHeader.LoadAllRetirementContributions(lbusBenefitCalculationHeader.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault());
            }
            else
            {
                lbusBenefitCalculationHeader.LoadAllRetirementContributions(null);
            }

            // Initial Setup for Checking Eligbility
            lbusBenefitCalculationHeader.ibusBenefitApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
            lbusBenefitCalculationHeader.ibusBenefitApplication.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
            lbusBenefitCalculationHeader.ibusBenefitApplication.ibusPerson.iclbPersonAccount = new Collection<busPersonAccount>();
            lbusBenefitCalculationHeader.ibusBenefitApplication.ibusPerson = lbusBenefitCalculationHeader.ibusPerson;
            lbusBenefitCalculationHeader.ibusBenefitApplication.ibusPerson.iclbPersonAccount = lbusBenefitCalculationHeader.ibusPerson.iclbPersonAccount;

            //PIR 355 MD Batch Esti Calc - Incorrect Benefit Rate (emailed issue on 05/12/2015)
            lbusBenefitCalculationHeader.ibusBenefitApplication.icdoBenefitApplication.retirement_date = busGlobalFunctions.GetLastDayofMonth(DateTime.Now).AddDays(1);

            lbusBenefitCalculationHeader.ibusBenefitApplication.LoadandProcessWorkHistory_ForAllPlans(); //Code-Added -Abhishek (Imp to have work history state in background)

            if (lbusBenefitCalculationHeader.ibusBenefitApplication.aclbPersonWorkHistory_MPI.IsNullOrEmpty() && lbusBenefitCalculationHeader.ibusBenefitApplication.aclbPersonWorkHistory_IAP.IsNullOrEmpty())
            {
                return lbusBenefitCalculationHeader;
            }

            lbusBenefitCalculationHeader.iclbBenefitCalculationDetail = new Collection<busBenefitCalculationDetail>();

            #region Load Spouse Detail
            busRelationship lbusRelationship = new busRelationship();
            busPerson lbusParticipantBeneficiary = new busPerson();
            lbusParticipantBeneficiary = lbusRelationship.LoadExistingSpouseDetails(lbusBenefitCalculationHeader.ibusPerson.icdoPerson.person_id);

            if (lbusParticipantBeneficiary != null)
            {
                lintBeneficiaryPersonId = lbusParticipantBeneficiary.icdoPerson.person_id;                
            }
            else
            {
                iblnNoBeneficiaryExists = busConstant.MPIPHPBatch.NOTAPPLICABLE;
            }

            #endregion
            int lintPersonAccountId = 0;
            if (lbusBenefitCalculationHeader.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(
                                             plan => plan.icdoPersonAccount.plan_id == abusPerson.icdoPerson.iintPlanId).Count() > 0)
                lintPersonAccountId = lbusBenefitCalculationHeader.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(
                                                 plan => plan.icdoPersonAccount.plan_id == abusPerson.icdoPerson.iintPlanId).First().icdoPersonAccount.person_account_id;

            if (lbusBenefitCalculationHeader.ibusBenefitApplication.ibusTempPersonAccountEligibility != null)
            {
                //PIR 854
                if (astrPlanCode == busConstant.MPIPP)
                {
                    lbusBenefitCalculationHeader.ibusBenefitApplication.CheckAlreadyVested(astrPlanCode);
                }

                DateTime ldtVestedDate = lbusBenefitCalculationHeader.ibusBenefitApplication.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date;

                //RID 118418 USING NEW FUNCTION TO GET MD DATE
                DateTime ldtMDDate = busGlobalFunctions.GetMinDistributionDate(lbusBenefitCalculationHeader.ibusPerson.icdoPerson.person_id, ldtVestedDate);
                lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.retirement_date = ldtMDDate;

                //if (ldtVestedDate > abusPerson.icdoPerson.idtDatePersonAge70andHalf)
                //{
                //    lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.retirement_date = new DateTime(ldtVestedDate.Year + 1, 01, 01);//PIR 854
                //}
                //else
                //{
                //    lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.retirement_date =
                //                                Convert.ToDateTime(busGlobalFunctions.CalculateMinDistributionDate(abusPerson.icdoPerson.idtDateofBirth, ldtVestedDate));//new DateTime(adtBatchRunDate.Year + 1, 04, 1);
                //}

                lbusBenefitCalculationHeader.ibusBenefitApplication.icdoBenefitApplication.retirement_date = lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.retirement_date;

                decimal ldecPartcipantAge = busGlobalFunctions.CalculatePersonAgeInDec(lbusBenefitCalculationHeader.ibusPerson.icdoPerson.idtDateofBirth,
                                                                                        lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.retirement_date);

                lbusBenefitCalculationHeader.PopulateInitialDataBenefitCalculationHeader(lbusBenefitCalculationHeader.ibusPerson.icdoPerson.person_id, busConstant.ZERO_INT, lintBeneficiaryPersonId,
                                                               busConstant.BENEFIT_TYPE_RETIREMENT, busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE,
                                                               lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.retirement_date,
                                                               ldecPartcipantAge, abusPerson.icdoPerson.iintPlanId);

                lbusBenefitCalculationHeader.ibusBenefitApplication.idecAge = lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.age;
                lbusBenefitCalculationHeader.ibusBenefitApplication.LoadWorkHistoryandSetupPrerequisites_Retirement();

                //PIR 854 On the Minimum Distribution Calculations OPUS should not include the accrued benefit for hours worked in the year of MD date              
                if (lbusBenefitCalculationHeader.ibusBenefitApplication.aclbPersonWorkHistory_MPI != null && lbusBenefitCalculationHeader.ibusBenefitApplication.aclbPersonWorkHistory_MPI.Count > 0
                    && lbusBenefitCalculationHeader.ibusBenefitApplication.aclbPersonWorkHistory_MPI.Where(item => item.year < lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.retirement_date.Year).Count() > 0)
                {
                    lbusBenefitCalculationHeader.ibusBenefitApplication.aclbPersonWorkHistory_MPI = lbusBenefitCalculationHeader.ibusBenefitApplication.aclbPersonWorkHistory_MPI.Where(item => item.year < lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.retirement_date.Year).ToList().ToCollection();
                }

                bool lblnIsEligibleForRetirement = false;

                foreach (string lstrPlanCode in lbusBenefitCalculationHeader.ibusBenefitApplication.iclbEligiblePlans)
                {
                    lbusBenefitCalculationHeader.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.istrPlanCode == lstrPlanCode).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_MINIMUM_DISTRIBUTION;
                    if (lstrPlanCode == astrPlanCode)
                    {
                        lblnIsEligibleForRetirement = true;
                    }
                }

                if (lblnIsEligibleForRetirement)
                {

                    lbusBenefitCalculationHeader.iblnCalcualteUVHPBenefit = lbusBenefitCalculationHeader.iblnCalculateIAPBenefit = lbusBenefitCalculationHeader.iblnCalculateL161SplAccBenefit = lbusBenefitCalculationHeader.iblnCalculateL52SplAccBenefit = lbusBenefitCalculationHeader.iblnCalculateMPIPPBenefit = true;
                    lbusBenefitCalculationHeader.Setup_Retirement_Calculations(true);
                    //Check added for MPI plan : There are a few scenarios where the participant is vested under Local Plan and MPI/IAP because of merger but no hours worked under MPI
                    if (!lbusBenefitCalculationHeader.iclbBenefitCalculationDetail.IsNullOrEmpty())
                    {
                        bool lblnCheckIfOptions = false;
                        if (lbusBenefitCalculationHeader.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).Count() > 0)
                        {
                            if (!lbusBenefitCalculationHeader.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().iclbBenefitCalculationOptions.IsNullOrEmpty())
                            {
                                lblnCheckIfOptions = true;
                            }
                        }
                        else
                        {
                            if (!lbusBenefitCalculationHeader.iclbBenefitCalculationDetail.FirstOrDefault().iclbBenefitCalculationOptions.IsNullOrEmpty())
                            {
                                lblnCheckIfOptions = true;
                            }
                        }
                        if (lblnCheckIfOptions)
                        {
                            lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.Insert();
                            lbusBenefitCalculationHeader.AfterPersistChanges();
                        }
                    }
                }
            }
            return lbusBenefitCalculationHeader;
        }

        public busBenefitCalculationRetirement GenerateRequiredMinDistributionEstiFromBatch(busPerson abusPerson, DateTime adtBatchRunDate, string astrPlanCode, DateTime retirement_date)
        {
            int lintBeneficiaryPersonId = 0;
            busBenefitCalculationRetirement lbusBenefitCalculationHeader =
                new busBenefitCalculationRetirement { icdoBenefitCalculationHeader = new cdoBenefitCalculationHeader() };

            lbusBenefitCalculationHeader.ibusPerson = abusPerson;
            lbusBenefitCalculationHeader.ibusPerson.LoadPersonAccounts();

            lbusBenefitCalculationHeader.iclbPersonAccountRetirementContribution = new Collection<busPersonAccountRetirementContribution>();
            //lbusBenefitCalculationHeader.LoadAllRetirementContributions();

            if (!lbusBenefitCalculationHeader.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.IAP_PLAN_ID).IsNullOrEmpty())
            {
                lbusBenefitCalculationHeader.LoadAllRetirementContributions(lbusBenefitCalculationHeader.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault());
            }
            else
            {
                lbusBenefitCalculationHeader.LoadAllRetirementContributions(null);
            }

            // Initial Setup for Checking Eligbility
            lbusBenefitCalculationHeader.ibusBenefitApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
            lbusBenefitCalculationHeader.ibusBenefitApplication.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
            lbusBenefitCalculationHeader.ibusBenefitApplication.ibusPerson.iclbPersonAccount = new Collection<busPersonAccount>();
            lbusBenefitCalculationHeader.ibusBenefitApplication.ibusPerson = lbusBenefitCalculationHeader.ibusPerson;
            lbusBenefitCalculationHeader.ibusBenefitApplication.ibusPerson.iclbPersonAccount = lbusBenefitCalculationHeader.ibusPerson.iclbPersonAccount;

            //PIR 355 MD Batch Esti Calc - Incorrect Benefit Rate (emailed issue on 05/12/2015)
           // lbusBenefitCalculationHeader.ibusBenefitApplication.icdoBenefitApplication.retirement_date = busGlobalFunctions.GetLastDayofMonth(DateTime.Now).AddDays(1);

            lbusBenefitCalculationHeader.ibusBenefitApplication.LoadandProcessWorkHistory_ForAllPlans(); //Code-Added -Abhishek (Imp to have work history state in background)

            if (lbusBenefitCalculationHeader.ibusBenefitApplication.aclbPersonWorkHistory_MPI.IsNullOrEmpty() && lbusBenefitCalculationHeader.ibusBenefitApplication.aclbPersonWorkHistory_IAP.IsNullOrEmpty())
            {
                return lbusBenefitCalculationHeader;
            }

            lbusBenefitCalculationHeader.iclbBenefitCalculationDetail = new Collection<busBenefitCalculationDetail>();

            #region Load Spouse Detail
            busRelationship lbusRelationship = new busRelationship();
            busPerson lbusParticipantBeneficiary = new busPerson();
            lbusParticipantBeneficiary = lbusRelationship.LoadExistingSpouseDetails(lbusBenefitCalculationHeader.ibusPerson.icdoPerson.person_id);

            if (lbusParticipantBeneficiary != null)
            {
                lintBeneficiaryPersonId = lbusParticipantBeneficiary.icdoPerson.person_id;
            }
            else
            {
                iblnNoBeneficiaryExists = busConstant.MPIPHPBatch.NOTAPPLICABLE;
            }

            #endregion
            int lintPersonAccountId = 0;
            if (lbusBenefitCalculationHeader.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(
                                             plan => plan.icdoPersonAccount.plan_id == abusPerson.icdoPerson.iintPlanId).Count() > 0)
                lintPersonAccountId = lbusBenefitCalculationHeader.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(
                                                 plan => plan.icdoPersonAccount.plan_id == abusPerson.icdoPerson.iintPlanId).First().icdoPersonAccount.person_account_id;

            if (lbusBenefitCalculationHeader.ibusBenefitApplication.ibusTempPersonAccountEligibility != null)
            {
                //PIR 854
                if (astrPlanCode == busConstant.MPIPP)
                {
                    lbusBenefitCalculationHeader.ibusBenefitApplication.CheckAlreadyVested(astrPlanCode);
                }

                DateTime ldtVestedDate = lbusBenefitCalculationHeader.ibusBenefitApplication.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date;

                if (ldtVestedDate > abusPerson.icdoPerson.idtDatePersonAge70andHalf)
                {
                    lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.retirement_date = new DateTime(ldtVestedDate.Year + 1, 01, 01);//PIR 854
                }
                else
                {
                    lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.retirement_date =
                                                Convert.ToDateTime(busGlobalFunctions.CalculateMinDistributionDate(abusPerson.icdoPerson.idtDateofBirth, ldtVestedDate));//new DateTime(adtBatchRunDate.Year + 1, 04, 1);
                }

                if(retirement_date <= DateTime.Now)
                {
                    //var ldate = retirement_date;
                    //if(ldate.Month == 4)
                    //{
                    //    if(DateTime.Now.Month <= 4)
                    //    {
                    //        lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.retirement_date = new DateTime(DateTime.Now.Year, 04, 01);
                    //    }
                    //    else
                    //    {
                    //        lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.retirement_date = new DateTime(DateTime.Now.Year + 1, 04, 01);

                    //    }
                        

                    //}
                    //else
                    //{
                        lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.retirement_date = busGlobalFunctions.GetLastDayofMonth(DateTime.Now).AddDays(1);
                    // }

                }
                else
                {
                    lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.retirement_date = retirement_date;
                }
                // var ldate = lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.retirement_date;
               // lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.retirement_date = retirement_date; //new DateTime(ldtVestedDate.Year + 1, ldate.Month, 01); //busGlobalFunctions.GetLastDayofMonth(DateTime.Now).AddDays(1);
               


                lbusBenefitCalculationHeader.ibusBenefitApplication.icdoBenefitApplication.retirement_date = lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.retirement_date;

                decimal ldecPartcipantAge = busGlobalFunctions.CalculatePersonAgeInDec(lbusBenefitCalculationHeader.ibusPerson.icdoPerson.idtDateofBirth,
                                                                                        lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.retirement_date);

                lbusBenefitCalculationHeader.PopulateInitialDataBenefitCalculationHeader(lbusBenefitCalculationHeader.ibusPerson.icdoPerson.person_id, busConstant.ZERO_INT, lintBeneficiaryPersonId,
                                                               busConstant.BENEFIT_TYPE_RETIREMENT, busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE,
                                                               lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.retirement_date,
                                                               ldecPartcipantAge, abusPerson.icdoPerson.iintPlanId);

                lbusBenefitCalculationHeader.ibusBenefitApplication.idecAge = lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.age;
                lbusBenefitCalculationHeader.ibusBenefitApplication.LoadWorkHistoryandSetupPrerequisites_Retirement();

                //PIR 854 On the Minimum Distribution Calculations OPUS should not include the accrued benefit for hours worked in the year of MD date              
                if (lbusBenefitCalculationHeader.ibusBenefitApplication.aclbPersonWorkHistory_MPI != null && lbusBenefitCalculationHeader.ibusBenefitApplication.aclbPersonWorkHistory_MPI.Count > 0
                    && lbusBenefitCalculationHeader.ibusBenefitApplication.aclbPersonWorkHistory_MPI.Where(item => item.year < lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.retirement_date.Year).Count() > 0)
                {
                    lbusBenefitCalculationHeader.ibusBenefitApplication.aclbPersonWorkHistory_MPI = lbusBenefitCalculationHeader.ibusBenefitApplication.aclbPersonWorkHistory_MPI.Where(item => item.year < lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.retirement_date.Year).ToList().ToCollection();
                }

                //bool lblnIsEligibleForRetirement = false;

                //foreach (string lstrPlanCode in lbusBenefitCalculationHeader.ibusBenefitApplication.iclbEligiblePlans)
                //{
                //    lbusBenefitCalculationHeader.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.istrPlanCode == lstrPlanCode).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_MINIMUM_DISTRIBUTION;
                //    if (lstrPlanCode == astrPlanCode)
                //    {
                //        lblnIsEligibleForRetirement = true;
                //    }
                //}

                //if (lblnIsEligibleForRetirement)
                //{

                    lbusBenefitCalculationHeader.iblnCalcualteUVHPBenefit = lbusBenefitCalculationHeader.iblnCalculateIAPBenefit = lbusBenefitCalculationHeader.iblnCalculateL161SplAccBenefit = lbusBenefitCalculationHeader.iblnCalculateL52SplAccBenefit = lbusBenefitCalculationHeader.iblnCalculateMPIPPBenefit = true;
                    lbusBenefitCalculationHeader.Setup_Retirement_Calculations(true);
                    //Check added for MPI plan : There are a few scenarios where the participant is vested under Local Plan and MPI/IAP because of merger but no hours worked under MPI
                    if (!lbusBenefitCalculationHeader.iclbBenefitCalculationDetail.IsNullOrEmpty())
                    {
                        bool lblnCheckIfOptions = false;
                        if (lbusBenefitCalculationHeader.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).Count() > 0)
                        {
                            if (!lbusBenefitCalculationHeader.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().iclbBenefitCalculationOptions.IsNullOrEmpty())
                            {
                                lblnCheckIfOptions = true;
                            }
                        }
                        else
                        {
                            if (!lbusBenefitCalculationHeader.iclbBenefitCalculationDetail.FirstOrDefault().iclbBenefitCalculationOptions.IsNullOrEmpty())
                            {
                                lblnCheckIfOptions = true;
                            }
                        }
                        if (lblnCheckIfOptions)
                        {
                            lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.Insert();
                            lbusBenefitCalculationHeader.AfterPersistChanges();
                        }
                    }
                }
            //}
            return lbusBenefitCalculationHeader;
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
                UpdateorInsertValuesInPayeeAccountTable(DateTime.MinValue);
            }

            return this.iarrErrors;
        }

        public void UpdateorInsertValuesInPayeeAccountTable(DateTime adtOrginalRetirementDate, bool ablnMDReEvaluationBatch = false)
        {

            #region PAYEE ACCOUNT RELATED LOGIC (PAYMENT - SPRINT 3.0) -- Abhishek
            int flag = 0;
            if (flag != 1)
            {
                if (this.iclbBenefitCalculationDetail.First().iclbBenefitCalculationOptions.Count > 0
                    && this.iclbBenefitCalculationDetail.First().iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.benefit_amount <= Decimal.Zero
                    && !ablnMDReEvaluationBatch &&
                    !(this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                            icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT))//PIR 985 10262015
                {
                    utlError lobjError = new utlError();
                    lobjError = AddError(6057, "");
                    this.iarrErrors.Add(lobjError);
                }

                else if ((this.iclbBenefitCalculationDetail.First().iclbBenefitCalculationOptions.Count() > 0 && this.iclbBenefitCalculationDetail.First().iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.benefit_amount > Decimal.Zero)
                    || (this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                            icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT))//PIR 985 10262015
                {
                    int lintBenefitAccountID = 0;
                    int lintPayeeAccountID = 0;


                    string lstrFundsType = String.Empty;

                    //Benefit Account Related
                    decimal ldecAccountOwnerStartingTaxableAmount = 0.0M;
                    decimal ldecAccountOwnerStartingNonTaxableAmount = 0.0M;
                    decimal ldecAccountOwnerStartingGrossAmount = 0.0M;

                    decimal ldecPopUpBenefitAmount = 0M;


                    busPayeeAccount lbusPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
                    busPayeeBenefitAccount lbusPayeeBenefitAccount = new busPayeeBenefitAccount { icdoPayeeBenefitAccount = new cdoPayeeBenefitAccount() };

                    DateTime ldteBenefitBeginDate = new DateTime();
                    //Prod PIR 229
                    if (adtOrginalRetirementDate != DateTime.MinValue && ablnMDReEvaluationBatch)
                    {
                        ldteBenefitBeginDate = busPayeeAccountHelper.GetBenefitBeginDate(adtOrginalRetirementDate, this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.plan_id, this.iclbBenefitCalculationDetail.FirstOrDefault().icdoBenefitCalculationDetail.benefit_subtype_value);
                    }
                    else
                    {
                        ldteBenefitBeginDate = busPayeeAccountHelper.GetBenefitBeginDate(this.icdoBenefitCalculationHeader.retirement_date, this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.plan_id, this.iclbBenefitCalculationDetail.FirstOrDefault().icdoBenefitCalculationDetail.benefit_subtype_value);
                    }

                    busPayeeAccount lbusPayeeAccountMinDist = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
                    lbusPayeeAccountMinDist.FindPayeeAccount(this.icdoBenefitCalculationHeader.payee_account_id);
                    lbusPayeeAccountMinDist.LoadPaymentHistoryHeaderDetails();
                    lbusPayeeAccountMinDist.LoadNextBenefitPaymentDate();



                    switch (this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.plan_id)
                    {
                        //R3view - Based on Per Plan we need to set the TAXABLE and NON-TAXABLE ITEMS
                        case busConstant.MPIPP_PLAN_ID:
                            if (this.icdoBenefitCalculationHeader.payee_account_id > 0)
                            {
                                DataTable ldtblPayeeBenefitAccount = Select("cdoPayeeBenefitAccount.GetPayeeBenefitAccount", new object[1] { lbusPayeeAccountMinDist.icdoPayeeAccount.payee_benefit_account_id });
                                if (ldtblPayeeBenefitAccount.Rows.Count > 0)
                                {
                                    if (Convert.ToString(ldtblPayeeBenefitAccount.Rows[0][enmPayeeBenefitAccount.starting_taxable_amount.ToString().ToUpper()]).IsNotNullOrEmpty())
                                        ldecAccountOwnerStartingTaxableAmount = Convert.ToDecimal(ldtblPayeeBenefitAccount.Rows[0][enmPayeeBenefitAccount.starting_taxable_amount.ToString().ToUpper()]);

                                    if (Convert.ToString(ldtblPayeeBenefitAccount.Rows[0][enmPayeeBenefitAccount.starting_nontaxable_amount.ToString().ToUpper()]).IsNotNullOrEmpty())
                                        ldecAccountOwnerStartingNonTaxableAmount = lbusPayeeAccountMinDist.idecRemainingNonTaxableBeginningBalance;//PIR - 915
                                    ldecAccountOwnerStartingGrossAmount = ldecAccountOwnerStartingTaxableAmount + ldecAccountOwnerStartingNonTaxableAmount;
                                    lstrFundsType = Convert.ToString(ldtblPayeeBenefitAccount.Rows[0][enmPayeeBenefitAccount.funds_type_value.ToString().ToUpper()]);
                                    this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.minimum_guarantee_amount = lbusPayeeAccountMinDist.idecRemainingMinimumGuaranteeAmount; //PIR - 915
                                }
                            }
                            else
                            {
                                ldecAccountOwnerStartingNonTaxableAmount = this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.vested_ee_amount;

                                ldecAccountOwnerStartingTaxableAmount = this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.vested_ee_interest;
                            }
                            ldecAccountOwnerStartingGrossAmount = ldecAccountOwnerStartingNonTaxableAmount + ldecAccountOwnerStartingTaxableAmount;

                            break;

                        case busConstant.IAP_PLAN_ID:

                            ldecAccountOwnerStartingGrossAmount = this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.iap_balance_amount; //+ this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.qdro_offset;
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
                                                                                         busConstant.BENEFIT_TYPE_RETIREMENT, lstrFundsType,
                                                                                         this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.benefit_application_detail_id, 0);  //R3view  the Query and code for this one.

                    lintBenefitAccountID = lbusPayeeBenefitAccount.ManagePayeeBenefitAccount(lintBenefitAccountID, this.icdoBenefitCalculationHeader.person_id,
                                                                      this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).First().icdoPersonAccount.person_account_id,
                                                                      ldecAccountOwnerStartingTaxableAmount, ldecAccountOwnerStartingNonTaxableAmount, ldecAccountOwnerStartingGrossAmount, lstrFundsType);

                    //Payee Account                  
                    busPayeeAccount lbusIAPPayeeAccount = null;

                    //PIR 944
                    if (this.icdoBenefitCalculationHeader.lump_sum_payment == busConstant.FLAG_YES)
                    {
                        DataTable ldtbResult = new DataTable();

                        ldtbResult = busBase.Select<cdoPayeeAccount>(
                              new string[5] { "PERSON_ID", "ACCOUNT_RELATION_VALUE", "PAYEE_BENEFIT_ACCOUNT_ID", "BENEFIT_ACCOUNT_TYPE_VALUE", "PLAN_BENEFIT_ID" },
                              new object[5] { this.icdoBenefitCalculationHeader.person_id, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER, lintBenefitAccountID, busConstant.BENEFIT_TYPE_RETIREMENT, 9 }, null, null);
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
                            this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.plan_id, astrBenefitOption: this.iclbBenefitCalculationDetail.First().iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.istrBenefitOptionDescription,0,astrRetirementType: this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.benefit_subtype_value, this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.benefit_application_detail_id);//RID 60954
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

                    //PIR 985 10222015
                    if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT)
                    {
                        if (this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID)
                        {
                            lbusIAPPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
                            if (lintPayeeAccountID > 0)
                            {
                                lbusIAPPayeeAccount.FindPayeeAccount(lintPayeeAccountID);
                            }
                        }
                    }

                    decimal ldecNonTaxableBeginningBalance = 0.0M;
                    DateTime ldteTermCertainEndDate = new DateTime();
                    string lstrFamilyRelationshipValue = string.Empty;


                    //IF Term Year Certain Option FIND the end Date 
                    LoadPlanBenefitsForPlan(this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.plan_id);
                    iintTermCertainMonths = busConstant.ZERO_INT;
                    iintTermCertainMonths = busPayeeAccountHelper.IsTermCertainBenefitOption(this.iclbBenefitCalculationDetail.First().iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.plan_benefit_id, this.iclbcdoPlanBenefit);
                    if (iintTermCertainMonths > 0)
                    {
                        //PIR RID 72598 In case of MD to RD and Term Certain Benefit option use original Term Certain date
                        if ((this.iclbBenefitCalculationDetail != null && this.iclbBenefitCalculationDetail.Count() > 0) &&
                                this.iclbBenefitCalculationDetail.FirstOrDefault().icdoBenefitCalculationDetail.benefit_subtype_value == busConstant.RETIREMENT_TYPE_LATE)
                        {

                            #region In case of MD to RD and Term Certain Benefit option use original Term Certain date
                            DataTable ldtMDPayeeAccount = busBase.Select<cdoPayeeAccount>(
                               new string[2] { enmPayeeAccount.person_id.ToString(), enmPayeeAccount.retirement_type_value.ToString() },
                             new object[2] { this.icdoBenefitCalculationHeader.person_id, busConstant.RETIREMENT_TYPE_MINIMUM_DISTRIBUTION }, null, null);

                            busPayeeAccount lbusMDPayeeAccount = null;
                            busPayeeAccountStatus lbusMDPayeeAccountStatus = null;
                            if (ldtMDPayeeAccount != null && ldtMDPayeeAccount.Rows.Count > 0)
                            {
                                lbusMDPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
                                lbusMDPayeeAccount.icdoPayeeAccount.LoadData(ldtMDPayeeAccount.Rows[0]);
                                lbusMDPayeeAccount.LoadNextBenefitPaymentDate();

                                DataTable ldtMDPayeeAccountStatus = busBase.Select<cdoPayeeAccountStatus>(
                                new string[1] { enmPayeeAccountStatus.payee_account_id.ToString() },
                                 new object[1] { lbusMDPayeeAccount.icdoPayeeAccount.payee_account_id }, null, "status_effective_date desc");

                                if (ldtMDPayeeAccountStatus != null && ldtMDPayeeAccountStatus.Rows.Count > 0)
                                {
                                    lbusMDPayeeAccountStatus = new busPayeeAccountStatus { icdoPayeeAccountStatus = new cdoPayeeAccountStatus() };
                                    lbusMDPayeeAccountStatus.icdoPayeeAccountStatus.LoadData(ldtMDPayeeAccountStatus.Rows[0]);
                                }
                            }
                            if (lbusMDPayeeAccount != null && lbusMDPayeeAccountStatus != null && lbusMDPayeeAccountStatus.icdoPayeeAccountStatus.status_value == busConstant.PAYEE_ACCOUNT_STATUS_COMPLETED)
                            {
                                ldteTermCertainEndDate = lbusMDPayeeAccount.icdoPayeeAccount.term_certain_end_date;
                            }
                            else
                            {
                                ldteTermCertainEndDate = ldteBenefitBeginDate.AddMonths(iintTermCertainMonths);
                                if (ldteTermCertainEndDate != DateTime.MinValue)
                                    ldteTermCertainEndDate = ldteTermCertainEndDate.AddDays(-1);
                            }
                            #endregion In case of MD to RD and Term Certain Benefit option use original Term Certain date
                        }
                        else
                        {
                            ldteTermCertainEndDate = ldteBenefitBeginDate.AddMonths(iintTermCertainMonths);
                            if (ldteTermCertainEndDate != DateTime.MinValue)
                                ldteTermCertainEndDate = ldteTermCertainEndDate.AddDays(-1);
                        }
                    }


                    busPayeeAccount lbusOldPayeeDetails = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };

                    //NonTaxable Beginning Balance
                    bool lblnAdjustmentPaymentFlag = false;
                    if (this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.adjustment_iap_payment_flag == busConstant.FLAG_YES)
                    {
                        lblnAdjustmentPaymentFlag = true;
                    }
                    DateTime adtAdjustFromDate = ldteBenefitBeginDate;//If Late Hours reported
                    if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT)
                    {
                        DateTime ldtlastWorkingDate = new DateTime();
                        string lstrEmployerName = string.Empty;
                        if (idictWorkHrsAfterRetirement.IsNullOrEmpty())
                            idictWorkHrsAfterRetirement = ibusCalculation.LoadMPIHoursAfterRetirementDate(this.ibusPerson.icdoPerson.istrSSNNonEncrypted, this.icdoBenefitCalculationHeader.retirement_date, this.icdoBenefitCalculationHeader.iintPlanId, ref ldtlastWorkingDate, ref lstrEmployerName);

                        busPlanBenefitXr lbusPlanBenXr = new busPlanBenefitXr();
                        lbusPlanBenXr.FindPlanBenefitXr(this.iclbBenefitCalculationDetail.First().iclbBenefitCalculationOptions.FirstOrDefault().icdoBenefitCalculationOptions.plan_benefit_id);
                        if (lintPayeeAccountID > 0)
                        {
                            lbusOldPayeeDetails.FindPayeeAccount(lintPayeeAccountID);
                            lbusOldPayeeDetails.LoadBenefitDetails();
                            if ((lbusOldPayeeDetails.icdoPayeeAccount.istrBenefitOptionValue == busConstant.JOINT_50_PERCENT_POPUP_ANNUITY ||
                                lbusOldPayeeDetails.icdoPayeeAccount.istrBenefitOptionValue == busConstant.JOINT_100_PERCENT_POPUP_ANNUITY) && lbusPlanBenXr.icdoPlanBenefitXr.benefit_option_value == busConstant.LIFE_ANNUTIY)
                            {
                                busPerson lbusSpouse = new busPerson();
                                lbusSpouse.FindPerson(this.icdoBenefitCalculationHeader.beneficiary_person_id);
                                if (lbusSpouse.icdoPerson.date_of_death != DateTime.MinValue)
                                {
                                    // PIR - 800
                                    if (lbusSpouse.icdoPerson.date_of_death.Day == 1)
                                    {
                                        adtAdjustFromDate = lbusSpouse.icdoPerson.date_of_death;
                                    }
                                    else
                                    {
                                        adtAdjustFromDate = lbusSpouse.icdoPerson.date_of_death.GetLastDayofMonth().AddDays(1);//If Convert To Benefit Option Executed.
                                    }

                                    //RID 115034
                                    int lintLifeConversionDateExists = (int)DBFunction.DBExecuteScalar("cdoPayeeAccount.CheckIfConvertToLifeDateExists", new object[1] { lintPayeeAccountID },
                                    iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                                    if (lintLifeConversionDateExists == 0)
                                    {
                                        DBFunction.DBExecuteScalar("cdoPayeeAccount.UpdatePopupToLifeConversionDate", new object[3] { adtAdjustFromDate, lbusPlanBenXr.icdoPlanBenefitXr.plan_id, lintPayeeAccountID }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                                    }
                                }
                            }
                        }

                    }

                    //CHECK QDRO here in CONTEXT OF EE to determine PAYEE's starting NON-TAXABLE Amount
                    if (this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID)
                    {
                        this.ibusCalculation.ProcessQDROOffset(this.iclbBenefitCalculationDetail.First(), this.icdoBenefitCalculationHeader.person_id, ref ldecNonTaxableBeginningBalance, false, false,
                            false, false, icdoBenefitCalculationHeader.calculation_type_value);
                        //Ticket#73070
                        if (ldecAccountOwnerStartingNonTaxableAmount > 0)
                            ldecNonTaxableBeginningBalance = ldecAccountOwnerStartingNonTaxableAmount - this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.idecAlt_payee_ee_contribution;


                        //Ticket#73070
                        if (lbusOldPayeeDetails.icdoPayeeAccount.payee_account_id > 0)//PIR 915
                            ldecNonTaxableBeginningBalance = lbusOldPayeeDetails.icdoPayeeAccount.nontaxable_beginning_balance - this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.idecAlt_payee_ee_contribution;

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
                                                                              0, 0, lintBenefitAccountID, busConstant.BENEFIT_TYPE_RETIREMENT, this.icdoBenefitCalculationHeader.istrRetirementType,
                                                                              ldteBenefitBeginDate, DateTime.MinValue, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER, lstrFamilyRelationshipValue,
                                                                              this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.minimum_guarantee_amount,
                                                                              ldecNonTaxableBeginningBalance, lintPlanBenefitId,//PIR 944
                                                                              ldteTermCertainEndDate, busConstant.FLAG_NO, busConstant.FLAG_NO, lblnAdjustmentPaymentFlag);
                    lbusPayeeAccount.LoadNextBenefitPaymentDate();
                    DateTime ldteNextBenefitPaymentDate = lbusPayeeAccount.idtNextBenefitPaymentDate;//R3vview this once with Vinovin





                    decimal ldecTaxableAmount = 0M;
                    decimal ldecNonTaxableAmount = 0M;
                    decimal ldecBenefitAmount = decimal.Zero;
                    decimal ldecPopupNonTaxableAmount = 0M; //RequestID: 72091

                    //PIR 894
                    if (this.iclbBenefitCalculationDetail.First().iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.pop_up_benefit_amount > decimal.Zero)
                    {
                        ldecPopUpBenefitAmount = this.iclbBenefitCalculationDetail.First().iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.pop_up_benefit_amount;
                        ldecPopupNonTaxableAmount = Math.Round(this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.popup_monthly_exclusion_amount,2, MidpointRounding.AwayFromZero);    //RequestID: 72091                    
                    }

                    if (this.iclbBenefitCalculationDetail.First().iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.istrBenefitOptionDescription.Contains(busConstant.LUMP_SUM_DESCRIPTION)
                        && this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID)
                    {
                        //10 Percent
                        ldecBenefitAmount = this.iclbBenefitCalculationDetail.First().iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.benefit_amount
                            - this.iclbBenefitCalculationDetail.First().iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.paid_amount;

                        if (this.iclbBenefitCalculationDetail.First().iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.overridden_benefit_amount > decimal.Zero)
                        {
                            ldecBenefitAmount = this.iclbBenefitCalculationDetail.First().iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.overridden_benefit_amount;
                        }
                        if (ldecBenefitAmount > 0)
                            busPayeeAccountHelper.CalculateMonthlyPaymentComponents(DateTime.MinValue, ldecBenefitAmount,
                                                                       ref ldecNonTaxableAmount, ref ldecTaxableAmount, ldecNonTaxableBeginningBalance);
                    }
                    else if (this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID)
                    {
                        decimal ldecIapPaid = decimal.Zero;
                        int lintPersonAccountId = 0;
                        decimal ldecIapAdjustedBalance = decimal.Zero;
                        decimal ldecWithHoldingAmount = decimal.Zero;
                        if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT)
                        {

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
                        ldecBenefitAmount = this.iclbBenefitCalculationDetail.First().iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.benefit_amount;
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
                    if (ldecNonTaxableAmount >= 0M)//RID 60954
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
                    else   //Bug Fix to fix the issue reported in RID 61391. System was creating payee account but was not inserting Review status when using convert to lumpsum option for MD.
                    {
                        lbusPayeeAccount.CreateReviewPayeeAccountStatus(ablnFromApprovedCalc: true);
                    }

                    lbusPayeeAccount.LoadPayeeAccountPaymentItemType();
                    //Ticket#88098
                    if (this.icdoBenefitCalculationHeader.retirement_date < ldteNextBenefitPaymentDate && (!this.iclbBenefitCalculationDetail.First().iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.istrBenefitOptionDescription.Contains(busConstant.LUMP_SUM_DESCRIPTION))
                               && this.iclbBenefitCalculationDetail.Where(i=>i.icdoBenefitCalculationDetail.plan_id != busConstant.IAP_PLAN_ID).Count()>0)
                    {
                        // PROD PIR 127
                        this.CreatePayeeAccountForRetireeIncrease(lbusPayeeAccount, lintBenefitAccountID, lstrFamilyRelationshipValue, ldecNonTaxableBeginningBalance, ldteNextBenefitPaymentDate, 0, busConstant.BENEFIT_TYPE_RETIREMENT, lbusPayeeAccount.icdoPayeeAccount.account_relation_value);
                    }

                    //Retro Calculation Items to be Created
                    //R3view Payment Logic                         
                    if (this.icdoBenefitCalculationHeader.retirement_date < ldteNextBenefitPaymentDate && (!this.iclbBenefitCalculationDetail.First().iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.istrBenefitOptionDescription.Contains(busConstant.LUMP_SUM_DESCRIPTION))
                        && this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.plan_id != busConstant.IAP_PLAN_ID &&
                        this.icdoBenefitCalculationHeader.calculation_type_value != busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT && !ablnMDReEvaluationBatch) //R3view LatestGreatest Payment Date from Payment Schedule 
                    {
                        #region Overpayment/Underpayment creation in case of MD to RD
                        //PROD PIR 171
                        DataTable ldtMDPayeeAccount = busBase.Select<cdoPayeeAccount>(
                               new string[2] { enmPayeeAccount.person_id.ToString(), enmPayeeAccount.retirement_type_value.ToString() },
                             new object[2] { lbusPayeeAccount.icdoPayeeAccount.person_id, busConstant.RETIREMENT_TYPE_MINIMUM_DISTRIBUTION }, null, null);

                        busPayeeAccount lbusMDPayeeAccount = null;
                        busPayeeAccountStatus lbusMDPayeeAccountStatus = null;
                        if (ldtMDPayeeAccount != null && ldtMDPayeeAccount.Rows.Count > 0)
                        {
                            lbusMDPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
                            lbusMDPayeeAccount.icdoPayeeAccount.LoadData(ldtMDPayeeAccount.Rows[0]);
                            lbusMDPayeeAccount.LoadNextBenefitPaymentDate();

                            DataTable ldtMDPayeeAccountStatus = busBase.Select<cdoPayeeAccountStatus>(
                            new string[1] { enmPayeeAccountStatus.payee_account_id.ToString() },
                             new object[1] { lbusMDPayeeAccount.icdoPayeeAccount.payee_account_id }, null, "status_effective_date desc");

                            if (ldtMDPayeeAccountStatus != null && ldtMDPayeeAccountStatus.Rows.Count > 0)
                            {
                                lbusMDPayeeAccountStatus = new busPayeeAccountStatus { icdoPayeeAccountStatus = new cdoPayeeAccountStatus() };
                                lbusMDPayeeAccountStatus.icdoPayeeAccountStatus.LoadData(ldtMDPayeeAccountStatus.Rows[0]);
                            }
                        }
                        if (lbusPayeeAccount.icdoPayeeAccount.retirement_type_value == busConstant.RETIREMENT_TYPE_LATE
                            && lbusMDPayeeAccount != null && lbusMDPayeeAccountStatus != null && lbusMDPayeeAccountStatus.icdoPayeeAccountStatus.status_value == busConstant.PAYEE_ACCOUNT_STATUS_COMPLETED)
                        {
                            ;
                        }
                        #endregion Overpayment/Underpayment creation in case of MD to RD
                        else
                        {
                            lbusPayeeAccount.CreateRetroPayments(lbusPayeeAccount, ldteNextBenefitPaymentDate, this.icdoBenefitCalculationHeader.retirement_date, lintPayeeAccountID, busConstant.RETRO_PAYMENT_INITIAL);
                        }
                    }

                    // DateTime ldtParticipantMDDate = new DateTime(this.ibusPerson.icdoPerson.date_of_birth.AddYears(70).AddMonths(6).Year + 1, 04, 01);
                    // RID# 153935 
                    DateTime ldtParticipantNormalMDDate = busGlobalFunctions.GetMinDistributionDate(this.ibusBenefitApplication.ibusPerson.icdoPerson.date_of_birth, this.ibusBenefitApplication.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date);
                    DateTime ldtParticipantMDDate = busGlobalFunctions.GetMinDistributionDate(this.ibusPerson.icdoPerson.person_id, this.ibusBenefitApplication.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date);


                    if (
                        (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT
                            || (this.icdoBenefitCalculationHeader.calculation_type_value != busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT
                                   && icdoBenefitCalculationHeader.retirement_date.Year > ldtParticipantMDDate.Year))
                        && (this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.benefit_subtype_value.IsNullOrEmpty()
                            ||
                            this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.benefit_subtype_value != busConstant.RETIREMENT_TYPE_MINIMUM_DISTRIBUTION)
                        )
                    {
                        bool lblnAdjustmentCalculationForRetireeIncrease = false;
                        if (iclbDisabilityRetireeIncrease != null && iclbDisabilityRetireeIncrease.Count > 0)
                            lblnAdjustmentCalculationForRetireeIncrease = true;

                        //PIR 985 10262015
                        if (this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                            this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.final_monthly_benefit_amount < 0)
                            ;
                        else
                            lbusPayeeAccount.CreateReviewPayeeAccountStatus(ablnFromApprovedCalc: true);//PIR 1055


                        //PIR 993  added check for Lumpsum benefit type for MPIPP plan.
                        if ((!this.iclbBenefitCalculationDetail.First().iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.istrBenefitOptionDescription.Contains(busConstant.LUMP_SUM_DESCRIPTION))
                            && (this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.plan_id != busConstant.IAP_PLAN_ID))
                        {
                            //Payment Adjustments - Benefit Adjustment Batch
                            Collection<busBenefitMonthwiseAdjustmentDetail> lclbBenefitMonthwiseAdjustmentDetail = new Collection<busBenefitMonthwiseAdjustmentDetail>();
                            lclbBenefitMonthwiseAdjustmentDetail = ibusCalculation.GetAmountActuallyPaid(lbusPayeeAccount, adtAdjustFromDate, lbusPayeeAccount.idtLastBenefitPaymentDate, lblnAdjustmentCalculationForRetireeIncrease);
                            DateTime ldtParticipantTurns65 = this.ibusPerson.icdoPerson.date_of_birth.AddYears(65);
                            //PIR 967

                            //for PIR-638
                            DateTime ldtAfter400HoursAfterRetirement = new DateTime();
                            DateTime ldtYearStartDateAfter400HoursAfterRetirement = new DateTime();
                            DateTime ldtAfterParticipantTurns65 = new DateTime();
                            bool lblnLastYearMonthReemployed_URED_Flag = false;
                            if (ldtParticipantTurns65.Day != 1)
                            {
                                ldtParticipantTurns65 = ldtParticipantTurns65.GetLastDayofMonth().AddDays(1);
                            }
                            //for PIR-638 
                            ldtAfterParticipantTurns65 = ldtParticipantTurns65;
                            Dictionary<int, List<int>> ldictYearMonthForWorkHrsAfterRetirement = new Dictionary<int, List<int>>();
                            List<int> ilistMonths = new List<int>();
                            //PIR 936
                            if (this.ibusPerson.iclbPersonSuspendibleMonth.IsNullOrEmpty())
                            {
                                this.ibusPerson.LoadPersonSuspendibleMonth();
                            }

                            //PIR 967
                            if ((!idictWorkHrsAfterRetirement.IsNullOrEmpty() || (this.ibusPerson.iclbPersonSuspendibleMonth != null && this.ibusPerson.iclbPersonSuspendibleMonth.Count() > 0)) && icdoBenefitCalculationHeader.retirement_date.Year < ldtParticipantMDDate.Year)
                            {
                                //SuspendibleHoursChange
                                busCalculation lbusCalculation = new busCalculation();
                                int lintMonth = 0;
                                int lintYear = 0;
                                decimal ldecTaxableAmtPaid = decimal.Zero;
                                ibusCalculation.FillYearlyDetailSetBenefitAmountForEachYear(this, this.iclbBenefitCalculationDetail.FirstOrDefault());
                                bool lblnGetAmountForReemployment = false;
                                lclbBenefitMonthwiseAdjustmentDetail = lclbBenefitMonthwiseAdjustmentDetail.OrderBy(i => i.icdoBenefitMonthwiseAdjustmentDetail.payment_date).ToList().ToCollection();//PIR-638 (RASHMI)
                                foreach (busBenefitMonthwiseAdjustmentDetail lbusBenefitMonthwiseAdjustmentDetail in lclbBenefitMonthwiseAdjustmentDetail)
                                {
                                    lblnGetAmountForReemployment = false;
                                    ldecTaxableAmtPaid = decimal.Zero;
                                    lintMonth = lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date.Month;
                                    lintYear = lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date.Year;

                                    //PIR-638 (to get the full suspension period(starting from when participant completes 400th hrs after retirement till participant turns age 65)) - RASHMI
                                    decimal ldec400Hours = 0.0m;
                                    int lintMonthAfter400Hours = 0;
                                    if (idictWorkHrsAfterRetirement.Keys.Contains(lintYear) || ibusCalculation.CheckIfMonthIsSuspendibleMonthFromNonSignatory(this.ibusPerson, lintMonth, lintYear))
                                    {
                                        //PIR:936
                                        if (idictWorkHrsAfterRetirement.Keys.Contains(lintYear) && idictWorkHrsAfterRetirement[lintYear].Sum(item => item.Value) >= 400 && lbusPayeeAccount.icdoPayeeAccount.retirement_type_value == busConstant.RETIREMENT_TYPE_UNREDUCED_EARLY)
                                        {
                                            if (ldtAfter400HoursAfterRetirement == DateTime.MinValue)
                                            {
                                                foreach (KeyValuePair<int, decimal> lkvpHoursInfo in idictWorkHrsAfterRetirement[lintYear].OrderBy(key => key.Key))
                                                {
                                                    ldec400Hours += lkvpHoursInfo.Value;
                                                    if (ldec400Hours >= 400)
                                                    {
                                                        lintMonthAfter400Hours = lkvpHoursInfo.Key;
                                                        break;
                                                    }
                                                }
                                                ldtAfter400HoursAfterRetirement = new DateTime(lintYear, lintMonthAfter400Hours, 01);
                                                ldtAfter400HoursAfterRetirement = ldtAfter400HoursAfterRetirement.AddMonths(1);
                                            }
                                            if (ldtYearStartDateAfter400HoursAfterRetirement.Year != lintYear)
                                            {
                                                ldtYearStartDateAfter400HoursAfterRetirement = DateTime.MinValue;
                                                ilistMonths = null;
                                            }
                                            if (ldtYearStartDateAfter400HoursAfterRetirement == DateTime.MinValue)
                                            {
                                                ldtYearStartDateAfter400HoursAfterRetirement = new DateTime(lintYear, lintMonth, 01);
                                            }
                                            if (ldtYearStartDateAfter400HoursAfterRetirement.Year == lintYear)
                                            {
                                                if (lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date >= ldtAfter400HoursAfterRetirement
                                                    && lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date < ldtAfterParticipantTurns65)
                                                {
                                                    if (ilistMonths == null)
                                                        ilistMonths = new List<int>();
                                                    ilistMonths.Add(lintMonth);
                                                    if (lintMonth == 12)
                                                    {
                                                        ldictYearMonthForWorkHrsAfterRetirement.Add(lintYear, ilistMonths);
                                                    }
                                                }
                                                else if (ldtAfterParticipantTurns65.Year == lintYear && lblnLastYearMonthReemployed_URED_Flag == busConstant.BOOL_FALSE)
                                                {
                                                    lblnLastYearMonthReemployed_URED_Flag = true;
                                                    ldictYearMonthForWorkHrsAfterRetirement.Add(lintYear, ilistMonths);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            ldtYearStartDateAfter400HoursAfterRetirement = DateTime.MinValue;
                                            ilistMonths = null;
                                        }
                                    }

                                    //SuspendibleHoursChange
                                    if ((idictWorkHrsAfterRetirement.Keys.Contains(lintYear) && idictWorkHrsAfterRetirement[lintYear][lintMonth] >= lbusCalculation.GetSuspendibleHoursValue(lintYear, lintMonth) /*40*/) || ibusCalculation.CheckIfMonthIsSuspendibleMonthFromNonSignatory(this.ibusPerson, lintMonth, lintYear))
                                    {
                                        if (idictWorkHrsAfterRetirement.Keys.Contains(lintYear) && idictWorkHrsAfterRetirement[lintYear][lintMonth] >= lbusCalculation.GetSuspendibleHoursValue(lintYear, lintMonth) /*40*/)
                                        {
                                            lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.hours = idictWorkHrsAfterRetirement[lintYear][lintMonth];
                                        }
                                        lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.suspended_flag = busConstant.FLAG_YES;


                                        //PIR-638 
                                        if (ldtAfter400HoursAfterRetirement != DateTime.MinValue && //ldtYearStartDateAfter400HoursAfterRetirement != DateTime.MinValue && -- RequestID: 72091
                                             lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date >= ldtAfter400HoursAfterRetirement &&
                                             lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date < ldtAfterParticipantTurns65)
                                        {
                                            lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.non_taxable_amount_to_be_paid = 0M;
                                            lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.taxable_amount_to_be_paid = 0M;
                                        }
                                        else if (this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.ee_derived_benefit_amount > 0 &&
                                                 lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date < ldtParticipantMDDate)
                                        {
                                            //RequestID: 72091
                                            if (icdoBenefitCalculationHeader.iblnPopUpToLife && lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date < icdoBenefitCalculationHeader.idtJointAnnuitantDOD)
                                            {
                                                lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.non_taxable_amount_to_be_paid = ldecPopupNonTaxableAmount;
                                                lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.taxable_amount_to_be_paid = (this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.ee_derived_benefit_amount * this.iclbBenefitCalculationDetail.First().iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.pop_up_option_factor_at_ret) - ldecPopupNonTaxableAmount;
                                            }
                                            else
                                            {
                                                lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.non_taxable_amount_to_be_paid = ldecNonTaxableAmount;
                                                lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.taxable_amount_to_be_paid = this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.ee_derived_benefit_amount - ldecNonTaxableAmount;
                                            }
                                        } //PIR - 930
                                        else if (lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date >= ldtParticipantMDDate)
                                        {
                                            if (lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date >= ldtParticipantTurns65)
                                            {
                                                lblnGetAmountForReemployment = true;
                                            }

                                            //RequestID: 72091
                                            if (icdoBenefitCalculationHeader.iblnPopUpToLife && lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date < icdoBenefitCalculationHeader.idtJointAnnuitantDOD)
                                                lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.non_taxable_amount_to_be_paid = ldecPopupNonTaxableAmount;
                                            else
                                                lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.non_taxable_amount_to_be_paid = ldecNonTaxableAmount;

                                            //PIR 894
                                            if (icdoBenefitCalculationHeader.iblnPopUpToLife)
                                                ldecTaxableAmtPaid = ibusCalculation.GetBenefitAmountOfParticipantFromYearlyDetail(this, this.iclbBenefitCalculationDetail.First(), lintYear, ablnGetAmountForReemployment: lblnGetAmountForReemployment,ldtPaymentDate: lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date);
                                            else
                                                ldecTaxableAmtPaid = ibusCalculation.GetBenefitAmountOfParticipantFromYearlyDetail(this, this.iclbBenefitCalculationDetail.First(), lintYear, ablnGetAmountForReemployment: lblnGetAmountForReemployment);

                                            if (ldecTaxableAmount > 0)
                                            {
                                                //RequestID: 72091
                                                if (icdoBenefitCalculationHeader.iblnPopUpToLife && lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date < icdoBenefitCalculationHeader.idtJointAnnuitantDOD)
                                                    lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.taxable_amount_to_be_paid = ldecTaxableAmtPaid - ldecPopupNonTaxableAmount;
                                                else
                                                    lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.taxable_amount_to_be_paid = ldecTaxableAmtPaid - ldecNonTaxableAmount;
                                            }
                                        }

                                    }
                                    else
                                    {
                                        if (this.icdoBenefitCalculationHeader.iintPlanId == busConstant.MPIPP_PLAN_ID)
                                        {
                                            if (!this.iclbBenefitCalculationDetail.First().iclbBenefitCalculationYearlyDetail.IsNullOrEmpty())
                                            {

                                                if (lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date >= ldtParticipantTurns65)
                                                {
                                                    lblnGetAmountForReemployment = true;
                                                }

                                                //PIR 638
                                                if (ldtAfter400HoursAfterRetirement != DateTime.MinValue //&& ldtYearStartDateAfter400HoursAfterRetirement != DateTime.MinValue --RequestID: 72091
                                                    && lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date >= ldtAfter400HoursAfterRetirement
                                                    && lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date < ldtAfterParticipantTurns65)
                                                { }
                                                else
                                                {
                                                    //RequestID: 72091
                                                    if (icdoBenefitCalculationHeader.iblnPopUpToLife && lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date < icdoBenefitCalculationHeader.idtJointAnnuitantDOD)
                                                        lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.non_taxable_amount_to_be_paid = ldecPopupNonTaxableAmount;
                                                    else
                                                        lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.non_taxable_amount_to_be_paid = ldecNonTaxableAmount;

                                                    //PIR 894
                                                    if (icdoBenefitCalculationHeader.iblnPopUpToLife)
                                                        ldecTaxableAmtPaid = ibusCalculation.GetBenefitAmountOfParticipantFromYearlyDetail(this, this.iclbBenefitCalculationDetail.First(), lintYear, ablnGetAmountForReemployment: lblnGetAmountForReemployment, ldtPaymentDate: lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date);
                                                    else
                                                        ldecTaxableAmtPaid = ibusCalculation.GetBenefitAmountOfParticipantFromYearlyDetail(this, this.iclbBenefitCalculationDetail.First(), lintYear, ablnGetAmountForReemployment: lblnGetAmountForReemployment);

                                                    //PIR 894
                                                    //if (icdoBenefitCalculationHeader.iblnPopUpToLife
                                                    //    && lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date < icdoBenefitCalculationHeader.idtJointAnnuitantDOD)
                                                    //{
                                                    //    ldecTaxableAmtPaid = ldecPopUpBenefitAmount;
                                                    //}

                                                    if (ldecTaxableAmtPaid > 0)
                                                    {
                                                        //RequestID: 72091
                                                        if (icdoBenefitCalculationHeader.iblnPopUpToLife && lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date < icdoBenefitCalculationHeader.idtJointAnnuitantDOD)
                                                            lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.taxable_amount_to_be_paid = ldecTaxableAmtPaid - ldecPopupNonTaxableAmount;
                                                        else
                                                            lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.taxable_amount_to_be_paid = ldecTaxableAmtPaid - ldecNonTaxableAmount;
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.taxable_amount_to_be_paid = ldecTaxableAmount;

                                            //PIR 894
                                            if (icdoBenefitCalculationHeader.iblnPopUpToLife
                                                && lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date < icdoBenefitCalculationHeader.idtJointAnnuitantDOD)
                                            {
                                                lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.taxable_amount_to_be_paid = ldecPopUpBenefitAmount - ldecPopupNonTaxableAmount; //RequestID: 72091
                                            }
                                        }
                                    }
                                }
                            }
                            else if (icdoBenefitCalculationHeader.retirement_date.Year > ldtParticipantMDDate.Year)
                            {
                                Collection<busBenefitMonthwiseAdjustmentDetail> lclbMDMonthwiseAdjustmentDetail = null;

                                //Ticket - 72115
                                DataTable ldtblMDPayeeAccount = Select("cdoPayeeAccount.GetMinimumDistributionPayeeAccount", new object[2] { icdoBenefitCalculationHeader.person_id, this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.plan_id });
                                if (ldtblMDPayeeAccount != null && ldtblMDPayeeAccount.Rows.Count > 0)
                                {
                                    lclbMDMonthwiseAdjustmentDetail = new Collection<busBenefitMonthwiseAdjustmentDetail>();
                                    busPayeeAccount lbusMDPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
                                    lbusMDPayeeAccount.icdoPayeeAccount.LoadData(ldtblMDPayeeAccount.Rows[0]);
                                    lbusMDPayeeAccount.LoadNextBenefitPaymentDate();
                                    lclbMDMonthwiseAdjustmentDetail = ibusCalculation.GetAmountActuallyPaid(lbusMDPayeeAccount, lbusMDPayeeAccount.icdoPayeeAccount.benefit_begin_date, lbusMDPayeeAccount.idtLastBenefitPaymentDate);

                                    if (lclbMDMonthwiseAdjustmentDetail != null && lclbMDMonthwiseAdjustmentDetail.Count > 0)
                                    {
                                        lclbBenefitMonthwiseAdjustmentDetail = ibusCalculation.GetAmountActuallyPaid(lbusPayeeAccount, lbusPayeeAccount.icdoPayeeAccount.benefit_begin_date, lbusPayeeAccount.idtLastBenefitPaymentDate, lblnAdjustmentCalculationForRetireeIncrease);

                                        foreach(busBenefitMonthwiseAdjustmentDetail lbusMDMonthwiseAdjustmentDetail in lclbMDMonthwiseAdjustmentDetail)
                                        {
                                            if(lclbBenefitMonthwiseAdjustmentDetail.Where(t=>t.icdoBenefitMonthwiseAdjustmentDetail.payment_date.Date == lbusMDMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date.Date).Count() > 0)
                                            {
                                                lclbBenefitMonthwiseAdjustmentDetail.Where(t => t.icdoBenefitMonthwiseAdjustmentDetail.payment_date.Date == lbusMDMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date.Date)
                                                    .FirstOrDefault().icdoBenefitMonthwiseAdjustmentDetail.taxable_amount_paid +=
                                                        lbusMDMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.taxable_amount_paid;

                                                lclbBenefitMonthwiseAdjustmentDetail.Where(t => t.icdoBenefitMonthwiseAdjustmentDetail.payment_date.Date == lbusMDMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date.Date)
                                                    .FirstOrDefault().icdoBenefitMonthwiseAdjustmentDetail.non_taxable_amount_paid +=
                                                    lbusMDMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.non_taxable_amount_paid;
                                            }
                                            else
                                            {
                                                lclbBenefitMonthwiseAdjustmentDetail.Add(lbusMDMonthwiseAdjustmentDetail);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        lclbBenefitMonthwiseAdjustmentDetail = ibusCalculation.GetAmountActuallyPaid(lbusPayeeAccount, ldtParticipantMDDate, lbusPayeeAccount.idtLastBenefitPaymentDate, lblnAdjustmentCalculationForRetireeIncrease);
                                    }

                                    lclbBenefitMonthwiseAdjustmentDetail = lclbBenefitMonthwiseAdjustmentDetail.OrderBy(t => t.icdoBenefitMonthwiseAdjustmentDetail.payment_date).ToList().ToCollection();
                                }
                                else
                                {
                                    lclbBenefitMonthwiseAdjustmentDetail = ibusCalculation.GetAmountActuallyPaid(lbusPayeeAccount, ldtParticipantMDDate, lbusPayeeAccount.idtLastBenefitPaymentDate, lblnAdjustmentCalculationForRetireeIncrease);
                                }

                                ibusCalculation.FillYearlyDetailSetBenefitAmountForEachYear(this, this.iclbBenefitCalculationDetail.FirstOrDefault());
                                DateTime ldtRetirementDate = icdoBenefitCalculationHeader.retirement_date;

                                foreach (busBenefitMonthwiseAdjustmentDetail lbusBenefitMonthwiseAdjustmentDetail in lclbBenefitMonthwiseAdjustmentDetail)
                                {
                                    //Special scenario, for 10% RMD increase for year 2017. For all the New MDs and MD reevaluation calculation , 10% RMD increase for 2017 should be paid from 01/01/2018 payment and 
                                    //not before that. Even for 04/01/2017 MD , the 2017 increase should be paid from 01/01/2018 and not 04/01/2017.
                                    //To handle the scenaio of new MD in 2017 ,business has requested to show same amount in amount paid and amount should have been paid columns for year 2017 
                                    if (ldtParticipantMDDate != DateTime.MinValue && ldtParticipantMDDate.Year == busConstant.BenefitCalculation.RMD_INCREASE_YEAR_SPECIAL_SCENARIO
                                        && lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date.Year == busConstant.BenefitCalculation.RMD_INCREASE_YEAR_SPECIAL_SCENARIO)
                                    {
                                        lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.non_taxable_amount_to_be_paid = lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.non_taxable_amount_paid;
                                        lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.taxable_amount_to_be_paid = lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.taxable_amount_paid;
                                    }
                                    else
                                    {
                                        if (icdoBenefitCalculationHeader.iblnPopUpToLife && lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date < icdoBenefitCalculationHeader.idtJointAnnuitantDOD)
                                            lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.non_taxable_amount_to_be_paid = ldecPopupNonTaxableAmount;
                                        else
                                            lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.non_taxable_amount_to_be_paid = ldecNonTaxableAmount;

                                        if (lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date < ldtRetirementDate)
                                        {
                                            icdoBenefitCalculationHeader.retirement_date = ldtParticipantMDDate;
                                            //RequestID: 72091
                                            if (icdoBenefitCalculationHeader.iblnPopUpToLife && lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date < icdoBenefitCalculationHeader.idtJointAnnuitantDOD)
                                                lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.taxable_amount_to_be_paid = ibusCalculation.GetBenefitAmountOfParticipantFromYearlyDetail(this, this.iclbBenefitCalculationDetail.FirstOrDefault(), lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date.Year, ablnMDReEvaluation: true, ldtPaymentDate: lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date) - ldecPopupNonTaxableAmount;
                                            else if (icdoBenefitCalculationHeader.iblnPopUpToLife && lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date > icdoBenefitCalculationHeader.idtJointAnnuitantDOD)
                                                lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.taxable_amount_to_be_paid = ibusCalculation.GetBenefitAmountOfParticipantFromYearlyDetail(this, this.iclbBenefitCalculationDetail.FirstOrDefault(), lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date.Year, ablnMDReEvaluation: true, ldtPaymentDate: lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date) - ldecNonTaxableAmount;
                                            else
                                                lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.taxable_amount_to_be_paid = ibusCalculation.GetBenefitAmountOfParticipantFromYearlyDetail(this, this.iclbBenefitCalculationDetail.FirstOrDefault(), lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date.Year, ablnMDReEvaluation: true) - ldecNonTaxableAmount;
                                        }
                                        else
                                        {
                                            icdoBenefitCalculationHeader.retirement_date = ldtRetirementDate;
                                            //RequestID: 72091
                                            if (icdoBenefitCalculationHeader.iblnPopUpToLife && lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date < icdoBenefitCalculationHeader.idtJointAnnuitantDOD)
                                                lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.taxable_amount_to_be_paid = ibusCalculation.GetBenefitAmountOfParticipantFromYearlyDetail(this, this.iclbBenefitCalculationDetail.First(), lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date.Year, ablnGetAmountForReemployment: true, ablnReemployedAfterMD: true, ldtPaymentDate: lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date) - ldecPopupNonTaxableAmount;
                                            else if (icdoBenefitCalculationHeader.iblnPopUpToLife && lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date > icdoBenefitCalculationHeader.idtJointAnnuitantDOD)
                                                lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.taxable_amount_to_be_paid = ibusCalculation.GetBenefitAmountOfParticipantFromYearlyDetail(this, this.iclbBenefitCalculationDetail.First(), lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date.Year, ablnGetAmountForReemployment: true, ablnReemployedAfterMD: true, ldtPaymentDate: lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date) - ldecNonTaxableAmount;
                                            else
                                                lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.taxable_amount_to_be_paid = ibusCalculation.GetBenefitAmountOfParticipantFromYearlyDetail(this, this.iclbBenefitCalculationDetail.First(), lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date.Year, ablnGetAmountForReemployment: true, ablnReemployedAfterMD: true) - ldecNonTaxableAmount;
                                        }
                                    }
                                }

                                icdoBenefitCalculationHeader.retirement_date = ldtRetirementDate;

                            }
                            else
                            {
                                ibusCalculation.CalculateAmountShouldHaveBeenPaid(lbusPayeeAccount, ref lclbBenefitMonthwiseAdjustmentDetail);

                                //PIR 894
                                foreach (busBenefitMonthwiseAdjustmentDetail lbusBenefitMonthwiseAdjustmentDetail in lclbBenefitMonthwiseAdjustmentDetail)
                                {
                                    if(icdoBenefitCalculationHeader.iblnPopUpToLife
                                        && lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date < icdoBenefitCalculationHeader.idtJointAnnuitantDOD)
                                    {
                                        lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.taxable_amount_to_be_paid = ldecPopUpBenefitAmount - ldecPopupNonTaxableAmount;
                                        lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.non_taxable_amount_to_be_paid = ldecPopupNonTaxableAmount; //RequestID: 72091
                                    }
                                }
                            }

                            ibusCalculation.CalculateRetireeIncreaseAmountShouldHaveBeenPaid(lbusPayeeAccount, iclbDisabilityRetireeIncrease, ref lclbBenefitMonthwiseAdjustmentDetail, ldictYearMonthForWorkHrsAfterRetirement); // PROD PIR 581, 638
                            ibusCalculation.CreateOverpaymentUnderPayment(lbusPayeeAccount, lclbBenefitMonthwiseAdjustmentDetail, busConstant.RETRO_PAYMENT_ADJUSTMENT_BATCH, 0.0m, 0.0m, null);
                        }
                        //132650
                        lbusPayeeAccount.icdoPayeeAccount.verified_flag = busConstant.FLAG_NO;
                        lbusPayeeAccount.icdoPayeeAccount.verified_date = DateTime.MinValue;
                        lbusPayeeAccount.icdoPayeeAccount.verified_by = null;
                        // PROD PIR 581
                        lbusPayeeAccount.icdoPayeeAccount.reemployed_flag = busConstant.FLAG_NO;
                        lbusPayeeAccount.icdoPayeeAccount.reemployed_flag_as_of_date = DateTime.MinValue;
                        lbusPayeeAccount.icdoPayeeAccount.Update();
                    }
                    else if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT
                        && this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.benefit_subtype_value == busConstant.RETIREMENT_TYPE_MINIMUM_DISTRIBUTION
                        && ((!this.iclbBenefitCalculationDetail.First().iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.istrBenefitOptionDescription.Contains(busConstant.LUMP_SUM_DESCRIPTION))
                            && (this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.plan_id != busConstant.IAP_PLAN_ID)))
                    {
                        Collection<busBenefitMonthwiseAdjustmentDetail> lclbBenefitMonthwiseAdjustmentDetail = new Collection<busBenefitMonthwiseAdjustmentDetail>();
                        lclbBenefitMonthwiseAdjustmentDetail = ibusCalculation.GetAmountActuallyPaid(lbusPayeeAccount, lbusPayeeAccount.icdoPayeeAccount.benefit_begin_date, lbusPayeeAccount.idtLastBenefitPaymentDate);

                        foreach (busBenefitMonthwiseAdjustmentDetail lbusBenefitMonthwiseAdjustmentDetail in lclbBenefitMonthwiseAdjustmentDetail)
                        {

                            //Special scenario, for 10% RMD increase for year 2017. For all the New MDs and MD reevaluation calculation , 10% RMD increase for 2017 should be paid from 01/01/2018 payment and 
                            //not before that. Even for 04/01/2017 MD , the 2017 increase should be paid from 01/01/2018 and not 04/01/2017.
                            //To handle the scenaio of new MD in 2017 ,business has requested to show same amount in amount paid and amount should have been paid columns for year 2017 
                            if (ldtParticipantMDDate != DateTime.MinValue && ldtParticipantMDDate.Year == busConstant.BenefitCalculation.RMD_INCREASE_YEAR_SPECIAL_SCENARIO
                                && lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date.Year == busConstant.BenefitCalculation.RMD_INCREASE_YEAR_SPECIAL_SCENARIO)
                            {
                                lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.non_taxable_amount_to_be_paid = lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.non_taxable_amount_paid;
                                lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.taxable_amount_to_be_paid = lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.taxable_amount_paid;
                            }
                            else
                            {
                                //RequestID: 72091
                                if(this.icdoBenefitCalculationHeader.iblnPopUpToLife && lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date < icdoBenefitCalculationHeader.idtJointAnnuitantDOD)
                                {
                                    lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.non_taxable_amount_to_be_paid = ldecPopupNonTaxableAmount;
                                    lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.taxable_amount_to_be_paid = ibusCalculation.GetBenefitAmountOfParticipantFromYearlyDetail(this, this.iclbBenefitCalculationDetail.FirstOrDefault(), lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date.Year, ablnMDReEvaluation: true, ldtPaymentDate: lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date) - ldecPopupNonTaxableAmount;
                                }
                                else
                                {
                                    lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.non_taxable_amount_to_be_paid = ldecNonTaxableAmount;
                                    lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.taxable_amount_to_be_paid = ibusCalculation.GetBenefitAmountOfParticipantFromYearlyDetail(this, this.iclbBenefitCalculationDetail.FirstOrDefault(), lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date.Year, ablnMDReEvaluation: true, ldtPaymentDate: lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date) - ldecNonTaxableAmount;
                                }
                            }
                        }

                        lclbBenefitMonthwiseAdjustmentDetail = lclbBenefitMonthwiseAdjustmentDetail.OrderBy(t => t.icdoBenefitMonthwiseAdjustmentDetail.payment_date).ToList().ToCollection();//PIR 894
                        ibusCalculation.CreateOverpaymentUnderPayment(lbusPayeeAccount, lclbBenefitMonthwiseAdjustmentDetail, busConstant.RETRO_PAYMENT_ADJUSTMENT_BATCH);
                        lbusPayeeAccount.CreateReviewPayeeAccountStatus(ablnFromApprovedCalc: true);//PIR 1055
                    }

                    #region ReCalculate Post Retirement DRO
                    if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT)
                    {
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
                    }
                    #endregion

                    #region TO POST PARTIAL INTEREST AND QUATERLY IAP ALLOCATIONS IN THE RETIREMENT CONTRIBUTION TABLE

                    if (icdoBenefitCalculationHeader.calculation_type_value != busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT)
                    {
                        bool lblnInsertEEPartialInterest = busConstant.BOOL_FALSE;
                        foreach (busBenefitCalculationDetail lbusBenefitCalculationDetail in this.iclbBenefitCalculationDetail)
                        {
                            if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID)
                            {
                                int lintPersonAccountId = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().icdoPersonAccount.person_account_id;

                                if (this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().icdoPersonAccount.idecVestedEE > 0M)
                                {
                                    // Insert the Partial Interest for EE Contributions in the SGT_PERSON_RETIREMENT_CONTRIBUTION table
                                    if (!lblnInsertEEPartialInterest)
                                    {
                                        decimal ldecPriorYearInterest = decimal.Zero;
                                        decimal ldecPartialEEInterestAmount = ibusCalculation.CalculatePartialEEInterest(this.icdoBenefitCalculationHeader.retirement_date,
                                                                        this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault(),
                                                                        true, false, iclbPersonAccountRetirementContribution, out ldecPriorYearInterest);
                                        if (ldecPartialEEInterestAmount - ldecPriorYearInterest > decimal.Zero)
                                        {
                                            busPersonAccountRetirementContribution lbusPersonAccountRetirementContribution = new busPersonAccountRetirementContribution() { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };
                                            lbusPersonAccountRetirementContribution.InsertPersonAccountRetirementContirbution(lintPersonAccountId, busGlobalFunctions.GetLastDateOfComputationYear(this.icdoBenefitCalculationHeader.retirement_date.Year - 1).AddDays(1),
                                                DateTime.Now, this.icdoBenefitCalculationHeader.retirement_date.Year, adecEEInterestAmount: ldecPartialEEInterestAmount - ldecPriorYearInterest, astrTransactionType: busConstant.TRANSACTION_TYPE_INTEREST,
                                                astrContributionType: busConstant.CONTRIBUTION_TYPE_EE, astrContributionSubtype: busConstant.CONTRIBUTION_SUBTYPE_VESTED, aintReferenceID: this.icdoBenefitCalculationHeader.benefit_calculation_header_id);
                                            lblnInsertEEPartialInterest = busConstant.BOOL_TRUE;
                                        }
                                        if (ldecPriorYearInterest > decimal.Zero)
                                        {
                                            busPersonAccountRetirementContribution lbusPersonAccountRetirementContribution = new busPersonAccountRetirementContribution() { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };
                                            lbusPersonAccountRetirementContribution.InsertPersonAccountRetirementContirbution(lintPersonAccountId, busGlobalFunctions.GetLastDateOfComputationYear(this.icdoBenefitCalculationHeader.retirement_date.Year - 1).AddDays(1),
                                                DateTime.Now, this.icdoBenefitCalculationHeader.retirement_date.AddYears(-1).Year, adecEEInterestAmount: ldecPriorYearInterest, astrTransactionType: busConstant.TRANSACTION_TYPE_INTEREST,
                                                astrContributionType: busConstant.CONTRIBUTION_TYPE_EE, astrContributionSubtype: busConstant.CONTRIBUTION_SUBTYPE_VESTED, aintReferenceID: this.icdoBenefitCalculationHeader.benefit_calculation_header_id);
                                            lblnInsertEEPartialInterest = busConstant.BOOL_TRUE;

                                        }
                                    }
                                }
                            }

                        }
                    }

                    // Insert the IAP Quarterly Allocations in the SGT_PERSON_RETIREMENT_CONTRIBUTION table
                    if (this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).Count() > 0)
                    {
                        bool lblnInsert = true;
                        //PIR 534: OPUS should not be push quarterly allocations and RY allocations when approving Adjustment Calc.
                        //if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT && !lblnDeductPrevIAPBalance)
                        if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT)
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

                    if (this.ibusBaseActivityInstance.IsNotNull())
                    {
                        this.SetWFVariables4PayeeAccount(lintPayeeAccountID, this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.plan_id);
                        this.SetProcessInstanceParameters();
                    }
                    iobjPassInfo.idictParams["aintPayeeAccountId"] = lintPayeeAccountID;
                    utlPassInfo lobjMainPassInfo = iobjPassInfo;
                }
            }
            #endregion

        }

        public void InsertIAPQuarterlyAllocations(Collection<busBenefitCalculationDetail> aclbBenefitCalculationDetail)
        {
            decimal ldecIAPHours4QtrAlloc = 0.0M;
            decimal ldecIAPHoursA2forQtrAlloc = 0.0M;
            decimal ldecIAPPercent4forQtrAlloc = 0.0M;


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

            this.ibusCalculation.GetIAPAndSpecialAccountBalance(busConstant.BOOL_FALSE, null, aclbBenefitCalculationDetail, this, null, this.icdoBenefitCalculationHeader.retirement_date, ldecIAPHours4QtrAlloc, ldecIAPHoursA2forQtrAlloc, ldecIAPPercent4forQtrAlloc);

            int lintPersonAccountId = 0;
            lintPersonAccountId = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().icdoPersonAccount.person_account_id;

            if (this.ibusCalculation.idecQuarterlyAllocationIAP != 0 || this.ibusCalculation.idecQuarterlyAllocationL52Spl != 0 ||
                this.ibusCalculation.idecQuarterlyAllocationL161Spl != 0) //PIR 985
            {
                //PIR RID 66354 added check to verify quarterly allocation is not already posted
                if (!this.ibusCalculation.CheckQuaterlyAlreadyPosted(lintPersonAccountId, icdoBenefitCalculationHeader.retirement_date, this.ibusCalculation.idecQuarterlyAllocationIAP, this.ibusCalculation.idecQuarterlyAllocationL52Spl, this.ibusCalculation.idecQuarterlyAllocationL161Spl))
                {

                    busPersonAccountRetirementContribution lbusPersonAccountRetirementContribution = new busPersonAccountRetirementContribution() { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };
                    lbusPersonAccountRetirementContribution.InsertPersonAccountRetirementContirbution(lintPersonAccountId, icdoBenefitCalculationHeader.retirement_date,//busGlobalFunctions.GetLastDateOfComputationYear(this.icdoBenefitCalculationHeader.retirement_date.Year - 1).AddDays(1),
                        DateTime.Now, this.icdoBenefitCalculationHeader.retirement_date.Year, adecIAPBalanceAmount: this.ibusCalculation.idecQuarterlyAllocationIAP, adec52SplAccountBalance: this.ibusCalculation.idecQuarterlyAllocationL52Spl,
                        adec161SplAccountBalance: this.ibusCalculation.idecQuarterlyAllocationL161Spl, astrTransactionType: busConstant.TRANSACTION_TYPE_QUARTERLY_ALLOCATION, astrContributionType: busConstant.RCContributionTypeAllocation1,
                        aintReferenceID: this.icdoBenefitCalculationHeader.benefit_calculation_header_id);
                }
            }

            if (this.ibusCalculation.idecRYAlloc2 != 0) //PIR 985
            {
                busPersonAccountRetirementContribution lbusPersonAccountRetirementContribution = new busPersonAccountRetirementContribution() { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };
                lbusPersonAccountRetirementContribution.InsertPersonAccountRetirementContirbution(lintPersonAccountId, icdoBenefitCalculationHeader.retirement_date,//busGlobalFunctions.GetLastDateOfComputationYear(this.icdoBenefitCalculationHeader.retirement_date.Year - 1).AddDays(1),
                    DateTime.Now, this.icdoBenefitCalculationHeader.retirement_date.Year, adecIAPBalanceAmount: this.ibusCalculation.idecRYAlloc2,
                     astrTransactionType: "RETR", astrContributionType: busConstant.RCContributionTypeAllocation2, aintReferenceID: this.icdoBenefitCalculationHeader.benefit_calculation_header_id);
            }

            if (this.ibusCalculation.idecRYAlloc4 != 0)//PIR 985
            {
                busPersonAccountRetirementContribution lbusPersonAccountRetirementContribution = new busPersonAccountRetirementContribution() { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };
                lbusPersonAccountRetirementContribution.InsertPersonAccountRetirementContirbution(lintPersonAccountId, icdoBenefitCalculationHeader.retirement_date,//busGlobalFunctions.GetLastDateOfComputationYear(this.icdoBenefitCalculationHeader.retirement_date.Year - 1).AddDays(1),
                    DateTime.Now, this.icdoBenefitCalculationHeader.retirement_date.Year, adecIAPBalanceAmount: this.ibusCalculation.idecRYAlloc4,
                     astrTransactionType: "RETR", astrContributionType: busConstant.RCContributionTypeAllocation4, aintReferenceID: this.icdoBenefitCalculationHeader.benefit_calculation_header_id);
            }

        }

        public void RevertMPIContributions()
        {
            decimal ldecPartialEEInterestAmount = 0.0M;
            decimal ldecPartialUVHPInterestAmount = 0.0M;
            bool lblnInsertEEPartialInterest = busConstant.BOOL_FALSE;

            if (this.iclbPersonAccountRetirementContribution == null)
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

            if (this.ibusBenefitApplication == null)
            {

            }

            foreach (busBenefitCalculationDetail lbusBenefitCalculationDetail in this.iclbBenefitCalculationDetail)
            {
                if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID)
                {
                    if (this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.reference_id
                                         == this.icdoBenefitCalculationHeader.benefit_calculation_header_id).Count() > 0)
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
            }
        }

        public ArrayList btn_CancelCalculation()
        {
            base.btn_CancelCalculation();

            if (this.iarrErrors.IsNull())
                this.iarrErrors = new ArrayList();

            if (this.iarrErrors.Count == 0) // PROD PIR 792
            {
                if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL)
                {
                    #region MPI
                    RevertMPIContributions();
                    #endregion

                    #region IAP
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

        /// <summary>
        /// Calculate MD Annual Adjustment
        /// </summary>
        /// <param name="abusPayeeAccount"></param>
        /// <param name="adtDateOfBirth"></param>
        /// 
        #region Commented Code of Mahua - In_Construction
        //public void CalculateMDBenefitForReEvaluationBatch(DataRow adrPersonInfo)
        //{
        //    decimal ldecBenefitAsOfDeteminationDate = 0, ldecActiveRetireeInc = 0, ldecNormalRetAge = 0, 
        //            ldecCumBenefitAmount = 0, ldecYTDAccruedBenefit = 0, ldecERCurrentYear = 0, ldecGAM71Factor = 0, 
        //            ldecBenefitOptionFactor = 0, ldecValueBenefitPaid = 0;
        //    int lintSuspendibleMonths = 0, lintAdjustmentStartYear = 0;
        //    DateTime ldtNormalRetirementDate = new DateTime();
        //    DateTime ldtDate = new DateTime(1986, 08, 01);

        //    #region Create New final claculation

        //    DateTime ldtDOB = Convert.ToDateTime(adrPersonInfo[enmPerson.date_of_birth.ToString()]);

        //    busPayeeAccount lbusPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
        //    lbusPayeeAccount.icdoPayeeAccount.LoadData(adrPersonInfo);

        //    busPlanBenefitXr lbusPlanBenefitXr = new busPlanBenefitXr();
        //    lbusPlanBenefitXr.FindPlanBenefitXr(lbusPayeeAccount.icdoPayeeAccount.plan_benefit_id);
        //    lbusPayeeAccount.icdoPayeeAccount.iintPlanId = lbusPlanBenefitXr.icdoPlanBenefitXr.plan_id;

        //    busBenefitCalculationRetirement lbusBenefitCalculationRetirement = new busBenefitCalculationRetirement();
        //    lbusBenefitCalculationRetirement = lbusPayeeAccount.ReCalculateBenefitForRetirement(lbusPlanBenefitXr.icdoPlanBenefitXr.benefit_option_value, true);
        //    #endregion

        //    #region Load Calculation Header and Detail objects
        //    if (lbusBenefitCalculationRetirement != null)
        //    {
        //        busBenefitCalculationDetail lbusBenefitCalculationDetail = new busBenefitCalculationDetail();
        //        busBenefitCalculationOptions lbusBenefitCalculationOptions = new busBenefitCalculationOptions();
        //        busBenefitCalculationYearlyDetail lbusBenefitCalculationYearlyDetail = new busBenefitCalculationYearlyDetail();

        //        lbusBenefitCalculationRetirement.LoadBenefitCalculationDetails();
        //        //lbusBenefitCalculationDetail = lbusBenefitCalculationRetirement.iclbBenefitCalculationDetail.Where(
        //        //                                item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault();         

        //        lbusBenefitCalculationDetail = lbusBenefitCalculationRetirement.iclbBenefitCalculationDetail.Where(
        //                                       item => item.icdoBenefitCalculationDetail.plan_id == lbusPayeeAccount.icdoPayeeAccount.iintPlanId).FirstOrDefault();

        //        ldtNormalRetirementDate = ldtDOB.AddYears(65);
        //        ldecNormalRetAge = 65;

        //        if (lbusBenefitCalculationDetail != null)
        //        {
        //            lbusBenefitCalculationDetail.LoadBenefitCalculationYearlyDetails();
        //            lbusBenefitCalculationDetail.LoadBenefitCalculationOptionss();
        //            lbusBenefitCalculationOptions = lbusBenefitCalculationDetail.iclbBenefitCalculationOptions[0];

        //            ldecBenefitOptionFactor = lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.benefit_option_factor;

        //            lintAdjustmentStartYear = lbusBenefitCalculationRetirement.ibusBenefitApplication.icdoBenefitApplication.min_distribution_date.Year + 1;

        //    #endregion

        //            for (int i = lintAdjustmentStartYear; i <= DateTime.Now.Year - 1; i++)
        //            {
        //                DateTime ldtPlanYrEndDate = busGlobalFunctions.GetLastDateOfComputationYear(i);
        //                lbusBenefitCalculationYearlyDetail = lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.Where(
        //                                                    item => item.icdoBenefitCalculationYearlyDetail.plan_year == i).FirstOrDefault();

        //                #region Set Accrual Flag

        //                bool lblnAccrualFlag = false;

        //                decimal ldecVestedHours = lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.Where(
        //                                                    item => item.icdoBenefitCalculationYearlyDetail.plan_year == i).FirstOrDefault().icdoBenefitCalculationYearlyDetail.vested_hours;

        //                int lintQualifiedYearCount = lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.Where(
        //                                                    item => item.icdoBenefitCalculationYearlyDetail.plan_year == i).FirstOrDefault().icdoBenefitCalculationYearlyDetail.qualified_years_count;

        //                if (ldecVestedHours >= 400 || lintQualifiedYearCount > 21)
        //                {
        //                    lblnAccrualFlag = true;
        //                }

        //                #endregion

        //                #region Calculate suspendible Month count

        //                DateTime ldtStartDate = busGlobalFunctions.GetFirstPayrollDayOfMonth(i, 1);
        //                DateTime ldtEndDate = busGlobalFunctions.GetLastPayrollDayOfMonth(i, 12);

        //                lintSuspendibleMonths = 12 - ibusCalculation.GetNonSuspendibleMonths(
        //                       lbusBenefitCalculationRetirement.ibusPerson.icdoPerson.istrSSNNonEncrypted.ToString(), lbusBenefitCalculationRetirement.ibusPerson, i, busConstant.MPIPP_PLAN_ID, null, ldtStartDate, ldtEndDate);

        //                #endregion

        //                #region Calculate YTD Benefit

        //                if (lblnAccrualFlag)
        //                {
        //                    ldecYTDAccruedBenefit = 0;
        //                }
        //                else
        //                {
        //                    ldecYTDAccruedBenefit = lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.Where(item => item.icdoBenefitCalculationYearlyDetail.plan_year ==
        //                                            i).FirstOrDefault().icdoBenefitCalculationYearlyDetail.accrued_benefit_amount;
        //                }

        //                #endregion

        //                #region Calculate Cummulative Benefit Amount

        //                ldecCumBenefitAmount = lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.Where(item => item.icdoBenefitCalculationYearlyDetail.plan_year ==
        //                                       i - 1).FirstOrDefault().icdoBenefitCalculationYearlyDetail.total_accrued_benefit + ldecYTDAccruedBenefit;

        //                #endregion

        //                #region Calculate EE contribution upto the plan year

        //                //ldecTableBfactor = ibusCalculation.GetTableBFactor(busConstant.BENEFIT_TYPE_RETIREMENT, busConstant.RETIREMENT_TYPE_LATE, busConstant.MPIPP_PLAN_ID,
        //                //                                                       lbusBenefitCalculationRetirement.ibusBenefitApplication.idecAge, DateTime.Now.Year - 1 < 1988 ? 1988 : DateTime.Now.Year - 1);

        //                //ibusCalculation.GetEEContributionsUptoPlanYear(lbusBenefitCalculationRetirement.iclbPersonAccountRetirementContribution,
        //                //        lbusBenefitCalculationRetirement.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault(),
        //                //        DateTime.Now.Year - 1, adtForfietureDate, out ldecEEContributionAmount, out ldecEEInterestAmount, out ldtEEContributionAsOfDate);

        //                #endregion

        //                #region Calculate Active Retiree Increase

        //                ldecActiveRetireeInc = lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.Where(item => item.icdoBenefitCalculationYearlyDetail.plan_year ==
        //                                       i).FirstOrDefault().icdoBenefitCalculationYearlyDetail.benefit_rate;


        //                #endregion

        //                #region Calculate ER current Year/ ER Derived Amount
        //                // EE Derived and EE Actuarial Increase will be the same as Prev Yrs Max EE Derived Benefit amount
        //                //decimal ldecEEDerivedBenefit = Math.Round((((ldecEEContributionAmount + ldecEEInterestAmount) * ldecTableAfactor) / ldecTableBfactor), 2);
        //                decimal ldecERDerivedBenefit = lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.Where(item => item.icdoBenefitCalculationYearlyDetail.plan_year ==
        //                                                        (i - 1)).FirstOrDefault().icdoBenefitCalculationYearlyDetail.annual_adjustment_amount -
        //                                                        lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.Where(item => item.icdoBenefitCalculationYearlyDetail.plan_year ==
        //                                                        (i - 1)).FirstOrDefault().icdoBenefitCalculationYearlyDetail.ee_derived_amount;

        //                if (ldtPlanYrEndDate > lbusBenefitCalculationRetirement.ibusBenefitApplication.icdoBenefitApplication.min_distribution_date ||
        //                       (IsParticipantDisabled(lbusBenefitCalculationRetirement.ibusBenefitApplication.icdoBenefitApplication.person_id)))
        //                {
        //                    ldecERCurrentYear = (ldecERDerivedBenefit * lintSuspendibleMonths) +
        //                                         (ldecERDerivedBenefit * (1 + ldecActiveRetireeInc) * lintSuspendibleMonths) +
        //                                        lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.Where(item => item.icdoBenefitCalculationYearlyDetail.plan_year ==
        //                                                            (i - 1)).FirstOrDefault().icdoBenefitCalculationYearlyDetail.er_derived_amount;
        //                }
        //                #endregion

        //                #region Calculate GAM factor and values

        //                decimal ldecAge = Math.Max(ldecNormalRetAge, Convert.ToInt32(lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.Where(item => item.icdoBenefitCalculationYearlyDetail.plan_year ==
        //                                       i).FirstOrDefault().icdoBenefitCalculationYearlyDetail.age));

        //                ldecGAM71Factor = ibusCalculation.GetBenefitTypeFactor(ldecAge) / ldecBenefitOptionFactor;

        //                #endregion

        //                #region Calculate Value Benefit Paid

        //                if (ldecERCurrentYear == 0)
        //                {
        //                    ldecValueBenefitPaid = 0;
        //                }
        //                else if (ldecERCurrentYear != 0 && ldecGAM71Factor != 0)
        //                {
        //                    ldecValueBenefitPaid = Math.Round((ldecERCurrentYear / ldecGAM71Factor) * (-1), 3);
        //                }

        //                #endregion

        //                #region Calculate Benefit As of Determination Date


        //                ldecBenefitAsOfDeteminationDate = Math.Round(lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.Where(item => item.icdoBenefitCalculationYearlyDetail.plan_year ==
        //                                                        (i - 1)).FirstOrDefault().icdoBenefitCalculationYearlyDetail.annual_adjustment_amount * (1 + ldecActiveRetireeInc), 2);

        //                if (Convert.ToInt32(lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.Where(item => item.icdoBenefitCalculationYearlyDetail.plan_year ==
        //                                       i).FirstOrDefault().icdoBenefitCalculationYearlyDetail.age) > ldecNormalRetAge)
        //                {
        //                    if ((!lblnAccrualFlag) || lbusBenefitCalculationRetirement.icdoBenefitCalculationHeader.retirement_date > ldtDate &&
        //                    ldtPlanYrEndDate < lbusBenefitCalculationRetirement.ibusBenefitApplication.icdoBenefitApplication.min_distribution_date &&
        //                        (!IsParticipantDisabled(lbusBenefitCalculationRetirement.ibusBenefitApplication.icdoBenefitApplication.person_id)))
        //                    {
        //                        ldecBenefitAsOfDeteminationDate = ldecBenefitAsOfDeteminationDate + ldecYTDAccruedBenefit;
        //                    }
        //                    else
        //                    {
        //                        ldecBenefitAsOfDeteminationDate = ldecBenefitAsOfDeteminationDate + Math.Max(0, ldecCumBenefitAmount + ldecValueBenefitPaid);
        //                    }
        //                }

        //                #endregion

        //                #region Update yearly detail table with MD ajdustment values

        //                //lbusBenefitCalculationYearlyDetail = lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.Where(
        //                //                                    item => item.icdoBenefitCalculationYearlyDetail.plan_year == DateTime.Now.Year - 1).FirstOrDefault();
        //                //lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.ee_derived_amount = ldecEEDerivedBenefit;
        //                lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.er_derived_amount = ldecERCurrentYear;
        //                lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.value_benefit_paid = ldecValueBenefitPaid;
        //                lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.gam_71_factor = ldecGAM71Factor;
        //                lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.active_retiree_inc = ldecActiveRetireeInc;
        //                lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.annual_adjustment_amount = ldecBenefitAsOfDeteminationDate;
        //                lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.accrued_benefit_amount = ldecYTDAccruedBenefit;
        //                lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.Update();

        //                #endregion
        //            }

        //            //Create payee account

        //            lbusBenefitCalculationRetirement.icdoBenefitCalculationHeader.istrRetirementType = busConstant.RETIREMENT_TYPE_MINIMUM_DISTRIBUTION;
        //            lbusBenefitCalculationRetirement.UpdateorInsertVlauesInPayeeAccountTable(true);

        //        }
        //    }
        //}
        #endregion

        #region Abhishek MOdification to Calculate Re-aval MD
        public void CalculateMDBenefitForReEvaluationBatch(DataRow adrPersonInfo, DataRow adrReportRow, DataTable dtReport)
        {
            decimal ldecBenefitAsOfDeteminationDate = 0, ldecActiveRetireeInc = 0, ldecNormalRetAge = 0,
                    ldecCumBenefitAmount = 0, ldecYTDAccruedBenefit = 0, ldecERCurrentYear = 0, ldecGAM71Factor = 0,
                    ldecBenefitOptionFactor = 0, ldecValueBenefitPaid = 0, ldecGrossBenefitAmount = 0.0m;
            int lintSuspendibleMonths = 0, lintAdjustmentStartYear = 0, lintPersonAcntId = 0;
            string istrBenefitOptionValue = string.Empty;
            int lintSurvivorAgeatMinDist = 0;
            DateTime ldtNormalRetirementDate = new DateTime();
            DateTime ldtDate = new DateTime(1986, 08, 01);

            #region Create New final claculation

            DateTime ldtDOB = Convert.ToDateTime(adrPersonInfo[enmPerson.date_of_birth.ToString()]);

            busPayeeAccount lbusPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
            lbusPayeeAccount.icdoPayeeAccount.LoadData(adrPersonInfo);

            busPlanBenefitXr lbusPlanBenefitXr = new busPlanBenefitXr();
            lbusPlanBenefitXr.FindPlanBenefitXr(lbusPayeeAccount.icdoPayeeAccount.plan_benefit_id);
            lbusPayeeAccount.icdoPayeeAccount.iintPlanId = lbusPlanBenefitXr.icdoPlanBenefitXr.plan_id;

            //for PIR-619
            #region NEW
            busPersonAccount lbusPersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };
            DataTable ldtbGetPersonAcntId = busBase.Select("cdoPersonAccount.GetPersonAccountID", new object[2] {lbusPayeeAccount.icdoPayeeAccount.iintPlanId ,
                                                                                                                 Convert.ToInt32(adrPersonInfo[enmPerson.person_id.ToString()])});
            if (ldtbGetPersonAcntId.Rows.Count > 0)
                lintPersonAcntId = Convert.ToInt32(ldtbGetPersonAcntId.Rows[0]["PERSON_ACCOUNT_ID"]);

            DataTable ldtbList = busBase.Select("cdoBenefitCalculationHeader.GetGrossAmtForMDReport", new object[2] { lbusPayeeAccount.icdoPayeeAccount.payee_account_id,
                                                                                                                     Convert.ToInt32(adrPersonInfo[enmPerson.person_id.ToString()])});
            if (ldtbList.IsNotNull() && ldtbList.Rows.Count > 0)
            {
                DataRow ldtrRow = ldtbList.Rows[0];
                if (ldtrRow["GROSS_BENEFIT_AMOUNT"] != DBNull.Value)
                    ldecGrossBenefitAmount = Convert.ToDecimal(ldtrRow["GROSS_BENEFIT_AMOUNT"]);
            }
            #endregion

            busBenefitCalculationRetirement lbusBenefitCalculationRetirement = new busBenefitCalculationRetirement();
            lbusBenefitCalculationRetirement = lbusPayeeAccount.ReCalculateBenefitForRetirement(lbusPlanBenefitXr.icdoPlanBenefitXr.benefit_option_value, null, true);
            #endregion

            #region Load Calculation Header and Detail objects
            if (lbusBenefitCalculationRetirement != null)
            {
                busBenefitCalculationDetail lbusBenefitCalculationDetail = new busBenefitCalculationDetail();
                //busBenefitCalculationOptions lbusBenefitCalculationOptions = new busBenefitCalculationOptions();
                busBenefitCalculationYearlyDetail lbusBenefitCalculationYearlyDetail = new busBenefitCalculationYearlyDetail();

                lbusBenefitCalculationRetirement.LoadBenefitCalculationDetails();
                //lbusBenefitCalculationDetail = lbusBenefitCalculationRetirement.iclbBenefitCalculationDetail.Where(
                //                                item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault();         

                lbusBenefitCalculationDetail = lbusBenefitCalculationRetirement.iclbBenefitCalculationDetail.Where(
                                               item => item.icdoBenefitCalculationDetail.plan_id == lbusPayeeAccount.icdoPayeeAccount.iintPlanId).FirstOrDefault();

                ldtNormalRetirementDate = ldtDOB.AddYears(65);
                ldecNormalRetAge = 65;

                if (lbusBenefitCalculationDetail != null)
                {
                    lbusBenefitCalculationDetail.LoadBenefitCalculationYearlyDetails();
                    lbusBenefitCalculationDetail.LoadBenefitCalculationOptionss();
                    //lbusBenefitCalculationOptions = lbusBenefitCalculationDetail.iclbBenefitCalculationOptions[0];
                    if (!lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.IsNullOrEmpty())
                        istrBenefitOptionValue = lbusBenefitCalculationDetail.iclbBenefitCalculationOptions[0].ibusPlanBenefitXr.icdoPlanBenefitXr.benefit_option_value;

                    //lintAdjustmentStartYear = lbusBenefitCalculationRetirement.ibusBenefitApplication.icdoBenefitApplication.min_distribution_date.Year + 1;
                    lintAdjustmentStartYear = lbusBenefitCalculationRetirement.ibusBenefitApplication.icdoBenefitApplication.min_distribution_date.Year;

                    busPerson lbusSurvivourInfo = new busPerson();
                    if (lbusSurvivourInfo.FindPerson(lbusBenefitCalculationRetirement.icdoBenefitCalculationHeader.beneficiary_person_id))
                    {
                        lintSurvivorAgeatMinDist = Convert.ToInt32(busGlobalFunctions.CalculatePersonAge(lbusSurvivourInfo.icdoPerson.idtDateofBirth, lbusBenefitCalculationRetirement.ibusBenefitApplication.icdoBenefitApplication.min_distribution_date));
                    }

                    if (!lbusBenefitCalculationRetirement.ibusBenefitApplication.aclbPersonWorkHistory_MPI.IsNullOrEmpty() &&
                         lbusBenefitCalculationRetirement.ibusBenefitApplication.aclbPersonWorkHistory_MPI.Where(i => i.year == lbusBenefitCalculationRetirement.ibusBenefitApplication.icdoBenefitApplication.retirement_date.Year).Count() > 0)
                    {
                        lbusBenefitCalculationRetirement.ibusBenefitApplication.aclbPersonWorkHistory_MPI.RemoveAt(lbusBenefitCalculationRetirement.ibusBenefitApplication.aclbPersonWorkHistory_MPI.IndexOf(lbusBenefitCalculationRetirement.ibusBenefitApplication.aclbPersonWorkHistory_MPI.Where(i => i.year == lbusBenefitCalculationRetirement.ibusBenefitApplication.icdoBenefitApplication.retirement_date.Year).FirstOrDefault()));
                    }

                    #endregion

                    if (lbusBenefitCalculationRetirement.ibusBenefitApplication.icdoBenefitApplication.min_distribution_date != DateTime.MinValue)
                    {
                        int counter = 0;
                        decimal ldecBenefitOptionFactorStartingPoint = 0;
                        decimal ldecErDerivedPrevious = 0;
                        decimal ldecTotalAccuredBenefit = Decimal.Zero;
                        decimal ldecAnnualAdjustmentPreviousYear = Decimal.Zero;
                        decimal ldecEEDerivedAmt = Decimal.Zero;
                        decimal ldecEEDerivedPreviousYear = Decimal.Zero;

                        for (int i = lintAdjustmentStartYear; i <= DateTime.Now.Year - 1; i++)
                        {
                            DateTime ldtPlanYrEndDate = busGlobalFunctions.GetLastDateOfComputationYear(i);
                            lbusBenefitCalculationYearlyDetail = lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.Where(
                                                                item => item.icdoBenefitCalculationYearlyDetail.plan_year == i).LastOrDefault();

                            //Rashmi Fixed 04/23/2014(check if lbusBenefitCalculationYearlyDetail is not null)
                            if (lbusBenefitCalculationYearlyDetail != null)
                            {
                                if (counter == 0)
                                    ldecBenefitOptionFactorStartingPoint = ibusCalculation.GetFactor(lbusBenefitCalculationRetirement, istrBenefitOptionValue, lintSurvivorAgeatMinDist, Convert.ToInt32(lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.age) - 1);

                                //PROD PIR 275 -- Getting factors for 10LA which depends on remaining term certain years
                                if (i > lbusBenefitCalculationRetirement.ibusBenefitApplication.icdoBenefitApplication.min_distribution_date.Year && istrBenefitOptionValue == busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY)
                                {
                                    int lintReferenceNumber = (lbusPayeeAccount.icdoPayeeAccount.term_certain_end_date.Year - i);
                                    ldecBenefitOptionFactor = ibusCalculation.GetFactor(lbusBenefitCalculationRetirement, istrBenefitOptionValue, lintSurvivorAgeatMinDist + 1 + counter, Convert.ToInt32(lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.age), lintReferenceNumber, busConstant.BOOL_TRUE);
                                }
                                else
                                    ldecBenefitOptionFactor = ibusCalculation.GetFactor(lbusBenefitCalculationRetirement, istrBenefitOptionValue, lintSurvivorAgeatMinDist + 1 + counter, Convert.ToInt32(lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.age));


                                #region Set Accrual Flag

                                bool lblnAccrualFlag = false;

                                decimal ldecVestedHours = lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.vested_hours;

                                int lintQualifiedYearCount = lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.qualified_years_count;

                                if (ldecVestedHours >= 400 || lintQualifiedYearCount > 20)
                                {
                                    lblnAccrualFlag = true;
                                }

                                #endregion

                                #region Calculate suspendible Month count

                                DateTime ldtStartDate = busGlobalFunctions.GetFirstPayrollDayOfMonth(i, 1);
                                DateTime ldtEndDate = busGlobalFunctions.GetLastPayrollDayOfMonth(i, 12);

                                lintSuspendibleMonths = 12 - ibusCalculation.GetNonSuspendibleMonths(
                                       lbusBenefitCalculationRetirement.ibusPerson.icdoPerson.istrSSNNonEncrypted.ToString(), lbusBenefitCalculationRetirement.ibusPerson, i, busConstant.MPIPP_PLAN_ID, null, ldtStartDate, ldtEndDate);

                                #endregion

                                #region Calculate YTD Benefit

                                if (!lblnAccrualFlag)
                                {
                                    ldecYTDAccruedBenefit = 0;
                                }
                                else
                                {
                                    //ldecYTDAccruedBenefit = Math.Round(lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.accrued_benefit_amount * ldecBenefitOptionFactor, 2);
                                }

                                #endregion

                                #region Calculate Cummulative Benefit Amount

                                ldecCumBenefitAmount = ldecCumBenefitAmount + ldecYTDAccruedBenefit;

                                //if (counter == 0)
                                //{
                                //    ldecTotalAccuredBenefit = lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.Where(item => item.icdoBenefitCalculationYearlyDetail.plan_year ==
                                //                                      i - 1).FirstOrDefault().icdoBenefitCalculationYearlyDetail.total_accrued_benefit + ldecYTDAccruedBenefit;
                                //}
                                //else
                                //{
                                //    ldecTotalAccuredBenefit = ldecTotalAccuredBenefit + ldecCumBenefitAmount;
                                //}

                                //if (counter == 0)
                                //{
                                //    ldecCumBenefitAmount = lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.Where(item => item.icdoBenefitCalculationYearlyDetail.plan_year ==
                                //                            i - 1).FirstOrDefault().icdoBenefitCalculationYearlyDetail.annual_adjustment_amount * ldecBenefitOptionFactorStartingPoint + ldecYTDAccruedBenefit;
                                //}
                                //else
                                //{
                                //    ldecCumBenefitAmount = lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.Where(item => item.icdoBenefitCalculationYearlyDetail.plan_year ==
                                //                       i - 1).FirstOrDefault().icdoBenefitCalculationYearlyDetail.total_accrued_benefit * ldecBenefitOptionFactor + ldecYTDAccruedBenefit;
                                //}

                                #endregion

                                #region Calculate Active Retiree Increase

                                ldecActiveRetireeInc = lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.active_retiree_inc;


                                #endregion

                                #region Calculate ER current Year/ ER Derived Amount
                                // EE Derived and EE Actuarial Increase will be the same as Prev Yrs Max EE Derived Benefit amount
                                //decimal ldecEEDerivedBenefit = Math.Round((((ldecEEContributionAmount + ldecEEInterestAmount) * ldecTableAfactor) / ldecTableBfactor), 2);
                                decimal ldecERDerivedBenefit = 0.0M;

                                if (counter == 0)
                                {
                                    ldecEEDerivedAmt = Math.Round(lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.max_ee_derv_amt * ldecBenefitOptionFactor, 2);

                                    Decimal ldecEEDerivedReference = Math.Round(lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.Where(item => item.icdoBenefitCalculationYearlyDetail.plan_year ==
                                                                            (i - 1)).FirstOrDefault().icdoBenefitCalculationYearlyDetail.max_ee_derv_amt * ldecBenefitOptionFactorStartingPoint, 2);

                                    ldecERDerivedBenefit = Math.Round(lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.Last().icdoBenefitCalculationYearlyDetail.actuarial_equivalent_amount * ldecBenefitOptionFactorStartingPoint - ldecEEDerivedReference, 2);
                                }
                                else
                                {
                                    ldecEEDerivedAmt = Math.Round(lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.max_ee_derv_amt * ldecBenefitOptionFactor, 2);

                                    ldecERDerivedBenefit = Math.Round(ldecAnnualAdjustmentPreviousYear - ldecEEDerivedPreviousYear, 2);
                                }




                                if (ldtPlanYrEndDate > lbusBenefitCalculationRetirement.ibusBenefitApplication.icdoBenefitApplication.min_distribution_date ||
                                       (IsParticipantDisabled(lbusBenefitCalculationRetirement.ibusBenefitApplication.icdoBenefitApplication.person_id)))
                                {


                                    ldecERCurrentYear = Math.Round(((ldecERDerivedBenefit * lintSuspendibleMonths) +
                                                         (ldecERDerivedBenefit * (1 + ldecActiveRetireeInc) * 0) + ldecErDerivedPrevious), 2);
                                    //lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.Where(item => item.icdoBenefitCalculationYearlyDetail.plan_year ==
                                    //                    (i - 1)).FirstOrDefault().icdoBenefitCalculationYearlyDetail.er_derived_amount;                            
                                }
                                #endregion

                                #region Calculate GAM factor and values

                                decimal ldecAge = Math.Max(ldecNormalRetAge, Convert.ToInt32(lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.age));

                                if (ldecBenefitOptionFactor != 0)
                                    ldecGAM71Factor = Math.Round((ibusCalculation.GetBenefitTypeFactor(ldecAge) / ldecBenefitOptionFactor), 3);

                                #endregion

                                #region Calculate Value Benefit Paid

                                if (ldecERCurrentYear == 0)
                                {
                                    ldecValueBenefitPaid = 0;
                                }
                                else if (ldecERCurrentYear != 0 && ldecGAM71Factor != 0)
                                {
                                    ldecValueBenefitPaid = (ldecERCurrentYear / ldecGAM71Factor) * (-1);
                                }

                                #endregion

                                #region Calculate Benefit As of Determination Date

                                if (counter == 0)
                                {
                                    ldecBenefitAsOfDeteminationDate = Math.Round(lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.Where(item => item.icdoBenefitCalculationYearlyDetail.plan_year ==
                                                                            (i - 1)).FirstOrDefault().icdoBenefitCalculationYearlyDetail.actuarial_equivalent_amount * ldecBenefitOptionFactorStartingPoint * (1 + ldecActiveRetireeInc), 2);

                                }

                                else
                                {
                                    ldecBenefitAsOfDeteminationDate = Math.Round((ldecAnnualAdjustmentPreviousYear) * (1 + ldecActiveRetireeInc), 2);
                                }

                                if (Convert.ToInt32(lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.Where(item => item.icdoBenefitCalculationYearlyDetail.plan_year ==
                                                       i).FirstOrDefault().icdoBenefitCalculationYearlyDetail.age) > ldecNormalRetAge)
                                {
                                    if ((!lblnAccrualFlag) || lbusBenefitCalculationRetirement.icdoBenefitCalculationHeader.retirement_date > ldtDate &&
                                    ldtPlanYrEndDate < lbusBenefitCalculationRetirement.ibusBenefitApplication.icdoBenefitApplication.min_distribution_date &&
                                        (!IsParticipantDisabled(lbusBenefitCalculationRetirement.ibusBenefitApplication.icdoBenefitApplication.person_id)))
                                    {
                                        ldecBenefitAsOfDeteminationDate = Math.Round(ldecBenefitAsOfDeteminationDate + ldecYTDAccruedBenefit, 2);
                                    }
                                    else
                                    {
                                        ldecBenefitAsOfDeteminationDate = Math.Round(ldecBenefitAsOfDeteminationDate + Math.Max(0, ldecCumBenefitAmount + ldecValueBenefitPaid), 2);
                                    }
                                }

                                #endregion

                                #region Update yearly detail table with MD ajdustment values

                                //lbusBenefitCalculationYearlyDetail = lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.Where(
                                //                                    item => item.icdoBenefitCalculationYearlyDetail.plan_year == DateTime.Now.Year - 1).FirstOrDefault();

                                ldecErDerivedPrevious = ldecERCurrentYear;
                                ldecEEDerivedPreviousYear = ldecEEDerivedAmt;

                                lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.ee_derived_amount = ldecEEDerivedAmt;
                                lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.er_derived_amount = ldecERDerivedBenefit;
                                lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.value_benefit_paid = ldecValueBenefitPaid;
                                lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.gam_71_factor = ldecGAM71Factor;
                                lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.active_retiree_inc = ldecActiveRetireeInc;
                                lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.annual_adjustment_amount = ldecBenefitAsOfDeteminationDate;
                                ldecAnnualAdjustmentPreviousYear = ldecBenefitAsOfDeteminationDate;
                                if (ldecYTDAccruedBenefit != 0)
                                {
                                    lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.accrued_benefit_amount = ldecYTDAccruedBenefit;
                                }

                                lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.benefit_option_factor = ldecBenefitOptionFactor;
                                //lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.total_accrued_benefit = ldecTotalAccuredBenefit;
                                //Write Code to Modify the Total Accured Benefit According 
                                lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.Update();

                                #endregion

                                counter++;
                            }
                        }

                        //Create payee account

                        lbusBenefitCalculationRetirement.icdoBenefitCalculationHeader.istrRetirementType = busConstant.RETIREMENT_TYPE_MINIMUM_DISTRIBUTION;

                        //Rashmi Fixed 04/23/2014(changed the busObject used in the lambda first it was lbusBenefitCalculationHeader now changed to lbusBenefitCalculationRetirement)
                        if ((ldecBenefitAsOfDeteminationDate.IsNull() || ldecBenefitAsOfDeteminationDate == busConstant.ZERO_DECIMAL)
                                && lbusBenefitCalculationRetirement.iclbBenefitCalculationDetail.IsNotNull()
                                && lbusBenefitCalculationRetirement.iclbBenefitCalculationDetail.Where(i => i.icdoBenefitCalculationDetail.plan_id == lbusPayeeAccount.icdoPayeeAccount.iintPlanId).Count() > 0)
                            ldecBenefitAsOfDeteminationDate = lbusBenefitCalculationRetirement.iclbBenefitCalculationDetail.Where(i => i.icdoBenefitCalculationDetail.plan_id == lbusPayeeAccount.icdoPayeeAccount.iintPlanId).FirstOrDefault().icdoBenefitCalculationDetail.early_reduced_benefit_amount;

                        if (!lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.IsNullOrEmpty())
                        {
                            lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.First().LoadData(lbusPayeeAccount.icdoPayeeAccount.plan_benefit_id, ldecBenefitOptionFactor, ldecBenefitAsOfDeteminationDate,
                                                                                                        busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER, lbusPayeeAccount.icdoPayeeAccount.person_id, lbusBenefitCalculationRetirement.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                                                        istrBenefitOptionValue, 0);
                            lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.Update();
                        }

                        else
                        {
                            lbusBenefitCalculationDetail.iclbBenefitCalculationOptions = new Collection<busBenefitCalculationOptions>();
                            busBenefitCalculationOptions lbusBenefitCalculationOptions = new busBenefitCalculationOptions();
                            lbusBenefitCalculationOptions.icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions();
                            lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.benefit_calculation_detail_id = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.benefit_calculation_detail_id;
                            lbusBenefitCalculationOptions.LoadData(lbusPayeeAccount.icdoPayeeAccount.plan_benefit_id, ldecBenefitOptionFactor, ldecBenefitAsOfDeteminationDate,
                                                                    busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER, lbusPayeeAccount.icdoPayeeAccount.person_id, lbusBenefitCalculationRetirement.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                    istrBenefitOptionValue, 0);
                            lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
                            lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.Insert();
                        }

                        #region Prod PIR 619
                        decimal ldecMonthlyGrossAmount = 0.0m, ldecBenefitAmount = 0.0m;
                        busPerson lbusPerson = new busPerson { icdoPerson = new cdoPerson() };
                        lbusPerson.icdoPerson.LoadData(adrPersonInfo);
                        DataTable dt = new DataTable();
                        adrReportRow = dtReport.NewRow();
                        adrReportRow["First_Name"] = lbusPerson.icdoPerson.first_name;
                        adrReportRow["Last_Name"] = lbusPerson.icdoPerson.last_name;
                        adrReportRow["MPI_PERSON_ID"] = lbusPerson.icdoPerson.mpi_person_id;
                        adrReportRow["Minimum_Distribution_Date"] = lbusBenefitCalculationRetirement.ibusBenefitApplication.icdoBenefitApplication.min_distribution_date; //pir-619
                        adrReportRow["Calculation_Id"] = lbusBenefitCalculationRetirement.icdoBenefitCalculationHeader.benefit_calculation_header_id;

                        //ldecMonthlyGrossAmount = (Decimal)DBFunction.DBExecuteScalar("cdoPersonAccount.GetPayeeAccountwithGrossPayment",
                        //        new object[2] { lintPersonAcntId, lbusPayeeAccount.icdoPayeeAccount.iintPlanId }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework, iobjPassInfo.isrvMetaDataCache);
                        //adrReportRow["Benefit_Amount"] = ldecMonthlyGrossAmount;

                        //DataTable ldtbList = busBase.Select("cdoBenefitCalculationHeader.GetGrossAmtForMDReport", new object[2] { lbusPayeeAccount.icdoPayeeAccount.payee_account_id, 
                        //                                                                                                        lbusPerson.icdoPerson.person_id});
                        //if (ldtbList.IsNotNull() && ldtbList.Rows.Count > 0)
                        //{
                        //    DataRow ldtrRow = ldtbList.Rows[0];
                        //    if (ldtrRow["GROSS_BENEFIT_AMOUNT"] != DBNull.Value)
                        //        ldecGrossBenefitAmount = Convert.ToDecimal(ldtrRow["GROSS_BENEFIT_AMOUNT"]);
                        //}
                        adrReportRow["Monthly_Benefit"] = ldecGrossBenefitAmount;
                        adrReportRow["Suspendible_Months"] = lintSuspendibleMonths;
                        adrReportRow["ER_CurrentYear"] = ldecERCurrentYear;
                        adrReportRow["BenefitAsOfDeterminationDt"] = ldecBenefitAsOfDeteminationDate;

                        ldecBenefitAmount = lbusBenefitCalculationDetail.iclbBenefitCalculationOptions[0].icdoBenefitCalculationOptions.benefit_amount;
                        adrReportRow["DifferenceInAmount"] = ldecGrossBenefitAmount - ldecBenefitAmount;
                        adrReportRow["PLAN_NAME"] = adrPersonInfo["PLAN_NAME"];

                        dtReport.Rows.Add(adrReportRow);
                        //Commenting Payee Account Code
                        //lbusBenefitCalculationRetirement.UpdateorInsertVlauesInPayeeAccountTable(Convert.ToDateTime(adrPersonInfo[enmPayeeAccount.benefit_begin_date.ToString()]), true);
                        #endregion
                    }
                }
            }
        }

        public void RecalculateMDBenefits(busBenefitCalculationRetirement abusBenefitCalculationRetirement, busPayeeAccount abusPayeeAccount, bool ablnReEmployed = false,bool ablnRetiree = false)
        {

            decimal ldecBenefitAsOfDeteminationDate = 0, ldecActiveRetireeInc = 0, ldecNormalRetAge = 0,
                    ldecCumBenefitAmount = 0, ldecYTDAccruedBenefit = 0, ldecERCurrentYear = 0, ldecGAM71Factor = 0,
                    ldecBenefitOptionFactor = 0, ldecLifeBenefitOptionFactor = 0, ldecValueBenefitPaid = 0, ldecLocal700GauranteedAmt = 0,
                    ldecLifeBenefitAsOfDeteminationDate = 0, ldecLifeCumBenefitAmount = 0, ldecLifeYTDAccruedBenefit = 0, ldecLifeERCurrentYear = 0, ldecLifeValueBenefitPaid = 0, ldecLifeGAM71Factor = 0; //PIR 894
            DateTime ldtMminimumDistributionEffectiveDate = DateTime.MinValue;

            int lintSuspendibleMonths = 0, lintSuspendibleMonthsAfterIncrease = 0, lintAdjustmentStartYear = 0, lintAdjustmentEndYear = 0;
            string istrBenefitOptionValue = string.Empty;
            string istrLifeBenefitOptionValue = busConstant.LIFE; //PIR 894
            int lintSurvivorAgeatMinDist = 0;
            DateTime ldtNormalRetirementDate = new DateTime();
            DateTime ldtDate = new DateTime(1986, 08, 01);
            DateTime ldtActiveIncreaseEffectiveDate = DateTime.MinValue;

            //PIR 894
            bool lblnPopupToLife = false;
            DateTime ldtJointAnnuitantDOD = DateTime.MinValue;

            //RequestID: 72091
            decimal ldecPopupOptionFactorAtRet = decimal.Zero;

            if (abusBenefitCalculationRetirement.iclbBenefitCalculationDetail != null &&  //RID 76605 To fix error
                abusBenefitCalculationRetirement.iclbBenefitCalculationDetail.First().iclbBenefitCalculationOptions != null &&  //RID 76605 To fix error
                abusBenefitCalculationRetirement.iclbBenefitCalculationDetail.First().iclbBenefitCalculationOptions.Count()>0 &&  //RID 76605 To fix error
                abusBenefitCalculationRetirement.iclbBenefitCalculationDetail.First().iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions != null &&
                abusBenefitCalculationRetirement.iclbBenefitCalculationDetail.First().iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.pop_up_option_factor_at_ret != 0)
                ldecPopupOptionFactorAtRet = abusBenefitCalculationRetirement.iclbBenefitCalculationDetail.First().iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.pop_up_option_factor_at_ret;

            busActiveRetireeIncreaseContract lbusActiveRetireeIncreaseContract = new busActiveRetireeIncreaseContract();
            Collection<busActiveRetireeIncreaseContract> lclbActiveRetireeIncreaseContract = new Collection<busActiveRetireeIncreaseContract>();

            #region Load Calculation Header and Detail objects
            if (abusBenefitCalculationRetirement != null)
            {
                busBenefitCalculationDetail lbusBenefitCalculationDetail = new busBenefitCalculationDetail();
                busBenefitCalculationYearlyDetail lbusBenefitCalculationYearlyDetail = new busBenefitCalculationYearlyDetail();
                busBenefitCalculationOptions lbusbenefitCalculationOption = new busBenefitCalculationOptions();

                //PIR - 930
                if (!ablnReEmployed)
                {
                    abusBenefitCalculationRetirement.LoadBenefitCalculationDetails();
                }

                lbusBenefitCalculationDetail = abusBenefitCalculationRetirement.iclbBenefitCalculationDetail.Where(
                                               item => item.icdoBenefitCalculationDetail.plan_id == abusPayeeAccount.icdoPayeeAccount.iintPlanId).FirstOrDefault();


                ldtNormalRetirementDate = abusBenefitCalculationRetirement.ibusPerson.icdoPerson.date_of_birth.AddYears(65);
                if (ldtNormalRetirementDate.Day != 1)
                {
                    ldtNormalRetirementDate = ldtNormalRetirementDate.AddMonths(1).GetFirstDayofMonth();
                }
                ldecNormalRetAge = 65;

                if (lbusBenefitCalculationDetail != null)
                {
                    //PIR - 930
                    if (!ablnReEmployed)
                    {
                        lbusBenefitCalculationDetail.LoadBenefitCalculationYearlyDetails();
                        lbusBenefitCalculationDetail.LoadBenefitCalculationOptionss();
                    }
                    else
                    {
                        lbusbenefitCalculationOption = lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.FirstOrDefault();
                    }

                    //PIR - 930
                    if (!lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.IsNullOrEmpty())
                    {
                        istrBenefitOptionValue = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.istrBenefitOptionValue;
                        if(Convert.ToString(istrBenefitOptionValue).IsNullOrEmpty())
                        {
                            istrBenefitOptionValue = lbusBenefitCalculationDetail.iclbBenefitCalculationOptions[0].ibusPlanBenefitXr.icdoPlanBenefitXr.benefit_option_value;
                        }

                        if (abusBenefitCalculationRetirement.ibusBenefitApplication.icdoBenefitApplication.min_distribution_date != DateTime.MinValue && !ablnReEmployed)
                        {
                            lintAdjustmentStartYear = abusBenefitCalculationRetirement.ibusBenefitApplication.icdoBenefitApplication.min_distribution_date.Year;
                        }
                        else
                        {
                           if(abusBenefitCalculationRetirement.ibusBenefitApplication.IsNotNull())
                            {
                                // RID# 153935 If Differ normal MD and retired before selected MD date, use Retirement date for adjustment. This is also fixing age 73 md selection.
                                //DateTime lMDDate = new DateTime();
                                //lMDDate = Convert.ToDateTime(busGlobalFunctions.CalculateMinDistributionDate(abusBenefitCalculationRetirement.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.vested_date));
                                //if (lMDDate == Convert.ToDateTime(busGlobalFunctions.Calculate72MinDistributionDate(abusBenefitCalculationRetirement.ibusBenefitApplication.ibusPerson.icdoPerson.date_of_birth, lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.vested_date)))
                                //{
                                //    if (abusBenefitCalculationRetirement.icdoBenefitCalculationHeader.retirement_date < lMDDate)
                                //    {
                                //        abusBenefitCalculationRetirement.ibusBenefitApplication.icdoBenefitApplication.min_distribution_date = Convert.ToDateTime(busGlobalFunctions.CalculateMinDistributionDate(abusBenefitCalculationRetirement.ibusBenefitApplication.ibusPerson.icdoPerson.date_of_birth, lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.vested_date));
                                //    }
                                //    else
                                //    {
                                //        abusBenefitCalculationRetirement.ibusBenefitApplication.icdoBenefitApplication.min_distribution_date = lMDDate;
                                //    }
                                //}
                                //else
                                //{
                                //    abusBenefitCalculationRetirement.ibusBenefitApplication.icdoBenefitApplication.min_distribution_date = lMDDate;
                                //}

                                int lintPersonId = abusBenefitCalculationRetirement.ibusBenefitApplication.ibusPerson.icdoPerson.person_id;
                                DateTime ldtVestedDate = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.vested_date;
                                DateTime ldtDOB = abusBenefitCalculationRetirement.ibusBenefitApplication.ibusPerson.icdoPerson.date_of_birth;
                                DateTime ldtRetirementDate = abusBenefitCalculationRetirement.icdoBenefitCalculationHeader.retirement_date;
                                DateTime ldtNormalMDDate = new DateTime();
                                DateTime ldtSelectedMDDate = new DateTime();
                                bool lblnMDDiffer = false;

                                ldtNormalMDDate = busGlobalFunctions.GetMinDistributionDate(ldtDOB, ldtVestedDate); //calculate MD date based on age 70.5
                                ldtSelectedMDDate = busGlobalFunctions.GetMinDistributionDate(lintPersonId, ldtVestedDate);  //calculate MD Date based on participant MD age option
                                lblnMDDiffer = ldtSelectedMDDate > ldtNormalMDDate;
                                //If participant had MD differ but retireed before Differed MD date, business asked to use 70.5 instead of if participant differed MD date.
                                if (lblnMDDiffer && ldtRetirementDate < ldtSelectedMDDate)
                                {
                                    //abusBenefitCalculationRetirement.ibusBenefitApplication.icdoBenefitApplication.min_distribution_date = ldtNormalMDDate;
                                    abusBenefitCalculationRetirement.ibusBenefitApplication.icdoBenefitApplication.min_distribution_date = ldtSelectedMDDate;
                                    lintAdjustmentStartYear = ldtRetirementDate.Year;
                                }
                                else
                                {
                                    abusBenefitCalculationRetirement.ibusBenefitApplication.icdoBenefitApplication.min_distribution_date = ldtSelectedMDDate;
                                    lintAdjustmentStartYear = abusBenefitCalculationRetirement.ibusBenefitApplication.icdoBenefitApplication.min_distribution_date.Year;
                                }
                            }

                            //lintAdjustmentStartYear = abusBenefitCalculationRetirement.ibusBenefitApplication.icdoBenefitApplication.min_distribution_date.Year;
                        }
                                               
                        busPerson lbusSurvivourInfo = new busPerson();
                        if (lbusSurvivourInfo.FindPerson(abusBenefitCalculationRetirement.icdoBenefitCalculationHeader.beneficiary_person_id))
                        {
                            //Check with Actuary
                            lintSurvivorAgeatMinDist = Convert.ToInt32(Math.Floor(busGlobalFunctions.CalculatePersonAge(lbusSurvivourInfo.icdoPerson.idtDateofBirth, abusBenefitCalculationRetirement.ibusBenefitApplication.icdoBenefitApplication.min_distribution_date)));                            
                        }

                        if (!abusBenefitCalculationRetirement.ibusBenefitApplication.aclbPersonWorkHistory_MPI.IsNullOrEmpty() &&
                             abusBenefitCalculationRetirement.ibusBenefitApplication.aclbPersonWorkHistory_MPI.Where(i => i.year == abusBenefitCalculationRetirement.ibusBenefitApplication.icdoBenefitApplication.retirement_date.Year).Count() > 0)
                        {
                            abusBenefitCalculationRetirement.ibusBenefitApplication.aclbPersonWorkHistory_MPI.RemoveAt(abusBenefitCalculationRetirement.ibusBenefitApplication.aclbPersonWorkHistory_MPI.IndexOf(abusBenefitCalculationRetirement.ibusBenefitApplication.aclbPersonWorkHistory_MPI.Where(i => i.year == abusBenefitCalculationRetirement.ibusBenefitApplication.icdoBenefitApplication.retirement_date.Year).FirstOrDefault()));
                        }

                        #endregion

                        //PIR - 930
                        bool lblnRetiredAfterMDAge = false;
                        if (ablnReEmployed && abusBenefitCalculationRetirement.ibusBenefitApplication.icdoBenefitApplication.min_distribution_date.Year <
                            abusBenefitCalculationRetirement.icdoBenefitCalculationHeader.retirement_date.Year)
                        {
                            ablnReEmployed = false;
                            lblnRetiredAfterMDAge = true;
                        }
                        else if (ablnRetiree)
                        {
                            //Ticket - 65946
                            if (abusBenefitCalculationRetirement.ibusBenefitApplication.icdoBenefitApplication.min_distribution_date >
                            abusBenefitCalculationRetirement.icdoBenefitCalculationHeader.retirement_date)
                                return;

                            lblnRetiredAfterMDAge = true;

                        }

                        int lintParticipantAgeAtRetirementMD = 0;
                        if(ablnReEmployed)
                        {
                            lintParticipantAgeAtRetirementMD = 
                                Convert.ToInt32(Math.Floor(busGlobalFunctions.CalculatePersonAge(abusBenefitCalculationRetirement.ibusPerson.icdoPerson.date_of_birth, abusBenefitCalculationRetirement.icdoBenefitCalculationHeader.retirement_date)));
                        }
                        else
                        {
                            lintParticipantAgeAtRetirementMD = 
                                Convert.ToInt32(Math.Floor(busGlobalFunctions.CalculatePersonAge(abusBenefitCalculationRetirement.ibusPerson.icdoPerson.date_of_birth, abusBenefitCalculationRetirement.ibusBenefitApplication.icdoBenefitApplication.min_distribution_date)));
                        }


                        if (abusBenefitCalculationRetirement.ibusBenefitApplication.icdoBenefitApplication.min_distribution_date != DateTime.MinValue)
                        {
                            int counter = 0;
                            decimal ldecBenefitOptionFactorStartingPoint = 0;
                            decimal ldecLifeBenefitOptionFactorStartingPoint = 0;
                            decimal ldecErDerivedPrevious = 0;
                            decimal ldecLifeErDerivedPrevioius = 0;

                            decimal ldecAnnualAdjustmentPreviousYear = Decimal.Zero;
                            decimal ldecLifeAnnualAdjustmentPreviousYear = Decimal.Zero; //PIR 894
                            decimal ldecEEDerivedAmt = Decimal.Zero;
                            decimal ldecEEDerivedPreviousYear = Decimal.Zero;
                            //PIR 894
                            decimal ldecLifeEEDerivedAmt = Decimal.Zero;
                            decimal ldecLifeEEDerivedPreviousYear = Decimal.Zero;
                            
                            decimal ldecPopupToLife = Decimal.Zero; 
                            decimal ldecPopupoptionfactor = Decimal.Zero;
                            decimal ldecPopupbenefitamount = Decimal.Zero;
                            decimal ldecPopupoptionfactoratret = decimal.Zero; //RequestID: 72091


                            //PIR - 930
                            bool lblnRetirementYearSplit = false;

                            // Fetch the Plan Benefit Rates
                            DataTable ldtbPlanBenefitRate = busBase.Select("cdoPlanBenefitRate.Lookup", new object[] { });
                            Collection<cdoPlanBenefitRate> lclbcdoPlanBenefitRate = cdoDummyWorkData.GetCollection<cdoPlanBenefitRate>(ldtbPlanBenefitRate);

                            //Ticket - 61734
                            if (lblnRetiredAfterMDAge && !ablnReEmployed && abusBenefitCalculationRetirement.icdoBenefitCalculationHeader.retirement_date.Year == DateTime.Now.Year)
                            {
                                lintAdjustmentEndYear = DateTime.Now.Year;
                            }
                            else
                            {
                                lintAdjustmentEndYear = DateTime.Now.Year - 1;
                            }


                            //RID 61301                           
                            if (lintAdjustmentStartYear <= lintAdjustmentEndYear)
                            {

                                //RID 61301
                                int lintParticipantAgeAtMD = 0;
                                if (ablnReEmployed && abusBenefitCalculationRetirement.ibusBenefitApplication.icdoBenefitApplication.min_distribution_date.Year >
                                    abusBenefitCalculationRetirement.icdoBenefitCalculationHeader.retirement_date.Year
                                    && lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.Where(item => item.icdoBenefitCalculationYearlyDetail.plan_year == lintAdjustmentStartYear).Count() > 0)
                                {
                                    lintParticipantAgeAtMD =
                                        Convert.ToInt32(Math.Floor(busGlobalFunctions.CalculatePersonAge(abusBenefitCalculationRetirement.ibusPerson.icdoPerson.date_of_birth, new DateTime(lintAdjustmentStartYear, 01,01))));
                                }
                                else if(lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.Where(item => item.icdoBenefitCalculationYearlyDetail.plan_year == lintAdjustmentStartYear).Count() > 0)
                                {
                                    lintParticipantAgeAtMD = Convert.ToInt32(Math.Floor(lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.Where(item => item.icdoBenefitCalculationYearlyDetail.plan_year == lintAdjustmentStartYear).FirstOrDefault().icdoBenefitCalculationYearlyDetail.age));
                                }

                                //PIR 894
                                if (istrBenefitOptionValue == busConstant.LIFE)
                                {
                                    DataTable ldtbBenefitOpValue = busBase.Select("cdoPayeeAccount.GetBenefitOptionValueFromBenefitDtlId", new object[1] { abusPayeeAccount.icdoPayeeAccount.benefit_application_detail_id });
                                    if (ldtbBenefitOpValue != null && ldtbBenefitOpValue.Rows.Count > 0)
                                    {
                                        if (!Convert.ToString(ldtbBenefitOpValue.Rows[0][enmPerson.date_of_death.ToString().ToUpper()]).IsNullOrEmpty()
                                            && Convert.ToDateTime(ldtbBenefitOpValue.Rows[0][enmPerson.date_of_death.ToString().ToUpper()]).Year >= lintAdjustmentStartYear)
                                        {
                                            lblnPopupToLife = true;
                                            if (!Convert.ToString(ldtbBenefitOpValue.Rows[0][enmPlanBenefitXr.benefit_option_value.ToString().ToUpper()]).IsNullOrEmpty())
                                                istrBenefitOptionValue = Convert.ToString(ldtbBenefitOpValue.Rows[0][enmPlanBenefitXr.benefit_option_value.ToString().ToUpper()]);

                                            ldtJointAnnuitantDOD = Convert.ToDateTime(ldtbBenefitOpValue.Rows[0][enmPerson.date_of_death.ToString().ToUpper()]);
                                        }
                                    }
                                }
								
								//Ticket - 69718
                                if (lintAdjustmentStartYear > DateTime.Now.Year - 1)
                                    return;

                                for (int i = lintAdjustmentStartYear; i <= DateTime.Now.Year - 1; i++)
                                {

                                    DateTime ldtPlanYrEndDate = busGlobalFunctions.GetLastDateOfComputationYear(i);

                                    //PIR 894
                                    //if (lblnPopupToLife && i > ldtJointAnnuitantDOD.Year)
                                    //{
                                    //    istrBenefitOptionValue = busConstant.LIFE;
                                    //}

                                    //PIR - 930
                                    if (lblnRetiredAfterMDAge && !lblnRetirementYearSplit && lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.Where(
                                                                        item => item.icdoBenefitCalculationYearlyDetail.plan_year == i && i > lintAdjustmentStartYear).Count() > 1)
                                    {
                                        lbusBenefitCalculationYearlyDetail = lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.Where(
                                                                        item => item.icdoBenefitCalculationYearlyDetail.plan_year == i && i > lintAdjustmentStartYear).OrderBy(t => t.icdoBenefitCalculationYearlyDetail.benefit_calculation_yearly_detail_id).FirstOrDefault();
                                        lblnRetirementYearSplit = true;
                                    }
                                    else if (lblnRetiredAfterMDAge && lblnRetirementYearSplit && lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.Where(
                                                                        item => item.icdoBenefitCalculationYearlyDetail.plan_year == i).Count() > 1)
                                    {
                                        lbusBenefitCalculationYearlyDetail = lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.Where(
                                                                        item => item.icdoBenefitCalculationYearlyDetail.plan_year == i).OrderByDescending(t => t.icdoBenefitCalculationYearlyDetail.benefit_calculation_yearly_detail_id).FirstOrDefault();
                                        lblnRetirementYearSplit = false;
                                        ablnReEmployed = true;
                                    }
                                    else
                                    {
                                        lbusBenefitCalculationYearlyDetail = lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.Where(
                                                                           item => item.icdoBenefitCalculationYearlyDetail.plan_year == i).LastOrDefault();
                                    }

                                    if (lblnRetiredAfterMDAge && abusBenefitCalculationRetirement.icdoBenefitCalculationHeader.retirement_date.Year == i
                                        && lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.Where(
                                                                        item => item.icdoBenefitCalculationYearlyDetail.plan_year == i && i > lintAdjustmentStartYear
                                                                        && abusBenefitCalculationRetirement.icdoBenefitCalculationHeader.retirement_date.Year == i).Count() < 2)
                                        lblnRetirementYearSplit = true;

                                    lintSuspendibleMonths = 0;
                                    lintSuspendibleMonthsAfterIncrease = 0;

                                    if (lbusBenefitCalculationYearlyDetail != null)
                                    {
                                        if (counter == 0)
                                        {
                                            //PIR RID 74489 Added check for Ten year certain.
                                            if (istrBenefitOptionValue == busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY)
                                            {
                                                int lintReferenceNumber = 10 - (lintParticipantAgeAtMD - lintParticipantAgeAtRetirementMD);
                                                ldecBenefitOptionFactorStartingPoint = ibusCalculation.GetFactor(abusBenefitCalculationRetirement, istrBenefitOptionValue, lintSurvivorAgeatMinDist, lintParticipantAgeAtMD, lintReferenceNumber, busConstant.BOOL_TRUE);
                                            }
                                            else
                                            {
                                                ldecBenefitOptionFactorStartingPoint = ibusCalculation.GetFactor(abusBenefitCalculationRetirement, istrBenefitOptionValue, lintSurvivorAgeatMinDist, lintParticipantAgeAtMD);
                                            }
                                            ldecLifeBenefitOptionFactorStartingPoint = ibusCalculation.GetFactor(abusBenefitCalculationRetirement, busConstant.LIFE, lintSurvivorAgeatMinDist, lintParticipantAgeAtMD);
                                        }
                                        //PIR - 930
                                        if (lblnRetiredAfterMDAge && lblnRetirementYearSplit)
                                        {
                                            lintSurvivorAgeatMinDist = Convert.ToInt32(Math.Floor(busGlobalFunctions.CalculatePersonAge(lbusSurvivourInfo.icdoPerson.idtDateofBirth, abusBenefitCalculationRetirement.icdoBenefitCalculationHeader.retirement_date)));
                                        }
                                        else
                                        {
                                            lintSurvivorAgeatMinDist = Convert.ToInt32(Math.Floor(busGlobalFunctions.CalculatePersonAge(lbusSurvivourInfo.icdoPerson.idtDateofBirth, new DateTime(i + 1, 01, 01))));
                                        }
                                        //PIR RID 73053 change condition i >= Year of MD instead of just check > 
                                        //PROD PIR 275 -- Getting factors for 10LA which depends on remaining term certain years
                                        if (i >= abusBenefitCalculationRetirement.ibusBenefitApplication.icdoBenefitApplication.min_distribution_date.Year && istrBenefitOptionValue == busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY)
                                        {
                                            //int lintReferenceNumber = (abusPayeeAccount.icdoPayeeAccount.term_certain_end_date.Year - i);
                                            //PIR RID 73053 Fixed order of operation to calculate Reference number.
                                            int lintReferenceNumber = 10 - (Convert.ToInt32(Math.Floor(lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.age)) - lintParticipantAgeAtRetirementMD);
                                            ldecBenefitOptionFactor = ibusCalculation.GetFactor(abusBenefitCalculationRetirement, istrBenefitOptionValue, lintSurvivorAgeatMinDist, Convert.ToInt32(Math.Floor(lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.age)), lintReferenceNumber, busConstant.BOOL_TRUE);
                                        }
                                        else
                                        {
                                            ldecBenefitOptionFactor = ibusCalculation.GetFactor(abusBenefitCalculationRetirement, istrBenefitOptionValue, lintSurvivorAgeatMinDist, Convert.ToInt32(Math.Floor(lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.age)));
                                            ldecLifeBenefitOptionFactor = ibusCalculation.GetFactor(abusBenefitCalculationRetirement, istrLifeBenefitOptionValue, lintSurvivorAgeatMinDist, Convert.ToInt32(Math.Floor(lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.age))); //PIR 894
                                        }


                                        #region Set Accrual Flag

                                        bool lblnAccrualFlag = false;

                                        decimal ldecVestedHours = lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.vested_hours;

                                        int lintQualifiedYearCount = lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.qualified_years_count;

                                        //PIR - 930
                                        if ((!ablnReEmployed && (ldecVestedHours >= 400 || lintQualifiedYearCount > 20)) || (ablnReEmployed && ldecVestedHours >= 870))
                                        {
                                            lblnAccrualFlag = true;
                                        }

                                        #endregion


                                        #region Calculate Active Retiree Increase

                                        ldecActiveRetireeInc = decimal.Zero;
                                        ldtActiveIncreaseEffectiveDate = DateTime.MinValue;
                                        ldtMminimumDistributionEffectiveDate = DateTime.MinValue;
                                        //PIR - 930
                                        //10 Percent RMD
                                        //Ticket#120964
                                        #region Old commented code
                                        /*
                                        if (!ablnReEmployed
                                             && lclbcdoPlanBenefitRate.Where(item => (item.plan_year == i && (item.minimum_distribution_effective_date == DateTime.MinValue && item.rate_type_value == busConstant.BenefitCalculation.PLAN_B && item.increase_percentage == 0)
                                                                                    || (item.minimum_distribution_effective_date != DateTime.MinValue
                                                                                        && (
                                                                                            (((!lblnRetiredAfterMDAge && lintAdjustmentStartYear < i) || (lblnRetiredAfterMDAge && abusBenefitCalculationRetirement.icdoBenefitCalculationHeader.retirement_date >= item.minimum_distribution_effective_date)) && item.minimum_distribution_effective_date.Year == i)
                                                                                            ||
                                                                                            (lblnRetiredAfterMDAge && abusBenefitCalculationRetirement.icdoBenefitCalculationHeader.retirement_date < item.minimum_distribution_effective_date)
                                                                                           )
                                                                                        && item.rate_type_value == busConstant.BenefitCalculation.PLAN_B && item.increase_percentage > 0
                                                                                        ))
                                                                            ).Count() > 0)

                                        {

                                            cdoPlanBenefitRate lcdoPlanBenefitRate =
                                                lclbcdoPlanBenefitRate.Where(item => (item.plan_year == i && (item.minimum_distribution_effective_date == DateTime.MinValue && item.rate_type_value == busConstant.BenefitCalculation.PLAN_B && item.increase_percentage == 0)
                                                                                    || (item.minimum_distribution_effective_date != DateTime.MinValue
                                                                                        && (
                                                                                            (((!lblnRetiredAfterMDAge && lintAdjustmentStartYear < i) || (lblnRetiredAfterMDAge && abusBenefitCalculationRetirement.icdoBenefitCalculationHeader.retirement_date >= item.minimum_distribution_effective_date)) && item.minimum_distribution_effective_date.Year == i)
                                                                                            ||
                                                                                            (lblnRetiredAfterMDAge && abusBenefitCalculationRetirement.icdoBenefitCalculationHeader.retirement_date < item.minimum_distribution_effective_date)
                                                                                           )
                                                                                        && item.rate_type_value == busConstant.BenefitCalculation.PLAN_B && item.increase_percentage > 0
                                                                                        ))
                                                                            ).FirstOrDefault();


                                            if (lintQualifiedYearCount > Convert.ToInt32(busConstant.BenefitCalculation.QUALIFIED_YEARS_20))
                                            {

                                                ldecActiveRetireeInc = lclbcdoPlanBenefitRate.Where(item => (item.plan_year == i && item.qualified_year_limit_value == busConstant.BenefitCalculation.QUALIFIED_YEARS_200 && ((item.minimum_distribution_effective_date == DateTime.MinValue && item.rate_type_value == busConstant.BenefitCalculation.PLAN_B && item.increase_percentage == 0)
                                                                                     || (item.minimum_distribution_effective_date != DateTime.MinValue
                                                                                         && (
                                                                                             (((!lblnRetiredAfterMDAge && lintAdjustmentStartYear < i) || (lblnRetiredAfterMDAge && abusBenefitCalculationRetirement.icdoBenefitCalculationHeader.retirement_date >= item.minimum_distribution_effective_date)) && item.minimum_distribution_effective_date.Year == i)
                                                                                             ||
                                                                                             (lblnRetiredAfterMDAge && abusBenefitCalculationRetirement.icdoBenefitCalculationHeader.retirement_date < item.minimum_distribution_effective_date)
                                                                                            )
                                                                                         && item.rate_type_value == busConstant.BenefitCalculation.PLAN_B && item.increase_percentage > 0
                                                                                         )))
                                                                             ).FirstOrDefault().increase_percentage;

                                                ldtActiveIncreaseEffectiveDate = lclbcdoPlanBenefitRate.Where(item => (item.plan_year == i && item.qualified_year_limit_value == busConstant.BenefitCalculation.QUALIFIED_YEARS_200 && ((item.minimum_distribution_effective_date == DateTime.MinValue && item.rate_type_value == busConstant.BenefitCalculation.PLAN_B && item.increase_percentage == 0)
                                                                                         || (item.minimum_distribution_effective_date != DateTime.MinValue
                                                                                             && (
                                                                                                 (((!lblnRetiredAfterMDAge && lintAdjustmentStartYear < i) || (lblnRetiredAfterMDAge && abusBenefitCalculationRetirement.icdoBenefitCalculationHeader.retirement_date >= item.minimum_distribution_effective_date)) && item.minimum_distribution_effective_date.Year == i)
                                                                                                 ||
                                                                                                 (lblnRetiredAfterMDAge && abusBenefitCalculationRetirement.icdoBenefitCalculationHeader.retirement_date < item.minimum_distribution_effective_date)
                                                                                                )
                                                                                             && item.rate_type_value == busConstant.BenefitCalculation.PLAN_B && item.increase_percentage > 0
                                                                                             )))
                                                                                 ).FirstOrDefault().effective_date;


                                            }
                                            else if (lintQualifiedYearCount > Convert.ToInt32(busConstant.BenefitCalculation.QUALIFIED_YEARS_10) && lintQualifiedYearCount <= Convert.ToInt32(busConstant.BenefitCalculation.QUALIFIED_YEARS_20))
                                            {

                                                ldecActiveRetireeInc = lclbcdoPlanBenefitRate.Where(item => (item.plan_year == i && item.qualified_year_limit_value == busConstant.BenefitCalculation.QUALIFIED_YEARS_20 && ((item.minimum_distribution_effective_date == DateTime.MinValue && item.rate_type_value == busConstant.BenefitCalculation.PLAN_B && item.increase_percentage == 0)
                                                                                     || (item.minimum_distribution_effective_date != DateTime.MinValue
                                                                                         && (
                                                                                             (((!lblnRetiredAfterMDAge && lintAdjustmentStartYear < i) || (lblnRetiredAfterMDAge && abusBenefitCalculationRetirement.icdoBenefitCalculationHeader.retirement_date >= item.minimum_distribution_effective_date)) && item.minimum_distribution_effective_date.Year == i)
                                                                                             ||
                                                                                             (lblnRetiredAfterMDAge && abusBenefitCalculationRetirement.icdoBenefitCalculationHeader.retirement_date < item.minimum_distribution_effective_date)
                                                                                            )
                                                                                         && item.rate_type_value == busConstant.BenefitCalculation.PLAN_B && item.increase_percentage > 0
                                                                                         )))
                                                                             ).FirstOrDefault().increase_percentage;

                                                ldtActiveIncreaseEffectiveDate = lclbcdoPlanBenefitRate.Where(item => (item.plan_year == i && item.qualified_year_limit_value == busConstant.BenefitCalculation.QUALIFIED_YEARS_20 && ((item.minimum_distribution_effective_date == DateTime.MinValue && item.rate_type_value == busConstant.BenefitCalculation.PLAN_B && item.increase_percentage == 0)
                                                                                        || (item.minimum_distribution_effective_date != DateTime.MinValue
                                                                                            && (
                                                                                                (((!lblnRetiredAfterMDAge && lintAdjustmentStartYear < i) || (lblnRetiredAfterMDAge && abusBenefitCalculationRetirement.icdoBenefitCalculationHeader.retirement_date >= item.minimum_distribution_effective_date)) && item.minimum_distribution_effective_date.Year == i)
                                                                                                ||
                                                                                                (lblnRetiredAfterMDAge && abusBenefitCalculationRetirement.icdoBenefitCalculationHeader.retirement_date < item.minimum_distribution_effective_date)
                                                                                               )
                                                                                            && item.rate_type_value == busConstant.BenefitCalculation.PLAN_B && item.increase_percentage > 0
                                                                                            )))
                                                                                ).FirstOrDefault().effective_date;

                                            }
                                            else if (lintQualifiedYearCount <= Convert.ToInt32(busConstant.BenefitCalculation.QUALIFIED_YEARS_10))
                                            {
                                                ldecActiveRetireeInc = lclbcdoPlanBenefitRate.Where(item => (item.plan_year == i && item.qualified_year_limit_value == busConstant.BenefitCalculation.QUALIFIED_YEARS_10 && ((item.minimum_distribution_effective_date == DateTime.MinValue && item.rate_type_value == busConstant.BenefitCalculation.PLAN_B && item.increase_percentage == 0)
                                                                                    || (item.minimum_distribution_effective_date != DateTime.MinValue
                                                                                        && (
                                                                                            (((!lblnRetiredAfterMDAge && lintAdjustmentStartYear < i) || (lblnRetiredAfterMDAge && abusBenefitCalculationRetirement.icdoBenefitCalculationHeader.retirement_date >= item.minimum_distribution_effective_date)) && item.minimum_distribution_effective_date.Year == i)
                                                                                            ||
                                                                                            (lblnRetiredAfterMDAge && abusBenefitCalculationRetirement.icdoBenefitCalculationHeader.retirement_date < item.minimum_distribution_effective_date)
                                                                                           )
                                                                                        && item.rate_type_value == busConstant.BenefitCalculation.PLAN_B && item.increase_percentage > 0
                                                                                        )))
                                                                            ).FirstOrDefault().increase_percentage;

                                                ldtActiveIncreaseEffectiveDate = lclbcdoPlanBenefitRate.Where(item => (item.plan_year == i && item.qualified_year_limit_value == busConstant.BenefitCalculation.QUALIFIED_YEARS_10 && ((item.minimum_distribution_effective_date == DateTime.MinValue && item.rate_type_value == busConstant.BenefitCalculation.PLAN_B && item.increase_percentage == 0)
                                                                                         || (item.minimum_distribution_effective_date != DateTime.MinValue
                                                                                             && (
                                                                                                 (((!lblnRetiredAfterMDAge && lintAdjustmentStartYear < i) || (lblnRetiredAfterMDAge && abusBenefitCalculationRetirement.icdoBenefitCalculationHeader.retirement_date >= item.minimum_distribution_effective_date)) && item.minimum_distribution_effective_date.Year == i)
                                                                                                 ||
                                                                                                 (lblnRetiredAfterMDAge && abusBenefitCalculationRetirement.icdoBenefitCalculationHeader.retirement_date < item.minimum_distribution_effective_date)
                                                                                                )
                                                                                             && item.rate_type_value == busConstant.BenefitCalculation.PLAN_B && item.increase_percentage > 0
                                                                                             )))
                                                                                 ).FirstOrDefault().effective_date;
                                            }


                                            if (ldecActiveRetireeInc > decimal.Zero && !ablnReEmployed && ldtActiveIncreaseEffectiveDate.Year == i && lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.Where(item => item.icdoBenefitCalculationYearlyDetail.plan_year == i
                                                    && item.icdoBenefitCalculationYearlyDetail.break_years_count >= 2).Count() > 0)
                                            {
                                                ldecActiveRetireeInc = decimal.Zero;
                                                ldtActiveIncreaseEffectiveDate = DateTime.MinValue;
                                            }
                                            else if(ldecActiveRetireeInc > decimal.Zero && !ablnReEmployed && ldtActiveIncreaseEffectiveDate.Year != i)
                                            {
                                                if(lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.Where(item => item.icdoBenefitCalculationYearlyDetail.plan_year >= lcdoPlanBenefitRate.plan_year 
                                                && item.icdoBenefitCalculationYearlyDetail.plan_year <= lcdoPlanBenefitRate.effective_end_year
                                                    && item.icdoBenefitCalculationYearlyDetail.break_years_count >= 2).Count() > 0)
                                                {
                                                    ldecActiveRetireeInc = decimal.Zero;
                                                    ldtActiveIncreaseEffectiveDate = DateTime.MinValue;
                                                }
                                            }

                                                if (lblnRetiredAfterMDAge && abusBenefitCalculationRetirement.icdoBenefitCalculationHeader.retirement_date < ldtActiveIncreaseEffectiveDate)
                                            {
                                                ldecActiveRetireeInc = decimal.Zero;
                                                ldtActiveIncreaseEffectiveDate = DateTime.MinValue;
                                            }
                                        }

                                        */
                                        #endregion

                                        #region old code commented to simplfy and readability
                                        //                                    if (!ablnReEmployed
                                        //&& lclbcdoPlanBenefitRate.Where(item =>
                                        //                    (item.minimum_distribution_effective_date == DateTime.MinValue && item.plan_year == i && item.rate_type_value == busConstant.BenefitCalculation.PLAN_B && item.increase_percentage > 0)
                                        //                    || (
                                        //                            item.minimum_distribution_effective_date != DateTime.MinValue
                                        //                            && (
                                        //                                (((!lblnRetiredAfterMDAge && lintAdjustmentStartYear < i) //MD
                                        //                                  || (lblnRetiredAfterMDAge && abusBenefitCalculationRetirement.icdoBenefitCalculationHeader.retirement_date >= item.minimum_distribution_effective_date)  //MD-RD LATE
                                        //                                   )
                                        //                                 && (item.minimum_distribution_effective_date.Year == i && lintAdjustmentStartYear >= item.plan_year)   //year we have to give increase to MD
                                        //                                )
                                        //                                ||
                                        //                                (lblnRetiredAfterMDAge && abusBenefitCalculationRetirement.icdoBenefitCalculationHeader.retirement_date < item.minimum_distribution_effective_date && item.plan_year == i) //MD-RD LATE
                                        //                               )
                                        //                            && item.rate_type_value == busConstant.BenefitCalculation.PLAN_B && item.increase_percentage > 0
                                        //                    )
                                        //                ).Count() > 0)

                                        //                                    {

                                        //                                        cdoPlanBenefitRate lcdoPlanBenefitRate =
                                        //                                        lclbcdoPlanBenefitRate.Where(item => (item.minimum_distribution_effective_date == DateTime.MinValue && item.plan_year == i && item.rate_type_value == busConstant.BenefitCalculation.PLAN_B && item.increase_percentage > 0)
                                        //                                                            || (item.minimum_distribution_effective_date != DateTime.MinValue
                                        //                                                            && (
                                        //                                                                (((!lblnRetiredAfterMDAge && lintAdjustmentStartYear < i)
                                        //                                                                    || (lblnRetiredAfterMDAge && abusBenefitCalculationRetirement.icdoBenefitCalculationHeader.retirement_date >= item.minimum_distribution_effective_date))
                                        //                                                                    && (item.minimum_distribution_effective_date.Year == i && lintAdjustmentStartYear >= item.plan_year)
                                        //                                                                )
                                        //                                                                ||
                                        //                                                                (lblnRetiredAfterMDAge && abusBenefitCalculationRetirement.icdoBenefitCalculationHeader.retirement_date < item.minimum_distribution_effective_date && item.plan_year == i)
                                        //                                                               )
                                        //                                                            && item.rate_type_value == busConstant.BenefitCalculation.PLAN_B && item.increase_percentage > 0
                                        //                                                            )
                                        //                                                        ).FirstOrDefault();

                                        //                                        if (lintQualifiedYearCount > Convert.ToInt32(busConstant.BenefitCalculation.QUALIFIED_YEARS_20))
                                        //                                        {

                                        //                                            ldecActiveRetireeInc = lclbcdoPlanBenefitRate.Where(item => item.qualified_year_limit_value == busConstant.BenefitCalculation.QUALIFIED_YEARS_200 && ((item.minimum_distribution_effective_date == DateTime.MinValue && item.plan_year == i && item.rate_type_value == busConstant.BenefitCalculation.PLAN_B && item.increase_percentage > 0)
                                        //                                                                || (item.minimum_distribution_effective_date != DateTime.MinValue
                                        //                                                                && (
                                        //                                                                    (((!lblnRetiredAfterMDAge && lintAdjustmentStartYear < i) || (lblnRetiredAfterMDAge && abusBenefitCalculationRetirement.icdoBenefitCalculationHeader.retirement_date >= item.minimum_distribution_effective_date)) && (item.minimum_distribution_effective_date.Year == i && lintAdjustmentStartYear >= item.plan_year))
                                        //                                                                    ||
                                        //                                                                    (lblnRetiredAfterMDAge && abusBenefitCalculationRetirement.icdoBenefitCalculationHeader.retirement_date < item.minimum_distribution_effective_date && item.plan_year == i)
                                        //                                                                   )
                                        //                                                                && item.rate_type_value == busConstant.BenefitCalculation.PLAN_B && item.increase_percentage > 0
                                        //                                                                ))
                                        //                                                            ).FirstOrDefault().increase_percentage;

                                        //                                            ldtActiveIncreaseEffectiveDate = lclbcdoPlanBenefitRate.Where(item => item.qualified_year_limit_value == busConstant.BenefitCalculation.QUALIFIED_YEARS_200 && ((item.minimum_distribution_effective_date == DateTime.MinValue && item.plan_year == i && item.rate_type_value == busConstant.BenefitCalculation.PLAN_B && item.increase_percentage > 0)
                                        //                                                                || (item.minimum_distribution_effective_date != DateTime.MinValue
                                        //                                                                    && (
                                        //                                                                    (((!lblnRetiredAfterMDAge && lintAdjustmentStartYear < i) || (lblnRetiredAfterMDAge && abusBenefitCalculationRetirement.icdoBenefitCalculationHeader.retirement_date >= item.minimum_distribution_effective_date)) && (item.minimum_distribution_effective_date.Year == i && lintAdjustmentStartYear >= item.plan_year))
                                        //                                                                    ||
                                        //                                                                    (lblnRetiredAfterMDAge && abusBenefitCalculationRetirement.icdoBenefitCalculationHeader.retirement_date < item.minimum_distribution_effective_date && item.plan_year == i)
                                        //                                                                       )
                                        //                                                                    && item.rate_type_value == busConstant.BenefitCalculation.PLAN_B && item.increase_percentage > 0
                                        //                                                                    ))
                                        //                                                            ).FirstOrDefault().effective_date;


                                        //                                        }
                                        //                                        else if (lintQualifiedYearCount > Convert.ToInt32(busConstant.BenefitCalculation.QUALIFIED_YEARS_10) && lintQualifiedYearCount <= Convert.ToInt32(busConstant.BenefitCalculation.QUALIFIED_YEARS_20))
                                        //                                        {

                                        //                                            ldecActiveRetireeInc = lclbcdoPlanBenefitRate.Where(item => item.qualified_year_limit_value == busConstant.BenefitCalculation.QUALIFIED_YEARS_20 && ((item.minimum_distribution_effective_date == DateTime.MinValue && item.plan_year == i && item.rate_type_value == busConstant.BenefitCalculation.PLAN_B && item.increase_percentage > 0)
                                        //                                                                || (item.minimum_distribution_effective_date != DateTime.MinValue
                                        //                                                                && (
                                        //                                                                    (((!lblnRetiredAfterMDAge && lintAdjustmentStartYear < i) || (lblnRetiredAfterMDAge && abusBenefitCalculationRetirement.icdoBenefitCalculationHeader.retirement_date >= item.minimum_distribution_effective_date)) && (item.minimum_distribution_effective_date.Year == i && lintAdjustmentStartYear >= item.plan_year))
                                        //                                                                    ||
                                        //                                                                    (lblnRetiredAfterMDAge && abusBenefitCalculationRetirement.icdoBenefitCalculationHeader.retirement_date < item.minimum_distribution_effective_date && item.plan_year == i)
                                        //                                                                   )
                                        //                                                                && item.rate_type_value == busConstant.BenefitCalculation.PLAN_B && item.increase_percentage > 0
                                        //                                                                ))
                                        //                                                            ).FirstOrDefault().increase_percentage;

                                        //                                            ldtActiveIncreaseEffectiveDate = lclbcdoPlanBenefitRate.Where(item => item.qualified_year_limit_value == busConstant.BenefitCalculation.QUALIFIED_YEARS_20 && ((item.minimum_distribution_effective_date == DateTime.MinValue && item.plan_year == i && item.rate_type_value == busConstant.BenefitCalculation.PLAN_B && item.increase_percentage > 0)
                                        //                                                                || (item.minimum_distribution_effective_date != DateTime.MinValue
                                        //                                                                    && (
                                        //                                                                    (((!lblnRetiredAfterMDAge && lintAdjustmentStartYear < i) || (lblnRetiredAfterMDAge && abusBenefitCalculationRetirement.icdoBenefitCalculationHeader.retirement_date >= item.minimum_distribution_effective_date)) && (item.minimum_distribution_effective_date.Year == i && lintAdjustmentStartYear >= item.plan_year))
                                        //                                                                    ||
                                        //                                                                    (lblnRetiredAfterMDAge && abusBenefitCalculationRetirement.icdoBenefitCalculationHeader.retirement_date < item.minimum_distribution_effective_date && item.plan_year == i)
                                        //                                                                       )
                                        //                                                                    && item.rate_type_value == busConstant.BenefitCalculation.PLAN_B && item.increase_percentage > 0
                                        //                                                                    ))
                                        //                                                            ).FirstOrDefault().effective_date;

                                        //                                        }
                                        //                                        else if (lintQualifiedYearCount <= Convert.ToInt32(busConstant.BenefitCalculation.QUALIFIED_YEARS_10))
                                        //                                        {
                                        //                                            ldecActiveRetireeInc = lclbcdoPlanBenefitRate.Where(item => item.qualified_year_limit_value == busConstant.BenefitCalculation.QUALIFIED_YEARS_10 && ((item.minimum_distribution_effective_date == DateTime.MinValue && item.plan_year == i && item.rate_type_value == busConstant.BenefitCalculation.PLAN_B && item.increase_percentage > 0)
                                        //                                                                || (item.minimum_distribution_effective_date != DateTime.MinValue
                                        //                                                                && (
                                        //                                                                    (((!lblnRetiredAfterMDAge && lintAdjustmentStartYear < i) || (lblnRetiredAfterMDAge && abusBenefitCalculationRetirement.icdoBenefitCalculationHeader.retirement_date >= item.minimum_distribution_effective_date)) && (item.minimum_distribution_effective_date.Year == i && lintAdjustmentStartYear >= item.plan_year))
                                        //                                                                    ||
                                        //                                                                    (lblnRetiredAfterMDAge && abusBenefitCalculationRetirement.icdoBenefitCalculationHeader.retirement_date < item.minimum_distribution_effective_date && item.plan_year == i)
                                        //                                                                   )
                                        //                                                                && item.rate_type_value == busConstant.BenefitCalculation.PLAN_B && item.increase_percentage > 0
                                        //                                                                ))
                                        //                                                            ).FirstOrDefault().increase_percentage;

                                        //                                            ldtActiveIncreaseEffectiveDate = lclbcdoPlanBenefitRate.Where(item => item.qualified_year_limit_value == busConstant.BenefitCalculation.QUALIFIED_YEARS_10 && ((item.minimum_distribution_effective_date == DateTime.MinValue && item.plan_year == i && item.rate_type_value == busConstant.BenefitCalculation.PLAN_B && item.increase_percentage > 0)
                                        //                                                                || (item.minimum_distribution_effective_date != DateTime.MinValue
                                        //                                                                    && (
                                        //                                                                    (((!lblnRetiredAfterMDAge && lintAdjustmentStartYear < i) || (lblnRetiredAfterMDAge && abusBenefitCalculationRetirement.icdoBenefitCalculationHeader.retirement_date >= item.minimum_distribution_effective_date)) && (item.minimum_distribution_effective_date.Year == i && lintAdjustmentStartYear >= item.plan_year))
                                        //                                                                    ||
                                        //                                                                    (lblnRetiredAfterMDAge && abusBenefitCalculationRetirement.icdoBenefitCalculationHeader.retirement_date < item.minimum_distribution_effective_date && item.plan_year == i)
                                        //                                                                       )
                                        //                                                                    && item.rate_type_value == busConstant.BenefitCalculation.PLAN_B && item.increase_percentage > 0
                                        //                                                                    ))
                                        //                                                            ).FirstOrDefault().effective_date;
                                        //                                        }


                                        //                                        if (ldecActiveRetireeInc > decimal.Zero && !ablnReEmployed && ldtActiveIncreaseEffectiveDate.Year == i && lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.Where(item => item.icdoBenefitCalculationYearlyDetail.plan_year == i
                                        //                                            && item.icdoBenefitCalculationYearlyDetail.break_years_count >= 2).Count() > 0)
                                        //                                        {
                                        //                                            ldecActiveRetireeInc = decimal.Zero;
                                        //                                            ldtActiveIncreaseEffectiveDate = DateTime.MinValue;
                                        //                                        }
                                        //                                        else if (ldecActiveRetireeInc > decimal.Zero && !ablnReEmployed && ldtActiveIncreaseEffectiveDate.Year != i)
                                        //                                        {
                                        //                                            if (lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.Where(item => item.icdoBenefitCalculationYearlyDetail.plan_year >= lcdoPlanBenefitRate.plan_year
                                        //                                             && item.icdoBenefitCalculationYearlyDetail.plan_year <= lcdoPlanBenefitRate.effective_end_year
                                        //                                                 && item.icdoBenefitCalculationYearlyDetail.break_years_count >= 2).Count() > 0)
                                        //                                            {
                                        //                                                ldecActiveRetireeInc = decimal.Zero;
                                        //                                                ldtActiveIncreaseEffectiveDate = DateTime.MinValue;
                                        //                                            }
                                        //                                        }

                                        //                                        if (lblnRetiredAfterMDAge && abusBenefitCalculationRetirement.icdoBenefitCalculationHeader.retirement_date < ldtActiveIncreaseEffectiveDate)
                                        //                                        {
                                        //                                            ldecActiveRetireeInc = decimal.Zero;
                                        //                                            ldtActiveIncreaseEffectiveDate = DateTime.MinValue;
                                        //                                        }
                                        //                                    }
                                        #endregion

                                        //Simplified the code for Active increase rate selection
                                        string lstrQualifiedYearLimitValue = "10";

                                        if (lintQualifiedYearCount > Convert.ToInt32(busConstant.BenefitCalculation.QUALIFIED_YEARS_20))
                                            lstrQualifiedYearLimitValue = busConstant.BenefitCalculation.QUALIFIED_YEARS_200;
                                        else if (lintQualifiedYearCount > Convert.ToInt32(busConstant.BenefitCalculation.QUALIFIED_YEARS_10) && lintQualifiedYearCount <= Convert.ToInt32(busConstant.BenefitCalculation.QUALIFIED_YEARS_20))
                                            lstrQualifiedYearLimitValue = busConstant.BenefitCalculation.QUALIFIED_YEARS_20;
                                        else if (lintQualifiedYearCount <= Convert.ToInt32(busConstant.BenefitCalculation.QUALIFIED_YEARS_10))
                                            lstrQualifiedYearLimitValue = busConstant.BenefitCalculation.QUALIFIED_YEARS_10;

                                        cdoPlanBenefitRate lcdoPlanBenefitRate =
                                        lclbcdoPlanBenefitRate.Where(item => item.qualified_year_limit_value == lstrQualifiedYearLimitValue && ((item.minimum_distribution_effective_date == DateTime.MinValue && item.plan_year == i && item.rate_type_value == busConstant.BenefitCalculation.PLAN_B && item.increase_percentage > 0)
                                                                || (item.minimum_distribution_effective_date != DateTime.MinValue
                                                                && (
                                                                    (((!lblnRetiredAfterMDAge && lintAdjustmentStartYear < i) || (lblnRetiredAfterMDAge && abusBenefitCalculationRetirement.icdoBenefitCalculationHeader.retirement_date >= item.minimum_distribution_effective_date)) && (item.minimum_distribution_effective_date.Year == i /*&& lintAdjustmentStartYear >= item.plan_year*/ && lintAdjustmentStartYear <= item.effective_end_year))
                                                                    ||
                                                                    (lblnRetiredAfterMDAge && abusBenefitCalculationRetirement.icdoBenefitCalculationHeader.retirement_date < item.minimum_distribution_effective_date && item.plan_year == i)
                                                                    )
                                                                && item.rate_type_value == busConstant.BenefitCalculation.PLAN_B && item.increase_percentage > 0
                                                                ))
                                                            ).FirstOrDefault();

                                        if (!ablnReEmployed && lcdoPlanBenefitRate != null)
                                        {
                                            ldecActiveRetireeInc = lcdoPlanBenefitRate.increase_percentage;
                                            ldtActiveIncreaseEffectiveDate = lcdoPlanBenefitRate.effective_date;
                                            ldtMminimumDistributionEffectiveDate = lcdoPlanBenefitRate.minimum_distribution_effective_date;

                                            if (ldecActiveRetireeInc > decimal.Zero && !ablnReEmployed && ldtActiveIncreaseEffectiveDate.Year == i && lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.Where(item => item.icdoBenefitCalculationYearlyDetail.plan_year == i
                                                && item.icdoBenefitCalculationYearlyDetail.break_years_count >= 2).Count() > 0)
                                            {
                                                ldecActiveRetireeInc = decimal.Zero;
                                                ldtActiveIncreaseEffectiveDate = DateTime.MinValue;
                                            }
                                            else if (ldecActiveRetireeInc > decimal.Zero && !ablnReEmployed && ldtActiveIncreaseEffectiveDate.Year != i)
                                            {
                                                if (lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.Where(item => item.icdoBenefitCalculationYearlyDetail.plan_year >= lcdoPlanBenefitRate.plan_year
                                                 && item.icdoBenefitCalculationYearlyDetail.plan_year <= lcdoPlanBenefitRate.effective_end_year
                                                     && item.icdoBenefitCalculationYearlyDetail.break_years_count >= 2).Count() > 0)
                                                {
                                                    ldecActiveRetireeInc = decimal.Zero;
                                                    ldtActiveIncreaseEffectiveDate = DateTime.MinValue;
                                                }
                                            }

                                            if (lblnRetiredAfterMDAge && abusBenefitCalculationRetirement.icdoBenefitCalculationHeader.retirement_date < ldtActiveIncreaseEffectiveDate)
                                            {
                                                ldecActiveRetireeInc = decimal.Zero;
                                                ldtActiveIncreaseEffectiveDate = DateTime.MinValue;
                                            }

                                        }
                                        //End of new Simplified the code for Active increase rate selection

                                        #endregion

                                        #region Calculate suspendible Month count

                                        //10 Percent
                                        if (ldtActiveIncreaseEffectiveDate != DateTime.MinValue && ldecActiveRetireeInc > 0 && ldtActiveIncreaseEffectiveDate.Year == i) //RMD 10 Percent
                                        {

                                            DateTime ldtStartDate = new DateTime();

                                            if (counter == 0)
                                                ldtStartDate = busGlobalFunctions.GetFirstPayrollDayOfMonth(i, abusBenefitCalculationRetirement.ibusBenefitApplication.icdoBenefitApplication.min_distribution_date.Month);
                                            else
                                                ldtStartDate = busGlobalFunctions.GetFirstPayrollDayOfMonth(i, 1);


                                            DateTime ldtEndDate = new DateTime();

                                            if (lblnRetiredAfterMDAge && lblnRetirementYearSplit)
                                            {
                                                if (abusBenefitCalculationRetirement.icdoBenefitCalculationHeader.retirement_date.Date > ldtActiveIncreaseEffectiveDate.Date)
                                                {
                                                    ldtEndDate = busGlobalFunctions.GetLastPayrollDayOfMonth(i, ldtActiveIncreaseEffectiveDate.AddMonths(-1).Month);

                                                    lintSuspendibleMonths = ((busGlobalFunctions.GetPayrollMonth(ldtEndDate) - busGlobalFunctions.GetPayrollMonth(ldtStartDate)) + 1) - ibusCalculation.GetNonSuspendibleMonths(
                                                           abusBenefitCalculationRetirement.ibusPerson.icdoPerson.istrSSNNonEncrypted.ToString(), abusBenefitCalculationRetirement.ibusPerson, i, busConstant.MPIPP_PLAN_ID, null, ldtStartDate, ldtEndDate);

                                                    ldtStartDate = busGlobalFunctions.GetFirstPayrollDayOfMonth(i, ldtActiveIncreaseEffectiveDate.Month);
                                                    ldtEndDate = busGlobalFunctions.GetLastPayrollDayOfMonth(i, abusBenefitCalculationRetirement.icdoBenefitCalculationHeader.retirement_date.AddMonths(-1).Month);

                                                    lintSuspendibleMonthsAfterIncrease = ((busGlobalFunctions.GetPayrollMonth(ldtEndDate) - busGlobalFunctions.GetPayrollMonth(ldtStartDate)) + 1) - ibusCalculation.GetNonSuspendibleMonths(
                                                           abusBenefitCalculationRetirement.ibusPerson.icdoPerson.istrSSNNonEncrypted.ToString(), abusBenefitCalculationRetirement.ibusPerson, i, busConstant.MPIPP_PLAN_ID, null, ldtStartDate, ldtEndDate);

                                                    //10 Percent - RMD
                                                    //Business do not require Suspendible Months After Increase.So we are keeping the split logic but adding Suspendible Months After Increase
                                                    //into Suspendible Months before Increase.
                                                    lintSuspendibleMonths += lintSuspendibleMonthsAfterIncrease;
                                                    lintSuspendibleMonthsAfterIncrease = 0;
                                                }
                                                else
                                                {

                                                    ldtEndDate = busGlobalFunctions.GetLastPayrollDayOfMonth(i, abusBenefitCalculationRetirement.icdoBenefitCalculationHeader.retirement_date.AddMonths(-1).Month);

                                                    lintSuspendibleMonths = ((busGlobalFunctions.GetPayrollMonth(ldtEndDate) - busGlobalFunctions.GetPayrollMonth(ldtStartDate)) + 1) - ibusCalculation.GetNonSuspendibleMonths(
                                                           abusBenefitCalculationRetirement.ibusPerson.icdoPerson.istrSSNNonEncrypted.ToString(), abusBenefitCalculationRetirement.ibusPerson, i, busConstant.MPIPP_PLAN_ID, null, ldtStartDate, ldtEndDate);

                                                }

                                            }
                                            else if (lblnRetiredAfterMDAge && !lblnRetirementYearSplit &&
                                                lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.Where(
                                                                        item => item.icdoBenefitCalculationYearlyDetail.plan_year == i && i > lintAdjustmentStartYear).Count() > 1)

                                            {
                                                if (abusBenefitCalculationRetirement.icdoBenefitCalculationHeader.retirement_date.Date < ldtActiveIncreaseEffectiveDate.Date)
                                                {
                                                    ldtStartDate = busGlobalFunctions.GetFirstPayrollDayOfMonth(i, abusBenefitCalculationRetirement.icdoBenefitCalculationHeader.retirement_date.Month);
                                                    ldtEndDate = busGlobalFunctions.GetLastPayrollDayOfMonth(i, ldtActiveIncreaseEffectiveDate.AddMonths(-1).Month);


                                                    lintSuspendibleMonths = ((busGlobalFunctions.GetPayrollMonth(ldtEndDate) - busGlobalFunctions.GetPayrollMonth(ldtStartDate)) + 1) - ibusCalculation.GetNonSuspendibleMonths(
                                                           abusBenefitCalculationRetirement.ibusPerson.icdoPerson.istrSSNNonEncrypted.ToString(), abusBenefitCalculationRetirement.ibusPerson, i, busConstant.MPIPP_PLAN_ID, null, ldtStartDate, ldtEndDate);

                                                    ldtStartDate = busGlobalFunctions.GetFirstPayrollDayOfMonth(i, ldtActiveIncreaseEffectiveDate.Month);
                                                    ldtEndDate = busGlobalFunctions.GetLastPayrollDayOfMonth(i, 12);

                                                    lintSuspendibleMonthsAfterIncrease = ((busGlobalFunctions.GetPayrollMonth(ldtEndDate) - busGlobalFunctions.GetPayrollMonth(ldtStartDate)) + 1) - ibusCalculation.GetNonSuspendibleMonths(
                                                           abusBenefitCalculationRetirement.ibusPerson.icdoPerson.istrSSNNonEncrypted.ToString(), abusBenefitCalculationRetirement.ibusPerson, i, busConstant.MPIPP_PLAN_ID, null, ldtStartDate, ldtEndDate);

                                                    //10 Percent - RMD
                                                    //Business do not require Suspendible Months After Increase.So we are keeping the split logic but adding Suspendible Months After Increase
                                                    //into Suspendible Months before Increase.
                                                    lintSuspendibleMonths += lintSuspendibleMonthsAfterIncrease;
                                                    lintSuspendibleMonthsAfterIncrease = 0;
                                                }
                                                else
                                                {
                                                    ldtStartDate = busGlobalFunctions.GetFirstPayrollDayOfMonth(i, abusBenefitCalculationRetirement.icdoBenefitCalculationHeader.retirement_date.Month);
                                                    ldtEndDate = busGlobalFunctions.GetLastPayrollDayOfMonth(i, 12);

                                                    lintSuspendibleMonthsAfterIncrease = ((busGlobalFunctions.GetPayrollMonth(ldtEndDate) - busGlobalFunctions.GetPayrollMonth(ldtStartDate)) + 1) - ibusCalculation.GetNonSuspendibleMonths(
                                                           abusBenefitCalculationRetirement.ibusPerson.icdoPerson.istrSSNNonEncrypted.ToString(), abusBenefitCalculationRetirement.ibusPerson, i, busConstant.MPIPP_PLAN_ID, null, ldtStartDate, ldtEndDate);

                                                    //10 Percent - RMD
                                                    //Business do not require Suspendible Months After Increase.So we are keeping the split logic but adding Suspendible Months After Increase
                                                    //into Suspendible Months before Increase.
                                                    lintSuspendibleMonths += lintSuspendibleMonthsAfterIncrease;
                                                    lintSuspendibleMonthsAfterIncrease = 0;
                                                }
                                            }
                                            else
                                            {

                                                ldtEndDate = busGlobalFunctions.GetLastPayrollDayOfMonth(i, ldtActiveIncreaseEffectiveDate.AddMonths(-1).Month);

                                                lintSuspendibleMonths = ((busGlobalFunctions.GetPayrollMonth(ldtEndDate) - busGlobalFunctions.GetPayrollMonth(ldtStartDate)) + 1) - ibusCalculation.GetNonSuspendibleMonths(
                                                       abusBenefitCalculationRetirement.ibusPerson.icdoPerson.istrSSNNonEncrypted.ToString(), abusBenefitCalculationRetirement.ibusPerson, i, busConstant.MPIPP_PLAN_ID, null, ldtStartDate, ldtEndDate);

                                                ldtStartDate = busGlobalFunctions.GetFirstPayrollDayOfMonth(i, ldtActiveIncreaseEffectiveDate.Month);
                                                ldtEndDate = busGlobalFunctions.GetLastPayrollDayOfMonth(i, 12);

                                                lintSuspendibleMonthsAfterIncrease = ((busGlobalFunctions.GetPayrollMonth(ldtEndDate) - busGlobalFunctions.GetPayrollMonth(ldtStartDate)) + 1) - ibusCalculation.GetNonSuspendibleMonths(
                                                       abusBenefitCalculationRetirement.ibusPerson.icdoPerson.istrSSNNonEncrypted.ToString(), abusBenefitCalculationRetirement.ibusPerson, i, busConstant.MPIPP_PLAN_ID, null, ldtStartDate, ldtEndDate);

                                                //10 Percent - RMD
                                                //Business do not require Suspendible Months After Increase.So we are keeping the split logic but adding Suspendible Months After Increase
                                                //into Suspendible Months before Increase.
                                                lintSuspendibleMonths += lintSuspendibleMonthsAfterIncrease;
                                                lintSuspendibleMonthsAfterIncrease = 0;
                                            }
                                        }
                                        else
                                        {
                                            //PIR - 930
                                            DateTime ldtStartDate = DateTime.MinValue;
                                            DateTime ldtEndDate = DateTime.MinValue;


                                            if (lblnRetiredAfterMDAge && lblnRetirementYearSplit)
                                            {

                                                ldtStartDate = busGlobalFunctions.GetFirstPayrollDayOfMonth(i, 1);
                                                ldtEndDate = busGlobalFunctions.GetLastPayrollDayOfMonth(i, abusBenefitCalculationRetirement.icdoBenefitCalculationHeader.retirement_date.AddMonths(-1).Month);
                                            }
                                            else if (lblnRetiredAfterMDAge && !lblnRetirementYearSplit &&
                                                lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.Where(
                                                                        item => item.icdoBenefitCalculationYearlyDetail.plan_year == i && i > lintAdjustmentStartYear).Count() > 1)

                                            {
                                                ldtStartDate = busGlobalFunctions.GetFirstPayrollDayOfMonth(i, abusBenefitCalculationRetirement.icdoBenefitCalculationHeader.retirement_date.Month);
                                                ldtEndDate = busGlobalFunctions.GetLastPayrollDayOfMonth(i, 12);
                                            }
                                            else
                                            {
                                                if (counter == 0)
                                                    ldtStartDate = busGlobalFunctions.GetFirstPayrollDayOfMonth(i, abusBenefitCalculationRetirement.ibusBenefitApplication.icdoBenefitApplication.min_distribution_date.Month);
                                                else
                                                    ldtStartDate = busGlobalFunctions.GetFirstPayrollDayOfMonth(i, 1);

                                                ldtEndDate = busGlobalFunctions.GetLastPayrollDayOfMonth(i, 12);
                                            }

                                            lintSuspendibleMonths = ((busGlobalFunctions.GetPayrollMonth(ldtEndDate) - busGlobalFunctions.GetPayrollMonth(ldtStartDate)) + 1) - ibusCalculation.GetNonSuspendibleMonths(
                                                   abusBenefitCalculationRetirement.ibusPerson.icdoPerson.istrSSNNonEncrypted.ToString(), abusBenefitCalculationRetirement.ibusPerson, i, busConstant.MPIPP_PLAN_ID, null, ldtStartDate, ldtEndDate);

                                        }
                                        #endregion

                                        #region Calculate YTD Benefit

                                        if (!lblnAccrualFlag)
                                        {
                                            ldecYTDAccruedBenefit = 0;
                                            ldecLifeYTDAccruedBenefit = 0;
                                        }
                                        else
                                        {
                                            ldecYTDAccruedBenefit = Math.Round(lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.accrued_benefit_amount * ldecBenefitOptionFactor, 2);
                                            ldecLifeYTDAccruedBenefit = Math.Round(lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.accrued_benefit_amount * ldecLifeBenefitOptionFactor, 2); //PIR 894
                                        }

                                        #endregion

                                        #region Calculate Cummulative Benefit Amount

                                        ldecCumBenefitAmount = ldecCumBenefitAmount + ldecYTDAccruedBenefit;
                                        ldecLifeCumBenefitAmount = ldecLifeCumBenefitAmount + ldecLifeYTDAccruedBenefit; //PIR 894

                                        #endregion



                                        #region Calculate ER current Year/ ER Derived Amount
                                        // EE Derived and EE Actuarial Increase will be the same as Prev Yrs Max EE Derived Benefit amount
                                        //decimal ldecEEDerivedBenefit = Math.Round((((ldecEEContributionAmount + ldecEEInterestAmount) * ldecTableAfactor) / ldecTableBfactor), 2);
                                        decimal ldecERDerivedBenefit = 0.0M;
                                        decimal ldecLifeERDerivedBenefit = 0.0M; //PIR 894

                                        if (counter == 0)
                                        {
                                            //PIR - 930
                                            if (!ablnReEmployed)
                                            {
                                                ldecEEDerivedAmt = Math.Round(lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.max_ee_derv_amt * ldecBenefitOptionFactor, 2);
                                                ldecLifeEEDerivedAmt = Math.Round(lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.max_ee_derv_amt * ldecLifeBenefitOptionFactor, 2); //PIR 894
                                            }
                                            else
                                            {
                                                ldecEEDerivedAmt = Math.Round(lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.ee_derived_benefit_amount, 2);
                                                ldecLifeEEDerivedAmt = Math.Round(lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.ee_derived_benefit_amount, 2); //PIR 894
                                            }

                                            //Ticket - 69718
                                            if (lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.Where(item => item.icdoBenefitCalculationYearlyDetail.plan_year ==
                                                                                     (i - 1)).Count() > 0)
                                            {
                                                Decimal ldecEEDerivedReference = Math.Round(lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.Where(item => item.icdoBenefitCalculationYearlyDetail.plan_year ==
                                                                                        (i - 1)).LastOrDefault().icdoBenefitCalculationYearlyDetail.max_ee_derv_amt * ldecBenefitOptionFactorStartingPoint, 2);//Ticket - 69718

                                                Decimal ldecLifeEEDerivedReference = Math.Round(lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.Where(item => item.icdoBenefitCalculationYearlyDetail.plan_year ==
                                                                                        (i - 1)).LastOrDefault().icdoBenefitCalculationYearlyDetail.max_ee_derv_amt * ldecLifeBenefitOptionFactorStartingPoint, 2); //PIR 894 //Ticket - 69718

                                                //PIR - 930
                                                if (!ablnReEmployed)
                                                {
                                                    ldecERDerivedBenefit = Math.Round(lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.Where(item => item.icdoBenefitCalculationYearlyDetail.plan_year ==
                                                                                          (i - 1)).LastOrDefault().icdoBenefitCalculationYearlyDetail.actuarial_equivalent_amount * ldecBenefitOptionFactorStartingPoint - ldecEEDerivedReference, 2);//Ticket - 69718

                                                    ldecLifeERDerivedBenefit = Math.Round(lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.Where(item => item.icdoBenefitCalculationYearlyDetail.plan_year ==
                                                                                          (i - 1)).LastOrDefault().icdoBenefitCalculationYearlyDetail.actuarial_equivalent_amount * ldecLifeBenefitOptionFactorStartingPoint - ldecLifeEEDerivedReference, 2); //PIR 894 //Ticket - 69718
                                                }
                                                else
                                                {
                                                    ldecERDerivedBenefit = Math.Round(lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.Where(item => item.icdoBenefitCalculationYearlyDetail.plan_year ==
                                                                                            (i - 1)).LastOrDefault().icdoBenefitCalculationYearlyDetail.actuarial_equivalent_amount - ldecEEDerivedAmt, 2); //Ticket - 69718

                                                    ldecLifeERDerivedBenefit = Math.Round(lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.Where(item => item.icdoBenefitCalculationYearlyDetail.plan_year ==
                                                                                            (i - 1)).LastOrDefault().icdoBenefitCalculationYearlyDetail.popup_to_life_amount - ldecLifeEEDerivedAmt, 2); //PIR 894 //Ticket - 69718
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (!ablnReEmployed)
                                            {
                                                ldecEEDerivedAmt = Math.Round(lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.max_ee_derv_amt * ldecBenefitOptionFactor, 2);
                                                ldecLifeEEDerivedAmt = Math.Round(lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.max_ee_derv_amt * ldecLifeBenefitOptionFactor, 2); //PIR 894
                                            }
                                            else
                                            {
                                                ldecEEDerivedAmt = Math.Round(lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.ee_derived_benefit_amount, 2);
                                                ldecLifeEEDerivedAmt = Math.Round(lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.ee_derived_benefit_amount, 2);
                                            }
                                            ldecERDerivedBenefit = Math.Round(ldecBenefitAsOfDeteminationDate - ldecEEDerivedPreviousYear, 2);
                                            ldecLifeERDerivedBenefit = Math.Round(ldecLifeBenefitAsOfDeteminationDate - ldecLifeEEDerivedPreviousYear, 2); //PIR 894
                                        }

                                        if (ldtPlanYrEndDate > abusBenefitCalculationRetirement.ibusBenefitApplication.icdoBenefitApplication.min_distribution_date ||
                                               (IsParticipantDisabled(abusBenefitCalculationRetirement.ibusBenefitApplication.icdoBenefitApplication.person_id)))
                                        {
                                            ldecERCurrentYear = Math.Round(((ldecERDerivedBenefit * lintSuspendibleMonths) +
                                                                 (ldecERDerivedBenefit * (1 + ldecActiveRetireeInc) * lintSuspendibleMonthsAfterIncrease) + ldecErDerivedPrevious), 2);

                                            ldecLifeERCurrentYear = Math.Round(((ldecLifeERDerivedBenefit * lintSuspendibleMonths) +
                                                                 (ldecLifeERDerivedBenefit * (1 + ldecActiveRetireeInc) * lintSuspendibleMonthsAfterIncrease) + ldecLifeErDerivedPrevioius), 2); //PIR 894

                                        }
                                        #endregion

                                        #region Calculate GAM factor and values

                                        decimal ldecAge = Math.Max(ldecNormalRetAge, Convert.ToInt32(Math.Floor(lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.age)));

                                        if (ldecBenefitOptionFactor != 0)
                                        {
                                            ldecGAM71Factor = Math.Round((ibusCalculation.GetBenefitTypeFactor(ldecAge) / ldecBenefitOptionFactor), 3);
                                            //ldecLifeGAM71Factor = Math.Round(ibusCalculation.GetBenefitTypeFactor(ldecAge) / ldecLifeBenefitOptionFactor, 3); //PIR 894
                                        }
                                        //PIR RID 73053 Added following code for Safe check before dividing by factor
                                        if (ldecLifeBenefitOptionFactor != 0)
                                        {
                                            ldecLifeGAM71Factor = Math.Round(ibusCalculation.GetBenefitTypeFactor(ldecAge) / ldecLifeBenefitOptionFactor, 3); //PIR 894
                                        }

                                        #endregion

                                        #region Calculate Value Benefit Paid

                                        if (ldecERCurrentYear == 0)
                                        {
                                            ldecValueBenefitPaid = 0;
                                        }
                                        else if (ldecERCurrentYear != 0 && ldecGAM71Factor != 0)
                                        {
                                            ldecValueBenefitPaid = (ldecERCurrentYear / ldecGAM71Factor) * (-1);
                                        }

                                        //PIR 894
                                        if (ldecLifeERCurrentYear == 0)
                                        {
                                            ldecLifeValueBenefitPaid = 0;
                                        }
                                        //PIR RID 73053 Changed safe check for the variable used inside condition.
                                        //else if (ldecLifeERCurrentYear != 0 && ldecGAM71Factor != 0)
                                        else if (ldecLifeERCurrentYear != 0 && ldecLifeGAM71Factor != 0)
                                        {
                                            ldecLifeValueBenefitPaid = (ldecLifeERCurrentYear / ldecLifeGAM71Factor) * (-1);
                                        }

                                        #endregion

                                        #region Calculate Benefit As of Determination Date

                                        if (counter == 0)
                                        {
                                            if (lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.Where(item => item.icdoBenefitCalculationYearlyDetail.plan_year ==
                                                                                         (i - 1)).Count() > 0)
                                            {
                                                //PIR - 930
                                                if (!ablnReEmployed)
                                                {
                                                    ldecBenefitAsOfDeteminationDate = Math.Round(lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.Where(item => item.icdoBenefitCalculationYearlyDetail.plan_year ==
                                                                                            (i - 1)).LastOrDefault().icdoBenefitCalculationYearlyDetail.actuarial_equivalent_amount * ldecBenefitOptionFactorStartingPoint * (1 + ldecActiveRetireeInc), 2);//Ticket - 69718

                                                    ldecLifeBenefitAsOfDeteminationDate = Math.Round(lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.Where(item => item.icdoBenefitCalculationYearlyDetail.plan_year ==
                                                                                            (i - 1)).LastOrDefault().icdoBenefitCalculationYearlyDetail.actuarial_equivalent_amount * ldecLifeBenefitOptionFactorStartingPoint * (1 + ldecActiveRetireeInc), 2); //PIR 894 //Ticket - 69718
                                                }
                                                else
                                                {
                                                    ldecBenefitAsOfDeteminationDate = Math.Round(lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.Where(item => item.icdoBenefitCalculationYearlyDetail.plan_year ==
                                                                                            (i - 1)).LastOrDefault().icdoBenefitCalculationYearlyDetail.actuarial_equivalent_amount, 2); //Ticket - 69718

                                                    ldecLifeBenefitAsOfDeteminationDate = Math.Round(lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.Where(item => item.icdoBenefitCalculationYearlyDetail.plan_year ==
                                                                                            (i - 1)).LastOrDefault().icdoBenefitCalculationYearlyDetail.popup_to_life_amount, 2); //PIR 894 //Ticket - 69718
                                                }
                                            }

                                            if (!ablnReEmployed && lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.Where(item => item.icdoBenefitCalculationYearlyDetail.plan_year == i
                                                && item.icdoBenefitCalculationYearlyDetail.break_years_count >= 2).Count() > 0)
                                            {
                                                ldecAnnualAdjustmentPreviousYear = 0;
                                                ldecLifeAnnualAdjustmentPreviousYear = 0;
                                            }
                                            else
                                            {
                                                ldecAnnualAdjustmentPreviousYear = ldecBenefitAsOfDeteminationDate;
                                                ldecLifeAnnualAdjustmentPreviousYear = ldecLifeBenefitAsOfDeteminationDate;
                                            }                                            
                                        }

                                        else
                                        {
                                            ////PIR 894
                                            ldecBenefitAsOfDeteminationDate = ldecBenefitAsOfDeteminationDate + Math.Round(ldecAnnualAdjustmentPreviousYear * (ldecActiveRetireeInc), 2);
                                            ldecLifeBenefitAsOfDeteminationDate = ldecLifeBenefitAsOfDeteminationDate + Math.Round(ldecLifeAnnualAdjustmentPreviousYear * (ldecActiveRetireeInc), 2); //PIR 894

                                            //10 Percent - RMD
                                            ldecAnnualAdjustmentPreviousYear += Math.Round(ldecAnnualAdjustmentPreviousYear * (ldecActiveRetireeInc), 2);
                                            ldecLifeAnnualAdjustmentPreviousYear += Math.Round(ldecLifeAnnualAdjustmentPreviousYear * (ldecActiveRetireeInc), 2); //PIR 894

                                            if (ldecAnnualAdjustmentPreviousYear == decimal.Zero)
                                                ldecActiveRetireeInc = 0;

                                            if (ldecLifeAnnualAdjustmentPreviousYear == decimal.Zero)
                                                ldecActiveRetireeInc = 0;
                                        }

                                        if (Convert.ToInt32(lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.Where(item => item.icdoBenefitCalculationYearlyDetail.plan_year ==
                                                               i).FirstOrDefault().icdoBenefitCalculationYearlyDetail.age) > ldecNormalRetAge)
                                        {
                                            if ((!lblnAccrualFlag) || abusBenefitCalculationRetirement.icdoBenefitCalculationHeader.retirement_date > ldtDate &&
                                            ldtPlanYrEndDate < abusBenefitCalculationRetirement.ibusBenefitApplication.icdoBenefitApplication.min_distribution_date &&
                                                (!IsParticipantDisabled(abusBenefitCalculationRetirement.ibusBenefitApplication.icdoBenefitApplication.person_id)))
                                            {
                                                ldecBenefitAsOfDeteminationDate = Math.Round(ldecBenefitAsOfDeteminationDate + ldecYTDAccruedBenefit, 2);
                                                ldecLifeBenefitAsOfDeteminationDate = Math.Round(ldecLifeBenefitAsOfDeteminationDate + ldecLifeYTDAccruedBenefit, 2); //PIR 894
                                            }
                                            else
                                            {
                                                ldecBenefitAsOfDeteminationDate = Math.Round(ldecBenefitAsOfDeteminationDate + Math.Max(0, ldecCumBenefitAmount + ldecValueBenefitPaid), 2);
                                                ldecLifeBenefitAsOfDeteminationDate = Math.Round(ldecLifeBenefitAsOfDeteminationDate + Math.Max(0, ldecLifeCumBenefitAmount + ldecLifeValueBenefitPaid), 2); //PIR 894
                                            }
                                        }

                                        //10 Percent
                                        if (!ablnReEmployed && lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.Where(item => item.icdoBenefitCalculationYearlyDetail.plan_year == i
                                        && item.icdoBenefitCalculationYearlyDetail.break_years_count >= 2).Count() > 0)
                                        {
                                            ldecAnnualAdjustmentPreviousYear = 0;
                                            ldecLifeAnnualAdjustmentPreviousYear = 0; //PIR 894
                                        }
                                        else
                                        {
                                            if (lblnAccrualFlag)
                                            {
                                                ldecAnnualAdjustmentPreviousYear = Math.Round(ldecAnnualAdjustmentPreviousYear + Math.Max(0, ldecCumBenefitAmount + ldecValueBenefitPaid), 2);
                                                ldecLifeAnnualAdjustmentPreviousYear = Math.Round(ldecLifeAnnualAdjustmentPreviousYear + Math.Max(0, ldecLifeCumBenefitAmount + ldecLifeValueBenefitPaid), 2);
                                            }
                                        }

                                        ////PIR 894
                                        if (lblnPopupToLife)
                                        {
                                            if (ldecPopupbenefitamount != ldecBenefitAsOfDeteminationDate && counter == 0)
                                            {
                                                ldecPopupoptionfactor = ldecBenefitOptionFactorStartingPoint;
                                                ldecPopupbenefitamount = ldecBenefitAsOfDeteminationDate;
                                            }
                                            else if(ldecPopupbenefitamount != ldecBenefitAsOfDeteminationDate)
                                            {
                                                ldecPopupoptionfactor = ldecBenefitOptionFactor;
                                                ldecPopupbenefitamount = ldecBenefitAsOfDeteminationDate;
                                            }
                                        }

                                        #endregion

                                        #region Update yearly detail table with MD ajdustment values


                                        ldecErDerivedPrevious = ldecERCurrentYear;
                                        ldecEEDerivedPreviousYear = ldecEEDerivedAmt;
                                        
                                        //894
                                        ldecLifeErDerivedPrevioius = ldecLifeERCurrentYear;
                                        ldecLifeEEDerivedPreviousYear = ldecLifeEEDerivedAmt;

                                        lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.ee_derived_amount = ldecEEDerivedAmt;
                                        lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.ee_act_inc_amt = 0M;                                        
                                        lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.er_derived_amount = ldecERDerivedBenefit;
                                        lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.value_benefit_paid = ldecValueBenefitPaid;
                                        lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.gam_71_factor = ldecGAM71Factor;
                                        lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.active_retiree_inc = ldecActiveRetireeInc * 100;
                                        lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.total_accrued_benefit = ldecCumBenefitAmount;
                                        lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.annual_adjustment_amount = ldecBenefitAsOfDeteminationDate;
                                        lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.actuarial_accrued_benenfit = decimal.Zero;
                                        lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.actuarial_equivalent_amount = decimal.Zero;
                                        lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.sus_months_before_incr = lintSuspendibleMonths;
                                        lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.sus_months_after_incr = lintSuspendibleMonthsAfterIncrease;
                                        lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.er_current_year = ldecERCurrentYear;
                                        lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.benefit_as_of_det_date = ldecBenefitAsOfDeteminationDate;
                                        lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.popup_to_life_amount = ldecLifeBenefitAsOfDeteminationDate; //PIR 894

                                        if (ldecYTDAccruedBenefit != 0)
                                        {
                                            lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.accrued_benefit_amount = ldecYTDAccruedBenefit;
                                        }

                                        lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.benefit_option_factor = ldecBenefitOptionFactor;
                                        
                                        //Write Code to Modify the Total Accured Benefit According 
                                        lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.Update();

                                        #endregion

                                        counter++;

                                        if (lblnRetirementYearSplit && lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.Where(
                                                                        item => item.icdoBenefitCalculationYearlyDetail.plan_year == i && i > lintAdjustmentStartYear
                                                                        && abusBenefitCalculationRetirement.icdoBenefitCalculationHeader.retirement_date.Year == i).Count() < 2)
                                        {
                                            lblnRetirementYearSplit = false;
                                        }

                                        //PIR - 930
                                        if (lblnRetiredAfterMDAge && lblnRetirementYearSplit)
                                            i = i - 1;

                                    }
                                }
                            }
                            else
                            {                             
                                return;
                            }

                            //Create payee account
                            //PIR - 930
                            if (!ablnReEmployed)
                            {
                                abusBenefitCalculationRetirement.icdoBenefitCalculationHeader.istrRetirementType = busConstant.RETIREMENT_TYPE_MINIMUM_DISTRIBUTION;
                            }
                            //PIR 967
                            if (abusBenefitCalculationRetirement.ibusBenefitApplication != null && abusBenefitCalculationRetirement.ibusBenefitApplication.icdoBenefitApplication.min_distribution_date != DateTime.MinValue
                                && lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.benefit_subtype_value == busConstant.RETIREMENT_TYPE_MINIMUM_DISTRIBUTION)
                            {
                                abusBenefitCalculationRetirement.icdoBenefitCalculationHeader.retirement_date = abusBenefitCalculationRetirement.ibusBenefitApplication.icdoBenefitApplication.min_distribution_date;
                                abusBenefitCalculationRetirement.icdoBenefitCalculationHeader.Update();
                            }

                            if (lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.Where(t => t.icdoBenefitCalculationYearlyDetail.plan_year == lintAdjustmentStartYear - 1).Count() > 0)
                            {
                                //PIR 894
                                if(lblnPopupToLife && lintAdjustmentStartYear - 1 > ldtJointAnnuitantDOD.Year)
                                lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.OrderByDescending(t => t.icdoBenefitCalculationYearlyDetail.benefit_calculation_yearly_detail_id).
                                    Where(t => t.icdoBenefitCalculationYearlyDetail.plan_year == lintAdjustmentStartYear - 1).FirstOrDefault().icdoBenefitCalculationYearlyDetail.benefit_option_factor =
                                    ldecLifeBenefitOptionFactorStartingPoint;
                                else
                                    lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.OrderByDescending(t => t.icdoBenefitCalculationYearlyDetail.benefit_calculation_yearly_detail_id).
                                    Where(t => t.icdoBenefitCalculationYearlyDetail.plan_year == lintAdjustmentStartYear - 1).FirstOrDefault().icdoBenefitCalculationYearlyDetail.benefit_option_factor =
                                    ldecBenefitOptionFactorStartingPoint;

                                lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.OrderByDescending(t => t.icdoBenefitCalculationYearlyDetail.benefit_calculation_yearly_detail_id).
                                    Where(t => t.icdoBenefitCalculationYearlyDetail.plan_year == lintAdjustmentStartYear - 1).FirstOrDefault().icdoBenefitCalculationYearlyDetail.Update();

                                if (lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.Where(t => t.icdoBenefitCalculationYearlyDetail.plan_year == lintAdjustmentStartYear).Count() > 0)
                                {
                                    if (abusBenefitCalculationRetirement.icdoBenefitCalculationHeader.retirement_date.Month != 1)
                                    {
                                        //PIR 894
                                        if(lblnPopupToLife)
                                            lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.OrderBy(t => t.icdoBenefitCalculationYearlyDetail.benefit_calculation_yearly_detail_id).
                                            Where(t => t.icdoBenefitCalculationYearlyDetail.plan_year == lintAdjustmentStartYear).FirstOrDefault().icdoBenefitCalculationYearlyDetail.benefit_option_factor =
                                            ldecLifeBenefitOptionFactorStartingPoint;
                                        else
                                            lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.OrderBy(t => t.icdoBenefitCalculationYearlyDetail.benefit_calculation_yearly_detail_id).
                                            Where(t => t.icdoBenefitCalculationYearlyDetail.plan_year == lintAdjustmentStartYear).FirstOrDefault().icdoBenefitCalculationYearlyDetail.benefit_option_factor =
                                            ldecBenefitOptionFactorStartingPoint;
                                    }
                                    lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.unreduced_benefit_amount = lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.OrderBy(t => t.icdoBenefitCalculationYearlyDetail.benefit_calculation_yearly_detail_id).
                                    Where(t => t.icdoBenefitCalculationYearlyDetail.plan_year == lintAdjustmentStartYear - 1).FirstOrDefault().icdoBenefitCalculationYearlyDetail.actuarial_equivalent_amount;


                                    lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.early_reduced_benefit_amount = lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.OrderBy(t => t.icdoBenefitCalculationYearlyDetail.benefit_calculation_yearly_detail_id).
                                    Where(t => t.icdoBenefitCalculationYearlyDetail.plan_year == lintAdjustmentStartYear - 1).FirstOrDefault().icdoBenefitCalculationYearlyDetail.actuarial_equivalent_amount;

                                    //RID 60954
                                    if (lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.Where(t => t.icdoBenefitCalculationYearlyDetail.plan_year == lintAdjustmentStartYear).Count() > 1)
                                    {
                                        lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.additional_accrued_benefit_amount = lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.OrderBy(t => t.icdoBenefitCalculationYearlyDetail.benefit_calculation_yearly_detail_id).
                                            Where(t => t.icdoBenefitCalculationYearlyDetail.plan_year == lintAdjustmentStartYear).FirstOrDefault().icdoBenefitCalculationYearlyDetail.annual_adjustment_amount;

                                        lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.actuarial_accrued_benefit_amount = lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.OrderBy(t => t.icdoBenefitCalculationYearlyDetail.benefit_calculation_yearly_detail_id).
                                            Where(t => t.icdoBenefitCalculationYearlyDetail.plan_year == lintAdjustmentStartYear).FirstOrDefault().icdoBenefitCalculationYearlyDetail.actuarial_equivalent_amount;
                                        
                                    }
                                    else
                                    {
                                        lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.additional_accrued_benefit_amount = lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.OrderBy(t => t.icdoBenefitCalculationYearlyDetail.benefit_calculation_yearly_detail_id).
                                        Where(t => t.icdoBenefitCalculationYearlyDetail.plan_year == lintAdjustmentStartYear - 1).FirstOrDefault().icdoBenefitCalculationYearlyDetail.annual_adjustment_amount;

                                        lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.actuarial_accrued_benefit_amount = lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.OrderBy(t => t.icdoBenefitCalculationYearlyDetail.benefit_calculation_yearly_detail_id).
                                            Where(t => t.icdoBenefitCalculationYearlyDetail.plan_year == lintAdjustmentStartYear - 1).FirstOrDefault().icdoBenefitCalculationYearlyDetail.actuarial_equivalent_amount;
                                    }
                                                                       

                                    lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.Update();
                                }
                            }

                            if ((ldecBenefitAsOfDeteminationDate.IsNull() || ldecBenefitAsOfDeteminationDate == busConstant.ZERO_DECIMAL)
                                    && abusBenefitCalculationRetirement.iclbBenefitCalculationDetail.IsNotNull()
                                    && abusBenefitCalculationRetirement.iclbBenefitCalculationDetail.Where(i => i.icdoBenefitCalculationDetail.plan_id == abusPayeeAccount.icdoPayeeAccount.iintPlanId).Count() > 0)
                                ldecBenefitAsOfDeteminationDate = abusBenefitCalculationRetirement.iclbBenefitCalculationDetail.Where(i => i.icdoBenefitCalculationDetail.plan_id == abusPayeeAccount.icdoPayeeAccount.iintPlanId).FirstOrDefault().icdoBenefitCalculationDetail.early_reduced_benefit_amount;

                            //PIR 894
                            if ((ldecLifeBenefitAsOfDeteminationDate.IsNull() || ldecLifeBenefitAsOfDeteminationDate == busConstant.ZERO_DECIMAL)
                                    && abusBenefitCalculationRetirement.iclbBenefitCalculationDetail.IsNotNull()
                                    && abusBenefitCalculationRetirement.iclbBenefitCalculationDetail.Where(i => i.icdoBenefitCalculationDetail.plan_id == abusPayeeAccount.icdoPayeeAccount.iintPlanId).Count() > 0)
                                ldecLifeBenefitAsOfDeteminationDate = abusBenefitCalculationRetirement.iclbBenefitCalculationDetail.Where(i => i.icdoBenefitCalculationDetail.plan_id == abusPayeeAccount.icdoPayeeAccount.iintPlanId).FirstOrDefault().icdoBenefitCalculationDetail.early_reduced_benefit_amount;


                            decimal ldecSurvivorBenefitAmt = 0M;

                            if (istrBenefitOptionValue == busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY || (istrBenefitOptionValue == busConstant.JOINT_50_PERCENT_POPUP_ANNUITY && !lblnPopupToLife)) //PIR 894
                            {
                                ldecSurvivorBenefitAmt = Convert.ToDecimal(Math.Round(ldecBenefitAsOfDeteminationDate * 0.50m, 2));
                            }
                            else if (istrBenefitOptionValue == busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY)
                            {
                                ldecSurvivorBenefitAmt = Convert.ToDecimal(Math.Round(ldecBenefitAsOfDeteminationDate * 0.75m, 2));
                            }
                            else if (istrBenefitOptionValue == busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY || (istrBenefitOptionValue == busConstant.JOINT_100_PERCENT_POPUP_ANNUITY && !lblnPopupToLife)
                                || istrBenefitOptionValue == busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY)
                            {
                                ldecSurvivorBenefitAmt = Convert.ToDecimal(Math.Round(ldecBenefitAsOfDeteminationDate , 2));
                            }
                           

                            if (!lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.IsNullOrEmpty())
                            {
                                if(!lblnPopupToLife) //PIR 894
                                lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.First().LoadData(abusPayeeAccount.icdoPayeeAccount.plan_benefit_id, ldecBenefitOptionFactor, ldecBenefitAsOfDeteminationDate,
                                                                                                            busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER, abusPayeeAccount.icdoPayeeAccount.person_id, abusBenefitCalculationRetirement.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                                                            istrBenefitOptionValue, ldecSurvivorBenefitAmt,adecPopupoptionfactor: ldecPopupoptionfactor, adecpopupbenefitamount: ldecPopupbenefitamount, adecpopupoptionfactoratret: ldecPopupOptionFactorAtRet); //PIR 894 //RequestID: 72091
                                else
                                    lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.First().LoadData(abusPayeeAccount.icdoPayeeAccount.plan_benefit_id, ldecLifeBenefitOptionFactor, ldecLifeBenefitAsOfDeteminationDate,
                                                                                                            busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER, abusPayeeAccount.icdoPayeeAccount.person_id, abusBenefitCalculationRetirement.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                                                            istrBenefitOptionValue, ldecSurvivorBenefitAmt, adecPopupoptionfactor: ldecPopupoptionfactor, adecpopupbenefitamount: ldecPopupbenefitamount, adecpopupoptionfactoratret: ldecPopupOptionFactorAtRet); //PIR 894 //RequestID: 72091

                                lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.Update();
                            }

                            else
                            {
                                lbusBenefitCalculationDetail.iclbBenefitCalculationOptions = new Collection<busBenefitCalculationOptions>();
                                busBenefitCalculationOptions lbusBenefitCalculationOptions = new busBenefitCalculationOptions();
                                lbusBenefitCalculationOptions.icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions();
                                lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.benefit_calculation_detail_id = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.benefit_calculation_detail_id;
                                lbusBenefitCalculationOptions.LoadData(abusPayeeAccount.icdoPayeeAccount.plan_benefit_id, ldecBenefitOptionFactor, ldecBenefitAsOfDeteminationDate,
                                                                        busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER, abusPayeeAccount.icdoPayeeAccount.person_id, abusBenefitCalculationRetirement.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                        istrBenefitOptionValue, ldecSurvivorBenefitAmt);
                                lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
                                lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.Insert();
                            }

                            //PIR 
                            if (ablnReEmployed || ablnRetiree)
                            {
                                //PIR 930
                                if (ablnRetiree && lintAdjustmentStartYear <= abusBenefitCalculationRetirement.icdoBenefitCalculationHeader.retirement_date.Year)
                                    lintAdjustmentStartYear = abusBenefitCalculationRetirement.icdoBenefitCalculationHeader.retirement_date.Year;

                                
                                abusBenefitCalculationRetirement.LoadDisabilityRetireeIncreases();
                                if (abusBenefitCalculationRetirement.iclbDisabilityRetireeIncrease != null && abusBenefitCalculationRetirement.iclbDisabilityRetireeIncrease.Count > 0)
                                {
                                    foreach (busDisabilityRetireeIncrease lbusDisabilityRetireeIncrease in abusBenefitCalculationRetirement.iclbDisabilityRetireeIncrease)
                                    {
                                        if (lbusDisabilityRetireeIncrease.icdoDisabilityRetireeIncrease.retiree_increase_date.Year >= lintAdjustmentStartYear)
                                        {
                                            lbusDisabilityRetireeIncrease.icdoDisabilityRetireeIncrease.Delete();
                                        }
                                    }
                                }

                                lclbActiveRetireeIncreaseContract = lbusActiveRetireeIncreaseContract.LoadActiveRetireeIncContractByMDYear(lintAdjustmentStartYear);

                                ibusCalculation.FillYearlyDetailSetBenefitAmountForEachYear(this, lbusBenefitCalculationDetail);

                                if (this.ibusPerson != null && this.ibusPerson.iclbPersonAccount != null &&
                                            this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.LOCAL_700_PLAN_ID).Count() > 0)
                                {
                                    ldecLocal700GauranteedAmt =
                                        ibusCalculation.GetLocal700GuarentedAmt(this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.LOCAL_700_PLAN_ID).FirstOrDefault().icdoPersonAccount.person_account_id);
                                }

                                foreach (busActiveRetireeIncreaseContract lbusRetireeIncreaseContract in lclbActiveRetireeIncreaseContract)
                                {
                                    //PIR 930
                                    if (abusBenefitCalculationRetirement.icdoBenefitCalculationHeader.retirement_date <= lbusRetireeIncreaseContract.icdoActiveRetireeIncreaseContract.retirement_date_to)
                                    {
                                        ibusCalculation.CalculateAndCreateRetireeIncreasePayeeAccount(lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.First(), null, lbusRetireeIncreaseContract, icdoBenefitCalculationHeader.retirement_date.Year,
                                                 icdoBenefitCalculationHeader.benefit_calculation_header_id, 0, ldecLocal700GauranteedAmt,
                                                 Convert.ToDecimal(lbusRetireeIncreaseContract.icdoActiveRetireeIncreaseContract.percent_increase_value)
                                                 , busConstant.BENEFIT_TYPE_RETIREMENT, this);
                                    }
                                }
                            }
                        }
                    } // code is over
                }

            }
        }

        #endregion

        #region Reevaluate MD From Reemployment
        public void CreateMDFromReemployment(busPayeeAccount abusPayeeAccount, DateTime ldtMinDistrubtionDate, string astrBenefitOptionValue)
        {

            int counter = 1;
            int lintAdjustmentStartYear = 0;
            decimal ldecPrevYearBenefitAmount = decimal.Zero;
            decimal ldecEEDerivedAmt = decimal.Zero;
            string lstrBenefitOptionValue = string.Empty;
            int lintParticipnatAgeAtMinDist = 0;
            int lintSurvivorAgeatMinDist = 0;
            decimal ldecBenefitAtMinDistributionYear = decimal.Zero;
            int lintSuspendibleMonths = 0;
            decimal ldecYTDAccruedBenefit = decimal.Zero; ;
            decimal ldecCumBenefitAmount = decimal.Zero; ;
            decimal ldecERDerivedBenefit = decimal.Zero; ;
            decimal ldecERCurrentYear = decimal.Zero;
            decimal ldecGAM71Factor = decimal.Zero;
            decimal ldecValueBenefitPaid = decimal.Zero;
            decimal ldecBenefitAsOfDeteminationDate = decimal.Zero;
            decimal ldecActiveRetireeInc = decimal.Zero;

            busBenefitCalculationDetail lbusBenefitCalculationDetail = this.iclbBenefitCalculationDetail.FirstOrDefault();
            busBenefitCalculationYearlyDetail lbusBenefitCalculationYearlyDetail = null;
            ibusCalculation.FillYearlyDetailSetBenefitAmountForEachYear(this, this.iclbBenefitCalculationDetail.FirstOrDefault());

            ldecEEDerivedAmt = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.ee_derived_benefit_amount;
            ldecBenefitAtMinDistributionYear = lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.OrderByDescending(item => item.icdoBenefitCalculationYearlyDetail.plan_year).Where(item =>
                item.icdoBenefitCalculationYearlyDetail.plan_year < ldtMinDistrubtionDate.Year).FirstOrDefault().icdoBenefitCalculationYearlyDetail.idecTotalBenefitAmount;
            ldecPrevYearBenefitAmount = ldecBenefitAtMinDistributionYear;

            lintParticipnatAgeAtMinDist = Convert.ToInt32(busGlobalFunctions.CalculatePersonAgeInDec(this.ibusPerson.icdoPerson.idtDateofBirth, new DateTime(ldtMinDistrubtionDate.Year, 1, 1)));
            lintSurvivorAgeatMinDist = Convert.ToInt32(busGlobalFunctions.CalculatePersonAgeInDec(this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth, new DateTime(ldtMinDistrubtionDate.Year, 1, 1)));
            lintAdjustmentStartYear = ldtMinDistrubtionDate.Year;
            #region Suspendible Months
            Dictionary<int, Dictionary<int, decimal>> ldictHoursAfterRetirement = new Dictionary<int, Dictionary<int, decimal>>();
            DateTime ldtLastWorkingDate = new DateTime();
            string lstrEmpName = string.Empty;
            int lintReemployedYear = 0;
            ldictHoursAfterRetirement = ibusCalculation.LoadMPIHoursAfterRetirementDate(this.ibusPerson.icdoPerson.istrSSNNonEncrypted,
                this.icdoBenefitCalculationHeader.retirement_date, busConstant.MPIPP_PLAN_ID, ref ldtLastWorkingDate, ref lstrEmpName, lintReemployedYear);
            this.ibusPerson.LoadPersonSuspendibleMonth();
            #endregion

            for (int i = lintAdjustmentStartYear; i <= DateTime.Now.Year - 1; i++)
            {
                lintSuspendibleMonths = 0;
                lbusBenefitCalculationYearlyDetail = null;
                DateTime ldtPlanYrEndDate = busGlobalFunctions.GetLastDateOfComputationYear(i);
                if (lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.Where(
                                                    item => item.icdoBenefitCalculationYearlyDetail.plan_year == i).Count() > 0)
                {
                    lbusBenefitCalculationYearlyDetail = lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.Where(
                                                        item => item.icdoBenefitCalculationYearlyDetail.plan_year == i).LastOrDefault();
                }

                if (lbusBenefitCalculationYearlyDetail == null)
                {
                    lbusBenefitCalculationYearlyDetail = new busBenefitCalculationYearlyDetail { icdoBenefitCalculationYearlyDetail = new cdoBenefitCalculationYearlyDetail() };
                    lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.plan_year = i;
                    lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.benefit_option_factor =
                        ibusCalculation.GetFactor(this, astrBenefitOptionValue, lintSurvivorAgeatMinDist + counter, lintParticipnatAgeAtMinDist + counter);
                }
                if (lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.benefit_option_factor == decimal.Zero)
                {
                    ibusCalculation.GetFactor(this, astrBenefitOptionValue, lintSurvivorAgeatMinDist + counter, lintParticipnatAgeAtMinDist + counter);
                }
                if (i >= ldtMinDistrubtionDate.Year)
                {
                    #region SetEEDerived & ER Derived
                    ldecERDerivedBenefit = ldecPrevYearBenefitAmount - ldecEEDerivedAmt;
                    #endregion

                    #region Set Accrual Flag

                    bool lblnAccrualFlag = false;

                    if (lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.vested_hours >= 870)
                    {
                        lblnAccrualFlag = true;
                    }

                    #endregion

                    #region Calculate suspendible Month count

                    DateTime ldtStartDate = busGlobalFunctions.GetFirstPayrollDayOfMonth(i, 1);
                    DateTime ldtEndDate = busGlobalFunctions.GetLastPayrollDayOfMonth(i, 12);

                    lintSuspendibleMonths = ibusCalculation.GetSuspendibleMonthsBetweenTwoDates(ldictHoursAfterRetirement, this.ibusPerson.iclbPersonSuspendibleMonth, new DateTime(i, 1, 1), new DateTime(i, 12, 1));

                    #endregion

                    #region Calculate YTD Benefit

                    if (!lblnAccrualFlag)
                    {
                        ldecYTDAccruedBenefit = 0;
                    }
                    else
                    {
                        ldecYTDAccruedBenefit = Math.Round((lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.accrued_benefit_amount * lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.benefit_option_factor), 2);
                    }

                    #endregion

                    #region Calculate Cummulative Benefit Amount

                    ldecCumBenefitAmount = ldecCumBenefitAmount + ldecYTDAccruedBenefit;

                    //if (counter == 1)
                    //{
                    //    ldecTotalAccuredBenefit = ldecBenefitAtMinDistributionYear + ldecYTDAccruedBenefit;
                    //}
                    //else
                    //{
                    //    ldecTotalAccuredBenefit = ldecTotalAccuredBenefit + ldecCumBenefitAmount;
                    //}


                    #endregion

                    #region Calculate Active Retiree Increase

                    ldecActiveRetireeInc = lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.active_retiree_inc;


                    #endregion

                    #region Calculate ER current Year/ ER Derived Amount
                    // EE Derived and EE Actuarial Increase will be the same as Prev Yrs Max EE Derived Benefit amount
                    //decimal ldecEEDerivedBenefit = Math.Round((((ldecEEContributionAmount + ldecEEInterestAmount) * ldecTableAfactor) / ldecTableBfactor), 2);
                    if (ldtPlanYrEndDate > ldtMinDistrubtionDate)
                    {
                        ldecERCurrentYear += Math.Round(((ldecERDerivedBenefit * lintSuspendibleMonths) +
                                             (ldecERDerivedBenefit * (1 + ldecActiveRetireeInc) * 0)), 2);
                    }
                    #endregion

                    #region Calculate GAM factor and values

                    decimal ldecAge = Math.Max(65, lintParticipnatAgeAtMinDist + counter);
                    if (lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.benefit_option_factor != decimal.Zero)
                    {
                        ldecGAM71Factor = Math.Round((ibusCalculation.GetBenefitTypeFactor(ldecAge) / lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.benefit_option_factor), 3);
                    }
                    #endregion

                    #region Calculate Value Benefit Paid

                    if (ldecERCurrentYear == 0)
                    {
                        ldecValueBenefitPaid = 0;
                    }
                    else if (ldecERCurrentYear != 0 && ldecGAM71Factor != 0)
                    {
                        ldecValueBenefitPaid = Math.Round((ldecERCurrentYear / ldecGAM71Factor) * (-1), 2);
                    }

                    #endregion
                }
                #region Calculate Benefit As of Determination Date

                ldecBenefitAsOfDeteminationDate = ldecPrevYearBenefitAmount + Math.Max(0, ldecCumBenefitAmount + ldecValueBenefitPaid);

                #endregion

                #region Update yearly detail table with MD ajdustment values


                lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.ee_derived_amount = ldecEEDerivedAmt;
                lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.er_derived_amount = ldecERDerivedBenefit;
                lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.value_benefit_paid = ldecValueBenefitPaid;
                lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.gam_71_factor = ldecGAM71Factor;
                lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.active_retiree_inc = ldecActiveRetireeInc;
                lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.annual_adjustment_amount = ldecBenefitAsOfDeteminationDate;
                ldecPrevYearBenefitAmount = ldecBenefitAsOfDeteminationDate;


                #endregion

                counter++;
            }

            //Create payee account

            this.icdoBenefitCalculationHeader.istrRetirementType = busConstant.RETIREMENT_TYPE_MINIMUM_DISTRIBUTION;
            if (!lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.IsNullOrEmpty())
            {
                lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.First().LoadData(abusPayeeAccount.icdoPayeeAccount.plan_benefit_id, lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.benefit_option_factor, ldecBenefitAsOfDeteminationDate,
                                                                                            busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER, abusPayeeAccount.icdoPayeeAccount.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                                            astrBenefitOptionValue, 0);
                //lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.Update();
            }
            //this.UpdateorInsertVlauesInPayeeAccountTable(true);
        }

        #endregion

        #endregion

        #region Private Methods

        #region Pension Calculation Business Logic

        protected void CalculateJointAndSurvivorBenefitOptions(decimal adecFinalAccruedBenefitAmount)
        {
            decimal ldecBenefitOptionFactor = busConstant.ZERO_DECIMAL;
            decimal ldecBenefitAmount = busConstant.ZERO_DECIMAL;
            busBenefitCalculationOptions lbusBenefitCalculationOptions;
            int lintParticipantAge = Convert.ToInt32(Math.Floor(this.icdoBenefitCalculationHeader.age));
            int lintSurvivorAge = Convert.ToInt32(Math.Floor(this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement));

            // Resetting the common variables
            ldecBenefitOptionFactor = 1;
            double ldblBenefitOptionFactor = 1;

            // Qualified Joint And 50% Survivor Annuity Benefit Option
            ldblBenefitOptionFactor = 0.86 + 0.005 * (lintSurvivorAge - lintParticipantAge) + 0.006 * (65 - lintParticipantAge);
            ldecBenefitOptionFactor = Convert.ToDecimal(Math.Round(ldblBenefitOptionFactor, 6));
            //PIR-940
            if (ldecBenefitOptionFactor>1 && this.icdoBenefitCalculationHeader.iintPlanId == busConstant.MPIPP_PLAN_ID)
            {
                ldecBenefitOptionFactor = 1;
            }
            lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
            this.idecQualifiedJointAndSurvivorAnnuity50 = Convert.ToDecimal(Math.Round(adecFinalAccruedBenefitAmount * ldecBenefitOptionFactor, 2));
            lbusBenefitCalculationOptions.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.MPIPP_PLAN_ID, busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY), ldecBenefitOptionFactor, this.idecQualifiedJointAndSurvivorAnnuity50,
                                                    busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER, this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                    busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY, Convert.ToDecimal(Math.Round(adecFinalAccruedBenefitAmount * ldecBenefitOptionFactor * 0.50m, 2)));

            // No Need to show the Relative Value for JOINT_50_PERCENT_SURVIVOR_ANNUITY Benefit Option
            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);

            // Resetting the common variables
            ldecBenefitOptionFactor = 1;
            ldblBenefitOptionFactor = 1;
            ldecBenefitAmount = busConstant.ZERO_DECIMAL;

            // Qualified Joint And 75% Survivor Annuity Benefit Option
            ldblBenefitOptionFactor = 0.80 + 0.01 * (lintSurvivorAge - lintParticipantAge) + 0.006 * (65 - lintParticipantAge);
            ldecBenefitOptionFactor = Convert.ToDecimal(Math.Round(ldblBenefitOptionFactor, 6));
            //PIR-940
            if (ldecBenefitOptionFactor > 1 && this.icdoBenefitCalculationHeader.iintPlanId == busConstant.MPIPP_PLAN_ID)
            {
                ldecBenefitOptionFactor = 1;
            }
            lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
            ldecBenefitAmount = Convert.ToDecimal(Math.Round(adecFinalAccruedBenefitAmount * ldecBenefitOptionFactor, 2));
            lbusBenefitCalculationOptions.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.MPIPP_PLAN_ID, busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY), ldecBenefitOptionFactor, ldecBenefitAmount,
                                                    busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER, this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                    busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, Convert.ToDecimal(Math.Round(adecFinalAccruedBenefitAmount * ldecBenefitOptionFactor * 0.75m, 2)));


            lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, this.icdoBenefitCalculationHeader.age, this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement);
            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);


            // Resetting the common variables
            ldecBenefitOptionFactor = 1;
            ldblBenefitOptionFactor = 1;
            ldecBenefitAmount = busConstant.ZERO_DECIMAL;

            // Qualified Joint And 100% Survivor Annuity Benefit Option
            ldblBenefitOptionFactor = 0.75 + 0.01 * (lintSurvivorAge - lintParticipantAge) + 0.006 * (65 - lintParticipantAge);
            ldecBenefitOptionFactor = Convert.ToDecimal(Math.Round(ldblBenefitOptionFactor, 6));
            //PIR-940
            if (ldecBenefitOptionFactor > 1 && this.icdoBenefitCalculationHeader.iintPlanId == busConstant.MPIPP_PLAN_ID)
            {
                ldecBenefitOptionFactor = 1;
            }
            lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
            ldecBenefitAmount = Convert.ToDecimal(Math.Round(adecFinalAccruedBenefitAmount * ldecBenefitOptionFactor, 2));
            lbusBenefitCalculationOptions.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.MPIPP_PLAN_ID, busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY), ldecBenefitOptionFactor, ldecBenefitAmount,
                                                    busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER, this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                    busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY, Convert.ToDecimal(Math.Round(adecFinalAccruedBenefitAmount * ldecBenefitOptionFactor, 2)));


            lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY, this.icdoBenefitCalculationHeader.age, this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement);
            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);

            // Resetting the common variables
            ldecBenefitOptionFactor = 1;
            ldblBenefitOptionFactor = 1;
            ldecBenefitAmount = busConstant.ZERO_DECIMAL;

            // Joint And 50% Survivor Pop-up Annuity Benefit Option
            ldblBenefitOptionFactor = 0.83 + 0.007 * (lintSurvivorAge - lintParticipantAge) + 0.006 * (65 - lintParticipantAge);
            ldecBenefitOptionFactor = Convert.ToDecimal(Math.Round(ldblBenefitOptionFactor, 6));
            //PIR-940
             if (ldecBenefitOptionFactor > 1 && this.icdoBenefitCalculationHeader.iintPlanId == busConstant.MPIPP_PLAN_ID)
                {
                ldecBenefitOptionFactor = 1;
                 }
            lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
            ldecBenefitAmount = Convert.ToDecimal(Math.Round(adecFinalAccruedBenefitAmount * ldecBenefitOptionFactor, 2));
            lbusBenefitCalculationOptions.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.MPIPP_PLAN_ID, busConstant.JOINT_50_PERCENT_POPUP_ANNUITY), ldecBenefitOptionFactor, ldecBenefitAmount,
                                                    busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER, this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                     busConstant.JOINT_50_PERCENT_POPUP_ANNUITY, Convert.ToDecimal(Math.Round(ldecBenefitAmount * 0.50m, 2)));


            lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.JOINT_50_PERCENT_POPUP_ANNUITY, this.icdoBenefitCalculationHeader.age, this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement);
            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);

            // Resetting the common variables
            ldecBenefitOptionFactor = 1;
            ldblBenefitOptionFactor = 1;
            ldecBenefitAmount = busConstant.ZERO_DECIMAL;

            // Joint And 100% Survivor Pop-up Annuity Benefit Option
            ldblBenefitOptionFactor = 0.71 + 0.01 * (lintSurvivorAge - lintParticipantAge) + 0.008 * (65 - lintParticipantAge);
            ldecBenefitOptionFactor = Convert.ToDecimal(ldblBenefitOptionFactor);
            //PIR-940
            if (ldecBenefitOptionFactor > 1 && this.icdoBenefitCalculationHeader.iintPlanId == busConstant.MPIPP_PLAN_ID)
            {
                ldecBenefitOptionFactor = 1;
            }
            lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
            ldecBenefitAmount = Convert.ToDecimal(Math.Round(adecFinalAccruedBenefitAmount * ldecBenefitOptionFactor, 2));
            lbusBenefitCalculationOptions.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.MPIPP_PLAN_ID, busConstant.JOINT_100_PERCENT_POPUP_ANNUITY), ldecBenefitOptionFactor, ldecBenefitAmount,
                                                    busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER, this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                    busConstant.JOINT_100_PERCENT_POPUP_ANNUITY, Convert.ToDecimal(Math.Round(adecFinalAccruedBenefitAmount * ldecBenefitOptionFactor, 2)));


            lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.JOINT_100_PERCENT_POPUP_ANNUITY, this.icdoBenefitCalculationHeader.age, this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement);
            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
        }


        public void CalculateIAPBenefitAmount(string astrBenefitOptionValue, bool ablnReEmployed = false, string astrAdjustmentFlag = "")
        {
            decimal ldecIAPBalance = busConstant.ZERO_DECIMAL;
            decimal ldecLocal52SpecialAccountBalance = busConstant.ZERO_DECIMAL;
            decimal ldecLocal161SpecialAccountBalance = busConstant.ZERO_DECIMAL;

            decimal ldecIAPHours4QtrAlloc = busConstant.ZERO_DECIMAL;
            decimal ldecIAPHoursA2forQtrAlloc = busConstant.ZERO_DECIMAL;
            decimal ldecIAPPercent4forQtrAlloc = busConstant.ZERO_DECIMAL;
            decimal ldecAnnunityAdjustmentMultiplier = this.GetAnnunityMultiplier();

            #region To Set Values for IAP QTR Allocations
            if (!ablnReEmployed)
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
                #endregion

                if (!this.iclbPersonAccountRetirementContribution.IsNullOrEmpty())
                {
                    if (this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.IAP).Count() > 0)
                    {
                        int lintPersonAccountId = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.IAP).First().icdoPersonAccount.person_account_id;
                        if (this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Count() > 0)
                        {
                            busCalculation lbusCalculation = new busCalculation();
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
                                lbusCalculation.GetIAPAndSpecialAccountBalance(busConstant.BOOL_FALSE, null, this.iclbBenefitCalculationDetail, this, null,
                                this.ibusBenefitApplication.icdoBenefitApplication.retirement_date, ldecIAPHours4QtrAlloc, ldecIAPHoursA2forQtrAlloc, ldecIAPPercent4forQtrAlloc, ablnExecuteIAPAllocation: lblnCalculateAllocations);
                            }
                        }
                    }
                }
            }
            else
            {
                this.iblnCalculateIAPBenefit = true;
                this.iblnCalculateL161SplAccBenefit = true;
                this.iblnCalculateL52SplAccBenefit = true;
                ibusCalculation.LoadIapBalanceForReEmployedParticipants(this);
            }

            if (this.iblnCalculateIAPBenefit)
            {
                ldecIAPBalance = this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().icdoBenefitCalculationDetail.iap_balance_amount;


                if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL)
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
                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().icdoBenefitCalculationDetail.unreduced_benefit_amount = ldecIAPBalance;
                    //Process QDRO Offset
                    this.ibusCalculation.ProcessQDROOffset(this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID && item.icdoBenefitCalculationDetail.l52_spl_acc_flag.IsNullOrEmpty() && item.icdoBenefitCalculationDetail.l161_spl_acc_flag.IsNullOrEmpty()).First()
                        , this.ibusPerson.icdoPerson.person_id, ref ldecIAPBalance);

                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).First().
                        icdoBenefitCalculationDetail.idecBenefitsAfterQDROOffSet = ldecIAPBalance;

                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().icdoBenefitCalculationDetail.early_reduction_factor = 1.0m;
                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().icdoBenefitCalculationDetail.early_reduced_benefit_amount = ldecIAPBalance;
                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().icdoBenefitCalculationDetail.final_monthly_benefit_amount = ldecIAPBalance;
                }

            }

            if (this.iblnCalculateL52SplAccBenefit)
            {
                ldecLocal52SpecialAccountBalance = this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().icdoBenefitCalculationDetail.local52_special_acct_bal_amount;

                this.ibusCalculation.ProcessQDROOffset(this.iclbBenefitCalculationDetail.Where(i => i.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault(), this.ibusPerson.icdoPerson.person_id, ref ldecLocal52SpecialAccountBalance, false, false, true, false, busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE);
            }

            if (this.iblnCalculateL161SplAccBenefit)
            {
                ldecLocal161SpecialAccountBalance = this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().icdoBenefitCalculationDetail.local161_special_acct_bal_amount;

                this.ibusCalculation.ProcessQDROOffset(this.iclbBenefitCalculationDetail.Where(i => i.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault(), this.ibusPerson.icdoPerson.person_id, ref ldecLocal52SpecialAccountBalance, false, false, false, true, busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE);
            }

            // Step 2. Multiply the amount with the Benefit Option Factors and store it in the BenefitCalculationOption Collection
            if ((ldecIAPBalance > busConstant.ZERO_DECIMAL || ldecLocal52SpecialAccountBalance > busConstant.ZERO_DECIMAL || ldecLocal161SpecialAccountBalance > busConstant.ZERO_DECIMAL)
                || icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT) //PIR 985 10252015
            {
                int lintPlanBenefitId = busConstant.ZERO_INT;
                decimal ldecBenefitOptionFactor = 1;
                busBenefitCalculationOptions lbusBenefitCalculationOptions;

                bool lblnCheckIfSpouse = busConstant.BOOL_FALSE;
                lblnCheckIfSpouse = this.ibusCalculation.CheckIfSpouse(this.icdoBenefitCalculationHeader.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id);

                switch (astrBenefitOptionValue)
                {
                    case busConstant.CodeValueAll:
                        // Calculate the Benefit Amounts for all Benefit Options Availble for IAP
                        if (lblnCheckIfSpouse)
                        {
                            #region Qualified Joint And 50% Survivor Annuity Benefit Option
                            // Qualified Joint And 50% Survivor Annuity Benefit Option
                            ldecBenefitOptionFactor = 1;
                            lintPlanBenefitId = ibusCalculation.GetPlanBenefitId(busConstant.IAP_PLAN_ID, busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY);
                            ldecBenefitOptionFactor = ibusCalculation.GetBenefitOptionFactor(this.icdoBenefitCalculationHeader.benefit_type_value, lintPlanBenefitId,
                                                                                  Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge), Convert.ToInt32(this.icdoBenefitCalculationHeader.idecSurvivorFullAge)) * 12;

                            if (this.iblnCalculateIAPBenefit && ldecIAPBalance >= busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT)
                            {

                                lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                                if(this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE)
                                {
                                    lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, ldecBenefitOptionFactor, ldecAnnunityAdjustmentMultiplier == 0 ? Convert.ToDecimal(Math.Round(ldecIAPBalance / ldecBenefitOptionFactor, 2)): Convert.ToDecimal(Math.Round(ldecIAPBalance / ldecBenefitOptionFactor, 2)) * ldecAnnunityAdjustmentMultiplier, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                        this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY,
                                                                     ldecAnnunityAdjustmentMultiplier == 0 ? Convert.ToDecimal(Math.Round(ldecIAPBalance / ldecBenefitOptionFactor * 0.50m, 2)) : Convert.ToDecimal(Math.Round(ldecIAPBalance / ldecBenefitOptionFactor * 0.50m, 2)) * ldecAnnunityAdjustmentMultiplier, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE);
                                }
                                else
                                {
                                    lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, ldecBenefitOptionFactor, Convert.ToDecimal(Math.Round(ldecIAPBalance / ldecBenefitOptionFactor, 2)), busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                        this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY,
                                                                        Convert.ToDecimal(Math.Round(ldecIAPBalance / ldecBenefitOptionFactor * 0.50m, 2)), busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE);


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

                            if (this.iblnCalculateL52SplAccBenefit && ldecLocal52SpecialAccountBalance >= busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT)
                            {
                                //Local52 Special Account Balance
                                lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                                if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE)
                                {
                                    lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, ldecBenefitOptionFactor, ldecAnnunityAdjustmentMultiplier == 0 ? Convert.ToDecimal(Math.Round(ldecLocal52SpecialAccountBalance / ldecBenefitOptionFactor, 2)): Convert.ToDecimal(Math.Round(ldecLocal52SpecialAccountBalance / ldecBenefitOptionFactor, 2)) * ldecAnnunityAdjustmentMultiplier, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                       this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY,
                                                                       ldecAnnunityAdjustmentMultiplier == 0 ? Convert.ToDecimal(Math.Round(ldecLocal52SpecialAccountBalance / ldecBenefitOptionFactor * 0.50m, 2)): Convert.ToDecimal(Math.Round(ldecLocal52SpecialAccountBalance / ldecBenefitOptionFactor * 0.50m, 2)) * ldecAnnunityAdjustmentMultiplier, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_TRUE, busConstant.BOOL_FALSE);
                                }
                                else
                                {
                                    lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, ldecBenefitOptionFactor, Convert.ToDecimal(Math.Round(ldecLocal52SpecialAccountBalance / ldecBenefitOptionFactor, 2)), busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                       this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY,
                                                                       Convert.ToDecimal(Math.Round(ldecLocal52SpecialAccountBalance / ldecBenefitOptionFactor * 0.50m, 2)), busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_TRUE, busConstant.BOOL_FALSE);
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

                            if (this.iblnCalculateL161SplAccBenefit && ldecLocal161SpecialAccountBalance >= busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT)
                            {
                                //Local161 Special Account Balance
                                lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                                if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE)
                                {
                                    lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, ldecBenefitOptionFactor, ldecAnnunityAdjustmentMultiplier == 0 ? Convert.ToDecimal(Math.Round(ldecLocal161SpecialAccountBalance / ldecBenefitOptionFactor, 2)): Convert.ToDecimal(Math.Round(ldecLocal161SpecialAccountBalance / ldecBenefitOptionFactor, 2)) * ldecAnnunityAdjustmentMultiplier, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                      this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY,
                                                                     ldecAnnunityAdjustmentMultiplier == 0 ? Convert.ToDecimal(Math.Round(ldecLocal161SpecialAccountBalance / ldecBenefitOptionFactor * 0.50m, 2)): Convert.ToDecimal(Math.Round(ldecLocal161SpecialAccountBalance / ldecBenefitOptionFactor * 0.50m, 2)) * ldecAnnunityAdjustmentMultiplier, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_TRUE);


                                }
                                else
                                {
                                    lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, ldecBenefitOptionFactor, Convert.ToDecimal(Math.Round(ldecLocal161SpecialAccountBalance / ldecBenefitOptionFactor, 2)), busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                      this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY,
                                                                      Convert.ToDecimal(Math.Round(ldecLocal161SpecialAccountBalance / ldecBenefitOptionFactor * 0.50m, 2)), busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_TRUE);

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
                            #endregion

                            #region Qualified Joint And 75% Survivor Annuity Benefit Option
                            // Qualified Joint And 75% Survivor Annuity Benefit Option
                            ldecBenefitOptionFactor = 1;
                            lintPlanBenefitId = ibusCalculation.GetPlanBenefitId(busConstant.IAP_PLAN_ID, busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY);
                            ldecBenefitOptionFactor = ibusCalculation.GetBenefitOptionFactor(this.icdoBenefitCalculationHeader.benefit_type_value, lintPlanBenefitId,
                                                                                  Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge), Convert.ToInt32(this.icdoBenefitCalculationHeader.idecSurvivorFullAge)) * 12;

                            if (this.iblnCalculateIAPBenefit && ldecIAPBalance >= busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT)
                            {
                                lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                                if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE)
                                {
                                    lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, ldecBenefitOptionFactor, ldecAnnunityAdjustmentMultiplier == 0 ? Convert.ToDecimal(Math.Round(ldecIAPBalance / ldecBenefitOptionFactor, 2)): Convert.ToDecimal(Math.Round(ldecIAPBalance / ldecBenefitOptionFactor, 2)) * ldecAnnunityAdjustmentMultiplier, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                        this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY,
                                                                        ldecAnnunityAdjustmentMultiplier == 0 ? Convert.ToDecimal(Math.Round(ldecIAPBalance / ldecBenefitOptionFactor * 0.75m, 2)): Convert.ToDecimal(Math.Round(ldecIAPBalance / ldecBenefitOptionFactor * 0.75m, 2)) * ldecAnnunityAdjustmentMultiplier, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE);
                                }
                                else
                                {
                                    lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, ldecBenefitOptionFactor, Convert.ToDecimal(Math.Round(ldecIAPBalance / ldecBenefitOptionFactor, 2)), busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                        this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY,
                                                                        Convert.ToDecimal(Math.Round(ldecIAPBalance / ldecBenefitOptionFactor * 0.75m, 2)), busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE);
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

                            if (this.iblnCalculateL52SplAccBenefit && ldecLocal52SpecialAccountBalance >= busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT)
                            {
                                //Local52 Special Account Balance
                                lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                                if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE)
                                {
                                    lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, ldecBenefitOptionFactor, ldecAnnunityAdjustmentMultiplier == 0 ? Convert.ToDecimal(Math.Round(ldecLocal52SpecialAccountBalance / ldecBenefitOptionFactor, 2)) : Convert.ToDecimal(Math.Round(ldecLocal52SpecialAccountBalance / ldecBenefitOptionFactor, 2)) * ldecAnnunityAdjustmentMultiplier, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                       this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY,
                                                                       ldecAnnunityAdjustmentMultiplier == 0 ? Convert.ToDecimal(Math.Round(ldecLocal52SpecialAccountBalance / ldecBenefitOptionFactor * 0.75m, 2)): Convert.ToDecimal(Math.Round(ldecLocal52SpecialAccountBalance / ldecBenefitOptionFactor * 0.75m, 2)) * ldecAnnunityAdjustmentMultiplier, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_TRUE, busConstant.BOOL_FALSE);
                                }
                                else
                                {
                                    lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, ldecBenefitOptionFactor, Convert.ToDecimal(Math.Round(ldecLocal52SpecialAccountBalance / ldecBenefitOptionFactor, 2)), busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                       this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY,
                                                                       Convert.ToDecimal(Math.Round(ldecLocal52SpecialAccountBalance / ldecBenefitOptionFactor * 0.75m, 2)), busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_TRUE, busConstant.BOOL_FALSE);

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
                            if (this.iblnCalculateL161SplAccBenefit && ldecLocal161SpecialAccountBalance >= busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT)
                            {
                                //Local161 Special Account Balance
                                lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                                if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE)
                                {
                                    lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, ldecBenefitOptionFactor, ldecAnnunityAdjustmentMultiplier == 0 ? Convert.ToDecimal(Math.Round(ldecLocal161SpecialAccountBalance / ldecBenefitOptionFactor, 2)): Convert.ToDecimal(Math.Round(ldecLocal161SpecialAccountBalance / ldecBenefitOptionFactor, 2)) * ldecAnnunityAdjustmentMultiplier, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                       this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY,
                                                                       ldecAnnunityAdjustmentMultiplier == 0 ? Convert.ToDecimal(Math.Round(ldecLocal161SpecialAccountBalance / ldecBenefitOptionFactor * 0.75m, 2)): Convert.ToDecimal(Math.Round(ldecLocal161SpecialAccountBalance / ldecBenefitOptionFactor * 0.75m, 2)) * ldecAnnunityAdjustmentMultiplier, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_TRUE);
                                }
                                else
                                {
                                    lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, ldecBenefitOptionFactor, Convert.ToDecimal(Math.Round(ldecLocal161SpecialAccountBalance / ldecBenefitOptionFactor, 2)), busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                       this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY,
                                                                       Convert.ToDecimal(Math.Round(ldecLocal161SpecialAccountBalance / ldecBenefitOptionFactor * 0.75m, 2)), busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_TRUE);
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

                            #endregion

                            #region  Qualified Joint And 100% Survivor Annuity Benefit Option
                            // Qualified Joint And 100% Survivor Annuity Benefit Option
                            ldecBenefitOptionFactor = 1;
                            lintPlanBenefitId = ibusCalculation.GetPlanBenefitId(busConstant.IAP_PLAN_ID, busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY);
                            ldecBenefitOptionFactor = ibusCalculation.GetBenefitOptionFactor(this.icdoBenefitCalculationHeader.benefit_type_value, lintPlanBenefitId,
                                                                                  Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge), Convert.ToInt32(this.icdoBenefitCalculationHeader.idecSurvivorFullAge)) * 12;

                            if (this.iblnCalculateIAPBenefit && ldecIAPBalance >= busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT)
                            {
                                lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                                if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE)
                                {
                                    lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, ldecBenefitOptionFactor, ldecAnnunityAdjustmentMultiplier == 0 ? Convert.ToDecimal(Math.Round(ldecIAPBalance / ldecBenefitOptionFactor, 2)): Convert.ToDecimal(Math.Round(ldecIAPBalance / ldecBenefitOptionFactor, 2)) * ldecAnnunityAdjustmentMultiplier, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                       this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY,
                                                                       ldecAnnunityAdjustmentMultiplier == 0 ? Convert.ToDecimal(Math.Round(ldecIAPBalance / ldecBenefitOptionFactor, 2)): Convert.ToDecimal(Math.Round(ldecIAPBalance / ldecBenefitOptionFactor, 2)) * ldecAnnunityAdjustmentMultiplier, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE);
                                }
                                else
                                {
                                    lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, ldecBenefitOptionFactor, Convert.ToDecimal(Math.Round(ldecIAPBalance / ldecBenefitOptionFactor, 2)), busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                       this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY,
                                                                       Convert.ToDecimal(Math.Round(ldecIAPBalance / ldecBenefitOptionFactor, 2)), busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE);

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
                            //Local 52 Special Account
                            //  lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                            //lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, ldecBenefitOptionFactor, Convert.ToDecimal(Math.Round(ldecLocal52SpecialAccountBalance / ldecBenefitOptionFactor, 2)), busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                            //                                        this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY,
                            //                                        Convert.ToDecimal(Math.Round(ldecLocal52SpecialAccountBalance / ldecBenefitOptionFactor, 2)), busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_TRUE, busConstant.BOOL_FALSE);
                            ////                                        Convert.ToDecimal(Math.Round(ldecSpecialAccountBalance / ldecBenefitOptionFactor, 2)));
                            //this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);

                            //Local 161 Special Account
                            //lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                            //lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, ldecBenefitOptionFactor, Convert.ToDecimal(Math.Round(ldecLocal161SpecialAccountBalance / ldecBenefitOptionFactor, 2)), busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                            //                                        this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY,
                            //                                        Convert.ToDecimal(Math.Round(ldecLocal161SpecialAccountBalance / ldecBenefitOptionFactor, 2)), busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_TRUE);
                            ////                                        Convert.ToDecimal(Math.Round(ldecSpecialAccountBalance / ldecBenefitOptionFactor, 2)));

                            //this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);

                            #endregion
                        }

                        // NOTE - This Benefit option is not provided for Retirement under IAP Plan. 
                        // But when participant is retiring under MPI Plan then we need to include all the Benefit Options for MPI
                        // Ten Years Certain and Life Annuity
                        //if (this.icdoBenefitCalculationHeader.iintPlanId == busConstant.MPIPP_PLAN_ID)
                        //{

                        #region Ten Years Certain and Life Annuity
                        // This estimate if for MPI & IAP Plans. Hence calculating the Benefit for Ten Years Cerrtain Option as well.
                        ldecBenefitOptionFactor = 1;
                        lintPlanBenefitId = ibusCalculation.GetPlanBenefitId(busConstant.IAP_PLAN_ID, busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY);
                        ldecBenefitOptionFactor = ibusCalculation.GetBenefitOptionFactor(this.icdoBenefitCalculationHeader.benefit_type_value, lintPlanBenefitId, Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge), busConstant.ZERO_INT) * 12;

                        if (this.iblnCalculateIAPBenefit && ldecIAPBalance >= busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT)
                        {
                            lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                            if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE)
                            {
                                lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, ldecBenefitOptionFactor, ldecAnnunityAdjustmentMultiplier == 0 ? Convert.ToDecimal(Math.Round(ldecIAPBalance / ldecBenefitOptionFactor, 2)): Convert.ToDecimal(Math.Round(ldecIAPBalance / ldecBenefitOptionFactor, 2)) * ldecAnnunityAdjustmentMultiplier, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                    this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY,
                                                                    ldecAnnunityAdjustmentMultiplier == 0 ? Convert.ToDecimal(Math.Round(ldecIAPBalance / ldecBenefitOptionFactor, 2)): Convert.ToDecimal(Math.Round(ldecIAPBalance / ldecBenefitOptionFactor, 2)) * ldecAnnunityAdjustmentMultiplier, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE);
                            }
                            else
                            {
                                lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, ldecBenefitOptionFactor, Convert.ToDecimal(Math.Round(ldecIAPBalance / ldecBenefitOptionFactor, 2)), busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                    this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY,
                                                                    Convert.ToDecimal(Math.Round(ldecIAPBalance / ldecBenefitOptionFactor, 2)), busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE);
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

                        //Local 52 Special Account Balance
                        //lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                        //lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, ldecBenefitOptionFactor, Convert.ToDecimal(Math.Round(ldecLocal52SpecialAccountBalance / ldecBenefitOptionFactor, 2)), busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                        //                                        this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY,
                        //                                        Convert.ToDecimal(Math.Round(ldecLocal52SpecialAccountBalance / ldecBenefitOptionFactor, 2)), busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_TRUE, busConstant.BOOL_FALSE);
                        //this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);

                        //Local 161 Special Account Balance
                        //lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                        //lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, ldecBenefitOptionFactor, Convert.ToDecimal(Math.Round(ldecLocal161SpecialAccountBalance / ldecBenefitOptionFactor, 2)), busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                        //                                        this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY,
                        //                                        Convert.ToDecimal(Math.Round(ldecLocal161SpecialAccountBalance / ldecBenefitOptionFactor, 2)), busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_TRUE);
                        //this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
                        #endregion
                        //}

                        #region Life Annunity
                        // Life Annuity
                        ldecBenefitOptionFactor = 1;
                        lintPlanBenefitId = ibusCalculation.GetPlanBenefitId(busConstant.IAP_PLAN_ID, busConstant.LIFE_ANNUTIY);
                        ldecBenefitOptionFactor = ibusCalculation.GetBenefitOptionFactor(this.icdoBenefitCalculationHeader.benefit_type_value, lintPlanBenefitId, Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge), busConstant.ZERO_INT) * 12;
                        if (this.iblnCalculateIAPBenefit && ldecIAPBalance >= busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT)
                        {
                            lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                            if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE)
                            {
                                lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, ldecBenefitOptionFactor, ldecAnnunityAdjustmentMultiplier == 0 ? Convert.ToDecimal(Math.Round(ldecIAPBalance / ldecBenefitOptionFactor, 2)): Convert.ToDecimal(Math.Round(ldecIAPBalance / ldecBenefitOptionFactor, 2)) * ldecAnnunityAdjustmentMultiplier, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                    this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.LIFE_ANNUTIY,
                                                                    ldecAnnunityAdjustmentMultiplier == 0 ? Convert.ToDecimal(Math.Round(ldecIAPBalance / ldecBenefitOptionFactor, 2)): Convert.ToDecimal(Math.Round(ldecIAPBalance / ldecBenefitOptionFactor, 2)) * ldecAnnunityAdjustmentMultiplier, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE);
                            }
                            else
                            {
                                lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, ldecBenefitOptionFactor, Convert.ToDecimal(Math.Round(ldecIAPBalance / ldecBenefitOptionFactor, 2)), busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                    this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.LIFE_ANNUTIY,
                                                                    Convert.ToDecimal(Math.Round(ldecIAPBalance / ldecBenefitOptionFactor, 2)), busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE);
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

                        if (this.iblnCalculateL52SplAccBenefit && ldecLocal52SpecialAccountBalance >= busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT)
                        {
                            //local 52 Special Account Balance
                            lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                            if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE)
                            {
                                lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, ldecBenefitOptionFactor, ldecAnnunityAdjustmentMultiplier == 0 ? Convert.ToDecimal(Math.Round(ldecLocal52SpecialAccountBalance / ldecBenefitOptionFactor, 2)): Convert.ToDecimal(Math.Round(ldecLocal52SpecialAccountBalance / ldecBenefitOptionFactor, 2)) * ldecAnnunityAdjustmentMultiplier, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                   this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.LIFE_ANNUTIY,
                                                                   ldecAnnunityAdjustmentMultiplier == 0 ? Convert.ToDecimal(Math.Round(ldecLocal52SpecialAccountBalance / ldecBenefitOptionFactor, 2)): Convert.ToDecimal(Math.Round(ldecLocal52SpecialAccountBalance / ldecBenefitOptionFactor, 2)) * ldecAnnunityAdjustmentMultiplier, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_TRUE, busConstant.BOOL_FALSE);
                            }
                            else
                            {
                                lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, ldecBenefitOptionFactor, Convert.ToDecimal(Math.Round(ldecLocal52SpecialAccountBalance / ldecBenefitOptionFactor, 2)), busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                   this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.LIFE_ANNUTIY,
                                                                   Convert.ToDecimal(Math.Round(ldecLocal52SpecialAccountBalance / ldecBenefitOptionFactor, 2)),busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_TRUE, busConstant.BOOL_FALSE);
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

                        if (this.iblnCalculateL161SplAccBenefit && ldecLocal161SpecialAccountBalance >= busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT)
                        {
                            //local 161 Special Account Balance
                            lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                            if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE)
                            {
                                lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, ldecBenefitOptionFactor, ldecAnnunityAdjustmentMultiplier == 0 ? Convert.ToDecimal(Math.Round(ldecLocal161SpecialAccountBalance / ldecBenefitOptionFactor, 2)): Convert.ToDecimal(Math.Round(ldecLocal161SpecialAccountBalance / ldecBenefitOptionFactor, 2)) * ldecAnnunityAdjustmentMultiplier, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                    this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.LIFE_ANNUTIY,
                                                                    ldecAnnunityAdjustmentMultiplier == 0 ? Convert.ToDecimal(Math.Round(ldecLocal161SpecialAccountBalance / ldecBenefitOptionFactor, 2)): Convert.ToDecimal(Math.Round(ldecLocal161SpecialAccountBalance / ldecBenefitOptionFactor, 2)) * ldecAnnunityAdjustmentMultiplier, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_TRUE);
                            }
                            else
                            {
                                lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, ldecBenefitOptionFactor, Convert.ToDecimal(Math.Round(ldecLocal161SpecialAccountBalance / ldecBenefitOptionFactor, 2)), busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                    this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.LIFE_ANNUTIY,
                                                                    Convert.ToDecimal(Math.Round(ldecLocal161SpecialAccountBalance / ldecBenefitOptionFactor, 2)), busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_TRUE);
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
                        #endregion

                        #region LUMPSUM
                        // Lumpsum Benefit Option
                        // No factor. The Lump sum for IAP will be the total IAP Balance itself.
                        lintPlanBenefitId = ibusCalculation.GetPlanBenefitId(busConstant.IAP_PLAN_ID, busConstant.LUMP_SUM);

                        if (this.iblnCalculateIAPBenefit)
                        {
                            lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                            lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, 1, ldecIAPBalance, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                    this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.LUMP_SUM, busConstant.ZERO_DECIMAL,
                                                                    busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE);

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

                        if (this.iblnCalculateL52SplAccBenefit)
                        {
                            //Local 52 Special Account Balance
                            lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                            lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, 1, ldecLocal52SpecialAccountBalance, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                    this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.LUMP_SUM, busConstant.ZERO_DECIMAL,
                                                                   busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_TRUE, busConstant.BOOL_FALSE);

                            if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL)
                            {
                                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID && item.icdoBenefitCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
                            }
                            else
                            {
                                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
                            }
                        }

                        if (this.iblnCalculateL161SplAccBenefit)
                        {
                            //Local 161 Special Account Balance
                            lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                            lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, 1, ldecLocal161SpecialAccountBalance, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                    this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.LUMP_SUM, busConstant.ZERO_DECIMAL,
                                                                    busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_TRUE);

                            if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL)
                            {
                                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID && item.icdoBenefitCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
                            }
                            else
                            {
                                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
                            }
                        }
                        #endregion

                        break;

                    case busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY:
                        if (lblnCheckIfSpouse && ldecIAPBalance >= busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT)
                        {
                            #region Qualified Joint And 50% Survivor Annuity Benefit Option
                            // Qualified Joint And 50% Survivor Annuity Benefit Option
                            ldecBenefitOptionFactor = 1;
                            lintPlanBenefitId = ibusCalculation.GetPlanBenefitId(busConstant.IAP_PLAN_ID, busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY);
                            ldecBenefitOptionFactor = ibusCalculation.GetBenefitOptionFactor(this.icdoBenefitCalculationHeader.benefit_type_value, lintPlanBenefitId,
                                                                                  Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge), Convert.ToInt32(this.icdoBenefitCalculationHeader.idecSurvivorFullAge)) * 12;

                            if (this.iblnCalculateIAPBenefit)
                            {

                                lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                                if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE)
                                {
                                    lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, ldecBenefitOptionFactor, ldecAnnunityAdjustmentMultiplier == 0 ? Convert.ToDecimal(Math.Round(ldecIAPBalance / ldecBenefitOptionFactor, 2)): Convert.ToDecimal(Math.Round(ldecIAPBalance / ldecBenefitOptionFactor, 2)) * ldecAnnunityAdjustmentMultiplier, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                        this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY,
                                                                       ldecAnnunityAdjustmentMultiplier == 0 ? Convert.ToDecimal(Math.Round(ldecIAPBalance / ldecBenefitOptionFactor * 0.50m, 2)): Convert.ToDecimal(Math.Round(ldecIAPBalance / ldecBenefitOptionFactor * 0.50m, 2)) * ldecAnnunityAdjustmentMultiplier, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE);
                                }
                                else
                                {
                                    lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, ldecBenefitOptionFactor, Convert.ToDecimal(Math.Round(ldecIAPBalance / ldecBenefitOptionFactor, 2)), busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                        this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY,
                                                                        Convert.ToDecimal(Math.Round(ldecIAPBalance / ldecBenefitOptionFactor * 0.50m, 2)), busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE);


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

                            if (this.iblnCalculateL52SplAccBenefit)
                            {
                                //Local52 Special Account Balance
                                lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                                if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE)
                                {
                                    lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, ldecBenefitOptionFactor, ldecAnnunityAdjustmentMultiplier == 0 ? Convert.ToDecimal(Math.Round(ldecLocal52SpecialAccountBalance / ldecBenefitOptionFactor, 2)): Convert.ToDecimal(Math.Round(ldecLocal52SpecialAccountBalance / ldecBenefitOptionFactor, 2)) * ldecAnnunityAdjustmentMultiplier, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                       this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY,
                                                                       ldecAnnunityAdjustmentMultiplier == 0 ? Convert.ToDecimal(Math.Round(ldecLocal52SpecialAccountBalance / ldecBenefitOptionFactor * 0.50m, 2)): Convert.ToDecimal(Math.Round(ldecLocal52SpecialAccountBalance / ldecBenefitOptionFactor * 0.50m, 2)) * ldecAnnunityAdjustmentMultiplier, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_TRUE, busConstant.BOOL_FALSE);
                                }
                                else
                                {
                                    lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, ldecBenefitOptionFactor, Convert.ToDecimal(Math.Round(ldecLocal52SpecialAccountBalance / ldecBenefitOptionFactor, 2)), busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                       this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY,
                                                                       Convert.ToDecimal(Math.Round(ldecLocal52SpecialAccountBalance / ldecBenefitOptionFactor * 0.50m, 2)), busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_TRUE, busConstant.BOOL_FALSE);
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

                            if (this.iblnCalculateL161SplAccBenefit)
                            {
                                //Local161 Special Account Balance
                                lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                                if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE)
                                {
                                    lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, ldecBenefitOptionFactor, ldecAnnunityAdjustmentMultiplier == 0 ? Convert.ToDecimal(Math.Round(ldecLocal161SpecialAccountBalance / ldecBenefitOptionFactor, 2)): Convert.ToDecimal(Math.Round(ldecLocal161SpecialAccountBalance / ldecBenefitOptionFactor, 2)) * ldecAnnunityAdjustmentMultiplier, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                       this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY,
                                                                       ldecAnnunityAdjustmentMultiplier == 0 ? Convert.ToDecimal(Math.Round(ldecLocal161SpecialAccountBalance / ldecBenefitOptionFactor * 0.50m, 2)): Convert.ToDecimal(Math.Round(ldecLocal161SpecialAccountBalance / ldecBenefitOptionFactor * 0.50m, 2)) * ldecAnnunityAdjustmentMultiplier, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_TRUE);
                                }
                                else
                                {
                                    lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, ldecBenefitOptionFactor, Convert.ToDecimal(Math.Round(ldecLocal161SpecialAccountBalance / ldecBenefitOptionFactor, 2)), busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                       this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY,
                                                                       Convert.ToDecimal(Math.Round(ldecLocal161SpecialAccountBalance / ldecBenefitOptionFactor * 0.50m, 2)), busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_TRUE);
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
                            #endregion
                        }
                        break;

                    case busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY:
                        if (lblnCheckIfSpouse && ldecIAPBalance >= busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT)
                        {
                            #region Qualified Joint And 75% Survivor Annuity Benefit Option
                            // Qualified Joint And 75% Survivor Annuity Benefit Option
                            ldecBenefitOptionFactor = 1;
                            lintPlanBenefitId = ibusCalculation.GetPlanBenefitId(busConstant.IAP_PLAN_ID, busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY);
                            ldecBenefitOptionFactor = ibusCalculation.GetBenefitOptionFactor(this.icdoBenefitCalculationHeader.benefit_type_value, lintPlanBenefitId,
                                                                                  Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge), Convert.ToInt32(this.icdoBenefitCalculationHeader.idecSurvivorFullAge)) * 12;

                            if (this.iblnCalculateIAPBenefit)
                            {
                                lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                                if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE)
                                {
                                    lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, ldecBenefitOptionFactor, ldecAnnunityAdjustmentMultiplier == 0 ? Convert.ToDecimal(Math.Round(ldecIAPBalance / ldecBenefitOptionFactor, 2)): Convert.ToDecimal(Math.Round(ldecIAPBalance / ldecBenefitOptionFactor, 2)) * ldecAnnunityAdjustmentMultiplier, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                        this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY,
                                                                        ldecAnnunityAdjustmentMultiplier == 0 ? Convert.ToDecimal(Math.Round(ldecIAPBalance / ldecBenefitOptionFactor * 0.75m, 2)): Convert.ToDecimal(Math.Round(ldecIAPBalance / ldecBenefitOptionFactor * 0.75m, 2)) * ldecAnnunityAdjustmentMultiplier, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE);
                                }
                                else
                                {
                                    lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, ldecBenefitOptionFactor, Convert.ToDecimal(Math.Round(ldecIAPBalance / ldecBenefitOptionFactor, 2)), busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                        this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY,
                                                                        Convert.ToDecimal(Math.Round(ldecIAPBalance / ldecBenefitOptionFactor * 0.75m, 2)), busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE);
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
                            #endregion
                        }
                        break;

                    case busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY:
                        if (lblnCheckIfSpouse && ldecIAPBalance >= busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT)
                        {
                            #region  Qualified Joint And 100% Survivor Annuity Benefit Option
                            // Qualified Joint And 100% Survivor Annuity Benefit Option
                            ldecBenefitOptionFactor = 1;
                            lintPlanBenefitId = ibusCalculation.GetPlanBenefitId(busConstant.IAP_PLAN_ID, busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY);
                            ldecBenefitOptionFactor = ibusCalculation.GetBenefitOptionFactor(this.icdoBenefitCalculationHeader.benefit_type_value, lintPlanBenefitId,
                                                                                  Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge), Convert.ToInt32(this.icdoBenefitCalculationHeader.idecSurvivorFullAge)) * 12;

                            if (this.iblnCalculateIAPBenefit)
                            {
                                lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                                if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE)
                                {
                                    lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, ldecBenefitOptionFactor, ldecAnnunityAdjustmentMultiplier == 0 ? Convert.ToDecimal(Math.Round(ldecIAPBalance / ldecBenefitOptionFactor, 2)): Convert.ToDecimal(Math.Round(ldecIAPBalance / ldecBenefitOptionFactor, 2)) * ldecAnnunityAdjustmentMultiplier, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                        this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY,
                                                                        ldecAnnunityAdjustmentMultiplier == 0 ? Convert.ToDecimal(Math.Round(ldecIAPBalance / ldecBenefitOptionFactor, 2)): Convert.ToDecimal(Math.Round(ldecIAPBalance / ldecBenefitOptionFactor, 2)) * ldecAnnunityAdjustmentMultiplier, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE);
                                }
                                else
                                {
                                    lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, ldecBenefitOptionFactor, Convert.ToDecimal(Math.Round(ldecIAPBalance / ldecBenefitOptionFactor, 2)), busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                        this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY,
                                                                        Convert.ToDecimal(Math.Round(ldecIAPBalance / ldecBenefitOptionFactor, 2)), busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE);
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
                            #endregion
                        }
                        break;

                    case busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY:
                        if (ldecIAPBalance >= busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT)
                        {
                            // This Benefit Option MAY BE provided for Retirement under IAP Plan (Only when Participant retires under MPI & IAP simultaneously)
                            #region Ten Years Certain and Life Annuity
                            // This estimate if for MPI & IAP Plans. Hence calculating the Benefit for Ten Years Cerrtain Option as well.
                            ldecBenefitOptionFactor = 1;
                            lintPlanBenefitId = ibusCalculation.GetPlanBenefitId(busConstant.IAP_PLAN_ID, busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY);
                            ldecBenefitOptionFactor = ibusCalculation.GetBenefitOptionFactor(this.icdoBenefitCalculationHeader.benefit_type_value, lintPlanBenefitId, Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge), busConstant.ZERO_INT) * 12;

                            if (this.iblnCalculateIAPBenefit)
                            {
                                lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                                if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE)
                                {
                                    lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, ldecBenefitOptionFactor, ldecAnnunityAdjustmentMultiplier ==0 ?Convert.ToDecimal(Math.Round(ldecIAPBalance / ldecBenefitOptionFactor, 2)): Convert.ToDecimal(Math.Round(ldecIAPBalance / ldecBenefitOptionFactor, 2)) * ldecAnnunityAdjustmentMultiplier, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                        this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY,
                                                                        ldecAnnunityAdjustmentMultiplier == 0 ? Convert.ToDecimal(Math.Round(ldecIAPBalance / ldecBenefitOptionFactor, 2)): Convert.ToDecimal(Math.Round(ldecIAPBalance / ldecBenefitOptionFactor, 2)) * ldecAnnunityAdjustmentMultiplier, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE);
                                }
                                else
                                {
                                    lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, ldecBenefitOptionFactor, Convert.ToDecimal(Math.Round(ldecIAPBalance / ldecBenefitOptionFactor, 2)), busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                        this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY,
                                                                        Convert.ToDecimal(Math.Round(ldecIAPBalance / ldecBenefitOptionFactor, 2)), busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE);

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
                            #endregion
                        }
                        break;

                    case busConstant.LIFE_ANNUTIY:
                        if (ldecIAPBalance >= busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT)
                        {
                            #region Life Annunity
                            // Life Annuity
                            ldecBenefitOptionFactor = 1;
                            lintPlanBenefitId = ibusCalculation.GetPlanBenefitId(busConstant.IAP_PLAN_ID, busConstant.LIFE_ANNUTIY);
                            ldecBenefitOptionFactor = ibusCalculation.GetBenefitOptionFactor(this.icdoBenefitCalculationHeader.benefit_type_value, lintPlanBenefitId, Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge), busConstant.ZERO_INT) * 12;
                            if (this.iblnCalculateIAPBenefit)
                            {
                                lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                                if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE)
                                {
                                    lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, ldecBenefitOptionFactor, ldecAnnunityAdjustmentMultiplier == 0 ? Convert.ToDecimal(Math.Round(ldecIAPBalance / ldecBenefitOptionFactor, 2)): Convert.ToDecimal(Math.Round(ldecIAPBalance / ldecBenefitOptionFactor, 2)) * ldecAnnunityAdjustmentMultiplier, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                        this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.LIFE_ANNUTIY,
                                                                        ldecAnnunityAdjustmentMultiplier == 0 ? Convert.ToDecimal(Math.Round(ldecIAPBalance / ldecBenefitOptionFactor, 2)): Convert.ToDecimal(Math.Round(ldecIAPBalance / ldecBenefitOptionFactor, 2)) * ldecAnnunityAdjustmentMultiplier, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE);
                                }
                                else
                                {
                                    lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, ldecBenefitOptionFactor, Convert.ToDecimal(Math.Round(ldecIAPBalance / ldecBenefitOptionFactor, 2)), busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                        this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.LIFE_ANNUTIY,
                                                                        Convert.ToDecimal(Math.Round(ldecIAPBalance / ldecBenefitOptionFactor, 2)), busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE);
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

                            if (this.iblnCalculateL52SplAccBenefit)
                            {
                                //local 52 Special Account Balance
                                lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                                if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE)
                                {
                                    lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, ldecBenefitOptionFactor, ldecAnnunityAdjustmentMultiplier == 0 ? Convert.ToDecimal(Math.Round(ldecLocal52SpecialAccountBalance / ldecBenefitOptionFactor, 2)): Convert.ToDecimal(Math.Round(ldecLocal52SpecialAccountBalance / ldecBenefitOptionFactor, 2)) * ldecAnnunityAdjustmentMultiplier, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                        this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.LIFE_ANNUTIY,
                                                                        ldecAnnunityAdjustmentMultiplier == 0 ? Convert.ToDecimal(Math.Round(ldecLocal52SpecialAccountBalance / ldecBenefitOptionFactor, 2)): Convert.ToDecimal(Math.Round(ldecLocal52SpecialAccountBalance / ldecBenefitOptionFactor, 2)) * ldecAnnunityAdjustmentMultiplier, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_TRUE, busConstant.BOOL_FALSE);


                                }
                                else
                                {
                                    lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, ldecBenefitOptionFactor, Convert.ToDecimal(Math.Round(ldecLocal52SpecialAccountBalance / ldecBenefitOptionFactor, 2)), busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                        this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.LIFE_ANNUTIY,
                                                                        Convert.ToDecimal(Math.Round(ldecLocal52SpecialAccountBalance / ldecBenefitOptionFactor, 2)), busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_TRUE, busConstant.BOOL_FALSE);
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

                            if (this.iblnCalculateL161SplAccBenefit)
                            {
                                //local 161 Special Account Balance
                                lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                                if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE)
                                {
                                    lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, ldecBenefitOptionFactor, ldecAnnunityAdjustmentMultiplier == 0 ? Convert.ToDecimal(Math.Round(ldecLocal161SpecialAccountBalance / ldecBenefitOptionFactor, 2)): Convert.ToDecimal(Math.Round(ldecLocal161SpecialAccountBalance / ldecBenefitOptionFactor, 2)) * ldecAnnunityAdjustmentMultiplier, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                        this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.LIFE_ANNUTIY,
                                                                        ldecAnnunityAdjustmentMultiplier == 0 ? Convert.ToDecimal(Math.Round(ldecLocal161SpecialAccountBalance / ldecBenefitOptionFactor, 2)): Convert.ToDecimal(Math.Round(ldecLocal161SpecialAccountBalance / ldecBenefitOptionFactor, 2)) * ldecAnnunityAdjustmentMultiplier, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_TRUE);
                                }
                                else
                                {
                                    lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, ldecBenefitOptionFactor, Convert.ToDecimal(Math.Round(ldecLocal161SpecialAccountBalance / ldecBenefitOptionFactor, 2)), busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                        this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.LIFE_ANNUTIY,
                                                                        Convert.ToDecimal(Math.Round(ldecLocal161SpecialAccountBalance / ldecBenefitOptionFactor, 2)), busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_TRUE);
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
                            #endregion
                        }
                        break;

                    case busConstant.LUMP_SUM:
                        #region LUMPSUM
                        // Lumpsum Benefit Option
                        // No factor. The Lump sum for IAP will be the total IAP Balance itself.
                        lintPlanBenefitId = ibusCalculation.GetPlanBenefitId(busConstant.IAP_PLAN_ID, busConstant.LUMP_SUM);

                        if (this.iblnCalculateIAPBenefit)
                        {
                            lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                            lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, 1, ldecIAPBalance, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                    this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.LUMP_SUM, busConstant.ZERO_DECIMAL,
                                                                    busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE);

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

                        if (this.iblnCalculateL52SplAccBenefit)
                        {
                            //Local 52 Special Account Balance
                            lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                            lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, 1, ldecLocal52SpecialAccountBalance, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                    this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.LUMP_SUM, busConstant.ZERO_DECIMAL,
                                                                   busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_TRUE, busConstant.BOOL_FALSE);

                            if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL)
                            {
                                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID && item.icdoBenefitCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
                            }
                            else
                            {
                                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
                            }
                        }

                        if (this.iblnCalculateL161SplAccBenefit)
                        {
                            //Local 161 Special Account Balance
                            lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                            lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, 1, ldecLocal161SpecialAccountBalance, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                    this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.LUMP_SUM, busConstant.ZERO_DECIMAL,
                                                                    busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_TRUE);

                            if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL)
                            {
                                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID && item.icdoBenefitCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
                            }
                            else
                            {
                                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
                            }
                        }
                        #endregion
                        break;

                    default:
                        break;
                }
            }
        }


        public void GetBenefitAmountForEachYearAfterRetirement(busBenefitCalculationHeader abusBenefitCalculationHeader, busPayeeAccount abusPayeeAccount, string astrBenefitOptionValue, DateTime adtBatchRunDate, DateTime adtSpouseDOB, DateTime adtBenefitEffectiveDate, bool ablnStartFromRetirement, decimal adecPrevBenefitAmount, decimal adecPrevLifeBenefitAmount, DateTime? adtTermCertainDate = null)
        {
            //PIR 894
            bool lblnPopupToLife = false;
            DateTime ldtJointAnnuitantDOD = DateTime.MinValue;
            DataTable ldtbBenefitOpValue = null;
            

            if (abusBenefitCalculationHeader.iclbBenefitCalculationDetail.FirstOrDefault().iclbBenefitCalculationYearlyDetail.IsNullOrEmpty())
                return;

            Collection<busBenefitCalculationYearlyDetail> lclbReemployedYearlyDetail = new Collection<busBenefitCalculationYearlyDetail>();
            busBenefitCalculationOptions lbusExistingbenefitcalculationOption = abusBenefitCalculationHeader.iclbBenefitCalculationDetail.FirstOrDefault().iclbBenefitCalculationOptions.FirstOrDefault();
            busBenefitCalculationOptions lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
            decimal ldecSurvivorAmount = decimal.Zero;
            decimal ldecFactor = decimal.Zero;
            decimal ldecBenefitAmount = adecPrevBenefitAmount;
            decimal ldecLifeBenefitAmount = adecPrevLifeBenefitAmount; //PIR 894
            //RequestID: 72091
            decimal ldecPopupFactor = decimal.Zero;
            decimal ldecPopupBenefitAmount = decimal.Zero;

            //PIR 894
            if (astrBenefitOptionValue == busConstant.LIFE)
            {
                ldtbBenefitOpValue = busBase.Select("cdoPayeeAccount.GetBenefitOptionValueFromBenefitDtlId", new object[1] { abusPayeeAccount.icdoPayeeAccount.benefit_application_detail_id });
            }

            //PIR 973
            int lintParticipantAgeAtRetirement = Convert.ToInt32(Math.Floor(busGlobalFunctions.CalculatePersonAge(abusBenefitCalculationHeader.ibusPerson.icdoPerson.idtDateofBirth, abusBenefitCalculationHeader.icdoBenefitCalculationHeader.retirement_date)));
            decimal ldecERF = 1;
            if (ablnStartFromRetirement)
            {
                //PIR 894
                if (ldtbBenefitOpValue != null && ldtbBenefitOpValue.Rows.Count > 0)
                {
                    if (!Convert.ToString(ldtbBenefitOpValue.Rows[0][enmPerson.date_of_death.ToString().ToUpper()]).IsNullOrEmpty()
                        && Convert.ToDateTime(ldtbBenefitOpValue.Rows[0][enmPerson.date_of_death.ToString().ToUpper()]).Year >
                        abusBenefitCalculationHeader.iclbBenefitCalculationDetail.FirstOrDefault().iclbBenefitCalculationYearlyDetail.OrderByDescending(item => item.icdoBenefitCalculationYearlyDetail.plan_year).Where(
                                item => (item.icdoBenefitCalculationYearlyDetail.reemployed_flag == busConstant.FLAG_YES)).FirstOrDefault().icdoBenefitCalculationYearlyDetail.plan_year
                                && lbusExistingbenefitcalculationOption.icdoBenefitCalculationOptions.pop_up_option_factor > 0M)
                    {
                        abusBenefitCalculationHeader.iclbBenefitCalculationDetail.FirstOrDefault().iclbBenefitCalculationYearlyDetail.OrderByDescending(item => item.icdoBenefitCalculationYearlyDetail.plan_year).Where(
                            item => (item.icdoBenefitCalculationYearlyDetail.reemployed_flag == busConstant.FLAG_YES)).FirstOrDefault().icdoBenefitCalculationYearlyDetail.benefit_option_factor = lbusExistingbenefitcalculationOption.icdoBenefitCalculationOptions.pop_up_option_factor;

                        //RequestID: 72091
                        //ldecBenefitAmount = Math.Round(adecPrevBenefitAmount * lbusExistingbenefitcalculationOption.icdoBenefitCalculationOptions.pop_up_option_factor,2);
                        ldecBenefitAmount = Math.Round(adecPrevBenefitAmount, 2);
                    }
                }
                else
                    abusBenefitCalculationHeader.iclbBenefitCalculationDetail.FirstOrDefault().iclbBenefitCalculationYearlyDetail.OrderByDescending(item => item.icdoBenefitCalculationYearlyDetail.plan_year).Where(
                                item => !(item.icdoBenefitCalculationYearlyDetail.reemployed_flag == busConstant.FLAG_YES)).FirstOrDefault().icdoBenefitCalculationYearlyDetail.benefit_option_factor = lbusExistingbenefitcalculationOption.icdoBenefitCalculationOptions.benefit_option_factor;

                lclbReemployedYearlyDetail = abusBenefitCalculationHeader.iclbBenefitCalculationDetail.FirstOrDefault().iclbBenefitCalculationYearlyDetail.Where(item => item.icdoBenefitCalculationYearlyDetail.reemployed_flag == busConstant.FLAG_YES).ToList().ToCollection();
            }
            else
            {
                DateTime adtReemployedYearsFrom = new DateTime();
                if (abusBenefitCalculationHeader.iclbBenefitCalculationDetail.FirstOrDefault().icdoBenefitCalculationDetail.reemployed_accrued_benefit_effective_date != DateTime.MinValue)
                {
                    int lintYear = abusBenefitCalculationHeader.iclbBenefitCalculationDetail.FirstOrDefault().icdoBenefitCalculationDetail.reemployed_accrued_benefit_effective_date.Year;
                    lclbReemployedYearlyDetail = abusBenefitCalculationHeader.iclbBenefitCalculationDetail.FirstOrDefault().iclbBenefitCalculationYearlyDetail.Where(item => item.icdoBenefitCalculationYearlyDetail.reemployed_flag == busConstant.FLAG_YES && Convert.ToInt32(item.icdoBenefitCalculationYearlyDetail.plan_year) >= lintYear).ToList().ToCollection();
                }
                else
                {
                    lclbReemployedYearlyDetail = abusBenefitCalculationHeader.iclbBenefitCalculationDetail.FirstOrDefault().iclbBenefitCalculationYearlyDetail.Where(item => item.icdoBenefitCalculationYearlyDetail.reemployed_flag == busConstant.FLAG_YES && Convert.ToInt32(item.icdoBenefitCalculationYearlyDetail.plan_year) == adtBenefitEffectiveDate.AddYears(-1).Year).ToList().ToCollection();
                }
            }
            if (!lclbReemployedYearlyDetail.IsNullOrEmpty())
            {
                int lintParticipantAge = 0;
                int lintSpouseAge = 0;
                int lintReferenceNumber = 10;
                
                DateTime adtAgeAtDate = new DateTime();
                bool lblnflag = false;

                //PIR 894
                if (ldtbBenefitOpValue != null && ldtbBenefitOpValue.Rows.Count > 0)
                {
                        if (!Convert.ToString(ldtbBenefitOpValue.Rows[0][enmPerson.date_of_death.ToString().ToUpper()]).IsNullOrEmpty()
                            && Convert.ToDateTime(ldtbBenefitOpValue.Rows[0][enmPerson.date_of_death.ToString().ToUpper()]).Year >= lclbReemployedYearlyDetail.OrderBy(item=>item.icdoBenefitCalculationYearlyDetail.plan_year).FirstOrDefault().icdoBenefitCalculationYearlyDetail.plan_year)
                        {
                            lblnPopupToLife = true;
                            if (!Convert.ToString(ldtbBenefitOpValue.Rows[0][enmPlanBenefitXr.benefit_option_value.ToString().ToUpper()]).IsNullOrEmpty())
                                astrBenefitOptionValue = Convert.ToString(ldtbBenefitOpValue.Rows[0][enmPlanBenefitXr.benefit_option_value.ToString().ToUpper()]);

                            ldtJointAnnuitantDOD = Convert.ToDateTime(ldtbBenefitOpValue.Rows[0][enmPerson.date_of_death.ToString().ToUpper()]);
                        }
                }

                foreach (busBenefitCalculationYearlyDetail lbusbenefitCalculationYearly in lclbReemployedYearlyDetail)
                {
                    lintParticipantAge = 0;
                    lintSpouseAge = 0;

                    //PIR 894
                    if(lblnPopupToLife && lbusbenefitCalculationYearly.icdoBenefitCalculationYearlyDetail.plan_year >= ldtJointAnnuitantDOD.Year)
                    {
                        astrBenefitOptionValue = busConstant.LIFE;
                        lblnPopupToLife = false;
                    }

                    //PIR 627 10292015
                    if (lclbReemployedYearlyDetail.
                        Where(t => t.icdoBenefitCalculationYearlyDetail.plan_year == lbusbenefitCalculationYearly.icdoBenefitCalculationYearlyDetail.plan_year).Count() > 1
                        && this.ibusPerson.icdoPerson.date_of_birth.AddYears(65).Year == Convert.ToInt32(lbusbenefitCalculationYearly.icdoBenefitCalculationYearlyDetail.plan_year))
                    {
                        if (!lblnflag)
                        {
                            lblnflag = true;
                            if (abusBenefitCalculationHeader.ibusPerson.icdoPerson.idtDateofBirth.Day == 1)
                            {
                                adtAgeAtDate = new DateTime(Convert.ToInt32(lbusbenefitCalculationYearly.icdoBenefitCalculationYearlyDetail.plan_year),
                                    abusBenefitCalculationHeader.ibusPerson.icdoPerson.idtDateofBirth.Month,
                                    abusBenefitCalculationHeader.ibusPerson.icdoPerson.idtDateofBirth.Day);
                            }
                            else
                            {
                                adtAgeAtDate = abusBenefitCalculationHeader.ibusPerson.icdoPerson.idtDateofBirth.AddYears(65).GetLastDayofMonth().AddDays(1);
                            }
                        }
                        else
                        {
                            lblnflag = false;
                        }
                    }

                    if (!lblnflag)
                        adtAgeAtDate = new DateTime(Convert.ToInt32(lbusbenefitCalculationYearly.icdoBenefitCalculationYearlyDetail.plan_year), 01, 01).AddYears(1);
                    
                    //PIR 973
                    lintParticipantAge = Convert.ToInt32(Math.Floor(busGlobalFunctions.CalculatePersonAge(abusBenefitCalculationHeader.ibusPerson.icdoPerson.idtDateofBirth, adtAgeAtDate)));
                    lintSpouseAge = Convert.ToInt32(Math.Floor(busGlobalFunctions.CalculatePersonAge(adtSpouseDOB, adtAgeAtDate)));
                    
                    //PROD PIR 275
                    //PIR 973
                    lintReferenceNumber = 10 - (lintParticipantAge - lintParticipantAgeAtRetirement);
                    ldecFactor = ibusCalculation.GetFactor(abusBenefitCalculationHeader, astrBenefitOptionValue, lintSpouseAge, lintParticipantAge, lintReferenceNumber, busConstant.BOOL_TRUE); // Optional parameters added as per PIR 275 Requirement -- Need to take factors for different table
                    ldecBenefitAmount += Math.Round(lbusbenefitCalculationYearly.icdoBenefitCalculationYearlyDetail.accrued_benefit_amount * ldecFactor, 3);
                    ldecLifeBenefitAmount += Math.Round(lbusbenefitCalculationYearly.icdoBenefitCalculationYearlyDetail.accrued_benefit_amount, 3); //PIR 894
                    lbusbenefitCalculationYearly.icdoBenefitCalculationYearlyDetail.benefit_option_factor = ldecFactor;
                    //PIR 627
                    lbusbenefitCalculationYearly.icdoBenefitCalculationYearlyDetail.total_accrued_benefit = ldecBenefitAmount;
                    //PIR - 930
                    lbusbenefitCalculationYearly.icdoBenefitCalculationYearlyDetail.actuarial_equivalent_amount = ldecBenefitAmount;
                    //PIR 894
                    lbusbenefitCalculationYearly.icdoBenefitCalculationYearlyDetail.popup_to_life_amount = ldecLifeBenefitAmount;
                    lbusbenefitCalculationYearly.icdoBenefitCalculationYearlyDetail.benefit_as_of_det_date = ldecBenefitAmount;

                    //RequestID: 72091
                    if(lbusbenefitCalculationYearly.icdoBenefitCalculationYearlyDetail.plan_year < ldtJointAnnuitantDOD.Year)
                    {                        
                        ldecPopupFactor = ldecFactor;
                        ldecPopupBenefitAmount = ldecBenefitAmount;
                    }                    
                }
            }

            if (lbusExistingbenefitcalculationOption.icdoBenefitCalculationOptions.pop_up_benefit_amount.IsNotNull() && lbusExistingbenefitcalculationOption.icdoBenefitCalculationOptions.pop_up_benefit_amount != 0)
            {
                ldecSurvivorAmount = ibusCalculation.GetSurvivorAmountFromBenefitAmount(ldecBenefitAmount, busConstant.LIFE);
                lbusBenefitCalculationOptions.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.MPIPP_PLAN_ID, abusPayeeAccount.icdoPayeeAccount.istrBenefitOptionValue), lbusExistingbenefitcalculationOption.icdoBenefitCalculationOptions.benefit_option_factor, ldecLifeBenefitAmount,
                                                        busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER, this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                        abusPayeeAccount.icdoPayeeAccount.istrBenefitOptionValue, ldecSurvivorAmount);
            }
            else
            {
                ldecSurvivorAmount = ibusCalculation.GetSurvivorAmountFromBenefitAmount(ldecBenefitAmount, astrBenefitOptionValue);
                lbusBenefitCalculationOptions.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.MPIPP_PLAN_ID, abusPayeeAccount.icdoPayeeAccount.istrBenefitOptionValue), lbusExistingbenefitcalculationOption.icdoBenefitCalculationOptions.benefit_option_factor, ldecBenefitAmount,
                                                        busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER, this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                        abusPayeeAccount.icdoPayeeAccount.istrBenefitOptionValue, ldecSurvivorAmount);

            }

            //PIR 894
            //lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.pop_up_option_factor = lbusExistingbenefitcalculationOption.icdoBenefitCalculationOptions.pop_up_option_factor;
            //lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.pop_up_benefit_amount = lbusExistingbenefitcalculationOption.icdoBenefitCalculationOptions.pop_up_benefit_amount;

            //RequestID: 72091
            if (ldecPopupBenefitAmount != decimal.Zero)
            {
                lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.pop_up_option_factor = ldecPopupFactor;
                lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.pop_up_benefit_amount = ldecPopupBenefitAmount;
                lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.pop_up_option_factor_at_ret = lbusExistingbenefitcalculationOption.icdoBenefitCalculationOptions.pop_up_option_factor_at_ret;
            }
            else
            {
                lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.pop_up_option_factor = lbusExistingbenefitcalculationOption.icdoBenefitCalculationOptions.pop_up_option_factor;
                lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.pop_up_benefit_amount = lbusExistingbenefitcalculationOption.icdoBenefitCalculationOptions.pop_up_benefit_amount;
                lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.pop_up_option_factor_at_ret = lbusExistingbenefitcalculationOption.icdoBenefitCalculationOptions.pop_up_option_factor_at_ret;
            }

            abusBenefitCalculationHeader.iclbBenefitCalculationDetail.FirstOrDefault().iclbBenefitCalculationOptions.Clear();

            abusBenefitCalculationHeader.iclbBenefitCalculationDetail.FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);

        }


        public void GetBenefitAmountForEachYearAfterRetirement(busBenefitCalculationHeader abusBenefitCalculationHeader, string astrBenefitOptionValue, DateTime adtBatchRunDate, DateTime adtParticipantDOB, DateTime adtSpouseDOB, DateTime adtBenefitEffectiveDate, bool ablnStartFromRetirement, decimal adecReferenceAccruedBenefit, decimal adecPrevBenefitAmount = decimal.Zero, DateTime? adtTermCertainDate = null)
        {
            if (abusBenefitCalculationHeader.iclbBenefitCalculationDetail.FirstOrDefault().iclbBenefitCalculationYearlyDetail.IsNullOrEmpty())
                return;

            Collection<busBenefitCalculationYearlyDetail> lclbReemployedYearlyDetail = new Collection<busBenefitCalculationYearlyDetail>();

            decimal ldecSurvivorAmount = decimal.Zero;
            decimal ldecFactorAtRet = decimal.Zero;
            decimal ldecFactor = decimal.Zero;
            decimal ldecBenefitAmount = adecReferenceAccruedBenefit;
            int lintParticipantAgeAtRetirement = Convert.ToInt32(busGlobalFunctions.CalculatePersonAge(adtParticipantDOB, abusBenefitCalculationHeader.icdoBenefitCalculationHeader.retirement_date));
            decimal ldecERF = 1;
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
                if (abusBenefitCalculationHeader.iclbBenefitCalculationDetail.FirstOrDefault().icdoBenefitCalculationDetail.early_reduced_benefit_amount == decimal.Zero)
                {
                    abusBenefitCalculationHeader.iclbBenefitCalculationDetail.FirstOrDefault().icdoBenefitCalculationDetail.early_reduced_benefit_amount = adecReferenceAccruedBenefit;
                }
                int lintSpouseAge = 0;
                lintSpouseAge = Convert.ToInt32(busGlobalFunctions.CalculatePersonAge(adtSpouseDOB, abusBenefitCalculationHeader.icdoBenefitCalculationHeader.retirement_date));
                ldecFactorAtRet = ibusCalculation.GetFactor(this, astrBenefitOptionValue, lintSpouseAge, lintParticipantAgeAtRetirement);
                ldecBenefitAmount = Math.Round(ldecBenefitAmount * ldecFactorAtRet, 2);
                if ((abusBenefitCalculationHeader.iclbBenefitCalculationDetail.FirstOrDefault().iclbBenefitCalculationYearlyDetail.OrderByDescending(item => item.icdoBenefitCalculationYearlyDetail.plan_year).Where(
                                    item => !(item.icdoBenefitCalculationYearlyDetail.reemployed_flag == busConstant.FLAG_YES))).Count() > 0)
                {
                    abusBenefitCalculationHeader.iclbBenefitCalculationDetail.FirstOrDefault().iclbBenefitCalculationYearlyDetail.OrderByDescending(item => item.icdoBenefitCalculationYearlyDetail.plan_year).Where(
                                        item => !(item.icdoBenefitCalculationYearlyDetail.reemployed_flag == busConstant.FLAG_YES)).FirstOrDefault().icdoBenefitCalculationYearlyDetail.benefit_option_factor = ldecFactorAtRet;
                }
                else
                {
                    busBenefitCalculationYearlyDetail lbusBenefitCalculationYearlyDetail = new busBenefitCalculationYearlyDetail { icdoBenefitCalculationYearlyDetail = new cdoBenefitCalculationYearlyDetail() };
                    lbusBenefitCalculationYearlyDetail.LoadData(0, 0, abusBenefitCalculationHeader.icdoBenefitCalculationHeader.retirement_date.Year,
                                                                0, 0, 0, 0,
                                                                0, 0, 0, adecReferenceAccruedBenefit, adecReferenceAccruedBenefit, busConstant.FLAG_NO);
                    lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.benefit_option_factor = ldecFactorAtRet;
                    abusBenefitCalculationHeader.iclbBenefitCalculationDetail.FirstOrDefault().iclbBenefitCalculationYearlyDetail.Add(lbusBenefitCalculationYearlyDetail);
                }
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
                //decimal ldecFactor = decimal.Zero;
                DateTime adtAgeAtDate = new DateTime();
                int lintReferenceNumber = 10;
                foreach (busBenefitCalculationYearlyDetail lbusbenefitCalculationYearly in lclbReemployedYearlyDetail)
                {
                    lintParticipantAge = 0;
                    lintSpouseAge = 0;
                    adtAgeAtDate = new DateTime(Convert.ToInt32(lbusbenefitCalculationYearly.icdoBenefitCalculationYearlyDetail.plan_year), 01, 01).AddYears(1);
                    lintParticipantAge = Convert.ToInt32(busGlobalFunctions.CalculatePersonAge(adtParticipantDOB, adtAgeAtDate));
                    lintSpouseAge = Convert.ToInt32(busGlobalFunctions.CalculatePersonAge(adtSpouseDOB, adtAgeAtDate));
                    //ldecFactor = ibusCalculation.GetFactor(abusBenefitCalculationHeader, astrBenefitOptionValue, lintSpouseAge, lintParticipantAge);
                    //PROD PIR 275
                    lintReferenceNumber = 10 - Convert.ToInt32(busGlobalFunctions.CalculatePersonAgeInDec(abusBenefitCalculationHeader.icdoBenefitCalculationHeader.retirement_date, adtAgeAtDate));
                    ldecFactor = ibusCalculation.GetFactor(abusBenefitCalculationHeader, astrBenefitOptionValue, lintSpouseAge, lintParticipantAge, lintReferenceNumber, busConstant.BOOL_TRUE); // Optional parameters added as per PIR 275 Requirement -- Need to take factors for different table
                    //PROD PIR 275
                    ldecBenefitAmount += Math.Round(lbusbenefitCalculationYearly.icdoBenefitCalculationYearlyDetail.accrued_benefit_amount * ldecFactor, 3);
                    lbusbenefitCalculationYearly.icdoBenefitCalculationYearlyDetail.benefit_option_factor = ldecFactor;
                    lbusbenefitCalculationYearly.icdoBenefitCalculationYearlyDetail.idecTotalBenefitAmount = ldecBenefitAmount;
                    //PIR 627
                    lbusbenefitCalculationYearly.icdoBenefitCalculationYearlyDetail.total_accrued_benefit = ldecBenefitAmount;
                }
            }

            //adecFinalBenefitAmount = ldecBenefitAmount;
            ldecSurvivorAmount = ibusCalculation.GetSurvivorAmountFromBenefitAmount(ldecBenefitAmount, astrBenefitOptionValue);
            busBenefitCalculationOptions lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
            lbusBenefitCalculationOptions.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.MPIPP_PLAN_ID, astrBenefitOptionValue), ldecFactorAtRet, ldecBenefitAmount,
                                                    busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER, this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                    astrBenefitOptionValue, ldecSurvivorAmount);

            abusBenefitCalculationHeader.iclbBenefitCalculationDetail.FirstOrDefault().iclbBenefitCalculationOptions.Clear();

            abusBenefitCalculationHeader.iclbBenefitCalculationDetail.FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);

        }


        public decimal CalculateFinalBenefitForPensionBenefitOptions(decimal adecFinalAccruedBenefitAmount, string astrBenefitOptionValue, int aintPlanId, bool ablnReEmployed = false,bool ablnConvertBenefitOption = false,string astrOriginalBenefitOption = "") //PIR 894
        {
            int lintParticipantAge = Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge);
            int lintSurvivorAge = Convert.ToInt32(Math.Floor(icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement));
            decimal ldecBenefitOptionFactor = busConstant.ZERO_DECIMAL;
            decimal ldecFinalBenefitAmount = busConstant.ZERO_DECIMAL;
            decimal ldecTotalLocalLumpsumAmount = busConstant.ZERO_DECIMAL;
            decimal ldecLumpSumBenefitAmount = decimal.Zero;
            busBenefitCalculationOptions lbusBenefitCalculationOptions;

           
            //Ticket - 61531
            ldecBenefitOptionFactor = Math.Round(this.GetLumpsumBenefitFactor((int)this.icdoBenefitCalculationHeader.idecParticipantFullAge, this.icdoBenefitCalculationHeader.retirement_date.Year) * 12,3);
            ldecLumpSumBenefitAmount = Convert.ToDecimal(Math.Round(adecFinalAccruedBenefitAmount * ldecBenefitOptionFactor, 2));
            //RequestID: 72830
            int lintParticipantAgeAtMD = 0;
            int lintSurvivorAgeAtMD = 0;            
            busPerson ibusMDBeneficiary;
           // Ticket# 73070
            if (ibusBenefitApplication.icdoBenefitApplication.min_distribution_date != DateTime.MinValue && (ibusBenefitApplication.icdoBenefitApplication.min_distribution_flag == "Y" || ibusBenefitApplication.icdoBenefitApplication.converted_min_distribution_flag == "Y"))
            {
                                                
                lintParticipantAgeAtMD = Convert.ToInt32(busGlobalFunctions.CalculatePersonAge(ibusPerson.icdoPerson.date_of_birth, ibusBenefitApplication.icdoBenefitApplication.min_distribution_date));

                if (icdoBenefitCalculationHeader.beneficiary_person_id != 0)
                {
                    ibusMDBeneficiary = new busPerson { icdoPerson = new cdoPerson() };
                    ibusMDBeneficiary.FindPerson(icdoBenefitCalculationHeader.beneficiary_person_id);
                    lintSurvivorAgeAtMD = Convert.ToInt32(busGlobalFunctions.CalculatePersonAge(ibusMDBeneficiary.icdoPerson.date_of_birth, ibusBenefitApplication.icdoBenefitApplication.min_distribution_date));
                }
            }

            if (!ablnReEmployed)
            {

                // Calculate the total benefit amount of MPIPP & Locals
                // Need to call the Local Code to fetch the Local Frozen Benefit Amount with ERF
                ldecTotalLocalLumpsumAmount = ibusCalculation.GetLocalLumpsumBenefitAmount(icdoBenefitCalculationHeader.age, ibusBenefitApplication, ibusPerson,
                                           icdoBenefitCalculationHeader.retirement_date, iclbPersonAccountRetirementContribution);
                // Calculate the Monthly Exclusion Amount & Minimum Guarantee

                //RequestID: 72830 ,Ticket# 73070  
                if (ibusBenefitApplication.icdoBenefitApplication.min_distribution_date != DateTime.MinValue && (ibusBenefitApplication.icdoBenefitApplication.min_distribution_flag == "Y" || ibusBenefitApplication.icdoBenefitApplication.converted_min_distribution_flag == "Y"))
                {
                    ibusCalculation.CalculateMEAAndMG(adecFinalAccruedBenefitAmount, ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.MPIPP).First(),
                                        ldecLumpSumBenefitAmount, this.icdoBenefitCalculationHeader.iintPlanId, lintParticipantAgeAtMD,  //Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge),
                                        this.icdoBenefitCalculationHeader.retirement_date, lintSurvivorAgeAtMD,
                                        this.ibusBenefitApplication.QualifiedSpouseExists, busConstant.BOOL_FALSE, null, this.iclbBenefitCalculationDetail,
                                        this.iclbPersonAccountRetirementContribution, icdoBenefitCalculationHeader.calculation_type_value,
                                        this.icdoBenefitCalculationHeader.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, string.Empty);
                }
                else
                {
                    ibusCalculation.CalculateMEAAndMG(adecFinalAccruedBenefitAmount, ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.MPIPP).First(),
                                        ldecLumpSumBenefitAmount, this.icdoBenefitCalculationHeader.iintPlanId, Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge),
                                        this.icdoBenefitCalculationHeader.retirement_date, lintSurvivorAge,
                                        this.ibusBenefitApplication.QualifiedSpouseExists, busConstant.BOOL_FALSE, null, this.iclbBenefitCalculationDetail,
                                        this.iclbPersonAccountRetirementContribution, icdoBenefitCalculationHeader.calculation_type_value,
                                        this.icdoBenefitCalculationHeader.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, string.Empty);
                }

                // Calculate the Final Benefit Amounts for all Benefit Options

                // Calculate the Lumpsum Benefit Option
            }

            switch (astrBenefitOptionValue)
            {
                case busConstant.CodeValueAll:

                    decimal ldecTotalPensionLocalBenefitAmount = ldecLumpSumBenefitAmount + ldecTotalLocalLumpsumAmount;

                    // Then check the total of MPI Accrued Benefit Amount and Local Benefit Amount
                    if (ldecTotalPensionLocalBenefitAmount <= busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT)
                    {
                        // Participant is eligible only for Lumpsum
                        lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                        lbusBenefitCalculationOptions.LoadData(ibusCalculation.GetPlanBenefitId(aintPlanId, busConstant.LUMP_SUM), ldecBenefitOptionFactor,
                                                                ldecLumpSumBenefitAmount, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.LUMP_SUM, busConstant.ZERO_DECIMAL);

                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
                        break;
                    }

                    if (ldecTotalPensionLocalBenefitAmount > busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT && ldecTotalPensionLocalBenefitAmount < 10000)
                    {
                        // Participant is eligible for Lumpsum and Annuity
                        // Lumpsum Benefit Option
                        lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                        lbusBenefitCalculationOptions.LoadData(ibusCalculation.GetPlanBenefitId(aintPlanId, busConstant.LUMP_SUM), ldecBenefitOptionFactor,
                                                                ldecLumpSumBenefitAmount, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.LUMP_SUM, busConstant.ZERO_DECIMAL);

                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
                       
                        // Annuity Benefit Option
                        int lintPlanBenefitId = ibusCalculation.GetPlanBenefitId(aintPlanId, busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY);
                        ldecBenefitOptionFactor = ibusCalculation.GetBenefitOptionFactor(this.icdoBenefitCalculationHeader.benefit_type_value, lintPlanBenefitId, lintParticipantAge, busConstant.ZERO_INT);
                        //PIR-940
                        if (ldecBenefitOptionFactor > 1)
                        {
                            ldecBenefitOptionFactor = 1;
                        }
                        lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                        lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, ldecBenefitOptionFactor, Convert.ToDecimal(Math.Round(adecFinalAccruedBenefitAmount * ldecBenefitOptionFactor, 2)),
                                                                busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER, this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, Convert.ToDecimal(Math.Round(adecFinalAccruedBenefitAmount * ldecBenefitOptionFactor, 2)));

                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);

                        // Life Annuity Benefit Option
                        ldecBenefitOptionFactor = 1;
                        lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                        lbusBenefitCalculationOptions.LoadData(ibusCalculation.GetPlanBenefitId(aintPlanId, busConstant.LIFE_ANNUTIY), ldecBenefitOptionFactor,
                                                                adecFinalAccruedBenefitAmount, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.LIFE_ANNUTIY, busConstant.ZERO_DECIMAL);

                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);

                        if (this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth != DateTime.MinValue &&
                            ibusCalculation.CheckIfSpouse(this.icdoBenefitCalculationHeader.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id))
                        {
                            // Participant is also eligible for All Joint & Survivor Benefit Options
                            CalculateJointAndSurvivorBenefitOptions(adecFinalAccruedBenefitAmount);
                        }

                        break;
                    }

                    if (!ibusCalculation.CheckIfSpouse(this.icdoBenefitCalculationHeader.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id) || this.icdoBenefitCalculationHeader.beneficiary_person_id.IsNull() || this.icdoBenefitCalculationHeader.beneficiary_person_id <= 0)
                    {
                        // No Qualified Spouse exists. Hence Participant is eligible only for Annuity
                        int lintPlanBenefitId = ibusCalculation.GetPlanBenefitId(aintPlanId, busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY);
                        
                        ldecBenefitOptionFactor = ibusCalculation.GetBenefitOptionFactor(this.icdoBenefitCalculationHeader.benefit_type_value, lintPlanBenefitId, lintParticipantAge, busConstant.ZERO_INT);
                        //PIR-940
                        if(ldecBenefitOptionFactor>1)
                        {
                            ldecBenefitOptionFactor = 1;
                        }
                        lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                        lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, ldecBenefitOptionFactor, Convert.ToDecimal(Math.Round(adecFinalAccruedBenefitAmount * ldecBenefitOptionFactor, 2)),
                                                                busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER, this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, Convert.ToDecimal(Math.Round(adecFinalAccruedBenefitAmount * ldecBenefitOptionFactor, 2)));

                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);

                        // Life Annuity Benefit Option
                        ldecBenefitOptionFactor = 1;
                        lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                        lbusBenefitCalculationOptions.LoadData(ibusCalculation.GetPlanBenefitId(aintPlanId, busConstant.LIFE_ANNUTIY), ldecBenefitOptionFactor,
                                                                adecFinalAccruedBenefitAmount, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.LIFE_ANNUTIY, busConstant.ZERO_DECIMAL);

                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);

                        break;
                    }
                    else if (ldecTotalPensionLocalBenefitAmount > busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT)
                    {

                        decimal ldecBenefitAmount = busConstant.ZERO_DECIMAL;
                        // Participant is eligible for all the Joint & Survivor Benefit Options and Annuity
                        // Participant is also eligible for All Joint & Survivor Benefit Options
                        CalculateJointAndSurvivorBenefitOptions(adecFinalAccruedBenefitAmount);

                        // Annuity Benefit Option
                        int lintPlanBenefitId = ibusCalculation.GetPlanBenefitId(aintPlanId, busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY);
                        ldecBenefitOptionFactor = ibusCalculation.GetBenefitOptionFactor(this.icdoBenefitCalculationHeader.benefit_type_value, lintPlanBenefitId, lintParticipantAge, busConstant.ZERO_INT);
                       //PIR-940
                       if(ldecBenefitOptionFactor>1)
                        {
                            ldecBenefitOptionFactor = 1;
                        }
                        ldecBenefitAmount = Convert.ToDecimal(Math.Round(adecFinalAccruedBenefitAmount * ldecBenefitOptionFactor, 2));
                        lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                        lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, ldecBenefitOptionFactor, ldecBenefitAmount,
                                                                busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER, this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, Convert.ToDecimal(Math.Round(adecFinalAccruedBenefitAmount * ldecBenefitOptionFactor, 2)));


                        lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, this.icdoBenefitCalculationHeader.age, this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement);
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);

                        // Life Annuity Benefit Option
                        ldecBenefitOptionFactor = 1;
                        lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                        lbusBenefitCalculationOptions.LoadData(ibusCalculation.GetPlanBenefitId(aintPlanId, busConstant.LIFE_ANNUTIY), ldecBenefitOptionFactor,
                                                                adecFinalAccruedBenefitAmount, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.LIFE_ANNUTIY, busConstant.ZERO_DECIMAL);


                        lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.LIFE_ANNUTIY, this.icdoBenefitCalculationHeader.age, this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement);
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
                        break;
                    }
                    break;

                case busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY:
                    // Qualified Joint And 50% Survivor Annuity Benefit Option
                    ldecBenefitOptionFactor = 1;
                    double ldblBenefitOptionFactorJnS50 = 0.86 + 0.005 * (lintSurvivorAge - lintParticipantAge) + 0.006 * (65 - lintParticipantAge);
                    ldecBenefitOptionFactor = Convert.ToDecimal(Math.Round(ldblBenefitOptionFactorJnS50, 6));
                    //PIR-940
                    if(ldecBenefitOptionFactor>1)
                    {
                        ldecBenefitOptionFactor = 1;
                    }
                    ldecFinalBenefitAmount = Convert.ToDecimal(Math.Round(adecFinalAccruedBenefitAmount * ldecBenefitOptionFactor, 2));
                    lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                    lbusBenefitCalculationOptions.LoadData(ibusCalculation.GetPlanBenefitId(aintPlanId, busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY), ldecBenefitOptionFactor, ldecFinalBenefitAmount,
                                                            busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER, this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                            busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY, Convert.ToDecimal(Math.Round(adecFinalAccruedBenefitAmount * ldecBenefitOptionFactor * 0.50m, 2)));

                    // No Need to show the Relative Value for JOINT_50_PERCENT_SURVIVOR_ANNUITY Benefit option
                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
                    break;

                case busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY:
                    // Qualified Joint And 75% Survivor Annuity Benefit Option
                    ldecBenefitOptionFactor = 1;
                    double ldblBenefitOptionFactorJnS75 = 0.80 + 0.01 * (lintSurvivorAge - lintParticipantAge) + 0.006 * (65 - lintParticipantAge);
                    ldecBenefitOptionFactor = Convert.ToDecimal(Math.Round(ldblBenefitOptionFactorJnS75, 6));
                    //PIR-940
                    if (ldecBenefitOptionFactor > 1)
                    {
                        ldecBenefitOptionFactor = 1;
                    }

                    ldecFinalBenefitAmount = Convert.ToDecimal(Math.Round(adecFinalAccruedBenefitAmount * ldecBenefitOptionFactor, 2));
                    lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                    lbusBenefitCalculationOptions.LoadData(ibusCalculation.GetPlanBenefitId(aintPlanId, busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY), ldecBenefitOptionFactor, ldecFinalBenefitAmount,
                                                            busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER, this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                            busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, Convert.ToDecimal(Math.Round(adecFinalAccruedBenefitAmount * ldecBenefitOptionFactor * 0.75m, 2)));

                    lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, this.icdoBenefitCalculationHeader.age, this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement);
                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
                    break;

                case busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY:
                    // Qualified Joint And 100% Survivor Annuity Benefit Option
                    ldecBenefitOptionFactor = 1;
                    double ldblBenefitOptionFactorJnS100 = 0.75 + 0.01 * (lintSurvivorAge - lintParticipantAge) + 0.006 * (65 - lintParticipantAge);
                    ldecBenefitOptionFactor = Convert.ToDecimal(Math.Round(ldblBenefitOptionFactorJnS100, 6));
                    //PIR-940
                    if (ldecBenefitOptionFactor > 1)
                    {
                        ldecBenefitOptionFactor = 1;
                    }
                    ldecFinalBenefitAmount = Convert.ToDecimal(Math.Round(adecFinalAccruedBenefitAmount * ldecBenefitOptionFactor, 2));
                    lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                    lbusBenefitCalculationOptions.LoadData(ibusCalculation.GetPlanBenefitId(aintPlanId, busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY), ldecBenefitOptionFactor, ldecFinalBenefitAmount,
                                                            busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER, this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                            busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY, Convert.ToDecimal(Math.Round(adecFinalAccruedBenefitAmount * ldecBenefitOptionFactor, 2)));

                    lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY, this.icdoBenefitCalculationHeader.age, this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement);
                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
                    break;

                case busConstant.JOINT_50_PERCENT_POPUP_ANNUITY:
                    // Joint And 50% Survivor Pop-up Annuity Benefit Option
                    ldecBenefitOptionFactor = 1;
                    double ldblBenefitOptionFactorJnS50Pop = 0.83 + 0.007 * (lintSurvivorAge - lintParticipantAge) + 0.006 * (65 - lintParticipantAge);
                    ldecBenefitOptionFactor = Convert.ToDecimal(Math.Round(ldblBenefitOptionFactorJnS50Pop, 6));
                    //PIR-940
                    if (ldecBenefitOptionFactor > 1)
                    {
                        ldecBenefitOptionFactor = 1;
                    }
                    ldecFinalBenefitAmount = Convert.ToDecimal(Math.Round(adecFinalAccruedBenefitAmount * ldecBenefitOptionFactor, 2));
                    lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                    lbusBenefitCalculationOptions.LoadData(ibusCalculation.GetPlanBenefitId(aintPlanId, busConstant.JOINT_50_PERCENT_POPUP_ANNUITY), ldecBenefitOptionFactor, ldecFinalBenefitAmount,
                                                            busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER, this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                            busConstant.JOINT_50_PERCENT_POPUP_ANNUITY, Convert.ToDecimal(Math.Round(adecFinalAccruedBenefitAmount * ldecBenefitOptionFactor * 0.50m, 2)));

                    lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.JOINT_50_PERCENT_POPUP_ANNUITY, this.icdoBenefitCalculationHeader.age, this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement);
                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
                    break;

                case busConstant.JOINT_100_PERCENT_POPUP_ANNUITY:
                    // Joint And 100% Survivor Pop-up Annuity Benefit Option
                    ldecBenefitOptionFactor = 1;
                    double ldblBenefitOptionFactorJnS100Pop = 0.71 + 0.01 * (lintSurvivorAge - lintParticipantAge) + 0.008 * (65 - lintParticipantAge);
                    ldecBenefitOptionFactor = Convert.ToDecimal(Math.Round(ldblBenefitOptionFactorJnS100Pop, 6));
                    //PIR-940
                    if (ldecBenefitOptionFactor > 1)
                    {
                        ldecBenefitOptionFactor = 1;
                    }
                    ldecFinalBenefitAmount = Convert.ToDecimal(Math.Round(adecFinalAccruedBenefitAmount * ldecBenefitOptionFactor, 2));
                    lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                    lbusBenefitCalculationOptions.LoadData(ibusCalculation.GetPlanBenefitId(aintPlanId, busConstant.JOINT_100_PERCENT_POPUP_ANNUITY), ldecBenefitOptionFactor, ldecFinalBenefitAmount,
                                                            busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER, this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                            busConstant.JOINT_100_PERCENT_POPUP_ANNUITY, Convert.ToDecimal(Math.Round(adecFinalAccruedBenefitAmount * ldecBenefitOptionFactor, 2)));

                    lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.JOINT_100_PERCENT_POPUP_ANNUITY, this.icdoBenefitCalculationHeader.age, this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement);
                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
                    break;

                case busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY:
                    // Annuity Benefit Option
                    int lPlanBenefitId = ibusCalculation.GetPlanBenefitId(aintPlanId, busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY);
                    ldecBenefitOptionFactor = ibusCalculation.GetBenefitOptionFactor(this.icdoBenefitCalculationHeader.benefit_type_value, lPlanBenefitId, lintParticipantAge, busConstant.ZERO_INT);
                    //PIR-940
                    if (ldecBenefitOptionFactor > 1)
                    {
                        ldecBenefitOptionFactor = 1;
                    }
                    ldecFinalBenefitAmount = Convert.ToDecimal(Math.Round(adecFinalAccruedBenefitAmount * ldecBenefitOptionFactor, 2));
                    lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                    lbusBenefitCalculationOptions.LoadData(lPlanBenefitId, ldecBenefitOptionFactor, ldecFinalBenefitAmount,
                                                            busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER, this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                            busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, Convert.ToDecimal(Math.Round(adecFinalAccruedBenefitAmount * ldecBenefitOptionFactor, 2)));

                    lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, this.icdoBenefitCalculationHeader.age, this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement);
                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
                    break;

                case busConstant.LUMP_SUM:
                    // Lumpsum Benefit Option
                    ldecFinalBenefitAmount = ldecLumpSumBenefitAmount;
                    lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                    lbusBenefitCalculationOptions.LoadData(ibusCalculation.GetPlanBenefitId(aintPlanId, busConstant.LUMP_SUM), ldecBenefitOptionFactor,
                                                            ldecLumpSumBenefitAmount, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                            this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.LUMP_SUM, busConstant.ZERO_DECIMAL);

                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
                    break;

                case busConstant.LIFE_ANNUTIY:
                    // Life Annuity - reduction factors applied if taking any early retirement benefit other that un-reduced
                    ldecBenefitOptionFactor = 1;
                    ldecFinalBenefitAmount = adecFinalAccruedBenefitAmount;
                    lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                    lbusBenefitCalculationOptions.LoadData(ibusCalculation.GetPlanBenefitId(aintPlanId, busConstant.LIFE_ANNUTIY), ldecBenefitOptionFactor,
                                                            adecFinalAccruedBenefitAmount, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                            this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.LIFE_ANNUTIY, busConstant.ZERO_DECIMAL);

                    //PIR 894
                    if(ablnConvertBenefitOption)
                    {
                        if(astrOriginalBenefitOption == busConstant.JOINT_50_PERCENT_POPUP_ANNUITY)
                        {
                            decimal ldecPopBenefitOptionFactor = Convert.ToDecimal(Math.Round(0.83 + 0.007 * (lintSurvivorAge - lintParticipantAge) + 0.006 * (65 - lintParticipantAge),6));                          
                            
                            if (ldecPopBenefitOptionFactor > 1)
                            {
                                ldecPopBenefitOptionFactor = 1;
                            }
                            
                            lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.pop_up_option_factor = ldecPopBenefitOptionFactor;
                            lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.pop_up_benefit_amount = Convert.ToDecimal(Math.Round(adecFinalAccruedBenefitAmount * ldecPopBenefitOptionFactor, 2));
                            lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.pop_up_option_factor_at_ret = ldecPopBenefitOptionFactor; //RequestID: 72091
                        }
                        else if(astrOriginalBenefitOption == busConstant.JOINT_100_PERCENT_POPUP_ANNUITY)
                        {
                            decimal ldecPopBenefitOptionFactor = Convert.ToDecimal(Math.Round(0.71 + 0.01 * (lintSurvivorAge - lintParticipantAge) + 0.008 * (65 - lintParticipantAge), 6));
                            
                            if (ldecPopBenefitOptionFactor > 1)
                            {
                                ldecPopBenefitOptionFactor = 1;
                            }

                            lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.pop_up_option_factor = ldecPopBenefitOptionFactor;
                            lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.pop_up_benefit_amount = Convert.ToDecimal(Math.Round(adecFinalAccruedBenefitAmount * ldecPopBenefitOptionFactor, 2));
                            lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.pop_up_option_factor_at_ret = ldecPopBenefitOptionFactor; //RequestID: 72091
                        }
                    }

                    lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.LIFE_ANNUTIY, this.icdoBenefitCalculationHeader.age, this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement);
                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
                    break;

                default:
                    break;
            }

            return ldecFinalBenefitAmount;
        }


        #endregion Pension Calculation Business Logic

        #region GET MPIPHP Lumsum

        //Check with abhishek. plan incorrect
        protected decimal GetMPIPHPLumpSum()
        {
            decimal ldecERF = busConstant.ZERO_DECIMAL;
            decimal ldecFinalAccruedBenefitAmount = busConstant.ZERO_DECIMAL;
            decimal ldecUnreducedAccruedBenefitAmount = busConstant.ZERO_DECIMAL;
            decimal ldecLateAdjustmentAmt = busConstant.ZERO_DECIMAL;
            int lintMPIQualifiedYear = busConstant.ZERO_INT;

            string lstrRateType = string.Empty;
            lstrRateType = ibusCalculation.DeterminePlanRate(this.ibusBenefitApplication.ibusPerson, this.ibusBenefitApplication.aclbPersonWorkHistory_MPI, icdoBenefitCalculationHeader.retirement_date);

            if (iclbcdoPlanBenefitRate == null)
                iclbcdoPlanBenefitRate = new Collection<cdoPlanBenefitRate>();

            DataTable ldtbPlanBenefitRate = busBase.Select("cdoPlanBenefitRate.Lookup", new object[] { });
            iclbcdoPlanBenefitRate = cdoDummyWorkData.GetCollection<cdoPlanBenefitRate>(ldtbPlanBenefitRate);
            //PIR 355
            iclbcdoPlanBenefitRate = ibusCalculation.BenefitRateScheduleSpecialCase(this.icdoBenefitCalculationHeader.retirement_date, iclbcdoPlanBenefitRate);

            string lstrRetirementSubType = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.MPIPP).First().icdoPersonAccount.istrRetirementSubType;
            this.ibusBenefitApplication.aclbPersonWorkHistory_MPI = this.ibusBenefitApplication.aclbPersonWorkHistory_MPI.OrderByDescending(item => item.year).ToList().ToCollection();
            ibusCalculation.GetRateForBenefitCalculation(this.ibusPerson, this.ibusPerson.icdoPerson.istrSSNNonEncrypted, lstrRateType, this.icdoBenefitCalculationHeader.retirement_date,
                                                         this.iclbcdoPlanBenefitRate, this.ibusBenefitApplication.aclbPersonWorkHistory_MPI, lstrRetirementSubType, this.icdoBenefitCalculationHeader.benefit_type_value,
                                                         this.ibusBenefitApplication.icdoBenefitApplication.min_distribution_flag == busConstant.FLAG_YES ? true : false);//10 Percent RMD

            if (lstrRetirementSubType != busConstant.RETIREMENT_TYPE_LATE &&
                lstrRetirementSubType != busConstant.RETIREMENT_TYPE_MINIMUM_DISTRIBUTION)
            {
                foreach (cdoDummyWorkData lcdoDummyWorkData in this.ibusBenefitApplication.aclbPersonWorkHistory_MPI)
                {
                    if (lcdoDummyWorkData.vested_hours >= 400) 
                    { 
                        lintMPIQualifiedYear += 1;
                    }
                    else if (lcdoDummyWorkData.year == 2023 && this.icdoBenefitCalculationHeader.retirement_date.Year >= 2023 && (lcdoDummyWorkData.qualified_hours >= 65 && lcdoDummyWorkData.qualified_hours < 400))
                    {
                    lintMPIQualifiedYear += 1;

                    }

                if (lcdoDummyWorkData.bis_years_count > lcdoDummyWorkData.vested_years_count)
                    {
                        // Participant has forfeited his Benefit.
                        // Reset the Total Benefit Amount to Zero.
                        lcdoDummyWorkData.idecBenefitAmount = busConstant.ZERO_DECIMAL;
                        ldecUnreducedAccruedBenefitAmount = busConstant.ZERO_DECIMAL;
                        ldecFinalAccruedBenefitAmount = busConstant.ZERO_DECIMAL;
                    }
                    // “Credited Pension QY in 2023 per MOA 2024” --Ticket#153518
                    else if (lcdoDummyWorkData.year == 2023 && this.icdoBenefitCalculationHeader.retirement_date.Year >= 2023 && lintMPIQualifiedYear < 20 && (lcdoDummyWorkData.qualified_hours >= 65 && lcdoDummyWorkData.qualified_hours < 400))
                    {
                        lcdoDummyWorkData.idecBenefitAmount = Math.Round(lcdoDummyWorkData.qualified_hours * lcdoDummyWorkData.idecBenefitRate, 2, MidpointRounding.AwayFromZero);

                    }
                    else if (lcdoDummyWorkData.qualified_years_count < Convert.ToInt32(busConstant.BenefitCalculation.QUALIFIED_YEARS_20) &&
                        lcdoDummyWorkData.qualified_hours < busConstant.MIN_HOURS_FOR_VESTED_YEAR)
                    {
                        lcdoDummyWorkData.idecBenefitAmount = busConstant.ZERO_DECIMAL;
                    }
                    else
                    {
                        if (lcdoDummyWorkData.qualified_years_count > Convert.ToInt32(busConstant.BenefitCalculation.QUALIFIED_YEARS_20) &&
                            lcdoDummyWorkData.qualified_hours < busConstant.MIN_HOURS_FOR_VESTED_YEAR)
                        {
                            if (lintMPIQualifiedYear > 20)
                                lcdoDummyWorkData.idecBenefitAmount = Math.Round(lcdoDummyWorkData.qualified_hours * lcdoDummyWorkData.idecBenefitRate, 2, MidpointRounding.AwayFromZero);
                            else
                            {
                                #region To Check If Local Merged - If yes give Benefit Else NO
                                if (this.ibusPerson.iclbPersonAccount.Where(local => local.icdoPersonAccount.istrPlanCode.Contains("Local")).Count() > 0)
                                {
                                    if (this.ibusPerson.iclbPersonAccount.Where(local => local.icdoPersonAccount.istrPlanCode.Contains(busConstant.Local_52)).Count() > 0)
                                    {
                                        if (lcdoDummyWorkData.year >= this.ibusPerson.iclbPersonAccount.Where(local => local.icdoPersonAccount.istrPlanCode == busConstant.Local_52).FirstOrDefault().icdoPersonAccount.idtMergerDate.Year)
                                            if (lcdoDummyWorkData.qualified_years_count > 20)
                                                lcdoDummyWorkData.idecBenefitAmount = Math.Round(lcdoDummyWorkData.qualified_hours * lcdoDummyWorkData.idecBenefitRate, 2, MidpointRounding.AwayFromZero);
                                    }

                                    if (this.ibusPerson.iclbPersonAccount.Where(local => local.icdoPersonAccount.istrPlanCode.Contains(busConstant.Local_161)).Count() > 0)
                                    {
                                        if (lcdoDummyWorkData.year >= this.ibusPerson.iclbPersonAccount.Where(local => local.icdoPersonAccount.istrPlanCode == busConstant.Local_161).FirstOrDefault().icdoPersonAccount.idtMergerDate.Year)
                                            if (lcdoDummyWorkData.qualified_years_count > 20)
                                                lcdoDummyWorkData.idecBenefitAmount = Math.Round(lcdoDummyWorkData.qualified_hours * lcdoDummyWorkData.idecBenefitRate, 2, MidpointRounding.AwayFromZero);
                                    }

                                    if (this.ibusPerson.iclbPersonAccount.Where(local => local.icdoPersonAccount.istrPlanCode.Contains(busConstant.Local_600)).Count() > 0)
                                    {
                                        if (lcdoDummyWorkData.year >= this.ibusPerson.iclbPersonAccount.Where(local => local.icdoPersonAccount.istrPlanCode == busConstant.Local_600).FirstOrDefault().icdoPersonAccount.idtMergerDate.Year)
                                            if (lcdoDummyWorkData.qualified_years_count > 20)
                                                lcdoDummyWorkData.idecBenefitAmount = Math.Round(lcdoDummyWorkData.qualified_hours * lcdoDummyWorkData.idecBenefitRate, 2, MidpointRounding.AwayFromZero);
                                    }

                                    if (this.ibusPerson.iclbPersonAccount.Where(local => local.icdoPersonAccount.istrPlanCode.Contains(busConstant.Local_666)).Count() > 0)
                                    {
                                        if (lcdoDummyWorkData.year >= this.ibusPerson.iclbPersonAccount.Where(local => local.icdoPersonAccount.istrPlanCode == busConstant.Local_666).FirstOrDefault().icdoPersonAccount.idtMergerDate.Year)
                                            if (lcdoDummyWorkData.qualified_years_count > 20)
                                                lcdoDummyWorkData.idecBenefitAmount = Math.Round(lcdoDummyWorkData.qualified_hours * lcdoDummyWorkData.idecBenefitRate, 2, MidpointRounding.AwayFromZero);
                                    }

                                    if (this.ibusPerson.iclbPersonAccount.Where(local => local.icdoPersonAccount.istrPlanCode.Contains(busConstant.LOCAL_700)).Count() > 0)
                                    {
                                        if (lcdoDummyWorkData.year >= this.ibusPerson.iclbPersonAccount.Where(local => local.icdoPersonAccount.istrPlanCode == busConstant.LOCAL_700).FirstOrDefault().icdoPersonAccount.idtMergerDate.Year)
                                            if (lcdoDummyWorkData.qualified_years_count > 20)
                                                lcdoDummyWorkData.idecBenefitAmount = Math.Round(lcdoDummyWorkData.qualified_hours * lcdoDummyWorkData.idecBenefitRate, 2, MidpointRounding.AwayFromZero);
                                    }

                                }
                                else
                                    lcdoDummyWorkData.idecBenefitAmount = busConstant.ZERO_DECIMAL;
                                #endregion
                            }
                        }
                        else
                        {
                            lcdoDummyWorkData.idecBenefitAmount = Math.Round(lcdoDummyWorkData.qualified_hours * lcdoDummyWorkData.idecBenefitRate, 2, MidpointRounding.AwayFromZero);
                        }
                    }
                    //}

                    ldecUnreducedAccruedBenefitAmount = ldecUnreducedAccruedBenefitAmount + lcdoDummyWorkData.idecBenefitAmount;
                }
            }

            switch (lstrRetirementSubType)
            {
                case busConstant.RETIREMENT_TYPE_REDUCED_EARLY:
                case busConstant.RETIREMENT_TYPE_SPL_REDUCED_EARLY:
                    ldecERF = ibusCalculation.GetEarlyReductionFactor(this.icdoBenefitCalculationHeader.iintPlanId, this.icdoBenefitCalculationHeader.benefit_type_value,
                                                                    lstrRetirementSubType, Convert.ToInt32(this.icdoBenefitCalculationHeader.age));
                    ldecFinalAccruedBenefitAmount = ldecUnreducedAccruedBenefitAmount * ldecERF;
                    break;

                case busConstant.RETIREMENT_TYPE_NORMAL:
                case busConstant.RETIREMENT_TYPE_UNREDUCED_EARLY:
                    ldecFinalAccruedBenefitAmount = ldecUnreducedAccruedBenefitAmount;
                    break;

                case busConstant.RETIREMENT_TYPE_LATE:
                case busConstant.RETIREMENT_TYPE_MINIMUM_DISTRIBUTION:
                    this.ibusBenefitApplication.aclbPersonWorkHistory_MPI = this.ibusBenefitApplication.aclbPersonWorkHistory_MPI.OrderBy(item => item.year).ToList().ToCollection();
                    ldecUnreducedAccruedBenefitAmount = ibusCalculation.CalculateLateRetirementAccruedBenefitAmount(
                                            this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.MPIPP).FirstOrDefault(),
                                            //icdoBenefitCalculationHeader.iintPlanId,      // We cannot pass the Local Plan Id. We need to pass the MPI Plan Id
                                            busConstant.MPIPP_PLAN_ID,
                                            busConstant.BENEFIT_TYPE_RETIREMENT, ibusPerson, ibusBenefitApplication.aclbPersonWorkHistory_MPI,
                                            busConstant.BOOL_FALSE, iclbBenefitCalculationDetail, null, ref ldecLateAdjustmentAmt, this.icdoBenefitCalculationHeader.retirement_date, busConstant.BOOL_FALSE,
                                            this.ibusBenefitApplication.icdoBenefitApplication.adtMPIVestingDate, iclbPersonAccountRetirementContribution);
                    ldecFinalAccruedBenefitAmount = ldecUnreducedAccruedBenefitAmount;
                    break;

                default:
                    break;
            }
            //Ticket - 61531
            decimal ldecLumpSumBenefitOptionFactor = Math.Round(this.GetLumpsumBenefitFactor(Convert.ToInt32(this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement), this.icdoBenefitCalculationHeader.retirement_date.Year),3);
            decimal ldecimalMPIPHPLumpSum = Convert.ToDecimal(Math.Round(ldecFinalAccruedBenefitAmount * ldecLumpSumBenefitOptionFactor, 2));

            //#region DONE specially for Calculating Accured Benefit -- We need this code here
            //if (!this.ibusBenefitApplication.iclbWorkData4RetirementYearMPIPP.IsNullOrEmpty())
            //{
            //    foreach (cdoDummyWorkData item in this.ibusBenefitApplication.iclbWorkData4RetirementYearMPIPP)
            //    {
            //        if (this.ibusBenefitApplication.aclbPersonWorkHistory_MPI.Contains(item))
            //        {
            //            this.ibusBenefitApplication.aclbPersonWorkHistory_MPI.Remove(item);
            //        }
            //    }
            //}
            //#endregion
            this.ibusBenefitApplication.aclbPersonWorkHistory_MPI = this.ibusBenefitApplication.aclbPersonWorkHistory_MPI.OrderBy(item => item.year).ToList().ToCollection();
            return ldecimalMPIPHPLumpSum;
        }

        #endregion

        public bool IsParticipantDisabled(int aintPersonID)
        {
            DataTable ldtbList = SelectWithOperator<cdoBenefitApplication>(new string[] {enmBenefitApplication.person_id.ToString(),
                                enmBenefitApplication.benefit_type_value.ToString(), enmBenefitApplication.application_status_value.ToString() },
                                      new string[] { busConstant.DBOperatorEquals, busConstant.DBOperatorEquals, busConstant.DBOperatorNotEquals },
                                      new object[] { aintPersonID, busConstant.BENEFIT_TYPE_DISABILITY, busConstant.BENEFIT_APPL_CANCELLED }, null);
            if (ldtbList.Rows.Count == 0)
            {
                return false;
            }

            return true;
        }

        #endregion

        #region Overridden Methods

        public override void BeforeValidate(Sagitec.Common.utlPageMode aenmPageMode)
        {
            this.ibusCalculation = new busCalculation();

            this.iblnCalcualteUVHPBenefit = this.iblnCalculateIAPBenefit = this.iblnCalculateL161SplAccBenefit = this.iblnCalculateL52SplAccBenefit = this.iblnCalculateMPIPPBenefit = true;
            //LoadAllRetirementContributions();

            if (ibusPerson.iclbPersonAccount != null && !ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).IsNullOrEmpty())
            {
                //PIR-1094 (to get ee contributions and interest)
                busAnnualBenefitSummaryOverview lbusAnnualBenefitSummaryOverview = new busAnnualBenefitSummaryOverview();
                if (lbusAnnualBenefitSummaryOverview.FindPerson(icdoBenefitCalculationHeader.person_id))
                {
                    lbusAnnualBenefitSummaryOverview.LoadEEcontributions();
                    lbusAnnualBenefitSummaryOverview.btn_PostEEInterest();
                }
                LoadAllRetirementContributions(ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault());
            }
            else
            {
                LoadAllRetirementContributions(null);
            }

            this.icdoBenefitCalculationHeader.istrRetirementType = String.Empty;

            if (this.icdoBenefitCalculationHeader.retirement_date != DateTime.MinValue)
            {
                this.ibusBenefitApplication.icdoBenefitApplication.retirement_date = this.icdoBenefitCalculationHeader.retirement_date;
                this.ibusBenefitApplication.idecAge = busGlobalFunctions.CalculatePersonAgeInDec(this.ibusPerson.icdoPerson.idtDateofBirth, this.icdoBenefitCalculationHeader.retirement_date);
                this.icdoBenefitCalculationHeader.age = this.ibusBenefitApplication.idecAge; //Load the AGE OF THE MAIN HEADER OBJECT AS WELL
                this.icdoBenefitCalculationHeader.idecParticipantFullAge = Math.Floor(this.ibusBenefitApplication.idecAge);

                if (this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth != DateTime.MinValue)
                {
                    this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement = busGlobalFunctions.CalculatePersonAgeInDec(this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth, this.icdoBenefitCalculationHeader.retirement_date);
                    this.icdoBenefitCalculationHeader.idecSurvivorFullAge = Math.Floor(this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement);
                }
                else
                {
                    this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement = 0;
                    this.icdoBenefitCalculationHeader.idecSurvivorFullAge = Math.Floor(this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement);
                }

                if (this.icdoBenefitCalculationHeader.ienuObjectState == ObjectState.Insert)
                    lblIsNew = true;
                else
                    lblIsNew = false;

                SetupPreRequisites_RetirementCalculations();
            }
            else
            {
                //Get age of Person as of today  
                decimal ldecParticipantCurrentAge = busGlobalFunctions.CalculatePersonAgeInDec(this.ibusPerson.icdoPerson.idtDateofBirth, DateTime.Now);
                if (ldecParticipantCurrentAge > 66)
                {
                    this.ibusBenefitApplication.icdoBenefitApplication.retirement_date = busGlobalFunctions.GetLastDayofMonth(this.ibusPerson.icdoPerson.idtDateofBirth.AddYears(Convert.ToInt32(ldecParticipantCurrentAge))).AddDays(1);
                    if (this.ibusBenefitApplication.icdoBenefitApplication.retirement_date <= DateTime.Now)  // RETIREMENT DATE CAN NEVER BE IN THE PAST 
                        this.ibusBenefitApplication.icdoBenefitApplication.retirement_date = busGlobalFunctions.GetLastDayofMonth(DateTime.Now).AddDays(1);

                    this.icdoBenefitCalculationHeader.retirement_date = this.ibusBenefitApplication.icdoBenefitApplication.retirement_date;
                    this.ibusBenefitApplication.idecAge = busGlobalFunctions.CalculatePersonAgeInDec(this.ibusPerson.icdoPerson.idtDateofBirth, this.icdoBenefitCalculationHeader.retirement_date);
                    this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement = busGlobalFunctions.CalculatePersonAgeInDec(this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth, this.icdoBenefitCalculationHeader.retirement_date);
                    this.icdoBenefitCalculationHeader.age = this.ibusBenefitApplication.idecAge;
                    SetupPreRequisites_RetirementCalculations();
                }
                else
                {
                    string lstrPlanCode = String.Empty;
                    DataTable ldtbPlanCode = busBase.Select("cdoPlan.GetPlanCodebyId", new object[1] { this.icdoBenefitCalculationHeader.iintPlanId });
                    if (ldtbPlanCode.Rows.Count > 0)
                    {
                        lstrPlanCode = ldtbPlanCode.Rows[0][0].ToString();
                    }

                    this.ibusBenefitApplication.idecAge = busGlobalFunctions.CalculatePersonAge(this.ibusPerson.icdoPerson.idtDateofBirth, DateTime.Now);

                    //Sid Jain 06252012
                    //if (lblIsNew == false)
                    this.ibusBenefitApplication.DetermineVesting();

                    #region Seed for getting Earliest RTMT date
                    int lintAgeToStartChecking = Convert.ToInt32(Math.Floor(ldecParticipantCurrentAge));

                    if (lintAgeToStartChecking <= 55 && this.icdoBenefitCalculationHeader.iintPlanId != busConstant.LOCAL_700_PLAN_ID)
                    {
                        lintAgeToStartChecking = 55;
                    }
                    else if (this.icdoBenefitCalculationHeader.iintPlanId == busConstant.LOCAL_700_PLAN_ID && lintAgeToStartChecking <= 52)
                    {
                        lintAgeToStartChecking = 52;
                    }
                    #endregion

                    for (int i = lintAgeToStartChecking; i <= 66; i++)
                    {
                        if (this.ibusPerson.icdoPerson.idtDateofBirth.Day == 1)
                        {
                            this.ibusBenefitApplication.icdoBenefitApplication.retirement_date = this.ibusPerson.icdoPerson.idtDateofBirth.AddYears(i);
                        }
                        else
                        {
                            this.ibusBenefitApplication.icdoBenefitApplication.retirement_date = busGlobalFunctions.GetLastDayofMonth(this.ibusPerson.icdoPerson.idtDateofBirth.AddYears(i)).AddDays(1);
                        }

                        if (this.ibusBenefitApplication.icdoBenefitApplication.retirement_date <= DateTime.Now)  // RETIREMENT DATE CAN NEVER BE IN THE PAST 
                            this.ibusBenefitApplication.icdoBenefitApplication.retirement_date = busGlobalFunctions.GetLastDayofMonth(DateTime.Now).AddDays(1);

                        this.icdoBenefitCalculationHeader.retirement_date = this.ibusBenefitApplication.icdoBenefitApplication.retirement_date;
                        this.icdoBenefitCalculationHeader.age = busGlobalFunctions.CalculatePersonAgeInDec(this.ibusPerson.icdoPerson.idtDateofBirth, this.icdoBenefitCalculationHeader.retirement_date);
                        this.ibusBenefitApplication.idecAge = this.icdoBenefitCalculationHeader.age;
                        this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement = busGlobalFunctions.CalculatePersonAgeInDec(this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth, this.ibusBenefitApplication.icdoBenefitApplication.retirement_date);

                        this.ibusBenefitApplication.DetermineBenefitSubTypeandEligibility_Retirement();

                        if (!(this.ibusBenefitApplication.NotEligible) && this.ibusBenefitApplication.iclbEligiblePlans.Where(plan => plan == lstrPlanCode).Count() > 0)
                        {
                            this.icdoBenefitCalculationHeader.istrRetirementType = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().icdoPersonAccount.istrRetirementSubType;
                            break;
                        }

                        else
                        {
                            this.icdoBenefitCalculationHeader.retirement_date = this.ibusBenefitApplication.icdoBenefitApplication.retirement_date = DateTime.MinValue;
                            this.icdoBenefitCalculationHeader.age = this.ibusBenefitApplication.idecAge = 0;
                            this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement = 0;
                        }

                    }
                }

                //Sid Jain 06252012

                //if (this.icdoBenefitCalculationHeader.ienuObjectState == ObjectState.Insert)
                //    lblIsNew = true;
                //else
                //    lblIsNew = false;

            }

            idecAgeDiff = this.icdoBenefitCalculationHeader.idecParticipantFullAge - this.icdoBenefitCalculationHeader.idecSurvivorFullAge;
            base.BeforeValidate(aenmPageMode);
        }

        public override void BeforePersistChanges()
        {
            if (!this.lblIsNew)
            {
                FlushOlderCalculations();
            }

            Setup_Retirement_Calculations();

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


                        if (icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT &&
                            icdoBenefitCalculationHeader.retirement_date < DateTime.Now && icdoBenefitCalculationHeader.iintPlanId != busConstant.IAP_PLAN_ID &&
                            lintPayementCount >= 1)
                        {
                            DateTime ldtAdjustFromDate = this.icdoBenefitCalculationHeader.retirement_date;
                            busPayeeAccount lbusOldPayeeDetails = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
                            if (this.iintOriginalPayeeAccountId > 0)
                            {
                                busPlanBenefitXr lbusPlanBenXr = new busPlanBenefitXr();
                                lbusPlanBenXr.FindPlanBenefitXr(this.iclbBenefitCalculationDetail.First().iclbBenefitCalculationOptions.FirstOrDefault().icdoBenefitCalculationOptions.plan_benefit_id);
                                lbusOldPayeeDetails.FindPayeeAccount(this.iintOriginalPayeeAccountId);
                                lbusOldPayeeDetails.LoadBenefitDetails();
                                if ((lbusOldPayeeDetails.icdoPayeeAccount.istrBenefitOptionValue == busConstant.JOINT_50_PERCENT_POPUP_ANNUITY || lbusOldPayeeDetails.icdoPayeeAccount.istrBenefitOptionValue == busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY
                                   || lbusOldPayeeDetails.icdoPayeeAccount.istrBenefitOptionValue == busConstant.JOINT_100_PERCENT_POPUP_ANNUITY) && lbusPlanBenXr.icdoPlanBenefitXr.benefit_option_value == busConstant.LIFE_ANNUTIY)
                                {

                                    busPerson lbusSpouse = new busPerson();
                                    lbusSpouse.FindPerson(this.icdoBenefitCalculationHeader.beneficiary_person_id);
                                    if (lbusSpouse.icdoPerson.date_of_death != DateTime.MinValue)
                                    {
                                        ldtAdjustFromDate = lbusSpouse.icdoPerson.date_of_death.GetLastDayofMonth().AddDays(1);//If Convert To Benefit Option Executed.
                                    }
                                }
                            }
                            if (ldtAdjustFromDate < DateTime.Now)
                            {
                                lclbActiveRetireeIncreaseContract = lbusActiveRetireeIncreaseContract.LoadActiveRetireeIncContractByRetirementDate(ldtAdjustFromDate);
                                ibusCalculation.FillYearlyDetailSetBenefitAmountForEachYear(this, lbusBenefitCalculationDetail);
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

                                        if (this.ibusPerson != null && this.ibusPerson.iclbPersonAccount != null &&
                                            this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.LOCAL_700_PLAN_ID).Count() > 0)
                                        {
                                            ldecLocal700GauranteedAmt =
                                                ibusCalculation.GetLocal700GuarentedAmt(this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.LOCAL_700_PLAN_ID).FirstOrDefault().icdoPersonAccount.person_account_id);
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
                                             Convert.ToDecimal(lbusRetireeIncreaseContract.icdoActiveRetireeIncreaseContract.percent_increase_value)
                                             , busConstant.BENEFIT_TYPE_RETIREMENT, this);
                                    }
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


                lbusBenefitCalculationDetail.LoadBenefitCalculationYearlyDetailsTotal(ldtForfietureDate, this.ibusBenefitApplication.idecTotalHoursTillLatestWithdrawalDate);
            }

            base.AfterPersistChanges();

            #region Commented Code
            //if (this.ibusBaseActivityInstance.IsNull() && this.lblIsNew)
            //{
            //    DataTable ldtbActivityInstance = busBase.Select("cdoActivityInstance.GetActivityInstanceIdByPersonIdForEstimate", new object[3] { this.icdoBenefitCalculationHeader.person_id, busConstant.RETIREMENT_WORKFLOW_NAME, busConstant.RETIREMENT_ESTIMATE_ACTIVITY_NAME });

            //    if (ldtbActivityInstance.Rows.Count > 0)
            //    {
            //        busActivityInstance lbusActivityInstance = new busActivityInstance();

            //        int lintActivityInstance = Convert.ToInt32(ldtbActivityInstance.Rows[0][enmActivityInstance.activity_instance_id.ToString()]);

            //        if (lbusActivityInstance.FindActivityInstance(lintActivityInstance))
            //        {
            //            lbusActivityInstance.LoadActivity();
            //            lbusActivityInstance.LoadProcessInstance();
            //            lbusActivityInstance.ibusProcessInstance.ibusProcess = lbusActivityInstance.ibusActivity.ibusProcess;
            //            lbusActivityInstance.ibusProcessInstance.LoadPerson();
            //            lbusActivityInstance.EvaluateInitialLoadRules();
            //            this.ibusBaseActivityInstance = lbusActivityInstance;
            //            this.SetProcessInstanceParameters();
            //            lbusActivityInstance.icdoActivityInstance.reference_id = this.icdoBenefitCalculationHeader.benefit_calculation_header_id;
            //            lbusActivityInstance.icdoActivityInstance.Update();
            //        }
            //    }
            //}

            //else if
            #endregion

            if (this.ibusBaseActivityInstance.IsNotNull())
                this.SetProcessInstanceParameters();

            if (ibusBaseActivityInstance.IsNull())
            {
                if (this.iobjPassInfo.idictParams.ContainsKey("aintActivityInstanceRefrenceId"))
                {
                    int RefrenceId = Convert.ToInt32(this.iobjPassInfo.idictParams["aintActivityInstanceRefrenceId"]);

                        busBpmActivityInstance lbusBpmActivityInstance = new busBpmActivityInstance();
                        if (lbusBpmActivityInstance.FindByPrimaryKey(RefrenceId))
                        {
                            lbusBpmActivityInstance.LoadBpmActivity();
                            lbusBpmActivityInstance.LoadBpmProcessInstance();
                            lbusBpmActivityInstance.ibusBpmProcessInstance.LoadBpmCaseInstance();
                            lbusBpmActivityInstance.ibusBpmProcessInstance.ibusBpmProcess = lbusBpmActivityInstance.ibusBpmActivity.ibusBpmProcess;
                            lbusBpmActivityInstance.UpdateParameterValue(busConstant.PersonAccountMaintenance.BENEFIT_CALCULATION_ID, this.icdoBenefitCalculationHeader.benefit_calculation_header_id);
                            lbusBpmActivityInstance.UpdateParameterValue(busConstant.PersonAccountMaintenance.BENEFIT_RETIREMENT_DATE, this.icdoBenefitCalculationHeader.retirement_date);
                            lbusBpmActivityInstance.icdoBpmActivityInstance.Update();
                        }
                }
            }

        }

        public override void ValidateHardErrors(Sagitec.Common.utlPageMode aenmPageMode)
        {
            utlError lobjError = null;
            if (this.iarrErrors.IsNull())
                this.iarrErrors = new ArrayList();

            this.EvaluateInitialLoadRules();

            if (this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth != DateTime.MinValue && this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth >= DateTime.Now)
            {
                lobjError = AddError(1131, " ");
                this.iarrErrors.Add(lobjError);
            }

            //TEMPORARY COMMENTED - 10/10/2013
            //if (this.icdoBenefitCalculationHeader.retirement_date != DateTime.MinValue && this.icdoBenefitCalculationHeader.retirement_date < DateTime.Today)
            //{
            //    if (iobjPassInfo.ienmPageMode == utlPageMode.New)
            //    {
            //        lobjError = AddError(5028, " ");
            //        this.iarrErrors.Add(lobjError);
            //    }
            //    else if (iobjPassInfo.ienmPageMode == utlPageMode.Update)
            //    {
            //        if (this.icdoBenefitCalculationHeader.retirement_date != Convert.ToDateTime(this.icdoBenefitCalculationHeader.ihstOldValues[enmBenefitCalculationHeader.retirement_date.ToString()]))
            //        {
            //            lobjError = AddError(5028, " ");
            //            this.iarrErrors.Add(lobjError);
            //        }
            //    }

            //}

            if (this.ibusPerson.iclbPersonAccount.Where(pl => pl.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).Count() > 0)
            {
                if (!this.ibusBenefitApplication.CheckAlreadyVested(this.ibusPerson.iclbPersonAccount.Where(pl => pl.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).First().icdoPersonAccount.istrPlanCode))
                {
                    // Participant is Not Vested. Throw an error
                    lobjError = AddError(5458, " ");
                    this.iarrErrors.Add(lobjError);
                }
            }

            //The reason this is added is because, while creating a RETIREMENT Estimate the PERSON could enter any Retirement Date if we enter a date at which person is not Eligible We should 
            //throw an HARD ERROR
            //If the Retirement Date is not Entered then Automatically it calculates the EARLIEST RETIREMENT DATE
            if (this.icdoBenefitCalculationHeader.istrRetirementType.IsNullOrEmpty())
            {
                utlError lutlError = new utlError();
                lutlError.istrErrorMessage = "NOT Eligible for Retirement";
                this.iarrErrors.Add(lutlError);

                //IMPORTANT POINT
                //If we delete the Calculation - we are violating the concept of HARD ERRORs which is nothing gets SAVED or DELETE when you get a HARD ERROR.
                //So there-fore we should just say NOT ELIGBILE as of ENTERED RETIREMENT DATE and keep the OLDER CALCULATIONS 
                // IF WE DELETE THE EXISITING CALCULATION DUE TO HARD ERRORS< the code will break at a BUNCH of PLACES.

                //if (!this.lblIsNew)
                //{
                //    FlushOlderCalculations();                    
                //}
            }

            if (this.icdoBenefitCalculationHeader.retirement_date != DateTime.MinValue &&
              this.icdoBenefitCalculationHeader.retirement_date.Day != 1)
            {
                lobjError = AddError(5088, " ");
                this.iarrErrors.Add(lobjError);
            }

            base.ValidateHardErrors(aenmPageMode);
        }

        public override void LoadCorresProperties(string astrTemplateName)
        {
            base.LoadCorresProperties(astrTemplateName);
            istrSurvivorFullName = base.istrSurvivorFullName;
            istrParticipantFullName = base.istrParticipantFullName;
            if (astrTemplateName == busConstant.GENERAL_BENEFIT_ESTIMATE || astrTemplateName == busConstant.GENERAL_BENEFIT_ESTIMATE_52
                || astrTemplateName == busConstant.GENERAL_BENEFIT_ESTIMATE_161 || astrTemplateName == busConstant.GENERAL_BENEFIT_ESTIMATE_600
                || astrTemplateName == busConstant.GENERAL_BENEFIT_ESTIMATE_666 || astrTemplateName == busConstant.GENERAL_BENEFIT_ESTIMATE_700 || astrTemplateName == busConstant.MPI_Retirement_Workshop)
            {
               
                // Still working on it :
                LoadCollectionForRetirement();
            }
            //Ticket# 68545
            else if (astrTemplateName == "PER-0006" || astrTemplateName == "PER-0016" || astrTemplateName == "RETR-0011" || astrTemplateName == "PER-0006R" || astrTemplateName == "RETR-0011R")
            {

                icdoPerson = ibusPerson.icdoPerson;

                DateTime ldtDob = icdoPerson.idtDateofBirth;
                ldtDob = ldtDob.AddYears(70);
                ldtDob = ldtDob.AddMonths(6);
                iintYear = Convert.ToInt32(ldtDob.Year);
                iintPlanYear = DateTime.Now.Year;
                //PIR - 1031
                //istrMinDistriDate = busGlobalFunctions.CalculateMinDistributionDate(icdoPerson.idtDateofBirth, icdoPerson.idtVestingDate);
                //RID 118418
                istrMinDistriDate = busGlobalFunctions.CalculateMinDistributionDate(icdoPerson.person_id, icdoPerson.idtVestingDate);
                //WI 23550 Ticket 143336 now new differed MD date will be 73, using same bookmark property but RMD73 Date
                //istr72MinDistriDate = busGlobalFunctions.Calculate72MinDistributionDate(icdoPerson.idtDateofBirth, icdoPerson.idtVestingDate);
                istr72MinDistriDate = busGlobalFunctions.Calculate73MinDistributionDate(icdoPerson.idtDateofBirth, icdoPerson.idtVestingDate);
                istrFebDate = busGlobalFunctions.CalculateFebDate(icdoPerson.idtDateofBirth);
                DateTime ldtMDDate = busGlobalFunctions.GetMinDistributionDate(icdoPerson.person_id, icdoPerson.idtVestingDate);
                //iintYear = Convert.ToInt32(ldtMDDate.Year);
                istrMDOptionDueDt = busGlobalFunctions.ConvertDateIntoDifFormat(new DateTime(DateTime.Now.Year, 03, 31));

                //For PER-0006(RASHMI)
                DateTime ldtMarchDt = new DateTime(DateTime.Now.Year, 02, 28); //PIR - 1031 Changed the Date from March 15 to Feb 28
                istrMarchDt = busGlobalFunctions.ConvertDateIntoDifFormat(ldtMarchDt);

                if (this.icdoBenefitCalculationHeader.iintPlanId == busConstant.MPIPP_PLAN_ID)
                {
                    istrIsVested = busConstant.FLAG_YES;

                }

                if (this.icdoPerson.marital_status_value != null)
                {
                    if (this.icdoPerson.marital_status_value != "M")
                        istrBenefitOption = "Life Annuity";
                    else
                        istrBenefitOption = "Qualified Joint & 50% Survivor Annuity";
                }

            }

            if (astrTemplateName == busConstant.RETIREMENT_APPLICATION_CANCELLATION_NOTICE) {

                ibusPersonOverview = new busPersonOverview { icdoPerson = new cdoPerson() };
            }

            if ((astrTemplateName == busConstant.IAP_ANNUITY_OPTION_ELECTION_CONFIRMATION) 
                && (iobjPassInfo.istrFormName == busConstant.BENEFIT_CALCULATION_RETIREMENT_MAINTENANCE || iobjPassInfo.istrFormName == busConstant.BENEFIT_CALCULATION_WITHDRAWL_MAINTENANCE ||
                    iobjPassInfo.istrFormName == busConstant.BenefitCalculation.DISABILITY_CALCULATION_MAINTENANCE || iobjPassInfo.istrFormName == busConstant.BENEFIT_CALCULATION_PRE_RETIREMENT_MAINTENANCE))
            {
                DataTable ldtBenefitApplicationData = busBase.Select("cdoPayeeAccount.GetPayeeAccountForRetirementAffidavit", new object[1] { this.ibusBenefitApplication.icdoBenefitApplication.person_id });
                if (ldtBenefitApplicationData.IsNotNull() && ldtBenefitApplicationData.Rows.Count > 0)
                {
                    this.ibusBenefitApplication.icdoBenefitApplication.LoadData(ldtBenefitApplicationData.Rows[0]);
                    RetirementAffidavitCoverLetter();
                }
            }

        }

        #endregion  Overridden Methods

        public void RetirementAffidavitCoverLetter()
        {

            if (ibusParticipant == null)
            {
                ibusParticipant = new busPerson { icdoPerson = new cdoPerson() };
                ibusParticipant.FindPerson(this.ibusBenefitApplication.icdoBenefitApplication.person_id);
            }

            if (ibusPerson == null)
            {
                ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
                ibusPerson.FindPerson(this.ibusBenefitApplication.icdoBenefitApplication.person_id);
            }

            iblnIAPFactor = false;
            busIapAllocationFactor lbusIapAllocationFactor = new busIapAllocationFactor();
            int lintPreviousQuarter = busGlobalFunctions.GetPreviousQuarter(this.ibusBenefitApplication.icdoBenefitApplication.retirement_date);

            if (lintPreviousQuarter == 0)
            {
                lbusIapAllocationFactor.LoadIAPAllocationFactorByPlanYear(this.ibusBenefitApplication.icdoBenefitApplication.retirement_date.Year - 1);
                if (lbusIapAllocationFactor != null && Convert.ToString(lbusIapAllocationFactor.icdoIapAllocationFactor.alloc1_qf4_factor).IsNotNullOrEmpty()
                    && Convert.ToDecimal(lbusIapAllocationFactor.icdoIapAllocationFactor.alloc1_qf4_factor) != 0)
                {
                    iblnIAPFactor = true;
                }

            }
            else
            {
                lbusIapAllocationFactor.LoadIAPAllocationFactorByPlanYear(this.ibusBenefitApplication.icdoBenefitApplication.retirement_date.Year);
                if (lbusIapAllocationFactor != null)
                {
                    if (lintPreviousQuarter == 1 && Convert.ToString(lbusIapAllocationFactor.icdoIapAllocationFactor.alloc1_qf1_factor).IsNotNullOrEmpty()
                    && Convert.ToDecimal(lbusIapAllocationFactor.icdoIapAllocationFactor.alloc1_qf1_factor) != 0)
                    {
                        iblnIAPFactor = true;
                    }
                    else if (lintPreviousQuarter == 2 && Convert.ToString(lbusIapAllocationFactor.icdoIapAllocationFactor.alloc1_qf2_factor).IsNotNullOrEmpty()
                    && Convert.ToDecimal(lbusIapAllocationFactor.icdoIapAllocationFactor.alloc1_qf2_factor) != 0)
                    {
                        iblnIAPFactor = true;
                    }
                    else if (lintPreviousQuarter == 3 && Convert.ToString(lbusIapAllocationFactor.icdoIapAllocationFactor.alloc1_qf3_factor).IsNotNullOrEmpty()
                   && Convert.ToDecimal(lbusIapAllocationFactor.icdoIapAllocationFactor.alloc1_qf3_factor) != 0)
                    {
                        iblnIAPFactor = true;
                    }
                }
            }
        }

        #region Calculate UV-HP Benefit Options
        public void CalculateUVHPBenefitOptions(string astrBenefitOptionValue, decimal ldecTotalBenefitAmount)
        {
            decimal ldecLifeyAnnuityFactor = 1;
            decimal ldecJAndS50Factor = new decimal();
            busBenefitCalculationOptions lbusBenefitCalculationOption;
            int lintParticipantAge = Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge);
            int lintSurvivorAge = Convert.ToInt32(Math.Floor(icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement));

            #region Get the Necessary Factors
            DataTable ldtMonthlyLifeAnnuity = Select("cdoBenefitProvisionUvhpLifeFactor.GetUVHPLifeFactor", new object[2] { Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge), this.ibusBenefitApplication.icdoBenefitApplication.retirement_date.Year });
            decimal ldecLifeAnnuityAmount = decimal.Zero;
            //Need not confuse the query, its just reused at many places. That's why did not change the name     
            //DataTable ldtMonthlyJS50Annuity = Select("cdoBenefitProvisionUvhpFactor.GetEEUVHPFactor", new object[3] { ibusCalculation.GetPlanBenefitId((busConstant.MPIPP_PLAN_ID), busConstant.QJ50), Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge), Convert.ToInt32(this.icdoBenefitCalculationHeader.idecSurvivorFullAge) });

            if (ldtMonthlyLifeAnnuity.Rows.Count > 0)
            {
                ldecLifeyAnnuityFactor = Convert.ToDecimal(ldtMonthlyLifeAnnuity.Rows[0][0]);
                ldecLifeyAnnuityFactor = Math.Round(ldecLifeyAnnuityFactor, 3);
                if (ldecLifeyAnnuityFactor > decimal.Zero)
                {
                    ldecLifeAnnuityAmount = Math.Round(ldecTotalBenefitAmount / ldecLifeyAnnuityFactor, 2);
                }
            }
            //if (ldtMonthlyJS50Annuity.Rows.Count > 0)
            //{
            //    ldecJAndS50Factor = Convert.ToDecimal(ldtMonthlyJS50Annuity.Rows[0][0]);
            //}
            ldecJAndS50Factor = Convert.ToDecimal(Math.Round(Math.Min(1, Math.Max(0, 0.86 + 0.005 * (lintSurvivorAge - lintParticipantAge) + 0.006 * (65 - lintParticipantAge))), 3));
            #endregion

            #region Switch Case Based on Benefit Options We have to Calculate
            bool lblnCheckIfSpouse = busConstant.BOOL_FALSE;
            lblnCheckIfSpouse = this.ibusCalculation.CheckIfSpouse(this.icdoBenefitCalculationHeader.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id);

            switch (astrBenefitOptionValue)
            {
                case busConstant.CodeValueAll:
                    #region UVHP LUMP SUM
                    lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                    lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.MPIPP_PLAN_ID, busConstant.LUMP_SUM), new decimal(), ldecTotalBenefitAmount, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER, this.icdoBenefitCalculationHeader.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.LUMP_SUM, busConstant.ZERO_DECIMAL, true, true);


                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);

                    #endregion
                    if (lblnCheckIfSpouse)
                    {
                        #region JOINT_50_PERCENT_SURVIVOR_ANNUITY
                        if (ldecTotalBenefitAmount > busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT)
                        {
                            decimal ldecJs50Annuity = Math.Round(ldecLifeAnnuityAmount * ldecJAndS50Factor, 2);
                            lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                            lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.MPIPP_PLAN_ID, busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY), ldecJAndS50Factor, ldecJs50Annuity, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER, this.icdoBenefitCalculationHeader.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY, Math.Ceiling(ldecJs50Annuity * 0.5M), true, true);
                            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                        }
                        #endregion
                    }

                    #region LIFE_ANNUTIY
                    if (ldecTotalBenefitAmount > busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT)
                    {
                        lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                        lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.MPIPP_PLAN_ID, busConstant.LIFE), ldecLifeyAnnuityFactor, ldecLifeAnnuityAmount, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER, this.icdoBenefitCalculationHeader.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.LIFE, busConstant.ZERO_DECIMAL, true, true);
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    }
                    #endregion
                    break;

                case busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY:
                    if (lblnCheckIfSpouse)
                    {
                        decimal ldecJs50Annuity = Math.Round(ldecLifeAnnuityAmount * ldecJAndS50Factor, 2);
                        #region JOINT_50_PERCENT_SURVIVOR_ANNUITY
                        lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                        lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.MPIPP_PLAN_ID, busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY), ldecJAndS50Factor, Math.Round(ldecLifeAnnuityAmount * ldecJAndS50Factor, 2), busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER, this.icdoBenefitCalculationHeader.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY, Math.Ceiling(ldecJs50Annuity * 0.5M), true, true);

                        if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL)
                        {
                            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId && item.icdoBenefitCalculationDetail.uvhp_flag.IsNotNullOrEmpty()).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                        }
                        else
                        {
                            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                        }
                        #endregion
                    }
                    break;

                case busConstant.LIFE_ANNUTIY:
                    #region LIFE_ANNUTIY
                    lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                    lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.MPIPP_PLAN_ID, busConstant.LIFE), ldecLifeyAnnuityFactor, ldecLifeAnnuityAmount, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER, this.icdoBenefitCalculationHeader.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.LIFE, busConstant.ZERO_DECIMAL, true, true);

                    if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL)
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
                    lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.MPIPP_PLAN_ID, busConstant.LUMP_SUM), new decimal(), Math.Ceiling(ldecTotalBenefitAmount), busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER, this.icdoBenefitCalculationHeader.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.LUMP_SUM, busConstant.ZERO_DECIMAL, true, true);

                    if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL)
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
        #endregion

        //RETR-0038
        public void LoadCollectionForRetirement()
        {
            idtLastWorkingDate = ibusCalculation.GetLastWorkingDate(this.ibusPerson.icdoPerson.ssn);

            int lintParticipantAge = Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge);
            int lintSurvivorAge = Convert.ToInt32(this.icdoBenefitCalculationHeader.idecSurvivorFullAge);
            decimal ldecLateAdjustmentAmt = decimal.Zero;
            decimal ldecReducedBenefit = decimal.Zero;

            int lintTotalQualifiedYear = 0;
            ArrayList arrOption = new ArrayList();
            this.iclbbusBenefitCalculationOptions = new Collection<busBenefitCalculationOptions>();
            if (this.ibusBenefitApplication.aclbPersonWorkHistory_MPI.IsNotNull())
            {
                lintTotalQualifiedYear = this.ibusBenefitApplication.aclbPersonWorkHistory_MPI.OrderByDescending(item => item.year).ToList().ToCollection().FirstOrDefault().qualified_years_count;
                if (lintTotalQualifiedYear > 10)
                {
                    iblnQualifiedYrs = busConstant.BOOL_TRUE;
                }
            }
            string lstrPrevRetirementType = string.Empty;
            busBenefitCalculationOptions lbus = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
            lbus.iintParticipantAge = lintParticipantAge;
            lbus.iintSpouseAge = lintSurvivorAge;
            string lstrRetirementTypeValue = string.Empty;
            Collection<busBenefitCalculationDetail> lclbCalcDetail = new Collection<busBenefitCalculationDetail>();

            #region CurrentObject
            foreach (busBenefitCalculationDetail lbusDetail in this.iclbBenefitCalculationDetail)
            {
                if (lbusDetail.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID || lbusDetail.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_52_PLAN_ID
                    || lbusDetail.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_161_PLAN_ID || lbusDetail.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_600_PLAN_ID
                    || lbusDetail.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_666_PLAN_ID || lbusDetail.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_700_PLAN_ID) //rid 78456
                {
                    lbus.istrRetirementType = lbusDetail.icdoBenefitCalculationDetail.benefit_subtype_description;
                    lstrRetirementTypeValue = lbusDetail.icdoBenefitCalculationDetail.benefit_subtype_value;

                    lstrPrevRetirementType = lstrRetirementTypeValue;

                    //PIR 916
                    //lbus.istrERFPercentage = (lbusDetail.icdoBenefitCalculationDetail.early_reduction_factor * 100).ToString("G29");
                    //lbus.istrERFPercentage = Convert.ToString(lbusDetail.icdoBenefitCalculationDetail.early_reduction_factor.ToString("P", CultureInfo.InvariantCulture));
                   
                    foreach (busBenefitCalculationOptions lbuscalcOptions in lbusDetail.iclbBenefitCalculationOptions)
                    {
                        if (lbuscalcOptions.ibusPlanBenefitXr.IsNull())
                            lbuscalcOptions.ibusPlanBenefitXr.FindPlanBenefitXr(lbuscalcOptions.icdoBenefitCalculationOptions.plan_benefit_id);
                        arrOption.Add(lbuscalcOptions.ibusPlanBenefitXr.icdoPlanBenefitXr.benefit_option_value);
                        if (lbuscalcOptions.icdoBenefitCalculationOptions.ee_flag == busConstant.FLAG_YES || lbuscalcOptions.icdoBenefitCalculationOptions.uvhp_flag == busConstant.FLAG_YES)
                            continue;
                        if (lbuscalcOptions.ibusPlanBenefitXr.icdoPlanBenefitXr.benefit_option_value == busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY)
                        {
                            lbus.istrJS50 = Convert.ToString(lbuscalcOptions.icdoBenefitCalculationOptions.benefit_amount);
                        }
                        if (lbuscalcOptions.ibusPlanBenefitXr.icdoPlanBenefitXr.benefit_option_value == busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY)
                        {
                            lbus.istrJS100 = Convert.ToString(lbuscalcOptions.icdoBenefitCalculationOptions.benefit_amount);
                        }
                        if (lbuscalcOptions.ibusPlanBenefitXr.icdoPlanBenefitXr.benefit_option_value == busConstant.JOINT_100_PERCENT_POPUP_ANNUITY)
                        {
                            lbus.istrJP100 = Convert.ToString(lbuscalcOptions.icdoBenefitCalculationOptions.benefit_amount);
                        }
                        if (lbuscalcOptions.ibusPlanBenefitXr.icdoPlanBenefitXr.benefit_option_value == busConstant.JOINT_50_PERCENT_POPUP_ANNUITY)
                        {
                            lbus.istrJP50 = Convert.ToString(lbuscalcOptions.icdoBenefitCalculationOptions.benefit_amount);
                        }
                        if (lbuscalcOptions.ibusPlanBenefitXr.icdoPlanBenefitXr.benefit_option_value == busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY)
                        {
                            lbus.istrJS75 = Convert.ToString(lbuscalcOptions.icdoBenefitCalculationOptions.benefit_amount);
                        }
                        if (lbuscalcOptions.ibusPlanBenefitXr.icdoPlanBenefitXr.benefit_option_value == busConstant.LIFE_ANNUTIY)
                        {
                            lbus.istrLife = Convert.ToString(lbuscalcOptions.icdoBenefitCalculationOptions.benefit_amount);
                        }
                        if (lbuscalcOptions.ibusPlanBenefitXr.icdoPlanBenefitXr.benefit_option_value == busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY)
                        {
                            lbus.istrTenYear = Convert.ToString(lbuscalcOptions.icdoBenefitCalculationOptions.benefit_amount);
                        }
                        if (lbuscalcOptions.ibusPlanBenefitXr.icdoPlanBenefitXr.benefit_option_value == busConstant.TWO_YEARS_CERTAIN_AND_LIFE_ANNUITY)
                        {                            
                            lbus.istrTwoYear = Convert.ToString(lbuscalcOptions.icdoBenefitCalculationOptions.benefit_amount);
                        }                        
                        if (lbuscalcOptions.ibusPlanBenefitXr.icdoPlanBenefitXr.benefit_option_value == busConstant.JOINT_66_2by3_PERCENT_SURVIVOR_ANNUITY)
                        {
                            lbus.istrJS66 = Convert.ToString(lbuscalcOptions.icdoBenefitCalculationOptions.benefit_amount);
                        }
                        if (lbuscalcOptions.ibusPlanBenefitXr.icdoPlanBenefitXr.benefit_option_value == busConstant.THREE_YEARS_CERTAIN_AND_LIFE_ANNUITY)
                        {
                            lbus.istrThreeYear = Convert.ToString(lbuscalcOptions.icdoBenefitCalculationOptions.benefit_amount);
                        }
                        if (lbuscalcOptions.ibusPlanBenefitXr.icdoPlanBenefitXr.benefit_option_value == busConstant.FIVE_YEARS_CERTAIN_AND_LIFE_ANNUTIY)
                        {
                            lbus.istrFiveYear = Convert.ToString(lbuscalcOptions.icdoBenefitCalculationOptions.benefit_amount);
                        }
                    }

                    //start rid 78456
                    int ledf = 1;
                    int lintRetAge = 65;

                    if (lbusDetail.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID)
                    {
                        lintRetAge = Convert.ToInt32(ibusCalculation.GetNormalRetirementAge(busConstant.MPIPP_PLAN_ID));
                        lbus.istrERFPercentage = lbus.iintParticipantAge == lintRetAge ? ledf.ToString("P", CultureInfo.InvariantCulture) : Convert.ToString(lbusDetail.icdoBenefitCalculationDetail.early_reduction_factor.ToString("P", CultureInfo.InvariantCulture));                        
                    }

                    if (lbusDetail.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_52_PLAN_ID)
                    {
                        if ((lstrRetirementTypeValue == busConstant.RETIREMENT_TYPE_UNREDUCED_EARLY && Convert.ToDecimal(lbus.istrTenYear) == Math.Round(idecAcrdBenefitLocal, 0))
                            || (lstrRetirementTypeValue == busConstant.RETIREMENT_TYPE_UNREDUCED_EARLY && Convert.ToDecimal(lbus.istrTenYear) == Math.Round(idecAcrdBenefitLocal, 0) + Convert.ToDecimal("1.0"))
                            || (lstrRetirementTypeValue == busConstant.RETIREMENT_TYPE_LATE))
                        {
                            lbus.istrERFPercentage = ledf.ToString("P", CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            lintRetAge = Convert.ToInt32(ibusCalculation.GetNormalRetirementAge(busConstant.LOCAL_52_PLAN_ID));
                            lbus.istrERFPercentage = lbus.iintParticipantAge >= lintRetAge ? ledf.ToString("P", CultureInfo.InvariantCulture) : Convert.ToString(lbusDetail.icdoBenefitCalculationDetail.early_reduction_factor.ToString("P", CultureInfo.InvariantCulture));
                        }
                    }

                    if (lbusDetail.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_161_PLAN_ID)
                    {
                        if ((lstrRetirementTypeValue == busConstant.RETIREMENT_TYPE_UNREDUCED_EARLY && Math.Round(Convert.ToDecimal(lbus.istrFiveYear), 0) == Math.Round(idecAcrdBenefitLocal, 0))
                            || (lstrRetirementTypeValue == busConstant.RETIREMENT_TYPE_UNREDUCED_EARLY && Math.Round(Convert.ToDecimal(lbus.istrFiveYear), 0) == Math.Round(idecAcrdBenefitLocal, 0) + Convert.ToDecimal("1.0"))
                            || (lstrRetirementTypeValue == busConstant.RETIREMENT_TYPE_LATE))
                        {
                            lbus.istrERFPercentage = ledf.ToString("P", CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            lintRetAge = Convert.ToInt32(ibusCalculation.GetNormalRetirementAge(busConstant.LOCAL_161_PLAN_ID));
                            lbus.istrERFPercentage = lbus.iintParticipantAge >= lintRetAge ? ledf.ToString("P", CultureInfo.InvariantCulture) : Convert.ToString(lbusDetail.icdoBenefitCalculationDetail.early_reduction_factor.ToString("P", CultureInfo.InvariantCulture));
                        }
                    }

                    if (lbusDetail.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_600_PLAN_ID)
                    {
                        if ((lstrRetirementTypeValue == busConstant.RETIREMENT_TYPE_UNREDUCED_EARLY && Math.Round(Convert.ToDecimal(lbus.istrTenYear), 0) == Math.Round(idecAcrdBenefitLocal, 0))
                            || (lstrRetirementTypeValue == busConstant.RETIREMENT_TYPE_UNREDUCED_EARLY && Math.Round(Convert.ToDecimal(lbus.istrTenYear), 0) == Math.Round(idecAcrdBenefitLocal, 0) + Convert.ToDecimal("1.0"))
                            || (lstrRetirementTypeValue == busConstant.RETIREMENT_TYPE_LATE))
                        {
                            lbus.istrERFPercentage = ledf.ToString("P", CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            lintRetAge = Convert.ToInt32(ibusCalculation.GetNormalRetirementAge(busConstant.LOCAL_600_PLAN_ID));
                            lbus.istrERFPercentage = lbus.iintParticipantAge >= lintRetAge ? ledf.ToString("P", CultureInfo.InvariantCulture) : Convert.ToString(lbusDetail.icdoBenefitCalculationDetail.early_reduction_factor.ToString("P", CultureInfo.InvariantCulture));
                        }
                    }

                    if (lbusDetail.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_666_PLAN_ID)
                    {
                        if ((lstrRetirementTypeValue == busConstant.RETIREMENT_TYPE_UNREDUCED_EARLY && Math.Ceiling(Convert.ToDecimal(lbus.istrThreeYear) / ldecFiftyCents) * ldecFiftyCents == Math.Ceiling(idecAcrdBenefitLocal / ldecFiftyCents) * ldecFiftyCents)
                            || (lstrRetirementTypeValue == busConstant.RETIREMENT_TYPE_UNREDUCED_EARLY && Math.Ceiling(Convert.ToDecimal(lbus.istrThreeYear) / ldecFiftyCents) * ldecFiftyCents == (Math.Ceiling(idecAcrdBenefitLocal / ldecFiftyCents) * ldecFiftyCents) + Convert.ToDecimal("1.0"))
                            || (lstrRetirementTypeValue == busConstant.RETIREMENT_TYPE_LATE))
                        {
                            lbus.istrERFPercentage = ledf.ToString("P", CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            lintRetAge = Convert.ToInt32(ibusCalculation.GetNormalRetirementAge(busConstant.LOCAL_666_PLAN_ID));
                            lbus.istrERFPercentage = lbus.iintParticipantAge >= lintRetAge ? ledf.ToString("P", CultureInfo.InvariantCulture) : Convert.ToString(lbusDetail.icdoBenefitCalculationDetail.early_reduction_factor.ToString("P", CultureInfo.InvariantCulture));
                        }
                    }

                    if (lbusDetail.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_700_PLAN_ID)
                    {
                        if (lstrRetirementTypeValue == busConstant.RETIREMENT_TYPE_UNREDUCED_EARLY || lstrRetirementTypeValue == busConstant.RETIREMENT_TYPE_LATE)
                        {
                            lbus.istrERFPercentage = ledf.ToString("P", CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            lintRetAge = Convert.ToInt32(ibusCalculation.GetNormalRetirementAge(busConstant.LOCAL_700_PLAN_ID));
                            lbus.istrERFPercentage = lbus.iintParticipantAge >= lintRetAge ? ledf.ToString("P", CultureInfo.InvariantCulture) : Convert.ToString(lbusDetail.icdoBenefitCalculationDetail.early_reduction_factor.ToString("P", CultureInfo.InvariantCulture));
                        }
                    }
                }
            }

            this.iclbbusBenefitCalculationOptions.Add(lbus);
            #endregion

            #region Calculate Benefit to Show in the Collection
            if (iclbBenefitCalculationDetail[0].icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && (lstrRetirementTypeValue == busConstant.RETIREMENT_TYPE_REDUCED_EARLY || lstrRetirementTypeValue == busConstant.RETIREMENT_TYPE_SPL_REDUCED_EARLY ||
                lstrRetirementTypeValue == busConstant.RETIREMENT_TYPE_UNREDUCED_EARLY))                
            {
                for (int i = lintParticipantAge + 1; i <= 65; i++)
                {
                    busBenefitCalculationDetail lbusDetail = new busBenefitCalculationDetail { icdoBenefitCalculationDetail = new cdoBenefitCalculationDetail() };
                    lclbCalcDetail = new Collection<busBenefitCalculationDetail>();

                    decimal ldecFactor;
                    lbus = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                    if (this.ibusPerson.icdoPerson.date_of_birth.Day == 1)
                    {
                        this.ibusBenefitApplication.icdoBenefitApplication.retirement_date = this.ibusPerson.icdoPerson.idtDateofBirth.AddYears(i);
                    }
                    else
                    {
                        this.ibusBenefitApplication.icdoBenefitApplication.retirement_date = busGlobalFunctions.GetLastDayofMonth(this.ibusPerson.icdoPerson.idtDateofBirth.AddYears(i)).AddDays(1);
                    }

                    lbusDetail.icdoBenefitCalculationDetail.plan_id = busConstant.MPIPP_PLAN_ID;
                    lbusDetail.icdoBenefitCalculationDetail.retirement_date = this.ibusBenefitApplication.icdoBenefitApplication.retirement_date;
                    lclbCalcDetail.Add(lbusDetail);

                    lintSurvivorAge = Convert.ToInt32(busGlobalFunctions.CalculatePersonAge(this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth, this.ibusBenefitApplication.icdoBenefitApplication.retirement_date));
                    lbus.iintParticipantAge = i;
                    lbus.iintSpouseAge = lintSurvivorAge;
                    this.ibusBenefitApplication.idecAge = i;
                    this.ibusBenefitApplication.LoadWorkHistoryandSetupPrerequisites_Retirement();
                    if (!(this.ibusBenefitApplication.NotEligible))
                    {
                        lbus.istrRetirementType = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().icdoPersonAccount.istrRetirementSubType;
                        //PIR 916
                        //if (lstrPrevRetirementType != lbus.istrRetirementType)
                        //{
                        lstrPrevRetirementType = lbus.istrRetirementType;
                        
                        ldecReducedBenefit = ibusCalculation.CalculateReducedBenefit(ibusPerson, busConstant.BENEFIT_TYPE_RETIREMENT, i, this.ibusBenefitApplication.icdoBenefitApplication.retirement_date,
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().icdoBenefitCalculationDetail.vested_date,
                        ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault(),
                        ibusBenefitApplication, busConstant.BOOL_FALSE,
                        lclbCalcDetail, null, lintTotalQualifiedYear, this.iclbBenefitCalculationDetail[0].icdoBenefitCalculationDetail.unreduced_benefit_amount, lbus.istrRetirementType, false,
                        ibusBenefitApplication.aclbPersonWorkHistory_MPI, this.iclbPersonAccountRetirementContribution, ref ldecLateAdjustmentAmt, aintPersonId: ibusPerson.icdoPerson.person_id);
                        
                        if (!lclbCalcDetail.IsNullOrEmpty())
                        {
                            //PIR 916
                            //lbus.istrERFPercentage = (lclbCalcDetail.FirstOrDefault().icdoBenefitCalculationDetail.early_reduction_factor * 100).ToString("G29");
                            lbus.istrERFPercentage = Convert.ToString(lclbCalcDetail.FirstOrDefault().icdoBenefitCalculationDetail.early_reduction_factor.ToString("P", CultureInfo.InvariantCulture));
                        }
                        foreach (string astrBenOption in arrOption)
                        {
                            ldecFactor = decimal.Zero;
                            ldecFactor = ibusCalculation.GetFactor(this, astrBenOption, lintSurvivorAge, i);
                            if (ldecFactor > 1 && lbusDetail.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID)
                            {
                                ldecFactor = 1;
                            }
                            if (astrBenOption == busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY)
                            {
                                lbus.istrJS50 = Convert.ToString(Math.Round(ldecFactor * ldecReducedBenefit, 2));
                            }
                            if (astrBenOption == busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY)
                            {
                                lbus.istrJS100 = Convert.ToString(Math.Round(ldecFactor * ldecReducedBenefit, 2));
                            }
                            if (astrBenOption == busConstant.JOINT_100_PERCENT_POPUP_ANNUITY)
                            {
                                lbus.istrJP100 = Convert.ToString(Math.Round(ldecFactor * ldecReducedBenefit, 2));
                            }
                            if (astrBenOption == busConstant.JOINT_50_PERCENT_POPUP_ANNUITY)
                            {
                                lbus.istrJP50 = Convert.ToString(Math.Round(ldecFactor * ldecReducedBenefit, 2));
                            }
                            if (astrBenOption == busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY)
                            {
                                lbus.istrJS75 = Convert.ToString(Math.Round(ldecFactor * ldecReducedBenefit, 2));
                            }
                            if (astrBenOption == busConstant.LIFE_ANNUTIY)
                            {
                                lbus.istrLife = Convert.ToString(Math.Round(ldecFactor * ldecReducedBenefit, 2));
                            }
                            if (astrBenOption == busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY)
                            {
                                lbus.istrTenYear = Convert.ToString(Math.Round(ldecFactor * ldecReducedBenefit, 2));
                            }
                        }
                        this.iclbbusBenefitCalculationOptions.Add(lbus);
                        //}
                    }

                }
            }
            #endregion

            if (iclbBenefitCalculationDetail[0].icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_52_PLAN_ID)
            {
                Calc52BenefitOptionsForCorr(lstrRetirementTypeValue, lintParticipantAge, ref arrOption, ref lclbCalcDetail, ref lbus);
            }

            if (iclbBenefitCalculationDetail[0].icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_161_PLAN_ID)
            {
                Calc161BenefitOptionsForCorr(lstrRetirementTypeValue, lintParticipantAge, ref arrOption, ref lclbCalcDetail, ref lbus);
            }

            if (iclbBenefitCalculationDetail[0].icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_600_PLAN_ID)
            {
                Calc600BenefitOptionsForCorr(lstrRetirementTypeValue, lintParticipantAge, ref arrOption, ref lclbCalcDetail, ref lbus);
            }

            if (iclbBenefitCalculationDetail[0].icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_666_PLAN_ID)
            {
                Calc666BenefitOptionsForCorr(lstrRetirementTypeValue, lintParticipantAge, ref arrOption, ref lclbCalcDetail, ref lbus);
            }

            if (iclbBenefitCalculationDetail[0].icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_700_PLAN_ID)
            {
                Calc700BenefitOptionsForCorr(lstrRetirementTypeValue, lintParticipantAge, ref arrOption, ref lclbCalcDetail, ref lbus);
            }
        }

        #region - local 52 Correspondence
        public void Calc52BenefitOptionsForCorr(string astrRetirementTypeValue, int aintParticipantAge, ref ArrayList arrOption, ref Collection<busBenefitCalculationDetail> lclbCalcDetail, ref busBenefitCalculationOptions lbus)
        {
            int lintPersonAccountId = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).First().icdoPersonAccount.person_account_id;
            //int lintPersonAccountId = ibusBenefitApplication.ibusPerson.iclbPersonAccount[0].icdoPersonAccount.person_account_id;
            string lstrRetirementTypeValue = astrRetirementTypeValue;
            int lintParticipantAge = aintParticipantAge;
            int lintSurvivorAge = 0;
            int lintRetAge = Convert.ToInt32(ibusCalculation.GetNormalRetirementAge(busConstant.LOCAL_52_PLAN_ID));
            DateTime ldtRetirementDate = this.icdoBenefitCalculationHeader.retirement_date;
            int lintYear = this.icdoBenefitCalculationHeader.retirement_date.Year + 1;
            int lintSpouseAge = this.iclbbusBenefitCalculationOptions.LastOrDefault().iintSpouseAge + 1;

            if (lstrRetirementTypeValue == busConstant.RETIREMENT_TYPE_REDUCED_EARLY || lstrRetirementTypeValue == busConstant.RETIREMENT_TYPE_SPL_REDUCED_EARLY)
            {                
                for (int i = lintParticipantAge + 1; i <= lintRetAge; i++)
                {
                    busBenefitCalculationDetail lbusDetail = new busBenefitCalculationDetail { icdoBenefitCalculationDetail = new cdoBenefitCalculationDetail() };
                    lclbCalcDetail = new Collection<busBenefitCalculationDetail>();

                    lbus = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                    if (this.ibusPerson.icdoPerson.date_of_birth.Day == 1)
                    {
                        this.ibusBenefitApplication.icdoBenefitApplication.retirement_date = this.ibusPerson.icdoPerson.idtDateofBirth.AddYears(i);
                    }
                    else
                    {
                        this.ibusBenefitApplication.icdoBenefitApplication.retirement_date = busGlobalFunctions.GetLastDayofMonth(this.ibusPerson.icdoPerson.idtDateofBirth.AddYears(i)).AddDays(1);
                    }

                    lbusDetail.icdoBenefitCalculationDetail.plan_id = busConstant.LOCAL_52_PLAN_ID;
                    lbusDetail.icdoBenefitCalculationDetail.retirement_date = this.ibusBenefitApplication.icdoBenefitApplication.retirement_date;
                    lclbCalcDetail.Add(lbusDetail);

                    lintSurvivorAge = Convert.ToInt32(busGlobalFunctions.CalculatePersonAge(this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth, this.ibusBenefitApplication.icdoBenefitApplication.retirement_date));
                    lbus.iintParticipantAge = i;
                    lbus.iintSpouseAge = lintSurvivorAge;
                    this.ibusBenefitApplication.idecAge = Convert.ToDecimal(i);                                     

                    //rid 82228
                    decimal ldecLocalFrozenBenefit_amount = this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_frozen_benefit_amount);
                    decimal ldecLocalPreBisAmount = this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_pre_bis_amount);
                    decimal ldecLocalPostBisAmount = this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_post_bis_amount);
                    int lintComputationalYear = Convert.ToInt32(this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Max(item => item.icdoPersonAccountRetirementContribution.computational_year));
                    ldtRetirementDate = Convert.ToDateTime(ldtRetirementDate.Month.ToString() + "/" + ldtRetirementDate.Day.ToString() + "/" + lintYear.ToString());

                    decimal ldecTotalBenefitAmount = 0;

                    if (iclbBenefitCalculationDetail[0].icdoBenefitCalculationDetail.early_reduction_factor <= Convert.ToDecimal(1.0))
                    {
                        ldecTotalBenefitAmount = this.ibusCalculation.CalculateTotalBenefitAmtForLocal52(astrRetirementTypeValue, //this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_52).First().icdoPersonAccount.istrRetirementSubType,
                                                             ldtRetirementDate, //this.icdoBenefitCalculationHeader.retirement_date,
                                                             this.ibusPerson.icdoPerson.idtDateofBirth,
                                                             ldecLocalFrozenBenefit_amount, false,
                                                             this.ibusBenefitApplication,
                                                             ldecLocalPreBisAmount,
                                                             ldecLocalPostBisAmount, null,
                                                             this.iclbBenefitCalculationDetail,
                                                             lintComputationalYear,
                                                             this.iclbPersonAccountRetirementContribution, busConstant.BOOL_FALSE, this.ibusPerson.icdoPerson.person_id);
                    }

                    if (iclbBenefitCalculationDetail[0].icdoBenefitCalculationDetail.early_reduction_factor > Convert.ToDecimal(1.0))
                    {
                        astrRetirementTypeValue = busConstant.RETIREMENT_TYPE_UNREDUCED_EARLY;
                        ldecTotalBenefitAmount = this.ibusCalculation.CalculateTotalBenefitAmtForLocal52(astrRetirementTypeValue, //this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_52).First().icdoPersonAccount.istrRetirementSubType,
                                                             ldtRetirementDate, //this.icdoBenefitCalculationHeader.retirement_date,
                                                             this.ibusPerson.icdoPerson.idtDateofBirth,
                                                             ldecLocalFrozenBenefit_amount, false,
                                                             this.ibusBenefitApplication,
                                                             ldecLocalPreBisAmount,
                                                             ldecLocalPostBisAmount, null,
                                                             this.iclbBenefitCalculationDetail,
                                                             lintComputationalYear,
                                                             this.iclbPersonAccountRetirementContribution, busConstant.BOOL_FALSE, this.ibusPerson.icdoPerson.person_id);
                    }

                    CalculateLocal52BenefitOptionsForCorr(busConstant.CodeValueAll, ldecTotalBenefitAmount, Convert.ToDecimal(i), lintSpouseAge);
                    
                    foreach (string astrBenOption in arrOption)
                    {
                        if (astrBenOption == busConstant.LIFE_ANNUTIY)
                        {
                            busBenefitCalculationOptions lobjAnnuity = this.iclbBenefitCalculationDetail.FirstOrDefault().iclbBenefitCalculationOptions.Where(x => x.icdoBenefitCalculationOptions.istrBenefitOptionDescription == "Life Annuity").FirstOrDefault();
                            lbus.istrLife = lobjAnnuity.icdoBenefitCalculationOptions.benefit_amount.ToString();
                        }
                        if (astrBenOption == busConstant.JOINT_50_PERCENT_POPUP_ANNUITY)
                        {
                            busBenefitCalculationOptions lobjJoint50 = this.iclbBenefitCalculationDetail.FirstOrDefault().iclbBenefitCalculationOptions.Where(x => x.icdoBenefitCalculationOptions.istrBenefitOptionDescription == "Joint & 50% Pop-up Annuity").FirstOrDefault();
                            lbus.istrJP50 = lobjJoint50.icdoBenefitCalculationOptions.benefit_amount.ToString();
                        }
                        if (astrBenOption == busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY)
                        {
                            busBenefitCalculationOptions lobjJoint75 = this.iclbBenefitCalculationDetail.FirstOrDefault().iclbBenefitCalculationOptions.Where(x => x.icdoBenefitCalculationOptions.istrBenefitOptionDescription == "Joint & 75% Survivor Annuity").FirstOrDefault();
                            lbus.istrJS75 = lobjJoint75.icdoBenefitCalculationOptions.benefit_amount.ToString();
                        }
                        if (astrBenOption == busConstant.JOINT_100_PERCENT_POPUP_ANNUITY)
                        {
                            busBenefitCalculationOptions lobjJoint100 = this.iclbBenefitCalculationDetail.FirstOrDefault().iclbBenefitCalculationOptions.Where(x => x.icdoBenefitCalculationOptions.istrBenefitOptionDescription == "Joint & 100% Pop-up Annuity").FirstOrDefault();
                            lbus.istrJP100 = lobjJoint100.icdoBenefitCalculationOptions.benefit_amount.ToString();
                        }
                        if (astrBenOption == busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY)
                        {
                            busBenefitCalculationOptions lobjTenYears = this.iclbBenefitCalculationDetail.FirstOrDefault().iclbBenefitCalculationOptions.Where(x => x.icdoBenefitCalculationOptions.istrBenefitOptionDescription == "Ten-Years-Certain and Life Annuity").FirstOrDefault();
                            lbus.istrTenYear = lobjTenYears.icdoBenefitCalculationOptions.benefit_amount.ToString();
                        }                        
                    }

                    if (lintRetAge == i || lbus.istrTenYear == Convert.ToString(idecAcrdBenefitLocal) || iclbBenefitCalculationDetail[0].icdoBenefitCalculationDetail.early_reduction_factor > Convert.ToDecimal(1.0))
                    {
                        int edf = 1;
                        lbus.istrERFPercentage = edf.ToString("P", CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        lbus.istrERFPercentage = iclbBenefitCalculationDetail[0].icdoBenefitCalculationDetail.early_reduction_factor.ToString("P", CultureInfo.InvariantCulture);
                    }
                    this.iclbbusBenefitCalculationOptions.Add(lbus);
                    lintSpouseAge++;
                    lintYear++;
                }
            }
            if (lstrRetirementTypeValue == busConstant.RETIREMENT_TYPE_UNREDUCED_EARLY)
            {
                for (int i = lintParticipantAge + 1; i <= lintRetAge; i++)
                {
                    busBenefitCalculationDetail lbusDetail = new busBenefitCalculationDetail { icdoBenefitCalculationDetail = new cdoBenefitCalculationDetail() };
                    lclbCalcDetail = new Collection<busBenefitCalculationDetail>();
                                        
                    lbus = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                    if (this.ibusPerson.icdoPerson.date_of_birth.Day == 1)
                    {
                        this.ibusBenefitApplication.icdoBenefitApplication.retirement_date = this.ibusPerson.icdoPerson.idtDateofBirth.AddYears(i);
                    }
                    else
                    {
                        this.ibusBenefitApplication.icdoBenefitApplication.retirement_date = busGlobalFunctions.GetLastDayofMonth(this.ibusPerson.icdoPerson.idtDateofBirth.AddYears(i)).AddDays(1);
                    }

                    lbusDetail.icdoBenefitCalculationDetail.plan_id = busConstant.LOCAL_52_PLAN_ID;
                    lbusDetail.icdoBenefitCalculationDetail.retirement_date = this.ibusBenefitApplication.icdoBenefitApplication.retirement_date;
                    lclbCalcDetail.Add(lbusDetail);

                    lintSurvivorAge = Convert.ToInt32(busGlobalFunctions.CalculatePersonAge(this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth, this.ibusBenefitApplication.icdoBenefitApplication.retirement_date));
                    lbus.iintParticipantAge = i;
                    lbus.iintSpouseAge = lintSurvivorAge;

                    foreach (string astrBenOption in arrOption)
                    {
                        if (astrBenOption == busConstant.LIFE_ANNUTIY)
                        {
                            lbus.istrLife = this.iclbbusBenefitCalculationOptions.FirstOrDefault().istrLife;
                        }
                        if (astrBenOption == busConstant.JOINT_50_PERCENT_POPUP_ANNUITY)
                        {
                            lbus.istrJP50 = this.iclbbusBenefitCalculationOptions.FirstOrDefault().istrJP50;
                        }
                        if (astrBenOption == busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY)
                        {
                            lbus.istrJS75 = this.iclbbusBenefitCalculationOptions.FirstOrDefault().istrJS75;
                        }
                        if (astrBenOption == busConstant.JOINT_100_PERCENT_POPUP_ANNUITY)
                        {
                            lbus.istrJP100 = this.iclbbusBenefitCalculationOptions.FirstOrDefault().istrJP100;
                        }
                        if (astrBenOption == busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY)
                        {                            
                            lbus.istrTenYear = this.iclbbusBenefitCalculationOptions.FirstOrDefault().istrTenYear;
                        }
                    }
                    lbus.istrERFPercentage = this.iclbbusBenefitCalculationOptions.FirstOrDefault().istrERFPercentage;
                    this.iclbbusBenefitCalculationOptions.Add(lbus);
                }
           }
        }

        public void CalculateLocal52BenefitOptionsForCorr(string astrBenefitOption, decimal adecTotalBenefitAmount, decimal adecParticipantFullAge, decimal adecSurvivorFullAge, bool ablnConvertBenefitOption = false, string astrOriginalBenefitOption = "")
        {
            this.iclbBenefitCalculationDetail.FirstOrDefault().iclbBenefitCalculationOptions.Clear();
            #region Variables Required in Switch Case
            decimal ldecFactor = new decimal();
            busBenefitCalculationOptions lbusBenefitCalculationOption;
            #endregion

            #region SwitchCase Based on BenefitOptions to be Calculated
            bool lblnCheckIfSpouse = busConstant.BOOL_FALSE;
            lblnCheckIfSpouse = adecSurvivorFullAge > 0 ? true : this.ibusCalculation.CheckIfSpouse(this.icdoBenefitCalculationHeader.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id);
            decimal ldecSpecialYears = this.ibusBenefitApplication.aclbPersonWorkHistory_MPI.Where(item => item.year >= busConstant.BenefitCalculation.MERGER_DATE_LOCAL_52.Year && item.qualified_hours >= 400).Count() + this.ibusBenefitApplication.Local52_PensionCredits;
            //PIR 371
            decimal ldecDiffAgeFactor = 0;
            if (this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth > this.ibusPerson.icdoPerson.idtDateofBirth)
            {
                ldecDiffAgeFactor = busGlobalFunctions.CalculatePersonAge(this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth, this.ibusPerson.icdoPerson.idtDateofBirth);
            }
            else
            {
                ldecDiffAgeFactor = busGlobalFunctions.CalculatePersonAge(this.ibusPerson.icdoPerson.idtDateofBirth, this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth) * -1;
            }
            switch (astrBenefitOption)
            {
                case busConstant.CodeValueAll:

                    #region Local 52 LUMP SUM
                    //Ticket - 61531
                    ldecFactor = Math.Round(ibusCalculation.GetLumpsumBenefitFactorByAGE(Convert.ToInt32(adecParticipantFullAge)) * 12, 3);
                    lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                    lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_52_PLAN_ID, busConstant.LUMP_SUM), ldecFactor,
                                                            Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor)),
                                                            busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                            this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                            busConstant.LUMP_SUM, busConstant.ZERO_DECIMAL);

                    decimal ldecTotalLumpSum = GetMPIPHPLumpSum() + lbusBenefitCalculationOption.icdoBenefitCalculationOptions.benefit_amount;
                    if (ldecTotalLumpSum <= busConstant.BenefitCalculation.LUMP_SUM_LIMIT)
                    {
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_52_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    }
                    #endregion

                    if (ldecTotalLumpSum > busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT)
                    {
                        if (lblnCheckIfSpouse)
                        {
                            #region JOINT_50_PERCENT_POPUP_ANNUITY
                            //ldecFactor = Math.Min((Math.Round((this.icdoBenefitCalculationHeader.age - this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement), 0) * Convert.ToDecimal(0.005)) + Convert.ToDecimal(0.94), Convert.ToDecimal(0.99));
                            // PIR - 371
                            ldecFactor = ibusCalculation.GetRetirementL52Factor(busConstant.JOINT_50_PERCENT_POPUP_ANNUITY, this.ibusPerson.icdoPerson.idtDateofBirth, this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth);
                            lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                            this.idecQualifiedJointAndSurvivorAnnuity50 = Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor));
                            lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_52_PLAN_ID, busConstant.JOINT_50_PERCENT_POPUP_ANNUITY), ldecFactor,
                                                                    Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor)),
                                                                    busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                    this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                    busConstant.JOINT_50_PERCENT_POPUP_ANNUITY, Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor)) * 50 / 100));

                            // No need to show the Relative Value for this Benefit Option
                            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_52_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                            #endregion

                            #region JOINT_75_PERCENT_SURVIVOR_ANNUITY
                            //ldecFactor = Math.Min((Math.Round((this.icdoBenefitCalculationHeader.age - this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement), 0) * Convert.ToDecimal(0.006)) + Convert.ToDecimal(0.895), Convert.ToDecimal(0.99));
                            // PIR - 371
                            ldecFactor = ibusCalculation.GetRetirementL52Factor(busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, this.ibusPerson.icdoPerson.idtDateofBirth, this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth);
                            lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                            lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_52_PLAN_ID, busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY), ldecFactor,
                                                                    Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor)),
                                                                    busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                    this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                    busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, Convert.ToDecimal(Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor)) * 75 / 100));

                            //if (Math.Round(Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor)) / this.idecQualifiedJointAndSurvivorAnnuity50, 2) == 0.95m)
                            //{
                            //    lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = busConstant.BenefitCalculation.RELATIVE_VALUE_APPROX_EQUAL;
                            //}
                            //else
                            //{
                            //    lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = Math.Round(Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor)) / this.idecQualifiedJointAndSurvivorAnnuity50 * 100, 2).ToString() + " %";
                            //}

                            lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, adecParticipantFullAge, adecSurvivorFullAge);
                            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_52_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                            #endregion

                            #region JOINT_100_PERCENT_POPUP_ANNUITY
                            //ldecFactor = Math.Min((Math.Round((this.icdoBenefitCalculationHeader.age - this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement), 0) * Convert.ToDecimal(0.006)) + Convert.ToDecimal(0.85), Convert.ToDecimal(0.99));
                            // PIR - 371
                            ldecFactor = ibusCalculation.GetRetirementL52Factor(busConstant.JOINT_100_PERCENT_POPUP_ANNUITY, this.ibusPerson.icdoPerson.idtDateofBirth, this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth);
                            lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                            lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_52_PLAN_ID, busConstant.JOINT_100_PERCENT_POPUP_ANNUITY), ldecFactor,
                                                                    Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor)),
                                                                    busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                    this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                    busConstant.JOINT_100_PERCENT_POPUP_ANNUITY, Convert.ToDecimal(Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor)) * 100 / 100));

                            //if (Math.Round(Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor)) / this.idecQualifiedJointAndSurvivorAnnuity50, 2) == 0.95m)
                            //{
                            //    lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = busConstant.BenefitCalculation.RELATIVE_VALUE_APPROX_EQUAL;
                            //}
                            //else
                            //{
                            //    lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = Math.Round(Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor)) / this.idecQualifiedJointAndSurvivorAnnuity50 * 100, 2).ToString() + " %";
                            //}

                            lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.JOINT_100_PERCENT_POPUP_ANNUITY, adecParticipantFullAge, adecSurvivorFullAge);
                            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_52_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                            #endregion
                        }

                        #region TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY
                        if (ldecSpecialYears >= 15)
                        {
                            lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                            lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_52_PLAN_ID, busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY), Decimal.One,
                                                                    Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount)),
                                                                    busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                    this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                    busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, busConstant.ZERO_DECIMAL);

                            lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, adecParticipantFullAge, adecSurvivorFullAge);
                            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_52_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                        }
                        #endregion

                        #region LIFE_ANNUTIY
                        if (ldecSpecialYears < 15)
                        {
                            lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                            lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_52_PLAN_ID, busConstant.LIFE_ANNUTIY), Decimal.One,
                                                                     Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount)),
                                                                    busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                    this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                    busConstant.LIFE_ANNUTIY, busConstant.ZERO_DECIMAL);

                            lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.LIFE_ANNUTIY, adecParticipantFullAge, adecSurvivorFullAge);
                            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_52_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                        }
                        #endregion
                    }
                    break;

                case busConstant.LIFE_ANNUTIY:
                    if (ldecSpecialYears < 15 || ablnConvertBenefitOption)  //PIR 894
                    {
                        lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                        lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_52_PLAN_ID, busConstant.LIFE_ANNUTIY), Decimal.One,
                                                                Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount)),
                                                                busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                busConstant.LIFE_ANNUTIY, busConstant.ZERO_DECIMAL);

                        //PIR 894
                        if (ablnConvertBenefitOption)
                        {
                            if (astrOriginalBenefitOption == busConstant.JOINT_50_PERCENT_POPUP_ANNUITY)
                            {
                                lbusBenefitCalculationOption.icdoBenefitCalculationOptions.pop_up_option_factor
                                    = ibusCalculation.GetRetirementL52Factor(busConstant.JOINT_50_PERCENT_POPUP_ANNUITY, this.ibusPerson.icdoPerson.idtDateofBirth, this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth);

                                lbusBenefitCalculationOption.icdoBenefitCalculationOptions.pop_up_benefit_amount = Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * lbusBenefitCalculationOption.icdoBenefitCalculationOptions.pop_up_option_factor));
                            }
                            else if (astrOriginalBenefitOption == busConstant.JOINT_100_PERCENT_POPUP_ANNUITY)
                            {
                                lbusBenefitCalculationOption.icdoBenefitCalculationOptions.pop_up_option_factor
                                    = ibusCalculation.GetRetirementL52Factor(busConstant.JOINT_100_PERCENT_POPUP_ANNUITY, this.ibusPerson.icdoPerson.idtDateofBirth, this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth);

                                lbusBenefitCalculationOption.icdoBenefitCalculationOptions.pop_up_option_factor = Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * lbusBenefitCalculationOption.icdoBenefitCalculationOptions.pop_up_option_factor));
                            }
                        }

                        lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.LIFE_ANNUTIY, adecParticipantFullAge, adecSurvivorFullAge);
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_52_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    }
                    break;

                case busConstant.JOINT_50_PERCENT_POPUP_ANNUITY:
                    if (lblnCheckIfSpouse)
                    {
                        //ldecFactor = Math.Min((Math.Round((this.icdoBenefitCalculationHeader.age - this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement), 0) * Convert.ToDecimal(0.005)) + Convert.ToDecimal(0.94), Convert.ToDecimal(0.99));
                        // PIR - 371
                        ldecFactor = ibusCalculation.GetRetirementL52Factor(busConstant.JOINT_50_PERCENT_POPUP_ANNUITY, this.ibusPerson.icdoPerson.idtDateofBirth, this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth);
                        lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                        lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_52_PLAN_ID, busConstant.JOINT_50_PERCENT_POPUP_ANNUITY), ldecFactor,
                                                                Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor)),
                                                                busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                busConstant.JOINT_50_PERCENT_POPUP_ANNUITY, Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) * 50 / 100)));

                        lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.JOINT_50_PERCENT_POPUP_ANNUITY, adecParticipantFullAge, adecSurvivorFullAge);
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_52_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    }
                    break;

                case busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY:
                    if (lblnCheckIfSpouse)
                    {
                        // PIR - 371
                        ldecFactor = ibusCalculation.GetRetirementL52Factor(busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, this.ibusPerson.icdoPerson.idtDateofBirth, this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth);
                        lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                        lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_52_PLAN_ID, busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY), ldecFactor,
                                                                Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor)),
                                                                busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) * 75 / 100)));

                        lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, adecParticipantFullAge, adecSurvivorFullAge);
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_52_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    }
                    break;

                case busConstant.JOINT_100_PERCENT_POPUP_ANNUITY:
                    if (lblnCheckIfSpouse)
                    {
                        // PIR - 371
                        ldecFactor = ibusCalculation.GetRetirementL52Factor(busConstant.JOINT_100_PERCENT_POPUP_ANNUITY, this.ibusPerson.icdoPerson.idtDateofBirth, this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth);
                        lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                        lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_52_PLAN_ID, busConstant.JOINT_100_PERCENT_POPUP_ANNUITY), ldecFactor,
                                                                Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor)),
                                                                busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                busConstant.JOINT_100_PERCENT_POPUP_ANNUITY, Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) * 100 / 100)));

                        lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.JOINT_100_PERCENT_POPUP_ANNUITY, adecParticipantFullAge, adecSurvivorFullAge);
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_52_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    }
                    break;

                case busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY:
                    if (ldecSpecialYears >= 15)
                    {
                        lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                        lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_52_PLAN_ID, busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY), Decimal.One,
                                                                Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * Decimal.One)),
                                                                busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, busConstant.ZERO_DECIMAL);

                        lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, adecParticipantFullAge, adecSurvivorFullAge);
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_52_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    }
                    break;

                case busConstant.LUMP_SUM:
                    //Ticket - 61531
                    ldecFactor = Math.Round(ibusCalculation.GetLumpsumBenefitFactorByAGE(Convert.ToInt32(adecParticipantFullAge)) * 12, 3);
                    lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                    lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_52_PLAN_ID, busConstant.LUMP_SUM), ldecFactor,
                                                            Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor)),
                                                            busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                            this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                            busConstant.LUMP_SUM, busConstant.ZERO_DECIMAL);

                    ldecTotalLumpSum = GetMPIPHPLumpSum() + lbusBenefitCalculationOption.icdoBenefitCalculationOptions.benefit_amount;
                    if (ldecTotalLumpSum < busConstant.BenefitCalculation.LUMP_SUM_LIMIT)
                    {
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_52_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    }
                    break;
            }
            #endregion

            decimal ldecLumpSumFactor = ldecFactor = ibusCalculation.GetLumpsumBenefitFactorByAGE(Convert.ToInt32(adecParticipantFullAge)) * 12;
            ldecLumpSumFactor = Math.Round(ldecLumpSumFactor, 3);
            decimal ldecPresentValue = Math.Round(adecTotalBenefitAmount * ldecLumpSumFactor, 3);
            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_52_PLAN_ID).First().icdoBenefitCalculationDetail.present_value_amount = ldecPresentValue;
        }
        #endregion

        #region - local 161 Correspondence
        public void Calc161BenefitOptionsForCorr(string astrRetirementTypeValue, int aintParticipantAge, ref ArrayList arrOption, ref Collection<busBenefitCalculationDetail> lclbCalcDetail, ref busBenefitCalculationOptions lbus)
        {
            int lintPersonAccountId = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).First().icdoPersonAccount.person_account_id;
            string lstrRetirementTypeValue = astrRetirementTypeValue;
            int lintParticipantAge = aintParticipantAge;
            int lintSurvivorAge = 0;
            int lintRetAge = Convert.ToInt32(ibusCalculation.GetNormalRetirementAge(busConstant.LOCAL_161_PLAN_ID));
            DateTime ldtRetirementDate = this.icdoBenefitCalculationHeader.retirement_date;
            int lintYear = this.icdoBenefitCalculationHeader.retirement_date.Year + 1;
            int lintSpouseAge = this.iclbbusBenefitCalculationOptions.LastOrDefault().iintSpouseAge + 1;

            if (lstrRetirementTypeValue == busConstant.RETIREMENT_TYPE_REDUCED_EARLY || lstrRetirementTypeValue == busConstant.RETIREMENT_TYPE_SPL_REDUCED_EARLY)
            {                
                for (int i = lintParticipantAge + 1; i <= lintRetAge; i++)
                {
                    busBenefitCalculationDetail lbusDetail = new busBenefitCalculationDetail { icdoBenefitCalculationDetail = new cdoBenefitCalculationDetail() };
                    lclbCalcDetail = new Collection<busBenefitCalculationDetail>();

                    lbus = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                    if (this.ibusPerson.icdoPerson.date_of_birth.Day == 1)
                    {
                        this.ibusBenefitApplication.icdoBenefitApplication.retirement_date = this.ibusPerson.icdoPerson.idtDateofBirth.AddYears(i);
                    }
                    else
                    {
                        this.ibusBenefitApplication.icdoBenefitApplication.retirement_date = busGlobalFunctions.GetLastDayofMonth(this.ibusPerson.icdoPerson.idtDateofBirth.AddYears(i)).AddDays(1);
                    }

                    lbusDetail.icdoBenefitCalculationDetail.plan_id = busConstant.LOCAL_161_PLAN_ID;
                    lbusDetail.icdoBenefitCalculationDetail.retirement_date = this.ibusBenefitApplication.icdoBenefitApplication.retirement_date;
                    lclbCalcDetail.Add(lbusDetail);

                    lintSurvivorAge = Convert.ToInt32(busGlobalFunctions.CalculatePersonAge(this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth, this.ibusBenefitApplication.icdoBenefitApplication.retirement_date));
                    lbus.iintParticipantAge = i;
                    lbus.iintSpouseAge = lintSurvivorAge;
                    this.ibusBenefitApplication.idecAge = Convert.ToDecimal(i);                    

                    //rid 82228
                    decimal ldecLocalFrozenBenefit_amount = this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_frozen_benefit_amount);
                    decimal ldecLocalPreBisAmount = this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_pre_bis_amount);
                    decimal ldecLocalPostBisAmount = this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_post_bis_amount);
                    int lintComputationalYear = Convert.ToInt32(this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Max(item => item.icdoPersonAccountRetirementContribution.computational_year));
                    ldtRetirementDate = Convert.ToDateTime(ldtRetirementDate.Month.ToString() + "/" + ldtRetirementDate.Day.ToString() + "/" + lintYear.ToString());

                    decimal ldecTotalBenefitAmount = 0;

                    if (iclbBenefitCalculationDetail[0].icdoBenefitCalculationDetail.early_reduction_factor <= Convert.ToDecimal(1.0))
                    {
                        ldecTotalBenefitAmount = this.ibusCalculation.CalculateTotalBenefitAmtForLocal161(astrRetirementTypeValue, //this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_161).First().icdoPersonAccount.istrRetirementSubType,
                                                            ldtRetirementDate, //this.icdoBenefitCalculationHeader.retirement_date,
                                                            this.ibusPerson.icdoPerson.idtDateofBirth,
                                                            ldecLocalFrozenBenefit_amount, false,
                                                            this.ibusBenefitApplication,
                                                            ldecLocalPreBisAmount,
                                                            ldecLocalPostBisAmount, null,
                                                            this.iclbBenefitCalculationDetail,
                                                            lintComputationalYear,
                                                            this.iclbPersonAccountRetirementContribution, busConstant.BOOL_FALSE, this.ibusPerson.icdoPerson.person_id);

                    }
                    if (iclbBenefitCalculationDetail[0].icdoBenefitCalculationDetail.early_reduction_factor > Convert.ToDecimal(1.0))
                    {
                        astrRetirementTypeValue = busConstant.RETIREMENT_TYPE_UNREDUCED_EARLY;
                        ldecTotalBenefitAmount = this.ibusCalculation.CalculateTotalBenefitAmtForLocal161(astrRetirementTypeValue, //this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_52).First().icdoPersonAccount.istrRetirementSubType,
                                                             ldtRetirementDate, //this.icdoBenefitCalculationHeader.retirement_date,
                                                             this.ibusPerson.icdoPerson.idtDateofBirth,
                                                             ldecLocalFrozenBenefit_amount, false,
                                                             this.ibusBenefitApplication,
                                                             ldecLocalPreBisAmount,
                                                             ldecLocalPostBisAmount, null,
                                                             this.iclbBenefitCalculationDetail,
                                                             lintComputationalYear,
                                                             this.iclbPersonAccountRetirementContribution, busConstant.BOOL_FALSE, this.ibusPerson.icdoPerson.person_id);
                    }
                    
                    CalculateLocal161BenefitOptionsForCorr(busConstant.CodeValueAll, ldecTotalBenefitAmount, Convert.ToDecimal(i), lintSpouseAge);

                    foreach (string astrBenOption in arrOption)
                    {
                        if (astrBenOption == busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY)
                        {
                            busBenefitCalculationOptions lobjJoint75 = this.iclbBenefitCalculationDetail.FirstOrDefault().iclbBenefitCalculationOptions.Where(x => x.icdoBenefitCalculationOptions.istrBenefitOptionDescription == "Qualified Joint & 50% Survivor Annuity").FirstOrDefault();
                            lbus.istrJS50 = lobjJoint75.icdoBenefitCalculationOptions.benefit_amount.ToString();
                        }
                        if (astrBenOption == busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY)
                        {
                            busBenefitCalculationOptions lobjJoint75 = this.iclbBenefitCalculationDetail.FirstOrDefault().iclbBenefitCalculationOptions.Where(x => x.icdoBenefitCalculationOptions.istrBenefitOptionDescription == "Joint & 75% Survivor Annuity").FirstOrDefault();
                            lbus.istrJS75 = lobjJoint75.icdoBenefitCalculationOptions.benefit_amount.ToString();
                        }
                        if (astrBenOption == busConstant.FIVE_YEARS_CERTAIN_AND_LIFE_ANNUTIY)
                        {
                            busBenefitCalculationOptions lobjFiveYears = this.iclbBenefitCalculationDetail.FirstOrDefault().iclbBenefitCalculationOptions.Where(x => x.icdoBenefitCalculationOptions.istrBenefitOptionDescription == "Five-Years-Certain and Life Annuity").FirstOrDefault();
                            lbus.istrFiveYear = lobjFiveYears.icdoBenefitCalculationOptions.benefit_amount.ToString();
                        }
                    }

                    if (lintRetAge == i || lbus.istrFiveYear == Convert.ToString(idecAcrdBenefitLocal) || iclbBenefitCalculationDetail[0].icdoBenefitCalculationDetail.early_reduction_factor > Convert.ToDecimal(1.0))
                    {
                        int edf = 1;
                        lbus.istrERFPercentage = edf.ToString("P", CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        lbus.istrERFPercentage = iclbBenefitCalculationDetail[0].icdoBenefitCalculationDetail.early_reduction_factor.ToString("P", CultureInfo.InvariantCulture);
                    }
                    this.iclbbusBenefitCalculationOptions.Add(lbus);
                    lintSpouseAge++;
                    lintYear++;
                }
            }
            if (lstrRetirementTypeValue == busConstant.RETIREMENT_TYPE_UNREDUCED_EARLY)
            {
                for (int i = lintParticipantAge + 1; i <= lintRetAge; i++)
                {
                    busBenefitCalculationDetail lbusDetail = new busBenefitCalculationDetail { icdoBenefitCalculationDetail = new cdoBenefitCalculationDetail() };
                    lclbCalcDetail = new Collection<busBenefitCalculationDetail>();

                    
                    lbus = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                    if (this.ibusPerson.icdoPerson.date_of_birth.Day == 1)
                    {
                        this.ibusBenefitApplication.icdoBenefitApplication.retirement_date = this.ibusPerson.icdoPerson.idtDateofBirth.AddYears(i);
                    }
                    else
                    {
                        this.ibusBenefitApplication.icdoBenefitApplication.retirement_date = busGlobalFunctions.GetLastDayofMonth(this.ibusPerson.icdoPerson.idtDateofBirth.AddYears(i)).AddDays(1);
                    }

                    lbusDetail.icdoBenefitCalculationDetail.plan_id = busConstant.LOCAL_161_PLAN_ID;
                    lbusDetail.icdoBenefitCalculationDetail.retirement_date = this.ibusBenefitApplication.icdoBenefitApplication.retirement_date;
                    lclbCalcDetail.Add(lbusDetail);
                   
                    lbus.iintParticipantAge = i;
                    lbus.iintSpouseAge = this.iclbbusBenefitCalculationOptions.LastOrDefault().iintSpouseAge + 1; ;

                    foreach (string astrBenOption in arrOption)
                    {
                        if (astrBenOption == busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY)
                        {
                            lbus.istrJS50 = this.iclbbusBenefitCalculationOptions.FirstOrDefault().istrJS50;
                        }
                        if (astrBenOption == busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY)
                        {
                            lbus.istrJS75 = this.iclbbusBenefitCalculationOptions.FirstOrDefault().istrJS75;
                        }
                        if (astrBenOption == busConstant.FIVE_YEARS_CERTAIN_AND_LIFE_ANNUTIY)
                        {
                            lbus.istrFiveYear = this.iclbbusBenefitCalculationOptions.FirstOrDefault().istrFiveYear;
                        }
                    }
                    lbus.istrERFPercentage = this.iclbbusBenefitCalculationOptions.FirstOrDefault().istrERFPercentage;
                    this.iclbbusBenefitCalculationOptions.Add(lbus);
                }
            }
        }

        public void CalculateLocal161BenefitOptionsForCorr(string astrBenefitOption, decimal adecTotalBenefitAmount, decimal adecParticipantFullAge, decimal adecSurvivorFullAge)
        {
            this.iclbBenefitCalculationDetail.FirstOrDefault().iclbBenefitCalculationOptions.Clear();
            #region Variables Required in Switch Case
            decimal ldecFactor = new decimal();
            busBenefitCalculationOptions lbusBenefitCalculationOption;

            #endregion

            bool lblnCheckIfSpouse = busConstant.BOOL_FALSE;
            lblnCheckIfSpouse = this.ibusCalculation.CheckIfSpouse(this.icdoBenefitCalculationHeader.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id);

            int lintYears;
            int lintMonths;
            int lintDays;
            int lintBeneficiaryYears;
            int lintBeneficiaryMonths;
            int lintBeneficiaryDays;
            
            #region Swtich Case to determine the Factors and Amounts
            switch (astrBenefitOption)
            {
                case busConstant.CodeValueAll:

                    #region Local 161 LUMP SUM
                    //Ticket - 61531
                    ldecFactor = Math.Round(ibusCalculation.GetLumpsumBenefitFactorByAGE(Convert.ToInt32(adecParticipantFullAge)) * 12, 3);
                    lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                    //Query to SGT_PLAN_BENEFIT_XR to get PLAN_BENEFIT_ID
                    //lbusBenefitCalculationOption.icdoBenefitCalculationOptions.istrBenefitOptionDescription = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(1504, busConstant.LUMP_SUM);
                    //lbusBenefitCalculationOption.icdoBenefitCalculationOptions.plan_benefit_id = ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_161_PLAN_ID, busConstant.LUMP_SUM);
                    //lbusBenefitCalculationOption.icdoBenefitCalculationOptions.benefit_option_factor = ldecFactor;
                    //lbusBenefitCalculationOption.icdoBenefitCalculationOptions.benefit_amount = Convert.ToDecimal(Math.Round(ldecTotalBenefitAmount * ldecFactor, 2, MidpointRounding.AwayFromZero));

                    lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_161_PLAN_ID, busConstant.LUMP_SUM), ldecFactor,
                                                            Convert.ToDecimal(adecTotalBenefitAmount * ldecFactor),
                                                            busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                            this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                            busConstant.LUMP_SUM, busConstant.ZERO_DECIMAL);



                    decimal ldecTotalLumpSum = GetMPIPHPLumpSum() + lbusBenefitCalculationOption.icdoBenefitCalculationOptions.benefit_amount;
                    if (ldecTotalLumpSum <= busConstant.BenefitCalculation.LUMP_SUM_LIMIT)
                    {
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_161_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    }
                    #endregion

                    if (ldecTotalLumpSum > busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT)
                    {
                        if (lblnCheckIfSpouse)
                        {
                            #region JOINT_50_PERCENT_SURVIVOR_ANNUITY
                            busGlobalFunctions.GetDetailTimeSpan(this.ibusBenefitApplication.icdoBenefitApplication.retirement_date, this.ibusPerson.icdoPerson.idtDateofBirth, out lintYears, out lintMonths, out lintDays);
                            busGlobalFunctions.GetDetailTimeSpan(this.ibusBenefitApplication.icdoBenefitApplication.retirement_date, this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth, out lintBeneficiaryYears, out lintBeneficiaryMonths, out lintBeneficiaryDays);

                            ldecFactor = ibusCalculation.GetBenefitProvisionBenefitOptionFactor(busConstant.LOCAL_161_PLAN_ID, busConstant.BENEFIT_TYPE_RETIREMENT, ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_161_PLAN_ID, busConstant.QJ50), lintYears, lintMonths, lintBeneficiaryYears, lintBeneficiaryMonths);

                            //ldecFactor = GetBenefitFactorLocal(busConstant.Local_161, busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY, busConstant.BENEFIT_TYPE_RETIREMENT, this.icdoBenefitCalculationHeader.age, this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement);
                            lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                            this.idecQualifiedJointAndSurvivorAnnuity50 = Math.Round(adecTotalBenefitAmount * ldecFactor, 2);
                            //PIR 229 : Local 161 
                            lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_161_PLAN_ID, busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY), ldecFactor,
                                            Math.Round(adecTotalBenefitAmount * ldecFactor, 2),
                                            busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                            this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                            busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY, Math.Round(this.idecQualifiedJointAndSurvivorAnnuity50 * 0.5M, 2));

                            // No Need to show the Relative Value for JOINT_50_PERCENT_SURVIVOR_ANNUITY Benefit Option
                            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_161_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                            #endregion


                            #region JOINT_75_PERCENT_SURVIVOR_ANNUITY
                            ldecFactor = ibusCalculation.GetBenefitProvisionBenefitOptionFactor(busConstant.LOCAL_161_PLAN_ID, busConstant.BENEFIT_TYPE_RETIREMENT, ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_161_PLAN_ID, busConstant.JS75), lintYears, lintMonths, lintBeneficiaryYears, lintBeneficiaryMonths);

                            lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                            lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_161_PLAN_ID, busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY), ldecFactor,
                                        Convert.ToDecimal(adecTotalBenefitAmount * ldecFactor),
                                        busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                        this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                        busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, Math.Round(Math.Round(adecTotalBenefitAmount * ldecFactor, 2) * 75 / 100, 2));


                            //if (Math.Round(Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor)) / this.idecQualifiedJointAndSurvivorAnnuity50, 2) == 0.95m)
                            //{
                            //    lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = busConstant.BenefitCalculation.RELATIVE_VALUE_APPROX_EQUAL;
                            //}
                            //else
                            //{
                            //    lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = Math.Round(Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor)) / this.idecQualifiedJointAndSurvivorAnnuity50 * 100, 2).ToString() + " %";
                            //}

                            lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, adecParticipantFullAge, adecSurvivorFullAge);
                            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_161_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                            #endregion
                        }

                        #region FIVE_YEARS_CERTAIN_AND_LIFE_ANNUTIY
                        lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                        lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_161_PLAN_ID, busConstant.FIVE_YEARS_CERTAIN_AND_LIFE_ANNUTIY), Decimal.One,
                                                                Convert.ToDecimal(adecTotalBenefitAmount * Decimal.One),
                                                                busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                busConstant.FIVE_YEARS_CERTAIN_AND_LIFE_ANNUTIY, busConstant.ZERO_DECIMAL);

                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_161_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                        #endregion
                    }

                    break;

                case busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY:
                    if (lblnCheckIfSpouse)
                    {



                        busGlobalFunctions.GetDetailTimeSpan(this.ibusBenefitApplication.icdoBenefitApplication.retirement_date, this.ibusPerson.icdoPerson.idtDateofBirth, out lintYears, out lintMonths, out lintDays);
                        busGlobalFunctions.GetDetailTimeSpan(this.ibusBenefitApplication.icdoBenefitApplication.retirement_date, this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth, out lintBeneficiaryYears, out lintBeneficiaryMonths, out lintBeneficiaryDays);

                        ldecFactor = ibusCalculation.GetBenefitProvisionBenefitOptionFactor(busConstant.LOCAL_161_PLAN_ID, busConstant.BENEFIT_TYPE_RETIREMENT, ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_161_PLAN_ID, busConstant.QJ50), lintYears, lintMonths, lintBeneficiaryYears, lintBeneficiaryMonths);

                        //ldecFactor = GetBenefitFactorLocal(busConstant.Local_161, busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY, busConstant.BENEFIT_TYPE_RETIREMENT, this.icdoBenefitCalculationHeader.age, this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement);
                        lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                        lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_161_PLAN_ID, busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY), ldecFactor,
                                                                Math.Round(adecTotalBenefitAmount * ldecFactor, 2),
                                                                busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY, Math.Round(Math.Round(adecTotalBenefitAmount * ldecFactor, 2) * 50 / 100, 2));

                        // No need to show the Relative Value for this Benefit Option
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_161_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    }
                    break;

                case busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY:
                    if (lblnCheckIfSpouse)
                    {
                        busGlobalFunctions.GetDetailTimeSpan(this.ibusBenefitApplication.icdoBenefitApplication.retirement_date, this.ibusPerson.icdoPerson.idtDateofBirth, out lintYears, out lintMonths, out lintDays);
                        busGlobalFunctions.GetDetailTimeSpan(this.ibusBenefitApplication.icdoBenefitApplication.retirement_date, this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth, out lintBeneficiaryYears, out lintBeneficiaryMonths, out lintBeneficiaryDays);

                        ldecFactor = ibusCalculation.GetBenefitProvisionBenefitOptionFactor(busConstant.LOCAL_161_PLAN_ID, busConstant.BENEFIT_TYPE_RETIREMENT, ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_161_PLAN_ID, busConstant.JS75), lintYears, lintMonths, lintBeneficiaryYears, lintBeneficiaryMonths);

                        //ldecFactor = GetBenefitFactorLocal(busConstant.Local_161, busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, busConstant.BENEFIT_TYPE_RETIREMENT, this.icdoBenefitCalculationHeader.idecParticipantFullAge, this.icdoBenefitCalculationHeader.idecSurvivorFullAge);
                        lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                        lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_161_PLAN_ID, busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY), ldecFactor,
                                                                Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor)),
                                                                busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, Math.Round(Math.Round(adecTotalBenefitAmount * ldecFactor, 2) * 75 / 100, 2));

                        lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, adecParticipantFullAge, adecSurvivorFullAge);
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_161_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    }
                    break;

                case busConstant.FIVE_YEARS_CERTAIN_AND_LIFE_ANNUTIY:
                    lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                    lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_161_PLAN_ID, busConstant.FIVE_YEARS_CERTAIN_AND_LIFE_ANNUTIY), Decimal.One,
                                                            Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * Decimal.One)),
                                                            busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                            this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                            busConstant.FIVE_YEARS_CERTAIN_AND_LIFE_ANNUTIY, busConstant.ZERO_DECIMAL);

                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_161_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    break;

                case busConstant.LUMP_SUM:
                    //Ticket - 61531
                    ldecFactor = Math.Round(ibusCalculation.GetLumpsumBenefitFactorByAGE(Convert.ToInt32(adecParticipantFullAge)) * 12, 3);
                    lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                    lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_161_PLAN_ID, busConstant.LUMP_SUM), ldecFactor,
                                                            Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor)),
                                                            busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                            this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                            busConstant.LUMP_SUM, busConstant.ZERO_DECIMAL);

                    ldecTotalLumpSum = GetMPIPHPLumpSum() + lbusBenefitCalculationOption.icdoBenefitCalculationOptions.benefit_amount;
                    if (ldecTotalLumpSum < busConstant.BenefitCalculation.LUMP_SUM_LIMIT)
                    {
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_161_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    }
                    break;

            }

            decimal ldecLumpSumFactor = ldecFactor = ibusCalculation.GetLumpsumBenefitFactorByAGE(Convert.ToInt32(adecParticipantFullAge)) * 12;
            ldecLumpSumFactor = Math.Round(ldecLumpSumFactor, 3);
            decimal ldecPresentValue = Math.Round(adecTotalBenefitAmount * ldecLumpSumFactor, 3);
            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_161_PLAN_ID).First().icdoBenefitCalculationDetail.present_value_amount = ldecPresentValue;
            #endregion
        }
        #endregion

        #region - local 600 Correspondence
        public void Calc600BenefitOptionsForCorr(string astrRetirementTypeValue, int aintParticipantAge, ref ArrayList arrOption, ref Collection<busBenefitCalculationDetail> lclbCalcDetail, ref busBenefitCalculationOptions lbus)
        {            
            int lintPersonAccountId = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).First().icdoPersonAccount.person_account_id;
            string lstrRetirementTypeValue = astrRetirementTypeValue;
            int lintParticipantAge = aintParticipantAge;
            int lintSurvivorAge = 0;
            int lintRetAge = Convert.ToInt32(ibusCalculation.GetNormalRetirementAge(busConstant.LOCAL_600_PLAN_ID));
            DateTime ldtRetirementDate = this.icdoBenefitCalculationHeader.retirement_date;
            int lintYear = this.icdoBenefitCalculationHeader.retirement_date.Year + 1;
            int lintSpouseAge = this.iclbbusBenefitCalculationOptions.LastOrDefault().iintSpouseAge + 1;

            if (lstrRetirementTypeValue == busConstant.RETIREMENT_TYPE_REDUCED_EARLY || lstrRetirementTypeValue == busConstant.RETIREMENT_TYPE_SPL_REDUCED_EARLY)
            {               
                for (int i = lintParticipantAge + 1; i <= lintRetAge; i++)
                {
                    busBenefitCalculationDetail lbusDetail = new busBenefitCalculationDetail { icdoBenefitCalculationDetail = new cdoBenefitCalculationDetail() };
                    lclbCalcDetail = new Collection<busBenefitCalculationDetail>();

                    lbus = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                    if (this.ibusPerson.icdoPerson.date_of_birth.Day == 1)
                    {
                        this.ibusBenefitApplication.icdoBenefitApplication.retirement_date = this.ibusPerson.icdoPerson.idtDateofBirth.AddYears(i);
                    }
                    else
                    {
                        this.ibusBenefitApplication.icdoBenefitApplication.retirement_date = busGlobalFunctions.GetLastDayofMonth(this.ibusPerson.icdoPerson.idtDateofBirth.AddYears(i)).AddDays(1);
                    }

                    lbusDetail.icdoBenefitCalculationDetail.plan_id = busConstant.LOCAL_600_PLAN_ID;
                    lbusDetail.icdoBenefitCalculationDetail.retirement_date = this.ibusBenefitApplication.icdoBenefitApplication.retirement_date;
                    lclbCalcDetail.Add(lbusDetail);

                    lintSurvivorAge = Convert.ToInt32(busGlobalFunctions.CalculatePersonAge(this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth, this.ibusBenefitApplication.icdoBenefitApplication.retirement_date));
                    lbus.iintParticipantAge = i;
                    lbus.iintSpouseAge = this.iclbbusBenefitCalculationOptions.LastOrDefault().iintSpouseAge + 1;   //lintSurvivorAge;
                    this.ibusBenefitApplication.idecAge = Convert.ToDecimal(i);

                    //rid 82228
                    decimal ldecLocalFrozenBenefit_amount = this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_frozen_benefit_amount);
                    decimal ldecLocalPreBisAmount = this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_pre_bis_amount);
                    decimal ldecLocalPostBisAmount = this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_post_bis_amount);
                    int lintComputationalYear = Convert.ToInt32(this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Max(item => item.icdoPersonAccountRetirementContribution.computational_year));
                    ldtRetirementDate = Convert.ToDateTime(ldtRetirementDate.Month.ToString() + "/" + ldtRetirementDate.Day.ToString() + "/" + lintYear.ToString());

                    decimal ldecTotalBenefitAmount = 0;

                    if (iclbBenefitCalculationDetail[0].icdoBenefitCalculationDetail.early_reduction_factor <= Convert.ToDecimal(1.0))
                    {
                        ldecTotalBenefitAmount = this.ibusCalculation.CalculateTotalBenefitAmtForLocal600(astrRetirementTypeValue,//this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_600).First().icdoPersonAccount.istrRetirementSubType,
                                                ldtRetirementDate, //this.icdoBenefitCalculationHeader.retirement_date,
                                                this.ibusPerson.icdoPerson.idtDateofBirth,
                                                ldecLocalFrozenBenefit_amount, false,
                                                this.ibusBenefitApplication,
                                                ldecLocalPreBisAmount,
                                                ldecLocalPostBisAmount, null,
                                                this.iclbBenefitCalculationDetail,
                                                lintComputationalYear,
                                                this.iclbPersonAccountRetirementContribution, busConstant.BOOL_FALSE, this.ibusPerson.icdoPerson.person_id);
                    }

                    if (iclbBenefitCalculationDetail[0].icdoBenefitCalculationDetail.early_reduction_factor > Convert.ToDecimal(1.0))
                    {
                        astrRetirementTypeValue = busConstant.RETIREMENT_TYPE_UNREDUCED_EARLY;
                        ldecTotalBenefitAmount = this.ibusCalculation.CalculateTotalBenefitAmtForLocal600(astrRetirementTypeValue, //this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_52).First().icdoPersonAccount.istrRetirementSubType,
                                                ldtRetirementDate, //this.icdoBenefitCalculationHeader.retirement_date,
                                                this.ibusPerson.icdoPerson.idtDateofBirth,
                                                ldecLocalFrozenBenefit_amount, false,
                                                this.ibusBenefitApplication,
                                                ldecLocalPreBisAmount,
                                                ldecLocalPostBisAmount, null,
                                                this.iclbBenefitCalculationDetail,
                                                lintComputationalYear,
                                                this.iclbPersonAccountRetirementContribution, busConstant.BOOL_FALSE, this.ibusPerson.icdoPerson.person_id);
                    }
                    
                    CalculateLocal600BenefitOptionsForCorr(busConstant.CodeValueAll, ldecTotalBenefitAmount, Convert.ToDecimal(i), lintSpouseAge);
                    foreach (string astrBenOption in arrOption)
                    {
                        if (astrBenOption == busConstant.JOINT_50_PERCENT_POPUP_ANNUITY)
                        {
                            busBenefitCalculationOptions lobjJoint50 = this.iclbBenefitCalculationDetail.FirstOrDefault().iclbBenefitCalculationOptions.Where(x => x.icdoBenefitCalculationOptions.istrBenefitOptionDescription == "Joint & 50% Pop-up Annuity").FirstOrDefault();
                            lbus.istrJP50 = lobjJoint50.icdoBenefitCalculationOptions.benefit_amount.ToString();
                        }
                        if (astrBenOption == busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY)
                        {
                            busBenefitCalculationOptions lobjJoint75 = this.iclbBenefitCalculationDetail.FirstOrDefault().iclbBenefitCalculationOptions.Where(x => x.icdoBenefitCalculationOptions.istrBenefitOptionDescription == "Joint & 75% Survivor Annuity").FirstOrDefault();
                            lbus.istrJS75 = lobjJoint75.icdoBenefitCalculationOptions.benefit_amount.ToString();
                        }
                        if (astrBenOption == busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY)
                        {
                            busBenefitCalculationOptions lobjTenYears = this.iclbBenefitCalculationDetail.FirstOrDefault().iclbBenefitCalculationOptions.Where(x => x.icdoBenefitCalculationOptions.istrBenefitOptionDescription == "Ten-Years-Certain and Life Annuity").FirstOrDefault();
                            lbus.istrTenYear = lobjTenYears.icdoBenefitCalculationOptions.benefit_amount.ToString();
                        }
                    }

                    if (lintRetAge == i || lbus.istrTenYear == Convert.ToString(idecAcrdBenefitLocal) || iclbBenefitCalculationDetail[0].icdoBenefitCalculationDetail.early_reduction_factor > Convert.ToDecimal(1.0))
                    {
                        int edf = 1;
                        lbus.istrERFPercentage = edf.ToString("P", CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        lbus.istrERFPercentage = iclbBenefitCalculationDetail[0].icdoBenefitCalculationDetail.early_reduction_factor.ToString("P", CultureInfo.InvariantCulture);
                    }
                    this.iclbbusBenefitCalculationOptions.Add(lbus);

                    lintSpouseAge++;
                    lintYear++;
                }
            }
            if (lstrRetirementTypeValue == busConstant.RETIREMENT_TYPE_UNREDUCED_EARLY)
            {
                for (int i = lintParticipantAge + 1; i <= lintRetAge; i++)
                {
                    busBenefitCalculationDetail lbusDetail = new busBenefitCalculationDetail { icdoBenefitCalculationDetail = new cdoBenefitCalculationDetail() };
                    lclbCalcDetail = new Collection<busBenefitCalculationDetail>();
                  
                    lbus = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                    if (this.ibusPerson.icdoPerson.date_of_birth.Day == 1)
                    {
                        this.ibusBenefitApplication.icdoBenefitApplication.retirement_date = this.ibusPerson.icdoPerson.idtDateofBirth.AddYears(i);
                    }
                    else
                    {
                        this.ibusBenefitApplication.icdoBenefitApplication.retirement_date = busGlobalFunctions.GetLastDayofMonth(this.ibusPerson.icdoPerson.idtDateofBirth.AddYears(i)).AddDays(1);
                    }

                    lbusDetail.icdoBenefitCalculationDetail.plan_id = busConstant.LOCAL_600_PLAN_ID;
                    lbusDetail.icdoBenefitCalculationDetail.retirement_date = this.ibusBenefitApplication.icdoBenefitApplication.retirement_date;
                    lclbCalcDetail.Add(lbusDetail);
                    
                    lbus.iintParticipantAge = i;
                    lbus.iintSpouseAge = this.iclbbusBenefitCalculationOptions.LastOrDefault().iintSpouseAge + 1;                    

                    foreach (string astrBenOption in arrOption)
                    {
                        if (astrBenOption == busConstant.JOINT_50_PERCENT_POPUP_ANNUITY)
                        {
                            lbus.istrJP50 = this.iclbbusBenefitCalculationOptions.FirstOrDefault().istrJP50;
                        }
                        if (astrBenOption == busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY)
                        {
                            lbus.istrJS75 = this.iclbbusBenefitCalculationOptions.FirstOrDefault().istrJS75;
                        }
                        if (astrBenOption == busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY)
                        {
                            lbus.istrTenYear = this.iclbbusBenefitCalculationOptions.FirstOrDefault().istrTenYear;
                        }
                    }
                    lbus.istrERFPercentage = this.iclbbusBenefitCalculationOptions.FirstOrDefault().istrERFPercentage;
                    this.iclbbusBenefitCalculationOptions.Add(lbus);
                }
            }
        }

        public void CalculateLocal600BenefitOptionsForCorr(string astrBenefitOption, decimal adecTotalBenefitAmount, decimal adecParticipantFullAge, decimal adecSurvivorFullAge, bool ablnConvertBenefitOption = false, string astrOriginalBenefitOption = "")//PIR 894
        {
            this.iclbBenefitCalculationDetail.FirstOrDefault().iclbBenefitCalculationOptions.Clear();
            #region Variables Required in Switch Case
            decimal ldecFactor = new decimal();
            busBenefitCalculationOptions lbusBenefitCalculationOption;
            #endregion

            #region SwitchCase Based on BenefitOptions to be Calculated
            bool lblnCheckIfSpouse = busConstant.BOOL_FALSE;
            lblnCheckIfSpouse = this.ibusCalculation.CheckIfSpouse(this.icdoBenefitCalculationHeader.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id);

            switch (astrBenefitOption)
            {
                case busConstant.CodeValueAll:

                    #region Local 600 LUMP SUM
                    //Ticket - 61531
                    ldecFactor = Math.Round(ibusCalculation.GetLumpsumBenefitFactorByAGE(Convert.ToInt32(adecParticipantFullAge)) * 12, 3);
                    lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                    lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_600_PLAN_ID, busConstant.LUMP_SUM), ldecFactor,
                                                        Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor)),
                                                            busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                            this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                            busConstant.LUMP_SUM, busConstant.ZERO_DECIMAL);

                    decimal ldecTotalLumpSum = GetMPIPHPLumpSum() + lbusBenefitCalculationOption.icdoBenefitCalculationOptions.benefit_amount;
                    if (ldecTotalLumpSum <= busConstant.BenefitCalculation.LUMP_SUM_LIMIT)
                    {
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_600_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    }
                    #endregion

                    if (ldecTotalLumpSum > busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT)
                    {
                        if (lblnCheckIfSpouse)
                        {
                            #region JOINT_50_PERCENT_POPUP_ANNUITY
                            //ldecFactor = Math.Min((Math.Round((this.icdoBenefitCalculationHeader.age - this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement), 0) * Convert.ToDecimal(0.005)) + Convert.ToDecimal(0.94), Convert.ToDecimal(0.99));
                            // PIR - 760
                            ldecFactor = ibusCalculation.GetRetirementL600Factor(busConstant.JOINT_50_PERCENT_POPUP_ANNUITY, this.ibusPerson.icdoPerson.idtDateofBirth, this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth);
                            lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                            this.idecQualifiedJointAndSurvivorAnnuity50 = Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor));
                            lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_600_PLAN_ID, busConstant.JOINT_50_PERCENT_POPUP_ANNUITY), ldecFactor,
                                                                    Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor)),
                                                                    busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                    this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                    busConstant.JOINT_50_PERCENT_POPUP_ANNUITY, Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) * 50 / 100)));//PIR-1064


                            // No  need to show the Relative Value for this Benefit Option.
                            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_600_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                            #endregion

                            #region JOINT_75_PERCENT_SURVIVOR_ANNUITY
                            //ldecFactor = Math.Min((Math.Round((this.icdoBenefitCalculationHeader.age - this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement), 0) * Convert.ToDecimal(0.005)) + Convert.ToDecimal(0.85), Convert.ToDecimal(1.0));
                            // PIR - 760
                            ldecFactor = ibusCalculation.GetRetirementL600Factor(busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, this.ibusPerson.icdoPerson.idtDateofBirth, this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth);
                            lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                            lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_600_PLAN_ID, busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY), ldecFactor,
                                                                    Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor)),
                                                                    busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                    this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                    busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) * 75 / 100)));//PIR-1064

                            //if (Math.Round(Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor)) / this.idecQualifiedJointAndSurvivorAnnuity50, 2) == 0.95m)
                            //{
                            //    lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = busConstant.BenefitCalculation.RELATIVE_VALUE_APPROX_EQUAL;
                            //}
                            //else
                            //{
                            //    lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = Math.Round(Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor)) / this.idecQualifiedJointAndSurvivorAnnuity50 * 100, 2).ToString() + " %";
                            //}

                            lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, adecParticipantFullAge, adecSurvivorFullAge);
                            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_600_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                            #endregion
                        }

                        #region TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY
                        lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                        lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_600_PLAN_ID, busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY), Decimal.One,
                                                                Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount)),
                                                                busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, busConstant.ZERO_DECIMAL);

                        lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, adecParticipantFullAge, adecSurvivorFullAge);
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_600_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                        #endregion
                    }
                    break;
                case busConstant.JOINT_50_PERCENT_POPUP_ANNUITY:
                    if (lblnCheckIfSpouse)
                    {
                        //ldecFactor = Math.Min((Math.Round((this.icdoBenefitCalculationHeader.age - this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement), 0) * Convert.ToDecimal(0.005)) + Convert.ToDecimal(0.94), Convert.ToDecimal(0.99));
                        // PIR - 760
                        ldecFactor = ibusCalculation.GetRetirementL600Factor(busConstant.JOINT_50_PERCENT_POPUP_ANNUITY, this.ibusPerson.icdoPerson.idtDateofBirth, this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth);
                        lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                        lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_600_PLAN_ID, busConstant.JOINT_50_PERCENT_POPUP_ANNUITY), ldecFactor,
                                                                Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor)),
                                                                busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                busConstant.JOINT_50_PERCENT_POPUP_ANNUITY, Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) * 50 / 100)));//PIR-1064

                        lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.JOINT_50_PERCENT_POPUP_ANNUITY, adecParticipantFullAge, adecSurvivorFullAge);
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_600_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    }
                    break;

                case busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY:
                    if (lblnCheckIfSpouse)
                    {
                        //ldecFactor = Math.Min((Math.Round((this.icdoBenefitCalculationHeader.age - this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement), 0) * Convert.ToDecimal(0.006)) + Convert.ToDecimal(0.895), Convert.ToDecimal(0.99));
                        // PIR - 760
                        ldecFactor = ibusCalculation.GetRetirementL600Factor(busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, this.ibusPerson.icdoPerson.idtDateofBirth, this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth);
                        lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                        lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_600_PLAN_ID, busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY), ldecFactor,
                                                                Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor)),
                                                                busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) * 75 / 100)));//PIR-1064

                        lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, adecParticipantFullAge, adecSurvivorFullAge);
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_600_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    }
                    break;

                case busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY:
                    lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                    lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_600_PLAN_ID, busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY), Decimal.One,
                                                            Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * Decimal.One)),
                                                            busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                            this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                            busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, busConstant.ZERO_DECIMAL);

                    lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, adecParticipantFullAge, adecSurvivorFullAge);
                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_600_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    break;

                case busConstant.LUMP_SUM:
                    //Ticket - 61531
                    ldecFactor = Math.Round(ibusCalculation.GetLumpsumBenefitFactorByAGE(Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge)) * 12, 3);
                    lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                    lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_600_PLAN_ID, busConstant.LUMP_SUM), ldecFactor,
                                    Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor)),
                                    busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                    this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                    busConstant.LUMP_SUM, busConstant.ZERO_DECIMAL);

                    ldecTotalLumpSum = GetMPIPHPLumpSum() + lbusBenefitCalculationOption.icdoBenefitCalculationOptions.benefit_amount;
                    if (ldecTotalLumpSum < busConstant.BenefitCalculation.LUMP_SUM_LIMIT)
                    {
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_600_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    }
                    break;

                //PIR 894
                case busConstant.LIFE_ANNUTIY:
                    if (ablnConvertBenefitOption)  //PIR 894
                    {
                        lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                        lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_600_PLAN_ID, busConstant.LIFE_ANNUTIY), Decimal.One,
                                                                Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount)),
                                                                busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                busConstant.LIFE_ANNUTIY, busConstant.ZERO_DECIMAL);

                        //PIR 894
                        if (ablnConvertBenefitOption)
                        {
                            if (astrOriginalBenefitOption == busConstant.JOINT_50_PERCENT_POPUP_ANNUITY)
                            {
                                lbusBenefitCalculationOption.icdoBenefitCalculationOptions.pop_up_option_factor
                                    = ibusCalculation.GetRetirementL600Factor(busConstant.JOINT_50_PERCENT_POPUP_ANNUITY, this.ibusPerson.icdoPerson.idtDateofBirth, this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth);

                                lbusBenefitCalculationOption.icdoBenefitCalculationOptions.pop_up_benefit_amount = Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * lbusBenefitCalculationOption.icdoBenefitCalculationOptions.pop_up_option_factor));
                            }
                        }

                        lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.LIFE_ANNUTIY, adecParticipantFullAge, adecSurvivorFullAge);
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_600_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    }
                    break;
            }
            #endregion

            decimal ldecLumpSumFactor = ldecFactor = ibusCalculation.GetLumpsumBenefitFactorByAGE(Convert.ToInt32(adecParticipantFullAge)) * 12;
            ldecLumpSumFactor = Math.Round(ldecLumpSumFactor, 3);
            decimal ldecPresentValue = Math.Round(adecTotalBenefitAmount * ldecLumpSumFactor, 3);
            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_600_PLAN_ID).First().icdoBenefitCalculationDetail.present_value_amount = ldecPresentValue;
        }
        #endregion

        #region - local 666 Correspondence
        public void Calc666BenefitOptionsForCorr(string astrRetirementTypeValue, int aintParticipantAge, ref ArrayList arrOption, ref Collection<busBenefitCalculationDetail> lclbCalcDetail, ref busBenefitCalculationOptions lbus)
        {
            int lintPersonAccountId = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).First().icdoPersonAccount.person_account_id;
            string lstrRetirementTypeValue = astrRetirementTypeValue;
            int lintParticipantAge = aintParticipantAge;
            int lintSurvivorAge = 0;
            //string lstrPrevRetirementType = "";
            //decimal ldecReducedBenefit = 0;
            int lintRetAge = Convert.ToInt32(ibusCalculation.GetNormalRetirementAge(busConstant.LOCAL_666_PLAN_ID));
            DateTime ldtRetirementDate = this.icdoBenefitCalculationHeader.retirement_date;
            int lintYear = this.icdoBenefitCalculationHeader.retirement_date.Year + 1;
            int lintSpouseAge = this.iclbbusBenefitCalculationOptions.LastOrDefault().iintSpouseAge + 1;

            if (lstrRetirementTypeValue == busConstant.RETIREMENT_TYPE_REDUCED_EARLY || lstrRetirementTypeValue == busConstant.RETIREMENT_TYPE_SPL_REDUCED_EARLY)
            {                
                for (int i = lintParticipantAge + 1; i <= lintRetAge; i++)
                {
                    busBenefitCalculationDetail lbusDetail = new busBenefitCalculationDetail { icdoBenefitCalculationDetail = new cdoBenefitCalculationDetail() };
                    lclbCalcDetail = new Collection<busBenefitCalculationDetail>();
                    
                    lbus = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                    if (this.ibusPerson.icdoPerson.date_of_birth.Day == 1)
                    {
                        this.ibusBenefitApplication.icdoBenefitApplication.retirement_date = this.ibusPerson.icdoPerson.idtDateofBirth.AddYears(i);
                    }
                    else
                    {
                        this.ibusBenefitApplication.icdoBenefitApplication.retirement_date = busGlobalFunctions.GetLastDayofMonth(this.ibusPerson.icdoPerson.idtDateofBirth.AddYears(i)).AddDays(1);
                    }

                    lbusDetail.icdoBenefitCalculationDetail.plan_id = busConstant.LOCAL_666_PLAN_ID;
                    lbusDetail.icdoBenefitCalculationDetail.retirement_date = this.ibusBenefitApplication.icdoBenefitApplication.retirement_date;
                    lclbCalcDetail.Add(lbusDetail);

                    lintSurvivorAge = Convert.ToInt32(busGlobalFunctions.CalculatePersonAge(this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth, this.ibusBenefitApplication.icdoBenefitApplication.retirement_date));
                    lbus.iintParticipantAge = i;
                    lbus.iintSpouseAge = lintSurvivorAge;
                    this.ibusBenefitApplication.idecAge = Convert.ToDecimal(i);

                    //rid 82228
                    decimal ldecLocalFrozenBenefit_amount = this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_frozen_benefit_amount);
                    decimal ldecLocalPreBisAmount = this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_pre_bis_amount);
                    decimal ldecLocalPostBisAmount = this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_post_bis_amount);
                    int lintComputationalYear = Convert.ToInt32(this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Max(item => item.icdoPersonAccountRetirementContribution.computational_year));
                    ldtRetirementDate = Convert.ToDateTime(ldtRetirementDate.Month.ToString() + "/" + ldtRetirementDate.Day.ToString() + "/" + lintYear.ToString());

                    decimal ldecTotalBenefitAmount = 0;

                    if (iclbBenefitCalculationDetail[0].icdoBenefitCalculationDetail.early_reduction_factor <= Convert.ToDecimal(1.0))
                    {
                        ldecTotalBenefitAmount = this.ibusCalculation.CalculateTotalBenefitAmtForLocal666(astrRetirementTypeValue, //this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_666).First().icdoPersonAccount.istrRetirementSubType,
                                                                                      ldtRetirementDate, //this.icdoBenefitCalculationHeader.retirement_date,
                                                                                      this.ibusPerson.icdoPerson.idtDateofBirth,
                                                                                      ldecLocalFrozenBenefit_amount, false,
                                                                                      this.ibusBenefitApplication,
                                                                                      ldecLocalPreBisAmount,
                                                                                      ldecLocalPostBisAmount,
                                                                                      Convert.ToDecimal(i), //this.icdoBenefitCalculationHeader.age,
                                                                                      null, this.iclbBenefitCalculationDetail,
                                                                                      lintComputationalYear,
                                                                                      this.iclbPersonAccountRetirementContribution, busConstant.BOOL_FALSE, this.ibusPerson.icdoPerson.person_id);

                    }
                    if (iclbBenefitCalculationDetail[0].icdoBenefitCalculationDetail.early_reduction_factor > Convert.ToDecimal(1.0))
                    {
                        astrRetirementTypeValue = busConstant.RETIREMENT_TYPE_UNREDUCED_EARLY;
                        ldecTotalBenefitAmount = this.ibusCalculation.CalculateTotalBenefitAmtForLocal666(astrRetirementTypeValue, //this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_666).First().icdoPersonAccount.istrRetirementSubType,
                                                                                      ldtRetirementDate, //this.icdoBenefitCalculationHeader.retirement_date,
                                                                                      this.ibusPerson.icdoPerson.idtDateofBirth,
                                                                                      ldecLocalFrozenBenefit_amount, false,
                                                                                      this.ibusBenefitApplication,
                                                                                      ldecLocalPreBisAmount,
                                                                                      ldecLocalPostBisAmount,
                                                                                      Convert.ToDecimal(i), //this.icdoBenefitCalculationHeader.age,
                                                                                      null, this.iclbBenefitCalculationDetail,
                                                                                      lintComputationalYear,
                                                                                      this.iclbPersonAccountRetirementContribution, busConstant.BOOL_FALSE, this.ibusPerson.icdoPerson.person_id);
                    }
                   
                    CalculateLocal666BenefitOptionsForCorr(busConstant.CodeValueAll, ldecTotalBenefitAmount, Convert.ToDecimal(i), lintSpouseAge);

                    foreach (string astrBenOption in arrOption)
                    {                        
                        if (astrBenOption == busConstant.JOINT_50_PERCENT_POPUP_ANNUITY)
                        {                            
                            busBenefitCalculationOptions lobjJoint50 = this.iclbBenefitCalculationDetail.FirstOrDefault().iclbBenefitCalculationOptions.Where(x => x.icdoBenefitCalculationOptions.istrBenefitOptionDescription == "Joint & 50% Pop-up Annuity").FirstOrDefault();
                            lbus.istrJP50 = lobjJoint50.icdoBenefitCalculationOptions.benefit_amount.ToString();
                        }
                        if (astrBenOption == busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY)
                        {                            
                            busBenefitCalculationOptions lobjJoint75 = this.iclbBenefitCalculationDetail.FirstOrDefault().iclbBenefitCalculationOptions.Where(x => x.icdoBenefitCalculationOptions.istrBenefitOptionDescription == "Joint & 75% Survivor Annuity").FirstOrDefault();
                            lbus.istrJS75 = lobjJoint75.icdoBenefitCalculationOptions.benefit_amount.ToString();
                        }
                        if (astrBenOption == busConstant.THREE_YEARS_CERTAIN_AND_LIFE_ANNUITY)
                        {
                            busBenefitCalculationOptions lobjThreeYears = this.iclbBenefitCalculationDetail.FirstOrDefault().iclbBenefitCalculationOptions.Where(x => x.icdoBenefitCalculationOptions.istrBenefitOptionDescription == "Three-Years-Certain and Life Annuity").FirstOrDefault();
                            lbus.istrThreeYear = lobjThreeYears.icdoBenefitCalculationOptions.benefit_amount.ToString();
                        }
                    }

                    if (lintRetAge == i || lbus.istrThreeYear == Convert.ToString(idecAcrdBenefitLocal) || iclbBenefitCalculationDetail[0].icdoBenefitCalculationDetail.early_reduction_factor > Convert.ToDecimal(1.0))
                    {
                        int edf = 1;
                        lbus.istrERFPercentage = edf.ToString("P", CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        lbus.istrERFPercentage = iclbBenefitCalculationDetail[0].icdoBenefitCalculationDetail.early_reduction_factor.ToString("P", CultureInfo.InvariantCulture);
                    }
                    this.iclbbusBenefitCalculationOptions.Add(lbus);
                    lintSpouseAge++;
                    lintYear++;                   
                }
            }
            if (lstrRetirementTypeValue == busConstant.RETIREMENT_TYPE_UNREDUCED_EARLY)
            {
                for (int i = lintParticipantAge + 1; i <= lintRetAge; i++)
                {
                    busBenefitCalculationDetail lbusDetail = new busBenefitCalculationDetail { icdoBenefitCalculationDetail = new cdoBenefitCalculationDetail() };
                    lclbCalcDetail = new Collection<busBenefitCalculationDetail>();
                    
                    lbus = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                    if (this.ibusPerson.icdoPerson.date_of_birth.Day == 1)
                    {
                        this.ibusBenefitApplication.icdoBenefitApplication.retirement_date = this.ibusPerson.icdoPerson.idtDateofBirth.AddYears(i);
                    }
                    else
                    {
                        this.ibusBenefitApplication.icdoBenefitApplication.retirement_date = busGlobalFunctions.GetLastDayofMonth(this.ibusPerson.icdoPerson.idtDateofBirth.AddYears(i)).AddDays(1);
                    }

                    lbusDetail.icdoBenefitCalculationDetail.plan_id = busConstant.LOCAL_666_PLAN_ID;
                    lbusDetail.icdoBenefitCalculationDetail.retirement_date = this.ibusBenefitApplication.icdoBenefitApplication.retirement_date;
                    lclbCalcDetail.Add(lbusDetail);
                    
                    lbus.iintParticipantAge = i;
                    lbus.iintSpouseAge = this.iclbbusBenefitCalculationOptions.LastOrDefault().iintSpouseAge + 1;

                    foreach (string astrBenOption in arrOption)
                    {                       

                        if (astrBenOption == busConstant.JOINT_50_PERCENT_POPUP_ANNUITY)
                        {
                            lbus.istrJP50 = this.iclbbusBenefitCalculationOptions.FirstOrDefault().istrJP50;
                        }
                        if (astrBenOption == busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY)
                        {
                            lbus.istrJS75 = this.iclbbusBenefitCalculationOptions.FirstOrDefault().istrJS75;
                        }
                        if (astrBenOption == busConstant.THREE_YEARS_CERTAIN_AND_LIFE_ANNUITY)
                        {                            
                            lbus.istrThreeYear = this.iclbbusBenefitCalculationOptions.FirstOrDefault().istrThreeYear;
                        }
                    }

                    lbus.istrERFPercentage = this.iclbbusBenefitCalculationOptions.FirstOrDefault().istrERFPercentage;
                    this.iclbbusBenefitCalculationOptions.Add(lbus);
                }
           }
        }

        public void CalculateLocal666BenefitOptionsForCorr(string astrBenefitOption, decimal adecTotalBenefitAmount, decimal adecParticipantFullAge, decimal adecSurvivorFullAge, bool ablnConvertBenefitOption = false, string astrOriginalBenefitOption = "")//PIR 894
        {
            this.iclbBenefitCalculationDetail.FirstOrDefault().iclbBenefitCalculationOptions.Clear();
            #region Variables Required in Switch Case
            decimal ldecFactor = new decimal();
            busBenefitCalculationOptions lbusBenefitCalculationOption;
            #endregion

            #region Switch Case Based on Benefit Options We have to Calculate
            bool lblnCheckIfSpouse = busConstant.BOOL_FALSE;
            lblnCheckIfSpouse = this.ibusCalculation.CheckIfSpouse(this.icdoBenefitCalculationHeader.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id);

            switch (astrBenefitOption)
            {
                case busConstant.CodeValueAll:

                    #region Local 666 LUMP SUM
                    //Ticket - 61531
                    ldecFactor = Math.Round(ibusCalculation.GetLumpsumBenefitFactorByAGE(Convert.ToInt32(adecParticipantFullAge)) * 12, 3);
                    lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                    lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_666_PLAN_ID, busConstant.LUMP_SUM), ldecFactor,
                                                            Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) / 0.5M) * 0.5M),
                                                            busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                            this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                            busConstant.LUMP_SUM, busConstant.ZERO_DECIMAL);

                    decimal ldecTotalLumpSum = GetMPIPHPLumpSum() + lbusBenefitCalculationOption.icdoBenefitCalculationOptions.benefit_amount;
                    if (ldecTotalLumpSum <= busConstant.BenefitCalculation.LUMP_SUM_LIMIT)
                    {
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_666_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    }
                    #endregion

                    if (ldecTotalLumpSum > busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT)
                    {
                        if (lblnCheckIfSpouse)
                        {
                            #region JOINT_50_PERCENT_POPUP_ANNUITY
                            //ldecFactor = Math.Min((Math.Round((this.icdoBenefitCalculationHeader.age - this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement), 0) * Convert.ToDecimal(0.004)) + Convert.ToDecimal(0.89), Convert.ToDecimal(0.99));
                            // PIR - 760
                            ldecFactor = ibusCalculation.GetRetirementL666Factor(busConstant.JOINT_50_PERCENT_POPUP_ANNUITY, this.ibusPerson.icdoPerson.idtDateofBirth, this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth);
                            lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                            this.idecQualifiedJointAndSurvivorAnnuity50 = Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) / 0.5M) * 0.5M);
                            lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_666_PLAN_ID, busConstant.JOINT_50_PERCENT_POPUP_ANNUITY), ldecFactor,
                                            Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) / 0.5M) * 0.5M),
                                            busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                            this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                            busConstant.JOINT_50_PERCENT_POPUP_ANNUITY, Convert.ToDecimal(Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) / 0.5M) * 0.5M) * 0.5M));

                            // No need to set Relative Value for this Benefit option
                            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_666_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                            #endregion

                            #region JOINT_75_PERCENT_SURVIVOR_ANNUITY
                            //ldecFactor = Math.Min((Math.Round((this.icdoBenefitCalculationHeader.age - this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement), 0) * Convert.ToDecimal(0.004)) + Convert.ToDecimal(0.81), Convert.ToDecimal(1));
                            // PIR - 760
                            ldecFactor = ibusCalculation.GetRetirementL666Factor(busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, this.ibusPerson.icdoPerson.idtDateofBirth, this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth);
                            lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                            lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_666_PLAN_ID, busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY), ldecFactor,
                                                                    Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) / 0.5M) * 0.5M),
                                                                    busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                    this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                    busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, Convert.ToDecimal(Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) / 0.5M) * 0.5M) * 0.75M));

                            //if (Math.Round(Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) / 0.5M) * 0.5M) / this.idecQualifiedJointAndSurvivorAnnuity50, 2) == 0.95m)
                            //{
                            //    lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = busConstant.BenefitCalculation.RELATIVE_VALUE_APPROX_EQUAL;
                            //}
                            //else
                            //{
                            //    lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = Math.Round(Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor)) / this.idecQualifiedJointAndSurvivorAnnuity50 * 100, 2).ToString() + " %";
                            //}

                            lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, adecParticipantFullAge, adecSurvivorFullAge);
                            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_666_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                            #endregion
                        }

                        #region THREE_YEARS_CERTAIN_AND_LIFE_ANNUITY
                        lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                        lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_666_PLAN_ID, busConstant.THREE_YEARS_CERTAIN_AND_LIFE_ANNUITY), Decimal.One,
                                                                Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount / 0.5M) * 0.5M),
                                                                busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                busConstant.THREE_YEARS_CERTAIN_AND_LIFE_ANNUITY, busConstant.ZERO_DECIMAL);


                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_666_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                        #endregion
                    }
                    break;

                case busConstant.JOINT_50_PERCENT_POPUP_ANNUITY:
                    if (lblnCheckIfSpouse)
                    {
                        //ldecFactor = Math.Min((Math.Round((this.icdoBenefitCalculationHeader.age - this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement), 0) * Convert.ToDecimal(0.004)) + Convert.ToDecimal(0.89), Convert.ToDecimal(0.99));
                        // PIR - 760
                        ldecFactor = ibusCalculation.GetRetirementL666Factor(busConstant.JOINT_50_PERCENT_POPUP_ANNUITY, this.ibusPerson.icdoPerson.idtDateofBirth, this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth);
                        lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                        lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_666_PLAN_ID, busConstant.JOINT_50_PERCENT_POPUP_ANNUITY), ldecFactor,
                                                                Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) / 0.5M) * 0.5M),
                                                                busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                busConstant.JOINT_50_PERCENT_POPUP_ANNUITY, Convert.ToDecimal(Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) / 0.5M) * 0.5M) * 50 / 100));

                        lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.JOINT_50_PERCENT_POPUP_ANNUITY, adecParticipantFullAge, adecSurvivorFullAge);
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_666_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    }
                    break;

                case busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY:
                    if (lblnCheckIfSpouse)
                    {
                        //ldecFactor = Math.Min((Math.Round((this.icdoBenefitCalculationHeader.age - this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement), 0) * Convert.ToDecimal(0.004)) + Convert.ToDecimal(0.81), Convert.ToDecimal(1));
                        // PIR - 760
                        ldecFactor = ibusCalculation.GetRetirementL666Factor(busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, this.ibusPerson.icdoPerson.idtDateofBirth, this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth);
                        lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                        lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_666_PLAN_ID, busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY), ldecFactor,
                                                                Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) / 0.5M) * 0.5M),
                                                                busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, Convert.ToDecimal(Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) / 0.5M) * 0.5M) * 75 / 100));

                        lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, adecParticipantFullAge, adecSurvivorFullAge);
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_666_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    }
                    break;

                case busConstant.THREE_YEARS_CERTAIN_AND_LIFE_ANNUITY:
                    lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                    lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_666_PLAN_ID, busConstant.THREE_YEARS_CERTAIN_AND_LIFE_ANNUITY), Decimal.One,
                                                            Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount / 0.5M) * 0.5M),
                                                            busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                            this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                            busConstant.THREE_YEARS_CERTAIN_AND_LIFE_ANNUITY, busConstant.ZERO_DECIMAL);

                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_666_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    break;

                case busConstant.LUMP_SUM:
                    //Ticket - 61531
                    ldecFactor = Math.Round(ibusCalculation.GetLumpsumBenefitFactorByAGE(Convert.ToInt32(adecParticipantFullAge)) * 12, 3);
                    lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                    lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_666_PLAN_ID, busConstant.LUMP_SUM), ldecFactor,
                                    Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor)),
                                    busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                    this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                    busConstant.LUMP_SUM, busConstant.ZERO_DECIMAL);

                    ldecTotalLumpSum = GetMPIPHPLumpSum() + lbusBenefitCalculationOption.icdoBenefitCalculationOptions.benefit_amount;
                    if (ldecTotalLumpSum < busConstant.BenefitCalculation.LUMP_SUM_LIMIT)
                    {
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_666_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    }
                    break;

                //PIR 894
                case busConstant.LIFE_ANNUTIY:
                    if (ablnConvertBenefitOption)  //PIR 894
                    {
                        lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                        lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_666_PLAN_ID, busConstant.LIFE_ANNUTIY), Decimal.One,
                                                                Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount)),
                                                                busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                busConstant.LIFE_ANNUTIY, busConstant.ZERO_DECIMAL);

                        //PIR 894
                        if (ablnConvertBenefitOption)
                        {
                            if (astrOriginalBenefitOption == busConstant.JOINT_50_PERCENT_POPUP_ANNUITY)
                            {
                                lbusBenefitCalculationOption.icdoBenefitCalculationOptions.pop_up_option_factor
                                    = ibusCalculation.GetRetirementL666Factor(busConstant.JOINT_50_PERCENT_POPUP_ANNUITY, this.ibusPerson.icdoPerson.idtDateofBirth, this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth);

                                lbusBenefitCalculationOption.icdoBenefitCalculationOptions.pop_up_benefit_amount = Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * lbusBenefitCalculationOption.icdoBenefitCalculationOptions.pop_up_option_factor));
                            }
                        }

                        lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.LIFE_ANNUTIY, adecParticipantFullAge, adecSurvivorFullAge);
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_666_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    }
                    break;
            }
            #endregion
            decimal ldecLumpSumFactor = ldecFactor = ibusCalculation.GetLumpsumBenefitFactorByAGE(Convert.ToInt32(adecParticipantFullAge)) * 12;
            ldecLumpSumFactor = Math.Round(ldecLumpSumFactor, 3);
            decimal ldecPresentValue = Math.Round(adecTotalBenefitAmount * ldecLumpSumFactor, 3);
            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_666_PLAN_ID).First().icdoBenefitCalculationDetail.present_value_amount = ldecPresentValue;

        }
        #endregion

        #region - local 700 Correspondence
        public void Calc700BenefitOptionsForCorr(string astrRetirementTypeValue, int aintParticipantAge, ref ArrayList arrOption, ref Collection<busBenefitCalculationDetail> lclbCalcDetail, ref busBenefitCalculationOptions lbus)
        {
            int lintPersonAccountId = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).First().icdoPersonAccount.person_account_id;
            string lstrRetirementTypeValue = astrRetirementTypeValue;
            int lintParticipantAge = aintParticipantAge;
            int lintRetAge = Convert.ToInt32(ibusCalculation.GetNormalRetirementAge(busConstant.LOCAL_700_PLAN_ID));
            DateTime ldtRetirementDate = this.icdoBenefitCalculationHeader.retirement_date;
            int lintYear = this.icdoBenefitCalculationHeader.retirement_date.Year + 1;
            int lintSpouseAge = this.iclbbusBenefitCalculationOptions.LastOrDefault().iintSpouseAge + 1;

            if (lstrRetirementTypeValue == busConstant.RETIREMENT_TYPE_REDUCED_EARLY || lstrRetirementTypeValue == busConstant.RETIREMENT_TYPE_SPL_REDUCED_EARLY)
            {                
                for (int i = lintParticipantAge + 1; i <= lintRetAge; i++)
                {


                    busBenefitCalculationDetail lbusDetail = new busBenefitCalculationDetail { icdoBenefitCalculationDetail = new cdoBenefitCalculationDetail() };
                    lclbCalcDetail = new Collection<busBenefitCalculationDetail>();

                    lbus = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                    if (this.ibusPerson.icdoPerson.date_of_birth.Day == 1)
                    {
                        this.ibusBenefitApplication.icdoBenefitApplication.retirement_date = this.ibusPerson.icdoPerson.idtDateofBirth.AddYears(i);
                    }
                    else
                    {
                        this.ibusBenefitApplication.icdoBenefitApplication.retirement_date = busGlobalFunctions.GetLastDayofMonth(this.ibusPerson.icdoPerson.idtDateofBirth.AddYears(i)).AddDays(1);
                    }

                    lbusDetail.icdoBenefitCalculationDetail.plan_id = busConstant.LOCAL_700_PLAN_ID;
                    lbusDetail.icdoBenefitCalculationDetail.retirement_date = this.ibusBenefitApplication.icdoBenefitApplication.retirement_date;
                    lclbCalcDetail.Add(lbusDetail);
                    
                    lbus.iintParticipantAge = i;
                    lbus.iintSpouseAge = this.iclbbusBenefitCalculationOptions.LastOrDefault().iintSpouseAge + 1;
                    this.ibusBenefitApplication.idecAge = Convert.ToDecimal(i);

                    //rid 82228
                    string listrRetirementSubType = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(x => x.icdoPersonAccount.istrPlanCode == busConstant.LOCAL_700).First().icdoPersonAccount.istrRetirementSubType;
                    decimal ldecLocalFrozenBenefitAmount = this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_frozen_benefit_amount);
                    decimal ldecLocalPreBisAmount = this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_pre_bis_amount);
                    decimal ldecLocalPostBisAmount = this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_post_bis_amount);
                    int lintcomputational_year = Convert.ToInt32(this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Max(item => item.icdoPersonAccountRetirementContribution.computational_year));
                    ldtRetirementDate = Convert.ToDateTime(ldtRetirementDate.Month.ToString() + "/" + ldtRetirementDate.Day.ToString() + "/" + lintYear.ToString());

                    decimal ldecTotalBenefitAmount = 0;

                    if (iclbBenefitCalculationDetail[0].icdoBenefitCalculationDetail.early_reduction_factor <= Convert.ToDecimal(1.0))
                    {
                        ldecTotalBenefitAmount = this.ibusCalculation.CalculateTotalBenefitAmtForLocal700(astrRetirementTypeValue,//listrRetirementSubType,
                                                this.ibusBenefitApplication.icdoBenefitApplication.retirement_date,//this.icdoBenefitCalculationHeader.retirement_date,
                                                this.ibusPerson.icdoPerson.idtDateofBirth,
                                                ldecLocalFrozenBenefitAmount, false,
                                                this.ibusBenefitApplication,
                                                ldecLocalPreBisAmount,
                                                ldecLocalPostBisAmount, null,
                                                this.iclbBenefitCalculationDetail,
                                                lintcomputational_year,
                                                this.iclbPersonAccountRetirementContribution, busConstant.BOOL_FALSE, this.ibusPerson.icdoPerson.person_id);
                    }
                    string lstrRetirementSubtype =  this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().icdoPersonAccount.istrRetirementSubType;
                    if (iclbBenefitCalculationDetail[0].icdoBenefitCalculationDetail.early_reduction_factor > Convert.ToDecimal(1.0) || lstrRetirementSubtype == busConstant.RETIREMENT_TYPE_UNREDUCED_EARLY)
                    {
                        ldecTotalBenefitAmount = this.ibusCalculation.CalculateTotalBenefitAmtForLocal700(astrRetirementTypeValue,//listrRetirementSubType,
                                                this.ibusBenefitApplication.icdoBenefitApplication.retirement_date,//this.icdoBenefitCalculationHeader.retirement_date,
                                                this.ibusPerson.icdoPerson.idtDateofBirth,
                                                ldecLocalFrozenBenefitAmount, false,
                                                this.ibusBenefitApplication,
                                                ldecLocalPreBisAmount,
                                                ldecLocalPostBisAmount, null,
                                                this.iclbBenefitCalculationDetail,
                                                lintcomputational_year,
                                                this.iclbPersonAccountRetirementContribution, busConstant.BOOL_FALSE, this.ibusPerson.icdoPerson.person_id);
                    }

                    this.icdoBenefitCalculationHeader.idecParticipantFullAge = Convert.ToDecimal(i);

                    CalculateLocal700BenefitOptionsForCorr(busConstant.CodeValueAll, ldecTotalBenefitAmount, Convert.ToDecimal(i), lintSpouseAge);

                    foreach (string astrBenOption in arrOption)
                    {                        
                        if (astrBenOption == busConstant.LIFE_ANNUTIY)
                        {                            
                            busBenefitCalculationOptions lobjAnnuity = this.iclbBenefitCalculationDetail.FirstOrDefault().iclbBenefitCalculationOptions.Where(x => x.icdoBenefitCalculationOptions.istrBenefitOptionDescription == "Life Annuity" && x.icdoBenefitCalculationOptions.created_by == null).FirstOrDefault();
                            lbus.istrLife = lobjAnnuity.icdoBenefitCalculationOptions.benefit_amount.ToString();
                        }
                        if (astrBenOption == busConstant.TWO_YEARS_CERTAIN_AND_LIFE_ANNUITY)
                        {                            
                            busBenefitCalculationOptions lobjTwoYears = this.iclbBenefitCalculationDetail.FirstOrDefault().iclbBenefitCalculationOptions.Where(x => x.icdoBenefitCalculationOptions.istrBenefitOptionDescription == "Two-Years-Certain and Life Annuity" && x.icdoBenefitCalculationOptions.created_by == null).FirstOrDefault();
                            lbus.istrTwoYear = lobjTwoYears.icdoBenefitCalculationOptions.benefit_amount.ToString();
                        }
                        if (astrBenOption == busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY)
                        {                           
                            busBenefitCalculationOptions lobjJoint50 = this.iclbBenefitCalculationDetail.FirstOrDefault().iclbBenefitCalculationOptions.Where(x => x.icdoBenefitCalculationOptions.istrBenefitOptionDescription == "Qualified Joint & 50% Survivor Annuity" && x.icdoBenefitCalculationOptions.created_by == null).FirstOrDefault();
                            lbus.istrJS50 = lobjJoint50.icdoBenefitCalculationOptions.benefit_amount.ToString();
                        }
                        if (astrBenOption == busConstant.JOINT_66_2by3_PERCENT_SURVIVOR_ANNUITY)
                        {                            
                            busBenefitCalculationOptions lobjJoint66 = this.iclbBenefitCalculationDetail.FirstOrDefault().iclbBenefitCalculationOptions.Where(x => x.icdoBenefitCalculationOptions.istrBenefitOptionDescription.Contains("Joint & 66") && x.icdoBenefitCalculationOptions.created_by == null).FirstOrDefault();
                            lbus.istrJS66 = lobjJoint66.icdoBenefitCalculationOptions.benefit_amount.ToString();
                        }
                        if (astrBenOption == busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY)
                        {
                            busBenefitCalculationOptions lobjJoint75 = this.iclbBenefitCalculationDetail.FirstOrDefault().iclbBenefitCalculationOptions.Where(x => x.icdoBenefitCalculationOptions.istrBenefitOptionDescription == "Joint & 75% Survivor Annuity" && x.icdoBenefitCalculationOptions.created_by == null).FirstOrDefault();
                            lbus.istrJS75 = lobjJoint75.icdoBenefitCalculationOptions.benefit_amount.ToString();
                        }
                        if (astrBenOption == busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY)
                        {
                            busBenefitCalculationOptions lobjJoint100 = this.iclbBenefitCalculationDetail.FirstOrDefault().iclbBenefitCalculationOptions.Where(x => x.icdoBenefitCalculationOptions.istrBenefitOptionDescription == "Joint & 100% Survivor Annuity" && x.icdoBenefitCalculationOptions.created_by == null).FirstOrDefault();
                            lbus.istrJS100 = lobjJoint100.icdoBenefitCalculationOptions.benefit_amount.ToString();
                        }
                        if (astrBenOption == busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY)
                        {
                            busBenefitCalculationOptions lobjTenYears = this.iclbBenefitCalculationDetail.FirstOrDefault().iclbBenefitCalculationOptions.Where(x => x.icdoBenefitCalculationOptions.istrBenefitOptionDescription == "Ten-Years-Certain and Life Annuity" && x.icdoBenefitCalculationOptions.created_by == null).FirstOrDefault();
                            lbus.istrTenYear = lobjTenYears.icdoBenefitCalculationOptions.benefit_amount.ToString();
                        }
                    }


                    if (lintRetAge == i || lbus.istrTenYear == Convert.ToString(idecAcrdBenefitLocal) || iclbBenefitCalculationDetail[0].icdoBenefitCalculationDetail.early_reduction_factor > Convert.ToDecimal(1.0))
                    {
                        int edf = 1;
                        lbus.istrERFPercentage = edf.ToString("P", CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        lbus.istrERFPercentage = iclbBenefitCalculationDetail[0].icdoBenefitCalculationDetail.early_reduction_factor.ToString("P", CultureInfo.InvariantCulture);
                    }
                    this.iclbbusBenefitCalculationOptions.Add(lbus);
                    lintSpouseAge++;
                    lintYear++;
                }
            }
            if (lstrRetirementTypeValue == busConstant.RETIREMENT_TYPE_UNREDUCED_EARLY)
            {
                for (int i = lintParticipantAge + 1; i <= lintRetAge; i++)
                {
                    busBenefitCalculationDetail lbusDetail = new busBenefitCalculationDetail { icdoBenefitCalculationDetail = new cdoBenefitCalculationDetail() };
                    lclbCalcDetail = new Collection<busBenefitCalculationDetail>();
                    
                    lbus = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                    if (this.ibusPerson.icdoPerson.date_of_birth.Day == 1)
                    {
                        this.ibusBenefitApplication.icdoBenefitApplication.retirement_date = this.ibusPerson.icdoPerson.idtDateofBirth.AddYears(i);
                    }
                    else
                    {
                        this.ibusBenefitApplication.icdoBenefitApplication.retirement_date = busGlobalFunctions.GetLastDayofMonth(this.ibusPerson.icdoPerson.idtDateofBirth.AddYears(i)).AddDays(1);
                    }

                    lbusDetail.icdoBenefitCalculationDetail.plan_id = busConstant.LOCAL_700_PLAN_ID;
                    lbusDetail.icdoBenefitCalculationDetail.retirement_date = this.ibusBenefitApplication.icdoBenefitApplication.retirement_date;
                    lclbCalcDetail.Add(lbusDetail);
                    
                    lbus.iintParticipantAge = i;
                    lbus.iintSpouseAge = this.iclbbusBenefitCalculationOptions.LastOrDefault().iintSpouseAge + 1;

                    foreach (string astrBenOption in arrOption)
                    {                      
                        if (astrBenOption == busConstant.LIFE_ANNUTIY)
                        {                            
                            lbus.istrLife = this.iclbbusBenefitCalculationOptions.FirstOrDefault().istrLife;
                        }
                        if (astrBenOption == busConstant.TWO_YEARS_CERTAIN_AND_LIFE_ANNUITY)
                        {                            
                            lbus.istrTwoYear = this.iclbbusBenefitCalculationOptions.FirstOrDefault().istrTwoYear;
                        }
                        if (astrBenOption == busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY)
                        {                            
                            lbus.istrJS50 = this.iclbbusBenefitCalculationOptions.FirstOrDefault().istrJS50;
                        }
                        if (astrBenOption == busConstant.JOINT_66_2by3_PERCENT_SURVIVOR_ANNUITY)
                        {
                            lbus.istrJS66 = this.iclbbusBenefitCalculationOptions.FirstOrDefault().istrJS66;
                        }
                        if (astrBenOption == busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY)
                        {
                            lbus.istrJS75 = this.iclbbusBenefitCalculationOptions.FirstOrDefault().istrJS75;
                        }
                        if (astrBenOption == busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY)
                        {
                            lbus.istrJS100 = this.iclbbusBenefitCalculationOptions.FirstOrDefault().istrJS100;
                        }
                        if (astrBenOption == busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY)
                        {
                            lbus.istrTenYear = this.iclbbusBenefitCalculationOptions.FirstOrDefault().istrTenYear;
                        }
                    }
                    lbus.istrERFPercentage = this.iclbbusBenefitCalculationOptions.FirstOrDefault().istrERFPercentage;
                    this.iclbbusBenefitCalculationOptions.Add(lbus);
                }
            }
        }

        public void CalculateLocal700BenefitOptionsForCorr(string astrBenefitOption, decimal adecTotalBenefitAmount, decimal adecParticipantFullAge, decimal adecSurvivorFullAge)
        {
            this.iclbBenefitCalculationDetail.FirstOrDefault().iclbBenefitCalculationOptions.Clear();
            #region Variables Required in Switch Case
            decimal ldecFactor = new decimal();
            busBenefitCalculationOptions lbusBenefitCalculationOption;
            #endregion

            #region Switch Case Based on Benefit Options We have to Calculate

            bool lblnCheckIfSpouse = busConstant.BOOL_FALSE;
            lblnCheckIfSpouse = this.ibusCalculation.CheckIfSpouse(this.icdoBenefitCalculationHeader.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id);

            switch (astrBenefitOption)
            {
                case busConstant.CodeValueAll:

                    #region Local 700 LUMP SUM
                    //Ticket - 61531
                    ldecFactor = Math.Round(ibusCalculation.GetLumpsumBenefitFactorByAGE(Convert.ToInt32(adecParticipantFullAge)) * 12, 3);
                    lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                    lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_700_PLAN_ID, busConstant.LUMP_SUM), ldecFactor,
                                                            Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) / 0.5M) * 0.5M),
                                                            busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                            this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                            busConstant.LUMP_SUM, busConstant.ZERO_DECIMAL);

                    decimal ldecTotalLumpSum = GetMPIPHPLumpSum() + lbusBenefitCalculationOption.icdoBenefitCalculationOptions.benefit_amount;
                    if (ldecTotalLumpSum <= busConstant.BenefitCalculation.LUMP_SUM_LIMIT)
                    {
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_700_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    }
                    #endregion

                    if (ldecTotalLumpSum > busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT)
                    {
                        if (lblnCheckIfSpouse)
                        {
                            #region JOINT_50_PERCENT_SURVIVOR_ANNUITY

                            ldecFactor = GetBenefitFactorLocal(busConstant.LOCAL_700, busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY, busConstant.BENEFIT_TYPE_RETIREMENT, adecParticipantFullAge, adecSurvivorFullAge);
                            lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                            this.idecQualifiedJointAndSurvivorAnnuity50 = Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) / 0.5M) * 0.5M);
                            lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_700_PLAN_ID, busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY), ldecFactor,
                                                                   Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) / 0.5M) * 0.5M),
                                                                   busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                   this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY,
                                                                   Convert.ToDecimal(Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) / 0.5M) * 0.5M) * 50 / 100));

                            // No Need to show the Relative Value for JOINT_50_PERCENT_SURVIVOR_ANNUITY Benefit Option
                            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_700_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);

                            #endregion

                            #region JOINT_100_PERCENT_SURVIVOR_ANNUITY
                            ldecFactor = GetBenefitFactorLocal(busConstant.LOCAL_700, busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY, busConstant.BENEFIT_TYPE_RETIREMENT, adecParticipantFullAge, adecSurvivorFullAge);
                            lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                            lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_700_PLAN_ID, busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY), ldecFactor,
                                                                    Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) / 0.5M) * 0.5M),
                                                                    busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                    this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                    busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY,
                                                                    Convert.ToDecimal(Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) / 0.5M) * 0.5M) * 100 / 100));

                            
                            lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY, adecParticipantFullAge, adecSurvivorFullAge);
                            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_700_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                            #endregion

                            #region JOINT_66_2by3_PERCENT_SURVIVOR_ANNUITY
                            ldecFactor = GetBenefitFactorLocal(busConstant.LOCAL_700, busConstant.JOINT_66_2by3_PERCENT_SURVIVOR_ANNUITY, busConstant.BENEFIT_TYPE_RETIREMENT, adecParticipantFullAge, adecSurvivorFullAge);
                            lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                            lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_700_PLAN_ID, busConstant.JOINT_66_2by3_PERCENT_SURVIVOR_ANNUITY), ldecFactor,
                                                                    Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) / 0.5M) * 0.5M),
                                                                    busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                    this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                    busConstant.JOINT_66_2by3_PERCENT_SURVIVOR_ANNUITY, Convert.ToDecimal(Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) / 0.5M) * 0.5M) * 44 / 100));

                            lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.JOINT_66_2by3_PERCENT_SURVIVOR_ANNUITY, adecParticipantFullAge, adecSurvivorFullAge);
                            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_700_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                            #endregion

                            #region JOINT_75_PERCENT_SURVIVOR_ANNUITY
                            ldecFactor = GetBenefitFactorLocal(busConstant.LOCAL_700, busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, busConstant.BENEFIT_TYPE_RETIREMENT, adecParticipantFullAge, this.icdoBenefitCalculationHeader.idecSurvivorFullAge);
                            lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                            lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_700_PLAN_ID, busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY), ldecFactor,
                                                                    Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) / 0.5M) * 0.5M),
                                                                    busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                    this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                    busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, Convert.ToDecimal(Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) / 0.5M) * 0.5M) * 75 / 100));

                            lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, adecParticipantFullAge, adecSurvivorFullAge);
                            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_700_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                            #endregion
                        }

                        #region TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY
                        ldecFactor = GetBenefitFactorLocal(busConstant.LOCAL_700, busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, busConstant.BENEFIT_TYPE_RETIREMENT, adecParticipantFullAge);
                        lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                        lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_700_PLAN_ID, busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY), ldecFactor,
                                                                Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) / 0.5M) * 0.5M),
                                                                busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, busConstant.ZERO_DECIMAL);

                        lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, adecParticipantFullAge, adecSurvivorFullAge);
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_700_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                        #endregion

                        #region TWO_YEARS_CERTAIN_AND_LIFE_ANNUITY
                        ldecFactor = GetBenefitFactorLocal(busConstant.LOCAL_700, busConstant.TWO_YEARS_CERTAIN_AND_LIFE_ANNUITY, busConstant.BENEFIT_TYPE_RETIREMENT, adecParticipantFullAge);
                        lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                        lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_700_PLAN_ID, busConstant.TWO_YEARS_CERTAIN_AND_LIFE_ANNUITY), ldecFactor,
                                                                Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount / ldecFactor) / 0.5M) * 0.5M),
                                                                busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                busConstant.TWO_YEARS_CERTAIN_AND_LIFE_ANNUITY, busConstant.ZERO_DECIMAL);


                        //lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.TWO_YEARS_CERTAIN_AND_LIFE_ANNUITY, this.icdoBenefitCalculationHeader.age, this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement);
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_700_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                        #endregion

                        #region LIFE_ANNUTIY
                        lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                        lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_700_PLAN_ID, busConstant.LIFE_ANNUTIY), Decimal.One,
                                                                Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * Decimal.One) / 0.5M) * 0.5M),
                                                                busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                busConstant.LIFE_ANNUTIY, busConstant.ZERO_DECIMAL);

                        lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.LIFE_ANNUTIY, adecParticipantFullAge, adecSurvivorFullAge);
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_700_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                        #endregion
                    }
                    break;
                case busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY:
                    if (lblnCheckIfSpouse)
                    {
                        ldecFactor = GetBenefitFactorLocal(busConstant.LOCAL_700, busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY, busConstant.BENEFIT_TYPE_RETIREMENT, adecParticipantFullAge, adecSurvivorFullAge);
                        lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                        lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_700_PLAN_ID, busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY), ldecFactor,
                                                                Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) / 0.5M) * 0.5M),
                                                                busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY, Convert.ToDecimal(Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) / 0.5M) * 0.5M) * 50 / 100));

                        // No Need to show the Relative Value for JOINT_50_PERCENT_SURVIVOR_ANNUITY Benefit Option
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_700_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    }
                    break;

                case busConstant.JOINT_66_2by3_PERCENT_SURVIVOR_ANNUITY:
                    if (lblnCheckIfSpouse)
                    {
                        ldecFactor = GetBenefitFactorLocal(busConstant.LOCAL_700, busConstant.JOINT_66_2by3_PERCENT_SURVIVOR_ANNUITY, busConstant.BENEFIT_TYPE_RETIREMENT, adecParticipantFullAge, adecSurvivorFullAge);
                        lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                        lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_700_PLAN_ID, busConstant.JOINT_66_2by3_PERCENT_SURVIVOR_ANNUITY), ldecFactor,
                                                                Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) / 0.5M) * 0.5M),
                                                                busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                busConstant.JOINT_66_2by3_PERCENT_SURVIVOR_ANNUITY, Convert.ToDecimal(Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) / 0.5M) * 0.5M) * 44 / 100));

                        lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.JOINT_66_2by3_PERCENT_SURVIVOR_ANNUITY, adecParticipantFullAge, adecSurvivorFullAge);
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_700_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    }
                    break;

                case busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY:
                    if (lblnCheckIfSpouse)
                    {
                        ldecFactor = GetBenefitFactorLocal(busConstant.LOCAL_700, busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, busConstant.BENEFIT_TYPE_RETIREMENT, adecParticipantFullAge, adecSurvivorFullAge);
                        lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                        lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_700_PLAN_ID, busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY), ldecFactor,
                                                                Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) / 0.5M) * 0.5M),
                                                                busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, Convert.ToDecimal(Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) / 0.5M) * 0.5M) * 75 / 100));

                        lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, adecParticipantFullAge, adecSurvivorFullAge);
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_700_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    }
                    break;

                case busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY:
                    if (lblnCheckIfSpouse)
                    {
                        ldecFactor = GetBenefitFactorLocal(busConstant.LOCAL_700, busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY, busConstant.BENEFIT_TYPE_RETIREMENT, adecParticipantFullAge, adecSurvivorFullAge);
                        lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                        lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_700_PLAN_ID, busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY), ldecFactor,
                                        Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) / 0.5M) * 0.5M),
                                        busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                        this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                        busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY, Convert.ToDecimal(Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) / 0.5M) * 0.5M) * 100 / 100));

                        lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY, adecParticipantFullAge, adecSurvivorFullAge);
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_700_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    }
                    break;

                case busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY:
                    ldecFactor = GetBenefitFactorLocal(busConstant.LOCAL_700, busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, busConstant.BENEFIT_TYPE_RETIREMENT, adecParticipantFullAge);
                    lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                    lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_700_PLAN_ID, busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY), ldecFactor,
                                                            Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) / 0.5M) * 0.5M),
                                                            busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                            this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                            busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, busConstant.ZERO_DECIMAL);

                    lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, adecParticipantFullAge, adecSurvivorFullAge);
                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_700_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    break;

                case busConstant.TWO_YEARS_CERTAIN_AND_LIFE_ANNUITY:
                    ldecFactor = GetBenefitFactorLocal(busConstant.LOCAL_700, busConstant.TWO_YEARS_CERTAIN_AND_LIFE_ANNUITY, busConstant.BENEFIT_TYPE_RETIREMENT, adecParticipantFullAge);
                    lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                    lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_700_PLAN_ID, busConstant.TWO_YEARS_CERTAIN_AND_LIFE_ANNUITY), ldecFactor,
                                    Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount / ldecFactor) / 0.5M) * 0.5M),  //PIR 833 should be divide instead of multiply by factor
                                    busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                    this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                    busConstant.TWO_YEARS_CERTAIN_AND_LIFE_ANNUITY, busConstant.ZERO_DECIMAL);

                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_700_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    break;

                case busConstant.LIFE_ANNUTIY:
                    lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                    lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_700_PLAN_ID, busConstant.LIFE_ANNUTIY), Decimal.One,
                                    Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * Decimal.One) / 0.5M) * 0.5M),
                                    busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                    this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                    busConstant.LIFE_ANNUTIY, busConstant.ZERO_DECIMAL);

                    lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.LIFE_ANNUTIY, adecParticipantFullAge, adecSurvivorFullAge);
                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_700_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    break;

                case busConstant.LUMP_SUM:
                    //Ticket - 61531
                    ldecFactor = Math.Round(ibusCalculation.GetLumpsumBenefitFactorByAGE(Convert.ToInt32(adecParticipantFullAge)) * 12, 3);
                    lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                    lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_700_PLAN_ID, busConstant.LUMP_SUM), ldecFactor,
                                    Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor)),
                                    busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                    this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                    busConstant.LUMP_SUM, busConstant.ZERO_DECIMAL);

                    ldecTotalLumpSum = GetMPIPHPLumpSum() + lbusBenefitCalculationOption.icdoBenefitCalculationOptions.benefit_amount;
                    if (ldecTotalLumpSum < busConstant.BenefitCalculation.LUMP_SUM_LIMIT)
                    {
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_700_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    }
                    break;
            }
            #endregion

            decimal ldecLumpSumFactor = ldecFactor = ibusCalculation.GetLumpsumBenefitFactorByAGE(Convert.ToInt32(adecParticipantFullAge)) * 12;
            ldecLumpSumFactor = Math.Round(ldecLumpSumFactor, 3);
            decimal ldecPresentValue = Math.Round(adecTotalBenefitAmount * ldecLumpSumFactor, 3);
            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_700_PLAN_ID).First().icdoBenefitCalculationDetail.present_value_amount = ldecPresentValue;

        }
        #endregion


        public bool IsPopUpToLifeConversion()
        {
            return icdoBenefitCalculationHeader.iblnPopUpToLife;
        }
        
    }
}
