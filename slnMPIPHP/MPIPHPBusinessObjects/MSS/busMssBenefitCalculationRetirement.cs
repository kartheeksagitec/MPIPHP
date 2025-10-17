
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
    public class busMssBenefitCalculationRetirement : busBenefitCalculationRetirement
    {
        public busMssBenefitCalculationRetirement()
        {

        }

        public Collection<cdoPlan> GetPlanValues()
        {
            Collection<cdoPlan> lColPlans = new Collection<cdoPlan>();
            DataTable ldtplan = busBase.Select("cdoMssBenefitCalculationHeader.GetActivePlansForRetCalculation", new object[1] { this.icdoBenefitCalculationHeader.person_id });
            if (ldtplan.Rows.Count > 0)
            {
                lColPlans = doBase.LoadData<cdoPlan>(ldtplan);
            }

            return lColPlans;
        }

        #region Public Methods

        //FM upgrade: 6.0.2.1 changes - method signature changed - added 4th param
        public override void BeforeWizardStepValidate(utlPageMode aenmPageMode, string astrWizardName, string astrWizardStepName, utlWizardNavigationEventArgs we = null)
        {
            this.EvaluateInitialLoadRules();
            if (astrWizardStepName == "wzsStep2")
            {
                BeforeValidate(utlPageMode.All);
                busPlan lbusPlan = new busPlan{icdoPlan = new cdoPlan()};
                lbusPlan.FindPlan(this.icdoBenefitCalculationHeader.iintPlanId);
                this.icdoBenefitCalculationHeader.istrPlanDescription = lbusPlan.icdoPlan.plan_name;
            }
            base.BeforeWizardStepValidate(aenmPageMode, astrWizardName, astrWizardStepName);
            
        }

        public override void ValidateGroupRules(string astrGroupName, utlPageMode aenmPageMode)
        {
            aenmPageMode = utlPageMode.New;
            base.ValidateGroupRules(astrGroupName, aenmPageMode);
            switch (astrGroupName)
            {
                case "Step1":
                    break;
                case "Step2":
                    ValidateStep2Details();
                    break;
                case "Step3":
                    break;
            }
        }

        public override void BeforePersistChanges()
        {
            
            Setup_MSS_Retirement_Calculations();
        }

        public void Setup_MSS_Retirement_Calculations()
        {
            DateTime ldtVestedDateMpi = DateTime.MinValue;
            DateTime ldtVestedDateIAP = DateTime.MinValue;
            {
                #region SETUP BENEFIT CALCULATION DETAIL BASED ON WHAT ESTIMATE HAS BEEN ASKED FOR

                if (this.iclbBenefitCalculationDetail == null)
                {
                    this.iclbBenefitCalculationDetail = new Collection<busBenefitCalculationDetail>();
                }

                if (this.icdoBenefitCalculationHeader.iintPlanId == busConstant.MPIPP_PLAN_ID)
                {
                    #region Setup Detail Records for MPIs Estimate

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
                    {
                        switch (this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).First().icdoPersonAccount.istrPlanCode)
                        {

                            case busConstant.Local_161:
                                if (this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Count() > 0)
                                {
                                    ldecTotalBenefitAmount = this.ibusCalculation.CalculateTotalBenefitAmtForLocal161(this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_161).First().icdoPersonAccount.istrRetirementSubType,
                                                                                             this.icdoBenefitCalculationHeader.retirement_date, this.ibusPerson.icdoPerson.idtDateofBirth,
                                                                                             this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_frozen_benefit_amount),
                                                                                             false, this.ibusBenefitApplication, this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_pre_bis_amount),
                                                                                             this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_post_bis_amount),
                                                                                             null, this.iclbBenefitCalculationDetail,
                                                                                             Convert.ToInt32(this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Max(item => item.icdoPersonAccountRetirementContribution.computational_year)),
                                                                                             this.iclbPersonAccountRetirementContribution, busConstant.BOOL_FALSE, this.ibusPerson.icdoPerson.person_id);

                                    this.iclbBenefitCalculationDetail.FirstOrDefault().icdoBenefitCalculationDetail.early_reduced_benefit_amount = ldecTotalBenefitAmount;
                                }
                                CalculateMssLocal161BenefitOptions(busConstant.CodeValueAll, ldecTotalBenefitAmount);

                                break;
                            case busConstant.Local_52:
                                if (this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Count() > 0)
                                {
                                    ldecTotalBenefitAmount = this.ibusCalculation.CalculateTotalBenefitAmtForLocal52(this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_52).First().icdoPersonAccount.istrRetirementSubType,
                                                                 this.icdoBenefitCalculationHeader.retirement_date, this.ibusPerson.icdoPerson.idtDateofBirth,
                                                                 this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_frozen_benefit_amount),
                                                                 false, this.ibusBenefitApplication, this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_pre_bis_amount),
                                                                 this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_post_bis_amount),
                                                                 null, this.iclbBenefitCalculationDetail,
                                                                 Convert.ToInt32(this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Max(item => item.icdoPersonAccountRetirementContribution.computational_year)),
                                                                 this.iclbPersonAccountRetirementContribution, busConstant.BOOL_FALSE, this.ibusPerson.icdoPerson.person_id);

                                    this.iclbBenefitCalculationDetail.FirstOrDefault().icdoBenefitCalculationDetail.early_reduced_benefit_amount = ldecTotalBenefitAmount;
                                }
                                CalculateMssLocal52BenefitOptions(busConstant.CodeValueAll, ldecTotalBenefitAmount);

                                break;

                            case busConstant.Local_600:
                                if (this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Count() > 0)
                                {
                                    ldecTotalBenefitAmount = this.ibusCalculation.CalculateTotalBenefitAmtForLocal600(this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_600).First().icdoPersonAccount.istrRetirementSubType,
                                                                                                                      this.icdoBenefitCalculationHeader.retirement_date, this.ibusPerson.icdoPerson.idtDateofBirth,
                                                                                                                      this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_frozen_benefit_amount),
                                                                                                                       false, this.ibusBenefitApplication, this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_pre_bis_amount),
                                                                                                                       this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_post_bis_amount),
                                                                                                                       null, this.iclbBenefitCalculationDetail,
                                                                                                                       Convert.ToInt32(this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Max(item => item.icdoPersonAccountRetirementContribution.computational_year)),
                                                                                                                       this.iclbPersonAccountRetirementContribution, busConstant.BOOL_FALSE, this.ibusPerson.icdoPerson.person_id);
                                    this.iclbBenefitCalculationDetail.FirstOrDefault().icdoBenefitCalculationDetail.early_reduced_benefit_amount = ldecTotalBenefitAmount;

                                }
                                CalculateMssLocal600BenefitOptions(busConstant.CodeValueAll, ldecTotalBenefitAmount);
                                break;

                            case busConstant.Local_666:
                                if (this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Count() > 0)
                                {
                                    ldecTotalBenefitAmount = this.ibusCalculation.CalculateTotalBenefitAmtForLocal666(this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_666).First().icdoPersonAccount.istrRetirementSubType,
                                                                                          this.icdoBenefitCalculationHeader.retirement_date, this.ibusPerson.icdoPerson.idtDateofBirth,
                                                                                          this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_frozen_benefit_amount),
                                                                                           false, this.ibusBenefitApplication, this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_pre_bis_amount),
                                                                                           this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_post_bis_amount), this.icdoBenefitCalculationHeader.age,
                                                                                           null, this.iclbBenefitCalculationDetail,
                                                                                           Convert.ToInt32(this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Max(item => item.icdoPersonAccountRetirementContribution.computational_year)),
                                                                                           this.iclbPersonAccountRetirementContribution, busConstant.BOOL_FALSE, this.ibusPerson.icdoPerson.person_id);
                                    this.iclbBenefitCalculationDetail.FirstOrDefault().icdoBenefitCalculationDetail.early_reduced_benefit_amount = ldecTotalBenefitAmount;

                                }
                                CalculateMssLocal666BenefitOptions(busConstant.CodeValueAll, ldecTotalBenefitAmount);
                                break;

                            case busConstant.LOCAL_700:
                                if (this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Count() > 0)
                                {
                                    ldecTotalBenefitAmount = this.ibusCalculation.CalculateTotalBenefitAmtForLocal700(this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.LOCAL_700).First().icdoPersonAccount.istrRetirementSubType,
                                                              this.icdoBenefitCalculationHeader.retirement_date, this.ibusPerson.icdoPerson.idtDateofBirth,
                                                              this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_frozen_benefit_amount),
                                                               false, this.ibusBenefitApplication, this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_pre_bis_amount),
                                                               this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_post_bis_amount),
                                                               null, this.iclbBenefitCalculationDetail,
                                                               Convert.ToInt32(this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Max(item => item.icdoPersonAccountRetirementContribution.computational_year)),
                                                               this.iclbPersonAccountRetirementContribution, busConstant.BOOL_FALSE, this.ibusPerson.icdoPerson.person_id);

                                    this.iclbBenefitCalculationDetail.FirstOrDefault().icdoBenefitCalculationDetail.early_reduced_benefit_amount = ldecTotalBenefitAmount;

                                }
                                CalculateMssLocal700BenefitOptions(busConstant.CodeValueAll, ldecTotalBenefitAmount);
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
                                             busConstant.BENEFIT_TYPE_RETIREMENT, 
                                             icdoBenefitCalculationHeader.age, icdoBenefitCalculationHeader.retirement_date, ldtVestedDateMpi,
                                             lbusPersonAccount,
                                             this.ibusBenefitApplication, false, iclbBenefitCalculationDetail, this.iclbPersonAccountRetirementContribution, null, true, lbusPersonAccount.icdoPersonAccount.istrRetirementSubType, ref ldecLateAdjustmentAmt, this.ibusPerson.icdoPerson.person_id);



                                if (ldecBenefitAmtForPension != 0)
                                {
                                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).
                                                               FirstOrDefault().icdoBenefitCalculationDetail.early_reduced_benefit_amount = ldecBenefitAmtForPension;



                                    if (this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.MPIPP).First().icdoPersonAccount.istrRetirementSubType == busConstant.RETIREMENT_TYPE_LATE)
                                    {
                                        ldecFinalAccruedBenefitAmount = CalculateMSSFinalBenefitForPensionBenefitOptions(ldecLateAdjustmentAmt, busConstant.CodeValueAll, busConstant.MPIPP_PLAN_ID);
                                    }
                                    else
                                    {
                                        ldecFinalAccruedBenefitAmount = CalculateMSSFinalBenefitForPensionBenefitOptions(ldecBenefitAmtForPension, busConstant.CodeValueAll, busConstant.MPIPP_PLAN_ID);
                                    }

                                    if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL)
                                    {
                                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id ==
                                            this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().icdoBenefitCalculationDetail.final_monthly_benefit_amount = ldecFinalAccruedBenefitAmount;
                                    }


                                    #region UVHP calculation in ESTIMATE 
                                    //Not Required in MSS 
                                    //If Needed Add it here
                                    #endregion

                                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().icdoBenefitCalculationDetail.ee_derived_benefit_amount = ibusCalculation.CalculateEEDerivedBenefitAsOfRetirementDate(this, null);
                                }

                            }

                            if (this.ibusBenefitApplication.aclbPersonWorkHistory_IAP != null && this.ibusBenefitApplication.aclbPersonWorkHistory_IAP.Count > 0)
                            {
                                this.CalculateMssIAPBenefitAmount(busConstant.CodeValueAll);
                            }
                            break;

                        case busConstant.IAP:
                            if (this.ibusBenefitApplication.aclbPersonWorkHistory_IAP != null && this.ibusBenefitApplication.aclbPersonWorkHistory_IAP.Count > 0)
                            {
                                this.CalculateMssIAPBenefitAmount(busConstant.CodeValueAll);
                            }
                            break;
                    }
                }
                #endregion
            }
        }

        public void ValidateStep2Details()
        {
            if (this.iarrErrors.IsNull())
                this.iarrErrors = new ArrayList();
            utlError lobjError = null;
            //FM upgrade: 6.0.0.37 changes - return type is changed from DataTable to the class utlMessageInfo
            //DataTable ldtbMessageInfo;
            utlMessageInfo lobjutlMessageInfo;
            if (!CheckIfRetirementDateIsValid())
            {
                //ldtbMessageInfo = iobjPassInfo.isrvDBCache.GetMessageInfo(5088);
                //string lstrMessage = ldtbMessageInfo.Rows[0]["display_message"].ToString();
                lobjutlMessageInfo = iobjPassInfo.isrvDBCache.GetMessageInfo(5088);
                string lstrMessage = lobjutlMessageInfo.display_message;
                lobjError = AddError(0, lstrMessage);
                this.iarrErrors.Add(lobjError);
                return;
            }

            if (!CheckIfParticipantIsVested())
            {
                //ldtbMessageInfo = iobjPassInfo.isrvDBCache.GetMessageInfo(5458);
                //string lstrMessage = ldtbMessageInfo.Rows[0]["display_message"].ToString();
                lobjutlMessageInfo = iobjPassInfo.isrvDBCache.GetMessageInfo(5458);
                string lstrMessage = lobjutlMessageInfo.display_message;
                lobjError = AddError(0, lstrMessage);
                this.iarrErrors.Add(lobjError);
                return;
            }
            if (this.icdoBenefitCalculationHeader.istrRetirementType.IsNullOrEmpty())
            {
                lobjError = new utlError();
                lobjError.istrErrorMessage = "NOT Eligible for Retirement";
                this.iarrErrors.Add(lobjError);
                return;
            }
            if (this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth >= DateTime.Now)
            {
                lobjError = new utlError();
                lobjError.istrErrorMessage = "Spouse Date of Birth cannot be a future date.";
                this.iarrErrors.Add(lobjError);
                return;
            }
            if (!CheckIfRetirementDateIsAfterTwoMonths())
            {
                lobjError = new utlError();
                lobjError.istrErrorMessage = "Retirement Date needs to be after two complete calendar months.";
                this.iarrErrors.Add(lobjError);
                return;
            }


        }
        
        public bool CheckIfRetirementDateIsValid()
        {
            if (this.icdoBenefitCalculationHeader.retirement_date == DateTime.MinValue || this.icdoBenefitCalculationHeader.retirement_date.Day != 1)
            {
                return false;
            }
            return true;
        }

        public bool CheckIfRetirementDateIsAfterTwoMonths()
        {
            DateTime ldtNormalRetiremetDate = this.ibusPerson.icdoPerson.date_of_birth.AddYears(65);
            if (ldtNormalRetiremetDate.Day != 1)
            {
                ldtNormalRetiremetDate = ldtNormalRetiremetDate.GetLastDayofMonth().AddDays(1);
            }

            DateTime ldtNextMonthRetirementDate = DateTime.Now.AddMonths(2);
            if (ldtNextMonthRetirementDate.Day != 1)
            {
                ldtNextMonthRetirementDate = ldtNextMonthRetirementDate.GetLastDayofMonth().AddDays(1);
            }

            if (this.icdoBenefitCalculationHeader.retirement_date < ldtNextMonthRetirementDate)
            {
                return false;
            }

            if (ldtNormalRetiremetDate > DateTime.Now)
            {
                return true;
            }

            return true;
        }

        public bool CheckIfParticipantIsVested()
        {
            if (this.ibusPerson.iclbPersonAccount.Where(pl => pl.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).Count() > 0)
            {
                if (!this.ibusBenefitApplication.CheckAlreadyVested(this.ibusPerson.iclbPersonAccount.Where(pl => pl.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).First().icdoPersonAccount.istrPlanCode))
                {
                    return false;
                }
            }
            return true;
        }

        public DateTime GetDefaultRetirementDateForTheParticipant()
        {
            DateTime ldtNormalRetiremetDate = this.ibusPerson.icdoPerson.date_of_birth.AddYears(65);
            if (ldtNormalRetiremetDate.Day != 1)
            {
                ldtNormalRetiremetDate = ldtNormalRetiremetDate.GetLastDayofMonth().AddDays(1);
            }

            DateTime ldtNextMonthRetirementDate = DateTime.Now.AddMonths(2);
            if (ldtNextMonthRetirementDate.Day != 1)
            {
                ldtNextMonthRetirementDate = ldtNextMonthRetirementDate.GetLastDayofMonth().AddDays(1);
            }
            if (ldtNormalRetiremetDate > DateTime.Now)
            {
                return ldtNormalRetiremetDate;
            }
            else
                return ldtNextMonthRetirementDate;
        }

        public bool CheckSpecialAccountBalance()
        {
            if (this.icdoBenefitCalculationHeader.iintPlanId == busConstant.MPIPP_PLAN_ID ||
                this.icdoBenefitCalculationHeader.iintPlanId == busConstant.IAP_PLAN_ID)
            {
                if (this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id ==
                     busConstant.IAP_PLAN_ID).Count() > 0)
                {
                    if (this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id ==
                      busConstant.IAP_PLAN_ID).FirstOrDefault().icdoBenefitCalculationDetail.local161_special_acct_bal_amount > decimal.Zero)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool CheckAlternatePayeeExists()
        {
            DataTable ldtplan = busBase.Select("cdoMssBenefitCalculationHeader.GetAlternatePayeeForPersonAndPlan", new object[2] { this.icdoBenefitCalculationHeader.person_id,this.icdoBenefitCalculationHeader.iintPlanId });
            if (ldtplan.IsNotNull() && ldtplan.Rows.Count > 0)
            {
                return true;
            }
            return false;
        }
        #endregion

        #region MPIPHP Benefit Options
        public decimal CalculateMSSFinalBenefitForPensionBenefitOptions(decimal adecFinalAccruedBenefitAmount, string astrBenefitOptionValue, int aintPlanId, bool ablnReEmployed = false)
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

            if (!ablnReEmployed)
            {

                // Calculate the total benefit amount of MPIPP & Locals
                // Need to call the Local Code to fetch the Local Frozen Benefit Amount with ERF
                ldecTotalLocalLumpsumAmount = ibusCalculation.GetLocalLumpsumBenefitAmount(icdoBenefitCalculationHeader.age, ibusBenefitApplication, ibusPerson,
                                           icdoBenefitCalculationHeader.retirement_date, iclbPersonAccountRetirementContribution);
                // Calculate the Monthly Exclusion Amount & Minimum Guarantee
                ibusCalculation.CalculateMEAAndMG(adecFinalAccruedBenefitAmount, ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.MPIPP).First(),
                                        ldecLumpSumBenefitAmount, this.icdoBenefitCalculationHeader.iintPlanId, Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge),
                                        this.icdoBenefitCalculationHeader.retirement_date, lintSurvivorAge,
                                        this.ibusBenefitApplication.QualifiedSpouseExists, busConstant.BOOL_FALSE, null, this.iclbBenefitCalculationDetail,
                                        this.iclbPersonAccountRetirementContribution, icdoBenefitCalculationHeader.calculation_type_value,
                                        this.icdoBenefitCalculationHeader.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, string.Empty);


                // Calculate the Final Benefit Amounts for all Benefit Options

                // Calculate the Lumpsum Benefit Option
            }

            switch (astrBenefitOptionValue)
            {
                case busConstant.CodeValueAll:

                    decimal ldecTotalPensionLocalBenefitAmount = ldecLumpSumBenefitAmount + ldecTotalLocalLumpsumAmount;

                    // Then check the total of MPI Accrued Benefit Amount and Local Benefit Amount
                    if (ldecTotalPensionLocalBenefitAmount < 10000)
                    {
                        // Participant is eligible only for Lumpsum
                        lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                        lbusBenefitCalculationOptions.LoadData(ibusCalculation.GetPlanBenefitId(aintPlanId, busConstant.LUMP_SUM), ldecBenefitOptionFactor,
                                                                ldecLumpSumBenefitAmount, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.LUMP_SUM, busConstant.ZERO_DECIMAL);

                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
                        break;
                    }

                    if (ldecTotalPensionLocalBenefitAmount > busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT)
                    {
                        if (this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth != DateTime.MinValue)
                        {
                            CalculateJointAndSurvivorBenefitOptions(adecFinalAccruedBenefitAmount);
                        }
                        // Annuity Benefit Option
                        int lintPlanBenefitId = ibusCalculation.GetPlanBenefitId(aintPlanId, busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY);
                        ldecBenefitOptionFactor = ibusCalculation.GetBenefitOptionFactor(this.icdoBenefitCalculationHeader.benefit_type_value, lintPlanBenefitId, lintParticipantAge, busConstant.ZERO_INT);
                        lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                        lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, ldecBenefitOptionFactor, Convert.ToDecimal(Math.Round(adecFinalAccruedBenefitAmount * ldecBenefitOptionFactor, 2)),
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

                        

                       
                    }
                    break;
                   
                default:
                    break;
            }

            return ldecFinalBenefitAmount;
        }

        public void CalculateMssUVHPBenefitOptions(string astrBenefitOptionValue, decimal ldecTotalBenefitAmount)
        {
            decimal ldecLifeyAnnuityFactor = 1;
            decimal ldecJAndS50Factor = new decimal();
            busBenefitCalculationOptions lbusBenefitCalculationOption;
            int lintParticipantAge = Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge);
            int lintSurvivorAge = Convert.ToInt32(Math.Floor(icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement));
            #region Get the Necessary Factors
            DataTable ldtMonthlyLifeAnnuity = Select("cdoBenefitProvisionUvhpLifeFactor.GetUVHPLifeFactor", new object[2] { Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge), this.ibusBenefitApplication.icdoBenefitApplication.retirement_date.Year });
            decimal ldecLifeAnnuityAmount = decimal.Zero;

            if (ldtMonthlyLifeAnnuity.Rows.Count > 0)
            {
                ldecLifeyAnnuityFactor = Convert.ToDecimal(ldtMonthlyLifeAnnuity.Rows[0][0]);
                ldecLifeyAnnuityFactor = Math.Round(ldecLifeyAnnuityFactor, 3);
                if (ldecLifeyAnnuityFactor > decimal.Zero)
                {
                    ldecLifeAnnuityAmount = Math.Round(ldecTotalBenefitAmount / ldecLifeyAnnuityFactor, 2);
                }
            }
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

                
            }
            #endregion
        }

        #endregion

        #region Local Benefit Options 

        public void CalculateMssLocal161BenefitOptions(string astrBenefitOption, decimal adecTotalBenefitAmount)
        {
            #region Variables Required in Switch Case
            decimal ldecFactor = new decimal();
            busBenefitCalculationOptions lbusBenefitCalculationOption;

            #endregion


            bool lblnCheckIfSpouse = busConstant.BOOL_FALSE;
            if(this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth != DateTime.MinValue)
            {
                lblnCheckIfSpouse = true;
            }

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

            }

            decimal ldecLumpSumFactor = ldecFactor = ibusCalculation.GetLumpsumBenefitFactorByAGE(Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge)) * 12;
            ldecLumpSumFactor = Math.Round(ldecLumpSumFactor, 3);
            decimal ldecPresentValue = Math.Round(adecTotalBenefitAmount * ldecLumpSumFactor, 3);
            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_161_PLAN_ID).First().icdoBenefitCalculationDetail.present_value_amount = ldecPresentValue;
            #endregion
        }

        public void CalculateMssLocal52BenefitOptions(string astrBenefitOption, decimal adecTotalBenefitAmount)
        {

            #region Variables Required in Switch Case
            decimal ldecFactor = new decimal();
            busBenefitCalculationOptions lbusBenefitCalculationOption;
            #endregion

            #region SwitchCase Based on BenefitOptions to be Calculated
            bool lblnCheckIfSpouse = busConstant.BOOL_FALSE;
            if (this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth != DateTime.MinValue)
            {
                lblnCheckIfSpouse = true;
            }
            switch (astrBenefitOption)
            {
                case busConstant.CodeValueAll:

                    #region Local 52 LUMP SUM
                    // //Ticket - 61531
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
                        decimal ldecSpecialYears = this.ibusBenefitApplication.aclbPersonWorkHistory_MPI.Where(item => item.year >= busConstant.BenefitCalculation.MERGER_DATE_LOCAL_52.Year && item.qualified_hours >= 400).Count() + this.ibusBenefitApplication.Local52_PensionCredits;

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
            }
            #endregion

            decimal ldecLumpSumFactor = ldecFactor = ibusCalculation.GetLumpsumBenefitFactorByAGE(Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge)) * 12;
            ldecLumpSumFactor = Math.Round(ldecLumpSumFactor, 3);
            decimal ldecPresentValue = Math.Round(adecTotalBenefitAmount * ldecLumpSumFactor, 3);
            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_52_PLAN_ID).First().icdoBenefitCalculationDetail.present_value_amount = ldecPresentValue;
        }

        public void CalculateMssLocal600BenefitOptions(string astrBenefitOption, decimal adecTotalBenefitAmount)
        {
            #region Variables Required in Switch Case
            decimal ldecFactor = new decimal();
            busBenefitCalculationOptions lbusBenefitCalculationOption;
            #endregion

            #region SwitchCase Based on BenefitOptions to be Calculated
            bool lblnCheckIfSpouse = busConstant.BOOL_FALSE;
            if (this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth != DateTime.MinValue)
            {
                lblnCheckIfSpouse = true;
            }
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
                                                                    busConstant.JOINT_50_PERCENT_POPUP_ANNUITY, Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor)) * 50 / 100));


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
                                                                    busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor)) * 75 / 100));

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
            }
            #endregion

            decimal ldecLumpSumFactor = ldecFactor = ibusCalculation.GetLumpsumBenefitFactorByAGE(Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge)) * 12;
            ldecLumpSumFactor = Math.Round(ldecLumpSumFactor, 3);
            decimal ldecPresentValue = Math.Round(adecTotalBenefitAmount * ldecLumpSumFactor, 3);
            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_600_PLAN_ID).First().icdoBenefitCalculationDetail.present_value_amount = ldecPresentValue;
        }

        public void CalculateMssLocal666BenefitOptions(string astrBenefitOption, decimal adecTotalBenefitAmount)
        {
            #region Variables Required in Switch Case
            decimal ldecFactor = new decimal();
            busBenefitCalculationOptions lbusBenefitCalculationOption;
            #endregion

            #region Switch Case Based on Benefit Options We have to Calculate
            bool lblnCheckIfSpouse = busConstant.BOOL_FALSE;
            if (this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth != DateTime.MinValue)
            {
                lblnCheckIfSpouse = true;
            }

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

            }
            #endregion
            decimal ldecLumpSumFactor = ldecFactor = ibusCalculation.GetLumpsumBenefitFactorByAGE(Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge)) * 12;
            ldecLumpSumFactor = Math.Round(ldecLumpSumFactor, 3);
            decimal ldecPresentValue = Math.Round(adecTotalBenefitAmount * ldecLumpSumFactor, 3);
            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_666_PLAN_ID).First().icdoBenefitCalculationDetail.present_value_amount = ldecPresentValue;

        }

        public void CalculateMssLocal700BenefitOptions(string astrBenefitOption, decimal adecTotalBenefitAmount)
        {
            #region Variables Required in Switch Case
            decimal ldecFactor = new decimal();
            busBenefitCalculationOptions lbusBenefitCalculationOption;
            #endregion

            #region Switch Case Based on Benefit Options We have to Calculate

            bool lblnCheckIfSpouse = busConstant.BOOL_FALSE;
            if (this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth != DateTime.MinValue)
            {
                lblnCheckIfSpouse = true;
            }
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
               
            }
            #endregion

            decimal ldecLumpSumFactor = ldecFactor = ibusCalculation.GetLumpsumBenefitFactorByAGE(Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge)) * 12;
            ldecLumpSumFactor = Math.Round(ldecLumpSumFactor, 3);
            decimal ldecPresentValue = Math.Round(adecTotalBenefitAmount * ldecLumpSumFactor, 3);
            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_700_PLAN_ID).First().icdoBenefitCalculationDetail.present_value_amount = ldecPresentValue;

        }

        #endregion Local

        #region IAP Benefit Options
        public void CalculateMssIAPBenefitAmount(string astrBenefitOptionValue, string astrAdjustmentFlag = "")
        {
            decimal ldecIAPBalance = busConstant.ZERO_DECIMAL;
            decimal ldecLocal52SpecialAccountBalance = busConstant.ZERO_DECIMAL;
            decimal ldecLocal161SpecialAccountBalance = busConstant.ZERO_DECIMAL;

            decimal ldecIAPHours4QtrAlloc = busConstant.ZERO_DECIMAL;
            decimal ldecIAPHoursA2forQtrAlloc = busConstant.ZERO_DECIMAL;
            decimal ldecIAPPercent4forQtrAlloc = busConstant.ZERO_DECIMAL;

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
                                if (astrAdjustmentFlag != busConstant.FLAG_YES)
                                {
                                    lblnCalculateAllocations = false;
                                }
                            }
                            lbusCalculation.GetIAPAndSpecialAccountBalance(busConstant.BOOL_FALSE, null, this.iclbBenefitCalculationDetail, this, null,
                                this.ibusBenefitApplication.icdoBenefitApplication.retirement_date, ldecIAPHours4QtrAlloc, ldecIAPHoursA2forQtrAlloc, ldecIAPPercent4forQtrAlloc, ablnExecuteIAPAllocation: lblnCalculateAllocations);
                        }
                    }
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
            if (ldecIAPBalance > busConstant.ZERO_DECIMAL || ldecLocal52SpecialAccountBalance > busConstant.ZERO_DECIMAL || ldecLocal161SpecialAccountBalance > busConstant.ZERO_DECIMAL)
            {
                int lintPlanBenefitId = busConstant.ZERO_INT;
                decimal ldecBenefitOptionFactor = 1;
                busBenefitCalculationOptions lbusBenefitCalculationOptions;

                bool lblnCheckIfSpouse = busConstant.BOOL_FALSE;
                if (this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth != DateTime.MinValue)
                {
                    lblnCheckIfSpouse = true;
                }
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

                                lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, ldecBenefitOptionFactor, Convert.ToDecimal(Math.Round(ldecIAPBalance / ldecBenefitOptionFactor, 2)), busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                        this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY,
                                                                        Convert.ToDecimal(Math.Round(ldecIAPBalance / ldecBenefitOptionFactor * 0.50m, 2)), busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE);

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

                                lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, ldecBenefitOptionFactor, Convert.ToDecimal(Math.Round(ldecLocal52SpecialAccountBalance / ldecBenefitOptionFactor, 2)), busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                       this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY,
                                                                       Convert.ToDecimal(Math.Round(ldecLocal52SpecialAccountBalance / ldecBenefitOptionFactor * 0.50m, 2)), busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_TRUE, busConstant.BOOL_FALSE);

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

                                lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, ldecBenefitOptionFactor, Convert.ToDecimal(Math.Round(ldecLocal161SpecialAccountBalance / ldecBenefitOptionFactor, 2)), busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                       this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY,
                                                                       Convert.ToDecimal(Math.Round(ldecLocal161SpecialAccountBalance / ldecBenefitOptionFactor * 0.50m, 2)), busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_TRUE);

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

                                lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, ldecBenefitOptionFactor, Convert.ToDecimal(Math.Round(ldecIAPBalance / ldecBenefitOptionFactor, 2)), busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                        this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY,
                                                                        Convert.ToDecimal(Math.Round(ldecIAPBalance / ldecBenefitOptionFactor * 0.75m, 2)), busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE);

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

                                lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, ldecBenefitOptionFactor, Convert.ToDecimal(Math.Round(ldecLocal52SpecialAccountBalance / ldecBenefitOptionFactor, 2)), busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                       this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY,
                                                                       Convert.ToDecimal(Math.Round(ldecLocal52SpecialAccountBalance / ldecBenefitOptionFactor * 0.75m, 2)), busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_TRUE, busConstant.BOOL_FALSE);

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

                                lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, ldecBenefitOptionFactor, Convert.ToDecimal(Math.Round(ldecLocal161SpecialAccountBalance / ldecBenefitOptionFactor, 2)), busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                       this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY,
                                                                       Convert.ToDecimal(Math.Round(ldecLocal161SpecialAccountBalance / ldecBenefitOptionFactor * 0.75m, 2)), busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_TRUE);

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

                                lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, ldecBenefitOptionFactor, Convert.ToDecimal(Math.Round(ldecIAPBalance / ldecBenefitOptionFactor, 2)), busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                        this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY,
                                                                        Convert.ToDecimal(Math.Round(ldecIAPBalance / ldecBenefitOptionFactor, 2)), busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE);


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


                        #region Ten Years Certain and Life Annuity
                        // This estimate if for MPI & IAP Plans. Hence calculating the Benefit for Ten Years Cerrtain Option as well.
                        ldecBenefitOptionFactor = 1;
                        lintPlanBenefitId = ibusCalculation.GetPlanBenefitId(busConstant.IAP_PLAN_ID, busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY);
                        ldecBenefitOptionFactor = ibusCalculation.GetBenefitOptionFactor(this.icdoBenefitCalculationHeader.benefit_type_value, lintPlanBenefitId, Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge), busConstant.ZERO_INT) * 12;

                        if (this.iblnCalculateIAPBenefit && ldecIAPBalance >= busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT)
                        {
                            lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                            lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, ldecBenefitOptionFactor, Convert.ToDecimal(Math.Round(ldecIAPBalance / ldecBenefitOptionFactor, 2)), busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                    this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY,
                                                                    Convert.ToDecimal(Math.Round(ldecIAPBalance / ldecBenefitOptionFactor, 2)), busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE);

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

                        #region Life Annunity
                        // Life Annuity
                        ldecBenefitOptionFactor = 1;
                        lintPlanBenefitId = ibusCalculation.GetPlanBenefitId(busConstant.IAP_PLAN_ID, busConstant.LIFE_ANNUTIY);
                        ldecBenefitOptionFactor = ibusCalculation.GetBenefitOptionFactor(this.icdoBenefitCalculationHeader.benefit_type_value, lintPlanBenefitId, Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge), busConstant.ZERO_INT) * 12;
                        if (this.iblnCalculateIAPBenefit && ldecIAPBalance >= busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT)
                        {
                            lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                            lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, ldecBenefitOptionFactor, Convert.ToDecimal(Math.Round(ldecIAPBalance / ldecBenefitOptionFactor, 2)), busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                    this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.LIFE_ANNUTIY,
                                                                    decimal.Zero, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE);

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

                            lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, ldecBenefitOptionFactor, Convert.ToDecimal(Math.Round(ldecLocal52SpecialAccountBalance / ldecBenefitOptionFactor, 2)), busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                    this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.LIFE_ANNUTIY,
                                                                    decimal.Zero, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_TRUE, busConstant.BOOL_FALSE);

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

                            lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, ldecBenefitOptionFactor, Convert.ToDecimal(Math.Round(ldecLocal161SpecialAccountBalance / ldecBenefitOptionFactor, 2)), busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                    this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.LIFE_ANNUTIY,
                                                                    decimal.Zero, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, busConstant.BOOL_TRUE);

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

                        if (this.iblnCalculateIAPBenefit && ldecIAPBalance > decimal.Zero)
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

                        if (this.iblnCalculateL52SplAccBenefit && ldecLocal52SpecialAccountBalance > decimal.Zero)
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

                        if (this.iblnCalculateL161SplAccBenefit && ldecLocal161SpecialAccountBalance > decimal.Zero)
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

        #endregion
    }
}
